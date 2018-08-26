using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class Battle
    {
        public IEnumerable<BattleUnit> Units { get; set; }

        public Battle(IEnumerable<Unit> units)
        {
            if (units == null)
                throw new ArgumentNullException();

            if (units.Count() < 2)
                throw new ArgumentException();

            var id = 1;

            var battleUnits = new List<BattleUnit>();

            foreach (var unit in units)
            {
                battleUnits.Add(new BattleUnit(id, unit));

                id++;
            }

            battleUnits.Sort(new BattleUnitComparer());

            Units = battleUnits;
        }

        public void RemoveUnit(BattleUnit unit)
        {
            var units = Units.ToList();

            units.Remove(unit);

            Units = units;
        }

        public void QueueUnit(BattleUnit unit)
        {
            var units = new Queue<BattleUnit>(Units);

            units.Enqueue(unit);

            Units = units;
        }

        public BattleUnit DequeueUnit()
        {
            var units = new Queue<BattleUnit>(Units);

            var unit = units.Dequeue();

            Units = units;

            return unit;
        }

        public int GetNumberOfEnemies()
        {
            return Units.Count(x => x.Unit.Team == Team.Enemy);
        }

        public IEnumerable<BattleUnit> GetEnemies()
        {
            return Units.Where(x => x.Unit.Team == Team.Enemy);
        }

        public BattleUnit GetHero()
        {
            return Units.SingleOrDefault(x => x.Unit.Team == Team.Hero);
        }

        public BattleUnit GetUnitById(int id)
        {
            return Units.Single(x => x.Id == id);
        }
    }
}