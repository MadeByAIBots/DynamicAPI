using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class InjectableStaticFileHandler
{
    private readonly ApiConfiguration _configuration;
    private readonly ILogger<InjectableStaticFileHandler> _logger;
    private readonly WorkingDirectoryResolver _resolver;

    public InjectableStaticFileHandler(ApiConfiguration configuration, WorkingDirectoryResolver resolver, ILogger<InjectableStaticFileHandler> logger)
    {
        _configuration = configuration;
        _resolver = resolver;
        _logger = logger;
    }

    public bool IsStaticFileRequest(HttpRequest request)
    {
        var isStaticFile = _configuration.StaticFilePaths.Contains(request.Path.Value);
        _logger.LogInformation($"Request path: {request.Path.Value}, Is static file: {isStaticFile}");
        return isStaticFile;
    }

    public async Task<string> ReadFileAsync(string path)
    {
        var wwwrootPath = GetWwwRootPath();
        var filePath = Path.Combine(wwwrootPath, path.TrimStart('/'));
        _logger.LogInformation($"Reading file: {filePath}");
        var fileContent = await File.ReadAllTextAsync(filePath);
        _logger.LogInformation($"Finished reading file: {filePath}");
        return fileContent;
    }

    private string GetWwwRootPath()
    {
        var wwwrootPath1 = Path.Combine(_resolver.WorkingDirectory(), "wwwroot");
        var wwwrootPath2 = Path.Combine(_resolver.WorkingDirectory(), "DynamicApiServer/wwwroot");

        if (Directory.Exists(wwwrootPath1))
            return wwwrootPath1;
        if (Directory.Exists(wwwrootPath2))
            return wwwrootPath2;

        throw new DirectoryNotFoundException("Failed to find wwwroot directory");
    }

    public string ModifyContent(string content)
    {
        var modifiedContent = content;
        
        modifiedContent = InjectConfigValue(modifiedContent, "OPEN_AI_VERIFICATION_TOKEN", _configuration.OpenAIVerificationToken);
        modifiedContent = InjectConfigValue(modifiedContent, "SERVER_URL", _configuration.ExternalUrl);
        
        _logger.LogInformation("Modified content with configuration values");
        
        return modifiedContent;
    }

    public string InjectConfigValue(string originalContent, string key, string value)
    {
        var modifiedContent = originalContent.Replace("{{" + key + "}}", value);
        _logger.LogInformation($"Injected value for key: {key}");
        return modifiedContent;
    }

    public async Task HandleRequest(HttpContext context)
    {
        _logger.LogInformation($"Handling request for path: {context.Request.Path.Value}");
        if (IsStaticFileRequest(context.Request))
        {
            var fileContent = await ReadFileAsync(context.Request.Path);
            var modifiedContent = ModifyContent(fileContent);
            await context.Response.WriteAsync(modifiedContent);
            _logger.LogInformation($"Handled request for static file: {context.Request.Path.Value}");
        }
    }
}