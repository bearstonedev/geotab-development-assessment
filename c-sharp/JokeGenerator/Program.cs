using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace JokeGenerator
{
    class Program
    {
        static JokesService jokesService = new JokesService(new ApiService("https://api.chucknorris.io/jokes/"));
        static NamesService namesService = new NamesService(new ApiService("https://names.privserv.com/api/"));
        static string[] jokeCategories;

        private enum Answers
        {
            Yes,
            No,
            Help,
            Exit,
            None
        }

        public static void Main(string[] args)
        {
            PrintIntroduction();
            do
            {
                Console.WriteLine();
                var name = GetRandomName();
                var category = GetCategory();
                var numberOfJokes = GetNumberOfJokes();
                var jokes = GetJokes(numberOfJokes, category);
                jokes = ReplaceName(name, jokes);
                PrintResults(jokes);
            } while (PromptForInput("Would you like to generate more jokes?") == Answers.Yes);
        }

        public static void PrintIntroduction()
        {
            Answers response;
            do
            {
                response = PromptForInput(@"Welcome to the Joke Generator! This program generates jokes for you so you can spend your time being the life of the party ;) 
By default, we'll give you Chuck Norris jokes - but if you'd prefer, we'll give you the option to use a generated name instead.
Shall we get started?");
            } while (response == Answers.Help);

            if (response == Answers.No)
            {
                Environment.Exit(0);
            }
        }

        private static Answers PromptForInput(string message)
        {
            Console.WriteLine($"{message} (Hit \"y\" for \"yes\", \"n\" for \"no\", \"?\" for more detail, or \"ESC\" to exit.)");
            var response = GetInput();
            Console.WriteLine();

            if (response == Answers.Exit)
            {
                Environment.Exit(0);
            }

            return response;
        }

        private static Answers GetInput()
        {
            var input = Console.ReadKey();
            if (input.Key == ConsoleKey.Escape)
            {
                return Answers.Exit;
            }

            switch (input.KeyChar)
            {
                case 'Y':
                case 'y':
                    return Answers.Yes;
                case 'N':
                case 'n':
                    return Answers.No;
                default:
                    return Answers.Help;
            }
        }

        public static (string first, string last, Genders gender) GetRandomName()
        {
            var response = PromptForInput("Would you like to use a generated name?");

            while (response == Answers.Help)
            {
                response = PromptForInput(@"If you choose ""yes"", we'll use a generated name in the jokes we create for you.
                                        For example: if the joke would be ""Chuck Norris knows the wrong way to eat a reeses"",
                                        then we'll replace it with ""[generated name] knows the wrong way to eat a reeses.""
                                        So, would you like to use a generated name?");
            }

            if (response == Answers.No)
            {
                Console.WriteLine($"OK, we'll stick to the Chuck Norris jokes! {Environment.NewLine}");
                return ("", "", Genders.Unknown);
            }

            Console.WriteLine("OK, we'll generate one for you. This might take a second or two...");
            (string first, string last, Genders gender) = namesService.GetRandomName().Result;
            string fullName = $"{first} {last}";
            Console.WriteLine($"Got it! The name is: {fullName}. {Environment.NewLine}");
            return (first, last, gender);
        }

        public static string GetCategory()
        {
            var response = PromptForInput(@"Would you like to choose a specific category of joke? Type ""?"" to get a list of all categories.");
            while (response == Answers.Help)
            {
                GetCategoryList();
                var joinedCategories = string.Join(", ", jokeCategories);
                response = PromptForInput($"You can choose a specific category of joke. The categories are: {joinedCategories}. Would you like to choose a category?");
            }

            if (response == Answers.No)
            {
                Console.WriteLine($"OK, we'll choose from all of the categories! {Environment.NewLine}");
                return "";
            }

            GetCategoryList();
            Console.WriteLine("OK, go ahead and type your category.");
            var input = Console.ReadLine();

            while (!Array.Exists(jokeCategories, category => input == category))
            {
                var confirmation = PromptForInput("Sorry, that doesn't seem to match any of the categories. We need an exact match in order to proceed. Would you like to choose a category?");
                while (confirmation == Answers.Help)
                {
                    GetCategoryList();
                    Console.WriteLine($"You can choose a specific category of joke. The categories are: {jokeCategories}.");
                    confirmation = PromptForInput("Would you like to choose a category?");
                }

                if (confirmation == Answers.No)
                {
                    Console.WriteLine("OK, we'll choose from all of the categories!");
                    return "";
                }

                Console.WriteLine("OK, go ahead and type your category. Be sure to hit \"Enter\" when you're done!");
                input = Console.ReadLine();
            }

            Console.WriteLine($"Great! The category is {input}. {Environment.NewLine}");
            return input;
        }

        private static void GetCategoryList()
        {
            if (jokeCategories != null && jokeCategories.Length > 0)
            {
                return;
            }

            jokeCategories = jokesService.GetCategories().Result;
        }

        public static int GetNumberOfJokes()
        {
            Console.WriteLine("OK, we're about to generate your jokes. How many would you like? You can choose between one (1) and nine (9) jokes.");
            var keyInfo = Console.ReadKey();
            Console.WriteLine();

            while (!IsKeyBetweenOneAndNine(keyInfo))
            {
                Console.WriteLine("Please choose a number between 1 and 9.");
                keyInfo = Console.ReadKey();
            }

            var number = GetNumber(keyInfo);
            Console.WriteLine($"Great! We'll generate {number} jokes. {Environment.NewLine}");
            return number;
        }

        private static bool IsKeyBetweenOneAndNine(ConsoleKeyInfo keyInfo)
        {
            var number = ConvertCharToInt(keyInfo.KeyChar);
            return number >= 1 && number <= 9;
        }

        private static int ConvertCharToInt(char c)
        {
            return c - '0';
        }

        private static int GetNumber(ConsoleKeyInfo keyInfo)
        {
            return ConvertCharToInt(keyInfo.KeyChar);
        }

        public static string[] GetJokes(int numberOfJokes, string category)
        {
            Console.WriteLine("OK! We're generating your jokes now. This might take a moment.");
            return jokesService.GetRandomJokes(numberOfJokes, category).Result;
        }

        public static string[] ReplaceName((string first, string last, Genders gender) name, string[] jokes)
        {
            if (string.IsNullOrEmpty(name.first) || string.IsNullOrEmpty(name.last))
            {
                return jokes;
            }

            jokes = jokes.Select(joke => joke.Replace("Chuck", name.first, ignoreCase: true, CultureInfo.CurrentCulture)).ToArray();
            jokes = jokes.Select(joke => joke.Replace("Norris", name.last, ignoreCase: true, CultureInfo.CurrentCulture)).ToArray();
            if (name.gender == Genders.Female)
            {
                jokes = jokes.Select(joke => Regex.Replace(joke, @"\bhe\b", "she", RegexOptions.IgnoreCase)).ToArray();
                jokes = jokes.Select(joke => Regex.Replace(joke, @"\bhis\b", "her", RegexOptions.IgnoreCase)).ToArray();
                jokes = jokes.Select(joke => Regex.Replace(joke, @"\bhim\b", "her", RegexOptions.IgnoreCase)).ToArray();
            }

            return jokes;
        }

        public static void PrintResults(string[] jokes)
        {
            Console.WriteLine($"Here are your jokes! {Environment.NewLine}");

            foreach (var joke in jokes)
            {
                Console.WriteLine(joke);
            }

            Console.WriteLine();
        }
    }
}
