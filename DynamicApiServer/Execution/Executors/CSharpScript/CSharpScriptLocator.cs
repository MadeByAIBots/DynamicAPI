using Microsoft.Extensions.Logging;
using System;
using System.IO;

public class CSharpScriptLocator
{
    private readonly ILogger _logger;

    private readonly ApiConfiguration _apiConfiguration;
    private readonly WorkingDirectoryResolver _workingDirectoryResolver;

    public CSharpScriptLocator(ILoggerFactory loggerFactory, ApiConfiguration apiConfiguration,
        WorkingDirectoryResolver workingDirectoryResolver)
    {
        _workingDirectoryResolver = workingDirectoryResolver;
        _logger = loggerFactory.CreateLogger<CSharpScriptLocator>();
        _apiConfiguration = apiConfiguration;
    }

    public string LocateScript(string scriptPath, string folderName)
    {
        _logger.LogInformation(
            $"Locating script file: {Path.Combine(_workingDirectoryResolver.WorkingDirectory(), _apiConfiguration.EndpointPath, folderName, scriptPath)}...");

        if (!File.Exists(Path.Combine(_workingDirectoryResolver.WorkingDirectory(), _apiConfiguration.EndpointPath,
                folderName, scriptPath)))
        {
            _logger.LogError($"Script file not found: {scriptPath}");
            throw new FileNotFoundException($"Script file not found: {scriptPath}");
        }

        _logger.LogInformation($"Script file located: {scriptPath}");

        return Path.Combine(_workingDirectoryResolver.WorkingDirectory(), _apiConfiguration.EndpointPath, folderName,
            scriptPath);
    }
}
