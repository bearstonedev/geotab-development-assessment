using System;
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
        private const string categories = @"[""animal"",""career"",""celebrity"",""dev"",""explicit"",""fashion"",""food"",""history"",""money"",""movie"",""music"",""political"",""religion"",""science"",""sport"",""travel""]";

        public JokeGeneratorApiTests()
        {
            Console.WriteLine("Setting up tests.");
            this.CreateSystemUnderTest();
        }

        [Fact]
        public void ShouldGetCategories()
        {
            var expected = new string[] { categories };
            var actual = this.sut.GetCategories();
            Assert.Equal<string[]>(expected, actual);
        }

        private void CreateSystemUnderTest()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(categories)
            }).Verifiable();
            this.mockMessageHandler = mockMessageHandler;
            this.sut = new JokeGeneratorApi(mockMessageHandler.Object, "https://api.chucknorris.io");
        }
    }
}