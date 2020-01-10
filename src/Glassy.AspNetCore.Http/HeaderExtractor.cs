using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Atkins.Http.AspNetCore
{
    internal class HeaderExtractor : IRequestTokenExtractor
    {
        public HeaderExtractor(string key)
        {
            Key = key;
        }

        /// <summary>
        ///     Gets the part of the <see cref="HttpRequest" /> that the value is extracted from (e.g. "query string").
        /// </summary>
        public string ExtractsFrom { get; } = "headers";

        /// <summary>
        ///     Gets the key used to extract with.
        /// </summary>
        /// <value>
        ///     The key.
        /// </value>
        public string Key { get; }

        /// <summary>
        ///     Extracts a value from the <paramref name="request" /> or returns <see langword="null" /> if the
        ///     <paramref name="request" /> does not contain the value.
        /// </summary>
        /// <param name="request">  The request. </param>
        /// <returns>
        ///     The value or <see langword="null" /> if the <paramref name="request" /> does not contain the value.
        /// </returns>
        public string Extract(HttpRequest request)
        {
            if (!request.Headers.TryGetValue(Key, out StringValues value))
                return null;

            return value;
        }
    }
}