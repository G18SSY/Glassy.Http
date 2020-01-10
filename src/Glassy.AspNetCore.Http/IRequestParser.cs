using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Atkins.Http.AspNetCore
{
    /// <summary>
    ///     Interface for an object that can be used to parse a <see cref="HttpRequest" />.
    /// </summary>
    public interface IRequestParser
    {
        /// <summary>
        ///     Parses the given request using default settings.
        /// </summary>
        /// <param name="request">  The request. </param>
        /// <returns>
        ///     An IParseResult.
        /// </returns>
        [NotNull]
        [PublicAPI]
        IParseResult Parse(HttpRequest request);

        /// <summary>
        ///     Parses the given request using specific settings.
        /// </summary>
        /// <param name="request">  The request. </param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        ///     An IParseResult.
        /// </returns>
        [NotNull]
        [PublicAPI]
        IParseResult Parse(HttpRequest request, RequestParserSettings settings);
    }
}