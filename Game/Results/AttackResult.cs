using Game.Items.Weapons;

namespace Game.Results
{
    public class AttackResult
    {
        public AttackResult(Weapon attackerWeapon, Unit defender)
        {
            if (attackerWeapon != null)
            {
                Damage = attackerWeapon.GetRandomDamage();

                if (defender.Shield != null)
                    IsBlocked = defender.Shield.HasBlocked();

                if (!IsBlocked)
                    defender.Health -= Damage;
            }
        }

        public decimal Damage { get; private set; }

        public bool IsBlocked { get; private set; }
    }
}