using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Retribution
{
    public class EffectiveCooldown : Rotation
    {

        public EffectiveCooldown(CombatStats combats) 
            : base(combats) 
        { 
        }


        public override void SetCharacterCalculations(CharacterCalculationsRetribution calc)
        {
            calc.Solution = new RotationSolution();

            foreach (Skill skill in new[] { Judge, CS, DS, Cons, Exo, HoW })
            {
                float effectiveCooldown;
                if (skill.UsableAfter20PercentHealth)
                {
                    if (skill.UsableBefore20PercentHealth)
                        effectiveCooldown =
                            Combats.CalcOpts.GetEffectiveAbilityCooldown(skill.RotationAbility.Value) *
                                (1f - Combats.CalcOpts.TimeUnder20) +
                            Combats.CalcOpts.GetEffectiveAbilityCooldownAfter20PercentHealth(
                                    skill.RotationAbility.Value) *
                                Combats.CalcOpts.TimeUnder20;
                    else
                        effectiveCooldown = Combats.CalcOpts.GetEffectiveAbilityCooldownAfter20PercentHealth(
                            skill.RotationAbility.Value);
                }
                else
                {
                    if (skill.UsableBefore20PercentHealth)
                        effectiveCooldown = Combats.CalcOpts.GetEffectiveAbilityCooldown(
                            skill.RotationAbility.Value);
                    else
                        effectiveCooldown = 0;
                }

                calc.Solution.SetAbilityEffectiveCooldown(skill.RotationAbility.Value, effectiveCooldown);
            }

            calc.Rotation = null;
        }

        public override float GetAbilityUsagePerSecond(Skill skill)
        {
            return 
                (skill.UsableBefore20PercentHealth ?
                    (1 - Combats.CalcOpts.TimeUnder20)
                        / Combats.CalcOpts.GetEffectiveAbilityCooldown(skill.RotationAbility.Value) :
                    0) +
                (skill.UsableAfter20PercentHealth ?
                    Combats.CalcOpts.TimeUnder20
                        / Combats.CalcOpts.GetEffectiveAbilityCooldownAfter20PercentHealth(
                            skill.RotationAbility.Value) :
                    0);
        }

    }
}
