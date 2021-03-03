using System;
using System.Threading.Tasks;

namespace JokeGenerator
{
    public enum Genders
    {
        Male,
        Female,
        Nonbinary,
        Agender,
        Bigender,
        Unknown,
    }

    public class NamesService
    {
        private readonly ApiService api;

        public NamesService(ApiService api)
        {
            Guard.NotNull(api, nameof(api));
            this.api = api;
        }

        public async Task<(string, string, Genders)> GetRandomName()
        {
            var getNamesTask = api.Get<dynamic>();
            await getNamesTask;
            (string first, string last) name = ParseFirstAndLastNames(getNamesTask.Result);
            Genders gender = ParseGender(getNamesTask.Result);
            return (name.first, name.last, gender);
        }

        private (string first, string last) ParseFirstAndLastNames(dynamic jsonResponse)
        {
            var first = jsonResponse.name;
            var last = jsonResponse.surname;
            return (first, last);
        }

        private Genders ParseGender(dynamic jsonResponse)
        {
            switch (((string)jsonResponse.gender).ToLower())
            {
                case "male":
                    return Genders.Male;
                case "female":
                    return Genders.Female;
                case "agender":
                    return Genders.Agender;
                case "bigender":
                    return Genders.Bigender;
                case "non-binary":
                    return Genders.Nonbinary;
                default:
                    return Genders.Unknown;
            }
        }
    }
}