using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace DynamicApiServer.Authentication
{
    public class TokenLoader
    {
        private readonly ILogger<TokenLoader> _logger;
        private readonly ApiConfiguration _config;

        public TokenLoader(ApiConfiguration config)
        {
            _config = config;
            _logger = new Logger<TokenLoader>(new LoggerFactory());
        }

        public TokenLoader(ApiConfiguration config, ILogger<TokenLoader> logger)
        {
            _config = config;
            _logger = logger;
        }

        public string LoadToken()
        {
            return LoadToken(_config.TokenFilePath);
        }

        public string LoadToken(string tokenFilePath)
        {
            try
            {
                var token = File.ReadAllText(tokenFilePath);
                _logger.LogInformation($"Successfully loaded token from {tokenFilePath}");
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to load token from {tokenFilePath}");
                throw;
            }
        }
    }
}