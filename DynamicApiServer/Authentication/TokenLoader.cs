using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace DynamicApiServer.Authentication
{
    public class TokenLoader
    {
        private readonly ILogger<TokenLoader> _logger;
        private readonly ApiConfiguration _config;
        private readonly WorkingDirectoryResolver _resolver;

        public TokenLoader(ApiConfiguration config, WorkingDirectoryResolver resolver)
        {
            _resolver = resolver;
            _config = config;
            _logger = new Logger<TokenLoader>(new LoggerFactory());
        }

        public TokenLoader(ApiConfiguration config, WorkingDirectoryResolver resolver, ILogger<TokenLoader> logger)
        {
            _resolver = resolver;
            _config = config;
            _logger = logger;
        }

        public string LoadToken()
        {
            return LoadToken(Path.Combine(_resolver.WorkingDirectory(), _config.TokenFilePath));
        }

        public string LoadToken(string tokenFilePath)
        {
            _logger.LogInformation("Token path: " + tokenFilePath);
            try
            {
                var token = File.ReadAllText(tokenFilePath).Trim();
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