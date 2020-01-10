using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Atkins.Http.AspNetCore
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
            IEnumerable<RegistrationProcessResult> processResults = registrations.Select(r=>ProcessRegistration(request,settings,r)).ToList();

            if (processResults.Any(r => !r.Success))
            {

            }
        }

        private RegistrationProcessResult ProcessRegistration(HttpRequest request, RequestParserSettings settings, RequestRegistration registration)
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

                    return new RegistrationProcessResult
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
                if (errors == null || !errors.Any())
                {
                    return new RegistrationProcessResult
                    {
                        Success = true,
                        UsedDefault = false,
                        Value = parsed
                    };
                }

                // Invalid
                RegistrationProcessResult invalidResult = new RegistrationProcessResult
                {
                    Success = false
                };

                invalidResult.Errors.Add(new ValidationError($"Parameter ({registration.Name}) failed validation..."));
                foreach (ValidationError error in errors)
                {
                    invalidResult.Errors.Add(error);
                }

                return new RegistrationProcessResult
                {
                    Success = true,
                    UsedDefault = false,
                    Value = parsed
                };
            }

            // Value(s) extracted but not parsed
            if (parseFailed)
            {
                return new RegistrationProcessResult
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
                return new RegistrationProcessResult
                {
                    Success = false,
                    Errors = new List<ValidationError>
                    {
                        new ValidationError($"Required parameter ({registration.Name}) missing, it can be specified using:\r\n{string.Join("\r\n", registration.TokenExtractors.Select(e => $"\t HTTP request {e.ExtractsFrom} with a key of {e.Key}"))}")
                    }
                };
            }

            // Value missing and optional
            return new RegistrationProcessResult
            {
                Success = true,
                UsedDefault = true,
                Value = registration.DefaultValue
            };
        }

        private class RegistrationProcessResult
        {
            public object Value { get; set; }

            public IList<ValidationError> Errors { get; set; } = new List<ValidationError>();

            public bool UsedDefault { get; set; }

            public bool Success { get; set; }
        }
    }
}