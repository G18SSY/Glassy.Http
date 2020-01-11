using JetBrains.Annotations;

namespace Glassy.Http.AspNetCore
{
    /// <summary>
    ///     Settings for the <see cref="IRequestParser" />.
    /// </summary>
    public class RequestParserSettings
    {
        /// <summary>
        ///     The default settings.
        /// </summary>
        public static readonly RequestParserSettings Default = new RequestParserSettings();

        /// <summary>
        ///     Gets or sets a value indicating whether to skip failed token parses.
        ///     <remarks>
        ///         Default is true. If set to true then if the parser finds a value but the value cannot be parsed into a
        ///         parameter it will be skipped. If false then when a badly formed value is found no more tokens for that
        ///         parameter will be analysed and the parser will return a failure.
        ///     </remarks>
        /// </summary>
        /// <value>
        ///     True if skip failed token parses, false if not.
        /// </value>
        [PublicAPI]
        public bool SkipFailedTokenParses { get; set; } = true;

        /// <summary>
        ///     Gets or sets a value indicating whether to skip failed pre-validations.
        ///     <remarks>
        ///         Default is true. If set to true then if the parser extracts a value but the value is invalid it will be
        ///         skipped. If false then when an invalid value is extracted no more tokens for that
        ///         parameter will be analysed and the parser will return a failure.
        ///     </remarks>
        /// </summary>
        /// <value>
        ///     True if skip failed token parses, false if not.
        /// </value>
        [PublicAPI]
        public bool SkipFailedPreValidations { get; set; } = true;
    }
}