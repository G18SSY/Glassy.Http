using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Glassy.Http.AspNetCore
{
    /// <summary>
    ///     Registration extensions.
    /// </summary>
    public static class RegistrationExtensions
    {
        /// <summary>
        ///     Sets the <see cref="IRequestRegistrationBuilder.Required" /> value.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <typeparam name="TParameter">   Type of the parameter. </typeparam>
        /// <param name="builder">  The builder to act on. </param>
        /// <param name="required"> (Optional) True if required. </param>
        [NotNull]
        [PublicAPI]
        public static IRequestRegistrationBuilder<TParameter> Required<TParameter>([NotNull] this IRequestRegistrationBuilder<TParameter> builder, bool required = true)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Required = required;

            return builder;
        }

        /// <summary>
        ///     Adds a <see cref="IRequestTokenExtractor" /> for extracting the parameter value from a header.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <typeparam name="TParameter">   Type of the parameter. </typeparam>
        /// <param name="builder">  The builder to act on. </param>
        /// <param name="key">      The header key. </param>
        [NotNull]
        [PublicAPI]
        public static IRequestRegistrationBuilder<TParameter> FromHeader<TParameter>([NotNull] this IRequestRegistrationBuilder<TParameter> builder, [NotNull] string key)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));

            builder.TokenExtractors.Add(new HeaderExtractor(key));

            return builder;
        }

        /// <summary>
        ///     Adds a <see cref="IRequestTokenExtractor" /> that uses a value from the route.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <typeparam name="TParameter">   Type of the parameter. </typeparam>
        /// <param name="builder">  The builder to act on. </param>
        /// <param name="key">      The route key. </param>
        /// <param name="value">The route value</param>
        [NotNull]
        [PublicAPI]
        public static IRequestRegistrationBuilder<TParameter> FromRoute<TParameter>([NotNull] this IRequestRegistrationBuilder<TParameter> builder, [NotNull] string key, [CanBeNull] string value)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));

            builder.TokenExtractors.Add(new RouteExtractor(key, value));

            return builder;
        }

        /// <summary>
        ///     Adds a <see cref="IRequestTokenExtractor" /> for extracting the parameter value from the query string.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <typeparam name="TParameter">   Type of the parameter. </typeparam>
        /// <param name="builder">  The builder to act on. </param>
        /// <param name="key">      The query parameter key. </param>
        [NotNull]
        [PublicAPI]
        public static IRequestRegistrationBuilder<TParameter> FromQuery<TParameter>([NotNull] this IRequestRegistrationBuilder<TParameter> builder, [NotNull] string key)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));

            builder.TokenExtractors.Add(new QueryExtractor(key));

            return builder;
        }

        /// <summary>
        ///     Sets the <see cref="IRequestRegistrationBuilder{TParameter}.TryParser" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <typeparam name="TParameter">   Type of the parameter. </typeparam>
        /// <param name="builder">      The builder to act on. </param>
        /// <param name="tryParser">    The try parser delegate. </param>
        [NotNull]
        [PublicAPI]
        public static IRequestRegistrationBuilder<TParameter> TryParser<TParameter>([NotNull] this IRequestRegistrationBuilder<TParameter> builder, [NotNull] TryParseDelegate<TParameter> tryParser)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (tryParser == null) throw new ArgumentNullException(nameof(tryParser));

            builder.TryParser = tryParser;

            return builder;
        }

        /// <summary>
        ///     Sets the <see cref="IRequestRegistrationBuilder{TParameter}.Validator" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <typeparam name="TParameter">   Type of the parameter. </typeparam>
        /// <param name="builder">      The builder to act on. </param>
        /// <param name="validator">    The validator. </param>
        [NotNull]
        [PublicAPI]
        public static IRequestRegistrationBuilder<TParameter> Validator<TParameter>([NotNull] this IRequestRegistrationBuilder<TParameter> builder, [CanBeNull] Func<TParameter, IEnumerable<ValidationError>> validator)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Validator = validator;

            return builder;
        }

        /// <summary>
        ///     Sets the <see cref="IRequestRegistrationBuilder{TParameter}.DefaultValue" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <typeparam name="TParameter">   Type of the parameter. </typeparam>
        /// <param name="builder">      The builder to act on. </param>
        /// <param name="defaultValue"> The default value. </param>
        [NotNull]
        [PublicAPI]
        public static IRequestRegistrationBuilder<TParameter> DefaultValue<TParameter>([NotNull] this IRequestRegistrationBuilder<TParameter> builder, [CanBeNull] TParameter defaultValue)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.DefaultValue = defaultValue;

            return builder;
        }

        /// <summary>
        ///     Sets the <see cref="IRequestRegistrationBuilder{TParameter}.OnParsedCallback" /> that will be called on successful
        ///     parsing.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
        /// <typeparam name="TParameter">   Type of the parameter. </typeparam>
        /// <param name="builder">  The builder to act on. </param>
        /// <param name="onParsed"> The on parsed callback. </param>
        [NotNull]
        [PublicAPI]
        public static IRequestRegistrationBuilder<TParameter> OnParsed<TParameter>([NotNull] this IRequestRegistrationBuilder<TParameter> builder, [CanBeNull] Action<TParameter> onParsed)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.OnParsedCallback = onParsed;

            return builder;
        }
    }
}