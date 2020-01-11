using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Glassy.Http.AspNetCore
{
    internal class TokenExtractorBuilder
    {
        public TokenExtractorBuilder([NotNull] string extractsFrom, [NotNull] string key, [NotNull] Func<HttpRequest, string> extractCallback)
        {
            ExtractsFrom = extractsFrom ?? throw new ArgumentNullException(nameof(extractsFrom));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            ExtractCallback = extractCallback ?? throw new ArgumentNullException(nameof(extractCallback));
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
        ///     Gets a callback that extracts a value from the request or returns <see langword="null" /> if the
        ///     request does not contain the value.
        /// </summary>
        [NotNull]
        public Func<HttpRequest, string> ExtractCallback { get; }

        /// <summary>
        ///     Gets the pre validators.
        /// </summary>
        /// <value>
        ///     The pre validators.
        /// </value>
        [NotNull]
        [ItemNotNull]
        internal List<Func<object, IEnumerable<ValidationError>>> PreValidators { get; } = new List<Func<object, IEnumerable<ValidationError>>>();
    }

    internal class TokenExtractorBuilder<TParameter> : TokenExtractorBuilder, ITokenExtractorBuilder<TParameter>
    {
        private readonly IRequestParameterBuilder<TParameter> parameterBuilder;

        public TokenExtractorBuilder([NotNull] IRequestParameterBuilder<TParameter> parameterBuilder, [NotNull] string extractsFrom, [NotNull] string key, [NotNull] Func<HttpRequest, string> extractCallback) : base(extractsFrom, key, extractCallback)
        {
            this.parameterBuilder = parameterBuilder;
        }

        /// <summary>
        ///     Sets whether the parameter is required.
        /// </summary>
        /// <param name="required"> (Optional) True if required. </param>
        public IRequestParameterBuilder<TParameter> Required(bool required = true) => parameterBuilder.Required(required);

        /// <summary>
        ///     Adds an extractor that will be used by the <see cref="IRequestParser" /> to extract data from a
        ///     <see cref="HttpRequest" /> header.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="key">      The header key. </param>
        public ITokenExtractorBuilder<TParameter> FromHeader(string key) => parameterBuilder.FromHeader(key);

        /// <summary>
        ///     Adds an extractor that will be used by the <see cref="IRequestParser" /> to extract data from the
        ///     <see cref="HttpRequest" /> route.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="key">      The route key. </param>
        /// <param name="value">The route value</param>
        public ITokenExtractorBuilder<TParameter> FromRoute(string key, string value) => parameterBuilder.FromRoute(key, value);

        /// <summary>
        ///     Adds an extractor that will be used by the <see cref="IRequestParser" /> to extract data from a
        ///     <see cref="HttpRequest" /> query string.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="key">      The query parameter key. </param>
        public ITokenExtractorBuilder<TParameter> FromQuery(string key) => parameterBuilder.FromQuery(key);

        /// <summary>
        ///     Sets the method that the <see cref="IRequestParser" /> will use to convert extracted data to
        ///     <typeparamref name="TParameter" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="tryParser">    The try parser delegate. </param>
        public IRequestParameterBuilder<TParameter> TryParser(TryParseDelegate<TParameter> tryParser) => parameterBuilder.TryParser(tryParser);

        /// <summary>
        ///     Adds a validator that is called for this parameter once all parameters have been successfully extracted/parsed. To
        ///     validate a parameter value immediately after if has been parsed use
        ///     <see cref="ITokenExtractorBuilder{TParameter}.AddPreValidator" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="validator">    The validator. </param>
        public IRequestParameterBuilder<TParameter> AddPostValidator(Func<TParameter, IEnumerable<ValidationError>> validator) => parameterBuilder.AddPostValidator(validator);

        /// <summary>
        ///     Sets the default value that an optional parameter will take if the value is not successfully extracted, parsed and
        ///     validated.
        /// </summary>
        /// <param name="defaultValue"> The default value. </param>
        public IRequestParameterBuilder<TParameter> DefaultValue(TParameter defaultValue) => parameterBuilder.DefaultValue(defaultValue);

        /// <summary>
        ///     Adds a callback that will be invoked when all parameters have been parsed and validated. This can be used to set a
        ///     variable to the value extracted by the <see cref="IRequestParser" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="onParsed"> The on parsed callback. </param>
        public IRequestParameterBuilder<TParameter> AddOnParseCompletedCallback(Action<TParameter> onParsed) => parameterBuilder.AddOnParseCompletedCallback(onParsed);

        /// <summary>
        ///     Adds a validator that is called for this parameter immediately after it has been extracted and parsed.
        ///     Pre-validators are set
        ///     for each extractor and not automatically duplicated between them if multiple extractors are added. To add a
        ///     validator that is called regardless of the extractor the value came from consider using
        ///     <see cref="IRequestParameterBuilder{TParameter}.AddPostValidator" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="validator">    The validator. </param>
        public ITokenExtractorBuilder<TParameter> AddPreValidator(Func<TParameter, IEnumerable<ValidationError>> validator)
        {
            PreValidators.Add(o => validator((TParameter)o));

            return this;
        }
    }
}