using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Glassy.Http.AspNetCore
{
    internal class RequestRegistration
    {
        public RequestRegistration([NotNull] string name, bool required, [NotNull] IReadOnlyList<IRequestTokenExtractor> tokenExtractors, [NotNull] object defaultValue, [NotNull] TryParseDelegate<object> tryParser, [CanBeNull] Func<object, IEnumerable<ValidationError>> validator, [CanBeNull] Action<object> onParsedCallback, Type parameterType)
        {
            Name = name;
            Required = required;
            TokenExtractors = tokenExtractors;
            DefaultValue = defaultValue;
            TryParser = tryParser;
            Validator = validator;
            OnParsedCallback = onParsedCallback;
            ParameterType = parameterType;
        }

        public Type ParameterType { get; }

        /// <summary>
        ///     Gets the name of the parameter.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [NotNull]
        public string Name { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the parameter is required.
        /// </summary>
        /// <value>
        ///     True if required, false if not.
        /// </value>
        public bool Required { get; }

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
        [NotNull]
        public IReadOnlyList<IRequestTokenExtractor> TokenExtractors { get; }

        /// <summary>
        ///     Gets or sets the default value to give the parameter if a value cannot be found on the
        ///     <see cref="Microsoft.AspNetCore.Http.HttpRequest" />.
        /// </summary>
        /// <value>
        ///     The default value. This will never be null.
        /// </value>
        [NotNull]
        public object DefaultValue { get; }

        /// <summary>
        ///     Gets or sets the parse method used to convert a string value into the parameter type.
        /// </summary>
        /// <value>
        ///     A function delegate that yields a bool indicating if the conversion was successful and return an (out) object
        ///     which is the result of the conversion. This will never be null.
        /// </value>
        [NotNull]
        public TryParseDelegate<object> TryParser { get; }

        /// <summary>
        ///     Gets or sets the validator that is used to validate a parameter once it has been parsed. The validator should yield
        ///     <see langword="null"> or an empty <see cref="IEnumerable{ValidationError}" /> if there are no errors.</see>
        /// </summary>
        /// <value>
        ///     A function delegate that yields an all errors with the parameter. This may be null.
        /// </value>
        [CanBeNull]
        public Func<object, IEnumerable<ValidationError>> Validator { get; }

        /// <summary>
        ///     Gets or sets a callback that occurs once a <see cref="IRequestParser" /> finished parsing successfully.
        /// </summary>
        /// <value>
        ///     The on parsed callback. This may be null.
        /// </value>
        [CanBeNull]
        public Action<object> OnParsedCallback { get; }
    }
}