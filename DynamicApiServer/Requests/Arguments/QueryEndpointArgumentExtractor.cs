using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace DynamicApiServer.Requests.Arguments
{
    public class QueryEndpointArgumentExtractor : BaseEndpointArgumentExtractor
    {
        public QueryEndpointArgumentExtractor(ILoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<QueryEndpointArgumentExtractor>())
        {
        }

        protected override async Task<string> ExtractSingleArgument(EndpointRequestInfo requestInfo, EndpointArgumentDefinition argumentDefinition)
        {
            try
            {
                _logger.LogDebug($"Starting extraction of query argument: {argumentDefinition.Name}");

                var values = requestInfo.Context.Request.Query[argumentDefinition.Name];

                if (values == StringValues.Empty)
                {
                    _logger.LogWarning($"Missing query argument: {argumentDefinition.Name}");
                    return null;
                }
                else
                {
                    _logger.LogDebug($"Successfully extracted query argument: {argumentDefinition.Name}");
                    return values[0];
                }
            }
            catch (Exception ex)
            {
                // Log the error and rethrow the exception
                _logger.LogError(ex, $"Error extracting query argument: {argumentDefinition.Name}");
                throw;
            }
        }
    }
}