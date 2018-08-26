using Game.Items;
using Game.Items.Shields;
using Game.Items.Weapons;

namespace Game
{
    public class UnitFactory
    {
        private readonly RandomFactory _randomFactory;

        public UnitFactory()
        {
            _randomFactory = new RandomFactory();
        }

        public Unit Create(string name)
        {
            var weapon = _randomFactory.Create<Weapon>();
            var shield = _randomFactory.Create<Shield>();

            var health = RandomGenerator.GetRandomDecimal(1, 40);
            var gold = RandomGenerator.GetRandomDecimal(0, 10);

            var unit = new Unit(name, Team.Enemy, health, weapon, gold, shield);

            var numberOfPotions = RandomGenerator.GetRandomNumber(1, 2);

            if (numberOfPotions > 0)
            {
                for (int i = 0; i < numberOfPotions; i++)
                    unit.AddPotion(new Potion());
            }

            return unit;
        }
    }
}