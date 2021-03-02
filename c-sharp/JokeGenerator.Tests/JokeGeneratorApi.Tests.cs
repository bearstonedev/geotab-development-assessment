using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
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
            this.mockMessageHandler = new Mock<HttpMessageHandler>();
            this.sut = new JokeGeneratorApi(mockMessageHandler.Object, norrisJokesBaseAddress);
        }

        [Fact]
        public void ShouldGetCategories()
        {
            string[] expected = {
                "some",
                "categories",
                "for",
                "testing"
            };
            this.MockResponse(expected);
            var actual = this.sut.GetCategories();
            this.VerifyRequest(1, $"{norrisJokesBaseAddress}/categories");
            Assert.Equal<string[]>(expected, actual);
        }

        [Fact]
        public async void ShouldGetOneRandomJoke()
        {
            string[] expected = { "An hilarious joke!" };
            this.MockResponseSequence(expected);
            var actual = await this.sut.GetRandomJokes();
            this.VerifyRequest(1, $"{norrisJokesBaseAddress}/random");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void ShouldGetManyRandomJokes()
        {
            const string hilariousJoke = "An hilarious joke!";
            string[] expected = Enumerable.Range(0, 9).Select(_ => hilariousJoke).ToArray();
            this.MockResponseSequence(expected);
            var actual = await this.sut.GetRandomJokes(9);
            this.VerifyRequest(9, $"{norrisJokesBaseAddress}/random");
            Assert.Equal(expected, actual);
        }

        private void MockResponse<T>(T payload)
        {
            this.mockMessageHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            ).ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(payload))
            }).Verifiable();
        }

        private void MockResponseSequence<T>(T[] payloadSequence)
        {
            var mockSequence = this.mockMessageHandler.Protected().SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
            foreach (T payload in payloadSequence)
            {
                var serialized = JsonConvert.SerializeObject(new { value = payload });
                mockSequence.ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(serialized)
                });
            }
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