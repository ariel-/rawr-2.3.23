using System;
using System.Collections.Generic;
using System.Text;
#if RAWR3
using System.Windows.Media;
#else
using System.Drawing;
#endif

namespace Rawr.Mage
{
    #region Helper Classes
    public class Segment
    {
        public int Index { get; set; }
        public double Duration { get; set; }
        public double TimeStart { get; set; }
        public double TimeEnd { get { return TimeStart + Duration; } }
        //public int FirstSpellColumn { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1} - {2})", Index, CalculationsMage.TimeFormat(TimeStart), CalculationsMage.TimeFormat(TimeEnd));
        }
    }

    public struct SegmentConstraint
    {
        public int Row;
        public int MinSegment;
        public int MaxSegment;
    }

    public struct StackingConstraint
    {
        public int Row;
        public EffectCooldown Effect1;
        public EffectCooldown Effect2;
        public double MaximumStackingDuration;
    }

    public class EffectCooldown
    {
        public int Mask { get; set; }
        public bool ItemBased { get; set; }
        public StandardEffect StandardEffect { get; set; }
        public SpecialEffect SpecialEffect { get; set; }
        public float Cooldown { get; set; }
        public float Duration { get; set; }
        public bool HasteEffect { get; set; }
        public int Row { get; set; }
        public List<SegmentConstraint> SegmentConstraints { get; set; }
        public string Name { get; set; }
        public float MaximumDuration { get; set; }
        public bool AutomaticConstraints { get; set; }
        public bool AutomaticStackingConstraints { get; set; }
        public Color Color { get; set; }

        public void Clear()
        {
            // Row is always initialized in ConstructRows
            // MaximumDuration is initialized in InitializeEffectCooldowns or in ConstructRows if AutomaticConstraints is true
            if (SegmentConstraints != null)
            {
                SegmentConstraints.Clear();
            }
        }

        public static implicit operator int(EffectCooldown cooldown)
        {
            return cooldown.Mask;
        }

        public EffectCooldown Clone()
        {
            EffectCooldown clone = (EffectCooldown)MemberwiseClone();
            clone.SegmentConstraints = null;
            return clone;
        }
    }
    #endregion

    public sealed partial class Solver
    {
        #region Variables
        // initialized in constructor
        public Character Character { get; set; }
        public MageTalents MageTalents { get; set; }
        public CalculationOptionsMage CalculationOptions { get; set; }
        private bool segmentCooldowns;
        private bool segmentMana;
        private int advancedConstraintsLevel;
        private bool integralMana;
        private string armor;
        private bool useIncrementalOptimizations;
        private bool useGlobalOptimizations;
        public bool NeedsDisplayCalculations { get; private set; }
        private bool requiresMIP;
        private bool needsSolutionVariables;
        private bool cancellationPending;
        private bool needsQuadratic;

        public ArraySet ArraySet { get; set; }

        // initialized in Initialize
        public Stats BaseStats { get; set; }
        public List<Buff> ActiveBuffs;
        private List<Buff> autoActivatedBuffs;
        private TargetDebuffStats targetDebuffs;
        private bool restrictThreat;

        public float ManaGemValue;
        public float ManaPotionValue;
        public float MaxManaGemValue;
        public float MaxManaPotionValue;

        private bool heroismAvailable;
        private bool arcanePowerAvailable;
        private bool icyVeinsAvailable;
        private bool combustionAvailable;
        private bool moltenFuryAvailable;
        private bool coldsnapAvailable;
        private bool potionOfWildMagicAvailable;
        private bool potionOfSpeedAvailable;
        private bool effectPotionAvailable;
        private bool berserkingAvailable;
        private bool flameCapAvailable;
        private bool waterElementalAvailable;
        private bool mirrorImageAvailable;
        private bool manaGemEffectAvailable;
        private bool powerInfusionAvailable;
        private bool evocationAvailable;
        private bool manaPotionAvailable;

        // initialized in InitializeEffectCooldowns
        private const int standardEffectCount = 14; // can't just compute from enum, because that counts the combined masks also
        public List<EffectCooldown> CooldownList { get; set; }
        public Dictionary<int, EffectCooldown> EffectCooldown { get; set; }
        private int[] effectExclusionList;
        private int cooldownCount;
        public float ManaGemEffectDuration;
        private int availableCooldownMask;

        public const float CombustionCooldown = 120.0f;
        public const float PowerInfusionDuration = 15.0f;
        public const float PowerInfusionCooldown = 120.0f;
        public const float MirrorImageDuration = 30.0f;
        public const float MirrorImageCooldown = 180.0f;
        public float IcyVeinsCooldown;
        public float ColdsnapCooldown;
        public float ArcanePowerCooldown;
        public float ArcanePowerDuration;
        public float WaterElementalCooldown;
        public float WaterElementalDuration;
        public float EvocationCooldown;

        public EffectCooldown[] ItemBasedEffectCooldowns { get; set; }
        public int ItemBasedEffectCooldownsCount;
        public EffectCooldown[] StackingHasteEffectCooldowns { get; set; }
        public int StackingHasteEffectCooldownsCount;
        public EffectCooldown[] StackingNonHasteEffectCooldowns { get; set; }
        public int StackingNonHasteEffectCooldownsCount;

        // initialized in InitializeProcEffects
        public SpecialEffect[] SpellPowerEffects { get; set; }
        public int SpellPowerEffectsCount;
        public SpecialEffect[] HasteRatingEffects { get; set; }
        public int HasteRatingEffectsCount;
        public SpecialEffect[] DamageProcEffects { get; set; }
        public int DamageProcEffectsCount;
        public SpecialEffect[] ManaRestoreEffects { get; set; }
        public int ManaRestoreEffectsCount;
        public SpecialEffect[] Mp5Effects { get; set; }
        public int Mp5EffectsCount;

        // initialized in CalculateBaseStateStats

        #region Base State Stats
        public float BaseSpellHit { get; set; }
        public float RawArcaneHitRate { get; set; }
        public float RawFireHitRate { get; set; }
        public float RawFrostHitRate { get; set; }
        public float BaseArcaneHitRate { get; set; }
        public float BaseFireHitRate { get; set; }
        public float BaseFrostHitRate { get; set; }
        public float BaseNatureHitRate { get; set; }
        public float BaseShadowHitRate { get; set; }
        public float BaseFrostFireHitRate { get; set; }
        public float BaseHolyHitRate { get; set; }

        public float ArcaneThreatMultiplier { get; set; }
        public float FireThreatMultiplier { get; set; }
        public float FrostThreatMultiplier { get; set; }
        public float NatureThreatMultiplier { get; set; }
        public float ShadowThreatMultiplier { get; set; }
        public float FrostFireThreatMultiplier { get; set; }
        public float HolyThreatMultiplier { get; set; }

        public float BaseSpellModifier { get; set; }
        public float BaseArcaneSpellModifier { get; set; }
        public float BaseFireSpellModifier { get; set; }
        public float BaseFrostSpellModifier { get; set; }
        public float BaseNatureSpellModifier { get; set; }
        public float BaseShadowSpellModifier { get; set; }
        public float BaseFrostFireSpellModifier { get; set; }
        public float BaseHolySpellModifier { get; set; }

        public float BaseAdditiveSpellModifier { get; set; }
        public float BaseArcaneAdditiveSpellModifier { get; set; }
        public float BaseFireAdditiveSpellModifier { get; set; }
        public float BaseFrostAdditiveSpellModifier { get; set; }
        public float BaseNatureAdditiveSpellModifier { get; set; }
        public float BaseShadowAdditiveSpellModifier { get; set; }
        public float BaseFrostFireAdditiveSpellModifier { get; set; }
        public float BaseHolyAdditiveSpellModifier { get; set; }

        public float BaseCritRate { get; set; }
        public float BaseArcaneCritRate { get; set; }
        public float BaseFireCritRate { get; set; }
        public float BaseFrostCritRate { get; set; }
        public float BaseNatureCritRate { get; set; }
        public float BaseShadowCritRate { get; set; }
        public float BaseFrostFireCritRate { get; set; }
        public float BaseHolyCritRate { get; set; }

        public float IgniteFactor { get; set; }

        public float BaseArcaneCritBonus { get; set; }
        public float BaseFireCritBonus { get; set; }
        public float BaseFrostCritBonus { get; set; }
        public float BaseNatureCritBonus { get; set; }
        public float BaseShadowCritBonus { get; set; }
        public float BaseFrostFireCritBonus { get; set; }
        public float BaseHolyCritBonus { get; set; }

        public float CombustionFireCritBonus { get; set; }
        public float CombustionFrostFireCritBonus { get; set; }

        public float BaseArcaneSpellPower { get; set; }
        public float BaseFireSpellPower { get; set; }
        public float BaseFrostSpellPower { get; set; }
        public float BaseNatureSpellPower { get; set; }
        public float BaseShadowSpellPower { get; set; }
        public float BaseHolySpellPower { get; set; }

        public float SpiritRegen { get; set; }
        public float ManaRegen { get; set; }
        public float ManaRegen5SR { get; set; }
        public float ManaRegenDrinking { get; set; }
        public float HealthRegen { get; set; }
        public float HealthRegenCombat { get; set; }
        public float HealthRegenEating { get; set; }
        public float MeleeMitigation { get; set; }
        public float Defense { get; set; }
        public float PhysicalCritReduction { get; set; }
        public float SpellCritReduction { get; set; }
        public float CritDamageReduction { get; set; }
        public float DamageTakenReduction { get; set; }
        public float Dodge { get; set; }

        public float BaseCastingSpeed { get; set; }
        public float CastingSpeedMultiplier { get; set; }
        public float BaseGlobalCooldown { get; set; }

        public float IncomingDamageAmpMelee { get; set; }
        public float IncomingDamageAmpPhysical { get; set; }
        public float IncomingDamageAmpArcane { get; set; }
        public float IncomingDamageAmpFire { get; set; }
        public float IncomingDamageAmpFrost { get; set; }
        public float IncomingDamageAmpNature { get; set; }
        public float IncomingDamageAmpShadow { get; set; }
        public float IncomingDamageAmpHoly { get; set; }

        public float IncomingDamageDpsMelee { get; set; }
        public float IncomingDamageDpsPhysical { get; set; }
        public float IncomingDamageDpsArcane { get; set; }
        public float IncomingDamageDpsFire { get; set; }
        public float IncomingDamageDpsFrost { get; set; }
        public float IncomingDamageDpsNature { get; set; }
        public float IncomingDamageDpsShadow { get; set; }
        public float IncomingDamageDpsHoly { get; set; }

        public float IncomingDamageDps { get; set; }

        public int MaxTalents { get; set; }
        public float Mastery { get; set; }
        public float ManaAdeptBonus { get; set; }

        #endregion

        // initialized in InitializeSpellTemplates

        #region Spell Templates
        private WaterboltTemplate _WaterboltTemplate;
        public WaterboltTemplate WaterboltTemplate
        {
            get
            {
                if (_WaterboltTemplate == null)
                {
                    _WaterboltTemplate = new WaterboltTemplate();
                }
                if (_WaterboltTemplate.Dirty)
                {
                    _WaterboltTemplate.Initialize(this);
                }
                return _WaterboltTemplate;
            }
        }

        private MirrorImageTemplate _MirrorImageTemplate;
        public MirrorImageTemplate MirrorImageTemplate
        {
            get
            {
                if (_MirrorImageTemplate == null)
                {
                    _MirrorImageTemplate = new MirrorImageTemplate();
                }
                if (_MirrorImageTemplate.Dirty)
                {
                    _MirrorImageTemplate.Initialize(this);
                }
                return _MirrorImageTemplate;
            }
        }

        private FireBlastTemplate _FireBlastTemplate;
        public FireBlastTemplate FireBlastTemplate
        {
            get
            {
                if (_FireBlastTemplate == null)
                {
                    _FireBlastTemplate = new FireBlastTemplate();
                }
                if (_FireBlastTemplate.Dirty)
                {
                    _FireBlastTemplate.Initialize(this);
                }
                return _FireBlastTemplate;
            }
        }

        private LightningBoltTemplate _LightningBoltTemplate;
        public LightningBoltTemplate LightningBoltTemplate
        {
            get
            {
                if (_LightningBoltTemplate == null)
                {
                    _LightningBoltTemplate = new LightningBoltTemplate();
                }
                if (_LightningBoltTemplate.Dirty)
                {
                    _LightningBoltTemplate.Initialize(this);
                }
                return _LightningBoltTemplate;
            }
        }

        private ThunderBoltTemplate _ThunderBoltTemplate;
        public ThunderBoltTemplate ThunderBoltTemplate
        {
            get
            {
                if (_ThunderBoltTemplate == null)
                {
                    _ThunderBoltTemplate = new ThunderBoltTemplate();
                }
                if (_ThunderBoltTemplate.Dirty)
                {
                    _ThunderBoltTemplate.Initialize(this);
                }
                return _ThunderBoltTemplate;
            }
        }

        private LightweaveBoltTemplate _LightweaveBoltTemplate;
        public LightweaveBoltTemplate LightweaveBoltTemplate
        {
            get
            {
                if (_LightweaveBoltTemplate == null)
                {
                    _LightweaveBoltTemplate = new LightweaveBoltTemplate();
                }
                if (_LightweaveBoltTemplate.Dirty)
                {
                    _LightweaveBoltTemplate.Initialize(this);
                }
                return _LightweaveBoltTemplate;
            }
        }

        private ArcaneBoltTemplate _ArcaneBoltTemplate;
        public ArcaneBoltTemplate ArcaneBoltTemplate
        {
            get
            {
                if (_ArcaneBoltTemplate == null)
                {
                    _ArcaneBoltTemplate = new ArcaneBoltTemplate();
                }
                if (_ArcaneBoltTemplate.Dirty)
                {
                    _ArcaneBoltTemplate.Initialize(this);
                }
                return _ArcaneBoltTemplate;
            }
        }

        private PendulumOfTelluricCurrentsTemplate _PendulumOfTelluricCurrentsTemplate;
        public PendulumOfTelluricCurrentsTemplate PendulumOfTelluricCurrentsTemplate
        {
            get
            {
                if (_PendulumOfTelluricCurrentsTemplate == null)
                {
                    _PendulumOfTelluricCurrentsTemplate = new PendulumOfTelluricCurrentsTemplate();
                }
                if (_PendulumOfTelluricCurrentsTemplate.Dirty)
                {
                    _PendulumOfTelluricCurrentsTemplate.Initialize(this);
                }
                return _PendulumOfTelluricCurrentsTemplate;
            }
        }

        private FrostboltTemplate _FrostboltTemplate;
        public FrostboltTemplate FrostboltTemplate
        {
            get
            {
                if (_FrostboltTemplate == null)
                {
                    _FrostboltTemplate = new FrostboltTemplate();
                }
                if (_FrostboltTemplate.Dirty)
                {
                    _FrostboltTemplate.Initialize(this);
                }
                return _FrostboltTemplate;
            }
        }

        private FrostfireBoltTemplate _FrostfireBoltTemplate;
        public FrostfireBoltTemplate FrostfireBoltTemplate
        {
            get
            {
                if (_FrostfireBoltTemplate == null)
                {
                    _FrostfireBoltTemplate = new FrostfireBoltTemplate();
                }
                if (_FrostfireBoltTemplate.Dirty)
                {
                    _FrostfireBoltTemplate.Initialize(this);
                }
                return _FrostfireBoltTemplate;
            }
        }

        private ArcaneMissilesTemplate _ArcaneMissilesTemplate;
        public ArcaneMissilesTemplate ArcaneMissilesTemplate
        {
            get
            {
                if (_ArcaneMissilesTemplate == null)
                {
                    _ArcaneMissilesTemplate = new ArcaneMissilesTemplate();
                }
                if (_ArcaneMissilesTemplate.Dirty)
                {
                    _ArcaneMissilesTemplate.Initialize(this);
                }
                return _ArcaneMissilesTemplate;
            }
        }

        private FireballTemplate _FireballTemplate;
        public FireballTemplate FireballTemplate
        {
            get
            {
                if (_FireballTemplate == null)
                {
                    _FireballTemplate = new FireballTemplate();
                }
                if (_FireballTemplate.Dirty)
                {
                    _FireballTemplate.Initialize(this);
                }
                return _FireballTemplate;
            }
        }

        private PyroblastTemplate _PyroblastTemplate;
        public PyroblastTemplate PyroblastTemplate
        {
            get
            {
                if (_PyroblastTemplate == null)
                {
                    _PyroblastTemplate = new PyroblastTemplate();
                }
                if (_PyroblastTemplate.Dirty)
                {
                    _PyroblastTemplate.Initialize(this);
                }
                return _PyroblastTemplate;
            }
        }

        private ScorchTemplate _ScorchTemplate;
        public ScorchTemplate ScorchTemplate
        {
            get
            {
                if (_ScorchTemplate == null)
                {
                    _ScorchTemplate = new ScorchTemplate();
                }
                if (_ScorchTemplate.Dirty)
                {
                    _ScorchTemplate.Initialize(this);
                }
                return _ScorchTemplate;
            }
        }

        private ArcaneBarrageTemplate _ArcaneBarrageTemplate;
        public ArcaneBarrageTemplate ArcaneBarrageTemplate
        {
            get
            {
                if (_ArcaneBarrageTemplate == null)
                {
                    _ArcaneBarrageTemplate = new ArcaneBarrageTemplate();
                }
                if (_ArcaneBarrageTemplate.Dirty)
                {
                    _ArcaneBarrageTemplate.Initialize(this);
                }
                return _ArcaneBarrageTemplate;
            }
        }

        private DeepFreezeTemplate _DeepFreezeTemplate;
        public DeepFreezeTemplate DeepFreezeTemplate
        {
            get
            {
                if (_DeepFreezeTemplate == null)
                {
                    _DeepFreezeTemplate = new DeepFreezeTemplate();
                }
                if (_DeepFreezeTemplate.Dirty)
                {
                    _DeepFreezeTemplate.Initialize(this);
                }
                return _DeepFreezeTemplate;
            }
        }

        private ArcaneBlastTemplate _ArcaneBlastTemplate;
        public ArcaneBlastTemplate ArcaneBlastTemplate
        {
            get
            {
                if (_ArcaneBlastTemplate == null)
                {
                    _ArcaneBlastTemplate = new ArcaneBlastTemplate();
                }
                if (_ArcaneBlastTemplate.Dirty)
                {
                    _ArcaneBlastTemplate.Initialize(this);
                }
                return _ArcaneBlastTemplate;
            }
        }

        private IceLanceTemplate _IceLanceTemplate;
        public IceLanceTemplate IceLanceTemplate
        {
            get
            {
                if (_IceLanceTemplate == null)
                {
                    _IceLanceTemplate = new IceLanceTemplate();
                }
                if (_IceLanceTemplate.Dirty)
                {
                    _IceLanceTemplate.Initialize(this);
                }
                return _IceLanceTemplate;
            }
        }

        private ArcaneExplosionTemplate _ArcaneExplosionTemplate;
        public ArcaneExplosionTemplate ArcaneExplosionTemplate
        {
            get
            {
                if (_ArcaneExplosionTemplate == null)
                {
                    _ArcaneExplosionTemplate = new ArcaneExplosionTemplate();
                }
                if (_ArcaneExplosionTemplate.Dirty)
                {
                    _ArcaneExplosionTemplate.Initialize(this);
                }
                return _ArcaneExplosionTemplate;
            }
        }

        private FlamestrikeTemplate _FlamestrikeTemplate;
        public FlamestrikeTemplate FlamestrikeTemplate
        {
            get
            {
                if (_FlamestrikeTemplate == null)
                {
                    _FlamestrikeTemplate = new FlamestrikeTemplate();
                }
                if (_FlamestrikeTemplate.Dirty)
                {
                    _FlamestrikeTemplate.Initialize(this);
                }
                return _FlamestrikeTemplate;
            }
        }

        private BlizzardTemplate _BlizzardTemplate;
        public BlizzardTemplate BlizzardTemplate
        {
            get
            {
                if (_BlizzardTemplate == null)
                {
                    _BlizzardTemplate = new BlizzardTemplate();
                }
                if (_BlizzardTemplate.Dirty)
                {
                    _BlizzardTemplate.Initialize(this);
                }
                return _BlizzardTemplate;
            }
        }

        private BlastWaveTemplate _BlastWaveTemplate;
        public BlastWaveTemplate BlastWaveTemplate
        {
            get
            {
                if (_BlastWaveTemplate == null)
                {
                    _BlastWaveTemplate = new BlastWaveTemplate();
                }
                if (_BlastWaveTemplate.Dirty)
                {
                    _BlastWaveTemplate.Initialize(this);
                }
                return _BlastWaveTemplate;
            }
        }

        private DragonsBreathTemplate _DragonsBreathTemplate;
        public DragonsBreathTemplate DragonsBreathTemplate
        {
            get
            {
                if (_DragonsBreathTemplate == null)
                {
                    _DragonsBreathTemplate = new DragonsBreathTemplate();
                }
                if (_DragonsBreathTemplate.Dirty)
                {
                    _DragonsBreathTemplate.Initialize(this);
                }
                return _DragonsBreathTemplate;
            }
        }

        private ConeOfColdTemplate _ConeOfColdTemplate;
        public ConeOfColdTemplate ConeOfColdTemplate
        {
            get
            {
                if (_ConeOfColdTemplate == null)
                {
                    _ConeOfColdTemplate = new ConeOfColdTemplate();
                }
                if (_ConeOfColdTemplate.Dirty)
                {
                    _ConeOfColdTemplate.Initialize(this);
                }
                return _ConeOfColdTemplate;
            }
        }

        private SlowTemplate _SlowTemplate;
        public SlowTemplate SlowTemplate
        {
            get
            {
                if (_SlowTemplate == null)
                {
                    _SlowTemplate = new SlowTemplate();
                }
                if (_SlowTemplate.Dirty)
                {
                    _SlowTemplate.Initialize(this);
                }
                return _SlowTemplate;
            }
        }

        private LivingBombTemplate _LivingBombTemplate;
        public LivingBombTemplate LivingBombTemplate
        {
            get
            {
                if (_LivingBombTemplate == null)
                {
                    _LivingBombTemplate = new LivingBombTemplate();
                }
                if (_LivingBombTemplate.Dirty)
                {
                    _LivingBombTemplate.Initialize(this);
                }
                return _LivingBombTemplate;
            }
        }

        private FireWardTemplate _FireWardTemplate;
        public FireWardTemplate FireWardTemplate
        {
            get
            {
                if (_FireWardTemplate == null)
                {
                    _FireWardTemplate = new FireWardTemplate();
                }
                if (_FireWardTemplate.Dirty)
                {
                    _FireWardTemplate.Initialize(this);
                }
                return _FireWardTemplate;
            }
        }

        private FrostWardTemplate _FrostWardTemplate;
        public FrostWardTemplate FrostWardTemplate
        {
            get
            {
                if (_FrostWardTemplate == null)
                {
                    _FrostWardTemplate = new FrostWardTemplate();
                }
                if (_FrostWardTemplate.Dirty)
                {
                    _FrostWardTemplate.Initialize(this);
                }
                return _FrostWardTemplate;
            }
        }

        private ConjureManaGemTemplate _ConjureManaGemTemplate;
        public ConjureManaGemTemplate ConjureManaGemTemplate
        {
            get
            {
                if (_ConjureManaGemTemplate == null)
                {
                    _ConjureManaGemTemplate = new ConjureManaGemTemplate();
                }
                if (_ConjureManaGemTemplate.Dirty)
                {
                    _ConjureManaGemTemplate.Initialize(this);
                }
                return _ConjureManaGemTemplate;
            }
        }

        private ArcaneDamageTemplate _ArcaneDamageTemplate;
        public ArcaneDamageTemplate ArcaneDamageTemplate
        {
            get
            {
                if (_ArcaneDamageTemplate == null)
                {
                    _ArcaneDamageTemplate = new ArcaneDamageTemplate();
                }
                if (_ArcaneDamageTemplate.Dirty)
                {
                    _ArcaneDamageTemplate.Initialize(this);
                }
                return _ArcaneDamageTemplate;
            }
        }

        private FireDamageTemplate _FireDamageTemplate;
        public FireDamageTemplate FireDamageTemplate
        {
            get
            {
                if (_FireDamageTemplate == null)
                {
                    _FireDamageTemplate = new FireDamageTemplate();
                }
                if (_FireDamageTemplate.Dirty)
                {
                    _FireDamageTemplate.Initialize(this);
                }
                return _FireDamageTemplate;
            }
        }

        private FrostDamageTemplate _FrostDamageTemplate;
        public FrostDamageTemplate FrostDamageTemplate
        {
            get
            {
                if (_FrostDamageTemplate == null)
                {
                    _FrostDamageTemplate = new FrostDamageTemplate();
                }
                if (_FrostDamageTemplate.Dirty)
                {
                    _FrostDamageTemplate.Initialize(this);
                }
                return _FrostDamageTemplate;
            }
        }

        private ShadowDamageTemplate _ShadowDamageTemplate;
        public ShadowDamageTemplate ShadowDamageTemplate
        {
            get
            {
                if (_ShadowDamageTemplate == null)
                {
                    _ShadowDamageTemplate = new ShadowDamageTemplate();
                }
                if (_ShadowDamageTemplate.Dirty)
                {
                    _ShadowDamageTemplate.Initialize(this);
                }
                return _ShadowDamageTemplate;
            }
        }

        private NatureDamageTemplate _NatureDamageTemplate;
        public NatureDamageTemplate NatureDamageTemplate
        {
            get
            {
                if (_NatureDamageTemplate == null)
                {
                    _NatureDamageTemplate = new NatureDamageTemplate();
                }
                if (_NatureDamageTemplate.Dirty)
                {
                    _NatureDamageTemplate.Initialize(this);
                }
                return _NatureDamageTemplate;
            }
        }

        private HolyDamageTemplate _HolyDamageTemplate;
        public HolyDamageTemplate HolyDamageTemplate
        {
            get
            {
                if (_HolyDamageTemplate == null)
                {
                    _HolyDamageTemplate = new HolyDamageTemplate();
                }
                if (_HolyDamageTemplate.Dirty)
                {
                    _HolyDamageTemplate.Initialize(this);
                }
                return _HolyDamageTemplate;
            }
        }

        private ValkyrDamageTemplate _ValkyrDamageTemplate;
        public ValkyrDamageTemplate ValkyrDamageTemplate
        {
            get
            {
                if (_ValkyrDamageTemplate == null)
                {
                    _ValkyrDamageTemplate = new ValkyrDamageTemplate();
                }
                if (_ValkyrDamageTemplate.Dirty)
                {
                    _ValkyrDamageTemplate.Initialize(this);
                }
                return _ValkyrDamageTemplate;
            }
        }

        private WandTemplate _WandTemplate;
        public WandTemplate WandTemplate
        {
            get
            {
                if (_WandTemplate == null)
                {
                    _WandTemplate = new WandTemplate();
                }
                return _WandTemplate;
            }
        }

        #endregion

        // initialized in GenerateStateList
        private List<CastingState> stateList;
        private List<CastingState> scratchStateList = new List<CastingState>();
        public CastingState BaseState { get; set; }

        // initialized in GenerateSpellList
        private List<CycleId> spellList;

        // initialized in ConstructProblem
        private bool segmentNonCooldowns;
        private bool minimizeTime;
        private bool restrictManaUse;
        private bool needsTimeExtension;
        private bool conjureManaGem;
        private float dpsTime;
        public int manaSegments;

        private SolverLP lp;
        private int[] segmentColumn;
        public List<SolutionVariable> SolutionVariable { get; set; }

        private const double ManaRegenLPScaling = 0.01;
        public float StartingMana { get; set; }

        public Cycle ConjureManaGem { get; set; }
        public int MaxConjureManaGem { get; set; }

        public Spell Wand { get; set; }

        public float MaxEvocation;
        public float EvocationDuration;
        public float EvocationRegen;
        public float EvocationDurationIV;
        public float EvocationRegenIV;
        public float EvocationDurationHero;
        public float EvocationRegenHero;
        public float EvocationDurationIVHero;
        public float EvocationRegenIVHero;

        public int MaxManaGem;
        public float ManaGemTps;
        public float ManaPotionTps;        

        // initialized in ConstructSegments
        public List<Segment> SegmentList { get; set; }

        // initialized in ConstructRows
        private StackingConstraint[] rowStackingConstraint;
        private int rowStackingConstraintCount;

        #region LP rows
        private int rowManaRegen;
        private int rowFightDuration;
        private int rowEvocation;
        private int rowEvocationIV;
        private int rowEvocationHero;
        private int rowEvocationIVHero;
        private int rowPotion;
        private int rowManaPotion;
        private int rowConjureManaGem;
        private int rowManaGem;
        private int rowManaGemMax;
        private int rowHeroism;
        private int rowArcanePower;
        private int rowIcyVeins;
        private int rowWaterElemental;
        private int rowMirrorImage;
        private int rowMoltenFury;
        private int rowMoltenFuryIcyVeins;
        private int rowFlameCap;
        private int rowManaGemEffect;
        private int rowManaGemEffectActivation;
        private int rowDpsTime;
        private int rowAoe;
        private int rowFlamestrike;
        private int rowConeOfCold;
        private int rowBlastWave;
        private int rowDragonsBreath;
        private int rowCombustion;
        private int rowPowerInfusion;
        private int rowMoltenFuryCombustion;
        private int rowHeroismCombustion;
        private int rowHeroismIcyVeins;
        private int rowSummonWaterElemental;
        private int rowSummonWaterElementalCount;
        private int rowSummonMirrorImage;
        private int rowSummonMirrorImageCount;
        private int rowThreat;
        private int rowBerserking;
        private int rowTimeExtension;
        private int rowAfterFightRegenMana;
        private int rowTargetDamage;
        private int rowSegment;
        private int rowSegmentManaOverflow;
        private int rowSegmentManaUnderflow;
        private int rowSegmentThreat;
        private List<SegmentConstraint> rowSegmentArcanePower;
        private List<SegmentConstraint> rowSegmentPowerInfusion;
        private List<SegmentConstraint> rowSegmentIcyVeins;
        private List<SegmentConstraint> rowSegmentWaterElemental;
        private List<SegmentConstraint> rowSegmentSummonWaterElemental;
        private List<SegmentConstraint> rowSegmentMirrorImage;
        private List<SegmentConstraint> rowSegmentSummonMirrorImage;
        private List<SegmentConstraint> rowSegmentCombustion;
        private List<SegmentConstraint> rowSegmentBerserking;
        private List<SegmentConstraint> rowSegmentFlameCap;
        private List<SegmentConstraint> rowSegmentManaGem;
        private List<SegmentConstraint> rowSegmentManaGemEffect;
        private List<SegmentConstraint> rowSegmentEvocation;
        #endregion

        // initialized in RestrictSolution
        private double[] solution;
        private double lowerBound;
        private double upperBound;

        // initialized in EvaluateSurvivability
        public float ChanceToDie { get; set; }
        public float MeanIncomingDps { get; set; }
        #endregion

        #region Public Methods and Properties
        public TargetDebuffStats TargetDebuffs
        {
            get
            {
                if (targetDebuffs == null)
                {
                    targetDebuffs = new TargetDebuffStats();
                    foreach (Buff buff in ActiveBuffs)
                    {
                        if (buff.IsTargetDebuff)
                        {
                            targetDebuffs.Accumulate(buff.Stats);
                        }
                    }
                }
                return targetDebuffs;
            }
        }

        internal bool CancellationPending
        {
            get
            {
                return cancellationPending;
            }
        }

        public void CancelAsync()
        {
            cancellationPending = true;
        }

        public List<EffectCooldown> GetEffectList(int effects)
        {
            return CooldownList.FindAll(effect => (effects & effect.Mask) == effect.Mask);
        }

        public string EffectsDescription(int effects)
        {
            List<string> buffList = new List<string>();
            List<EffectCooldown> cooldownList = CooldownList;
            for (int i = 0; i < cooldownList.Count; i++)
            {
                EffectCooldown effect = cooldownList[i];
                if ((effects & effect.Mask) == effect.Mask)
                {
                    buffList.Add(effect.Name);
                }
            }
            return string.Join("+", buffList.ToArray());
        }

        private static bool IsItemActivatable(ItemInstance item)
        {
            if (item == null || item.Item == null) return false;
            return (item.Item.Stats.ContainsSpecialEffect(effect => effect.Trigger == Trigger.Use));
        }
        #endregion

        public Solver(Character character, CalculationOptionsMage calculationOptions, bool segmentCooldowns, bool segmentMana, bool integralMana, int advancedConstraintsLevel, string armor, bool useIncrementalOptimizations, bool useGlobalOptimizations, bool needsDisplayCalculations, bool needsSolutionVariables)
        {
            Construct(character, calculationOptions, segmentCooldowns, segmentMana, integralMana, advancedConstraintsLevel, armor, useIncrementalOptimizations, useGlobalOptimizations, needsDisplayCalculations, needsSolutionVariables);
        }

        private void Construct(Character character, CalculationOptionsMage calculationOptions, bool segmentCooldowns, bool segmentMana, bool integralMana, int advancedConstraintsLevel, string armor, bool useIncrementalOptimizations, bool useGlobalOptimizations, bool needsDisplayCalculations, bool needsSolutionVariables)
        {
            this.Character = character;
            this.MageTalents = character.MageTalents;
            this.CalculationOptions = calculationOptions;
            this.segmentCooldowns = segmentCooldowns;
            this.segmentMana = segmentMana;
            this.advancedConstraintsLevel = advancedConstraintsLevel;
            this.integralMana = integralMana;
            this.armor = armor;
            this.useIncrementalOptimizations = useIncrementalOptimizations;
            this.useGlobalOptimizations = useGlobalOptimizations;
            this.NeedsDisplayCalculations = needsDisplayCalculations;
            this.requiresMIP = segmentCooldowns || integralMana;
            if (needsDisplayCalculations || requiresMIP) needsSolutionVariables = true;
            this.needsSolutionVariables = needsSolutionVariables;
            this.needsQuadratic = false;
            cancellationPending = false;
        }

        [ThreadStatic]
        private static Solver threadSolver;

        public static CharacterCalculationsMage GetCharacterCalculations(Character character, Item additionalItem, CalculationOptionsMage calculationOptions, CalculationsMage calculations, string armor, bool segmentCooldowns, bool segmentMana, bool integralMana, int advancedConstraintsLevel, bool useIncrementalOptimizations, bool useGlobalOptimizations, bool needsDisplayCalculations, bool needsSolutionVariables)
        {
            if (threadSolver == null)
            {
                threadSolver = new Solver(character, calculationOptions, segmentCooldowns, segmentMana, integralMana, advancedConstraintsLevel, armor, useIncrementalOptimizations, useGlobalOptimizations, needsDisplayCalculations, needsSolutionVariables);
            }
            else
            {
                threadSolver.Construct(character, calculationOptions, segmentCooldowns, segmentMana, integralMana, advancedConstraintsLevel, armor, useIncrementalOptimizations, useGlobalOptimizations, needsDisplayCalculations, needsSolutionVariables);
            }
            return threadSolver.GetCharacterCalculations(additionalItem);
        }

        public CharacterCalculationsMage GetCharacterCalculations(Item additionalItem)
        {
            ArraySet = ArrayPool.RequestArraySet(!useIncrementalOptimizations && NeedsDisplayCalculations && segmentCooldowns);

            Initialize(additionalItem);

            GenerateSpellList();
            GenerateStateList();

            ConstructProblem();

            if (needsQuadratic)
            {
                SolveQuadratic();
            }

            if (requiresMIP)
            {
                RestrictSolution();
            }

            solution = lp.Solve();            

            var ret = GetCalculationsResult();

            ArrayPool.ReleaseArraySet(ArraySet);
            ArraySet = null;

            return ret;
        }

        #region Effect Maximization
        private double MaximizeColdsnapDuration(double fightDuration, double coldsnapCooldown, double effectDuration, double effectCooldown, out int coldsnapCount)
        {
            int bestColdsnap = 0;
            double bestEffect = 0.0;
            List<int> coldsnap = new List<int>();
            List<double> startTime = new List<double>();
            List<double> coldsnapTime = new List<double>();
            int index = 0;
            coldsnap.Add(2);
            startTime.Add(0.0);
            coldsnapTime.Add(0.0);
            do
            {
                if (index > 0 && startTime[index - 1] + effectDuration >= fightDuration)
                {
                    double effect = (index - 1) * effectDuration + Math.Max(fightDuration - startTime[index - 1], 0.0);
                    if (effect > bestEffect)
                    {
                        bestEffect = effect;
                        bestColdsnap = 0;
                        for (int i = 0; i < index; i++)
                        {
                            if (startTime[i] < fightDuration - 20.0) bestColdsnap += coldsnap[i]; // if it is a coldsnap for a very short elemental, don't count it for IV
                        }
                    }
                    index--;
                }
                coldsnap[index]--;
                if (coldsnap[index] < 0)
                {
                    index--;
                }
                else
                {
                    double time = 0.0;
                    if (index > 0)
                    {
                        time = startTime[index - 1] + effectDuration;
                        int lastColdsnap = -1;
                        for (int j = 0; j < index; j++)
                        {
                            if (coldsnap[j] == 1) lastColdsnap = j;
                        }
                        if (coldsnap[index] == 1)
                        {
                            // use coldsnap
                            double normalTime = Math.Max(time, startTime[index - 1] + effectCooldown);
                            double coldsnapReady = 0.0;
                            if (lastColdsnap >= 0) coldsnapReady = coldsnapTime[lastColdsnap] + coldsnapCooldown;
                            if (coldsnapReady >= normalTime)
                            {
                                // coldsnap won't be ready until effect will be back anyway, so we don't actually need it
                                coldsnap[index] = 0;
                                time = normalTime;
                            }
                            else
                            {
                                // go now or when coldsnap is ready
                                time = Math.Max(coldsnapReady, time);
                                coldsnapTime[index] = Math.Max(coldsnapReady, startTime[index - 1]);
                            }
                        }
                        else
                        {
                            // we are not allowed to use coldsnap even if we could
                            // make sure to adjust by coldsnap constraints
                            time = Math.Max(time, startTime[index - 1] + effectCooldown);
                        }
                    }
                    else
                    {
                        coldsnap[index] = 0;
                    }
                    startTime[index] = time;
                    index++;
                    if (index >= coldsnap.Count)
                    {
                        coldsnap.Add(0);
                        coldsnapTime.Add(0.0);
                        startTime.Add(0.0);
                    }
                    coldsnap[index] = 2;
                }
            } while (index >= 0);
            coldsnapCount = bestColdsnap;
            return bestEffect;
        }

        private double MaximizeEffectDuration(double fightDuration, double effectDuration, double effectCooldown)
        {
            if (fightDuration < effectDuration) return fightDuration;
            double total = effectDuration;
            fightDuration -= effectDuration;
            int count = (int)(fightDuration / effectCooldown);
            total += effectDuration * count;
            fightDuration -= effectCooldown * count;
            fightDuration -= effectCooldown - effectDuration;
            if (fightDuration > 0) total += fightDuration;
            return total;
        }

        private double MaximizeStackingDuration(double fightDuration, double effect1Duration, double effect1Cooldown, double effect2Duration, double effect2Cooldown)
        {
            /*if (double.IsPositiveInfinity(effect1Cooldown) || double.IsPositiveInfinity(effect2Cooldown))
            {
                return MaximizeStackingDuration(fightDuration, effect1Duration, effect1Cooldown, effect2Duration, effect2Cooldown, 0, 0);
            }
            else*/
            {
                var cache = CalculationOptions.CooldownStackingCache;
                lock (cache)
                {
                    /*for (int i = 0; i < cache.Count; i++)
                    {
                        var entry = cache[i];
                        if ((entry.Effect1Cooldown == effect1Cooldown && entry.Effect2Cooldown == effect2Cooldown && entry.Effect1Duration == effect1Duration && entry.Effect2Duration == effect2Duration) ||
                            (entry.Effect1Cooldown == effect2Cooldown && entry.Effect2Cooldown == effect1Cooldown && entry.Effect1Duration == effect2Duration && entry.Effect2Duration == effect1Duration))
                        {
                            return entry.MaximumStackingDuration;
                        }
                    }*/
                    // do binary search
                    int num = 0;
                    int num2 = cache.Count - 1;
                    while (num <= num2)
                    {
                        int num3 = num + ((num2 - num) >> 1);
                        var entry = cache[num3];
                        double key = entry.Effect1Duration;
                        if (key < effect1Duration)
                        {
                            num = num3 + 1;
                        }
                        else if (key > effect1Duration)
                        {
                            num2 = num3 - 1;
                        }
                        else
                        {
                            key = entry.Effect1Cooldown;
                            if (key < effect1Cooldown)
                            {
                                num = num3 + 1;
                            }
                            else if (key > effect1Cooldown)
                            {
                                num2 = num3 - 1;
                            }
                            else
                            {
                                key = entry.Effect2Duration;
                                if (key < effect2Duration)
                                {
                                    num = num3 + 1;
                                }
                                else if (key > effect2Duration)
                                {
                                    num2 = num3 - 1;
                                }
                                else
                                {
                                    key = entry.Effect2Cooldown;
                                    if (key < effect2Cooldown)
                                    {
                                        num = num3 + 1;
                                    }
                                    else if (key > effect2Cooldown)
                                    {
                                        num2 = num3 - 1;
                                    }
                                    else
                                    {
                                        return entry.MaximumStackingDuration;
                                    }
                                }
                            }
                        }
                    }

                    double value;
                    //System.Diagnostics.Stopwatch clock = new System.Diagnostics.Stopwatch();
                    //clock.Reset();
                    if (double.IsPositiveInfinity(effect1Cooldown) || double.IsPositiveInfinity(effect2Cooldown))
                    {
                        //clock.Start();
                        value = MaximizeStackingDuration(fightDuration, effect1Duration, effect1Cooldown, effect2Duration, effect2Cooldown, 0, 0);
                        //clock.Stop();
                        //System.Diagnostics.Trace.WriteLine("noncache = " + clock.ElapsedTicks);
                        //clock.Reset();
                    }
                    else
                    {
                        //clock.Start();
                        memoizationCache = new List<StackingMemoizationEntry>[(int)fightDuration + 1];
                        value = MaximizeStackingDuration2((int)fightDuration, (int)effect1Duration, (int)effect1Cooldown, (int)effect2Duration, (int)effect2Cooldown, 0, 0, 0);
                        memoizationCache = null;
                        //clock.Stop();
                        //System.Diagnostics.Trace.WriteLine("cache = " + clock.ElapsedTicks);
                    }
                    cache.Insert(num, new CooldownStackingCacheEntry()
                    {
                        Effect1Duration = effect1Duration,
                        Effect1Cooldown = effect1Cooldown,
                        Effect2Duration = effect2Duration,
                        Effect2Cooldown = effect2Cooldown,
                        MaximumStackingDuration = value
                    });
                    return value;
                }
            }
        }

        private double MaximizeStackingDuration(double fightDuration, double effect1Duration, double effect1Cooldown, double effect2Duration, double effect2Cooldown, double effect2ActiveDuration, double effect2ActiveCooldown)
        {
            if (fightDuration <= 0) return 0;
            if (double.IsPositiveInfinity(effect2ActiveCooldown) && effect2ActiveDuration == 0) return 0;
            effect2ActiveDuration = Math.Min(effect2ActiveDuration, fightDuration);

            double slack = 0;
            double f = fightDuration;

            if (f < effect1Duration)
            {
                slack = 0;
            }
            else
            {
                f -= effect1Duration;
                int count = (int)(f / effect1Cooldown);
                if (count > 0)
                {
                    f -= effect1Cooldown * count;
                }
                if (f - effect1Cooldown + effect1Duration > 0)
                {
                    slack = 0;
                }
                else
                {
                    slack = f;
                }
            }
            if (!CalculationOptions.MaxUseAssumption)
            {
                slack = effect2ActiveCooldown;
            }


            // ####........|
            double best = 0;
            double value = 0;
            double min = 0;

            if (effect2ActiveCooldown > 0)
            {
                // if optimal placement of effect1 is stacked with effect2 activation
                // and it doesn't overlap two different activations
                // or if optimal placement has effect1 activated and finished before effect2 gets off cooldown
                // then we'll get as good or better stacking if we move effect1 all the way to the start
                if (effect1Cooldown < effect2ActiveCooldown)
                {
                    // effect1 will be off cooldown first
                    value = Math.Min(effect1Duration, effect2ActiveDuration) + MaximizeStackingDuration(fightDuration - effect1Cooldown, effect1Duration, effect1Cooldown, effect2Duration, effect2Cooldown, Math.Max(0, effect2ActiveDuration - effect1Cooldown), Math.Max(0, effect2ActiveCooldown - effect1Cooldown));
                }
                else
                {
                    // effect2 will be off cooldown first
                    value = Math.Min(effect1Duration, effect2ActiveDuration) + MaximizeStackingDuration(fightDuration - effect2ActiveCooldown, effect2Duration, effect2Cooldown, effect1Duration, effect1Cooldown, Math.Max(0, effect1Duration - effect2ActiveCooldown), Math.Max(0, effect1Cooldown - effect2ActiveCooldown));
                }
                if (value > best)
                {
                    best = value;
                }
            }
            // the next case is if effect1 activation crosses over effect2 cooldown
            // in this case it's just as good if effect2 starts right on cooldown
            // now in this case moving effect1 earlier has negative effect so we can't do that
            // we want however to push it as late as possible without affecting the optimum
            // but just as long as it ends before effect2 ends
            // ####........|#####
            // ..........#######
            // 0 <= offset <= effect2ActiveCooldown
            // offset <= effect1Duration
            // this can potentially still be optimized, I doubt we need to look at every option
            // but it seems in practice it works well enough so don't waste time unless profiling shows need
            // TODO ok it works well enough, but still costs a significant amount, time to improve this
            if (!double.IsPositiveInfinity(effect2ActiveCooldown))
            {
                int minOffset = Math.Max(0, (int)(effect2ActiveCooldown - slack));
                int maxOffset = (int)Math.Min(effect1Duration, effect2ActiveCooldown);
                if (effect2Cooldown >= effect1Duration)
                {
                    int endAlignedOffset = (int)(effect1Duration - effect2Duration);
                    if (endAlignedOffset > minOffset)
                    {
                        minOffset = Math.Min(endAlignedOffset, maxOffset);
                    }
                }
                for (int offset = minOffset; offset <= maxOffset; offset++)
                {
                    // is there any stacking left from current effect2 activation?
                    double leftover = Math.Max(0, effect2ActiveDuration - (effect2ActiveCooldown - offset));
                    min = Math.Min(effect1Duration - offset, effect2Duration);
                    if (effect1Cooldown - offset < effect2Cooldown)
                    {
                        // effect1 will be off cooldown first
                        value = leftover + Math.Min(min, fightDuration - effect2ActiveCooldown) + MaximizeStackingDuration(fightDuration - effect2ActiveCooldown - effect1Cooldown + offset, effect1Duration, effect1Cooldown, effect2Duration, effect2Cooldown, Math.Max(0, effect2Duration - effect1Cooldown + offset), Math.Max(0, effect2Cooldown - effect1Cooldown + offset));
                    }
                    else
                    {
                        // effect2 will be off cooldown first
                        value = leftover + Math.Min(min, fightDuration - effect2ActiveCooldown) + MaximizeStackingDuration(fightDuration - effect2ActiveCooldown - effect2Cooldown, effect2Duration, effect2Cooldown, effect1Duration, effect1Cooldown, Math.Max(0, effect1Duration - offset - effect2Cooldown), Math.Max(0, effect1Cooldown - offset - effect2Cooldown));
                    }
                    if (value > best)
                    {
                        best = value;
                    }
                }
            }
            return best;
        }

        private struct StackingMemoizationEntry
        {
            public int Order;
            public int Effect2ActiveDuration;
            public int Effect2ActiveCooldown;
            public int MaximizeStackingDuration;
        }

        private List<StackingMemoizationEntry>[] memoizationCache;

        private int MaximizeStackingDuration2(int fightDuration, int effect1Duration, int effect1Cooldown, int effect2Duration, int effect2Cooldown, int effect2ActiveDuration, int effect2ActiveCooldown, int order)
        {
            if (fightDuration <= 0) return 0;
            effect2ActiveDuration = Math.Min(effect2ActiveDuration, fightDuration);

            List<StackingMemoizationEntry> list = memoizationCache[fightDuration];
            if (list == null)
            {
                list = new List<StackingMemoizationEntry>();
                memoizationCache[fightDuration] = list;
            }

            for (int i = 0; i < list.Count; i++)
            {
                StackingMemoizationEntry entry = list[i];
                if (entry.Effect2ActiveCooldown == effect2ActiveCooldown && entry.Order == order && entry.Effect2ActiveDuration == effect2ActiveDuration)
                {
                    return entry.MaximizeStackingDuration;
                }
            }

            int slack = 0;
            int f = fightDuration;

            if (f < effect1Duration)
            {
                slack = 0;
            }
            else
            {
                f -= effect1Duration;
                int count = f / effect1Cooldown;
                if (count > 0)
                {
                    f -= effect1Cooldown * count;
                }
                if (f - effect1Cooldown + effect1Duration > 0)
                {
                    slack = 0;
                }
                else
                {
                    slack = f;
                }
            }
            if (!CalculationOptions.MaxUseAssumption)
            {
                slack = effect2ActiveCooldown;
            }


            // ####........|
            int best = 0;
            int value = 0;
            int min = 0;

            if (effect2ActiveCooldown > 0)
            {
                // if optimal placement of effect1 is stacked with effect2 activation
                // and it doesn't overlap two different activations
                // or if optimal placement has effect1 activated and finished before effect2 gets off cooldown
                // then we'll get as good or better stacking if we move effect1 all the way to the start
                if (effect1Cooldown < effect2ActiveCooldown)
                {
                    // effect1 will be off cooldown first
                    value = Math.Min(effect1Duration, effect2ActiveDuration) + MaximizeStackingDuration2(fightDuration - effect1Cooldown, effect1Duration, effect1Cooldown, effect2Duration, effect2Cooldown, Math.Max(0, effect2ActiveDuration - effect1Cooldown), Math.Max(0, effect2ActiveCooldown - effect1Cooldown), order);
                }
                else
                {
                    // effect2 will be off cooldown first
                    value = Math.Min(effect1Duration, effect2ActiveDuration) + MaximizeStackingDuration2(fightDuration - effect2ActiveCooldown, effect2Duration, effect2Cooldown, effect1Duration, effect1Cooldown, Math.Max(0, effect1Duration - effect2ActiveCooldown), Math.Max(0, effect1Cooldown - effect2ActiveCooldown), 1 - order);
                }
                if (value > best)
                {
                    best = value;
                }
            }
            // the next case is if effect1 activation crosses over effect2 cooldown
            // in this case it's just as good if effect2 starts right on cooldown
            // now in this case moving effect1 earlier has negative effect so we can't do that
            // we want however to push it as late as possible without affecting the optimum
            // but just as long as it ends before effect2 ends
            // ####........|#####
            // ..........#######
            // 0 <= offset <= effect2ActiveCooldown
            // offset <= effect1Duration
            // this can potentially still be optimized, I doubt we need to look at every option
            // but it seems in practice it works well enough so don't waste time unless profiling shows need
            // TODO ok it works well enough, but still costs a significant amount, time to improve this
            int minOffset = Math.Max(0, effect2ActiveCooldown - slack);
            int maxOffset = Math.Min(effect1Duration, effect2ActiveCooldown);
            if (effect2Cooldown >= effect1Duration)
            {
                int endAlignedOffset = effect1Duration - effect2Duration;
                if (endAlignedOffset > minOffset)
                {
                    minOffset = Math.Min(endAlignedOffset, maxOffset);
                }
            }
            for (int offset = minOffset; offset <= maxOffset; offset++)
            {
                // is there any stacking left from current effect2 activation?
                int leftover = Math.Max(0, effect2ActiveDuration - (effect2ActiveCooldown - offset));
                min = Math.Min(effect1Duration - offset, effect2Duration);
                if (effect1Cooldown - offset < effect2Cooldown)
                {
                    // effect1 will be off cooldown first
                    value = leftover + Math.Min(min, fightDuration - effect2ActiveCooldown) + MaximizeStackingDuration2(fightDuration - effect2ActiveCooldown - effect1Cooldown + offset, effect1Duration, effect1Cooldown, effect2Duration, effect2Cooldown, Math.Max(0, effect2Duration - effect1Cooldown + offset), Math.Max(0, effect2Cooldown - effect1Cooldown + offset), order);
                }
                else
                {
                    // effect2 will be off cooldown first
                    value = leftover + Math.Min(min, fightDuration - effect2ActiveCooldown) + MaximizeStackingDuration2(fightDuration - effect2ActiveCooldown - effect2Cooldown, effect2Duration, effect2Cooldown, effect1Duration, effect1Cooldown, Math.Max(0, effect1Duration - offset - effect2Cooldown), Math.Max(0, effect1Cooldown - offset - effect2Cooldown), 1 - order);
                }
                if (value > best)
                {
                    best = value;
                }
            }

            list.Add(new StackingMemoizationEntry()
            {
                Order = order,
                Effect2ActiveDuration = effect2ActiveDuration,
                Effect2ActiveCooldown = effect2ActiveCooldown,
                MaximizeStackingDuration = best
            });

            return best;
        }
        #endregion

        #region Initialize
        public void Initialize(Item additionalItem)
        {
            Stats rawStats;
            if (NeedsDisplayCalculations || ArraySet == null)
            {
                rawStats = new Stats();
            }
            else
            {
                rawStats = ArraySet.accumulator;
                if (rawStats == null)
                {
                    rawStats = new Stats();
                    ArraySet.accumulator = rawStats;
                }
                else
                {
                    rawStats.Clear();
                }
            }

            CalculationsMage calculations = CalculationsMage.Instance;
            targetDebuffs = null;
            calculations.AccumulateRawStats(rawStats, Character, additionalItem, CalculationOptions, out autoActivatedBuffs, armor, out ActiveBuffs);
            BaseStats = calculations.GetCharacterStats(Character, additionalItem, rawStats, CalculationOptions);

            evocationAvailable = CalculationOptions.EvocationEnabled && !CalculationOptions.EffectDisableManaSources;
            manaPotionAvailable = CalculationOptions.ManaPotionEnabled && !CalculationOptions.EffectDisableManaSources;
            restrictThreat = segmentCooldowns && CalculationOptions.TpsLimit > 0f;
            powerInfusionAvailable = !CalculationOptions.DisableCooldowns && CalculationOptions.PowerInfusionAvailable;
            heroismAvailable = !CalculationOptions.DisableCooldowns && CalculationOptions.HeroismAvailable;
            arcanePowerAvailable = !CalculationOptions.DisableCooldowns && (MageTalents.ArcanePower == 1);
            icyVeinsAvailable = !CalculationOptions.DisableCooldowns && (MageTalents.IcyVeins == 1);
            combustionAvailable = !CalculationOptions.DisableCooldowns && (MageTalents.Combustion == 1);
            moltenFuryAvailable = MageTalents.MoltenFury > 0;
            coldsnapAvailable = !CalculationOptions.DisableCooldowns && (MageTalents.ColdSnap == 1);
            potionOfWildMagicAvailable = !CalculationOptions.DisableCooldowns && CalculationOptions.PotionOfWildMagic;
            potionOfSpeedAvailable = !CalculationOptions.DisableCooldowns && CalculationOptions.PotionOfSpeed;
            effectPotionAvailable = potionOfWildMagicAvailable || potionOfSpeedAvailable;
            flameCapAvailable = !CalculationOptions.DisableCooldowns && CalculationOptions.FlameCap;
            berserkingAvailable = !CalculationOptions.DisableCooldowns && Character.Race == CharacterRace.Troll;
            waterElementalAvailable = !CalculationOptions.DisableCooldowns && (MageTalents.SummonWaterElemental == 1);
            mirrorImageAvailable = !CalculationOptions.DisableCooldowns && CalculationOptions.MirrorImageEnabled;
            manaGemEffectAvailable = CalculationOptions.ManaGemEnabled && BaseStats.ContainsSpecialEffect(effect => effect.Trigger == Trigger.ManaGem);

            if (!CalculationOptions.EffectDisableManaSources)
            {
                if (CalculationOptions.PlayerLevel < 77)
                {
                    ManaGemValue = 2400.0f;
                    MaxManaGemValue = 2460.0f;
                }
                else
                {
                    ManaGemValue = 3415.0f;
                    MaxManaGemValue = 3500.0f;
                }
                if (CalculationOptions.PlayerLevel <= 70)
                {
                    ManaPotionValue = 2400.0f;
                    MaxManaPotionValue = 3000.0f;
                }
                else
                {
                    ManaPotionValue = 4300.0f;
                    MaxManaPotionValue = 4400.0f;
                }
            }

            InitializeEffectCooldowns();
            InitializeProcEffects();

            if (armor == null)
            {
                if (Character.ActiveBuffs.Contains(CalculationsMage.MoltenArmorBuff)) armor = "Molten Armor";
                else if (Character.ActiveBuffs.Contains(CalculationsMage.MageArmorBuff)) armor = "Mage Armor";
                else if (Character.ActiveBuffs.Contains(CalculationsMage.IceArmorBuff)) armor = "Ice Armor";
            }

            CalculateBaseStateStats();

            InitializeSpellTemplates();
        }

        private void InitializeProcEffects()
        {
            Stats baseStats = BaseStats;
            int N = baseStats._rawSpecialEffectDataSize;
            HasteRatingEffectsCount = 0;
            if (HasteRatingEffects == null || HasteRatingEffects.Length < N)
            {
                HasteRatingEffects = new SpecialEffect[N];
            }
            SpellPowerEffectsCount = 0;
            if (SpellPowerEffects == null || SpellPowerEffects.Length < N)
            {
                SpellPowerEffects = new SpecialEffect[N];
            }
            DamageProcEffectsCount = 0;
            if (DamageProcEffects == null || DamageProcEffects.Length < N)
            {
                DamageProcEffects = new SpecialEffect[N];
            }
            ManaRestoreEffectsCount = 0;
            if (ManaRestoreEffects == null || ManaRestoreEffects.Length < N)
            {
                ManaRestoreEffects = new SpecialEffect[N];
            }
            Mp5EffectsCount = 0;
            if (Mp5Effects == null || Mp5Effects.Length < N)
            {
                Mp5Effects = new SpecialEffect[N];
            }
            for (int i = 0; i < baseStats._rawSpecialEffectDataSize; i++)
            {
                SpecialEffect effect = baseStats._rawSpecialEffectData[i];
                if (effect.Stats.HasteRating > 0 && effect.MaxStack == 1)
                {
                    if (effect.Cooldown >= effect.Duration && (effect.Trigger == Trigger.DamageSpellCrit || effect.Trigger == Trigger.SpellCrit || effect.Trigger == Trigger.DamageSpellHit || effect.Trigger == Trigger.SpellHit || effect.Trigger == Trigger.SpellCast || effect.Trigger == Trigger.DamageSpellCast))
                    {
                        HasteRatingEffects[HasteRatingEffectsCount++] = effect;
                    }
                    else if (effect.Cooldown == 0 && (effect.Trigger == Trigger.SpellCrit || effect.Trigger == Trigger.DamageSpellCrit))
                    {
                        HasteRatingEffects[HasteRatingEffectsCount++] = effect;
                    }
                }
                if (effect.Stats.SpellPower > 0 && effect.MaxStack == 1)
                {
                    if (effect.Trigger == Trigger.DamageSpellCrit || effect.Trigger == Trigger.SpellCrit || effect.Trigger == Trigger.DamageSpellHit || effect.Trigger == Trigger.SpellHit || effect.Trigger == Trigger.SpellCast || effect.Trigger == Trigger.DamageSpellCast || effect.Trigger == Trigger.SpellMiss || effect.Trigger == Trigger.MageNukeCast || effect.Trigger == Trigger.DamageDone || effect.Trigger == Trigger.DoTTick || effect.Trigger == Trigger.DamageOrHealingDone)
                    {
                        SpellPowerEffects[SpellPowerEffectsCount++] = effect;
                    }
                }
                if (effect.Stats.ArcaneDamage + effect.Stats.FireDamage + effect.Stats.FrostDamage + effect.Stats.NatureDamage + effect.Stats.ShadowDamage + effect.Stats.HolyDamage + effect.Stats.ValkyrDamage > 0 && effect.MaxStack == 1)
                {
                    if (effect.Trigger == Trigger.DamageSpellCrit || effect.Trigger == Trigger.SpellCrit || effect.Trigger == Trigger.DamageSpellHit || effect.Trigger == Trigger.SpellHit || effect.Trigger == Trigger.DamageDone || effect.Trigger == Trigger.DoTTick || effect.Trigger == Trigger.DamageOrHealingDone || effect.Trigger == Trigger.SpellCast || effect.Trigger == Trigger.DamageSpellCast)
                    {
                        DamageProcEffects[DamageProcEffectsCount++] = effect;
                    }
                }
                if (effect.Stats.ManaRestore > 0 && effect.MaxStack == 1)
                {
                    if (effect.Trigger == Trigger.Use || effect.Trigger == Trigger.DamageSpellCast || effect.Trigger == Trigger.DamageSpellCrit || effect.Trigger == Trigger.DamageSpellHit || effect.Trigger == Trigger.SpellCast || effect.Trigger == Trigger.SpellCrit || effect.Trigger == Trigger.SpellHit || effect.Trigger == Trigger.DamageDone || effect.Trigger == Trigger.DoTTick || effect.Trigger == Trigger.DamageOrHealingDone)
                    {
                        ManaRestoreEffects[ManaRestoreEffectsCount++] = effect;
                    }
                }
                if (effect.Stats.Mp5 > 0 && effect.MaxStack == 1)
                {
                    if (effect.Trigger == Trigger.Use || effect.Trigger == Trigger.DamageSpellCast || effect.Trigger == Trigger.DamageSpellCrit || effect.Trigger == Trigger.DamageSpellHit || effect.Trigger == Trigger.SpellCast || effect.Trigger == Trigger.SpellCrit || effect.Trigger == Trigger.SpellHit || effect.Trigger == Trigger.DamageDone || effect.Trigger == Trigger.DoTTick || effect.Trigger == Trigger.DamageOrHealingDone)
                    {
                        Mp5Effects[Mp5EffectsCount++] = effect;
                    }
                }
            }
        }

        private bool IsRelevantOnUseEffect(SpecialEffect effect, out bool hasteEffect, out bool stackingEffect)
        {
            // check if it is a stacking use effect
            stackingEffect = false;
            hasteEffect = false;
            Stats effectStats = effect.Stats;
            for (int i = 0; i < effectStats._rawSpecialEffectDataSize; i++)
            {
                SpecialEffect e = effectStats._rawSpecialEffectData[i];
                if (e.Chance == 1f && e.Cooldown == 0f && (e.Trigger == Trigger.DamageSpellCast || e.Trigger == Trigger.DamageSpellHit || e.Trigger == Trigger.SpellCast || e.Trigger == Trigger.SpellHit))
                {
                    if (e.Stats.HasteRating > 0)
                    {
                        hasteEffect = true;
                        stackingEffect = true;
                        break;
                    }
                }
                if (e.Chance == 1f && e.Cooldown == 0f && (e.Trigger == Trigger.DamageSpellCrit || e.Trigger == Trigger.SpellCrit))
                {
                    if (e.Stats.CritRating < 0 && effect.Stats.CritRating > 0)
                    {
                        stackingEffect = true;
                        break;
                    }
                }
            }
            if (stackingEffect)
            {
                return true;
            }
            if (effect.Stats.HasteRating > 0)
            {
                hasteEffect = true;
            }
            return effect.Stats.SpellPower + effect.Stats.HasteRating > 0;
        }

        private static readonly Color[] itemColors = new Color[] {
                Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF), //Aqua
                Color.FromArgb(255, 0, 0, 255),
                Color.FromArgb(0xFF, 0xFF, 0x7F, 0x50), //Coral
                Color.FromArgb(0xFF, 0xBD, 0xB7, 0x6B), //DarkKhaki
                Color.FromArgb(0xFF, 0x2F, 0x4F, 0x4F), //DarkSlateGray
                Color.FromArgb(0xFF, 0xB2, 0x22, 0x22), //Firebrick
                Color.FromArgb(0xFF, 0xFF, 0xD7, 0x00), //Gold
                Color.FromArgb(0xFF, 0xFF, 0xFF, 0xF0), //Ivory
            };

        private EffectCooldown NewEffectCooldown()
        {
            if (NeedsDisplayCalculations || ArraySet == null)
            {
                return new EffectCooldown();
            }
            else
            {
                EffectCooldown effect = ArraySet.NewEffectCooldown();
                effect.Clear();
                return effect;
            }
        }

        private EffectCooldown NewStandardEffectCooldown(EffectCooldown cachedEffect)
        {
            if (NeedsDisplayCalculations || ArraySet == null)
            {
                return cachedEffect.Clone();
            }
            else
            {
                cachedEffect.Clear();
                return cachedEffect;
            }
        }

        EffectCooldown cachedEffectEvocation = new EffectCooldown()
        {
            Mask = (int)StandardEffect.Evocation,
            Name = "Evocation",
            StandardEffect = StandardEffect.Evocation,
            Color = Color.FromArgb(0xFF, 0x7F, 0xFF, 0xD4) //Aquamarine
        };
        EffectCooldown cachedEffectPowerInfusion = new EffectCooldown()
        {
            Cooldown = PowerInfusionCooldown,
            Duration = PowerInfusionDuration,
            AutomaticConstraints = true,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.PowerInfusion,
            Name = "Power Infusion",
            StandardEffect = StandardEffect.PowerInfusion,
            Color = Color.FromArgb(255, 255, 255, 0),
        };
        EffectCooldown cachedEffectPotionOfSpeed = new EffectCooldown()
        {
            Cooldown = float.PositiveInfinity,
            Duration = 15.0f,
            MaximumDuration = 15.0f,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.PotionOfSpeed,
            Name = "Potion of Speed",
            StandardEffect = StandardEffect.PotionOfSpeed,
            Color = Color.FromArgb(0xFF, 0xFF, 0xFA, 0xCD) //LemonChiffon
        };
        EffectCooldown cachedEffectArcanePower = new EffectCooldown()
        {
            AutomaticConstraints = true,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.ArcanePower,
            Name = "Arcane Power",
            StandardEffect = StandardEffect.ArcanePower,
            Color = Color.FromArgb(0xFF, 0xF0, 0xFF, 0xFF) //Azure
        };
        EffectCooldown cachedEffectCombustion = new EffectCooldown()
        {
            Cooldown = CombustionCooldown,
            Mask = (int)StandardEffect.Combustion,
            Name = "Combustion",
            StandardEffect = StandardEffect.Combustion,
            Color = Color.FromArgb(255, 255, 69, 0),
        };
        EffectCooldown cachedEffectPotionOfWildMagic = new EffectCooldown()
        {
            Cooldown = float.PositiveInfinity,
            Duration = 15.0f,
            MaximumDuration = 15.0f,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.PotionOfWildMagic,
            Name = "Potion of Wild Magic",
            StandardEffect = StandardEffect.PotionOfWildMagic,
            Color = Color.FromArgb(255, 128, 0, 128),
        };
        EffectCooldown cachedEffectBerserking = new EffectCooldown()
        {
            Cooldown = 180.0f,
            Duration = 10.0f,
            AutomaticConstraints = true,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.Berserking,
            Name = "Berserking",
            StandardEffect = StandardEffect.Berserking,
            Color = Color.FromArgb(0xFF, 0xA5, 0x2A, 0x2A) //Brown
        };
        EffectCooldown cachedEffectFlameCap = new EffectCooldown()
        {
            Cooldown = 180.0f,
            Duration = 60.0f,
            AutomaticConstraints = true,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.FlameCap,
            Name = "Flame Cap",
            StandardEffect = StandardEffect.FlameCap,
            Color = Color.FromArgb(255, 255, 165, 0),
        };
        EffectCooldown cachedEffectHeroism = new EffectCooldown()
        {
            Cooldown = float.PositiveInfinity,
            Duration = 40.0f,
            AutomaticConstraints = true,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.Heroism,
            Name = "Heroism",
            StandardEffect = StandardEffect.Heroism,
            Color = Color.FromArgb(0xFF, 0x80, 0x80, 0x00) //Olive
        };
        EffectCooldown cachedEffectIcyVeins = new EffectCooldown()
        {
            Duration = 20.0f,
            Mask = (int)StandardEffect.IcyVeins,
            Name = "Icy Veins",
            StandardEffect = StandardEffect.IcyVeins,
            Color = Color.FromArgb(0xFF, 0x00, 0x00, 0x8B) //DarkBlue
        };
        EffectCooldown cachedEffectMoltenFury = new EffectCooldown()
        {
            Cooldown = float.PositiveInfinity,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.MoltenFury,
            Name = "Molten Fury",
            StandardEffect = StandardEffect.MoltenFury,
            Color = Color.FromArgb(0xFF, 0xDC, 0x14, 0x3C) //Crimson
        };
        EffectCooldown cachedEffectWaterElemental = new EffectCooldown()
        {
            Mask = (int)StandardEffect.WaterElemental,
            Name = "Water Elemental",
            StandardEffect = StandardEffect.WaterElemental,
            Color = Color.FromArgb(0xFF, 0x00, 0x8B, 0x8B) //DarkCyan
        };
        EffectCooldown cachedEffectMirrorImage = new EffectCooldown()
        {
            Cooldown = MirrorImageCooldown,
            Duration = MirrorImageDuration,
            Mask = (int)StandardEffect.MirrorImage,
            Name = "Mirror Image",
            StandardEffect = StandardEffect.MirrorImage,
            AutomaticConstraints = true,
            AutomaticStackingConstraints = true,
            Color = Color.FromArgb(0xFF, 0xFF, 0xA0, 0x7A), //LightSalmon
        };

        private void InitializeEffectCooldowns()
        {
            if (CooldownList == null)
            {
                CooldownList = new List<EffectCooldown>();
            }
            else
            {
                CooldownList.Clear();
            }

            EvocationCooldown = (240.0f - 60.0f * MageTalents.ArcaneFlows);
            ColdsnapCooldown = (8 * 60) * (1 - 0.1f * MageTalents.ColdAsIce);
            ArcanePowerCooldown = 120.0f * (1 - 0.15f * MageTalents.ArcaneFlows);
            ArcanePowerDuration = 15.0f + (MageTalents.GlyphOfArcanePower ? 3.0f : 0.0f);
            IcyVeinsCooldown = 180.0f * (1 - 0.07f * MageTalents.IceFloes + (MageTalents.IceFloes == 3 ? 0.01f : 0.00f));
            WaterElementalCooldown = (180.0f - (MageTalents.GlyphOfWaterElemental ? 30.0f : 0.0f)) * (1 - 0.1f * MageTalents.ColdAsIce);
            if (MageTalents.GlyphOfEternalWater)
            {
                WaterElementalDuration = float.PositiveInfinity;
            }
            else
            {
                WaterElementalDuration = 45.0f + 5.0f * MageTalents.EnduringWinter;
            }

            if (evocationAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectEvocation);
                cooldown.Cooldown = EvocationDuration;
                CooldownList.Add(cooldown);
            }
            if (powerInfusionAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectPowerInfusion);
                CooldownList.Add(cooldown);
            }
            if (potionOfSpeedAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectPotionOfSpeed);
                CooldownList.Add(cooldown);
            }
            if (arcanePowerAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectArcanePower);
                cooldown.Cooldown = ArcanePowerCooldown;
                cooldown.Duration = ArcanePowerDuration;
                CooldownList.Add(cooldown);
            }
            if (combustionAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectCombustion);
                CooldownList.Add(cooldown);
            }
            if (potionOfWildMagicAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectPotionOfWildMagic);
                CooldownList.Add(cooldown);
            }
            if (berserkingAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectBerserking);
                CooldownList.Add(cooldown);
            }
            if (flameCapAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectFlameCap);
                CooldownList.Add(cooldown);
            }
            if (heroismAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectHeroism);
                CooldownList.Add(cooldown);
            }
            if (icyVeinsAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectIcyVeins);
                cooldown.Cooldown = IcyVeinsCooldown;
                cooldown.AutomaticStackingConstraints = cooldown.AutomaticConstraints = (MageTalents.ColdSnap == 0);
                CooldownList.Add(cooldown);
            }
            if (moltenFuryAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectMoltenFury);
                cooldown.Duration = cooldown.MaximumDuration = CalculationOptions.MoltenFuryPercentage * CalculationOptions.FightDuration;
                CooldownList.Add(cooldown);
            }
            if (waterElementalAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectWaterElemental);
                cooldown.Cooldown = WaterElementalCooldown;
                cooldown.Duration = WaterElementalDuration;
                CooldownList.Add(cooldown);
            }
            if (mirrorImageAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectMirrorImage);
                CooldownList.Add(cooldown);
            }

            cooldownCount = standardEffectCount;
            int mask = 1 << standardEffectCount;

            int N = BaseStats._rawSpecialEffectDataSize;
            ItemBasedEffectCooldownsCount = 0;
            if (ItemBasedEffectCooldowns == null || ItemBasedEffectCooldowns.Length < N)
            {
                ItemBasedEffectCooldowns = new EffectCooldown[N];
            }
            StackingHasteEffectCooldownsCount = 0;
            if (StackingHasteEffectCooldowns == null || StackingHasteEffectCooldowns.Length < N)
            {
                StackingHasteEffectCooldowns = new EffectCooldown[N];
            }
            StackingNonHasteEffectCooldownsCount = 0;
            if (StackingNonHasteEffectCooldowns == null || StackingNonHasteEffectCooldowns.Length < N)
            {
                StackingNonHasteEffectCooldowns = new EffectCooldown[N];
            }
            int itemBasedMask = 0;
            bool hasteEffect, stackingEffect;

            int colorIndex = 0;

            if (!CalculationOptions.DisableCooldowns)
            {
                for (CharacterSlot i = 0; i < (CharacterSlot)Character.OptimizableSlotCount; i++)
                {
                    ItemInstance itemInstance = Character[i];
                    if ((object)itemInstance != null)
                    {
                        Item item = itemInstance.Item;
                        if (item != null)
                        {
                            Stats stats = item.Stats;
                            for (int j = 0; j < stats._rawSpecialEffectDataSize; j++)
                            {
                                SpecialEffect effect = stats._rawSpecialEffectData[j];
                                if (effect.Trigger == Trigger.Use && IsRelevantOnUseEffect(effect, out hasteEffect, out stackingEffect))
                                {
                                    EffectCooldown cooldown = NewEffectCooldown();
                                    cooldown.StandardEffect = 0;
                                    cooldown.SpecialEffect = effect;
                                    cooldown.HasteEffect = hasteEffect;
                                    cooldown.Mask = mask;
                                    itemBasedMask |= mask;
                                    mask <<= 1;
                                    cooldownCount++;
                                    cooldown.ItemBased = true;
                                    cooldown.Name = item.Name;
                                    cooldown.Cooldown = effect.Cooldown;
                                    cooldown.Duration = effect.Duration;
                                    cooldown.AutomaticConstraints = true;
                                    cooldown.AutomaticStackingConstraints = true;
                                    cooldown.Color = itemColors[Math.Min(itemColors.Length - 1, colorIndex++)];
                                    CooldownList.Add(cooldown);
                                    ItemBasedEffectCooldowns[ItemBasedEffectCooldownsCount++] = cooldown;
                                    if (stackingEffect)
                                    {
                                        if (hasteEffect)
                                        {
                                            StackingHasteEffectCooldowns[StackingHasteEffectCooldownsCount++] = cooldown;
                                        }
                                        else
                                        {
                                            StackingNonHasteEffectCooldowns[StackingNonHasteEffectCooldownsCount++] = cooldown;
                                        }
                                    }
                                }
                            }
                        }
                        Enchant enchant = itemInstance.Enchant;
                        if (enchant != null)
                        {
                            Stats stats = enchant.Stats;
                            for (int j = 0; j < stats._rawSpecialEffectDataSize; j++)
                            {
                                SpecialEffect effect = stats._rawSpecialEffectData[j];
                                if (effect.Trigger == Trigger.Use && IsRelevantOnUseEffect(effect, out hasteEffect, out stackingEffect))
                                {
                                    EffectCooldown cooldown = NewEffectCooldown();
                                    cooldown.StandardEffect = 0;
                                    cooldown.SpecialEffect = effect;
                                    cooldown.Mask = mask;
                                    cooldown.HasteEffect = hasteEffect;
                                    itemBasedMask |= mask;
                                    mask <<= 1;
                                    cooldownCount++;
                                    cooldown.ItemBased = true;
                                    cooldown.Name = enchant.Name;
                                    cooldown.Cooldown = effect.Cooldown;
                                    cooldown.Duration = effect.Duration;
                                    cooldown.AutomaticConstraints = true;
                                    cooldown.AutomaticStackingConstraints = true;
                                    cooldown.Color = itemColors[Math.Min(itemColors.Length - 1, colorIndex++)];
                                    CooldownList.Add(cooldown);
                                    ItemBasedEffectCooldowns[ItemBasedEffectCooldownsCount++] = cooldown;
                                    if (stackingEffect)
                                    {
                                        if (hasteEffect)
                                        {
                                            StackingHasteEffectCooldowns[StackingHasteEffectCooldownsCount++] = cooldown;
                                        }
                                        else
                                        {
                                            StackingNonHasteEffectCooldowns[StackingNonHasteEffectCooldownsCount++] = cooldown;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (manaGemEffectAvailable)
            {
                foreach (SpecialEffect effect in BaseStats.SpecialEffects(e => e.Trigger == Trigger.ManaGem))
                {
                    EffectCooldown cooldown = NewEffectCooldown();
                    cooldown.StandardEffect = 0;
                    cooldown.SpecialEffect = effect;
                    cooldown.Mask = (int)StandardEffect.ManaGemEffect;
                    cooldown.HasteEffect = false;
                    cooldown.ItemBased = true;
                    cooldown.Name = "Mana Gem Effect";
                    cooldown.Cooldown = 120f;
                    cooldown.Duration = effect.Duration;
                    cooldown.MaximumDuration = (float)MaximizeEffectDuration(CalculationOptions.FightDuration, effect.Duration, 120);
                    cooldown.AutomaticConstraints = false;
                    cooldown.AutomaticStackingConstraints = true;
                    cooldown.Color = Color.FromArgb(0xFF, 0x00, 0x64, 0x00); //DarkGreen
                    CooldownList.Add(cooldown);
                    ManaGemEffectDuration = effect.Duration;
                }
            }
            {
                ManaGemEffectDuration = 0;
            }

            if (EffectCooldown == null)
            {
                EffectCooldown = new Dictionary<int, EffectCooldown>(CooldownList.Count);
            }
            else
            {
                EffectCooldown.Clear();
            }
            availableCooldownMask = 0;
            foreach (EffectCooldown cooldown in CooldownList)
            {
                EffectCooldown[cooldown.Mask] = cooldown;
                if (cooldown.StandardEffect != StandardEffect.Evocation)
                {
                    availableCooldownMask |= cooldown.Mask;
                }
            }

            if (effectExclusionList == null)
            {
                effectExclusionList = new int[]
                {
                    (int)(StandardEffect.ArcanePower | StandardEffect.PowerInfusion),
                    (int)(StandardEffect.PotionOfSpeed | StandardEffect.PotionOfWildMagic),
                    itemBasedMask
                };
            }
            else
            {
                effectExclusionList[2] = itemBasedMask;
            }
        }

        private void CalculateBaseStateStats()
        {
            int[] talentData = MageTalents.Data;
            int arcane = 0;
            for (int i = 0; i <= 29; i++)
            {
                arcane += talentData[i];
            }
            int fire = 0;
            for (int i = 30; i <= 57; i++)
            {
                fire += talentData[i];
            }
            int frost = 0;
            for (int i = 58; i <= 85; i++)
            {
                frost += talentData[i];
            }
            MaxTalents = Math.Max(arcane, Math.Max(fire, frost));

            Stats baseStats = BaseStats;
            BaseSpellHit = baseStats.HitRating * CalculationOptions.LevelScalingFactor / 800f + baseStats.SpellHit + 0.01f * MageTalents.Precision;

            int targetLevel = CalculationOptions.TargetLevel;
            int playerLevel = CalculationOptions.PlayerLevel;

            float hitRate = ((targetLevel <= playerLevel + 2) ? (0.96f - (targetLevel - playerLevel) * 0.01f) : (0.94f - (targetLevel - playerLevel - 2) * 0.11f)) + BaseSpellHit;
            RawArcaneHitRate = hitRate + 0.01f * MageTalents.ArcaneFocus;
            RawFireHitRate = hitRate;
            RawFrostHitRate = hitRate;
            hitRate = Math.Min(Spell.MaxHitRate, hitRate);

            BaseArcaneHitRate = Math.Min(Spell.MaxHitRate, RawArcaneHitRate);
            BaseFireHitRate = hitRate;
            BaseFrostHitRate = hitRate;
            BaseNatureHitRate = hitRate;
            BaseShadowHitRate = hitRate;
            BaseFrostFireHitRate = hitRate;
            BaseHolyHitRate = hitRate;

            float threatFactor = (1 + baseStats.ThreatIncreaseMultiplier) * (1 - baseStats.ThreatReductionMultiplier);

            ArcaneThreatMultiplier = threatFactor * (1 - MageTalents.ArcaneSubtlety * 0.2f);
            FireThreatMultiplier = threatFactor * (1 - MageTalents.BurningSoul * 0.1f);
            FrostThreatMultiplier = threatFactor * (1 - ((MageTalents.FrostChanneling > 0) ? (0.01f + 0.03f * MageTalents.FrostChanneling) : 0f));
            FrostFireThreatMultiplier = threatFactor * Math.Min(1 - MageTalents.BurningSoul * 0.05f, 1 - ((MageTalents.FrostChanneling > 0) ? (0.01f + 0.03f * MageTalents.FrostChanneling) : 0f));
            NatureThreatMultiplier = threatFactor;
            ShadowThreatMultiplier = threatFactor;
            HolyThreatMultiplier = threatFactor;

            float baseSpellModifier = (1 + 0.01f * MageTalents.ArcaneInstability) * (1 + 0.01f * MageTalents.PlayingWithFire) * (1 + baseStats.BonusDamageMultiplier) * CalculationOptions.EffectDamageMultiplier;
            if (CalculationOptions.Beta)
            {
                baseSpellModifier *= (1 + (Math.Min(51, MaxTalents) * 0.15700000524521f) * 0.01f);
            }
            float baseAdditiveSpellModifier = 1.0f;
            BaseSpellModifier = baseSpellModifier;
            BaseAdditiveSpellModifier = baseAdditiveSpellModifier;
            BaseArcaneSpellModifier = baseSpellModifier * (1 + baseStats.BonusArcaneDamageMultiplier);
            BaseArcaneAdditiveSpellModifier = baseAdditiveSpellModifier;
            BaseFireSpellModifier = baseSpellModifier * (1 + baseStats.BonusFireDamageMultiplier);
            BaseFireAdditiveSpellModifier = baseAdditiveSpellModifier + 0.02f * MageTalents.FirePower;
            BaseFrostSpellModifier = baseSpellModifier * (1 + 0.02f * MageTalents.PiercingIce) * (1 + 0.01f * MageTalents.ArcticWinds) * (1 + baseStats.BonusFrostDamageMultiplier);
            BaseFrostAdditiveSpellModifier = baseAdditiveSpellModifier;
            BaseNatureSpellModifier = baseSpellModifier * (1 + baseStats.BonusNatureDamageMultiplier);
            BaseNatureAdditiveSpellModifier = baseAdditiveSpellModifier;
            BaseShadowSpellModifier = baseSpellModifier * (1 + baseStats.BonusShadowDamageMultiplier);
            BaseShadowAdditiveSpellModifier = baseAdditiveSpellModifier;
            BaseHolySpellModifier = baseSpellModifier * (1 + baseStats.BonusHolyDamageMultiplier);
            BaseHolyAdditiveSpellModifier = baseAdditiveSpellModifier;
            BaseFrostFireSpellModifier = baseSpellModifier * (1 + 0.02f * MageTalents.PiercingIce) * (1 + 0.01f * MageTalents.ArcticWinds) * Math.Max(1 + baseStats.BonusFireDamageMultiplier, 1 + baseStats.BonusFrostDamageMultiplier);
            BaseFrostFireAdditiveSpellModifier = baseAdditiveSpellModifier + 0.02f * MageTalents.FirePower;

            float spellCritPerInt = 0f;
            float spellCritBase = 0.9075f;
            float baseRegen = 0f;
            switch (playerLevel)
            {
                case 70:
                    spellCritPerInt = 0.0125f;
                    baseRegen = 0.005596f;
                    break;
                case 71:
                    spellCritPerInt = 0.0116f;
                    baseRegen = 0.005316f;
                    break;
                case 72:
                    spellCritPerInt = 0.0108f;
                    baseRegen = 0.005049f;
                    break;
                case 73:
                    spellCritPerInt = 0.0101f;
                    baseRegen = 0.004796f;
                    break;
                case 74:
                    spellCritPerInt = 0.0093f;
                    baseRegen = 0.004555f;
                    break;
                case 75:
                    spellCritPerInt = 0.0087f;
                    baseRegen = 0.004327f;
                    break;
                case 76:
                    spellCritPerInt = 0.0081f;
                    baseRegen = 0.004110f;
                    break;
                case 77:
                    spellCritPerInt = 0.0075f;
                    baseRegen = 0.003903f;
                    break;
                case 78:
                    spellCritPerInt = 0.007f;
                    baseRegen = 0.003708f;
                    break;
                case 79:
                    spellCritPerInt = 0.0065f;
                    baseRegen = 0.003522f;
                    break;
                case 80:
                    spellCritPerInt = 0.006f;
                    baseRegen = 0.003345f;
                    break;
                case 81:
                    spellCritPerInt = 0.0042f;
                    baseRegen = 0.003345f;
                    break;
                case 82:
                    //spellCritPerInt = 0.003f;
                    spellCritPerInt = 0.0029676f;  // 5/27/10
                    baseRegen = 0.003345f;
                    break;
                case 83:
                    spellCritPerInt = 0.003f;
                    baseRegen = 0.003345f;
                    break;
                case 84:
                    spellCritPerInt = 0.003f;
                    baseRegen = 0.003345f;
                    break;
                case 85:
                    spellCritPerInt = 0.003f;
                    baseRegen = 0.003345f;
                    break;
            }
            float levelScalingFactor = CalculationOptions.LevelScalingFactor;
            float spellCrit = 0.01f * (baseStats.Intellect * spellCritPerInt + spellCritBase) + 0.01f * MageTalents.ArcaneInstability + 0.15f * 0.02f * MageTalents.ArcaneConcentration * MageTalents.ArcanePotency + baseStats.CritRating / 1400f * levelScalingFactor + baseStats.SpellCrit + baseStats.SpellCritOnTarget + MageTalents.FocusMagic * 0.03f * (1 - (float)Math.Pow(1 - CalculationOptions.FocusMagicTargetCritRate, 10.0)) + 0.01f * MageTalents.Pyromaniac;
            if (CalculationOptions.Beta)
            {
                if (fire > arcane && fire >= frost)
                {
                    spellCrit += (1 + (Math.Min(51, MaxTalents) * 0.15700000524521f) * 0.01f);
                }
            }

            BaseCritRate = spellCrit;
            BaseArcaneCritRate = spellCrit;
            BaseFireCritRate = spellCrit + 0.02f * MageTalents.CriticalMass;
            BaseFrostFireCritRate = spellCrit + 0.02f * MageTalents.CriticalMass;
            BaseFrostCritRate = spellCrit;
            BaseNatureCritRate = spellCrit;
            BaseShadowCritRate = spellCrit;
            BaseHolyCritRate = spellCrit;

            if (!CalculationOptions.EffectDisableManaSources)
            {
                SpiritRegen = (0.001f + baseStats.Spirit * baseRegen * (float)Math.Sqrt(baseStats.Intellect)) * CalculationOptions.EffectRegenMultiplier;
                ManaRegen = SpiritRegen + baseStats.Mp5 / 5f + 15732 * CalculationOptions.Innervate / CalculationOptions.FightDuration + CalculationOptions.ManaTide * 0.24f * baseStats.Mana / CalculationOptions.FightDuration + baseStats.ManaRestoreFromMaxManaPerSecond * baseStats.Mana;
                ManaRegen5SR = SpiritRegen * baseStats.SpellCombatManaRegeneration + baseStats.Mp5 / 5f + 15732 * CalculationOptions.Innervate / CalculationOptions.FightDuration + CalculationOptions.ManaTide * 0.24f * baseStats.Mana / CalculationOptions.FightDuration + baseStats.ManaRestoreFromMaxManaPerSecond * baseStats.Mana;
            }
            else
            {
                SpiritRegen = 0;
                ManaRegen = 0;
                ManaRegen5SR = 0;
            }
            HealthRegen = 0.0312f * baseStats.Spirit + baseStats.Hp5 / 5f;
            HealthRegenCombat = baseStats.Hp5 / 5f;
            if (playerLevel < 75)
            {
                ManaRegenDrinking = ManaRegen + 240f;
                HealthRegenEating = HealthRegen + 250f;
            }
            else if (playerLevel < 80)
            {
                ManaRegenDrinking = ManaRegen + 306f;
                HealthRegenEating = HealthRegen + 440f;
            }
            else
            {
                ManaRegenDrinking = ManaRegen + 640f;
                HealthRegenEating = HealthRegen + 750f;
            }
            MeleeMitigation = (1 - 1 / (1 + 0.1f * baseStats.Armor / (8.5f * (targetLevel + 4.5f * (targetLevel - 59)) + 40)));
            Defense = 5 * playerLevel + baseStats.DefenseRating / 4.918498039f; // this is for level 80 only
            int molten = (armor == "Molten Armor") ? 1 : 0;
            float resilienceFactor = 2875f;
            PhysicalCritReduction = (0.04f * (Defense - 5 * CalculationOptions.PlayerLevel) / 100 + baseStats.Resilience / resilienceFactor * levelScalingFactor + molten * 0.05f);
            SpellCritReduction = (baseStats.Resilience / resilienceFactor * levelScalingFactor + molten * 0.05f);
            CritDamageReduction = (baseStats.Resilience / resilienceFactor * 2.2f * levelScalingFactor);
            DamageTakenReduction = baseStats.Resilience / resilienceFactor * levelScalingFactor;
            if (CalculationOptions.PVP)
            {
                DamageTakenReduction *= 2f;
            }
            Dodge = 0.043545f + 0.01f / (0.006650f + 0.953f / ((0.04f * (Defense - 5 * playerLevel)) / 100f + baseStats.DodgeRating / 1200 * levelScalingFactor + (baseStats.Agility - 46f) * 0.0195f));

            IgniteFactor = (1f - 0.02f * (float)Math.Max(0, targetLevel - playerLevel)) /* partial resist */ * 0.08f * MageTalents.Ignite;

            float mult = (1.5f * (1 + baseStats.BonusSpellCritMultiplier) - 1);
            float baseAddMult = (1 + 0.25f * MageTalents.SpellPower + 0.1f * MageTalents.Burnout + baseStats.CritBonusDamage);
            if (CalculationOptions.Beta)
            {
                if (frost > arcane && frost > fire)
                {
                    spellCrit += (1 + (Math.Min(51, MaxTalents) * 0.35299998521805f) * 0.01f);
                }
            }
            BaseArcaneCritBonus = (1 + mult * baseAddMult);
            BaseFireCritBonus = (1 + mult * baseAddMult) * (1 + IgniteFactor);
            BaseFrostCritBonus = (1 + mult * (baseAddMult + MageTalents.IceShards / 3.0f));
            BaseFrostFireCritBonus = (1 + mult * (baseAddMult + MageTalents.IceShards / 3.0f)) * (1 + IgniteFactor);
            BaseNatureCritBonus = 
            BaseShadowCritBonus =
            BaseHolyCritBonus = (1 + mult * baseAddMult); // unknown if affected by burnout

            float combustionCritBonus = 0.5f;

            CombustionFireCritBonus = (1 + (1.5f * (1 + baseStats.BonusSpellCritMultiplier) - 1) * (1 + combustionCritBonus + 0.25f * MageTalents.SpellPower + 0.1f * MageTalents.Burnout + baseStats.CritBonusDamage)) * (1 + IgniteFactor);
            CombustionFrostFireCritBonus = (1 + (1.5f * (1 + baseStats.BonusSpellCritMultiplier) - 1) * (1 + combustionCritBonus + MageTalents.IceShards / 3.0f + 0.25f * MageTalents.SpellPower + 0.1f * MageTalents.Burnout + baseStats.CritBonusDamage)) * (1 + IgniteFactor);

            CastingSpeedMultiplier = (1f + baseStats.SpellHaste) * (1f + 0.02f * MageTalents.NetherwindPresence) * CalculationOptions.EffectHasteMultiplier;
            if (CalculationOptions.Beta)
            {
                if (arcane >= fire && arcane >= frost)
                {
                    CastingSpeedMultiplier *= (1 + (Math.Min(51, MaxTalents) * 0.15700000524521f) * 0.01f);
                }
            }
            BaseCastingSpeed = (1 + baseStats.HasteRating / 1000f * levelScalingFactor) * CastingSpeedMultiplier;
            BaseGlobalCooldown = Math.Max(Spell.GlobalCooldownLimit, 1.5f / BaseCastingSpeed);

            IncomingDamageAmpMelee = (1 - 0.02f * MageTalents.PrismaticCloak) * (1 - 0.01f * MageTalents.ArcticWinds) * (1 - MeleeMitigation) * (1 - Dodge) * (1 - DamageTakenReduction);
            IncomingDamageAmpPhysical = (1 - 0.02f * MageTalents.PrismaticCloak) * (1 - 0.01f * MageTalents.ArcticWinds) * (1 - MeleeMitigation) * (1 - DamageTakenReduction);
            IncomingDamageAmpArcane = (1 - 0.02f * MageTalents.PrismaticCloak) * (1 + 0.01f * MageTalents.PlayingWithFire) * (1 - 0.02f * MageTalents.FrozenCore) * (1 - StatConversion.GetAverageResistance(targetLevel, playerLevel, baseStats.ArcaneResistance, 0)) * (1 - DamageTakenReduction);
            IncomingDamageAmpFire = (1 - 0.02f * MageTalents.PrismaticCloak) * (1 + 0.01f * MageTalents.PlayingWithFire) * (1 - 0.02f * MageTalents.FrozenCore) * (1 - StatConversion.GetAverageResistance(targetLevel, playerLevel, baseStats.FireResistance, 0)) * (1 - DamageTakenReduction);
            IncomingDamageAmpFrost = (1 - 0.02f * MageTalents.PrismaticCloak) * (1 + 0.01f * MageTalents.PlayingWithFire) * (1 - 0.02f * MageTalents.FrozenCore) * (1 - StatConversion.GetAverageResistance(targetLevel, playerLevel, baseStats.FrostResistance, 0)) * (1 - DamageTakenReduction);
            IncomingDamageAmpNature = (1 - 0.02f * MageTalents.PrismaticCloak) * (1 + 0.01f * MageTalents.PlayingWithFire) * (1 - 0.02f * MageTalents.FrozenCore) * (1 - StatConversion.GetAverageResistance(targetLevel, playerLevel, baseStats.NatureResistance, 0)) * (1 - DamageTakenReduction);
            IncomingDamageAmpShadow = (1 - 0.02f * MageTalents.PrismaticCloak) * (1 + 0.01f * MageTalents.PlayingWithFire) * (1 - 0.02f * MageTalents.FrozenCore) * (1 - StatConversion.GetAverageResistance(targetLevel, playerLevel, baseStats.ShadowResistance, 0)) * (1 - DamageTakenReduction);
            IncomingDamageAmpHoly = (1 - 0.02f * MageTalents.PrismaticCloak) * (1 + 0.01f * MageTalents.PlayingWithFire) * (1 - 0.02f * MageTalents.FrozenCore) * (1 - DamageTakenReduction);

            IncomingDamageDpsMelee = IncomingDamageAmpMelee * (CalculationOptions.MeleeDps * (1 + Math.Max(0, CalculationOptions.MeleeCrit / 100.0f - PhysicalCritReduction) * (2 * (1 - CritDamageReduction) - 1)) + CalculationOptions.MeleeDot);
            IncomingDamageDpsPhysical = IncomingDamageAmpPhysical * (CalculationOptions.PhysicalDps * (1 + Math.Max(0, CalculationOptions.PhysicalCrit / 100.0f - PhysicalCritReduction) * (2 * (1 - CritDamageReduction) - 1)) + CalculationOptions.PhysicalDot);
            IncomingDamageDpsArcane = IncomingDamageAmpArcane * (CalculationOptions.ArcaneDps * (1 + Math.Max(0, CalculationOptions.ArcaneCrit / 100.0f - SpellCritReduction) * (2 * (1 - CritDamageReduction) - 1)) + CalculationOptions.ArcaneDot);
            IncomingDamageDpsFire = IncomingDamageAmpFire * (CalculationOptions.FireDps * (1 + Math.Max(0, CalculationOptions.FireCrit / 100.0f - SpellCritReduction) * (2 * (1 - CritDamageReduction) - 1)) + CalculationOptions.FireDot);
            IncomingDamageDpsFrost = IncomingDamageAmpFrost * (CalculationOptions.FrostDps * (1 + Math.Max(0, CalculationOptions.FrostCrit / 100.0f - SpellCritReduction) * (2 * (1 - CritDamageReduction) - 1)) + CalculationOptions.FrostDot);
            IncomingDamageDpsNature = IncomingDamageAmpNature * (CalculationOptions.NatureDps * (1 + Math.Max(0, CalculationOptions.NatureCrit / 100.0f - SpellCritReduction) * (2 * (1 - CritDamageReduction) - 1)) + CalculationOptions.NatureDot);
            IncomingDamageDpsShadow = IncomingDamageAmpShadow * (CalculationOptions.ShadowDps * (1 + Math.Max(0, CalculationOptions.ShadowCrit / 100.0f - SpellCritReduction) * (2 * (1 - CritDamageReduction) - 1)) + CalculationOptions.ShadowDot);
            IncomingDamageDpsHoly = IncomingDamageAmpHoly * (CalculationOptions.HolyDps * (1 + Math.Max(0, CalculationOptions.HolyCrit / 100.0f - SpellCritReduction) * (2 * (1 - CritDamageReduction) - 1)) + CalculationOptions.HolyDot);

            IncomingDamageDps = IncomingDamageDpsMelee + IncomingDamageDpsPhysical + IncomingDamageDpsArcane + IncomingDamageDpsFire + IncomingDamageDpsFrost + IncomingDamageDpsShadow + IncomingDamageDpsNature + IncomingDamageDpsHoly;
            //float incanterSpellPower = Math.Min((float)Math.Min(calculationOptions.AbsorptionPerSecond, calculationResult.IncomingDamageDps) * 0.05f * talents.IncantersAbsorption * 10, 0.05f * baseStats.Health);
            if (CalculationOptions.AbsorptionPerSecond > IncomingDamageDps)
            {
                IncomingDamageDps = 0.0f;
            }
            else
            {
                IncomingDamageDps -= CalculationOptions.AbsorptionPerSecond;
            }

            BaseArcaneSpellPower = baseStats.SpellArcaneDamageRating + baseStats.SpellPower;
            BaseFireSpellPower = baseStats.SpellFireDamageRating + baseStats.SpellPower;
            BaseFrostSpellPower = baseStats.SpellFrostDamageRating + baseStats.SpellPower;
            BaseNatureSpellPower = baseStats.SpellNatureDamageRating + baseStats.SpellPower;
            BaseShadowSpellPower = baseStats.SpellShadowDamageRating + baseStats.SpellPower;
            BaseHolySpellPower = /* baseStats.SpellHolyDamageRating + */ baseStats.SpellPower;

            Mastery = baseStats.MasteryRating / 14 * levelScalingFactor;
            ManaAdeptBonus = 0.0f;
            if (CalculationOptions.Beta)
            {
                if (arcane >= fire && arcane >= frost)
                {
                    ManaAdeptBonus = (0.23600000143051f * Math.Min(51, arcane) + 1.5f * Mastery) * 0.01f; // 0.12036
                    needsQuadratic = true;
                    needsSolutionVariables = true;
                }
            }
        }

        private void InitializeSpellTemplates()
        {
            if (_WaterboltTemplate != null) _WaterboltTemplate.Dirty = true;
            if (_MirrorImageTemplate != null) _MirrorImageTemplate.Dirty = true;
            if (_FireBlastTemplate != null) _FireBlastTemplate.Dirty = true;
            if (_LightningBoltTemplate != null) _LightningBoltTemplate.Dirty = true;
            if (_ThunderBoltTemplate != null) _ThunderBoltTemplate.Dirty = true;
            if (_LightweaveBoltTemplate != null) _LightweaveBoltTemplate.Dirty = true;
            if (_ArcaneBoltTemplate != null) _ArcaneBoltTemplate.Dirty = true;
            if (_PendulumOfTelluricCurrentsTemplate != null) _PendulumOfTelluricCurrentsTemplate.Dirty = true;
            if (_FrostboltTemplate != null) _FrostboltTemplate.Dirty = true;
            if (_FrostfireBoltTemplate != null) _FrostfireBoltTemplate.Dirty = true;
            if (_ArcaneMissilesTemplate != null) _ArcaneMissilesTemplate.Dirty = true;
            if (_FireballTemplate != null) _FireballTemplate.Dirty = true;
            if (_PyroblastTemplate != null) _PyroblastTemplate.Dirty = true;
            if (_ScorchTemplate != null) _ScorchTemplate.Dirty = true;
            if (_ArcaneBarrageTemplate != null) _ArcaneBarrageTemplate.Dirty = true;
            if (_DeepFreezeTemplate != null) _DeepFreezeTemplate.Dirty = true;
            if (_ArcaneBlastTemplate != null) _ArcaneBlastTemplate.Dirty = true;
            if (_IceLanceTemplate != null) _IceLanceTemplate.Dirty = true;
            if (_ArcaneExplosionTemplate != null) _ArcaneExplosionTemplate.Dirty = true;
            if (_FlamestrikeTemplate != null) _FlamestrikeTemplate.Dirty = true;
            if (_BlizzardTemplate != null) _BlizzardTemplate.Dirty = true;
            if (_BlastWaveTemplate != null) _BlastWaveTemplate.Dirty = true;
            if (_DragonsBreathTemplate != null) _DragonsBreathTemplate.Dirty = true;
            if (_ConeOfColdTemplate != null) _ConeOfColdTemplate.Dirty = true;
            if (_SlowTemplate != null) _SlowTemplate.Dirty = true;
            if (_LivingBombTemplate != null) _LivingBombTemplate.Dirty = true;
            if (_FireWardTemplate != null) _FireWardTemplate.Dirty = true;
            if (_FrostWardTemplate != null) _FrostWardTemplate.Dirty = true;
            if (_ConjureManaGemTemplate != null) _ConjureManaGemTemplate.Dirty = true;
            if (_ArcaneDamageTemplate != null) _ArcaneDamageTemplate.Dirty = true;
            if (_FireDamageTemplate != null) _FireDamageTemplate.Dirty = true;
            if (_FrostDamageTemplate != null) _FrostDamageTemplate.Dirty = true;
            if (_ShadowDamageTemplate != null) _ShadowDamageTemplate.Dirty = true;
            if (_NatureDamageTemplate != null) _NatureDamageTemplate.Dirty = true;
            if (_HolyDamageTemplate != null) _HolyDamageTemplate.Dirty = true;
            if (_ValkyrDamageTemplate != null) _ValkyrDamageTemplate.Dirty = true;
        }
        #endregion

        #region Construct Problem
        private void AddSegmentTicks(List<double> ticks, double cooldownDuration)
        {
            for (int i = 0; i * 0.5 * cooldownDuration < CalculationOptions.FightDuration; i++)
            {
                ticks.Add(i * 0.5 * cooldownDuration);
            }
        }

        private void AddEffectTicks(List<double> ticks, double cooldownDuration, double effectDuration)
        {
            for (int i = 0; i * cooldownDuration + effectDuration < CalculationOptions.FightDuration; i++)
            {
                ticks.Add(i * cooldownDuration + effectDuration);
                if (i * cooldownDuration + effectDuration > CalculationOptions.FightDuration - effectDuration)
                {
                    ticks.Add(CalculationOptions.FightDuration - effectDuration);
                }
            }
        }

#if SILVERLIGHT
        private void ConstructProblem()
#else
        private unsafe void ConstructProblem()
#endif
        {
            Stats baseStats = BaseStats;

            ConstructSegments();

            //segments = (segmentCooldowns) ? (int)Math.Ceiling(calculationOptions.FightDuration / segmentDuration) : 1;
            if (requiresMIP)
            {
                segmentColumn = new int[SegmentList.Count + 1];
            }

            StartingMana = Math.Min(baseStats.Mana, BaseState.ManaRegenDrinking * CalculationOptions.DrinkingTime);
            double maxDrinkingTime = Math.Min(30, (baseStats.Mana - StartingMana) / BaseState.ManaRegenDrinking);
            bool drinkingEnabled = (maxDrinkingTime > 0.000001);

            needsTimeExtension = false;
            bool afterFightRegen = CalculationOptions.FarmingMode;
            conjureManaGem = CalculationOptions.ManaGemEnabled && CalculationOptions.FightDuration > 500.0f;
            //wardsAvailable = calculationResult.IncomingDamageDpsFire + calculationResult.IncomingDamageDpsFrost > 0.0 && talents.FrostWarding > 0;

            minimizeTime = false;
            if (CalculationOptions.TargetDamage > 0)
            {
                if (!needsQuadratic)
                {
                    minimizeTime = true;
                }
                needsTimeExtension = true;
            }

            restrictManaUse = false;
            if (segmentCooldowns || segmentMana) restrictManaUse = true;
            if (CalculationOptions.UnlimitedMana)
            {
                restrictManaUse = false;
                integralMana = false;
                segmentMana = false;
            }
            segmentNonCooldowns = false;
            if (segmentCooldowns)
            {
                if (restrictManaUse) segmentNonCooldowns = true;
                if (restrictThreat) segmentNonCooldowns = true;
            }

            dpsTime = CalculationOptions.DpsTime;
            float silenceTime = CalculationOptions.EffectShadowSilenceFrequency * CalculationOptions.EffectShadowSilenceDuration * Math.Max(1 - baseStats.ShadowResistance / CalculationOptions.TargetLevel * 0.15f, 0.25f);
            if (1 - silenceTime < dpsTime) dpsTime = 1 - silenceTime;
            if (CalculationOptions.MovementFrequency > 0)
            {
                float movementShare = CalculationOptions.MovementDuration / CalculationOptions.MovementFrequency / (1 + baseStats.MovementSpeed);
                dpsTime -= movementShare;
            }

            if (segmentMana)
            {
                if (segmentCooldowns)
                {
                    manaSegments = 2;
                }
                else
                {
                    manaSegments = (int)Math.Ceiling(CalculationOptions.FightDuration / EvocationCooldown) + 1;
                }
            }
            else
            {
                manaSegments = 1;
            }

            int rowCount = ConstructRows(minimizeTime, drinkingEnabled, needsTimeExtension, afterFightRegen);

            if (lp == null)
            {
                lp = new SolverLP();
            }
            lp.Initialize(ArraySet, rowCount, 9 + (12 + (CalculationOptions.EnableHastedEvocation ? 6 : 0) + spellList.Count * stateList.Count * (1 + (CalculationOptions.UseFireWard ? 1 : 0) + (CalculationOptions.UseFrostWard ? 1 : 0))) * manaSegments * SegmentList.Count, this, SegmentList.Count);

            SetCalculationReuseReferences();
            AddWardStates();

            if (needsSolutionVariables)
            {
                SolutionVariable = new List<SolutionVariable>();
            }

#if !SILVERLIGHT
            fixed (double* pRowScale = lp.ArraySet.rowScale, pColumnScale = lp.ArraySet.columnScale, pCost = lp.ArraySet._cost, pData = lp.ArraySet.SparseMatrixData, pValue = lp.ArraySet.SparseMatrixValue)
            fixed (int* pRow = lp.ArraySet.SparseMatrixRow, pCol = lp.ArraySet.SparseMatrixCol)
#endif
            {
#if SILVERLIGHT
                lp.BeginSafe(lp.ArraySet.rowScale, lp.ArraySet.columnScale, lp.ArraySet._cost, lp.ArraySet.SparseMatrixData, lp.ArraySet.SparseMatrixValue, lp.ArraySet.SparseMatrixRow, lp.ArraySet.SparseMatrixCol);
#else
                lp.BeginUnsafe(pRowScale, pColumnScale, pCost, pData, pValue, pRow, pCol);
#endif

                #region Set LP Scaling
                lp.SetRowScaleUnsafe(rowManaRegen, ManaRegenLPScaling);
                lp.SetRowScaleUnsafe(rowManaGem, 40.0);
                lp.SetRowScaleUnsafe(rowPotion, 40.0);
                lp.SetRowScaleUnsafe(rowManaGemMax, 40.0);
                lp.SetRowScaleUnsafe(rowManaPotion, 40.0);
                lp.SetRowScaleUnsafe(rowCombustion, 10.0);
                lp.SetRowScaleUnsafe(rowHeroismCombustion, 10.0);
                lp.SetRowScaleUnsafe(rowMoltenFuryCombustion, 10.0);
                lp.SetRowScaleUnsafe(rowThreat, 0.001);
                lp.SetRowScaleUnsafe(rowCount, 0.05);
                if (restrictManaUse)
                {
                    for (int ss = 0; ss < manaSegments * SegmentList.Count - 1; ss++)
                    {
                        lp.SetRowScaleUnsafe(rowSegmentManaOverflow + ss, ManaRegenLPScaling);
                        lp.SetRowScaleUnsafe(rowSegmentManaUnderflow + ss, ManaRegenLPScaling);
                    }
                }
                if (restrictThreat)
                {
                    for (int ss = 0; ss < SegmentList.Count - 1; ss++)
                    {
                        lp.SetRowScaleUnsafe(rowSegmentThreat + ss, 0.001);
                    }
                }
                #endregion

                float threatFactor = (1 + baseStats.ThreatIncreaseMultiplier) * (1 - baseStats.ThreatReductionMultiplier);

                ConstructIdleRegen();
                ConstructWand();
                ConstructEvocation(baseStats, threatFactor);
                ConstructManaPotion(baseStats, threatFactor);
                ConstructManaGem(baseStats, threatFactor);
                ConstructSummonWaterElemental();
                ConstructSummonMirrorImage();
                ConstructDrinking(maxDrinkingTime, drinkingEnabled);
                ConstructTimeExtension();
                ConstructAfterFightRegen(afterFightRegen);
                ConstructManaOverflow();
                ConstructConjureManaGem();
                ConstructSpells();

                lp.EndColumnConstruction();
                SetProblemRHS();

                lp.EndUnsafe();
            }
        }

        private void ConstructSpells()
        {
            int column = 0;
            if (useIncrementalOptimizations)
            {
                int lastSegment = -1;
                for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                {
                    if (CalculationOptions.IncrementalSetVariableType[index] == VariableType.Spell)
                    {
                        for (int buffset = 0; buffset < stateList.Count; buffset++)
                        {
                            CastingState state = stateList[buffset];
                            if ((state.Effects & (int)StandardEffect.NonItemBasedMask) == CalculationOptions.IncrementalSetStateIndexes[index])
                            {
                                if (CalculationOptions.CooldownRestrictionsValid(SegmentList[CalculationOptions.IncrementalSetSegments[index]], state))
                                {
                                    float mult = segmentCooldowns ? CalculationOptions.GetDamageMultiplier(SegmentList[CalculationOptions.IncrementalSetSegments[index]]) : 1.0f;
                                    if (mult > 0)
                                    {
                                        Cycle c = state.GetCycle(CalculationOptions.IncrementalSetSpells[index]);
                                        int seg = CalculationOptions.IncrementalSetSegments[index];
                                        int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                                        column = lp.AddColumnUnsafe();
                                        if (requiresMIP)
                                        {
                                            if (seg != lastSegment)
                                            {
                                                for (; lastSegment < seg; )
                                                {
                                                    segmentColumn[++lastSegment] = column;
                                                }
                                            }
                                        }
                                        if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { State = state, Cycle = c, Segment = seg, ManaSegment = manaSegment, Type = VariableType.Spell, Dps = c.DamagePerSecond, Mps = c.ManaPerSecond, Tps = c.ThreatPerSecond });
                                        SetSpellColumn(minimizeTime, seg, manaSegment, state, column, c, mult);
                                    }
                                }
                            }
                        }
                    }
                }
                if (requiresMIP)
                {
                    for (; lastSegment < SegmentList.Count; )
                    {
                        segmentColumn[++lastSegment] = column + 1;
                    }
                }
            }
            else
            {
                float mfMin = CalculationOptions.FightDuration * (1.0f - CalculationOptions.MoltenFuryPercentage) + 0.00001f;
                float heroMin = Math.Min(mfMin, CalculationOptions.FightDuration - 40.0f + 0.00001f);
                int firstMoltenFurySegment = SegmentList.FindIndex(s => s.TimeEnd > mfMin);
                int firstHeroismSegment = SegmentList.FindIndex(s => s.TimeEnd > heroMin);

                List<Cycle> placed = new List<Cycle>();
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    if (requiresMIP)
                    {
                        segmentColumn[seg] = lp.Columns;
                    }
                    for (int buffset = 0; buffset < stateList.Count; buffset++)
                    {
                        CastingState state = stateList[buffset];
                        if (CalculationOptions.CooldownRestrictionsValid(SegmentList[seg], state))
                        {
                            float mult = segmentCooldowns ? CalculationOptions.GetDamageMultiplier(SegmentList[seg]) : 1.0f;
                            if (mult > 0)
                            {
                                placed.Clear();
                                for (int spell = 0; spell < spellList.Count; spell++)
                                {
                                    if (segmentCooldowns && moltenFuryAvailable && state.MoltenFury && seg < firstMoltenFurySegment) continue;
                                    if (segmentCooldowns && moltenFuryAvailable && !state.MoltenFury && seg >= firstMoltenFurySegment) continue;
                                    if (!segmentNonCooldowns && state == BaseState && seg != 0) continue;
                                    if (segmentCooldowns && CalculationOptions.HeroismControl == 3 && state.Heroism && seg < firstHeroismSegment) continue;
                                    Cycle c = state.GetCycle(spellList[spell]);
                                    bool skip = false;
                                    foreach (Cycle s2 in placed)
                                    {
                                        // TODO verify it this is ok, it assumes that spells placed under same casting state are independent except for aoe spells
                                        // assuming there are no constraints that depend on properties of particular spell cycle instead of properties of casting state
                                        if (!c.AreaEffect && s2.DamagePerSecond >= c.DamagePerSecond - 0.00001 && s2.ManaPerSecond <= c.ManaPerSecond + 0.00001)
                                        {
                                            skip = true;
                                            break;
                                        }
                                    }
                                    if (!skip)
                                    {
                                        placed.Add(c);
                                        //for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                                        for (int manaSegment = manaSegments - 1; manaSegment >= 0; manaSegment--)
                                        {
                                            column = lp.AddColumnUnsafe();
                                            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { State = state, Cycle = c, Segment = seg, ManaSegment = manaSegment, Type = VariableType.Spell, Dps = c.DamagePerSecond, Mps = c.ManaPerSecond, Tps = c.ThreatPerSecond });
                                            SetSpellColumn(minimizeTime, seg, manaSegment, state, column, c, mult);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (requiresMIP)
                {
                    segmentColumn[SegmentList.Count] = column + 1;
                }
            }
        }

        private void ConstructConjureManaGem()
        {
            if (conjureManaGem)
            {
                int conjureSegments = (restrictManaUse) ? SegmentList.Count : 1;
                Cycle spell = ConjureManaGemTemplate.GetSpell(BaseState);
                ConjureManaGem = spell;
                MaxConjureManaGem = (int)((CalculationOptions.FightDuration - 300.0f) / 360.0f) + 1;
                double mps = spell.ManaPerSecond;
                double dps = 0.0;
                double tps = spell.ThreatPerSecond;
                for (int segment = 0; segment < conjureSegments; segment++)
                {
                    for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                    {
                        int column = lp.AddColumnUnsafe();
                        lp.SetColumnUpperBound(column, spell.CastTime * ((conjureSegments > 1) ? 1 : MaxConjureManaGem));
                        if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.ConjureManaGem, Cycle = spell, Segment = segment, ManaSegment = manaSegment, State = BaseState, Dps = dps, Tps = tps, Mps = mps });
                        lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
                        lp.SetElementUnsafe(rowManaRegen, column, mps);
                        lp.SetElementUnsafe(rowConjureManaGem, column, 1.0);
                        lp.SetElementUnsafe(rowFightDuration, column, 1.0);
                        lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
                        lp.SetElementUnsafe(rowThreat, column, tps = spell.ThreatPerSecond);
                        lp.SetElementUnsafe(rowTargetDamage, column, -spell.DamagePerSecond);
                        lp.SetCostUnsafe(column, minimizeTime ? -1 : spell.DamagePerSecond);
                        lp.SetElementUnsafe(rowManaGem, column, -3.0 / spell.CastTime); // one cast time gives 3 new gem uses
                        if (segmentNonCooldowns) lp.SetElementUnsafe(rowSegment + segment, column, 1.0);
                        if (restrictManaUse)
                        {
                            for (int ss = segment * manaSegments + manaSegment; ss < SegmentList.Count * manaSegments - 1; ss++)
                            {
                                lp.SetElementUnsafe(rowSegmentManaUnderflow + ss, column, mps);
                                lp.SetElementUnsafe(rowSegmentManaOverflow + ss, column, -mps);
                            }
                        }
                        if (restrictThreat)
                        {
                            for (int ss = segment; ss < SegmentList.Count - 1; ss++)
                            {
                                lp.SetElementUnsafe(rowSegmentThreat + ss, column, tps);
                            }
                        }
                    }
                }
            }
            else
            {
                ConjureManaGem = null;
                MaxConjureManaGem = 0;
            }
        }

        private void ConstructManaOverflow()
        {
            if (restrictManaUse)
            {
                if (useIncrementalOptimizations)
                {
                    for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                    {
                        if (CalculationOptions.IncrementalSetVariableType[index] == VariableType.ManaOverflow)
                        {
                            int segment = CalculationOptions.IncrementalSetSegments[index];
                            int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                            SetManaOverflowColumn(segment, manaSegment);
                        }
                    }
                }
                else
                {
                    for (int segment = 0; segment < SegmentList.Count; segment++)
                    {
                        for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                        {
                            SetManaOverflowColumn(segment, manaSegment);
                        }
                    }
                }
            }
        }

        private void SetManaOverflowColumn(int segment, int manaSegment)
        {
            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.ManaOverflow, Segment = segment, ManaSegment = manaSegment });
            int column = lp.AddColumnUnsafe();
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, 1.0);
            lp.SetElementUnsafe(rowManaRegen, column, 1.0);
            for (int ss = segment * manaSegments + manaSegment; ss < SegmentList.Count * manaSegments - 1; ss++)
            {
                lp.SetElementUnsafe(rowSegmentManaUnderflow + ss, column, 1.0);
                lp.SetElementUnsafe(rowSegmentManaOverflow + ss, column, -1.0);
            }
        }

        private void ConstructAfterFightRegen(bool afterFightRegen)
        {
            if (afterFightRegen)
            {
                if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.AfterFightRegen });
                int column = lp.AddColumnUnsafe();
                lp.SetColumnUpperBound(column, CalculationOptions.FightDuration);
                lp.SetElementUnsafe(rowFightDuration, column, 1.0);
                lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
                lp.SetElementUnsafe(rowAfterFightRegenMana, column, -BaseState.ManaRegenDrinking);
                lp.SetCostUnsafe(column, minimizeTime ? -1 : 0);
            }
        }

        private void ConstructTimeExtension()
        {
            if (needsTimeExtension)
            {
                if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.TimeExtension });
                int column = lp.AddColumnUnsafe();
                lp.SetColumnUpperBound(column, CalculationOptions.FightDuration);
                if (!needsQuadratic)
                {
                    lp.SetElementUnsafe(rowFightDuration, column, 1.0);
                }
                lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
                lp.SetElementUnsafe(rowEvocation, column, EvocationDuration / EvocationCooldown);
                //lp.SetElementUnsafe(rowPotion, column, 1.0 / 120.0);
                lp.SetElementUnsafe(rowManaGem, column, 1.0 / 120.0);
                lp.SetElementUnsafe(rowPowerInfusion, column, PowerInfusionDuration / PowerInfusionCooldown);
                lp.SetElementUnsafe(rowArcanePower, column, ArcanePowerDuration / ArcanePowerCooldown);
                lp.SetElementUnsafe(rowIcyVeins, column, 20.0 / IcyVeinsCooldown + (coldsnapAvailable ? 20.0 / ColdsnapCooldown : 0.0));
                lp.SetElementUnsafe(rowMoltenFury, column, CalculationOptions.MoltenFuryPercentage);
                lp.SetElementUnsafe(rowFlameCap, column, 60f / 180f);
                for (int i = 0; i < ItemBasedEffectCooldownsCount; i++)
                {
                    EffectCooldown cooldown = ItemBasedEffectCooldowns[i];
                    lp.SetElementUnsafe(cooldown.Row, column, cooldown.Duration / cooldown.Cooldown);
                }
                lp.SetElementUnsafe(rowManaGemEffect, column, ManaGemEffectDuration / 120f);
                lp.SetElementUnsafe(rowDpsTime, column, -(1 - dpsTime));
                lp.SetElementUnsafe(rowAoe, column, CalculationOptions.AoeDuration);
                lp.SetElementUnsafe(rowCombustion, column, 1.0 / CombustionCooldown);
                lp.SetElementUnsafe(rowBerserking, column, 10.0 / 180.0);
            }
        }

        private void ConstructDrinking(double maxDrinkingTime, bool drinkingEnabled)
        {
            if (drinkingEnabled)
            {
                double mps = -BaseState.ManaRegenDrinking;
                double dps = 0.0f;
                double tps = 0.0f;
                if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.Drinking, Dps = dps, Mps = mps, Tps = tps });
                int column = lp.AddColumnUnsafe();
                lp.SetColumnUpperBound(column, maxDrinkingTime);
                lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
                lp.SetElementUnsafe(rowManaRegen, column, mps);
                lp.SetElementUnsafe(rowFightDuration, column, 1.0);
                lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
                lp.SetCostUnsafe(column, minimizeTime ? -1 : 0);
                if (segmentNonCooldowns) lp.SetElementUnsafe(rowSegment + 0, column, 1.0);
                if (restrictManaUse)
                {
                    for (int ss = 0; ss < SegmentList.Count * manaSegments - 1; ss++)
                    {
                        lp.SetElementUnsafe(rowSegmentManaUnderflow + ss, column, mps);
                        lp.SetElementUnsafe(rowSegmentManaOverflow + ss, column, -mps);
                    }
                }
            }
        }

        private void ConstructSummonMirrorImage()
        {
            if (mirrorImageAvailable)
            {
                int mirrorImageSegments = SegmentList.Count; // always segment, we need it to guarantee each block has activation
                double mps = (int)(0.10 * SpellTemplate.BaseMana[CalculationOptions.PlayerLevel]) / BaseGlobalCooldown - BaseState.ManaRegen5SR;
                scratchStateList.Clear();
                bool found = false;
                for (int i = 0; i < stateList.Count; i++)
                {
                    if (stateList[i].Effects == (int)StandardEffect.MirrorImage)
                    {
                        scratchStateList.Add(stateList[i]);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    scratchStateList.Add(CastingState.New(this, (int)StandardEffect.MirrorImage, false, 0));
                }
                if (useIncrementalOptimizations)
                {
                    for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                    {
                        if (CalculationOptions.IncrementalSetVariableType[index] == VariableType.SummonMirrorImage)
                        {
                            int segment = CalculationOptions.IncrementalSetSegments[index];
                            int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                            foreach (CastingState state in scratchStateList)
                            {
                                SetSummonMirrorImageColumn(mirrorImageSegments, mps, segment, manaSegment, state);
                            }
                        }
                    }
                }
                else
                {
                    for (int segment = 0; segment < mirrorImageSegments; segment++)
                    {
                        for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                        {
                            foreach (CastingState state in scratchStateList)
                            {
                                SetSummonMirrorImageColumn(mirrorImageSegments, mps, segment, manaSegment, state);
                            }
                        }
                    }
                }
            }
        }

        private void SetSummonMirrorImageColumn(int mirrorImageSegments, double mps, int segment, int manaSegment, CastingState state)
        {
            Spell mirrorImage = state.GetSpell(SpellId.MirrorImage);
            double dps = mirrorImage.DamagePerSecond;
            double tps = 0.0;
            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.SummonMirrorImage, Segment = segment, ManaSegment = manaSegment, State = state, Dps = dps, Mps = mps, Tps = tps });
            int column = lp.AddColumnUnsafe();
            if (mirrorImageSegments > 1) lp.SetColumnUpperBound(column, BaseGlobalCooldown);
            //if (segment == 0 && state == states[0]) calculationResult.ColumnSummonWaterElemental = column;
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
            lp.SetElementUnsafe(rowManaRegen, column, mps);
            lp.SetElementUnsafe(rowFightDuration, column, 1.0);
            lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
            lp.SetElementUnsafe(rowSummonMirrorImage, column, -1 / BaseGlobalCooldown);
            lp.SetElementUnsafe(rowSummonMirrorImageCount, column, 1.0);
            lp.SetElementUnsafe(rowMirrorImage, column, 1.0);
            lp.SetCostUnsafe(column, minimizeTime ? -1 : mirrorImage.DamagePerSecond);
            lp.SetElementUnsafe(rowSegment + segment, column, 1.0);
            if (restrictManaUse)
            {
                for (int ss = segment * manaSegments + manaSegment; ss < SegmentList.Count * manaSegments - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentManaUnderflow + ss, column, mps);
                    lp.SetElementUnsafe(rowSegmentManaOverflow + ss, column, -mps);
                }
            }
            if (segmentCooldowns)
            {
                foreach (SegmentConstraint constraint in rowSegmentMirrorImage)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
                foreach (SegmentConstraint constraint in rowSegmentSummonMirrorImage)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
        }

        private void ConstructSummonWaterElemental()
        {
            if (waterElementalAvailable && !MageTalents.GlyphOfEternalWater)
            {
                int waterElementalSegments = SegmentList.Count; // always segment, we need it to guarantee each block has activation
                double mps = (int)(0.16 * SpellTemplate.BaseMana[CalculationOptions.PlayerLevel]) / BaseGlobalCooldown - BaseState.ManaRegen5SR;
                scratchStateList.Clear();
                bool found = false;
                // WE = 0x100
                for (int i = 0; i < stateList.Count; i++)
                {
                    if (stateList[i].Effects == (int)StandardEffect.WaterElemental)
                    {
                        scratchStateList.Add(stateList[i]);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    scratchStateList.Add(CastingState.New(this, (int)StandardEffect.WaterElemental, false, 0));
                }
                if (useIncrementalOptimizations)
                {
                    for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                    {
                        if (CalculationOptions.IncrementalSetVariableType[index] == VariableType.SummonWaterElemental)
                        {
                            int segment = CalculationOptions.IncrementalSetSegments[index];
                            int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                            foreach (CastingState state in scratchStateList)
                            {
                                SetSummonWaterElementalColumn(waterElementalSegments, mps, segment, manaSegment, state);
                            }
                        }
                    }
                }
                else
                {
                    for (int segment = 0; segment < waterElementalSegments; segment++)
                    {
                        for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                        {
                            foreach (CastingState state in scratchStateList)
                            {
                                SetSummonWaterElementalColumn(waterElementalSegments, mps, segment, manaSegment, state);
                            }
                        }
                    }
                }
            }
        }

        private void SetSummonWaterElementalColumn(int waterElementalSegments, double mps, int segment, int manaSegment, CastingState state)
        {
            Spell waterbolt = state.GetSpell(SpellId.Waterbolt);
            double dps = waterbolt.DamagePerSecond;
            double tps = 0.0;
            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.SummonWaterElemental, Segment = segment, ManaSegment = manaSegment, State = state, Dps = dps, Mps = mps, Tps = tps });
            int column = lp.AddColumnUnsafe();
            if (waterElementalSegments > 1) lp.SetColumnUpperBound(column, BaseGlobalCooldown);
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
            lp.SetElementUnsafe(rowManaRegen, column, mps);
            lp.SetElementUnsafe(rowFightDuration, column, 1.0);
            lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
            if (!MageTalents.GlyphOfEternalWater)
            {
                lp.SetElementUnsafe(rowSummonWaterElemental, column, -1 / BaseGlobalCooldown);
                lp.SetElementUnsafe(rowSummonWaterElementalCount, column, 1.0);
                lp.SetElementUnsafe(rowWaterElemental, column, 1.0);
            }
            lp.SetCostUnsafe(column, minimizeTime ? -1 : waterbolt.DamagePerSecond);
            lp.SetElementUnsafe(rowSegment + segment, column, 1.0);
            if (restrictManaUse)
            {
                for (int ss = segment; ss < SegmentList.Count - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentManaUnderflow + ss, column, mps);
                    lp.SetElementUnsafe(rowSegmentManaOverflow + ss, column, -mps);
                }
            }
            if (segmentCooldowns)
            {
                foreach (SegmentConstraint constraint in rowSegmentWaterElemental)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
                foreach (SegmentConstraint constraint in rowSegmentSummonWaterElemental)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
        }

        private void ConstructManaGem(Stats baseStats, float threatFactor)
        {
            if (CalculationOptions.ManaGemEnabled)
            {
                int manaGemSegments = (segmentCooldowns && restrictManaUse) ? SegmentList.Count : 1;
                if (segmentCooldowns && advancedConstraintsLevel >= 3)
                {
                    MaxManaGem = 1 + (int)((CalculationOptions.FightDuration - 1f) / 120f);
                }
                else
                {
                    MaxManaGem = 1 + (int)((CalculationOptions.FightDuration - 30f) / 120f);
                }
                double mps = -(1 + baseStats.BonusManaGem) * ManaGemValue;
                double tps = -mps * 0.5f * threatFactor;
                double dps = 0.0f;
                double upperBound;
                if (manaGemSegments > 1)
                {
                    upperBound = 1.0;
                }
                else
                {
                    if (needsTimeExtension || conjureManaGem)
                    {
                        upperBound = MaxManaGem;
                    }
                    else
                    {
                        upperBound = Math.Min(3, CalculationOptions.AverageCooldowns ? CalculationOptions.FightDuration / 120.0 : MaxManaGem);
                    }
                }
                if (useIncrementalOptimizations)
                {
                    for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                    {
                        if (CalculationOptions.IncrementalSetVariableType[index] == VariableType.ManaGem)
                        {
                            int segment = CalculationOptions.IncrementalSetSegments[index];
                            int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                            SetManaGemColumn(mps, tps, dps, upperBound, segment, manaSegment);
                        }
                    }
                }
                else
                {
                    for (int segment = 0; segment < manaGemSegments; segment++)
                    {
                        for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                        {
                            SetManaGemColumn(mps, tps, dps, upperBound, segment, manaSegment);
                        }
                    }
                }
            }
            else
            {
                MaxManaGem = 0;
                ManaGemTps = 0;
            }
        }

        private void SetManaGemColumn(double mps, double tps, double dps, double upperBound, int segment, int manaSegment)
        {
            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.ManaGem, Segment = segment, ManaSegment = manaSegment, Dps = dps, Mps = mps, Tps = tps });
            int column = lp.AddColumnUnsafe();
            lp.SetColumnScaleUnsafe(column, 1.0 / 40.0);
            lp.SetColumnUpperBound(column, upperBound);
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
            lp.SetElementUnsafe(rowManaRegen, column, mps);
            lp.SetElementUnsafe(rowManaGem, column, 1.0);
            lp.SetElementUnsafe(rowManaGemMax, column, 1.0);
            //lp.SetElementUnsafe(rowFlameCap, column, 1.0);
            lp.SetElementUnsafe(rowManaGemEffectActivation, column, -1.0);
            lp.SetElementUnsafe(rowThreat, column, tps);
            ManaGemTps = (float)tps;
            //lp.SetElementUnsafe(rowManaPotionManaGem, column, 40.0);
            lp.SetCostUnsafe(column, 0.0);
            if (segmentCooldowns)
            {
                foreach (SegmentConstraint constraint in rowSegmentManaGem)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (restrictManaUse)
            {
                for (int ss = segment * manaSegments + manaSegment; ss < SegmentList.Count * manaSegments - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentManaUnderflow + ss, column, mps);
                    lp.SetElementUnsafe(rowSegmentManaOverflow + ss, column, -mps);
                }
            }
            if (restrictThreat)
            {
                for (int ss = segment; ss < SegmentList.Count - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentThreat + ss, column, tps);
                }
            }
        }

        private void ConstructManaPotion(Stats baseStats, float threatFactor)
        {
            if (manaPotionAvailable)
            {
                int manaPotionSegments = (segmentCooldowns && (potionOfWildMagicAvailable || restrictManaUse)) ? SegmentList.Count : 1;
                double mps = -(1 + baseStats.BonusManaPotion) * ManaPotionValue;
                double dps = 0;
                double tps = (1 + baseStats.BonusManaPotion) * ManaPotionValue * 0.5f * threatFactor;
                if (useIncrementalOptimizations)
                {
                    for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                    {
                        if (CalculationOptions.IncrementalSetVariableType[index] == VariableType.ManaPotion)
                        {
                            int segment = CalculationOptions.IncrementalSetSegments[index];
                            int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                            SetManaPotionColumn(mps, dps, tps, segment, manaSegment);
                        }
                    }
                }
                else
                {
                    for (int segment = 0; segment < manaPotionSegments; segment++)
                    {
                        for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                        {
                            SetManaPotionColumn(mps, dps, tps, segment, manaSegment);
                        }
                    }
                }
            }
            else
            {
                ManaPotionTps = 0;
            }
        }

        private void SetManaPotionColumn(double mps, double dps, double tps, int segment, int manaSegment)
        {
            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.ManaPotion, Segment = segment, ManaSegment = manaSegment, Dps = dps, Mps = mps, Tps = tps });
            int column = lp.AddColumnUnsafe();
            lp.SetColumnScaleUnsafe(column, 1.0 / 40.0);
            lp.SetColumnUpperBound(column, 1.0);
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
            lp.SetElementUnsafe(rowManaRegen, column, mps);
            lp.SetElementUnsafe(rowPotion, column, 1.0);
            lp.SetElementUnsafe(rowManaPotion, column, 1.0);
            lp.SetElementUnsafe(rowThreat, column, tps);
            ManaPotionTps = (float)tps;
            //lp.SetElementUnsafe(rowManaPotionManaGem, column, 40.0);
            lp.SetCostUnsafe(column, 0.0);
            /*if (segmentCooldowns && effectPotionAvailable)
            {
                for (int ss = 0; ss < segments; ss++)
                {
                    double cool = 120;
                    int maxs = (int)Math.Floor(ss + cool / segmentDuration) - 1;
                    if (ss * segmentDuration + cool >= calculationOptions.FightDuration) maxs = segments - 1;
                    if (segment >= ss && segment <= maxs) lp.SetElementUnsafe(rowSegmentPotion + ss, column, 15.0);
                    if (ss * segmentDuration + cool >= calculationOptions.FightDuration) break;
                }
            }*/
            if (restrictManaUse)
            {
                for (int ss = segment * manaSegments + manaSegment; ss < SegmentList.Count * manaSegments - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentManaUnderflow + ss, column, mps);
                    lp.SetElementUnsafe(rowSegmentManaOverflow + ss, column, -mps);
                }
            }
            if (restrictThreat)
            {
                for (int ss = segment; ss < SegmentList.Count - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentThreat + ss, column, tps);
                }
            }
        }

        private void ConstructEvocation(Stats baseStats, float threatFactor)
        {
            if (evocationAvailable)
            {
                CastingState evoBaseState = BaseState;
                if (CalculationOptions.Enable2T10Evocation && baseStats.Mage2T10 > 0)
                {
                    evoBaseState = BaseState.Tier10TwoPieceState;
                }
                int evocationSegments = (restrictManaUse) ? SegmentList.Count : 1;
                float evocationDuration = (8f/* + baseStats.EvocationExtension*/) / evoBaseState.CastingSpeed;
                EvocationDuration = evocationDuration;
                EvocationDurationIV = evocationDuration / 1.2f;
                EvocationDurationHero = evocationDuration / 1.3f;
                EvocationDurationIVHero = evocationDuration / 1.2f / 1.3f;
                if (CalculationOptions.Beta)
                {
                    EvocationDuration = (float)Math.Floor(8 / (EvocationDuration / 4)) * EvocationDuration / 4;
                    EvocationDurationIV = (float)Math.Floor(8 / (EvocationDurationIV / 4)) * EvocationDurationIV / 4;
                    EvocationDurationHero = (float)Math.Floor(8 / (EvocationDurationHero / 4)) * EvocationDurationHero / 4;
                    EvocationDurationIVHero = (float)Math.Floor(8 / (EvocationDurationIVHero / 4)) * EvocationDurationIVHero / 4;
                }
                float evocationMana = baseStats.Mana;
                EvocationRegen = BaseState.ManaRegen5SR + 0.15f * evocationMana / 2f * evoBaseState.CastingSpeed;
                EvocationRegenIV = BaseState.ManaRegen5SR + 0.15f * evocationMana / 2f * evoBaseState.CastingSpeed * 1.2f;
                EvocationRegenHero = BaseState.ManaRegen5SR + 0.15f * evocationMana / 2f * evoBaseState.CastingSpeed * 1.3f;
                EvocationRegenIVHero = BaseState.ManaRegen5SR + 0.15f * evocationMana / 2f * evoBaseState.CastingSpeed * 1.2f * 1.3f;
                if (EvocationRegen * evocationDuration > baseStats.Mana)
                {
                    evocationDuration = baseStats.Mana / EvocationRegen;
                    EvocationDuration = evocationDuration;
                    EvocationDurationIV = baseStats.Mana / EvocationRegenIV;
                    EvocationDurationHero = baseStats.Mana / EvocationRegenHero;
                    EvocationDurationIVHero = baseStats.Mana / EvocationRegenIVHero;
                }
                if (CalculationOptions.AverageCooldowns)
                {
                    MaxEvocation = CalculationOptions.FightDuration / EvocationCooldown;
                }
                else if (segmentCooldowns && advancedConstraintsLevel >= 3)
                {
                    MaxEvocation = Math.Max(1, 1 + (float)Math.Floor((CalculationOptions.FightDuration - evocationDuration) / EvocationCooldown));
                }
                else
                {
                    MaxEvocation = Math.Max(1, 1 + (float)Math.Floor((CalculationOptions.FightDuration - 90f) / EvocationCooldown));
                }
                int mask = 0;
                if (waterElementalAvailable && MageTalents.GlyphOfEternalWater)
                {
                    mask |= (int)StandardEffect.WaterElemental;
                }
                CastingState evoState = null;
                CastingState evoStateIV = null;
                CastingState evoStateHero = null;
                CastingState evoStateIVHero = null;
                if (waterElementalAvailable && MageTalents.GlyphOfEternalWater)
                {
                    evoState = CastingState.New(this, (int)StandardEffect.Evocation | mask, false, 0);
                    if (CalculationOptions.EnableHastedEvocation)
                    {
                        evoStateIV = CastingState.New(this, (int)StandardEffect.Evocation | (int)StandardEffect.IcyVeins | mask, false, 0);
                        evoStateHero = CastingState.New(this, (int)StandardEffect.Evocation | (int)StandardEffect.Heroism | mask, false, 0);
                        evoStateIVHero = CastingState.New(this, (int)StandardEffect.Evocation | (int)StandardEffect.IcyVeins | (int)StandardEffect.Heroism | mask, false, 0);
                    }
                }
                else
                {
                    evoState = CastingState.NewRaw(this, (int)StandardEffect.Evocation | mask);
                    if (CalculationOptions.EnableHastedEvocation)
                    {
                        evoStateIV = CastingState.NewRaw(this, (int)StandardEffect.Evocation | (int)StandardEffect.IcyVeins | mask);
                        evoStateHero = CastingState.NewRaw(this, (int)StandardEffect.Evocation | (int)StandardEffect.Heroism | mask);
                        evoStateIVHero = CastingState.NewRaw(this, (int)StandardEffect.Evocation | (int)StandardEffect.IcyVeins | (int)StandardEffect.Heroism | mask);
                    }
                }
                if (useIncrementalOptimizations && segmentMana)
                {
                    for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                    {
                        int segment = CalculationOptions.IncrementalSetSegments[index];
                        int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                        int state = CalculationOptions.IncrementalSetStateIndexes[index];
                        switch (CalculationOptions.IncrementalSetVariableType[index])
                        {
                            case VariableType.Evocation:
                                if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoState))
                                {
                                    SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoState, segment, manaSegment, VariableType.Evocation, false);
                                }
                                break;
                            case VariableType.EvocationHero:
                                if (state == evoStateHero.Effects && CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoStateHero))
                                {
                                    // last tick of heroism
                                    SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoStateHero, segment, manaSegment, VariableType.EvocationHero, true);
                                }
                                if (state == evoState.Effects && CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoState))
                                {
                                    // remainder
                                    SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoState, segment, manaSegment, VariableType.EvocationHero, false);
                                }
                                break;
                            case VariableType.EvocationIV:
                                if (state == evoStateIV.Effects && CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoStateIV))
                                {
                                    // last tick of icy veins
                                    SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoStateIV, segment, manaSegment, VariableType.EvocationIV, true);
                                }
                                else if (state == evoState.Effects && CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoState))
                                {
                                    // remainder
                                    SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoState, segment, manaSegment, VariableType.EvocationIV, false);
                                }
                                break;
                            case VariableType.EvocationIVHero:
                                if (state == evoStateIVHero.Effects && CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoStateIVHero))
                                {
                                    // last tick of icy veins+heroism
                                    SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoStateIVHero, segment, manaSegment, VariableType.EvocationIVHero, true);
                                }
                                if (state == evoState.Effects && CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoState))
                                {
                                    // remainder
                                    SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoState, segment, manaSegment, VariableType.EvocationIVHero, false);
                                }
                                break;
                        }
                    }
                }
                else
                {
                    int minManaSegment = segmentMana ? 1 : 0;
                    for (int segment = 0; segment < evocationSegments; segment++)
                    {
                        for (int manaSegment = minManaSegment; manaSegment < manaSegments; manaSegment++)
                        {
                            // base evocation
                            if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoState))
                            {
                                SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoState, segment, manaSegment, VariableType.Evocation, false);
                            }
                            if (CalculationOptions.EnableHastedEvocation)
                            {
                                if (icyVeinsAvailable)
                                {
                                    if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoStateIV))
                                    {
                                        // last tick of icy veins
                                        SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoStateIV, segment, manaSegment, VariableType.EvocationIV, true);
                                    }
                                    if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoState))
                                    {
                                        // remainder
                                        SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoState, segment, manaSegment, VariableType.EvocationIV, false);
                                    }
                                }
                                if (heroismAvailable)
                                {
                                    if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoStateHero))
                                    {
                                        // last tick of heroism
                                        SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoStateHero, segment, manaSegment, VariableType.EvocationHero, true);
                                    }
                                    if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoState))
                                    {
                                        // remainder
                                        SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoState, segment, manaSegment, VariableType.EvocationHero, false);
                                    }
                                }
                                if (icyVeinsAvailable && heroismAvailable)
                                {
                                    if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoStateIVHero))
                                    {
                                        // last tick of icy veins+heroism
                                        SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoStateIVHero, segment, manaSegment, VariableType.EvocationIVHero, true);
                                    }
                                    if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoState))
                                    {
                                        // remainder
                                        SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoState, segment, manaSegment, VariableType.EvocationIVHero, false);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MaxEvocation = 0;
                EvocationDuration = 0;
                EvocationDurationIV = 0;
                EvocationDurationHero = 0;
                EvocationDurationIVHero = 0;
                EvocationRegen = 0;
                EvocationRegenIV = 0;
                EvocationRegenHero = 0;
                EvocationRegenIVHero = 0;
            }
        }

        private void SetEvocationColumn(float threatFactor, int evocationSegments, float evocationMana, CastingState evoState, int segment, int manaSegment, VariableType evocationType, bool lastTick)
        {
            double dps = 0.0f;
            if (waterElementalAvailable && MageTalents.GlyphOfEternalWater)
            {
                dps = evoState.GetSpell(SpellId.Waterbolt).DamagePerSecond;
            }
            double tps;            
            double mps;
            float evocationDuration;
            int column = lp.AddColumnUnsafe();
            double evoFactor;
            switch (evocationType)
            {
                case VariableType.Evocation:
                    mps = -EvocationRegen;
                    tps = 0.15f * evocationMana / 2f * BaseState.CastingSpeed * 0.5f * threatFactor;
                    evocationDuration = EvocationDuration;
                    evoFactor = 1.0;

                    lp.SetElementUnsafe(rowEvocation, column, 1.0);

                    break;
                case VariableType.EvocationHero:
                    mps = -EvocationRegenHero;
                    tps = 0.15f * evocationMana / 2f * BaseState.CastingSpeed * 1.3 * 0.5f * threatFactor;
                    evocationDuration = EvocationDurationHero;
                    evoFactor = 1.3;

                    if (lastTick)
                    {
                        lp.SetElementUnsafe(rowHeroism, column, 1.0);
                    }
                    lp.SetElementUnsafe(rowEvocation, column, 1.3);
                    lp.SetElementUnsafe(rowEvocationHero, column, 1.0);

                    break;
                case VariableType.EvocationIV:
                    mps = -EvocationRegenIV;
                    tps = 0.15f * evocationMana / 2f * BaseState.CastingSpeed * 1.2 * 0.5f * threatFactor;
                    evocationDuration = EvocationDurationIV;
                    evoFactor = 1.2;

                    if (lastTick)
                    {
                        lp.SetElementUnsafe(rowIcyVeins, column, 1.0);
                        if (segmentCooldowns)
                        {
                            foreach (SegmentConstraint constraint in rowSegmentIcyVeins)
                            {
                                if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                            }
                        }
                    }
                    lp.SetElementUnsafe(rowEvocation, column, 1.2);
                    lp.SetElementUnsafe(rowEvocationIV, column, 1.0);

                    break;
                case VariableType.EvocationIVHero:
                default:
                    mps = -EvocationRegenIVHero;
                    tps = 0.15f * evocationMana / 2f * BaseState.CastingSpeed * 1.2 * 1.3 * 0.5f * threatFactor;
                    evocationDuration = EvocationDurationIVHero;
                    evoFactor = 1.2 * 1.3;

                    if (lastTick)
                    {
                        lp.SetElementUnsafe(rowHeroism, column, 1.0);
                        lp.SetElementUnsafe(rowIcyVeins, column, 1.0);
                        lp.SetElementUnsafe(rowHeroismIcyVeins, column, 1.0);
                        if (segmentCooldowns)
                        {
                            foreach (SegmentConstraint constraint in rowSegmentIcyVeins)
                            {
                                if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                            }
                        }
                    }
                    lp.SetElementUnsafe(rowEvocation, column, 1.2 * 1.3);
                    lp.SetElementUnsafe(rowEvocationHero, column, 1.2);
                    lp.SetElementUnsafe(rowEvocationIVHero, column, 1.0);

                    break;
            }
            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = evocationType, Segment = segment, ManaSegment = manaSegment, State = evoState, Dps = dps, Mps = mps, Tps = tps });
            lp.SetColumnUpperBound(column, (evocationSegments > 1 || manaSegments > 1) ? evocationDuration : evocationDuration * MaxEvocation);
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
            lp.SetElementUnsafe(rowManaRegen, column, mps);
            lp.SetElementUnsafe(rowFightDuration, column, 1.0);
            lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
            lp.SetElementUnsafe(rowThreat, column, tps); // should split among all targets if more than one, assume one only
            lp.SetCostUnsafe(column, minimizeTime ? -1 : dps);
            if (segmentNonCooldowns) lp.SetElementUnsafe(rowSegment + segment, column, 1.0);
            if (restrictManaUse)
            {
                if (segmentMana)
                {
                    for (int ss = segment * manaSegments + manaSegment - 1; ss < SegmentList.Count * manaSegments - 1; ss++)
                    {
                        lp.SetElementUnsafe(rowSegmentManaOverflow + ss, column, -mps);
                    }
                    for (int ss = segment * manaSegments + manaSegment; ss < SegmentList.Count * manaSegments - 1; ss++)
                    {
                        lp.SetElementUnsafe(rowSegmentManaUnderflow + ss, column, mps);
                    }
                }
                else
                {
                    for (int ss = segment; ss < SegmentList.Count - 1; ss++)
                    {
                        lp.SetElementUnsafe(rowSegmentManaUnderflow + ss, column, mps);
                        lp.SetElementUnsafe(rowSegmentManaOverflow + ss, column, -mps);
                    }
                }
                if (segmentCooldowns)
                {
                    foreach (SegmentConstraint constraint in rowSegmentEvocation)
                    {
                        if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, evoFactor);
                    }
                }
            }
            if (restrictThreat)
            {
                for (int ss = segment; ss < SegmentList.Count - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentThreat + ss, column, tps);
                }
            }
        }

        private void ConstructWand()
        {
            if (Character.Ranged != null && Character.Ranged.Item.Type == ItemType.Wand)
            {
                if (useIncrementalOptimizations)
                {
                    bool first = true;
                    Cycle wand = null;
                    for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                    {
                        if (CalculationOptions.IncrementalSetVariableType[index] == VariableType.Wand)
                        {
                            if (first)
                            {
                                WandTemplate.Initialize(this, (MagicSchool)Character.Ranged.Item.DamageType, Character.Ranged.Item.MinDamage, Character.Ranged.Item.MaxDamage, Character.Ranged.Item.Speed);
                                Spell w = WandTemplate.GetSpell(BaseState);
                                Wand = w;
                                wand = w;
                                first = false;
                            }
                            int segment = CalculationOptions.IncrementalSetSegments[index];
                            int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                            float mult = segmentCooldowns ? CalculationOptions.GetDamageMultiplier(SegmentList[segment]) : 1.0f;
                            double mps = wand.ManaPerSecond;
                            double dps = wand.DamagePerSecond * mult;
                            double tps = wand.ThreatPerSecond;
                            if (mult > 0)
                            {
                                SetWandColumn(wand, mps, segment, manaSegment, dps, tps);
                            }
                        }
                    }
                }
                else
                {
                    int wandSegments = (segmentNonCooldowns) ? SegmentList.Count : 1;
                    WandTemplate.Initialize(this, (MagicSchool)Character.Ranged.Item.DamageType, Character.Ranged.Item.MinDamage, Character.Ranged.Item.MaxDamage, Character.Ranged.Item.Speed);
                    Spell w = WandTemplate.GetSpell(BaseState);
                    Wand = w;
                    Cycle wand = w;
                    double mps = wand.ManaPerSecond;
                    for (int segment = 0; segment < wandSegments; segment++)
                    {
                        for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                        {
                            float mult = segmentCooldowns ? CalculationOptions.GetDamageMultiplier(SegmentList[segment]) : 1.0f;
                            double dps = wand.DamagePerSecond * mult;
                            double tps = wand.ThreatPerSecond;
                            if (mult > 0)
                            {
                                SetWandColumn(wand, mps, segment, manaSegment, dps, tps);
                            }
                        }
                    }
                }
            }
            else
            {
                Wand = null;
            }
        }

        private void SetWandColumn(Cycle wand, double mps, int segment, int manaSegment, double dps, double tps)
        {
            int column = lp.AddColumnUnsafe();
            lp.SetColumnUpperBound(column, segmentNonCooldowns ? SegmentList[segment].Duration : CalculationOptions.FightDuration);
            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.Wand, Cycle = wand, Segment = segment, ManaSegment = manaSegment, State = BaseState, Dps = dps, Mps = mps, Tps = tps });
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
            lp.SetElementUnsafe(rowManaRegen, column, mps);
            lp.SetElementUnsafe(rowFightDuration, column, 1.0);
            lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
            lp.SetElementUnsafe(rowThreat, column, tps);
            lp.SetElementUnsafe(rowTargetDamage, column, -wand.DamagePerSecond);
            lp.SetCostUnsafe(column, minimizeTime ? -1 : dps);
            if (segmentNonCooldowns) lp.SetElementUnsafe(rowSegment + segment, column, 1.0);
            if (restrictManaUse)
            {
                for (int ss = segment * manaSegments + manaSegment; ss < SegmentList.Count * manaSegments - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentManaUnderflow + ss, column, mps);
                    lp.SetElementUnsafe(rowSegmentManaOverflow + ss, column, -mps);
                }
            }
            if (restrictThreat)
            {
                for (int ss = segment; ss < SegmentList.Count - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentThreat + ss, column, tps);
                }
            }
        }

        private void ConstructIdleRegen()
        {
            int idleRegenSegments = (restrictManaUse) ? SegmentList.Count : 1;
            double dps = 0.0f;
            double tps = 0.0f;
            double mps = -(BaseState.ManaRegen * (1 - CalculationOptions.Fragmentation) + BaseState.ManaRegen5SR * CalculationOptions.Fragmentation);
            if (useIncrementalOptimizations)
            {
                for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                {
                    if (CalculationOptions.IncrementalSetVariableType[index] == VariableType.IdleRegen)
                    {
                        int segment = CalculationOptions.IncrementalSetSegments[index];
                        int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                        SetIdleRegenColumn(idleRegenSegments, dps, tps, mps, segment, manaSegment);
                    }
                }
            }
            else
            {
                for (int segment = 0; segment < idleRegenSegments; segment++)
                {
                    for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                    {
                        SetIdleRegenColumn(idleRegenSegments, dps, tps, mps, segment, manaSegment);
                    }
                }
            }
        }

        private void SetIdleRegenColumn(int idleRegenSegments, double dps, double tps, double mps, int segment, int manaSegment)
        {
            int column = lp.AddColumnUnsafe();
            lp.SetColumnUpperBound(column, (idleRegenSegments > 1) ? SegmentList[segment].Duration : CalculationOptions.FightDuration);
            if (idleRegenSegments == 1 && manaSegments == 1 && !needsTimeExtension)
            {
                lp.SetColumnLowerBound(column, (1 - dpsTime) * CalculationOptions.FightDuration);
            }
            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.IdleRegen, Segment = segment, ManaSegment = manaSegment, State = BaseState, Dps = dps, Mps = mps, Tps = tps });
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
            lp.SetElementUnsafe(rowManaRegen, column, mps);
            lp.SetElementUnsafe(rowFightDuration, column, 1.0);
            lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
            lp.SetElementUnsafe(rowDpsTime, column, -1.0);
            lp.SetCostUnsafe(column, minimizeTime ? -1 : 0);
            if (segmentNonCooldowns) lp.SetElementUnsafe(rowSegment + segment, column, 1.0);
            if (restrictManaUse)
            {
                for (int ss = segment * manaSegments + manaSegment; ss < idleRegenSegments * manaSegments - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentManaUnderflow + ss, column, mps);
                    lp.SetElementUnsafe(rowSegmentManaOverflow + ss, column, -mps);
                }
            }
        }

        private void SetCalculationReuseReferences()
        {
            // determine which effects only cause a change in haste, thus allowing calculation reuse (only recalculating cast time)
            int recalcCastTime = (int)StandardEffect.IcyVeins | (int)StandardEffect.Heroism | (int)StandardEffect.PotionOfSpeed | (int)StandardEffect.Berserking | (int)StandardEffect.PowerInfusion;
            for (int i = 0; i < ItemBasedEffectCooldownsCount; i++)
            {
                EffectCooldown effect = ItemBasedEffectCooldowns[i];
                if (effect.HasteEffect)
                {
                    recalcCastTime |= effect.Mask;
                }
            }
            if (BaseStats.Mage4T10 == 0)
            {
                recalcCastTime |= (int)StandardEffect.MirrorImage; // for this it's actually identical, potential for further optimization
            }
            // states will be calculated in forward manner, see if some can reuse previous states
            for (int i = 0; i < stateList.Count; i++)
            {
                CastingState si = stateList[i];
                for (int j = 0; j < i; j++)
                {
                    CastingState sj = stateList[j];
                    // check the difference
                    int diff = si.Effects ^ sj.Effects;
                    if ((diff & ~recalcCastTime) == 0)
                    {
                        // the only difference is in haste effects
                        si.ReferenceCastingState = sj;
                        break;
                    }
                }
            }
        }

        private void AddWardStates()
        {
            if (CalculationOptions.UseFireWard || CalculationOptions.UseFrostWard)
            {
                List<CastingState> newStates = new List<CastingState>();
                foreach (CastingState state in stateList)
                {
                    newStates.Add(state);
                    if (CalculationOptions.UseFireWard)
                    {
                        CastingState s = new CastingState(this, state.Effects, false, 0);
                        s.UseFireWard = true;
                        s.ReferenceCastingState = state;
                        newStates.Add(s);
                    }
                    if (CalculationOptions.UseFrostWard)
                    {
                        CastingState s = new CastingState(this, state.Effects, false, 0);
                        s.UseFrostWard = true;
                        s.ReferenceCastingState = state;
                        newStates.Add(s);
                    }
                }
                stateList = newStates;
            }
        }

        private void ConstructSegments()
        {
            if (SegmentList == null)
            {
                SegmentList = new List<Segment>();
            }
            else
            {
                SegmentList.Clear();
            }
            if (segmentCooldowns)
            {
                List<double> ticks = new List<double>();
                if (CalculationOptions.VariableSegmentDuration)
                {
                    // variable segment durations to get a better grasp on varied cooldown durations
                    // create ticks in intervals of half cooldown duration
                    if (potionOfSpeedAvailable || potionOfWildMagicAvailable || manaPotionAvailable)
                    {
                        AddSegmentTicks(ticks, 120.0);
                    }
                    if (arcanePowerAvailable)
                    {
                        AddSegmentTicks(ticks, ArcanePowerCooldown);
                        //AddEffectTicks(ticks, ArcanePowerCooldown, ArcanePowerDuration);
                    }
                    if (combustionAvailable) AddSegmentTicks(ticks, 300.0);
                    if (berserkingAvailable) AddSegmentTicks(ticks, 180.0);
                    if (CalculationOptions.ManaGemEnabled || manaGemEffectAvailable)
                    {
                        ticks.Add(15.0); // get a better grasp on mana overflow
                        AddSegmentTicks(ticks, 60.0);
                    }
                    if (icyVeinsAvailable)
                    {
                        AddSegmentTicks(ticks, IcyVeinsCooldown);
                        //if (!coldsnapAvailable) AddEffectTicks(ticks, calculationResult.IcyVeinsCooldown, 20.0);
                    }
                    if (waterElementalAvailable && !MageTalents.GlyphOfEternalWater) AddSegmentTicks(ticks, WaterElementalCooldown);
                    for (int i = 0; i < ItemBasedEffectCooldownsCount; i++)
                    {
                        EffectCooldown cooldown = ItemBasedEffectCooldowns[i];
                        AddSegmentTicks(ticks, cooldown.Cooldown);
                    }
                }
                else
                {
                    for (int i = 0; CalculationOptions.FixedSegmentDuration * i < CalculationOptions.FightDuration - 0.00001; i++)
                    {
                        //segmentList.Add(new Segment() { TimeStart = calculationOptions.FixedSegmentDuration * i, Duration = Math.Min(calculationOptions.FixedSegmentDuration, calculationOptions.FightDuration - calculationOptions.FixedSegmentDuration * i) });
                        ticks.Add(CalculationOptions.FixedSegmentDuration * i);
                    }
                }
                if (!string.IsNullOrEmpty(CalculationOptions.AdditionalSegmentSplits))
                {
                    string[] splits = CalculationOptions.AdditionalSegmentSplits.Split(',');
                    foreach (string split in splits)
                    {
                        double tick;
                        if (double.TryParse(split.Trim(), out tick))
                        {
                            ticks.Add(tick);
                        }
                    }
                }
                if (moltenFuryAvailable)
                {
                    ticks.Add((1 - CalculationOptions.MoltenFuryPercentage) * CalculationOptions.FightDuration);
                }
                if (CalculationOptions.HeroismControl == 3)
                {
                    ticks.Add(Math.Max(0.0, Math.Min(CalculationOptions.FightDuration - CalculationOptions.MoltenFuryPercentage * CalculationOptions.FightDuration, CalculationOptions.FightDuration - 40.0)));
                }
                if (!string.IsNullOrEmpty(CalculationOptions.CooldownRestrictions) && CalculationOptions.CooldownRestrictionList == null)
                {
                    StateDescription.Scanner scanner = new StateDescription.Scanner();
                    StateDescription.Parser parser = new StateDescription.Parser(scanner);
                    CalculationOptions.CooldownRestrictionList = new List<CooldownRestriction>();
                    string[] lines = CalculationOptions.CooldownRestrictions.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string line in lines)
                    {
#if SILVERLIGHT
                        string[] tokens = line.Split(new char[] { '-', ':' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < tokens.Length; i++)
                        {
                            tokens[i] = tokens[i].Trim();
                        }
#else
                        string[] tokens = line.Split(new char[] { '-', ':', ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
#endif
                        if (tokens.Length == 3)
                        {
                            CooldownRestriction restriction = new CooldownRestriction();
                            double value;
                            if (!double.TryParse(tokens[0], out value)) continue;
                            restriction.TimeStart = value;
                            if (!double.TryParse(tokens[1], out value)) continue;
                            restriction.TimeEnd = value;
                            StateDescription.ParseTree parseTree = parser.Parse(tokens[2]);
                            if (parseTree != null && parseTree.Errors.Count == 0)
                            {
                                try
                                {
                                    restriction.IsMatch = parseTree.Compile();
                                    CalculationOptions.CooldownRestrictionList.Add(restriction);
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                }
                if (CalculationOptions.CooldownRestrictionList != null)
                {
                    foreach (CooldownRestriction restriction in CalculationOptions.CooldownRestrictionList)
                    {
                        ticks.Add(restriction.TimeStart);
                        ticks.Add(restriction.TimeEnd);
                    }
                }
                if (CalculationOptions.EncounterEnabled && CalculationOptions.Encounter != null)
                {
                    foreach (DamageMultiplier multiplier in CalculationOptions.Encounter.GlobalMultipliers)
                    {
                        if (multiplier.RelativeTime)
                        {
                            ticks.Add(multiplier.StartTime * CalculationOptions.FightDuration);
                            ticks.Add(multiplier.EndTime * CalculationOptions.FightDuration);
                        }
                        else
                        {
                            ticks.Add(multiplier.StartTime);
                            ticks.Add(multiplier.EndTime);
                        }
                    }
                    foreach (TargetGroup group in CalculationOptions.Encounter.TargetGroups)
                    {
                        float startTime, endTime;
                        if (group.RelativeTime)
                        {
                            startTime = group.EntranceTime * CalculationOptions.FightDuration;
                            endTime = group.ExitTime * CalculationOptions.FightDuration;
                        }
                        else
                        {
                            startTime = group.EntranceTime;
                            endTime = group.ExitTime;
                        }
                        ticks.Add(startTime);
                        ticks.Add(endTime);
                        foreach (DamageMultiplier multiplier in group.Multipliers)
                        {
                            if (multiplier.RelativeTime)
                            {
                                ticks.Add(startTime + multiplier.StartTime * (endTime - startTime));
                                ticks.Add(startTime + multiplier.EndTime * (endTime - startTime));
                            }
                            else
                            {
                                ticks.Add(startTime + multiplier.StartTime);
                                ticks.Add(startTime + multiplier.EndTime);
                            }
                        }
                    }
                }
                ticks.Sort();
                for (int i = 0; i < ticks.Count; i++)
                {
                    if ((i == 0 || ticks[i] > ticks[i - 1] + 0.00001) && ticks[i] < CalculationOptions.FightDuration - 0.00001)
                    {
                        if (SegmentList.Count > 0)
                        {
                            SegmentList[SegmentList.Count - 1].Duration = ticks[i] - ticks[i - 1];
                        }
                        SegmentList.Add(new Segment() { TimeStart = ticks[i] });
                    }
                }
                SegmentList[SegmentList.Count - 1].Duration = CalculationOptions.FightDuration - SegmentList[SegmentList.Count - 1].TimeStart;
            }
            else
            {
                SegmentList.Add(new Segment() { TimeStart = 0, Duration = CalculationOptions.FightDuration });
            }
            for (int i = 0; i < SegmentList.Count; i++)
            {
                SegmentList[i].Index = i;
            }
        }

        private void SetProblemRHS()
        {
            int coldsnapCount = coldsnapAvailable ? (1 + (int)((CalculationOptions.FightDuration - WaterElementalDuration) / ColdsnapCooldown)) : 0;

            // water elemental
            double weDuration = 0.0;
            if (waterElementalAvailable)
            {
                if (MageTalents.GlyphOfEternalWater)
                {
                    weDuration = CalculationOptions.FightDuration;
                }
                else
                {
                    weDuration = MaximizeEffectDuration(CalculationOptions.FightDuration, WaterElementalDuration, WaterElementalCooldown);
                    if (coldsnapAvailable) weDuration = MaximizeColdsnapDuration(CalculationOptions.FightDuration, ColdsnapCooldown, WaterElementalDuration, WaterElementalCooldown, out coldsnapCount);
                }
            }

            double combustionCount = combustionAvailable ? (1 + (int)((CalculationOptions.FightDuration - 15f) / 195f)) : 0;

            double ivlength = 0.0;
            if ((!waterElementalAvailable || MageTalents.GlyphOfEternalWater) && coldsnapAvailable)
            {
                ivlength = Math.Floor(MaximizeColdsnapDuration(CalculationOptions.FightDuration, ColdsnapCooldown, 20.0, IcyVeinsCooldown, out coldsnapCount));
            }
            else if (waterElementalAvailable && coldsnapAvailable)
            {
                // TODO recheck this logic
                double wecount = (weDuration / WaterElementalDuration);
                if (wecount >= Math.Floor(wecount) + 20.0 / WaterElementalDuration)
                    ivlength = Math.Ceiling(wecount) * 20.0;
                else
                    ivlength = Math.Floor(wecount) * 20.0;
            }
            else
            {
                double effectiveDuration = CalculationOptions.FightDuration;
                if (waterElementalAvailable) effectiveDuration -= BaseGlobalCooldown; // EXPERIMENTAL
                ivlength = MaximizeEffectDuration(effectiveDuration, 20.0, IcyVeinsCooldown);
            }

            double mflength = CalculationOptions.MoltenFuryPercentage * CalculationOptions.FightDuration;

            lp.SetRHSUnsafe(rowManaRegen, StartingMana);
            lp.SetRHSUnsafe(rowFightDuration, CalculationOptions.FightDuration);
            lp.SetRHSUnsafe(rowTimeExtension, -CalculationOptions.FightDuration);
            lp.SetRHSUnsafe(rowEvocation, EvocationDuration * MaxEvocation);
            if (icyVeinsAvailable) lp.SetRHSUnsafe(rowEvocationIV, EvocationDurationIV * MaxEvocation);
            if (heroismAvailable) lp.SetRHSUnsafe(rowEvocationHero, EvocationDurationHero);
            if (icyVeinsAvailable && heroismAvailable) lp.SetRHSUnsafe(rowEvocationIVHero, EvocationDurationIVHero);
            lp.SetRHSUnsafe(rowPotion, 1.0);
            lp.SetRHSUnsafe(rowManaPotion, 1.0);
            lp.SetRHSUnsafe(rowManaGem, Math.Min(3, CalculationOptions.AverageCooldowns ? CalculationOptions.FightDuration / 120.0 : MaxManaGem));
            lp.SetRHSUnsafe(rowManaGemMax, CalculationOptions.AverageCooldowns ? CalculationOptions.FightDuration / 120.0 : MaxManaGem);
            if (conjureManaGem) lp.SetRHSUnsafe(rowConjureManaGem, MaxConjureManaGem * ConjureManaGem.CastTime);
            //if (wardsAvailable) lp.SetRHSUnsafe(rowWard, calculationResult.MaxWards * calculationResult.Ward.CastTime);

            foreach (EffectCooldown cooldown in CooldownList)
            {
                if (cooldown.AutomaticConstraints)
                {
                    lp.SetRHSUnsafe(cooldown.Row, (CalculationOptions.AverageCooldowns && !float.IsPositiveInfinity(cooldown.Cooldown)) ? CalculationOptions.FightDuration * cooldown.Duration / cooldown.Cooldown : cooldown.MaximumDuration);
                }
            }

            if (heroismAvailable)
            {
                double minDuration = Math.Min(0.99 * CalculationOptions.FightDuration * dpsTime, 40.0);
                if (moltenFuryAvailable && CalculationOptions.HeroismControl == 3 && mflength < minDuration)
                {
                    minDuration = 0.99 * mflength;
                }
                lp.SetLHSUnsafe(rowHeroism, minDuration); // if heroism is marked as available then this implies that it has to be used, not only that it can be used
            }
            //if (powerInfusionAvailable) lp.SetRHSUnsafe(rowPowerInfusion, calculationOptions.AverageCooldowns ? calculationResult.PowerInfusionDuration / calculationResult.PowerInfusionCooldown * calculationOptions.FightDuration : pilength);
            //if (arcanePowerAvailable) lp.SetRHSUnsafe(rowArcanePower, calculationOptions.AverageCooldowns ? calculationResult.ArcanePowerDuration / calculationResult.ArcanePowerCooldown * calculationOptions.FightDuration : aplength);
            //if (heroismAvailable && arcanePowerAvailable) lp.SetRHSUnsafe(rowHeroismArcanePower, calculationResult.ArcanePowerDuration);
            //if (heroismAvailable && manaGemEffectAvailable) lp.SetRHSUnsafe(rowHeroismManaGemEffect, calculationResult.ManaGemEffectDuration);
            if (icyVeinsAvailable && coldsnapAvailable)
            {
                lp.SetRHSUnsafe(rowIcyVeins, CalculationOptions.AverageCooldowns ? (20.0 / IcyVeinsCooldown + 20.0 / ColdsnapCooldown) * CalculationOptions.FightDuration : ivlength);
                if (moltenFuryAvailable) lp.SetRHSUnsafe(rowMoltenFuryIcyVeins, 40);
                if (heroismAvailable) lp.SetRHSUnsafe(rowHeroismIcyVeins, 40);
            }
            if (moltenFuryAvailable) lp.SetRHSUnsafe(rowMoltenFury, mflength);
            //if (moltenFuryAvailable) lp.SetRHSUnsafe(rowMoltenFuryDestructionPotion, 15);
            //if (moltenFuryAvailable && manaGemEffectAvailable) lp.SetRHSUnsafe(rowMoltenFuryManaGemEffect, manaGemEffectDuration);
            //if (heroismAvailable) lp.SetRHSUnsafe(rowHeroismDestructionPotion, 15);
            //if (icyVeinsAvailable) lp.SetRHSUnsafe(rowIcyVeinsDestructionPotion, dpivlength);
            //if (flameCapAvailable)
            //{
            //    lp.SetRHSUnsafe(rowFlameCap, calculationOptions.AverageCooldowns ? calculationOptions.FightDuration / 120.0 : ((int)(calculationOptions.FightDuration / 180.0 + 2.0 / 3.0)) * 3.0 / 2.0);
            //}
            //if (moltenFuryAvailable) lp.SetRHSUnsafe(rowMoltenFuryFlameCap, 60);
            //lp.SetRHSUnsafe(rowFlameCapDestructionPotion, dpflamelength);
            if (manaGemEffectAvailable) lp.SetRHSUnsafe(rowManaGemEffect, CalculationOptions.AverageCooldowns ? CalculationOptions.FightDuration * ManaGemEffectDuration / 120f : MaximizeEffectDuration(CalculationOptions.FightDuration, ManaGemEffectDuration, 120.0));
            lp.SetRHSUnsafe(rowDpsTime, -(1 - dpsTime) * CalculationOptions.FightDuration);
            lp.SetRHSUnsafe(rowAoe, CalculationOptions.AoeDuration * CalculationOptions.FightDuration);
            lp.SetRHSUnsafe(rowCombustion, CalculationOptions.AverageCooldowns ? CalculationOptions.FightDuration / CombustionCooldown : combustionCount);
            lp.SetRHSUnsafe(rowMoltenFuryCombustion, 1);
            lp.SetRHSUnsafe(rowHeroismCombustion, 1);
            //lp.SetRHSUnsafe(rowMoltenFuryBerserking, 10);
            //lp.SetRHSUnsafe(rowHeroismBerserking, 10);
            //lp.SetRHSUnsafe(rowIcyVeinsDrumsOfBattle, drumsivlength);
            //lp.SetRHSUnsafe(rowArcanePowerDrumsOfBattle, drumsaplength);
            lp.SetRHSUnsafe(rowThreat, CalculationOptions.TpsLimit * CalculationOptions.FightDuration);
            double manaConsum;
            /*if (integralMana)
            {
                manaConsum = Math.Ceiling((calculationOptions.FightDuration - 7800 / manaBurn) / 60f + 2);
            }
            else
            {
                manaConsum = ((calculationOptions.FightDuration - 7800 / manaBurn) / 60f + 2);
            }
            if (manaGemEffectAvailable && manaConsum < calculationResult.MaxManaGem)*/
            manaConsum = MaxManaGem;
            //lp.SetRHSUnsafe(rowManaPotionManaGem, manaConsum * 40.0);
            lp.SetRHSUnsafe(rowBerserking, CalculationOptions.AverageCooldowns ? CalculationOptions.FightDuration * 10.0 / 180.0 : 10.0 * (1 + (int)((CalculationOptions.FightDuration - 10) / 180)));
            if (waterElementalAvailable && !MageTalents.GlyphOfEternalWater)
            {
                double duration = CalculationOptions.AverageCooldowns ? (WaterElementalDuration / WaterElementalCooldown + (coldsnapAvailable ? WaterElementalDuration / ColdsnapCooldown : 0.0)) * CalculationOptions.FightDuration : weDuration;
                lp.SetRHSUnsafe(rowWaterElemental, duration);
                lp.SetRHSUnsafe(rowSummonWaterElementalCount, BaseGlobalCooldown * Math.Ceiling(duration / WaterElementalDuration));
            }
            if (mirrorImageAvailable)
            {
                double duration = EffectCooldown[(int)StandardEffect.MirrorImage].MaximumDuration;
                lp.SetRHSUnsafe(rowSummonMirrorImageCount, BaseGlobalCooldown * Math.Ceiling(duration / MirrorImageDuration));
            }
            lp.SetRHSUnsafe(rowTargetDamage, -CalculationOptions.TargetDamage);

            for (int i = 0; i < rowStackingConstraintCount; i++)
            {
                lp.SetRHSUnsafe(rowStackingConstraint[i].Row, rowStackingConstraint[i].MaximumStackingDuration);
            }

            if (segmentCooldowns)
            {
                // heroism
                // ap
                if (arcanePowerAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentArcanePower)
                    {
                        lp.SetRHSUnsafe(constraint.Row, ArcanePowerDuration);
                    }
                }
                // pi
                if (powerInfusionAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentPowerInfusion)
                    {
                        lp.SetRHSUnsafe(constraint.Row, PowerInfusionDuration);
                    }
                }
                // iv
                if (icyVeinsAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentIcyVeins)
                    {
                        lp.SetRHSUnsafe(constraint.Row, 20 + (coldsnapAvailable ? 20 : 0));
                    }
                }
                // combustion
                if (combustionAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentCombustion)
                    {
                        lp.SetRHSUnsafe(constraint.Row, 1.0);
                    }
                }
                // berserking
                if (berserkingAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentBerserking)
                    {
                        lp.SetRHSUnsafe(constraint.Row, 10.0);
                    }
                }
                // water elemental
                if (waterElementalAvailable && !MageTalents.GlyphOfEternalWater)
                {
                    foreach (SegmentConstraint constraint in rowSegmentWaterElemental)
                    {
                        lp.SetRHSUnsafe(constraint.Row, WaterElementalDuration + (coldsnapAvailable ? WaterElementalDuration : 0.0));
                    }
                    foreach (SegmentConstraint constraint in rowSegmentSummonWaterElemental)
                    {
                        lp.SetRHSUnsafe(constraint.Row, BaseGlobalCooldown + (coldsnapAvailable ? BaseGlobalCooldown : 0.0));
                    }
                }
                // mirror image
                if (mirrorImageAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentMirrorImage)
                    {
                        lp.SetRHSUnsafe(constraint.Row, MirrorImageDuration);
                    }
                    foreach (SegmentConstraint constraint in rowSegmentSummonMirrorImage)
                    {
                        lp.SetRHSUnsafe(constraint.Row, BaseGlobalCooldown);
                    }
                }
                // flamecap
                if (flameCapAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentFlameCap)
                    {
                        lp.SetRHSUnsafe(constraint.Row, 60.0);
                    }
                }
                if (CalculationOptions.ManaGemEnabled)
                {
                    foreach (SegmentConstraint constraint in rowSegmentManaGem)
                    {
                        lp.SetRHSUnsafe(constraint.Row, 1.0);
                    }
                }
                // effect potion
                /*if (effectPotionAvailable)
                {
                    for (int seg = 0; seg < segments; seg++)
                    {
                        lp.SetRHSUnsafe(rowSegmentPotion + seg, 15.0);
                        double cool = 120;
                        if (seg * segmentDuration + cool >= calculationOptions.FightDuration) break;
                    }
                }*/
                for (int i = 0; i < ItemBasedEffectCooldownsCount; i++)
                {
                    EffectCooldown cooldown = ItemBasedEffectCooldowns[i];
                    foreach (SegmentConstraint constraint in cooldown.SegmentConstraints)
                    {
                        lp.SetRHSUnsafe(constraint.Row, cooldown.Duration);
                    }
                }
                if (manaGemEffectAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentManaGemEffect)
                    {
                        lp.SetRHSUnsafe(constraint.Row, ManaGemEffectDuration);
                    }
                }
                if (evocationAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentEvocation)
                    {
                        lp.SetRHSUnsafe(constraint.Row, EvocationDuration);
                    }
                }
                // timing
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    lp.SetRHSUnsafe(rowSegment + seg, SegmentList[seg].Duration);
                }
            }
            if (restrictManaUse)
            {
                for (int ss = 0; ss < SegmentList.Count * manaSegments - 1; ss++)
                {
                    lp.SetRHSUnsafe(rowSegmentManaUnderflow + ss, StartingMana);
                    lp.SetRHSUnsafe(rowSegmentManaOverflow + ss, BaseStats.Mana - StartingMana);
                }
            }
            if (restrictThreat)
            {
                for (int ss = 0; ss < SegmentList.Count - 1; ss++)
                {
                    lp.SetRHSUnsafe(rowSegmentThreat + ss, CalculationOptions.TpsLimit * SegmentList[ss].TimeEnd);
                }
            }
        }

        private int ConstructRows(bool minimizeTime, bool drinkingEnabled, bool needsTimeExtension, bool afterFightRegen)
        {
            #region Reset Rows
            rowManaRegen = -1;
            rowFightDuration = -1;
            rowEvocation = -1;
            rowEvocationIV = -1;
            rowEvocationHero = -1;
            rowEvocationIVHero = -1;
            rowPotion = -1;
            rowManaPotion = -1;
            rowConjureManaGem = -1;
            rowManaGem = -1;
            rowManaGemMax = -1;
            rowHeroism = -1;
            rowArcanePower = -1;
            rowIcyVeins = -1;
            rowWaterElemental = -1;
            rowMirrorImage = -1;
            rowMoltenFury = -1;
            rowMoltenFuryIcyVeins = -1;
            rowFlameCap = -1;
            rowManaGemEffect = -1;
            rowManaGemEffectActivation = -1;
            rowDpsTime = -1;
            rowAoe = -1;
            rowFlamestrike = -1;
            rowConeOfCold = -1;
            rowBlastWave = -1;
            rowDragonsBreath = -1;
            rowCombustion = -1;
            rowPowerInfusion = -1;
            rowMoltenFuryCombustion = -1;
            rowHeroismCombustion = -1;
            rowHeroismIcyVeins = -1;
            rowSummonWaterElemental = -1;
            rowSummonWaterElementalCount = -1;
            rowSummonMirrorImage = -1;
            rowSummonMirrorImageCount = -1;
            rowThreat = -1;
            rowBerserking = -1;
            rowTimeExtension = -1;
            rowAfterFightRegenMana = -1;
            rowTargetDamage = -1;
            rowSegment = -1;
            rowSegmentManaOverflow = -1;
            rowSegmentManaUnderflow = -1;
            rowSegmentThreat = -1;
            rowSegmentArcanePower = null;
            rowSegmentPowerInfusion = null;
            rowSegmentIcyVeins = null;
            rowSegmentWaterElemental = null;
            rowSegmentSummonWaterElemental = null;
            rowSegmentMirrorImage = null;
            rowSegmentSummonMirrorImage = null;
            rowSegmentCombustion = null;
            rowSegmentBerserking = null;
            rowSegmentFlameCap = null;
            rowSegmentManaGem = null;
            rowSegmentManaGemEffect = null;
            rowSegmentEvocation = null;
            #endregion

            int rowCount = 0;

            if (!CalculationOptions.UnlimitedMana) rowManaRegen = rowCount++;
            rowFightDuration = rowCount++;
            if (evocationAvailable && (needsTimeExtension || restrictManaUse || integralMana || CalculationOptions.EnableHastedEvocation)) rowEvocation = rowCount++;
            if (CalculationOptions.EnableHastedEvocation)
            {
                if (evocationAvailable && icyVeinsAvailable)
                {
                    if (needsTimeExtension || restrictManaUse || integralMana) rowEvocationIV = rowCount++;
                    //rowEvocationIVActivation = rowCount++;
                }
                if (evocationAvailable && heroismAvailable)
                {
                    if (needsTimeExtension || restrictManaUse || integralMana) rowEvocationHero = rowCount++;
                    //rowEvocationHeroActivation = rowCount++;
                }
                if (evocationAvailable && icyVeinsAvailable && heroismAvailable)
                {
                    if (needsTimeExtension || restrictManaUse || integralMana) rowEvocationIVHero = rowCount++;
                    //rowEvocationIVHeroActivation = rowCount++;
                }
            }
            if (manaPotionAvailable || effectPotionAvailable) rowPotion = rowCount++;
            if (manaPotionAvailable && integralMana) rowManaPotion = rowCount++;
            if (CalculationOptions.ManaGemEnabled)
            {
                if (segmentCooldowns || conjureManaGem || needsTimeExtension || segmentMana)
                {
                    rowManaGem = rowCount++;
                }
                if (requiresMIP || conjureManaGem)
                {
                    rowManaGemMax = rowCount++;
                }
                if (conjureManaGem)
                {
                    rowConjureManaGem = rowCount++;
                }
            }
            /*if (wardsAvailable)
            {
                rowWard = rowCount++;
            }*/
            //if (heroismAvailable) rowHeroism = rowCount++;
            //if (arcanePowerAvailable) rowArcanePower = rowCount++;
            //if (powerInfusionAvailable) rowPowerInfusion = rowCount++;
            //if (heroismAvailable && arcanePowerAvailable) rowHeroismArcanePower = rowCount++;
            //if (heroismAvailable && manaGemEffectAvailable) rowHeroismManaGemEffect = rowCount++;
            if (moltenFuryAvailable) rowMoltenFury = rowCount++;
            //if (moltenFuryAvailable && potionOfWildMagicAvailable) rowMoltenFuryDestructionPotion = rowCount++;
            //if (moltenFuryAvailable && manaGemEffectAvailable) rowMoltenFuryManaGemEffect = rowCount++;
            //if (heroismAvailable && effectPotionAvailable) rowHeroismDestructionPotion = rowCount++;
            //if (icyVeinsAvailable && effectPotionAvailable) rowIcyVeinsDestructionPotion = rowCount++;
            //if (flameCapAvailable) rowFlameCap = rowCount++;
            //if (moltenFuryAvailable && flameCapAvailable) rowMoltenFuryFlameCap = rowCount++;
            //if (flameCapAvailable && destructionPotionAvailable) rowFlameCapDestructionPotion = rowCount++;
            foreach (EffectCooldown cooldown in CooldownList)
            {
                if (cooldown.AutomaticConstraints)
                {
                    cooldown.Row = rowCount++;
                    cooldown.MaximumDuration = (float)MaximizeEffectDuration(CalculationOptions.FightDuration, cooldown.Duration, cooldown.Cooldown);
                }
            }
            if (manaGemEffectAvailable) rowManaGemEffectActivation = rowCount++;
            if (CalculationOptions.AoeDuration > 0)
            {
                rowAoe = rowCount++;
                rowFlamestrike = rowCount++;
                rowConeOfCold = rowCount++;
                if (MageTalents.BlastWave == 1) rowBlastWave = rowCount++;
                if (MageTalents.DragonsBreath == 1) rowDragonsBreath = rowCount++;
            }
            if (combustionAvailable) rowCombustion = rowCount++;
            if (combustionAvailable && moltenFuryAvailable) rowMoltenFuryCombustion = rowCount++;
            if (combustionAvailable && heroismAvailable) rowHeroismCombustion = rowCount++;
            //if (berserkingAvailable && moltenFuryAvailable) rowMoltenFuryBerserking = rowCount++;
            //if (berserkingAvailable && heroismAvailable) rowHeroismBerserking = rowCount++;
            //if (drumsOfBattleAvailable && icyVeinsAvailable) rowIcyVeinsDrumsOfBattle = rowCount++;
            //if (drumsOfBattleAvailable && arcanePowerAvailable) rowArcanePowerDrumsOfBattle = rowCount++;
            if (CalculationOptions.TpsLimit > 0f) rowThreat = rowCount++;
            //if (berserkingAvailable) rowBerserking = rowCount++;
            if (needsTimeExtension) rowTimeExtension = rowCount++;
            if (afterFightRegen) rowAfterFightRegenMana = rowCount++;
            //if (afterFightRegen) rowAfterFightRegenHealth = rowCount++;
            if (minimizeTime) rowTargetDamage = rowCount++;
            if (waterElementalAvailable && !MageTalents.GlyphOfEternalWater)
            {
                rowWaterElemental = rowCount++;
                rowSummonWaterElemental = rowCount++;
                rowSummonWaterElementalCount = rowCount++;
            }
            if (mirrorImageAvailable)
            {
                rowSummonMirrorImage = rowCount++;
                if (requiresMIP)
                {
                    rowSummonMirrorImageCount = rowCount++;
                }
            }
            if (dpsTime < 1 && (needsTimeExtension || segmentCooldowns || segmentMana))
            {
                rowDpsTime = rowCount++;
            }

            rowStackingConstraintCount = 0;
            if (rowStackingConstraint == null)
            {
                rowStackingConstraint = new StackingConstraint[8];
            }
            for (int i = 0; i < CooldownList.Count; i++)
            {
                EffectCooldown cooli = CooldownList[i];
                if (cooli.AutomaticStackingConstraints)
                {
                    for (int j = i + 1; j < CooldownList.Count; j++)
                    {
                        EffectCooldown coolj = CooldownList[j];
                        if (coolj.AutomaticStackingConstraints)
                        {
                            bool valid = true;
                            foreach (int exclusionMask in effectExclusionList)
                            {
                                if (BitCount2((cooli.Mask | coolj.Mask) & exclusionMask))
                                {
                                    valid = false;
                                    break;
                                }
                            }
                            if (valid)
                            {
                                double maxDuration = MaximizeStackingDuration(CalculationOptions.FightDuration, cooli.Duration, cooli.Cooldown, coolj.Duration, coolj.Cooldown);
                                if (maxDuration < cooli.MaximumDuration && maxDuration < coolj.MaximumDuration)
                                {
                                    int scCount = rowStackingConstraintCount;
                                    if (scCount >= rowStackingConstraint.Length)
                                    {
                                        StackingConstraint[] newArr = new StackingConstraint[rowStackingConstraint.Length * 2];
                                        Array.Copy(rowStackingConstraint, 0, newArr, 0, scCount);
                                        rowStackingConstraint = newArr;
                                    }
                                    // Heroism is before IcyVeins in cooldown list in cooldown list (initialized in InitializeEffectCooldowns)
                                    if (cooli.StandardEffect == StandardEffect.Heroism && coolj.StandardEffect == StandardEffect.IcyVeins)
                                    {
                                        rowHeroismIcyVeins = rowCount;
                                    }
                                    StackingConstraint[] arr = rowStackingConstraint;
                                    arr[scCount].Row = rowCount++;
                                    arr[scCount].MaximumStackingDuration = maxDuration;
                                    arr[scCount].Effect1 = cooli;
                                    arr[scCount].Effect2 = coolj;
                                    rowStackingConstraintCount = scCount + 1;
                                }
                            }
                        }
                    }
                }
            }

            if (coldsnapAvailable)
            {
                if (icyVeinsAvailable) rowIcyVeins = rowCount++;
                if (icyVeinsAvailable && heroismAvailable) rowHeroismIcyVeins = rowCount++;
                if (moltenFuryAvailable && icyVeinsAvailable) rowMoltenFuryIcyVeins = rowCount++;
            }
            else if (icyVeinsAvailable)
            {
                rowIcyVeins = EffectCooldown[(int)StandardEffect.IcyVeins].Row;
            }
            if (heroismAvailable) rowHeroism = EffectCooldown[(int)StandardEffect.Heroism].Row;
            if (arcanePowerAvailable) rowArcanePower = EffectCooldown[(int)StandardEffect.ArcanePower].Row;
            if (powerInfusionAvailable) rowPowerInfusion = EffectCooldown[(int)StandardEffect.PowerInfusion].Row;
            if (flameCapAvailable) rowFlameCap = EffectCooldown[(int)StandardEffect.FlameCap].Row;
            if (berserkingAvailable) rowBerserking = EffectCooldown[(int)StandardEffect.Berserking].Row;
            if (mirrorImageAvailable) rowMirrorImage = EffectCooldown[(int)StandardEffect.MirrorImage].Row;


            //rowManaPotionManaGem = rowCount++;
            if (segmentCooldowns)
            {
                rowCount = ConstructSegmentationRows(rowCount);
            }
            if (segmentCooldowns || segmentMana)
            {
                if (restrictManaUse)
                {
                    int segments = segmentCooldowns ? SegmentList.Count : 1;
                    rowSegmentManaOverflow = rowCount;
                    rowCount += segments * manaSegments - 1;
                    rowSegmentManaUnderflow = rowCount;
                    rowCount += segments * manaSegments - 1;
                }
            }
            return rowCount;
        }

        private int ConstructSegmentationRows(int rowCount)
        {
            // mf, heroism, ap, iv, combustion, drums, flamecap, destruction, t1, t2
            // mf
            // heroism
            // ap
            if (arcanePowerAvailable)
            {
                List<SegmentConstraint> list = rowSegmentArcanePower = new List<SegmentConstraint>();
                double cool = ArcanePowerCooldown;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            // pi
            if (powerInfusionAvailable)
            {
                List<SegmentConstraint> list = rowSegmentPowerInfusion = new List<SegmentConstraint>();
                double cool = PowerInfusionCooldown;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            // iv
            if (icyVeinsAvailable)
            {
                List<SegmentConstraint> list = rowSegmentIcyVeins = new List<SegmentConstraint>();
                double cool = IcyVeinsCooldown + (coldsnapAvailable ? 20 : 0);
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            // combustion
            if (combustionAvailable)
            {
                List<SegmentConstraint> list = rowSegmentCombustion = new List<SegmentConstraint>();
                double cool = CombustionCooldown + 15;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            // berserking
            if (berserkingAvailable)
            {
                List<SegmentConstraint> list = rowSegmentBerserking = new List<SegmentConstraint>();
                double cool = 120;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            if (waterElementalAvailable && !MageTalents.GlyphOfEternalWater)
            {
                List<SegmentConstraint> list = rowSegmentWaterElemental = new List<SegmentConstraint>();
                double cool = WaterElementalCooldown + (coldsnapAvailable ? WaterElementalDuration : 0.0);
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
                list = rowSegmentSummonWaterElemental = new List<SegmentConstraint>();
                cool = WaterElementalCooldown + (coldsnapAvailable ? WaterElementalDuration : 0.0);
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            if (mirrorImageAvailable)
            {
                List<SegmentConstraint> list = rowSegmentMirrorImage = new List<SegmentConstraint>();
                double cool = MirrorImageCooldown;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
                list = rowSegmentSummonMirrorImage = new List<SegmentConstraint>();
                cool = MirrorImageCooldown;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            // flamecap & mana gem
            if (flameCapAvailable)
            {
                List<SegmentConstraint> list = rowSegmentFlameCap = new List<SegmentConstraint>();
                double cool = 180;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            if (CalculationOptions.ManaGemEnabled)
            {
                List<SegmentConstraint> list = rowSegmentManaGem = new List<SegmentConstraint>();
                double cool = 120;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            // effect potion
            /*if (effectPotionAvailable)
            {
                rowSegmentPotion = rowCount;
                for (int seg = 0; seg < segments; seg++)
                {
                    rowCount++;
                    double cool = 120;
                    if (seg * segmentDuration + cool >= calculationOptions.FightDuration) break;
                }
            }*/
            for (int i = 0; i < ItemBasedEffectCooldownsCount; i++)
            {
                EffectCooldown cooldown = ItemBasedEffectCooldowns[i];
                if (cooldown.SegmentConstraints == null) // if there's existing one we guarantee that it is cleared
                {
                    cooldown.SegmentConstraints = new List<SegmentConstraint>();
                }
                List<SegmentConstraint> list = cooldown.SegmentConstraints;
                double cool = cooldown.Cooldown;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            // mana gem effect
            if (manaGemEffectAvailable)
            {
                List<SegmentConstraint> list = rowSegmentManaGemEffect = new List<SegmentConstraint>();
                double cool = 120.0;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            if (evocationAvailable)
            {
                List<SegmentConstraint> list = rowSegmentEvocation = new List<SegmentConstraint>();
                double cool = EvocationCooldown;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            // max segment time
            rowSegment = rowCount;
            rowCount += SegmentList.Count;
            // mana overflow & underflow (don't need over all segments, that is already verified)
            if (restrictThreat)
            {
                rowSegmentThreat = rowCount;
                rowCount += SegmentList.Count - 1;
            }
            return rowCount;
        }

        private void SetSpellColumn(bool minimizeTime, int segment, int manaSegment, CastingState state, int column, Cycle cycle, float multiplier)
        {
            double bound = CalculationOptions.FightDuration;
            double manaRegen = cycle.ManaPerSecond;
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, manaRegen);
            lp.SetElementUnsafe(rowManaRegen, column, manaRegen);
            lp.SetElementUnsafe(rowFightDuration, column, 1.0);
            lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
            if (state.PotionOfWildMagic || state.PotionOfSpeed)
            {
                lp.SetElementUnsafe(rowPotion, column, 1.0 / 15.0);
            }
            if (state.WaterElemental && !MageTalents.GlyphOfEternalWater)
            {
                lp.SetElementUnsafe(rowWaterElemental, column, 1.0);
                lp.SetElementUnsafe(rowSummonWaterElemental, column, 1 / (WaterElementalDuration - BaseGlobalCooldown));
            }
            if (state.MirrorImage)
            {
                lp.SetElementUnsafe(rowMirrorImage, column, 1.0);
                lp.SetElementUnsafe(rowSummonMirrorImage, column, 1 / (MirrorImageDuration - BaseGlobalCooldown));
            }
            if (state.Heroism) lp.SetElementUnsafe(rowHeroism, column, 1.0);
            if (state.ArcanePower) lp.SetElementUnsafe(rowArcanePower, column, 1.0);
            if (state.PowerInfusion) lp.SetElementUnsafe(rowPowerInfusion, column, 1.0);
            if (state.MoltenFury) lp.SetElementUnsafe(rowMoltenFury, column, 1.0);
            //if (state.Heroism && state.ArcanePower) lp.SetElementUnsafe(rowHeroismArcanePower, column, 1.0);
            //if (state.Heroism && state.ManaGemEffect) lp.SetElementUnsafe(rowHeroismManaGemEffect, column, 1.0);
            if (state.IcyVeins)
            {
                lp.SetElementUnsafe(rowIcyVeins, column, 1.0);
                if (state.MoltenFury) lp.SetElementUnsafe(rowMoltenFuryIcyVeins, column, 1.0);
                if (coldsnapAvailable && state.Heroism)
                {
                    // VERY IMPORTANT!!!
                    // you're only allowed to set this for coldsnap case, because otherwise it's already handled
                    // by automatic constraints and those already include it
                    // otherwise you end with two identical entries in sparse matrix which creates problems
                    lp.SetElementUnsafe(rowHeroismIcyVeins, column, 1.0);
                }
            }
            //if (state.MoltenFury && state.PotionOfWildMagic) lp.SetElementUnsafe(rowMoltenFuryDestructionPotion, column, 1.0);
            //if (state.MoltenFury && state.ManaGemEffect) lp.SetElementUnsafe(rowMoltenFuryManaGemEffect, column, 1.0);
            //if (state.PotionOfWildMagic && state.Heroism) lp.SetElementUnsafe(rowHeroismDestructionPotion, column, 1.0);
            //if (state.PotionOfWildMagic && state.IcyVeins) lp.SetElementUnsafe(rowIcyVeinsDestructionPotion, column, 1.0);
            if (state.FlameCap) lp.SetElementUnsafe(rowFlameCap, column, 1.0);
            //if (state.MoltenFury && state.FlameCap) lp.SetElementUnsafe(rowMoltenFuryFlameCap, column, 1.0);
            //if (state.PotionOfWildMagic && state.FlameCap) lp.SetElementUnsafe(rowFlameCapDestructionPotion, column, 1.0);
            for (int i = 0; i < ItemBasedEffectCooldownsCount; i++)
            {
                EffectCooldown cooldown = ItemBasedEffectCooldowns[i];
                if (state.EffectsActive(cooldown.Mask))
                {
                    lp.SetElementUnsafe(cooldown.Row, column, 1.0);
                }
            }
            if (state.ManaGemEffect) lp.SetElementUnsafe(rowManaGemEffectActivation, column, 1 / ManaGemEffectDuration);
            if (cycle.AreaEffect)
            {
                lp.SetElementUnsafe(rowAoe, column, 1.0);
                Spell fs = cycle.AoeSpell;
                if (fs.SpellTemplate is FlamestrikeTemplate)
                {
                    if (!fs.SpammedDot) lp.SetElementUnsafe(rowFlamestrike, column, fs.DotDuration / fs.CastTime);
                }
                else
                {
                    lp.SetElementUnsafe(rowFlamestrike, column, -1.0);
                }
                Spell coc = cycle.AoeSpell;
                if (coc.SpellTemplate is ConeOfColdTemplate)
                {
                    lp.SetElementUnsafe(rowConeOfCold, column, (coc.Cooldown / coc.CastTime - 1.0));
                }
                else
                {
                    lp.SetElementUnsafe(rowConeOfCold, column, -1.0);
                }
                Spell bw = cycle.AoeSpell;
                if (bw.SpellTemplate is BlastWaveTemplate)
                {
                    lp.SetElementUnsafe(rowBlastWave, column, (bw.Cooldown / bw.CastTime - 1.0));
                }
                else
                {
                    lp.SetElementUnsafe(rowBlastWave, column, -1);
                }
                Spell db = cycle.AoeSpell;
                if (db.SpellTemplate is DragonsBreathTemplate)
                {
                    lp.SetElementUnsafe(rowDragonsBreath, column, (db.Cooldown / db.CastTime - 1.0));
                }
                else
                {
                    lp.SetElementUnsafe(rowDragonsBreath, column, -1.0);
                }
            }
            if (state.Combustion)
            {
                lp.SetElementUnsafe(rowCombustion, column, (1 / (state.CombustionDuration * cycle.CastTime / cycle.CastProcs)));
                if (state.MoltenFury) lp.SetElementUnsafe(rowMoltenFuryCombustion, column, (1 / (state.CombustionDuration * cycle.CastTime / cycle.CastProcs)));
                if (state.Heroism) lp.SetElementUnsafe(rowHeroismCombustion, column, (1 / (state.CombustionDuration * cycle.CastTime / cycle.CastProcs)));
            }
            if (state.Berserking) lp.SetElementUnsafe(rowBerserking, column, 1.0);
            //if (state.Berserking && state.MoltenFury) lp.SetElementUnsafe(rowMoltenFuryBerserking, column, 1.0);
            //if (state.Berserking && state.Heroism) lp.SetElementUnsafe(rowHeroismBerserking, column, 1.0);
            //if (state.Berserking && state.IcyVeins) lp.SetElementUnsafe(rowIcyVeinsBerserking, column, 1.0);
            //if (state.Berserking && state.ArcanePower) lp.SetElementUnsafe(rowArcanePowerBerserking, column, 1.0);
            lp.SetElementUnsafe(rowThreat, column, cycle.ThreatPerSecond);
            //lp[rowManaPotionManaGem, index] = (statsList[buffset].FlameCap ? 1 : 0) + (statsList[buffset].DestructionPotion ? 40.0 / 15.0 : 0);
            if (needsQuadratic)
            {
                float dps = cycle.GetDamagePerSecond(ManaAdeptBonus);
                lp.SetElementUnsafe(rowTargetDamage, column, -dps * multiplier);
                lp.SetCostUnsafe(column, minimizeTime ? -1 : dps * multiplier);
                lp.SetSpellDpsUnsafe(column, cycle.GetSpellDamagePerSecond() * multiplier);
            }
            else
            {
                lp.SetElementUnsafe(rowTargetDamage, column, -cycle.DamagePerSecond * multiplier);
                lp.SetCostUnsafe(column, minimizeTime ? -1 : cycle.DamagePerSecond * multiplier);
            }

            for (int i = 0; i < rowStackingConstraintCount; i++)
            {
                //if (state.EffectsActive(rowStackingConstraint[i].Effect1.Mask | rowStackingConstraint[i].Effect2.Mask))
                int effects = rowStackingConstraint[i].Effect1.Mask | rowStackingConstraint[i].Effect2.Mask;
                if ((effects & state.Effects) == effects)
                {
                    lp.SetElementUnsafe(rowStackingConstraint[i].Row, column, 1.0);
                }
            }

            if (segmentCooldowns)
            {
                bound = SetSpellColumnSegment(segment, manaSegment, state, column, cycle, bound, manaRegen);
            }
            if (restrictManaUse)
            {
                for (int ss = segment * manaSegments + manaSegment; ss < SegmentList.Count * manaSegments - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentManaUnderflow + ss, column, manaRegen);
                    lp.SetElementUnsafe(rowSegmentManaOverflow + ss, column, -manaRegen);
                }
            }
            lp.SetColumnUpperBound(column, bound);
        }

        private double SetSpellColumnSegment(int segment, int manaSegment, CastingState state, int column, Cycle cycle, double bound, double manaRegen)
        {
            // mf, heroism, ap, iv, combustion, drums, flamecap, destro, t1, t2
            //lp[rowOffset + 1 * segments + seg, index] = 1;
            if (state.ArcanePower)
            {
                bound = Math.Min(bound, ArcanePowerDuration);
                foreach (SegmentConstraint constraint in rowSegmentArcanePower)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (state.PowerInfusion)
            {
                bound = Math.Min(bound, PowerInfusionDuration);
                foreach (SegmentConstraint constraint in rowSegmentPowerInfusion)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (state.IcyVeins)
            {
                bound = Math.Min(bound, (coldsnapAvailable) ? 40.0 : 20.0);
                foreach (SegmentConstraint constraint in rowSegmentIcyVeins)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (state.WaterElemental)
            {
                foreach (SegmentConstraint constraint in rowSegmentWaterElemental)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (state.MirrorImage)
            {
                foreach (SegmentConstraint constraint in rowSegmentMirrorImage)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (state.Combustion)
            {
                foreach (SegmentConstraint constraint in rowSegmentCombustion)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (state.Berserking)
            {
                foreach (SegmentConstraint constraint in rowSegmentBerserking)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (state.FlameCap)
            {
                foreach (SegmentConstraint constraint in rowSegmentFlameCap)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (state.PotionOfWildMagic || state.PotionOfSpeed)
            {
                bound = Math.Min(bound, 15.0);
                /*for (int ss = 0; ss < segments; ss++)
                {
                    double cool = 120;
                    int maxs = (int)Math.Floor(ss + cool / segmentDuration) - 1;
                    if (ss * segmentDuration + cool >= calculationOptions.FightDuration) maxs = segments - 1;
                    if (segment >= ss && segment <= maxs) lp.SetElementUnsafe(rowSegmentPotion + ss, column, 1.0);
                    if (ss * segmentDuration + cool >= calculationOptions.FightDuration) break;
                }*/
            }
            for (int i = 0; i < ItemBasedEffectCooldownsCount; i++)
            {
                EffectCooldown cooldown = ItemBasedEffectCooldowns[i];
                if (state.EffectsActive(cooldown.Mask))
                {
                    bound = Math.Min(bound, cooldown.Duration);
                    foreach (SegmentConstraint constraint in cooldown.SegmentConstraints)
                    {
                        if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                    }
                }
            }
            if (state.ManaGemEffect)
            {
                bound = Math.Min(bound, ManaGemEffectDuration);
                foreach (SegmentConstraint constraint in rowSegmentManaGemEffect)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (segmentNonCooldowns || state != BaseState)
            {
                bound = Math.Min(bound, SegmentList[segment].Duration);
                lp.SetElementUnsafe(rowSegment + segment, column, 1.0);
            }
            if (restrictThreat)
            {
                for (int ss = segment; ss < SegmentList.Count - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentThreat + ss, column, cycle.ThreatPerSecond);
                }
            }
            return bound;
        }

        private void GenerateSpellList()
        {
            if (spellList == null)
            {
                spellList = new List<CycleId>();
            }
            else
            {
                spellList.Clear();
            }

            if (CalculationOptions.CustomSpellMixEnabled || CalculationOptions.CustomSpellMixOnly)
            {
                spellList.Add(CycleId.CustomSpellMix);
            }
            if (!CalculationOptions.CustomSpellMixOnly)
            {
                if (CalculationOptions.MaintainScorch && CalculationOptions.MaintainSnare && MageTalents.ImprovedScorch > 0 && MageTalents.Slow > 0)
                {
                    // no cycles right now that provide scorch and snare
                }
                if (CalculationOptions.MaintainScorch && MageTalents.ImprovedScorch > 0)
                {
                    if (useGlobalOptimizations)
                    {
                        if (MageTalents.PiercingIce == 3 && MageTalents.IceShards == 3 && CalculationOptions.PlayerLevel >= 75)
                        {
                            if (MageTalents.LivingBomb > 0)
                            {
                                spellList.Add(CycleId.FFBScLBPyro);
                            }
                            spellList.Add(CycleId.FFBScPyro);
                        }
                        else
                        {
                            if (MageTalents.LivingBomb > 0)
                            {
                                spellList.Add(CycleId.FBScLBPyro);
                            }
                            spellList.Add(CycleId.FBScPyro);
                        }
                    }
                    else
                    {
                        if (MageTalents.LivingBomb > 0)
                        {
                            spellList.Add(CycleId.FBScLBPyro);
                            spellList.Add(CycleId.ScLBPyro);
                        }
                        spellList.Add(CycleId.FBScPyro);
                        if (CalculationOptions.PlayerLevel >= 75)
                        {
                            spellList.Add(CycleId.FFBScPyro);
                            if (MageTalents.LivingBomb > 0)
                            {
                                spellList.Add(CycleId.FFBScLBPyro);
                            }
                        }
                    }
                }
                // deprecated, if there is demand for this we have to create completely new cycles
                /*else if (calculationOptions.MaintainSnare && talents.Slow > 0)
                {
                    if (useGlobalOptimizations)
                    {
                        if (talents.ArcaneBarrage > 0)
                        {
                            if (talents.ImprovedFrostbolt > 0)
                            {
                                list.Add(CycleId.FrBABarSlow);
                            }
                            if (talents.ImprovedFireball > 0)
                            {
                                list.Add(CycleId.FBABarSlow);
                            }
                            if (talents.ArcaneEmpowerment > 0)
                            {
                                list.Add(CycleId.ABABarSlow);
                            }
                            if (talents.ImprovedFrostbolt == 0 && talents.ImprovedFireball == 0 && talents.ArcaneEmpowerment == 0)
                            {
                                list.Add(CycleId.FrBABarSlow);
                                list.Add(CycleId.FBABarSlow);
                                list.Add(CycleId.ABABarSlow);
                            }
                        }
                    }
                    else
                    {
                        list.Add(CycleId.FrBABarSlow);
                        list.Add(CycleId.FBABarSlow);
                        list.Add(CycleId.ABABarSlow);
                    }
                }*/
                else
                {
                    if (useGlobalOptimizations)
                    {
                        if (MageTalents.EmpoweredFire > 0)
                        {
                            if (MageTalents.PiercingIce == 3 && MageTalents.IceShards == 3 && CalculationOptions.PlayerLevel >= 75)
                            {
                                spellList.Add(CycleId.FFBPyro);
                                if (MageTalents.LivingBomb > 0) spellList.Add(CycleId.FFBLBPyro);
                            }
                            else
                            {
                                if (MageTalents.HotStreak > 0 && MageTalents.Pyroblast > 0)
                                {
                                    spellList.Add(CycleId.FBPyro);
                                }
                                else
                                {
                                    spellList.Add(CycleId.Fireball);
                                }
                                if (MageTalents.LivingBomb > 0) spellList.Add(CycleId.FBLBPyro);
                            }
                        }
                        else if (MageTalents.EmpoweredFrostbolt > 0)
                        {
                            if (MageTalents.BrainFreeze > 0)
                            {
                                spellList.Add(CycleId.FrBFB);
                                spellList.Add(CycleId.FrBFBIL);
                                spellList.Add(CycleId.FrBILFB);
                            }
                            if (MageTalents.DeepFreeze > 0)
                            {
                                spellList.Add(CycleId.FrBDFFBIL);
                                spellList.Add(CycleId.FrBDFFFB);
                            }
                            spellList.Add(CycleId.FrBIL);
                            spellList.Add(CycleId.FrostboltFOF);
                        }
                        else if (MageTalents.ArcaneEmpowerment > 0)
                        {
                            spellList.Add(CycleId.AB2AM);
                            spellList.Add(CycleId.AB3AM023MBAM);
                            spellList.Add(CycleId.AB4AM0234MBAM);
                            if (MageTalents.MissileBarrage > 0)
                            {
                                spellList.Add(CycleId.ABSpam0234MBAM);
                                spellList.Add(CycleId.ABSpam024MBAM);
                                spellList.Add(CycleId.ABSpam034MBAM);
                                spellList.Add(CycleId.ABSpam04MBAM);
                            }
                            spellList.Add(CycleId.ArcaneBlastSpam);
                        }
                        else
                        {
                            spellList.Add(CycleId.ArcaneMissiles);
                            spellList.Add(CycleId.Fireball);
                            spellList.Add(CycleId.FrostboltFOF);
                            if (CalculationOptions.PlayerLevel >= 75) spellList.Add(CycleId.FrostfireBoltFOF);
                        }
                    }
                    else
                    {
                        spellList.Add(CycleId.ArcaneMissiles);
                        spellList.Add(CycleId.Scorch);
                        if (MageTalents.LivingBomb > 0) spellList.Add(CycleId.ScLBPyro);
                        if (MageTalents.HotStreak > 0 && MageTalents.Pyroblast > 0)
                        {
                            spellList.Add(CycleId.FBPyro);
                        }
                        else
                        {
                            spellList.Add(CycleId.Fireball);
                        }
                        if (CalculationOptions.PlayerLevel >= 75)
                        {
                            spellList.Add(CycleId.FrostfireBoltFOF);
                            spellList.Add(CycleId.FFBPyro);
                            if (MageTalents.LivingBomb > 0) spellList.Add(CycleId.FFBLBPyro);
                        }
                        if (MageTalents.LivingBomb > 0) spellList.Add(CycleId.FBLBPyro);
                        spellList.Add(CycleId.FrostboltFOF);
                        if (MageTalents.BrainFreeze > 0) spellList.Add(CycleId.FrBFB);
                        if (MageTalents.FingersOfFrost > 0)
                        {
                            if (MageTalents.BrainFreeze > 0)
                            {
                                spellList.Add(CycleId.FrBFBIL);
                                spellList.Add(CycleId.FrBILFB);
                            }
                            spellList.Add(CycleId.FrBIL);
                            if (MageTalents.DeepFreeze > 0)
                            {
                                spellList.Add(CycleId.FrBDFFBIL);
                                spellList.Add(CycleId.FrBDFFFB);
                            }
                        }
                        spellList.Add(CycleId.AB2AM);
                        spellList.Add(CycleId.AB3AM023MBAM);
                        spellList.Add(CycleId.AB4AM0234MBAM);
                        if (MageTalents.MissileBarrage > 0)
                        {
                            spellList.Add(CycleId.ABSpam0234MBAM);
                            spellList.Add(CycleId.ABSpam024MBAM);
                            spellList.Add(CycleId.ABSpam034MBAM);
                            spellList.Add(CycleId.ABSpam04MBAM);
                        }
                        spellList.Add(CycleId.ArcaneBlastSpam);
                        /*list.Add(CycleId.ArcaneBlastSpam);
                        list.Add(CycleId.ABAM);
                        list.Add(CycleId.AB2AM);
                        list.Add(CycleId.AB3AM);
                        if (talents.MissileBarrage > 0)
                        {
                            list.Add(CycleId.AB3AM2MBAM);
                            list.Add(CycleId.ABSpam03MBAM);
                            list.Add(CycleId.ABSpam3MBAM);
                        }
                        if (talents.ArcaneBarrage > 0 && talents.MissileBarrage > 0)
                        {
                            list.Add(CycleId.ABABar0C);
                            list.Add(CycleId.ABABar1C);
                            list.Add(CycleId.ABABar0MBAM);
                            list.Add(CycleId.AB2ABar2MBAM);
                            list.Add(CycleId.AB2ABar2C);
                            list.Add(CycleId.AB2ABar3C);
                            list.Add(CycleId.AB3ABar3C);
                            list.Add(CycleId.ABSpam3C);
                            list.Add(CycleId.ABSpam03C);
                            list.Add(CycleId.ABABar3C);
                            list.Add(CycleId.ABABar2C);
                            list.Add(CycleId.ABABar2MBAM);
                            list.Add(CycleId.ABABar1MBAM);
                            list.Add(CycleId.AB3ABar3MBAM);
                            list.Add(CycleId.AB3AMABar);
                            list.Add(CycleId.AB3AMABar2C);
                        }*/
                    }
                }
                if (CalculationOptions.AoeDuration > 0)
                {
                    spellList.Add(CycleId.ArcaneExplosion);
                    spellList.Add(CycleId.FlamestrikeSpammed);
                    spellList.Add(CycleId.FlamestrikeSingle);
                    spellList.Add(CycleId.Blizzard);
                    spellList.Add(CycleId.ConeOfCold);
                    if (MageTalents.BlastWave == 1) spellList.Add(CycleId.BlastWave);
                    if (MageTalents.DragonsBreath == 1) spellList.Add(CycleId.DragonsBreath);
                }
            }            
        }

        // http://tekpool.wordpress.com/category/bit-count/
        int BitCount(int i)
        {
            uint u = (uint)i;
            int uCount = 0;

            for (; u != 0; u &= (u - 1))
                uCount++;

            return uCount;
        }

        bool BitCount2(int i)
        {
            uint u = (uint)i;

            if (u == 0) return false;
            u &= (u - 1);
            return u != 0;
        }

        private void GenerateStateList()
        {
            BaseState = CastingState.New(this, 0, false, 0);

            if (stateList == null)
            {
                stateList = new List<CastingState>(64);
            }
            else
            {
                stateList.Clear();
            }
            if (useIncrementalOptimizations)
            {
                int[] sortedStates = CalculationOptions.IncrementalSetSortedStates;
                for (int incrementalSortedIndex = 0; incrementalSortedIndex < sortedStates.Length; incrementalSortedIndex++)
                {
                    // incremental index is filtered by non-item based cooldowns
                    int incrementalSetIndex = sortedStates[incrementalSortedIndex];
                    bool mf = (incrementalSetIndex & (int)StandardEffect.MoltenFury) != 0;
                    bool heroism = (incrementalSetIndex & (int)StandardEffect.Heroism) != 0;
                    int itemBasedMax = 1 << ItemBasedEffectCooldownsCount;
                    for (int index = 0; index < itemBasedMax; index++)
                    {
                        int combinedIndex = incrementalSetIndex | (index << standardEffectCount);
                        if ((combinedIndex & availableCooldownMask) == combinedIndex) // make sure all are available
                        {
                            bool valid = true;
                            foreach (int exclusionMask in effectExclusionList)
                            {
                                if (BitCount2(combinedIndex & exclusionMask))
                                {
                                    valid = false;
                                    break;
                                }
                            }
                            if (valid)
                            {
                                if ((CalculationOptions.HeroismControl != 1 || !heroism || !mf) && (CalculationOptions.HeroismControl != 2 || !heroism || (combinedIndex == (int)StandardEffect.Heroism && index == 0)) && (CalculationOptions.HeroismControl != 3 || !moltenFuryAvailable || !heroism || mf))
                                {
                                    if (combinedIndex == 0)
                                    {
                                        stateList.Add(BaseState);
                                    }
                                    else
                                    {
                                        stateList.Add(CastingState.New(this, combinedIndex, false, 0));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int incrementalSetIndex = 0; incrementalSetIndex <= availableCooldownMask; incrementalSetIndex++)
                {
                    if (((incrementalSetIndex) & availableCooldownMask) == (incrementalSetIndex)) // make sure all are available
                    {
                        bool valid = true;
                        foreach (int exclusionMask in effectExclusionList)
                        {
                            if (BitCount2(incrementalSetIndex & exclusionMask))
                            {
                                valid = false;
                                break;
                            }
                        }
                        if (valid)
                        {
                            bool mf = (incrementalSetIndex & (int)StandardEffect.MoltenFury) != 0;
                            bool heroism = (incrementalSetIndex & (int)StandardEffect.Heroism) != 0;
                            if ((CalculationOptions.HeroismControl != 1 || !heroism || !mf) && (CalculationOptions.HeroismControl != 2 || !heroism || (incrementalSetIndex == (int)StandardEffect.Heroism)) && (CalculationOptions.HeroismControl != 3 || !moltenFuryAvailable || !heroism || mf))
                            {
                                if (incrementalSetIndex == 0)
                                {
                                    stateList.Add(BaseState);
                                }
                                else
                                {
                                    stateList.Add(CastingState.New(this, incrementalSetIndex, false, 0));
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Quadratic Solver
        private void SolveQuadratic()
        {
            // for now requires solution variables to work
            int[] sort = new int[lp.Columns];
            for (int j = 0; j < lp.Columns; j++)
            {
                sort[j] = j;
            }
            Array.Sort(sort, (x, y) =>
            {
                SolutionVariable vx = SolutionVariable[x];
                SolutionVariable vy = SolutionVariable[y];
                if (vx.State != null && vx.State.Evocation && vy.Type == VariableType.ManaOverflow && vx.Segment == vy.Segment && vx.ManaSegment == vy.ManaSegment + 1)
                {
                    return -1;
                }
                if (vy.State != null && vy.State.Evocation && vx.Type == VariableType.ManaOverflow && vx.Segment == vy.Segment && vy.ManaSegment == vx.ManaSegment + 1)
                {
                    return 1;
                }
                int comp = vx.Segment.CompareTo(vy.Segment);
                if (comp != 0) return comp;
                comp = vx.ManaSegment.CompareTo(vy.ManaSegment);
                if (comp != 0) return comp;
                if (vx.Dps == 0.0)
                {
                    if (vy.Dps == 0.0)
                    {
                        return vx.Mps.CompareTo(vy.Mps);
                    }
                    else
                    {
                        if (vx.Mps <= 0)
                        {
                            return -1;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
                else if (vy.Dps == 0.0)
                {
                    if (vy.Mps <= 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                return vx.Mps.CompareTo(vy.Mps);
            });

            lp.SolvePrimalQuadratic(rowManaRegen, sort, ManaAdeptBonus / (BaseStats.Mana * ManaRegenLPScaling), useIncrementalOptimizations, CalculationOptions.TargetDamage > 0 ? lp.Columns + rowFightDuration : -1, CalculationOptions.TargetDamage);
        }
        #endregion

        #region Calculation Result
        private DisplayCalculations GetDisplayCalculations(CharacterCalculationsMage baseCalculations)
        {
            DisplayCalculations displayCalculations = new DisplayCalculations();

            displayCalculations.BaseStats = BaseStats;
            displayCalculations.BaseState = BaseState;
            displayCalculations.Character = Character;
            displayCalculations.CalculationOptions = CalculationOptions;
            displayCalculations.SolutionVariable = SolutionVariable;
            displayCalculations.MageTalents = MageTalents;
            displayCalculations.Solution = solution;
            displayCalculations.BaseCalculations = baseCalculations;

            displayCalculations.RawArcaneHitRate = RawArcaneHitRate;
            displayCalculations.RawFireHitRate = RawFireHitRate;
            displayCalculations.RawFrostHitRate = RawFrostHitRate;

            displayCalculations.CooldownList = new List<EffectCooldown>(CooldownList);
            displayCalculations.EffectCooldown = new Dictionary<int,EffectCooldown>(EffectCooldown);
            displayCalculations.ItemBasedEffectCooldowns = new EffectCooldown[ItemBasedEffectCooldownsCount];
            Array.Copy(ItemBasedEffectCooldowns, 0, displayCalculations.ItemBasedEffectCooldowns, 0, ItemBasedEffectCooldownsCount);

            displayCalculations.SegmentList = new List<Segment>(SegmentList);

            displayCalculations.SpellPowerEffects = new SpecialEffect[SpellPowerEffectsCount];
            Array.Copy(SpellPowerEffects, 0, displayCalculations.SpellPowerEffects, 0, SpellPowerEffectsCount);
            displayCalculations.HasteRatingEffects = new SpecialEffect[HasteRatingEffectsCount];
            Array.Copy(HasteRatingEffects, 0, displayCalculations.HasteRatingEffects, 0, HasteRatingEffectsCount);

            displayCalculations.BaseGlobalCooldown = BaseGlobalCooldown;
            displayCalculations.ManaAdeptBonus = ManaAdeptBonus;

            displayCalculations.EvocationDuration = EvocationDuration;
            displayCalculations.EvocationRegen = EvocationRegen;
            displayCalculations.EvocationDurationIV = EvocationDurationIV;
            displayCalculations.EvocationRegenIV = EvocationRegenIV;
            displayCalculations.EvocationDurationHero = EvocationDurationHero;
            displayCalculations.EvocationRegenHero = EvocationRegenHero;
            displayCalculations.EvocationDurationIVHero = EvocationDurationIVHero;
            displayCalculations.EvocationRegenIVHero = EvocationRegenIVHero;

            displayCalculations.MaxManaGem = MaxManaGem;
            displayCalculations.MaxEvocation = MaxEvocation;
            displayCalculations.ManaGemTps = ManaGemTps;
            displayCalculations.ManaPotionTps = ManaPotionTps;
            displayCalculations.ManaGemValue = ManaGemValue;
            displayCalculations.MaxManaGemValue = MaxManaGemValue;
            displayCalculations.ManaPotionValue = ManaPotionValue;
            displayCalculations.MaxManaPotionValue = MaxManaPotionValue;

            displayCalculations.CombustionCooldown = CombustionCooldown;
            displayCalculations.PowerInfusionDuration = PowerInfusionDuration;
            displayCalculations.PowerInfusionCooldown = PowerInfusionCooldown;
            displayCalculations.MirrorImageDuration = MirrorImageDuration;
            displayCalculations.MirrorImageCooldown = MirrorImageCooldown;
            displayCalculations.IcyVeinsCooldown = IcyVeinsCooldown;
            displayCalculations.ColdsnapCooldown = ColdsnapCooldown;
            displayCalculations.ArcanePowerCooldown = ArcanePowerCooldown;
            displayCalculations.ArcanePowerDuration = ArcanePowerDuration;
            displayCalculations.WaterElementalCooldown = WaterElementalCooldown;
            displayCalculations.WaterElementalDuration = WaterElementalDuration;
            displayCalculations.EvocationCooldown = EvocationCooldown;
            displayCalculations.ManaGemEffectDuration = ManaGemEffectDuration;

            displayCalculations.StartingMana = StartingMana;
            displayCalculations.ConjureManaGem = ConjureManaGem;
            displayCalculations.MaxConjureManaGem = MaxConjureManaGem;
            displayCalculations.ManaGemEffect = manaGemEffectAvailable;
            displayCalculations.ChanceToDie = ChanceToDie;
            displayCalculations.MeanIncomingDps = MeanIncomingDps;
            displayCalculations.MageArmor = armor;
            displayCalculations.DamageTakenReduction = DamageTakenReduction;
            displayCalculations.Wand = Wand;

            if (!requiresMIP)
            {
                displayCalculations.UpperBound = lp.Value;
                displayCalculations.LowerBound = 0.0;
            }
            else
            {
                displayCalculations.UpperBound = upperBound;
                if (integralMana && segmentCooldowns && advancedConstraintsLevel >= 5) displayCalculations.LowerBound = lowerBound;
            }

            float threat = 0;
            for (int i = 0; i < SolutionVariable.Count; i++)
            {
                threat += (float)(SolutionVariable[i].Tps * solution[i]);
            }
            displayCalculations.Tps = threat / CalculationOptions.FightDuration;

            return displayCalculations;
        }

        private OptimizableCalculations GetOptimizableCalculations()
        {
            OptimizableCalculations optimizableCalculations = new OptimizableCalculations();

            optimizableCalculations.ArcaneResistance = BaseStats.ArcaneResistance;
            optimizableCalculations.ChanceToDie = ChanceToDie;
            optimizableCalculations.FireResistance = BaseStats.FireResistance;
            optimizableCalculations.FrostResistance = BaseStats.FrostResistance;
            optimizableCalculations.HasteRating = BaseStats.HasteRating;
            optimizableCalculations.Health = BaseStats.Health;
            optimizableCalculations.HitRating = BaseStats.HitRating;
            optimizableCalculations.MovementSpeed = BaseStats.MovementSpeed;
            optimizableCalculations.NatureResistance = BaseStats.NatureResistance;
            optimizableCalculations.PVPTrinket = BaseStats.PVPTrinket;
            optimizableCalculations.Resilience = BaseStats.Resilience;
            optimizableCalculations.ShadowResistance = BaseStats.ShadowResistance;

            return optimizableCalculations;
        }

        private CharacterCalculationsMage GetCalculationsResult()
        {
            CharacterCalculationsMage calculationResult = new CharacterCalculationsMage();

            if (CalculationOptions.TargetDamage > 0)
            {
                calculationResult.SubPoints[0] = -(float)(CalculationOptions.TargetDamage / solution[solution.Length - 1]);
            }
            else
            {
                calculationResult.SubPoints[0] = ((float)solution[solution.Length - 1] /*+ calculationResult.WaterElementalDamage*/) / CalculationOptions.FightDuration;
            }
            calculationResult.SubPoints[1] = EvaluateSurvivability();
            calculationResult.OverallPoints = calculationResult.SubPoints[0] + calculationResult.SubPoints[1];

            if (NeedsDisplayCalculations)
            {
                calculationResult.DisplayCalculations = GetDisplayCalculations(calculationResult);
            }
            calculationResult.OptimizableCalculations = GetOptimizableCalculations();

            if (autoActivatedBuffs != null)
            {
                calculationResult.AutoActivatedBuffs.AddRange(autoActivatedBuffs);
            }

            SolutionVariable = null;

            return calculationResult;
        }
        #endregion

        #region Survivability
        public void CalculateChanceToDie()
        {
            double ampMelee = IncomingDamageAmpMelee;
            double ampPhysical = IncomingDamageAmpPhysical;
            double ampArcane = IncomingDamageAmpArcane;
            double ampFire = IncomingDamageAmpFire;
            double ampFrost = IncomingDamageAmpFrost;
            double ampNature = IncomingDamageAmpNature;
            double ampShadow = IncomingDamageAmpShadow;
            double ampHoly = IncomingDamageAmpHoly;

            double melee = IncomingDamageDpsMelee;
            double physical = IncomingDamageDpsPhysical;
            double arcane = IncomingDamageDpsArcane;
            double fire = IncomingDamageDpsFire;
            double frost = IncomingDamageDpsFrost;
            double nature = IncomingDamageDpsNature;
            double shadow = IncomingDamageDpsShadow;
            double holy = IncomingDamageDpsHoly;

            double burstWindow = CalculationOptions.BurstWindow;
            double burstImpacts = CalculationOptions.BurstImpacts;

            // B(n, p) ~ N(np, np(1-p))
            // n = burstImpacts
            // Xi ~ ampi * (dpsi * (1 + B(n, criti) / n * critMulti) + doti)
            //    ~ ampi * (dpsi * (1 + N(n * criti, n * criti * (1 - criti)) / n * critMulti) + doti)
            //    ~ N(ampi * (doti + dpsi * (1 + critMulti * criti)), ampi^2 * dpsi^2 * critMulti^2 / n * criti * (1 - criti))
            // X = sum Xi ~ N(sum ampi * (doti + dpsi * (1 + critMulti * criti)), sum ampi^2 * dpsi^2 * critMulti^2 / n * criti * (1 - criti))
            // H = Health + hp5 / 5 * burstWindow
            // P(burstWindow * sum Xi >= H) = 1 - P(burstWindow * sum Xi <= H) = 1 / 2 * (1 - Erf((H - mu) / (sigma * sqrt(2)))) =
            //                = 1 / 2 * (1 - Erf((H / burstWindow - [sum ampi * (doti + dpsi * (1 + critMulti * criti))]) / sqrt(2 * [sum ampi^2 * dpsi^2 * critMulti^2 / n * criti * (1 - criti)])))

            double meleeVar = Math.Pow(ampMelee * CalculationOptions.MeleeDps * (2 * (1 - BaseState.CritDamageReduction) - 1), 2) / burstImpacts * Math.Max(0, CalculationOptions.MeleeCrit / 100.0 - BaseState.PhysicalCritReduction) * (1 - Math.Max(0, CalculationOptions.MeleeCrit / 100.0 - BaseState.PhysicalCritReduction));
            double physicalVar = Math.Pow(ampPhysical * CalculationOptions.PhysicalDps * (2 * (1 - BaseState.CritDamageReduction) - 1), 2) / burstImpacts * Math.Max(0, CalculationOptions.PhysicalCrit / 100.0 - BaseState.PhysicalCritReduction) * (1 - Math.Max(0, CalculationOptions.PhysicalCrit / 100.0 - BaseState.PhysicalCritReduction));
            double arcaneVar = Math.Pow(ampArcane * CalculationOptions.ArcaneDps * (1.75 * (1 - BaseState.CritDamageReduction) - 1), 2) / burstImpacts * Math.Max(0, CalculationOptions.ArcaneCrit / 100.0 - BaseState.SpellCritReduction) * (1 - Math.Max(0, CalculationOptions.ArcaneCrit / 100.0 - BaseState.SpellCritReduction));
            double fireVar = Math.Pow(ampFire * CalculationOptions.FireDps * (2.1 * (1 - BaseState.CritDamageReduction) - 1), 2) / burstImpacts * Math.Max(0, CalculationOptions.FireCrit / 100.0 - BaseState.SpellCritReduction) * (1 - Math.Max(0, CalculationOptions.FireCrit / 100.0 - BaseState.SpellCritReduction));
            double frostVar = Math.Pow(ampFrost * CalculationOptions.FrostDps * (2 * (1 - BaseState.CritDamageReduction) - 1), 2) / burstImpacts * Math.Max(0, CalculationOptions.FrostCrit / 100.0 - BaseState.SpellCritReduction) * (1 - Math.Max(0, CalculationOptions.FrostCrit / 100.0 - BaseState.SpellCritReduction));
            double holyVar = Math.Pow(ampHoly * CalculationOptions.HolyDps * (1.5 * (1 - BaseState.CritDamageReduction) - 1), 2) / burstImpacts * Math.Max(0, CalculationOptions.HolyCrit / 100.0 - BaseState.SpellCritReduction) * (1 - Math.Max(0, CalculationOptions.HolyCrit / 100.0 - BaseState.SpellCritReduction));
            double natureVar = Math.Pow(ampNature * CalculationOptions.NatureDps * (2 * (1 - BaseState.CritDamageReduction) - 1), 2) / burstImpacts * Math.Max(0, CalculationOptions.NatureCrit / 100.0 - BaseState.SpellCritReduction) * (1 - Math.Max(0, CalculationOptions.NatureCrit / 100.0 - BaseState.SpellCritReduction));
            double shadowVar = Math.Pow(ampShadow * CalculationOptions.ShadowDps * (2 * (1 - BaseState.CritDamageReduction) - 1), 2) / burstImpacts * Math.Max(0, CalculationOptions.ShadowCrit / 100.0 - BaseState.SpellCritReduction) * (1 - Math.Max(0, CalculationOptions.ShadowCrit / 100.0 - BaseState.SpellCritReduction));

            double Xmean = melee + physical + arcane + fire + frost + holy + nature + shadow;
            double Xvar = meleeVar + physicalVar + arcaneVar + fireVar + frostVar + holyVar + natureVar + shadowVar;

            // T = healing response time ~ N(Tmean, Tvar)
            // T * X ~ N(Tmean * Xmean, Tvar * Xvar + Tmean^2 * Xvar + Xmean^2 * Tvar)   // approximation reasonable for high Tmean / sqrt(Tvar)
            // P(T * X >= H) = 1 / 2 * (1 - Erf((H - mean) / (sigma * sqrt(2)))) =
            //               = 1 / 2 * (1 - Erf((H - mean) / sqrt(2 * var)))
            //               = 1 / 2 * (1 - Erf((H - Tmean * Xmean) / sqrt(2 * (Tvar * Xvar + Tmean^2 * Xvar + Xmean^2 * Tvar))))

            // Tvar := Tk * Tmean^2,   Tk <<< 1

            // P(T * X >= H) = 1 / 2 * (1 - Erf((H / Tmean - Xmean) / sqrt(2 * (Xvar * (Tk + 1) + Xmean^2 * Tk))))

            double Tk = 0.01;

            ChanceToDie = (float)(0.5f * (1f - SpecialFunction.Erf((BaseStats.Health / burstWindow + BaseStats.Hp5 / 5 - Xmean) / Math.Sqrt(2 * (Xvar * (1 + Tk) + Xmean * Xmean * Tk)))));
            MeanIncomingDps = (float)Xmean;
        }

        private float EvaluateSurvivability()
        {
            float ret = BaseStats.Health * CalculationOptions.SurvivabilityRating;
            if (CalculationOptions.ChanceToLiveScore > 0 || NeedsDisplayCalculations)
            {
                CalculateChanceToDie();

                //double maxTimeToDie = 1.0 / (1 - calculationOptions.ChanceToLiveLimit / 100.0) - 1;
                //double timeToDie = Math.Min(1.0 / calculatedStats.ChanceToDie - 1, maxTimeToDie);

                //calculatedStats.SubPoints[1] = calculatedStats.BasicStats.Health * calculationOptions.SurvivabilityRating + (float)(calculationOptions.ChanceToLiveScore * timeToDie / maxTimeToDie);
                ret += (float)(CalculationOptions.ChanceToLiveScore * Math.Pow(1 - ChanceToDie, CalculationOptions.ChanceToLiveAttenuation));
                if (float.IsNaN(ret)) ret = 0f;
            }
            else
            {
                ChanceToDie = 0f;
                MeanIncomingDps = 0f;
            }
            return ret;
        }
        #endregion
    }
}
