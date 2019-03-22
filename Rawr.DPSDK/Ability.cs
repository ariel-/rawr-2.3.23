using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.DPSDK
{
    public enum RunesUsed
    {
        Blood,
        Frost,
        Unholy,
        UnholyFrost,
        RunicPower,
        BloodUnholyFrost,
        None
    }
    public enum GCDType
    {
        Spell,
        Melee,
        None
    }
   
    public abstract class RuneAbility
    {
        private Character _character;

        public Character character
        {
            get { return _character; }
            set { _character = value; }
        }

        

        private String _name;
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private double _damageMod;
        public double DamageMod
        {
            get { return _damageMod; }
            set { _damageMod = value; }
        }

        private double _secondaryDamageMod;
        public double SecondaryDamageMod
        {
            get { return _secondaryDamageMod; }
            set { _secondaryDamageMod = value; }
        }

        public abstract double Damage { get; }

        public abstract double SecondaryDamage{get;}

        private double _staticThreat;
        public double StaticThreat
        {
            get { return _staticThreat; }
            set { _staticThreat = value; }
        }

        private double _threatMod;
        public double ThreatMod
        {
            get { return _threatMod; }
            set { _threatMod = value; }
        }
        private Stats _stats;
        public Stats stats
        {
            get { return _stats; }
            set { _stats = value; }
        }
        
        private CombatTable _combatTable;

        public CombatTable combatTable
        {
            get { return _combatTable; }
            set { _combatTable = value; }
        }

        private RunesUsed _runes;

        public RunesUsed Runes
        {
            get { return _runes; }
            set { _runes = value; }
        }
        private GCDType gcdType;

        public GCDType GcdType
        {
            get { return gcdType; }
            set { gcdType = value; }
        }
        private CalculationOptionsDPSDK _calcOpts;

        public CalculationOptionsDPSDK calcOpts
        {
            get { return _calcOpts; }
            set { _calcOpts = value; }
        }
        private DeathKnightTalents _talents;
        public DeathKnightTalents talents
        {
            get { return _talents; }
            set { _talents = value; }
        }
        private double _threat;
        public double Threat
        {
            get
            {
                _threat = Damage + SecondaryDamage;
                _threat += StaticThreat;
                _threat *= ThreatMod;
                if (calcOpts.rotation.presence != CalculationOptionsDPSDK.Presence.Frost)
                {
                    _threat *= 0.8f;
                    _threat *= 1f - (talents.Subversion * 25d / 3d);
                }
                else
                {
                    _threat *= 1.45f;
                }
                return _threat;
            }
        }
        public RuneAbility()
        {
            character = null;
            stats = null;
            combatTable = null;
            Runes = RunesUsed.Blood;
            gcdType = GCDType.Melee;
            calcOpts = null;
        }
        public override String ToString()
        {
            return Name;
        }
    }
    
    public class BloodStrike : RuneAbility
    {
        public BloodStrike(Character c, Stats s, CalculationOptionsDPSDK CalcOpts, CombatTable table, DeathKnightTalents Talents)
        {
            character = c; stats = s; calcOpts = CalcOpts; combatTable = table; GcdType = GCDType.Melee; Runes = RunesUsed.Blood; talents = Talents; Name = "Blood Strike";
        }
        private double _damage;
        public override double Damage
        {
            get
            {
                double BSDmg = ((combatTable.MH.baseDamage + ((stats.AttackPower / 14d) * combatTable.normalizationFactor) + 764d) *
                            0.4d) + stats.BonusBloodStrikeDamage;
                if (combatTable.DW)
                {
                    double BSDmgOH = ((combatTable.OH.baseDamage + ((stats.AttackPower / 14d) *
                            combatTable.normalizationFactor) + 764d) * 0.4d);
                    BSDmgOH *= 0.5d;
                    BSDmgOH += stats.BonusBloodStrikeDamage;
                    BSDmgOH *= 1d + 0.08333333333d * (double)talents.NervesOfColdSteel;
                    BSDmgOH *= (talents.ThreatOfThassarian * (1d / 3d));
                    BSDmg += BSDmgOH;
                }
                BSDmg *= 1d + 0.125d * (double)calcOpts.rotation.AvgDiseaseMult * (1d + stats.BonusPerDiseaseBloodStrikeDamage);
                _damage = BSDmg;
                double BSCritDmgMult = 1d + (.15d * (double)talents.MightOfMograine);
                BSCritDmgMult += (.15d * (double)talents.GuileOfGorefiend) + stats.BonusCritMultiplier;
                double BSCrit = 1d + ((combatTable.physCrits + (.03f * (double)talents.Subversion)) * BSCritDmgMult);
                _damage = (_damage) * BSCrit;  
                return _damage * DamageMod;
            }
        }
        public override double SecondaryDamage
        {
            get { return 0; }
        }
    }
  
    public class  DeathCoil : RuneAbility
    {
        public DeathCoil(Character c, Stats s, CalculationOptionsDPSDK CalcOpts, CombatTable table, DeathKnightTalents Talents)
        {
            character = c; stats = s; calcOpts = CalcOpts; combatTable = table; GcdType = GCDType.Spell; Runes = RunesUsed.RunicPower; talents = Talents; Name = "Death Coil";
        }
        private double _damage;
        public override double Damage
        {
            get
            {
                _damage = 443f + ((0.15d * (1d + (talents.Impurity * .04d))) * stats.AttackPower) + stats.BonusDeathCoilDamage;
                double DCCritDmgMult = 0.5f * (2f + stats.BonusCritMultiplier);
                double DCCrit = 1d + ((combatTable.spellCrits + stats.BonusDeathCoilCrit) * DCCritDmgMult);
                _damage *= DCCrit;
                return _damage * DamageMod;
            }
        }
        public override double SecondaryDamage
        {
            get { return 0; }
        }
    }

    public class IcyTouch : RuneAbility
    {
        public IcyTouch(Character c, Stats s, CalculationOptionsDPSDK CalcOpts, CombatTable table, DeathKnightTalents Talents)
        {
            character = c; stats = s; calcOpts = CalcOpts; combatTable = table; GcdType = GCDType.Spell; Runes = RunesUsed.Frost; talents = Talents; Name = "Icy Touch";
        }
        private double _damage;
        public override double Damage
        {
            get
            {
                _damage = 236f + ((0.1d * (1d + talents.Impurity * .04d)) * stats.AttackPower) + stats.BonusIcyTouchDamage;
                double ITCritDmgMult = .5f * (2f + stats.CritBonusDamage + stats.BonusCritMultiplier);
                double ITCrit = 1f + (Math.Min((combatTable.spellCrits + (.05f * (float)talents.Rime)), 1f) * ITCritDmgMult);
                _damage *= ITCrit;          
                return _damage * DamageMod;
            }
        }
        public override double SecondaryDamage
        {
            get { return 0; }
        }
    }

    public class PlagueStrike : RuneAbility
    {
        public PlagueStrike(Character c, Stats s, CalculationOptionsDPSDK CalcOpts, CombatTable table, DeathKnightTalents Talents)
        {
            character = c; stats = s; calcOpts = CalcOpts; combatTable = table; talents = Talents; GcdType = GCDType.Melee; Runes = RunesUsed.Unholy; Name = "Plague Strike";
        }
        private double _damage;
        public override double Damage
        {
            get
            {
                _damage = (combatTable.MH.baseDamage + ((stats.AttackPower / 14f) *
                                        combatTable.normalizationFactor) + 378f) * 0.5f;
                _damage += ((combatTable.DW ? ((combatTable.OH.baseDamage + ((stats.AttackPower / 14f) *
                                        combatTable.normalizationFactor) + 378f) * 0.5f) : 0d) * talents.ThreatOfThassarian / 3d) *
                    (0.5d * 1d + talents.NervesOfColdSteel * .0833333333d);
                double PSCritDmgMult = 1d + (.15d * talents.ViciousStrikes) + stats.BonusCritMultiplier;
                PSCritDmgMult = 1d + ((combatTable.physCrits + (.03d * talents.ViciousStrikes) + stats.BonusPlagueStrikeCrit) * PSCritDmgMult);
                _damage *= PSCritDmgMult;
                return _damage * DamageMod;
            }
        }
        public override double SecondaryDamage
        {
            get { return 0; }
        }
    }

    public class ScourgeStrike : RuneAbility
    {
        public ScourgeStrike(Character c, Stats s, CalculationOptionsDPSDK CalcOpts, CombatTable table, DeathKnightTalents Talents)
        {
            character = c; stats = s; calcOpts = CalcOpts; combatTable = table; talents = Talents; GcdType = GCDType.Melee; Runes = RunesUsed.UnholyFrost; Name = "Scourge Strike";
        }

        private double _damage;
        public override double Damage
        {
            get
            {
                _damage = (combatTable.MH.baseDamage + ((stats.AttackPower / 14d) *
                                combatTable.normalizationFactor)) * 0.7d + 560d + stats.BonusScourgeStrikeDamage; 
                double SSCritDmgMult = 1d + .15d * talents.ViciousStrikes + stats.BonusCritMultiplier;
                SSCritDmgMult = 1d + ((combatTable.physCrits + .03d * (talents.ViciousStrikes + talents.Subversion) + stats.BonusScourgeStrikeCrit) * SSCritDmgMult);
                _damage *= SSCritDmgMult;
                _damage *= DamageMod;
                return _damage;
            }
        }
        private double _secondaryDamage;
        public override double SecondaryDamage
        {
            get
            {
                _secondaryDamage = Damage * calcOpts.rotation.AvgDiseaseMult * .12d;
                double SSCritDmgMult = 1d + .15d * talents.ViciousStrikes + stats.BonusCritMultiplier;
                SSCritDmgMult = 1d + ((combatTable.physCrits + .03d * (talents.ViciousStrikes + talents.Subversion) + stats.BonusScourgeStrikeCrit) * SSCritDmgMult);
                SSCritDmgMult = 1d;
                _secondaryDamage *= SSCritDmgMult;
                _secondaryDamage *= SecondaryDamageMod;
                return _secondaryDamage;
            }
        }
    }

    public class HowlingBlast : RuneAbility
    {
        public HowlingBlast(Character c, Stats s, CalculationOptionsDPSDK CalcOpts, CombatTable table, DeathKnightTalents Talents)
        {
            character = c; stats = s; calcOpts = CalcOpts; combatTable = table; talents = Talents; GcdType = GCDType.Spell; Runes = RunesUsed.UnholyFrost; Name = "Howling Blast";
        }
        private double _damage;
        public override double Damage
        {
            get
            {
                _damage = 540 + 0.2d * (1d + talents.Impurity * .04d) * stats.AttackPower;
                double HBCritDmgMult = (0.5d * (2d + (.15d * talents.GuileOfGorefiend) + stats.BonusCritMultiplier));
                HBCritDmgMult = 1d + (Math.Min((combatTable.spellCrits), 1d) * HBCritDmgMult);
                _damage *= HBCritDmgMult;
                _damage *= DamageMod;
                return _damage;
            }
        }
        private double _secondaryDamage;
        public override double SecondaryDamage
        {
            get
            {
                _secondaryDamage = 540 + 0.2d * (1d + talents.Impurity * .04d) * stats.AttackPower;
                double HBCritDmgMult = (0.5d * (2d + (.15d * talents.GuileOfGorefiend) + stats.BonusCritMultiplier));
                HBCritDmgMult = 1d + HBCritDmgMult;
                _secondaryDamage *= HBCritDmgMult;
                _secondaryDamage *= DamageMod;
                return _secondaryDamage;
            }
        }
    }
    
    public class FrostStrike : RuneAbility
    {
        public FrostStrike(Character c, Stats s, CalculationOptionsDPSDK CalcOpts, CombatTable table, DeathKnightTalents Talents)
        {
            character = c; stats = s; calcOpts = CalcOpts; combatTable = table; talents = Talents; GcdType = GCDType.Melee; Runes = RunesUsed.RunicPower; Name = "Frost Strike";
        }
        private double _damage;
        public override double Damage
        {
            get
            {
                _damage = ((combatTable.MH.baseDamage + ((stats.AttackPower / 14d) *
                                combatTable.normalizationFactor)) +
                                250d) * 0.55d + stats.BonusFrostStrikeDamage;
                _damage += ((!combatTable.DW ? 0 : (((combatTable.OH.baseDamage + ((stats.AttackPower / 14d) *
                                combatTable.normalizationFactor)) +
                                250d) * 0.55d * 0.5d) + stats.BonusFrostStrikeDamage) 
                                        * (1d + (talents.NervesOfColdSteel * .083333333333d))) * talents.ThreatOfThassarian / 3d;
                double FSCritDmgMult = 1d + .15d * talents.GuileOfGorefiend + stats.BonusCritMultiplier;
                FSCritDmgMult = 1d + (Math.Min((combatTable.physCrits + stats.BonusFrostStrikeCrit), 1d) * FSCritDmgMult);
                _damage *= FSCritDmgMult;
                _damage *= DamageMod;
                return _damage;
            }
        }

        private double _secondaryDamage;
        public override double SecondaryDamage
        {
            get
            {
                _secondaryDamage = ((combatTable.MH.baseDamage + ((stats.AttackPower / 14d) *
                                combatTable.normalizationFactor)) +
                                250d) * 0.55f + stats.BonusFrostStrikeDamage;
                _secondaryDamage += ((!combatTable.DW ? 0 : (((combatTable.OH.baseDamage + ((stats.AttackPower / 14d) *
                                combatTable.normalizationFactor)) +
                                250d) * 0.55d * 0.5d) + stats.BonusFrostStrikeDamage)
                                        * (1d + (talents.NervesOfColdSteel * .08333333333333d))) * talents.ThreatOfThassarian / 3d;
                double FSCritDmgMult = 1d + .15d * talents.GuileOfGorefiend + stats.BonusCritMultiplier;
                FSCritDmgMult = 1d + FSCritDmgMult;
                _secondaryDamage *= FSCritDmgMult;
                _secondaryDamage *= DamageMod;
                return _secondaryDamage;
            }
        }
    }

    public class Obliterate : RuneAbility
    {
        public Obliterate(Character c, Stats s, CalculationOptionsDPSDK CalcOpts, CombatTable table, DeathKnightTalents Talents)
        {
            character = c; stats = s; calcOpts = CalcOpts; combatTable = table; talents = Talents; GcdType = GCDType.Melee; Runes = RunesUsed.UnholyFrost; Name = "Obliterate";
        }

        private double _damage;
        public override double Damage
        {
            get
            {
                _damage = ((combatTable.MH.baseDamage + ((stats.AttackPower / 14d) * combatTable.normalizationFactor) + 584d) *
                    (talents.GlyphofObliterate ? 1 : 0.8d)) + stats.BonusObliterateDamage;
                _damage += ((!combatTable.DW ? 0 : (((combatTable.OH.baseDamage + ((stats.AttackPower / 14d) * combatTable.normalizationFactor) + 584f) *
                    (talents.GlyphofObliterate ? 1 : 0.8d)) * 0.5d) + stats.BonusObliterateDamage)
                                        * (1d + (talents.NervesOfColdSteel * .08333333333d))) * talents.ThreatOfThassarian / 3d;
                _damage *= 1f + (talents.GlyphofObliterate ? 0.1d : 0.125d) * calcOpts.rotation.AvgDiseaseMult * (1f + stats.BonusPerDiseaseObliterateDamage);
                double OblitCritDmgMult = 1d + (.15d * talents.GuileOfGorefiend) + stats.BonusCritMultiplier;
                OblitCritDmgMult = 1d + (combatTable.physCrits + 
                        .03d * talents.Subversion +
                        .05d * talents.Rime +
                        stats.BonusObliterateCrit) * OblitCritDmgMult;
                _damage *= OblitCritDmgMult;
                _damage *= DamageMod;
                return _damage;
            }
        }
        public override double SecondaryDamage
        {
            get { return 0; }
        }
    }

    public class DeathStrike : RuneAbility
    {
        public DeathStrike(Character c, Stats s, CalculationOptionsDPSDK CalcOpts, CombatTable table, DeathKnightTalents Talents)
        {
            character = c; stats = s; calcOpts = CalcOpts; combatTable = table; talents = Talents; GcdType = GCDType.Melee; Runes = RunesUsed.UnholyFrost; Name = "Death Strike";
        }
        private double _damage;
        public override double Damage
        {
            get
            {
                _damage = ((combatTable.MH.baseDamage + ((stats.AttackPower / 14d) * combatTable.normalizationFactor) + 297) * 0.75d) + stats.BonusDeathStrikeDamage;
                _damage += ((!combatTable.DW ? 0 : (((combatTable.OH.baseDamage + ((stats.AttackPower / 14d) * combatTable.normalizationFactor) + 297) * 0.75d) * 0.5d) + stats.BonusDeathStrikeDamage)
                                        * (1d + (talents.NervesOfColdSteel * .0833333333333d))) * talents.ThreatOfThassarian / 3d;
                double DSCritDmgMult = 1d + (.15d * talents.MightOfMograine) + stats.BonusCritMultiplier;
                DSCritDmgMult = 1d + (combatTable.physCrits +
                     .03d * talents.ImprovedDeathStrike +
                     stats.BonusDeathStrikeCrit) * DSCritDmgMult;
                _damage *= DSCritDmgMult;
                _damage *= DamageMod;
                return _damage;
            }
        }
        public override double SecondaryDamage
        {
            get { return 0; }
        }
    }

    public class HeartStrike : RuneAbility
    {
        public HeartStrike(Character c, Stats s, CalculationOptionsDPSDK CalcOpts, CombatTable table, DeathKnightTalents Talents)
        {
            character = c; stats = s; calcOpts = CalcOpts; combatTable = table; talents = Talents; GcdType = GCDType.Melee; Runes = RunesUsed.Blood; Name = "Heart Strike";
        }

        private double _damage;
        public override double Damage
        {
            get
            {
                _damage = ((combatTable.MH.baseDamage + ((stats.AttackPower / 14f) * combatTable.normalizationFactor) + 736f) *
                            0.5f) + stats.BonusHeartStrikeDamage;
                _damage *= 1d + 0.1d * calcOpts.rotation.AvgDiseaseMult * (1d + stats.BonusPerDiseaseHeartStrikeDamage);
                double HSCritDmgMult = 1d + (.15d * talents.MightOfMograine) + stats.BonusCritMultiplier;
                HSCritDmgMult = 1d + ((combatTable.physCrits + (.03d * talents.Subversion)) * HSCritDmgMult);
                _damage *= HSCritDmgMult;
                _damage *= DamageMod;
                return _damage;
            }
        }
        public override double SecondaryDamage
        {
            get { return 0; }
        }
    }

    public class BloodBoil : RuneAbility
    {
        public BloodBoil(Character c, Stats s, CalculationOptionsDPSDK CalcOpts, CombatTable table, DeathKnightTalents Talents)
        {
            character = c; stats = s; calcOpts = CalcOpts; combatTable = table; talents = Talents; GcdType = GCDType.Spell; Runes = RunesUsed.Blood; Name = "Blood Boil";
        }

        private double _damage;
        public override double Damage
        {
            get
            {
                if (calcOpts.rotation.AvgDiseaseMult > 0)
                {
                    _damage = 295d + 0.095d * (1d + talents.Impurity * .04d) * stats.AttackPower;
                }
                else
                {
                    _damage = 200d + 0.06d * (1d + talents.Impurity * .04d) * stats.AttackPower;
                }
                double BloodBoilCritMult = .5d * (2d + stats.BonusCritMultiplier);
                BloodBoilCritMult = 1d + combatTable.spellCrits * BloodBoilCritMult;
                _damage *= BloodBoilCritMult;
                _damage *= DamageMod;
                return _damage;
            }
        }
        public override double SecondaryDamage
        {
            get { return 0; }
        }
    }

    public class Disease : RuneAbility
    {
        public Disease(Character c, Stats s, CalculationOptionsDPSDK Calcopts, CombatTable table, DeathKnightTalents Talents)
        {
            character = c; stats = s; calcOpts = Calcopts; combatTable = table; talents = Talents; GcdType = GCDType.None; Runes = RunesUsed.None; Name = "A Disease";
        }

        private int _duration = 0;
        public int Duration
        {
            get { return (_duration == 0 ? talents.Epidemic * 3 + 15 : _duration); }
            set { _duration = value; }
        }

        private double _damage;
        public override double Damage
        {
            get
            {
                _damage = stats.AttackPower * .055 * 1.15 * (1d + talents.Impurity * .04d);
                _damage *= DamageMod;
                return _damage;
            }
        }
        public override double SecondaryDamage
        {
            get { return 0; }
        }
    }
}