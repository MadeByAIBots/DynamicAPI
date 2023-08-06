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
        
        Console.WriteLine("Working dir: " + _workingDirectory);
        _logger.LogInformation("Working dir: " + _workingDirectory);

        return _workingDirectory;
    }
    
    public string BinariesDirectory()
    {
        if (_binariesDirectory == null)
        {
            _binariesDirectory = WorkingDirectory() + "/bin";
            // _binariesDirectory = FindDirectoryContainingFileInCurrentOrAnyParent("bin/DynamicApiServer.dll");
            // _binariesDirectory += "/bin";
        }

        return _binariesDirectory;
    }

    private string FindDirectoryContainingFileInCurrentOrAnyParent(string fileName)
    {
        _logger.LogDebug($"Looking for directory containing provided file: {fileName}");
        
        var directory = Directory.GetCurrentDirectory();
        
        _logger.LogDebug($"  Starting directory: {directory}");

        while (!File.Exists(Path.Combine(directory, fileName)))
        {
            directory = Directory.GetParent(directory)?.FullName;
            
            _logger.LogDebug($"  Checking dir: {directory}");

            if (directory == null)
            {
                throw new FileNotFoundException($"Could not find {fileName} file.");
            }
        }

        _logger.LogInformation($"Found directory containing {fileName} file: {directory}");

        return directory;
    }
}