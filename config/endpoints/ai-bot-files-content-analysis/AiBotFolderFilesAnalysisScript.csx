using System;
using TalkFlow.Messages.Core.Provider;
using TalkFlow.Messages.Model;
using TalkFlow.Messages.Providers.Backend.OpenAI;
using System.IO;
using System.Threading.Tasks;
using DynamicApi.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

public class AiBotSendFilesScriptEndpoint : DynamicEndpointExecutorBase
{
    protected string[] IgnoreFolders { get;set; } = new string[]{
        "obj",
        "bin",
        ".git"
    };
    
    protected string[] IgnoreFiles { get;set; } = new string[]{
    };

    public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var logger = parameters.LoggerFactory.CreateLogger<AiBotSendFilesScriptEndpoint>();
    
        if (!parameters.ApiConfig.Variables.TryGetValue("OpenAIKey", out var apiKey))
        {
            return Fail("OpenAI API key not found in configuration.");
        }

        var workingDirectory = parameters.GetRequiredString("workingDirectory");

        var message = parameters.GetRequiredString("message");
        
        var allFileExtensions = GetAllFileExtensions(workingDirectory);
        
         var relevantExtensions = await GetRelevantFileExtensions(allFileExtensions, message, apiKey, logger);
         
         if (relevantExtensions.Count()==0)
            return Fail("AI bot failed to identify any relevant file extensions.");
         
         var relevantExtensionsString = String.Join('\n', relevantExtensions);
         
         var filesWithExtensions = GetListOfFilePathsMatchingProvidedExtensions(workingDirectory, relevantExtensions);
         
         logger.LogInformation("File paths with matching extensions:\n" + String.Join('\n', filesWithExtensions));
         
         var relativeFilePaths = await GetRelevantFilePaths(message, filesWithExtensions, apiKey, logger);
         
         if (relativeFilePaths.Count()==0)
            return Fail("AI bot failed to identify any relevant file paths.");
            
         var relativeFileContent = GetContentOfMultipleFiles(workingDirectory, relativeFilePaths, logger);
         
         var answerResponse = await GetAnswerFromRelativeFiles(message, relativeFileContent, apiKey, logger);
         
         var answerSectionFromResponse = ExtractAnswerSectionFromAnswerResponse(answerResponse);
         
         return Success(answerSectionFromResponse);
    }
    
    private string[] GetAllFileExtensions(string rootPath)
    {
        var extensions = new HashSet<string>();
        var directories = new Stack<string>();
        directories.Push(rootPath);

        while (directories.Count > 0)
        {
            string dirPath = directories.Pop();
            try
            {
                extensions.UnionWith(Directory.GetFiles(dirPath, "*.*").Select(Path.GetExtension));
                foreach (var directory in Directory.GetDirectories(dirPath))
                {
                    directories.Push(directory);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Access denied to {dirPath}, skipping...");
            }
        }

        return extensions.ToArray();
    }
    
    public async Task<string[]> GetRelevantFileExtensions(string[] allFileExtensions, string message, string apiKey, ILogger<AiBotSendFilesScriptEndpoint> logger)
    {
        var allFileExtensionsString = "\n" + String.Join('\n', allFileExtensions) + "\n";
        
        var inquiryPromptSection = GenerateInquiryPromptSection(message);
        
        var fullContent = @$"
{inquiryPromptSection}

Here is a list of file extensions found in the working directory...

[File Extensions] 
{allFileExtensionsString}
[/File Extensions]

What I would like you to do is figure out which file extensions (from the provided list) might be relevant to respond to the inquiry. Once we have the list, we will then read the file content, and send those files with the inquiry to another bot to get an answer. For example a question about code would mean we need to look at code files.

Please explain your analysis and then afterwards output a list of extensions, with each one starting with a slash, so we can parse using code in the following format...

[Rules]
- The output must have each extension on its own line, starting with a double slash //, as shown in the 'example output'. Otherwise it won't be possible to parse the extensions with code.
- NEVER include binaries/dlls or other files which cannot be read. Only include text based file extensions such as code, configs, etc.
[/Rules]

[Example Output] 
The file extensions relevant to the inquiry are...
// .ext1 
// .ext2
[/Example Output]
        ";
        
        
         logger.LogInformation($"Relevant extensions prompt: {fullContent}");
         
        var responseString = await SendAndReceive(fullContent, apiKey);
        
        logger.LogInformation($"Relevant extensions response:\n" + responseString);
        
        //throw new Exception(responseString);
        var relevantExtensions = ExtractExtensionsFromText(responseString);
        
        logger.LogInformation($"Relevant extensions: {String.Join(' ', relevantExtensions)}");
        
        return relevantExtensions;
    }
    
    
    public async Task<string[]> GetRelevantFilePaths(string message, string[] relativeFilePaths, string apiKey, ILogger<AiBotSendFilesScriptEndpoint> logger)
    {
         var allFilePathsString = "\n" + String.Join('\n', relativeFilePaths) + "\n";

         var inquiryPromptSection = GenerateInquiryPromptSection(message);
        
         var fullContent = @$"
 {inquiryPromptSection}
 
 Here is a list of file paths found in the working directory... 
 {allFilePathsString}
 
 [Instructions]
 1) Analyse the inquiry and provide an explanation of what kinds of files, and file names, might provide relevant information.
 2) Brainstorm where we might possibly find information/answers relevant to the inquiry
 3) Analyse the folder structure in relation to the inquiry, as this will often help to understand which files are relevant.
 4) Analyse each file name/path and provide an explanation of what it is likely to be for, and what it might possibly be for.
 5) Identify which files might possibly be relevant to the inquiry.
 6) Provide a list of possibly relevant files in the example output format below.
 [/Instructions]
 
 
[Rules]
- The relevant output must have each relevant file on its own new line, starting with a double slash //, as shown in the 'example output'. Otherwise it won't be possible to parse the file paths with code.
- DO INCLUDE ALL files that MIGHT POSSIBLY be relevant.
- DON'T include files that are certainly not relevant.
- It is better to include a file in the list which is not relevant, than to accidentally exclude a file that is relevant.
- DO NOT include folders in your suggestions. ONLY include files. Files have extensions. If it does not have an extension do not include it in the list.
- Ensure you include the full relative path to any file you suggest including parent folders
- ONLY include files which are in the file list above. Do not guess or make up file names which do not exist. 
- If it is a question about code then the files may not explicitly say what the project does. In this case try to pick code files that could be analysed to figure out the answer to the inquiry.
[/Rules]

 Please explain your analysis and then afterwards output a list of relative file paths, with each one starting with a slash, so we can parse using code in the following format...
 [Example Output]
 1) Inquiry analysis: ...
 (add analysis here)
 
 2) Brainstorming where we might find answers: ...
 (add brainstorming here)
 
 3) Analysing folder structure: ...
 (add analysis here)
 
 4) Analysing file names: ...
 (add analysis here)
 
 5) Identifying possibly relevant file names: ...
 (add analysis here)
 
 6) The files relevant to the inquiry are...
 // /<folder1>/<file1>.txt
 // /<file2>.txt
 // /<folder3>/<subfolder1>/<file3>.txt
 [/Example Output]
         ";

         logger.LogInformation($"Relevant file paths prompt: {fullContent}");

         var responseString = await SendAndReceive(fullContent, apiKey);
         
         logger.LogInformation($"Relevant file paths response:\n" + responseString);
         

         var relevantFilePaths = ExtractFilePathsFromText(responseString);
         
         logger.LogInformation($"Relevant file paths:\n{String.Join(' ', relevantFilePaths)}");
         
         return relevantFilePaths;
    }
    
    
    public async Task<string> GetAnswerFromRelativeFiles(string message, string contentOfMultipleFiles, string apiKey, ILogger<AiBotSendFilesScriptEndpoint> logger)
    {
         var inquiryPromptSection = GenerateInquiryPromptSection(message);
        
         var fullContent = @$"
         {inquiryPromptSection}
         
         Here is the content of all relevant files... 
         {contentOfMultipleFiles}
         
         [Instructions]
         1) Analyse the inquiry and provide an explanation of what the bot is looking for, and where we might find that information.
         2) Analyse each file and provide a summary of what it does.
         3) Identify which files might possibly be relevant to the inquiry.
         4) Analyse the relevant files in relation to the inquiry and output an answer/response to the inquiry, including all relevant details.
         [/Instructions]
         
         [Relevant Details]
         - Full relative file path
         - Class names (with summaries) if relevant to the inquiry
         - Method/function names (with summaries) if relevant to the inquiry
         - Property/field names (with summaries) if relevant to the inquiry
         [/Relevant Details]
         
         Your response needs to be divided into two sections.
         - The first section (irrelevant) is simply you 'thinking out loud'. This section is to help you think through your reasoning BEFORE generating an answer. This section will NOT be sent back to the bot, so do not expect the bot to see what's in this section.
         - The second section (relevant) is your answer/response to the inquiry. This section will be sent back to the bot which submitted the inquiry. This section needs to include all relevant information to answer/respond to the inquiry. But do not include any relevant information as we do not want to waste the recipient bot's tokens.
         
         - RULE: ENSURE that the '// Irrelevant...' and '// Relevant...' lines exist in your output, and remain the same as shown in the example. These are required to be able to parse your response using code.
         - Note: The rest of your output will need to be adjusted/adapted to suit the inquiry and the files provided. In your working out, explain/think about how best to format the answer to provide the best response possible.
         - Rule: DO NOT include ANY IRRELEVANT files or information in the // Relevant section. This information can be included in the // Irrelevant section instead.
         
         [Example Output]
         // Irrelevant to the inquiry (IMPORTANT: include/keep this line, and keep the double slash at the start)
         File: /folder1/irrelevant-file1.cs
         This file contains to the code to do XYZ but is not relevant to the inquiry.
         
         Class: IrrelevantClassName
         This class handles ABC but is not relevant to the inquiry.
                  
         // Relevant answer/response to the inquiry (IMPORTANT: include/keep this line, and keep the double slash at the start)
         File: /folder1/relevant-file1.cs
         This file contains to the code to do XYZ and is relevant to the inquiry because...
          
         Class: RelevantClassName
         This class handles ABC and is relevant to the inquiry because
         
         ....etc... (adapt your response to the inquiry and files provided)
         [/Example Output]
         ";

         logger.LogInformation($"Relevant file content prompt: {fullContent}");

         var responseString = await SendAndReceive(fullContent, apiKey);
         
         logger.LogInformation($"Answer response:\n" + responseString);
         

//          var relevantFilePaths = ExtractFilePathsFromText(responseString);
//          
//          logger.LogInformation($"Relevant file paths:\n{String.Join(' ', relevantFilePaths)}");
//          
         return responseString;
    }
    
    private string[] GetListOfFilePathsMatchingProvidedExtensions(string rootPath, string[] extensions, string prefix = "")
     {
                 var builder = new StringBuilder();
                 
                         var entries = Directory.EnumerateFileSystemEntries(rootPath)
                             .Select(entry => new
                             {
                                 Path = entry,
                                 IsDirectory = (File.GetAttributes(entry) & FileAttributes.Directory) != 0
                             })
                             .Where(entry => !IgnoreFolders.Any(folder => entry.Path.Contains($@"/{folder}/"))
                                             && !IgnoreFiles.Contains(Path.GetFileName(entry.Path)))
                             .ToList();
                 
                         for (int i = 0; i < entries.Count; i++)
                         {
                             var entry = entries[i];
                 
                             if (entry.IsDirectory)
                             {
                                 string newPrefix = prefix + (i == entries.Count - 1 ? "    " : "│   ");
                                 string[] subTree = GetListOfFilePathsMatchingProvidedExtensions(entry.Path, extensions, newPrefix);
                 
                                 // Only append the directory and its subtree if the subtree is not empty
                                 if (!string.IsNullOrWhiteSpace(subTree[0]))
                                 {
                                     builder.AppendLine($"{prefix}{(i == entries.Count - 1 ? "└── " : "├── ")}{Path.GetFileName(entry.Path)}");
                                     builder.Append(subTree[0]);
                                 }
                             }
                             else if (extensions.Contains(Path.GetExtension(entry.Path).Trim('.')))
                             {
                                 builder.AppendLine($"{prefix}{(i == entries.Count - 1 ? "└── " : "├── ")}{Path.GetFileName(entry.Path)}");
                             }
                         }
                 
                         return new string[] { builder.ToString() };
     }
    
    public async Task<string> SendAndReceive(string content, string apiKey)
    {
        var messageBackendProvider = new OpenAIBackendProvider("https://api.openai.com", apiKey);
        var messageToBot = new Message(content);
        var response = await messageBackendProvider.SendAndReceive(messageToBot);
        
        return response.Content;
    }
    
    private string[] ExtractExtensionsFromText(string response)
    {
        var lines = response.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var extensions = new List<string>();

        foreach (var line in lines)
        {
            if (line.TrimStart().StartsWith("//"))
            {
                var ext = line.Trim().TrimStart('/').Trim().Trim('.').Trim().Trim('.');
                extensions.Add(ext);
            }
        }

        return extensions.ToArray();
    }
    
    private string[] ExtractFilePathsFromText(string response)
    {
        var lines = response.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var filePaths = new List<string>();

        foreach (var line in lines)
        {
            if (line.TrimStart().Trim('-').Trim().StartsWith("//"))
            {
                var filePath = line.TrimStart().Trim('-').Trim().TrimStart('/');
                if (!filePaths.Contains(filePath))
                    filePaths.Add(filePath);
            }
        }

        return filePaths.ToArray();
    }
    
    private string GenerateInquiryPromptSection(string message)
    {
        return @$"A bot has submitted an inquiry about files in the current working directory. The inquiry is...
            [Inquiry]
            {message}
            [/Inquiry]
            ";
    }
    
        private string GetContentOfMultipleFiles(string workingDirectory, string[] relativeFilePaths, ILogger<AiBotSendFilesScriptEndpoint> logger)
        {
            logger.LogInformation(
            $@"Getting content of multiple files...
            Relative file paths:
            {String.Join('\n', relativeFilePaths)}
            ");
        
            var builder = new StringBuilder();
    
            foreach (var relativePath in relativeFilePaths)
            {
                string fullPath = workingDirectory + "/" + relativePath.Trim().TrimStart('/').Trim();
                logger.LogInformation("Loading file content: " + fullPath);
                if (File.Exists(fullPath))
                {
                    builder.AppendLine($"({relativePath})");
                    builder.AppendLine(File.ReadAllText(fullPath));
                    builder.AppendLine();
                }
                else
                {
                    builder.AppendLine($"File {relativePath} does not exist.");
                }
            }
    
            return builder.ToString();
        }
        
        private string ExtractAnswerSectionFromAnswerResponse(string response)
        {
            const string answerMarker = "// Relevant";
            var lines = response.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int answerLineIndex = Array.FindIndex(lines, line => line.Contains(answerMarker, StringComparison.OrdinalIgnoreCase));
    
            if (answerLineIndex >= 0 && answerLineIndex < lines.Length - 1)
            {
                return string.Join(Environment.NewLine, lines.Skip(answerLineIndex + 1));
            }
            else
            {
                return "No answer found in the response.";
            }
        }
}
