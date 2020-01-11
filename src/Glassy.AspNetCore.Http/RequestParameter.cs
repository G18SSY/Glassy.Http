using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Glassy.Http.AspNetCore
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    internal class RequestParameter
    {
        public RequestParameter([NotNull] string name, bool required, [NotNull] [ItemNotNull] IEnumerable<TokenExtractor> tokenExtractors, [CanBeNull] object defaultValue, [NotNull] TryParseDelegate<object> tryParser, [NotNull] [ItemNotNull] IEnumerable<Func<object, IEnumerable<ValidationError>>> postValidators, [NotNull] [ItemNotNull] IEnumerable<Action<object>> onParseCompletedCallbacks)
        {
            Name = name;
            Required = required;
            TokenExtractors = tokenExtractors;
            DefaultValue = defaultValue;
            TryParser = tryParser;
            PostValidators = postValidators;
            OnParseCompletedCallbacks = onParseCompletedCallbacks;
        }

        /// <summary>
        ///     Gets the name of the parameter.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [NotNull]
        public string Name { get; }

        /// <summary>
        ///     Gets a value indicating whether the parameter is required.
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
        [ItemNotNull]
        public IEnumerable<TokenExtractor> TokenExtractors { get; }

        /// <summary>
        ///     Gets or sets the default value to give the parameter if a value cannot be found on the
        ///     <see cref="Microsoft.AspNetCore.Http.HttpRequest" />.
        /// </summary>
        /// <value>
        ///     The default value.
        /// </value>
        [CanBeNull]
        public object DefaultValue { get; }

        /// <summary>
        ///     Gets the parse method used to convert a string value into the parameter type.
        /// </summary>
        /// <value>
        ///     A function delegate that yields a bool indicating if the conversion was successful and return an (out) object
        ///     which is the result of the conversion. This will never be null.
        /// </value>
        [NotNull]
        public TryParseDelegate<object> TryParser { get; }

        /// <summary>
        ///     Gets the validators that is used to validate a parameter once all parameters have been parsed. The validator should
        ///     yield
        ///     <see langword="null"> or an empty <see cref="IEnumerable{ValidationError}" /> if there are no errors.</see>
        /// </summary>
        /// <value>
        ///     A function delegate that yields an all errors with the parameter.
        /// </value>
        [NotNull]
        [ItemNotNull]
        public IEnumerable<Func<object, IEnumerable<ValidationError>>> PostValidators { get; }

        /// <summary>
        ///     Gets a callback that will be invoked when all parameters have been parsed and validated. This can be used to set a
        ///     variable to the value extracted by the <see cref="IRequestParser" />.
        /// </summary>
        /// <value>
        ///     The on parsed callback. This may be null.
        /// </value>
        [NotNull]
        [ItemNotNull]
        public IEnumerable<Action<object>> OnParseCompletedCallbacks { get; }
    }
}