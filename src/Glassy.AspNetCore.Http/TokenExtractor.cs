using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Glassy.Http.AspNetCore
{
    [DebuggerDisplay("{" + nameof(ExtractsFrom) + "}: {" + nameof(Key) + "}")]
    internal class TokenExtractor
    {
        private readonly Func<HttpRequest, string> extractCallback;

        public TokenExtractor([NotNull] string extractsFrom, [NotNull] string key, [NotNull] [ItemNotNull] IEnumerable<Func<object, IEnumerable<ValidationError>>> preValidators, [NotNull] Func<HttpRequest, string> extractCallback)
        {
            this.extractCallback = extractCallback;
            ExtractsFrom = extractsFrom;
            Key = key;
            PreValidators = preValidators;
        }

        /// <summary>
        ///     Gets the part of the <see cref="HttpRequest" /> that the value is extracted from (e.g. "query string").
        /// </summary>
        [NotNull]
        public string ExtractsFrom { get; }

        /// <summary>
        ///     Gets the key used to extract with.
        /// </summary>
        /// <value>
        ///     The key.
        /// </value>
        [NotNull]
        public string Key { get; }

        /// <summary>
        ///     Gets the pre validators.
        /// </summary>
        /// <value>
        ///     The pre validators.
        /// </value>
        [NotNull]
        [ItemNotNull]
        public IEnumerable<Func<object, IEnumerable<ValidationError>>> PreValidators { get; }

        /// <summary>
        ///     Extracts a value from the <paramref name="request" /> or returns <see langword="null" /> if the
        ///     <paramref name="request" /> does not contain the value.
        /// </summary>
        /// <param name="request">  The request. </param>
        /// <returns>
        ///     The value or <see langword="null" /> if the <paramref name="request" /> does not contain the value.
        /// </returns>
        [CanBeNull]
        public string Extract([NotNull] HttpRequest request) => extractCallback(request);
    }
}