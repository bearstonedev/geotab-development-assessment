using System;

namespace JokeGenerator
{
    class Program
    {
        static string[] results = new string[50];
        static char key;
        static Tuple<string, string> names;
        static ConsolePrinter printer = new ConsolePrinter();

        static void Main(string[] args)
        {
            printer.Value("Press ? to get instructions.").ToString();
            if (Console.ReadLine() == "?")
            {
                while (true)
                {
                    printer.Value("Press c to get categories").ToString();
                    printer.Value("Press r to get random jokes").ToString();
                    GetEnteredKey(Console.ReadKey());
                    if (key == 'c')
                    {
                        GetCategories();
                        PrintResults();
                    }
                    if (key == 'r')
                    {
                        printer.Value("Want to use a random name? y/n").ToString();
                        GetEnteredKey(Console.ReadKey());
                        if (key == 'y')
                            GetNames();
                        printer.Value("Want to specify a category? y/n").ToString();
                        if (key == 'y')
                        {
                            printer.Value("How many jokes do you want? (1-9)").ToString();
                            int n = Int32.Parse(Console.ReadLine());
                            printer.Value("Enter a category;").ToString();
                            GetRandomJokes(Console.ReadLine(), n);
                            PrintResults();
                        }
                        else
                        {
                            printer.Value("How many jokes do you want? (1-9)").ToString();
                            int n = Int32.Parse(Console.ReadLine());
                            GetRandomJokes(null, n);
                            PrintResults();
                        }
                    }
                    names = null;
                }
            }

        }

        private static void PrintResults()
        {
            printer.Value("[" + string.Join(",", results) + "]").ToString();
        }

        private static void GetEnteredKey(ConsoleKeyInfo consoleKeyInfo)
        {
            switch (consoleKeyInfo.Key)
            {
                case ConsoleKey.C:
                    key = 'c';
                    break;
                case ConsoleKey.D0:
                    key = '0';
                    break;
                case ConsoleKey.D1:
                    key = '1';
                    break;
                case ConsoleKey.D3:
                    key = '3';
                    break;
                case ConsoleKey.D4:
                    key = '4';
                    break;
                case ConsoleKey.D5:
                    key = '5';
                    break;
                case ConsoleKey.D6:
                    key = '6';
                    break;
                case ConsoleKey.D7:
                    key = '7';
                    break;
                case ConsoleKey.D8:
                    key = '8';
                    break;
                case ConsoleKey.D9:
                    key = '9';
                    break;
                case ConsoleKey.R:
                    key = 'r';
                    break;
                case ConsoleKey.Y:
                    key = 'y';
                    break;
            }
        }

        private static void GetRandomJokes(string category, int number)
        {
            results = new JokeGeneratorApi("https://api.chucknorris.io/jokes/").GetRandomJokes(names?.Item1, names?.Item2, category);
        }

        private static void GetCategories()
        {
            results = new JokeGeneratorApi("https://api.chucknorris.io/jokes/").GetCategories();
        }

        private static void GetNames()
        {
            dynamic result = new JokeGeneratorApi("https://www.names.privserv.com/api/").GetNames();
            names = Tuple.Create(result.name.ToString(), result.surname.ToString());
        }
    }
}
