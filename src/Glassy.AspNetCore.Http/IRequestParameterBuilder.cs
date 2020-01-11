using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Glassy.Http.AspNetCore
{
    /// <summary>
    ///     Interface for building request parameter registrations.
    /// </summary>
    /// <typeparam name="TParameter">   Type of the parameter. </typeparam>
    public interface IRequestParameterBuilder<TParameter> : IHideObjectMembers
    {
        /// <summary>
        ///     Sets whether the parameter is required.
        /// </summary>
        /// <param name="required"> (Optional) True if required. </param>
        [NotNull]
        [PublicAPI]
        IRequestParameterBuilder<TParameter> Required(bool required = true);

        /// <summary>
        ///     Adds an extractor that will be used by the <see cref="IRequestParser" /> to extract data from a
        ///     <see cref="Microsoft.AspNetCore.Http.HttpRequest" /> header.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="key">      The header key. </param>
        [NotNull]
        [PublicAPI]
        ITokenExtractorBuilder<TParameter> FromHeader([NotNull] string key);

        /// <summary>
        ///     Adds an extractor that will be used by the <see cref="IRequestParser" /> to extract data from the
        ///     <see cref="Microsoft.AspNetCore.Http.HttpRequest" /> route.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="key">      The route key. </param>
        /// <param name="value">The route value</param>
        [NotNull]
        [PublicAPI]
        ITokenExtractorBuilder<TParameter> FromRoute([NotNull] string key, [CanBeNull] string value);

        /// <summary>
        ///     Adds an extractor that will be used by the <see cref="IRequestParser" /> to extract data from a
        ///     <see cref="Microsoft.AspNetCore.Http.HttpRequest" /> query string.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="key">      The query parameter key. </param>
        [NotNull]
        [PublicAPI]
        ITokenExtractorBuilder<TParameter> FromQuery([NotNull] string key);

        /// <summary>
        ///     Sets the method that the <see cref="IRequestParser" /> will use to convert extracted data to
        ///     <typeparamref name="TParameter" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="tryParser">    The try parser delegate. </param>
        [NotNull]
        [PublicAPI]
        IRequestParameterBuilder<TParameter> TryParser([NotNull] TryParseDelegate<TParameter> tryParser);

        /// <summary>
        ///     Adds a validator that is called for this parameter once all parameters have been successfully extracted/parsed. To
        ///     validate a parameter value immediately after if has been parsed use
        ///     <see cref="ITokenExtractorBuilder{TParameter}.AddPreValidator" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="validator">    The validator. </param>
        [NotNull]
        [PublicAPI]
        IRequestParameterBuilder<TParameter> AddPostValidator([NotNull] Func<TParameter, IEnumerable<ValidationError>> validator);

        /// <summary>
        ///     Sets the default value that an optional parameter will take if the value is not successfully extracted, parsed and
        ///     validated.
        /// </summary>
        /// <param name="defaultValue"> The default value. </param>
        [NotNull]
        [PublicAPI]
        IRequestParameterBuilder<TParameter> DefaultValue([CanBeNull] TParameter defaultValue);

        /// <summary>
        ///     Adds a callback that will be invoked when all parameters have been parsed and validated. This can be used to set a
        ///     variable to the value extracted by the <see cref="IRequestParser" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="onParsed"> The on parsed callback. </param>
        [NotNull]
        [PublicAPI]
        IRequestParameterBuilder<TParameter> AddOnParseCompletedCallback([NotNull] Action<TParameter> onParsed);
    }
}