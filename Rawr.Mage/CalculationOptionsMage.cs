using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.ComponentModel;

namespace Rawr.Mage
{
    public class SpellWeight
    {
        public SpellId Spell { get; set; }
        public float Weight { get; set; }
    }

    public class CooldownRestriction
    {
        public double TimeStart { get; set; }
        public double TimeEnd { get; set; }
        public StateDescription.StateDescriptionDelegate IsMatch { get; set; }
    }

    public struct CooldownStackingCacheEntry
    {
        public double Effect1Duration;
        public double Effect1Cooldown;
        public double Effect2Duration;
        public double Effect2Cooldown;
        public double MaximumStackingDuration;
    }

    [GenerateSerializer]
    public sealed class CalculationOptionsMage : ICalculationOptionBase, INotifyPropertyChanged
    {
        private int playerLevel;
        private float levelScalingFactor;

        public int PlayerLevel
        {
            get
            {
                return playerLevel;
            }
            set
            {
                playerLevel = value;
                if (playerLevel <= 80)
                {
                    levelScalingFactor = (float)((52f / 82f) * Math.Pow(63f / 131f, (playerLevel - 70) / 10f));
                }
                else
                {
                    levelScalingFactor = (float)((1638f / 5371f) * Math.Pow(7f / 41f, (playerLevel - 80) / 5f));
                }
                OnPropertyChanged("PlayerLevel");
            }
        }

        public float LevelScalingFactor
        {
            get
            {
                return levelScalingFactor;
            }
        }

        public const float SetBonus4T8ProcRate = 0.25f;

        /*private bool _Mode333;
        public bool Mode333
        {
            get { return _Mode333; }
            set { _Mode333 = value; OnPropertyChanged("Mode333"); }
        }*/

        private bool _UseFireWard;
        public bool UseFireWard
        {
            get { return _UseFireWard; }
            set { _UseFireWard = value; OnPropertyChanged("UseFireWard"); }
        }

        private bool _UseFrostWard;
        public bool UseFrostWard
        {
            get { return _UseFrostWard; }
            set { _UseFrostWard = value; OnPropertyChanged("UseFrostWard"); }
        }

        private float _FrostbiteUtilization;
        public float FrostbiteUtilization
        {
            get { return _FrostbiteUtilization; }
            set { _FrostbiteUtilization = value; OnPropertyChanged("FrostbiteUtilization"); }
        }

        private bool _MaxUseAssumption;
        public bool MaxUseAssumption
        {
            get { return _MaxUseAssumption; }
            set 
            { 
                _MaxUseAssumption = value;
                UpdateCooldownStackingCache();
                OnPropertyChanged("MaxUseAssumption"); 
            }
        }

        private bool _Beta;
        public bool Beta
        {
            get { return _Beta; }
            set { _Beta = value; OnPropertyChanged("Beta"); }
        }

        private int _TargetLevel;
        public int TargetLevel
        {
            get { return _TargetLevel; }
            set { _TargetLevel = value; OnPropertyChanged("TargetLevel"); }
        }

        private int _AoeTargetLevel;
        public int AoeTargetLevel
        {
            get { return _AoeTargetLevel; }
            set { _AoeTargetLevel = value; OnPropertyChanged("AoeTargetLevel"); }
        }

        private float _LatencyCast;
        public float LatencyCast
        {
            get { return _LatencyCast; }
            set { _LatencyCast = value; OnPropertyChanged("LatencyCast"); }
        }

        private float _LatencyGCD;
        public float LatencyGCD
        {
            get { return _LatencyGCD; }
            set { _LatencyGCD = value; OnPropertyChanged("LatencyGCD"); }
        }

        private float _LatencyChannel;
        public float LatencyChannel
        {
            get { return _LatencyChannel; }
            set { _LatencyChannel = value; OnPropertyChanged("LatencyChannel"); }
        }

        private float _MovementFrequency;
        public float MovementFrequency
        {
            get { return _MovementFrequency; }
            set { _MovementFrequency = value; OnPropertyChanged("MovementFrequency"); }
        }

        private float _MovementDuration;
        public float MovementDuration
        {
            get { return _MovementDuration; }
            set { _MovementDuration = value; OnPropertyChanged("MovementDuration"); }
        }

        private int _AoeTargets;
        public int AoeTargets
        {
            get { return _AoeTargets; }
            set { _AoeTargets = value; OnPropertyChanged("AoeTargets"); }
        }

        private float _ArcaneResist;
        public float ArcaneResist
        {
            get { return _ArcaneResist; }
            set { _ArcaneResist = value; OnPropertyChanged("ArcaneResist"); }
        }

        private float _FireResist;
        public float FireResist
        {
            get { return _FireResist; }
            set { _FireResist = value; OnPropertyChanged("FireResist"); }
        }

        private float _FrostResist;
        public float FrostResist
        {
            get { return _FrostResist; }
            set { _FrostResist = value; OnPropertyChanged("FrostResist"); }
        }

        private float _NatureResist;
        public float NatureResist
        {
            get { return _NatureResist; }
            set { _NatureResist = value; OnPropertyChanged("NatureResist"); }
        }

        private float _ShadowResist;
        public float ShadowResist
        {
            get { return _ShadowResist; }
            set { _ShadowResist = value; OnPropertyChanged("ShadowResist"); }
        }

        private float _HolyResist;
        public float HolyResist
        {
            get { return _HolyResist; }
            set { _HolyResist = value; OnPropertyChanged("HolyResist"); }
        }

        private float _FightDuration;
        public float FightDuration
        {
            get { return _FightDuration; }
            set 
            {
                _FightDuration = value;
                UpdateCooldownStackingCache();
                OnPropertyChanged("FightDuration");
            }
        }

        private static Dictionary<float, List<CooldownStackingCacheEntry>> cooldownStackingCacheMapNoMaxUseAssumption = new Dictionary<float, List<CooldownStackingCacheEntry>>();
        private static Dictionary<float, List<CooldownStackingCacheEntry>> cooldownStackingCacheMap = new Dictionary<float, List<CooldownStackingCacheEntry>>();

        [XmlIgnore]
        public List<CooldownStackingCacheEntry> CooldownStackingCache { get; private set; }

        private void UpdateCooldownStackingCache()
        {
            var map = MaxUseAssumption ? cooldownStackingCacheMap : cooldownStackingCacheMapNoMaxUseAssumption;
            lock(map)
            {
                List<CooldownStackingCacheEntry> cache;
                map.TryGetValue(_FightDuration, out cache);
                if (cache == null)
                {
                    cache = new List<CooldownStackingCacheEntry>();
                    map[_FightDuration] = cache;
                }
                CooldownStackingCache = cache;
            }
        }

        private float _TpsLimit;
        public float TpsLimit
        {
            get { return _TpsLimit; }
            set { _TpsLimit = value; OnPropertyChanged("TpsLimit"); }
        }

        private bool _HeroismAvailable;
        public bool HeroismAvailable
        {
            get { return _HeroismAvailable; }
            set { _HeroismAvailable = value; OnPropertyChanged("HeroismAvailable"); }
        }

        private bool _PowerInfusionAvailable;
        public bool PowerInfusionAvailable
        {
            get { return _PowerInfusionAvailable; }
            set { _PowerInfusionAvailable = value; OnPropertyChanged("PowerInfusionAvailable"); }
        }

        private bool _PotionOfWildMagic;
        public bool PotionOfWildMagic
        {
            get { return _PotionOfWildMagic; }
            set { _PotionOfWildMagic = value; OnPropertyChanged("PotionOfWildMagic"); }
        }

        private bool _PotionOfSpeed;
        public bool PotionOfSpeed
        {
            get { return _PotionOfSpeed; }
            set { _PotionOfSpeed = value; OnPropertyChanged("PotionOfSpeed"); }
        }

        private bool _FlameCap;
        public bool FlameCap
        {
            get { return _FlameCap; }
            set { _FlameCap = value; OnPropertyChanged("FlameCap"); }
        }

        private float _MoltenFuryPercentage;
        public float MoltenFuryPercentage
        {
            get { return _MoltenFuryPercentage; }
            set { _MoltenFuryPercentage = value; OnPropertyChanged("MoltenFuryPercentage"); }
        }

        private bool _MaintainScorch;
        public bool MaintainScorch
        {
            get { return _MaintainScorch; }
            set { _MaintainScorch = value; OnPropertyChanged("MaintainScorch"); }
        }

        private bool _MaintainSnare;
        public bool MaintainSnare
        {
            get { return _MaintainSnare; }
            set { _MaintainSnare = value; OnPropertyChanged("MaintainSnare"); }
        }

        private float _InterruptFrequency;
        public float InterruptFrequency
        {
            get { return _InterruptFrequency; }
            set { _InterruptFrequency = value; OnPropertyChanged("InterruptFrequency"); }
        }

        private float _AoeDuration;
        public float AoeDuration
        {
            get { return _AoeDuration; }
            set { _AoeDuration = value; OnPropertyChanged("AoeDuration"); }
        }

        private bool _SmartOptimization;
        public bool SmartOptimization
        {
            get { return _SmartOptimization; }
            set { _SmartOptimization = value; OnPropertyChanged("SmartOptimization"); }
        }

        private float _DpsTime;
        public float DpsTime
        {
            get { return _DpsTime; }
            set { _DpsTime = value; OnPropertyChanged("DpsTime"); }
        }

        private bool _DrumsOfBattle;
        public bool DrumsOfBattle
        {
            get { return _DrumsOfBattle; }
            set { _DrumsOfBattle = value; OnPropertyChanged("DrumsOfBattle"); }
        }

        private bool _AutomaticArmor;
        public bool AutomaticArmor
        {
            get { return _AutomaticArmor; }
            set { _AutomaticArmor = value; OnPropertyChanged("AutomaticArmor"); }
        }

        private bool _ForceIncrementalOptimizations;
        public bool ForceIncrementalOptimizations
        {
            get { return _ForceIncrementalOptimizations; }
            set { _ForceIncrementalOptimizations = value; OnPropertyChanged("ForceIncrementalOptimizations"); }
        }

        private bool _IncrementalOptimizations;
        public bool IncrementalOptimizations
        {
            get { return _IncrementalOptimizations; }
            set { _IncrementalOptimizations = value; OnPropertyChanged("IncrementalOptimizations"); }
        }

        private float _SnaredTime;
        public float SnaredTime
        {
            get { return _SnaredTime; }
            set { _SnaredTime = value; OnPropertyChanged("SnaredTime"); }
        }

        private float _WarlockSpellPower;
        public float WarlockSpellPower
        {
            get { return _WarlockSpellPower; }
            set { _WarlockSpellPower = value; OnPropertyChanged("WarlockSpellPower"); }
        }

        [XmlIgnore]
        public int[] IncrementalSetStateIndexes;
        [XmlIgnore]
        public int[] IncrementalSetSortedStates;
        [XmlIgnore]
        public int[] IncrementalSetSegments;
        [XmlIgnore]
        public CycleId[] IncrementalSetSpells;
        [XmlIgnore]
        public string IncrementalSetArmor;
        [XmlIgnore]
        public VariableType[] IncrementalSetVariableType;
        [XmlIgnore]
        public int[] IncrementalSetManaSegment;
        [XmlIgnore]
        public Rawr.Mage.SequenceReconstruction.Sequence SequenceReconstruction;
        [XmlIgnore]
        public bool AdviseAdvancedSolver;
        [XmlIgnore]
        public DisplayCalculations Calculations; // calculations that are result of the last display in rawr

        [XmlIgnore]
        private Character _character;
        [XmlIgnore]
        public Character Character
        {
            get
            {
                return _character;
            }
            set
            {
                value.CalculationsInvalidated += new EventHandler(Character_ItemsChanged);
                _character = value;
            }
        }

        private void Character_ItemsChanged(object sender, EventArgs e)
        {
            IncrementalSetStateIndexes = null;
            IncrementalSetSegments = null;
            IncrementalSetSortedStates = null;
            IncrementalSetSpells = null;
            IncrementalSetManaSegment = null;
            IncrementalSetVariableType = null;
            CooldownRestrictionList = null;
        }

        private bool _ReconstructSequence;
        public bool ReconstructSequence
        {
            get { return _ReconstructSequence; }
            set { _ReconstructSequence = value; OnPropertyChanged("ReconstructSequence"); }
        }

        private bool _ComparisonSegmentCooldowns;
        public bool ComparisonSegmentCooldowns
        {
            get { return _ComparisonSegmentCooldowns; }
            set { _ComparisonSegmentCooldowns = value; OnPropertyChanged("ComparisonSegmentCooldowns"); }
        }

        private bool _DisplaySegmentCooldowns;
        public bool DisplaySegmentCooldowns
        {
            get { return _DisplaySegmentCooldowns; }
            set { _DisplaySegmentCooldowns = value; OnPropertyChanged("DisplaySegmentCooldowns"); }
        }

        private bool _ComparisonIntegralMana;
        public bool ComparisonIntegralMana
        {
            get { return _ComparisonIntegralMana; }
            set { _ComparisonIntegralMana = value; OnPropertyChanged("ComparisonIntegralMana"); }
        }

        private bool _DisplayIntegralMana;
        public bool DisplayIntegralMana
        {
            get { return _DisplayIntegralMana; }
            set { _DisplayIntegralMana = value; OnPropertyChanged("DisplayIntegralMana"); }
        }

        private bool _ComparisonSegmentMana;
        public bool ComparisonSegmentMana
        {
            get { return _ComparisonSegmentMana; }
            set { _ComparisonSegmentMana = value; OnPropertyChanged("ComparisonSegmentMana"); }
        }

        private bool _DisplaySegmentMana;
        public bool DisplaySegmentMana
        {
            get { return _DisplaySegmentMana; }
            set { _DisplaySegmentMana = value; OnPropertyChanged("DisplaySegmentMana"); }
        }

        private int _ComparisonAdvancedConstraintsLevel;
        public int ComparisonAdvancedConstraintsLevel
        {
            get { return _ComparisonAdvancedConstraintsLevel; }
            set { _ComparisonAdvancedConstraintsLevel = value; OnPropertyChanged("ComparisonAdvancedConstraintsLevel"); }
        }

        private int _DisplayAdvancedConstraintsLevel;
        public int DisplayAdvancedConstraintsLevel
        {
            get { return _DisplayAdvancedConstraintsLevel; }
            set { _DisplayAdvancedConstraintsLevel = value; OnPropertyChanged("DisplayAdvancedConstraintsLevel"); }
        }

        private int _FixedSegmentDuration;
        public int FixedSegmentDuration
        {
            get { return _FixedSegmentDuration; }
            set { _FixedSegmentDuration = value; OnPropertyChanged("FixedSegmentDuration"); }
        }

        private bool _VariableSegmentDuration;
        public bool VariableSegmentDuration
        {
            get { return _VariableSegmentDuration; }
            set { _VariableSegmentDuration = value; OnPropertyChanged("VariableSegmentDuration"); }
        }

        private string _AdditionalSegmentSplits;
        public string AdditionalSegmentSplits
        {
            get { return _AdditionalSegmentSplits; }
            set { _AdditionalSegmentSplits = value; OnPropertyChanged("AdditionalSegmentSplits"); }
        }

        private double _LowerBoundHint;
        public double LowerBoundHint
        {
            get { return _LowerBoundHint; }
            set { _LowerBoundHint = value; OnPropertyChanged("LowerBoundHint"); }
        }

        private string _CooldownRestrictions;
        public string CooldownRestrictions
        {
            get { return _CooldownRestrictions; }
            set { _CooldownRestrictions = value; OnPropertyChanged("CooldownRestrictions"); }
        }

        private bool _EnableHastedEvocation;
        public bool EnableHastedEvocation
        {
            get { return _EnableHastedEvocation; }
            set { _EnableHastedEvocation = value; OnPropertyChanged("EnableHastedEvocation"); }
        }

        private bool _Enable2T10Evocation;
        public bool Enable2T10Evocation
        {
            get { return _Enable2T10Evocation; }
            set { _Enable2T10Evocation = value; OnPropertyChanged("Enable2T10Evocation"); }
        }

        private bool _AdvancedHasteProcs;
        public bool AdvancedHasteProcs
        {
            get { return _AdvancedHasteProcs; }
            set { _AdvancedHasteProcs = value; OnPropertyChanged("AdvancedHasteProcs"); }
        }

        private bool _EncounterEnabled;
        public bool EncounterEnabled
        {
            get { return _EncounterEnabled; }
            set { _EncounterEnabled = value; OnPropertyChanged("EncounterEnabled"); }
        }

        public Encounter Encounter { get; set; }

        [XmlIgnore]
        public List<CooldownRestriction> CooldownRestrictionList;
        public bool CooldownRestrictionsValid(Segment segment, CastingState state)
        {
            if (CooldownRestrictionList == null) return true;
            foreach (CooldownRestriction restriction in CooldownRestrictionList)
            {
                if (segment.TimeStart >= restriction.TimeStart && segment.TimeEnd <= restriction.TimeEnd)
                {
                    if (!restriction.IsMatch(state.Effects)) return false;
                }
            }
            return true;
        }

        public float GetDamageMultiplier(Segment segment)
        {
            float mult = 1.0f;
            if (Encounter != null && EncounterEnabled)
            {
                // very naive for now, assume that exactly one target can apply
                foreach (DamageMultiplier multiplier in Encounter.GlobalMultipliers)
                {
                    float startTime, endTime;
                    if (multiplier.RelativeTime)
                    {
                        startTime = multiplier.StartTime * FightDuration;
                        endTime = multiplier.EndTime * FightDuration;
                    }
                    else
                    {
                        startTime = multiplier.StartTime;
                        endTime = multiplier.EndTime;
                    }
                    if (segment.TimeStart >= startTime && segment.TimeEnd <= endTime)
                    {
                        mult *= multiplier.Multiplier;
                    }
                }
                bool foundTarget = false;
                foreach (TargetGroup group in Encounter.TargetGroups)
                {
                    float startTime, endTime;
                    if (group.RelativeTime)
                    {
                        startTime = group.EntranceTime * FightDuration;
                        endTime = group.ExitTime * FightDuration;
                    }
                    else
                    {
                        startTime = group.EntranceTime;
                        endTime = group.ExitTime;
                    }
                    if (segment.TimeStart >= startTime && segment.TimeEnd <= endTime)
                    {
                        foundTarget = true;
                        foreach (DamageMultiplier multiplier in group.Multipliers)
                        {
                            float t1, t2;
                            if (multiplier.RelativeTime)
                            {
                                t1 = startTime + multiplier.StartTime * (endTime - startTime);
                                t2 = startTime + multiplier.EndTime * (endTime - startTime);
                            }
                            else
                            {
                                t1 = startTime + multiplier.StartTime;
                                t2 = startTime + multiplier.EndTime;
                            }
                            if (segment.TimeStart >= t1 && segment.TimeEnd <= t2)
                            {
                                mult *= multiplier.Multiplier;
                            }
                        }
                        break;
                    }
                }
                if (!foundTarget)
                {
                    mult = 0.0f;
                }
            }
            return mult;
        }

        private MIPMethod _MIPMethod;
        public MIPMethod MIPMethod
        {
            get { return _MIPMethod; }
            set { _MIPMethod = value; OnPropertyChanged("MIPMethod"); }
        }


        private float _Innervate;
        public float Innervate
        {
            get { return _Innervate; }
            set { _Innervate = value; OnPropertyChanged("Innervate"); }
        }

        private float _ManaTide;
        public float ManaTide
        {
            get { return _ManaTide; }
            set { _ManaTide = value; OnPropertyChanged("ManaTide"); }
        }

        private float _Fragmentation;
        public float Fragmentation
        {
            get { return _Fragmentation; }
            set { _Fragmentation = value; OnPropertyChanged("Fragmentation"); }
        }

        private float _SurvivabilityRating;
        public float SurvivabilityRating
        {
            get { return _SurvivabilityRating; }
            set { _SurvivabilityRating = value; OnPropertyChanged("SurvivabilityRating"); }
        }

        private bool _Aldor;
        public bool Aldor
        {
            get { return _Aldor; }
            set { _Aldor = value; OnPropertyChanged("Aldor"); }
        }

        private bool _PVP;
        public bool PVP
        {
            get { return _PVP; }
            set { _PVP = value; OnPropertyChanged("PVP"); }
        }

        private int _HeroismControl;
        // 0 = optimal, 1 = before 20%, 2 = no cooldowns, 3 = after 20%
        public int HeroismControl
        {
            get { return _HeroismControl; }
            set { _HeroismControl = value; OnPropertyChanged("HeroismControl"); }
        }

        private bool _AverageCooldowns;
        public bool AverageCooldowns
        {
            get { return _AverageCooldowns; }
            set { _AverageCooldowns = value; OnPropertyChanged("AverageCooldowns"); }
        }

        private bool _EvocationEnabled;
        public bool EvocationEnabled
        {
            get { return _EvocationEnabled; }
            set { _EvocationEnabled = value; OnPropertyChanged("EvocationEnabled"); }
        }

        private bool _ManaPotionEnabled;
        public bool ManaPotionEnabled
        {
            get { return _ManaPotionEnabled; }
            set { _ManaPotionEnabled = value; OnPropertyChanged("ManaPotionEnabled"); }
        }

        private bool _ManaGemEnabled;
        public bool ManaGemEnabled
        {
            get { return _ManaGemEnabled; }
            set { _ManaGemEnabled = value; OnPropertyChanged("ManaGemEnabled"); }
        }

        private bool _MirrorImageEnabled;
        public bool MirrorImageEnabled
        {
            get { return _MirrorImageEnabled; }
            set { _MirrorImageEnabled = value; OnPropertyChanged("MirrorImageEnabled"); }
        }

        private bool _DisableCooldowns;
        public bool DisableCooldowns
        {
            get { return _DisableCooldowns; }
            set { _DisableCooldowns = value; OnPropertyChanged("DisableCooldowns"); }
        }

        private int _MaxHeapLimit;
        public int MaxHeapLimit
        {
            get { return _MaxHeapLimit; }
            set { _MaxHeapLimit = value; OnPropertyChanged("MaxHeapLimit"); }
        }

        private float _DrinkingTime;
        public float DrinkingTime
        {
            get { return _DrinkingTime; }
            set { _DrinkingTime = value; OnPropertyChanged("DrinkingTime"); }
        }

        private float _TargetDamage;
        public float TargetDamage
        {
            get { return _TargetDamage; }
            set { _TargetDamage = value; OnPropertyChanged("TargetDamage"); }
        }

        private bool _FarmingMode;
        public bool FarmingMode
        {
            get { return _FarmingMode; }
            set { _FarmingMode = value; OnPropertyChanged("FarmingMode"); }
        }

        private float _FocusMagicTargetCritRate;
        public float FocusMagicTargetCritRate
        {
            get { return _FocusMagicTargetCritRate; }
            set { _FocusMagicTargetCritRate = value; OnPropertyChanged("FocusMagicTargetCritRate"); }
        }

        private bool _UnlimitedMana;
        public bool UnlimitedMana
        {
            get { return _UnlimitedMana; }
            set { _UnlimitedMana = value; OnPropertyChanged("UnlimitedMana"); }
        }

        private float _AbsorptionPerSecond;
        public float AbsorptionPerSecond
        {
            get { return _AbsorptionPerSecond; }
            set { _AbsorptionPerSecond = value; OnPropertyChanged("AbsorptionPerSecond"); }
        }


        public List<SpellWeight> CustomSpellMix { get; set; }
        private bool _CustomSpellMixEnabled;
        public bool CustomSpellMixEnabled
        {
            get { return _CustomSpellMixEnabled; }
            set { _CustomSpellMixEnabled = value; OnPropertyChanged("CustomSpellMixEnabled"); }
        }

        private int _GlobalRestarts;
        public int GlobalRestarts
        {
            get { return _GlobalRestarts; }
            set { _GlobalRestarts = value; OnPropertyChanged("GlobalRestarts"); }
        }

        private bool _CustomSpellMixOnly;
        public bool CustomSpellMixOnly
        {
            get { return _CustomSpellMixOnly; }
            set { _CustomSpellMixOnly = value; OnPropertyChanged("CustomSpellMixOnly"); }
        }


        private float _MeleeDps;
        public float MeleeDps
        {
            get { return _MeleeDps; }
            set { _MeleeDps = value; OnPropertyChanged("MeleeDps"); }
        }

        private float _MeleeCrit;
        public float MeleeCrit
        {
            get { return _MeleeCrit; }
            set { _MeleeCrit = value; OnPropertyChanged("MeleeCrit"); }
        }

        private float _MeleeDot;
        public float MeleeDot
        {
            get { return _MeleeDot; }
            set { _MeleeDot = value; OnPropertyChanged("MeleeDot"); }
        }

        private float _PhysicalDps;
        public float PhysicalDps
        {
            get { return _PhysicalDps; }
            set { _PhysicalDps = value; OnPropertyChanged("PhysicalDps"); }
        }

        private float _PhysicalCrit;
        public float PhysicalCrit
        {
            get { return _PhysicalCrit; }
            set { _PhysicalCrit = value; OnPropertyChanged("PhysicalCrit"); }
        }

        private float _PhysicalDot;
        public float PhysicalDot
        {
            get { return _PhysicalDot; }
            set { _PhysicalDot = value; OnPropertyChanged("PhysicalDot"); }
        }

        private float _FireDps;
        public float FireDps
        {
            get { return _FireDps; }
            set { _FireDps = value; OnPropertyChanged("FireDps"); }
        }

        private float _FireCrit;
        public float FireCrit
        {
            get { return _FireCrit; }
            set { _FireCrit = value; OnPropertyChanged("FireCrit"); }
        }

        private float _FireDot;
        public float FireDot
        {
            get { return _FireDot; }
            set { _FireDot = value; OnPropertyChanged("FireDot"); }
        }

        private float _FrostDps;
        public float FrostDps
        {
            get { return _FrostDps; }
            set { _FrostDps = value; OnPropertyChanged("FrostDps"); }
        }

        private float _FrostCrit;
        public float FrostCrit
        {
            get { return _FrostCrit; }
            set { _FrostCrit = value; OnPropertyChanged("FrostCrit"); }
        }

        private float _FrostDot;
        public float FrostDot
        {
            get { return _FrostDot; }
            set { _FrostDot = value; OnPropertyChanged("FrostDot"); }
        }

        private float _ArcaneDps;
        public float ArcaneDps
        {
            get { return _ArcaneDps; }
            set { _ArcaneDps = value; OnPropertyChanged("ArcaneDps"); }
        }

        private float _ArcaneCrit;
        public float ArcaneCrit
        {
            get { return _ArcaneCrit; }
            set { _ArcaneCrit = value; OnPropertyChanged("ArcaneCrit"); }
        }

        private float _ArcaneDot;
        public float ArcaneDot
        {
            get { return _ArcaneDot; }
            set { _ArcaneDot = value; OnPropertyChanged("ArcaneDot"); }
        }

        private float _ShadowDps;
        public float ShadowDps
        {
            get { return _ShadowDps; }
            set { _ShadowDps = value; OnPropertyChanged("ShadowDps"); }
        }

        private float _ShadowCrit;
        public float ShadowCrit
        {
            get { return _ShadowCrit; }
            set { _ShadowCrit = value; OnPropertyChanged("ShadowCrit"); }
        }

        private float _ShadowDot;
        public float ShadowDot
        {
            get { return _ShadowDot; }
            set { _ShadowDot = value; OnPropertyChanged("ShadowDot"); }
        }

        private float _NatureDps;
        public float NatureDps
        {
            get { return _NatureDps; }
            set { _NatureDps = value; OnPropertyChanged("NatureDps"); }
        }

        private float _NatureCrit;
        public float NatureCrit
        {
            get { return _NatureCrit; }
            set { _NatureCrit = value; OnPropertyChanged("NatureCrit"); }
        }

        private float _NatureDot;
        public float NatureDot
        {
            get { return _NatureDot; }
            set { _NatureDot = value; OnPropertyChanged("NatureDot"); }
        }

        private float _HolyDps;
        public float HolyDps
        {
            get { return _HolyDps; }
            set { _HolyDps = value; OnPropertyChanged("HolyDps"); }
        }

        private float _HolyCrit;
        public float HolyCrit
        {
            get { return _HolyCrit; }
            set { _HolyCrit = value; OnPropertyChanged("HolyCrit"); }
        }

        private float _HolyDot;
        public float HolyDot
        {
            get { return _HolyDot; }
            set { _HolyDot = value; OnPropertyChanged("HolyDot"); }
        }


        private float _BurstWindow;
        public float BurstWindow
        {
            get { return _BurstWindow; }
            set { _BurstWindow = value; OnPropertyChanged("BurstWindow"); }
        }

        private float _BurstImpacts;
        public float BurstImpacts
        {
            get { return _BurstImpacts; }
            set { _BurstImpacts = value; OnPropertyChanged("BurstImpacts"); }
        }

        //public float ChanceToLiveLimit { get; set; }
        private float _ChanceToLiveScore;
        public float ChanceToLiveScore
        {
            get { return _ChanceToLiveScore; }
            set { _ChanceToLiveScore = value; OnPropertyChanged("ChanceToLiveScore"); }
        }

        private float _ChanceToLiveAttenuation;
        public float ChanceToLiveAttenuation
        {
            get { return _ChanceToLiveAttenuation; }
            set { _ChanceToLiveAttenuation = value; OnPropertyChanged("ChanceToLiveAttenuation"); }
        }

        private float _EffectSpiritMultiplier;
        public float EffectSpiritMultiplier
        {
            get { return _EffectSpiritMultiplier; }
            set { _EffectSpiritMultiplier = value; OnPropertyChanged("EffectSpiritMultiplier"); }
        }

        private float _EffectCritBonus;
        public float EffectCritBonus
        {
            get { return _EffectCritBonus; }
            set { _EffectCritBonus = value; OnPropertyChanged("EffectCritBonus"); }
        }

        private float _EffectCritDamageBonus;
        public float EffectCritDamageBonus
        {
            get { return _EffectCritDamageBonus; }
            set { _EffectCritDamageBonus = value; OnPropertyChanged("EffectCritDamageBonus"); }
        }

        private float _EffectDamageMultiplier;
        public float EffectDamageMultiplier
        {
            get { return _EffectDamageMultiplier; }
            set { _EffectDamageMultiplier = value; OnPropertyChanged("EffectDamageMultiplier"); }
        }

        private float _EffectHasteMultiplier;
        public float EffectHasteMultiplier
        {
            get { return _EffectHasteMultiplier; }
            set { _EffectHasteMultiplier = value; OnPropertyChanged("EffectHasteMultiplier"); }
        }

        private float _EffectCostMultiplier;
        public float EffectCostMultiplier
        {
            get { return _EffectCostMultiplier; }
            set { _EffectCostMultiplier = value; OnPropertyChanged("EffectCostMultiplier"); }
        }

        private float _EffectRegenMultiplier;
        public float EffectRegenMultiplier
        {
            get { return _EffectRegenMultiplier; }
            set { _EffectRegenMultiplier = value; OnPropertyChanged("EffectRegenMultiplier"); }
        }

        private bool _EffectDisableManaSources;
        public bool EffectDisableManaSources
        {
            get { return _EffectDisableManaSources; }
            set { _EffectDisableManaSources = value; OnPropertyChanged("EffectDisableManaSources"); }
        }

        private float _EffectShadowSilenceFrequency;
        public float EffectShadowSilenceFrequency
        {
            get { return _EffectShadowSilenceFrequency; }
            set { _EffectShadowSilenceFrequency = value; OnPropertyChanged("EffectShadowSilenceFrequency"); }
        }

        private float _EffectShadowSilenceDuration;
        public float EffectShadowSilenceDuration
        {
            get { return _EffectShadowSilenceDuration; }
            set { _EffectShadowSilenceDuration = value; OnPropertyChanged("EffectShadowSilenceDuration"); }
        }

        private float _EffectShadowManaDrainFrequency;
        public float EffectShadowManaDrainFrequency
        {
            get { return _EffectShadowManaDrainFrequency; }
            set { _EffectShadowManaDrainFrequency = value; OnPropertyChanged("EffectShadowManaDrainFrequency"); }
        }

        private float _EffectShadowManaDrain;
        public float EffectShadowManaDrain
        {
            get { return _EffectShadowManaDrain; }
            set { _EffectShadowManaDrain = value; OnPropertyChanged("EffectShadowManaDrain"); }
        }

        private float _EffectArcaneOtherBinary;
        public float EffectArcaneOtherBinary
        {
            get { return _EffectArcaneOtherBinary; }
            set { _EffectArcaneOtherBinary = value; OnPropertyChanged("EffectArcaneOtherBinary"); }
        }

        private float _EffectFireOtherBinary;
        public float EffectFireOtherBinary
        {
            get { return _EffectFireOtherBinary; }
            set { _EffectFireOtherBinary = value; OnPropertyChanged("EffectFireOtherBinary"); }
        }

        private float _EffectFrostOtherBinary;
        public float EffectFrostOtherBinary
        {
            get { return _EffectFrostOtherBinary; }
            set { _EffectFrostOtherBinary = value; OnPropertyChanged("EffectFrostOtherBinary"); }
        }

        private float _EffectShadowOtherBinary;
        public float EffectShadowOtherBinary
        {
            get { return _EffectShadowOtherBinary; }
            set { _EffectShadowOtherBinary = value; OnPropertyChanged("EffectShadowOtherBinary"); }
        }

        private float _EffectNatureOtherBinary;
        public float EffectNatureOtherBinary
        {
            get { return _EffectNatureOtherBinary; }
            set { _EffectNatureOtherBinary = value; OnPropertyChanged("EffectNatureOtherBinary"); }
        }

        private float _EffectHolyOtherBinary;
        public float EffectHolyOtherBinary
        {
            get { return _EffectHolyOtherBinary; }
            set { _EffectHolyOtherBinary = value; OnPropertyChanged("EffectHolyOtherBinary"); }
        }

        private float _EffectArcaneOther;
        public float EffectArcaneOther
        {
            get { return _EffectArcaneOther; }
            set { _EffectArcaneOther = value; OnPropertyChanged("EffectArcaneOther"); }
        }

        private float _EffectFireOther;
        public float EffectFireOther
        {
            get { return _EffectFireOther; }
            set { _EffectFireOther = value; OnPropertyChanged("EffectFireOther"); }
        }

        private float _EffectFrostOther;
        public float EffectFrostOther
        {
            get { return _EffectFrostOther; }
            set { _EffectFrostOther = value; OnPropertyChanged("EffectFrostOther"); }
        }

        private float _EffectShadowOther;
        public float EffectShadowOther
        {
            get { return _EffectShadowOther; }
            set { _EffectShadowOther = value; OnPropertyChanged("EffectShadowOther"); }
        }

        private float _EffectNatureOther;
        public float EffectNatureOther
        {
            get { return _EffectNatureOther; }
            set { _EffectNatureOther = value; OnPropertyChanged("EffectNatureOther"); }
        }

        private float _EffectHolyOther;
        public float EffectHolyOther
        {
            get { return _EffectHolyOther; }
            set { _EffectHolyOther = value; OnPropertyChanged("EffectHolyOther"); }
        }


        [XmlIgnore]
        public string ShattrathFaction
        {
            get
            {
                return Aldor ? "Aldor" : "Scryers";
            }
            set
            {
                Aldor = (value == "Aldor");
            }
        }

        public CalculationOptionsMage Clone()
        {
            CalculationOptionsMage clone = (CalculationOptionsMage)MemberwiseClone();
            clone.IncrementalSetArmor = null;
            clone.IncrementalSetStateIndexes = null;
            clone.IncrementalSetSegments = null;
            clone.IncrementalSetSpells = null;
            clone.IncrementalSetVariableType = null;
            clone.IncrementalSetManaSegment = null;
            return clone;
        }

        public CalculationOptionsMage()
        {
            TargetLevel = 83;
            AoeTargetLevel = 80;
            LatencyCast = 0.01f;
            LatencyGCD = 0.05f;
            LatencyChannel = 0.2f;
            AoeTargets = 9;
            FightDuration = 300;
            HeroismAvailable = true;
            MoltenFuryPercentage = 0.3f;
            PotionOfWildMagic = true;
            PotionOfSpeed = true;
            FlameCap = false;
            DpsTime = 1;
            InterruptFrequency = 0;
            AoeDuration = 0;
            SmartOptimization = true;
            DrumsOfBattle = false;
            AutomaticArmor = false;
            TpsLimit = 0;
            IncrementalOptimizations = true;
            ReconstructSequence = false;
            Innervate = 0;
            ManaTide = 0;
            Fragmentation = 0;
            SurvivabilityRating = 0.0001f;
            Aldor = true;
            EvocationEnabled = true;
            ManaPotionEnabled = true;
            ManaGemEnabled = true;
            MaxHeapLimit = 300;
            DrinkingTime = 300;
            BurstWindow = 5f;
            BurstImpacts = 5f;
            //ChanceToLiveLimit = 99f;
            PlayerLevel = 80;
            FocusMagicTargetCritRate = 0.2f;
            SnaredTime = 1f;
            FixedSegmentDuration = 30;
            WarlockSpellPower = 2800;
            EffectSpiritMultiplier = 1.0f;
            EffectDamageMultiplier = 1.0f;
            EffectHasteMultiplier = 1.0f;
            EffectRegenMultiplier = 1.0f;
            EffectCostMultiplier = 1.0f;
            ChanceToLiveAttenuation = 0.1f;
            MaxUseAssumption = true;
            FrostbiteUtilization = 1.0f;
            MirrorImageEnabled = true;
            Enable2T10Evocation = true;
            Encounter = new Encounter();
        }

        public CalculationOptionsMage(Character character)
            : this()
        {
            _character = character;
            character.CalculationsInvalidated += new EventHandler(Character_ItemsChanged);
        }

        string ICalculationOptionBase.GetXml()
        {
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsMage));
            StringBuilder xml = new StringBuilder();
            System.IO.StringWriter writer = new System.IO.StringWriter(xml);
            serializer.Serialize(writer, this);
            return xml.ToString();
        }

        #region INotifyPropertyChanged Members
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
