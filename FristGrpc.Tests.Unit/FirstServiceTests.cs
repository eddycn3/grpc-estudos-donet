using Basics;
using EstudoServiceGrpc.Services;
using FluentAssertions;
using FristGrpc.Tests.Unit.Helpers;

namespace FristGrpc.Tests.Unit
{
    public class FirstServiceTests
    {
        private readonly IFirstService firstService;
        public FirstServiceTests()
        {
            firstService = new FirstService();
        }

        [Fact]
        public async Task Unary_shouldReturn_an_Object()
        {
            // Arrange
            var request = new Request()
            {
                Content = "message"
            };

            var callContext = TestServerCallContext.Create();
            var expectedResponse = new Response()
            {
                Message = "message from server"
            };

            // Act
            var actualResponse = await firstService.Unary(request, callContext);

            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);

        }
    }
}
