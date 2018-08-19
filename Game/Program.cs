using Game.Items;
using Game.Items.Shields;
using Game.Items.Weapons;
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
            const ConsoleKey CreateNewHero = ConsoleKey.C;
            const ConsoleKey LoadSavedHeroKey = ConsoleKey.L;

            Console.Clear();
            Console.WriteLine("Menu");
            Console.WriteLine();
            Console.WriteLine($"{CreateNewHero} - Create a new hero");
            Console.WriteLine($"{LoadSavedHeroKey} - Load a saved hero");

            var pressedKey = KeyPresser.WaitFor(new[] { CreateNewHero, LoadSavedHeroKey });

            Unit hero;

            switch (pressedKey)
            {
                case CreateNewHero: hero = Program.CreateNewHero();
                    break;
                case LoadSavedHeroKey: hero = LoadSavedHero();
                    break;
                default:
                    throw new Exception();
            }

            var boris = new Unit("Boris the Brutal", Team.Enemy, new Sword(), new Buckler()) { Health = 15 };
            boris.AddPotion(new Potion());

            var units = new Unit[]
            {
                hero,
                new Unit("Steve the Shrew", Team.Enemy, new Sword()) { Health = 7 },
                new Unit("Gary the Goblin", Team.Enemy, new Sword()) { Health = 9 },
                boris
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
                    
                    if (attacker.Unit.HasPotions())
                    {
                        const ConsoleKey DrinkPotionKey = ConsoleKey.D;
                        const ConsoleKey AttackKey = ConsoleKey.A;

                        Console.WriteLine("Choose a turn");
                        Console.WriteLine($"Hit {DrinkPotionKey} to drink a potion. {attacker.Unit.Name}'s health: {attacker.Unit.Health}.");
                        Console.WriteLine($"Hit {AttackKey} to attack an enemy.");

                        var key = KeyPresser.WaitFor(new[] { DrinkPotionKey, AttackKey });
                        Console.WriteLine();

                        switch (key)
                        {
                            case DrinkPotionKey:
                                AttackerDrinksPotion(attacker.Unit);
                                break;
                            case AttackKey:
                                var enemies = battle.GetEnemies();

                                foreach (var enemy in enemies)
                                    Console.WriteLine($"Hit {enemy.Id} to attack {enemy.Unit.Name}. Health: {enemy.Unit.Health}.");

                                var targetIds = enemies.Select(x => Convert.ToChar(Convert.ToString(x.Id)));

                                var targetId = Convert.ToInt32(Convert.ToString(KeyPresser.WaitFor(targetIds)));

                                Console.WriteLine();

                                var defender = battle.GeUnitById(targetId);

                                AttackerAttacksDefender(attacker, defender, battle);
                                break;
                            default:
                                throw new Exception();
                        }
                    }
                    else
                    {
                        var enemies = battle.GetEnemies();

                        foreach (var enemy in enemies)
                            Console.WriteLine($"Hit {enemy.Id} to attack {enemy.Unit.Name}. Health: {enemy.Unit.Health}.");

                        var targetIds = enemies.Select(x => Convert.ToChar(Convert.ToString(x.Id)));

                        var targetId = Convert.ToInt32(Convert.ToString(KeyPresser.WaitFor(targetIds)));

                        Console.WriteLine();

                        var defender = battle.GeUnitById(targetId);

                        AttackerAttacksDefender(attacker, defender, battle);
                    }
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
                Console.WriteLine();
                Console.WriteLine("Game Saved.");
            }

            Console.ReadKey();
        }

        private static void AttackerAttacksDefender(BattleUnit attacker, BattleUnit defender, Battle battle)
        {
            var defenderHealthBeforeAttack = defender.Unit.Health;

            Console.WriteLine();

            var attack = attacker.Unit.Attack(defender.Unit);

            if (!attack.IsBlocked)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{attacker.Unit.Name} attacks {defender.Unit.Name} for {attack.Damage} damage.");
                Console.ResetColor();

                Console.WriteLine($"{defender.Unit.Name}'s health is reduced from {defenderHealthBeforeAttack} to {defender.Unit.Health}.");

                if (defender.Unit.IsDead())
                {
                    battle.RemoveUnit(defender);

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{defender.Unit.Name} has died.");
                    Console.ResetColor();

                    if (defender.Unit.Team == Team.Enemy)
                    {
                        if (defender.Unit.HasPotions())
                        {
                            const ConsoleKey PickUpPotionKey = ConsoleKey.P;

                            var numberOfPotions = defender.Unit.Potions.Count();

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"{defender.Unit.Name} has dropped {numberOfPotions} potion(s).");
                            Console.ResetColor();
                            Console.WriteLine($"Hit {PickUpPotionKey} to pick up.");
                            KeyPresser.WaitFor(PickUpPotionKey);
                            Console.WriteLine();

                            attacker.Unit.TakePotions(defender.Unit);

                            Console.WriteLine($"{attacker.Unit.Name} has picked up {numberOfPotions} potion(s).");
                            Console.WriteLine($"{attacker.Unit.Name} now has {numberOfPotions} potion(s).");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"{attacker.Unit.Name} attacks {defender.Unit.Name} for {attack.Damage} damage.");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{defender.Unit.Name} BLOCKS {attacker.Unit.Name}'s attack.");
                Console.ResetColor();
            }
        }

        private static void AttackerDrinksPotion(Unit attacker)
        {
            var result = attacker.DrinkPotion();

            if (result.DrankPotion)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{attacker.Name} drinks potion. Health increases by {result.HealAmount} from {result.HealthBeforeDrinkingPotion} to {result.HealthAfterDrinkingPotion}.");
                Console.ResetColor();
                Console.WriteLine($"{attacker.Name} has {attacker.Potions.Count()} potions left.");
            }
        }
    }
}