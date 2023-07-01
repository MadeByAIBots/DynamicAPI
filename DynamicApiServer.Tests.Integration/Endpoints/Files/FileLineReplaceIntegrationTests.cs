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
	public class FileLineReplaceIntegrationTests
	{
		[Test]
		public async Task TestFileLineReplaceEndpoint()
		{
			using var context = new IntegrationTestContext();
			context.UseToken();

			// Set up
			var workingDirectory = Path.GetTempPath();
			var filePath = Path.GetRandomFileName();
			var initialLines = new string[] { "Line 1", "Line 2", "Line 3" };
			var initialContent = string.Join("\n", initialLines);
			await File.WriteAllTextAsync(Path.Combine(workingDirectory, filePath), initialContent);

			var lineHash = HashUtils.GenerateSimpleHash("Line 2");

			// Exercise
			var lineNumber = 2;
			var newContent = "New Line 2";
			var response = await context.Client.PostAsync($"/file-line-replace", new StringContent("{ \"workingDirectory\": \"" + workingDirectory + "\", \"filePath\": \"" + filePath + "\", \"lineNumber\": \"" + lineNumber + "\",\"lineHash\": \"" + lineHash + "\", \"newContent\": \"" + newContent + "\" }", Encoding.UTF8, "application/json"));

			// Verify
			var expectedLines = initialLines;
			expectedLines[1] = newContent;

			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var responseContent = await response.Content.ReadAsStringAsync();
			responseContent.Trim().Should().Be("Line replaced successfully\nNew file content:\n" + expectedLines.ToNumbered());

			var updatedContent = await File.ReadAllTextAsync(Path.Combine(workingDirectory, filePath));
			updatedContent.Trim().Should().Be(string.Join('\n', expectedLines));

			// Teardown
			File.Delete(Path.Combine(workingDirectory, filePath));
		}

		[Test]
		public async Task TestFileLineReplaceEndpoint_IncorrectHash()
		{
			using var context = new IntegrationTestContext();
			context.UseToken();

			// Set up
			var workingDirectory = Path.GetTempPath();
			var filePath = Path.GetRandomFileName();
			var initialContent = "Line 1\nLine 2\nLine 3";
			await File.WriteAllTextAsync(Path.Combine(workingDirectory, filePath), initialContent);

			var lineHash = "INVALID";

			// Exercise
			var lineNumber = 2;
			var newContent = "New Line 2";
			var response = await context.Client.PostAsync($"/file-line-replace", new StringContent("{ \"workingDirectory\": \"" + workingDirectory + "\", \"filePath\": \"" + filePath + "\", \"lineNumber\": \"" + lineNumber + "\",\"lineHash\": \"" + lineHash + "\", \"newContent\": \"" + newContent + "\" }", Encoding.UTF8, "application/json"));

			// Verify
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var responseContent = await response.Content.ReadAsStringAsync();
			responseContent.Trim().Should().Be("Error: Invalid hash. Read the lines to find out the correct hash and line number.");

			//var updatedContent = await File.ReadAllTextAsync(Path.Combine(workingDirectory, filePath));
			//updatedContent.Trim().Should().Be("Line 1\n" + newContent + "\nLine 3");

			// Teardown
			File.Delete(Path.Combine(workingDirectory, filePath));
		}
	}
}