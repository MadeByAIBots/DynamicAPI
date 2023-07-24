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
    public override async Task<EndpointExecutionResult> ExecuteAsync(DynamicExecutionParameters parameters)
    {
        var logger = parameters.LoggerFactory.CreateLogger<AiBotSendFilesScriptEndpoint>();
    
        if (!parameters.ApiConfig.Variables.TryGetValue("OpenAIKey", out var apiKey))
        {
            return Fail("OpenAI API key not found in configuration.");
        }

        var workingDirectory = parameters.GetRequiredString("workingDirectory");
        //var messageContentToBot = "";

            var message = parameters.GetRequiredString("message");
            //messageContentToBot += message + "\n\n\n";
            
            var allFileExtensions = GetAllFileExtensions(workingDirectory);
            
            //return Success(String.Join('\n', allFileExtensions));
             var relevantExtensions = await GetRelevantFileExtensions(allFileExtensions, message, apiKey, logger);
             
             if (relevantExtensions.Count()==0)
                return Fail("AI bot failed to identify any relevant file extensions.");
             
             var relevantExtensionsString = String.Join('\n', relevantExtensions);
             
             var filesWithExtensions = GetFilesWithExtensions(workingDirectory, relevantExtensions);
             
             logger.LogInformation("File paths with matching extensions:\n" + String.Join('\n', filesWithExtensions));
             
             var relativeFilePaths = await GetRelevantFilePaths(message, filesWithExtensions, apiKey, logger);
             
             if (relativeFilePaths.Count()==0)
                return Fail("AI bot failed to identify any relevant file paths.");
                
             var relativeFileContent = GetContentOfMultipleFiles(workingDirectory, relativeFilePaths);
             
             var answerResponse = await GetAnswerFromRelativeFiles(message, relativeFileContent, apiKey, logger);
             
             var answerSectionFromResponse = ExtractAnswerSectionFromAnswerResponse(answerResponse);
             
             return Success(answerSectionFromResponse);
//         foreach (var path in filePaths)
//         {
//             var absolutePath = Path.Combine(workingDirectory, path);
// 
//             if (!File.Exists(absolutePath))
//             {
//                 return Fail($"File does not exist at path: {absolutePath}");
//             }
// 
//             try
//             {
//                 messageContentToBot += absolutePath + "\n" + File.ReadAllText(absolutePath).Trim() + "\n\n\n";
//             }
//             catch (Exception e)
//             {
//                 return Fail($"Failed to read file at {absolutePath}: {e.Message}");
//             }
//         }

//         var messageBackendProvider = new OpenAIBackendProvider("https://api.openai.com", apiKey);
//         var messageToBot = new Message(messageContentToBot);
//         var response = await messageBackendProvider.SendAndReceive(messageToBot);
//         if (string.IsNullOrEmpty(response.Content))
//         {
//             return Fail("No response received from OpenAI API.");
//         }
//         return Success(response.Content);
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
        - The output must have each extension on its own line, starting with a slash, as shown in the 'example output'. Otherwise it won't be possible to parse the extensions with code.
        [/Rules]
         
        [Example Output] 
        /.ext1 
        /.ext2
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
         2) Analyse each file name/path and provide an explanation of what it is likely to be for, and what it might possibly be for.
         3) Identify which files might possibly be relevant to the inquiry.
         4) Provide a list of possibly relevant files in the format below.
         [/Instructions]
         
         
        [Rules]
        - The output must have each file on its own line, starting with a slash, as shown in the 'example output'. Otherwise it won't be possible to parse the file paths with code.
        - DO INCLUDE files that MIGHT POSSIBLY be relevant.
        - DON'T include files that are highly unlikely to be relevant.
        - It is better to include a file in the list which is not relevant, than to accidentally exclude a file that is relevant.
        [/Rules]
        
         Please explain your analysis and then afterwards output a list of relative file paths, with each one starting with a slash, so we can parse using code in the following format...
         [Example Output]
         /folder1/file1.txt
         /file2.txt
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
         // Irrelevant to the inquiry
         File: /folder1/irrelevant-file1.cs
         This file contains to the code to do XYZ but is not relevant to the inquiry.
         
         Class: IrrelevantClassName
         This class handles ABC but is not relevant to the inquiry.
                  
         // Relevant to the inquiry
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
    
    private string[] GetFilesWithExtensions(string rootPath, string[] extensions)
    {
        var files = new List<string>();
        var directories = new Stack<string>();
        directories.Push(rootPath);

        while (directories.Count > 0)
        {
            string dirPath = directories.Pop();
            try
            {
                files.AddRange(Directory.EnumerateFiles(dirPath, "*.*")
                    .Where(file => extensions.Contains(Path.GetExtension(file).Trim('.')))
                    .Select(file => Path.GetRelativePath(rootPath, file)));

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

        return files.ToArray();
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
            if (line.TrimStart().StartsWith("/"))
            {
                var ext = line.TrimStart('/').Trim().Trim('.').Trim().Trim('.');
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
            if (line.TrimStart().StartsWith("/"))
            {
                var filePath = line.TrimStart('/');
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
    
        private string GetContentOfMultipleFiles(string workingDirectory, string[] relativeFilePaths)
        {
            var builder = new StringBuilder();
    
            foreach (var relativePath in relativeFilePaths)
            {
                string fullPath = Path.Combine(workingDirectory, relativePath);
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
            int answerIndex = response.IndexOf(answerMarker, StringComparison.OrdinalIgnoreCase);
    
            if (answerIndex >= 0)
            {
                return response.Substring(answerIndex + answerMarker.Length).TrimStart();
            }
            else
            {
                return "No answer found in the response.";
            }
        }
}
