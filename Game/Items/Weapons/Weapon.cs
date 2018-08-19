using System.Runtime.Serialization;

namespace Game.Items.Weapons
{
    [DataContract]
    [KnownType(typeof(Sword))]
    public abstract class Weapon
    {
        public abstract decimal MinDamage { get; }
        public abstract decimal MaxDamage { get; }

        public decimal GetRandomDamage()
        {
            var randomDamage = RandomNumberGenerator.GetRandomDecimal(MinDamage, MaxDamage);

            return randomDamage;
        }
    }
}