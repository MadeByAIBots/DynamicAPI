using System.IO;

namespace DynamicApiServer;

public class WorkingDirectoryResolver
{
    private string _workingDirectory;

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

        Console.WriteLine("Found working directory:");
        Console.WriteLine(directory);

        return directory;
    }
}
