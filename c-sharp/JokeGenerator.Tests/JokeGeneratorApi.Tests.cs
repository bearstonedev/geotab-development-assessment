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
        private JokeGeneratorApi sut;
        private Mock<HttpMessageHandler> mockMessageHandler;
        private const string baseAddress = "https://api.chucknorris.io";
        private readonly string[] categories = {
            "some",
            "categories",
            "for",
            "testing"
        };

        public JokeGeneratorApiTests()
        {
            Console.WriteLine("Setting up tests.");
            this.CreateSystemUnderTest(baseAddress);
        }

        [Fact]
        public void ShouldGetCategories()
        {
            var expected = categories;
            var actual = this.sut.GetCategories();
            Assert.Equal<string[]>(expected, actual);
            this.mockMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri.ToString() == $"{baseAddress}/jokes/categories"
                    && req.Headers.GetValues("Accept").FirstOrDefault() == "application/json"),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        private void CreateSystemUnderTest(string baseAddress)
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"[
                        ""some"",
                        ""categories"",
                        ""for"",
                        ""testing"",
                    ]")
                }).Verifiable();
            this.mockMessageHandler = mockMessageHandler;
            this.sut = new JokeGeneratorApi(mockMessageHandler.Object, baseAddress);
        }
    }
}