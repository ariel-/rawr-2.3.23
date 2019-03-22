using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Retribution
{
    public class RotationSolution
    {

        public delegate RotationSolution RotationSolutionSource();


        public static RotationSolution Combine(
            RotationSolution solution1,
            float solution1Weight,
            RotationSolution solution2,
            float solution2Weight)
        {
            RotationSolution result = new RotationSolution();
            for (int ability = 0; ability <= (int)Ability.Last; ability++)
			{
                result.abilityUsagesPerSecond[ability] = Utilities.GetWeightedSum(
                    solution1.abilityUsagesPerSecond[ability],
					solution1Weight,
                    solution2.abilityUsagesPerSecond[ability],
                    solution2Weight);

                // Zero CD means the ability is not in rotation
                if (solution1.abilityEffectiveCooldowns[ability] == 0)
                    result.abilityEffectiveCooldowns[ability] = solution2.abilityEffectiveCooldowns[ability];
                else if (solution2.abilityEffectiveCooldowns[ability] == 0)
					result.abilityEffectiveCooldowns[ability] = solution1.abilityEffectiveCooldowns[ability];
                else
                    result.abilityEffectiveCooldowns[ability] = 1 / Utilities.GetWeightedSum(
                        1 / solution1.abilityEffectiveCooldowns[ability],
                        solution1Weight,
						1 / solution2.abilityEffectiveCooldowns[ability],
                        solution2Weight);
            }

            return result;
		}
		
        public static RotationSolution Combine(
            RotationSolutionSource solution1,
            float solution1Weight,
            RotationSolutionSource solution2,
            float solution2Weight)
        {
            if (solution1Weight == 0)
            {
                if (solution2Weight == 0)
                    return new RotationSolution();
				
                return solution2();
            }

            if (solution2Weight == 0)
				return solution1();

            return Combine(solution1(), solution1Weight, solution2(), solution2Weight);
        }


		private float[] abilityUsagesPerSecond = new float[(int)Ability.Last + 1];
        private float[] abilityEffectiveCooldowns = new float[(int)Ability.Last + 1];

		
        public float GetAbilityUsagePerSecond(Ability ability)
        {
            return abilityUsagesPerSecond[(int)ability];
        }

        public void SetAbilityUsagePerSecond(Ability ability, float usagePerSecond)
        {
			abilityUsagesPerSecond[(int)ability] = usagePerSecond;
        }

        public float GetAbilityEffectiveCooldown(Ability ability)
        {
			return abilityEffectiveCooldowns[(int)ability];
        }
		
        public void SetAbilityEffectiveCooldown(Ability ability, float effectiveCooldown)
        {
            abilityEffectiveCooldowns[(int)ability] = effectiveCooldown;
        }

    }
}