using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Glassy.Http.AspNetCore
{
    internal class RequestParser : IRequestParser
    {
        private readonly IEnumerable<RequestRegistration> registrations;

        public RequestParser(IEnumerable<RequestRegistration> registrations)
        {
            this.registrations = registrations;
        }

        /// <summary>
        ///     Parses the given request using default settings.
        /// </summary>
        /// <param name="request">  The request. </param>
        /// <returns>
        ///     An IParseResult.
        /// </returns>
        public IParseResult Parse(HttpRequest request) => Parse(request, RequestParserSettings.Default);

        /// <summary>
        ///     Parses the given request using specific settings.
        /// </summary>
        /// <param name="request">  The request. </param>
        /// <param name="settings">The settings.</param>
        /// <returns>
        ///     An IParseResult.
        /// </returns>
        public IParseResult Parse(HttpRequest request, RequestParserSettings settings)
        {
            // Process all the registered parameters
            IEnumerable<RegistrationProcessResult> processResults = registrations.Select(r => ProcessRegistration(request, settings, r)).ToList();

            if (processResults.Any(r => !r.Success))
                return new ParseResult(string.Join("\r\n", processResults.SelectMany(r => r.Errors)));

            foreach (RegistrationProcessResult result in processResults.Where(r => r.Registration.OnParsedCallback != null))
            {
                // ReSharper disable once PossibleNullReferenceException
                result.Registration.OnParsedCallback(result.Value);
            }

            return new ParseResult(processResults.ToDictionary(r => r.Registration.Name, r => r.Value));
        }

        private static RegistrationProcessResult ProcessRegistration(HttpRequest request, RequestParserSettings settings, RequestRegistration registration)
        {
            bool parseFailed = false;

            foreach (IRequestTokenExtractor extractor in registration.TokenExtractors)
            {
                // Extract value
                string extracted = extractor.Extract(request);

                if (extracted == null)
                    continue;

                // Parse
                if (!registration.TryParser(extracted, out object parsed))
                {
                    parseFailed = true;

                    if (settings.SkipFailedTokenParses)
                        continue;

                    return new RegistrationProcessResult(registration)
                    {
                        Success = false,
                        Errors = new List<ValidationError>
                        {
                            new ValidationError($"Value provided ({extracted}) for parameter ({registration.Name}) was invalid and could not be parsed")
                        }
                    };
                }

                // Validate
                IEnumerable<ValidationError> errors = registration.Validator?.Invoke(parsed);

                // Valid
                // ReSharper disable once PossibleMultipleEnumeration
                if (errors == null || !errors.Any())
                {
                    return new RegistrationProcessResult(registration)
                    {
                        Success = true,
                        Value = parsed
                    };
                }

                // Invalid
                RegistrationProcessResult invalidResult = new RegistrationProcessResult(registration)
                {
                    Success = false
                };

                invalidResult.Errors.Add(new ValidationError($"Parameter ({registration.Name}) failed validation..."));
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (ValidationError error in errors)
                {
                    invalidResult.Errors.Add(error);
                }

                return new RegistrationProcessResult(registration)
                {
                    Success = true,
                    Value = parsed
                };
            }

            // Value(s) extracted but not parsed
            if (parseFailed)
            {
                return new RegistrationProcessResult(registration)
                {
                    Success = false,
                    Errors = new List<ValidationError>
                    {
                        new ValidationError($"Value(s) provided for parameter ({registration.Name}) were invalid and could not be parsed")
                    }
                };
            }

            // Value missing and required
            if (registration.Required)
            {
                return new RegistrationProcessResult(registration)
                {
                    Success = false,
                    Errors = new List<ValidationError>
                    {
                        new ValidationError($"Required parameter ({registration.Name}) missing, it can be specified using:\r\n{string.Join("\r\n", registration.TokenExtractors.Select(e => $"\t HTTP request {e.ExtractsFrom} with a key of {e.Key}"))}")
                    }
                };
            }

            // Value missing and optional
            return new RegistrationProcessResult(registration)
            {
                Success = true,
                Value = registration.DefaultValue
            };
        }

        private class RegistrationProcessResult
        {
            public RegistrationProcessResult(RequestRegistration registration)
            {
                Registration = registration;
            }

            public object Value { get; set; }

            public IList<ValidationError> Errors { get; set; } = new List<ValidationError>();

            public bool Success { get; set; }

            public RequestRegistration Registration { get; }
        }
    }
}