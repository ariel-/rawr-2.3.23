using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Mage.SequenceReconstruction
{
    public class SequenceGroup
    {
        public bool UnavailableForMinManaCorrections;
        public double Mana;
        public double Threat;
        public double Duration;
        public double Mps
        {
            get
            {
                if (Duration > 0.0)
                {
                    return Mana / Duration;
                }
                else
                {
                    return 0.0;
                }
            }
        }

        public double Tps
        {
            get
            {
                if (Duration > 0.0)
                {
                    return Threat / Duration;
                }
                else
                {
                    return 0.0;
                }
            }
        }

        public bool ManaGemActivation
        {
            get
            {
                foreach (SequenceItem item in Item)
                {
                    if (item.CastingState.ManaGemEffect) return true;
                }
                return false;
            }
        }

        public List<SequenceItem> Item = new List<SequenceItem>();
        public List<CooldownConstraint> Constraint = new List<CooldownConstraint>();

        public double MinTime
        {
            get
            {
                int mini = int.MaxValue;
                double min = 0;
                foreach (SequenceItem item in Item)
                {
                    if (item.SuperIndex < mini)
                    {
                        mini = item.SuperIndex;
                        min = item.MinTime;
                    }
                }
                return min;
            }
            set
            {
                foreach (SequenceItem item in Item)
                {
                    item.MinTime = value;
                }
            }
        }

        public double MaxTime
        {
            get
            {
                double t = 0;
                double max = SequenceItem.Calculations.CalculationOptions.FightDuration;
                foreach (SequenceItem item in Item)
                {
                    max = Math.Min(max, item.MaxTime - t);
                    t += item.Duration;
                }
                return max;
            }
            set
            {
                foreach (SequenceItem item in Item)
                {
                    item.MaxTime = value;
                }
            }
        }

        public int Segment
        {
            get
            {
                if (Item.Count == 0) return -1;
                int seg = int.MaxValue;
                foreach (SequenceItem item in Item)
                {
                    seg = Math.Min(seg, item.Segment);
                }
                return seg;
            }
        }

        public void Add(SequenceItem item)
        {
            Mana += item.Mps * item.Duration;
            Threat += item.Tps * item.Duration;
            Duration += item.Duration;
            Item.Add(item);
        }

        public void AddRange(IEnumerable<SequenceItem> collection)
        {
            foreach (SequenceItem item in collection)
            {
                Add(item);
            }
        }

        public void SortByMps(double minMps, double maxMps)
        {
            List<SequenceItem> Item2 = new List<SequenceItem>(Item);
            List<SequenceItem> sorted = new List<SequenceItem>();
            while (Item2.Count > 0)
            {
                foreach (SequenceItem item in Item2)
                {
                    item.Tail = new List<SequenceGroup>(item.Group);
                }

                foreach (SequenceItem item in Item2)
                {
                    foreach (SequenceItem tailitem in Item2)
                    {
                        List<SequenceGroup> intersect = Rawr.Mage.ListUtils.Intersect<SequenceGroup>(item.Group, tailitem.Group);
                        if (intersect.Count > 0)
                        {
                            item.Tail = Rawr.Mage.ListUtils.Intersect<SequenceGroup>(intersect, item.Tail);
                        }
                    }
                }

                SequenceItem best = null;
                foreach (SequenceItem item in Item2)
                {
                    if (best == null || Compare(item, best, (sorted.Count > 0) ? sorted[sorted.Count - 1].Group : null, minMps, maxMps) < 0)
                    {
                        best = item;
                    }
                }
                Item2.Remove(best);
                sorted.Add(best);
            }
            //Item = sorted;
            for (int i = 0; i < sorted.Count; i++)
            {
                sorted[i].SuperIndex = i;
            }
        }

        public void SetSuperIndex()
        {
            for (int i = 0; i < Item.Count; i++)
            {
                Item[i].SuperIndex = i;
            }
        }

        private int Compare(SequenceItem x, SequenceItem y, List<SequenceGroup> tail, double minMps, double maxMps)
        {
            bool xsingletail = x.Tail.Count > 0;
            bool ysingletail = y.Tail.Count > 0;
            int compare = ysingletail.CompareTo(xsingletail);
            if (compare != 0) return compare;
            int xintersect = (tail == null) ? 0 : Rawr.Mage.ListUtils.Intersect<SequenceGroup>(x.Group, tail).Count;
            int yintersect = (tail == null) ? 0 : Rawr.Mage.ListUtils.Intersect<SequenceGroup>(y.Group, tail).Count;
            compare = yintersect.CompareTo(xintersect);
            if (compare != 0) return compare;
            return Sequence.CompareMps(x.Mps, y.Mps, minMps, maxMps);
        }

        // helper variables
        public int OrderIndex;
    }
}
