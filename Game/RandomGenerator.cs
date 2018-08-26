using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public static class RandomGenerator
    {
        private static readonly Random random = new Random();

        private static readonly object syncLock = new object();

        public static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
                return random.Next(min, max);
        }

        public static decimal GetRandomDecimal(decimal min, decimal max)
        {
            var minAsInt = Convert.ToInt32(min * 100);
            var maxAsInt = Convert.ToInt32(max * 100);

            lock (syncLock)
            {
                var randomNumber = random.Next(minAsInt, maxAsInt);
                return Convert.ToDecimal(randomNumber / 100);
            }
        }

        public static T GetRandomItem<T>(IEnumerable<T> enumerable)
        {
            lock (syncLock)
            {
                var array = enumerable.ToArray();

                var randomItemIndex = random.Next(array.Count());

                return array[randomItemIndex];
            }
        }
    }
}