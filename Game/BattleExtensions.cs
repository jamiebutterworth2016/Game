using System.Linq;

namespace Game
{
    public static class BattleExtensions
    {
        public static bool IsOver(this Battle battle)
        {
            var hero = battle.GetHero();

            if (hero == null || hero.Unit.IsDead())
                return true;

            if (battle.GetEnemies().Count() == 0)
                return true;

            return false;
        }
    }
}