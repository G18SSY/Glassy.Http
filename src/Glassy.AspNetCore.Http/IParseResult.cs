using JetBrains.Annotations;

namespace Glassy.Http.AspNetCore
{
    /// <summary>
    ///     Interface for the results of a parsing operation.
    /// </summary>
    public interface IParseResult
    {
        /// <summary>
        ///     Gets a value indicating whether the parse was successful.
        /// </summary>
        /// <value>
        ///     True if successful, false if not.
        /// </value>
        [PublicAPI]
        bool Success { get; }

        /// <summary>
        ///     Indexer to get parameter values using array index syntax.
        /// </summary>
        /// <param name="parameter">    The parameter name. </param>
        /// <returns>
        ///     The parameter value.
        /// </returns>
        [CanBeNull]
        [PublicAPI]
        object this[[NotNull] string parameter] { get; }

        /// <summary>
        ///     Gets a message describing the error. This is only set if <see cref="Success" /> is false.
        /// </summary>
        /// <value>
        ///     A message describing the error. This may be null.
        /// </value>
        [CanBeNull]
        [PublicAPI]
        string ErrorMessage { get; }
    }
}