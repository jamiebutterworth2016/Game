using System;
using System.Linq;

namespace Game
{
    public static partial class Program
    {
        private static Unit LoadSavedHero()
        {
            var gameSaver = _container.GetInstance<GameSaver>();

            var savedHeroes = gameSaver.GetSavedHeroes();

            Console.Clear();
            Console.WriteLine("Load Hero");
            Console.WriteLine();
            Console.WriteLine($"Enter the id of the hero and press enter.");

            foreach (var savedHero in savedHeroes)
                Console.WriteLine($"{savedHero.Id} - {savedHero.Hero.Name}");

            var ids = savedHeroes.Select(x => x.Id);

            var id = KeyPresser.WaitFor(ids);

            var heroName = savedHeroes.Single(x => x.Id == id).Hero.Name;

            var hero = gameSaver.GetSavedHero(heroName);

            return hero;
        }
    }
}