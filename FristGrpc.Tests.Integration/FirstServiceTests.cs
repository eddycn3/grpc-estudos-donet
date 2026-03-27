using Basics;
using FluentAssertions;

namespace FristGrpc.Tests.Integration
{
    public class FirstServiceTests : IClassFixture<MyFactory<Program>>
    {
        private readonly MyFactory<Program> factory;

        public FirstServiceTests(MyFactory<Program> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public void GetUnaryMessage()
        {
            // Arrange
            var client = factory.CreateGrpcClient();
            var expectedResponse = new Response() { Message = "Hello message from server" };

            // Act
            var actualResponse = client.Unary(new Request() { Content = "message" });
            
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
