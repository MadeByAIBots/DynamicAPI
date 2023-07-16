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
	public class FileLineInsertIntegrationTests
	{
		[Test]
		public async Task TestFileLineInsertEndpoint()
		{
			using var context = new IntegrationTestContext();
			context.UseToken();

			// Set up
			var workingDirectory = Path.GetTempPath();
			var filePath = Path.GetRandomFileName();
			var lines = new[] { "First line", "Second line", "Third line" };
			await File.WriteAllLinesAsync(Path.Combine(workingDirectory, filePath), lines);

			var lineHash = DynamicApi.Utilities.Files.HashUtils.GenerateSimpleHash(lines[2]);

			// Exercise
			var response = await context.Client.PostAsync($"/file-line-insert", new StringContent("{ \"workingDirectory\": \"" + workingDirectory + "\", \"filePath\": \"" + filePath + "\",  \"beforeLineHash\": \"" + lineHash + "\",\"beforeLineNumber\": \"3\", \"newContent\": \"New line\" }", Encoding.UTF8, "application/json"));

			// Verify
			var expectedLines = new List<string>(lines);
			expectedLines.Insert(2, "New line");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var responseContent = await response.Content.ReadAsStringAsync();
			responseContent.Trim().Should().Be("New file content:\n\n" + expectedLines.ToArray().ToNumbered() + "\n\nLine inserted successfully");

			var updatedLines = await File.ReadAllLinesAsync(Path.Combine(workingDirectory, filePath));
			updatedLines[2].Should().Be("New line");

			// Teardown
			File.Delete(Path.Combine(workingDirectory, filePath));
		}
	}
}