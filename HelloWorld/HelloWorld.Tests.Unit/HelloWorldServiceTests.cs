using FluentAssertions;
using NUnit.Framework;
using HelloWorldLibrary;

namespace HelloWorld.Tests.Unit
{
    public class HelloWorldServiceTests
    {
        [Test]
        public void GetMessage_ReturnsHelloWorld()
        {
            // Arrange
            var service = new HelloWorldService();

            // Act
            var message = service.GetMessage();

            // Assert
            message.Should().Be("Hello World");
        }
    }
}