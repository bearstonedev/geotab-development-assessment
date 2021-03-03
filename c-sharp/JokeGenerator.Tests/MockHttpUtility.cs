using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace JokeGenerator.Tests
{
    internal class MockHttpUtility
    {
        private readonly Mock<HttpMessageHandler> mockMessageHandler;

        internal HttpMessageHandler MockedHttpHandler
        {
            get => mockMessageHandler.Object;
        }

        internal MockHttpUtility()
        {
            this.mockMessageHandler = new Mock<HttpMessageHandler>();
        }

        internal void MockResponse<T>(T payload)
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

        internal void MockResponseSequence<T>(T[] payloadSequence)
        {
            var mockSequence = this.mockMessageHandler.Protected().SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
            foreach (T payload in payloadSequence)
            {
                var serialized = JsonConvert.SerializeObject(payload);
                mockSequence.ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(serialized)
                });
            }
        }

        internal void VerifyRequest(int expectedCalls, string targetUrl)
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