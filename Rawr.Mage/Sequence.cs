using System;
using System.Collections.Generic;
using System.Text;
#if RAWR3
using System.Linq;
#endif

namespace Rawr.Mage.SequenceReconstruction
{
    public class Sequence : IComparer<SequenceItem>
    {
        internal List<SequenceItem> sequence = new List<SequenceItem>();

        internal int ColumnManaGem;
        internal int ColumnManaPotion;

        public void Add(SequenceItem item)
        {
            sequence.Add(item);
        }

        /*public bool IsCooldownBreakpoint(int index)
        {
            if (index == 0) return true;
            if (sequence[index].IsManaPotionOrGem || sequence[index].VariableType == VariableType.DrumsOfBattle) return false;
            CastingState state = sequence[index].CastingState;
            CastingState lastState = null;
            int lastindex = index - 1;
            while (lastindex >= 0 && (sequence[lastindex].IsManaPotionOrGem || sequence[lastindex].VariableType == VariableType.DrumsOfBattle))
            {
                lastindex--;
            }
            if (lastindex >= 0) lastState = sequence[lastindex].CastingState;
            if (lastState == null || !((lastState.ArcanePower && state.ArcanePower) || (lastState.IcyVeins && state.IcyVeins) || (lastState.Heroism && state.Heroism) || (lastState.MoltenFury && state.MoltenFury) || (lastState.PotionOfWildMagic && state.PotionOfWildMagic) || (lastState.PotionOfSpeed && state.PotionOfSpeed) || (lastState.FlameCap && state.FlameCap) || (lastState.DrumsOfBattle && state.DrumsOfBattle) || (lastState.Evocation && state.Evocation) || (lastState.PowerInfusion && state.PowerInfusion) || (lastState.ManaGemEffect && state.ManaGemEffect)))
            {
                return true;
            }
            return false;
        }*/

        List<SequenceGroup> superGroup = new List<SequenceGroup>();

        private void CalculateSuperGroups()
        {
            superGroup.Clear();
            SequenceGroup group = null;
            List<SequenceGroup> lastGroup = null;
            for (int i = 0; i < sequence.Count; i++)
            {
                if (lastGroup == null || (!sequence[i].IsManaPotionOrGem && Rawr.Mage.ListUtils.Intersect<SequenceGroup>(lastGroup, sequence[i].Group).Count == 0))
                {
                    group = new SequenceGroup();
                    superGroup.Add(group);
                }
                if (!sequence[i].IsManaPotionOrGem) lastGroup = sequence[i].Group;
                group.Add(sequence[i]);
                sequence[i].SuperGroup = group;
            }
        }

        private bool sortByMps = true;
        private bool preserveCooldowns = true;
        private double sortMaxMps = double.PositiveInfinity;
        private double sortMinMps = double.NegativeInfinity;
        private double sortStartTime;
        private double sortTargetTime;

        public void ComputeTimestamps()
        {
            double t = 0;
            for (int i = 0; i < sequence.Count; i++)
            {
                double d = sequence[i].Duration;
                if (sequence[i].IsManaPotionOrGem) d = 0;
                sequence[i].Timestamp = t;
                t += d;
            }
        }

        private double MinTime(SequenceGroup super, int placedUpTo)
        {
            const double eps = 0.000001;
            double minTime = super.MinTime;
            foreach (SequenceGroup group in GetAllGroups(super.Item))
            {
                double diff = group.MinTime - super.MinTime;
                foreach (CooldownConstraint constraint in group.Constraint)
                {
                    for (int j = 0; j <= placedUpTo; j++)
                    {
                        if (sequence[j].Group.Contains(constraint.Group))
                        {
                            if (!constraint.ColdSnap || (group.MinTime - sequence[j].MinTime >= SequenceItem.Calculations.IcyVeinsCooldown - eps)) // make sure to ignore coldsnapped constraints
                            {
                                minTime = Math.Max(minTime, sequence[j].Timestamp + constraint.Cooldown - diff);
                            }
                            break;
                        }
                    }
                }
            }
            return minTime;
        }

        private double MinTime(int i, int placedUpTo)
        {
            if (placedUpTo >= i) placedUpTo = i - 1;
            return MinTime(sequence[i].SuperGroup, placedUpTo) + sequence[i].MinTime - sequence[i].SuperGroup.MinTime;
        }

        public void SortByMps(bool preserveCooldowns, double minMps, double maxMps, double maxTps, double startTime, double targetTime, double extraMana, double startMana)
        {
            const double eps = 0.000001;
            if (minMps > maxMps) maxMps = minMps;
            double limitTime = sequence[sequence.Count - 1].Timestamp + sequence[sequence.Count - 1].Duration;
            if (targetTime > limitTime) targetTime = limitTime;

            this.sortByMps = true;
            this.preserveCooldowns = preserveCooldowns;
            this.sortMaxMps = maxMps;
            this.sortMinMps = minMps;
            this.sortStartTime = startTime;
            this.sortTargetTime = targetTime;

            CalculateSuperGroups();

            double maxMana = maxMps * (targetTime - startTime);
            double minMana = minMps * (targetTime - startTime);
            double mana = 0;
            double threat = 0;

            double t = 0;
            int i;
            SequenceGroup lastGroup = null;
            for (i = 0; i < sequence.Count; i++)
            {
                double d = sequence[i].Duration;
                if (sequence[i].IsManaPotionOrGem) d = 0;
                if (d > 0 && t + eps >= startTime)
                {
                    if (lastGroup != sequence[i].SuperGroup) break;
                    else
                    {
                        threat += sequence[i].Tps * sequence[i].Duration;
                        mana += sequence[i].Mps * d;
                    }
                }
                else
                {
                    if (t + d > startTime) mana += sequence[i].Mps * (t + d - startTime);
                    threat += sequence[i].Tps * sequence[i].Duration;
                    lastGroup = sequence[i].SuperGroup;
                }
                t += d;
            }
            if (extraMana == 0)
            {
                foreach (SequenceGroup group in superGroup)
                {
                    //group.SortByMps(minMps, maxMps);
                    group.SetSuperIndex();
                }
                sequence.Sort(i, sequence.Count - i, this);
                ComputeTimestamps();
            }
            if (targetTime < t) return; // there is nothing we can do at this point
            double T = t;
            double Mana = mana;
            double Threat = threat;
        Retry:
            // first we have sections in the right mps range, then higher, then lower (at the end sections that are not ready yet)
            // so the constraint that will be broken first is maxmana (unless no high burn section is available at the moment)
            // forward to target time
            int j;
            int lastHigh = i;
            double tLastHigh = t;
            double overflowMana = startMana - Mana; // for overflow calculations assume there are no mana consumables placed after start time yet, if we skipped a super group since startTime then we have to adjust starting mana
            if (overflowMana > BaseStats.Mana)
            {
                overflowMana = BaseStats.Mana; // can't have more mana than max
            }
            double overflowLimit = BaseStats.Mana; // was maxMana before, but I think when we have splittable group we can insert just enough to not go over (I did not think this through too much, so if something is fishy look into this)
            SequenceGroup lastSuper = null;
            for (j = i; j < sequence.Count; j++)
            {
                double d = sequence[j].Duration;
                if (sequence[j].IsManaPotionOrGem) d = 0;
                double overflowBuffer = 0.0;
                double threatBuffer = 0.0;
                if (lastSuper != sequence[j].SuperGroup)
                {
                    /*if (sequence[j].Group.Count > 0)
                    {
                        overflowLimit = BasicStats.Mana - overflowMana;
                    }
                    else
                    {
                        overflowLimit = BasicStats.Mana; // this too
                    }*/
                    overflowLimit = BaseStats.Mana - overflowMana;
                    lastSuper = sequence[j].SuperGroup;
                    // analize if the group overflows
                    double projectMana = overflowMana;
                    double projectThreat = threat;
                    double projectTime = t;
                    for (int k = 0; k < sequence[j].SuperGroup.Item.Count; k++)
                    {
                        SequenceItem kitem = sequence[j].SuperGroup.Item[k];
                        double kd = kitem.Duration;
                        if (kitem.CastingState.ManaGemEffect && (k == 0 || (!sequence[j].SuperGroup.Item[k - 1].CastingState.ManaGemEffect && sequence[j].SuperGroup.Item[k - 1].VariableType != VariableType.ManaGem && sequence[j].SuperGroup.Item[k - 1].VariableType != VariableType.ManaPotion)))
                        {
                            projectMana += SequenceItem.Calculations.ManaGemValue * (1 + BaseStats.BonusManaGem);
                            if (projectMana - BaseStats.Mana > overflowBuffer)
                            {
                                sequence[j].SuperGroup.UnavailableForMinManaCorrections = true;
                                overflowBuffer = projectMana - BaseStats.Mana;
                            }
                        }
                        if (projectTime + kd > targetTime)
                        {
                            projectMana -= kitem.Mps * (targetTime - projectTime);
                            projectThreat += kitem.Tps * (targetTime - projectTime);
                            projectTime = targetTime;
                            break;
                        }
                        else
                        {
                            projectMana -= kitem.Mps * kd;
                            projectThreat += kitem.Tps * kd;
                            projectTime += kd;
                        }
                        // don't care if idle regen is placed at start if we have threat limitations
                        // don't mark as problem if we were overflowed already, can't fix that
                        if (!(maxTps < 5000.0 && kitem.VariableType == VariableType.IdleRegen && kitem.Segment == 0) && (projectMana - BaseStats.Mana > overflowBuffer) && (overflowMana <= overflowBuffer + eps))
                        {
                            sequence[j].SuperGroup.UnavailableForMinManaCorrections = true;
                            overflowBuffer = projectMana - BaseStats.Mana;
                        }
                        // only buffer items that are above tps limit; if we're above limit from before start time
                        // then this would try buffering items that were used to buffer other items, it gets ugly
                        if (kitem.Tps > maxTps && projectThreat - maxTps * projectTime > threatBuffer)
                        {
                            threatBuffer = projectThreat - maxTps * projectTime;
                        }
                    }
                }
                double minTimeJ = MinTime(j, j - 1);
                if (t < minTimeJ - eps || overflowBuffer > eps || threatBuffer > eps)
                {
                    // sequence positioned too early, we have to buffer up with something that can
                    // be positioned at t and is either small enough not to disrupt max time
                    // or is splittable
                    bool updated = false;
                    double minbuffer = minTimeJ - t;
                    double buffer = sequence[j].MaxTime - t;
                    int k;
                    for (k = j + 1; k < sequence.Count && (minbuffer > eps || overflowBuffer > eps || threatBuffer > eps) && buffer > eps; k++)
                    {
                        if (sequence[k].SuperGroup != lastSuper) // intra super ordering not allowed
                        {
                            if (MinTime(k, j - 1) <= t + eps)
                            {
                                if (sequence[k].Group.Count == 0 && (minbuffer > eps || (overflowBuffer > eps && sequence[k].Mps > 0 && !sequence[k].CastingState.ManaGemEffect) || (threatBuffer > eps && sequence[k].Tps < maxTps)))
                                {
                                    // if we're not fixing cooldown constraints then make sure not to break threat limits by moving stuff around
                                    if (minbuffer < eps && sequence[k].Tps > maxTps)
                                    {
                                        // threat + duration * tps <= maxTps * t + maxTps * duration
                                        // duration <= (maxTps * t - threat) / (tps - maxTps)
                                        double maxSplit = (maxTps * t - threat) / (sequence[k].Tps - maxTps);
                                        if (maxSplit < eps)
                                        {
                                            continue;
                                        }
                                        if (maxSplit < buffer)
                                        {
                                            buffer = maxSplit;
                                        }
                                    }
                                    if (minbuffer > eps && sequence[k].Duration > minbuffer + eps)
                                    {
                                        SplitAt(k, minbuffer);
                                    }
                                    else if (overflowBuffer > eps && sequence[k].Duration > overflowBuffer / sequence[k].Mps)
                                    {
                                        SplitAt(k, Math.Min(buffer, overflowBuffer / sequence[k].Mps));
                                    }
                                    else if (threatBuffer > eps && (maxTps - sequence[k].Tps) * sequence[k].Duration > threatBuffer)
                                    {
                                        SplitAt(k, Math.Min(buffer, threatBuffer / (maxTps - sequence[k].Tps)));
                                    }
                                    else if (sequence[k].Duration > buffer + eps)
                                    {
                                        SplitAt(k, buffer);
                                    }
                                    SequenceItem copy = sequence[k];
                                    sequence.RemoveAt(k);
                                    sequence.Insert(j, copy);
                                    ComputeTimestamps();
                                    minbuffer -= copy.Duration;
                                    buffer -= copy.Duration;
                                    if (overflowBuffer > eps) overflowBuffer -= copy.Mps * copy.Duration;
                                    if (threatBuffer > eps) threatBuffer -= (maxTps - copy.Tps) * copy.Duration;
                                    t += copy.Duration;
                                    updated = true;
                                    j++;
                                    k = j;
                                }
                                else if (sequence[k].SuperGroup.Duration <= buffer && (minbuffer > eps || (overflowBuffer > eps && sequence[k].SuperGroup.Mps > 0 && ! sequence[k].SuperGroup.ManaGemActivation) || (threatBuffer > eps && sequence[k].SuperGroup.Tps < maxTps)))
                                {
                                    int l;
                                    for (l = k + 1; l < sequence.Count; l++)
                                    {
                                        if (sequence[l].SuperGroup != sequence[k].SuperGroup) break;
                                    }
                                    List<SequenceItem> copy = sequence.GetRange(k, l - k);
                                    sequence.RemoveRange(k, l - k);
                                    sequence.InsertRange(j, copy);
                                    ComputeTimestamps();
                                    minbuffer -= copy[0].SuperGroup.Duration;
                                    buffer -= copy[0].SuperGroup.Duration;
                                    if (overflowBuffer > eps) overflowBuffer -= copy[0].SuperGroup.Mps * copy[0].SuperGroup.Duration;
                                    if (threatBuffer > eps) threatBuffer -= (maxTps - copy[0].SuperGroup.Tps) * copy[0].SuperGroup.Duration;
                                    t += copy[0].SuperGroup.Duration;
                                    updated = true;
                                    j += copy.Count;
                                    k = j;
                                }
                            }
                        }
                    }
                    if (updated)
                    {
                        t = T;
                        mana = Mana;
                        threat = Threat;
                        goto Retry;
                    }
                }
                if (d > 0 && sequence[j].SuperGroup.Mps > maxMps)
                {
                    lastHigh = j;
                    tLastHigh = t;
                }
                if (t + d > targetTime)
                {
                    mana += sequence[j].Mps * (targetTime - t);
                    threat += sequence[j].Tps * (targetTime - t);
                    overflowMana -= sequence[j].Mps * (targetTime - t);
                    break;
                }
                else
                {
                    mana += sequence[j].Mps * d;
                    threat += sequence[j].Mps * d;
                    overflowMana -= sequence[j].Mps * d;
                    if (maxTps < 5000.0 && overflowMana > BaseStats.Mana) overflowMana = BaseStats.Mana;
                    t += d;
                }
            }
            // verify max time constraints
            double tt = T;
            int a;
            lastSuper = null;
            for (a = i; a < sequence.Count; a++)
            {
                double d = sequence[a].Duration;
                if (sequence[a].IsManaPotionOrGem) d = 0;
                if (lastSuper != sequence[a].SuperGroup)
                {
                    lastSuper = sequence[a].SuperGroup;
                    if (tt > lastSuper.MaxTime + eps && tt > T && tt > MinTime(lastSuper, a - 1) + eps) // there might be other cases where it is impossible to move back without breaking others, double check for infinite cycles
                    {
                        // compute buffer of items that can be moved way back
                        double buffer = 0;
                        int b;
                        for (b = i; b < a; b++)
                        {
                            if (sequence[b].MaxTime >= tt + lastSuper.Duration) buffer += sequence[b].Duration;
                        }
                        // place it at max time, but move back over non-splittable super groups
                        // if move breaks constraint on some other group cancel
                        double t3 = tt;
                        bool updated = false;
                        for (b = a - 1; b >= i; b--)
                        {
                            t3 -= sequence[b].Duration;
                            if (t3 <= lastSuper.MaxTime + eps)
                            {
                                // possible insert point
                                if (sequence[b].Group.Count == 0)
                                {
                                    if (sequence[b].MaxTime >= lastSuper.MaxTime + lastSuper.Duration - buffer)
                                    {
                                        // splittable, make a split at max time
                                        if (lastSuper.MaxTime > t3 + eps)
                                        {
                                            SplitAt(b, lastSuper.MaxTime - t3);
                                            a++;
                                            b++;
                                        }
                                        sequence.InsertRange(b, RemoveSuperGroup(a));
                                        ComputeTimestamps();
                                        updated = true;
                                    }
                                }
                                else
                                {
                                    // we are in super group, move to start
                                    SequenceGroup super = sequence[b].SuperGroup;
                                    while (b >= i && sequence[b].SuperGroup == super)
                                    {
                                        b--;
                                        if (b >= 0) t3 -= sequence[b].Duration;
                                    }
                                    if (b >= 0) t3 += sequence[b].Duration;
                                    b++;
                                    if (super.MaxTime >= t3 + lastSuper.Duration - buffer)
                                    {
                                        sequence.InsertRange(b, RemoveSuperGroup(a));
                                        ComputeTimestamps();
                                        updated = true;
                                    }
                                }
                                break;
                            }
                            else
                            {
                                // make sure we wouldn't push it out of max
                                if (sequence[b].MaxTime < t3 + lastSuper.Duration - buffer)
                                {
                                    break;
                                }
                            }
                        }
                        if (updated)
                        {
                            t = T;
                            mana = Mana;
                            threat = Threat;
                            goto Retry;
                        }
                    }
                }
                tt += d;
            }
            bool extraMode = extraMana > 0;
            if (mana > maxMana + eps || extraMana > 0)
            {
                // [i....j]XXX[k....]
                int k;
                // [i..|jj..j]XXX[k..|kk.]
                int jj = j; double jT = targetTime - t;
                double tjj = t;
                double tkk = tLastHigh;
                if (jT <= 0.00001)
                {
                    jj--;
                    if (jj >= 0 && jj < sequence.Count)
                    {
                        if (jj < sequence.Count - 1 && sequence[jj].SuperGroup != sequence[jj + 1].SuperGroup) overflowLimit -= sequence[jj].SuperGroup.Mana;
                        jT += sequence[jj].Duration;
                        tjj -= sequence[jj].Duration;
                    }
                }
                double maxPush = double.PositiveInfinity;
                if (lastHigh <= jj && jj < sequence.Count - 1) // it only makes sense if we move from after target time and move in front
                {
                    if (sequence[jj].Group.Count == 0) lastHigh = jj;
                    else lastHigh = jj + 1;
                }
                // if jj > lastHigh then jj actually won't be pushed back as the swap will occur before it (it might actually be moving toward start)
                if (jj < sequence.Count && jj <= lastHigh) maxPush = sequence[jj].MaxTime - tjj; // you can assume jj won't be split
                for (k = lastHigh; k < sequence.Count; k++)
                {
                    // make sure item is low mps and can be moved back
                    //if ((sequence[k].SuperGroup.Mps <= 0 || (!extraMode && sequence[k].SuperGroup.Mps <= maxMps)) && MinTime(k, j) <= tjj + jT) break;
                    // the only thing really required to have mana drop is for mps to be lower than at jj
                    if (sequence[k].SuperGroup != sequence[jj].SuperGroup && sequence[k].SuperGroup.Mps < sequence[jj].Mps - eps && (MinTime(k, j) + eps < sequence[k].Timestamp)) break;
                    // everything we skip will have to be pushed so make sure there is space
                    maxPush = Math.Min(maxPush, sequence[k].MaxTime - tkk);
                    tkk += sequence[k].Duration;
                }
                if (k < sequence.Count && maxPush > eps)
                {
                    int kk = k; double kT = 0;
                    double currentPush = 0;
                    do
                    {
                        double nextT = Math.Min(jT, sequence[kk].Duration - kT);
                        double mpsdiff = sequence[jj].Mps - sequence[kk].Mps;
                        if (mpsdiff < -eps)
                        {
                            break; // if we go on we'll actually make it worse, so stop here and hope that we can do some swapping
                        }
                        if (mana - mpsdiff * nextT <= minMana)
                        {
                            nextT = (mana - minMana) / mpsdiff;
                        }
                        if (sequence[jj].Group.Count > 0)
                        {
                            if (overflowLimit + sequence[kk].Mps * nextT < 0)
                            {
                                nextT = -overflowLimit / sequence[kk].Mps;
                            }
                        }
                        else
                        {
                            if (mpsdiff > 0 && overflowLimit + sequence[jj].Mps * jT - mpsdiff * nextT < 0)
                            {
                                nextT = (overflowLimit + sequence[jj].Mps * jT) / mpsdiff;
                            }
                        }
                        if (currentPush + nextT > maxPush)
                        {
                            nextT = maxPush - currentPush;
                        }
                        mana -= mpsdiff * nextT;
                        overflowLimit += sequence[kk].Mps * nextT;
                        currentPush += nextT;
                        if (extraMode) extraMana += sequence[kk].Mps * nextT;
                        jT -= nextT;
                        if (jT <= 0)
                        {
                            SequenceGroup currentSuper = sequence[jj].SuperGroup;
                            jj--;
                            if (jj >= 0)
                            {
                                jT += sequence[jj].Duration;
                                tjj -= sequence[jj].Duration;
                                maxPush = Math.Min(maxPush, sequence[jj].MaxTime - tjj);
                                if (sequence[jj].SuperGroup != sequence[jj + 1].SuperGroup) overflowLimit -= sequence[jj].SuperGroup.Mana;
                                // seems like this never really applies
                                // if we don't have enough mana for long sequence then pushing it out
                                // won't create new opportunities for mana consumables
                                //if (extraMode && sequence[jj].SuperGroup != currentSuper) extraMana = double.NegativeInfinity;
                            }
                            else
                            {
                                if (extraMode) extraMana = double.NegativeInfinity;
                            }
                        }
                        kT += nextT;
                        if (kT >= sequence[kk].Duration - eps)
                        {
                            kT -= sequence[kk].Duration;
                            kk++;
                        }
                        if (((mana <= maxMana && (extraMana <= eps || mana <= minMana)) || (sequence[jj].Group.Count > 0 && overflowLimit <= 0)) && (kk >= sequence.Count || sequence[kk].Group.Count == 0 || (kT < eps && (kk == 0 || sequence[kk].SuperGroup != sequence[kk - 1].SuperGroup)))) // make sure not to force a split of super group, if you actually have a low mps super then you have to move it as a whole
                        {
                            break;
                        }
                        if (nextT == 0.0) break; // if we get stopped in the middle of super group then abort
                    } while (jj >= i && kk < sequence.Count && MinTime(k, jj) <= tjj + jT && MinTime(kk, jj) <= tjj + jT + currentPush - kT && currentPush < maxPush);
                    // [i..[k..||jj..j]XXXkk.]
                    if (kk >= sequence.Count || sequence[kk].Group.Count == 0 || (kT < eps && (kk == 0 || sequence[kk].SuperGroup != sequence[kk - 1].SuperGroup))) // if require super split, then abort, consider restarting at higher lastHigh
                    {
                        if (kT > 0)
                        {
                            SplitAt(kk, kT);
                            kk++;
                        }
                        // if k has negative mps, then just placing it at the end won't work
                        // we're breaking max mana constraint, this means we're most likely (but not necessarily) oom
                        // placing -mps at the end won't help us get from negative
                        // we have to place it before it gets to that point (but don't go breaking maxtime constraints)
                        // when we're filling with extra mana this does not apply
                        List<SequenceItem> copy = sequence.GetRange(k, kk - k);
                        double totalmana = 0;
                        foreach (SequenceItem item in copy)
                            totalmana += item.Mps * item.Duration;
                        while (jj >= i && currentPush <= maxPush && !extraMode && totalmana < 0)
                        {
                            if (sequence[jj].Mps * jT > -totalmana)
                            {
                                jT += totalmana / sequence[jj].Mps;
                                break;
                            }
                            totalmana += sequence[jj].Mps * jT;
                            jj--;
                            jT = sequence[jj].Duration;
                            tjj -= sequence[jj].Duration;
                            maxPush = Math.Min(maxPush, sequence[jj].MaxTime - tjj);
                        }
                        if (jj < 0) jj = 0; // TODO investigate
                        if (jT >= sequence[jj].Duration)
                        {
                            jT -= sequence[jj].Duration;
                            jj++;
                        }
                        if (jj >= sequence.Count) return;
                        // don't split into supergroup, make a clean cut
                        if (sequence[jj].Group.Count > 0 && currentPush <= maxPush && (jT > 0 || (jj > 0 && sequence[jj].SuperGroup == sequence[jj - 1].SuperGroup))) // if we're mid super group, and we can push the end we can push the whole super group
                        {
                            // move to start of super group
                            jT = 0;
                            SequenceGroup super = sequence[jj].SuperGroup;
                            while (jj >= 0 && sequence[jj].SuperGroup == super)
                            {
                                jj--;
                            }
                            jj++;
                        }
                        // final split and reinsert
                        if (jj >= i)
                        {
                            sequence.RemoveRange(k, kk - k);
                            if (jj < sequence.Count && jT > eps) // only happens in release, wasn't able to track it down
                            {
                                SplitAt(jj, jT);
                                jj++;
                                kk++;
                            }
                            if (jj >= sequence.Count) sequence.AddRange(copy); // only happens in release, wasn't able to track it down
                            else sequence.InsertRange(jj, copy);
                        }
                        ComputeTimestamps();
                    }
                }
            }
            else if (mana < minMana)
            {
                // no high burn sequence is available yet
                // take first super group with enough burn and place it as soon as possible
                // TODO: this needs a complete redesign, need some logic to ensure the changes made actually result in increase of mana consumption
                tt = T;
                lastSuper = null;
                for (a = i; a < sequence.Count; a++)
                {
                    double d = sequence[a].Duration;
                    if (sequence[a].IsManaPotionOrGem) d = 0;
                    if (lastSuper != sequence[a].SuperGroup)
                    {
                        lastSuper = sequence[a].SuperGroup;
                        double minLastSuper = MinTime(lastSuper, a - 1);
                        if (!lastSuper.UnavailableForMinManaCorrections && tt + lastSuper.Duration > targetTime && lastSuper.Mps > minMps && tt > T && tt > minLastSuper + eps && minLastSuper < targetTime - eps)
                        {
                            // compute buffer of items that can be moved way back
                            double buffer = 0;
                            int b;
                            for (b = i; b < a; b++)
                            {
                                if (sequence[b].MaxTime >= tt + lastSuper.Duration) buffer += sequence[b].Duration;
                            }
                            // place it at min time, but move forward over non-splittable super groups
                            // if move breaks constraint on some other group cancel
                            int lastSafeInsert = a;
                            double t3 = tt;
                            bool updated = false;
                            bool manaTest = false;
                            for (b = a - 1; b >= i; b--)
                            {
                                if (b == 0 || sequence[b].SuperGroup != sequence[b - 1].SuperGroup) lastSafeInsert = b;
                                if (lastSuper.Mps > sequence[b].SuperGroup.Mps) manaTest = true;
                                t3 -= sequence[b].Duration;
                                if (t3 <= minLastSuper + eps)
                                {
                                    if (!manaTest) break;
                                    // possible insert point
                                    if (sequence[b].Group.Count == 0)
                                    {
                                        if (sequence[b].MaxTime >= minLastSuper + lastSuper.Duration - buffer)
                                        {
                                            // splittable, make a split at max time
                                            if (MinTime(lastSuper, a - 1) > t3 + eps)
                                            {
                                                SplitAt(b, minLastSuper - t3);
                                                a++;
                                                b++;
                                            }
                                            sequence.InsertRange(b, RemoveSuperGroup(a));
                                            ComputeTimestamps();
                                            updated = true;
                                        }
                                    }
                                    else
                                    {
                                        // we are in super group, use last safe insert
                                        if (lastSafeInsert < a)
                                        {
                                            sequence.InsertRange(lastSafeInsert, RemoveSuperGroup(a));
                                            ComputeTimestamps();
                                            updated = true;
                                        }
                                    }
                                    break;
                                }
                                else
                                {
                                    // make sure we wouldn't push it out of max
                                    if (sequence[b].MaxTime < t3 + lastSuper.Duration - buffer)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (updated)
                            {
                                t = T;
                                mana = Mana;
                                threat = Threat;
                                goto Retry;
                            }
                        }
                    }
                    tt += d;
                }
            }
        }

        int IComparer<SequenceItem>.Compare(SequenceItem x, SequenceItem y)
        {
            if (sortByMps)
            {
                if (preserveCooldowns)
                {
                    if (x.SuperGroup == y.SuperGroup)
                    {
                        return x.SuperIndex.CompareTo(y.SuperIndex);
                    }
                    else
                    {
                        int compare = x.SuperGroup.Segment.CompareTo(y.SuperGroup.Segment);
                        if (compare != 0) return compare;
                        bool xcritical = x.SuperGroup.MaxTime <= sortStartTime;
                        bool ycritical = y.SuperGroup.MaxTime <= sortStartTime;
                        compare = ycritical.CompareTo(xcritical);
                        if (compare != 0) return compare;
                        bool xsplit = x.Group.Count == 0;
                        bool ysplit = y.Group.Count == 0;
                        compare = xsplit.CompareTo(ysplit);
                        if (compare != 0) return compare;
                        if (x.Group.Count == 0 && y.Group.Count == 0)
                        {
                            // only sort by mps for normal casting without cooldowns
                            // if both are in super groups we have to sort in the way as prescribed by SortGroups solution
                            // only sort by mps if both are splittable (otherwise this would not necessarily be an ordering)
                            compare = CompareByMps(x.SuperGroup, y.SuperGroup);
                            if (compare != 0) return compare;
                        }
                        // if two super groups have same mps make sure to group by super group
                        return x.SuperGroup.MinTime.CompareTo(y.SuperGroup.MinTime); // is min time unique???
                    }
                }
                else
                {
                    return CompareByMps(x, y);
                }
            }
            return 0;
        }

        private int CompareByMps(SequenceItem x, SequenceItem y)
        {
            return CompareMps(x.Mps, y.Mps, sortMinMps, sortMaxMps);
        }

        private int CompareByMps(SequenceGroup x, SequenceGroup y)
        {
            return CompareMps(x.Mps, y.Mps, sortMinMps, sortMaxMps);
        }

        public static int CompareMps(double x, double y, double minMps, double maxMps)
        {
            int xrange, yrange;
            if (x < maxMps && x >= minMps) xrange = 0;
            else if (x >= maxMps) xrange = 1;
            else xrange = 2;
            if (y < maxMps && y >= minMps) yrange = 0;
            else if (y >= maxMps) yrange = 1;
            else yrange = 2;
            int compare = xrange.CompareTo(yrange);
            if (compare != 0) return compare;
            if (xrange == 0 || xrange == 2) return y.CompareTo(x);
            else return x.CompareTo(y);
        }

        private void SplitAt(double time)
        {
            double t = 0;
            for (int i = 0; i < sequence.Count; i++)
            {
                double d = sequence[i].Duration;
                if (sequence[i].IsManaPotionOrGem) d = 0;
                if (t + d > time)
                {
                    if (time > t)
                    {
                        SplitAt(i, time - t);
                    }
                    return;
                }
                t += d;
            }
        }

        private List<SequenceItem> RemoveSuperGroup(int index)
        {
            int i;
            SequenceGroup super = sequence[index].SuperGroup;
            for (i = index; i < sequence.Count; i++)
            {
                if (sequence[i].SuperGroup != super) break;
            }
            List<SequenceItem> copy = sequence.GetRange(index, i - index);
            sequence.RemoveRange(index, i - index);
            return copy;
        }

        private void SplitAt(int index, double time)
        {
            if (time > 0.00001 && time < sequence[index].Duration - 0.00001)
            {
                double d = sequence[index].Duration;
                sequence.Insert(index, sequence[index].Clone());
                sequence[index].Duration = time;
                sequence[index + 1].Duration = d - time;
                if (sequence[index].Group.Count == 0)
                {
                    SequenceGroup newSuperGroup = new SequenceGroup();
                    newSuperGroup.Add(sequence[index]);
                    sequence[index].SuperGroup = newSuperGroup;
                    newSuperGroup = new SequenceGroup();
                    newSuperGroup.Add(sequence[index + 1]);
                    sequence[index + 1].SuperGroup = newSuperGroup;
                    sequence[index].SetTimeConstraint(sequence[index].MinTime, sequence[index].MaxTime + sequence[index + 1].Duration);
                    sequence[index + 1].SetTimeConstraint(sequence[index + 1].MinTime, sequence[index + 1].MaxTime + sequence[index].Duration);
                }
                else if (sequence[index].SuperGroup != null)
                {
                    sequence[index].SuperGroup.Item.Insert(sequence[index].SuperGroup.Item.IndexOf(sequence[index + 1]), sequence[index]);
                }
            }
        }

        private SequenceItem Split(SequenceItem item, double time)
        {
            int index = sequence.IndexOf(item);
            SplitAt(index, time);
            return sequence[index];
        }

        public double RemoveIndex(int index)
        {
            return RemoveIndex(index, 0);
        }

        private double RemoveIndex(int index, int startAt)
        {
            double ret = 0;
            for (int i = startAt; i < sequence.Count; i++)
            {
                if (sequence[i].Index == index)
                {
                    ret += sequence[i].Duration;
                    sequence.RemoveAt(i);
                    i--;
                }
            }
            return ret;
        }

        public double RemoveIndex(VariableType type)
        {
            return RemoveIndex(type, 0);
        }

        private double RemoveIndex(VariableType type, int startAt)
        {
            double ret = 0;
            for (int i = startAt; i < sequence.Count; i++)
            {
                if (sequence[i].VariableType == type)
                {
                    ret += sequence[i].Duration;
                    sequence.RemoveAt(i);
                    i--;
                }
            }
            return ret;
        }

        private SequenceItem InsertIndex(int index, double duration, double time)
        {
            SequenceItem item = new SequenceItem(index, duration);
            SplitAt(time);
            InsertIndex(item, time);
            return item;
        }

        private void InsertIndex(SequenceItem item, double time)
        {
            const double eps = 0.000001;
            double t = 0;
            for (int i = 0; i < sequence.Count; i++)
            {
                double d = sequence[i].Duration;
                if (sequence[i].IsManaPotionOrGem) d = 0;
                if (t + d > time + eps && time <= t + eps)
                {
                    // splitting should have been done beforehand
                    sequence.Insert(i, item);
                    return;
                    /*if (time <= t + eps)
                    {
                        sequence.Insert(i, item);
                        return;
                    }
                    else
                    {
                        sequence.Insert(i, sequence[i].Clone());
                        sequence[i].Duration = time - t;
                        sequence[i + 1].Duration = d - (time - t);
                        sequence.Insert(i + 1, item);
                        return;
                    }*/
                }
                t += d;
            }
        }

        public enum EvaluationMode
        {
            Unexplained,
            ManaBelow,
            ManaAtTime,
            CooldownBreak,
        }

        private DisplayCalculations Calculations
        {
            get
            {
                return SequenceItem.Calculations;
            }
        }

        private Stats BaseStats
        {
            get
            {
                return SequenceItem.Calculations.BaseStats;
            }
        }

        private double EvocationRegen
        {
            get
            {
                return SequenceItem.Calculations.EvocationRegen;
            }
        }

        private double EvocationDuration
        {
            get
            {
                return SequenceItem.Calculations.EvocationDuration;
            }
        }

        private double FightDuration
        {
            get
            {
                return SequenceItem.Calculations.CalculationOptions.FightDuration;
            }
        }

        private double ManaGemEffectDuration
        {
            get
            {
                return SequenceItem.Calculations.ManaGemEffectDuration;
            }
        }

        public List<SequenceGroup> GroupSpecialEffect(EffectCooldown type)
        {
            List<SequenceItem> list = new List<SequenceItem>();
            foreach (SequenceItem item in sequence)
            {
                if (item.CastingState.EffectsActive(type)) list.Add(item);
            }
            return GroupCooldown(list, type.Duration, type.Cooldown, type);
        }

        public List<SequenceGroup> GroupManaGemEffect()
        {
            List<SequenceItem> list = new List<SequenceItem>();
            foreach (SequenceItem item in sequence)
            {
                if (item.CastingState.ManaGemEffect) list.Add(item);
            }
            if (list.Count > 0) return GroupCooldown(list, ManaGemEffectDuration, 120f, Calculations.EffectCooldown[(int)StandardEffect.ManaGemEffect]);
            return null;
        }

        public void ConstrainTrinkets(List<SequenceGroup> t1, List<SequenceGroup> t2)
        {
            if (t1.Count == 0 || t2.Count == 0) return;
            foreach (SequenceGroup g1 in t1)
            {
                foreach (SequenceGroup g2 in t2)
                {
                    g1.Constraint.Add(new CooldownConstraint() { Group = g2, Cooldown = g2.Duration });
                    g2.Constraint.Add(new CooldownConstraint() { Group = g1, Cooldown = g2.Duration });
                }
            }
        }

        public void GroupCombustion()
        {
            List<SequenceItem> list = new List<SequenceItem>();
            foreach (SequenceItem item in sequence)
            {
                if (item.CastingState.Combustion) list.Add(item);
            }
            if (list.Count > 0) GroupCooldown(list, 0, SequenceItem.Calculations.CombustionCooldown + 15.0, true, false, Calculations.EffectCooldown[(int)StandardEffect.Combustion], VariableType.None, 0.0);
        }

        public void GroupArcanePower()
        {
            List<SequenceItem> list = new List<SequenceItem>();
            foreach (SequenceItem item in sequence)
            {
                if (item.CastingState.ArcanePower) list.Add(item);
            }
            if (list.Count > 0) GroupCooldown(list, SequenceItem.Calculations.ArcanePowerDuration, SequenceItem.Calculations.ArcanePowerCooldown, Calculations.EffectCooldown[(int)StandardEffect.ArcanePower]);
        }

        public void GroupPowerInfusion()
        {
            List<SequenceItem> list = new List<SequenceItem>();
            foreach (SequenceItem item in sequence)
            {
                if (item.CastingState.PowerInfusion) list.Add(item);
            }
            if (list.Count > 0) GroupCooldown(list, SequenceItem.Calculations.PowerInfusionDuration, SequenceItem.Calculations.PowerInfusionCooldown, Calculations.EffectCooldown[(int)StandardEffect.PowerInfusion]);
        }

        public void GroupEvocation()
        {
            List<SequenceItem> list = new List<SequenceItem>();
            foreach (SequenceItem item in sequence)
            {
                if (item.VariableType == VariableType.EvocationIV) list.Add(item);
            }
            if (list.Count > 0) GroupCooldown(list, SequenceItem.Calculations.EvocationDurationIV, SequenceItem.Calculations.EvocationCooldown, Calculations.EffectCooldown[(int)StandardEffect.Evocation]);
            list.Clear();
            foreach (SequenceItem item in sequence)
            {
                if (item.VariableType == VariableType.EvocationHero) list.Add(item);
            }
            if (list.Count > 0) GroupCooldown(list, SequenceItem.Calculations.EvocationDurationHero, SequenceItem.Calculations.EvocationCooldown, Calculations.EffectCooldown[(int)StandardEffect.Evocation]);
            list.Clear();
            foreach (SequenceItem item in sequence)
            {
                if (item.VariableType == VariableType.EvocationIVHero) list.Add(item);
            }
            if (list.Count > 0) GroupCooldown(list, SequenceItem.Calculations.EvocationDurationIVHero, SequenceItem.Calculations.EvocationCooldown, Calculations.EffectCooldown[(int)StandardEffect.Evocation]);
            list.Clear();
            foreach (SequenceItem item in sequence)
            {
                if (item.VariableType == VariableType.Evocation) list.Add(item);
            }
            if (list.Count > 0) GroupCooldown(list, SequenceItem.Calculations.EvocationDuration, SequenceItem.Calculations.EvocationCooldown, Calculations.EffectCooldown[(int)StandardEffect.Evocation]);
        }

        public void GroupIcyVeins()
        {
            List<SequenceItem> list = new List<SequenceItem>();
            foreach (SequenceItem item in sequence)
            {
                if (item.CastingState.IcyVeins) list.Add(item);
            }
            if (list.Count > 0) GroupCooldown(list, 20.0, SequenceItem.Calculations.IcyVeinsCooldown, false, SequenceItem.Calculations.Character.MageTalents.ColdSnap == 1, Calculations.EffectCooldown[(int)StandardEffect.IcyVeins], VariableType.None, 0.0);
        }

        public void GroupWaterElemental()
        {
            List<SequenceItem> list = new List<SequenceItem>();
            foreach (SequenceItem item in sequence)
            {
                if (item.CastingState.WaterElemental) list.Add(item);
            }
            if (list.Count > 0) GroupCooldown(list, SequenceItem.Calculations.WaterElementalDuration, SequenceItem.Calculations.WaterElementalCooldown, false, SequenceItem.Calculations.Character.MageTalents.ColdSnap == 1, Calculations.EffectCooldown[(int)StandardEffect.WaterElemental], VariableType.SummonWaterElemental, SequenceItem.Calculations.BaseGlobalCooldown);
        }

        public void GroupMirrorImage()
        {
            List<SequenceItem> list = new List<SequenceItem>();
            foreach (SequenceItem item in sequence)
            {
                if (item.CastingState.MirrorImage) list.Add(item);
            }
            if (list.Count > 0) GroupCooldown(list, SequenceItem.Calculations.MirrorImageDuration, SequenceItem.Calculations.MirrorImageCooldown, false, false, Calculations.EffectCooldown[(int)StandardEffect.MirrorImage], VariableType.SummonMirrorImage, SequenceItem.Calculations.BaseGlobalCooldown);
        }

        public List<SequenceGroup> GroupFlameCap()
        {
            List<SequenceItem> list = new List<SequenceItem>();
            foreach (SequenceItem item in sequence)
            {
                if (item.CastingState.FlameCap) list.Add(item);
            }
            if (list.Count > 0) return GroupCooldown(list, 60, 180, Calculations.EffectCooldown[(int)StandardEffect.FlameCap]);
            return null;
        }

        public void GroupPotionOfWildMagic()
        {
            List<SequenceItem> list = new List<SequenceItem>();
            foreach (SequenceItem item in sequence)
            {
                if (item.CastingState.PotionOfWildMagic) list.Add(item);
            }
            if (list.Count > 0) GroupCooldown(list, 15, 120, Calculations.EffectCooldown[(int)StandardEffect.PotionOfWildMagic]);
        }

        public void GroupPotionOfSpeed()
        {
            List<SequenceItem> list = new List<SequenceItem>();
            foreach (SequenceItem item in sequence)
            {
                if (item.CastingState.PotionOfSpeed) list.Add(item);
            }
            if (list.Count > 0) GroupCooldown(list, 15, 120, Calculations.EffectCooldown[(int)StandardEffect.PotionOfSpeed]);
        }

        public void GroupBerserking()
        {
            List<SequenceItem> list = new List<SequenceItem>();
            foreach (SequenceItem item in sequence)
            {
                if (item.CastingState.Berserking) list.Add(item);
            }
            if (list.Count > 0) GroupCooldown(list, 10, 180, false, false, Calculations.EffectCooldown[(int)StandardEffect.Berserking], VariableType.None, 0.0);
        }

        private List<SequenceGroup> GroupCooldown(List<SequenceItem> cooldownItems, double maxDuration, double cooldown, EffectCooldown type)
        {
            return GroupCooldown(cooldownItems, maxDuration, cooldown, false, false, type, VariableType.None, 0.0);
        }

        private bool ItemsCompatible(List<SequenceItem> item1, List<SequenceItem> item2, double maxCooldown)
        {
            return ConstraintsCompatible(GetAllGroups(item1), GetAllGroups(item2), maxCooldown);
        }

        private bool ItemsCompatible(List<SequenceItem> item1, SequenceItem item2, double maxCooldown)
        {
            return ConstraintsCompatible(GetAllGroups(item1), item2.Group, maxCooldown);
        }

        private List<SequenceGroup> GetAllGroups(List<SequenceItem> items)
        {
            //return items.Aggregate<SequenceItem, IEnumerable<SequenceGroup>>(new List<SequenceGroup>(), (list, item) => list.Union(item.Group)).ToList();
            List<SequenceGroup> result = new List<SequenceGroup>();
            foreach (SequenceItem item in items)
            {
                foreach (SequenceGroup group in item.Group)
                {
                    if (!result.Contains(group)) result.Add(group);
                }
            }
            return result;
        }

        private bool ConstraintsCompatible(List<SequenceGroup> group1, List<SequenceGroup> group2, double maxCooldown)
        {
            foreach (SequenceGroup group in group1)
            {
                foreach (CooldownConstraint constraint in group.Constraint)
                {
                    if (constraint.Cooldown > maxCooldown && group2.Contains(constraint.Group))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool InActivationDistance(int seg1, int seg2, double effectDuration)
        {
            if (seg1 > seg2)
            {
                int tmp = seg1;
                seg1 = seg2;
                seg2 = tmp;
            }
            return SequenceItem.Calculations.SegmentList[seg2].TimeStart - SequenceItem.Calculations.SegmentList[seg1].TimeEnd < effectDuration - 0.00001;
        }

        private List<SequenceGroup> GroupCooldown(List<SequenceItem> cooldownItems, double maxDuration, double cooldown, bool combustionMode, bool coldSnapMode, EffectCooldown type, VariableType activation, double activationDuration)
        {
            const double eps = 0.00001;
            List<SequenceGroup> existingGroup = new List<SequenceGroup>();
            List<SequenceGroup> unresolvedGroup = new List<SequenceGroup>();
            foreach (SequenceItem item in cooldownItems)
            {
                foreach (SequenceGroup group in item.Group)
                {
                    if (!existingGroup.Contains(group)) existingGroup.Add(group);
                }
            }
            List<List<SequenceItem>> chains = new List<List<SequenceItem>>();
            foreach (SequenceGroup group in existingGroup)
            {
                // if group duration is less than cooldown then all must be contiguous
                if (group.Duration <= cooldown)
                {
                    // check if any of the items is already an existing chain
                    List<SequenceItem> chain = null;
                    foreach (SequenceItem item in group.Item)
                    {
                        if (cooldownItems.Contains(item))
                        {
                            for (int i = 0; i < chains.Count; i++)
                            {
                                List<SequenceItem> c = chains[i];
                                if (c.Contains(item))
                                {
                                    if (chain != null && chain != c)
                                    {
                                        // merge chains
                                        chain.AddRange(c);
                                        chains.RemoveAt(i);
                                        i--;
                                    }
                                    else
                                    {
                                        chain = c;
                                    }
                                }
                            }
                        }
                    }
                    // add new items to chain
                    if (chain == null)
                    {
                        chain = new List<SequenceItem>();
                        chains.Add(chain);
                    }
                    foreach (SequenceItem item in group.Item)
                    {
                        if (cooldownItems.Contains(item))
                        {
                            if (!chain.Contains(item)) chain.Add(item);
                        }
                    }
                }
                else
                {
                    unresolvedGroup.Add(group);
                }
            }
            List<SequenceItem> unchained = new List<SequenceItem>();
            double totalDuration = 0;
            double combustionCount = 0;
            foreach (SequenceItem item in cooldownItems)
            {
                totalDuration += item.Duration;
                if (combustionMode) combustionCount += item.Duration / (item.CastingState.CombustionDuration * item.Cycle.CastTime / item.Cycle.CastProcs);
                bool chained = false;
                foreach (List<SequenceItem> chain in chains)
                {
                    if (chain.Contains(item))
                    {
                        chained = true;
                        break;
                    }
                }
                if (!chained) unchained.Add(item);
            }
            // at this point we have a number of chains and remaining unchained items
            // chains cannot be split, unchained items can
            int maxChains = 0;
            if (combustionMode)
            {
                maxChains = (int)Math.Ceiling(combustionCount - eps);
            }
            else
            {
                maxChains = (int)Math.Ceiling((totalDuration - eps) / maxDuration);
            }
            List<SequenceGroup> partialGroups = new List<SequenceGroup>();
            foreach (List<SequenceItem> chain in chains)
            {
                SequenceGroup group = new SequenceGroup();
                group.AddRange(chain);
                partialGroups.Add(group);
            }
            partialGroups.Sort((x, y) => y.Duration.CompareTo(x.Duration));
            if (partialGroups.Count < maxChains)
            {
                int addCount = maxChains - partialGroups.Count;
                for (int i = 0; i < addCount; i++)
                {
                    partialGroups.Add(new SequenceGroup());
                }
            }

            for (int i = 0; i < partialGroups.Count; i++)
            {
                SequenceGroup group = partialGroups[i];
                double gap = 0.0;
                double activationGap = 0.0;
                if (combustionMode)
                {
                    double tempSum = 0;
                    foreach (SequenceItem item in group.Item)
                        tempSum += item.Duration / (item.CastingState.CombustionDuration * item.Cycle.CastTime / item.Cycle.CastProcs);
                    gap = 1 - tempSum;
                }
                else
                {
                    gap = maxDuration - group.Duration;
                    if (activationDuration > 0.0)
                    {
                        activationGap = activationDuration;
                        foreach (SequenceItem item in group.Item)
                        {
                            if (item.VariableType == activation) activationGap -= item.Duration;
                        }
                    }
                }
                if (gap < -eps && coldSnapMode)
                {
                    gap += maxDuration;
                    activationGap += activationDuration;
                }
                if (gap > eps)
                {
                    //int maxSegDistance = (int)Math.Ceiling(maxDuration / 30.0) + 1;
                    //if (combustionMode) maxSegDistance = (int)Math.Ceiling(15.0 / 30.0) + 1;
                    double allowedDistance = maxDuration;
                    if (combustionMode) allowedDistance = 15.0;
                    for (int j = i + 1; j < partialGroups.Count; j++)
                    {
                        SequenceGroup subgroup = partialGroups[j];
                        double gapReduction = 0.0;
                        double activationGapReduction = 0.0;
                        if (combustionMode)
                        {
                            double tempSum = 0;
                            foreach (SequenceItem item in subgroup.Item)
                                tempSum += item.Duration / (item.CastingState.CombustionDuration * item.Cycle.CastTime / item.Cycle.CastProcs);
                            gapReduction = tempSum;
                        }
                        else
                        {
                            gapReduction = subgroup.Duration;
                            if (activationDuration > 0.0)
                            {
                                foreach (SequenceItem item in group.Item)
                                {
                                    if (item.VariableType == activation) activationGapReduction += item.Duration;
                                }
                            }
                        }
                        if (subgroup.Duration > 0 && gapReduction <= gap + eps && activationGapReduction <= activationGap + eps && gap - gapReduction >= activationGap - activationGapReduction - eps && InActivationDistance(group.Segment, subgroup.Segment, allowedDistance) && ItemsCompatible(group.Item, subgroup.Item, 0))
                        {
                            gap -= gapReduction;
                            activationGap -= activationGapReduction;
                            group.AddRange(subgroup.Item);
                            partialGroups.RemoveAt(j);
                            j--;
                            if (partialGroups.Count < maxChains)
                            {
                                int addCount = maxChains - partialGroups.Count;
                                for (int k = 0; k < addCount; k++)
                                {
                                    partialGroups.Add(new SequenceGroup());
                                }
                            }
                            if (gap < eps) break;
                        }
                    }
                    if (gap > eps)
                    {
                        for (int j = 0; j < unchained.Count; j++)
                        {
                            SequenceItem item = unchained[j];
                            if (group.Segment == -1 || InActivationDistance(group.Segment, item.Segment, allowedDistance))
                            {
                                double gapReduction = 0.0;
                                double activationGapReduction = 0.0;
                                if (combustionMode)
                                {
                                    gapReduction = item.Duration / (item.CastingState.CombustionDuration * item.Cycle.CastTime / item.Cycle.CastProcs);
                                }
                                else
                                {
                                    gapReduction = item.Duration;
                                    if (activationDuration > 0.0 && item.VariableType == activation) activationGapReduction = item.Duration;
                                }
                                if (gapReduction <= gap + eps && activationGapReduction <= activationGap + eps && gap - gapReduction >= activationGap - activationGapReduction - eps)
                                {
                                    gap -= gapReduction;
                                    activationGap -= activationGapReduction;
                                    group.Add(item);
                                    unchained.RemoveAt(j);
                                    j--;
                                    if (gap < eps) break;
                                }
                                else if ((activationGapReduction > eps && activationGap > eps) || (activationGapReduction == 0.0 && gap > activationGap + eps))
                                {
                                    double split = 0;
                                    if (combustionMode)
                                    {
                                        split = gap * (item.CastingState.CombustionDuration * item.Cycle.CastTime / item.Cycle.CastProcs);
                                    }
                                    else if (activationGapReduction > eps && activationGap > eps)
                                    {
                                        split = activationGap;
                                    }
                                    else
                                    {
                                        split = gap - activationGap;
                                    }
                                    group.Add(Split(item, split));
                                    if (activationDuration > 0.0 && item.VariableType == activation)
                                    {
                                        activationGap -= split;
                                        gap -= split;
                                    }
                                    else
                                    {
                                        gap = activationGap;
                                        if (gap < eps) break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            partialGroups.RemoveAll(group => group.Duration == 0);
            // finalize groups
            for (int i = 0; i < partialGroups.Count; i++)
            {
                SequenceGroup group = partialGroups[i];
                foreach (SequenceItem item in group.Item)
                {
                    item.Group.Add(group);
                }
                for (int j = 0; j < partialGroups.Count; j++)
                {
                    if (i != j)
                    {
                        group.Constraint.Add(new CooldownConstraint() { Cooldown = cooldown, Duration = maxDuration, Group = partialGroups[j], ColdSnap = coldSnapMode, EffectCooldown = type });
                    }
                }
            }
            return partialGroups;
        }

        public SequenceGroup GroupHeroism()
        {
            SequenceGroup group = new SequenceGroup();
            foreach (SequenceItem item in sequence)
            {
                if (item.CastingState.Heroism)
                {
                    group.Add(item);
                    item.Group.Add(group);
                }
            }
            return group;
        }

        private double moltenFuryStart = 0;
        public void GroupMoltenFury()
        {
            SequenceGroup group = new SequenceGroup();
            foreach (SequenceItem item in sequence)
            {
                if (item.CastingState.MoltenFury)
                {
                    group.Add(item);
                    item.Group.Add(group);
                }
            }
            moltenFuryStart = SequenceItem.Calculations.CalculationOptions.FightDuration - group.Duration;
            group.MinTime = moltenFuryStart;
        }

        List<SequenceItem> compactItems;
        List<double> compactTime;
        double compactTotalTime;
        int compactGroupSplits;
        int compactInversions;
        //double compactLastDestro;

        public bool SortGroups(Solver solver)
        {
            const double eps = 0.000001;
            List<SequenceItem> groupedItems = new List<SequenceItem>();
            foreach (SequenceItem item in sequence)
            {
                if (item.Group.Count > 0)
                {
                    groupedItems.Add(item);
                }
            }
            compactTotalTime = double.PositiveInfinity;
            compactGroupSplits = int.MaxValue;
            compactInversions = int.MaxValue;
            //compactLastDestro = double.NegativeInfinity;
            //SortGroups_AddRemainingItems(new List<SequenceItem>(), new List<double>(), groupedItems);
            groupedItems.Sort((x, y) => x.Segment.CompareTo(y.Segment));
            if (Calculations.CalculationOptions.DisplaySegmentCooldowns)
            {
                SortGroups_ComputeSegmented(groupedItems, solver);
            }
            else
            {
                SortGroups_Compute(groupedItems, solver);
            }
            if (compactItems == null)
            {
                //return false;
            }
            else /*if (compactItems != null)*/
            {
                for (int i = 0; i < compactItems.Count; i++)
                {
                    compactItems[i].MinTime = compactTime[i];
                }

                // compute max time
                double time = FightDuration;
                for (int i = compactItems.Count - 1; i >= 0; i--)
                {
                    SequenceItem item = compactItems[i];
                    time = Math.Min(time - item.Duration, item.MaxTime);
                    // check constraints
                    foreach (SequenceGroup group in item.Group)
                    {
                        // only compute max for first item in group
                        if (i == 0 || !compactItems[i - 1].Group.Contains(group))
                        {
                            foreach (CooldownConstraint constraint in group.Constraint)
                            {
                                for (int j = i + 1; j < compactItems.Count; j++)
                                {
                                    // skip cooldown constraints that are coldsnapped in the solution
                                    if (compactItems[j].Group.Contains(constraint.Group) && (!constraint.ColdSnap || (compactItems[j].MinTime - item.MinTime >= SequenceItem.Calculations.IcyVeinsCooldown - eps)))
                                    {
                                        time = Math.Min(time, compactItems[j].MaxTime - constraint.Cooldown);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    item.MaxTime = time;
                    double t = time;
                    for (int j = i + 1; j < compactItems.Count && compactItems[j].SuperIndex == compactItems[j - 1].SuperIndex; j++)
                    {
                        t += compactItems[j - 1].Duration;
                        compactItems[j].MaxTime = t;
                    }
                }
            }

            sequence.Sort((x, y) =>
            {
                bool xgrouped = x.Group.Count > 0;
                bool ygrouped = y.Group.Count > 0;
                int compare = xgrouped.CompareTo(ygrouped);
                if (compare != 0) return compare;
                compare = x.MinTime.CompareTo(y.MinTime);
                if (compare != 0) return compare;
                compare = x.Segment.CompareTo(y.Segment);
                return compare;
            });
            return true;
        }

        private int SortGroups_Compare(SequenceItem x, SequenceItem y, List<SequenceGroup> tail)
        {
            bool xsingletail = x.Tail.Count > 0;
            bool ysingletail = y.Tail.Count > 0;
            int compare = ysingletail.CompareTo(xsingletail);
            if (compare != 0) return compare;
            int xintersect = (tail == null) ? 0 : Rawr.Mage.ListUtils.Intersect<SequenceGroup>(x.Group, tail).Count;
            int yintersect = (tail == null) ? 0 : Rawr.Mage.ListUtils.Intersect<SequenceGroup>(y.Group, tail).Count;
            return yintersect.CompareTo(xintersect);
        }

        private int HexCount(int hex)
        {
            int count = 0;
            while (hex > 0)
            {
                count += (hex & 1);
                hex >>= 1;
            }
            return count;
        }

        private void SortGroups_Compute(List<SequenceItem> itemList, Solver solver)
        {
            const double eps = 0.000001;
            int N = itemList.Count;
            if (N == 0) return;
            List<double> constructionTime = new List<double>();
            List<double>[] constructionTimeHistory = new List<double>[N];
            bool[] used = new bool[N];
            int[] index = new int[N];
            int[] coldsnap = new int[N];
            double[] coldsnapTime = new double[N];
            int[] maxIntersect = new int[N];
            for (int j = 0; j < N; j++) itemList[j].SuperIndex = -1;
            int super = -1;
            for (int j = 0; j < N; j++)
            {
                if (itemList[j].SuperIndex == -1)
                {
                    super++;
                    itemList[j].SuperIndex = super;
                    List<SequenceItem> superList = new List<SequenceItem>();
                    superList.Add(itemList[j]);
                    bool more = false;
                    do
                    {
                        more = false;
                        for (int k = j + 1; k < N; k++)
                        {
                            if (itemList[k].SuperIndex == -1)
                            {
                                foreach (SequenceItem item in superList)
                                {
                                    if (Rawr.Mage.ListUtils.Intersect<SequenceGroup>(item.Group, itemList[k].Group).Count > 0)
                                    {
                                        itemList[k].SuperIndex = super;
                                        superList.Add(itemList[k]);
                                        more = true;
                                        break;
                                    }
                                }
                            }
                        }
                    } while (more);
                    List<SequenceGroup> superGroups = new List<SequenceGroup>();
                    foreach (SequenceItem item in superList)
                    {
                        int hex = 0;
                        foreach (SequenceGroup group in item.Group)
                        {
                            if (!superGroups.Contains(group)) superGroups.Add(group);
                            hex |= (1 << superGroups.IndexOf(group));
                        }
                        item.CooldownHex = hex;
                    }
                }
            }
            List<SequenceGroup> groupList = GetAllGroups(itemList);
            super++;
            int[] superLeft = new int[super];
            for (int j = 0; j < N; j++)
            {
                superLeft[itemList[j].SuperIndex]++;
                foreach (SequenceGroup group in itemList[j].Group)
                {
                    group.OrderIndex = -1;
                }
            }
            int maxSegment = 0;
            for (int j = 0; j < N; j++)
            {
                if (itemList[j].Segment > maxSegment)
                {
                    maxSegment = itemList[j].Segment;
                }
            }
            int[] earlierCount = new int[maxSegment + 1];
            for (int j = 0; j < N; j++)
            {
                for (int k = itemList[j].Segment + 1; k <= maxSegment; k++)
                {
                    earlierCount[k]++;
                }
            }

            int i = 0;
            index[0] = 0;
            constructionTimeHistory[0] = constructionTime;
            coldsnap[0] = 2;
            int tickLimit = Environment.TickCount + 5000;
            do
            {
                if (i == N)
                {
                    double time = 0;
                    if (constructionTime.Count > 0) time = constructionTime[constructionTime.Count - 1] + itemList[index[N - 1]].Duration;
                    // compute group splits
                    int groupSplits = 0;
                    //double lastDestro = double.NegativeInfinity;
                    foreach (SequenceGroup group in groupList)
                    {
                        /*if (group.Item.Count > 0 && group.Item[0].CastingState.PotionOfWildMagic)
                        {
                            double min = double.PositiveInfinity;
                            foreach (SequenceItem item in group.Item)
                            {
                                min = Math.Min(min, constructionTime[item.OrderIndex]);
                            }
                            if (min > lastDestro) lastDestro = min;
                        }*/
                        int minIndex = N - 1;
                        int maxIndex = 0;
                        foreach (SequenceItem item in group.Item)
                        {
                            if (item.OrderIndex < minIndex) minIndex = item.OrderIndex;
                            if (item.OrderIndex > maxIndex) maxIndex = item.OrderIndex;
                        }
                        groupSplits += (maxIndex - minIndex + 1) - group.Item.Count;
                    }
                    //if (lastDestro < FightDuration - 120.0) lastDestro = double.NegativeInfinity;
                    if (groupSplits < compactGroupSplits || (groupSplits == compactGroupSplits && time < compactTotalTime) /*|| (groupSplits == compactGroupSplits && time == compactTotalTime && lastDestro > compactLastDestro)*/)
                    {
                        compactGroupSplits = groupSplits;
                        compactTotalTime = time;
                        //compactLastDestro = lastDestro;
                        compactTime = new List<double>(constructionTime);
                        compactItems = new List<SequenceItem>();
                        for (int j = 0; j < N; j++)
                        {
                            compactItems.Add(itemList[index[j]]);
                        }
                    }
                    i--;
                    if (i >= 0)
                    {
                        constructionTime = constructionTimeHistory[i];
                        used[index[i]] = false;
                        superLeft[itemList[index[i]].SuperIndex]++;
                        foreach (SequenceGroup group in itemList[index[i]].Group)
                        {
                            if (group.OrderIndex == i)
                            {
                                group.OrderIndex = -1;
                            }
                        }
                    }
                }
                else
                {
                    coldsnap[i]--;
                    if (coldsnap[i] < 0 || used[index[i]])
                    {
                        coldsnap[i] = 1;
                        do
                        {
                            index[i]++;
                        } while (index[i] < N && used[index[i]]);
                    }
                    if (index[i] == N)
                    {
                        i--;
                        if (i >= 0)
                        {
                            constructionTime = constructionTimeHistory[i];
                            used[index[i]] = false;
                            superLeft[itemList[index[i]].SuperIndex]++;
                            foreach (SequenceGroup group in itemList[index[i]].Group)
                            {
                                if (group.OrderIndex == i)
                                {
                                    group.OrderIndex = -1;
                                }
                            }
                        }
                    }
                    else
                    {
                        // check if valid
                        SequenceItem item = itemList[index[i]];
                        // if we have segmentation data take a more directed search, respect segmentation ordering (jumps should be eliminated, but check back in case something looks strange)
                        if (i == 0 || ((item.SuperIndex == itemList[index[i - 1]].SuperIndex || superLeft[itemList[index[i - 1]].SuperIndex] == 0) && item.Segment >= itemList[index[i - 1]].Segment && i >= earlierCount[item.Segment]))
                        {
                            int tail = item.CooldownHex;
                            int activeTail = 0;
                            int intersectHex = 0;
                            if (i > 0 && item.SuperIndex == itemList[index[i - 1]].SuperIndex)
                            {
                                activeTail = itemList[index[i - 1]].CooldownHex;
                                intersectHex = HexCount(tail & activeTail);
                                if (intersectHex > maxIntersect[i]) maxIntersect[i] = intersectHex;
                            }
                            if (intersectHex >= maxIntersect[i])
                            {
                                used[index[i]] = true;
                                itemList[index[i]].OrderIndex = i;
                                superLeft[itemList[index[i]].SuperIndex]--;
                                foreach (SequenceGroup group in itemList[index[i]].Group)
                                {
                                    if (group.OrderIndex == -1)
                                    {
                                        // first item in this group
                                        group.OrderIndex = i;
                                    }
                                }
                                // skip tests for coldsnap == 0
                                //if (coldsnap[i] == 1) // just compute it, if you want to optimize take a bit more time to think about it
                                if (superLeft[itemList[index[i]].SuperIndex] > 0)
                                {
                                    for (int j = 0; j < used.Length; j++)
                                    {
                                        if (!used[j])
                                        {
                                            if (itemList[j].SuperIndex == item.SuperIndex)
                                            {
                                                // make sure activations are placed before use
                                                if (item.CastingState.WaterElemental && item.VariableType != VariableType.SummonWaterElemental && itemList[j].VariableType == VariableType.SummonWaterElemental)
                                                {
                                                    tail = 0;
                                                    break;
                                                }
                                                if (item.CastingState.MirrorImage && item.VariableType != VariableType.SummonMirrorImage && itemList[j].VariableType == VariableType.SummonMirrorImage)
                                                {
                                                    tail = 0;
                                                    break;
                                                }
                                                if (i > 0 && item.SuperIndex == itemList[index[i - 1]].SuperIndex)
                                                {
                                                    int intersectHexJ = HexCount(itemList[j].CooldownHex & activeTail);
                                                    if (intersectHexJ > intersectHex)
                                                    {
                                                        if (j > index[i] && (i == 0 || itemList[j].Segment >= itemList[index[i - 1]].Segment))
                                                        {
                                                            // anything up to j is not valid, so skip ahead
                                                            used[index[i]] = false;
                                                            foreach (SequenceGroup group in itemList[index[i]].Group)
                                                            {
                                                                if (group.OrderIndex == i)
                                                                {
                                                                    group.OrderIndex = -1;
                                                                }
                                                            }
                                                            superLeft[itemList[index[i]].SuperIndex]++;
                                                            index[i] = j;
                                                            used[j] = true;
                                                            itemList[j].OrderIndex = i;
                                                            superLeft[itemList[index[i]].SuperIndex]--;
                                                            foreach (SequenceGroup group in itemList[j].Group)
                                                            {
                                                                if (group.OrderIndex == -1)
                                                                {
                                                                    // first item in this group
                                                                    group.OrderIndex = i;
                                                                }
                                                            }
                                                            intersectHex = intersectHexJ;
                                                            maxIntersect[i] = intersectHex;
                                                            item = itemList[j];
                                                            tail = item.CooldownHex;
                                                            j = -1;
                                                            continue;
                                                        }
                                                        else
                                                        {
                                                            // invalidate
                                                            tail = 0;
                                                            break;
                                                        }
                                                    }
                                                }
                                                if ((itemList[j].CooldownHex & activeTail & ~item.CooldownHex) != 0) // strong requirement, if converting for reconstruction on nonsegmented data might have to remove this
                                                {
                                                    // we'll have to place something from active tail that is being removed
                                                    // from active tail
                                                    tail = 0;
                                                    break;
                                                }
                                                int intersect = item.CooldownHex & itemList[j].CooldownHex;
                                                if (intersect > 0)
                                                {
                                                    tail = intersect & tail;
                                                    if (tail == 0) break;
                                                }
                                            }
                                        }
                                    }
                                }
                                double time = 0;
                                if (constructionTime.Count > 0) time = constructionTime[constructionTime.Count - 1] + itemList[index[i - 1]].Duration;
                                time = Math.Max(time, item.MinTime);
                                double minTotalTime = time + item.Duration;
                                //for (int j = 0; j < N; j++) // too expensive
                                //{
                                //    if (!used[j]) minTotalTime += itemList[j].Duration;
                                //}
                                if (tail > 0 && minTotalTime < compactTotalTime)
                                {
                                    // check constraints
                                    List<int> icyVeinsStarts = new List<int>();
                                    List<int> waterElementalStarts = new List<int>();
                                    SequenceGroup icyVeinsGroup = null;
                                    SequenceGroup waterElementalGroup = null;
                                    foreach (SequenceGroup group in item.Group)
                                    {
                                        foreach (CooldownConstraint constraint in group.Constraint)
                                        {
                                            if (!constraint.ColdSnap)
                                            {
                                                //for (int j = 0; j < i; j++)
                                                //{
                                                //    if (itemList[index[j]].Group.Contains(constraint.Group))
                                                //    {
                                                //        time = Math.Max(time, constructionTime[j] + constraint.Cooldown);
                                                //        break;
                                                //    }
                                                //}
                                                int j = constraint.Group.OrderIndex;
                                                if (j >= 0)
                                                {
                                                    time = Math.Max(time, constructionTime[j] + constraint.Cooldown);
                                                }
                                            }
                                            else
                                            {
                                                // if we're in group with already placed item then no need to redo all this
                                                if (i > 0 && itemList[index[i - 1]].Group.Contains(group)) continue;
                                                if (constraint.EffectCooldown == (int)StandardEffect.IcyVeins) icyVeinsGroup = group;
                                                if (constraint.EffectCooldown == (int)StandardEffect.WaterElemental) waterElementalGroup = group;
                                                int minIndex = i;
                                                foreach (SequenceItem coldsnapItem in constraint.Group.Item)
                                                {
                                                    if (coldsnapItem.OrderIndex >= 0 && coldsnapItem.OrderIndex < N && index[coldsnapItem.OrderIndex] < N && itemList[index[coldsnapItem.OrderIndex]] == coldsnapItem && used[index[coldsnapItem.OrderIndex]] && coldsnapItem.OrderIndex < minIndex)
                                                    {
                                                        minIndex = coldsnapItem.OrderIndex;
                                                    }
                                                }
                                                if (minIndex < i)
                                                {
                                                    if (constraint.EffectCooldown == (int)StandardEffect.IcyVeins) icyVeinsStarts.Add(minIndex);
                                                    if (constraint.EffectCooldown == (int)StandardEffect.WaterElemental) waterElementalStarts.Add(minIndex);
                                                }
                                            }
                                        }
                                    }
                                    bool valid = true;
                                    // we absolutely can't come faster than time
                                    // now check coldsnap constraints
                                    // the constraints should link to all the other icy veins/water elemental groups
                                    // look at the ones that were placed already and sort them by order index
                                    // if the last one that needed coldsnap is farther than coldsnap cooldown then we can use it again
                                    // if we don't need to use coldsnap anyway then adjust coldsnap to 0
                                    if (icyVeinsGroup != null || waterElementalGroup != null)
                                    {
                                        // this is only called for first coldsnap item in group
                                        icyVeinsStarts.Sort();
                                        waterElementalStarts.Sort();
                                        int lastColdsnap = -1;
                                        for (int j = 0; j < i; j++)
                                        {
                                            if (coldsnap[j] == 1) lastColdsnap = j;
                                        }
                                        if ((icyVeinsGroup != null && icyVeinsGroup.Duration > 20.0 + eps) || (waterElementalGroup != null && waterElementalGroup.Duration > SequenceItem.Calculations.WaterElementalDuration + eps))
                                        {
                                            // we need internal coldsnap
                                            if (coldsnap[i] == 1)
                                            {
                                                double normalTime = time;
                                                if (icyVeinsGroup != null && icyVeinsStarts.Count > 0) normalTime = Math.Max(normalTime, constructionTime[icyVeinsStarts[icyVeinsStarts.Count - 1]] + SequenceItem.Calculations.IcyVeinsCooldown);
                                                if (waterElementalGroup != null && waterElementalStarts.Count > 0) normalTime = Math.Max(normalTime, constructionTime[waterElementalStarts[waterElementalStarts.Count - 1]] + SequenceItem.Calculations.WaterElementalCooldown);
                                                double coldsnapReady = 0;
                                                if (lastColdsnap >= 0) coldsnapReady = coldsnapTime[lastColdsnap] + SequenceItem.Calculations.ColdsnapCooldown;
                                                // we have to do first one on normal time and have coldsnap ready in the middle
                                                time = normalTime;
                                                if (icyVeinsGroup != null) time = Math.Max(time, coldsnapReady - 20.0);
                                                if (waterElementalGroup != null) time = Math.Max(time, coldsnapReady - SequenceItem.Calculations.WaterElementalDuration);
                                                coldsnapTime[i] = Math.Max(time, coldsnapReady);
                                            }
                                            else
                                            {
                                                // can't do without coldsnap
                                                valid = false;
                                            }
                                        }
                                        else if ((icyVeinsGroup == null || icyVeinsStarts.Count == 0 || time - constructionTime[icyVeinsStarts[icyVeinsStarts.Count - 1]] >= SequenceItem.Calculations.IcyVeinsCooldown - eps) && (waterElementalGroup == null || waterElementalStarts.Count == 0 || time - constructionTime[waterElementalStarts[waterElementalStarts.Count - 1]] >= SequenceItem.Calculations.WaterElementalCooldown - eps))
                                        {
                                            // don't need coldsnap and can start right at time
                                            coldsnap[i] = 0;
                                        }
                                        else if (coldsnap[i] == 1)
                                        {
                                            // use coldsnap
                                            double normalTime = time;
                                            if (icyVeinsGroup != null && icyVeinsStarts.Count > 0) normalTime = Math.Max(normalTime, constructionTime[icyVeinsStarts[icyVeinsStarts.Count - 1]] + SequenceItem.Calculations.IcyVeinsCooldown);
                                            if (waterElementalGroup != null && waterElementalStarts.Count > 0) normalTime = Math.Max(normalTime, constructionTime[waterElementalStarts[waterElementalStarts.Count - 1]] + SequenceItem.Calculations.WaterElementalCooldown);
                                            double coldsnapReady = 0;
                                            if (lastColdsnap >= 0) coldsnapReady = coldsnapTime[lastColdsnap] + SequenceItem.Calculations.ColdsnapCooldown;
                                            if (coldsnapReady >= normalTime)
                                            {
                                                // coldsnap won't be ready until IV/WE will be back anyway, so we don't actually need it
                                                coldsnap[i] = 0;
                                                time = normalTime;
                                            }
                                            else
                                            {
                                                // go now or when coldsnap is ready
                                                time = Math.Max(coldsnapReady, time);
                                                coldsnapTime[i] = coldsnapReady;
                                                if (icyVeinsStarts.Count > 0) coldsnapTime[i] = Math.Max(coldsnapTime[i], constructionTime[icyVeinsStarts[icyVeinsStarts.Count - 1]]);
                                                if (waterElementalStarts.Count > 0) coldsnapTime[i] = Math.Max(coldsnapTime[i], constructionTime[waterElementalStarts[waterElementalStarts.Count - 1]]);
                                            }
                                        }
                                        else
                                        {
                                            // we are not allowed to use coldsnap even if we could
                                            // make sure to adjust by coldsnap constraints
                                            double normalTime = time;
                                            if (icyVeinsGroup != null && icyVeinsStarts.Count > 0) normalTime = Math.Max(normalTime, constructionTime[icyVeinsStarts[icyVeinsStarts.Count - 1]] + SequenceItem.Calculations.IcyVeinsCooldown);
                                            if (waterElementalGroup != null && waterElementalStarts.Count > 0) normalTime = Math.Max(normalTime, constructionTime[waterElementalStarts[waterElementalStarts.Count - 1]] + SequenceItem.Calculations.WaterElementalCooldown);
                                            time = normalTime;
                                        }
                                    }
                                    else
                                    {
                                        // no coldsnap constraints active
                                        coldsnap[i] = 0;
                                    }
                                    if (valid)
                                    {
                                        List<double> adjustedConstructionTime = new List<double>(constructionTime);
                                        adjustedConstructionTime.Add(time);
                                        // adjust min time of items in same super group
                                        for (int j = adjustedConstructionTime.Count - 2; j >= 0 && itemList[index[j]].SuperIndex == item.SuperIndex; j--)
                                        {
                                            time -= itemList[index[j]].Duration;
                                            adjustedConstructionTime[j] = time;
                                        }
                                        constructionTimeHistory[i] = constructionTime;
                                        constructionTime = adjustedConstructionTime;
                                        i++;
                                        if (i < N)
                                        {
                                            index[i] = 0;
                                            maxIntersect[i] = 0;
                                            coldsnap[i] = 2;
                                        }
                                    }
                                    else
                                    {
                                        used[index[i]] = false;
                                        superLeft[itemList[index[i]].SuperIndex]++;
                                        foreach (SequenceGroup group in itemList[index[i]].Group)
                                        {
                                            if (group.OrderIndex == i)
                                            {
                                                group.OrderIndex = -1;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    used[index[i]] = false;
                                    superLeft[itemList[index[i]].SuperIndex]++;
                                    foreach (SequenceGroup group in itemList[index[i]].Group)
                                    {
                                        if (group.OrderIndex == i)
                                        {
                                            group.OrderIndex = -1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } while (i >= 0 && (solver == null || !solver.CancellationPending) && Environment.TickCount < tickLimit);
        }

        private void SortGroups_ComputeSegmented(List<SequenceItem> itemList, Solver solver)
        {
            const double eps = 0.000001;
            int N = itemList.Count;
            if (N == 0) return;
            List<double> constructionTime = new List<double>();
            List<double>[] constructionTimeHistory = new List<double>[N];
            bool[] used = new bool[N];
            int[] index = new int[N];
            int[] inversions = new int[N];
            int[] coldsnap = new int[N];
            double[] coldsnapTime = new double[N];
            int[] maxIntersect = new int[N];
            for (int j = 0; j < N; j++) itemList[j].SuperIndex = -1;
            int super = -1;
            for (int j = 0; j < N; j++)
            {
                if (itemList[j].SuperIndex == -1)
                {
                    super++;
                    itemList[j].SuperIndex = super;
                    List<SequenceItem> superList = new List<SequenceItem>();
                    superList.Add(itemList[j]);
                    bool more = false;
                    do
                    {
                        more = false;
                        for (int k = j + 1; k < N; k++)
                        {
                            if (itemList[k].SuperIndex == -1)
                            {
                                foreach (SequenceItem item in superList)
                                {
                                    if (Rawr.Mage.ListUtils.Intersect<SequenceGroup>(item.Group, itemList[k].Group).Count > 0)
                                    {
                                        itemList[k].SuperIndex = super;
                                        superList.Add(itemList[k]);
                                        more = true;
                                        break;
                                    }
                                }
                            }
                        }
                    } while (more);
                    List<SequenceGroup> superGroups = new List<SequenceGroup>();
                    foreach (SequenceItem item in superList)
                    {
                        int hex = 0;
                        foreach (SequenceGroup group in item.Group)
                        {
                            if (!superGroups.Contains(group)) superGroups.Add(group);
                            hex |= (1 << superGroups.IndexOf(group));
                        }
                        item.CooldownHex = hex;
                    }
                }
            }
            List<SequenceGroup> groupList = GetAllGroups(itemList);
            super++;
            int[] superLeft = new int[super];
            for (int j = 0; j < N; j++)
            {
                superLeft[itemList[j].SuperIndex]++;
                foreach (SequenceGroup group in itemList[j].Group)
                {
                    group.OrderIndex = -1;
                }
            }
            int maxSegment = 0;
            for (int j = 0; j < N; j++)
            {
                if (itemList[j].Segment > maxSegment)
                {
                    maxSegment = itemList[j].Segment;
                }
            }
            int[] earlierCount = new int[maxSegment + 1];
            for (int j = 0; j < N; j++)
            {
                for (int k = itemList[j].Segment + 1; k <= maxSegment; k++)
                {
                    earlierCount[k]++;
                }
            }

            int i = 0;
            index[0] = 0;
            constructionTimeHistory[0] = constructionTime;
            int maxColdsnap = Calculations.MageTalents.ColdSnap + 1;
            coldsnap[0] = maxColdsnap;
            do
            {
                if (i == N)
                {
                    //double time = 0;
                    //if (constructionTime.Count > 0) time = constructionTime[constructionTime.Count - 1] + itemList[index[N - 1]].Duration;
                    // compute group splits
                    int groupSplits = 0;
                    //double lastDestro = double.NegativeInfinity;
                    foreach (SequenceGroup group in groupList)
                    {
                        /*if (group.Item.Count > 0 && group.Item[0].CastingState.PotionOfWildMagic)
                        {
                            double min = double.PositiveInfinity;
                            foreach (SequenceItem item in group.Item)
                            {
                                min = Math.Min(min, constructionTime[item.OrderIndex]);
                            }
                            if (min > lastDestro) lastDestro = min;
                        }*/
                        int minIndex = N - 1;
                        int maxIndex = 0;
                        foreach (SequenceItem item in group.Item)
                        {
                            if (item.OrderIndex < minIndex) minIndex = item.OrderIndex;
                            if (item.OrderIndex > maxIndex) maxIndex = item.OrderIndex;
                        }
                        groupSplits += (maxIndex - minIndex + 1) - group.Item.Count;
                    }
                    //if (lastDestro < FightDuration - 120.0) lastDestro = double.NegativeInfinity;
                    if (inversions[N - 1] < compactInversions || (inversions[N - 1] == compactInversions && groupSplits < compactGroupSplits))
                    {
                        compactInversions = inversions[N - 1];
                        compactGroupSplits = groupSplits;
                        //compactTotalTime = time;
                        //compactLastDestro = lastDestro;
                        compactTime = new List<double>(constructionTime);
                        compactItems = new List<SequenceItem>();
                        for (int j = 0; j < N; j++)
                        {
                            compactItems.Add(itemList[index[j]]);
                        }
                    }
                    i--;
                    if (i >= 0)
                    {
                        constructionTime = constructionTimeHistory[i];
                        used[index[i]] = false;
                        superLeft[itemList[index[i]].SuperIndex]++;
                        foreach (SequenceGroup group in itemList[index[i]].Group)
                        {
                            if (group.OrderIndex == i)
                            {
                                group.OrderIndex = -1;
                            }
                        }
                    }
                }
                else
                {
                    coldsnap[i]--;
                    if (coldsnap[i] < 0 || used[index[i]])
                    {
                        coldsnap[i] = maxColdsnap - 1;
                        do
                        {
                            index[i]++;
                        } while (index[i] < N && used[index[i]]);
                    }
                    if (index[i] == N)
                    {
                        i--;
                        if (i >= 0)
                        {
                            constructionTime = constructionTimeHistory[i];
                            used[index[i]] = false;
                            superLeft[itemList[index[i]].SuperIndex]++;
                            foreach (SequenceGroup group in itemList[index[i]].Group)
                            {
                                if (group.OrderIndex == i)
                                {
                                    group.OrderIndex = -1;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            inversions[0] = 0;
                        }
                        else
                        {
                            inversions[i] = inversions[i - 1];
                        }
                        // check if valid
                        SequenceItem item = itemList[index[i]];
                        // if we have segmentation data take a more directed search, respect segmentation ordering (jumps should be eliminated, but check back in case something looks strange)
                        if (i == 0 || (item.Segment >= itemList[index[i - 1]].Segment && i >= earlierCount[item.Segment]))
                        {
                            if (i > 0)
                            {
                                if (!(item.SuperIndex == itemList[index[i - 1]].SuperIndex || superLeft[itemList[index[i - 1]].SuperIndex] == 0))
                                {
                                    inversions[i]++; // this is actually just a group split, but should be fine i think
                                }
                            }
                            int tail = item.CooldownHex;
                            int activeTail = 0;
                            int intersectHex = 0;
                            if (i > 0 && item.SuperIndex == itemList[index[i - 1]].SuperIndex)
                            {
                                activeTail = itemList[index[i - 1]].CooldownHex;
                                intersectHex = HexCount(tail & activeTail);
                                if (intersectHex > maxIntersect[i]) maxIntersect[i] = intersectHex;
                            }
                            if (intersectHex < maxIntersect[i])
                            {
                                inversions[i]++;
                            }
                            used[index[i]] = true;
                            itemList[index[i]].OrderIndex = i;
                            superLeft[itemList[index[i]].SuperIndex]--;
                            foreach (SequenceGroup group in itemList[index[i]].Group)
                            {
                                if (group.OrderIndex == -1)
                                {
                                    // first item in this group
                                    group.OrderIndex = i;
                                }
                            }
                            // skip tests for coldsnap == 0
                            //if (coldsnap[i] == 1) // just compute it, if you want to optimize take a bit more time to think about it
                            if (superLeft[itemList[index[i]].SuperIndex] > 0) // if we don't have enough constraints not all of these might be enforced
                            {
                                for (int j = 0; j < used.Length; j++)
                                {
                                    if (!used[j])
                                    {
                                        if (itemList[j].SuperIndex == item.SuperIndex)
                                        {
                                            // make sure activations are placed before use
                                            if (item.CastingState.WaterElemental && item.VariableType != VariableType.SummonWaterElemental && itemList[j].VariableType == VariableType.SummonWaterElemental)
                                            {
                                                inversions[i]++;
                                                //tail = 0;
                                                //break;
                                            }
                                            if (item.CastingState.MirrorImage && item.VariableType != VariableType.SummonMirrorImage && itemList[j].VariableType == VariableType.SummonMirrorImage)
                                            {
                                                inversions[i]++;
                                                //tail = 0;
                                                //break;
                                            }
                                            if (i > 0 && item.SuperIndex == itemList[index[i - 1]].SuperIndex)
                                            {
                                                int intersectHexJ = HexCount(itemList[j].CooldownHex & activeTail);
                                                if (intersectHexJ > intersectHex)
                                                {
                                                    inversions[i]++;
                                                    /*if (j > index[i] && (i == 0 || (itemList[j].Segment >= itemList[index[i - 1]].Segment && i >= earlierCount[itemList[j].Segment])))
                                                    {
                                                        // anything up to j is not valid, so skip ahead
                                                        used[index[i]] = false;
                                                        foreach (SequenceGroup group in itemList[index[i]].Group)
                                                        {
                                                            if (group.OrderIndex == i)
                                                            {
                                                                group.OrderIndex = -1;
                                                            }
                                                        }
                                                        superLeft[itemList[index[i]].SuperIndex]++;
                                                        index[i] = j;
                                                        used[j] = true;
                                                        itemList[j].OrderIndex = i;
                                                        superLeft[itemList[index[i]].SuperIndex]--;
                                                        foreach (SequenceGroup group in itemList[j].Group)
                                                        {
                                                            if (group.OrderIndex == -1)
                                                            {
                                                                // first item in this group
                                                                group.OrderIndex = i;
                                                            }
                                                        }
                                                        intersectHex = intersectHexJ;
                                                        maxIntersect[i] = intersectHex;
                                                        item = itemList[j];
                                                        tail = item.CooldownHex;
                                                        j = -1;
                                                        continue;
                                                    }
                                                    else
                                                    {
                                                        // invalidate
                                                        tail = 0;
                                                        break;
                                                    }*/
                                                }
                                            }
                                            if ((itemList[j].CooldownHex & activeTail & ~item.CooldownHex) != 0) // strong requirement, if converting for reconstruction on nonsegmented data might have to remove this
                                            {
                                                // we'll have to place something from active tail that is being removed
                                                // from active tail
                                                inversions[i]++;
                                                //tail = 0;
                                                //break;
                                            }
                                            int intersect = item.CooldownHex & itemList[j].CooldownHex;
                                            if (intersect > 0)
                                            {
                                                tail = intersect & tail;
                                                if (tail == 0)
                                                {
                                                    inversions[i]++;
                                                    //break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            double time = 0;
                            if (constructionTime.Count > 0) time = constructionTime[constructionTime.Count - 1] + itemList[index[i - 1]].Duration;
                            time = Math.Max(time, item.MinTime);
                            double minTotalTime = time + item.Duration;
                            //for (int j = 0; j < N; j++) // too expensive
                            //{
                            //    if (!used[j]) minTotalTime += itemList[j].Duration;
                            //}
                            //if (tail > 0 && minTotalTime < compactTotalTime)
                            if (inversions[i] < compactInversions)
                            {
                                // check constraints
                                List<int> icyVeinsStarts = new List<int>();
                                List<int> waterElementalStarts = new List<int>();
                                SequenceGroup icyVeinsGroup = null;
                                SequenceGroup waterElementalGroup = null;
                                foreach (SequenceGroup group in item.Group)
                                {
                                    foreach (CooldownConstraint constraint in group.Constraint)
                                    {
                                        if (!constraint.ColdSnap)
                                        {
                                            //for (int j = 0; j < i; j++)
                                            //{
                                            //    if (itemList[index[j]].Group.Contains(constraint.Group))
                                            //    {
                                            //        time = Math.Max(time, constructionTime[j] + constraint.Cooldown);
                                            //        break;
                                            //    }
                                            //}
                                            int j = constraint.Group.OrderIndex;
                                            if (j >= 0)
                                            {
                                                time = Math.Max(time, constructionTime[j] + constraint.Cooldown);
                                            }
                                        }
                                        else
                                        {
                                            // if we're in group with already placed item then no need to redo all this
                                            if (i > 0 && itemList[index[i - 1]].Group.Contains(group)) continue;
                                            if (constraint.EffectCooldown == (int)StandardEffect.IcyVeins) icyVeinsGroup = group;
                                            if (constraint.EffectCooldown == (int)StandardEffect.WaterElemental) waterElementalGroup = group;
                                            int minIndex = i;
                                            foreach (SequenceItem coldsnapItem in constraint.Group.Item)
                                            {
                                                if (coldsnapItem.OrderIndex >= 0 && coldsnapItem.OrderIndex < N && index[coldsnapItem.OrderIndex] < N && itemList[index[coldsnapItem.OrderIndex]] == coldsnapItem && used[index[coldsnapItem.OrderIndex]] && coldsnapItem.OrderIndex < minIndex)
                                                {
                                                    minIndex = coldsnapItem.OrderIndex;
                                                }
                                            }
                                            if (minIndex < i)
                                            {
                                                if (constraint.EffectCooldown == (int)StandardEffect.IcyVeins) icyVeinsStarts.Add(minIndex);
                                                if (constraint.EffectCooldown == (int)StandardEffect.WaterElemental) waterElementalStarts.Add(minIndex);
                                            }
                                        }
                                    }
                                }
                                bool valid = true;
                                // we absolutely can't come faster than time
                                // now check coldsnap constraints
                                // the constraints should link to all the other icy veins/water elemental groups
                                // look at the ones that were placed already and sort them by order index
                                // if the last one that needed coldsnap is farther than coldsnap cooldown then we can use it again
                                // if we don't need to use coldsnap anyway then adjust coldsnap to 0
                                if (icyVeinsGroup != null || waterElementalGroup != null)
                                {
                                    // this is only called for first coldsnap item in group
                                    icyVeinsStarts.Sort();
                                    waterElementalStarts.Sort();
                                    int lastColdsnap = -1;
                                    for (int j = 0; j < i; j++)
                                    {
                                        if (coldsnap[j] == 1) lastColdsnap = j;
                                    }
                                    if ((icyVeinsGroup != null && icyVeinsGroup.Duration > 20.0 + eps) || (waterElementalGroup != null && waterElementalGroup.Duration > SequenceItem.Calculations.WaterElementalDuration + eps))
                                    {
                                        // we need internal coldsnap
                                        if (coldsnap[i] == 1)
                                        {
                                            double normalTime = time;
                                            if (icyVeinsGroup != null && icyVeinsStarts.Count > 0) normalTime = Math.Max(normalTime, constructionTime[icyVeinsStarts[icyVeinsStarts.Count - 1]] + SequenceItem.Calculations.IcyVeinsCooldown);
                                            if (waterElementalGroup != null && waterElementalStarts.Count > 0) normalTime = Math.Max(normalTime, constructionTime[waterElementalStarts[waterElementalStarts.Count - 1]] + SequenceItem.Calculations.WaterElementalCooldown);
                                            double coldsnapReady = 0;
                                            if (lastColdsnap >= 0) coldsnapReady = coldsnapTime[lastColdsnap] + SequenceItem.Calculations.ColdsnapCooldown;
                                            // we have to do first one on normal time and have coldsnap ready in the middle
                                            time = normalTime;
                                            if (icyVeinsGroup != null) time = Math.Max(time, coldsnapReady - 20.0);
                                            if (waterElementalGroup != null) time = Math.Max(time, coldsnapReady - SequenceItem.Calculations.WaterElementalDuration);
                                            coldsnapTime[i] = Math.Max(time, coldsnapReady);
                                        }
                                        else
                                        {
                                            // can't do without coldsnap
                                            valid = false;
                                        }
                                    }
                                    else if ((icyVeinsGroup == null || icyVeinsStarts.Count == 0 || time - constructionTime[icyVeinsStarts[icyVeinsStarts.Count - 1]] >= SequenceItem.Calculations.IcyVeinsCooldown - eps) && (waterElementalGroup == null || waterElementalStarts.Count == 0 || time - constructionTime[waterElementalStarts[waterElementalStarts.Count - 1]] >= SequenceItem.Calculations.WaterElementalCooldown - eps))
                                    {
                                        // don't need coldsnap and can start right at time
                                        coldsnap[i] = 0;
                                    }
                                    else if (coldsnap[i] == 1)
                                    {
                                        // use coldsnap
                                        double normalTime = time;
                                        if (icyVeinsGroup != null && icyVeinsStarts.Count > 0) normalTime = Math.Max(normalTime, constructionTime[icyVeinsStarts[icyVeinsStarts.Count - 1]] + SequenceItem.Calculations.IcyVeinsCooldown);
                                        if (waterElementalGroup != null && waterElementalStarts.Count > 0) normalTime = Math.Max(normalTime, constructionTime[waterElementalStarts[waterElementalStarts.Count - 1]] + SequenceItem.Calculations.WaterElementalCooldown);
                                        double coldsnapReady = 0;
                                        if (lastColdsnap >= 0) coldsnapReady = coldsnapTime[lastColdsnap] + SequenceItem.Calculations.ColdsnapCooldown;
                                        if (coldsnapReady >= normalTime)
                                        {
                                            // coldsnap won't be ready until IV/WE will be back anyway, so we don't actually need it
                                            coldsnap[i] = 0;
                                            time = normalTime;
                                        }
                                        else
                                        {
                                            // go now or when coldsnap is ready
                                            time = Math.Max(coldsnapReady, time);
                                            coldsnapTime[i] = coldsnapReady;
                                            if (icyVeinsStarts.Count > 0) coldsnapTime[i] = Math.Max(coldsnapTime[i], constructionTime[icyVeinsStarts[icyVeinsStarts.Count - 1]]);
                                            if (waterElementalStarts.Count > 0) coldsnapTime[i] = Math.Max(coldsnapTime[i], constructionTime[waterElementalStarts[waterElementalStarts.Count - 1]]);
                                        }
                                    }
                                    else
                                    {
                                        // we are not allowed to use coldsnap even if we could
                                        // make sure to adjust by coldsnap constraints
                                        double normalTime = time;
                                        if (icyVeinsGroup != null && icyVeinsStarts.Count > 0) normalTime = Math.Max(normalTime, constructionTime[icyVeinsStarts[icyVeinsStarts.Count - 1]] + SequenceItem.Calculations.IcyVeinsCooldown);
                                        if (waterElementalGroup != null && waterElementalStarts.Count > 0) normalTime = Math.Max(normalTime, constructionTime[waterElementalStarts[waterElementalStarts.Count - 1]] + SequenceItem.Calculations.WaterElementalCooldown);
                                        time = normalTime;
                                    }
                                }
                                else
                                {
                                    // no coldsnap constraints active
                                    coldsnap[i] = 0;
                                }
                                if (valid)
                                {
                                    List<double> adjustedConstructionTime = new List<double>(constructionTime);
                                    adjustedConstructionTime.Add(time);
                                    // adjust min time of items in same super group
                                    for (int j = adjustedConstructionTime.Count - 2; j >= 0 && itemList[index[j]].SuperIndex == item.SuperIndex; j--)
                                    {
                                        time -= itemList[index[j]].Duration;
                                        adjustedConstructionTime[j] = time;
                                    }
                                    constructionTimeHistory[i] = constructionTime;
                                    constructionTime = adjustedConstructionTime;
                                    i++;
                                    if (i < N)
                                    {
                                        index[i] = 0;
                                        maxIntersect[i] = 0;
                                        coldsnap[i] = maxColdsnap;
                                    }
                                }
                                else
                                {
                                    used[index[i]] = false;
                                    superLeft[itemList[index[i]].SuperIndex]++;
                                    foreach (SequenceGroup group in itemList[index[i]].Group)
                                    {
                                        if (group.OrderIndex == i)
                                        {
                                            group.OrderIndex = -1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                used[index[i]] = false;
                                superLeft[itemList[index[i]].SuperIndex]++;
                                foreach (SequenceGroup group in itemList[index[i]].Group)
                                {
                                    if (group.OrderIndex == i)
                                    {
                                        group.OrderIndex = -1;
                                    }
                                }
                            }
                        }
                    }
                }
            } while (i >= 0 && CalculationsMage.IsSolverEnabled(solver));
        }

        private void SortGroups_AddRemainingItems(List<SequenceItem> constructionList, List<double> constructionTime, List<SequenceItem> remainingList)
        {
            if (remainingList.Count == 0)
            {
                double time = 0;
                if (constructionTime.Count > 0) time = constructionTime[constructionTime.Count - 1] + constructionList[constructionTime.Count - 1].Duration;
                if (time < compactTotalTime)
                {
                    compactTotalTime = time;
                    compactTime = new List<double>(constructionTime);
                    compactItems = new List<SequenceItem>(constructionList);
                }
            }
            else
            {
                foreach (SequenceItem item in remainingList)
                {
                    item.Tail = new List<SequenceGroup>(item.Group);
                }

                foreach (SequenceItem item in remainingList)
                {
                    foreach (SequenceItem tailitem in remainingList)
                    {
                        List<SequenceGroup> intersect = Rawr.Mage.ListUtils.Intersect<SequenceGroup>(item.Group, tailitem.Group);
                        if (intersect.Count > 0)
                        {
                            item.Tail = Rawr.Mage.ListUtils.Intersect<SequenceGroup>(intersect, item.Tail);
                        }
                    }
                }

                List<SequenceGroup> tail = (constructionList.Count > 0) ? constructionList[constructionList.Count - 1].Group : null;
                SequenceItem best = null;
                foreach (SequenceItem item in remainingList)
                {
                    if (best == null || SortGroups_Compare(item, best, tail) < 0)
                    {
                        best = item;
                    }
                }

                for (int i = 0; i < remainingList.Count; i++)
                {
                    SequenceItem item = remainingList[i];
                    if (SortGroups_Compare(item, best, tail) == 0)
                    {
                        remainingList.RemoveAt(i);
                        constructionList.Add(item);
                        double time = 0;
                        if (constructionTime.Count > 0) time = constructionTime[constructionTime.Count - 1] + constructionList[constructionTime.Count - 1].Duration;
                        time = Math.Max(time, item.MinTime);
                        // check constraints
                        foreach (SequenceGroup group in item.Group)
                        {
                            foreach (CooldownConstraint constraint in group.Constraint)
                            {
                                for (int j = 0; j < constructionList.Count; j++)
                                {
                                    if (constructionList[j].Group.Contains(constraint.Group))
                                    {
                                        time = Math.Max(time, constructionTime[j] + constraint.Cooldown);
                                        break;
                                    }
                                }
                            }
                        }
                        List<double> adjustedConstructionTime = new List<double>(constructionTime);
                        adjustedConstructionTime.Add(time);
                        // adjust min time of items in same super group
                        for (int j = adjustedConstructionTime.Count - 2; j >= 0 && Rawr.Mage.ListUtils.Intersect<SequenceGroup>(constructionList[j].Group, constructionList[j + 1].Group).Count > 0; j--)
                        {
                            time -= constructionList[j].Duration;
                            adjustedConstructionTime[j] = time;
                        }
                        SortGroups_AddRemainingItems(constructionList, adjustedConstructionTime, remainingList);
                        //constructionTime.RemoveAt(constructionTime.Count - 1);
                        constructionList.RemoveAt(constructionList.Count - 1);
                        remainingList.Insert(i, item);
                    }
                }
            }
        }

        public void Compact(bool compactGroups)
        {
            for (int i = 0; i + 1 < sequence.Count; i++)
            {
                if (sequence[i].Index == sequence[i + 1].Index && (sequence[i].Group.Count == 0 || compactGroups))
                {
                    sequence[i].Duration += sequence[i + 1].Duration;
                    if (sequence[i].Group.Count == 0)
                    {
                        // when merging noncooldowns make sure to correct the max time
                        sequence[i].SetTimeConstraint(sequence[i].MinTime, sequence[i].MaxTime - sequence[i + 1].Duration);
                    }
                    sequence.RemoveAt(i + 1);
                    i--;
                }
            }
        }

        public void RepositionManaConsumption()
        {
            const double eps = 0.000001;
            if (sequence.Count == 0) return;
            double ghostMana = Math.Max(0, -ManaCheck());
            double fight = FightDuration;
            /*List<bool> gemPotList = new List<bool>();
            List<int> gemPotSegList = new List<int>();
            for (int i = 0; i < sequence.Count; i++)
            {
                if (sequence[i].IsManaPotionOrGem)
                {
                    gemPotList.Add(sequence[i].VariableType == VariableType.ManaPotion);
                    gemPotSegList.Add(sequence[i].Segment);
                }
            }
            bool[] gemPotOrder = gemPotList.ToArray();
            int[] gemPotSeg = gemPotSegList.ToArray();
            Array.Sort<int, bool>(gemPotSeg, gemPotOrder);
            int gemPotIndex = 0;*/            
            double potTime = RemoveIndex(VariableType.ManaPotion);
            double gemTime = RemoveIndex(VariableType.ManaGem);
            //double evoTime = RemoveIndex(VariableType.Evocation);
            bool gemActivated = SequenceItem.Calculations.ManaGemEffect;
            double gemValue = SequenceItem.Calculations.ManaGemValue;
            double gemMaxValue = SequenceItem.Calculations.MaxManaGemValue;
            double potValue = SequenceItem.Calculations.ManaPotionValue;
            double potMaxValue = SequenceItem.Calculations.MaxManaPotionValue;
            double time = 0;
            double nextGem = 0;
            double nextPot = 0;
            //double nextEvo = 0;
            double maxTps = 50000.0;
            //double evocationFactor = 1.0;
            if (SequenceItem.Calculations.CalculationOptions.TpsLimit > 0.0)
            {
                maxTps = SequenceItem.Calculations.CalculationOptions.TpsLimit;
            }
            if (sequence[0].VariableType == VariableType.Drinking)
            {
                time += sequence[0].Duration;
            }
            ComputeTimestamps(); 
            do
            {
            DoStart:
                double mana = Evaluate(null, EvaluationMode.ManaAtTime, time);
                double maxMps = double.PositiveInfinity;
                /*if (evoTime > 0)
                {
                    int numTicks = 4;
                    int ticksLeft = (int)Math.Round(evoTime / (EvocationDuration / numTicks));
                    if (ticksLeft % numTicks != 0)
                    {
                        evocationFactor = (ticksLeft % numTicks) / (double)numTicks;
                    }
                    else
                    {
                        evocationFactor = 1.0;
                    }
                }*/
                if (!((potTime > 0 && nextPot == 0) || (gemTime > 0 && nextGem == 0)/* || (evoTime > 0 && nextEvo == 0)*/))
                {
                    double m = mana + ghostMana;
                    for (double _potTime = potTime; _potTime > eps; _potTime -= 1.0)
                    {
                        m += (1 + BaseStats.BonusManaPotion) * potValue;
                    }
                    for (double _gemTime = gemTime; _gemTime > eps; _gemTime -= 1.0)
                    {
                        m += (1 + BaseStats.BonusManaGem) * gemValue;
                    }
                    //m += EvocationRegen * evoTime;
                    maxMps = m / (fight - time);
                }
                double minMps = double.NegativeInfinity;
                double targetTime = sequence[sequence.Count - 1].Timestamp + sequence[sequence.Count - 1].Duration;
                if (nextGem > time && gemTime > 0)
                {
                    double m = mana;
                    if (potTime > 0 && nextPot < nextGem) m += (1 + BaseStats.BonusManaPotion) * potValue;
                    //if (evoTime > 0 && nextEvo < nextGem) m += EvocationRegen * Math.Min(evoTime, EvocationDuration * evocationFactor);
                    double mps = m / (nextGem - time);
                    if (mps < maxMps)
                    {
                        maxMps = mps;
                        targetTime = nextGem;
                    }
                }
                if (nextPot > time && potTime > 0)
                {
                    double m = mana;
                    if (gemTime > 0 && nextGem < nextPot) m += (1 + BaseStats.BonusManaGem) * gemValue;
                    //if (evoTime > 0 && nextEvo < nextPot) m += EvocationRegen * Math.Min(evoTime, EvocationDuration * evocationFactor);
                    double mps = m / (nextPot - time);
                    if (mps < maxMps)
                    {
                        maxMps = mps;
                        targetTime = nextPot;
                    }
                }
                /*if (nextEvo > time && evoTime > 0)
                {
                    double m = mana;
                    if (potTime > 0 && nextPot < nextEvo) m += (1 + BaseStats.BonusManaPotion) * potValue;
                    if (gemTime > 0 && nextGem < nextEvo) m += (1 + BaseStats.BonusManaGem) * gemValue;
                    double mps = m / (nextEvo - time);
                    if (mps < maxMps)
                    {
                        maxMps = mps;
                        targetTime = nextEvo;
                    }
                }*/
                if (potTime > 0 && (nextPot <= nextGem || gemTime <= 0 || nextPot == 0) /*&& (nextPot <= nextEvo || nextPot == 0 || evoTime <= 0)*/)
                {
                    if (nextPot <= time)
                    {
                        minMps = maxMps;
                    }
                    else
                    {
                        targetTime = nextPot;
                        minMps = ((1 + BaseStats.BonusManaPotion) * potMaxValue - (BaseStats.Mana - mana)) / (targetTime - time);
                    }
                }
                else if (gemTime > 0 /*&& (nextGem <= nextEvo || nextGem == 0 || evoTime <= 0)*/)
                {
                    if (nextGem <= time)
                    {
                        minMps = maxMps;
                    }
                    else
                    {
                        targetTime = nextGem;
                        minMps = ((1 + BaseStats.BonusManaGem) * gemMaxValue - (BaseStats.Mana - mana)) / (targetTime - time);
                    }
                }
                /*else if (evoTime > 0)
                {
                    if (nextEvo <= time)
                    {
                        minMps = maxMps;
                    }
                    else
                    {
                        targetTime = nextEvo;
                        minMps = (EvocationRegen * Math.Min(evoTime, EvocationDuration * evocationFactor) - (BaseStats.Mana - mana)) / (targetTime - time);
                    }
                }*/
                if (potTime <= 0 && gemTime <= 0/* && evoTime <= 0*/)
                {
                    maxMps = mana / (targetTime - time);
                    minMps = -(BaseStats.Mana - mana) / (targetTime - time);
                }
                if (maxMps < minMps) maxMps = minMps; // if we have min mps constraint then at that point we'll be full on mana, whatever max mana has to be handled will have to deal with it later
                double lastTargetMana = -1;
                double extraMana = 0;
                double oomtime = double.PositiveInfinity;
            Retry:
                SortByMps(true, minMps, maxMps, maxTps, time, Math.Min(oomtime, targetTime), extraMana, mana);
            VerifyOOM:
                Compact(false);
                // guard against oom
                //double targetmana = Evaluate(null, EvaluationMode.ManaAtTime, targetTime);
                double targetmana = mana;
                double t = 0;
                int i;
                for (i = 0; i < sequence.Count; i++)
                {
                    double d = sequence[i].Duration;
                    if (sequence[i].IsManaPotionOrGem) d = 0;
                    if (d > 0 && t + d > targetTime)
                    {
                        targetmana -= sequence[i].Mps * (targetTime - t);
                        // if we are not in super group and we run out of mana this means that Sort ran into boundaries
                        // resort to swapping in this case
                        // the other handler takes care only of trailing oom, we have to make sure here that we
                        // get to the point where the pot/gem can be used (targetTime is always for next consumable or end of fight)
                        if (targetmana < -eps && !((nextPot <= time && potTime > 0) || (nextGem <= time && gemTime > 0)/* || (nextEvo <= time && evoTime > 0)*/))
                        {
                            // only split if it is splittable
                            if (sequence[i].Group.Count == 0)
                            {
                                SplitAt(i, targetTime - t);
                            }
                            extraMana = -targetmana;
                            goto SwapRecovery;
                        }
                        break;
                    }
                    else if (d > 0 && t + d > time)
                    {
                        targetmana -= sequence[i].Mps * sequence[i].Duration;
                        if (i < sequence.Count - 1 && targetmana < -eps && !((nextPot <= time && potTime > 0) || (nextGem <= time && gemTime > 0)/* || (nextEvo <= time && evoTime > 0)*/)) // only worry if we already started all mana cooldowns and we're not at last item (at which point we can't do anything and it's only ghost mana left)
                        {
                            // we run oom during construction
                            if (oomtime < double.PositiveInfinity && Math.Abs(t + d - oomtime) < eps && Math.Abs(lastTargetMana - targetmana) < eps)
                            {
                                // we were not successful in recovering from oom
                                // go into swap mode
                                // extraMana might not be set
                                extraMana = -targetmana;
                                goto SwapRecovery;
                            }
                            oomtime = t + d;
                            minMps = -(BaseStats.Mana - mana) / (oomtime - time);
                            extraMana = -targetmana;
                            lastTargetMana = targetmana;
                            goto Retry;
                        }
                    }
                    t += d;
                }
                if (oomtime < double.PositiveInfinity) lastTargetMana = -1;
                double tmana = targetmana;
                oomtime = double.PositiveInfinity;
                if (!(i >= sequence.Count - 1 || sequence[i].Group.Count == 0 || (targetTime <= t && (i == 0 || sequence[i - 1].SuperGroup != sequence[i].SuperGroup))))
                {
                    SequenceGroup super = sequence[i].SuperGroup;
                    if (targetmana != lastTargetMana)
                    {
                        // count mana till end of super group
                        // account for to be used consumables (don't assume evo during super group unless we haven't placed the first one, in that case it will actually be placed before the super group)
                        /*if (evoTime > 0 && nextEvo == 0.0)
                        {
                            targetmana += EvocationRegen * Math.Min(evoTime, EvocationDuration * evocationFactor);
                        }*/
                        if (sequence[sequence.Count - 1].SuperGroup == super) targetmana += ghostMana;
                        double _potTime = potTime;
                        double _nextPot = nextPot;
                        double _gemTime = gemTime;
                        double _nextGem = nextGem;
                        if (targetmana - sequence[i].Mps * (sequence[i].Duration - (targetTime - t)) < -eps)
                        {
                            // targetmana / sequence[i].Mps = timeFromTargetTime
                            double oomt = targetTime + targetmana / sequence[i].Mps;
                            while (_potTime > eps && _nextPot < oomt + eps && targetmana - sequence[i].Mps * (sequence[i].Duration - (targetTime - t)) < -eps)
                            {
                                targetmana += (1 + BaseStats.BonusManaPotion) * potValue;
                                _potTime -= 1.0;
                                _nextPot += 120.0;
                                oomt = targetTime + targetmana / sequence[i].Mps;
                            }
                            while (_gemTime > eps && _nextGem < oomt + eps && targetmana - sequence[i].Mps * (sequence[i].Duration - (targetTime - t)) < -eps)
                            {
                                targetmana += (1 + BaseStats.BonusManaGem) * gemValue;
                                _gemTime -= 1.0;
                                _nextGem += 120.0;
                                oomt = targetTime + targetmana / sequence[i].Mps;
                            }
                            if (targetmana - sequence[i].Mps * (sequence[i].Duration - (targetTime - t)) < -eps)
                            {
                                double regenTime = t + sequence[i].Duration;
                                if (_potTime > eps && _nextPot < regenTime) regenTime = _nextPot;
                                if (_gemTime > eps && _nextGem < regenTime) regenTime = _nextGem;
                                // we run oom and regen options are not ready
                                minMps = -(BaseStats.Mana - mana) / (targetTime - time);
                                extraMana = -(targetmana - sequence[i].Mps * (regenTime - targetTime));
                                lastTargetMana = tmana;
                                goto Retry;
                            }
                        }
                        targetmana -= sequence[i].Mps * (sequence[i].Duration - (targetTime - t));
                        t += sequence[i].Duration;
                        i++;
                        while (i < sequence.Count && sequence[i].SuperGroup == super)
                        {
                            if (targetmana - sequence[i].Mps * sequence[i].Duration < -eps)
                            {
                                // targetmana / sequence[i].Mps = timeFromTargetTime
                                double oomt = t + targetmana / sequence[i].Mps;
                                while (_potTime > eps && _nextPot < oomt + eps && targetmana - sequence[i].Mps * sequence[i].Duration < -eps)
                                {
                                    targetmana += (1 + BaseStats.BonusManaPotion) * potValue;
                                    _potTime -= 1.0;
                                    _nextPot += 120.0;
                                    oomt = t + targetmana / sequence[i].Mps;
                                }
                                while (_gemTime > eps && _nextGem < oomt + eps && targetmana - sequence[i].Mps * sequence[i].Duration < -eps)
                                {
                                    targetmana += (1 + BaseStats.BonusManaGem) * gemValue;
                                    _gemTime -= 1.0;
                                    _nextGem += 120.0;
                                    oomt = t + targetmana / sequence[i].Mps;
                                }
                                if (targetmana - sequence[i].Mps * sequence[i].Duration < -eps)
                                {
                                    double regenTime = t + sequence[i].Duration;
                                    if (_potTime > eps && _nextPot < regenTime) regenTime = _nextPot;
                                    if (_gemTime > eps && _nextGem < regenTime) regenTime = _nextGem;
                                    // we run oom and regen options are not ready
                                    minMps = -(BaseStats.Mana - mana) / (targetTime - time);
                                    extraMana = -(targetmana - sequence[i].Mps * (regenTime - t));
                                    lastTargetMana = tmana;
                                    goto Retry;
                                }
                            }
                            targetmana -= sequence[i].Mps * sequence[i].Duration;
                            t += sequence[i].Duration;
                            i++;
                        }
                    }
                    else
                    {
                        goto SwapRecovery;
                    }
                }
                goto Abort;
            SwapRecovery:
                // we run out of mana in a long super group and most likely we've placed
                // some mana consumables inside the group already or the group is at its
                // minimum allowed time, preventing us to move things in front of it
                // resort to finding a non-cooldown high mps item before group and low mps
                // item after group and swap to gain extraMana mana
                // evaluate how much space we have with already placed mana consumables
                // if gap is less than extraMana then the super group is probably too expensive
                // for our mana pool, consider starting with full mana before super group
                // and delaying mana consumables to be used inside the super group
                int superIndex = i;
                double superTime = t;
                int lowestMpsIndex = 0;
                while (extraMana > eps && lowestMpsIndex >= 0)
                {
                    i = superIndex + 1;
                    t = superTime;
                    lowestMpsIndex = -1;
                    for (; i < sequence.Count; i++)
                    {
                        if (sequence[i].Group.Count == 0 && (lowestMpsIndex == -1 || sequence[i].Mps < sequence[lowestMpsIndex].Mps))
                        {
                            lowestMpsIndex = i;
                        }
                    }
                    if (lowestMpsIndex > -1)
                    {
                        bool swapped = false;
                        // we've found a suitable candidate for swap
                        // find first suitable non-cooldown with higher mps to perform swap
                        // and validate mana gap as we go over items
                        for (i = superIndex; i >= 0; i--)
                        {
                            double mpsdiff = sequence[i].Mps - sequence[lowestMpsIndex].Mps;
                            bool good = sequence[i].Group.Count == 0 && (!sequence[i].IsManaPotionOrGem && !sequence[i].IsEvocation) && mpsdiff > 0;
                            // don't allow swaps if the non-cooldown has time restrictions that won't be met after swap
                            if (good)
                            {
                                if (sequence[lowestMpsIndex].Timestamp < sequence[i].MinTime - eps || sequence[lowestMpsIndex].Timestamp > sequence[i].MaxTime + eps)
                                {
                                    good = false;
                                }
                            }
                            if (good)
                            {
                                // we can do the swap, this will happen at i < superIndex
                                // this can happen at i == superIndex also if there are items later that can't be pushed back
                                double neededSwap = extraMana / mpsdiff;
                                if (neededSwap > sequence[i].Duration) neededSwap = sequence[i].Duration;
                                if (neededSwap > sequence[lowestMpsIndex].Duration) neededSwap = sequence[lowestMpsIndex].Duration;
                                if (neededSwap < sequence[i].Duration - eps)
                                {
                                    SplitAt(i, sequence[i].Duration - neededSwap);
                                    superIndex++;
                                    i++;
                                    lowestMpsIndex++;
                                }
                                if (neededSwap < sequence[lowestMpsIndex].Duration - eps)
                                {
                                    SplitAt(lowestMpsIndex, neededSwap);
                                }
                                // make the swap and recalc timestamps
                                SequenceItem tmp = sequence[i];
                                sequence[i] = sequence[lowestMpsIndex];
                                sequence[lowestMpsIndex] = tmp;
                                ComputeTimestamps();
                                extraMana -= neededSwap * mpsdiff;
                                swapped = true;
                                break;
                            }
                            else
                            {
                                double imana = Evaluate(null, EvaluationMode.ManaAtTime, t);
                                if (BaseStats.Mana - imana < extraMana)
                                {
                                    // we cannot perform the swap, but let's at least do a partial swap if possible
                                    // so that it is clear in the output that even starting from full mana our mana
                                    // pool is too shallow to go through this sequence
                                    if (BaseStats.Mana - imana > eps)
                                    {
                                        extraMana = BaseStats.Mana - imana;
                                    }
                                    else
                                    {
                                        goto Abort;
                                    }
                                }
                                if (i > 0)
                                {
                                    double d = sequence[i - 1].Duration;
                                    if (sequence[i - 1].IsManaPotionOrGem) d = 0;
                                    t -= d;
                                }
                            }
                        }
                        if (!swapped)
                        {
                            lowestMpsIndex = -1; // if the lowest can't be swapped anywhere don't go into infinite loop
                        }
                    }
                }
                if (lowestMpsIndex == -1) goto Abort;
                mana = Evaluate(null, EvaluationMode.ManaAtTime, time);
                goto VerifyOOM;
            Abort:
                double gem = nextGem;
                double pot = nextPot;
                //double evo = nextEvo;
                //bool evoMoved = false;
                if (gemTime > 0) gem = Evaluate(null, EvaluationMode.ManaBelow, BaseStats.Mana - (1 + BaseStats.BonusManaGem) * gemMaxValue, Math.Max(time, nextGem), 4);
                if (potTime > 0) pot = Evaluate(null, EvaluationMode.ManaBelow, BaseStats.Mana - (1 + BaseStats.BonusManaPotion) * potMaxValue, Math.Max(time, nextPot), 3);
                /*if (evoTime > 0)
                {
                    evo = Evaluate(null, EvaluationMode.ManaBelow, BaseStats.Mana - EvocationRegen * Math.Min(evoTime, EvocationDuration * evocationFactor), Math.Max(time, nextEvo), 2);
                    double breakpoint = Evaluate(null, EvaluationMode.CooldownBreak, evo);
                    if (breakpoint < fight && breakpoint > evo)
                    {
                        evo = breakpoint;
                        evoMoved = true;
                    }
                }*/
                // use pot & gem before evo, they need tighter packing
                // start with pot because pot needs more buffer than gem usually
                // verify timing requirements for flame cap, destro pot
                bool forceGem = false;
                bool forcePot = false;
                t = 0;
                //double nextFlameCap = double.PositiveInfinity;
                //double nextFlameCapMin = double.PositiveInfinity;
                double nextEffectPotion = double.PositiveInfinity;
                double nextEffectPotionMin = double.PositiveInfinity;
                for (i = 0; i < sequence.Count; i++)
                {
                    double d = sequence[i].Duration;
                    if (sequence[i].IsManaPotionOrGem) d = 0;
                    /*if (d > 0 && t >= gem)
                    {
                        if (sequence[i].CastingState != null && sequence[i].CastingState.FlameCap)
                        {
                            nextFlameCap = Math.Min(nextFlameCap, sequence[i].MaxTime);
                            nextFlameCapMin = Math.Min(nextFlameCapMin, sequence[i].MinTime);
                        }
                    }*/
                    if (d > 0 && t >= pot)
                    {
                        if (sequence[i].CastingState != null && (sequence[i].CastingState.PotionOfWildMagic || sequence[i].CastingState.PotionOfSpeed))
                        {
                            nextEffectPotion = Math.Min(nextEffectPotion, sequence[i].MaxTime);
                            nextEffectPotionMin = Math.Min(nextEffectPotionMin, sequence[i].MinTime);
                        }
                    }
                    t += d;
                }
                /*if (gemTime > 0 && !double.IsPositiveInfinity(nextFlameCap))
                {
                    if (gem > nextFlameCap - 120.0 + eps && gem < nextFlameCap)
                    {
                        nextGem = nextFlameCapMin + 180.0;
                        gem = Evaluate(null, EvaluationMode.ManaBelow, BaseStats.Mana - (1 + BaseStats.BonusManaGem) * gemMaxValue, Math.Max(time, nextGem), 4);
                        if (nextGem > fight)
                        {
                            nextGem = fight;
                            potTime = 0.0;
                        }
                    }
                }*/
                if (potTime > 0 && !double.IsPositiveInfinity(nextEffectPotion))
                {
                    if (pot > nextEffectPotion - 120.0 + eps && pot < nextEffectPotion)
                    {
                        nextPot = nextEffectPotionMin + 120.0;
                        pot = Evaluate(null, EvaluationMode.ManaBelow, BaseStats.Mana - (1 + BaseStats.BonusManaPotion) * potMaxValue, Math.Max(time, nextPot), 3);
                        if (nextPot > fight)
                        {
                            nextPot = fight;
                            potTime = 0.0;
                        }
                    }
                }
                if (potTime > 0 && gemTime > 0)
                {
                    // very special case for now, revisit later
                    /*if (!double.IsPositiveInfinity(nextFlameCap))
                    {
                        if (gem <= nextFlameCap - 120.0 + eps && pot > gem - 30.0 && double.IsPositiveInfinity(nextEffectPotion)) forceGem = true;
                    }*/
                    if (!double.IsPositiveInfinity(nextEffectPotion))
                    {
                        if (pot <= nextEffectPotion - 120.0 + eps && gem > pot - 30.0/* && double.IsPositiveInfinity(nextFlameCap)*/) forcePot = true;
                    }
                }
                // if gem is activated then check for activations
                if (gemActivated && gemTime > 0)
                {
                    double maxtime = fight;
                    // don't wait for pot unless we'll still have room for gem after, just heuristic for now, do something more sophisticated
                    if (potTime > 0 && pot < maxtime && (gem < pot || gem - pot > 20)) maxtime = pot;
                    //if (evoTime > 0 && evo < maxtime) maxtime = evo;
                    if (time == 0 && gemTime > 1 + eps) maxtime = fight; // assume that at start we want to activate SCB as soon as possible, in most cases we have to insert burn before activation to prevent overflow and that can cause pot to activate earlier recausing overflow
                    t = 0;
                    for (i = 0; i < sequence.Count; i++)
                    {
                        double d = sequence[i].Duration;
                        if (sequence[i].IsManaPotionOrGem) d = 0;
                        if (d > 0 && t >= time && t < maxtime && sequence[i].CastingState.ManaGemEffect && (i == 0 || (!sequence[i - 1].CastingState.ManaGemEffect && sequence[i - 1].VariableType != VariableType.ManaGem && sequence[i - 1].VariableType != VariableType.ManaPotion)))
                        {
                            // make sure that we have verified time up to here (give some extra room to make sure overflow is accounted for)
                            if (t + d * 0.1 > targetTime + eps)
                            {
                                targetTime = t + d * 0.1;
                                lastTargetMana = -1;
                                extraMana = 0;
                                oomtime = double.PositiveInfinity;
                                goto Retry;
                            }
                            // insert gem
                            InsertIndex(ColumnManaGem, Math.Min(1.0, gemTime), t);
                            time = Math.Min(t + 0.01, t + d / 2); // block activation from moving
                            nextGem = t + 120;
                            gemTime -= 1.0;
                            if (gemTime <= eps)
                            {
                                nextGem = fight;
                                gemTime = 0.0;
                            }
                            goto DoStart;
                        }
                        t += d;
                    }
                }
                if (potTime > 0 && (((forcePot && !forceGem) || gemActivated || pot <= gem || gemTime <= 0 || (nextPot == 0 && pot < gem + 30 && potTime >= gemTime)) && (forcePot || !forceGem))/* && ((pot <= evo && nextEvo > SequenceItem.Calculations.EvocationCooldown) || evoTime <= 0)*/)
                {
                    if (pot > targetTime + 0.00001)
                    {
                        targetTime = pot;
                        lastTargetMana = -1;
                        extraMana = 0;
                        oomtime = double.PositiveInfinity;
                        goto Retry;
                    }
                    InsertIndex(ColumnManaPotion, Math.Min(1.0, potTime), pot);
                    ComputeTimestamps();
                    time = pot;
                    nextPot = pot + 120;
                    potTime -= 1.0;
                    if (potTime <= eps || nextPot > fight)
                    {
                        nextPot = fight;
                        potTime = 0.0;
                    }
                }
                else if (!gemActivated && gemTime > 0/* && (gem <= evo || (nextGem == 0 && gem < evo + 30) || evoTime <= 0)*/)
                {
                    if (gem > targetTime + 0.00001)
                    {
                        targetTime = gem;
                        lastTargetMana = -1;
                        extraMana = 0;
                        oomtime = double.PositiveInfinity;
                        goto Retry;
                    }
                    InsertIndex(ColumnManaGem, Math.Min(1.0, gemTime), gem);
                    ComputeTimestamps();
                    time = gem;
                    nextGem = gem + 120;
                    gemTime -= 1.0;
                    if (gemTime <= eps || nextGem > fight)
                    {
                        nextGem = fight;
                        gemTime = 0.0;
                    }
                }
                /*else if (evoTime > 0)
                {
                    if (nextEvo <= time && evoMoved) // if we ignored mana checks, but decided to wait for cooldown split, make sure not to run oom during supergroup
                    {
                        nextEvo = evo;
                        lastTargetMana = -1;
                        extraMana = 0;
                        oomtime = double.PositiveInfinity;
                        goto Retry;
                    }
                    if (evo > targetTime + 0.00001)
                    {
                        targetTime = evo;
                        lastTargetMana = -1;
                        extraMana = 0;
                        oomtime = double.PositiveInfinity;
                        goto Retry;
                    }
                    InsertIndex(SequenceItem.Calculations.ColumnEvocation, Math.Min(EvocationDuration * evocationFactor, evoTime), evo);
                    ComputeTimestamps();
                    time = evo + Math.Min(EvocationDuration * evocationFactor, evoTime);
                    nextEvo = evo + SequenceItem.Calculations.EvocationCooldown;
                    evoTime -= EvocationDuration * evocationFactor;
                    if (evoTime <= eps || nextEvo > fight)
                    {
                        evoTime = 0.0;
                        nextEvo = fight;
                    }
                }*/
                else
                {
                    break;
                }
            } while (true);
        }

        public double ManaCheck()
        {
            const double eps = 0.000001;
            double mana = SequenceItem.Calculations.StartingMana;
            for (int i = 0; i < sequence.Count; i++)
            {
                VariableType type = sequence[i].VariableType;
                double duration = sequence[i].Duration;
                double mps = sequence[i].Mps;
                if (type == VariableType.ManaPotion)
                {
                    for (double _potTime = duration; _potTime > eps; _potTime -= 1.0)
                    {
                        mana += (1 + BaseStats.BonusManaPotion) * SequenceItem.Calculations.ManaPotionValue;
                    }
                }
                else if (type == VariableType.ManaGem)
                {
                    for (double _gemTime = duration; _gemTime > eps; _gemTime -= 1.0)
                    {
                        mana += (1 + BaseStats.BonusManaGem) * SequenceItem.Calculations.ManaGemValue;
                    }
                }
                else if (type == VariableType.Evocation)
                {
                    mana += EvocationRegen * sequence[i].Duration;
                }
                else
                {
                    mana -= mps * duration;
                }
            }
            return mana;
        }

        public enum ReportMode
        {
            Listing,
            Compact
        }

        private static string TimeFormat(double time)
        {
            TimeSpan span = new TimeSpan((long)(Math.Round(time, 2) / 0.0000001));
            return string.Format("{0:00}:{1:00}.{2:000}", span.Minutes, span.Seconds, span.Milliseconds);
        }

        private class CooldownData
        {
            public EffectCooldown Effect;
            public double Cooldown;
            public double Time = double.NegativeInfinity;
            public bool Active;
            public bool Warning;
        }

        public double Evaluate(StringBuilder timing, EvaluationMode mode, params double[] data)
        {
            const double eps = 0.00001;
            double time = 0;
            double mana = SequenceItem.Calculations.StartingMana;

            ReportMode reportMode = ReportMode.Compact;

            bool coldsnap = SequenceItem.Calculations.Character.MageTalents.ColdSnap == 1;
            double coldsnapCooldownDuration = SequenceItem.Calculations.ColdsnapCooldown;
            bool gemActivated = SequenceItem.Calculations.ManaGemEffect;

            int gemCount = 0;
            double potionCooldown = 0;
            double gemCooldown = 0;
            double flameCapCooldown = 0;
            bool heroismUsed = false;
            double evocationCooldown = 0;
            double berserkingCooldown = 0;
            double apCooldown = 0;
            double piCooldown = 0;
            double ivCooldown = 0;
            double weCooldown = 0;
            double miCooldown = 0;
            double combustionCooldown = 0;

            double flameCapTime = double.NegativeInfinity;
            double berserkingTime = double.NegativeInfinity;
            double potionOfWildMagicTime = double.NegativeInfinity;
            double potionOfSpeedTime = double.NegativeInfinity;
            double combustionTime = double.NegativeInfinity;
            double moltenFuryTime = double.NegativeInfinity;
            double heroismTime = double.NegativeInfinity;
            double apTime = double.NegativeInfinity;
            double piTime = double.NegativeInfinity;
            double ivTime = double.NegativeInfinity;
            double weTime = double.NegativeInfinity;
            double miTime = double.NegativeInfinity;
            double manaGemEffectTime = double.NegativeInfinity;

            bool flameCapActive = false;
            bool berserkingActive = false;
            bool potionOfWildMagicActive = false;
            bool potionOfSpeedActive = false;
            bool combustionActive = false;
            bool moltenFuryActive = false;
            bool heroismActive = false;
            bool apActive = false;
            bool piActive = false;
            bool manaGemEffectActive = false;
            bool ivActive = false;
            bool weActive = false;
            bool miActive = false;

            double coldsnapTimeMin = double.NegativeInfinity;
            double coldsnapTimeMax = double.NegativeInfinity;

            bool potionWarning = false;
            //bool gemWarning = false;
            bool flameCapWarning = false;
            bool apWarning = false;
            bool piWarning = false;
            bool ivWarning = false;
            bool weWarning = false;
            bool miWarning = false;
            bool combustionWarning = false;
            bool berserkingWarning = false;
            bool manaWarning = false;

            CooldownData[] itemData = new CooldownData[Calculations.ItemBasedEffectCooldowns.Length];
            for (int i = 0; i < itemData.Length; i++)
            {
                itemData[i] = new CooldownData();
                itemData[i].Effect = Calculations.ItemBasedEffectCooldowns[i];
            }

            double combustionLeft = 0;

            double unexplained = 0;

            if (timing != null) timing.Length = 0;
            if (timing != null) timing.Append("*");

            // for each cooldown compute how much time is unexplainable
            for (int i = 0; i < sequence.Count; i++)
            {
                int index = sequence[i].Index;
                VariableType type = sequence[i].VariableType;
                double duration = sequence[i].Duration;
                Cycle cycle = sequence[i].Cycle;
                CastingState state = sequence[i].CastingState;
                double mps = sequence[i].Mps;
                if (sequence[i].IsManaPotionOrGem) duration = 0;
                double manabefore = mana;
                bool cooldownContinuation = false;
                if (berserkingActive || flameCapActive || potionOfWildMagicActive || potionOfSpeedActive || heroismActive || moltenFuryActive || combustionActive || apActive || ivActive || manaGemEffectActive || piActive)
                {
                    cooldownContinuation = true;
                }
                foreach (CooldownData d in itemData)
                {
                    if (d.Active)
                    {
                        cooldownContinuation = true;
                        break;
                    }
                }
                // Mana
                if (mode == EvaluationMode.ManaBelow)
                {
                    if (data[0] < 0) data[0] = 0.0;
                    if (data.Length > 2)
                    {
                        if (data[2] == 3)
                        {
                            if (potionCooldown > 0) data[1] = Math.Max(data[1], time + potionCooldown);
                        }
                        else if (data[2] == 4)
                        {
                            if (gemCooldown > 0) data[1] = Math.Max(data[1], time + gemCooldown);
                        }
                    }
                }
                if (mps > 0)
                {
                    double maxtime = mana / mps;
                    if (duration > maxtime + eps)
                    {
                        // allow some leeway due to rounding errors from LP solver
                        if (!(i == sequence.Count - 1 && mps * duration < mana + 200))
                        {
                            unexplained += duration - maxtime;
                            if (timing != null && !manaWarning) timing.AppendLine("WARNING: Will run out of mana!");
                            manaWarning = true;
                        }
                    }
                    if (mode == EvaluationMode.ManaBelow)
                    {
                        double limit = data[0];
                        double aftertime = data[1];
                        double timetolimit = (mana - limit) / mps;
                        if (time + duration > aftertime + eps)
                        {
                            if (time >= aftertime && mana < limit) return time;
                            if (time + timetolimit >= aftertime && timetolimit < duration) return time + timetolimit;
                            if (timetolimit < duration) return aftertime;
                        }
                    }
                }
                else
                {
                    if (mode == EvaluationMode.ManaBelow)
                    {
                        double limit = data[0];
                        double aftertime = data[1];
                        if (time + duration > aftertime + eps)
                        {
                            if (time >= aftertime && mana < limit) return time;
                            if (aftertime > time && mana - mps * (aftertime - time) < limit) return aftertime;
                        }
                    }
                }
                if (mode == EvaluationMode.ManaAtTime && duration > 0)
                {
                    double evalTime = data[0];
                    if (time + duration > evalTime + eps)
                    {
                        return mana - mps * (evalTime - time);
                    }
                }
                mana -= mps * duration;
                // Mana Potion
                if (type == VariableType.ManaPotion)
                {
                    if (potionCooldown > eps)
                    {
                        unexplained += sequence[i].Duration;
                        if (timing != null) timing.AppendLine("WARNING: Potion cooldown not ready!");
                    }
                    else
                    {
                        if (sequence[i].Duration > 1.0)
                        {
                            unexplained += sequence[i].Duration - 1.0;
                            if (timing != null) timing.AppendLine("WARNING: Potion ammount too big!");
                        }
                        if (timing != null) timing.AppendLine(TimeFormat(time) + ": Mana Potion (" + Math.Round(mana).ToString() + " mana)");
                        mana += (1 + BaseStats.BonusManaPotion) * SequenceItem.Calculations.ManaPotionValue;
                        potionCooldown = 120;
                        potionWarning = false;
                    }
                }
                // Mana Gem
                if (type == VariableType.ManaGem)
                {
                    if (gemCooldown > eps)
                    {
                        unexplained += sequence[i].Duration;
                        if (timing != null) timing.AppendLine("WARNING: Gem cooldown not ready!");
                    }
                    else
                    {
                        if (sequence[i].Duration > 1.0)
                        {
                            unexplained += sequence[i].Duration - 1.0;
                            if (timing != null) timing.AppendLine("WARNING: Gem ammount too big!");
                        }
                        if (timing != null) timing.AppendLine(TimeFormat(time) + ": Mana Gem (" + Math.Round(mana).ToString() + " mana)");
                        mana += (1 + BaseStats.BonusManaGem) * SequenceItem.Calculations.ManaGemValue;
                        gemCount++;
                        gemCooldown = 120;
                        //gemWarning = false;
                        if (gemActivated)
                        {
                            manaGemEffectActive = true;
                            manaGemEffectTime = time;
                        }
                    }
                }
                // Evocation
                if (type == VariableType.Evocation)
                {
                    if (i == 0 || sequence[i - 1].VariableType != VariableType.Evocation)
                    {
                        if (evocationCooldown > eps)
                        {
                            unexplained += duration;
                            if (timing != null) timing.AppendLine("WARNING: Evocation cooldown not ready!");
                        }
                        else
                        {
                            if (duration > EvocationDuration)
                            {
                                unexplained += duration - EvocationDuration;
                                if (timing != null) timing.AppendLine("WARNING: Evocation duration too long!");
                            }
                            if (timing != null) timing.AppendLine(TimeFormat(time) + ": Evocation (" + Math.Round(mana).ToString() + " mana)");
                            mana += Math.Min(EvocationDuration, duration) * EvocationRegen;
                            evocationCooldown = SequenceItem.Calculations.EvocationCooldown;
                        }
                    }
                    else
                    {
                        mana += Math.Min(EvocationDuration, duration) * EvocationRegen;
                    }
                }
                if (type == VariableType.EvocationIV)
                {
                    if (i == 0 || sequence[i - 1].VariableType != VariableType.EvocationIV)
                    {
                        if (!(sequence[i].CastingState != null && sequence[i].CastingState.EffectsActive((int)StandardEffect.IcyVeins) || (i > 0 && sequence[i - 1].CastingState != null && sequence[i - 1].CastingState.EffectsActive((int)StandardEffect.IcyVeins))))
                        {
                            unexplained += duration;
                            if (timing != null) timing.AppendLine("WARNING: Evocation (Icy Veins) without Icy Veins to activate it!");
                        }
                        else if (evocationCooldown > eps)
                        {
                            unexplained += duration;
                            if (timing != null) timing.AppendLine("WARNING: Evocation cooldown not ready!");
                        }
                        else
                        {
                            if (duration > SequenceItem.Calculations.EvocationDurationIV)
                            {
                                unexplained += duration - SequenceItem.Calculations.EvocationDurationIV;
                                if (timing != null) timing.AppendLine("WARNING: Evocation duration too long!");
                            }
                            if (timing != null) timing.AppendLine(TimeFormat(time) + ": Evocation (Icy Veins) (" + Math.Round(mana).ToString() + " mana)");
                            mana += Math.Min(SequenceItem.Calculations.EvocationDurationIV, duration) * SequenceItem.Calculations.EvocationRegenIV;
                            evocationCooldown = SequenceItem.Calculations.EvocationCooldown;
                        }
                    }
                    else
                    {
                        mana += Math.Min(SequenceItem.Calculations.EvocationDurationIV, duration) * SequenceItem.Calculations.EvocationRegenIV;
                    }
                }
                if (type == VariableType.EvocationHero)
                {
                    if (i == 0 || sequence[i - 1].VariableType != VariableType.EvocationHero)
                    {
                        if (!(sequence[i].CastingState != null && sequence[i].CastingState.EffectsActive((int)StandardEffect.Heroism) || (i > 0 && sequence[i - 1].CastingState != null && sequence[i - 1].CastingState.EffectsActive((int)StandardEffect.Heroism))))
                        {
                            unexplained += duration;
                            if (timing != null) timing.AppendLine("WARNING: Evocation (Heroism) without Heroism to activate it!");
                        }
                        else if (evocationCooldown > eps)
                        {
                            unexplained += duration;
                            if (timing != null) timing.AppendLine("WARNING: Evocation cooldown not ready!");
                        }
                        else
                        {
                            if (duration > SequenceItem.Calculations.EvocationDurationHero)
                            {
                                unexplained += duration - SequenceItem.Calculations.EvocationDurationHero;
                                if (timing != null) timing.AppendLine("WARNING: Evocation duration too long!");
                            }
                            if (timing != null) timing.AppendLine(TimeFormat(time) + ": Evocation (Heroism) (" + Math.Round(mana).ToString() + " mana)");
                            mana += Math.Min(SequenceItem.Calculations.EvocationDurationHero, duration) * SequenceItem.Calculations.EvocationRegenHero;
                            evocationCooldown = SequenceItem.Calculations.EvocationCooldown;
                        }
                    }
                    else
                    {
                        mana += Math.Min(SequenceItem.Calculations.EvocationDurationHero, duration) * SequenceItem.Calculations.EvocationRegenHero;
                    }
                }
                if (type == VariableType.EvocationIVHero)
                {
                    if (i == 0 || sequence[i - 1].VariableType != VariableType.EvocationIVHero)
                    {
                        if (!(sequence[i].CastingState != null && sequence[i].CastingState.EffectsActive((int)StandardEffect.IcyVeins | (int)StandardEffect.Heroism) || (i > 0 && sequence[i - 1].CastingState != null && sequence[i - 1].CastingState.EffectsActive((int)StandardEffect.IcyVeins | (int)StandardEffect.Heroism))))
                        {
                            unexplained += duration;
                            if (timing != null) timing.AppendLine("WARNING: Evocation (Icy Veins+Heroism) without Icy Veins+Heroism to activate it!");
                        }
                        else if (evocationCooldown > eps)
                        {
                            unexplained += duration;
                            if (timing != null) timing.AppendLine("WARNING: Evocation cooldown not ready!");
                        }
                        else
                        {
                            if (duration > SequenceItem.Calculations.EvocationDurationIVHero)
                            {
                                unexplained += duration - SequenceItem.Calculations.EvocationDurationIVHero;
                                if (timing != null) timing.AppendLine("WARNING: Evocation duration too long!");
                            }
                            if (timing != null) timing.AppendLine(TimeFormat(time) + ": Evocation (Icy Veins+Heroism) (" + Math.Round(mana).ToString() + " mana)");
                            mana += Math.Min(SequenceItem.Calculations.EvocationDurationIVHero, duration) * SequenceItem.Calculations.EvocationRegenIVHero;
                            evocationCooldown = SequenceItem.Calculations.EvocationCooldown;
                        }
                    }
                    else
                    {
                        mana += Math.Min(SequenceItem.Calculations.EvocationDurationIVHero, duration) * SequenceItem.Calculations.EvocationRegenIVHero;
                    }
                }
                if (mana < 0) mana = 0;
                if (mana > BaseStats.Mana + eps)
                {
                    if (timing != null) timing.AppendLine("INFO: Mana overflow!");
                    mana = BaseStats.Mana;
                }
                if (mana > 0) manaWarning = false;
                // Berserking
                if (berserkingActive)
                {
                    if (state != null && state.Berserking)
                    {
                        if (time + duration > berserkingTime + 10 + eps)
                        {
                            unexplained += time + duration - berserkingTime - 10;
                            if (timing != null) timing.AppendLine("WARNING: Berserking duration too long!");
                        }
                    }
                    else if (duration > 0 && 10 - (time - berserkingTime) > eps)
                    {
                        //unexplained += Math.Min(duration, 10 - (time - berserkingTime));
                        if (timing != null) timing.AppendLine("INFO: Berserking is still up!");
                    }
                }
                else
                {
                    if (state != null && state.Berserking)
                    {
                        if (berserkingCooldown > eps)
                        {
                            unexplained += duration;
                            if (timing != null && !berserkingWarning) timing.AppendLine("WARNING: Berserking cooldown not ready!");
                            berserkingWarning = true;
                        }
                        else
                        {
                            if (timing != null && reportMode == ReportMode.Listing) timing.AppendLine(TimeFormat(time) + ": Berserking (" + Math.Round(manabefore).ToString() + " mana)");
                            berserkingCooldown = 180.0f;
                            berserkingTime = time;
                            berserkingWarning = false;
                            berserkingActive = true;
                        }
                    }
                }
                // Flame Cap
                if (flameCapActive)
                {
                    if (state != null && state.FlameCap)
                    {
                        if (time + duration > flameCapTime + 60 + eps)
                        {
                            unexplained += time + duration - flameCapTime - 60;
                            if (timing != null) timing.AppendLine("WARNING: Flame Cap duration too long!");
                        }
                    }
                    else if (duration > 0 && 60 - (time - flameCapTime) > eps)
                    {
                        //unexplained += Math.Min(duration, 15 - (time - apTime));
                        if (timing != null) timing.AppendLine("INFO: Flame Cap is still up!");
                    }
                }
                else
                {
                    if (state != null && state.FlameCap)
                    {
                        if (flameCapCooldown > eps)
                        {
                            unexplained += duration;
                            if (timing != null && !flameCapWarning) timing.AppendLine("WARNING: Flame Cap cooldown not ready!");
                            flameCapWarning = true;
                        }
                        else
                        {
                            if (timing != null && reportMode == ReportMode.Listing) timing.AppendLine(TimeFormat(time) + ": Flame Cap (" + Math.Round(manabefore).ToString() + " mana)");
                            flameCapCooldown = 180;
                            flameCapTime = time;
                            flameCapWarning = false;
                            flameCapActive = true;
                        }
                    }
                }
                // Mana Gem Effect
                if (manaGemEffectActive)
                {
                    if (state != null && state.ManaGemEffect)
                    {
                        if (time + duration > manaGemEffectTime + SequenceItem.Calculations.ManaGemEffectDuration + eps)
                        {
                            unexplained += time + duration - manaGemEffectTime - SequenceItem.Calculations.ManaGemEffectDuration;
                            if (timing != null) timing.AppendLine("WARNING: Mana Gem Effect duration too long!");
                        }
                    }
                    else if (duration > 0 && SequenceItem.Calculations.ManaGemEffectDuration - (time - manaGemEffectTime) > eps)
                    {
                        //unexplained += Math.Min(duration, 15 - (time - apTime));
                        if (timing != null) timing.AppendLine("INFO: Mana Gem Effect is still up!");
                    }
                }
                // Potion of Wild Magic
                if (potionOfWildMagicActive)
                {
                    if (state != null && state.PotionOfWildMagic)
                    {
                        if (time + duration > potionOfWildMagicTime + 15 + eps)
                        {
                            unexplained += time + duration - potionOfWildMagicTime - 15;
                            if (timing != null) timing.AppendLine("WARNING: Potion of Wild Magic duration too long!");
                        }
                    }
                    else if (duration > 0 && 15 - (time - potionOfWildMagicTime) > eps)
                    {
                        //unexplained += Math.Min(duration, 15 - (time - apTime));
                        if (timing != null) timing.AppendLine("INFO: Potion of Wild Magic is still up!");
                    }
                }
                else
                {
                    if (state != null && state.PotionOfWildMagic)
                    {
                        if (potionCooldown > eps)
                        {
                            unexplained += duration;
                            if (timing != null && !potionWarning) timing.AppendLine("WARNING: Potion of Wild Magic cooldown not ready!");
                            potionWarning = true;
                        }
                        else
                        {
                            if (timing != null && reportMode == ReportMode.Listing) timing.AppendLine(TimeFormat(time) + ": Potion of Wild Magic (" + Math.Round(manabefore).ToString() + " mana)");
                            potionCooldown = 120;
                            potionOfWildMagicTime = time;
                            potionWarning = false;
                            potionOfWildMagicActive = true;
                        }
                    }
                }
                // Potion of Speed
                if (potionOfSpeedActive)
                {
                    if (state != null && state.PotionOfSpeed)
                    {
                        if (time + duration > potionOfSpeedTime + 15 + eps)
                        {
                            unexplained += time + duration - potionOfSpeedTime - 15;
                            if (timing != null) timing.AppendLine("WARNING: Potion of Speed duration too long!");
                        }
                    }
                    else if (duration > 0 && 15 - (time - potionOfSpeedTime) > eps)
                    {
                        //unexplained += Math.Min(duration, 15 - (time - apTime));
                        if (timing != null) timing.AppendLine("INFO: Potion of Speed is still up!");
                    }
                }
                else
                {
                    if (state != null && state.PotionOfSpeed)
                    {
                        if (potionCooldown > eps)
                        {
                            unexplained += duration;
                            if (timing != null && !potionWarning) timing.AppendLine("WARNING: Potion of Speed cooldown not ready!");
                            potionWarning = true;
                        }
                        else
                        {
                            if (timing != null && reportMode == ReportMode.Listing) timing.AppendLine(TimeFormat(time) + ": Potion of Speed (" + Math.Round(manabefore).ToString() + " mana)");
                            potionCooldown = 120;
                            potionOfSpeedTime = time;
                            potionWarning = false;
                            potionOfSpeedActive = true;
                        }
                    }
                }
                // items
                foreach (CooldownData d in itemData)
                {
                    if (d.Active)
                    {
                        if (state != null && state.EffectsActive(d.Effect))
                        {
                            if (time + duration > d.Time + d.Effect.Duration + eps)
                            {
                                unexplained += time + duration - d.Time - d.Effect.Duration;
                                if (timing != null) timing.AppendLine("WARNING: " + d.Effect.Name + " duration too long!");
                            }
                        }
                        else if (duration > 0 && d.Effect.Duration - (time - d.Time) > eps)
                        {
                            //unexplained += Math.Min(duration, Trinket1Duration - (time - trinket1time));
                            if (timing != null) timing.AppendLine("INFO: " + d.Effect.Name + " is still up!");
                        }
                    }
                    else
                    {
                        if (state != null && state.EffectsActive(d.Effect))
                        {
                            if (d.Cooldown > eps)
                            {
                                unexplained += duration;
                                if (timing != null && !d.Warning) timing.AppendLine("WARNING: " + d.Effect.Name + " cooldown not ready!");
                                d.Warning = true;
                            }
                            else
                            {
                                if (timing != null && reportMode == ReportMode.Listing) timing.AppendLine(TimeFormat(time) + ": " + d.Effect.Name + " (" + Math.Round(manabefore).ToString() + " mana)");
                                d.Cooldown = d.Effect.Cooldown;
                                d.Time = time;
                                d.Warning = false;
                                d.Active = true;
                            }
                        }
                    }
                }
                // Heroism
                if (heroismActive)
                {
                    if (state != null && state.Heroism)
                    {
                        if (time + duration > heroismTime + 40 + eps)
                        {
                            unexplained += time + duration - heroismTime - 40;
                            if (timing != null) timing.AppendLine("WARNING: Heroism duration too long!");
                        }
                    }
                    else if (duration > 0 && 40 - (time - heroismTime) > eps)
                    {
                        //unexplained += Math.Min(duration, 40 - (time - heroismTime));
                        if (timing != null) timing.AppendLine("INFO: Heroism is still up!");
                    }
                }
                else
                {
                    if (state != null && state.Heroism)
                    {
                        if (heroismUsed)
                        {
                            unexplained += duration;
                            if (timing != null) timing.AppendLine("WARNING: Heroism cooldown not ready!");
                        }
                        else
                        {
                            if (timing != null && reportMode == ReportMode.Listing) timing.AppendLine(TimeFormat(time) + ": Heroism (" + Math.Round(manabefore).ToString() + " mana)");
                            heroismUsed = true;
                            heroismTime = time;
                            heroismActive = true;
                        }
                    }
                }
                // Molten Fury
                if (moltenFuryActive)
                {
                    if (!(state != null && state.MoltenFury) && duration > 0)
                    {
                        //unexplained += duration;
                        if (timing != null) timing.AppendLine("INFO: Molten Fury is still up!");
                    }
                }
                else
                {
                    if (state != null && state.MoltenFury)
                    {
                        if (time < moltenFuryStart - eps)
                        {
                            unexplained += Math.Min(duration, moltenFuryStart - time);
                            if (timing != null) timing.AppendLine("WARNING: Molten Fury is not available yet!");
                        }
                        else
                        {
                            if (timing != null && reportMode == ReportMode.Listing) timing.AppendLine(TimeFormat(time) + ": Molten Fury (" + Math.Round(manabefore).ToString() + " mana)");
                            moltenFuryTime = time;
                            moltenFuryActive = true;
                        }
                    }
                }
                // Combustion
                if (combustionActive)
                {
                    if (state != null && state.Combustion)
                    {
                        if (duration / (state.CombustionDuration * cycle.CastTime / cycle.CastProcs) >= combustionLeft + eps)
                        {
                            unexplained += time + duration - combustionTime - (state.CombustionDuration * cycle.CastTime / cycle.CastProcs);
                            if (timing != null) timing.AppendLine("WARNING: Combustion duration too long!");
                        }
                        combustionLeft -= duration / (state.CombustionDuration * cycle.CastTime / cycle.CastProcs);
                    }
                    else if (cycle != null && duration > 0 && combustionLeft > eps)
                    {
                        //unexplained += Math.Min(duration, 15 - (time - apTime));
                        combustionTime = -1;
                        combustionLeft = 0;
                        combustionActive = false;
                        if (timing != null) timing.AppendLine("INFO: Combustion is still up!");
                    }
                    else if (duration > 0) // do not cancel for gems/pots
                    {
                        combustionTime = -1;
                        combustionLeft = 0;
                        combustionActive = false;
                    }
                }
                else
                {
                    if (state != null && state.Combustion)
                    {
                        if (combustionCooldown > eps)
                        {
                            unexplained += duration;
                            if (timing != null && !combustionWarning) timing.AppendLine("WARNING: Combustion cooldown not ready!");
                            combustionWarning = true;
                        }
                        else
                        {
                            if (timing != null && reportMode == ReportMode.Listing) timing.AppendLine(TimeFormat(time) + ": Combustion (" + Math.Round(manabefore).ToString() + " mana)");
                            combustionLeft = 1;
                            combustionCooldown = SequenceItem.Calculations.CombustionCooldown + (state.CombustionDuration * cycle.CastTime / cycle.CastProcs);
                            combustionTime = time;
                            combustionWarning = false;
                            combustionLeft -= duration / (state.CombustionDuration * cycle.CastTime / cycle.CastProcs);
                            combustionActive = true;
                        }
                    }
                }
                // Arcane Power
                if (apActive)
                {
                    if (state != null && state.ArcanePower)
                    {
                        if (time + duration > apTime + SequenceItem.Calculations.ArcanePowerDuration + eps)
                        {
                            unexplained += time + duration - apTime - SequenceItem.Calculations.ArcanePowerDuration;
                            if (timing != null) timing.AppendLine("WARNING: Arcane Power duration too long!");
                        }
                    }
                    else if (duration > 0 && SequenceItem.Calculations.ArcanePowerDuration - (time - apTime) > eps)
                    {
                        //unexplained += Math.Min(duration, SequenceItem.Calculations.ArcanePowerDuration - (time - apTime));
                        if (timing != null) timing.AppendLine("INFO: Arcane Power is still up!");
                    }
                }
                else
                {
                    if (state != null && state.ArcanePower)
                    {
                        if (apCooldown > eps)
                        {
                            unexplained += duration;
                            if (timing != null && !apWarning) timing.AppendLine("WARNING: Arcane Power cooldown not ready!");
                            apWarning = true;
                        }
                        else
                        {
                            if (timing != null && reportMode == ReportMode.Listing) timing.AppendLine(TimeFormat(time) + ": Arcane Power (" + Math.Round(manabefore).ToString() + " mana)");
                            apCooldown = SequenceItem.Calculations.ArcanePowerCooldown;
                            apTime = time;
                            apWarning = false;
                            apActive = true;
                        }
                    }
                }
                // Power Infusion
                if (piActive)
                {
                    if (state != null && state.PowerInfusion)
                    {
                        if (time + duration > piTime + SequenceItem.Calculations.PowerInfusionDuration + eps)
                        {
                            unexplained += time + duration - piTime - SequenceItem.Calculations.PowerInfusionDuration;
                            if (timing != null) timing.AppendLine("WARNING: Power Infusion duration too long!");
                        }
                    }
                    else if (duration > 0 && SequenceItem.Calculations.PowerInfusionDuration - (time - piTime) > eps)
                    {
                        //unexplained += Math.Min(duration, SequenceItem.Calculations.ArcanePowerDuration - (time - apTime));
                        if (timing != null) timing.AppendLine("INFO: Power Infusion is still up!");
                    }
                }
                else
                {
                    if (state != null && state.PowerInfusion)
                    {
                        if (piCooldown > eps)
                        {
                            unexplained += duration;
                            if (timing != null && !piWarning) timing.AppendLine("WARNING: Power Infusion cooldown not ready!");
                            apWarning = true;
                        }
                        else
                        {
                            if (timing != null && reportMode == ReportMode.Listing) timing.AppendLine(TimeFormat(time) + ": Power Infusion (" + Math.Round(manabefore).ToString() + " mana)");
                            piCooldown = SequenceItem.Calculations.PowerInfusionCooldown;
                            piTime = time;
                            piWarning = false;
                            piActive = true;
                        }
                    }
                }
                // Icy Veins
                if (!ivActive && state != null && state.IcyVeins)
                {
                    if (ivCooldown > eps && coldsnap)
                    {
                        // we need to coldsnap somewhere from [ivTime] and [time]
                        double restrictedColdsnapMin = Math.Max(coldsnapTimeMin, ivTime);
                        double restrictedColdsnapMax = Math.Min(coldsnapTimeMax, time);
                        if (restrictedColdsnapMax > restrictedColdsnapMin + eps)
                        {
                            // we can reuse last coldsnap
                            coldsnapTimeMin = restrictedColdsnapMin;
                            coldsnapTimeMax = restrictedColdsnapMax;
                            ivCooldown = 0.0;
                        }
                        else if (coldsnapCooldownDuration - (time - coldsnapTimeMin) <= eps)
                        {
                            // coldsnap is ready
                            coldsnapTimeMin = Math.Max(coldsnapTimeMin + coldsnapCooldownDuration, ivTime);
                            coldsnapTimeMax = time;
                            ivCooldown = 0.0;
                        }
                    }
                    if (ivCooldown > eps)
                    {
                        unexplained += duration;
                        if (timing != null && !ivWarning) timing.AppendLine("WARNING: Icy Veins cooldown not ready!");
                        ivWarning = true;
                    }
                    else
                    {
                        if (timing != null && reportMode == ReportMode.Listing) timing.AppendLine(TimeFormat(time) + ": Icy Veins (" + Math.Round(manabefore).ToString() + " mana)");
                        ivCooldown = SequenceItem.Calculations.IcyVeinsCooldown;
                        ivTime = time;
                        ivWarning = false;
                        ivActive = true;
                    }
                }
                if (ivActive)
                {
                    if (state != null && state.IcyVeins)
                    {
                        if (time + duration > ivTime + 20 + eps)
                        {
                            if (coldsnap)
                            {
                                // we need to coldsnap somewhere from [ivTime] and [ivTime + 20]
                                double restrictedColdsnapMin = Math.Max(coldsnapTimeMin, ivTime);
                                double restrictedColdsnapMax = Math.Min(coldsnapTimeMax, ivTime + 20.0);
                                if (restrictedColdsnapMax > restrictedColdsnapMin + eps)
                                {
                                    // we can reuse last coldsnap
                                    coldsnapTimeMin = restrictedColdsnapMin;
                                    coldsnapTimeMax = restrictedColdsnapMax;
                                    ivTime += 20.0;
                                    ivCooldown += 20.0;
                                }
                                else if (coldsnapCooldownDuration + coldsnapTimeMin <= ivTime + 20 + eps)
                                {
                                    coldsnapTimeMin = Math.Max(coldsnapTimeMin + coldsnapCooldownDuration, ivTime);
                                    coldsnapTimeMax = ivTime + 20.0;
                                    ivTime += 20.0;
                                    ivCooldown += 20.0;
                                }
                            }
                            if (time + duration > ivTime + 20 + eps)
                            {
                                unexplained += time + duration - ivTime - 20;
                                if (timing != null) timing.AppendLine("WARNING: Icy Veins duration too long!");
                            }
                        }
                    }
                    else if (duration > 0 && 20 - (time - ivTime) > eps)
                    {
                        //unexplained += Math.Min(duration, 20 - (time - ivTime));
                        if (timing != null) timing.AppendLine("INFO: Icy Veins is still up!");
                    }
                }
                // Water Elemental
                if (!weActive && state != null && state.WaterElemental)
                {
                    if (weCooldown > eps && coldsnap)
                    {
                        // we need to coldsnap somewhere from [weTime] and [time]
                        double restrictedColdsnapMin = Math.Max(coldsnapTimeMin, weTime);
                        double restrictedColdsnapMax = Math.Min(coldsnapTimeMax, time);
                        if (restrictedColdsnapMax > restrictedColdsnapMin + eps)
                        {
                            // we can reuse last coldsnap
                            coldsnapTimeMin = restrictedColdsnapMin;
                            coldsnapTimeMax = restrictedColdsnapMax;
                            weCooldown = 0.0;
                        }
                        else if (coldsnapCooldownDuration - (time - coldsnapTimeMin) <= eps)
                        {
                            // coldsnap is ready
                            coldsnapTimeMin = Math.Max(coldsnapTimeMin + coldsnapCooldownDuration, weTime);
                            coldsnapTimeMax = time;
                            weCooldown = 0.0;
                        }
                    }
                    if (weCooldown > eps)
                    {
                        unexplained += duration;
                        if (timing != null && !weWarning) timing.AppendLine("WARNING: Water Elemental cooldown not ready!");
                        weWarning = true;
                    }
                    else
                    {
                        if (timing != null && reportMode == ReportMode.Listing) timing.AppendLine(TimeFormat(time) + ": Water Elemental (" + Math.Round(manabefore).ToString() + " mana)");
                        weCooldown = SequenceItem.Calculations.WaterElementalCooldown;
                        weTime = time;
                        weWarning = false;
                        weActive = true;
                    }
                }
                if (weActive)
                {
                    if (state != null && state.WaterElemental)
                    {
                        if (time + duration > weTime + SequenceItem.Calculations.WaterElementalDuration + eps)
                        {
                            if (coldsnap)
                            {
                                // we need to coldsnap somewhere from [weTime] and [weTime + 45]
                                double restrictedColdsnapMin = Math.Max(coldsnapTimeMin, weTime);
                                double restrictedColdsnapMax = Math.Min(coldsnapTimeMax, weTime + SequenceItem.Calculations.WaterElementalDuration);
                                if (restrictedColdsnapMax > restrictedColdsnapMin + eps)
                                {
                                    // we can reuse last coldsnap
                                    coldsnapTimeMin = restrictedColdsnapMin;
                                    coldsnapTimeMax = restrictedColdsnapMax;
                                    weTime += SequenceItem.Calculations.WaterElementalDuration;
                                    weCooldown += SequenceItem.Calculations.WaterElementalDuration;
                                }
                                else if (coldsnapCooldownDuration - (time - coldsnapTimeMin) <= (weTime + SequenceItem.Calculations.WaterElementalDuration - time) + eps)
                                {
                                    coldsnapTimeMin = Math.Max(coldsnapTimeMin + coldsnapCooldownDuration, weTime);
                                    coldsnapTimeMax = weTime + SequenceItem.Calculations.WaterElementalDuration;
                                    weTime += SequenceItem.Calculations.WaterElementalDuration;
                                    weCooldown += SequenceItem.Calculations.WaterElementalDuration;
                                }
                            }
                            if (time + duration > weTime + SequenceItem.Calculations.WaterElementalDuration + eps)
                            {
                                unexplained += time + duration - weTime - SequenceItem.Calculations.WaterElementalDuration;
                                if (timing != null) timing.AppendLine("WARNING: Water Elemental duration too long!");
                            }
                        }
                    }
                    else if (duration > 0 && SequenceItem.Calculations.WaterElementalDuration - (time - weTime) > eps)
                    {
                        //unexplained += Math.Min(duration, 45 - (time - weTime));
                        if (timing != null) timing.AppendLine("INFO: Water Elemental is still up!");
                    }
                }
                // Mirror Image
                if (!miActive && state != null && state.MirrorImage)
                {
                    if (miCooldown > eps)
                    {
                        unexplained += duration;
                        if (timing != null && !miWarning) timing.AppendLine("WARNING: Mirror Image cooldown not ready!");
                        miWarning = true;
                    }
                    else
                    {
                        if (timing != null && reportMode == ReportMode.Listing) timing.AppendLine(TimeFormat(time) + ": Mirror Image (" + Math.Round(manabefore).ToString() + " mana)");
                        miCooldown = SequenceItem.Calculations.MirrorImageCooldown;
                        miTime = time;
                        miWarning = false;
                        miActive = true;
                    }
                }
                if (miActive)
                {
                    if (state != null && state.MirrorImage)
                    {
                        if (time + duration > miTime + SequenceItem.Calculations.MirrorImageDuration + eps)
                        {
                            unexplained += time + duration - miTime - SequenceItem.Calculations.MirrorImageDuration;
                            if (timing != null) timing.AppendLine("WARNING: Mirror Image duration too long!");
                        }
                    }
                    else if (duration > 0 && SequenceItem.Calculations.MirrorImageDuration - (time - miTime) > eps)
                    {
                        //unexplained += Math.Min(duration, 45 - (time - weTime));
                        if (timing != null) timing.AppendLine("INFO: Mirror Image is still up!");
                    }
                }
                // Move time forward
                if (mode == EvaluationMode.CooldownBreak)
                {
                    double aftertime = data[0];
                    if (aftertime >= time && aftertime <= time + duration)
                    {
                        if (!berserkingActive && !flameCapActive && !potionOfWildMagicActive && !heroismActive && !moltenFuryActive && !combustionActive && !apActive && !ivActive && !manaGemEffectActive)
                        {
                            bool valid = true;
                            foreach (CooldownData d in itemData)
                            {
                                if (d.Active)
                                {
                                    valid = false;
                                    break;
                                }
                            }
                            if (valid)
                            {
                                return aftertime;
                            }
                        }
                    }
                    if (time >= aftertime && !cooldownContinuation)
                    {
                        return time;
                    }
                }
                string label = null;
                if (type == VariableType.IdleRegen)
                {
                    label = "Idle Regen";
                }
                else if (type == VariableType.Wand)
                {
                    label = "Wand";
                }
                else if (type == VariableType.SummonWaterElemental)
                {
                    label = "Summon";
                }
                else if (type == VariableType.SummonMirrorImage)
                {
                    label = "Summon";
                }
                else if (type == VariableType.Drinking)
                {
                    label = "Drink";
                }
                else if (type == VariableType.TimeExtension)
                {
                    label = "End";
                }
                else if (type == VariableType.AfterFightRegen)
                {
                    label = "Drink";
                }
                else if (type == VariableType.ConjureManaGem)
                {
                    label = "Conjure Mana Gem";
                }
                /*else if (type == VariableType.Ward)
                {
                    label = SequenceItem.Calculations.Ward.Name;
                }*/
                else if (type == VariableType.Spell)
                {
                    label = cycle.Name;
                }
                if (reportMode == ReportMode.Listing)
                {
                    if (timing != null && label != null && (i == 0 || cycle == null || sequence[i].CastingState != sequence[i - 1].CastingState || sequence[i].Cycle != sequence[i - 1].Cycle)) timing.AppendLine(TimeFormat(time) + ": " + label + " (" + Math.Round(manabefore).ToString() + " mana)");
                }
                else if (reportMode == ReportMode.Compact)
                {
                    if (timing != null && label != null && (i == 0 || sequence[i].VariableType != sequence[i - 1].VariableType || sequence[i].CastingState != sequence[i - 1].CastingState || sequence[i].Cycle != sequence[i - 1].Cycle))
                    {
                        timing.AppendLine(TimeFormat(time) + ": " + (string.IsNullOrEmpty(state.BuffLabel) ? "" : (state.BuffLabel + "+")) + label + " (" + Math.Round(manabefore).ToString() + " mana)");
                    }
                }
                time += duration;
                apCooldown -= duration;
                piCooldown -= duration;
                ivCooldown -= duration;
                weCooldown -= duration;
                miCooldown -= duration;
                potionCooldown -= duration;
                gemCooldown -= duration;
                flameCapCooldown -= duration;
                foreach (CooldownData d in itemData)
                {
                    d.Cooldown -= duration;
                }
                combustionCooldown -= duration;
                berserkingCooldown -= duration;
                evocationCooldown -= duration;
                if (apActive && SequenceItem.Calculations.ArcanePowerDuration - (time - apTime) <= eps) apActive = false;
                if (piActive && SequenceItem.Calculations.PowerInfusionDuration - (time - piTime) <= eps) piActive = false;
                if (ivActive && 20 - (time - ivTime) <= eps) ivActive = false;
                if (weActive && SequenceItem.Calculations.WaterElementalDuration - (time - weTime) <= eps) weActive = false;
                if (miActive && SequenceItem.Calculations.MirrorImageDuration - (time - miTime) <= eps) miActive = false;
                if (heroismActive && 40 - (time - heroismTime) <= eps) heroismActive = false;
                if (potionOfWildMagicActive && 15 - (time - potionOfWildMagicTime) <= eps) potionOfWildMagicActive = false;
                if (potionOfSpeedActive && 15 - (time - potionOfSpeedTime) <= eps) potionOfSpeedActive = false;
                if (flameCapActive && 60 - (time - flameCapTime) <= eps) flameCapActive = false;
                foreach (CooldownData d in itemData)
                {
                    if (d.Active && d.Effect.Duration - (time - d.Time) <= eps) d.Active = false;
                }
                if (berserkingActive && 10 - (time - berserkingTime) <= 0) berserkingActive = false;
                if (manaGemEffectActive && SequenceItem.Calculations.ManaGemEffectDuration - (time - manaGemEffectTime) <= eps) manaGemEffectActive = false;
            }
            if (timing != null && unexplained > 0)
            {
                timing.AppendLine();
                timing.AppendLine(string.Format("Score: {0:F}", Math.Max(0, 100 - 100 * unexplained / FightDuration)));
            }

            switch (mode)
            {
                case EvaluationMode.Unexplained:
                    return unexplained;
                case EvaluationMode.ManaBelow:
                    return FightDuration;
                case EvaluationMode.CooldownBreak:
                    return FightDuration;
            }
            return unexplained;
        }
    }
}
