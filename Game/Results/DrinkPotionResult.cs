using System.Linq;

namespace Game.Results
{
    public class DrinkPotionResult
    {
        public DrinkPotionResult(Unit unit)
        {
            if (!unit.Potions.Any())
            {
                DrankPotion = false;
                HealthBeforeDrinkingPotion = unit.Health;
            }
            else
            {
                HealthBeforeDrinkingPotion = unit.Health;

                var potion = unit.DequeuePotion();

                HealAmount = potion.HealAmount;

                unit.Health += potion.HealAmount;

                DrankPotion = true;
            }

            HealthAfterDrinkingPotion = unit.Health;
        }

        public bool DrankPotion { get; private set; }
        public decimal HealAmount { get; private set; }
        public decimal HealthBeforeDrinkingPotion { get; private set; }
        public decimal HealthAfterDrinkingPotion { get; private set; }
    }
}