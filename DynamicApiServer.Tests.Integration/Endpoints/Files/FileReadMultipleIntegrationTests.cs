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
	public class FileReadMultipleIntegrationTests
	{
		[Test]
		public async Task TestFileReadMultipleEndpoint()
		{
			using var context = new IntegrationTestContext();
			context.UseToken();

			// Set up
			var workingDirectory = Path.GetTempPath();
			var filePaths = new[] { Path.GetRandomFileName(), Path.GetRandomFileName(), Path.GetRandomFileName() };

			var fileContents = new[] { "Contents of file1", "Contents of file2", "Contents of file3" };
			for (int i = 0; i < filePaths.Length; i++)
			{
				Console.WriteLine($"Attempting to create file at {Path.Combine(workingDirectory, filePaths[i])}");
				await File.WriteAllTextAsync(Path.Combine(workingDirectory, filePaths[i]), fileContents[i]);
			}
			Console.WriteLine($"Created files at {string.Join(", ", filePaths)} with contents {string.Join(", ", fileContents)}");

			var expectedResponseBuilder = new StringBuilder();
			for (int i = 0; i < filePaths.Length; i++)
			{
				expectedResponseBuilder.AppendLine($"[{filePaths[i]}]");
				expectedResponseBuilder.AppendLine(File.ReadAllText(Path.Combine(workingDirectory, filePaths[i])));
				expectedResponseBuilder.AppendLine();
				expectedResponseBuilder.AppendLine();
			}

			// Exercise
			var requestContent = new StringContent("{ \"workingDirectory\": \"" + workingDirectory + "\", \"filePaths\": \"" + string.Join(";", filePaths) + "\" }", Encoding.UTF8, "application/json");
			Console.WriteLine($"Sending request to /file-read-multiple with content {await requestContent.ReadAsStringAsync()}");
			var response = await context.Client.PostAsync("/file-read-multiple", requestContent);

			// Verify
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var responseContent = await response.Content.ReadAsStringAsync();

			Console.WriteLine($"Received response with status code {response.StatusCode} and content {responseContent}");
			responseContent.Trim().Should().Be(expectedResponseBuilder.ToString().Trim());

			// Teardown
			foreach (var filePath in filePaths)
			{
				File.Delete(Path.Combine(workingDirectory, filePath));
			}
			Console.WriteLine($"Deleted files at {string.Join(", ", filePaths)}");
		}
	}
}
