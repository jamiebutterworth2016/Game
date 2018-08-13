using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public static class KeyPresser
    {
        public static void WaitFor(ConsoleKey expectedKey)
        {
            while (Console.ReadKey().Key != expectedKey)
            {
            }
        }

        public static ConsoleKey WaitFor(IEnumerable<ConsoleKey> expectedKeys)
        {
            ConsoleKey key;

            do
            {
                key = Console.ReadKey().Key;
            }
            while (!expectedKeys.Contains(key));

            return key;
        }

        public static char WaitFor(IEnumerable<char> expectedKeys)
        {
            char key;

            do
            {
                key = Console.ReadKey().KeyChar;
            }
            while (!expectedKeys.Contains(key));

            return key;
        }

        public static int WaitFor(IEnumerable<int> expectedInts)
        {
            while (true)
            {
                var input = Console.ReadLine();

                if (int.TryParse(input, out var parsedInt) && expectedInts.Contains(parsedInt))
                {
                    return parsedInt;
                }
            }
        }
    }
}