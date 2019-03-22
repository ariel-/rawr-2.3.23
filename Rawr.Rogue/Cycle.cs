using System;
using System.Collections.Generic;
using Rawr.Rogue.FinishingMoves;
#if RAWR3
using System.Linq;
#endif

namespace Rawr.Rogue
{
#if (SILVERLIGHT == false)
    [Serializable]
#endif
    public class Cycle
    {
        private readonly List<CycleComponent> _components = new List<CycleComponent>();

        public int TotalComboPoints
        {
            get
            {
                var ret = 0;
                for (var i = 0; i < _components.Count; i++)
                    ret += (_components[i]).Rank;
                return ret;
            }
        }

        public override string ToString()
        {
            return string.Join("", _components.ConvertAll(c => c.ToString()).ToArray());
        }

        public List<CycleComponent> Components
        {
            get { return _components; }
        }

        /*public bool Includes(string componentName)
        {
            return FindAll(componentName).Count > 0;
        }

        public CycleComponent Find(string name)
        {
            return FindAll(name).Count > 0 ? FindAll(name)[0] : null;
        }

        public List<CycleComponent> FindAll(string name)
        {
            return _components.FindAll(c => c.Finisher.Name == name);
        }*/
    }

#if (SILVERLIGHT == false)
    [Serializable]
#endif
    public class CycleComponent
    {
        private FinisherBase _finisher;
        private int _rank;

        public CycleComponent()
        {
            _rank = 0;
            _finisher = new NoFinisher();
        }

        public CycleComponent(int r, FinisherBase f)
        {
            _rank = r;
            _finisher = f;
        }

        public int Rank
        {
            get { return _rank; }
            set { _rank = value; }
        }

        public FinisherBase Finisher
        {
            get { return _finisher; }
            set { _finisher = value; }
        }

        /*public float CalcFinisherDps( CalculationOptionsRogue calcOpts, CombatFactors combatFactors, Stats stats, WhiteAttacks whiteAttacks, CycleTime cycleTime, CharacterCalculationsRogue displayValues )
        {
            return _finisher.CalcFinisherDPS(calcOpts, stats, combatFactors, _rank, cycleTime, whiteAttacks, displayValues);
        }

        public override string ToString()
        {
            return _rank == 0 || _finisher.Id == new NoFinisher().Id ? "" : _rank + _finisher.Id.ToString();
        }*/
    }
}