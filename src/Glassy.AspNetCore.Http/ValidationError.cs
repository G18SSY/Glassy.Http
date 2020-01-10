using System;
using JetBrains.Annotations;

namespace Glassy.Http.AspNetCore
{
    /// <summary>
    ///     A validation error.
    /// </summary>
    [PublicAPI]
    public class ValidationError
    {
        /// <summary>
        ///     Instantiates a new <see cref="ValidationError" />.
        /// </summary>
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or illegal values. </exception>
        /// <param name="message">  The message. This cannot be null or whitespace. </param>
        public ValidationError([NotNull] string message)
        {
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));
            Message = message;
        }

        /// <summary>
        ///     Gets the message.
        /// </summary>
        /// <value>
        ///     The message.
        /// </value>
        public string Message { get; }
    }
}