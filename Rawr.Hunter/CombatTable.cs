using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Hunter
{
    public abstract class CombatTable {
        public static CombatTable NULL = new NullCombatTable();
        protected Character Char;
        protected CalculationOptionsHunter calcOpts;
        protected CombatFactors combatFactors;
        protected Stats StatS;
        protected Skills.Ability Abil;
        protected PetAttacks PetAbil;
        protected bool useSpellHit = false;

        public bool isWhite;
        
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
        protected virtual void Calculate(float[] avoidChances) {
            _anyNotLand = Dodge + Parry + Miss;
            _anyLand = 1f - _anyNotLand;
        }
        protected virtual void CalculateAlwaysHit() {
            Miss = Dodge = Parry = Block = Glance = Crit = 0f;
            Hit = 1f;
            _anyLand = 1f;
            _anyNotLand = 0f;
        }

        protected void Initialize(Character character, Stats stats, CombatFactors cf, CalculationOptionsHunter co,
            Skills.Ability ability, bool useSpellHit, bool alwaysHit)
        {
            Char = character;
            StatS = stats;
            calcOpts = co;
            combatFactors = cf;
            Abil = ability;
            PetAbil = PetAttacks.None;
            isWhite = (Abil == null);
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
            if (alwaysHit) CalculateAlwaysHit();
            else Calculate();            
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

        public override string ToString() {
            return string.Format("Land: {0:0.00%} "
                               + "Not Land: {1:0.00%} "
                               + "Miss: {2:0.00%} "
                               + "Dodge: {3:0.00%} "
                               + "Parry: {4:0.00%} "
                               + "Glance: {5:0.00%} "
                               + "Crit: {6:0.00%} "
                               + "Hit: {7:0.00%}",
                               AnyLand,AnyNotLand,
                               Miss,Dodge,Parry,Glance,Crit,Hit);
        }
    }

    public class NullCombatTable : CombatTable
    {
        public NullCombatTable()
        {
            Block = Crit = Hit = Dodge = Glance = Miss = Parry = 0;
        }
    }
    public class AttackTable : CombatTable
    {
        protected override void Calculate() {
            float tableSize = 0f;

            // Miss
            if (useSpellHit) {
                //float hitIncrease = StatConversion.GetHitFromRating(StatS.HitRating, Char.Class) + StatS.SpellHit;
                Miss = Math.Min(1f - tableSize, Math.Max(0.17f - (StatConversion.GetHitFromRating(StatS.HitRating, Char.Class) + StatS.SpellHit), 0f));
            } else {
                Miss = Math.Min(1f - tableSize, isWhite ? combatFactors._c_wmiss : combatFactors._c_ymiss);
            }
            tableSize += Miss;
            // Dodge
            if (isWhite || Abil.CanBeDodged) {
                Dodge = 0f; //Math.Min(1f - tableSize, combatFactors._c_rwdodge);
                tableSize += Dodge;
            } else { Dodge = 0f; }
            // Parry
            if (isWhite || Abil.CanBeParried) {
                Parry = 0f; // Math.Min(1f - tableSize, combatFactors._c_rwparry);
                tableSize += Parry;
            } else { Parry = 0f; }
            // Block
            if (isWhite || Abil.CanBeBlocked) {
                Block = 0f; // Math.Min(1f - tableSize, combatFactors._c_rwblock);
                tableSize += Block;
            } else { Block = 0f; }
            // Glancing Blow
            if (isWhite) {
                Glance = Math.Min(1f - tableSize, combatFactors._c_glance);
                tableSize += Glance;
            } else { Glance = 0f; }
            // Critical Hit
            if (isWhite) {
                Crit = Math.Min(1f - tableSize, combatFactors._c_rwycrit);
                tableSize += Crit;
            } else if (Abil.CanCrit) {
                Crit = Math.Min(1f - tableSize, Abil.ContainCritValue_RW);
                tableSize += Crit;
            } else {
                Crit = 0f;
            }
            // Normal Hit
            Hit = Math.Max(0f, 1f - tableSize);
            base.Calculate();
        }

        public AttackTable(Character character, Stats stats, CombatFactors cf, CalculationOptionsHunter co, bool useSpellHit, bool alwaysHit) {
            Initialize(character, stats, cf, co, null, useSpellHit, alwaysHit);
        }

        public AttackTable(Character character, Stats stats, CombatFactors cf, CalculationOptionsHunter co, Skills.Ability ability, bool useSpellHit, bool alwaysHit) {
            Initialize(character, stats, cf, co, ability, useSpellHit, alwaysHit);
        }
    }
    public class PetAttackTable : CombatTable
    {
        protected override void Calculate(float[] avoidChances) {
            float tableSize = 0f;

            // Miss
            Miss = Math.Min(1f - tableSize, (useSpellHit ? avoidChances[1] : avoidChances[0]));
            tableSize += Miss;
            // Dodge
            if (isWhite || true) { //Abil.CanBeDodged
                Dodge = Math.Min(1f - tableSize, avoidChances[2]); //Math.Min(1f - tableSize, combatFactors._c_rwdodge);
                tableSize += Dodge;
            } else { Dodge = 0f; }
            // Parry
            if (isWhite || true) { //Abil.CanBeParried
                Parry = 0f; // Math.Min(1f - tableSize, combatFactors._c_rwparry);
                tableSize += Parry;
            } else { Parry = 0f; }
            // Block
            if (isWhite || false) {//Abil.CanBeBlocked
                Block = 0f; // Math.Min(1f - tableSize, combatFactors._c_rwblock);
                tableSize += Block;
            } else { Block = 0f; }
            // Glancing Blow
            if (isWhite) {
                Glance = Math.Min(1f - tableSize, 0.25f);//combatFactors._c_glance
                tableSize += Glance;
            } else { Glance = 0f; }
            // Critical Hit
            if (isWhite) {
                Crit = Math.Min(1f - tableSize, StatS.PhysicalCrit);
            } else if (true) {//Abil.CanCrit
                Crit = Math.Min(1f - tableSize, StatS.PhysicalCrit);//Abil.ContainCritValue_RW
            } else {
                //Crit = 0f;
            }
            tableSize += Crit;
            // Normal Hit
            Hit = Math.Max(0f, 1f - tableSize);
            base.Calculate();
        }

        protected void Initialize(Character character, Stats stats, CalculationOptionsHunter co,
            float[] avoidChances, PetAttacks ability, bool useSpellHit, bool alwaysHit)
        {
            Char = character;
            StatS = stats;
            calcOpts = co;
            combatFactors = null;
            Abil = null;
            PetAbil = ability;
            isWhite = (PetAbil == PetAttacks.None);
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
            if (alwaysHit) CalculateAlwaysHit();
            else Calculate(avoidChances);
        }

        public PetAttackTable(Character character, Stats stats, CalculationOptionsHunter co,
            float[] avoidChances, bool useSpellHit, bool alwaysHit) {
            Initialize(character, stats, co, avoidChances, PetAttacks.None, useSpellHit, alwaysHit);
        }

        public PetAttackTable(Character character, Stats stats, CalculationOptionsHunter co,
            float[] avoidChances, PetAttacks ability, bool useSpellHit, bool alwaysHit) {
            Initialize(character, stats, co, avoidChances, ability, useSpellHit, alwaysHit);
        }
    }
}
