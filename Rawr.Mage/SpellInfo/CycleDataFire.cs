using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Mage
{
    public static class FBPyro
    {
        public static Cycle GetCycle(bool needsDisplayCalculations, CastingState castingState)
        {
            Cycle cycle = Cycle.New(needsDisplayCalculations, castingState);
            float K;
            cycle.Name = "FBPyro";

            Spell FB = castingState.GetSpell(SpellId.Fireball);
            Spell Pyro = castingState.GetSpell(SpellId.PyroblastPOMDotUptime);

            // S0: no proc, 0 count
            // FB   => S0   1-c
            //      => S1   c
            // S1: no proc, 1 count
            // FB   => S0   1-c
            //      => S2   c
            // S2: proc, 0 count
            // Pyro  => S0  (1-T8)
            //       => S2  T8

            // S0 = S0 * (1-c) + S1 * (1 - c) + S2 * (1 - T8)
            // S1 = S0 * c
            // S2 = S1 * c + S2 * T8
            // S0 + S1 + S2 = 1

            // no Pyro
            // FB => no Pyro 1 - c*c/(1+c)
            //    => Pyro c*c/(1+c)
            // Pyro
            // Pyro => no Pyro

            // 1 - c*c/(1+c)
            // FB

            // c*c/(1+c)

            // c^2/((1 + c)*(1 - T8))

            float T8 = CalculationOptionsMage.SetBonus4T8ProcRate * castingState.BaseStats.Mage4T8;

            K = FB.CritRate * FB.CritRate / (1.0f + FB.CritRate) * castingState.MageTalents.HotStreak / 3.0f / (1 - T8);
            if (castingState.MageTalents.Pyroblast == 0) K = 0.0f;

            float hasteFactor = 1.0f;
            if (castingState.BaseStats.Mage2T10 > 0)
            {
                // p = K / (1 + K)
                // N = (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FB.CastTime * X + LB.CastTime * (1-X) + Pyro.CastTime * K)
                hasteFactor = 1.12f / ((float)Math.Pow(1 - K / (1.0f + K), (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FB.CastTime + Pyro.CastTime * K) + 0.5f) * 0.12f + 1f);
            }


            //dot uptime

            //4 ticks, 1 every 3 seconds

            //on average when you cast the spell in a specific state, how many ticks
            //will you get?

            //sum_i=0..3 i/4 * Pr(i * 3 < time till next pyro <= (i + 1) * 3) +
            //Pr(time till next pyro > 12)

            //T ~ time till next pyro

            //1/4 * Pr(3 < T <= 6) + 2/4 * Pr(6 < T <= 9) + 3/4 * Pr(9 < T <= 12) + Pr(12 < T)

            //1/4 * Pr(T <= 6) - 1/4 * Pr(T <= 3) + 2/4 * Pr(T <= 9) - 2/4 * Pr(T <= 6) + 3/4 * Pr(T <= 12) - 3/4 * Pr(T <= 9) + 1 - Pr(T <= 12)

            //1 - 1/4 * (Pr(T <= 3) + Pr(T <= 6) + Pr(T <= 9) + Pr(T <= 12))


            //1 - 1/N * sum_i=1..N Pr(T <= i * tick)


            //T2 = DP * (T8 * D0 + (1-T8) * T0)

            //T0 = DFB * (c * T1 + (1-c) * T0)

            //T1 = DFB * (c * HS * D0 + (1 - c * HS) * T0)


            //T0 = DFB * (c * DFB * (c * HS * D0 + (1 - c * HS) * T0) + (1-c) * T0)

            //D0 * T0 = c^2 * HS * D2FB + (c * (1 - c * HS) * D2FB + (1-c) * DFB) * T0

            //(D0 - c * (1 - c * HS) * D2FB - (1-c) * DFB) * T0 = c^2 * HS * D2FB


            //T0 := sum_i=0..inf ki * Dti

            //k0 = 0
            //k1 = 0
            //k2 = c^2 * HS

            //kn+2 - c*(1-c*HS) * kn - (1-c) * kn+1 = 0, n>0

            //kn+2 = c*(1-c*HS) * kn + (1-c) * kn+1

            //k3 = (1-c)*c^2

            //k4 = (1-c)* (c^3 + (1-c)*c^2)

            //((sqrt((-3)*c^2+2*c+1)-c+1)^n*(c*sqrt((-3)*c^2+2*c+1)-3*c^2-c))/((6*c^2-4*c-2)*2^n)-
            //((-sqrt((-3)*c^2+2*c+1)-c+1)^n*(c*sqrt((-3)*c^2+2*c+1)+3*c^2+c))/((6*c^2-4*c-2)*2^n)

            //((((sqrt(-3*c^2+2*c+1)-c+1)^n-(-sqrt(-3*c^2+2*c+1)-c+1)^n)*sqrt(-3*c^2+2*c+1)+
            //(-3*c-1)*(sqrt(-3*c^2+2*c+1)-c+1)^n+(-3*c-1)*(-sqrt(-3*c^2+2*c+1)-c+1)^n)*c)/(2*(3*c^2-2*c-1)*2^n)

            //Kn = sum_i=0..n ki

            //K2 = c^2

            //Kn+2= Kn+1 + kn+2 = Kn+1 + c*(1-c) * kn + (1-c) * kn+1



            //T2 = DP * (T8 * D0 + (1-T8) * T0)

            //T0 = (x * DFB * (cFB * T1 + (1-cFB) * T0) + (1 - x) * DLB * (cLB * T1 +(1-cLB) * T0))

            //T1 = x * DFB * (cFB * D0 + (1-cFB) * T0) + (1 - x) * DLB * (cLB * D0 +(1-cLB) * T0)



            //T0 = [x * cFB * DFB + (1 - x) * cLB * DLB] * T1 + [x * (1-cFB) * DFB +(1 - x) * (1-cLB) * DLB] * T0

            //T1 = [x * cFB * DFB + (1 - x) * cLB * DLB] * D0 + [x * (1-cFB) * DFB +(1 - x) * (1-cLB) * DLB] * T0


            //A := [x * cFB * DFB + (1 - x) * cLB * DLB]  ~ (x * cFB + (1 - x) * cLB)* D[x * FB + (1 - x) * LB]
            //B := [x * (1-cFB) * DFB + (1 - x) * (1-cLB) * DLB] ~ (1 - (x * cFB + (1- x) * cLB)) * D[x * FB + (1 - x) * LB]

            //T0 = A * T1 + B * T0
            //T1 = A * D0 + B * T0

            //T0 = A * (A * D0 + B * T0) + B * T0

            //(D0 - B - A * B) * T0 = A * A

            // for one spell case we evaluate the dot tick uptime exactly, for others we use approximation using an average "filler" spell
            // if this turns out to be computationally expensive add it as an accuracy option

            //k0 = 0
            //k1 = 0
            //k2 = c^2 * HS
            //kn+2 = c*(1-c*HS) * kn + (1-c) * kn+1

            float averageTicks = 0f;
            float C = FB.CritRate;
            float HS = castingState.MageTalents.HotStreak / 3.0f;
            float k1 = 0;
            float k2 = C * C * HS;
            float totalChance = k2;
            int n = 2;

            averageTicks += Math.Min((int)(Pyro.CastTime / hasteFactor / 3.0f), 4) * T8;
            averageTicks += Math.Min((int)((Pyro.CastTime + n * FB.CastTime) / hasteFactor / 3.0f), 4) * (1 - T8) * k2;

            while ((Pyro.CastTime + n * FB.CastTime) / hasteFactor < 12)
            {
                float tmp = k1;
                k1 = k2;
                k2 = C * (1 - C * HS) * tmp + (1 - C) * k1;
                totalChance += k2;
                n++;
                averageTicks += Math.Min((int)((Pyro.CastTime + n * FB.CastTime) / hasteFactor / 3.0f), 4) * (1 - T8) * k2;
            }
            averageTicks += 4 * (1 - T8) * (1 - totalChance);

            // ignite ticks



            cycle.AddSpell(needsDisplayCalculations, FB, 1);
            cycle.AddSpell(needsDisplayCalculations, Pyro, K, averageTicks / 4.0f);
            cycle.CastTime /= hasteFactor;
            cycle.Calculate();
            return cycle;
        }
    }

    public static class FBLBPyro
    {
        public static Cycle GetCycle(bool needsDisplayCalculations, CastingState castingState)
        {
            Cycle cycle = Cycle.New(needsDisplayCalculations, castingState);
            Spell FB;
            Spell LB;
            Spell Pyro;
            float X;
            float K;
            float C;
            cycle.Name = "FBLBPyro";

            FB = castingState.GetSpell(SpellId.Fireball);
            LB = castingState.GetSpell(SpellId.LivingBomb);
            Pyro = castingState.GetSpell(SpellId.PyroblastPOMDotUptime);

            float T8 = CalculationOptionsMage.SetBonus4T8ProcRate * castingState.BaseStats.Mage4T8;
            float H = castingState.MageTalents.HotStreak / 3.0f;

            // 3.0.8 calcs

            // 0 HS charge:
            // FB     => 0 HS charge    (1 - FBcrit) * X
            //        => 1 HS charge    FBcrit * X
            // LB     => 0 HS charge    (1 - LBcrit) * (1 - X)
            //        => 1 HS charge    LBcrit * (1 - X)
            // 1 HS charge:
            // FB     => 0 HS charge    (1 - FBcrit) * X + (1 - H) * FBcrit * X
            // FBPyro => 0 HS charge    H * FBcrit * X
            // LB     => 0 HS charge    (1 - LBcrit) * (1 - X) + (1 - H) * LBcrit * (1 - X)
            // LBPyro => 0 HS charge    H * LBcrit * (1 - X)

            // S0 + S1 = 1
            // S0 = S0 * [(1 - FBcrit) * X + (1 - LBcrit) * (1 - X)] + S1
            // S1 = S0 * [FBcrit * X + LBcrit * (1 - X)]

            // 1 - S0 = S0 * [FBcrit * X + LBcrit * (1 - X)]
            // S0 = 1 / (1 + FBcrit * X + LBcrit * (1 - X))
            // C := FBcrit * X + LBcrit * (1 - X) = LBcrit + X * (FBcrit - LBcrit)
            // S0 = 1 / (1 + C)
            // S1 = C / (1 + C)

            // value = S0 * (X * value(FB) + (1 - X) * value(LB)) + S1 * (X * value(FB) + (1 - X) * value(LB) + H * C * value(Pyro))
            //       = X * value(FB) + (1 - X) * value(LB) + H * C * C / (1 + C) / (1 - T8) * value(Pyro)

            // time(LB) * (1 - X) / [time(FB) * X + time(LB) * (1 - X) + time(Pyro) * H * C * C / (1 + C) / (1 - T8)] = time(LB) / 12
            // 12 * (1 - X) = time(FB) * X + time(LB) * (1 - X) + time(Pyro) * H * C * C / (1 + C) / (1 - T8)

            // (1 + C) * (12 * (1 - X) - time(FB) * X - time(LB) * (1 - X)) = time(Pyro) / (1 - T8) * H * C * C
            // (1 + C) * (12 - 12 * X - time(FB) * X - time(LB) + time(LB) * X) = time(Pyro) / (1 - T8) * H * C * C
            // (1 + LBcrit + X * (FBcrit - LBcrit)) * (12 - time(LB) + X * (time(LB) - time(FB) - 12)) = time(Pyro) / (1 - T8) * H * C * C
            // [(1 + LBcrit) * (12 - time(LB)) - time(Pyro) / (1 - T8) * H * LBcrit * LBcrit] + X * [(FBcrit - LBcrit) * (12 - time(LB)) + (time(LB) - time(FB) - 12) * (1 + LBcrit) - time(Pyro) / (1 - T8) * H * 2 * LBcrit * (FBcrit - LBcrit)] + X * X * [(FBcrit - LBcrit) * (time(LB) - time(FB) - 12) - (FBcrit - LBcrit) * (FBcrit - LBcrit) * time(Pyro) / (1 - T8) * H] = 0

            // more accurate LB uptime model
            // LB is cast every 12 + half of average non LB spell duration
            // K := H * C * C / (1 + C) / (1 - T8)
            // average non lb cast time = (time(FB) * X + time(Pyro) * K) / (X + K)

            // time(LB) * (1 - X) / [time(FB) * X + time(LB) * (1 - X) + time(Pyro) * K] = time(LB) / (12 + 0.5 * ((time(FB) * X + time(Pyro) * K) / (X + K)))
            // 12 - 12 * X + 0.5 * (1 - X) * ((time(FB) * X + time(Pyro) * K) / (X + K)) = time(FB) * X + time(LB) * (1 - X) + time(Pyro) * K

            // solving this exactly is ... polynomial order 4 with very very ugly coefficients
            // so let's do an approximation instead
            // we'll use the max uptime assumption to get an approximation for K
            // then we'll compute the new X under the new uptime equation, which basically means we'll replace the 12 by the adjusted value

            float FBcrit = FB.CritRate;
            float LBcrit = LB.CritRate;
            if (castingState.MageTalents.Pyroblast == 0) H = 0.0f;
            float A2 = (FBcrit - LBcrit) * (LB.CastTime - FB.CastTime - 12) - (FBcrit - LBcrit) * (FBcrit - LBcrit) * Pyro.CastTime / (1 - T8) * H;
            float A1 = (FBcrit - LBcrit) * (12 - LB.CastTime) + (LB.CastTime - FB.CastTime - 12) * (1 + LBcrit) - Pyro.CastTime / (1 - T8) * H * 2 * LBcrit * (FBcrit - LBcrit);
            float A0 = (1 + LBcrit) * (12 - LB.CastTime) - Pyro.CastTime / (1 - T8) * H * LBcrit * LBcrit;
            if (Math.Abs(A2) < 0.00001)
            {
                X = -A0 / A1;
            }
            else
            {
                X = (float)((-A1 - Math.Sqrt(A1 * A1 - 4 * A2 * A0)) / (2 * A2));
            }
            C = LBcrit + X * (FBcrit - LBcrit);
            K = H * C * C / (1 + C) / (1 - T8);

            // 2T10 doesn't affect the state space, so we can calculate X ignoring 2T10
            // then we can expand the state to include haste to get the actual 2T10 haste distribution
            // 0 HS charge:
            // FB     => 0 HS charge    (1 - FBcrit) * X
            //        => 1 HS charge    FBcrit * X
            // LB     => 0 HS charge    (1 - LBcrit) * (1 - X)
            //        => 1 HS charge    LBcrit * (1 - X)
            // 1 HS charge:
            // FB     => 0 HS charge    (1 - FBcrit) * X + (1 - H) * FBcrit * X
            // FBPyro => 0 HS charge    H * FBcrit * X
            // LB     => 0 HS charge    (1 - LBcrit) * (1 - X) + (1 - H) * LBcrit * (1 - X)
            // LBPyro => 0 HS charge    H * LBcrit * (1 - X)

            // reducing the state space to one state model for purpose of 2T10 haste share calculation we get 
            // probability of being hasted = 1 - (1-p)^N
            // where
            // p = probability of haste generating spell
            // N = average number of spells affected by haste = (haste duration - average cast time of haste generating spell) / (average cast time of hasted spells) + 0.5
            // hasteFactor = 1 / (((1-p)^N) * 1 + (1 - (1-p)^N) * 1/1.12)
            //             = 1.12 / (((1-p)^N) * 0.12 + 1)

            float hasteFactor = 1.0f;
            if (castingState.BaseStats.Mage2T10 > 0)
            {
                // p = K / (1 + K)
                // N = (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FB.CastTime * X + LB.CastTime * (1-X) + Pyro.CastTime * K)
                hasteFactor = 1.12f / ((float)Math.Pow(1 - K / (1.0f + K), (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FB.CastTime * X + LB.CastTime * (1 - X) + Pyro.CastTime * K) + 0.5f) * 0.12f + 1f);
            }

            // first order correction for lower LB uptime

            // LBrecastInterval = 12 + 0.5 * ((time(FB) * X + time(Pyro) * K) / (X + K))
            float LBrecastInterval = 12 + 0.5f * ((FB.CastTime * FB.CastTime * X + Pyro.CastTime * Pyro.CastTime * K) / (FB.CastTime * X + Pyro.CastTime * K)) / hasteFactor;

            // now recalculate the spell distribution under the new uptime assumption
            A2 = (FBcrit - LBcrit) * (LB.CastTime / hasteFactor - FB.CastTime / hasteFactor - LBrecastInterval) - (FBcrit - LBcrit) * (FBcrit - LBcrit) * Pyro.CastTime / hasteFactor / (1 - T8) * H;
            A1 = (FBcrit - LBcrit) * (LBrecastInterval - LB.CastTime / hasteFactor) + (LB.CastTime / hasteFactor - FB.CastTime / hasteFactor - LBrecastInterval) * (1 + LBcrit) - Pyro.CastTime / hasteFactor / (1 - T8) * H * 2 * LBcrit * (FBcrit - LBcrit);
            A0 = (1 + LBcrit) * (LBrecastInterval - LB.CastTime / hasteFactor) - Pyro.CastTime / hasteFactor / (1 - T8) * H * LBcrit * LBcrit;
            if (Math.Abs(A2) < 0.00001)
            {
                X = -A0 / A1;
            }
            else
            {
                X = (float)((-A1 - Math.Sqrt(A1 * A1 - 4 * A2 * A0)) / (2 * A2));
            }
            C = LBcrit + X * (FBcrit - LBcrit);
            K = H * C * C / (1 + C) / (1 - T8);

            // pyro dot uptime 

            //A := [x * cFB * DFB + (1 - x) * cLB * DLB]  ~ (x * cFB + (1 - x) * cLB)* D[x * FB + (1 - x) * LB]
            //B := [x * (1-cFB) * DFB + (1 - x) * (1-cLB) * DLB] ~ (1 - (x * cFB + (1- x) * cLB)) * D[x * FB + (1 - x) * LB]

            float averageTicks = 0f;
            float k1 = 0;
            float k2 = C * C * H;
            float totalChance = k2;
            float averageCastTime = (LB.CastTime + X * (FB.CastTime - LB.CastTime)) / hasteFactor;
            int n = 2;

            averageTicks += Math.Min((int)(Pyro.CastTime / hasteFactor / 3.0f), 4) * T8;
            averageTicks += Math.Min((int)((Pyro.CastTime / hasteFactor + n * averageCastTime) / 3.0f), 4) * (1 - T8) * k2;

            while (Pyro.CastTime / hasteFactor + n * averageCastTime < 12)
            {
                float tmp = k1;
                k1 = k2;
                k2 = C * (1 - C * H) * tmp + (1 - C) * k1;
                totalChance += k2;
                n++;
                averageTicks += Math.Min((int)((Pyro.CastTime / hasteFactor + n * averageCastTime) / 3.0f), 4) * (1 - T8) * k2;
            }
            averageTicks += 4 * (1 - T8) * (1 - totalChance);

            cycle.AddSpell(needsDisplayCalculations, FB, X);
            cycle.AddSpell(needsDisplayCalculations, LB, 1 - X);
            cycle.AddSpell(needsDisplayCalculations, Pyro, K, averageTicks / 4.0f);
            cycle.CastTime /= hasteFactor;
            cycle.Calculate();
            return cycle;
        }
    }

    public static class FFBLBPyro
    {
        public static Cycle GetCycle(bool needsDisplayCalculations, CastingState castingState)
        {
            Cycle cycle = Cycle.New(needsDisplayCalculations, castingState);
            Spell FFB;
            Spell LB;
            Spell Pyro;
            float X;
            float K;
            cycle.Name = "FFBLBPyro";

            FFB = castingState.GetSpell(SpellId.FrostfireBolt);
            LB = castingState.GetSpell(SpellId.LivingBomb);
            Pyro = castingState.GetSpell(SpellId.PyroblastPOMDotUptime);

            float T8 = CalculationOptionsMage.SetBonus4T8ProcRate * castingState.BaseStats.Mage4T8;

            float FFBcrit = FFB.CritRate;
            float LBcrit = LB.CritRate;
            float LBDotCrit = castingState.FireCritRate;
            float H = castingState.MageTalents.HotStreak / 3.0f;
            float C;
            if (castingState.MageTalents.Pyroblast == 0) H = 0.0f;

                float A2 = (FFBcrit - LBcrit) * (LB.CastTime - FFB.CastTime - 12) - (FFBcrit - LBcrit) * (FFBcrit - LBcrit) * Pyro.CastTime / (1 - T8) * H;
                float A1 = (FFBcrit - LBcrit) * (12 - LB.CastTime) + (LB.CastTime - FFB.CastTime - 12) * (1 + LBcrit) - Pyro.CastTime / (1 - T8) * H * 2 * LBcrit * (FFBcrit - LBcrit);
                float A0 = (1 + LBcrit) * (12 - LB.CastTime) - Pyro.CastTime / (1 - T8) * H * LBcrit * LBcrit;
                if (Math.Abs(A2) < 0.00001)
                {
                    X = -A0 / A1;
                }
                else
                {
                    X = (float)((-A1 - Math.Sqrt(A1 * A1 - 4 * A2 * A0)) / (2 * A2));
                }
                C = LBcrit + X * (FFBcrit - LBcrit);
                K = H * C * C / (1 + C) / (1 - T8);

                float hasteFactor = 1.0f;
                if (castingState.BaseStats.Mage2T10 > 0)
                {
                    // p = K / (1 + K)
                    // N = (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FB.CastTime * X + LB.CastTime * (1-X) + Pyro.CastTime * K)
                    hasteFactor = 1.12f / ((float)Math.Pow(1 - K / (1.0f + K), (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FFB.CastTime * X + LB.CastTime * (1 - X) + Pyro.CastTime * K) + 0.5f) * 0.12f + 1f);
                }

                float LBrecastInterval = 12 + 0.5f * ((FFB.CastTime * FFB.CastTime * X + Pyro.CastTime * Pyro.CastTime * K) / (FFB.CastTime * X + Pyro.CastTime * K)) / hasteFactor;

                A2 = (FFBcrit - LBcrit) * (LB.CastTime / hasteFactor - FFB.CastTime / hasteFactor - LBrecastInterval) - (FFBcrit - LBcrit) * (FFBcrit - LBcrit) * Pyro.CastTime / hasteFactor / (1 - T8) * H;
                A1 = (FFBcrit - LBcrit) * (LBrecastInterval - LB.CastTime / hasteFactor) + (LB.CastTime / hasteFactor - FFB.CastTime / hasteFactor - LBrecastInterval) * (1 + LBcrit) - Pyro.CastTime / hasteFactor / (1 - T8) * H * 2 * LBcrit * (FFBcrit - LBcrit);
                A0 = (1 + LBcrit) * (LBrecastInterval - LB.CastTime / hasteFactor) - Pyro.CastTime / hasteFactor / (1 - T8) * H * LBcrit * LBcrit;
                if (Math.Abs(A2) < 0.00001)
                {
                    X = -A0 / A1;
                }
                else
                {
                    X = (float)((-A1 - Math.Sqrt(A1 * A1 - 4 * A2 * A0)) / (2 * A2));
                }
                C = LBcrit + X * (FFBcrit - LBcrit);
                K = H * C * C / (1 + C) / (1 - T8);        

            // pyro dot uptime 

            //A := [x * cFB * DFB + (1 - x) * cLB * DLB]  ~ (x * cFB + (1 - x) * cLB)* D[x * FB + (1 - x) * LB]
            //B := [x * (1-cFB) * DFB + (1 - x) * (1-cLB) * DLB] ~ (1 - (x * cFB + (1- x) * cLB)) * D[x * FB + (1 - x) * LB]

            float averageTicks = 0f;
            float k1 = 0;
            float k2 = C * C * H;
            float totalChance = k2;
            float averageCastTime = LB.CastTime + X * (FFB.CastTime - LB.CastTime);
            int n = 2;

            averageTicks += Math.Min((int)(Pyro.CastTime / hasteFactor / 3.0f), 4) * T8;
            averageTicks += Math.Min((int)((Pyro.CastTime + n * averageCastTime) / hasteFactor / 3.0f), 4) * (1 - T8) * k2;

            while ((Pyro.CastTime + n * averageCastTime) / hasteFactor < 12)
            {
                float tmp = k1;
                k1 = k2;
                k2 = C * (1 - C * H) * tmp + (1 - C) * k1;
                totalChance += k2;
                n++;
                averageTicks += Math.Min((int)((Pyro.CastTime + n * averageCastTime) / hasteFactor / 3.0f), 4) * (1 - T8) * k2;
            }
            averageTicks += 4 * (1 - T8) * (1 - totalChance);

            cycle.AddSpell(needsDisplayCalculations, FFB, X);
            cycle.AddSpell(needsDisplayCalculations, LB, 1 - X);
            cycle.AddSpell(needsDisplayCalculations, Pyro, K, averageTicks / 4.0f);
            cycle.CastTime /= hasteFactor;
            cycle.Calculate();
            return cycle;
        }
    }

    public static class ScLBPyro
    {
        public static Cycle GetCycle(bool needsDisplayCalculations, CastingState castingState)
        {
            Cycle cycle = Cycle.New(needsDisplayCalculations, castingState);
            Spell Sc;
            Spell LB;
            Spell Pyro;
            float X;
            float K;
            cycle.Name = "ScLBPyro";
            cycle.ProvidesScorch = (castingState.MageTalents.ImprovedScorch > 0);

            Sc = castingState.GetSpell(SpellId.Scorch);
            LB = castingState.GetSpell(SpellId.LivingBomb);
            Pyro = castingState.GetSpell(SpellId.PyroblastPOMDotUptime);

            float T8 = CalculationOptionsMage.SetBonus4T8ProcRate * castingState.BaseStats.Mage4T8;

            float SCcrit = Sc.CritRate;
            float LBcrit = LB.CritRate;
            float H = castingState.MageTalents.HotStreak / 3.0f;
            if (castingState.MageTalents.Pyroblast == 0) H = 0.0f;
            float A2 = (SCcrit - LBcrit) * (LB.CastTime - Sc.CastTime - 12) - (SCcrit - LBcrit) * (SCcrit - LBcrit) * Pyro.CastTime / (1 - T8) * H;
            float A1 = (SCcrit - LBcrit) * (12 - LB.CastTime) + (LB.CastTime - Sc.CastTime - 12) * (1 + LBcrit) - Pyro.CastTime / (1 - T8) * H * 2 * LBcrit * (SCcrit - LBcrit);
            float A0 = (1 + LBcrit) * (12 - LB.CastTime) - Pyro.CastTime / (1 - T8) * H * LBcrit * LBcrit;
            if (Math.Abs(A2) < 0.00001)
            {
                X = -A0 / A1;
            }
            else
            {
                X = (float)((-A1 - Math.Sqrt(A1 * A1 - 4 * A2 * A0)) / (2 * A2));
            }
            float C = LBcrit + X * (SCcrit - LBcrit);
            K = H * C * C / (1 + C) / (1 - T8);

            float hasteFactor = 1.0f;
            if (castingState.BaseStats.Mage2T10 > 0)
            {
                // p = K / (1 + K)
                // N = (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FB.CastTime * X + LB.CastTime * (1-X) + Pyro.CastTime * K)
                hasteFactor = 1.12f / ((float)Math.Pow(1 - K / (1.0f + K), (5 * 1.12 - Pyro.CastTime) * (1 + K) / (Sc.CastTime * X + LB.CastTime * (1 - X) + Pyro.CastTime * K) + 0.5f) * 0.12f + 1f);
            }

            float LBrecastInterval = 12 + 0.5f * ((Sc.CastTime * Sc.CastTime * X + Pyro.CastTime * Pyro.CastTime * K) / (Sc.CastTime * X + Pyro.CastTime * K)) / hasteFactor;

            A2 = (SCcrit - LBcrit) * (LB.CastTime / hasteFactor - Sc.CastTime / hasteFactor - LBrecastInterval) - (SCcrit - LBcrit) * (SCcrit - LBcrit) * Pyro.CastTime / hasteFactor / (1 - T8) * H;
            A1 = (SCcrit - LBcrit) * (LBrecastInterval - LB.CastTime / hasteFactor) + (LB.CastTime / hasteFactor - Sc.CastTime / hasteFactor - LBrecastInterval) * (1 + LBcrit) - Pyro.CastTime / hasteFactor / (1 - T8) * H * 2 * LBcrit * (SCcrit - LBcrit);
            A0 = (1 + LBcrit) * (LBrecastInterval - LB.CastTime / hasteFactor) - Pyro.CastTime / hasteFactor / (1 - T8) * H * LBcrit * LBcrit;
            if (Math.Abs(A2) < 0.00001)
            {
                X = -A0 / A1;
            }
            else
            {
                X = (float)((-A1 - Math.Sqrt(A1 * A1 - 4 * A2 * A0)) / (2 * A2));
            }
            C = LBcrit + X * (SCcrit - LBcrit);
            K = H * C * C / (1 + C) / (1 - T8);

            // pyro dot uptime 

            //A := [x * cFB * DFB + (1 - x) * cLB * DLB]  ~ (x * cFB + (1 - x) * cLB)* D[x * FB + (1 - x) * LB]
            //B := [x * (1-cFB) * DFB + (1 - x) * (1-cLB) * DLB] ~ (1 - (x * cFB + (1- x) * cLB)) * D[x * FB + (1 - x) * LB]

            float averageTicks = 0f;
            float k1 = 0;
            float k2 = C * C * H;
            float totalChance = k2;
            float averageCastTime = LB.CastTime + X * (Sc.CastTime - LB.CastTime);
            int n = 2;

            averageTicks += Math.Min((int)(Pyro.CastTime / hasteFactor / 3.0f), 4) * T8;
            averageTicks += Math.Min((int)((Pyro.CastTime + n * averageCastTime) / hasteFactor / 3.0f), 4) * (1 - T8) * k2;

            while ((Pyro.CastTime + n * averageCastTime) / hasteFactor < 12)
            {
                float tmp = k1;
                k1 = k2;
                k2 = C * (1 - C * H) * tmp + (1 - C) * k1;
                totalChance += k2;
                n++;
                averageTicks += Math.Min((int)((Pyro.CastTime + n * averageCastTime) / hasteFactor / 3.0f), 4) * (1 - T8) * k2;
            }
            averageTicks += 4 * (1 - T8) * (1 - totalChance);

            cycle.AddSpell(needsDisplayCalculations, Sc, X);
            cycle.AddSpell(needsDisplayCalculations, LB, 1 - X);
            cycle.AddSpell(needsDisplayCalculations, Pyro, K, averageTicks / 4.0f);
            cycle.CastTime /= hasteFactor;
            cycle.Calculate();
            return cycle;
        }
    }

    public static class FFBPyro
    {
        public static Cycle GetCycle(bool needsDisplayCalculations, CastingState castingState)
        {
            Cycle cycle = Cycle.New(needsDisplayCalculations, castingState);
            Spell FFB;
            float K;
            cycle.Name = "FFBPyro";

            FFB = castingState.GetSpell(SpellId.FrostfireBoltFOF);
            Spell Pyro = castingState.GetSpell(SpellId.PyroblastPOMDotUptime);

            // no Pyro
            // FB => no Pyro 1 - c*c/(1+c)
            //    => Pyro c*c/(1+c)
            // Pyro
            // Pyro => no Pyro

            // 1 - c*c/(1+c)
            // FB

            // c*c/(1+c)

            float T8 = CalculationOptionsMage.SetBonus4T8ProcRate * castingState.BaseStats.Mage4T8;

            K = FFB.CritRate * FFB.CritRate / (1.0f + FFB.CritRate) * castingState.MageTalents.HotStreak / 3.0f / (1 - T8);
            if (castingState.MageTalents.Pyroblast == 0) K = 0.0f;

            float hasteFactor = 1.0f;
            if (castingState.BaseStats.Mage2T10 > 0)
            {
                // p = K / (1 + K)
                // N = (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FB.CastTime * X + LB.CastTime * (1-X) + Pyro.CastTime * K)
                hasteFactor = 1.12f / ((float)Math.Pow(1 - K / (1.0f + K), (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FFB.CastTime + Pyro.CastTime * K) + 0.5f) * 0.12f + 1f);
            }

            // pyro dot uptime 

            //A := [x * cFB * DFB + (1 - x) * cLB * DLB]  ~ (x * cFB + (1 - x) * cLB)* D[x * FB + (1 - x) * LB]
            //B := [x * (1-cFB) * DFB + (1 - x) * (1-cLB) * DLB] ~ (1 - (x * cFB + (1- x) * cLB)) * D[x * FB + (1 - x) * LB]

            float averageTicks = 0f;
            float C = FFB.CritRate;
            float H = castingState.MageTalents.HotStreak / 3.0f;
            float k1 = 0;
            float k2 = C * C * H;
            float totalChance = k2;
            float averageCastTime = FFB.CastTime;
            int n = 2;

            averageTicks += Math.Min((int)(Pyro.CastTime / hasteFactor / 3.0f), 4) * T8;
            averageTicks += Math.Min((int)((Pyro.CastTime + n * averageCastTime) / hasteFactor / 3.0f), 4) * (1 - T8) * k2;

            while ((Pyro.CastTime + n * averageCastTime) / hasteFactor < 12)
            {
                float tmp = k1;
                k1 = k2;
                k2 = C * (1 - C * H) * tmp + (1 - C) * k1;
                totalChance += k2;
                n++;
                averageTicks += Math.Min((int)((Pyro.CastTime + n * averageCastTime) / hasteFactor / 3.0f), 4) * (1 - T8) * k2;
            }
            averageTicks += 4 * (1 - T8) * (1 - totalChance);

            cycle.AddSpell(needsDisplayCalculations, FFB, 1);
            cycle.AddSpell(needsDisplayCalculations, Pyro, K, averageTicks / 4.0f);
            cycle.CastTime /= hasteFactor;
            cycle.Calculate();
            return cycle;
        }
    }

    public static class FBScPyro
    {
        public static Cycle GetCycle(bool needsDisplayCalculations, CastingState castingState)
        {
            Cycle cycle = Cycle.New(needsDisplayCalculations, castingState);
            Spell FB;
            Spell Sc;
            Spell Pyro;
            float K;
            float X;
            cycle.Name = "FBScPyro";
            cycle.ProvidesScorch = true;

            FB = castingState.GetSpell(SpellId.Fireball);
            Sc = castingState.GetSpell(SpellId.Scorch);
            Pyro = castingState.GetSpell(SpellId.PyroblastPOMDotUptime);
            Spell PyroSpam = castingState.GetSpell(SpellId.PyroblastPOMSpammed);

            int averageScorchesNeeded = (int)Math.Ceiling(3f / (float)castingState.MageTalents.ImprovedScorch);
            int extraScorches = 0;

            // proportion of time casting non-scorch spells has to be less than gap := (30 - (averageScorchesNeeded + extraScorches)) / (30 - extraScorches)
            // 0 HS charge:
            // FB     => 0 HS charge    (1 - FBcrit) * X
            //        => 1 HS charge    FBcrit * X
            // Sc     => 0 HS charge    (1 - SCcrit) * (1 - X)
            //           1 HS charge    SCcrit * (1 - X)
            // 1 HS charge:
            // FB     => 0 HS charge    (1 - FBcrit) * X + (1 - H) * FBcrit * X
            // FBPyro => 0 HS charge    H * FBcrit * X
            // Sc     => 0 HS charge    (1 - SCcrit) * (1 - X) + (1 - H) * SCcrit * (1 - X)
            // ScPyro => 0 HS charge    H * SCcrit * (1 - X)

            // S0 = FB0a + FB0b + Sc0a + Sc0b
            // S1 = FB1 + FBPyro + Sc1 + ScPyro

            // solve for stationary distribution
            // FB0a = (1 - FBcrit) * X * S0
            // FB0b = FBcrit * X * S0
            // Sc0a = (1 - SCcrit) * (1 - X) * S0
            // Sc0b = SCcrit * (1 - X) * S0
            // FB1 = ((1 - FBcrit) * X + (1 - H) * FBcrit * X) * S1
            // FBPyro = H * FBcrit * X * S1
            // Sc1 = ((1 - SCcrit) * (1 - X) + (1 - H) * SCcrit * (1 - X)) * S1
            // ScPyro = H * SCcrit * (1 - X) * S1

            // S0 + S1 = 1
            // S0 = FB0a + Sc0a + S1
            // S1 = FB0b + Sc0b

            // S1 = (FBcrit * X  + SCcrit * (1 - X)) * S0
            // C := (FBcrit * X  + SCcrit * (1 - X))
            //    = X * (FBcrit - SCcrit) + SCcrit

            // S1 = C * (1 - S1)
            // S1 = C / (1 + C)
            // S0 = 1 / (1 + C)

            // value = (X * value(FB) + (1 - X) * value(Sc)) * 1 / (1 + C) + (X * value(FB) + (1 - X) * value(Sc) + H * (FBcrit * X + SCcrit * (1 - X)) * value(Pyro) / (1 - T8)) * C / (1 + C)
            //         X * value(FB) + (1 - X) * value(Sc) + value(Pyro) * H * C * C / (1 + C) / (1 - T8)

            // (X * time(FB) + time(Pyro) * H * C * C / (1 + C)) / [X * time(FB) + (1 - X) * time(Sc) + time(Pyro) * H * C * C / (1 + C)] = gap
            // (X * time(FB) + time(Pyro) * H * C * C / (1 + C)) = gap * [X * time(FB) + time(Pyro) * H * C * C / (1 + C)] + gap * (1 - X) * time(Sc)
            // (X * time(FB) + time(Pyro) * H * C * C / (1 + C)) * (1 - gap) = gap * (1 - X) * time(Sc)
            // (X * (1 + C) * time(FB) + time(Pyro) * H * C * C) * (1 - gap) = gap * (1 - X) * (1 + C) * time(Sc)
            // (X * (1 + C) * time(FB) + time(Pyro) * H * C * C) * (1 - gap) = gap * (1 + C) * time(Sc) - gap * X * (1 + C) * time(Sc)
            // (X * time(FB) + X * C * time(FB) + time(Pyro) * H * C * C) * (1 - gap) = gap * time(Sc) + gap * C * time(Sc) - gap * X * time(Sc) - gap * X * C * time(Sc)
            // (X * time(FB) + X * (X * (FBcrit - SCcrit) + SCcrit) * time(FB) + time(Pyro) * H * (X * (FBcrit - SCcrit) + SCcrit) * (X * (FBcrit - SCcrit) + SCcrit)) * (1 - gap) = gap * time(Sc) + gap * (X * (FBcrit - SCcrit) + SCcrit) * time(Sc) - gap * X * time(Sc) - gap * X * (X * (FBcrit - SCcrit) + SCcrit) * time(Sc)
            // X * time(FB) * (1 - gap) + X * (X * (FBcrit - SCcrit) + SCcrit) * time(FB) * (1 - gap) + time(Pyro) * H * (X * (FBcrit - SCcrit) + SCcrit) * (X * (FBcrit - SCcrit) + SCcrit) * (1 - gap) = gap * time(Sc) + gap * (X * (FBcrit - SCcrit) + SCcrit) * time(Sc) - gap * X * time(Sc) - gap * X * (X * (FBcrit - SCcrit) + SCcrit) * time(Sc)
            // X * [time(FB) * (1 - gap) + SCcrit * time(FB) * (1 - gap) + 2 * (FBcrit - SCcrit) * SCcrit * time(Pyro) * H * (1 - gap)] + X * X * [(FBcrit - SCcrit) * time(FB) * (1 - gap) + (FBcrit - SCcrit) * (FBcrit - SCcrit) * time(Pyro) * H * (1 - gap)] + SCcrit * SCcrit * time(Pyro) * H * (1 - gap) = gap * time(Sc) + gap * (X * (FBcrit - SCcrit) + SCcrit) * time(Sc) - gap * X * time(Sc) - gap * X * (X * (FBcrit - SCcrit) + SCcrit) * time(Sc)
            // X * X * [(FBcrit - SCcrit) * time(FB) * (1 - gap) + (FBcrit - SCcrit) * (FBcrit - SCcrit) * time(Pyro) * H * (1 - gap) + gap * (FBcrit - SCcrit) * time(Sc)] + X * [time(FB) * (1 - gap) + SCcrit * time(FB) * (1 - gap) + 2 * (FBcrit - SCcrit) * SCcrit * time(Pyro) * H * (1 - gap) - gap * (FBcrit - SCcrit) * time(Sc) + gap * time(Sc) + gap * SCcrit * time(Sc)] + [SCcrit * SCcrit * time(Pyro) * H * (1 - gap) - gap * time(Sc) - gap * SCcrit * time(Sc)] = 0

            // A2 :=
            // (FBcrit - SCcrit) * time(FB) * (1 - gap) + (FBcrit - SCcrit) * (FBcrit - SCcrit) * time(Pyro) * H * (1 - gap) + gap * (FBcrit - SCcrit) * time(Sc)
            // (FBcrit - SCcrit) * [time(FB) * (1 - gap) + (FBcrit - SCcrit) * time(Pyro) * H * (1 - gap) + gap * time(Sc)]
            // (FBcrit - SCcrit) * [time(FB) * (1 - gap) + (FBcrit - SCcrit) * time(Pyro) * H * (1 - gap) - (1 - gap) * time(Sc) + time(Sc)]
            // (FBcrit - SCcrit) * [(1 - gap) * (time(FB) + (FBcrit - SCcrit) * time(Pyro) * H - time(Sc)) + time(Sc)]
            // A1 :=
            // time(FB) * (1 - gap) + SCcrit * time(FB) * (1 - gap) + 2 * (FBcrit - SCcrit) * SCcrit * time(Pyro) * H * (1 - gap) - gap * (FBcrit - SCcrit) * time(Sc) + gap * time(Sc) + gap * SCcrit * time(Sc)
            // time(FB) * [(1 - gap) + SCcrit * (1 - gap)] + time(Pyro) * H * [2 * (FBcrit - SCcrit) * SCcrit * (1 - gap)] + time(Sc) * [gap + gap * SCcrit - gap * (FBcrit - SCcrit)]
            // time(FB) * (1 - gap) * (1 + SCcrit) + time(Pyro) * H * [2 * (FBcrit - SCcrit) * SCcrit * (1 - gap)] + time(Sc) * gap * [1 + 2 * SCcrit - FBcrit]
            // (1 - gap) * [time(FB) * (1 + SCcrit) + time(Pyro) * H * 2 * (FBcrit - SCcrit) * SCcrit - time(Sc) * (1 + 2 * SCcrit - FBcrit)] + time(Sc) * (1 + 2 * SCcrit - FBcrit)
            // A0 :=
            // SCcrit * SCcrit * time(Pyro) * H * (1 - gap) - gap * time(Sc) * (1 + SCcrit)
            // (1 - gap) * (SCcrit * SCcrit * time(Pyro) * H + time(Sc) * (1 + SCcrit)) - time(Sc) * (1 + SCcrit)

            // A2 * X * X + A1 * X + A0 = 0
            // X = [- A1 +/- sqrt[A1 * A1 - 4 * A2 * A0]] / (2 * A2)

            // A1 * A1 - 4 * A2 * A0

            float T8 = CalculationOptionsMage.SetBonus4T8ProcRate * castingState.BaseStats.Mage4T8;

            float gap = (30.0f - (averageScorchesNeeded + extraScorches) * Sc.CastTime) / (30.0f - extraScorches * Sc.CastTime);
            if (castingState.MageTalents.ImprovedScorch == 0)
            {
                cycle.ProvidesScorch = false;
                gap = 1.0f;
            }
            float FBcrit = FB.CritRate;
            float SCcrit = Sc.CritRate;
            float H = castingState.MageTalents.HotStreak / 3.0f;
            if (castingState.MageTalents.Pyroblast == 0) H = 0.0f;
            float A2 = (FBcrit - SCcrit) * FB.CastTime * (1 - gap) + (FBcrit - SCcrit) * (FBcrit - SCcrit) * Pyro.CastTime / (1 - T8) * H * (1 - gap) + gap * (FBcrit - SCcrit) * Sc.CastTime;
            float A1 = FB.CastTime * (1 - gap) * (1 + SCcrit) + Pyro.CastTime / (1 - T8) * H * (2 * (FBcrit - SCcrit) * SCcrit * (1 - gap)) + Sc.CastTime * gap * (1 + 2 * SCcrit - FBcrit);
            float A0 = SCcrit * SCcrit * Pyro.CastTime / (1 - T8) * H * (1 - gap) - gap * Sc.CastTime * (1 + SCcrit);
            if (Math.Abs(A2) < 0.00001)
            {
                X = -A0 / A1;
            }
            else
            {
                X = (float)((-A1 + Math.Sqrt(A1 * A1 - 4 * A2 * A0)) / (2 * A2));
            }
            if (gap == 1.0f) X = 1.0f; //avoid rounding errors
            float C = X * (FBcrit - SCcrit) + SCcrit;
            K = H * C * C / (1 + C) / (1 - T8);

            float hasteFactor = 1.0f;
            if (castingState.BaseStats.Mage2T10 > 0)
            {
                // p = K / (1 + K)
                // N = (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FB.CastTime * X + LB.CastTime * (1-X) + Pyro.CastTime * K)
                hasteFactor = 1.12f / ((float)Math.Pow(1 - K / (1.0f + K), (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FB.CastTime * X + Sc.CastTime * (1 - X) + Pyro.CastTime * K) + 0.5f) * 0.12f + 1f);
            }


            // pyro dot uptime 

            //A := [x * cFB * DFB + (1 - x) * cLB * DLB]  ~ (x * cFB + (1 - x) * cLB)* D[x * FB + (1 - x) * LB]
            //B := [x * (1-cFB) * DFB + (1 - x) * (1-cLB) * DLB] ~ (1 - (x * cFB + (1- x) * cLB)) * D[x * FB + (1 - x) * LB]

            float averageTicks = 0f;
            float k1 = 0;
            float k2 = C * C * H;
            float totalChance = k2;
            float averageCastTime = X * (FB.CastTime - Sc.CastTime) + Sc.CastTime;
            int n = 2;

            averageTicks += Math.Min((int)(Pyro.CastTime / hasteFactor / 3.0f), 4) * T8;
            averageTicks += Math.Min((int)((Pyro.CastTime + n * averageCastTime) / hasteFactor / 3.0f), 4) * (1 - T8) * k2;

            while ((Pyro.CastTime + n * averageCastTime) / hasteFactor < 12)
            {
                float tmp = k1;
                k1 = k2;
                k2 = C * (1 - C * H) * tmp + (1 - C) * k1;
                totalChance += k2;
                n++;
                averageTicks += Math.Min((int)((Pyro.CastTime + n * averageCastTime) / hasteFactor / 3.0f), 4) * (1 - T8) * k2;
            }
            averageTicks += 4 * (1 - T8) * (1 - totalChance);

            cycle.AddSpell(needsDisplayCalculations, FB, X);
            cycle.AddSpell(needsDisplayCalculations, Sc, 1 - X);
            cycle.AddSpell(needsDisplayCalculations, Pyro, K, averageTicks / 4.0f);
            cycle.CastTime /= hasteFactor;
            cycle.Calculate();
            return cycle;
        }
    }

    public static class FBScLBPyro
    {
        public static Cycle GetCycle(bool needsDisplayCalculations, CastingState castingState)
        {
            Cycle cycle = Cycle.New(needsDisplayCalculations, castingState);
            Spell FB;
            Spell Sc;
            Spell LB;
            Spell Pyro;
            float K;
            float X;
            float Y;
            cycle.Name = "FBScLBPyro";
            cycle.ProvidesScorch = true;

            FB = castingState.GetSpell(SpellId.Fireball);
            Sc = castingState.GetSpell(SpellId.Scorch);
            LB = castingState.GetSpell(SpellId.LivingBomb);
            Pyro = castingState.GetSpell(SpellId.PyroblastPOMDotUptime);

            int averageScorchesNeeded = (int)Math.Ceiling(3f / (float)castingState.MageTalents.ImprovedScorch);
            int extraScorches = 0;

            float T8 = CalculationOptionsMage.SetBonus4T8ProcRate * castingState.BaseStats.Mage4T8;
            float C, H, averageCastTime;

                // 3.1 calculations

                // 0 HS charge:
                // FB     => 0 HS charge    (1 - FBcrit) * X
                //        => 1 HS charge    FBcrit * X
                // LB     => 0 HS charge    (1 - LBcrit) * (1 - X - Y)
                //        => 1 HS charge    LBcrit * (1 - X - Y)
                // Sc     => 0 HS charge    (1 - SCcrit) * Y
                //           1 HS charge    SCcrit * Y
                // 1 HS charge:
                // FB     => 0 HS charge    (1 - FBcrit) * X + (1 - H) * FBcrit * X
                // FBPyro => 0 HS charge    H * FBcrit * X
                // LB     => 0 HS charge    (1 - LBcrit) * (1 - X - Y) + (1 - H) * LBcrit * (1 - X - Y)
                // LBPyro => 0 HS charge    H * LBcrit * (1 - X - Y)
                // Sc     => 0 HS charge    (1 - SCcrit) * Y + (1 - H) * SCcrit * Y
                // ScPyro => 0 HS charge    H * SCcrit * Y

                // S0 + S1 = 1
                // C := FBcrit * X + SCcrit * Y + LBcrit * (1 - X - Y)
                // S0 = S0 * (1 - C) + S1
                // S1 = S0 * C
                // 1 - S0 = S0 * C
                // S0 = 1 / (1 + C)
                // S1 = C / (1 + C)

                // value = (X * value(FB) + Y * value(Sc) + (1 - X - Y) * value(LB)) * 1 / (1 + C) + (X * value(FB) + Y * value(Sc) + (1 - X - Y) * value(LB) + H * C * value(Pyro)) * C / (1 + C)
                //         X * value(FB) + Y * value(Sc) + (1 - X - Y) * value(LB) + value(Pyro) * H * C * C / (1 + C) / (1 - T8)

                // (1 - X - Y) * time(LB) / [X * time(FB) + Y * time(Sc) + (1 - X - Y) * time(LB) + time(Pyro) / (1 - T8) * H * C * C / (1 + C)] = time(LB) / 12
                // Z = 1 - X - Y
                // Z * time(LB) / [X * time(FB) + Y * time(Sc) + Z * time(LB) + time(Pyro) / (1 - T8) * H * C * C / (1 + C)] = time(LB) / 12
                // 12 * Z = X * time(FB) + Y * time(Sc) + Z * time(LB) + time(Pyro) * H * C * C / (1 + C) / (1 - T8)
                // T := X * time(FB) + Y * time(Sc) + Z * time(LB) + time(Pyro) * H * C * C / (1 + C) / (1 - T8)
                // 12 * Z = T

                // (X * time(FB) + Z * time(LB) + time(Pyro) * H * C * C / (1 + C) / (1 - T8)) / T = gap
                // (T - Y * time(Sc)) / T = gap
                // T * (1 - gap) = Y * time(Sc)
                // if gap = 1 => Y = 0
                // 12 * Z * (1 - gap) = Y * time(Sc)
                // X = 1 - Y * [1 + time(Sc) / (12 * (1 - gap))]
                // P := 1 + time(Sc) / (12 * (1 - gap))
                // X = 1 - P * Y
                // Z = 1 - X - Y = 1 - 1 + P * Y - Y = Y * (P - 1)
                // C = (FBcrit * X  + SCcrit * Y + LBcrit * Z)
                //     (FBcrit * (1 - P * Y) + SCcrit * Y + LBcrit * Y * (P - 1))
                //     (FBcrit + Y * (SCcrit - FBcrit * P + LBcrit * (P - 1)))
                // C / (1 + C) = (FBcrit + Y * (SCcrit - FBcrit * P + LBcrit * (P - 1))) / (1 + FBcrit + Y * (SCcrit - FBcrit * P + LBcrit * (P - 1)))
                // 12 * Y * (P - 1) = T
                // CY := SCcrit - FBcrit * P + LBcrit * (P - 1)
                // C = FBcrit + Y * CY

                // T =
                // X * time(FB) + Y * time(Sc) + Z * time(LB) + time(Pyro) * H * C * C / (1 + C) / (1 - T8)
                // (1 - P * Y) * time(FB) + Y * time(Sc) + Y * (P - 1) * time(LB) + time(Pyro) * H * C * C / (1 + C) / (1 - T8)
                // time(FB) + Y * [time(Sc) - P * time(FB) + (P - 1) * time(LB)] + time(Pyro) * H * C * C / (1 + C) / (1 - T8)

                // 0 = time(FB) + Y * [time(Sc) - P * time(FB) + (P - 1) * time(LB) - 12 * (P - 1)] + time(Pyro) * H * C * C / (1 + C) / (1 - T8)
                // T1 := time(Sc) - P * time(FB) + (P - 1) * time(LB) - 12 * (P - 1)
                // 0 = time(FB) + Y * T1 + time(Pyro) * H * C * C / (1 + C) / (1 - T8)
                // 0 = time(FB) + C * time(FB) + Y * T1 + Y * C * T1 + time(Pyro) / (1 - T8) * H * C * C
                // 0 = time(FB) + (FBcrit + Y * CY) * time(FB) + Y * T1 + Y * (FBcrit + Y * CY) * T1 + time(Pyro) / (1 - T8) * H * (FBcrit + Y * CY) * (FBcrit + Y * CY)
                // 0 = [time(FB) + FBcrit * time(FB) + time(Pyro) / (1 - T8) * H * FBcrit * FBcrit] + Y * [CY * time(FB) + T1 + FBcrit * T1 + 2 * time(Pyro) / (1 - T8) * H * FBcrit * CY] + Y * Y * [CY * T1 + time(Pyro) / (1 - T8) * H * CY * CY]

                float gap = (30.0f - (averageScorchesNeeded + extraScorches) * Sc.CastTime) / (30.0f - extraScorches * Sc.CastTime);
                if (castingState.MageTalents.ImprovedScorch == 0)
                {
                    cycle.ProvidesScorch = false;
                    gap = 1.0f;
                }
                float hasteFactor = 1.0f;

                if (gap == 1.0f)
                {
                    Y = 0.0f;
                    float FBcrit = FB.CritRate;
                    float LBcrit = LB.CritRate;
                    H = castingState.MageTalents.HotStreak / 3.0f;
                    if (castingState.MageTalents.Pyroblast == 0) H = 0.0f;
                    float A2 = (FBcrit - LBcrit) * (LB.CastTime - FB.CastTime - 12) - (FBcrit - LBcrit) * (FBcrit - LBcrit) * Pyro.CastTime / (1 - T8) * H;
                    float A1 = (FBcrit - LBcrit) * (12 - LB.CastTime) + (LB.CastTime - FB.CastTime - 12) * (1 + LBcrit) - Pyro.CastTime / (1 - T8) * H * 2 * LBcrit * (FBcrit - LBcrit);
                    float A0 = (1 + LBcrit) * (12 - LB.CastTime) - Pyro.CastTime / (1 - T8) * H * LBcrit * LBcrit;
                    if (Math.Abs(A2) < 0.00001)
                    {
                        X = -A0 / A1;
                    }
                    else
                    {
                        X = (float)((-A1 - Math.Sqrt(A1 * A1 - 4 * A2 * A0)) / (2 * A2));
                    }
                    C = LBcrit + X * (FBcrit - LBcrit);
                    K = H * C * C / (1 + C) / (1 - T8);

                    // first order correction for lower LB uptime
                    if (castingState.BaseStats.Mage2T10 > 0)
                    {
                        // p = K / (1 + K)
                        // N = (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FB.CastTime * X + LB.CastTime * (1-X) + Pyro.CastTime * K)
                        hasteFactor = 1.12f / ((float)Math.Pow(1 - K / (1.0f + K), (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FB.CastTime * X + LB.CastTime * (1 - X) + Pyro.CastTime * K) + 0.5f) * 0.12f + 1f);
                    }

                    // LBrecastInterval = 12 + 0.5 * ((time(FB) * X + time(Pyro) * K) / (X + K))
                    float LBrecastInterval = 12 + 0.5f * ((FB.CastTime * FB.CastTime * X + Pyro.CastTime * Pyro.CastTime * K) / (FB.CastTime * X + Pyro.CastTime * K)) / hasteFactor;

                    // now recalculate the spell distribution under the new uptime assumption
                    A2 = (FBcrit - LBcrit) * (LB.CastTime / hasteFactor - FB.CastTime / hasteFactor - LBrecastInterval) - (FBcrit - LBcrit) * (FBcrit - LBcrit) * Pyro.CastTime / hasteFactor / (1 - T8) * H;
                    A1 = (FBcrit - LBcrit) * (LBrecastInterval - LB.CastTime / hasteFactor) + (LB.CastTime / hasteFactor - FB.CastTime / hasteFactor - LBrecastInterval) * (1 + LBcrit) - Pyro.CastTime / hasteFactor / (1 - T8) * H * 2 * LBcrit * (FBcrit - LBcrit);
                    A0 = (1 + LBcrit) * (LBrecastInterval - LB.CastTime / hasteFactor) - Pyro.CastTime / hasteFactor / (1 - T8) * H * LBcrit * LBcrit;
                    if (Math.Abs(A2) < 0.00001)
                    {
                        X = -A0 / A1;
                    }
                    else
                    {
                        X = (float)((-A1 - Math.Sqrt(A1 * A1 - 4 * A2 * A0)) / (2 * A2));
                    }
                    C = LBcrit + X * (FBcrit - LBcrit);
                    averageCastTime = LB.CastTime + X * (FB.CastTime - LB.CastTime);
                    K = H * C * C / (1 + C) / (1 - T8);
                }
                else
                {
                    float P = 1.0f + Sc.CastTime / (12.0f * (1.0f - gap));
                    float FBcrit = FB.CritRate;
                    float SCcrit = Sc.CritRate;
                    float LBcrit = LB.CritRate;
                    H = castingState.MageTalents.HotStreak / 3.0f;
                    if (castingState.MageTalents.Pyroblast == 0) H = 0.0f;
                    float T1 = Sc.CastTime - P * FB.CastTime + (P - 1) * LB.CastTime - 12 * (P - 1);
                    float CY = SCcrit - FBcrit * P + LBcrit * (P - 1);

                    float A2 = CY * T1 + Pyro.CastTime / (1 - T8) * H * CY * CY;
                    float A1 = CY * FB.CastTime + T1 + FBcrit * T1 + 2 * Pyro.CastTime / (1 - T8) * H * FBcrit * CY;
                    float A0 = FB.CastTime + FBcrit * FB.CastTime + Pyro.CastTime / (1 - T8) * H * FBcrit * FBcrit;
                    if (Math.Abs(A2) < 0.00001)
                    {
                        Y = -A0 / A1;
                    }
                    else
                    {
                        Y = (float)((-A1 - Math.Sqrt(A1 * A1 - 4 * A2 * A0)) / (2 * A2));
                    }
                    X = 1 - P * Y;
                    C = (FBcrit * X + SCcrit * Y + LBcrit * (1 - X - Y));
                    K = H * C * C / (1 + C) / (1 - T8);

                    if (castingState.BaseStats.Mage2T10 > 0)
                    {
                        // p = K / (1 + K)
                        // N = (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FB.CastTime * X + LB.CastTime * (1-X) + Pyro.CastTime * K)
                        hasteFactor = 1.12f / ((float)Math.Pow(1 - K / (1.0f + K), (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FB.CastTime * X + Sc.CastTime * Y + LB.CastTime * (1 - X - Y) + Pyro.CastTime * K) + 0.5f) * 0.12f + 1f);
                    }

                    float LBrecastInterval = 12 + 0.5f * ((FB.CastTime * FB.CastTime * X + Sc.CastTime * Sc.CastTime * Y + Pyro.CastTime * Pyro.CastTime * K) / (FB.CastTime * X + Sc.CastTime * Y + Pyro.CastTime * K)) / hasteFactor;

                    P = 1.0f + Sc.CastTime / hasteFactor / (LBrecastInterval * (1.0f - gap));
                    T1 = Sc.CastTime / hasteFactor - P * FB.CastTime / hasteFactor + (P - 1) * LB.CastTime / hasteFactor - LBrecastInterval * (P - 1);
                    CY = SCcrit - FBcrit * P + LBcrit * (P - 1);

                    A2 = CY * T1 + Pyro.CastTime / hasteFactor / (1 - T8) * H * CY * CY;
                    A1 = CY * FB.CastTime / hasteFactor + T1 + FBcrit * T1 + 2 * Pyro.CastTime / hasteFactor / (1 - T8) * H * FBcrit * CY;
                    A0 = FB.CastTime / hasteFactor + FBcrit * FB.CastTime / hasteFactor + Pyro.CastTime / hasteFactor / (1 - T8) * H * FBcrit * FBcrit;
                    if (Math.Abs(A2) < 0.00001)
                    {
                        Y = -A0 / A1;
                    }
                    else
                    {
                        Y = (float)((-A1 - Math.Sqrt(A1 * A1 - 4 * A2 * A0)) / (2 * A2));
                    }
                    X = 1 - P * Y;
                    C = (FBcrit * X + SCcrit * Y + LBcrit * (1 - X - Y));
                    averageCastTime = (FB.CastTime * X + Sc.CastTime * Y + LB.CastTime * (1 - X - Y));
                    K = H * C * C / (1 + C) / (1 - T8);
                }

            // pyro dot uptime 

            //A := [x * cFB * DFB + (1 - x) * cLB * DLB]  ~ (x * cFB + (1 - x) * cLB)* D[x * FB + (1 - x) * LB]
            //B := [x * (1-cFB) * DFB + (1 - x) * (1-cLB) * DLB] ~ (1 - (x * cFB + (1- x) * cLB)) * D[x * FB + (1 - x) * LB]

            float averageTicks = 0f;
            float k1 = 0;
            float k2 = C * C * H;
            float totalChance = k2;
            int n = 2;

            averageTicks += Math.Min((int)(Pyro.CastTime / hasteFactor / 3.0f), 4) * T8;
            averageTicks += Math.Min((int)((Pyro.CastTime + n * averageCastTime) / hasteFactor / 3.0f), 4) * (1 - T8) * k2;

            while ((Pyro.CastTime + n * averageCastTime) / hasteFactor < 12)
            {
                float tmp = k1;
                k1 = k2;
                k2 = C * (1 - C * H) * tmp + (1 - C) * k1;
                totalChance += k2;
                n++;
                averageTicks += Math.Min((int)((Pyro.CastTime + n * averageCastTime) / hasteFactor / 3.0f), 4) * (1 - T8) * k2;
            }
            averageTicks += 4 * (1 - T8) * (1 - totalChance);

            cycle.AddSpell(needsDisplayCalculations, FB, X);
            cycle.AddSpell(needsDisplayCalculations, Sc, Y);
            cycle.AddSpell(needsDisplayCalculations, LB, 1 - X - Y);
            cycle.AddSpell(needsDisplayCalculations, Pyro, K, averageTicks / 4.0f);
            cycle.CastTime /= hasteFactor;
            cycle.Calculate();
            return cycle;
        }
    }

    public static class FFBScLBPyro
    {
        public static Cycle GetCycle(bool needsDisplayCalculations, CastingState castingState)
        {
            Cycle cycle = Cycle.New(needsDisplayCalculations, castingState);
            Spell FFB;
            Spell Sc;
            Spell LB;
            Spell Pyro;
            float K;
            float X;
            float Y;
            cycle.Name = "FFBScLBPyro";
            cycle.ProvidesScorch = true;

            FFB = castingState.GetSpell(SpellId.FrostfireBolt);
            Sc = castingState.GetSpell(SpellId.Scorch);
            LB = castingState.GetSpell(SpellId.LivingBomb);
            Pyro = castingState.GetSpell(SpellId.PyroblastPOMDotUptime);

            int averageScorchesNeeded = (int)Math.Ceiling(3f / (float)castingState.MageTalents.ImprovedScorch);
            int extraScorches = 0;

            float T8 = CalculationOptionsMage.SetBonus4T8ProcRate * castingState.BaseStats.Mage4T8;

            float gap = (30.0f - (averageScorchesNeeded + extraScorches) * Sc.CastTime) / (30.0f - extraScorches * Sc.CastTime);
            if (castingState.MageTalents.ImprovedScorch == 0)
            {
                cycle.ProvidesScorch = false;
                gap = 1.0f;
            }
            float hasteFactor = 1.0f;
            float C, H, averageCastTime;

                if (gap == 1.0f)
                {
                    Y = 0.0f;
                    float FFBcrit = FFB.CritRate;
                    float LBcrit = LB.CritRate;
                    H = castingState.MageTalents.HotStreak / 3.0f;
                    if (castingState.MageTalents.Pyroblast == 0) H = 0.0f;
                    float A2 = (FFBcrit - LBcrit) * (LB.CastTime - FFB.CastTime - 12) - (FFBcrit - LBcrit) * (FFBcrit - LBcrit) * Pyro.CastTime / (1 - T8) * H;
                    float A1 = (FFBcrit - LBcrit) * (12 - LB.CastTime) + (LB.CastTime - FFB.CastTime - 12) * (1 + LBcrit) - Pyro.CastTime / (1 - T8) * H * 2 * LBcrit * (FFBcrit - LBcrit);
                    float A0 = (1 + LBcrit) * (12 - LB.CastTime) - Pyro.CastTime / (1 - T8) * H * LBcrit * LBcrit;
                    if (Math.Abs(A2) < 0.00001)
                    {
                        X = -A0 / A1;
                    }
                    else
                    {
                        X = (float)((-A1 - Math.Sqrt(A1 * A1 - 4 * A2 * A0)) / (2 * A2));
                    }
                    C = LBcrit + X * (FFBcrit - LBcrit);
                    K = H * C * C / (1 + C) / (1 - T8);

                    if (castingState.BaseStats.Mage2T10 > 0)
                    {
                        // p = K / (1 + K)
                        // N = (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FB.CastTime * X + LB.CastTime * (1-X) + Pyro.CastTime * K)
                        hasteFactor = 1.12f / ((float)Math.Pow(1 - K / (1.0f + K), (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FFB.CastTime * X + LB.CastTime * (1 - X) + Pyro.CastTime * K) + 0.5f) * 0.12f + 1f);
                    }

                    float LBrecastInterval = 12 + 0.5f * ((FFB.CastTime * FFB.CastTime * X + Pyro.CastTime * Pyro.CastTime * K) / (FFB.CastTime * X + Pyro.CastTime * K)) / hasteFactor;

                    A2 = (FFBcrit - LBcrit) * (LB.CastTime / hasteFactor - FFB.CastTime / hasteFactor - LBrecastInterval) - (FFBcrit - LBcrit) * (FFBcrit - LBcrit) * Pyro.CastTime / hasteFactor / (1 - T8) * H;
                    A1 = (FFBcrit - LBcrit) * (LBrecastInterval - LB.CastTime / hasteFactor) + (LB.CastTime / hasteFactor - FFB.CastTime / hasteFactor - LBrecastInterval) * (1 + LBcrit) - Pyro.CastTime / hasteFactor / (1 - T8) * H * 2 * LBcrit * (FFBcrit - LBcrit);
                    A0 = (1 + LBcrit) * (LBrecastInterval - LB.CastTime / hasteFactor) - Pyro.CastTime / hasteFactor / (1 - T8) * H * LBcrit * LBcrit;
                    if (Math.Abs(A2) < 0.00001)
                    {
                        X = -A0 / A1;
                    }
                    else
                    {
                        X = (float)((-A1 - Math.Sqrt(A1 * A1 - 4 * A2 * A0)) / (2 * A2));
                    }
                    C = LBcrit + X * (FFBcrit - LBcrit);
                    averageCastTime = LB.CastTime + X * (FFB.CastTime - LB.CastTime);
                    K = H * C * C / (1 + C) / (1 - T8);
                }
                else
                {
                    float P = 1.0f + Sc.CastTime / (12.0f * (1.0f - gap));
                    float FFBcrit = FFB.CritRate;
                    float SCcrit = Sc.CritRate;
                    float LBcrit = LB.CritRate;
                    H = castingState.MageTalents.HotStreak / 3.0f;
                    if (castingState.MageTalents.Pyroblast == 0) H = 0.0f;
                    float T1 = Sc.CastTime - P * FFB.CastTime + (P - 1) * LB.CastTime - 12 * (P - 1);
                    float CY = SCcrit - FFBcrit * P + LBcrit * (P - 1);

                    float A2 = CY * T1 + Pyro.CastTime / (1 - T8) * H * CY * CY;
                    float A1 = CY * FFB.CastTime + T1 + FFBcrit * T1 + 2 * Pyro.CastTime / (1 - T8) * H * FFBcrit * CY;
                    float A0 = FFB.CastTime + FFBcrit * FFB.CastTime + Pyro.CastTime / (1 - T8) * H * FFBcrit * FFBcrit;
                    if (Math.Abs(A2) < 0.00001)
                    {
                        Y = -A0 / A1;
                    }
                    else
                    {
                        Y = (float)((-A1 - Math.Sqrt(A1 * A1 - 4 * A2 * A0)) / (2 * A2));
                    }
                    X = 1 - P * Y;
                    C = (FFBcrit * X + SCcrit * Y + LBcrit * (1 - X - Y));
                    K = H * C * C / (1 + C) / (1 - T8);

                    if (castingState.BaseStats.Mage2T10 > 0)
                    {
                        // p = K / (1 + K)
                        // N = (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FB.CastTime * X + LB.CastTime * (1-X) + Pyro.CastTime * K)
                        hasteFactor = 1.12f / ((float)Math.Pow(1 - K / (1.0f + K), (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FFB.CastTime * X + Sc.CastTime * Y + LB.CastTime * (1 - X - Y) + Pyro.CastTime * K) + 0.5f) * 0.12f + 1f);
                    }

                    float LBrecastInterval = 12 + 0.5f * ((FFB.CastTime * FFB.CastTime * X + Sc.CastTime * Sc.CastTime * Y + Pyro.CastTime * Pyro.CastTime * K) / (FFB.CastTime * X + Sc.CastTime * Y + Pyro.CastTime * K)) / hasteFactor;

                    P = 1.0f + Sc.CastTime / hasteFactor / (LBrecastInterval * (1.0f - gap));
                    T1 = Sc.CastTime / hasteFactor - P * FFB.CastTime / hasteFactor + (P - 1) * LB.CastTime / hasteFactor - LBrecastInterval * (P - 1);
                    CY = SCcrit - FFBcrit * P + LBcrit * (P - 1);

                    A2 = CY * T1 + Pyro.CastTime / hasteFactor / (1 - T8) * H * CY * CY;
                    A1 = CY * FFB.CastTime / hasteFactor + T1 + FFBcrit * T1 + 2 * Pyro.CastTime / hasteFactor / (1 - T8) * H * FFBcrit * CY;
                    A0 = FFB.CastTime / hasteFactor + FFBcrit * FFB.CastTime / hasteFactor + Pyro.CastTime / hasteFactor / (1 - T8) * H * FFBcrit * FFBcrit;
                    if (Math.Abs(A2) < 0.00001)
                    {
                        Y = -A0 / A1;
                    }
                    else
                    {
                        Y = (float)((-A1 - Math.Sqrt(A1 * A1 - 4 * A2 * A0)) / (2 * A2));
                    }
                    X = 1 - P * Y;
                    C = (FFBcrit * X + SCcrit * Y + LBcrit * (1 - X - Y));
                    averageCastTime = (FFB.CastTime * X + Sc.CastTime * Y + LB.CastTime * (1 - X - Y));
                    K = H * C * C / (1 + C) / (1 - T8);
                }       

            // pyro dot uptime 

            //A := [x * cFB * DFB + (1 - x) * cLB * DLB]  ~ (x * cFB + (1 - x) * cLB)* D[x * FB + (1 - x) * LB]
            //B := [x * (1-cFB) * DFB + (1 - x) * (1-cLB) * DLB] ~ (1 - (x * cFB + (1- x) * cLB)) * D[x * FB + (1 - x) * LB]

            float averageTicks = 0f;
            float k1 = 0;
            float k2 = C * C * H;
            float totalChance = k2;
            int n = 2;

            averageTicks += Math.Min((int)(Pyro.CastTime / hasteFactor / 3.0f), 4) * T8;
            averageTicks += Math.Min((int)((Pyro.CastTime + n * averageCastTime) / hasteFactor / 3.0f), 4) * (1 - T8) * k2;

            while ((Pyro.CastTime + n * averageCastTime) / hasteFactor < 12)
            {
                float tmp = k1;
                k1 = k2;
                k2 = C * (1 - C * H) * tmp + (1 - C) * k1;
                totalChance += k2;
                n++;
                averageTicks += Math.Min((int)((Pyro.CastTime + n * averageCastTime) / hasteFactor / 3.0f), 4) * (1 - T8) * k2;
            }
            averageTicks += 4 * (1 - T8) * (1 - totalChance);

            cycle.AddSpell(needsDisplayCalculations, FFB, X);
            cycle.AddSpell(needsDisplayCalculations, Sc, Y);
            cycle.AddSpell(needsDisplayCalculations, LB, 1 - X - Y);
            cycle.AddSpell(needsDisplayCalculations, Pyro, K, averageTicks / 4.0f);
            cycle.CastTime /= hasteFactor;
            cycle.Calculate();
            return cycle;
        }
    }

    public static class FFBScPyro
    {
        public static Cycle GetCycle(bool needsDisplayCalculations, CastingState castingState)
        {
            Cycle cycle = Cycle.New(needsDisplayCalculations, castingState);
            Spell FFB;
            Spell Sc;
            Spell Pyro;
            float K;
            float X;
            cycle.Name = "FFBScPyro";
            cycle.ProvidesScorch = true;

            FFB = castingState.GetSpell(SpellId.FrostfireBoltFOF);
            Sc = castingState.GetSpell(SpellId.Scorch);
            Pyro = castingState.GetSpell(SpellId.PyroblastPOMDotUptime);

            int averageScorchesNeeded = (int)Math.Ceiling(3f / (float)castingState.MageTalents.ImprovedScorch);
            int extraScorches = 0;

            float T8 = CalculationOptionsMage.SetBonus4T8ProcRate * castingState.BaseStats.Mage4T8;

            float gap = (30.0f - (averageScorchesNeeded + extraScorches) * Sc.CastTime) / (30.0f - extraScorches * Sc.CastTime);
            if (castingState.MageTalents.ImprovedScorch == 0)
            {
                cycle.ProvidesScorch = false;
                gap = 1.0f;
            }
            float FFBcrit = FFB.CritRate;
            float SCcrit = Sc.CritRate;
            float H = castingState.MageTalents.HotStreak / 3.0f;
            if (castingState.MageTalents.Pyroblast == 0) H = 0.0f;
            float A2 = (FFBcrit - SCcrit) * FFB.CastTime * (1 - gap) + (FFBcrit - SCcrit) * (FFBcrit - SCcrit) * Pyro.CastTime / (1 - T8) * H * (1 - gap) + gap * (FFBcrit - SCcrit) * Sc.CastTime;
            float A1 = FFB.CastTime * (1 - gap) * (1 + SCcrit) + Pyro.CastTime / (1 - T8) * H * (2 * (FFBcrit - SCcrit) * SCcrit * (1 - gap)) + Sc.CastTime * gap * (1 + 2 * SCcrit - FFBcrit);
            float A0 = SCcrit * SCcrit * Pyro.CastTime / (1 - T8) * H * (1 - gap) - gap * Sc.CastTime * (1 + SCcrit);
            if (Math.Abs(A2) < 0.00001)
            {
                X = -A0 / A1;
            }
            else
            {
                X = (float)((-A1 + Math.Sqrt(A1 * A1 - 4 * A2 * A0)) / (2 * A2));
            }
            if (gap == 1.0f) X = 1.0f; //avoid rounding errors
            float C = X * (FFBcrit - SCcrit) + SCcrit;
            K = H * C * C / (1 + C) / (1 - T8);

            float hasteFactor = 1.0f;
            if (castingState.BaseStats.Mage2T10 > 0)
            {
                // p = K / (1 + K)
                // N = (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FB.CastTime * X + LB.CastTime * (1-X) + Pyro.CastTime * K)
                hasteFactor = 1.12f / ((float)Math.Pow(1 - K / (1.0f + K), (5 * 1.12 - Pyro.CastTime) * (1 + K) / (FFB.CastTime * X + Sc.CastTime * (1 - X) + Pyro.CastTime * K) + 0.5f) * 0.12f + 1f);
            }


            // pyro dot uptime 

            //A := [x * cFB * DFB + (1 - x) * cLB * DLB]  ~ (x * cFB + (1 - x) * cLB)* D[x * FB + (1 - x) * LB]
            //B := [x * (1-cFB) * DFB + (1 - x) * (1-cLB) * DLB] ~ (1 - (x * cFB + (1- x) * cLB)) * D[x * FB + (1 - x) * LB]

            float averageTicks = 0f;
            float k1 = 0;
            float k2 = C * C * H;
            float totalChance = k2;
            float averageCastTime = X * (FFB.CastTime - Sc.CastTime) + Sc.CastTime;
            int n = 2;

            averageTicks += Math.Min((int)(Pyro.CastTime / hasteFactor / 3.0f), 4) * T8;
            averageTicks += Math.Min((int)((Pyro.CastTime + n * averageCastTime) / hasteFactor / 3.0f), 4) * (1 - T8) * k2;

            while ((Pyro.CastTime + n * averageCastTime) / hasteFactor < 12)
            {
                float tmp = k1;
                k1 = k2;
                k2 = C * (1 - C * H) * tmp + (1 - C) * k1;
                totalChance += k2;
                n++;
                averageTicks += Math.Min((int)((Pyro.CastTime + n * averageCastTime) / hasteFactor / 3.0f), 4) * (1 - T8) * k2;
            }
            averageTicks += 4 * (1 - T8) * (1 - totalChance);

            cycle.AddSpell(needsDisplayCalculations, FFB, X);
            cycle.AddSpell(needsDisplayCalculations, Sc, 1 - X);
            cycle.AddSpell(needsDisplayCalculations, Pyro, K, averageTicks / 4.0f);
            cycle.CastTime /= hasteFactor;
            cycle.Calculate();
            return cycle;
        }
    }

    public class FireCycleGenerator : CycleGenerator
    {
        private class State : CycleState
        {
            public float ScorchDuration { get; set; }
            public float LivingBombDuration { get; set; }
            public int HotStreakCount { get; set; }
            public float PyroDuration { get; set; }
            public bool PyroRegistered { get; set; }
            public float Tier10TwoPieceDuration { get; set; }
        }

        public Spell[] Pyro, FB, LB, Sc;

        private float HS;
        private float T8;
        private bool T10;

        private bool maintainScorch = false;
        private bool livingBombGlyph;

        private float LBDotCritRate;

        public FireCycleGenerator(CastingState castingState)
        {
            FB = new Spell[2];
            Sc = new Spell[2];
            LB = new Spell[2];
            Pyro = new Spell[2];

            FB[0] = castingState.GetSpell(SpellId.Fireball);
            Sc[0] = castingState.GetSpell(SpellId.Scorch);
            LB[0] = castingState.GetSpell(SpellId.LivingBomb);
            Pyro[0] = castingState.GetSpell(SpellId.PyroblastPOM); // does not account for dot uptime
            FB[0].Label = "Fireball";
            Sc[0].Label = "Scorch";
            LB[0].Label = "Living Bomb";
            Pyro[0].Label = "Pyroblast";

            FB[1] = castingState.Tier10TwoPieceState.GetSpell(SpellId.Fireball);
            Sc[1] = castingState.Tier10TwoPieceState.GetSpell(SpellId.Scorch);
            LB[1] = castingState.Tier10TwoPieceState.GetSpell(SpellId.LivingBomb);
            Pyro[1] = castingState.Tier10TwoPieceState.GetSpell(SpellId.PyroblastPOM); // does not account for dot uptime
            FB[1].Label = "2T10:Fireball";
            Sc[1].Label = "2T10:Scorch";
            LB[1].Label = "2T10:Living Bomb";
            Pyro[1].Label = "2T10:Pyroblast";

            HS = castingState.MageTalents.HotStreak / 3.0f;
            T8 = CalculationOptionsMage.SetBonus4T8ProcRate * castingState.BaseStats.Mage4T8;
            //maintainScorch = castingState.CalculationOptions.MaintainScorch;
            livingBombGlyph = castingState.MageTalents.GlyphOfLivingBomb;
            LBDotCritRate = castingState.FireCritRate;
            T10 = castingState.BaseStats.Mage2T10 > 0;

            GenerateStateDescription();
        }

        protected override CycleState GetInitialState()
        {
            return GetState(0.0f, 0.0f, 0, 0.0f, false, 0.0f);
        }

        protected override List<CycleControlledStateTransition> GetStateTransitions(CycleState state)
        {
            State s = (State)state;
            List<CycleControlledStateTransition> list = new List<CycleControlledStateTransition>();
            Spell Sc = this.Sc[s.Tier10TwoPieceDuration > 0 ? 1 : 0];
            Spell FB = this.FB[s.Tier10TwoPieceDuration > 0 ? 1 : 0];
            Spell LB = this.LB[s.Tier10TwoPieceDuration > 0 ? 1 : 0];
            Spell Pyro = this.Pyro[s.Tier10TwoPieceDuration > 0 ? 1 : 0];
            if (maintainScorch && s.ScorchDuration < Sc.CastTime)
            {
                // LB explosion and/or ticks can happen during the cast
                // account for all combinations of hot streak interaction
                float castTime = Sc.CastTime;
                int ticksLeft = Math.Max(0, (int)(s.LivingBombDuration / 3.0f));
                int ticksLeftAfter = Math.Max(0, (int)((s.LivingBombDuration - castTime) / 3.0f));
                int hotStreakEvents = ticksLeft - ticksLeftAfter;
                //if (!livingBombGlyph)
                {
                    hotStreakEvents = 0;
                }
                bool explosion = false;
                if (ticksLeft > 0 && ticksLeftAfter == 0)
                {
                    explosion = true;
                    hotStreakEvents++;
                }
                for (int i = 0; i < (1 << (hotStreakEvents + 1)); i++)
                {
                    int k = i;
                    int hsc = s.HotStreakCount;
                    float pd = Math.Max(0f, s.PyroDuration - castTime);
                    bool pr = s.PyroDuration > castTime;
                    float chance = 1.0f;
                    for (int j = 0; j < hotStreakEvents; j++)
                    {
                        float crit = (j == hotStreakEvents - 1 && explosion) ? LB.CritRate : LBDotCritRate;
                        if (k % 2 == 1)
                        {
                            chance *= crit;
                            if (hsc == 1)
                            {
                                pd = 10.0f - (s.LivingBombDuration - ticksLeft * 3);
                                pr = true;
                            }
                            hsc = (hsc + 1) % 2;
                        }
                        else
                        {
                            chance *= (1 - crit);
                            hsc = 0;
                        }
                        k = k >> 1;
                    }
                    if (k % 2 == 1)
                    {
                        chance *= Sc.CritRate;
                        if (hsc == 1)
                        {
                            pd = 10.0f;
                        }
                        hsc = (hsc + 1) % 2;
                    }
                    else
                    {
                        chance *= (1 - Sc.CritRate);
                        hsc = 0;
                    }
                    list.Add(new CycleControlledStateTransition()
                    {
                        Spell = Sc,
                        TargetState = GetState(
                            30.0f + Sc.CastTime,
                            Math.Max(0f, s.LivingBombDuration - castTime),
                            hsc,
                            pd,
                            pr,
                            Math.Max(0.0f, s.Tier10TwoPieceDuration - Sc.CastTime)
                        ),
                        TransitionProbability = chance
                    });
                }
            }
            else if (s.LivingBombDuration <= 0f)
            {
                list.Add(new CycleControlledStateTransition()
                {
                    Spell = LB,
                    TargetState = GetState(
                        Math.Max(0f, s.ScorchDuration - LB.CastTime),
                        12f - LB.CastTime,
                        s.HotStreakCount,
                        Math.Max(0f, s.PyroDuration - LB.CastTime),
                        s.PyroDuration > LB.CastTime,
                        Math.Max(0.0f, s.Tier10TwoPieceDuration - LB.CastTime)
                    ),
                    TransitionProbability = 1
                });
            }
            else if (s.PyroRegistered)
            {
                float castTime = Pyro.CastTime;
                int ticksLeft = Math.Max(0, (int)(s.LivingBombDuration / 3.0f));
                int ticksLeftAfter = Math.Max(0, (int)((s.LivingBombDuration - castTime) / 3.0f));
                int hotStreakEvents = ticksLeft - ticksLeftAfter;
                //if (!livingBombGlyph)
                {
                    hotStreakEvents = 0;
                }
                bool explosion = false;
                if (ticksLeft > 0 && ticksLeftAfter == 0)
                {
                    explosion = true;
                    hotStreakEvents++;
                }
                for (int i = 0; i < (1 << hotStreakEvents); i++)
                {
                    int k = i;
                    int hsc = s.HotStreakCount;
                    float pd1 = Math.Max(0f, s.PyroDuration - castTime);
                    bool pr1 = s.PyroDuration > castTime;
                    float pd2 = 0.0f;
                    bool pr2 = false;
                    float chance = 1.0f;
                    for (int j = 0; j < hotStreakEvents; j++)
                    {
                        float crit = (j == hotStreakEvents - 1 && explosion) ? LB.CritRate : LBDotCritRate;
                        if (k % 2 == 1)
                        {
                            chance *= crit;
                            if (hsc == 1)
                            {
                                pd1 = 10.0f - (s.LivingBombDuration - ticksLeft * 3);
                                pr1 = true;
                                pd2 = 10.0f - (s.LivingBombDuration - ticksLeft * 3);
                                pr2 = true;
                            }
                            hsc = (hsc + 1) % 2;
                        }
                        else
                        {
                            chance *= (1 - crit);
                            hsc = 0;
                        }
                        k = k >> 1;
                    }
                    if (T8 > 0)
                    {
                        list.Add(new CycleControlledStateTransition()
                        {
                            Spell = Pyro,
                            TargetState = GetState(
                                Math.Max(0f, s.ScorchDuration - castTime),
                                Math.Max(0f, s.LivingBombDuration - castTime),
                                hsc,
                                pd1,
                                pr1,
                                T10 ? Math.Max(0.0f, 5.0f - Pyro.CastTime) : 0f
                            ),
                            TransitionProbability = chance * T8
                        });
                    }
                    list.Add(new CycleControlledStateTransition()
                    {
                        Spell = Pyro,
                        TargetState = GetState(
                            Math.Max(0f, s.ScorchDuration - castTime),
                            Math.Max(0f, s.LivingBombDuration - castTime),
                            hsc,
                            pd2,
                            pr2,
                            T10 ? Math.Max(0.0f, 5.0f - Pyro.CastTime) : 0f
                        ),
                        TransitionProbability = chance * (1 - T8)
                    });
                }
            }
            else
            {
                float castTime = FB.CastTime;
                int ticksLeft = Math.Max(0, (int)(s.LivingBombDuration / 3.0f));
                int ticksLeftAfter = Math.Max(0, (int)((s.LivingBombDuration - castTime) / 3.0f));
                int hotStreakEvents = ticksLeft - ticksLeftAfter;
                //if (!livingBombGlyph)
                {
                    hotStreakEvents = 0;
                }
                bool explosion = false;
                if (ticksLeft > 0 && ticksLeftAfter == 0)
                {
                    explosion = true;
                    hotStreakEvents++;
                }
                for (int i = 0; i < (1 << (hotStreakEvents + 1)); i++)
                {
                    int k = i;
                    int hsc = s.HotStreakCount;
                    float pd = Math.Max(0f, s.PyroDuration - castTime);
                    bool pr = s.PyroDuration > castTime;
                    float chance = 1.0f;
                    for (int j = 0; j < hotStreakEvents; j++)
                    {
                        float crit = (j == hotStreakEvents - 1 && explosion) ? LB.CritRate : LBDotCritRate;
                        if (k % 2 == 1)
                        {
                            chance *= crit;
                            if (hsc == 1)
                            {
                                pd = 10.0f - (s.LivingBombDuration - ticksLeft * 3);
                                pr = true;
                            }
                            hsc = (hsc + 1) % 2;
                        }
                        else
                        {
                            chance *= (1 - crit);
                            hsc = 0;
                        }
                        k = k >> 1;
                    }
                    if (k % 2 == 1)
                    {
                        chance *= FB.CritRate;
                        if (hsc == 1)
                        {
                            pd = 10.0f;
                        }
                        hsc = (hsc + 1) % 2;
                    }
                    else
                    {
                        chance *= (1 - FB.CritRate);
                        hsc = 0;
                    }
                    list.Add(new CycleControlledStateTransition()
                    {
                        Spell = FB,
                        TargetState = GetState(
                            Math.Max(0f, s.ScorchDuration - castTime),
                            Math.Max(0f, s.LivingBombDuration - castTime),
                            hsc,
                            pd,
                            pr,
                            Math.Max(0.0f, s.Tier10TwoPieceDuration - FB.CastTime)
                        ),
                        TransitionProbability = chance
                    });
                }
            }

            return list;
        }

        private Dictionary<string, State> stateDictionary = new Dictionary<string, State>();

        private State GetState(float scorchDuration, float livingBombDuration, int hotStreakCount, float pyroDuration, bool pyroRegistered, float tier10TwoPieceDuration)
        {
            string name = string.Format("Sc{0},LB{1},HS{2},Pyro{3}{4},2T10={5}", scorchDuration, livingBombDuration, hotStreakCount, pyroDuration, pyroRegistered ? "+" : "-", tier10TwoPieceDuration);
            State state;
            if (!stateDictionary.TryGetValue(name, out state))
            {
                state = new State() { Name = name, ScorchDuration = scorchDuration, LivingBombDuration = livingBombDuration, HotStreakCount = hotStreakCount, PyroDuration = pyroDuration, PyroRegistered = pyroRegistered, Tier10TwoPieceDuration = tier10TwoPieceDuration };
                stateDictionary[name] = state;
            }
            return state;
        }

        protected override bool CanStatesBeDistinguished(CycleState state1, CycleState state2)
        {
            State a = (State)state1;
            State b = (State)state2;
            return (a.ScorchDuration != b.ScorchDuration || a.LivingBombDuration != b.LivingBombDuration || a.HotStreakCount != b.HotStreakCount || a.PyroDuration != b.PyroDuration || a.PyroRegistered != b.PyroRegistered || a.Tier10TwoPieceDuration != b.Tier10TwoPieceDuration);
        }
    }
}
