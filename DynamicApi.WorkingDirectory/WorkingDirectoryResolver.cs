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
    private string? _binariesDirectory;

    private readonly ILogger<WorkingDirectoryResolver> _logger;

    public string WorkingDirectory()
    {
        if (_workingDirectory == null)
        {
            _workingDirectory = FindDirectoryContainingFileInCurrentOrAnyParent("config.json");
        }

        return _workingDirectory;
    }
    
    public string BinariesDirectory()
    {
        if (_binariesDirectory == null)
        {
            _binariesDirectory = FindDirectoryContainingFileInCurrentOrAnyParent("DynamicApiServer.dll");
        }

        return _binariesDirectory;
    }

    private string FindDirectoryContainingFileInCurrentOrAnyParent(string fileName)
    {
        var directory = Directory.GetCurrentDirectory();

        while (!File.Exists(Path.Combine(directory, fileName)))
        {
            directory = Directory.GetParent(directory)?.FullName;

            if (directory == null)
            {
                throw new FileNotFoundException($"Could not find {fileName} file.");
            }
        }

        _logger.LogInformation($"Found directory containing {fileName} file: {directory}");

        return directory;
    }
}