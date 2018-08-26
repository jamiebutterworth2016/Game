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
            _container.Register<UnitFactory>(Lifestyle.Transient);
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

            var unitFactory = _container.GetInstance<UnitFactory>();

            var units = new Unit[]
            {
                hero,
                unitFactory.Create("Boris the Brutal"),
                unitFactory.Create("Steve the Shrew"),
                unitFactory.Create("Gary the Goblin")
            };

            var battle = new Battle(units);

            Console.Clear();
            Console.WriteLine($"You are confronted by enemies.");
            Console.WriteLine();

            do
            {
                var attacker = battle.DequeueUnit();

                if (attacker.Unit.Team == Team.Hero)
                {                    
                    if (attacker.Unit.HasPotions())
                    {
                        const ConsoleKey DrinkPotionKey = ConsoleKey.D;
                        const ConsoleKey AttackKey = ConsoleKey.A;

                        Console.WriteLine("Choose a turn");
                        Console.WriteLine($"Hit {DrinkPotionKey} to drink a potion. {attacker.Unit.Name}'s health: {attacker.Unit.Health}.");
                        Console.WriteLine($"Hit {AttackKey} to attack an enemy.");
                        Console.WriteLine();

                        var key = KeyPresser.WaitFor(new[] { DrinkPotionKey, AttackKey });
                        Console.WriteLine();

                        switch (key)
                        {
                            case DrinkPotionKey:
                                AttackerDrinksPotion(attacker.Unit);
                                break;
                            case AttackKey:
                                var enemy = GetDefenderToAttack(battle);
                                AttackerAttacksDefender(attacker, enemy, battle);
                                break;
                            default:
                                throw new Exception();
                        }
                    }
                    else
                    {
                        var enemy = GetDefenderToAttack(battle);
                        AttackerAttacksDefender(attacker, enemy, battle);
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

        private static BattleUnit GetDefenderToAttack(Battle battle)
        {
            var enemies = battle.GetEnemies();

            foreach (var enemy in enemies)
                Console.WriteLine($"Hit {enemy.Id} to attack {enemy.Unit.Name}. {enemy.Unit.Weapon.GetType().Name}, {enemy.Unit.Shield.GetType().Name}. Health: {enemy.Unit.Health}.");

            var targetIds = enemies.Select(x => Convert.ToChar(Convert.ToString(x.Id)));

            var targetId = Convert.ToInt32(Convert.ToString(KeyPresser.WaitFor(targetIds)));

            Console.WriteLine();

            var defender = battle.GetUnitById(targetId);

            return defender;
        }

        private static void AttackerAttacksDefender(BattleUnit attacker, BattleUnit defender, Battle battle)
        {
            var defenderHealthBeforeAttack = defender.Unit.Health;

            var attack = attacker.Unit.Attack(defender.Unit);

            if (!attack.IsBlocked)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{attacker.Unit.Name} attacks {defender.Unit.Name} for {attack.Damage} damage.");
                Console.ResetColor();

                Console.WriteLine($"{defender.Unit.Name}'s health is reduced from {defenderHealthBeforeAttack} to {defender.Unit.Health}.");
                Console.WriteLine();

                if (defender.Unit.IsDead())
                {
                    battle.RemoveUnit(defender);

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{defender.Unit.Name} has died.");
                    Console.ResetColor();
                    Console.WriteLine();

                    if (defender.Unit.Team == Team.Enemy)
                    {
                        const ConsoleKey PickUpKey = ConsoleKey.P;

                        if (defender.Unit.HasPotions())
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"{defender.Unit.Name} has dropped {defender.Unit.Potions.Count()} potion(s).");
                            Console.ResetColor();

                            Console.WriteLine($"Hit {PickUpKey} to pick up.");
                            KeyPresser.WaitFor(PickUpKey);
                            Console.WriteLine();

                            var numberOfPotions = defender.Unit.Potions.Count();

                            attacker.Unit.TakePotions(defender.Unit);

                            Console.WriteLine($"{attacker.Unit.Name} has picked up {numberOfPotions} potion(s).");
                            Console.WriteLine($"{attacker.Unit.Name} now has {attacker.Unit.Potions.Count()} potion(s).");
                            Console.WriteLine();
                        }

                        if (defender.Unit.HasGold())
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"{defender.Unit.Name} has dropped {defender.Unit.Gold} gold.");
                            Console.ResetColor();

                            Console.WriteLine($"Hit {PickUpKey} to pick up.");
                            KeyPresser.WaitFor(PickUpKey);
                            Console.WriteLine();

                            var gold = defender.Unit.Gold;

                            attacker.Unit.TakeGold(defender.Unit);

                            Console.WriteLine($"{attacker.Unit.Name} has picked up {gold} gold.");
                            Console.WriteLine($"{attacker.Unit.Name} now has {attacker.Unit.Gold} gold.");
                            Console.WriteLine();
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
                Console.WriteLine();
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
                Console.WriteLine();
            }
        }
    }
}