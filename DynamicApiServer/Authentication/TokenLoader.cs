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

        public TokenLoader(ApiConfiguration config, WorkingDirectoryResolver resolver, ILoggerFactory loggerFactory)
        {
            _resolver = resolver;
            _config = config;
            _logger = loggerFactory.CreateLogger<TokenLoader>();
        }

private string GetTokenPath()
        {
            try
            {
                var workingDirectory = _resolver.WorkingDirectory();
                if (workingDirectory == null)
                {
                    _logger.LogError("Working directory is null.");
                    throw new ArgumentNullException(nameof(workingDirectory));
                }

                if (_config.TokenFilePath == null)
                {
                    _logger.LogError("Token file path from configuration is null.");
                    throw new ArgumentNullException(nameof(_config.TokenFilePath));
                }

                var tokenFilePath = Path.Combine(workingDirectory, _config.TokenFilePath);
                _logger.LogInformation("Token path: " + tokenFilePath);

                if (string.IsNullOrEmpty(_config.TokenFilePath))
                {
                    _logger.LogError("Token file path is not set in the configuration.");
                    throw new InvalidOperationException("Token file path is not set in the configuration.");
                }

                if (!File.Exists(tokenFilePath))
                {
                    _logger.LogError($"Token file does not exist at {tokenFilePath}.");
                    throw new FileNotFoundException($"Token file does not exist at {tokenFilePath}.");
                }

                return tokenFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the token file path.");
                throw;
            }
        }

        public string LoadToken()
        {
            var tokenFilePath = GetTokenPath();
            try
            {
                var token = File.ReadAllText(tokenFilePath).Trim();
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading token from file.");
                throw;
            }
        }
    }
}
