#r "nuget: Newtonsoft.Json, 13.0.1"
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Define the required fields
string[] requiredFields = {
    "description",
    "path",
    "executor",
    "method",
    "responses"
};
string[] argsFields = { "name", "type", "source", "description" };
string[] responsesFields = { "statusCode", "description", "type" };

// Get the current directory
string currentDirectory = Directory.GetCurrentDirectory();

// Get all endpoint.json files in the current directory and subdirectories
string[] jsonFiles = Directory.GetFiles(currentDirectory, "endpoint.json", SearchOption.AllDirectories);

foreach (string jsonFile in jsonFiles)
{
string endpointName = Path.GetFileName(Path.GetDirectoryName(jsonFile));
    try
    {
        string jsonText = File.ReadAllText(jsonFile);
        JObject jsonObj = JObject.Parse(jsonText);

        // Check for required fields
        foreach (string field in requiredFields)
        {
            if (jsonObj[field] == null)
            {
Console.WriteLine($"ERROR: Missing required field '{field}' in {endpointName}");
            }
        }

        // Check 'args' and 'responses' arrays
        JArray argsArray = (JArray)jsonObj["args"];
        JArray responsesArray = (JArray)jsonObj["responses"];

if (argsArray != null)
{
        foreach (JObject arg in argsArray)
        {
            foreach (string field in argsFields)
            {
                if (arg[field] == null)
                {
Console.WriteLine($"ERROR: Missing required field '{field}' in 'args' of {endpointName}");
                }
            }
        }
}

        foreach (JObject response in responsesArray)
        {
            foreach (string field in responsesFields)
            {
                if (response[field] == null)
                {
Console.WriteLine($"ERROR: Missing required field '{field}' in 'responses' of {endpointName}");
                }
            }
        }

Console.WriteLine($"SUCCESS: No errors found in {endpointName}");
    }
    catch (JsonReaderException)
    {
Console.WriteLine($"ERROR: Invalid JSON in {endpointName}");
    }
    catch (Exception ex)
    {
Console.WriteLine($"ERROR: An unexpected error occurred in {endpointName}: {ex.Message}");
    }
}
