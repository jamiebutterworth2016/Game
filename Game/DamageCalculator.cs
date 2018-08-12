using Game.Items;
using System;

namespace Game
{
    public static class DamageCalculator
    {
        public static decimal CalculateRandomDamage(Weapon weapon)
        {
            if (weapon == null)
                throw new ArgumentNullException();

            var random = new Random();

            var minDamage = Convert.ToInt32(weapon.MinDamage * 100);
            var maxDamage = Convert.ToInt32(weapon.MaxDamage * 100);

            var randomDamage = Convert.ToDecimal(random.Next(minDamage, maxDamage) / 100);

            return randomDamage;
        }
    }
}