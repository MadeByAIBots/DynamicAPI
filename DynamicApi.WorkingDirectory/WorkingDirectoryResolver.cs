using Microsoft.Extensions.Logging;
using System.IO;

namespace DynamicApi.WorkingDirectory;

public class WorkingDirectoryResolver
{
    public WorkingDirectoryResolver(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<WorkingDirectoryResolver>();
    }

    private string? _workingDirectory;

    private readonly ILogger<WorkingDirectoryResolver> _logger;

    public string WorkingDirectory()
    {
        if (_workingDirectory == null)
        {
            _workingDirectory = ResolveDirectory();
        }

        return _workingDirectory;
    }

    private string ResolveDirectory()
    {
        var directory = Directory.GetCurrentDirectory();

        while (!File.Exists(Path.Combine(directory, "config.json")))
        {
            directory = Directory.GetParent(directory)?.FullName;

            if (directory == null)
            {
                throw new FileNotFoundException("Could not find config.json file.");
            }
        }

        _logger.LogInformation("Found working directory:");
        _logger.LogInformation(directory);

        return directory;
    }
}