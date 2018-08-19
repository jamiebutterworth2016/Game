using Game.Items;
using Game.Items.Shields;
using Game.Items.Weapons;
using System;

namespace Game
{
    public static partial class Program
    {
        private static Unit CreateNewHero()
        {
            Console.Clear();
            Console.WriteLine("New Hero");
            Console.WriteLine();

            var gameSaver = _container.GetInstance<GameSaver>();

            string name;
            var validName = false;

            do
            {
                Console.Write("Enter the name of your hero and press enter: ");

                name = Console.ReadLine();

                validName = !gameSaver.SavedHeroExists(name);

                if (!validName)
                {
                    Console.WriteLine($"There is already a saved hero with the name {name}.");
                    Console.WriteLine();
                }
            }
            while (!validName);

            var hero = new Unit(name, Team.Hero, new Sword(), new Buckler()) { Health = 100 };

            const ConsoleKey SaveKey = ConsoleKey.S;
            const ConsoleKey DoNotSaveKey = ConsoleKey.N;

            Console.WriteLine();
            Console.WriteLine($"{SaveKey} - Save hero {hero.Name}");
            Console.WriteLine($"{DoNotSaveKey} - Do not save hero {hero.Name} (you can save later)");

            var key = KeyPresser.WaitFor(new[] { SaveKey, DoNotSaveKey });

            if (key == SaveKey)
            {
                gameSaver.SaveNew(hero);

                Console.WriteLine();
                Console.WriteLine($"Hero {hero.Name} has been saved.");
            }

            return hero;
        }
    }
}