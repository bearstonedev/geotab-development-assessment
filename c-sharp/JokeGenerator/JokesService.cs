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

        // public string[] GetRandomJokes(string firstname, string lastname, string category)
        // {
        //     string url = "jokes/random";
        //     if (category != null)
        //     {
        //         if (url.Contains('?'))
        //             url += "&";
        //         else url += "?";
        //         url += "category=";
        //         url += category;
        //     }

        //     string joke = Task.FromResult(client.GetStringAsync(url).Result).Result;

        //     if (firstname != null && lastname != null)
        //     {
        //         int index = joke.IndexOf("Chuck Norris");
        //         string firstPart = joke.Substring(0, index);
        //         string secondPart = joke.Substring(0 + index + "Chuck Norris".Length, joke.Length - (index + "Chuck Norris".Length));
        //         joke = firstPart + " " + firstname + " " + lastname + secondPart;
        //     }

        //     return new string[] { JsonConvert.DeserializeObject<dynamic>(joke).value };
        // }

        /// <summary>
        /// returns an object that contains name and surname
        /// </summary>
        /// <param name="client2"></param>
        /// <returns></returns>
		// public dynamic GetNames()
        // {
        //     var result = client.GetStringAsync("").Result;
        //     return JsonConvert.DeserializeObject<dynamic>(result);
        // }

        public string[] GetCategories()
        {
            return api.Get<string[]>(JokesService.categoriesPath).Result;
        }
    }
}
