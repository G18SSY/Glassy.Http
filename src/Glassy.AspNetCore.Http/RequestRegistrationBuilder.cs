using System;
using System.Collections.Generic;
using System.Reflection;

namespace Glassy.Http.AspNetCore
{
    internal class RequestRegistrationBuilder : IRequestRegistrationBuilder
    {
        protected RequestRegistrationBuilder(string name, Type parameterType)
        {
            Name = name;
            ParameterType = parameterType;
        }

        internal object DefaultValue { get; private protected set; }

        internal TryParseDelegate<object> TryParser { get; private protected set; }

        internal Func<object, IEnumerable<ValidationError>> Validator { get; private protected set; }

        internal Action<object> OnParsedCallback { get; private protected set; }

        internal Type ParameterType { get; }

        public string Name { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the parameter is required.
        /// </summary>
        /// <value>
        ///     True if required, false if not.
        /// </value>
        public bool Required { get; set; }

        /// <summary>
        ///     Gets the token extractors that can be used to extract the string value of a parameter from a
        ///     <see cref="Microsoft.AspNetCore.Http.HttpRequest" />.
        ///     <remarks>
        ///         Extractors are run in the list order, if any parser returns a value then subsequent ones are skipped. Ensure
        ///         that
        ///         the highest priority parsers are at the start of the list (index == 0).
        ///     </remarks>
        /// </summary>
        /// <value>
        ///     The token extractor. This will never be null.
        /// </value>
        public List<IRequestTokenExtractor> TokenExtractors { get; } = new List<IRequestTokenExtractor>();
    }

    internal class RequestRegistrationBuilder<TParameter> : RequestRegistrationBuilder, IRequestRegistrationBuilder<TParameter>
    {
        private TParameter defaultValue;
        private TryParseDelegate<TParameter> tryParser;
        private Func<TParameter, IEnumerable<ValidationError>> validator;
        private Action<TParameter> onParsedCallback;

        public RequestRegistrationBuilder(string name) : base(name, typeof(TParameter))
        {
            ((IRequestRegistrationBuilder<TParameter>)this).DefaultValue = default;
            ((IRequestRegistrationBuilder<TParameter>)this).TryParser = GenericTryParse;
        }

        /// <summary>
        ///     Gets or sets the default value to give the parameter if a value cannot be found on the
        ///     <see cref="Microsoft.AspNetCore.Http.HttpRequest" />.
        /// </summary>
        /// <value>
        ///     The default value. This will never be null.
        /// </value>
        TParameter IRequestRegistrationBuilder<TParameter>.DefaultValue
        {
            get => defaultValue;
            set
            {
                defaultValue = value;
                DefaultValue = value;
            }
        }

        /// <summary>
        ///     Gets or sets the parse method used to convert a string value into the parameter type.
        /// </summary>
        /// <value>
        ///     A function delegate that yields a bool indicating if the conversion was successful and return an (out) TParameter
        ///     which is the result of the conversion. This will never be null.
        /// </value>
        TryParseDelegate<TParameter> IRequestRegistrationBuilder<TParameter>.TryParser
        {
            get => tryParser;
            set
            {
                tryParser = value;
                TryParseObjectConverter<TParameter> converter = new TryParseObjectConverter<TParameter>(value);
                TryParser = converter.TryParse;
            }
        }

        /// <summary>
        ///     Gets or sets the validator that is used to validate a parameter once it has been parsed. The validator should yield
        ///     <see langword="null"> or an empty <see cref="IEnumerable{ValidationError}" /> if there are no errors.</see>
        /// </summary>
        /// <value>
        ///     A function delegate that yields an all errors with the parameter. This may be null.
        /// </value>
        Func<TParameter, IEnumerable<ValidationError>> IRequestRegistrationBuilder<TParameter>.Validator
        {
            get => validator;
            set
            {
                validator = value;
                if (value == null)
                    Validator = null;
                else
                    Validator = o => value((TParameter)o);
            }
        }

        /// <summary>
        ///     Gets or sets a callback that occurs once a <see cref="IRequestParser" /> finished parsing successfully.
        /// </summary>
        /// <value>
        ///     The on parsed callback. This may be null.
        /// </value>
        Action<TParameter> IRequestRegistrationBuilder<TParameter>.OnParsedCallback
        {
            get => onParsedCallback;
            set
            {
                onParsedCallback = value;
                if (value == null)
                    OnParsedCallback = null;
                else
                    OnParsedCallback = o => value((TParameter)o);
            }
        }

        private static bool GenericTryParse(string value, out TParameter result)
        {
            if (value is TParameter stringParameter)
            {
                result = stringParameter;

                return true;
            }

            MethodInfo tryParseMethod = typeof(TParameter).GetMethod("TryParse", new[] {typeof(string), typeof(TParameter).MakeByRefType()});

            if (tryParseMethod == null)
                throw new FormatException($"Cannot parse {typeof(TParameter).FullName} because it does not have a default TryParse method");

            object[] args = {value, null};
            bool success = (bool)tryParseMethod.Invoke(null, args);

            result = (TParameter)args[1];

            return success;
        }

        private class TryParseObjectConverter<T>
        {
            private readonly TryParseDelegate<T> typedDelegate;

            public TryParseObjectConverter(TryParseDelegate<T> typedDelegate)
            {
                this.typedDelegate = typedDelegate;
            }

            public bool TryParse(string input, out object result)
            {
                bool boolResult = typedDelegate(input, out T typedResult);
                result = typedResult;

                return boolResult;
            }
        }
    }
}