using System.Diagnostics;
using System.Text;
using DynamicApi.Contracts;
using DynamicApi.Utilities.Files;

namespace FileLineInsertEndpoint;

public class FileLineInsertEndpoint : DynamicEndpointExecutorBase
{
	public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
	{
		var workingDirectory = parameters.GetRequiredString("workingDirectory");
		var filePath = parameters.GetRequiredString("filePath");
		var beforeLineNumber = parameters.GetRequiredInt32("beforeLineNumber");
		var lineHash = parameters.GetRequiredString("beforeLineHash");
		var newContent = parameters.GetRequiredString("newContent");

		// Parameter checks
		if (string.IsNullOrEmpty(workingDirectory))
		{
		    return Fail("Error: The 'workingDirectory' parameter is null or empty.");
		}

		if (string.IsNullOrEmpty(filePath))
		{
			return Fail("Error: The 'filePath' parameter is null or empty.");
		}

		if (beforeLineNumber < 1)
		{
			return Fail("Error: The 'beforeLineNumber' parameter must be greater than 0.");
		}

		if (string.IsNullOrEmpty(newContent))
		{
			return Fail("Error: The 'newContent' parameter is null or empty.");
		}

		var fullPath = Path.Combine(workingDirectory, filePath);

		// File existence check
		if (!File.Exists(fullPath))
		{
			return Fail("Error: The file '{fullPath}' does not exist.");
		}

		var lines = new List<string>(File.ReadAllLines(fullPath));

		// Check that beforeLineNumber is a valid line number in the file
		if (beforeLineNumber > lines.Count + 1)
		{
			return Fail("Error: The 'beforeLineNumber' parameter is greater than the number of lines in the file plus 1.");
		}

		// Hash check
		var generatedHash = DynamicApi.Utilities.Files.HashUtils.GenerateSimpleHash(lines[beforeLineNumber - 1]);
		if (generatedHash.ToLower() != lineHash.ToLower())
		{
			return Fail(File.ReadAllText(fullPath).ToNumbered() + "\n\nError: Line hash and line number do not match. Verify and try again.");
		}

        var beforeLineIndex = beforeLineNumber - 1;

		// Subtract 1 from beforeLineNumber to get the correct index
		lines.Insert(beforeLineIndex, newContent);

		File.WriteAllLines(fullPath, lines);

		return Success("New file content:\n\n" + File.ReadAllText(fullPath).ToNumbered() + "\n\nLine inserted successfully");
	}
}
