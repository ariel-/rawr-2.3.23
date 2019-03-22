using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Mage.SequenceReconstruction
{
    public class CooldownConstraint
    {
        public SequenceGroup Group { get; set; }
        public double Cooldown { get; set; }
        public double Duration { get; set; }
        public bool ColdSnap { get; set; }
        public EffectCooldown EffectCooldown { get; set; }
    }

#if RAWR3
    public class SequenceItem
#else
    public class SequenceItem : ICloneable
#endif
    {
        public static DisplayCalculations Calculations;

        private SequenceItem() { }
        public SequenceItem(int index, double duration) : this(index, duration, null) { }

        public SequenceItem(int index, double duration, List<SequenceGroup> group)
        {
            if (group == null) group = new List<SequenceGroup>();
            this.Group = group;
            this.index = index;
            this.variableType = Calculations.SolutionVariable[index].Type;
            mps = Calculations.SolutionVariable[index].Mps;
            tps = Calculations.SolutionVariable[index].Tps;
            this.Duration = duration;
            this.cycle = Calculations.SolutionVariable[index].Cycle;
            this.castingState = Calculations.SolutionVariable[index].State;
            this.segment = Calculations.SolutionVariable[index].Segment;
            if (castingState == null) castingState = Calculations.BaseState;

            minTime = 0.0;
            maxTime = Calculations.CalculationOptions.FightDuration;

            if (variableType == VariableType.Wand)
            {
                cycle = Calculations.Wand;
                mps = cycle.ManaPerSecond;
                tps = cycle.ThreatPerSecond;
            }
            else if (variableType == VariableType.ManaGem)
            {
                mps = 0.0;
                tps = Calculations.ManaGemTps;
            }
            else if (variableType == VariableType.ManaPotion)
            {
                mps = 0.0;
                tps = Calculations.ManaPotionTps;
            }
            else if (variableType == VariableType.Drinking)
            {
                maxTime = 0.0;
                mps = -Calculations.BaseState.ManaRegenDrinking;
                tps = 0.0;
            }
            else if (variableType == VariableType.TimeExtension)
            {
                minTime = maxTime;
                tps = 0.0;
            }
            else if (variableType == VariableType.AfterFightRegen)
            {
                mps = -Calculations.BaseState.ManaRegenDrinking;
                tps = 0.0;
                minTime = maxTime;
            }
        }

        private int index;
        public int Index
        {
            get
            {
                return index;
            }
        }

        private VariableType variableType;
        public VariableType VariableType
        {
            get
            {
                return variableType;
            }
        }

        public bool IsEvocation
        {
            get
            {
                return variableType == VariableType.Evocation || variableType == VariableType.EvocationIV || variableType == VariableType.EvocationHero || variableType == VariableType.EvocationIVHero;
            }
        }

        public bool IsManaPotionOrGem
        {
            get
            {
                return variableType == VariableType.ManaPotion || variableType == VariableType.ManaGem;
            }
        }

        private int segment;
        public int Segment
        {
            get
            {
                return segment;
            }
            set
            {
                segment = value;
            }
        }

        public double Duration;
        public double Timestamp;

        private double minTime;
        public double MinTime
        {
            get
            {
                return minTime;
            }
            set
            {
                minTime = Math.Max(minTime, value);
            }
        }

        private double maxTime;
        public double MaxTime
        {
            get
            {
                return maxTime;
            }
            set
            {
                maxTime = Math.Min(maxTime, value);
            }
        }

        public void SetTimeConstraint(double minTime, double maxTime)
        {
            this.minTime = minTime;
            this.maxTime = maxTime;
        }

        public List<SequenceGroup> Group;
        public SequenceGroup SuperGroup;

        // helper variables
        public int SuperIndex;
        public List<SequenceGroup> Tail;
        public int CooldownHex;
        public int OrderIndex;

        private Cycle cycle;
        public Cycle Cycle
        {
            get
            {
                return cycle;
            }
        }

        private CastingState castingState;
        public CastingState CastingState
        {
            get
            {
                return castingState;
            }
        }

        private double mps;
        public double Mps
        {
            get
            {
                return mps;
            }
        }

        private double tps;
        public double Tps
        {
            get
            {
                return tps;
            }
        }

        #region ICloneable Members

#if !RAWR3
        object ICloneable.Clone()
        {
            return Clone();
        }
#endif

        public SequenceItem Clone()
        {
            SequenceItem clone = (SequenceItem)MemberwiseClone();
            clone.Group = new List<SequenceGroup>(Group);
            return clone;
        }

        #endregion

        public override string ToString()
        {
            if (cycle == null) return string.Format("{0}: {1}", Segment, VariableType);
            return string.Format("{0}: {1}", Segment, castingState.BuffLabel + "+" + cycle.Name);
        }
    }
}
