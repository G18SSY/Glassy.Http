using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Atkins.Http.AspNetCore
{
    /// <summary>
    ///     Interface for an object that can be used to extract a specific value from a <see cref="HttpRequest" />.
    /// </summary>
    public interface IRequestTokenExtractor
    {
        /// <summary>
        ///     Gets the part of the <see cref="HttpRequest" /> that the value is extracted from (e.g. "query string").
        /// </summary>
        [NotNull]
        string ExtractsFrom { get; }

        /// <summary>
        ///     Gets the key used to extract with.
        /// </summary>
        /// <value>
        ///     The key.
        /// </value>
        [NotNull]
        string Key { get; }

        /// <summary>
        ///     Extracts a value from the <paramref name="request" /> or returns <see langword="null" /> if the
        ///     <paramref name="request" /> does not contain the value.
        /// </summary>
        /// <param name="request">  The request. </param>
        /// <returns>
        ///     The value or <see langword="null" /> if the <paramref name="request" /> does not contain the value.
        /// </returns>
        [PublicAPI]
        string Extract([NotNull] HttpRequest request);
    }
}