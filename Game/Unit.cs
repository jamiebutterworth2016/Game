using Game.Items;
using Game.Items.Shields;
using Game.Items.Weapons;
using Game.Results;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Game
{
    [DataContract]
    public class Unit
    {
        public Unit(string name, Team team, Weapon weapon, Shield shield = null)
        {
            Name = name;
            Team = team;
            Weapon = weapon;
            Shield = shield;
            Potions = new List<Potion>();
        }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public Team Team { get; private set; }

        [DataMember]
        public Weapon Weapon { get; set; }

        [DataMember]
        public Shield Shield { get; set; }

        [DataMember]
        public decimal Health { get; set; }

        [DataMember]
        public IEnumerable<Potion> Potions { get; set; }

        public bool HasPotions()
        {
            return Potions.Any();
        }

        public void AddPotion(Potion potion)
        {
            var potions = Potions.ToList();
            potions.Add(potion);
            Potions = potions;
        }

        public Potion DequeuePotion()
        {
            var potions = new Queue<Potion>(Potions);
            var potion = potions.Dequeue();
            Potions = potions;
            return potion;
        }

        public void TakePotions(Unit defender)
        {
            while (defender.HasPotions())
            {
                var potion = defender.DequeuePotion();
                AddPotion(potion);
            }
        }

        public AttackResult Attack(Unit defender)
        {
            return new AttackResult(Weapon, defender);
        }

        public DrinkPotionResult DrinkPotion()
        {
            return new DrinkPotionResult(this);
        }
    }
}