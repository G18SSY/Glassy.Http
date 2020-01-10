using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Glassy.Http.AspNetCore
{
    internal class ParseResult : IParseResult
    {
        private readonly IDictionary<string, object> values;

        public ParseResult([NotNull] IDictionary<string, object> values)
        {
            this.values = values ?? throw new ArgumentNullException(nameof(values));
            Success = true;
        }

        public ParseResult([NotNull] string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(errorMessage));
            ErrorMessage = errorMessage;
            Success = false;
        }

        /// <summary>
        ///     Gets a value indicating whether the parse was successful.
        /// </summary>
        /// <value>
        ///     True if successful, false if not.
        /// </value>
        public bool Success { get; }

        /// <summary>
        ///     Indexer to get parameter values using array index syntax.
        /// </summary>
        /// <param name="parameter">    The parameter name. </param>
        /// <returns>
        ///     The parameter value.
        /// </returns>
        public object this[string parameter]
        {
            get
            {
                if (values == null)
                    throw new InvalidOperationException("Cannot retrieve a value for a failed parse");

                return values[parameter];
            }
        }

        /// <summary>
        ///     Gets a message describing the error. This is only set if <see cref="IParseResult.Success" /> is false.
        /// </summary>
        /// <value>
        ///     A message describing the error. This may be null.
        /// </value>
        public string ErrorMessage { get; }
    }
}