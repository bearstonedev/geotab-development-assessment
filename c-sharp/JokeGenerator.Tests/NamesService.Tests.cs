using Xunit;

namespace JokeGenerator.Tests
{
    public class NamesServiceTests
    {
        private readonly NamesService sut;
        private readonly MockHttpUtility mockUtil;
        private const string baseAddress = "https://getsomerandomnames.com/api/";

        public NamesServiceTests()
        {
            this.mockUtil = new MockHttpUtility();
            this.sut = CreateSystemUnderTest(mockUtil);
        }

        private NamesService CreateSystemUnderTest(MockHttpUtility mockUtil)
        {
            var namesService = new ApiService(this.mockUtil.MockedHttpHandler, baseAddress);
            return new NamesService(namesService);
        }

        [Fact]
        public async void ShouldGetNames()
        {
            (string first, string last, Genders gender) expected = ("firstname", "lastname", Genders.Agender);
            this.mockUtil.MockResponse(new { name = expected.first, surname = expected.last, gender = expected.gender.ToString() });
            var actual = await this.sut.GetRandomName();
            this.mockUtil.VerifyRequest(1, $"{baseAddress}");
            Assert.Equal<(string, string, Genders)>(expected, actual);
        }
    }
}