namespace Game
{
    public static class UnitExtensions
    {
        public static bool IsAlive(this Unit unit)
        {
            if (unit.Health > 0)
            {
                return true;
            }

            return false;
        }

        public static bool IsDead(this Unit unit)
        {
            if (unit.Health <= 0)
            {
                return true;
            }

            return false;
        }
    }
}