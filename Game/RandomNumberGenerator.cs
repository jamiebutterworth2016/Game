using System;

namespace Game
{
    public static class RandomNumberGenerator
    {
        private static readonly Random random = new Random();

        private static readonly object syncLock = new object();

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
    }
}