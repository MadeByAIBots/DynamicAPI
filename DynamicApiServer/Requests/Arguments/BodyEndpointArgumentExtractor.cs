using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DynamicApiServer.Requests.Arguments
{
    public class BodyEndpointArgumentExtractor : BaseEndpointArgumentExtractor
    {
        public BodyEndpointArgumentExtractor(ILoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<BodyEndpointArgumentExtractor>())
        {
        }

        protected override async Task<string> ExtractSingleArgument(EndpointRequestInfo requestInfo, EndpointArgumentDefinition argumentDefinition)
        {
            try
            {

                _logger.LogDebug($"Starting extraction of body argument: {argumentDefinition.Name}");

                var body = await requestInfo.Body();

                _logger.LogDebug("Body: " + body);
                if (argumentDefinition.Type == "string")
                {
                    return await ExtractPlainTextArgument(body, argumentDefinition);
                }
                else if (argumentDefinition.Type == "json")
                {
                    return await ExtractJsonArgument(body, argumentDefinition);
                }
                else
                {
                    // Handle other argument types if necessary
                    throw new Exception($"Unsupported argument type: {argumentDefinition.Type}");
                }
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, $"IO error while reading request body for argument: {argumentDefinition.Name}");
                throw;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, $"Operation cancelled while reading request body for argument: {argumentDefinition.Name}");
                throw;
            }
            catch (Exception ex)
            {
                // Log the error and rethrow the exception
                _logger.LogError(ex, $"Unexpected error extracting body argument: {argumentDefinition.Name}");
                throw;
            }
        }

        private async Task<string> ExtractPlainTextArgument(string body, EndpointArgumentDefinition argumentDefinition)
        {
            // For plain text, the entire body is the argument value
            _logger.LogDebug($"Successfully extracted body argument: {argumentDefinition.Name}");
            return body;
        }

        private async Task<string> ExtractJsonArgument(string body, EndpointArgumentDefinition argumentDefinition)
        {
            // For JSON, parse the body and extract the argument by its name
            try
            {
                var jsonDocument = JsonDocument.Parse(body);
                if (jsonDocument.RootElement.TryGetProperty(argumentDefinition.Name, out var jsonElement))
                {
                    _logger.LogDebug($"Successfully extracted body argument: {argumentDefinition.Name}");
                    return jsonElement.GetString();
                }
                else
                {
                    _logger.LogWarning($"Missing body argument: {argumentDefinition.Name}");
                    return null;
                }
            }
            catch (JsonException ex)
            {
                var sanitizedBody = Regex.Replace(body, "\\W", "*");
                _logger.LogError(ex, $"Error parsing JSON body for argument: {argumentDefinition.Name}. Sanitized body content: {sanitizedBody}");
                throw new Exception($"Error parsing JSON body for argument: {argumentDefinition.Name}. Sanitized body content: {sanitizedBody}", ex);
            }
        }
    }
}
