using Game.Items;
using SimpleInjector;
using System;
using System.Linq;

namespace Game
{
    public static partial class Program
    {
        private static readonly Container _container;

        static Program()
        {
            _container = new Container();
            _container.Register<GameSaver>(Lifestyle.Transient);
            _container.Verify();
        }

        public static void Main(string[] args)
        {
            const ConsoleKey NewHeroKey = ConsoleKey.N;
            const ConsoleKey LoadHeroKey = ConsoleKey.L;

            Console.Clear();
            Console.WriteLine("Menu");
            Console.WriteLine();
            Console.WriteLine($"{NewHeroKey} - Create a new hero");
            Console.WriteLine($"{LoadHeroKey} - Load a saved hero");

            var pressedKey = KeyPresser.WaitFor(new[] { NewHeroKey, LoadHeroKey });

            Unit hero;

            switch (pressedKey)
            {
                case NewHeroKey: hero = CreateNewHero();
                    break;
                case LoadHeroKey: hero = LoadSavedHero();
                    break;
                default:
                    throw new Exception();
            }

            var units = new Unit[]
            {
                hero,
                new Unit("Steve the Shrew", Team.Enemy, new Sword()) { Health = 7 },
                new Unit("Gary the Goblin", Team.Enemy, new Sword()) { Health = 15 },
                new Unit("Boris the Brutal", Team.Enemy, new Sword()) { Health = 9 }
            };

            var battle = new Battle(units);

            Console.Clear();
            Console.WriteLine($"You are confronted by enemies.");

            do
            {
                var attacker = battle.DequeueUnit();

                if (attacker.Unit.Team == Team.Hero)
                {
                    Console.WriteLine();

                    var enemies = battle.GetEnemies();

                    foreach (var enemy in enemies)
                        Console.WriteLine($"Hit {enemy.Id} to attack {enemy.Unit.Name}.");

                    var targetIds = enemies.Select(x => Convert.ToChar(Convert.ToString(x.Id)));

                    var targetId = Convert.ToInt32(Convert.ToString(KeyPresser.WaitFor(targetIds)));

                    var defender = battle.GeUnitById(targetId);

                    AttackerAttacksDefender(attacker, defender, battle);
                }
                else if (attacker.Unit.Team == Team.Enemy)
                {
                    AttackerAttacksDefender(attacker, battle.GetHero(), battle);
                }
                else throw new Exception();

                battle.QueueUnit(attacker);
            }
            while (!battle.IsOver());

            Console.WriteLine();
            Console.WriteLine("The battle is over.");

            if (hero.IsAlive())
            {
                Console.WriteLine();
                Console.WriteLine("S - Save");
                KeyPresser.WaitFor(ConsoleKey.S);

                var gameSaver = _container.GetInstance<GameSaver>();

                gameSaver.OverwriteSave(hero);

                Console.WriteLine();
                Console.WriteLine("Game Saved.");
            }

            Console.ReadKey();
        }

        private static void AttackerAttacksDefender(BattleUnit attacker, BattleUnit defender, Battle battle)
        {
            var defenderHealthBeforeAttack = defender.Unit.Health;

            Console.WriteLine();
            Console.WriteLine($"{attacker.Unit.Name} attacks {defender.Unit.Name}.");

            attacker.Unit.Attack(defender.Unit);

            Console.WriteLine($"{defender.Unit.Name}'s health is reduced from {defenderHealthBeforeAttack} to {defender.Unit.Health}.");

            if (defender.Unit.IsDead())
            {
                battle.RemoveUnit(defender);

                Console.WriteLine($"{defender.Unit.Name} has died.");
            }
        }
    }
}