using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JokeGenerator
{
    public class JokesService
    {
        private readonly ApiService api;

        private const string randomJokePath = "random";

        private const string categoriesPath = "categories";

        public JokesService(ApiService api)
        {
            Guard.NotNull(api, nameof(api));
            this.api = api;
        }

        public async Task<string[]> GetRandomJokes(int numberOfJokes = 1, string category = "")
        {
            var getJokesTaskList = new List<Task<string>>(Enumerable.Range(0, numberOfJokes).Select(_ => GetRandomJoke(category)));
            var aggregateTask = Task.WhenAll(getJokesTaskList.ToArray());
            return await aggregateTask;
        }

        private async Task<string> GetRandomJoke(string category)
        {
            var path = BuildRandomJokePath(category);
            var getJokeTask = api.Get<dynamic>(path);
            await getJokeTask;
            return ParseJoke(getJokeTask.Result);
        }

        private string BuildRandomJokePath(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return randomJokePath;
            }

            return $"{randomJokePath}?{nameof(category)}={category}";
        }

        private string ParseJoke(dynamic jokeResponse)
        {
            return jokeResponse.value;
        }

        public string[] GetCategories()
        {
            return api.Get<string[]>(JokesService.categoriesPath).Result;
        }
    }
}
