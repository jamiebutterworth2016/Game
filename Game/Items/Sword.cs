namespace Game.Items
{
    public class Sword : Weapon
    {
        public override decimal MinDamage { get => 5; }
        public override decimal MaxDamage { get => 15; }
    }
}