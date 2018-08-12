using Game.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter your name and press enter: ");
            var name = Console.ReadLine();

            var units = new Unit[]
            {
                new Unit(name, Team.Hero, new Sword()) { Health = 100 },
                new Unit("Steve the Shrew", Team.Enemy, new Sword()) { Health = 7 },
                new Unit("Gary the Goblin", Team.Enemy, new Sword()) { Health = 15 },
                new Unit("Boris the Brutal", Team.Enemy, new Sword()) { Health = 9 }
            };

            var battle = new Battle(units);

            Console.WriteLine();
            Console.WriteLine($"You are confronted by enemies.");

            do
            {
                var attacker = battle.DequeueAttacker();

                if (attacker.Unit.Team == Team.Hero)
                {
                    Console.WriteLine();

                    var enemies = battle.GetEnemies();

                    foreach (var enemy in enemies)
                    {
                        Console.WriteLine($"Hit {enemy.Id} to attack {enemy.Unit.Name}.");
                    }

                    var targetIds = enemies.Select(x => Convert.ToChar(Convert.ToString(x.Id)));

                    var targetId = Convert.ToInt32(Convert.ToString(WaitForKeyPress(targetIds)));

                    var target = battle.GetAttackerById(targetId);

                    var targetHealthBeforeAttack = target.Unit.Health;

                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine($"{attacker.Unit.Name} attacks {target.Unit.Name}.");
                    attacker.Unit.Attack(target.Unit);

                    Console.WriteLine($"{target.Unit.Name}'s health is reduced from {targetHealthBeforeAttack} to {target.Unit.Health}.");

                    if (target.Unit.IsDead())
                    {
                        battle.RemoveAttacker(target);

                        Console.WriteLine($"{target.Unit.Name} has died.");
                    }
                }
                else if (attacker.Unit.Team == Team.Enemy)
                {
                    var hero = battle.GetHero();

                    var heroHealthBeforeAttack = hero.Unit.Health;

                    Console.WriteLine();
                    Console.WriteLine($"{attacker.Unit.Name} attacks {hero.Unit.Name}.");
                    attacker.Unit.Attack(hero.Unit);

                    Console.WriteLine($"{hero.Unit.Name}'s health is reduced from {heroHealthBeforeAttack} to {hero.Unit.Health}.");

                    if (hero.Unit.IsDead())
                    {
                        Console.WriteLine($"{hero.Unit.Name} has died.");
                    }
                }
                else
                {
                    throw new Exception();
                }

                battle.QueueAttacker(attacker);
            }
            while (!battle.IsOver());

            Console.WriteLine();
            Console.WriteLine("The battle is over.");

            Console.ReadKey();
        }


        public static void WaitForKeyPress(ConsoleKey expectedKey)
        {
            while (Console.ReadKey().Key != expectedKey)
            {
            }
        }

        public static char WaitForKeyPress(IEnumerable<char> expectedKeys)
        {
            char key;

            do
            {
                key = Console.ReadKey().KeyChar;
            }
            while (!expectedKeys.Contains(key));

            return key;
        }
    }
}