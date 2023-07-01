using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DynamicApiServer.Tests.Integration.Endpoints.Files
{
	public class DirectorySearchNamesIntegrationTests
	{
		[Test]
		public async Task TestDirectorySearchNamesEndpoint()
		{
			using var context = new IntegrationTestContext();
			context.UseToken();

			// Set up
			var uniqueId = Guid.NewGuid().ToString();
			var workingDirectory = Path.Combine(Path.GetTempPath(), uniqueId);
			Directory.CreateDirectory(workingDirectory);

			var searchPath = Guid.NewGuid().ToString();
			Directory.CreateDirectory(Path.Combine(workingDirectory, searchPath));


			Directory.CreateDirectory(Path.Combine(workingDirectory, searchPath, "MyDirectory"));
			var query = "MyDir*";

			// Exercise
			var payload = new { workingDirectory, searchPath, query };
			var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
			var response = await context.Client.PostAsync("/directory-search-names", content);

			// Verify
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			var responseContent = await response.Content.ReadAsStringAsync();
			responseContent.Trim().Should().Be(Path.Combine(searchPath, "MyDirectory"));

			// Teardown
			Directory.Delete(workingDirectory, true);
		}
	}
}