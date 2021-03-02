using System;
using System.Diagnostics;

namespace JokeGenerator
{
    public static class Guard
    {
        [DebuggerStepThrough]
        public static void NotNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}