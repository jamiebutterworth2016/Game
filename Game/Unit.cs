using Game.Items;
using System.Runtime.Serialization;

namespace Game
{
    [DataContract]
    public class Unit
    {
        public Unit(string name, Team team, Weapon weapon)
        {
            Name = name;
            Team = team;
            Weapon = weapon;
        }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public Team Team { get; private set; }

        [DataMember]
        public Weapon Weapon { get; set; }

        [DataMember]
        public decimal Health { get; set; }

        public void Attack(Unit unit)
        {
            var randomDamage = DamageCalculator.CalculateRandomDamage(Weapon);

            unit.Health = unit.Health - randomDamage;
        }
    }
}