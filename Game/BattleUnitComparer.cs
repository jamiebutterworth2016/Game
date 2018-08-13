using System;
using System.Collections.Generic;

namespace Game
{
    public class BattleUnitComparer : IComparer<BattleUnit>
    {
        private readonly Die _die = new Die();

        public int Compare(BattleUnit x, BattleUnit y)
        {
            if (x == null || y == null)
                throw new ArgumentNullException();

            var rollForX = _die.Roll();
            var rollForY = _die.Roll();

            if (rollForX > rollForY)
            {
                return -1;
            }
            else if (rollForX < rollForY)
            {
                return 1;
            }
            else
            {
                return Compare(x, y);
            }
        }
    }
}