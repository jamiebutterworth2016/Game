using System;
using System.Linq;

namespace Game
{
    public class RandomFactory
    {
        public T Create<T>()
        {
            var typesOfT = typeof(T).Assembly.GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(T)))
                .Select(x => (T)Activator.CreateInstance(x));

            var typeOfT = RandomGenerator.GetRandomItem(typesOfT);

            return typeOfT;
        }
    }
}