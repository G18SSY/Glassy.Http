using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Glassy.Http.AspNetCore
{
    /// <summary>
    ///     Interface for building token extractors and parameter registrations.
    /// </summary>
    /// <typeparam name="TParameter">   Type of the parameter. </typeparam>
    public interface ITokenExtractorBuilder<TParameter> : IRequestParameterBuilder<TParameter>
    {
        /// <summary>
        ///     Adds a validator that is called for this parameter immediately after it has been extracted and parsed.
        ///     Pre-validators are set
        ///     for each extractor and not automatically duplicated between them if multiple extractors are added. To add a
        ///     validator that is called regardless of the extractor the value came from consider using
        ///     <see cref="IRequestParameterBuilder{TParameter}.AddPostValidator" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <param name="validator">    The validator. </param>
        [NotNull]
        [PublicAPI]
        ITokenExtractorBuilder<TParameter> AddPreValidator([NotNull] Func<TParameter, IEnumerable<ValidationError>> validator);
    }
}