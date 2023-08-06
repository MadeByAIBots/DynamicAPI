using System.Diagnostics;
using System.Text;
using DynamicApi.Contracts;
using Microsoft.Extensions.Logging;

namespace DirectoryListEndpoint;

public class DirectoryListEndpoint : DynamicEndpointExecutorBase
{
    public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var workingDirectory = parameters.GetRequiredString("workingDirectory");
        var directoryPath = parameters.GetString("directoryPath");
        bool recursive = parameters.GetBool("recursive", false);
        
        var logger = new Logger<DirectoryListEndpoint>(parameters.LoggerFactory);

        logger.LogInformation($"Working directory: {workingDirectory}");
        logger.LogInformation($"Relative directory: {directoryPath}");

        var fullPath = workingDirectory;

        if (!String.IsNullOrEmpty(directoryPath))
            fullPath = Path.Combine(workingDirectory, directoryPath);

        logger.LogInformation($"Absolute directory: {fullPath}");

        if (!Directory.Exists(fullPath))
        {
            return Fail($"Error: The directory '{fullPath}' does not exist.");
        }

        var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        logger.LogInformation($"Search option: {searchOption}");

        var files = Directory.GetFiles(fullPath, "*", searchOption).OrderBy(f => f).ToArray();
        var directories = Directory.GetDirectories(fullPath, "*", searchOption).OrderBy(d => d).ToArray();

        var relativeFiles = Array.ConvertAll(files, file => Path.GetRelativePath(workingDirectory, file));
        var relativeDirectories =
            Array.ConvertAll(directories, directory => Path.GetRelativePath(workingDirectory, directory));

        var relativeFilesString = string.Join("\n", relativeFiles).Trim();
        if (String.IsNullOrEmpty(relativeFilesString))
            relativeFilesString = "[none]";

        var relativeDirectoriesString = string.Join("\n", relativeDirectories).Trim();
        if (String.IsNullOrEmpty(relativeDirectoriesString))
            relativeDirectoriesString = "[none]";

        var output = "Files:\n" + relativeFilesString + "\n\nDirectories:\n" + relativeDirectoriesString;

        return Success(output);
    }
}