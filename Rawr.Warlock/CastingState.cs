using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Rawr.Warlock {
    
    public class CastingState {

        public CharacterCalculationsWarlock Mommy;
        public float Elapsed;
        public float Probability;
        public Dictionary<Spell, float> Cooldowns;
        public List<Spell> Series;
        public List<int> SeriesPriorities;
        public List<float> SeriesTimes;
        public List<bool> SeriesHits;

        public int BackdraftCharges;
        public int MoltenCoreCharges;

        public CastingState(
            CharacterCalculationsWarlock mommy,
            Spell precedingSpell,
            float probability) {

            Mommy = mommy;
            Probability = probability;
            Cooldowns = new Dictionary<Spell, float>();
            SeriesPriorities = new List<int>();
            Series = new List<Spell>();
            SeriesTimes = new List<float>();
            SeriesHits = new List<bool>();

            if (precedingSpell != null) {
                Elapsed = precedingSpell.GetAvgTimeUsed();
            }
        }

        public CastingState(CastingState toCopy) {

            Mommy = toCopy.Mommy;
            Probability = toCopy.Probability;
            Cooldowns = new Dictionary<Spell, float>(toCopy.Cooldowns);
            SeriesPriorities = new List<int>(toCopy.SeriesPriorities);
            Series = new List<Spell>(toCopy.Series);
            SeriesTimes = new List<float>(toCopy.SeriesTimes);
            SeriesHits = new List<bool>(toCopy.SeriesHits);

            BackdraftCharges = toCopy.BackdraftCharges;
            MoltenCoreCharges = toCopy.MoltenCoreCharges;
        }

        public float GetMaxTimeQueued(Spell spell) {

            if (Cooldowns.ContainsKey(spell)) {
                return -Cooldowns[spell];
            }

            // This method answers the qusetion, "If my spell is being cast now,
            // how long could it have been since it queued up to be cast?"
            // 
            // We know if there are lower priority spells that have already been
            // cast, it was not queued before that.  So we backtrack until we
            // find a lower priority spell.
            // 
            // Lower index == higher priority.

            float t = 0;
            int newPriority = Mommy.Priorities.IndexOf(spell);
            for (int i = SeriesTimes.Count; --i >= 0; ) {
                t += SeriesTimes[i];
                if (SeriesPriorities[i] > newPriority) {
                    return t;
                }
            }
            return Elapsed;
        }

        public void AddSpell(Spell spell, float timeAdvance, bool hit) {

            Elapsed += timeAdvance;
            Series.Add(spell);
            SeriesTimes.Add(timeAdvance);
            SeriesPriorities.Add(Mommy.Priorities.IndexOf(spell));
            SeriesHits.Add(hit);
            foreach (Spell key in new List<Spell>(Cooldowns.Keys)) {
                Cooldowns[key] -= timeAdvance;
            }
        }

        public override string ToString() {

            string str = string.Format("{0:0.00000}:", Probability);
            foreach (Spell spell in Series) {
                str += " " + spell.GetType().Name;
            }
            return str;
        }

        public bool LastCastHit(Spell spell) {

            return SeriesHits[Series.LastIndexOf(spell)];
        }
    }
}
