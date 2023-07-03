using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DynamicApiServer.Tests.Integration.Endpoints.Files
{
    public class FileLinesRangeReplaceIntegrationTests
    {
        private IntegrationTestContext _context;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _context = new IntegrationTestContext();
            _client = _context.Client;
        }

        [Test]
        public async Task TestFileLinesRangeReplaceEndpoint()
        {
            var startLineNumber = 2;
            _context.UseToken();
            var endLineNumber = 4;
var newContents = "New Line 2\nNew Line 3\nNew Line 4";

            var lines = new List<string> { "Line 1", "Line 2", "Line 3", "Line 4", "Line 5" };
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, string.Join("\n", lines));

            var startLineHash = lines[startLineNumber - 1].GetHashCode().ToString();
            var endLineHash = lines[endLineNumber - 1].GetHashCode().ToString();

            var request = new
            {
                workingDirectory = Path.GetDirectoryName(tempFile),
                filePath = Path.GetFileName(tempFile),
                startLineNumber = startLineNumber.ToString(),
                endLineNumber = endLineNumber.ToString(),
                startLineHash = startLineHash,
                endLineHash = endLineHash,
                newContents = newContents
            };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response = await _context.Client.PostAsync("/file-lines-range-replace", content);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var fileContents = File.ReadAllText(tempFile);
            var fileLines = fileContents.Split(new[] { '\n' }, StringSplitOptions.None);
            var replacedLines = string.Join("\n", fileLines[startLineNumber - 1..endLineNumber]);

            Assert.AreEqual(newContents, replacedLines);
        }
    }
}
