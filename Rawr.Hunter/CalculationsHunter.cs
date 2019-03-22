using System;
using System.Collections.Generic;
#if RAWR3 || SILVERLIGHT
using System.Windows.Media;
#else
using System.Drawing;
#endif
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using Rawr.Hunter.Skills;

namespace Rawr.Hunter {
    public struct HunterCharacter
    {
        public Character Char;
        public Rotation Rot;
        public CombatFactors combatFactors;
        public CalculationOptionsHunter calcOpts;
    }
    [Rawr.Calculations.RawrModelInfo("Hunter", "Inv_Weapon_Bow_07", CharacterClass.Hunter)]
    public class CalculationsHunter : CalculationsBase {
        #region Variables and Properties

        public override List<GemmingTemplate> DefaultGemmingTemplates {
            get {
                // == Relevant Gem IDs for Hunters ==
                #region Red
                int[] deli = { 41434, 39997, 40112, 42143 }; // Agi
                int[] brit = { 39906, 39999, 40114, 36766 }; // AP
                int[] frac = { 39909, 40002, 40117, 42153 }; // ArP
                #endregion
                #region Purple
                int[] shif = { 41460, 40023, 40130, 40130 }; // Agi  /Stam
                int[] bala = { 41450, 40029, 40136, 40136 }; // AP   /Stam
                int[] puis = { 41456, 40033, 40140, 40140 }; // ArP  /Stam
                int[] infd = { 41454, 40030, 40137, 40137 }; // AP   /Mp5
                int[] tenu = { 41462, 40024, 40131, 40131 }; // Agi  /Mp5
                #endregion
                #region Blue
                int[] lust = { 41440, 40010, 40121, 42146 }; // Mp5
                #endregion
                #region Green
                int[] vivd = { 41481, 40088, 40166, 40166 }; // Hit  /Stam
                int[] jagd = { 41468, 40086, 40165, 40165 }; // Crit /Stam
                int[] forc = { 41466, 40091, 40169, 40169 }; // Haste/Stam
                int[] dazl = { 41463, 40094, 40175, 40175 }; // Int  /Mp5
                int[] lmbt = { 41469, 40100, 40177, 40177 }; // Hit  /Mp5
                int[] sndr = { 41477, 40096, 40176, 40176 }; // Crit /Mp5
                int[] enrg = { 41465, 40105, 40179, 40179 }; // Haste/Mp5
                #endregion
                #region Yellow
                int[] rigd = { 41447, 40014, 40125, 42156 }; // Hit
                int[] smth = { 41448, 40013, 40124, 42149 }; // Crit
                int[] quik = { 41446, 40017, 40128, 42150 }; // Haste
                #endregion
                #region Orange
                int[] glnt = { 41491, 40044, 40148, 40148 }; // Agi  /Hit
                int[] ddly = { 41484, 40043, 40147, 40147 }; // Agi  /Crit
                int[] deft = { 41485, 40046, 40150, 40150 }; // Agi  /Haste
                int[] prst = { 41496, 40053, 40157, 40157 }; // AP   /Hit
                int[] wckd = { 41429, 40052, 40156, 40156 }; // AP   /Crit
                int[] strk = { 41501, 40055, 40159, 40159 }; // AP   /Haste
                #endregion
                #region Meta
                int relentless = 41398; // 21 Agi  3% Crit DMG
                int chaotic    = 41285; // 21 Crit 3% Crit DMG
                int[] metas = { relentless, chaotic };
                #endregion

                string[] groups = new string[] { "Uncommon", "Rare", "Epic", "Jeweler" }; List<GemmingTemplate> templates = new List<GemmingTemplate>(); int i = 0; string m = "Hunter"; 
                foreach (string s in groups) {
                    bool e = s == "Epic";
                    for(int j=0;j<metas.Length;j++){
templates.AddRange(new List<GemmingTemplate>(){
#region Purity
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=deli[i],YellowId=deli[i],BlueId=deli[i],PrismaticId=deli[i],MetaId=metas[j]},//Max Agi
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=brit[i],YellowId=brit[i],BlueId=brit[i],PrismaticId=brit[i],MetaId=metas[j]},//Max AP
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=frac[i],YellowId=frac[i],BlueId=frac[i],PrismaticId=frac[i],MetaId=metas[j]},//Max ArP
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=smth[i],YellowId=smth[i],BlueId=smth[i],PrismaticId=smth[i],MetaId=metas[j]},//Max Crit
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=quik[i],YellowId=quik[i],BlueId=quik[i],PrismaticId=quik[i],MetaId=metas[j]},//Max Haste
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=lust[i],YellowId=lust[i],BlueId=lust[i],PrismaticId=lust[i],MetaId=metas[j]},//Max Mp5
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=rigd[i],YellowId=rigd[i],BlueId=rigd[i],PrismaticId=rigd[i],MetaId=metas[j]},//Max Hit
#endregion
#region Consider Socket Bonuses/Meta Activation
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=deli[i],YellowId=ddly[i],BlueId=shif[i],PrismaticId=deli[i],MetaId=metas[j]},//Agi
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=brit[i],YellowId=wckd[i],BlueId=bala[i],PrismaticId=brit[i],MetaId=metas[j]},//AP
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=frac[i],YellowId=ddly[i],BlueId=puis[i],PrismaticId=frac[i],MetaId=metas[j]},//ArP
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=ddly[i],YellowId=smth[i],BlueId=jagd[i],PrismaticId=smth[i],MetaId=metas[j]},//Crit
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=deft[i],YellowId=quik[i],BlueId=forc[i],PrismaticId=quik[i],MetaId=metas[j]},//Haste
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=glnt[i],YellowId=rigd[i],BlueId=vivd[i],PrismaticId=rigd[i],MetaId=metas[j]},//Hit
#endregion
#region Consider Hit
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=deli[i],YellowId=rigd[i],BlueId=vivd[i],PrismaticId=glnt[i],MetaId=metas[j]},//Agi  /Hit
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=brit[i],YellowId=rigd[i],BlueId=vivd[i],PrismaticId=prst[i],MetaId=metas[j]},//AP   /Hit
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=frac[i],YellowId=rigd[i],BlueId=vivd[i],PrismaticId=frac[i],MetaId=metas[j]},//ArP  /Hit
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=ddly[i],YellowId=rigd[i],BlueId=vivd[i],PrismaticId=smth[i],MetaId=metas[j]},//Crit /Hit
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=deft[i],YellowId=rigd[i],BlueId=vivd[i],PrismaticId=quik[i],MetaId=metas[j]},//Haste/Hit
new GemmingTemplate(){Model=m,Group=s,Enabled=e,RedId=tenu[i],YellowId=rigd[i],BlueId=lust[i],PrismaticId=lust[i],MetaId=metas[j]},//Mp5  /Hit
#endregion
#region Consider Mp5
new GemmingTemplate(){Model=m,Group=s,Enabled=false,RedId=deli[i],YellowId=ddly[i],BlueId=tenu[i],PrismaticId=lust[i],MetaId=metas[j]},//Agi  /Mp5
new GemmingTemplate(){Model=m,Group=s,Enabled=false,RedId=brit[i],YellowId=wckd[i],BlueId=infd[i],PrismaticId=lust[i],MetaId=metas[j]},//AP   /Mp5
new GemmingTemplate(){Model=m,Group=s,Enabled=false,RedId=frac[i],YellowId=ddly[i],BlueId=tenu[i],PrismaticId=lust[i],MetaId=metas[j]},//ArP  /Mp5
new GemmingTemplate(){Model=m,Group=s,Enabled=false,RedId=tenu[i],YellowId=smth[i],BlueId=sndr[i],PrismaticId=lust[i],MetaId=metas[j]},//Crit /Mp5
new GemmingTemplate(){Model=m,Group=s,Enabled=false,RedId=deft[i],YellowId=quik[i],BlueId=enrg[i],PrismaticId=lust[i],MetaId=metas[j]},//Haste/Mp5
#endregion
});
                    }
                    i++;
                }

                return templates;
            }
        }

        #if RAWR3 || SILVERLIGHT
            private ICalculationOptionsPanel calculationOptionsPanel = null;
            public override ICalculationOptionsPanel CalculationOptionsPanel
        #else
            private CalculationOptionsPanelBase calculationOptionsPanel = null;
            public override CalculationOptionsPanelBase CalculationOptionsPanel
        #endif
            { get { return calculationOptionsPanel ?? (calculationOptionsPanel = new CalculationOptionsPanelHunter()); } }

        private string[] _characterDisplayCalculationLabels = null;
        public override string[] CharacterDisplayCalculationLabels {
            get {
                if (_characterDisplayCalculationLabels == null) {
                    _characterDisplayCalculationLabels = new string[] {
                        "Basic Stats:Health and Stamina",
                        "Basic Stats:Mana",
                        "Basic Stats:Armor",
                        "Basic Stats:Agility",
                        "Basic Stats:Ranged Attack Power",
                        "Basic Stats:Intellect",
                        @"Basic Stats:Hit*8.00% chance to miss base for Yellow Attacks
Focused Aim 0 - 8%-0%=8%=263 Rating soft cap
Focused Aim 1 - 8%-1%=7%=230 Rating soft cap
Focused Aim 2 - 8%-2%=6%=197 Rating soft cap
Focused Aim 3 - 8%-3%=5%=164 Rating soft cap",
                        "Basic Stats:Crit",
                        "Basic Stats:Armor Penetration*Rating Cap 1400",
                        "Basic Stats:Haste",

                        "Pet Stats:Pet Attack Power",
                        "Pet Stats:Pet Hit %",
                        "Pet Stats:Pet Dodge %",
                        "Pet Stats:Pet Melee Crit %",
                        "Pet Stats:Pet Specials Crit %",
                        "Pet Stats:Pet White DPS",
                        "Pet Stats:Pet Kill Command DPS",
                        "Pet Stats:Pet Specials DPS",

                        "Shot Stats:Aimed Shot",
                        "Shot Stats:Arcane Shot",
                        "Shot Stats:Multi Shot",
                        "Shot Stats:Silencing Shot",
                        "Shot Stats:Steady Shot",
                        "Shot Stats:Kill Shot",
                        "Shot Stats:Explosive Shot",
                        "Shot Stats:Black Arrow",
                        "Shot Stats:Volley",
                        "Shot Stats:Chimera Shot",

                        //"Shot Stats:Rapid Fire",
                        //"Shot Stats:Readiness",
                        //"Shot Stats:Bestial Wrath",

                        "Sting Stats:Serpent Sting",
                        "Sting Stats:Scorpid Sting",
                        "Sting Stats:Viper Sting",

                        "Trap Stats:Immolation Trap",
                        "Trap Stats:Explosive Trap",
                        "Trap Stats:Freezing Trap",
                        "Trap Stats:Frost Trap",

                        "Mana:Mana Usage Per Second",
                        "Mana:Mana Regen Per Second",
                        "Mana:Normal Change",
                        "Mana:Change during Viper",
                        "Mana:Time to OOM",
                        "Mana:Time to Full",
                        "Mana:Viper Damage Penalty",
                        "Mana:Viper Uptime",
                        "Mana:No Mana Damage Penalty",

                        "Hunter DPS:Autoshot DPS",
                        "Hunter DPS:Priority Rotation DPS",
                        "Hunter DPS:Wild Quiver DPS",
                        "Hunter DPS:Kill Shot low HP gain",
                        "Hunter DPS:Aspect Loss",
                        "Hunter DPS:Piercing Shots DPS",
                        "Hunter DPS:Special DMG Procs DPS*Like Bandit's Insignia or Hand-Mounted Pyro Rockets",

                        "Combined DPS:Hunter DPS",
                        "Combined DPS:Pet DPS",
                        "Combined DPS:Total DPS"
                    };
                }
                return _characterDisplayCalculationLabels;
            }
        }

        private string[] _optimizableCalculationLabels = null;
        public override string[] OptimizableCalculationLabels {
            get {
                if (_optimizableCalculationLabels == null) {
                    _optimizableCalculationLabels = new string[] {
                        "Health",
                        "Attack Power",
                        "Agility",
                        "Mana",
                        "Crit %",
                        "Haste %",
                        "Armor Penetration %",
                        "% Chance to Miss (Yellow)",
                    };
                }
                return _optimizableCalculationLabels;
            }
        }

        private Dictionary<string, Color> _subPointNameColors = null;
        private Dictionary<string, Color> _subPointNameColorsDPS = null;
        private Dictionary<string, Color> _subPointNameColorsMPS = null;
        private Dictionary<string, Color> _subPointNameColorsDPM = null;
        public override Dictionary<string, Color> SubPointNameColors {
            get {
                if (_subPointNameColorsDPS == null) {
                    _subPointNameColorsDPS = new Dictionary<string, Color>();
                    _subPointNameColorsDPS.Add("Hunter DPS", Color.FromArgb(255, 255, 0, 0));
                    _subPointNameColorsDPS.Add("Pet DPS", Color.FromArgb(255, 255, 100, 0));
                    _subPointNameColorsDPS.Add("Hunter Survivability", Color.FromArgb(255, 64, 128, 32));
                    _subPointNameColorsDPS.Add("Pet Survivability", Color.FromArgb(255, 29, 131, 87));
                }
                if (_subPointNameColorsMPS == null) {
                    _subPointNameColorsMPS = new Dictionary<string, Color>();
                    _subPointNameColorsMPS.Add("MPS", Color.FromArgb(255, 0, 0, 255));
                }
                if (_subPointNameColorsDPM == null) {
                    _subPointNameColorsDPM = new Dictionary<string, Color>();
                    _subPointNameColorsDPM.Add("Damage per Mana", Color.FromArgb(255, 0, 0, 255));
                }
                if (_subPointNameColors == null) {
                    _subPointNameColors = _subPointNameColorsDPS;
                }
                return _subPointNameColors;
            }
        }

        public override ComparisonCalculationBase CreateNewComparisonCalculation() { return new ComparisonCalculationHunter(); }
        public override CharacterCalculationsBase CreateNewCharacterCalculations() { return new CharacterCalculationsHunter(); }

        public override ICalculationOptionBase DeserializeDataObject(string xml) {
            XmlSerializer s = new XmlSerializer(typeof(CalculationOptionsHunter));
            StringReader sr = new StringReader(xml);
            CalculationOptionsHunter calcOpts = s.Deserialize(sr) as CalculationOptionsHunter;

            // convert buffs here!
            calcOpts.petActiveBuffs = new List<Buff>(calcOpts._petActiveBuffsXml.ConvertAll(buff => Buff.GetBuffByName(buff)));
            calcOpts.petActiveBuffs.RemoveAll(buff => buff == null);

            return calcOpts;
        }

        #endregion

        #region Relevancy

        public override CharacterClass TargetClass { get { return CharacterClass.Hunter; } }

        private List<ItemType> _relevantItemTypes = null;
        public override List<ItemType> RelevantItemTypes {
            get {
                if (_relevantItemTypes == null) {
                    _relevantItemTypes = new List<ItemType>(new[] {
                        ItemType.None,
                        ItemType.AmmoPouch,
                        ItemType.Arrow,
                        ItemType.Bow,
                        ItemType.Bullet,
                        ItemType.Crossbow,
                        ItemType.Dagger,
                        ItemType.FistWeapon,
                        ItemType.Gun,
                        ItemType.Leather,
                        ItemType.Mail,
                        ItemType.OneHandAxe,                        
                        ItemType.OneHandSword,
                        ItemType.Polearm,
                        ItemType.Quiver,
                        ItemType.Staff,                    
                        ItemType.TwoHandAxe,
                        ItemType.TwoHandSword
                    });
                }
                return _relevantItemTypes;
            }
        }

        public override bool EnchantFitsInSlot(Enchant enchant, Character character, ItemSlot slot) {
            return base.EnchantFitsInSlot(enchant, character, slot);
        }

        private bool HidingBadStuff { get { return HidingBadStuff_Spl || HidingBadStuff_PvP; } }
        private static bool _HidingBadStuff_Spl = true;
        internal static bool HidingBadStuff_Spl { get { return _HidingBadStuff_Spl; } set { _HidingBadStuff_Spl = value; } }
        private static bool _HidingBadStuff_PvP = true;
        internal static bool HidingBadStuff_PvP { get { return _HidingBadStuff_PvP; } set { _HidingBadStuff_PvP = value; } }

        public override Stats GetRelevantStats(Stats stats)
        {
            Stats relevantStats = new Stats() {
                // Basic Stats
                Stamina = stats.Stamina,
                Health = stats.Health,
                Agility = stats.Agility,
                Strength = stats.Strength,
                Intellect = stats.Intellect,
                Mana = stats.Mana,
                Mp5 = stats.Mp5,
                Armor = stats.Armor,
                BonusArmor = stats.BonusArmor,
                // Ratings
                AttackPower = stats.AttackPower,
                RangedAttackPower = stats.RangedAttackPower,
                PetAttackPower = stats.PetAttackPower,
                PhysicalCrit = stats.PhysicalCrit,
                CritRating = stats.CritRating,
                RangedCritRating = stats.RangedCritRating,
                PhysicalHit = stats.PhysicalHit,
                HitRating = stats.HitRating,
                RangedHitRating = stats.RangedHitRating,
                PhysicalHaste = stats.PhysicalHaste,
                HasteRating = stats.HasteRating,
                RangedHasteRating = stats.RangedHasteRating,

                ArmorPenetrationRating = stats.ArmorPenetrationRating,
                ArmorPenetration = stats.ArmorPenetration,
                Miss = stats.Miss,
                ScopeDamage = stats.ScopeDamage,

                // Special
                HighestStat = stats.HighestStat,
                Paragon = stats.Paragon,
                DeathbringerProc = stats.DeathbringerProc,
                MovementSpeed = stats.MovementSpeed,
                StunDurReduc = stats.StunDurReduc,
                SnareRootDurReduc = stats.SnareRootDurReduc,
                FearDurReduc = stats.FearDurReduc,
                DarkmoonCardDeathProc = stats.DarkmoonCardDeathProc,
                ManaRestoreFromMaxManaPerSecond = stats.ManaRestoreFromMaxManaPerSecond,
                ManaRestoreFromBaseManaPPM = stats.ManaRestoreFromBaseManaPPM,
                ManaorEquivRestore = stats.ManaorEquivRestore,
                ManaRestore = stats.ManaRestore,

                // Survivability
                HealthRestore = stats.HealthRestore,
                HealthRestoreFromMaxHealth = stats.HealthRestoreFromMaxHealth,
                Dodge = stats.Dodge,
                DodgeRating = stats.DodgeRating,
                Parry = stats.Parry,
                ParryRating = stats.ParryRating,

                // Set Bonuses
                BonusHunter_PvP_4pc = stats.BonusHunter_PvP_4pc,
                BonusHunter_T7_4P_ViperSpeed = stats.BonusHunter_T7_4P_ViperSpeed,
                BonusHunter_T8_2P_SerpDmg = stats.BonusHunter_T8_2P_SerpDmg,
                BonusHunter_T9_2P_SerpCanCrit = stats.BonusHunter_T9_2P_SerpCanCrit,
                BonusHunter_T9_4P_SteadyShotPetAPProc = stats.BonusHunter_T9_4P_SteadyShotPetAPProc,

                // Multipliers
                BonusStaminaMultiplier = stats.BonusStaminaMultiplier,
                BonusAgilityMultiplier = stats.BonusAgilityMultiplier,
                BonusIntellectMultiplier = stats.BonusIntellectMultiplier,
                BonusAttackPowerMultiplier = stats.BonusAttackPowerMultiplier,
                BonusRangedAttackPowerMultiplier = stats.BonusRangedAttackPowerMultiplier,
                BonusPetAttackPowerMultiplier = stats.BonusPetAttackPowerMultiplier,

                BonusManaPotion = stats.BonusManaPotion,
                DamageTakenMultiplier = stats.DamageTakenMultiplier,
                BonusDamageMultiplier = stats.BonusDamageMultiplier,
                BaseArmorMultiplier = stats.BaseArmorMultiplier,
                BonusArmorMultiplier = stats.BonusArmorMultiplier,
                BonusCritMultiplier = stats.BonusCritMultiplier,
                BonusSpiritMultiplier = stats.BonusSpiritMultiplier,
                BonusPetDamageMultiplier = stats.BonusPetDamageMultiplier,
                BonusPetCritChance = stats.BonusPetCritChance,
                BonusManaMultiplier = stats.BonusManaMultiplier,
                BonusBleedDamageMultiplier = stats.BonusBleedDamageMultiplier,
                BonusPhysicalDamageMultiplier = stats.BonusPhysicalDamageMultiplier,

                // Damage Procs
                ShadowDamage = stats.ShadowDamage,
                ArcaneDamage = stats.ArcaneDamage,
                HolyDamage = stats.HolyDamage,
                NatureDamage = stats.NatureDamage,
                FrostDamage = stats.FrostDamage,
                FireDamage = stats.FireDamage,
                BonusShadowDamageMultiplier = stats.BonusShadowDamageMultiplier,
                BonusArcaneDamageMultiplier = stats.BonusArcaneDamageMultiplier,
                BonusHolyDamageMultiplier = stats.BonusHolyDamageMultiplier,
                BonusNatureDamageMultiplier = stats.BonusNatureDamageMultiplier,
                BonusFrostDamageMultiplier = stats.BonusFrostDamageMultiplier,
                BonusFireDamageMultiplier = stats.BonusFireDamageMultiplier,

                ZodProc = stats.ZodProc,
            };
            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if ((effect.Trigger == Trigger.Use ||
                     effect.Trigger == Trigger.PhysicalCrit ||
                     effect.Trigger == Trigger.PhysicalHit ||
                     effect.Trigger == Trigger.DoTTick ||
                     effect.Trigger == Trigger.DamageDone ||
                     effect.Trigger == Trigger.DamageOrHealingDone ||
                     effect.Trigger == Trigger.DamageTaken ||
                     // Hunter Specific
                     effect.Trigger == Trigger.RangedHit ||
                     effect.Trigger == Trigger.RangedCrit ||
                     effect.Trigger == Trigger.SteadyShotHit ||
                     effect.Trigger == Trigger.PetClawBiteSmackCrit ||
                     effect.Trigger == Trigger.HunterAutoShotHit ||
                     effect.Trigger == Trigger.SerpentWyvernStingsDoDamage)
                    && HasRelevantStats(effect.Stats))
                {
                    relevantStats.AddSpecialEffect(effect);
                }
            }
            return relevantStats;
        }
        public override bool HasRelevantStats(Stats stats) {
            bool relevant = HasWantedStats(stats) && !HasIgnoreStats(stats);
            return relevant;
        }

        private bool HasWantedStats(Stats stats)
        {
            bool isRelevant = (
                // Base Stats
                stats.Agility +
                stats.Strength +
                stats.AttackPower + stats.RangedAttackPower + stats.PetAttackPower +
                stats.Intellect +
                // Ratings
                stats.Mp5 +
                stats.ArmorPenetrationRating +
                stats.CritRating  + stats.RangedCritRating +
                stats.HasteRating + stats.RangedHasteRating +
                stats.HitRating   + stats.RangedHitRating +
                // Bonuses
                stats.ArmorPenetration +
                stats.PhysicalCrit +
                stats.RangedHaste +
                stats.PhysicalHit +
                stats.MovementSpeed + stats.StunDurReduc + stats.SnareRootDurReduc + stats.FearDurReduc +
                stats.ManaRestore + // Mana Pots
                stats.ManaRestoreFromMaxManaPerSecond + // Replenishment Buffs
                stats.ManaRestoreFromBaseManaPPM + // Judgement of Wisdom Buff
                // Target Debuffs
                // Procs
                stats.DarkmoonCardDeathProc +
                stats.HighestStat +
                stats.Paragon +
                stats.ManaorEquivRestore +
                stats.DeathbringerProc +
                // Damage Procs
                stats.ShadowDamage +
                stats.ArcaneDamage +
                stats.HolyDamage +
                stats.NatureDamage +
                stats.FrostDamage +
                stats.FireDamage +
                stats.BonusShadowDamageMultiplier +
                stats.BonusArcaneDamageMultiplier +
                stats.BonusHolyDamageMultiplier +
                stats.BonusNatureDamageMultiplier +
                stats.BonusFrostDamageMultiplier +
                stats.BonusFireDamageMultiplier +
                stats.ZodProc +
                // Multipliers
                stats.BonusAgilityMultiplier +
                stats.BonusAttackPowerMultiplier +
                stats.BonusRangedAttackPowerMultiplier +
                stats.BonusPetAttackPowerMultiplier +
                stats.BonusCritMultiplier +
                stats.BonusIntellectMultiplier +
                stats.BonusPetDamageMultiplier +
                stats.BonusDamageMultiplier +
                stats.BonusSpiritMultiplier +
                stats.DamageTakenMultiplier +
                stats.BaseArmorMultiplier +
                stats.BonusArmorMultiplier +
                stats.BonusBleedDamageMultiplier +
                stats.BonusPhysicalDamageMultiplier +
                stats.BonusManaMultiplier +
                // Set Bonuses
                stats.BonusHunter_T7_4P_ViperSpeed +
                stats.BonusHunter_T8_2P_SerpDmg +
                stats.BonusHunter_T9_2P_SerpCanCrit +
                stats.BonusHunter_T9_4P_SteadyShotPetAPProc +
                stats.BonusHunter_PvP_4pc +
                // Special
                stats.ScopeDamage +
                stats.BonusManaPotion +
                stats.BonusPetCritChance
            ) != 0;

            foreach (SpecialEffect e in stats.SpecialEffects())
            {
                if (e.Trigger == Trigger.DamageDone
                    || e.Trigger == Trigger.DamageOrHealingDone
                    || e.Trigger == Trigger.DoTTick
                    || e.Trigger == Trigger.PhysicalCrit
                    || e.Trigger == Trigger.PhysicalHit
                    || e.Trigger == Trigger.Use
                    || e.Trigger == Trigger.DamageTaken
                    // Hunter Specific
                    || e.Trigger == Trigger.RangedHit
                    || e.Trigger == Trigger.RangedCrit
                    || e.Trigger == Trigger.SteadyShotHit
                    || e.Trigger == Trigger.PetClawBiteSmackCrit
                    || e.Trigger == Trigger.HunterAutoShotHit
                    || e.Trigger == Trigger.SerpentWyvernStingsDoDamage)
                {
                    isRelevant |= HasRelevantStats(e.Stats);
                }
            }

            return isRelevant;
        }

        private bool HasSurvivabilityStats(Stats stats)
        {
            bool retVal = false;
            if ((stats.Health
                + stats.Stamina
                + stats.BonusHealthMultiplier
                + stats.BonusStaminaMultiplier
                + stats.HealthRestore
                + stats.HealthRestoreFromMaxHealth
                ) > 0)
            {
                retVal = true;
            }
            return retVal;
        }

        private bool HasIgnoreStats(Stats stats)
        {
            if (!HidingBadStuff) { return false; }
            return (
                // Remove Spellcasting only Stuff
                (HidingBadStuff_Spl ? stats.SpellPower + stats.SpellPenetration
                                    : 0f)
                // Remove PvP Items
                + (HidingBadStuff_PvP ? stats.Resilience
                                      : 0f)
                ) > 0;
        }

        public override bool IsEnchantRelevant(Enchant enchant, Character character) 
        {
            return 
                IsEnchantAllowedForClass(enchant, character.Class) &&
                IsProfEnchantRelevant(enchant, character) && 
                (HasWantedStats(enchant.Stats) || 
                    (HasSurvivabilityStats(enchant.Stats) && !HasIgnoreStats(enchant.Stats)));
        }

        public override bool IsBuffRelevant(Buff buff, Character character)
        {
            string name = buff.Name;
            // Force some buffs to active
            if (name.Contains("Potion of Wild Magic")
                || name.Contains("Insane Strength Potion")
            ) {
                return true;
            }
            // Force some buffs to go away
            else if (!buff.AllowedClasses.Contains(CharacterClass.Hunter)
                     // these four foods give stam, which is the only useful part of their buff.
                     // removed because you shouldn't use these foods - other foods are always better.
                     || name.Contains("Spirit Food")
                     || name.Contains("Strength Food")
                     || name.Contains("Expertise Food")
                     || name.Contains("Spell Power Food")
            ) {
                return false;
            }
            bool haswantedStats = HasWantedStats(buff.Stats);
            bool hassurvStats = HasSurvivabilityStats(buff.Stats);
            bool hasbadstats = HasIgnoreStats(buff.Stats);
            bool retVal = haswantedStats || (hassurvStats && !hasbadstats);
            return retVal;
            //return base.IsBuffRelevant(buff);
        }

        public override bool CanUseAmmo { get { return true; } }

        public override bool IsItemRelevant(Item item) {
            if ( // Manual override for +X to all Stats gems
                   item.Name == "Nightmare Tear"
                || item.Name == "Enchanted Tear"
                || item.Name == "Enchanted Pearl"
                || item.Name == "Chromatic Sphere"
                ) {
                return true;
                //}else if (item.Type == ItemType.Polearm && 
            } else if (item.Slot == ItemSlot.Ranged && item.Type == ItemType.Idol) {
                return false;
            } else if (item.Slot == ItemSlot.Projectile ||
                (item.Slot == ItemSlot.Ranged && (item.Type == ItemType.Gun || item.Type == ItemType.Bow || item.Type == ItemType.Crossbow)))
            {
                return true;
            } else {
                Stats stats = item.Stats;
                bool wantedStats = HasWantedStats(stats);
                bool survstats = HasSurvivabilityStats(stats);
                bool ignoreStats = HasIgnoreStats(stats);
                return (wantedStats || survstats) && !ignoreStats && base.IsItemRelevant(item);
            }
        }

        private static List<Buff> _relevantPetBuffs = new List<Buff>();
        public static List<Buff> RelevantPetBuffs {
            get {
                if (_relevantPetBuffs.Count == 0) {
                    _relevantPetBuffs = Buff.AllBuffs.FindAll(buff => CalculationsHunter.IsPetBuffRelevant(buff));
                }
                return _relevantPetBuffs;
            }
        }

        private static bool IsPetBuffRelevant(Buff buff)
        {
            if (buff.Group == "Elixirs and Flasks"
                || buff.Group == "Food"
                || buff.Group == "Set Bonuses"
                || buff.Group == "Profession Buffs"
                || buff.Group == "Temporary Buffs"
                || buff.Group == "Potion")
            { return false; }

            // Greater Blessing of Kings
            if (buff.Stats.BonusAgilityMultiplier != 0) return true;
            if (buff.Stats.BonusStrengthMultiplier != 0) return true;
            if (buff.Stats.BonusStaminaMultiplier != 0) return true;

            // Commanding Shout & Blood Pact
            if (buff.Stats.Health != 0) return true;

            // Greater Blessing of Might & Battle Shout
            if (buff.Stats.AttackPower != 0) return true;
            if (buff.Stats.PetAttackPower != 0) return true;

            // True Shot Aura (not you) & Abo. Might & Unl. Rage
            if (buff.Stats.BonusAttackPowerMultiplier != 0) return true;
            if (buff.Stats.BonusPetAttackPowerMultiplier != 0) return true;

            // Leader of the Pack/Rampage
            if (buff.Stats.PhysicalCrit != 0) return true;

            // Strength of Earth Totem & Horn of Winter &  Gift of the Wild & Prayer of Fortitude & Scrolls
            if (buff.Stats.Strength != 0) return true;
            if (buff.Stats.Agility != 0) return true;
            if (buff.Stats.Stamina != 0) return true;

            // WF Totem & Imp. Icy Talons & Swift Ret & Moonkin Aura
            if (buff.Stats.PhysicalHaste != 0) return true;

            // Sunder Armor, Sting
            if (buff.Stats.ArmorPenetration != 0) return true;

            // Ret Aura & Feroc. Insp.
            if (buff.Stats.BonusDamageMultiplier != 0) return true;

            // Pet Food
            if (buff.Stats.PetStamina != 0) return true;
            if (buff.Stats.PetStrength != 0) return true;

            if (buff.Stats._rawSpecialEffectData != null && buff.Stats._rawSpecialEffectData.Length > 0) {
                return IsPetBuffRelevant(new Buff() { Group = buff.Group, Stats = buff.Stats._rawSpecialEffectData[0].Stats });
            }

            return false;
        }

        public Stats GetBuffsStats(Character character, CalculationOptionsHunter calcOpts) {
            List<Buff> removedBuffs = new List<Buff>();
            List<Buff> addedBuffs = new List<Buff>();

            List<Buff> buffGroup = new List<Buff>();
            #region Maintenance Auto-Fixing
            /*// Removes the Sunder Armor if you are maintaining it yourself
            // Also removes Acid Spit and Expose Armor
            // We are now calculating this internally for better accuracy and to provide value to relevant talents
            if (calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.SunderArmor_])
            {
                buffGroup.Clear();
                buffGroup.Add(Buff.GetBuffByName("Sunder Armor"));
                buffGroup.Add(Buff.GetBuffByName("Acid Spit"));
                buffGroup.Add(Buff.GetBuffByName("Expose Armor"));
                MaintBuffHelper(buffGroup, character, removedBuffs);
            }

            // Removes the Thunder Clap & Improved Buffs if you are maintaining it yourself
            // Also removes Judgements of the Just, Infected Wounds, Frost Fever, Improved Icy Touch
            // We are now calculating this internally for better accuracy and to provide value to relevant talents
            if (calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.ThunderClap_])
            {
                buffGroup.Clear();
                buffGroup.Add(Buff.GetBuffByName("Thunder Clap"));
                buffGroup.Add(Buff.GetBuffByName("Improved Thunder Clap"));
                buffGroup.Add(Buff.GetBuffByName("Judgements of the Just"));
                buffGroup.Add(Buff.GetBuffByName("Infected Wounds"));
                buffGroup.Add(Buff.GetBuffByName("Frost Fever"));
                buffGroup.Add(Buff.GetBuffByName("Improved Icy Touch"));
                MaintBuffHelper(buffGroup, character, removedBuffs);
            }

            // Removes the Demoralizing Shout & Improved Buffs if you are maintaining it yourself
            // We are now calculating this internally for better accuracy and to provide value to relevant talents
            if (calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.DemoralizingShout_])
            {
                buffGroup.Clear();
                buffGroup.Add(Buff.GetBuffByName("Demoralizing Shout"));
                buffGroup.Add(Buff.GetBuffByName("Improved Demoralizing Shout"));
                MaintBuffHelper(buffGroup, character, removedBuffs);
            }

            // Removes the Battle Shout & Commanding Presence Buffs if you are maintaining it yourself
            // Also removes their equivalent of Blessing of Might (+Improved)
            // We are now calculating this internally for better accuracy and to provide value to relevant talents
            if (calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.BattleShout_])
            {
                buffGroup.Clear();
                buffGroup.Add(Buff.GetBuffByName("Commanding Presence (Attack Power)"));
                buffGroup.Add(Buff.GetBuffByName("Battle Shout"));
                buffGroup.Add(Buff.GetBuffByName("Improved Blessing of Might"));
                buffGroup.Add(Buff.GetBuffByName("Blessing of Might"));
                MaintBuffHelper(buffGroup, character, removedBuffs);
            }

            // Removes the Commanding Shout & Commanding Presence Buffs if you are maintaining it yourself
            // Also removes their equivalent of Blood Pact (+Improved Imp)
            // We are now calculating this internally for better accuracy and to provide value to relevant talents
            if (calcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.CommandingShout_])
            {
                buffGroup.Clear();
                buffGroup.Add(Buff.GetBuffByName("Commanding Presence (Health)"));
                buffGroup.Add(Buff.GetBuffByName("Commanding Shout"));
                buffGroup.Add(Buff.GetBuffByName("Improved Imp"));
                buffGroup.Add(Buff.GetBuffByName("Blood Pact"));
                MaintBuffHelper(buffGroup, character, removedBuffs);
            }*/
            #endregion

            #region Passive Ability Auto-Fixing
            // Removes the Trueshot Aura Buff and it's equivalents Unleashed Rage and Abomination's Might if you are
            // maintaining it yourself. We are now calculating this internally for better accuracy and to provide
            // value to relevant talents
            if (character.HunterTalents.TrueshotAura > 0) {
                buffGroup.Clear();
                buffGroup.Add(Buff.GetBuffByName("Trueshot Aura"));
                buffGroup.Add(Buff.GetBuffByName("Unleashed Rage"));
                buffGroup.Add(Buff.GetBuffByName("Abomination's Might"));
                MaintBuffHelper(buffGroup, character, removedBuffs);
            }

            // Removes the Hunter's Mark Buff and it's Children 'Glyphed', 'Improved' and 'Both' if you are
            // maintaining it yourself. We are now calculating this internally for better accuracy and to provide
            // value to relevant talents
            if (character.HunterTalents.ImprovedHuntersMark + (character.HunterTalents.GlyphOfHuntersMark ? 1 : 0) > 0) {
                buffGroup.Clear();
                buffGroup.Add(Buff.GetBuffByName("Hunter's Mark"));
                buffGroup.Add(Buff.GetBuffByName("Glyphed Hunter's Mark"));
                buffGroup.Add(Buff.GetBuffByName("Improved Hunter's Mark"));
                buffGroup.Add(Buff.GetBuffByName("Improved and Glyphed Hunter's Mark"));
                MaintBuffHelper(buffGroup, character, removedBuffs);
            }
            /* [More Buffs to Come to this method]
             * Ferocious Inspiration | Sanctified Retribution
             * Hunting Party | Judgements of the Wise, Vampiric Touch, Improved Soul Leech, Enduring Winter
             * Acid Spit | Expose Armor, Sunder Armor (requires BM & Worm Pet)
             */
            #endregion

            Stats statsBuffs = GetBuffsStats(character.ActiveBuffs);

            foreach (Buff b in removedBuffs) { character.ActiveBuffsAdd(b); }
            foreach (Buff b in addedBuffs) { character.ActiveBuffs.Remove(b); }

            return statsBuffs;
        }
        private void MaintBuffHelper(List<Buff> buffGroup, Character character, List<Buff> removedBuffs)
        {
            foreach (Buff b in buffGroup)
            {
                if (character.ActiveBuffs.Remove(b)) { removedBuffs.Add(b); }
            }
        }
        public override void SetDefaults(Character character) { }
        public Stats GetPetBuffsStats(Character character, CalculationOptionsHunter calcOpts)
        {
            if (calcOpts == null) return new Stats();
            Stats statsBuffs;
            try {
                List<Buff> removedBuffs = new List<Buff>();
                List<Buff> addedBuffs = new List<Buff>();

                List<Buff> buffGroup = new List<Buff>();

                #region Passive Ability Auto-Fixing
                // Removes the Trueshot Aura Buff and it's equivalents Unleashed Rage and Abomination's Might if you are
                // maintaining it yourself. We are now calculating this internally for better accuracy and to provide
                // value to relevant talents
                if (character.HunterTalents.TrueshotAura > 0) {
                    buffGroup.Clear();
                    buffGroup.Add(Buff.GetBuffByName("Trueshot Aura"));
                    buffGroup.Add(Buff.GetBuffByName("Unleashed Rage"));
                    buffGroup.Add(Buff.GetBuffByName("Abomination's Might"));
                    MaintPetBuffHelper(buffGroup, calcOpts, removedBuffs);
                }
                /* [More Buffs to Come to this method]
                 * Ferocious Inspiration | Sanctified Retribution
                 * Hunting Party | Judgements of the Wise, Vampiric Touch, Improved Soul Leech, Enduring Winter
                 * Acid Spit | Expose Armor, Sunder Armor (requires BM & Worm Pet)
                 */
                #endregion

                statsBuffs = GetBuffsStats(calcOpts.petActiveBuffs);

                foreach (Buff b in removedBuffs) { calcOpts.petActiveBuffs.Add(b); }
                foreach (Buff b in addedBuffs) { calcOpts.petActiveBuffs.Remove(b); }
            } catch (Exception ex) {
                Rawr.Base.ErrorBox eb = new Rawr.Base.ErrorBox("Error Generating Pet Buff Stats",
                    ex.Message, "GetPetBuffsStats(...)",
                    "No Additional Info", ex.StackTrace);
                eb.Show();
                statsBuffs = new Stats();
            }

            return statsBuffs;
        }
        private void MaintPetBuffHelper(List<Buff> buffGroup, CalculationOptionsHunter calcOpts, List<Buff> removedBuffs)
        {
            foreach (Buff b in buffGroup)
            {
                if (calcOpts.petActiveBuffs.Remove(b)) { removedBuffs.Add(b); }
            }
        }

        #endregion

        #region Special Comparison Charts
        private string[] _customChartNames = null;
        public override string[] CustomChartNames {
            get {
                if (_customChartNames == null) {
                    _customChartNames = new string[] {
                        "Pet Talents",
#if RAWR3 || SILVERLIGHT
                        //"Pet Talent Specs + Armory Pets",
#endif
                        //"Pet Buffs",
                        "Spammed Shots DPS",
                        "Spammed Shots MPS",
                        "Rotation DPS",
                        "Rotation MPS",
                        "Shot Damage per Mana",
                        "Item Budget",
                    };
                }
                return _customChartNames;
            }
        }
        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
        {
            CharacterCalculationsHunter calculations = GetCharacterCalculations(character) as CharacterCalculationsHunter;

            switch (chartName)
            {
                #region Pet Talents
                case "Pet Talents":
                    _subPointNameColors = _subPointNameColorsDPS;
                    return GetPetTalentChart(character, calculations);
                #endregion
                #region Pet Talents
                /*case "Pet Talent Specs + Armory Pets":
                    _subPointNameColors = _subPointNameColorsDPS;
                    return GetPetTalentSpecsChart(character, calculations);*/
                #endregion
                #region Pet Buffs
                case "Pet Buffs":
                    _subPointNameColors = _subPointNameColorsDPS;
                    CalculationOptionsHunter calcOpts = character.CalculationOptions as CalculationOptionsHunter;
                    return GetPetBuffCalculations(character, calcOpts, calculations, "All").ToArray();
                #endregion
                #region Spammed Shots DPS
                case "Spammed Shots DPS":
                    _subPointNameColors = _subPointNameColorsDPS;
                    return new ComparisonCalculationBase[] {
                        comparisonFromShotSpammedDPS(calculations.aimedShot),
                        comparisonFromShotSpammedDPS(calculations.arcaneShot),
                        comparisonFromShotSpammedDPS(calculations.multiShot),
                        comparisonFromShotSpammedDPS(calculations.serpentSting),
                        comparisonFromShotSpammedDPS(calculations.scorpidSting),
                        comparisonFromShotSpammedDPS(calculations.viperSting),
                        comparisonFromShotSpammedDPS(calculations.silencingShot),
                        comparisonFromShotSpammedDPS(calculations.steadyShot),
                        comparisonFromShotSpammedDPS(calculations.killShot),
                        comparisonFromShotSpammedDPS(calculations.explosiveShot),
                        comparisonFromShotSpammedDPS(calculations.blackArrow),
                        comparisonFromShotSpammedDPS(calculations.immolationTrap),
                        comparisonFromShotSpammedDPS(calculations.explosiveTrap),
                        comparisonFromShotSpammedDPS(calculations.freezingTrap),
                        comparisonFromShotSpammedDPS(calculations.frostTrap),
                        comparisonFromShotSpammedDPS(calculations.volley),
                        comparisonFromShotSpammedDPS(calculations.chimeraShot),
                    };
                #endregion
                #region Spammed Shots MPS
                case "Spammed Shots MPS":
                    _subPointNameColors = _subPointNameColorsMPS;
                    return new ComparisonCalculationBase[] {
                        comparisonFromShotSpammedMPS(calculations.aimedShot),
                        comparisonFromShotSpammedMPS(calculations.arcaneShot),
                        comparisonFromShotSpammedMPS(calculations.multiShot),
                        comparisonFromShotSpammedMPS(calculations.serpentSting),
                        comparisonFromShotSpammedMPS(calculations.scorpidSting),
                        comparisonFromShotSpammedMPS(calculations.viperSting),
                        comparisonFromShotSpammedMPS(calculations.silencingShot),
                        comparisonFromShotSpammedMPS(calculations.steadyShot),
                        comparisonFromShotSpammedMPS(calculations.killShot),
                        comparisonFromShotSpammedMPS(calculations.explosiveShot),
                        comparisonFromShotSpammedMPS(calculations.blackArrow),
                        comparisonFromShotSpammedMPS(calculations.immolationTrap),
                        comparisonFromShotSpammedMPS(calculations.explosiveTrap),
                        comparisonFromShotSpammedMPS(calculations.freezingTrap),
                        comparisonFromShotSpammedMPS(calculations.frostTrap),
                        comparisonFromShotSpammedMPS(calculations.volley),
                        comparisonFromShotSpammedMPS(calculations.chimeraShot),
                        comparisonFromShotSpammedMPS(calculations.rapidFire),
                        comparisonFromShotSpammedMPS(calculations.readiness),
                        comparisonFromShotSpammedMPS(calculations.bestialWrath),
                    };
                #endregion
                #region Rotation DPS
                case "Rotation DPS":
                    _subPointNameColors = _subPointNameColorsDPS;
                    return new ComparisonCalculationBase[] {
                        comparisonFromShotRotationDPS(calculations.aimedShot),
                        comparisonFromShotRotationDPS(calculations.arcaneShot),
                        comparisonFromShotRotationDPS(calculations.multiShot),
                        comparisonFromShotRotationDPS(calculations.serpentSting),
                        comparisonFromShotRotationDPS(calculations.scorpidSting),
                        comparisonFromShotRotationDPS(calculations.viperSting),
                        comparisonFromShotRotationDPS(calculations.silencingShot),
                        comparisonFromShotRotationDPS(calculations.steadyShot),
                        comparisonFromShotRotationDPS(calculations.killShot),
                        comparisonFromShotRotationDPS(calculations.explosiveShot),
                        comparisonFromShotRotationDPS(calculations.blackArrow),
                        comparisonFromShotRotationDPS(calculations.immolationTrap),
                        comparisonFromShotRotationDPS(calculations.explosiveTrap),
                        comparisonFromShotRotationDPS(calculations.freezingTrap),
                        comparisonFromShotRotationDPS(calculations.frostTrap),
                        comparisonFromShotRotationDPS(calculations.volley),
                        comparisonFromShotRotationDPS(calculations.chimeraShot),
                        comparisonFromDoubles("Autoshot", calculations.AutoshotDPS, 0),
                        comparisonFromDoubles("WildQuiver", calculations.WildQuiverDPS, 0),
                        comparisonFromDoubles("KillShotSub20", calculations.killShotSub20FinalGain, 0),
                        comparisonFromDoubles("AspectBeastLoss", calculations.aspectBeastLostDPS, 0),
                        comparisonFromDoubles("PetAutoAttack", 0, calculations.petWhiteDPS),
                        comparisonFromDoubles("PetSkills", 0, calculations.petSpecialDPS),
                        comparisonFromDoubles("KillCommand", 0, calculations.petKillCommandDPS),
                    };
                #endregion
                #region Rotation MPS
                case "Rotation MPS":
                    _subPointNameColors = _subPointNameColorsMPS;
                    return new ComparisonCalculationBase[] {
                        comparisonFromShotRotationMPS(calculations.aimedShot),
                        comparisonFromShotRotationMPS(calculations.arcaneShot),
                        comparisonFromShotRotationMPS(calculations.multiShot),
                        comparisonFromShotRotationMPS(calculations.serpentSting),
                        comparisonFromShotRotationMPS(calculations.scorpidSting),
                        comparisonFromShotRotationMPS(calculations.viperSting),
                        comparisonFromShotRotationMPS(calculations.silencingShot),
                        comparisonFromShotRotationMPS(calculations.steadyShot),
                        comparisonFromShotRotationMPS(calculations.killShot),
                        comparisonFromShotRotationMPS(calculations.explosiveShot),
                        comparisonFromShotRotationMPS(calculations.blackArrow),
                        comparisonFromShotRotationMPS(calculations.immolationTrap),
                        comparisonFromShotRotationMPS(calculations.explosiveTrap),
                        comparisonFromShotRotationMPS(calculations.freezingTrap),
                        comparisonFromShotRotationMPS(calculations.frostTrap),
                        comparisonFromShotRotationMPS(calculations.volley),
                        comparisonFromShotRotationMPS(calculations.chimeraShot),
                        comparisonFromShotRotationMPS(calculations.rapidFire),
                        comparisonFromShotRotationMPS(calculations.readiness),
                        comparisonFromShotRotationMPS(calculations.bestialWrath),
                        comparisonFromDouble("KillCommand", calculations.petKillCommandMPS),
                    };
                #endregion
                #region Shot Damage per Mana
                case "Shot Damage per Mana":
                    _subPointNameColors = _subPointNameColorsDPM;
                    return new ComparisonCalculationBase[] {
                        comparisonFromShotDPM(calculations.aimedShot),
                        comparisonFromShotDPM(calculations.arcaneShot),
                        comparisonFromShotDPM(calculations.multiShot),
                        comparisonFromShotDPM(calculations.serpentSting),
                        comparisonFromShotDPM(calculations.scorpidSting),
                        comparisonFromShotDPM(calculations.viperSting),
                        comparisonFromShotDPM(calculations.silencingShot),
                        comparisonFromShotDPM(calculations.steadyShot),
                        comparisonFromShotDPM(calculations.killShot),
                        comparisonFromShotDPM(calculations.explosiveShot),
                        comparisonFromShotDPM(calculations.blackArrow),
                        comparisonFromShotDPM(calculations.immolationTrap),
                        comparisonFromShotDPM(calculations.explosiveTrap),
                        comparisonFromShotDPM(calculations.freezingTrap),
                        comparisonFromShotDPM(calculations.frostTrap),
                        comparisonFromShotDPM(calculations.volley),
                        comparisonFromShotDPM(calculations.chimeraShot),
                    };
                #endregion
                #region Item Budget
                case "Item Budget":
                    _subPointNameColors = _subPointNameColorsDPS;
                    return new ComparisonCalculationBase[] { 
                        comparisonFromStat(character, calculations, new Stats() { Intellect = 10f }, "10 Intellect"),
                        comparisonFromStat(character, calculations, new Stats() { Agility = 10f }, "10 Agility"),
                        comparisonFromStat(character, calculations, new Stats() { Mp5 = 4f }, "4 MP5"),
                        comparisonFromStat(character, calculations, new Stats() { CritRating = 10f }, "10 Crit Rating"),
                        comparisonFromStat(character, calculations, new Stats() { HitRating = 10f }, "10 Hit Rating"),
                        comparisonFromStat(character, calculations, new Stats() { ArmorPenetrationRating = 10f }, "10 Armor Penetration Rating"),
                        comparisonFromStat(character, calculations, new Stats() { AttackPower = 20f }, "20 Attack Power"),
                        comparisonFromStat(character, calculations, new Stats() { RangedAttackPower = 25f }, "25 Ranged Attack Power"),
                        comparisonFromStat(character, calculations, new Stats() { HasteRating = 10f }, "10 Haste Rating"),
                    };
                #endregion
            }

            return new ComparisonCalculationBase[0];
        }
        public virtual List<ComparisonCalculationBase> GetPetBuffCalculations(Character character, CalculationOptionsHunter calcOpts, CharacterCalculationsBase currentCalcs, string filter)
        {
            //ClearCache();
            List<ComparisonCalculationBase> buffCalcs = new List<ComparisonCalculationBase>();
            CharacterCalculationsBase calcsOpposite = null;
            //CharacterCalculationsBase calcsEquipped = null;
            //CharacterCalculationsBase calcsUnequipped = null;
            Character charAutoActivated = character.Clone();
            foreach (Buff autoBuff in currentCalcs.AutoActivatedBuffs)
            {
                if (!charAutoActivated.ActiveBuffs.Contains(autoBuff))
                {
                    charAutoActivated.ActiveBuffsAdd(autoBuff);
                    RemoveConflictingBuffs(charAutoActivated.ActiveBuffs, autoBuff);
                }
            }
            charAutoActivated.DisableBuffAutoActivation = true;

            string[] multiFilter = filter.Split('|');

            List<Buff> relevantBuffs = new List<Buff>();
            foreach (Buff buff in RelevantPetBuffs)
            {
                bool isinMultiFilter = false;
                if (multiFilter.Length > 0)
                {
                    foreach (string mFilter in multiFilter)
                    {
                        if (buff.Group.Equals(mFilter, StringComparison.CurrentCultureIgnoreCase))
                        {
                            isinMultiFilter = true;
                            break;
                        }
                    }
                }
                if (filter == null || filter == "All" || filter == "Current"
                    || buff.Group.Equals(filter, StringComparison.CurrentCultureIgnoreCase)
                    || isinMultiFilter)
                {
                    relevantBuffs.Add(buff);
                    relevantBuffs.AddRange(buff.Improvements);
                }
            }

            foreach (Buff buff in relevantBuffs)
            {
                if (!"Current".Equals(filter, StringComparison.CurrentCultureIgnoreCase) || (charAutoActivated.CalculationOptions as CalculationOptionsHunter).petActiveBuffs.Contains(buff))
                {
                    Character charOpposite = charAutoActivated.Clone();
                    //Character charUnequipped = charAutoActivated.Clone();
                    //Character charEquipped = charAutoActivated.Clone();
                    CalculationOptionsHunter calcOptsOpposite = charOpposite.CalculationOptions as CalculationOptionsHunter;
                    //CalculationOptionsHunter calcOptsUnequipped = charUnequipped.CalculationOptions as CalculationOptionsHunter;
                    //CalculationOptionsHunter calcOptsEquipped = charEquipped.CalculationOptions as CalculationOptionsHunter;
                    bool which = (charAutoActivated.CalculationOptions as CalculationOptionsHunter).petActiveBuffs.Contains(buff);
                    charOpposite.DisableBuffAutoActivation = true;
                    //charUnequipped.DisableBuffAutoActivation = true;
                    //charEquipped.DisableBuffAutoActivation = true;
                    if (which) { calcOptsOpposite.petActiveBuffs.Remove(buff); } else { calcOptsOpposite.petActiveBuffs.Add(buff); }
                    //if (calcOptsUnequipped.petActiveBuffs.Contains(buff)) { calcOptsUnequipped.petActiveBuffs.Remove(buff); }
                    //if (!calcOptsEquipped.petActiveBuffs.Contains(buff)) { calcOptsEquipped.petActiveBuffs.Add(buff); }

                    RemoveConflictingBuffs(calcOptsOpposite.petActiveBuffs, buff);
                    //RemoveConflictingBuffs(calcOptsEquipped.petActiveBuffs, buff);
                    //RemoveConflictingBuffs(calcOptsUnequipped.petActiveBuffs, buff);

                    calcsOpposite = GetCharacterCalculations(charOpposite, null, false, false, false);
                    //calcsUnequipped = GetCharacterCalculations(charUnequipped, null, false, false, false);
                    //calcsEquipped = GetCharacterCalculations(charEquipped, null, false, false, false);

                    ComparisonCalculationBase buffCalc = CreateNewComparisonCalculation();
                    buffCalc.Name = buff.Name;
                    buffCalc.Item = new Item() { Name = buff.Name, Stats = buff.Stats, Quality = ItemQuality.Temp };
                    buffCalc.Equipped = which;//(charAutoActivated.CalculationOptions as CalculationOptionsHunter).petActiveBuffs.Contains(buff);
                    buffCalc.OverallPoints = (which ? currentCalcs.OverallPoints - calcsOpposite.OverallPoints
                                                    : calcsOpposite.OverallPoints - currentCalcs.OverallPoints);
                    //buffCalc.OverallPoints = currentCalcs.OverallPoints - (buffCalc.Equipped ? calcsEquipped.OverallPoints : calcsUnequipped.OverallPoints);
                    float[] subPoints = new float[calcsOpposite.SubPoints.Length];
                    //float[] subPoints = new float[calcsEquipped.SubPoints.Length];
                    for (int i = 0; i < calcsOpposite.SubPoints.Length; i++) {
                        subPoints[i] = (which ? currentCalcs.SubPoints[i] - calcsOpposite.SubPoints[i]
                                              : calcsOpposite.SubPoints[i] - currentCalcs.SubPoints[i]);
                    }
                    //for (int i = 0; i < calcsEquipped.SubPoints.Length; i++) { subPoints[i] = calcsEquipped.SubPoints[i] - calcsUnequipped.SubPoints[i]; }
                    buffCalc.SubPoints = subPoints;
                    buffCalcs.Add(buffCalc);
                    // Revert, cuz it's evil
                    if (!which) { calcOptsOpposite.petActiveBuffs.Remove(buff); } else { calcOptsOpposite.petActiveBuffs.Add(buff); }
                }
            }
            return buffCalcs;
        }
        private ComparisonCalculationHunter[] GetPetTalentChart(Character character, CharacterCalculationsHunter calcs)
        {
            List<ComparisonCalculationHunter> talentCalculations = new List<ComparisonCalculationHunter>();
            Character baseChar = character.Clone(); CalculationOptionsHunter baseCalcOpts = baseChar.CalculationOptions as CalculationOptionsHunter;
            Character newChar = character.Clone(); CalculationOptionsHunter newCalcOpts = newChar.CalculationOptions as CalculationOptionsHunter;
            CharacterCalculationsHunter currentCalc;
            CharacterCalculationsHunter newCalc;
            ComparisonCalculationHunter compare;
            currentCalc = (CharacterCalculationsHunter)Calculations.GetCharacterCalculations(baseChar, null, false, true, false);

#if RAWR3 || SILVERLIGHT
            foreach (PropertyInfo pi in baseCalcOpts.PetTalents.GetType().GetProperties())
            {
                PetTalentDataAttribute[] petTalentDatas = pi.GetCustomAttributes(typeof(PetTalentDataAttribute), true) as PetTalentDataAttribute[];
                int orig;
                if (petTalentDatas.Length > 0) {
                    PetTalentDataAttribute petTalentData = petTalentDatas[0];
                    orig = baseCalcOpts.PetTalents.Data[petTalentData.Index];
                    if (petTalentData.MaxPoints == (int)pi.GetValue(baseCalcOpts.PetTalents, null)) {
                        newCalcOpts.PetTalents.Data[petTalentData.Index]--;
                        newCalc = (CharacterCalculationsHunter)GetCharacterCalculations(newChar, null, false, true, false);
                        compare = (ComparisonCalculationHunter)GetCharacterComparisonCalculations(newCalc, currentCalc, petTalentData.Name, petTalentData.MaxPoints == orig, orig != 0 && orig != petTalentData.MaxPoints);
                    } else {
                        newCalcOpts.PetTalents.Data[petTalentData.Index]++;
                        newCalc = (CharacterCalculationsHunter)GetCharacterCalculations(newChar, null, false, true, false);
                        compare = (ComparisonCalculationHunter)GetCharacterComparisonCalculations(currentCalc, newCalc, petTalentData.Name, petTalentData.MaxPoints == orig, orig != 0 && orig != petTalentData.MaxPoints);
                    }
                    string text = string.Format("Current Rank {0}/{1}\r\n\r\n", orig, petTalentData.MaxPoints);
                    if (orig == 0) {
                        // We originally didn't have it, so first rank is next rank
                        text += "Next Rank:\r\n";
                        text += petTalentData.Description[0];
                    } else if (orig >= petTalentData.MaxPoints) {
                        // We originally were at max, so there isn't a next rank, just show the capped one
                        text += petTalentData.Description[petTalentData.MaxPoints - 1];
                    } else {
                        // We aren't at 0 or MaxPoints originally, so it's just a point in between
                        text += petTalentData.Description[orig - 1];
                        text += "\r\n\r\nNext Rank:\r\n";
                        text += petTalentData.Description[orig];
                    }
                    compare.Description = text;
                    compare.Item = null;
                    talentCalculations.Add(compare);
                    newCalcOpts.PetTalents.Data[petTalentData.Index] = orig;
                }
            }
#else
            foreach (PetTalent pi in baseCalcOpts.PetTalents.TalentTree)
            {
                int Index = pi.ID;
                int orig = 0;
                PetTalent talentData = pi;
                orig = pi.Value;
                if (talentData.Max == pi.Value) {
                    newCalcOpts.PetTalents.TalentTree[Index].Value--;
                    newCalc = (CharacterCalculationsHunter)Calculations.GetCharacterCalculations(newChar, null, false, true, false);
                    compare = (ComparisonCalculationHunter)Calculations.GetCharacterComparisonCalculations(newCalc, currentCalc, talentData.Name, talentData.Max == orig, orig != 0 && orig != talentData.Max);
                } else {
                    newCalcOpts.PetTalents.TalentTree[Index].Value++;
                    newCalc = (CharacterCalculationsHunter)Calculations.GetCharacterCalculations(newChar, null, false, true, false);
                    compare = (ComparisonCalculationHunter)Calculations.GetCharacterComparisonCalculations(currentCalc, newCalc, talentData.Name, talentData.Max == orig, orig != 0 && orig != talentData.Max);
                }
                string text = string.Format("Current Rank {0}/{1}\r\n\r\n", orig, talentData.Max);
                if (orig == 0) {
                    // We originally didn't have it, so first rank is next rank
                    text += "Next Rank:\r\n";
                    text += talentData.Desc[0];
                } else if (orig >= talentData.Max) {
                    // We originally were at max, so there isn't a next rank, just show the capped one
                    text += talentData.Desc[talentData.Max];
                } else {
                    // We aren't at 0 or MaxPoints originally, so it's just a point in between
                    text += talentData.Desc[orig];
                    text += "\r\n\r\nNext Rank:\r\n";
                    text += talentData.Desc[orig+1];
                }
                compare.Description = text;
                compare.Item = null;
                talentCalculations.Add(compare);
                newCalcOpts.PetTalents.TalentTree[Index].Value = orig;
            }
#endif
            return talentCalculations.ToArray();
        }
        /*private ComparisonCalculationHunter[] GetPetTalentSpecsChart(Character character, CharacterCalculationsHunter calcs)
        {
            List<ComparisonCalculationBase> talentCalculations = new List<ComparisonCalculationBase>();
            Character newChar = character.Clone();

            /*PetTalentsBase nothing = character.CurrentTalents.Clone();
            for (int i = 0; i < nothing.Data.Length; i++) nothing.Data[i] = 0;
            for (int i = 0; i < nothing.GlyphData.Length; i++) nothing.GlyphData[i] = false;
            newChar.CurrentTalents = nothing;

            CharacterCalculationsBase baseCalc = Calculations.GetCharacterCalculations(newChar, null, false, true, true);
            CharacterCalculationsBase newCalc;
            ComparisonCalculationBase compare;

            bool same, found = false;
            foreach (SavedTalentSpec sts in SavedTalentSpec.SpecsFor(character.Class))
            {
                same = false;
                if (sts.Equals(character.CurrentTalents)) same = true;
                newChar.CurrentTalents = sts.TalentSpec();
                newCalc = Calculations.GetCharacterCalculations(newChar, null, false, true, true);
                compare = Calculations.GetCharacterComparisonCalculations(baseCalc, newCalc, sts.Name, same);
                compare.Item = null;
                compare.Name = sts.ToString();
                compare.Description = sts.Spec;
                talentCalculations.Add(compare);
                found = found || same;
            }
            if (!found)
            {
                newCalc = Calculations.GetCharacterCalculations(character, null, false, true, true);
                compare = Calculations.GetCharacterComparisonCalculations(baseCalc, newCalc, "Custom", true);
                talentCalculations.Add(compare);
            }
            //CGL_Legend.LegendItems = Calculations.SubPointNameColors;
            //ComparisonGraph.LegendItems = Calculations.SubPointNameColors;
            //ComparisonGraph.Mode = ComparisonGraph.DisplayMode.Subpoints;
            //ComparisonGraph.DisplayCalcs(talentCalculations.ToArray());
            return talentCalculations.ToArray();

            /*List<ComparisonCalculationHunter> talentCalculations = new List<ComparisonCalculationHunter>();
            Character baseChar = character.Clone(); CalculationOptionsHunter baseCalcOpts = baseChar.CalculationOptions as CalculationOptionsHunter;
            Character newChar = character.Clone(); CalculationOptionsHunter newCalcOpts = newChar.CalculationOptions as CalculationOptionsHunter;
            CharacterCalculationsHunter currentCalc;
            CharacterCalculationsHunter newCalc;
            ComparisonCalculationHunter compare;
            currentCalc = (CharacterCalculationsHunter)Calculations.GetCharacterCalculations(baseChar, null, false, true, false);

            foreach (PropertyInfo pi in baseCalcOpts.PetTalents.GetType().GetProperties())
            {
                PetTalentDataAttribute[] petTalentDatas = pi.GetCustomAttributes(typeof(PetTalentDataAttribute), true) as PetTalentDataAttribute[];
                int orig;
                if (petTalentDatas.Length > 0) {
                    PetTalentDataAttribute petTalentData = petTalentDatas[0];
                    orig = baseCalcOpts.PetTalents.Data[petTalentData.Index];
                    if (petTalentData.MaxPoints == (int)pi.GetValue(baseCalcOpts.PetTalents, null)) {
                        newCalcOpts.PetTalents.Data[petTalentData.Index]--;
                        newCalc = (CharacterCalculationsHunter)GetCharacterCalculations(newChar, null, false, true, false);
                        compare = (ComparisonCalculationHunter)GetCharacterComparisonCalculations(newCalc, currentCalc, petTalentData.Name, petTalentData.MaxPoints == orig);
                    } else {
                        newCalcOpts.PetTalents.Data[petTalentData.Index]++;
                        newCalc = (CharacterCalculationsHunter)GetCharacterCalculations(newChar, null, false, true, false);
                        compare = (ComparisonCalculationHunter)GetCharacterComparisonCalculations(currentCalc, newCalc, petTalentData.Name, petTalentData.MaxPoints == orig);
                    }
                    string text = string.Format("Current Rank {0}/{1}\r\n\r\n", orig, petTalentData.MaxPoints);
                    if (orig == 0) {
                        // We originally didn't have it, so first rank is next rank
                        text += "Next Rank:\r\n";
                        text += petTalentData.Description[0];
                    } else if (orig >= petTalentData.MaxPoints) {
                        // We originally were at max, so there isn't a next rank, just show the capped one
                        text += petTalentData.Description[petTalentData.MaxPoints - 1];
                    } else {
                        // We aren't at 0 or MaxPoints originally, so it's just a point in between
                        text += petTalentData.Description[orig - 1];
                        text += "\r\n\r\nNext Rank:\r\n";
                        text += petTalentData.Description[orig];
                    }
                    compare.Description = text;
                    compare.Item = null;
                    talentCalculations.Add(compare);
                    newCalcOpts.PetTalents.Data[petTalentData.Index] = orig;
                }
            }
            return talentCalculations.ToArray();
        }*/
        private ComparisonCalculationHunter comparisonFromShotSpammedDPS(ShotData shot)
        {
            ComparisonCalculationHunter comp =  new ComparisonCalculationHunter();

            float shotWait = shot.Duration > shot.Cd ? shot.Duration : shot.Cd;
            float dps = shotWait > 0 ? (float)(shot.Damage / shotWait) : 0;

            comp.Name = Enum.GetName(typeof(Shots), shot.Type);
            comp.HunterDPSPoints = dps;
            comp.OverallPoints = dps;
            return comp;
        }
        private ComparisonCalculationHunter comparisonFromShotSpammedMPS(ShotData shot)
        {
            ComparisonCalculationHunter comp = new ComparisonCalculationHunter();

            float shotWait = shot.Duration > shot.Cd ? shot.Duration : shot.Cd;
            float mps = shotWait > 0 ? (float)(shot.ManaCost / shotWait) : 0;

            comp.Name = Enum.GetName(typeof(Shots), shot.Type);
            comp.SubPoints = new float[] { mps };
            comp.OverallPoints = mps;
            return comp;
        }
        private ComparisonCalculationHunter comparisonFromShotRotationDPS(ShotData shot)
        {
            ComparisonCalculationHunter comp = new ComparisonCalculationHunter();
            comp.Name = Enum.GetName(typeof(Shots), shot.Type);
            comp.SubPoints = new float[] { (float)shot.DPS };
            comp.OverallPoints = (float)shot.DPS;
            return comp;
        }
        private ComparisonCalculationHunter comparisonFromShotRotationMPS(ShotData shot)
        {
            ComparisonCalculationHunter comp = new ComparisonCalculationHunter();
            comp.Name = Enum.GetName(typeof(Shots), shot.Type);
            comp.SubPoints = new float[] { (float)shot.MPS };
            comp.OverallPoints = (float)shot.MPS;
            return comp;
        }
        private ComparisonCalculationHunter comparisonFromShotDPM(ShotData shot)
        {
            ComparisonCalculationHunter comp = new ComparisonCalculationHunter();

            float dpm = shot.ManaCost > 0 ? (float)(shot.Damage / shot.ManaCost) : 0;

            comp.Name = Enum.GetName(typeof(Shots), shot.Type);
            comp.SubPoints = new float[] { dpm };
            comp.OverallPoints = dpm;
            return comp;
        }
        private ComparisonCalculationHunter comparisonFromStat(Character character, CharacterCalculationsHunter calcBase, Stats stats, string label)
        {
            ComparisonCalculationHunter comp = new ComparisonCalculationHunter();

            CharacterCalculationsHunter calcStat = GetCharacterCalculations(character, new Item() { Stats = stats }) as CharacterCalculationsHunter;

            comp.Name = label;
            comp.HunterDPSPoints = calcStat.HunterDpsPoints - calcBase.HunterDpsPoints;
            comp.PetDPSPoints = calcStat.PetDpsPoints - calcBase.PetDpsPoints;
            comp.OverallPoints = calcStat.OverallPoints - calcBase.OverallPoints;

            return comp;
        }
        private ComparisonCalculationHunter comparisonFromDouble(string label, float value)
        {
            return new ComparisonCalculationHunter()
            {
                Name = label,
                SubPoints = new float[] { (float)value },
                OverallPoints = (float)value,
            };
        }
        private ComparisonCalculationHunter comparisonFromDoubles(string label, float value1, float value2)
        {
            return new ComparisonCalculationHunter()
            {
                Name = label,
                SubPoints = new float[] { (float)value1, (float)value2 },
                OverallPoints = (float)(value1 + value2),
            };
        }
        #endregion

        #region CalculationsBase Overrides
        private void GenPrioRotation(CharacterCalculationsHunter calculatedStats, CalculationOptionsHunter calcOpts, HunterTalents talents) {
            calculatedStats.priorityRotation = new ShotPriority(calcOpts);
            calculatedStats.priorityRotation.priorities[0] = getShotByIndex(calcOpts.PriorityIndex1, calculatedStats);
            calculatedStats.priorityRotation.priorities[1] = getShotByIndex(calcOpts.PriorityIndex2, calculatedStats);
            calculatedStats.priorityRotation.priorities[2] = getShotByIndex(calcOpts.PriorityIndex3, calculatedStats);
            calculatedStats.priorityRotation.priorities[3] = getShotByIndex(calcOpts.PriorityIndex4, calculatedStats);
            calculatedStats.priorityRotation.priorities[4] = getShotByIndex(calcOpts.PriorityIndex5, calculatedStats);
            calculatedStats.priorityRotation.priorities[5] = getShotByIndex(calcOpts.PriorityIndex6, calculatedStats);
            calculatedStats.priorityRotation.priorities[6] = getShotByIndex(calcOpts.PriorityIndex7, calculatedStats);
            calculatedStats.priorityRotation.priorities[7] = getShotByIndex(calcOpts.PriorityIndex8, calculatedStats);
            calculatedStats.priorityRotation.priorities[8] = getShotByIndex(calcOpts.PriorityIndex9, calculatedStats);
            calculatedStats.priorityRotation.priorities[9] = getShotByIndex(calcOpts.PriorityIndex10, calculatedStats);
            calculatedStats.priorityRotation.validateShots(talents);
        }
        private void GenAbilityCds(Character character, CharacterCalculationsHunter calculatedStats, CalculationOptionsHunter calcOpts, BossOptions bossOpts, HunterTalents talents)
        {
            calculatedStats.serpentSting.Cd = 1.5f;
            calculatedStats.serpentSting.Duration = talents.GlyphOfSerpentSting ? 21 : 15;

            calculatedStats.aimedShot.Cd = talents.GlyphOfAimedShot ? 8 : 10;

            calculatedStats.explosiveShot.Cd = 6;

            calculatedStats.chimeraShot.Cd = talents.GlyphOfChimeraShot ? 9 : 10;

            calculatedStats.arcaneShot.Cd = 6;

            calculatedStats.multiShot.Cd = talents.GlyphOfMultiShot ? 9 : 10;

            calculatedStats.blackArrow.Cd = 30 - (talents.Resourcefulness * 2);
            calculatedStats.blackArrow.Duration = 15;

            calculatedStats.killShot.Cd = talents.GlyphOfKillShot ? 9 : 15;

            calculatedStats.silencingShot.Cd = 20;

            calculatedStats.scorpidSting.Cd = 20;
            calculatedStats.scorpidSting.Duration = 15;

            calculatedStats.viperSting.Cd = 15;
            calculatedStats.viperSting.Duration = 8;

            calculatedStats.immolationTrap.Cd = 30 - talents.Resourcefulness * 2;
            calculatedStats.immolationTrap.Duration = talents.GlyphOfImmolationTrap ? 9 : 15;

            calculatedStats.explosiveTrap.Cd = 30 - talents.Resourcefulness * 2;
            calculatedStats.explosiveTrap.Duration = 20;

            calculatedStats.freezingTrap.Cd = 30 - talents.Resourcefulness * 2;
            calculatedStats.freezingTrap.Duration = 20;

            calculatedStats.frostTrap.Cd = 30 - talents.Resourcefulness * 2;
            calculatedStats.frostTrap.Duration = 30;

#if RAWR3 || SILVERLIGHT
            if (bossOpts.MultiTargs && bossOpts.MultiTargsTime > 0) {
                // Good to go, now change the cooldown based on the multitargs uptime
                calculatedStats.volley.Duration = 6f;
                calculatedStats.volley.Cd = (1f / (bossOpts.MultiTargsTime / calculatedStats.volley.Duration)) * bossOpts.BerserkTimer;
#else
            if (calcOpts.MultipleTargets && calcOpts.MultipleTargetsPerc > 0) {
                // Good to go, now change the cooldown based on the multitargs uptime
                calculatedStats.volley.Duration = 6f;
                calculatedStats.volley.Cd = (1f / ((calcOpts.MultipleTargetsPerc * calcOpts.Duration) / calculatedStats.volley.Duration)) * calcOpts.Duration;
#endif
            } else {
                // invalidate it
                calculatedStats.volley.Cd = -1f;
                //calculatedStats.volley.CastTime = -1f;
                calculatedStats.volley.Duration = -1f;
            }

            if (calculatedStats.priorityRotation.containsShot(Shots.Readiness)) {
                calculatedStats.rapidFire.Cd = 157.5f - (30f * talents.RapidKilling);
            } else {
                calculatedStats.rapidFire.Cd = (5 - talents.RapidKilling) * 60f;
            }
            calculatedStats.rapidFire.Duration = 15;

            // We will set the correct value for this later, after we've calculated haste
            calculatedStats.steadyShot.Cd = 2;

            calculatedStats.readiness.Cd = 180;

            calculatedStats.bestialWrath.Cd = (talents.GlyphOfBestialWrath ? 100f : 120f) * (1f - talents.Longevity * 0.10f);
            calculatedStats.bestialWrath.Duration = calcOpts.PetFamily == PetFamily.None ? 0 : 10;

            // We can calculate the rough frequencies now
            calculatedStats.priorityRotation.initializeTimings();
            if (!calcOpts.UseRotationTest) {
                calculatedStats.priorityRotation.calculateFrequencies();
                calculatedStats.priorityRotation.calculateLALProcs(character);
                calculatedStats.priorityRotation.calculateFrequencies();
                calculatedStats.priorityRotation.calculateFrequencySums();
            }
        }
        private static void GenRotation(Character character, Stats stats, CharacterCalculationsHunter calculatedStats,
            CalculationOptionsHunter calcOpts, BossOptions bossOpts, HunterTalents talents,
            out float rangedWeaponSpeed, out float rangedAmmoDPS, out float rangedWeaponDamage, out float autoShotSpeed,
            out float autoShotsPerSecond, out float specialShotsPerSecond, out float totalShotsPerSecond, out float shotsPerSecondWithoutHawk,
            out RotationTest rotationTest)
        {
            #region Ranged Weapon Stats
            rangedWeaponDamage = 0;
            rangedWeaponSpeed = 0;
            rangedAmmoDPS = 0;

            if (character.Ranged != null) {
                rangedWeaponDamage = (float)(character.Ranged.Item.MinDamage + character.Ranged.Item.MaxDamage) / 2f;
                rangedWeaponSpeed = (float)Math.Round(character.Ranged.Item.Speed * 10f) / 10f;
            }
            if (character.Projectile != null) {
                rangedAmmoDPS = (float)(character.Projectile.Item.MaxDamage + character.Projectile.Item.MinDamage) / 2f;
            }
            #endregion
            #region Static Haste Calcs
            // default quiver speed
            calculatedStats.hasteFromBase = 0.15f;
            // haste from haste rating
            calculatedStats.hasteFromRating = StatConversion.GetHasteFromRating(stats.HasteRating, character.Class);
            // serpent swiftness
            calculatedStats.hasteFromTalentsStatic = 0.04f * talents.SerpentsSwiftness;
            // haste buffs
            calculatedStats.hasteFromRangedBuffs = stats.RangedHaste;
            // total hastes
            calculatedStats.hasteStaticTotal = stats.PhysicalHaste;
            // Needed by the rotation test
            calculatedStats.autoShotStaticSpeed = rangedWeaponSpeed / (1f + calculatedStats.hasteStaticTotal);
            #endregion
            #region Rotation Test
            // Using the rotation test will get us better frequencies
            //RotationTest
                rotationTest = new RotationTest(character, calculatedStats, calcOpts, bossOpts);

            if (calcOpts.UseRotationTest) {
                // The following properties of CalculatedStats must be ready by this call:
                //  * priorityRotation (shot order, durations, cooldowns)
                //  * quickShotsEffect
                //  * hasteStaticTotal
                //  * autoShotStaticSpeed

                rotationTest.RunTest();
            }
            #endregion
            #region Dynamic Haste Calcs
            // Now we have the haste, we can calculate steady shot cast time,
            // then rebuild other various stats.
            // (future enhancement: we only really need to rebuild steady shot)
            calculatedStats.steadyShot.Cd = 2f / (1f + calculatedStats.hasteStaticTotal);
            if (calcOpts.UseRotationTest) {
                calculatedStats.priorityRotation.initializeTimings();
                calculatedStats.priorityRotation.recalculateRatios();
                calculatedStats.priorityRotation.calculateFrequencySums();
            } else {
                calculatedStats.priorityRotation.initializeTimings();
                calculatedStats.priorityRotation.calculateFrequencies();
                calculatedStats.priorityRotation.calculateFrequencySums();
            }
            //float
                autoShotSpeed = rangedWeaponSpeed / (1f + calculatedStats.hasteStaticTotal);
            #endregion
            #region Shots Per Second
            float baseAutoShotsPerSecond = autoShotSpeed > 0 ? 1f / autoShotSpeed : 0;
            //float
                autoShotsPerSecond = baseAutoShotsPerSecond;
            //float
                specialShotsPerSecond = calculatedStats.priorityRotation.specialShotsPerSecond;
            //float
                totalShotsPerSecond = autoShotsPerSecond + specialShotsPerSecond;

            float crittingSpecialsPerSecond = calculatedStats.priorityRotation.critSpecialShotsPerSecond;
            float crittingShotsPerSecond = autoShotsPerSecond + crittingSpecialsPerSecond;

            //float
                shotsPerSecondWithoutHawk = specialShotsPerSecond + baseAutoShotsPerSecond;

            calculatedStats.BaseAttackSpeed = (float)autoShotSpeed;
            calculatedStats.shotsPerSecondCritting = crittingShotsPerSecond;
            #endregion
        }

        public float ConstrainCrit(float lvlDifMOD, float chance) { return Math.Min(1f + lvlDifMOD, Math.Max(0f, chance)); }

        public override CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem,
            bool referenceCalculation, bool significantChange, bool needsDisplayCalculations)
        {
            CharacterCalculationsHunter calculatedStats = new CharacterCalculationsHunter();
            if (character == null) { return calculatedStats; }
            calculatedStats.character = character;
            CalculationOptionsHunter calcOpts = character.CalculationOptions as CalculationOptionsHunter;
            calculatedStats.CalcOpts = calcOpts;
            BossOptions bossOpts = character.BossOptions;
            calculatedStats.BossOpts = bossOpts;
            Stats stats = GetCharacterStats(character, additionalItem);
            HunterTalents talents = character.HunterTalents;
            CombatFactors combatFactors = new CombatFactors(character, stats, calcOpts, bossOpts);
            WhiteAttacks whiteAttacks = new WhiteAttacks(character, stats, combatFactors, calcOpts, bossOpts);

            Stats statsItems = GetItemStats(character, additionalItem);
            Stats statsBuffs = GetBuffsStats(character, calcOpts);
            Stats statsRace = BaseStats.GetBaseStats(character.Level, CharacterClass.Hunter, character.Race);

            //ExplosiveShot es = new ExplosiveShot(character, stats, combatFactors, whiteAttacks, calcOpts);
            //ChimeraShot cs = new ChimeraShot(character, stats, combatFactors, whiteAttacks, calcOpts);

            calculatedStats.BasicStats = stats;
            calculatedStats.BaseHealth = statsRace.Health;

            calculatedStats.pet = new PetCalculations(character, calculatedStats, calcOpts, bossOpts, stats, GetPetBuffsStats(character, calcOpts));
            
            if (character.Ranged == null || (character.Ranged.Item.Type != ItemType.Bow
                                             && character.Ranged.Item.Type != ItemType.Gun
                                             && character.Ranged.Item.Type != ItemType.Crossbow))
            {
                //skip all the calculations if there is no ranged weapon
                return calculatedStats;
            }
#if RAWR3 || SILVERLIGHT
            int   levelDifI = bossOpts.Level - character.Level;
#else
            int   levelDifI = calcOpts.TargetLevel - character.Level;
#endif
            float levelDifF = (float)levelDifI;

            float critMOD = StatConversion.NPC_LEVEL_CRIT_MOD[levelDifI];

            GenPrioRotation(calculatedStats, calcOpts, talents);
            GenAbilityCds(character, calculatedStats, calcOpts, bossOpts, talents);

            float rangedWeaponSpeed = 0, rangedAmmoDPS = 0, rangedWeaponDamage = 0;
            float autoShotSpeed = 0;
            float autoShotsPerSecond = 0, specialShotsPerSecond = 0, totalShotsPerSecond = 0, shotsPerSecondWithoutHawk = 0;
            RotationTest rotationTest;
            GenRotation(character, stats, calculatedStats, calcOpts, bossOpts, talents,
                out rangedWeaponSpeed, out rangedAmmoDPS, out rangedWeaponDamage, out autoShotSpeed,
                out autoShotsPerSecond, out specialShotsPerSecond, out totalShotsPerSecond, out shotsPerSecondWithoutHawk,
                out rotationTest);

            // Hits
            #region Hit vs Miss Chance
            float ChanceToMiss = (float)Math.Max(0f, StatConversion.WHITE_MISS_CHANCE_CAP[levelDifI] - stats.PhysicalHit);
            #endregion

            // Crits
            #region Crit Chance
            float totalAGI = (float)Math.Ceiling(stats.Agility);
            float baseAGI = (float)Math.Ceiling(statsRace.Agility * (1f + stats.BonusAgilityMultiplier));
            calculatedStats.critFromAgi = ConstrainCrit(critMOD, StatConversion.GetCritFromAgility(stats.Agility/*statsRace.Agility * (1f + stats.BonusAgilityMultiplier)*//*totalAGI - baseAGI*/, character.Class) - 0.01536f);
            calculatedStats.critFromRating = ConstrainCrit(critMOD, StatConversion.GetCritFromRating(stats.CritRating, character.Class));

            calculatedStats.critRateOverall = stats.PhysicalCrit;
            #endregion
            #region Bonus Crit Chance
            //Improved Barrage
            float improvedBarrageCritModifier = 0.04f * talents.ImprovedBarrage;
            // Survival instincts
            float survivalInstinctsCritModifier = 0.02f * talents.SurvivalInstincts;
            // Explosive Shot Glyph
            float glyphOfExplosiveShotCritModifier = talents.GlyphOfExplosiveShot ? 0.04f : 0;
            // Sniper Training
            float sniperTrainingCritModifier = 0.05f * talents.SniperTraining;
            #endregion
            #region Shot Crit Chances
            calculatedStats.steadyShot.CritChance = ConstrainCrit(critMOD, stats.PhysicalCrit + survivalInstinctsCritModifier);
            calculatedStats.aimedShot.CritChance = ConstrainCrit(critMOD, stats.PhysicalCrit
                                + (talents.GlyphOfTrueshotAura && talents.TrueshotAura > 0 ? 0.10f : 0f)
                                + improvedBarrageCritModifier);
            calculatedStats.explosiveShot.CritChance = ConstrainCrit(critMOD, stats.PhysicalCrit + glyphOfExplosiveShotCritModifier + survivalInstinctsCritModifier);
            calculatedStats.serpentSting.CritChance = ConstrainCrit(critMOD, stats.PhysicalCrit);
            calculatedStats.chimeraShot.CritChance = ConstrainCrit(critMOD, stats.PhysicalCrit);
            calculatedStats.arcaneShot.CritChance = ConstrainCrit(critMOD, stats.PhysicalCrit + survivalInstinctsCritModifier);
            calculatedStats.multiShot.CritChance = ConstrainCrit(critMOD, stats.PhysicalCrit + improvedBarrageCritModifier);
            calculatedStats.killShot.CritChance = ConstrainCrit(critMOD, stats.PhysicalCrit + sniperTrainingCritModifier);
            calculatedStats.silencingShot.CritChance = ConstrainCrit(critMOD, stats.PhysicalCrit);
            calculatedStats.priorityRotation.calculateCrits();
            #endregion

            // pet - part 1
            #region Pet MPS/Timing Calculations
            // this first block needs to run before the mana adjustments code,
            // since kill command effects mana usage.
            float baseMana = statsRace.Mana;
            calculatedStats.baseMana = statsRace.Mana;
            calculatedStats.pet.GenPetStats();
            #endregion

            // target debuffs
            #region Target Debuffs
            // The pet debuffs deal with stacking correctly themselves
            float targetDebuffsArmor = 1f - (1f - calculatedStats.petArmorDebuffs)
                                          * (1f - statsBuffs.ArmorPenetration); // Buffs!G77

            float targetDebuffsMP5JudgmentOfWisdom = 0;
            if (stats.ManaRestoreFromBaseManaPPM > 0)
            {
                // Note: we ignore the value stored in Buff.cs and calculate it as the spreadsheet
                // does, using shots per second and a derived 50% proc chance.                
                float jowAvgShotTime = autoShotsPerSecond + specialShotsPerSecond > 0f ? 1f / (autoShotsPerSecond + specialShotsPerSecond) : 0f;
                float jowProcChance = 0.5f;
                float jowTimeToProc = jowProcChance > 0 ? 0.25f + jowAvgShotTime / jowProcChance : 0f;
                float jowManaGained = statsRace.Mana * 0.02f;
                float jowMPSGained = jowTimeToProc > 0f ? jowManaGained / jowTimeToProc : 0f;
                targetDebuffsMP5JudgmentOfWisdom = jowTimeToProc > 0f ? jowManaGained / jowTimeToProc * 5f : 0f;
            }
            float targetDebuffsMP5 = targetDebuffsMP5JudgmentOfWisdom; // Buffs!H77

            float targetDebuffsFire   = statsBuffs.BonusFireDamageMultiplier  ; // Buffs!I77
            float targetDebuffsArcane = statsBuffs.BonusArcaneDamageMultiplier; // Buffs!J77
            float targetDebuffsNature = statsBuffs.BonusNatureDamageMultiplier; // Buffs!K77
            float targetDebuffsShadow = statsBuffs.BonusShadowDamageMultiplier;

            float targetDebuffsPetDamage = statsBuffs.BonusPhysicalDamageMultiplier;

            calculatedStats.targetDebuffsArmor = 1f - targetDebuffsArmor;
            calculatedStats.targetDebuffsNature = 1f + targetDebuffsNature;
            calculatedStats.targetDebuffsPetDamage = 1f + targetDebuffsPetDamage;

            //29-10-2009 Drizz: For PiercingShots
            float targetDebuffBleed = statsBuffs.BonusBleedDamageMultiplier;
            #endregion

            // Mana Consumption
            #region Mana Adjustments
            float efficiencyManaAdjust = 1f - (talents.Efficiency * 0.03f);
            float thrillOfTheHuntManaAdjust = 1f - (calculatedStats.priorityRotation.critsCompositeSum * 0.40f * (talents.ThrillOfTheHunt / 3f));
            float masterMarksmanManaAdjust = 1f - (talents.MasterMarksman * 0.05f);
            float glyphOfArcaneShotManaAdjust = 1f;
            if (calculatedStats.priorityRotation.containsShot(Shots.SerpentSting)
                || calculatedStats.priorityRotation.containsShot(Shots.ScorpidSting))
            {
                glyphOfArcaneShotManaAdjust = talents.GlyphOfArcaneShot ? 0.8f : 1f;
            }

            #region Improved Steady Shot
            float resourcefullnessManaAdjust = 1f - talents.Resourcefulness * 0.2f;

            float ISSAimedShotManaAdjust = 1f;
            float ISSArcaneShotManaAdjust = 1f;
            float ISSChimeraShotManaAdjust = 1f;

            float ISSChimeraShotDamageAdjust = 1f;
            float ISSArcaneShotDamageAdjust = 1f;
            float ISSAimedShotDamageAdjust = 1f;

            float ISSProcChance = 0.05f * talents.ImprovedSteadyShot;
            if (ISSProcChance > 0f)
            {                
                if (calcOpts.UseRotationTest)
                {
                    ISSChimeraShotDamageAdjust = 1f + rotationTest.ISSChimeraUptime * 0.15f;
                    ISSArcaneShotDamageAdjust  = 1f + rotationTest.ISSArcaneUptime * 0.15f;
                    ISSAimedShotDamageAdjust   = 1f + rotationTest.ISSAimedUptime * 0.15f;

                    ISSChimeraShotManaAdjust   = 1f - rotationTest.ISSChimeraUptime * 0.2f;
                    ISSArcaneShotManaAdjust    = 1f - rotationTest.ISSArcaneUptime * 0.2f;
                    ISSAimedShotManaAdjust     = 1f - rotationTest.ISSAimedUptime * 0.2f;
                }
                else
                {
                    float ISSRealProcChance = 0f; // N120
                    if (calculatedStats.steadyShot.Freq > 0f)
                    {
                        float ISSSteadyFreq = calculatedStats.steadyShot.Freq;
                        float ISSOtherFreq = calculatedStats.arcaneShot.Freq
                                            + calculatedStats.chimeraShot.Freq
                                            + calculatedStats.aimedShot.Freq;

                        ISSRealProcChance = 1f - (float)Math.Pow(1f - ISSProcChance, ISSOtherFreq / ISSSteadyFreq);
                    }
                    float ISSProcFreqChimera = ISSRealProcChance > 0f ? calculatedStats.chimeraShot.Freq / ISSRealProcChance : 0f; // N121
                    float ISSProcFreqArcane = ISSRealProcChance > 0f ? calculatedStats.arcaneShot.Freq / ISSRealProcChance : 0f; // N122
                    float ISSProcFreqAimed = ISSRealProcChance > 0f ? calculatedStats.aimedShot.Freq / ISSRealProcChance : 0f; // N123

                    float ISSProcFreqSumInverse = (ISSProcFreqChimera > 0f ? 1f / ISSProcFreqChimera : 0f)
                                                 + (ISSProcFreqArcane > 0f ? 1f / ISSProcFreqArcane : 0f)
                                                 + (ISSProcFreqAimed  > 0f ? 1f / ISSProcFreqAimed : 0f);
                    float ISSProcFreqCombined = ISSProcFreqSumInverse > 0f ? 1f / ISSProcFreqSumInverse : 0f; // N124

                    ISSChimeraShotDamageAdjust = ISSProcFreqChimera > 0f ? 1f + ISSRealProcChance * ISSProcFreqCombined / ISSProcFreqChimera * 0.15f : 1f;
                    ISSArcaneShotDamageAdjust  = ISSProcFreqArcane  > 0f ? 1f + ISSRealProcChance * ISSProcFreqCombined / ISSProcFreqArcane * 0.15f : 1f;
                    ISSAimedShotDamageAdjust   = ISSProcFreqAimed   > 0f ? 1f + ISSRealProcChance * ISSProcFreqCombined / ISSProcFreqAimed * 0.15f : 1f;

                    ISSChimeraShotManaAdjust   = ISSProcFreqChimera > 0f ? 1f - ISSRealProcChance * ISSProcFreqCombined / ISSProcFreqChimera * 0.2f : 1f;
                    ISSArcaneShotManaAdjust    = ISSProcFreqArcane  > 0f ? 1f - ISSRealProcChance * ISSProcFreqCombined / ISSProcFreqArcane * 0.2f : 1f;
                    ISSAimedShotManaAdjust     = ISSProcFreqAimed   > 0f ? 1f - ISSRealProcChance * ISSProcFreqCombined / ISSProcFreqAimed * 0.2f : 1f;
                }
            }

            float resourcefulnessManaAdjust = 1f - (talents.Resourcefulness * 0.2f);
            #endregion
            #endregion
            #region Shot Mana Usage

            // we do this ASAP so that we can get the MPS.
            // this allows us to calculate viper/aspect bonuses & penalties

            calculatedStats.steadyShot.ManaCost     = (baseMana * 0.05f) * efficiencyManaAdjust * thrillOfTheHuntManaAdjust * masterMarksmanManaAdjust;
            calculatedStats.serpentSting.ManaCost   = (baseMana * 0.09f) * efficiencyManaAdjust;
            calculatedStats.aimedShot.ManaCost      = (baseMana * 0.08f) * efficiencyManaAdjust * thrillOfTheHuntManaAdjust * masterMarksmanManaAdjust * ISSAimedShotManaAdjust;
            calculatedStats.explosiveShot.ManaCost  = (baseMana * 0.07f) * efficiencyManaAdjust * thrillOfTheHuntManaAdjust;
            calculatedStats.chimeraShot.ManaCost    = (baseMana * 0.12f) * efficiencyManaAdjust * thrillOfTheHuntManaAdjust * masterMarksmanManaAdjust * ISSChimeraShotManaAdjust;
            calculatedStats.arcaneShot.ManaCost     = (baseMana * 0.05f) * efficiencyManaAdjust * thrillOfTheHuntManaAdjust * ISSArcaneShotManaAdjust * glyphOfArcaneShotManaAdjust;
            calculatedStats.multiShot.ManaCost      = (baseMana * 0.09f) * efficiencyManaAdjust * thrillOfTheHuntManaAdjust;
            calculatedStats.blackArrow.ManaCost     = (baseMana * 0.06f) * efficiencyManaAdjust * resourcefulnessManaAdjust;
            calculatedStats.killShot.ManaCost       = (baseMana * 0.07f) * efficiencyManaAdjust * thrillOfTheHuntManaAdjust;
            calculatedStats.silencingShot.ManaCost  = (baseMana * 0.06f) * efficiencyManaAdjust * thrillOfTheHuntManaAdjust;
            calculatedStats.scorpidSting.ManaCost   = (baseMana * 0.11f) * efficiencyManaAdjust;
            calculatedStats.viperSting.ManaCost     = (baseMana * 0.08f) * efficiencyManaAdjust;
            calculatedStats.immolationTrap.ManaCost = (baseMana * 0.13f) * resourcefullnessManaAdjust;
            calculatedStats.explosiveTrap.ManaCost  = (baseMana * 0.19f) * resourcefullnessManaAdjust;
            calculatedStats.freezingTrap.ManaCost   = (baseMana * 0.03f) * resourcefullnessManaAdjust;
            calculatedStats.frostTrap.ManaCost      = (baseMana * 0.02f) * resourcefullnessManaAdjust;
            calculatedStats.volley.ManaCost         = (baseMana * 0.17f) * (1f - (talents.GlyphOfVolley ? 0.20f : 0.00f));
            calculatedStats.rapidFire.ManaCost      = (baseMana * 0.03f);

            calculatedStats.priorityRotation.calculateRotationMPS();

            #endregion
            #region Mana Regen
            // Mp5
            calculatedStats.manaRegenGearBuffs = stats.Mp5 / 5f; // Convert to per sec
            
            // Viper Regen if viper is up 100%
            calculatedStats.manaRegenConstantViper = 0;
            if (calcOpts.SelectedAspect == Aspect.Viper) {
                float viperGlyphAdjust  = talents.GlyphOfAspectOfTheViper ? 1.1f : 1;
                float viperRegenShots   = calculatedStats.BasicStats.Mana * rangedWeaponSpeed / 100f * totalShotsPerSecond * viperGlyphAdjust;
                float viperRegenPassive = calculatedStats.BasicStats.Mana * 0.04f / 3f;
                calculatedStats.manaRegenConstantViper = viperRegenShots + viperRegenPassive;
            }

            // Rapid Recuperation
            // You gain (2*Pts)% of your mana every 3 sec while under the effect
            // of Rapid Fire, and you gain 2% of your mana every 2 sec for
            // 6 sec when you gain Rapid Killing.
            calculatedStats.manaRegenRapidRecuperation = 0;
            if (calculatedStats.rapidFire.Freq > 0) {
                float rapidRecuperationManaGain = (((0.02f * talents.RapidRecuperation) * calculatedStats.BasicStats.Mana) / 3f) * 15f;
                calculatedStats.manaRegenRapidRecuperation = rapidRecuperationManaGain / calculatedStats.rapidFire.Freq;
            }

            // Chimera shot refreshing Viper
            calculatedStats.manaRegenChimeraViperProc = 0;
            if (calculatedStats.priorityRotation.chimeraRefreshesViper) {
                if (calculatedStats.chimeraShot.Freq > 0) {
                    //29-10-2009 Drizz: Comment, 3092 is fetched from the Viper Sting Table on the SpellValues sheet (v92b). The 0.6 comes from ChimeraShotEffect.
                    calculatedStats.manaRegenChimeraViperProc = 0.6f * 3092f / calculatedStats.chimeraShot.Freq;
                }
            }

            // Hunting Party
            float huntingPartyProc = (float)talents.HuntingParty / 3.0f;

            float huntingPartyArcaneFreq = calculatedStats.arcaneShot.Freq;
            float huntingPartyArcaneCrit = calculatedStats.arcaneShot.CritChance;
            float huntingPartyArcaneUptime = huntingPartyArcaneFreq > 0 ? 1f - (float)Math.Pow(1f - huntingPartyArcaneCrit * huntingPartyProc, 15f / huntingPartyArcaneFreq) : 0;

            float huntingPartyExplosiveFreq = calculatedStats.explosiveShot.Freq; // spreadsheet divides by 3, but doesn't use that value?
            float huntingPartyExplosiveCrit = calculatedStats.explosiveShot.CritChance;
            float huntingPartyExplosiveUptime = huntingPartyExplosiveFreq > 0 ? 1f - (float)Math.Pow(1f - huntingPartyExplosiveCrit * huntingPartyProc, 15f / huntingPartyExplosiveFreq) : 0;

            float huntingPartySteadyFreq = calculatedStats.steadyShot.Freq;
            float huntingPartySteadyCrit = calculatedStats.steadyShot.CritChance;
            float huntingPartySteadyUptime = huntingPartySteadyFreq > 0 ? 1f - (float)Math.Pow(1f - huntingPartySteadyCrit * huntingPartyProc, 15f / huntingPartySteadyFreq) : 0;

            float huntingPartyCumulativeUptime = huntingPartyArcaneUptime + ((1f - huntingPartyArcaneUptime) * huntingPartyExplosiveUptime);
            float huntingPartyUptime = huntingPartyCumulativeUptime + ((1f - huntingPartyCumulativeUptime) * huntingPartySteadyUptime);

            calculatedStats.manaRegenHuntingParty = 0.002f * calculatedStats.BasicStats.Mana * huntingPartyUptime;

            // If we've got a replenishment buff up, use that instead of our own Hunting Party
            float manaRegenReplenishment = stats.ManaRestoreFromMaxManaPerSecond * calculatedStats.BasicStats.Mana;
            if (manaRegenReplenishment > 0) {
                calculatedStats.manaRegenHuntingParty = manaRegenReplenishment;
            }

#if RAWR3 || SILVERLIGHT
            calculatedStats.manaRegenFromPots = stats.ManaRestore / (float)bossOpts.BerserkTimer;
#else
            calculatedStats.manaRegenFromPots = stats.ManaRestore / (float)calcOpts.Duration;
#endif

            // Target Debuffs
            calculatedStats.manaRegenTargetDebuffs = targetDebuffsMP5 / 5f;

            // Total
            calculatedStats.manaRegenTotal =
                calculatedStats.manaRegenGearBuffs +
                calculatedStats.manaRegenConstantViper +
                calculatedStats.manaRegenRoarOfRecovery +
                calculatedStats.manaRegenRapidRecuperation +
                calculatedStats.manaRegenChimeraViperProc +
                calculatedStats.manaRegenInvigoration +
                calculatedStats.manaRegenHuntingParty +
                calculatedStats.manaRegenTargetDebuffs +
                calculatedStats.manaRegenFromPots;
            #endregion
            #region Aspect Usage
            float manaRegenTier7ViperBonus = stats.BonusHunter_T7_4P_ViperSpeed > 0 ? 1.2f : 1f;
            float glpyhOfAspectOfTheViperBonus = talents.GlyphOfAspectOfTheViper ? 1.1f : 1f;

            calculatedStats.manaRegenViper = calculatedStats.BasicStats.Mana * (float)Math.Round(rangedWeaponSpeed, 1) / 100f * shotsPerSecondWithoutHawk
                                        * manaRegenTier7ViperBonus * glpyhOfAspectOfTheViperBonus
                                        + stats.Mana * 0.04f / 3f;

            calculatedStats.manaUsageKillCommand = calculatedStats.petKillCommandMPS * (stats.ManaCostPerc);
            calculatedStats.manaUsageRotation = calculatedStats.priorityRotation.MPS;

            calculatedStats.manaUsageTotal = calculatedStats.manaUsageRotation
                                           + calculatedStats.manaUsageKillCommand;

            calculatedStats.manaChangeDuringViper  = calculatedStats.manaRegenTotal - calculatedStats.manaUsageTotal + calculatedStats.manaRegenViper;
            calculatedStats.manaChangeDuringNormal = calculatedStats.manaRegenTotal - calculatedStats.manaUsageTotal;

            calculatedStats.manaTimeToFull = calculatedStats.manaChangeDuringViper  > 0f ? stats.Mana /       calculatedStats.manaChangeDuringViper   : -1f;
            calculatedStats.manaTimeToOOM  = calculatedStats.manaChangeDuringNormal < 0f ? stats.Mana / (0f - calculatedStats.manaChangeDuringNormal) : -1f;

            float PercTimeNoDPSforNoMana     = 0f,
                  viperTimeNeededToLastFight = 0f,
                  aspectUptimeHawk           = 0f,
                  aspectUptimeViper          = 0f;

#if RAWR3 || SILVERLIGHT
            if (calculatedStats.manaTimeToOOM >= 0f && calculatedStats.manaTimeToOOM < bossOpts.BerserkTimer && calculatedStats.manaRegenViper > 0f)
            {
                viperTimeNeededToLastFight = (((0f - calculatedStats.manaChangeDuringNormal) * bossOpts.BerserkTimer) - calculatedStats.BasicStats.Mana) / calculatedStats.manaRegenViper;
            }

            if (calculatedStats.manaTimeToOOM >= 0 && calculatedStats.manaTimeToOOM < bossOpts.BerserkTimer)
            {
                if (calcOpts.AspectUsage == AspectUsage.ViperRegen)
                {
                    aspectUptimeViper = Math.Min(bossOpts.BerserkTimer - calculatedStats.manaTimeToOOM, calculatedStats.manaTimeToFull) / (calculatedStats.manaTimeToFull + calculatedStats.manaTimeToOOM);
                }
                else if (calcOpts.AspectUsage == AspectUsage.ViperToOOM && viperTimeNeededToLastFight > 0f)
                {
                    aspectUptimeViper = Math.Min(bossOpts.BerserkTimer - calculatedStats.manaTimeToOOM, viperTimeNeededToLastFight) / bossOpts.BerserkTimer;
                }
                else if (calcOpts.AspectUsage == AspectUsage.None)
                {
                    PercTimeNoDPSforNoMana = (bossOpts.BerserkTimer - calculatedStats.manaTimeToOOM) / bossOpts.BerserkTimer;
                }
            }

            float aspectUptimeBeast = calcOpts.UseBeastDuringBestialWrath && calculatedStats.bestialWrath.Freq > 0
                ? (calculatedStats.bestialWrath.Duration * (bossOpts.BerserkTimer / calculatedStats.bestialWrath.Cd)) / bossOpts.BerserkTimer : 0;
#else
            if (calculatedStats.manaTimeToOOM >= 0f && calculatedStats.manaTimeToOOM < calcOpts.Duration && calculatedStats.manaRegenViper > 0f) {
                viperTimeNeededToLastFight = (((0f - calculatedStats.manaChangeDuringNormal) * calcOpts.Duration) - calculatedStats.BasicStats.Mana) / calculatedStats.manaRegenViper;
            }

            if (calculatedStats.manaTimeToOOM >= 0 && calculatedStats.manaTimeToOOM < calcOpts.Duration) {
                if      (calcOpts.AspectUsage == AspectUsage.ViperRegen) {
                    aspectUptimeViper = Math.Min(calcOpts.Duration - calculatedStats.manaTimeToOOM, calculatedStats.manaTimeToFull) / (calculatedStats.manaTimeToFull + calculatedStats.manaTimeToOOM);
                }else if(calcOpts.AspectUsage == AspectUsage.ViperToOOM && viperTimeNeededToLastFight > 0f) {
                    aspectUptimeViper = Math.Min(calcOpts.Duration - calculatedStats.manaTimeToOOM, viperTimeNeededToLastFight) / calcOpts.Duration;
                }else if(calcOpts.AspectUsage == AspectUsage.None) {
                    PercTimeNoDPSforNoMana = (calcOpts.Duration - calculatedStats.manaTimeToOOM) / calcOpts.Duration;
                }
            }

            float aspectUptimeBeast = calcOpts.UseBeastDuringBestialWrath && calculatedStats.bestialWrath.Freq > 0
                ? (calculatedStats.bestialWrath.Duration * (calcOpts.Duration / calculatedStats.bestialWrath.Cd)) / calcOpts.Duration : 0;
#endif

            switch (calcOpts.SelectedAspect) {
                case Aspect.Viper:
                    aspectUptimeViper = calcOpts.UseBeastDuringBestialWrath ? 1f - aspectUptimeBeast : 1f;
                    break;
                case Aspect.Beast:
                    aspectUptimeBeast = (calcOpts.AspectUsage == AspectUsage.None) ? 1f : 1f - aspectUptimeViper;
                    break;
                case Aspect.Hawk:
                case Aspect.Dragonhawk:
                    aspectUptimeHawk = 1f - aspectUptimeViper - aspectUptimeBeast;
                    break;
            }

            // we now know aspect uptimes - calculate bonuses and penalties
            float viperDamageEffect  = talents.AspectMastery > 0 ? 0.40f : 0.50f;
            float viperDamagePenalty = aspectUptimeViper * viperDamageEffect;

            float beastStaticAPBonus = talents.GlyphOfTheBeast ? 0.12f : 0.10f;
            float beastAPBonus       = aspectUptimeBeast * beastStaticAPBonus;

            float tier7ViperDamageAdjust = 1.0f + stats.BonusHunter_T7_4P_ViperSpeed * aspectUptimeViper;

            calculatedStats.aspectUptimeHawk      = aspectUptimeHawk;
            calculatedStats.aspectUptimeBeast     = aspectUptimeBeast;
            calculatedStats.aspectUptimeViper     = aspectUptimeViper;
            calculatedStats.aspectViperPenalty    = viperDamagePenalty;
            calculatedStats.aspectBonusAPBeast    = beastAPBonus;
            calculatedStats.NoManaDPSDownTimePerc = PercTimeNoDPSforNoMana;
            #endregion

            // damage
            #region Ranged Attack Power
            calculatedStats.apFromBase = character.Level * 2f - 20f;
            calculatedStats.apFromAGI  = stats.Agility;
            calculatedStats.apFromGear = stats.AttackPower
                                       - calculatedStats.apFromBase
                                       - calculatedStats.apFromAGI;

            // use for pet calculations
            calculatedStats.apSelfBuffed = stats.AttackPower;

            // used for hunter calculations
            calculatedStats.apTotal = calculatedStats.apSelfBuffed;

            float RAP = calculatedStats.apTotal;
            #endregion
            #region Armor Penetration
            float ArmorDamageReduction = GetArmorDamageReduction(character, stats, calcOpts, bossOpts);
            calculatedStats.damageReductionFromArmor = (1f - ArmorDamageReduction);
            #endregion
            #region Damage Adjustments
            // Partial Resists
            float averageResist = (levelDifF) * 0.02f;
            float resist10 = 5.0f * averageResist;
            float resist20 = 2.5f * averageResist;
            float partialResistDamageAdjust = 1f - (resist10 * 0.1f + resist20 * 0.2f);

            // Focused Fire
            float focusedFireDamageAdjust = 1f + 0.01f * talents.FocusedFire;

            // Black Arrow Damage Multiplier
            float blackArrowUptime = 0;
            if (calculatedStats.priorityRotation.containsShot(Shots.BlackArrow)) {
                SpecialEffect blackarrow = new SpecialEffect(Trigger.Use,new Stats(),
                                            calculatedStats.blackArrow.Duration, calculatedStats.blackArrow.Freq);
#if RAWR3 || SILVERLIGHT
                blackArrowUptime = blackarrow.GetAverageUptime(0f, 1f, calculatedStats.autoShotStaticSpeed, (float)bossOpts.BerserkTimer);
#else
                blackArrowUptime = blackarrow.GetAverageUptime(0f, 1f, calculatedStats.autoShotStaticSpeed, (float)calcOpts.Duration);
#endif
            }
            float blackArrowAuraDamageAdjust = 1f + (0.06f * blackArrowUptime);
            float blackArrowSelfDamageAdjust = 1f + (RAP / 225000f);

            // Noxious Stings
            float noxiousStingsSerpentUptime = 0;
            if (calculatedStats.serpentSting.Freq > 0) { noxiousStingsSerpentUptime = calculatedStats.serpentSting.Duration / calculatedStats.serpentSting.Freq; }
            if (calculatedStats.priorityRotation.chimeraRefreshesSerpent) { noxiousStingsSerpentUptime = 1; }
            float noxiousStingsDamageAdjust = 1f + (0.01f * talents.NoxiousStings * noxiousStingsSerpentUptime);
            float noxiousStingsSerpentDamageAdjust = 1f + (0.01f * talents.NoxiousStings);

            // Ferocious Inspiration (calculated by pet model)
            float ferociousInspirationDamageAdjust = calculatedStats.ferociousInspirationDamageAdjust;
            float ferociousInspirationArcaneDamageAdjust = 1f + (0.03f * talents.FerociousInspiration);

            // Improved Tracking
            float improvedTrackingDamageAdjust = 1f + 0.01f * talents.ImprovedTracking;

            // Ranged Weapon Specialization
            float rangedWeaponSpecializationDamageAdjust = 1;
            if (talents.RangedWeaponSpecialization == 1) rangedWeaponSpecializationDamageAdjust = 1.01f;
            if (talents.RangedWeaponSpecialization == 2) rangedWeaponSpecializationDamageAdjust = 1.03f;
            if (talents.RangedWeaponSpecialization == 3) rangedWeaponSpecializationDamageAdjust = 1.05f;

            // Marked For Death (assume hunter's mark is on target)
            float markedForDeathDamageAdjust = 1f + 0.01f * talents.MarkedForDeath;

            // DamageTakenDebuffs
            float targetPhysicalDebuffsDamageAdjust = (1f + statsBuffs.BonusPhysicalDamageMultiplier);

            // Barrage
            float barrageDamageAdjust = 1f + 0.04f * talents.Barrage;

            // Sniper Training
            float sniperTrainingDamageAdjust = 1f + 0.02f * talents.SniperTraining;

            // Improve Stings
            float improvedStingsDamageAdjust = 1f + 0.1f * talents.ImprovedStings;

            // Trap Mastery
            float trapMasteryDamageAdjust = 1f + 0.1f * talents.TrapMastery;

            // T.N.T.
            float TNTDamageAdjust = 1f + 0.02f * talents.TNT;

            // These intermediates group the two common sets of adjustments
            float talentDamageAdjust = focusedFireDamageAdjust
                                            //* beastWithinDamageAdjust
                                            //* sancRetributionAuraDamageAdjust
                                            * blackArrowAuraDamageAdjust
                                            * noxiousStingsDamageAdjust
                                            * ferociousInspirationDamageAdjust
                                            * improvedTrackingDamageAdjust
                                            * rangedWeaponSpecializationDamageAdjust
                                            * markedForDeathDamageAdjust;

            float talentDamageStingAdjust = focusedFireDamageAdjust
                                            //* beastWithinDamageAdjust
                                            //* sancRetributionAuraDamageAdjust
                                            * blackArrowAuraDamageAdjust
                                            * noxiousStingsDamageAdjust
                                            * ferociousInspirationDamageAdjust;

            // Full Bonus Damage Adjust
            float BonusDamageAdjust = 1f + stats.BonusDamageMultiplier;
            #endregion
            #region Bonus Crit Damage
            // MortalShots
            float mortalShotsCritDamage = 0.06f * talents.MortalShots;
            // CritDamageMetaGems
            float metaGemCritDamage = 1f + (statsItems.BonusCritMultiplier * 2);
            // Marked For Death
            float markedForDeathCritDamage = 0.02f * talents.MarkedForDeath;
            float baseCritDamage = (1f + mortalShotsCritDamage) * metaGemCritDamage; // CriticalHitDamage
            float specialCritDamage = (1f + mortalShotsCritDamage + markedForDeathCritDamage) * metaGemCritDamage; // SpecialCritDamage
            #endregion

            // pet - part 2
            #region Pet DPS Calculations
            calculatedStats.pet.calculateDPS();
            #endregion

            // shot damage calcs
            #region AutoShot
            // scope damage only applies to autoshot, so is not added to the normalized damage
            float rangedAmmoDamage           = rangedAmmoDPS * rangedWeaponSpeed;
            float rangedAmmoDamageNormalized = rangedAmmoDPS * 2.8f;

            float damageFromRAP           = RAP / 14f * rangedWeaponSpeed;
            float damageFromRAPNormalized = RAP / 14f * 2.8f;

            float autoShotDamage = rangedWeaponDamage
                                 + rangedAmmoDamage
                                 + stats.WeaponDamage
                                 + damageFromRAP
                                 + stats.ScopeDamage;
            float autoShotDamageNormalized = rangedWeaponDamage
                                           + rangedAmmoDamageNormalized
                                           + stats.WeaponDamage
                                           + damageFromRAPNormalized
                                           + stats.ScopeDamage;

            float autoShotDamageAdjust = talentDamageAdjust
                                       * targetPhysicalDebuffsDamageAdjust
                                       * ArmorDamageReduction
                                       * BonusDamageAdjust;
            float autoShotCritAdjust = 1f * metaGemCritDamage;

            float autoShotDamageReal = CalcEffectiveDamage(autoShotDamage,
                                                           ChanceToMiss,
                                                           ConstrainCrit(critMOD, stats.PhysicalCrit),
                                                           autoShotCritAdjust,
                                                           autoShotDamageAdjust);

            float hunterAutoDPS = autoShotsPerSecond
                                * autoShotDamageReal
                                * (1f - calculatedStats.aspectViperPenalty)
                                * tier7ViperDamageAdjust;

            float QSBaseFrequencyIncrease = 0f;
            if ((calcOpts.SelectedAspect == Aspect.Hawk || calcOpts.SelectedAspect == Aspect.Dragonhawk)
                && talents.ImprovedAspectOfTheHawk > 0)
            {
                float quickShotsEffect = 0.03f * talents.ImprovedAspectOfTheHawk;
                if (talents.GlyphOfTheHawk) { quickShotsEffect += 0.06f; }
                SpecialEffect QuickShots = new SpecialEffect(Trigger.PhysicalHit,
                    new Stats() { RangedHaste = quickShotsEffect, },
                    12f, 0f, 0.10f);
#if RAWR3 || SILVERLIGHT
                QSBaseFrequencyIncrease = QuickShots.GetAverageStats(1f / totalShotsPerSecond, (1f - stats.PhysicalHit), rangedWeaponSpeed, bossOpts.BerserkTimer).RangedHaste;
#else
                QSBaseFrequencyIncrease = QuickShots.GetAverageStats(1f / totalShotsPerSecond, (1f - stats.PhysicalHit), rangedWeaponSpeed, calcOpts.Duration).RangedHaste;
#endif
            }

            calculatedStats.aspectBeastLostDPS = (0f - QSBaseFrequencyIncrease) * (1f - calculatedStats.aspectUptimeHawk) * hunterAutoDPS;

            calculatedStats.AutoshotDPS = hunterAutoDPS;
            #endregion
            #region August 2009 Wild Quiver

            calculatedStats.WildQuiverDPS = 0;
            if (talents.WildQuiver > 0)
            {
                float wildQuiverProcChance = talents.WildQuiver * 0.04f;
                float wildQuiverProcFrequency = (autoShotSpeed / wildQuiverProcChance);
                float wildQuiverDamageNormal = 0.8f * (rangedWeaponDamage + statsItems.WeaponDamage + damageFromRAP);
                float wildQuiverDamageAdjust = talentDamageAdjust * partialResistDamageAdjust * (1f + targetDebuffsNature) * BonusDamageAdjust;

                float wildQuiverDamageReal = CalcEffectiveDamage(
                                                wildQuiverDamageNormal,
                                                ChanceToMiss,
                                                ConstrainCrit(critMOD, stats.PhysicalCrit),
                                                1f,
                                                wildQuiverDamageAdjust
                                              );

                //29-10-2009 Drizz: Added the ViperUpTIme penalty
                calculatedStats.WildQuiverDPS = (wildQuiverDamageReal / wildQuiverProcFrequency) * (1f - calculatedStats.aspectViperPenalty);
            }

            #endregion
            #region August 2009 Steady Shot

            // base = shot_base + gear_weapon_damage + normalized_ammo_dps + (RAP * 0.1)
            //        + (rangedWeaponDamage / ranged_weapon_speed * 2.8)
            float steadyShotDamageNormal = 252f
                        + statsItems.WeaponDamage
                        + rangedAmmoDamageNormalized
                        + (RAP * 0.1f)
                        + (rangedWeaponDamage / rangedWeaponSpeed * 2.8f);
            // adjust = talent_adjust * gronnstalker_bonus * glyph_of_steadyshot
            //          * sniper_training * physcial_debuffs
            // to-maybe-do: Gronnstalker set bonus
            float steadyShotDamageAdjust = talentDamageAdjust
                                            * targetPhysicalDebuffsDamageAdjust
                                            * sniperTrainingDamageAdjust
                                            * (talents.GlyphOfSteadyShot ? 1.1f : 1f)
                                            * BonusDamageAdjust;

            // ****************************************************************************
            // Drizz: 31-10-2009 Aligned the calculations with spreadsheet v92b
            // Also moved the armorReduction adjust to be multiplied after DamageReal Calc
            // Corrected from Spreadsheet changelog 91e "T9 2-set bonus only crits for spell-crit bonus damage (i.e. 50% instead of 100%), not affected by Mortal Shots"
            // This is the reason for the 0.5 multiplier and that markedForDeath is kept outside
            float steadyShotCritAdjust = metaGemCritDamage + 0.5f * mortalShotsCritDamage * (1f + metaGemCritDamage) + markedForDeathCritDamage;

            float steadyShotDamageReal = CalcEffectiveDamage(
                                            steadyShotDamageNormal,
                                            ChanceToMiss,
                                            calculatedStats.steadyShot.CritChance,
                                            steadyShotCritAdjust,
                                            steadyShotDamageAdjust
                                          );

            steadyShotDamageReal *= ArmorDamageReduction;

            calculatedStats.steadyShot.Damage = steadyShotDamageReal;

            float steadyShotAvgNonCritDamage = steadyShotDamageNormal * steadyShotDamageAdjust * ArmorDamageReduction;
            float steadyShotAvgCritDamage = steadyShotAvgNonCritDamage * (1f + steadyShotCritAdjust);
            //021109 Drizz: Have to add the Mangle/Trauma buff effect.
            float steadyShotPiercingShots = (1f + targetDebuffBleed)
                                          * (talents.PiercingShots * 0.1f)
                                          * calculatedStats.steadyShot.CritChance * steadyShotAvgCritDamage;

            //Drizz: Add the piercingShots effect
            calculatedStats.steadyShot.Damage = steadyShotDamageReal + steadyShotPiercingShots;

            #endregion
            #region August 2009 Serpent Sting

            // base_damage = 1210 + (0.2 * RAP)
            float serpentStingDamageBase = (float)Math.Round(1210 + (RAP * 0.2f), 1);

            // T9 2-piece bonus
            float serpentStingT9CritAdjust = 1;
            float serpentStingInterimBonus;
            float serpentStingCriticalHitDamage;
            // 29-10-2009 Drizz: The name in the buff have not switched from Battlegear (i.e. the name is of the Horde buff)
            // if (character.ActiveBuffsContains("Windrunner's Pursuit 2 Piece Bonus"))
            if (stats.BonusHunter_T9_2P_SerpCanCrit > 0) {
                // Drizz : aligned with v92b
                serpentStingInterimBonus = 0.5f + 0.5f * mortalShotsCritDamage + 0.5f;
                serpentStingCriticalHitDamage = serpentStingInterimBonus * (1f + (1f + 0.5f) * (metaGemCritDamage - 1f) / 2f + (1f + 0.5f) * (metaGemCritDamage - 1) / 2);
                serpentStingT9CritAdjust = 1f + ConstrainCrit(critMOD, stats.PhysicalCrit) * serpentStingCriticalHitDamage;
            }

            double serpentStingCritAdjustment = serpentStingT9CritAdjust;

            // damage_adjust = (sting_talent_adjusts ~ noxious stings) * improved_stings * improved_tracking
            //                  + partial_resists * tier-8_2-piece_bonus * target_nature_debuffs * 100%_noxious_stings
            float serpentStingDamageAdjust = focusedFireDamageAdjust
                                                //* beastWithinDamageAdjust
                                                //* sancRetributionAuraDamageAdjust
                                                * blackArrowAuraDamageAdjust
                                                * ferociousInspirationDamageAdjust
                                                * noxiousStingsSerpentDamageAdjust
                                                * improvedStingsDamageAdjust
                                                * improvedTrackingDamageAdjust
                                                * partialResistDamageAdjust
                                                * serpentStingT9CritAdjust
                                                * (1f + targetDebuffsNature)
                                                * BonusDamageAdjust;

            // T8 2-piece bonus
            serpentStingDamageAdjust += statsBuffs.BonusHunter_T8_2P_SerpDmg;

            float serpentStingTicks = calculatedStats.serpentSting.Duration / 3f;
            float serpentStingDamagePerTick = (float)Math.Round(serpentStingDamageBase * serpentStingDamageAdjust / 5f, 1);
            float serpentStingDamageReal = serpentStingDamagePerTick * serpentStingTicks;

            calculatedStats.serpentSting.Type = Shots.SerpentSting;
            calculatedStats.serpentSting.Damage = serpentStingDamageReal;

            #endregion
            #region August 2009 Aimed Shot
            // ****************************************************************************
            // Drizz: 31-10-2009 Aligned the calculations with spreadsheet v92b
            // Also moved the armorReduction adjust to be multiplied after DamageReal Calc

            // base_damage = normalized_shot + 408 (but ammo is not normalized!)
            float aimedShotDamageNormal = (rangedWeaponDamage + rangedAmmoDamage + statsItems.WeaponDamage + damageFromRAPNormalized) + 408f;

            // Corrected from Spreadsheet changelog 91e "T9 2-set bonus only crits for spell-crit bonus damage (i.e. 50% instead of 100%), not affected by Mortal Shots"
            // This is the reason for the 0.5 multiplier and that markedForDeath is kept outside
            float aimedShotCritAdjust = metaGemCritDamage + 0.5f * mortalShotsCritDamage * (1f + metaGemCritDamage) + markedForDeathCritDamage;

            // damage_adjust = talent_adjust * barrage_adjust * target_debuff_adjust * sniper_training_adjust * improved_ss_adjust
            float aimedShotDamageAdjust = talentDamageAdjust * barrageDamageAdjust * targetPhysicalDebuffsDamageAdjust
                                            * sniperTrainingDamageAdjust * ISSAimedShotDamageAdjust * BonusDamageAdjust;

            float aimedShotDamageReal = CalcEffectiveDamage(
                                            aimedShotDamageNormal,
                                            ChanceToMiss,
                                            calculatedStats.aimedShot.CritChance,
                                            aimedShotCritAdjust,
                                            aimedShotDamageAdjust
                                          );

            calculatedStats.aimedShot.Damage = aimedShotDamageReal;

            aimedShotDamageReal *= ArmorDamageReduction;

            //Drizz: added for piercing shots
            float aimedShotAvgNonCritDamage = aimedShotDamageNormal * aimedShotDamageAdjust * ArmorDamageReduction;
            float aimedShotAvgCritDamage = aimedShotAvgNonCritDamage * (1f + aimedShotCritAdjust);
            //021109 Drizz: Have to add the Mangle/Trauma buff effect. 
            float aimedShotPiercingShots = (1f + targetDebuffBleed)
                                         * (character.HunterTalents.PiercingShots * 0.1f)
                                         * calculatedStats.aimedShot.CritChance * aimedShotAvgCritDamage;

            //Drizz: Trying out...
            calculatedStats.aimedShot.Damage = aimedShotDamageReal + aimedShotPiercingShots;

            #endregion
            #region August 2009 Explosive Shot

            // base_damage = 425 + 14% of RAP
            float explosiveShotDamageNormal = 425f + (RAP * 0.14f);

            // crit_damage = 1 + mortal_shots + gem-crit
            float explosiveShotCritAdjust = (1f + mortalShotsCritDamage) * metaGemCritDamage;

            // damage_adjust = talent_adjust * tnt * fire_debuffs * sinper_training * partial_resist
            float explosiveShotDamageAdjust = talentDamageAdjust * TNTDamageAdjust * sniperTrainingDamageAdjust
                                             * partialResistDamageAdjust * (1f + targetDebuffsFire) * BonusDamageAdjust;

            float explosiveShotDamageReal = CalcEffectiveDamage(
                                                explosiveShotDamageNormal,
                                                ChanceToMiss,
                                                calculatedStats.explosiveShot.CritChance,
                                                explosiveShotCritAdjust,
                                                explosiveShotDamageAdjust
                                              );

            float explosiveShotDamagePerShot = explosiveShotDamageReal * 3f;

            calculatedStats.explosiveShot.Damage = explosiveShotDamagePerShot;

            #endregion
            #region August 2009 Chimera Shot

            // base_damage = normalized_autoshot * 125%
            //float chimeraShotDamageNormal = autoShotDamageNormalized * 1.25f;
            // Drizz: Making Changes
            float chimeraShotDamageNormal = (rangedAmmoDamage + (RAP / 14f * 2.8f) + rangedWeaponDamage) * 1.25f;

            // Drizz: In the spreadsheet there is also added a row for + Weapon Damage Gear... not included here.

            // Drizz: 
            // Corrected from Spreadsheet changelog 91e "T9 2-set bonus only crits for spell-crit bonus damage (i.e. 50% instead of 100%), not affected by Mortal Shots"
            // This is the reason for the 0.5 multiplier and that markedForDeath is kept outside
            float chimeraShotCritAdjust = metaGemCritDamage + 0.5f * mortalShotsCritDamage * (1f + metaGemCritDamage) + markedForDeathCritDamage;

            // damage_adjust = talent_adjust * nature_debuffs * ISS_cs_bonus * partial_resist
            float chimeraShotDamageAdjust = talentDamageAdjust * ISSChimeraShotDamageAdjust
                                           * partialResistDamageAdjust * (1f + targetDebuffsNature) * BonusDamageAdjust;

            float chimeraShotDamageReal = CalcEffectiveDamage(
                                                chimeraShotDamageNormal,
                                                ChanceToMiss,
                                                ConstrainCrit(critMOD, stats.PhysicalCrit),
                                                chimeraShotCritAdjust,
                                                chimeraShotDamageAdjust
                                           );

            //Drizz: added for piercing shots
            float chimeraShotAvgNonCritDamage = chimeraShotDamageNormal * talentDamageAdjust * ISSChimeraShotDamageAdjust * (1f + targetDebuffsNature);
            float chimeraShotAvgCritDamage = chimeraShotAvgNonCritDamage * (1f + chimeraShotCritAdjust);
            // 021109 - Drizz: Had to add the Bleed Damage Multiplier
            float chimeraShotPiercingShots = (1f + targetDebuffBleed)
                                           * (character.HunterTalents.PiercingShots * 0.1f)
                                           * ConstrainCrit(critMOD, stats.PhysicalCrit)
                                           * chimeraShotAvgCritDamage;

            calculatedStats.chimeraShot.Damage = chimeraShotDamageReal + chimeraShotPiercingShots;

            // calculate damage from serpent sting
            // Drizz: Adding
            float chimeraShotSerpentMultiplier = improvedStingsDamageAdjust
                                                  * improvedTrackingDamageAdjust
                                                  * noxiousStingsDamageAdjust
                                                  * partialResistDamageAdjust
                                                  * (1f + targetDebuffsNature)
                                                  * (1f + stats.BonusHunter_T8_2P_SerpDmg)
                                                  * focusedFireDamageAdjust
                                                  //* beastWithinDamageAdjust
                                                  //* sancRetributionAuraDamageAdjust
                                                  * blackArrowAuraDamageAdjust
                                                  * ferociousInspirationArcaneDamageAdjust;

            float chimeraShotSerpentStingDamage = (float)Math.Round(serpentStingDamageBase * chimeraShotSerpentMultiplier / 5f, 1) * serpentStingTicks;

            float chimeraShotEffect;
            if (calculatedStats.serpentSting.FailReason_AlreadyUsed)
                chimeraShotEffect = chimeraShotSerpentStingDamage * 0.4f;
            else
                chimeraShotEffect = 0;

            // Drizz: Updates
            float chimeraShotSerpentCritAdjust = metaGemCritDamage + (0.5f * metaGemCritDamage + 0.5f) * mortalShotsCritDamage;
            float chimeraShotSerpentDamageAdjust = (1f - ChanceToMiss) * (1f + ConstrainCrit(critMOD, stats.PhysicalCrit) * chimeraShotSerpentCritAdjust);

            float chimeraShotSerpentTotalAdjust = chimeraShotSerpentDamageAdjust * talentDamageAdjust * (1f + targetDebuffsNature);

            calculatedStats.chimeraShot.Damage += chimeraShotEffect * chimeraShotSerpentTotalAdjust;

            #endregion
            #region August 2009 Arcane Shot

            // base_damage = 492 + weapon_damage_gear + (RAP * 15%)
            float arcaneShotDamageNormal = 492f + statsItems.WeaponDamage + (RAP * 0.15f);

            // Drizz:
            // Corrected from Spreadsheet changelog 91e "T9 2-set bonus only crits for spell-crit bonus damage (i.e. 50% instead of 100%), not affected by Mortal Shots"
            // This is the reason for the 0.5 multiplier and that markedForDeath is kept outside
            float arcaneShotCritAdjust = metaGemCritDamage + 0.5f * mortalShotsCritDamage * (1f + metaGemCritDamage) + markedForDeathCritDamage;

            float arcaneShotDamageAdjust = talentDamageAdjust * partialResistDamageAdjust * (1f + 0.05f * talents.ImprovedArcaneShot)
                                            * ferociousInspirationArcaneDamageAdjust * ISSArcaneShotDamageAdjust * BonusDamageAdjust; // missing arcane_debuffs!

            float arcaneShotDamageReal = CalcEffectiveDamage(
                                            arcaneShotDamageNormal,
                                            ChanceToMiss,
                                            calculatedStats.arcaneShot.CritChance,
                                            arcaneShotCritAdjust,
                                            arcaneShotDamageAdjust
                                          );

            calculatedStats.arcaneShot.Damage = arcaneShotDamageReal;

            #endregion
            #region August 2009 Multi Shot

            float multiShotDamageNormal = rangedWeaponDamage + statsItems.WeaponDamage + rangedAmmoDamage
                                        + calculatedStats.BasicStats.ScopeDamage + 408f + (RAP * 0.2f);
            float multiShotDamageAdjust = talentDamageAdjust * barrageDamageAdjust * targetPhysicalDebuffsDamageAdjust
                                            * ArmorDamageReduction * BonusDamageAdjust; // missing: pvp gloves bonus

            float multiShotDamageReal = CalcEffectiveDamage(
                                            multiShotDamageNormal,
                                            ChanceToMiss,
                                            calculatedStats.multiShot.CritChance,
                                            1f,
                                            multiShotDamageAdjust
                                         );

            calculatedStats.multiShot.Damage = multiShotDamageReal;
            //calculatedStats.multiShot.Dump("Multi Shot");

            #endregion
            #region August 2009 Volley
            float volleyDamageNormal = 353f + RAP * 0.0837f; // * AvgNumTargets
            float volleyDamageAdjust = talentDamageAdjust
                                     * partialResistDamageAdjust
                                     * barrageDamageAdjust
                                     * BonusDamageAdjust;
            float numvolleyTicks = 6f;
            float volleyDamageReal = volleyDamageNormal * volleyDamageAdjust * numvolleyTicks;

            // TODO: Add possibility to crit
            // TODO: Enforce channelled aspect of this ability,
            //       right now it acts like they can just keep going

            calculatedStats.volley.Damage = volleyDamageReal;
            #endregion
            #region August 2009 Black Arrow
            float blackArrowDamageNormal = 2765f + (RAP * 0.1f);

            // this is a long list...
            float blackArrowDamageAdjust = partialResistDamageAdjust * focusedFireDamageAdjust //* beastWithinDamageAdjust
                                          //* sancRetributionAuraDamageAdjust
                                          * noxiousStingsDamageAdjust
                                          * ferociousInspirationDamageAdjust * improvedTrackingDamageAdjust
                                          * rangedWeaponSpecializationDamageAdjust * markedForDeathDamageAdjust
                                          * (sniperTrainingDamageAdjust + trapMasteryDamageAdjust + TNTDamageAdjust - 2f)
                                          * blackArrowSelfDamageAdjust * (1f + targetDebuffsShadow) * BonusDamageAdjust;

            float blackArrowDamage = blackArrowDamageNormal * blackArrowDamageAdjust;

            calculatedStats.blackArrow.Damage = blackArrowDamage;
            #endregion
            #region August 2009 Kill Shot
            // ****************************************************************************
            // Drizz: 31-10-2009 Aligned the calculations with spreadsheet v92b
            // Also moved the armorReduction adjust to be multiplied after DamageReal Calc

            float killShotDamageNormal = (autoShotDamage * 2f) + statsItems.WeaponDamage + 650f + (RAP * 0.4f);

            // Corrected from Spreadsheet changelog 91e "T9 2-set bonus only crits for spell-crit bonus damage (i.e. 50% instead of 100%), not affected by Mortal Shots"
            // This is the reasone for the 0.5 multiplier and that markedForDeath is kept outside
            float killShotCritAdjust = metaGemCritDamage + 0.5f * mortalShotsCritDamage * (1f + metaGemCritDamage) + markedForDeathCritDamage;

            float killShotDamageAdjust = talentDamageAdjust * targetPhysicalDebuffsDamageAdjust * ArmorDamageReduction * BonusDamageAdjust;

            float killShotDamageReal = CalcEffectiveDamage(
                                            killShotDamageNormal,
                                            ChanceToMiss,
                                            calculatedStats.killShot.CritChance,
                                            killShotCritAdjust,
                                            killShotDamageAdjust
                                        );

            calculatedStats.killShot.Damage = killShotDamageReal;

            #endregion
            #region August 2009 Silencing Shot

            float silencingShotDamageNormal = (rangedWeaponDamage + rangedAmmoDamage + damageFromRAPNormalized) * 0.5f;
            float silencingShotDamageAdjust = talentDamageAdjust * targetPhysicalDebuffsDamageAdjust * ArmorDamageReduction * BonusDamageAdjust;
            float silencingShotCritAdjust = 1f * metaGemCritDamage;

            float silencingShotDamageReal = CalcEffectiveDamage(
                                                silencingShotDamageNormal,
                                                ChanceToMiss,
                                                ConstrainCrit(critMOD, stats.PhysicalCrit),
                                                silencingShotCritAdjust,
                                                silencingShotDamageAdjust
                                             );

            calculatedStats.silencingShot.Damage = silencingShotDamageReal;

            #endregion
            #region August 2009 Immolation Trap
            float immolationTrapDamage = 1885f + (0.1f * RAP);
            float immolationTrapDamageAdjust = (1f - targetDebuffsFire) * partialResistDamageAdjust * trapMasteryDamageAdjust
                                              * TNTDamageAdjust * talentDamageStingAdjust * BonusDamageAdjust;
            float immolationTrapProjectedDamage = immolationTrapDamage * immolationTrapDamageAdjust;
            float immolationTrapDamagePerTick = immolationTrapProjectedDamage / (talents.GlyphOfImmolationTrap ? 2.5f : 5f);
            float immolationTrapTicks = talents.GlyphOfImmolationTrap ? 3f : 5f;

            calculatedStats.immolationTrap.Damage = immolationTrapDamagePerTick * immolationTrapTicks;
            #endregion
            #region Explosive Trap
            float explosiveTrapDamage = (((0.1f * RAP) + 523f) + ((0.1f * RAP) + 671f)) / 2f + 900f;
            float explosiveTrapDamageAdjust = (1f - targetDebuffsFire)
                                            * partialResistDamageAdjust
                                            * trapMasteryDamageAdjust
                                            * TNTDamageAdjust
                                            * talentDamageStingAdjust
                                            * BonusDamageAdjust;
            float explosiveTrapCritAdjust = 1f * metaGemCritDamage;
            float explosiveTrapProjectedDamage = explosiveTrapDamage * explosiveTrapDamageAdjust;
            float explosiveTrapDamagePerTick = explosiveTrapProjectedDamage / 20f;
            float explosiveTrapTicks = 20f;

            if (talents.GlyphOfExplosiveTrap) {
                calculatedStats.explosiveTrap.Damage = CalcEffectiveDamage(
                                                            explosiveTrapDamagePerTick,
                                                            0f,
                                                            ConstrainCrit(critMOD, stats.PhysicalCrit),
                                                            explosiveShotCritAdjust,
                                                            explosiveShotDamageAdjust
                                                        )
                                                      * explosiveTrapTicks;
            } else {
                calculatedStats.explosiveTrap.Damage = explosiveTrapDamagePerTick * explosiveTrapTicks;
            }
            #endregion
            #region Freezing Trap
            calculatedStats.freezingTrap.Damage = 0f;
            #endregion
            #region Frost Trap
            calculatedStats.frostTrap.Damage = 0f;
            #endregion
            #region Rapid Fire
            calculatedStats.rapidFire.Damage = 0;
            #endregion
            #region Piercing Shots
            calculatedStats.PiercingShotsDPS            = 0;
            calculatedStats.PiercingShotsDPSSteadyShot  = 0;
            calculatedStats.PiercingShotsDPSAimedShot   = 0;
            calculatedStats.PiercingShotsDPSChimeraShot = 0;

            if (talents.PiercingShots > 0) {
                float piercingShotsDamageDone             = talents.PiercingShots * 0.1f;
                float piercingShotsMangleOnTarget         = targetDebuffBleed;
                float piercingShotsTotalModifier          = piercingShotsDamageDone * piercingShotsMangleOnTarget;
                float piercingShotsSteadyShotFrequency    = calculatedStats.steadyShot.Freq;
                float piercingShotsSteadyShotDamageAdded  = steadyShotPiercingShots;

                float piercingShotsAimedShotFrequency     = calculatedStats.aimedShot.Freq;
                float piercingShotsAimedShotDamageAdded   = aimedShotPiercingShots;
                float piercingShotsChimeraShotFrequency   = calculatedStats.chimeraShot.Freq;
                float piercingShotsChimeraShotDamageAdded = chimeraShotPiercingShots;

                if (piercingShotsSteadyShotFrequency > 0) {
                    calculatedStats.PiercingShotsDPS += piercingShotsSteadyShotDamageAdded / piercingShotsSteadyShotFrequency;
                }
                if (piercingShotsAimedShotFrequency > 0) {
                    calculatedStats.PiercingShotsDPS += piercingShotsAimedShotDamageAdded / piercingShotsAimedShotFrequency;
                }
                if (piercingShotsChimeraShotFrequency > 0) {
                    calculatedStats.PiercingShotsDPS += piercingShotsChimeraShotDamageAdded / piercingShotsChimeraShotFrequency;
                }

                calculatedStats.PiercingShotsDPSSteadyShot  = piercingShotsSteadyShotDamageAdded;
                calculatedStats.PiercingShotsDPSAimedShot   = piercingShotsAimedShotDamageAdded;
                calculatedStats.PiercingShotsDPSChimeraShot = piercingShotsChimeraShotDamageAdded;
            }
            #endregion

            #region August 2009 Shot Rotation
            calculatedStats.priorityRotation.viperDamagePenalty = calculatedStats.aspectViperPenalty;
            calculatedStats.priorityRotation.NoManaPenalty = PercTimeNoDPSforNoMana;
            calculatedStats.priorityRotation.calculateRotationDPS();
            calculatedStats.CustomDPS = calculatedStats.priorityRotation.DPS;
            #endregion
            #region August 2009 Kill Shot Sub-20% Usage

            float killShotCurrentFreq = calculatedStats.killShot.Freq;
            float killShotPossibleFreq = calcOpts.UseRotationTest ? calculatedStats.killShot.Freq : calculatedStats.killShot.start_freq;
            float steadyShotCurrentFreq = calculatedStats.steadyShot.Freq;

            float steadyShotNewFreq = steadyShotCurrentFreq;
            if (killShotCurrentFreq == 0f && steadyShotCurrentFreq > 0f && killShotPossibleFreq > 0f)
            {
                steadyShotNewFreq = 1f / (1f / steadyShotCurrentFreq - 1f / killShotPossibleFreq);
            }

            float oldKillShotDPS = calculatedStats.killShot.DPS;
            float newKillShotDPS = killShotPossibleFreq > 0 ? calculatedStats.killShot.Damage / killShotPossibleFreq : 0f;
            newKillShotDPS *= (1f - calculatedStats.aspectViperPenalty);

            float oldSteadyShotDPS = calculatedStats.steadyShot.DPS;
            float newSteadyShotDPS = steadyShotNewFreq > 0 ? calculatedStats.steadyShot.Damage / steadyShotNewFreq : 0f;
            newSteadyShotDPS *= (1f - calculatedStats.aspectViperPenalty);

            float killShotDPSGain = newKillShotDPS > 0f ? (newKillShotDPS + newSteadyShotDPS) - (oldKillShotDPS + oldSteadyShotDPS) : 0f;

            float timeSpentSubTwenty = 0;
#if RAWR3 || SILVERLIGHT
            if (bossOpts.BerserkTimer > 0 && bossOpts.Under20Perc > 0) timeSpentSubTwenty = (float)bossOpts.Under20Perc;
#else
            if (calcOpts.Duration > 0 && calcOpts.TimeSpentSub20 > 0) timeSpentSubTwenty = (float)calcOpts.TimeSpentSub20 / (float)calcOpts.Duration;
#endif
            if (calcOpts.BossHPPerc < 0.2f) timeSpentSubTwenty = 1f;

            float killShotSubGain = timeSpentSubTwenty * killShotDPSGain;

            calculatedStats.killShotSub20NewSteadyFreq = steadyShotNewFreq;
            calculatedStats.killShotSub20NewDPS = newKillShotDPS;
            calculatedStats.killShotSub20NewSteadyDPS = newSteadyShotDPS;
            calculatedStats.killShotSub20Gain = killShotDPSGain;
            calculatedStats.killShotSub20TimeSpent = timeSpentSubTwenty;
            calculatedStats.killShotSub20FinalGain = killShotSubGain;

            #endregion

            #region Survivability
            float Health2SurvHunter = (stats.Health) / 100f;
            Health2SurvHunter += (stats.HealthRestore) / 1000f;
            float Health2SurvPet = (calculatedStats.pet.PetStats.Health) / 100f;
            Health2SurvPet += (calculatedStats.pet.PetStats.HealthRestore) / 1000f;
            float DmgTakenMods2SurvHunter = (1f - stats.DamageTakenMultiplier) * 100f;
            float DmgTakenMods2SurvPet = (1f - calculatedStats.pet.PetStats.DamageTakenMultiplier) * 100f;
            float BossAttackPower2Surv = stats.BossAttackPower / 14f * -1f;
            float BossAttackSpeedMods2Surv = (1f - stats.BossAttackSpeedMultiplier) * 100f;
            float AvoidanceHunter = stats.Dodge + stats.Parry;
            float AvoidancePet = calculatedStats.pet.PetStats.Dodge + calculatedStats.pet.PetStats.Parry;// should be pet stats
            float Armor2SurvHunter = (stats.Armor) / 100f;
            float Armor2SurvPet = (calculatedStats.pet.PetStats.Armor) / 100f;
            //float spiritbondcoeff = (talents.SpiritBond * 0.01f * (calcOpts.Duration / 10f));
#if RAWR3 || SILVERLIGHT
            float HealsPerSecHunter = ((talents.SpiritBond * 0.01f * stats.Health) * (bossOpts.BerserkTimer / 10f)) / bossOpts.BerserkTimer;
            float HealsPerSecPet = ((talents.SpiritBond * 0.01f * calculatedStats.pet.PetStats.Health) * (bossOpts.BerserkTimer / 10f)) / bossOpts.BerserkTimer;
#else
            float HealsPerSecHunter = ((talents.SpiritBond * 0.01f * stats.Health) * (calcOpts.Duration / 10f)) / calcOpts.Duration;
            float HealsPerSecPet = ((talents.SpiritBond * 0.01f * calculatedStats.pet.PetStats.Health) * (calcOpts.Duration / 10f)) / calcOpts.Duration;
#endif
            #endregion

            #region Zod's Repeating Longbow
            // Equip: Your ranged attacks have a [x]% chance to cause you
            // to instantly attack with this weapon for 50% weapon damage.
            // iLevel 264 | x = 4
            // iLevel 277 | x = 5
            calculatedStats.BonusAttackProcsDPS = 0f;
            if (stats.ZodProc > 0) {
                float Chance = stats.ZodProc;
                float ProcDamage = rangedWeaponDamage
                                 + rangedAmmoDamage
                                 + stats.WeaponDamage
                                 + stats.ScopeDamage;
                float ProcDamageReal = CalcEffectiveDamage(ProcDamage * 0.50f,
                                                           ChanceToMiss,
                                                           0f,
                                                           1f,
                                                           autoShotDamageAdjust);
                SpecialEffect zod = new SpecialEffect(Trigger.RangedHit, new Stats(), 0f, 0f, Chance);
#if RAWR3 || SILVERLIGHT
                float numProcs = bossOpts.BerserkTimer * zod.GetAverageProcsPerSecond(totalShotsPerSecond, 1f, autoShotSpeed, bossOpts.BerserkTimer);
                float totalDamage = numProcs * ProcDamageReal;
                calculatedStats.BonusAttackProcsDPS = totalDamage / bossOpts.BerserkTimer;
#else
                float numProcs = calcOpts.Duration * zod.GetAverageProcsPerSecond(totalShotsPerSecond, 1f, autoShotSpeed, calcOpts.Duration);
                float totalDamage = numProcs * ProcDamageReal;
                calculatedStats.BonusAttackProcsDPS = totalDamage / calcOpts.Duration;
#endif
            }
            #endregion

            #region Special Damage Procs, like Bandit's Insignia or Hand-mounted Pyro Rockets
            Dictionary<Trigger, float> triggerIntervals = new Dictionary<Trigger, float>();
            Dictionary<Trigger, float> triggerChances = new Dictionary<Trigger, float>();
            CalculateTriggers(character, calculatedStats, stats, calcOpts, bossOpts, triggerIntervals, triggerChances);
            DamageProcs.SpecialDamageProcs SDP;
            calculatedStats.SpecProcDPS = 0f;
            if (stats._rawSpecialEffectData != null)
            {
                SDP = new Rawr.DamageProcs.SpecialDamageProcs(character, stats,
#if RAWR3 || SILVERLIGHT
                    bossOpts.Level - character.Level, new List<SpecialEffect>(stats._rawSpecialEffectData),
                    triggerIntervals, triggerChances, bossOpts.BerserkTimer, combatFactors.DamageReduction);
#else
                    calcOpts.TargetLevel - character.Level, new List<SpecialEffect>(stats._rawSpecialEffectData),
                    triggerIntervals, triggerChances, calcOpts.Duration, combatFactors.DamageReduction);
#endif
                calculatedStats.SpecProcDPS += SDP.Calculate(ItemDamageType.Physical);
                calculatedStats.SpecProcDPS += SDP.Calculate(ItemDamageType.Shadow);
                calculatedStats.SpecProcDPS += SDP.Calculate(ItemDamageType.Holy);
                calculatedStats.SpecProcDPS += SDP.Calculate(ItemDamageType.Arcane);
                calculatedStats.SpecProcDPS += SDP.Calculate(ItemDamageType.Nature);
                calculatedStats.SpecProcDPS += SDP.Calculate(ItemDamageType.Fire);
                calculatedStats.SpecProcDPS += SDP.Calculate(ItemDamageType.Frost);
            }
            #endregion

            calculatedStats.HunterDpsPoints = (float)(calculatedStats.AutoshotDPS
                                                    + calculatedStats.WildQuiverDPS
                                                    + calculatedStats.CustomDPS
                                                    + calculatedStats.killShotSub20FinalGain
                                                    + calculatedStats.aspectBeastLostDPS
                                                    + calculatedStats.BonusAttackProcsDPS
                                                    + calculatedStats.SpecProcDPS);
            calculatedStats.HunterSurvPoints = calcOpts.SurvScale *
                                               (0 // TotalHPSOnHunter
                                                + Health2SurvHunter
                                                + DmgTakenMods2SurvHunter
                                                + BossAttackPower2Surv
                                                + BossAttackSpeedMods2Surv
                                                + AvoidanceHunter * 100f
                                                + Armor2SurvHunter
                                                + HealsPerSecHunter);
            calculatedStats.PetSurvPoints = calcOpts.SurvScale *
                                            (0 // TotalHPSOnPet
                                             + Health2SurvPet
                                             + DmgTakenMods2SurvPet
                                             + BossAttackPower2Surv
                                             + BossAttackSpeedMods2Surv
                                             + AvoidancePet * 100f
                                             + Armor2SurvPet
                                             + HealsPerSecPet);
            calculatedStats.OverallPoints = calculatedStats.HunterDpsPoints
                                          + calculatedStats.PetDpsPoints
                                          + calculatedStats.HunterSurvPoints
                                          + calculatedStats.PetSurvPoints;

            return calculatedStats;
        }

        public override Stats GetCharacterStats(Character character, Item additionalItem) {
            try {
                CalculationOptionsHunter calcOpts = character.CalculationOptions as CalculationOptionsHunter;
                if (calcOpts == null) { calcOpts = new CalculationOptionsHunter(); character.CalculationOptions = calcOpts; }
                BossOptions bossOpts = character.BossOptions;
                HunterTalents talents = character.HunterTalents;
#if RAWR3 || SILVERLIGHT
                PetTalents petTalents = calcOpts.PetTalents;
                int levelDif = bossOpts.Level - character.Level;
#else
                PetTalentTreeData petTalents = calcOpts.PetTalents;
                int levelDif = calcOpts.TargetLevel - character.Level;
#endif

                #region From Race
                Stats statsRace = BaseStats.GetBaseStats(character.Level, CharacterClass.Hunter, character.Race);
                statsRace.PhysicalCrit += (character.Ranged != null && 
                    ((character.Race == CharacterRace.Dwarf && character.Ranged.Item.Type == ItemType.Gun) ||
                    (character.Race == CharacterRace.Troll && character.Ranged.Item.Type == ItemType.Bow))) ?
                    0.01f : 0.00f;
                #endregion
                #region From Gear/Buffs
                Stats statsBuffs = GetBuffsStats(character, calcOpts);
                Stats statsItems = GetItemStats(character, additionalItem);
                #endregion
                #region From Options
                Stats statsOptionsPanel = new Stats() {
                    PhysicalCrit = StatConversion.NPC_LEVEL_CRIT_MOD[levelDif],
                    PhysicalHaste = 0.15f, // This is from what Bags used to give that got rolled into the class
                };
                CharacterCalculationsHunter calculatedStats = new CharacterCalculationsHunter();
                GenPrioRotation(calculatedStats, calcOpts, talents);
                GenAbilityCds(character, calculatedStats, calcOpts, bossOpts, talents);
                if (calculatedStats.priorityRotation.containsShot(Shots.RapidFire)) {
                    statsOptionsPanel.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                        new Stats() { RangedHaste = (talents.GlyphOfRapidFire ? 0.48f : 0.40f), },
                        15f, (5 - talents.RapidKilling) * 60f));
                }
                #endregion
                #region From Talents
                Stats statsTalents = new Stats()
                {
                    PhysicalHit = (talents.FocusedAim * 0.01f),
                    PhysicalCrit = (0.01f * talents.LethalShots)
                                 + (0.01f * talents.MasterMarksman)
                                 + (0.01f * talents.KillerInstinct),
                    BonusAgilityMultiplier = (1f + 0.02f * talents.CombatExperience)
                                           * (1f + 0.03f * talents.LightningReflexes)
                                           * (1f + 0.01f * talents.HuntingParty)
                                           - 1f,
                    BonusIntellectMultiplier = 0.02f * talents.CombatExperience,
                    BonusStaminaMultiplier = 0.02f * talents.Survivalist,
                    BaseArmorMultiplier = (talents.ThickHide / 3f) * 0.10f,
                    PhysicalHaste = 0.04f * talents.SerpentsSwiftness,
                    BonusAttackPowerMultiplier = 0.10f * talents.TrueshotAura,
                    BonusDamageMultiplier = 0.10f * talents.TheBeastWithin,
                    BonusPetDamageMultiplier = -1f * 0.10f * talents.TheBeastWithin,
                    Dodge = talents.CatlikeReflexes * 0.01f,
                    Parry = talents.Deflection * 0.01f,
                    BonusHealthMultiplier = talents.EnduranceTraining * 0.01f,
                    DamageTakenMultiplier = -0.02f * talents.SurvivalInstincts,
                };
                if (talents.MasterTactician > 0) {
                    SpecialEffect mt = new SpecialEffect(Trigger.PhysicalHit,
                        new Stats() { PhysicalCrit = talents.MasterTactician * 0.02f, }, 8f, 0f, 0.10f);
                    statsTalents.AddSpecialEffect(mt);
                }
                if ((calcOpts.SelectedAspect == Aspect.Hawk || calcOpts.SelectedAspect == Aspect.Dragonhawk)
                    && talents.ImprovedAspectOfTheHawk > 0)
                {
                    float quickShotsEffect = 0.03f * talents.ImprovedAspectOfTheHawk;
                    if (talents.GlyphOfTheHawk) { quickShotsEffect += 0.06f; }
                    SpecialEffect QuickShots = new SpecialEffect(Trigger.PhysicalHit,
                        new Stats() { RangedHaste = quickShotsEffect, },
                        12f, 0f, 0.10f);
                    statsTalents.AddSpecialEffect(QuickShots);
                }
                if (calcOpts.SelectedAspect == Aspect.Hawk || (calcOpts.SelectedAspect == Aspect.Dragonhawk && talents.AspectMastery > 0)) {
                    statsOptionsPanel.RangedAttackPower += 155f * (1f + talents.AspectMastery * 0.30f);
                }
#if RAWR3 || SILVERLIGHT
                if (petTalents.CallOfTheWild > 0) {
#else
                if (petTalents.CallOfTheWild.Value > 0) {
#endif
                    SpecialEffect callofthewild = new SpecialEffect(Trigger.Use,
                        new Stats() { BonusRangedAttackPowerMultiplier = 0.10f, BonusPetAttackPowerMultiplier = 0.10f, },
                        20f, (5f * 60f) * (1f - talents.Longevity * 0.10f));
                    statsTalents.AddSpecialEffect(callofthewild);
                }
                if (calcOpts.PetFamily != PetFamily.None
                    && calculatedStats.priorityRotation.containsShot(Shots.BestialWrath)
                    && talents.BestialWrath > 0)
                {
                    float cooldown = (talents.GlyphOfBestialWrath ? 100f : 120f) * (1f - talents.Longevity * 0.10f);
                    float val1 = talents.TheBeastWithin > 0 ?  0.10f : 0f;
                    float val2 = talents.TheBeastWithin > 0 ? -0.50f : 0f;
                    SpecialEffect WrathBeastWithin = new SpecialEffect(Trigger.Use,
                        new Stats() { BonusPetDamageMultiplier = (0.50f /* / (1f + val1)*/), BonusDamageMultiplier = val1, ManaCostPerc = val2 },
                        10f, cooldown);
                    statsTalents.AddSpecialEffect(WrathBeastWithin);
                }
#if RAWR3 || SILVERLIGHT
                if (petTalents.CullingTheHerd > 0) {
                    float val1 = petTalents.CullingTheHerd * 0.01f;
#else
                if (petTalents.CullingTheHerd.Value > 0) {
                    float val1 = petTalents.CullingTheHerd.Value * 0.01f;
#endif
                    SpecialEffect CullingTheHerd = new SpecialEffect(Trigger.PetClawBiteSmackCrit,
                        new Stats() { BonusDamageMultiplier = val1, BonusPetDamageMultiplier = val1, },
                        10f, 0f);
                    statsTalents.AddSpecialEffect(CullingTheHerd);
                }
                #endregion

                // Totals
                Stats statsGearEnchantsBuffs = new Stats();
                statsGearEnchantsBuffs.Accumulate(statsItems);
                statsGearEnchantsBuffs.Accumulate(statsBuffs);
                Stats statsTotal = new Stats();
                statsTotal.Accumulate(statsRace);
                statsTotal.Accumulate(statsItems);
                statsTotal.Accumulate(statsBuffs);
                statsTotal.Accumulate(statsTalents);
                statsTotal.Accumulate(statsOptionsPanel);
                Stats statsProcs = new Stats();

                #region Stamina & Health
                float totalBSTAM = statsTotal.BonusStaminaMultiplier;
                float staBase    = (float)Math.Floor((1f + totalBSTAM) * statsRace.Stamina);
                float staBonus   = (float)Math.Floor((1f + totalBSTAM) * statsGearEnchantsBuffs.Stamina);
                statsTotal.Stamina = staBase + staBonus;

                // Health
                statsTotal.Health += StatConversion.GetHealthFromStamina(statsTotal.Stamina);
                statsTotal.Health *= 1f + statsTotal.BonusHealthMultiplier;
                #endregion
                #region Strength
                float totalBSTRM = statsTotal.BonusStrengthMultiplier;
                float strBase    = (float)Math.Floor((1f + totalBSTRM) * statsRace.Strength);
                float strBonus   = (float)Math.Floor((1f + totalBSTRM) * statsGearEnchantsBuffs.Strength);
                statsTotal.Strength = strBase + strBonus;
                #endregion
                #region  Agility
                float totalBAGIM = statsTotal.BonusAgilityMultiplier;
                float agiBase    = /*(float)Math.Floor(*/(1f + totalBAGIM) * statsRace.Agility/*)*/;
                float agiBonus   = /*(float)Math.Floor(*/(1f + totalBAGIM) * statsGearEnchantsBuffs.Agility/*)*/;
                statsTotal.Agility = Math.Max(0f, agiBase + agiBonus);

                if (talents.ExposeWeakness > 0) {
                    SpecialEffect ExposeWeakness = new SpecialEffect(Trigger.RangedCrit,
                        new Stats() { AttackPower = statsTotal.Agility * 0.25f },
                        7f, 0f, (1f / 3f * talents.ExposeWeakness));
                    statsTotal.AddSpecialEffect(ExposeWeakness);
                }
                #endregion
                #region  Intellect
                float totalBINTM = statsTotal.BonusIntellectMultiplier;
                float intBase    = (float)Math.Floor((1f + totalBINTM) * statsRace.Intellect);
                float intBonus   = (float)Math.Floor((1f + totalBINTM) * statsGearEnchantsBuffs.Intellect);
                statsTotal.Intellect = intBase + intBonus;
                #endregion
                #region Armor
                statsTotal.Armor = /*(float)Math.Floor(*/statsTotal.Armor * (1f + statsTotal.BaseArmorMultiplier)/*)*/;
                statsTotal.BonusArmor += statsTotal.Agility * 2f;
                statsTotal.BonusArmor = /*(float)Math.Floor(*/statsTotal.BonusArmor * (1f + statsTotal.BonusArmorMultiplier)/*)*/;
                statsTotal.Armor += statsTotal.BonusArmor;
                #endregion
                #region Attack Power
                statsTotal.BonusAttackPowerMultiplier *= (1f + statsTotal.BonusRangedAttackPowerMultiplier);
                float totalBAPM    = statsTotal.BonusAttackPowerMultiplier;
                float apBase       = (1f + totalBAPM) * (statsRace.AttackPower + statsRace.RangedAttackPower);
                float apFromAGI    = (1f + totalBAPM) * (statsTotal.Agility);
                //float apFromSTR    = (1f + totalBAPM) * (statsTotal.Strength);
                float apFromHvW    = (1f + totalBAPM) * (statsTotal.Stamina * (0.10f) * talents.HunterVsWild);
                float apFromCAim   = (1f + totalBAPM) * (statsTotal.Intellect * (1f/3f) * talents.CarefulAim);
                float apFromHM     = (1f + totalBAPM) * (500f * (1f + talents.ImprovedHuntersMark * 0.10f) * (talents.GlyphOfHuntersMark ? 1.20f : 1f));
                float apBonusOther = (1f + totalBAPM) * (statsGearEnchantsBuffs.AttackPower + statsGearEnchantsBuffs.RangedAttackPower
                                                         + statsOptionsPanel.AttackPower + statsOptionsPanel.RangedAttackPower);
                statsTotal.AttackPower = Math.Max(0f, apBase + apFromAGI /*+ apFromSTR*/ + apFromHvW + apFromCAim + apFromHM + apBonusOther);
                statsTotal.RangedAttackPower = statsTotal.AttackPower;
                #endregion
                #region Spell Power
                float totalBSPM    = statsTotal.BonusSpellPowerMultiplier;
                float spBase       = (1f + totalBSPM) * (statsRace.SpellPower);
                float spBonusOther = (1f + totalBSPM) * (statsGearEnchantsBuffs.SpellPower);
                statsTotal.SpellPower = (float)Math.Floor(spBase + spBonusOther);
                #endregion
                #region Crit
                statsTotal.CritRating += statsTotal.RangedCritRating;
                statsTotal.PhysicalCrit += StatConversion.GetCritFromAgility(statsTotal.Agility /*- agiBase*/, character.Class)
                                         + -0.01536f
                                         + StatConversion.GetCritFromRating(Math.Max(0f, statsTotal.CritRating), character.Class);
                #endregion
                #region Haste
                statsTotal.HasteRating += statsTotal.RangedHasteRating;
                float ratingHasteBonus = StatConversion.GetPhysicalHasteFromRating(Math.Max(0f, statsTotal.HasteRating), character.Class);
                statsTotal.PhysicalHaste = (1f + statsRace.PhysicalHaste) *
                                           (1f + statsItems.PhysicalHaste) *
                                           (1f + statsBuffs.PhysicalHaste) *
                                           (1f + statsTalents.PhysicalHaste) *
                                           (1f + statsOptionsPanel.PhysicalHaste) *
                                           (1f + statsTotal.RangedHaste) *
                                           (1f + ratingHasteBonus)
                                           - 1f;
                #endregion
                #region Hit
                statsTotal.PhysicalHit += StatConversion.GetHitFromRating(Math.Max(0f, statsTotal.HitRating + statsTotal.RangedHitRating));
                statsTotal.SpellHit    += StatConversion.GetHitFromRating(Math.Max(0f, statsTotal.HitRating));
                #endregion
                #region Mana
                // The first 20 Int = 20 Mana, while each subsequent Int = 15 Mana
                // (20-(20/15)) = 18.66666
                // spreadsheet uses 18.7, so we will too :)
                statsTotal.Mana = (float)(statsRace.Mana + 15f * (statsTotal.Intellect - 18.7f) + statsGearEnchantsBuffs.Mana);
                #endregion
                #region ArP
                if (statsTotal.ArmorPenetrationRating < 0) statsTotal.ArmorPenetrationRating = 0;
                #endregion

                #region Handle Special Effects
                calculatedStats.pet = new PetCalculations(character, calculatedStats, calcOpts, bossOpts, statsTotal,
                    GetPetBuffsStats(character, calcOpts));
                calculatedStats.pet.GenPetStats();

                Dictionary<Trigger, float> triggerIntervals = new Dictionary<Trigger, float>();
                Dictionary<Trigger, float> triggerChances = new Dictionary<Trigger, float>();

                CalculateTriggers(character, calculatedStats, statsTotal, calcOpts, bossOpts, triggerIntervals, triggerChances);

                if (calcOpts.PetFamily == PetFamily.Wolf
                    && calculatedStats.pet.priorityRotation.getSkillFrequency(PetAttacks.FuriousHowl) > 0)
                {
                    statsTotal.AddSpecialEffect(FuriousHowl);
                }
                
                statsProcs += GetSpecialEffectsStats(character, triggerIntervals, triggerChances, statsTotal, null);

                #region Handle Results of Special Effects
                // Base Stats
                statsProcs.Stamina  = (float)Math.Floor(statsProcs.Stamina     * (1f + totalBSTAM) * (1f + statsProcs.BonusStaminaMultiplier));
                statsProcs.Strength = (float)Math.Floor(statsProcs.Strength    * (1f + totalBSTRM) * (1f + statsProcs.BonusStrengthMultiplier));
                statsProcs.Agility  = statsProcs.Agility     * (1f + totalBAGIM) * (1f + statsProcs.BonusAgilityMultiplier);
                statsProcs.Agility += statsProcs.HighestStat * (1f + totalBAGIM) * (1f + statsProcs.BonusAgilityMultiplier);
                statsProcs.Agility += statsProcs.Paragon     * (1f + totalBAGIM) * (1f + statsProcs.BonusAgilityMultiplier);
                statsProcs.HighestStat = statsProcs.Paragon = 0f; // we've added them into agi so kill it
                statsProcs.Health  += (float)Math.Floor(statsProcs.Stamina * 10f);

                // Armor
                statsProcs.Armor = statsProcs.Armor * (1f + statsTotal.BaseArmorMultiplier + statsProcs.BaseArmorMultiplier);
                statsProcs.BonusArmor += statsProcs.Agility * 2f;
                statsProcs.BonusArmor = statsProcs.BonusArmor * (1f + statsTotal.BonusArmorMultiplier + statsProcs.BonusArmorMultiplier);
                statsProcs.Armor += statsProcs.BonusArmor;
                statsProcs.BonusArmor = 0; //it's been added to Armor so kill it

                // Attack Power
                statsProcs.BonusAttackPowerMultiplier *= (1f + statsProcs.BonusRangedAttackPowerMultiplier);
                statsProcs.BonusRangedAttackPowerMultiplier = 0; //it's been added to Armor so kill it
                float totalBAPMProcs    = (1f + totalBAPM) * (1f + statsProcs.BonusAttackPowerMultiplier) - 1f;
                float apFromAGIProcs    = (1f + totalBAPMProcs) * (statsProcs.Agility);
                float apFromSTRProcs    = (1f + totalBAPMProcs) * (statsProcs.Strength);
                float apBonusOtherProcs = (1f + totalBAPMProcs) * (statsProcs.AttackPower + statsProcs.RangedAttackPower);
                statsProcs.AttackPower = Math.Max(0f, apFromAGIProcs + apFromSTRProcs + apBonusOtherProcs);
                statsProcs.RangedAttackPower = statsProcs.AttackPower;
                statsTotal.AttackPower *= (1f + statsProcs.BonusAttackPowerMultiplier); // Make sure the originals get your AP% procs

                // Crit
                statsProcs.PhysicalCrit += StatConversion.GetCritFromAgility(statsProcs.Agility, character.Class);
                statsProcs.PhysicalCrit += StatConversion.GetCritFromRating(statsProcs.CritRating + statsProcs.RangedCritRating, character.Class);

                // Haste
                statsProcs.PhysicalHaste = (1f + statsProcs.PhysicalHaste)
                                         * (1f + statsProcs.RangedHaste)
                                         * (1f + StatConversion.GetPhysicalHasteFromRating(statsProcs.HasteRating, character.Class))
                                         - 1f;
                #endregion
                // Add it back into the fold
                statsTotal.Accumulate(statsProcs);
                #endregion

                statsTotal.Mana = (float)Math.Floor((1f + statsTotal.BonusManaMultiplier) * statsTotal.Mana);
                return statsTotal;
            } catch (Exception ex) {
                Rawr.Base.ErrorBox eb = new Rawr.Base.ErrorBox(
                    "Error Generating Character Stats",
                    ex.Message, "GetCharacterStats(...)", "No Additional Info", ex.StackTrace);
                return new Stats();
            }
        }

        private static readonly SpecialEffect FuriousHowl = new SpecialEffect(Trigger.Use, new Stats() { AttackPower = 320f, PetAttackPower = 320f, }, 20f, 40f);

        private static void CalculateTriggers(Character character, CharacterCalculationsHunter calculatedStats, Stats statsTotal,
            CalculationOptionsHunter calcOpts, BossOptions bossOpts,
            Dictionary<Trigger, float> triggerIntervals, Dictionary<Trigger, float> triggerChances)
        {
#if RAWR3 || SILVERLIGHT
            int levelDif = bossOpts.Level - character.Level;
#else
            int levelDif = calcOpts.TargetLevel - character.Level;
#endif
            float critMOD = StatConversion.NPC_LEVEL_CRIT_MOD[levelDif];
            HunterTalents talents = character.HunterTalents;
            float rangedWeaponSpeed = 0, rangedAmmoDPS = 0, rangedWeaponDamage = 0;
            float autoShotSpeed = 0;
            float autoShotsPerSecond = 0, specialShotsPerSecond = 0, totalShotsPerSecond = 0, shotsPerSecondWithoutHawk = 0;
            RotationTest rotationTest;
            GenRotation(character, statsTotal, calculatedStats, calcOpts, bossOpts, talents,
                out rangedWeaponSpeed, out rangedAmmoDPS, out rangedWeaponDamage, out autoShotSpeed,
                out autoShotsPerSecond, out specialShotsPerSecond, out totalShotsPerSecond, out shotsPerSecondWithoutHawk,
                out rotationTest);

            triggerIntervals.Add(Trigger.Use, 0f);
            triggerIntervals.Add(Trigger.MeleeHit, Math.Max(0f, calculatedStats.pet.PetCompInterval));
            triggerIntervals.Add(Trigger.RangedHit, 1f / totalShotsPerSecond);
            triggerIntervals.Add(Trigger.PhysicalHit, 1f / totalShotsPerSecond);
            triggerIntervals.Add(Trigger.MeleeCrit, Math.Max(0f, calculatedStats.pet.PetCompInterval));
            triggerIntervals.Add(Trigger.RangedCrit, 1f / totalShotsPerSecond);
            triggerIntervals.Add(Trigger.PhysicalCrit, 1f / totalShotsPerSecond);
            triggerIntervals.Add(Trigger.DoTTick, talents.PiercingShots > 0 ? 1f : 0f);
            triggerIntervals.Add(Trigger.DamageDone, Math.Max(0f, 1f / (totalShotsPerSecond + ((talents.PiercingShots > 0 ? 1f : 0f) > 0 ? 1f / (talents.PiercingShots > 0 ? 1f : 0f) : 0f))));
            triggerIntervals.Add(Trigger.DamageOrHealingDone, Math.Max(0f, 1f / (totalShotsPerSecond + ((talents.PiercingShots > 0 ? 1f : 0f) > 0 ? 1f / (talents.PiercingShots > 0 ? 1f : 0f) : 0f)))); // Need to add Self-Heals
            triggerIntervals.Add(Trigger.HunterAutoShotHit, 1f / autoShotsPerSecond);
            triggerIntervals.Add(Trigger.SteadyShotHit, calculatedStats.steadyShot.Cd);
            triggerIntervals.Add(Trigger.PetClawBiteSmackCrit, Math.Max(0f, calculatedStats.pet.PetClawBiteSmackInterval));
            triggerIntervals.Add(Trigger.SerpentWyvernStingsDoDamage, (calculatedStats.serpentSting.Freq > 0 || calculatedStats.serpentSting.is_refreshed ? 3f : 0f));

            float ChanceToMiss = Math.Max(0f, StatConversion.WHITE_MISS_CHANCE_CAP[levelDif] - statsTotal.PhysicalHit);
            float ChanceToSpellMiss = Math.Max(0f, StatConversion.GetSpellMiss(levelDif, false) - statsTotal.SpellHit);

            triggerChances.Add(Trigger.Use, 1f);
            triggerChances.Add(Trigger.MeleeHit, calculatedStats.pet.WhAtkTable.AnyLand);
            triggerChances.Add(Trigger.RangedHit, (1f - ChanceToMiss));
            triggerChances.Add(Trigger.PhysicalHit, (1f - ChanceToMiss));
            triggerChances.Add(Trigger.MeleeCrit, Math.Min(1f + critMOD, Math.Max(0f, calculatedStats.pet.WhAtkTable.Crit)));
            triggerChances.Add(Trigger.RangedCrit, Math.Min(1f + critMOD, Math.Max(0f, statsTotal.PhysicalCrit)));
            triggerChances.Add(Trigger.PhysicalCrit, Math.Min(1f + critMOD, Math.Max(0f, statsTotal.PhysicalCrit)));
            triggerChances.Add(Trigger.DoTTick, 1f);
            triggerChances.Add(Trigger.DamageDone, 1f);
            triggerChances.Add(Trigger.DamageOrHealingDone, 1f);
            triggerChances.Add(Trigger.HunterAutoShotHit, (1f - ChanceToMiss));
            triggerChances.Add(Trigger.SteadyShotHit, (1f - ChanceToMiss));
            triggerChances.Add(Trigger.PetClawBiteSmackCrit, Math.Min(1f + critMOD, Math.Max(0f, calculatedStats.pet.WhAtkTable.Crit)));
            triggerChances.Add(Trigger.SerpentWyvernStingsDoDamage, 1f);
        }

        private Stats GetSpecialEffectsStats(Character Char,
            Dictionary<Trigger, float> triggerIntervals, Dictionary<Trigger, float> triggerChances,
            Stats statsTotal, Stats statsToProcess)
        {
            CalculationOptionsHunter calcOpts = Char.CalculationOptions as CalculationOptionsHunter;
            BossOptions bossOpts = Char.BossOptions;
            ItemInstance RangeWeap = Char.MainHand;
            float speed = (RangeWeap != null ? RangeWeap.Speed : 2.4f);
            HunterTalents talents = Char.HunterTalents;
            Stats statsProcs = new Stats();
#if RAWR3 || SILVERLIGHT
            float fightDuration_M = bossOpts.BerserkTimer;
#else
            float fightDuration_M = calcOpts.Duration;
#endif
            Stats _stats;
            //
            foreach (SpecialEffect effect in (statsToProcess != null ? statsToProcess.SpecialEffects() : statsTotal.SpecialEffects())) {
                float fightDuration = (effect.Stats.DeathbringerProc == 1 ? 0f : fightDuration_M);
                //float oldArp = float.Parse(effect.Stats.ArmorPenetrationRating.ToString());
                float arpToHardCap = StatConversion.RATING_PER_ARMORPENETRATION;
                if (effect.Stats.ArmorPenetrationRating > 0) {
                    float arpenBuffs = 0.0f;
                    float currentArp = arpenBuffs + StatConversion.GetArmorPenetrationFromRating(statsTotal.ArmorPenetrationRating
                        + (statsToProcess != null ? statsToProcess.ArmorPenetrationRating : 0f));
                    arpToHardCap *= (1f - currentArp);
                }
                if (triggerIntervals.ContainsKey(effect.Trigger) && (triggerIntervals[effect.Trigger] > 0f || effect.Trigger == Trigger.Use))
                {
                    float weight = 1f;
                    switch (effect.Trigger) {
                        case Trigger.Use:
                            _stats = new Stats();
                            if (effect.Stats._rawSpecialEffectDataSize == 1 && statsToProcess == null) {
                                float uptime = effect.GetAverageUptime(0f, 1f, speed, fightDuration);
                                _stats.AddSpecialEffect(effect.Stats._rawSpecialEffectData[0]);
                                Stats _stats2 = GetSpecialEffectsStats(Char,
                                    triggerIntervals, triggerChances,
                                    statsTotal, _stats);
                                _stats = _stats2 * uptime;
                            } else {
                                if (effect.Stats.ArmorPenetrationRating > 0 && arpToHardCap < effect.Stats.ArmorPenetrationRating) {
                                    float uptime = effect.GetAverageUptime(0f, 1f, speed, fightDuration);
                                    weight = uptime;
                                    _stats.ArmorPenetrationRating = arpToHardCap;
                                } else {
                                    _stats = effect.GetAverageStats(0f, 1f, speed, fightDuration);
                                }
                            }
                            statsProcs.Accumulate(_stats, weight);
                            _stats = null;
                            break;
                        case Trigger.RangedHit:
                        case Trigger.PhysicalHit:
                            _stats = new Stats();
                            weight = 1.0f;
                            if (effect.Stats.DeathbringerProc > 0) {
                                _stats = effect.GetAverageStats(triggerIntervals[effect.Trigger], triggerChances[effect.Trigger], speed, fightDuration);
                                _stats.Agility = _stats.DeathbringerProc;
                                _stats.CritRating = _stats.DeathbringerProc;
                                _stats.AttackPower = _stats.DeathbringerProc * 2f;
                                _stats.DeathbringerProc = 0f;
                                weight = 1f / 3f;
                            } else {
                                if (effect.Stats.ArmorPenetrationRating > 0 && arpToHardCap < effect.Stats.ArmorPenetrationRating) {
                                    float uptime = effect.GetAverageUptime(triggerIntervals[effect.Trigger], triggerChances[effect.Trigger], speed, fightDuration);
                                    weight = uptime;
                                    _stats.ArmorPenetrationRating = arpToHardCap;
                                } else {
                                    _stats = effect.GetAverageStats(triggerIntervals[effect.Trigger], triggerChances[effect.Trigger], speed, fightDuration);
                                }
                            }
                            statsProcs.Accumulate(_stats, weight);
                            _stats = null;
                            break;
                        case Trigger.MeleeHit: // Pets Only
                        case Trigger.MeleeCrit: // Pets Only
                        case Trigger.RangedCrit:
                        case Trigger.PhysicalCrit:
                        case Trigger.DoTTick:
                        case Trigger.DamageDone: // physical and dots
                        case Trigger.DamageOrHealingDone: // physical and dots
                        case Trigger.HunterAutoShotHit:
                        case Trigger.SteadyShotHit:
                        case Trigger.PetClawBiteSmackCrit:
                            _stats = new Stats();
                            weight = 1.0f;
                            if (effect.Stats.ArmorPenetrationRating > 0 && arpToHardCap < effect.Stats.ArmorPenetrationRating) {
                                float uptime = effect.GetAverageUptime(triggerIntervals[effect.Trigger], triggerChances[effect.Trigger], speed, fightDuration);
                                weight = uptime;
                                _stats.ArmorPenetrationRating = arpToHardCap;
                            } else {
                                _stats = effect.GetAverageStats(triggerIntervals[effect.Trigger], triggerChances[effect.Trigger], speed, fightDuration);
                            }
                            statsProcs.Accumulate(_stats, weight);
                            break;
                        case Trigger.SerpentWyvernStingsDoDamage:
                            _stats = new Stats();
                            weight = 1.0f;
                            if (effect.Stats.ArmorPenetrationRating > 0 && arpToHardCap < effect.Stats.ArmorPenetrationRating) {
                                float uptime = effect.GetAverageUptime(triggerIntervals[effect.Trigger], triggerChances[effect.Trigger], speed, fightDuration);
                                weight = uptime;
                                _stats.ArmorPenetrationRating = arpToHardCap;
                            } else {
                                _stats = effect.GetAverageStats(triggerIntervals[effect.Trigger], triggerChances[effect.Trigger], speed, fightDuration);
                            }
                            statsProcs.Accumulate(_stats, weight);
                            break;
                    }
                }
                //effect.Stats.ArmorPenetrationRating = oldArp;
            }

            return statsProcs;
        }

        #endregion //overrides

        #region Private Functions
        private ShotData getShotByIndex(int index, CharacterCalculationsHunter calculatedStats)
        {
            if (index ==  1) return calculatedStats.aimedShot;
            if (index ==  2) return calculatedStats.arcaneShot;
            if (index ==  3) return calculatedStats.multiShot;
            if (index ==  4) return calculatedStats.serpentSting;
            if (index ==  5) return calculatedStats.scorpidSting;
            if (index ==  6) return calculatedStats.viperSting;
            if (index ==  7) return calculatedStats.silencingShot;
            if (index ==  8) return calculatedStats.steadyShot;
            if (index ==  9) return calculatedStats.killShot;
            if (index == 10) return calculatedStats.explosiveShot;
            if (index == 11) return calculatedStats.blackArrow;
            if (index == 12) return calculatedStats.immolationTrap;
            if (index == 13) return calculatedStats.explosiveTrap;
            if (index == 14) return calculatedStats.freezingTrap;
            if (index == 15) return calculatedStats.frostTrap;
            if (index == 16) return calculatedStats.volley;
            if (index == 17) return calculatedStats.chimeraShot;
            if (index == 18) return calculatedStats.rapidFire;
            if (index == 19) return calculatedStats.readiness;
            if (index == 20) return calculatedStats.bestialWrath;
            return null;
        }
        public static float CalcEffectiveDamage(float damageNormal, float missChance, float critChance, float critAdjust, float damageAdjust) {
            /* OLD CODE
             * 
            float damageCrit  =  damageNormal * (1f + critAdjust);
            float damageTotal = (damageNormal * (1f - critChance)
                                 + (damageCrit * critChance));
            float damageReal  = damageTotal * damageAdjust * (1f - missChance);

            return damageReal;*/

            float dmg = damageNormal; // MhDamage;                  // Base Damage
            //dmg *= combatFactors.DamageBonus;      // Global Damage Bonuses
            //dmg *= combatFactors.DamageReduction;  // Global Damage Penalties
            dmg *= damageAdjust;

            // Work the Attack Table
            float dmgDrop = (1f
                - missChance // MHAtkTable.Miss   // no damage when being missed
                - critChance // MHAtkTable.Crit   // crits   handled below
                );

            float dmgCrit = dmg
                          * critChance // MHAtkTable.Crit
                          * (1f
                             + critAdjust//combatFactors.BonusWhiteCritDmg
                             );//Bonus Damage when critting

            dmg *= dmgDrop;

            dmg += dmgCrit;

            return dmg;
        }
        public float GetArmorDamageReduction(Character Char, Stats StatS, CalculationOptionsHunter CalcOpts, BossOptions BossOpts) {
            float armorReduction;
            float arpenBuffs = 0.0f;

#if RAWR3 || SILVERLIGHT
            if (CalcOpts == null) {
#else
            if (BossOpts == null) {
#endif
                armorReduction = Math.Max(0f, 1f - StatConversion.GetArmorDamageReduction(Char.Level, (int)StatConversion.NPC_ARMOR[3], StatS.ArmorPenetration, arpenBuffs, StatS.ArmorPenetrationRating)); // default is vs raid boss
            } else {
#if RAWR3 || SILVERLIGHT
                armorReduction = Math.Max(0f, 1f - StatConversion.GetArmorDamageReduction(Char.Level, BossOpts.Armor, StatS.ArmorPenetration, arpenBuffs, StatS.ArmorPenetrationRating));
#else
                armorReduction = Math.Max(0f, 1f - StatConversion.GetArmorDamageReduction(Char.Level, CalcOpts.TargetArmor, StatS.ArmorPenetration, arpenBuffs, StatS.ArmorPenetrationRating));
#endif
            }

            return armorReduction;
        }
        #endregion
    }
}
