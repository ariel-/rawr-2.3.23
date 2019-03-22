/**********
 * Owner: Jothay
 **********/
using System;

namespace Rawr.DPSWarr.Skills
{
    #region Instants
    public class MortalStrike : Ability
    {
        /// <summary>
        /// A vicious strike that deals weapon damage plus 380 and wounds the target, reducing
        /// the effectiveness of any healing by 50% for 10 sec.
        /// </summary>
        /// <TalentsAffecting>
        /// Mortal Strike (Requires Talent),
        /// Improved Mortal Strike [+(10-ROUNDUP(10/3*Pts))% damage, -(1/3*Pts) sec cooldown]
        /// </TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Mortal Strike [+10% damage]</GlyphsAffecting>
        public MortalStrike(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsDPSWarr co, BossOptions bo)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co; BossOpts = bo;
            //
            Name = "Mortal Strike";
            Description = "A vicious strike that deals weapon damage plus 380 and wounds the target, reducing the effectiveness of any healing by 50% for 10 sec.";
            AbilIterater = (int)CalculationOptionsDPSWarr.Maintenances.MortalStrike_;
            ReqTalent = true;
            Talent2ChksValue = Talents.MortalStrike;
            ReqMeleeWeap = true;
            ReqMeleeRange = true;
            //Targets += StatS.BonusTargets;
            Cd = 6f - (Talents.ImprovedMortalStrike / 3f); // In Seconds
            RageCost = 30f - (Talents.FocusedRage * 1f);
            StanceOkFury = StanceOkArms = StanceOkDef = true;
            DamageBase = combatFactors.NormalizedMhWeaponDmg + 380f;
            DamageBonus = (1f + Talents.ImprovedMortalStrike / 3f * 0.1f)
                        * (1f + (Talents.GlyphOfMortalStrike ? 0.1f : 0f));
            BonusCritChance = StatS.BonusWarrior_T8_4P_MSBTCritIncrease;
            //
            Initialize();
        }
    }
    public class Suddendeath : Ability
    {
        // Constructors
        /// <summary>
        /// Your melee hits have a (3*Pts)% chance of allowing the use of Execute regardless of
        /// the target's Health state. This Execute only uses up to 30 total rage. In addition,
        /// you keep at least (3/7/10) rage after using Execute.
        /// </summary>
        /// <TalentsAffecting>Sudden Death (Requires Talent) [(3*Pts)% chance to proc and (3/7/10) rage kept after],
        /// Improved Execute [-(2.5*Pts) rage cost]</TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Execute [Execute acts as if it had 10 additional rage]</GlyphsAffecting>
        /// <SetsAffecting>
        /// T10-4P [You have a 20% chance for your Sudden Death talent to grant 2 charges of
        /// its effect instead of 1, reduce the global cooldown on Execute or Slam by 0.5 sec,
        /// and for the duration of the effect to be increased by 100%.]
        /// </SetsAffecting>
        public Suddendeath(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsDPSWarr co, BossOptions bo, Ability ex)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co; BossOpts = bo;
            //
            Name = "Sudden Death";
            Description = "Your melee hits have a (3*Pts)% chance of allowing the use of Execute regardless of the target's Health state. This Execute only uses up to 30 total rage. In addition, you keep at least (3/7/10) rage after using Execute.";
            AbilIterater = (int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.SuddenDeath_;
            Exec = ex as Execute;
            RageCost = Exec.RageCost;
            ReqTalent = true;
            Talent2ChksValue = Talents.SuddenDeath;
            ReqMeleeWeap = Exec.GetReqMeleeWeap();
            ReqMeleeRange = Exec.GetReqMeleeRange();
            //Targets += StatS.BonusTargets;
            Cd = Exec.Cd;
            if (s.BonusWarrior_T10_4P_BSSDProcChange > 0f)
            {
                // 20% proc rate, so 20% of the time we get 2 SDs over 2*(GCD-0.5)
                float procRate = 0.2f;
                float numSDActivates = 1f + procRate;
                float activatesHasted = procRate * 2f; // assuming both activates have the 0.5sec GCD redux
                float percActivatesHasted = activatesHasted / numSDActivates;
                float percActivatesUnhasted = 1f - percActivatesHasted;
                GCDTime = ((Cd - 0.5f) * percActivatesHasted) + percActivatesUnhasted * Cd;
            }
            StanceOkArms = true;
            UseReact = true;
            //
            Initialize();
        }
        // Variables
        public Execute Exec;
        public float FreeRage { get { return Exec.FreeRage; } set { Exec.FreeRage = value; } }
        public float UsedExtraRage { get { return Exec.UsedExtraRage; } set { Exec.UsedExtraRage = value; } }
        private static readonly SpecialEffect[/*Talents.SuddenDeath*/][/*StatS.BonusWarrior_T10_4P_BSSDProcChange>0?1:0*/] _buff = {
            new SpecialEffect[] { new SpecialEffect(Trigger.MeleeHit, new Stats() { }, (false ? 20f : 10f), 0f, 0 * 0.03f), new SpecialEffect(Trigger.MeleeHit, new Stats() { }, (true ? 20f : 10f), 0f, 0 * 0.03f), },
            new SpecialEffect[] { new SpecialEffect(Trigger.MeleeHit, new Stats() { }, (false ? 20f : 10f), 0f, 1 * 0.03f), new SpecialEffect(Trigger.MeleeHit, new Stats() { }, (true ? 20f : 10f), 0f, 1 * 0.03f), },
            new SpecialEffect[] { new SpecialEffect(Trigger.MeleeHit, new Stats() { }, (false ? 20f : 10f), 0f, 2 * 0.03f), new SpecialEffect(Trigger.MeleeHit, new Stats() { }, (true ? 20f : 10f), 0f, 2 * 0.03f), },
            new SpecialEffect[] { new SpecialEffect(Trigger.MeleeHit, new Stats() { }, (false ? 20f : 10f), 0f, 3 * 0.03f), new SpecialEffect(Trigger.MeleeHit, new Stats() { }, (true ? 20f : 10f), 0f, 3 * 0.03f), },
        };
        protected SpecialEffect Buff { get { return _buff[Talents.SuddenDeath][StatS.BonusWarrior_T10_4P_BSSDProcChange > 0 ? 1 : 0]; } }
        // Functions
        public float GetActivates(float landedatksoverdur) {
            if (AbilIterater != -1 && !CalcOpts.Maintenance[AbilIterater]) { return 0f; }
            float acts = Buff.GetAverageProcsPerSecond(landedatksoverdur / FightDuration, 1f/*MHAtkTable.AnyLand*/, combatFactors._c_mhItemSpeed, FightDuration);
            acts *= FightDuration;
            if (StatS.BonusWarrior_T10_4P_BSSDProcChange > 0) { acts *= 1.20f; }

            return acts * (1f - Whiteattacks.RageSlip(FightDuration / acts, RageCost + UsedExtraRage));
        }
        public override float DamageOverride { get { return Exec.DamageOverride; } }
        public override float GetRageUseOverDur(float acts)
        {
            if (!Validated) { return 0f; }
            return acts * (RageCost + UsedExtraRage - SDRageReduc[Talents.SuddenDeath]);
        }
        private float[] _SDRageReduc = { 0, 3, 7, 10 };
        private float[] SDRageReduc { get{ return _SDRageReduc; } }
    }
    public class OverPower : Ability
    {
        // Constructors
        /// <summary>
        /// Instantly overpower the enemy, causing weapon damage plus 125. Only usable after the target dodges.
        /// The Overpower cannot be blocked, dodged or parried.
        /// </summary>
        /// <TalentsAffecting>Improved Overpower [+(25*Pts)% Crit Chance],
        /// Unrelenting Assault [-(2*Pts) sec cooldown, +(10*Pts)% Damage.]</TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Overpower [Can proc when parried]</GlyphsAffecting>
        public OverPower(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsDPSWarr co, BossOptions bo, Ability ss)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co; BossOpts = bo;
            //
            Name = "Overpower";
            Description = "Instantly overpower the enemy, causing weapon damage plus 125. Only usable after the target dodges. The Overpower cannot be blocked, dodged or parried.";
            AbilIterater = (int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.Overpower_;
            SS = ss;
            ReqMeleeWeap = true;
            ReqMeleeRange = true;
            CanBeDodged = false;
            CanBeParried = false;
            CanBeBlocked = false;
            Cd = 5f - (2f * Talents.UnrelentingAssault); // In Seconds
            GCDTime = Math.Min(1.5f, Cd);
            RageCost = 5f - (Talents.FocusedRage * 1f);
            //Targets += StatS.BonusTargets;
            StanceOkArms = true;
            DamageBase = combatFactors.NormalizedMhWeaponDmg;
            DamageBonus = 1f + (0.1f * Talents.UnrelentingAssault);
            BonusCritChance = 0.25f * Talents.ImprovedOverpower;
            UseReact = true; // can't plan for this
            //
            Initialize();
        }
        private Ability SS;
        public float GetActivates(float YellowAttacksThatDodgeOverDur, float YellowAttacksThatParryOverDur, float ssActs)
        {
            if (AbilIterater != -1 && !CalcOpts.Maintenance[AbilIterater]) { return 0f; }

            float acts = 0f;
            float LatentGCD = (1.5f + CalcOpts.Latency + (UseReact ? CalcOpts.React / 1000f : CalcOpts.AllowedReact));

            float dodge = Whiteattacks.MHAtkTable.Dodge;
            float parry = (Talents.GlyphOfOverpower ? Whiteattacks.MHAtkTable.Parry : 0f);

            // Chance to activate: Dodges + (if glyphed) Parries
            if (dodge + parry > 0f)
            {
                float WhtHitsOverDur = FightDuration / Whiteattacks.MhEffectiveSpeed
              + (combatFactors.useOH ? FightDuration / Whiteattacks.OhEffectiveSpeed : 0f)
                                       + ssActs;

                float dodgesoverDur = 0f
                    + WhtHitsOverDur * (dodge + parry)
                    + (dodge > 0 ? YellowAttacksThatDodgeOverDur : 0)
                    + (parry > 0 ? YellowAttacksThatParryOverDur : 0);

                //acts += Math.Max(0f, dodgesoverDur * (1f - Whiteattacks.AvoidanceStreak));
                acts += Math.Max(0f, dodgesoverDur * (1f - Whiteattacks.RageSlip(FightDuration / dodgesoverDur, RageCost)));
            }

            return acts;
        }
    }
    public class TasteForBlood : Ability
    {
        // Constructors
        /// <summary>
        /// Instantly overpower the enemy, causing weapon damage. Only usable after the target takes Rend Damage.
        /// The Overpower cannot be blocked, dodged or parried.
        /// </summary>
        /// <TalentsAffecting>Improved Overpower [+(25*Pts)% Crit Chance],
        /// Unrelenting Assault [-(2*Pts) sec cooldown, +(10*Pts)% Damage.]</TalentsAffecting>
        /// <GlyphsAffecting></GlyphsAffecting>
        public TasteForBlood(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsDPSWarr co, BossOptions bo)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co; BossOpts = bo;
            //
            Name = "Taste for Blood";
            Description = "Instantly overpower the enemy, causing weapon damage plus 125. Only usable after the target takes Rend Damage. The Overpower cannot be blocked, dodged or parried.";
            AbilIterater = (int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.TasteForBlood_;
            ReqTalent = true;
            Talent2ChksValue = Talents.TasteForBlood;
            ReqMeleeWeap = true;
            ReqMeleeRange = true;
            CanBeDodged = false;
            CanBeParried = false;
            CanBeBlocked = false;
            GCDTime = Math.Min(1.5f, 5f - (2f * Talents.UnrelentingAssault));
            Cd = 6f; // In Seconds
            RageCost = 5f - (Talents.FocusedRage * 1f);
            StanceOkArms = true;
            DamageBase = combatFactors.NormalizedMhWeaponDmg;
            DamageBonus = 1f + (0.1f * Talents.UnrelentingAssault);
            BonusCritChance = 0.25f * Talents.ImprovedOverpower;
            //UseReact = true; // you can plan for it ahead of time, unlike SD and normal OP
            //
            Initialize();
        }
        protected override float ActivatesOverride
        {
            get
            {
                float acts = 0f;

                // Chance to activate Requires Rend
                if (CalcOpts.Maintenance[(int)CalculationOptionsDPSWarr.Maintenances.Rend_])
                {
                    acts = base.ActivatesOverride * (1f / 3f * Talents.TasteForBlood);
                }

                return acts;
            }
        }
    }
    public class Bladestorm : Ability
    {
        // Constructors
        /// <summary>
        /// Instantly Whirlwind up to 4 nearby targets and for the next 6 sec you will
        /// perform a whirlwind attack every 1 sec. While under the effects of Bladestorm, you can move but cannot
        /// perform any other abilities but you do not feel pity or remorse or fear and you cannot be stopped
        /// unless killed.
        /// </summary>
        /// <TalentsAffecting>Bladestorm [Requires Talent]</TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Bladestorm [-15 sec Cd]</GlyphsAffecting>
        public Bladestorm(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsDPSWarr co, BossOptions bo, Ability ww)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co; BossOpts = bo;
            //
            WW = ww;
            Name = "Bladestorm";
            Description = "Instantly Whirlwind up to 4 nearby targets and for the next 6 sec you will perform a whirlwind attack every 1 sec. While under the effects of Bladestorm, you can move but cannot perform any other abilities but you do not feel pity or remorse or fear and you cannot be stopped unless killed.";
            AbilIterater = (int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.Bladestorm_;
            ReqTalent = true;
            Talent2ChksValue = Talents.Bladestorm;
            ReqMeleeWeap = true;
            ReqMeleeRange = true;
            MaxRange = WW.MaxRange; // In Yards
            Targets = WW.Targets; // Handled in WW
            DamageBase = WW.DamageBase;
            Cd = 90f - (Talents.GlyphOfBladestorm ? 15f : 0f); // In Seconds
            RageCost = 25f - (Talents.FocusedRage * 1f);
            CastTime = 6f; // In Seconds // Channeled
            GCDTime = CastTime;
            StanceOkFury = StanceOkArms = StanceOkDef = true;
            SwingsOffHand = true;
            SwingsPerActivate = 7f;
            //
            Initialize();
        }
        // Variables
        public Ability WW;
        // Functions
        public override float DamageOnUseOverride
        {
            get
            {
                if (!Validated) { return 0f; }
                float Damage = WW.DamageOnUseOverride; // WW.DamageOnUseOverride;
                return Damage * 7f; // it WW's 7 times
            }
        }
    }
    public class Swordspec : Ability
    {
        /// <summary>
        /// Gives a (1*Pts)% chance to get an extra attack on the same target after hitting
        /// your target with your Sword. This effect cannot occur more than once every 6 seconds.
        /// </summary>
        /// <TalentsAffecting>Sword Specialization (Requires Talent)</TalentsAffecting>
        /// <GlyphsAffecting></GlyphsAffecting>
        public Swordspec(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsDPSWarr co, BossOptions bo)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co; BossOpts = bo;
            //
            Name = "Sword Specialization";
            Description = "Gives a (1*Pts)% chance to get an extra attack on the same target after hitting your target with your Sword. This effect cannot occur more than once every 6 seconds.";
            ReqTalent = true;
            Talent2ChksValue = Talents.SwordSpecialization;
            //Targets += StatS.BonusTargets;
            Cd = 6f; // In Seconds
            StanceOkFury = StanceOkArms = StanceOkDef = true;
            DamageBase = combatFactors.AvgMhWeaponDmgUnhasted;
            RageCost = -Whiteattacks.MHSwingRage;
            UsesGCD = false;
            //
            Initialize();
            MHAtkTable = Whiteattacks.MHAtkTable;
        }
        public override bool Validated
        {
            get
            {
                return base.Validated &&
                      (combatFactors._c_mhItemType == ItemType.TwoHandSword ||
                       combatFactors._c_mhItemType == ItemType.OneHandSword);
            }
        }

        private static readonly SpecialEffect[] _SE_SwordSpec = {
            null,
            new SpecialEffect(Trigger.MeleeHit, null, 0f, 6, 1 * 0.02f),
            new SpecialEffect(Trigger.MeleeHit, null, 0f, 6, 2 * 0.02f),
            new SpecialEffect(Trigger.MeleeHit, null, 0f, 6, 3 * 0.02f),
            new SpecialEffect(Trigger.MeleeHit, null, 0f, 6, 4 * 0.02f),
            new SpecialEffect(Trigger.MeleeHit, null, 0f, 6, 5 * 0.02f),
        };

        public float GetActivates(float YellowsThatLandOverDur, float heroic, float cleave)
        {
            if (combatFactors._c_mhItemType != ItemType.TwoHandSword && combatFactors._c_mhItemType != ItemType.OneHandSword) { return 0.0f; }
            // This attack doesnt consume GCDs and doesn't affect the swing timer
            Whiteattacks.HSOverridesOverDur = heroic;
            Whiteattacks.CLOverridesOverDur = cleave;
            float rawActs = (YellowsThatLandOverDur + Whiteattacks.LandedAtksOverDur) / FightDuration;
            float effectActs = _SE_SwordSpec[Talents.SwordSpecialization].GetAverageProcsPerSecond(rawActs, 1f, combatFactors._c_mhItemSpeed, FightDuration);
            effectActs *= FightDuration;
            return effectActs;
        }
        public override string GenTooltip(float acts, float ttldpsperc)
        {
            float misses = GetXActs(AttackTableSelector.Missed , acts), missesPerc = (acts == 0f ? 0f : misses / acts);
            float dodges = GetXActs(AttackTableSelector.Dodged , acts), dodgesPerc = (acts == 0f ? 0f : dodges / acts);
            float parrys = GetXActs(AttackTableSelector.Parried, acts), parrysPerc = (acts == 0f ? 0f : parrys / acts);
            float blocks = GetXActs(AttackTableSelector.Blocked, acts), blocksPerc = (acts == 0f ? 0f : blocks / acts);
            float glance = GetXActs(AttackTableSelector.Glance , acts), glancePerc = (acts == 0f ? 0f : glance / acts);
            float crits  = GetXActs(AttackTableSelector.Crit   , acts), critsPerc  = (acts == 0f ? 0f : crits  / acts);
            float hits   = GetXActs(AttackTableSelector.Hit    , acts), hitsPerc   = (acts == 0f ? 0f : hits   / acts);

            bool showmisss = misses > 0f;
            bool showdodge = CanBeDodged && dodges > 0f;
            bool showparry = CanBeParried && parrys > 0f;
            bool showblock = CanBeBlocked && blocks > 0f;
            bool showglance= true         && glance > 0f;
            bool showcrits = CanCrit && crits > 0f;

            string tooltip = "*" + Name +
                Environment.NewLine + "Cast Time: " + (CastTime != -1 ? CastTime.ToString() : "Instant")
                                    + ", CD: " + (Cd != -1 ? Cd.ToString() : "None")
                                    + ", Rage Generated: " + (RageCost != -1 ? (-1f * RageCost).ToString() : "None") +
            Environment.NewLine + Environment.NewLine + acts.ToString("000.00") + " Activates over Attack Table:" +
            (showmisss ? Environment.NewLine + "- " + misses.ToString("000.00") + " : " + missesPerc.ToString("00.00%") + " : Missed "  : "") +
            (showdodge ? Environment.NewLine + "- " + dodges.ToString("000.00") + " : " + dodgesPerc.ToString("00.00%") + " : Dodged "  : "") +
            (showparry ? Environment.NewLine + "- " + parrys.ToString("000.00") + " : " + parrysPerc.ToString("00.00%") + " : Parried " : "") +
            (showblock ? Environment.NewLine + "- " + blocks.ToString("000.00") + " : " + blocksPerc.ToString("00.00%") + " : Blocked " : "") +
            (showglance? Environment.NewLine + "- " + glance.ToString("000.00") + " : " + glancePerc.ToString("00.00%") + " : Glanced " : "") +
            (showcrits ? Environment.NewLine + "- " + crits.ToString( "000.00") + " : " + critsPerc.ToString( "00.00%") + " : Crit "    : "") +
                         Environment.NewLine + "- " + hits.ToString(  "000.00") + " : " + hitsPerc.ToString(  "00.00%") + " : Hit "           +
                Environment.NewLine +
                //Environment.NewLine + "Damage per Blocked|Hit|Crit: x|x|x" +
                Environment.NewLine + "Targets Hit: " + (Targets != -1 ? AvgTargets.ToString("0.00") : "None") +
                Environment.NewLine + "DPS: " + (GetDPS(acts) > 0 ? GetDPS(acts).ToString("0.00") : "None") +
                Environment.NewLine + "Percentage of Total DPS: " + (ttldpsperc > 0 ? ttldpsperc.ToString("00.00%") : "None");

            return tooltip;
        }
    }
    public class Execute : Ability
    {
        // Constructors
        /// <summary>
        /// Attempt to finish off a wounded foe, causing (1456+AP*0.2) damage and converting each
        /// extra point of rage into 38 additional damage. Only usable on enemies that have less
        /// than 20% health.
        /// </summary>
        /// <TalentsAffecting>Improved Execute [Reduces the rage cost of your Execute ability by (2.5/5).]</TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Execute [Your Execute ability acts as if it has 10 additional rage.]</GlyphsAffecting>
        public Execute(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsDPSWarr co, BossOptions bo)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co; BossOpts = bo;
            //
            Name = "Execute";
            Description = "Attempt to finish off a wounded foe, causing (1456+AP*0.2) damage and converting each extra point of rage into 38 additional damage. Only usable on enemies that have less than 20% health.";
            AbilIterater = (int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.ExecuteSpam_;
            ReqMeleeWeap = true;
            ReqMeleeRange = true;
            Cd = 1.5f;
            RageCost = 15f - (Talents.ImprovedExecute * 2.5f) - (Talents.FocusedRage * 1f);
            FreeRage = 0f;
            StanceOkFury = StanceOkArms = true;
            PercTimeUnder20 = 0.17f;
            //
            Initialize();
        }
        public bool GetReqMeleeWeap() { return this.ReqMeleeWeap; }
        public bool GetReqMeleeRange() { return this.ReqMeleeRange; }
        private float FREERAGE;
        public float FreeRage
        {
            get { return FREERAGE; }
            set
            {
                FREERAGE = value;
                UsedExtraRage = Math.Max(0f, Math.Min(30f, FreeRage));
            }
        }
        public float UsedExtraRage;
        public float PercTimeUnder20;
        protected override float ActivatesOverride { get { return base.ActivatesOverride * PercTimeUnder20; } }
        public override float DamageOverride
        {
            get
            {
                //UsedExtraRage = Math.Max(0f, Math.Min(30f, FreeRage));
                float executeRage = UsedExtraRage + (Talents.GlyphOfExecution ? 10.00f : 0.00f);

                float Damage = 1456f + StatS.AttackPower * 0.2f + executeRage * 38f;

                return Damage * AvgTargets;
            }
        }
        public override float GetRageUseOverDur(float acts)
        {
            if (!Validated) { return 0f; }
            return acts * (RageCost + UsedExtraRage);
        }
    }
    public class Slam : Ability
    {
        // Constructors
        /// <summary>Slams the opponent, causing weapon damage plus 250.</summary>
        /// <TalentsAffecting>Improved Slam [Reduces cast time of your Slam ability by (0.5/1) sec.]</TalentsAffecting>
        /// <SetsAffecting>T7 Deadnaught Battlegear 2 Pc [+10% Damage]</SetsAffecting>
        public Slam(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsDPSWarr co, BossOptions bo)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co; BossOpts = bo;
            //
            Name = "Slam";
            Description = "Slams the opponent, causing weapon damage plus 250.";
            AbilIterater = (int)Rawr.DPSWarr.CalculationOptionsDPSWarr.Maintenances.Slam_;
            ReqMeleeWeap = true;
            ReqMeleeRange = true;
            Cd = 1.5f;
            RageCost = 15f - (Talents.FocusedRage * 1f);
            CastTime = (1.5f - (Talents.ImprovedSlam * 0.5f)); // In Seconds
            StanceOkArms = StanceOkDef = true;
            DamageBase = combatFactors.AvgMhWeaponDmgUnhasted + 250f;
            DamageBonus = (1f + Talents.UnendingFury * 0.02f) * (1f + StatS.BonusWarrior_T7_2P_SlamDamage);
            BonusCritChance = StatS.BonusWarrior_T9_4P_SLHSCritIncrease;
            //
            Initialize();
        }
    }
    #endregion

    public class Rend : DoT
    {
        /// <summary>
        /// Wounds the target causing them to bleed for 380 damage plus an additional
        /// (0.2*5*MWB+mwb/2+AP/14*MWS) (based on weapon damage) over 15 sec. If used while your
        /// target is above 75% health, Rend does 35% more damage.
        /// </summary>
        /// <TalentsAffecting>Improved Rend [+(10*Pts)% Bleed Damage], Trauma [+(15*Pts)% Bleed Damage]</TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Rending [+2 damage ticks]</GlyphsAffecting>
        public Rend(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsDPSWarr co, BossOptions bo)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co; BossOpts = bo;
            //
            Name = "Rend";
            Description = "Wounds the target causing them to bleed for 380 damage plus an additional (0.2*5*MWB+mwb/2+AP/14*MWS) (based on weapon damage) over 15 sec. If used while your target is above 75% health, Rend does 35% more damage.";
            AbilIterater = (int)CalculationOptionsDPSWarr.Maintenances.Rend_;
            ReqMeleeWeap = true;
            ReqMeleeRange = true;
            CanCrit = false;
            Duration = 15f + (Talents.GlyphOfRending ? 6f : 0f); // In Seconds
            Cd = Duration + 3f;
            TimeBtwnTicks = 3f; // In Seconds
            RageCost = 10f - (Talents.FocusedRage * 1f);
            StanceOkArms = StanceOkDef = true;
            DamageBase = 380f;
            DamageBonus = (1f + 0.10f * Talents.ImprovedRend);// *(1f + 0.15f * Talents.Trauma);
            //
            Initialize();
        }
        protected float addMisses;
        protected float addDodges;
        protected float addParrys;
        protected override float ActivatesOverride
        {
            get
            {
                // Since Rend has to be up in order to Taste for Blood
                // this override has the intention of taking the baseline
                // activates and forcing the char to use Rend again to ensure it's up
                // in the event that the attemtped activate didn't land (it Missed, was Dodged or Parried)
                // We're only doing the additional check once so it will at most Rend
                // twice in a row, may consider doing a settler
                float result = 0f;
                float Base = base.ActivatesOverride;
                addMisses = (MHAtkTable.Miss > 0) ? Base * MHAtkTable.Miss : 0f;
                addDodges = (MHAtkTable.Dodge > 0) ? Base * MHAtkTable.Dodge : 0f;
                addParrys = (MHAtkTable.Parry > 0) ? Base * MHAtkTable.Parry : 0f;

                result = Base + addMisses + addDodges + addParrys;

                return result;
            }
        }
        public override float TickSize
        {
            get
            {
                if (!Validated) { return 0f; }

                float DmgBonusBase = (StatS.AttackPower * combatFactors._c_mhItemSpeed) / 14f
                                   + (combatFactors.MH.MaxDamage + combatFactors.MH.MinDamage) / 2f;
                float DmgBonusU75 = 0.75f * 1.00f;
                float DmgBonusO75 = 0.25f * 1.35f;
                float DmgMod = (1f + StatS.BonusBleedDamageMultiplier)
                             * (1f + StatS.BonusDamageMultiplier)
                             * DamageBonus;
                float GlyphMOD = Talents.GlyphOfRending ? 7f / 5f : 1f;

                float damageUnder75 = (DamageBase + DmgBonusBase) * DmgBonusU75;
                float damageOver75 = (DamageBase + DmgBonusBase) * DmgBonusO75;

                float TheDamage = (damageUnder75 + damageOver75) * DmgMod;

                float TickSize = (TheDamage * GlyphMOD) / NumTicks;

                return TickSize;
            }
        }
        public override float GetDPS(float acts)
        {
            float dmgonuse = TickSize;
            float numticks = NumTicks * (acts - addMisses - addDodges - addParrys);
            float result = GetDmgOverTickingTime(acts) / FightDuration;
            return result;
        }
    }

    public class FakeWhite : Ability
    {
        public FakeWhite(Character c, Stats s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsDPSWarr co, BossOptions bo)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co; BossOpts = bo;
            //
            Name = "MH White Swing";
            Description = "White Damage";
            ReqMeleeWeap = true;
            ReqMeleeRange = true;
            Cd = Whiteattacks.MhEffectiveSpeed;
            RageCost = Whiteattacks.MHSwingRage;
            StanceOkArms = StanceOkFury = StanceOkDef = true;
            DamageBase = Whiteattacks.MhDamageOnUse;
            //DamageBonus = (1f + Talents.UnendingFury * 0.02f) * (1f + StatS.BonusWarrior_T7_2P_SlamDamage);
            //BonusCritChance = StatS.BonusWarrior_T9_4P_SLHSCritIncrease;
            //
            Initialize();
            MHAtkTable = Whiteattacks.MHAtkTable;
        }
    }
}