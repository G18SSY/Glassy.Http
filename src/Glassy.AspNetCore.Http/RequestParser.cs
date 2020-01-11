using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Glassy.Http.AspNetCore
{
    internal class RequestParser : IRequestParser
    {
        private readonly IEnumerable<RequestParameter> parameters;

        public RequestParser(IEnumerable<RequestParameter> parameters)
        {
            this.parameters = parameters;
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
            IEnumerable<RegistrationProcessResult> results = parameters.Select(r => ProcessRegistration(request, settings, r)).ToList();

            if (results.Any(r => !r.Success))
                return new ParseResult(string.Join("\r\n", results.SelectMany(r => r.Errors).Select(e => e.Message)));

            // Post validate
            IEnumerable<ValidationError> errors = PostValidate(results);

            if (errors.Any())
                return new ParseResult(string.Join("\r\n", results.SelectMany(r => r.Errors).Select(e=>e.Message)));

            foreach (RegistrationProcessResult result in results)
            {
                foreach (Action<object> parseCompletedCallback in result.Parameter.OnParseCompletedCallbacks)
                {
                    parseCompletedCallback(result.Value);
                }
            }

            return new ParseResult(results.ToDictionary(r => r.Parameter.Name, r => r.Value));
        }

        private static IEnumerable<ValidationError> PostValidate(IEnumerable<RegistrationProcessResult> results)
        {
            foreach (RegistrationProcessResult result in results)
            {
                IEnumerable<ValidationError> errors = result.Parameter.PostValidators
                                                            .Select(pv => pv(result.Value))
                                                            .Where(e => e != null)
                                                            .SelectMany(e => e)
                                                            .ToList();

                if (!errors.Any()) continue;

                yield return new ValidationError($"Parameter ({result.Parameter.Name}) failed validation...");

                foreach (ValidationError error in errors)
                {
                    yield return error;
                }
            }
        }

        private static RegistrationProcessResult ProcessRegistration(HttpRequest request, RequestParserSettings settings, RequestParameter parameter)
        {
            bool parseFailed = false;

            foreach (TokenExtractor extractor in parameter.TokenExtractors)
            {
                // Extract value
                string extracted = extractor.Extract(request);

                if (extracted == null)
                    continue;

                // Parse
                if (!parameter.TryParser(extracted, out object parsed))
                {
                    parseFailed = true;

                    if (settings.SkipFailedTokenParses)
                        continue;

                    return new RegistrationProcessResult(parameter)
                    {
                        Success = false,
                        Errors = new List<ValidationError>
                        {
                            new ValidationError($"Value provided ({extracted}) for parameter ({parameter.Name}) was invalid and could not be parsed")
                        }
                    };
                }

                // Pre validate
                IList<ValidationError> errors = extractor.PreValidators
                                                         .Select(pv => pv(parsed))
                                                         .Where(e => e != null)
                                                         .SelectMany(e => e)
                                                         .ToList();

                // Valid
                if (errors.Count == 0)
                {
                    return new RegistrationProcessResult(parameter)
                    {
                        Success = true,
                        Value = parsed
                    };
                }

                // Invalid
                if (settings.SkipFailedPreValidations)
                    continue;

                RegistrationProcessResult invalidResult = new RegistrationProcessResult(parameter)
                {
                    Success = false
                };

                invalidResult.Errors.Add(new ValidationError($"Parameter ({parameter.Name}) failed pre-validation..."));
                foreach (ValidationError error in errors)
                {
                    invalidResult.Errors.Add(error);
                }

                return new RegistrationProcessResult(parameter)
                {
                    Success = true,
                    Value = parsed
                };
            }

            // Value(s) extracted but not parsed
            if (parseFailed)
            {
                return new RegistrationProcessResult(parameter)
                {
                    Success = false,
                    Errors = new List<ValidationError>
                    {
                        new ValidationError($"Value(s) provided for parameter ({parameter.Name}) were invalid and could not be parsed")
                    }
                };
            }

            // Value missing and required
            if (parameter.Required)
            {
                return new RegistrationProcessResult(parameter)
                {
                    Success = false,
                    Errors = new List<ValidationError>
                    {
                        new ValidationError($"Required parameter ({parameter.Name}) missing, it can be specified using:\r\n{string.Join("\r\n", parameter.TokenExtractors.Select(e => $"\t HTTP request {e.ExtractsFrom} with a key of {e.Key}"))}")
                    }
                };
            }

            // Value missing and optional
            return new RegistrationProcessResult(parameter)
            {
                Success = true,
                Value = parameter.DefaultValue
            };
        }

        private class RegistrationProcessResult
        {
            public RegistrationProcessResult(RequestParameter parameter)
            {
                Parameter = parameter;
            }

            public object Value { get; set; }

            public IList<ValidationError> Errors { get; set; } = new List<ValidationError>();

            public bool Success { get; set; }

            public RequestParameter Parameter { get; }
        }
    }
}