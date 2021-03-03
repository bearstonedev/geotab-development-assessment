using System.Threading.Tasks;

namespace JokeGenerator
{
    public class NamesService
    {
        private readonly ApiService api;

        public NamesService(ApiService api)
        {
            Guard.NotNull(api, nameof(api));
            this.api = api;
        }

        public async Task<(string, string)> GetRandomName()
        {
            var getNamesTask = api.Get<dynamic>();
            await getNamesTask;
            return ParseFirstAndLastNames(getNamesTask.Result);
        }

        private (string first, string last) ParseFirstAndLastNames(dynamic jsonResponse)
        {
            var first = jsonResponse.name;
            var last = jsonResponse.surname;
            return (first, last);
        }
    }
}