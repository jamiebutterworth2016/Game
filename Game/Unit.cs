using Game.Items;

namespace Game
{
    public class Unit
    {
        public Unit(string name, Team team, Weapon weapon)
        {
            Name = name;
            Team = team;
            Weapon = weapon;
        }

        public string Name { get; private set; }

        public Team Team { get; private set; }

        public Weapon Weapon { get; set; }

        public decimal Health { get; set; }

        public void Attack(Unit unit)
        {
            var randomDamage = DamageCalculator.CalculateRandomDamage(Weapon);

            unit.Health = unit.Health - randomDamage;
        }
    }
}