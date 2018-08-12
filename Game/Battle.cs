using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class Battle
    {
        public IEnumerable<BattleUnit> Attackers { get; set; }

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

            Attackers = battleUnits;
        }

        public void RemoveAttacker(BattleUnit attacker)
        {
            var attackers = Attackers.ToList();

            attackers.Remove(attacker);

            Attackers = attackers;
        }

        public void QueueAttacker(BattleUnit attacker)
        {
            var attackers = new Queue<BattleUnit>(Attackers);

            attackers.Enqueue(attacker);

            Attackers = attackers;
        }

        public BattleUnit DequeueAttacker()
        {
            var attackers = new Queue<BattleUnit>(Attackers);

            var attacker = attackers.Dequeue();

            Attackers = attackers;

            return attacker;
        }

        public int GetNumberOfEnemies()
        {
            return Attackers.Count(x => x.Unit.Team == Team.Enemy);
        }

        public IEnumerable<BattleUnit> GetEnemies()
        {
            return Attackers.Where(x => x.Unit.Team == Team.Enemy);
        }

        public BattleUnit GetHero()
        {
            return Attackers.SingleOrDefault(x => x.Unit.Team == Team.Hero);
        }

        public BattleUnit GetAttackerById(int id)
        {
            return Attackers.Single(x => x.Id == id);
        }
    }

    public class BattleUnitComparer : IComparer<BattleUnit>
    {
        private readonly Die _die = new Die();

        public int Compare(BattleUnit x, BattleUnit y)
        {
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