using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Retribution
{
    public class SimulatorParameters : IEquatable<SimulatorParameters>
    {

        public bool T7_4pc { get; private set; }
        public int ImpJudgements { get; private set; }
        public bool GlyphConsecrate { get; private set; }
        public Ability[] Priorities { get; private set; }
        public decimal Wait { get; private set; }
        public decimal Delay { get; private set; }
        public decimal DivineStormCooldown { get; private set; }
		public decimal SpellGCD { get; private set; }
        public decimal SimulationTime { get; private set; }


        public SimulatorParameters(
            Ability[] priorities, 
            float wait, 
            float delay, 
            bool t7_4pc, 
            int impJudgements, 
            bool glyphConsecrate, 
            float divineStormCooldown,
            float spellHaste,
            decimal simulationTime)
		{
            Priorities = (Ability[])priorities.Clone();
            T7_4pc = t7_4pc;
            GlyphConsecrate = glyphConsecrate;
            Wait = (decimal)Math.Round(wait, 5);
            Delay = (decimal)Math.Round(delay, 5);
            ImpJudgements = impJudgements;
            DivineStormCooldown = (decimal)Math.Round(divineStormCooldown, 5);
            SpellGCD = (decimal)Math.Round(Math.Max(1f, 1.5f / (1 + spellHaste)), 5);
            SimulationTime = simulationTime;
        }

        public override bool Equals(Object obj)
        {
            return Equals(obj as SimulatorParameters);
        }

        public bool Equals(SimulatorParameters other)
        {
            if (other == null) 
                return false;

			// Default comparer is slow for enums
            if (!Utilities.AreArraysEqual(Priorities, other.Priorities, (x, y) => x == y))
                return false;

            return (T7_4pc == other.T7_4pc) &&
                (DivineStormCooldown == other.DivineStormCooldown) &&
                (GlyphConsecrate == other.GlyphConsecrate) &&
                (Delay == other.Delay) &&
                (Wait == other.Wait) &&
                (ImpJudgements == other.ImpJudgements) &&
                (SpellGCD == other.SpellGCD) &&
                (SimulationTime == other.SimulationTime);
        }

        public override int GetHashCode()
        {
            return Utilities.GetCombinedHashCode(
                T7_4pc,
                ImpJudgements,
                GlyphConsecrate,
                Wait,
                Delay,
                DivineStormCooldown,
                SpellGCD,
                Utilities.GetArrayHashCode(Priorities),
                SimulationTime);
        }

        public static string ShortAbilityString(Ability ability)
        {
            if (ability == Ability.Consecration) return "Con";
            if (ability == Ability.CrusaderStrike) return "CS";
            if (ability == Ability.Judgement) return "Jud";
            if (ability == Ability.DivineStorm) return "DS";
            if (ability == Ability.Exorcism) return "Exo";
            if (ability == Ability.HammerOfWrath) return "HoW";
            return "?";
        }

        public static string RotationString(Ability[] rotation)
        {
            string s = "";
            for (int i = 0; i < rotation.Length; i++)
            {
                s += SimulatorParameters.ShortAbilityString(rotation[i]);
                if (i + 1 < rotation.Length) s += " > ";
            }
            return s;
        }

        public static string ShortRotationString(Ability[] rotation)
        {
            string s = "";
            for (int i = 0; i < rotation.Length; i++)
			{
                s += SimulatorParameters.ShortAbilityString(rotation[i]);
                if (i + 1 < rotation.Length) s += "-";
            }
            return s;
        }

		public static string AbilityString(Ability ability)
        {
            if (ability == Ability.Consecration) return "Consecration";
			if (ability == Ability.CrusaderStrike) return "Crusader Strike";
            if (ability == Ability.Judgement) return "Judgement";
            if (ability == Ability.DivineStorm) return "Divine Storm";
			if (ability == Ability.Exorcism) return "Exorcism";
            if (ability == Ability.HammerOfWrath) return "Hammer of Wrath";
            return "?";
        }

        public static Ability[] DefaultRotation()
        {
            return new Ability[] { Ability.CrusaderStrike, Ability.HammerOfWrath, Ability.Judgement,
                Ability.Consecration, Ability.DivineStorm, Ability.Exorcism };
        }

		public override string ToString()
        {
			string ret = "";
            foreach (Ability a in Priorities)
            {
                ret += ShortAbilityString(a) + ", ";
			}
            return ret;
        }

    }

}