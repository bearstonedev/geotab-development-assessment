using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;

namespace JokeGenerator.Tests
{
    public class JokeGeneratorApiTests
    {
        private readonly JokeGeneratorApi sut;
        private readonly Mock<HttpMessageHandler> mockMessageHandler;
        private const string norrisJokesBaseAddress = "https://api.chucknorris.io/jokes";

        public JokeGeneratorApiTests()
        {
            Console.WriteLine("Setting up tests.");
            this.mockMessageHandler = new Mock<HttpMessageHandler>();
            this.sut = new JokeGeneratorApi(mockMessageHandler.Object, norrisJokesBaseAddress);
        }

        [Fact]
        public void ShouldGetCategories()
        {
            this.SetupResponse(@"[
                ""some"",
                ""categories"",
                ""for"",
                ""testing"",
            ]");
            string[] expected = {
                "some",
                "categories",
                "for",
                "testing"
            };
            var actual = this.sut.GetCategories();
            this.VerifyRequest(1, $"{norrisJokesBaseAddress}/categories");
            Assert.Equal<string[]>(expected, actual);
        }

        [Fact]
        public void ShouldGetRandomJoke()
        {
            this.SetupResponse(@"{""value"":""An hilarious joke!""}");
            string[] expected = { "An hilarious joke!" };
            var actual = this.sut.GetRandomJoke();
            this.VerifyRequest(1, $"{norrisJokesBaseAddress}/random");
            Assert.Equal(expected, actual);
        }

        private void SetupResponse(string jsonResponseContent)
        {
            this.mockMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            ).ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponseContent)
            }).Verifiable();
        }

        private void VerifyRequest(int expectedCalls, string targetUrl)
        {
            this.mockMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(expectedCalls),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri.ToString() == targetUrl
                    && req.Headers.GetValues("Accept").FirstOrDefault() == "application/json"),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}