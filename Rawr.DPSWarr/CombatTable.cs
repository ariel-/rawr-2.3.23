using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.DPSWarr
{
    public abstract class CombatTable {
        public static CombatTable NULL = new NullCombatTable();
        protected Character Char;
        protected CalculationOptionsDPSWarr calcOpts;
        protected BossOptions bossOpts;
        protected CombatFactors combatFactors;
        protected Stats StatS;
        protected Skills.Ability Abil;
        protected bool useSpellHit = false;

        public bool isWhite;
        public bool isMH;

        public int levelDif
        {
            get {
                return
#if RAWR3 || SILVERLIGHT
                    bossOpts.Level
#else
                    calcOpts.TargetLevel
#endif
                    - Char.Level;
            }
        }
        
        public float Miss { get; protected set; }
        public float Dodge { get; protected set; }
        public float Parry { get; protected set; }
        public float Block { get; protected set; }
        public float Glance { get; protected set; }
        public float Crit { get; protected set; }
        public float Hit { get; protected set; }

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

        protected void Initialize(Character character, Stats stats, CombatFactors cf, CalculationOptionsDPSWarr co, BossOptions bo,
            Skills.Ability ability, bool ismh, bool useSpellHit, bool alwaysHit)
        {
            Char = character;
            StatS = stats;
            calcOpts = co;
            bossOpts = bo;
            combatFactors = cf;
            Abil = ability;
            isWhite = (Abil == null);
            isMH = ismh;
            this.useSpellHit = useSpellHit;
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
                Miss = Math.Min(1f - tableSize, Math.Max(0.17f - (StatConversion.GetHitFromRating(StatS.HitRating, Char.Class) + StatS.SpellHit), 0f));
            } else {
                Miss = Math.Min(1f - tableSize, isWhite ? combatFactors._c_wmiss : combatFactors._c_ymiss);
            }
            tableSize += Miss;
            // Dodge
            if (isWhite || Abil.CanBeDodged) {
                Dodge = Math.Min(1f - tableSize, isMH ? combatFactors._c_mhdodge : combatFactors._c_ohdodge);
                tableSize += Dodge;
            } else { Dodge = 0f; }
            // Parry
            if (isWhite || Abil.CanBeParried) {
                Parry = Math.Min(1f - tableSize, isMH ? combatFactors._c_mhparry : combatFactors._c_ohparry);
                tableSize += Parry;
            } else { Parry = 0f; }
            // Block
            if (isWhite || Abil.CanBeBlocked) {
                Block = Math.Min(1f - tableSize, isMH ?  combatFactors._c_mhblock : combatFactors._c_ohblock);
                tableSize += Block;
            } else { Block = 0f; }
            // Glancing Blow
            if (isWhite) {
                Glance = Math.Min(1f - tableSize, combatFactors._c_glance);
                tableSize += Glance;
            } else { Glance = 0f; }
            // Critical Hit
            Crit = 0;
            if (isWhite) {
                float critValueToUse = (isMH ? combatFactors._c_mhwcrit : combatFactors._c_ohwcrit)
                                            + StatConversion.NPC_LEVEL_CRIT_MOD[levelDif];
                foreach (WeightedStat ws in combatFactors.critProcs)
                {
                    float modCritChance = Math.Min(1f - tableSize, critValueToUse + StatConversion.GetCritFromRating(ws.Value, Char.Class));
                    Crit += ws.Chance * modCritChance;
                }
                tableSize += Crit;
            } else if (Abil.CanCrit) {
                float critValueToUse = StatConversion.NPC_LEVEL_CRIT_MOD[levelDif]
                    + (isMH ? combatFactors._c_mhycrit : combatFactors._c_ohycrit)
                    + Abil.BonusCritChance;
                foreach (WeightedStat ws in combatFactors.critProcs)
                {
                    float modCritChance = Math.Min(1f - tableSize, (critValueToUse + StatConversion.GetCritFromRating(ws.Value, Char.Class)) * (1f - Dodge - Miss));
                    Crit += ws.Chance * modCritChance;
                }
                tableSize += Crit;
            }
            // Normal Hit
            Hit = Math.Max(0f, 1f - tableSize);
            base.Calculate();
        }

        public AttackTable() { }

        public AttackTable(Character character, Stats stats, CombatFactors cf, CalculationOptionsDPSWarr co, BossOptions bo,bool ismh, bool useSpellHit, bool alwaysHit) {
            Initialize(character, stats, cf, co, bo, null, ismh, useSpellHit, alwaysHit);
        }

        public AttackTable(Character character, Stats stats, CombatFactors cf, CalculationOptionsDPSWarr co, BossOptions bo, Skills.Ability ability, bool ismh, bool useSpellHit, bool alwaysHit) {
            Initialize(character, stats, cf, co, bo, ability, ismh, useSpellHit, alwaysHit);
        }
    }
}
