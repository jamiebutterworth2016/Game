using System;

namespace Game
{
    public class BattleUnit
    {
        public BattleUnit(int id, Unit unit)
        {
            Id = id;
            Unit = unit ?? throw new ArgumentNullException();
        }

        public int Id { get; set; }
        public Unit Unit { get; set; }
    }
}