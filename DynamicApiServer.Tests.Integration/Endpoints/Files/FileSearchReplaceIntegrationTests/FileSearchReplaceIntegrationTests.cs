using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using DynamicApi.Utilities.Files;

namespace DynamicApiServer.Tests.Integration.Endpoints.Files
{
    public class FileSearchReplaceIntegrationTests
    {
        [Test]
        public async Task TestFileSearchReplaceEndpoint()
        {
            using var context = new IntegrationTestContext();
            context.UseToken();

            // Set up
            var workingDirectory = Path.GetTempPath();
            var filePath = Path.GetRandomFileName();
            var initialContent = "Hello, world! This is a test string with special characters: $*^%#@";
            await File.WriteAllTextAsync(Path.Combine(workingDirectory, filePath), initialContent);
            var searchQuery = "This is a test string with special characters: $*^%#@";
            var replacementString = "Replacement string";

            // Exercise
            var response = await context.Client.PostAsync("/file-search-replace", new StringContent("{ \"workingDirectory\": \"" + workingDirectory + "\", \"filePath\": \"" + filePath + "\", \"searchQuery\": \"" + searchQuery + "\", \"replacementString\": \"" + replacementString + "\" }", Encoding.UTF8, "application/json"));

            // Verify
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
responseContent.Trim().Should().Contain("Search and replace operation completed successfully");
responseContent.Trim().Should().Contain("Replaced");
responseContent.Trim().Should().Contain("New file content:");
            var updatedContent = await File.ReadAllTextAsync(Path.Combine(workingDirectory, filePath));
            updatedContent.Should().NotContain(searchQuery);
            updatedContent.Should().Contain(replacementString);

            // Teardown
            File.Delete(Path.Combine(workingDirectory, filePath));
        }
    }
}
