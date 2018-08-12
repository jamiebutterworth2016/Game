using System;

namespace Game
{
    public class Die
    {
        private readonly Random _random;

        public Die()
        {
            _random = new Random();
        }

        public int Roll()
        {
            var side = _random.Next(1, 7);

            return side;
        }
    }
}