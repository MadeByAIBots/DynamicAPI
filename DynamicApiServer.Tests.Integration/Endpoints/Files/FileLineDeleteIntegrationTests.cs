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
	public class FileLineDeleteIntegrationTests
	{
		[Test]
		public async Task TestFileLineDeleteEndpoint()
		{
			// Setup
			var testContext = new IntegrationTestContext();
			testContext.UseToken();

			var tempDirectory = Path.GetTempPath();
			var randomFileName = Path.GetRandomFileName();
			var fileLines = new[] { "First line", "Second line", "Third line" };
			var fileFullPath = Path.Combine(tempDirectory, randomFileName);

			await File.WriteAllLinesAsync(fileFullPath, fileLines);

			var lineToDelete = fileLines[1];
			var lineHash = HashUtils.GenerateSimpleHash(lineToDelete);

			var requestContent = new StringContent(
				"{ \"workingDirectory\": \"" + tempDirectory +
				"\", \"filePath\": \"" + randomFileName +
				"\", \"lineNumber\": \"2\", \"lineHash\": \"" + lineHash + "\" }",
				Encoding.UTF8, "application/json");

			// Act
			var response = await testContext.Client.PostAsync("/file-line-delete", requestContent);

			var expectedLines = new[] { "First line", "Third line", "" };
			// Verify
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var responseContent = await response.Content.ReadAsStringAsync();
			responseContent.Trim().Should().StartWith("Line deleted successfully\nNew file content:\n" + expectedLines.ToNumbered().Trim());

			var updatedFileContent = await File.ReadAllTextAsync(fileFullPath);
			var expectedFileContent = string.Join('\n', expectedLines);
			updatedFileContent.Should().Be(expectedFileContent);

			// Teardown
			File.Delete(fileFullPath);
		}
	}
}