using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Primitives;

namespace Glassy.Http.AspNetCore
{
    internal class RequestParameterBuilder
    {
        protected RequestParameterBuilder(string name)
        {
            Name = name;
        }

        internal object DefaultValue { get; private protected set; }

        internal TryParseDelegate<object> TryParser { get; private protected set; }

        internal List<Func<object, IEnumerable<ValidationError>>> PostValidators { get; } = new List<Func<object, IEnumerable<ValidationError>>>();

        internal List<Action<object>> OnParseCompletedCallbacks { get; } = new List<Action<object>>();

        public string Name { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the parameter is required.
        /// </summary>
        /// <value>
        ///     True if required, false if not.
        /// </value>
        public bool Required { get; private protected set; }

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
        public List<TokenExtractorBuilder> TokenExtractorBuilders { get; } = new List<TokenExtractorBuilder>();
    }

    internal class RequestParameterBuilder<TParameter> : RequestParameterBuilder, IRequestParameterBuilder<TParameter>
    {
        public RequestParameterBuilder(string name) : base(name)
        {
            DefaultValue = default(TParameter);
            TryParser = ReflectionBasedTryParse;
        }

        private static bool ReflectionBasedTryParse(string value, out object result)
        {
            if (value is TParameter stringParameter)
            {
                result = stringParameter;

                return true;
            }

            MethodInfo tryParseMethod = typeof(TParameter).GetMethod("TryParse", new[] {typeof(string), typeof(TParameter).MakeByRefType()});

            if (tryParseMethod == null)
                throw new InvalidOperationException($"Cannot parse {typeof(TParameter).FullName} because it does not have a default TryParse method");

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

        #region NewMembers
        /// <summary>
        ///     Sets whether the parameter is required.
        /// </summary>
        /// <param name="required"> (Optional) True if required. </param>
        IRequestParameterBuilder<TParameter> IRequestParameterBuilder<TParameter>.Required(bool required)
        {
            Required = required;

            return this;
        }

        /// <summary>
        ///     Adds an extractor that will be used by the <see cref="IRequestParser" /> to extract data from a
        ///     <see cref="Microsoft.AspNetCore.Http.HttpRequest" /> header.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="key">      The header key. </param>
        ITokenExtractorBuilder<TParameter> IRequestParameterBuilder<TParameter>.FromHeader(string key)
        {
            const string extractsFrom = "headers";

            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));

            TokenExtractorBuilder<TParameter> builder = new TokenExtractorBuilder<TParameter>(this,
                                                                                              extractsFrom,
                                                                                              key,
                                                                                              r =>
                                                                                              {
                                                                                                  if (!r.Headers.TryGetValue(key, out StringValues outValue))
                                                                                                      return null;

                                                                                                  return outValue;
                                                                                              });

            TokenExtractorBuilders.Add(builder);

            return builder;
        }

        /// <summary>
        ///     Adds an extractor that will be used by the <see cref="IRequestParser" /> to extract data from the
        ///     <see cref="Microsoft.AspNetCore.Http.HttpRequest" /> route.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="key">      The route key. </param>
        /// <param name="value">The route value</param>
        ITokenExtractorBuilder<TParameter> IRequestParameterBuilder<TParameter>.FromRoute(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));

            const string extractsFrom = "route";
            TokenExtractorBuilder<TParameter> builder = new TokenExtractorBuilder<TParameter>(this,
                                                                                              extractsFrom,
                                                                                              key,
                                                                                              r => value);

            TokenExtractorBuilders.Add(builder);

            return builder;
        }

        /// <summary>
        ///     Adds an extractor that will be used by the <see cref="IRequestParser" /> to extract data from a
        ///     <see cref="Microsoft.AspNetCore.Http.HttpRequest" /> query string.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="key">      The query parameter key. </param>
        ITokenExtractorBuilder<TParameter> IRequestParameterBuilder<TParameter>.FromQuery(string key)
        {
            const string extractsFrom = "query string";

            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));

            TokenExtractorBuilder<TParameter> builder = new TokenExtractorBuilder<TParameter>(this,
                                                                                              extractsFrom,
                                                                                              key,
                                                                                              r =>
                                                                                              {
                                                                                                  if (!r.Query.TryGetValue(key, out StringValues outValue))
                                                                                                      return null;

                                                                                                  return outValue;
                                                                                              });

            TokenExtractorBuilders.Add(builder);

            return builder;
        }

        /// <summary>
        ///     Sets the method that the <see cref="IRequestParser" /> will use to convert extracted data to
        ///     <typeparamref name="TParameter" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="tryParser">    The try parser delegate. </param>
        IRequestParameterBuilder<TParameter> IRequestParameterBuilder<TParameter>.TryParser(TryParseDelegate<TParameter> tryParser)
        {
            TryParseObjectConverter<TParameter> converter = new TryParseObjectConverter<TParameter>(tryParser);
            TryParser = converter.TryParse;

            return this;
        }

        /// <summary>
        ///     Adds a validator that is called for this parameter once all parameters have been successfully extracted/parsed. To
        ///     validate a parameter value immediately after if has been parsed use
        ///     <see cref="ITokenExtractorBuilder{TParameter}.AddPreValidator" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="validator">    The validator. </param>
        IRequestParameterBuilder<TParameter> IRequestParameterBuilder<TParameter>.AddPostValidator(Func<TParameter, IEnumerable<ValidationError>> validator)
        {
            PostValidators.Add(o => validator((TParameter)o));

            return this;
        }

        /// <summary>
        ///     Sets the default value that an optional parameter will take if the value is not successfully extracted, parsed and
        ///     validated.
        /// </summary>
        /// <param name="defaultValue"> The default value. </param>
        IRequestParameterBuilder<TParameter> IRequestParameterBuilder<TParameter>.DefaultValue(TParameter defaultValue)
        {
            DefaultValue = defaultValue;

            return this;
        }

        /// <summary>
        ///     Adds a callback that will be invoked when all parameters have been parsed and validated. This can be used to set a
        ///     variable to the value extracted by the <see cref="IRequestParser" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="onParsed"> The on parsed callback. </param>
        IRequestParameterBuilder<TParameter> IRequestParameterBuilder<TParameter>.AddOnParseCompletedCallback(Action<TParameter> onParsed)
        {
            OnParseCompletedCallbacks.Add(o =>
            {
                TParameter parameter = (TParameter)o;

                onParsed(parameter);
            });

            return this;
        }
        #endregion
    }
}