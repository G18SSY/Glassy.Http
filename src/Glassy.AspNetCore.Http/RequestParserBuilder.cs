using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Glassy.Http.AspNetCore
{
    /// <summary>
    ///     A request parser builder used to add registrations and create a new <see cref="IRequestParser" />.
    /// </summary>
    public class RequestParserBuilder
    {
        private readonly Dictionary<string, RequestRegistrationBuilder> registrationBuilders = new Dictionary<string, RequestRegistrationBuilder>();

        /// <summary>
        ///     Registers the parameter described by name.
        /// </summary>
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or illegal values. </exception>
        /// <typeparam name="TParameter">   Type of the parameter. </typeparam>
        /// <param name="name"> The name. This cannot be null. </param>
        [PublicAPI]
        public IRequestRegistrationBuilder<TParameter> RegisterParameter<TParameter>([NotNull] string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            RequestRegistrationBuilder<TParameter> builder = new RequestRegistrationBuilder<TParameter>(name);
            registrationBuilders.Add(name, builder);

            return builder;
        }

        /// <summary>
        ///     Builds a <see cref="IRequestParser" /> based on the registered parameters.
        /// </summary>
        /// <returns>
        ///     An IRequestParser.
        /// </returns>
        [PublicAPI]
        public IRequestParser Build() => new RequestParser(registrationBuilders.Values.Select(BuildRegistration));

        private static RequestRegistration BuildRegistration(RequestRegistrationBuilder registrationBuilder) =>
                new RequestRegistration(registrationBuilder.Name,
                                        registrationBuilder.Required,
                                        registrationBuilder.TokenExtractors,
                                        registrationBuilder.DefaultValue,
                                        registrationBuilder.TryParser,
                                        registrationBuilder.Validator,
                                        registrationBuilder.OnParsedCallback,
                                        registrationBuilder.ParameterType);
    }
}