using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Glassy.Http.AspNetCore
{
    /// <summary>
    ///     Tries to convert the specified string representation of a logical value to its <typeparamref name="T" />
    ///     equivalent. A return value indicates whether the conversion succeeded   or failed.
    /// </summary>
    /// <typeparam name="T">    The type to convert to. </typeparam>
    /// <param name="value">     A string containing the value to convert. </param>
    /// <param name="result">   [out] The result if the conversion succeeded. </param>
    /// <returns>
    ///     true if value was converted successfully; otherwise, false.
    /// </returns>
    public delegate bool TryParseDelegate<T>(string value, out T result);

    /// <summary>
    ///     Base interface for building request parameter registrations.
    /// </summary>
    public interface IRequestRegistrationBuilder
    {
        /// <summary>
        ///     Gets the name of the parameter.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [NotNull]
        [PublicAPI]
        string Name { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the parameter is required.
        /// </summary>
        /// <value>
        ///     True if required, false if not.
        /// </value>
        [PublicAPI]
        bool Required { get; set; }

        /// <summary>
        ///     Gets the token extractors that can be used to extract the string value of a parameter from a
        ///     <see cref="Microsoft.AspNetCore.Http.HttpRequest" />.
        ///     <remarks>
        ///         Extractors are run in the list order, if any parser returns a value then subsequent ones are skipped. Ensure
        ///         that
        ///         the highest priority parsers are at the start of the list (index == 0).
        ///     </remarks>
        /// </summary>
        /// <value>
        ///     The token extractor. This will never be null.
        /// </value>
        [NotNull]
        [PublicAPI]
        List<IRequestTokenExtractor> TokenExtractors { get; }
    }

    /// <summary>
    ///     Interface for building request parameter registrations.
    /// </summary>
    /// <typeparam name="TParameter">   Type of the parameter. </typeparam>
    public interface IRequestRegistrationBuilder<TParameter> : IRequestRegistrationBuilder
    {
        /// <summary>
        ///     Gets or sets the default value to give the parameter if a value cannot be found on the
        ///     <see cref="Microsoft.AspNetCore.Http.HttpRequest" />.
        /// </summary>
        /// <value>
        ///     The default value.
        /// </value>
        [CanBeNull]
        [PublicAPI]
        TParameter DefaultValue { get; set; }

        /// <summary>
        ///     Gets or sets the parse method used to convert a string value into the parameter type.
        /// </summary>
        /// <value>
        ///     A function delegate that yields a bool indicating if the conversion was successful and return an (out) TParameter
        ///     which is the result of the conversion. This will never be null.
        /// </value>
        [NotNull]
        [PublicAPI]
        TryParseDelegate<TParameter> TryParser { get; set; }

        /// <summary>
        ///     Gets or sets the validator that is used to validate a parameter once it has been parsed. The validator should yield
        ///     <see langword="null"> or an empty <see cref="IEnumerable{ValidationError}" /> if there are no errors.</see>
        /// </summary>
        /// <value>
        ///     A function delegate that yields an all errors with the parameter. This may be null.
        /// </value>
        [CanBeNull]
        [PublicAPI]
        Func<TParameter, IEnumerable<ValidationError>> Validator { get; set; }

        /// <summary>
        ///     Gets or sets a callback that occurs once a <see cref="IRequestParser" /> finished parsing successfully.
        /// </summary>
        /// <value>
        ///     The on parsed callback. This may be null.
        /// </value>
        [CanBeNull]
        [PublicAPI]
        Action<TParameter> OnParsedCallback { get; set; }
    }
}