using System;

namespace ConsoleApp1
{
    public class ConsolePrinter
    {
        public static object PrintValue;

        public ConsolePrinter Value(string value)
        {
            PrintValue = value;
            return this;
        }

        public override string ToString()
        {
            Console.WriteLine(PrintValue);
            return null;
        }
    }
}
