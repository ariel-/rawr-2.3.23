using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Retribution
{
	public abstract class Rotation
    {

        public static IEnumerable<Ability[]> GetAllRotations()
        {
			Ability[] abilities = new Ability[(int)Ability.Last + 1];

            for (int ability = 0; ability <= (int)Ability.Last; ability++)
                abilities[ability] = (Ability)ability;

			return Utilities.GetDifferentElementPermutations(abilities);
        }


        protected Rotation(CombatStats combats)
        {
            if (combats == null)
                throw new ArgumentNullException("combats");
			
            Combats = combats;
			CS = combats.Talents.CrusaderStrike == 0 ? (Skill)new NullCrusaderStrike(combats) : (Skill)new CrusaderStrike(combats);
            DS = combats.Talents.DivineStorm == 0 ? (Skill)new NullCrusaderStrike(combats) : (Skill)new DivineStorm(combats);
            Exo = new Exorcism(combats);
            HoW = new HammerOfWrath(combats);
            Cons = new Consecration(combats);
			White = new White(combats);
            HoR = new HandOfReckoning(combats);

            switch (combats.CalcOpts.Seal)
            {
                case SealOf.Righteousness:
                    Seal = new SealOfRighteousness(combats);
                    SealDot = new NullSealDoT(combats);
                    Judge = new JudgementOfRighteousness(combats);
                    break;

                case SealOf.Command:
                    if (combats.Talents.SealOfCommand == 0)
                        goto default;

					Seal = new SealOfCommand(combats);
                    SealDot = new NullSealDoT(combats);
                    Judge = new JudgementOfCommand(combats);
                    break;

                case SealOf.Vengeance:
                    float stack = AverageSoVStackSize();
                    Seal = new SealOfVengeance(combats, stack);
                    SealDot = new SealOfVengeanceDoT(combats, stack);
                    Judge = new JudgementOfVengeance(combats, stack);
                    break;

                default:
                    Seal = new NullSeal(combats);
                    SealDot = new NullSealDoT(combats);
					Judge = new NullJudgement(combats);
                    break;
            }
        }


        public Skill CS { get; private set; }
        public Skill Judge { get; private set; }
        public Skill DS { get; private set; }
        public Skill Exo { get; private set; }
        public Skill HoW { get; private set; }
        public Skill Cons { get; private set; }
		public Skill Seal { get; private set; }
        public Skill SealDot { get; private set; }
        public Skill HoR { get; private set; }
        public White White { get; private set; }
        public CombatStats Combats { get; private set; }


        public abstract void SetCharacterCalculations(CharacterCalculationsRetribution calc);

        public void SetDPS(CharacterCalculationsRetribution calc)
        {
            SetCharacterCalculations(calc);

            calc.AverageSoVStack = AverageSoVStackSize();
            calc.SoVOvertake = SoVOvertakeTime();

            calc.WhiteDPS = White.WhiteDPS();
            calc.SealDPS = SealDPS(Seal, SealDot);
            calc.JudgementDPS = GetAbilityDps(Judge);
            calc.DivineStormDPS = GetAbilityDps(DS);
            calc.CrusaderStrikeDPS = GetAbilityDps(CS);
            calc.ConsecrationDPS = GetAbilityDps(Cons);
            calc.ExorcismDPS = GetAbilityDps(Exo);
            calc.HammerOfWrathDPS = GetAbilityDps(HoW);
            calc.HandOfReckoningDPS = HandOfReckoningDPS(HoR);

            calc.WhiteSkill = White;
            calc.SealSkill = Seal;
            calc.JudgementSkill = Judge;
            calc.DivineStormSkill = DS;
            calc.CrusaderStrikeSkill = CS;
            calc.ConsecrationSkill = Cons;
            calc.ExorcismSkill = Exo;
			calc.HammerOfWrathSkill = HoW;
            calc.HandOfReckoningSkill = HoR;

            calc.DPSPoints =
                calc.WhiteDPS +
                calc.SealDPS +
                calc.JudgementDPS +
				calc.CrusaderStrikeDPS +
                calc.DivineStormDPS +
                calc.ExorcismDPS +
                calc.HandOfReckoningDPS +
				calc.ConsecrationDPS +
                calc.HammerOfWrathDPS +
                calc.OtherDPS;
        }

        public float DPS()
        {
            return 
                White.WhiteDPS() + 
                SealDPS(Seal, SealDot)+ 
                GetAbilityDps(Judge) + 
                GetAbilityDps(DS) +
				GetAbilityDps(CS) + 
                GetAbilityDps(Cons) + 
                GetAbilityDps(Exo) + 
                GetAbilityDps(HoW) + 
                HandOfReckoningDPS(HoR);
        }

        public float SealProcsPerSec(Skill seal)
        {
            if (seal.GetType() == typeof(SealOfVengeance))
                return GetMeleeAttacksPerSec();
            else
                return GetMeleeAttacksPerSec() + GetAbilityHitsPerSecond(Judge);
        }

        public float SoVOvertakeTime()
        {
			float sov0dps = GetAbilityDps(new JudgementOfVengeance(Combats, 0));
            float sov5dps = GetAbilityDps(new JudgementOfVengeance(Combats, 5))
                + SealDPS(new SealOfVengeance(Combats, 5), new SealOfVengeanceDoT(Combats, 5));
            float sordps = GetAbilityDps(new JudgementOfRighteousness(Combats))
                + SealDPS(new SealOfRighteousness(Combats), new NullSealDoT(Combats));

            if (sordps > sov0dps)
            {
                float averageStack = (sordps - sov0dps) / (sov5dps - sov0dps) * 5f;
				float timeToMaxStack = Combats.AttackSpeed * 4f;
                return 2.5f * timeToMaxStack / (5f - averageStack);
            }
            else { return 0; }
        }

        public float AverageSoVStackSize()
		{
            float averageTimeOnMob = Combats.CalcOpts.FightLength * 60f / (Combats.CalcOpts.TargetSwitches + 1);
            float timeToMaxStack = Combats.AttackSpeed * 4f;
            if (averageTimeOnMob > timeToMaxStack)
            {
				return (2.5f * timeToMaxStack + 5f * (averageTimeOnMob - timeToMaxStack)) / averageTimeOnMob;
            }
            else
            {
                return 2.5f * averageTimeOnMob / timeToMaxStack;
            }
        }

		public virtual float SealDPS(Skill seal, Skill sealdot)
        {
            return sealdot.AverageDamage() / 3f + seal.AverageDamage() * SealProcsPerSec(seal);
        }

        public virtual float HandOfReckoningDPS(Skill hor) 
        { 
            return hor.AverageDamage() / 8f * Combats.CalcOpts.HoREff; 
        }

        public float GetAbilityDps(Skill skill)
        {
            return skill.AverageDamage() * GetAbilityUsagePerSecond(skill);
        }

        public float GetAbilityHitsPerSecond(Skill skill)
		{
            return 
                GetAbilityUsagePerSecond(skill) *
                skill.ChanceToLand() * 
                skill.Targets() * 
                skill.TickCount();
        }

        public float GetAbilityCritsPerSecond(Skill skill)
        {
            return 
                GetAbilityUsagePerSecond(skill) *
                skill.ChanceToCrit() * 
                skill.Targets() * 
                skill.TickCount();
		}

        public float GetCrusaderStrikeCD()
        {
            return 1 / GetAbilityUsagePerSecond(CS);
        }

        public float GetJudgementCD()
        {
            return 1 / GetAbilityUsagePerSecond(Judge);
        }

        public abstract float GetAbilityUsagePerSecond(Skill skill);

        public float GetMeleeAttacksPerSec()
        {
            // Melee hit procs can be triggered by:
            // - Crusader Strike hits
            // - Divine Storm hits
			// - Weapon swing hits
            // - Tiny Abomination in a Jar releasing attack hits
            // (2 multiplier needs to be moved to another place)
            // - Judgement hits

            return
                GetAbilityHitsPerSecond(CS) +
                GetAbilityHitsPerSecond(DS) +
                White.ChanceToLand() / Combats.AttackSpeed +
                Combats.Stats.MoteOfAnger * 2 * White.ChanceToLand() +
                GetAbilityHitsPerSecond(Judge);
		}

        public float GetPhysicalAttacksPerSec()
        {
            // Physical hit procs can be triggered by:
            // - Crusader Strike hits
            // - Divine Storm hits
            // - Weapon swing hits
            // - Tiny Abomination in a Jar releasing attack hits
			// (2 multiplier needs to be moved to another place)
            // - Judgement hits
            // - Hammer of Wrath hits

            return
                GetAbilityHitsPerSecond(CS) +
                GetAbilityHitsPerSecond(DS) +
                White.ChanceToLand() / Combats.AttackSpeed +
                Combats.Stats.MoteOfAnger * 2 * White.ChanceToLand() +
				GetAbilityHitsPerSecond(Judge) +
                GetAbilityHitsPerSecond(HoW);
        }

        public float GetSpellAttacksPerSec()
        {
            // Spell hit procs can be triggered by:
            // - Exorcism hits
            // - Consecration hits (first tick)
            return
                GetAbilityHitsPerSecond(Exo) +
                GetAbilityHitsPerSecond(Cons) / Cons.TickCount();
                //GetAbilityHitsPerSecond(Judge); There was some talk about Judgement also triggering this, but that would make Judgement be both spell and melee hit?
        }

        public float GetMeleeCritsPerSec()
        {
            // Melee crit procs can be triggered by:
            // - Crusader Strike crits
            // - Divine Storm crits on each target
            // - Weapon swing crits
            // - Tiny Abomination in a Jar releasing attack crits
            // (2 multiplier needs to be moved to another place)
            // - Judgement crits
			
            return
                GetAbilityCritsPerSecond(CS) +
                GetAbilityCritsPerSecond(DS) +
                White.ChanceToCrit() / Combats.AttackSpeed +
                Combats.Stats.MoteOfAnger * 2 * White.ChanceToCrit() +
                GetAbilityCritsPerSecond(Judge);
       }

        public float GetPhysicalCritsPerSec()
        {
            // Physical crit procs can be triggered by:
            // - Crusader Strike crits
            // - Divine Storm crits on each target
            // - Weapon swing crits
			// - Tiny Abomination in a Jar releasing attack crits
            // (2 multiplier needs to be moved to another place)
            // - Judgement crits
            // - Hammer of Wrath crits

            return
                GetAbilityCritsPerSecond(CS) +
                GetAbilityCritsPerSecond(DS) +
                White.ChanceToCrit() / Combats.AttackSpeed +
                Combats.Stats.MoteOfAnger * 2 * White.ChanceToCrit() +
                GetAbilityCritsPerSecond(Judge) +
                GetAbilityCritsPerSecond(HoW);
        }

        public float GetSpellCritsPerSec()
        {
            // Spell hit procs can be triggered by:
            // - Exorcism crits
            // - Consecration crits (first tick)
            return
                GetAbilityCritsPerSecond(Exo) +
                GetAbilityCritsPerSecond(Cons) / Cons.TickCount();
            //GetAbilityHitsPerSecond(Judge); There was some talk about Judgement also triggering this, but that would make Judgement be both spell and melee hit?
        }

        public float GetAttacksPerSec()
        {
            // Damage done procs and damage or healing done procs can be triggered by:
            // - Crusader Strike hits
            // - Divine Storm hits
            // - Weapon swing hits
            // - Tiny Abomination in a Jar releasing attack hits
            // (2 multiplier needs to be moved to another place)
            // - Judgement hits
            // - Hammer of Wrath hits
            // - Consecration damage ticks
            // - Exorcism hits

			return
                GetAbilityHitsPerSecond(CS) +
                GetAbilityHitsPerSecond(DS) +
                White.ChanceToLand() / Combats.AttackSpeed +
                Combats.Stats.MoteOfAnger * 2 * White.ChanceToLand() +
                GetAbilityHitsPerSecond(Judge) +
                GetAbilityHitsPerSecond(HoW) +
                GetAbilityHitsPerSecond(Cons) +
                GetAbilityHitsPerSecond(Exo);
        }
		
    }
}
