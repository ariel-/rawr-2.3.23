using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Retribution
{
    public class Simulator : Rotation
    {

        private static Ability[] RemoveHammerOfWrathFromRotation(Ability[] rotation)
        {
            List<Ability> abilities = new List<Ability>(rotation);
            abilities.Remove(Ability.HammerOfWrath);
            return abilities.ToArray();
        }

        private static Ability[] RemoveUnavailableAbilitiesFromRotation(
            Ability[] rotation,
            CombatStats combats)
        {
            Ability[] result = rotation;

            if (combats.Talents.CrusaderStrike == 0)
            {
                List<Ability> abilities = new List<Ability>(result);
                abilities.Remove(Ability.CrusaderStrike);
                result = abilities.ToArray();
            }

            if (combats.Talents.DivineStorm == 0)
            {
                List<Ability> abilities = new List<Ability>(result);
                abilities.Remove(Ability.DivineStorm);
                result = abilities.ToArray();
            }

            return result;
        }

        /// <summary>
        /// Calculates the solution with concrete parameters
        /// </summary>
        private static RotationSolution GetSolution(
            CombatStats combats,
            Ability[] rotation,
            decimal simulationTime,
            float divineStormCooldown,
            float spellHaste)
        {
            return SimulatorEngine.SimulateRotation(new SimulatorParameters(
                rotation,
                combats.CalcOpts.Wait,
                combats.CalcOpts.Delay,
                combats.Stats.JudgementCDReduction > 0,
                combats.Talents.ImprovedJudgements,
                combats.Talents.GlyphOfConsecration,
                divineStormCooldown,
                spellHaste,
                simulationTime));
        }

        /// <summary>
        /// Calculates the solution by combining subsolutions with the boss above and under 20% health
        /// </summary>
        private static RotationSolution GetCombinedSolutionWithUnder20PercentHealth(
            CombatStats combats,
            Ability[] rotation,
            decimal simulationTime,
            float divineStormCooldown,
            float spellHaste)
        {
            return RotationSolution.Combine(
                () => GetSolution(
                    combats,
                    RemoveHammerOfWrathFromRotation(rotation),
                    simulationTime,
                    divineStormCooldown,
                    spellHaste),
                1 - combats.CalcOpts.TimeUnder20,
                () => GetSolution(
                    combats,
                    rotation,
                    simulationTime,
                    divineStormCooldown,
                    spellHaste),
                combats.CalcOpts.TimeUnder20);
        }

        /// <summary>
        /// Calculates the solution by combining subsolutions with different Divine Storm cooldowns,
        /// if 2 piece T10 bonus is active.
        /// </summary>
        private static RotationSolution GetCombinedSolutionWithDivineStormCooldown(
            CombatStats combats,
            Ability[] rotation,
            decimal simulationTime,
            float spellHaste,
            float swingTime)
        {
            const float normalDivineStormCooldown = 10;
            const float cooldownRangeStep = 0.6f;

            if (combats.Stats.DivineStormRefresh == 0)
                return GetCombinedSolutionWithUnder20PercentHealth(
                    combats,
                    rotation,
                    simulationTime,
                    normalDivineStormCooldown,
                    spellHaste);

            // Calculate solutions for different Divine Storm cooldowns
            // and combine them weighted by their neighbourhood cooldown range probabilities
            RotationSolution result = null;
            float resultProbability = 0;
            for (
                    float cooldownRangeStart = 0;
                    cooldownRangeStart < normalDivineStormCooldown;
                    cooldownRangeStart += cooldownRangeStep)
            {
                float currentSolutionProbability =
                    GetT10DivineStormCooldownProbability(
                        swingTime,
                        cooldownRangeStart,
                        Math.Min(normalDivineStormCooldown, cooldownRangeStart + cooldownRangeStep),
                        combats.Stats.DivineStormRefresh);
                result = RotationSolution.Combine(
                    () => result,
                    resultProbability,
                    () => GetCombinedSolutionWithUnder20PercentHealth(
                        combats,
                        rotation,
                        simulationTime,
                        Math.Min(
                            normalDivineStormCooldown,
                            cooldownRangeStart + cooldownRangeStart / 2 + combats.CalcOpts.Delay),
                        spellHaste),
                    currentSolutionProbability);
                resultProbability += currentSolutionProbability;
            }

            // Combine with normal Divine Storm cooldown in cases when T10 doesn't proc
            return RotationSolution.Combine(
                () => result,
                resultProbability,
                () => GetCombinedSolutionWithUnder20PercentHealth(
                    combats,
                    rotation,
                    simulationTime,
                    normalDivineStormCooldown,
                    spellHaste),
                1 - resultProbability);
        }

        /// <summary>
        /// Calculates the solution by combining subsolutions with and withou Bloodlust
        /// </summary>
        private static RotationSolution GetCombinedSolutionWithBloodlust(
            CombatStats combats,
            Ability[] rotation,
            decimal simulationTime)
        {
            const float bloodlustDuration = 40f;
            const float secondsPerMinute = 60f;
            const float bloodlustHaste = 0.3f;

            // in seconds
            float fightLengthWithBloodlust = combats.CalcOpts.Bloodlust ?
                Math.Min(combats.CalcOpts.FightLength * secondsPerMinute, bloodlustDuration) :
                0;
            // in seconds
            float fightLengthWithoutBloodlust =
                Math.Max(0, combats.CalcOpts.FightLength * secondsPerMinute - fightLengthWithBloodlust);

            float bloodlustSpellHaste = (1 + combats.Stats.SpellHaste) * (1 + bloodlustHaste) - 1;

            float normalSwingTime = combats.BaseWeaponSpeed / (1 + combats.Stats.PhysicalHaste);
            float bloodlustSwingTime = normalSwingTime / (1 + bloodlustHaste);

            return RotationSolution.Combine(
                    () => GetCombinedSolutionWithDivineStormCooldown(
                        combats,
                        rotation,
                        simulationTime,
                        combats.Stats.SpellHaste,
                        normalSwingTime),
                    fightLengthWithoutBloodlust,
                    () => GetCombinedSolutionWithDivineStormCooldown(
                        combats,
                        rotation,
                        simulationTime,
                        bloodlustSpellHaste,
                        bloodlustSwingTime),
                    fightLengthWithBloodlust);
        }

        /// <summary>
        /// Calculates probability of divine storm cooldown being in the given time range.
        /// For cooldown to end early after N swings, (N - 1) swings must not proc and then 1 swing must proc.
        /// </summary>
        /// <param name="swingSpeed">Time between weapon swings</param>
        /// <param name="minCooldown">Minimal cooldown wanted</param>
        /// <param name="maxCooldown">Maximal cooldown wanted</param>
        /// <param name="procChance">Proc chance</param>
        /// <returns></returns>
        private static float GetT10DivineStormCooldownProbability(
            float swingSpeed,
            float minCooldown,
            float maxCooldown,
            float procChance)
        {
            if (swingSpeed == 0) { swingSpeed = 2.0f; } // This is to prevent accidental infinite loops

            const float divineStormCooldown = 10;

            for (
                    float integerSwingCountTime = 0;
                    integerSwingCountTime < divineStormCooldown;
                    integerSwingCountTime += swingSpeed)
                if ((minCooldown < (float)integerSwingCountTime) && ((float)integerSwingCountTime < maxCooldown))
                    return
                        GetT10DivineStormCooldownProbability(
                            swingSpeed,
                            minCooldown,
                            integerSwingCountTime,
                            procChance) +
                        GetT10DivineStormCooldownProbability(
                            swingSpeed,
                            integerSwingCountTime,
                            maxCooldown,
                            procChance);

            int nonProcSwingCount = (int)(minCooldown / swingSpeed);
            return
                GetIntegerPower(1 - procChance, nonProcSwingCount) // nonProcSwingCount swings must not proc
                * procChance                                       // 1 swing must proc
                * (maxCooldown - minCooldown) / swingSpeed;        // adjust to range size
        }

        private static float GetIntegerPower(float x, int power)
        {
            float result = 1;
            for (int currentPower = 0; currentPower < power; currentPower++)
                result *= x;

            return result;
        }


        public RotationSolution Solution { get; set; }
        public Ability[] Rotation { get; set; }

        public Simulator(CombatStats combats, Ability[] rotation, decimal simulationTime)
            : base(combats)
        {
            if (rotation == null)
                throw new ArgumentNullException("rotation");

            Ability[] effectiveRotation = RemoveUnavailableAbilitiesFromRotation(rotation, combats);

            Rotation = effectiveRotation;
            Solution = GetCombinedSolutionWithBloodlust(combats, effectiveRotation, simulationTime);
        }


        public override void SetCharacterCalculations(CharacterCalculationsRetribution calc)
        {
            calc.Solution = Solution;
            calc.Rotation = Rotation;
        }

        public override float GetAbilityUsagePerSecond(Skill skill)
        {
            return Solution.GetAbilityUsagePerSecond(skill.RotationAbility.Value);
        }
    }
}
