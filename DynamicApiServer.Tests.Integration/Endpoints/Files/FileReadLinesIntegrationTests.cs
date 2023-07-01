using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using System.Security.Cryptography;
using DynamicApi.Utilities.Files;

namespace DynamicApiServer.Tests.Integration.Endpoints.Files
{
	public class FileReadLinesIntegrationTests
	{
		[Test]
		public async Task TestFileReadLinesEndpoint()
		{
			using var context = new IntegrationTestContext();
			context.UseToken();

			// Set up
			var workingDirectory = Path.GetTempPath();
			var filePath = Path.GetRandomFileName();

			var lines = new[] { "First line", "Second line", "Modified third line" };
			var hashes = lines.Select(HashUtils.GenerateSimpleHash).ToArray();
			var expectedOutput = string.Join(Environment.NewLine, lines.ToNumbered());
			await File.WriteAllLinesAsync(Path.Combine(workingDirectory, filePath), lines);

			// Exercise
			var response = await context.Client.PostAsync($"/file-read-lines", new StringContent("{ \"workingDirectory\": \"" + workingDirectory + "\", \"filePath\": \"" + filePath + "\" }", Encoding.UTF8, "application/json"));


			// Verify
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var responseContent = await response.Content.ReadAsStringAsync();
			responseContent.Trim().Should().Be(expectedOutput);

			// Teardown
			File.Delete(Path.Combine(workingDirectory, filePath));
		}
	}
}