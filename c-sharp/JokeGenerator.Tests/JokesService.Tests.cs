using System.Linq;
using Xunit;

namespace JokeGenerator.Tests
{
    public class JokesServiceTests
    {
        private readonly JokesService sut;
        private readonly MockHttpUtility mockHttpUtil;
        private const string baseAddress = "https://baseaddress.com";

        public JokesServiceTests()
        {
            this.mockHttpUtil = new MockHttpUtility();
            this.sut = CreateSystemUnderTest(mockHttpUtil);
        }

        private JokesService CreateSystemUnderTest(MockHttpUtility mockUtil)
        {
            var api = new ApiService(mockUtil.MockedHttpHandler, baseAddress);
            return new JokesService(api);
        }

        [Fact]
        public async void ShouldGetCategories()
        {
            string[] expected = {
                "some",
                "categories",
                "for",
                "testing"
            };
            this.mockHttpUtil.MockResponse(expected);
            var actual = await this.sut.GetCategories();
            this.mockHttpUtil.VerifyRequest(1, $"{baseAddress}/categories");
            Assert.Equal<string[]>(expected, actual);
        }

        [Fact]
        public async void ShouldGetOneRandomJoke()
        {
            string[] expected = { "An hilarious joke!" };
            this.mockHttpUtil.MockResponseSequence(expected.Select(joke => new { value = joke }).ToArray());
            var actual = await this.sut.GetRandomJokes();
            this.mockHttpUtil.VerifyRequest(1, $"{baseAddress}/random");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void ShouldGetManyRandomJokes()
        {
            string[] expected = Enumerable.Range(0, 9).Select(_ => "An hilarious joke!").ToArray();
            this.mockHttpUtil.MockResponseSequence(expected.Select(joke => new { value = joke }).ToArray());
            var actual = await this.sut.GetRandomJokes(9);
            this.mockHttpUtil.VerifyRequest(9, $"{baseAddress}/random");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void ShouldGetRandomJokesFromGivenCategory()
        {
            string[] expected = { "An hilarious joke!", "Another hilarious joke!" };
            this.mockHttpUtil.MockResponseSequence(expected.Select(joke => new { value = joke }).ToArray());
            var actual = await this.sut.GetRandomJokes(2, "theCategory");
            this.mockHttpUtil.VerifyRequest(2, $"{baseAddress}/random?category=theCategory");
            Assert.Equal(expected, actual);
        }
    }
}