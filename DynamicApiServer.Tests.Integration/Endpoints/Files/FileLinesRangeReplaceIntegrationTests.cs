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
	public class FileLinesRangeReplaceIntegrationTests
	{
		[Test]
		public async Task TestFileLinesRangeReplaceEndpoint()
		{
			using var context = new IntegrationTestContext();
			context.UseToken();

			// Set up
			var workingDirectory = Path.GetTempPath();
			var filePath = Path.GetRandomFileName();
			var initialLines = new string[] { "Line 1", "Line 2", "Line 3", "Line 4", "Line 5" };
			var initialContent = string.Join("\n", initialLines);
			await File.WriteAllTextAsync(Path.Combine(workingDirectory, filePath), initialContent);

			var startLineHash = HashUtils.GenerateSimpleHash("Line 2");
			var endLineHash = HashUtils.GenerateSimpleHash("Line 4");

			// Exercise
			var startLineNumber = 2;
			var endLineNumber = 4;
var newContents = "New Line 2\\nNew Line 3\\nNew Line 4";
			var response = await context.Client.PostAsync($"/file-lines-range-replace", new StringContent("{ \"workingDirectory\": \"" + workingDirectory + "\", \"filePath\": \"" + filePath + "\", \"startLineNumber\": \"" + startLineNumber + "\", \"endLineNumber\": \"" + endLineNumber + "\", \"startLineHash\": \"" + startLineHash + "\", \"endLineHash\": \"" + endLineHash + "\", \"newContents\": \"" + newContents + "\" }", Encoding.UTF8, "application/json"));

			// Verify
			var expectedLines = initialLines;
			expectedLines[1] = "New Line 2";
			expectedLines[2] = "New Line 3";
			expectedLines[3] = "New Line 4";

			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var responseContent = await response.Content.ReadAsStringAsync();
			responseContent.Trim().Should().Be("Line(s) replaced successfully\nNew file content:\n" + expectedLines.ToNumbered());

			var updatedContent = await File.ReadAllTextAsync(Path.Combine(workingDirectory, filePath));
			updatedContent.Trim().Should().Be(string.Join('\n', expectedLines));

			// Teardown
			File.Delete(Path.Combine(workingDirectory, filePath));
		}
	}
}
