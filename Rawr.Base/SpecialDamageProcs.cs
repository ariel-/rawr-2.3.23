using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.DamageProcs
{
    /// <summary>
    /// The intention of this class is to calculate all Damage Procs such
    /// as Hand-Mounted Pyro Rocket, Bandit's Insignia, Icebreaker etc by
    /// feeding in your pre-calculated Character Stats. The damage amounts
    /// returned should be with the inclusions of all factors such as
    /// Spell Crit and BonusXDamageMultipliers.
    /// </summary>
    public class SpecialDamageProcs
    {
        #region Constuctors
        public SpecialDamageProcs(Character c, Stats charStats, int levelDelta,
            List<SpecialEffect> effectsList,
            Dictionary<Trigger, float> triggerIntervals, Dictionary<Trigger, float> triggerChances,
            float fightDuration, float armorDmgReduc)
        {
            Char = c;
            StatS = charStats;
            LevelDelta = levelDelta;
            EffectsList = effectsList;
            TriggerIntervals = triggerIntervals;
            TriggerChances = triggerChances;
            FightDuration = fightDuration;
            ArmorDmgReduc = armorDmgReduc;
            CreateDictionaries();
            if (TriggerIntervals.ContainsKey(Trigger.OffHandHit) || TriggerIntervals.ContainsKey(Trigger.MainHandHit))
            {
                if (c.MainHandEnchant != null)
                {
                    Stats.SpecialEffectEnumerator e = c.MainHandEnchant.Stats.SpecialEffects();
                    while (e.MoveNext()) MainHandEffects.Add(e.Current);
                }
                if (c.MainHand != null && c.MainHand.Item != null)
                {
                    Stats.SpecialEffectEnumerator e = c.MainHand.Item.Stats.SpecialEffects();
                    while (e.MoveNext()) MainHandEffects.Add(e.Current);
                }
                if (c.OffHandEnchant != null)
                {
                    Stats.SpecialEffectEnumerator e = c.OffHandEnchant.Stats.SpecialEffects();
                    while (e.MoveNext()) OffHandEffects.Add(e.Current);
                }
                if (c.OffHand != null && c.OffHand.Item != null)
                {
                    Stats.SpecialEffectEnumerator e = c.OffHand.Item.Stats.SpecialEffects();
                    while (e.MoveNext()) OffHandEffects.Add(e.Current);
                }
            }

        }
        #endregion
        #region Variables
        Character Char = null;
        Stats StatS = null;
        int LevelDelta = 0;
        float FightDuration = 0f;
        float ArmorDmgReduc = 0f;
        public Dictionary<Trigger, float> TriggerChances = new Dictionary<Trigger, float>();
        public Dictionary<Trigger, float> TriggerIntervals = new Dictionary<Trigger, float>();
        public List<SpecialEffect> EffectsList = new List<SpecialEffect>() { };
        public Dictionary<ItemDamageType, float> DamageMultipliers = new Dictionary<ItemDamageType, float>();
        public Dictionary<ItemDamageType, float> DamageCritMultipliers = new Dictionary<ItemDamageType, float>();
        public Dictionary<ItemDamageType, float> DamageResistances = new Dictionary<ItemDamageType, float>();
        public Dictionary<ItemDamageType, AttackTable> AttackTables = new Dictionary<ItemDamageType, AttackTable>();
        // Results
        public Dictionary<ItemDamageType, float> TotalDamage = new Dictionary<ItemDamageType, float>();
        public Dictionary<ItemDamageType, float> TotalDamagePerSec = new Dictionary<ItemDamageType, float>();
        public Dictionary<ItemDamageType, float> TotalNumProcs = new Dictionary<ItemDamageType, float>();
        public Dictionary<ItemDamageType, float> TotalNumProcsPerSec = new Dictionary<ItemDamageType, float>();

        private List<SpecialEffect> MainHandEffects = new List<SpecialEffect>();
        private List<SpecialEffect> OffHandEffects = new List<SpecialEffect>();
        private bool dualWieldProcs = false;
        #endregion
        #region Functions
        /// <summary>
        /// Resets the Dictionaries, you will need to run CreateDictionaries after this action.
        /// </summary>
        public void Reset() {
            TotalDamage.Clear();
            DamageMultipliers.Clear();
            DamageCritMultipliers.Clear();
            DamageResistances.Clear();
            AttackTables.Clear();
            TotalDamage.Clear();
            TotalDamagePerSec.Clear();
            TotalNumProcs.Clear();
            TotalNumProcsPerSec.Clear();
            dualWieldProcs = false;
        }
        /// <summary>
        /// Generates the necessary Damage Multipliers and Attack Tables.
        /// This function is automatically called by the constructor.
        /// </summary>
        public void CreateDictionaries()
        {
            Reset();
            // Partial Spell Resists
            float averageResist = (LevelDelta) * 0.02f;
            float resist10 = 5.0f * averageResist;
            float resist20 = 2.5f * averageResist;
            float partialResist = (resist10 * 0.1f + resist20 * 0.2f);

            { // Physical
                ItemDamageType type = ItemDamageType.Physical;
                AttackTable a = new AttackTable(Char, StatS, false, false, LevelDelta);
                float DamageMult = StatS.BonusPhysicalDamageMultiplier;
                float DamageCritMult = StatS.BonusCritMultiplier;
                DamageMultipliers.Add(type, DamageMult);
                DamageCritMultipliers.Add(type, DamageCritMult);
                DamageResistances.Add(type, 1f - ArmorDmgReduc);
                AttackTables.Add(type, a);
                TotalDamage.Add(type, 0f);
                TotalDamagePerSec.Add(type, 0f);
                TotalNumProcs.Add(type, 0f);
                TotalNumProcsPerSec.Add(type, 0f);
            }
            { // Arcane
                ItemDamageType type = ItemDamageType.Arcane;
                AttackTable a = new AttackTable(Char, StatS, true, false, LevelDelta);
                float DamageMult = StatS.BonusArcaneDamageMultiplier;
                float DamageCritMult = StatS.BonusSpellCritMultiplier;
                DamageMultipliers.Add(type, DamageMult);
                DamageCritMultipliers.Add(type, DamageCritMult);
                DamageResistances.Add(type, partialResist);
                AttackTables.Add(type, a);
                TotalDamage.Add(type, 0f);
                TotalDamagePerSec.Add(type, 0f);
                TotalNumProcs.Add(type, 0f);
                TotalNumProcsPerSec.Add(type, 0f);
            }
            { // Holy
                ItemDamageType type = ItemDamageType.Holy;
                AttackTable a = new AttackTable(Char, StatS, true, false, LevelDelta);
                float DamageMult = StatS.BonusHolyDamageMultiplier;
                float DamageCritMult = StatS.BonusSpellCritMultiplier;
                DamageMultipliers.Add(type, DamageMult);
                DamageCritMultipliers.Add(type, DamageCritMult);
                DamageResistances.Add(type, partialResist);
                AttackTables.Add(type, a);
                TotalDamage.Add(type, 0f);
                TotalDamagePerSec.Add(type, 0f);
                TotalNumProcs.Add(type, 0f);
                TotalNumProcsPerSec.Add(type, 0f);
            }
            { // Nature
                ItemDamageType type = ItemDamageType.Nature;
                AttackTable a = new AttackTable(Char, StatS, true, false, LevelDelta);
                float DamageMult = StatS.BonusNatureDamageMultiplier;
                float DamageCritMult = StatS.BonusSpellCritMultiplier;
                DamageMultipliers.Add(type, DamageMult);
                DamageCritMultipliers.Add(type, DamageCritMult);
                DamageResistances.Add(type, partialResist);
                AttackTables.Add(type, a);
                TotalDamage.Add(type, 0f);
                TotalDamagePerSec.Add(type, 0f);
                TotalNumProcs.Add(type, 0f);
                TotalNumProcsPerSec.Add(type, 0f);
            }
            { // Shadow
                ItemDamageType type = ItemDamageType.Shadow;
                AttackTable a = new AttackTable(Char, StatS, true, false, LevelDelta);
                float DamageMult = StatS.BonusShadowDamageMultiplier;
                float DamageCritMult = StatS.BonusSpellCritMultiplier;
                DamageMultipliers.Add(type, DamageMult);
                DamageCritMultipliers.Add(type, DamageCritMult);
                DamageResistances.Add(type, partialResist);
                AttackTables.Add(type, a);
                TotalDamage.Add(type, 0f);
                TotalDamagePerSec.Add(type, 0f);
                TotalNumProcs.Add(type, 0f);
                TotalNumProcsPerSec.Add(type, 0f);
            }
            { // Fire
                ItemDamageType type = ItemDamageType.Fire;
                AttackTable a = new AttackTable(Char, StatS, true, false, LevelDelta);
                float DamageMult = StatS.BonusFireDamageMultiplier;
                float DamageCritMult = StatS.BonusSpellCritMultiplier;
                DamageMultipliers.Add(type, DamageMult);
                DamageCritMultipliers.Add(type, DamageCritMult);
                DamageResistances.Add(type, partialResist);
                AttackTables.Add(type, a);
                TotalDamage.Add(type, 0f);
                TotalDamagePerSec.Add(type, 0f);
                TotalNumProcs.Add(type, 0f);
                TotalNumProcsPerSec.Add(type, 0f);
            }
            { // Frost
                ItemDamageType type = ItemDamageType.Frost;
                AttackTable a = new AttackTable(Char, StatS, true, false, LevelDelta);
                float DamageMult = StatS.BonusFrostDamageMultiplier;
                float DamageCritMult = StatS.BonusSpellCritMultiplier;
                DamageMultipliers.Add(type, DamageMult);
                DamageCritMultipliers.Add(type, DamageCritMult);
                DamageResistances.Add(type, partialResist);
                AttackTables.Add(type, a);
                TotalDamage.Add(type, 0f);
                TotalDamagePerSec.Add(type, 0f);
                TotalNumProcs.Add(type, 0f);
                TotalNumProcsPerSec.Add(type, 0f);
            }
        }
        /// <summary>
        /// Performs the meat and potatoes intention of this class.
        /// All the DPS, etc variables will be populated.
        /// Do not run Calculate multiple times for the same ItemDamageType without resetting.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public float Calculate(ItemDamageType type) {
            float retVal = 0f;
            try{
                foreach (SpecialEffect effect in EffectsList) {
                    if (effect == null
                        || effect.Stats == null
                        || !TriggerChances.ContainsKey(effect.Trigger)
                        || !TriggerIntervals.ContainsKey(effect.Trigger))
                    {
                    }else if(effect.Stats.PhysicalDamage > 0 && type == ItemDamageType.Physical) { retVal = CalculateTotalDamagePerSecond(effect, effect.Stats.PhysicalDamage, type);
                    }else if(effect.Stats.ArcaneDamage   > 0 && type == ItemDamageType.Arcane  ) { retVal = CalculateTotalDamagePerSecond(effect, effect.Stats.ArcaneDamage, type);
                    }else if(effect.Stats.HolyDamage     > 0 && type == ItemDamageType.Holy    ) { retVal = CalculateTotalDamagePerSecond(effect, effect.Stats.HolyDamage, type);
                    }else if(effect.Stats.NatureDamage   > 0 && type == ItemDamageType.Nature  ) { retVal = CalculateTotalDamagePerSecond(effect, effect.Stats.NatureDamage, type);
                    }else if(effect.Stats.ShadowDamage   > 0 && type == ItemDamageType.Shadow  ) { retVal = CalculateTotalDamagePerSecond(effect, effect.Stats.ShadowDamage, type);
                    }else if(effect.Stats.FireDamage     > 0 && type == ItemDamageType.Fire    ) { retVal = CalculateTotalDamagePerSecond(effect, effect.Stats.FireDamage, type);
                    }else if(effect.Stats.FrostDamage    > 0 && type == ItemDamageType.Frost   ) { retVal = CalculateTotalDamagePerSecond(effect, effect.Stats.FrostDamage, type);}
                }
            }catch(Exception ex){
                Rawr.Base.ErrorBox eb = new Rawr.Base.ErrorBox("Error calculating special proc DPS",
                    ex.Message, "Calculate(...)", "", ex.StackTrace);
            }
            return retVal;
        }

        /// <summary>
        /// Functional equivalent of calling Calculate() over all possible ItemDamageTypes, with less perf implication.  Note that you should only call this on a new object or after calling reset
        /// </summary>
        /// <returns>Total DPS from all procs</returns>
        public float CalculateAll()
        {
            float retVal = 0f;

            foreach (SpecialEffect effect in EffectsList)
            {
                if (!TriggerChances.ContainsKey(effect.Trigger) || effect.Stats == null) continue;
                if (effect.Stats.PhysicalDamage > 0) retVal += CalculateTotalDamagePerSecond(effect, effect.Stats.PhysicalDamage, ItemDamageType.Physical);
                if (effect.Stats.ArcaneDamage > 0) retVal += CalculateTotalDamagePerSecond(effect, effect.Stats.ArcaneDamage, ItemDamageType.Arcane);
                if (effect.Stats.HolyDamage > 0) retVal += CalculateTotalDamagePerSecond(effect, effect.Stats.HolyDamage, ItemDamageType.Holy);
                if (effect.Stats.NatureDamage > 0) retVal += CalculateTotalDamagePerSecond(effect, effect.Stats.NatureDamage, ItemDamageType.Nature);
                if (effect.Stats.ShadowDamage> 0) retVal += CalculateTotalDamagePerSecond(effect, effect.Stats.ShadowDamage, ItemDamageType.Shadow);
                if (effect.Stats.FireDamage > 0) retVal += CalculateTotalDamagePerSecond(effect, effect.Stats.FireDamage, ItemDamageType.Fire);
                if (effect.Stats.FrostDamage > 0) retVal += CalculateTotalDamagePerSecond(effect, effect.Stats.FrostDamage, ItemDamageType.Frost);
            }

            return retVal;
        }

        private float CalculateTotalDamagePerSecond(SpecialEffect effect, float baseDamage, ItemDamageType type) {
            float totalDamage = 0f;
            float totalDamagePerSec = 0f;
            float totalNumProcs = 0f;
            float totalNumProcsPerSec = 0f;
            try {
                float triggerInterval = TriggerIntervals[effect.Trigger];
                float triggerChance = TriggerChances[effect.Trigger];
                if (TriggerChances.ContainsKey(Trigger.MainHandHit) && MainHandEffects.Contains(effect) && (!OffHandEffects.Contains(effect) || !dualWieldProcs))
                {
                    triggerInterval = TriggerIntervals[Trigger.MainHandHit];
                    triggerChance = TriggerChances[Trigger.MainHandHit];
                    if (OffHandEffects.Contains(effect)) dualWieldProcs = true;
                }
                else if (TriggerChances.ContainsKey(Trigger.OffHandHit) && OffHandEffects.Contains(effect))
                {
                    triggerInterval = TriggerIntervals[Trigger.OffHandHit];
                    triggerChance = TriggerChances[Trigger.OffHandHit];
                }
                // Process the Effects
                totalNumProcsPerSec = effect.GetAverageProcsPerSecond(triggerInterval, triggerChance, Char.MainHand.Speed, FightDuration);
                totalNumProcs = totalNumProcsPerSec * FightDuration;
                totalDamage = totalNumProcs * CalculateThisDamage(type, baseDamage);
                totalDamagePerSec = totalDamage / FightDuration;

                // Set our Results into the Dictionaries
                TotalDamage[type] += totalDamage;
                TotalDamagePerSec[type] += totalDamagePerSec;
                TotalNumProcs[type] += totalNumProcs;
                TotalNumProcsPerSec[type] += totalNumProcsPerSec;
            } catch (Exception ex) {
                Rawr.Base.ErrorBox eb = new Rawr.Base.ErrorBox("Error calculating special proc DPS",
                    ex.Message, "CalculateTotalDamagePerSecond(...)", "", ex.StackTrace);
            }

            return totalDamagePerSec;
        }
        private float CalculateThisDamage(ItemDamageType type, float baseDamage) {
            float dmg = 0;
            try {
                AttackTable MHAtkTable = AttackTables[type];
                dmg  = baseDamage; // Base Damage
                dmg *= 1f + DamageMultipliers[type];     // Global Damage Bonuses
                dmg *= 1f - DamageResistances[type];     // Global Damage Penalties

                // Work the Attack Table, note: some of these points will always be zero
                float dmgDrop = (1f
                    - MHAtkTable.Miss   // no damage when being missed
                    - MHAtkTable.Dodge  // no damage when being dodged
                    - MHAtkTable.Parry  // no damage when being parried
                    - MHAtkTable.Glance // glancing handled below
                    - MHAtkTable.Block  // blocked  handled below
                    - MHAtkTable.Crit); // crits    handled below

                float dmgGlance = dmg * MHAtkTable.Glance * 0.70f; // Partial Damage when glancing
                float dmgBlock  = dmg * MHAtkTable.Block  * 0.70f; // Partial damage when blocked
                float dmgCrit   = dmg * MHAtkTable.Crit   * (1f + DamageCritMultipliers[type]); // Bonus Damage when critting

                dmg *= dmgDrop;

                dmg += dmgGlance + dmgBlock + dmgCrit;
            } catch (Exception ex) {
                Rawr.Base.ErrorBox eb = new Rawr.Base.ErrorBox("Error calculating special proc DPS",
                    ex.Message, "CalculateThisDamage(...)", "", ex.StackTrace);
            }

            return dmg;
        }
        #endregion
        // Bandit's Insignia
        // Equip: Your melee and ranged attacks have a chance to strike your enemy, dealing 1504 to 2256 arcane damage.
        // Hand-Mounted Pyro Rocket
        // Use: Deal 1654 to 2020 Fire damage to an enemy at long range. The rocket can only be fired once every 45 sec.

    }

    #region Combat Table
    public abstract class CombatTable {
        public static CombatTable NULL = new NullCombatTable();
        protected Character Char;
        protected Stats StatS;
        protected bool useSpellHit = false;
        protected int LevelDelta = 0;
        public WeightedStat[] critProcs { get; set; }
        
        public float Miss { get; set; }
        public float Dodge { get; set; }
        public float Parry { get; set; }
        public float Block { get; set; }
        public float Glance { get; set; }
        public float Crit { get; set; }
        public float Hit { get; set; }

        private float _anyLand = 0f;
        private float _anyNotLand = 1f;
        public float AnyLand { get { return _anyLand; } }
        public float AnyNotLand { get { return _anyNotLand; } }
        private bool _alwaysHit = false;
        protected virtual void Calculate() {
            _anyNotLand = Dodge + Parry + Miss;
            _anyLand = 1f - _anyNotLand;
        }
        protected virtual void CalculateAlwaysHit()
        {
            _alwaysHit = true;
            Miss = Dodge = Parry = Block = Glance = Crit = 0f;
            Hit = 1f;
            _anyLand = 1f;
            _anyNotLand = 0f;
        }

        protected void Initialize(Character character, Stats stats, bool useSpellHit, bool alwaysHit, int delta) {
            Char = character;
            StatS = stats;
            this.useSpellHit = useSpellHit;
            LevelDelta = delta;
            critProcs = new WeightedStat[] { new WeightedStat() { Chance = 1f, Value = 0f } };
            /*// Defaults
            Miss 
            Dodge
            Parry
            Block
            Glance
            Critical
            Hit*/
            // Start a calc            
            Reset(alwaysHit);            
        }
        protected void Reset(bool alwaysHit)
        {
            if (alwaysHit) CalculateAlwaysHit();
            else Calculate();
        }
        public void Reset()
        {
            if (_alwaysHit) return;
            Reset(false);
        }
    }

    public class NullCombatTable : CombatTable
    {
        public NullCombatTable()
        {
            Block = Crit = Hit = Dodge = Glance = Miss = Parry = 0;
        }
    }
    public class AttackTable : CombatTable {
        protected override void Calculate() {
            float tableSize = 0f;

            // Miss
            if (useSpellHit) {
                Miss = Math.Min(1f - tableSize, Math.Max(0f, StatConversion.GetSpellMiss(-LevelDelta, false) - StatS.SpellHit - StatConversion.GetSpellHitFromRating(StatS.HitRating)));
            } else {
                Miss = Math.Min(1f - tableSize, Math.Max(0f, StatConversion.YELLOW_MISS_CHANCE_CAP[LevelDelta] - StatS.PhysicalHit - StatConversion.GetSpellHitFromRating(StatS.HitRating)));
            }
            tableSize += Miss;
            // Dodge
            if (!useSpellHit) {
                Dodge = Math.Min(1f - tableSize, Math.Max(0f, StatConversion.YELLOW_DODGE_CHANCE_CAP[LevelDelta]
                                                 - StatConversion.GetDodgeParryReducFromExpertise(
                                                    StatConversion.GetExpertiseFromRating(StatS.ExpertiseRating, Char.Class), Char.Class))
                                );
                tableSize += Dodge;
            } else { Dodge = 0f; }
            // Parry
            if (!useSpellHit) {
                Parry = Math.Min(1f - tableSize, Math.Max(0f, StatConversion.YELLOW_PARRY_CHANCE_CAP[LevelDelta]
                                                 - StatConversion.GetDodgeParryReducFromExpertise(
                                                    StatConversion.GetExpertiseFromRating(StatS.ExpertiseRating, Char.Class), Char.Class))
                                );
                tableSize += Parry;
            } else { Parry = 0f; }
            // Block
            //if (isWhite || Abil.CanBeBlocked) {
                //Block = Math.Min(1f - tableSize, isMH ?  combatFactors._c_mhblock : combatFactors._c_ohblock);
                //tableSize += Block;
            //} else { Block = 0f; }
            // Glancing Blow
            //if (isWhite) {
                //Glance = Math.Min(1f - tableSize, combatFactors._c_glance);
                //tableSize += Glance;
            //} else { Glance = 0f; }
            // Critical Hit
            Crit = 0;
            /*if (isWhite) {
                float critValueToUse = (isMH ? combatFactors._c_mhwcrit : combatFactors._c_ohwcrit);
                foreach (WeightedStat ws in combatFactors.critProcs)
                {
                    float modCritChance = Math.Min(1f - tableSize, critValueToUse + StatConversion.GetCritFromRating(ws.Value, Char.Class))
                        + StatConversion.NPC_LEVEL_CRIT_MOD[calcOpts.TargetLevel - Char.Level];
                    Crit += ws.Chance * modCritChance;
                }
                tableSize += Crit;
            } else if (Abil.CanCrit) {*/
                float critValueToUse =  StatConversion.NPC_LEVEL_CRIT_MOD[LevelDelta]
                    + (useSpellHit ? StatS.SpellCrit : StatS.PhysicalCrit);
                foreach (WeightedStat ws in critProcs)
                {
                    float modCritChance = Math.Min(1f - tableSize, (critValueToUse + StatConversion.GetCritFromRating(ws.Value, Char.Class)) * (1f - Dodge - Miss));
                    Crit += ws.Chance * modCritChance;
                }
                if (Crit < 0) Crit = 0f;
                tableSize += Crit;
            //}
            // Normal Hit
            Hit = Math.Max(0f, 1f - tableSize);
            base.Calculate();
        }

        public AttackTable() { }

        public AttackTable(Character character, Stats stats, bool useSpellHit, bool alwaysHit, int delta) {
            Initialize(character, stats, useSpellHit, alwaysHit, delta);
        }
    }
    #endregion
}
