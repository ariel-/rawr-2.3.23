using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr
{
    public struct WeightedStat
    {
        public float Chance;
        public float Value;
    }

    public partial class SpecialEffect
    {
        private delegate float Ibeta(int a, float b, float x);

        const int maxRecursionDepth = 20;

        private class Parameters
        {
            public SpecialEffect[] effects; /*in*/
            public float[] d; /*in*/
            public float[] p; /*in*/
            public float[] c; /*in*/
            public float[] o; /*in*/
            public float[] k; /*in*/
            public float[] v; /*in*/
            public float[] triggerInterval; /*in*/
            public float[] uptime; /*out*/
            public float[,] combinedUptime; /*out*/
            public float[,] partialIntegral; /*out*/
            public int N;
            public int NC;
            public Ibeta Ibeta;
            public int I;

            public Parameters(SpecialEffect[] effects, float[] triggerInterval, float[] triggerChance, float[] offset, float attackSpeed) : this(effects, triggerInterval, triggerChance, offset, attackSpeed, null, null, 0.0f)
            {
            }

            public Parameters(SpecialEffect[] effects, float[] triggerInterval, float[] triggerChance, float[] offset, float attackSpeed, float[] scale, float[] value, float fightDuration)
            {
                //this.effects = effects;
                //this.triggerInterval = triggerInterval;
                d = new float[effects.Length];
                p = new float[effects.Length];
                c = new float[effects.Length];
                o = new float[effects.Length];
                k = new float[effects.Length];
                v = new float[effects.Length];
                this.effects = new SpecialEffect[effects.Length];
                this.triggerInterval = new float[effects.Length];
                /*k = scale;
                if (scale == null)
                {
                    k = new float[effects.Length];
                    for (int i = 0; i < effects.Length; i++)
                    {
                        k[i] = 1.0f;
                    }
                }*/

                bool discretizationCorrection = true;

                // sort it so that we have nonstationary effects first
                int j = 0;
                for (int i = 0; i < effects.Length; i++)
                {
                    if (effects[i].Cooldown > effects[i].Duration)
                    {
                        I = i;
                        this.effects[j] = effects[i];
                        this.triggerInterval[j] = triggerInterval[i];
                        if (scale == null)
                        {
                            k[j] = 1.0f;
                        }
                        else
                        {
                            k[j] = scale[i];
                        }
                        if (value != null)
                        {
                            v[j] = value[i];
                        }
                        p[j] = triggerChance[i] * effects[i].GetChance(attackSpeed);
                        if (triggerInterval[i] == 0.0f)
                        {
                            // on use effects
                            d[j] = effects[i].Duration;
                            c[j] = effects[i].Cooldown;
                            o[j] = offset[i];
                        }
                        else
                        {
                            d[j] = effects[i].Duration / triggerInterval[i];
                            c[j] = effects[i].Cooldown / triggerInterval[i];
                            if (discretizationCorrection)
                            {
                                c[j] += 0.5f;
                            }
                            if (c[j] < 1.0f) c[j] = 0.0f; // no cooldown model, WARNING: we currently don't support the case where 0 < cooldown < duration
                            o[j] = offset[i] / triggerInterval[i];
                        }
                        j++;
                    }
                }

                N = j;
                NC = (1 << N);

                uptime = new float[effects.Length];
                combinedUptime = new float[5 + 2 * maxRecursionDepth, NC];
                partialIntegral = new float[5 + 2 * maxRecursionDepth, NC];

                for (int i = 0; i < effects.Length; i++)
                {
                    if (effects[i].Cooldown <= effects[i].Duration)
                    {
                        this.effects[j] = effects[i];
                        if (value != null)
                        {
                            v[j] = value[i];
                        }
                        uptime[j] = effects[i].GetAverageUptime(triggerInterval[i], triggerChance[i], attackSpeed, fightDuration);
                        if (scale != null)
                        {
                            uptime[j] *= scale[i];
                        }
                        j++;
                    }
                }

                switch (Properties.GeneralSettings.Default.CombinationEffectMode)
                {
                    case 0:
                        Ibeta = SpecialFunction.IbetaInterpolatedLinear;
                        break;
                    case 1:
                        Ibeta = SpecialFunction.IbetaInterpolated;
                        break;
                    default:
                        Ibeta = SpecialFunction.Ibeta;
                        break;
                }
            }
        }

        /// <summary>
        /// Computes the average uptime of all effects being active.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds for each effect.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced for each effect.</param>
        /// <param name="offset">Initial cooldown for each effect.</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="fightDuration">Duration of fight in seconds.</param>
        public static float GetAverageCombinedUptime(SpecialEffect[] effects, float[] triggerInterval, float[] triggerChance, float[] offset, float attackSpeed, float fightDuration)
        {
            // CombinedAverageUptime = integrate_0..fightDuration prod_i Uptime[i](t) dt

            // initialize data, translate into interval time
            Parameters p = new Parameters(effects, triggerInterval, triggerChance, offset, attackSpeed);

            // integrate using adaptive Simspon's method
            double totalCombinedUptime = AdaptiveSimpsonsMethod(p, fightDuration, 0.001f, 20);

            return (float)(totalCombinedUptime / fightDuration);
        }

        /// <summary>
        /// Computes the average uptime of specific effects being active/inactive.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds for each effect.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced for each effect.</param>
        /// <param name="active">Determines if specific effects are being active/inactive for the uptime calculation.</param>
        /// <param name="offset">Initial cooldown for each effect.</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="fightDuration">Duration of fight in seconds.</param>
        public static WeightedStat[] GetAverageCombinedUptimeCombinations(SpecialEffect[] effects, float[] triggerInterval, float[] triggerChance, float[] offset, float attackSpeed, float fightDuration, AdditiveStat stat)
        {
            return GetAverageCombinedUptimeCombinations(effects, triggerInterval, triggerChance, offset, null, attackSpeed, fightDuration, stat);
        }

        /// <summary>
        /// Computes the average uptime of specific effects being active/inactive.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds for each effect.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced for each effect.</param>
        /// <param name="active">Determines if specific effects are being active/inactive for the uptime calculation.</param>
        /// <param name="offset">Initial cooldown for each effect.</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="fightDuration">Duration of fight in seconds.</param>
        /// <param name="scale">Chance that the effect will give the desired proc.</param>
        /// <param name="effects">The effects for which the combined uptime is to be computed.</param>
        /// <param name="stat">The stat for which we're computing the combinations.</param>
        public static WeightedStat[] GetAverageCombinedUptimeCombinations(SpecialEffect[] effects, float[] triggerInterval, float[] triggerChance, float[] offset, float[] scale, float attackSpeed, float fightDuration, AdditiveStat stat)
        {
            float[] value = new float[effects.Length];
            for (int j = 0; j < effects.Length; j++)
            {
                value[j] = effects[j].Stats._rawAdditiveData[(int)stat];
            }
            return GetAverageCombinedUptimeCombinations(effects, triggerInterval, triggerChance, offset, scale, attackSpeed, fightDuration, value);
        }

        /// <summary>
        /// Computes the average uptime of specific effects being active/inactive.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds for each effect.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced for each effect.</param>
        /// <param name="active">Determines if specific effects are being active/inactive for the uptime calculation.</param>
        /// <param name="offset">Initial cooldown for each effect.</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="fightDuration">Duration of fight in seconds.</param>
        /// <param name="scale">Chance that the effect will give the desired proc.</param>
        /// <param name="effects">The effects for which the combined uptime is to be computed.</param>
        /// <param name="value">The value of special effects when procced, used to compute the value in returned WeightedStat.</param>
        public static WeightedStat[] GetAverageCombinedUptimeCombinations(SpecialEffect[] effects, float[] triggerInterval, float[] triggerChance, float[] offset, float[] scale, float attackSpeed, float fightDuration, float[] value)
        {
            // CombinedAverageUptime = integrate_0..fightDuration prod_i Uptime[i](t) dt

            // initialize data, translate into interval time
            Parameters p = new Parameters(effects, triggerInterval, triggerChance, offset, attackSpeed, scale, value, fightDuration);

            // integrate using adaptive Simspon's method
            // if there's only one nonstationary use noncombination solution for efficiency
            if (p.N > 1)
            {
                AdaptiveSimpsonsMethodCombinations(p, fightDuration, 0.001f, maxRecursionDepth);
            }
            else if (p.N == 1)
            {
                p.partialIntegral[0, 1] = fightDuration * p.k[0] * p.effects[0].GetAverageUptime(triggerInterval[p.I], triggerChance[p.I], attackSpeed, fightDuration);
                p.partialIntegral[0, 0] = fightDuration - p.partialIntegral[0, 1];
            }

            WeightedStat[] result = new WeightedStat[1 << effects.Length];

            int mask = p.NC - 1;
            for (int i = 0; i < (1 << effects.Length); i++)
            {
                if (p.N > 0)
                {
                    result[i].Chance = p.partialIntegral[0, i & mask] / fightDuration;
                }
                else
                {
                    result[i].Chance = 1.0f;
                }
                for (int j = 0; j < effects.Length; j++)
                {
                    if ((i & (1 << j)) != 0)
                    {
                        result[i].Value += p.v[j];
                        if (j >= p.N)
                        {
                            result[i].Chance *= p.uptime[j];
                        }
                    }
                    else
                    {
                        if (j >= p.N)
                        {
                            result[i].Chance *= (1.0f - p.uptime[j]);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Computes the average uptime of specific effects being active/inactive.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds for each effect.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced for each effect.</param>
        /// <param name="active">Determines if specific effects are being active/inactive for the uptime calculation.</param>
        /// <param name="offset">Initial cooldown for each effect.</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="fightDuration">Duration of fight in seconds.</param>
        public static WeightedStat[] GetAverageCombinedUptimeCombinationsMultiplicative(SpecialEffect[] effects, float[] triggerInterval, float[] triggerChance, float[] offset, float attackSpeed, float fightDuration, MultiplicativeStat stat)
        {
            return GetAverageCombinedUptimeCombinationsMultiplicative(effects, triggerInterval, triggerChance, offset, null, attackSpeed, fightDuration, stat);
        }

        /// <summary>
        /// Computes the average uptime of specific effects being active/inactive.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds for each effect.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced for each effect.</param>
        /// <param name="active">Determines if specific effects are being active/inactive for the uptime calculation.</param>
        /// <param name="offset">Initial cooldown for each effect.</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="fightDuration">Duration of fight in seconds.</param>
        /// <param name="scale">Chance that the effect will give the desired proc.</param>
        /// <param name="effects">The effects for which the combined uptime is to be computed.</param>
        /// <param name="stat">The stat for which we're computing the combinations.</param>
        public static WeightedStat[] GetAverageCombinedUptimeCombinationsMultiplicative(SpecialEffect[] effects, float[] triggerInterval, float[] triggerChance, float[] offset, float[] scale, float attackSpeed, float fightDuration, MultiplicativeStat stat)
        {
            float[] value = new float[effects.Length];
            for (int j = 0; j < effects.Length; j++)
            {
                value[j] = effects[j].Stats._rawMultiplicativeData[(int)stat];
            }
            return GetAverageCombinedUptimeCombinationsMultiplicative(effects, triggerInterval, triggerChance, offset, scale, attackSpeed, fightDuration, value);
        }

        /// <summary>
        /// Computes the average uptime of specific effects being active/inactive.
        /// </summary>
        /// <param name="triggerInterval">Average time interval between triggers in seconds for each effect.</param>
        /// <param name="triggerChance">Chance that trigger of correct type is produced for each effect.</param>
        /// <param name="active">Determines if specific effects are being active/inactive for the uptime calculation.</param>
        /// <param name="offset">Initial cooldown for each effect.</param>
        /// <param name="attackSpeed">Average unhasted attack speed, used in PPM calculations.</param>
        /// <param name="fightDuration">Duration of fight in seconds.</param>
        /// <param name="scale">Chance that the effect will give the desired proc.</param>
        /// <param name="effects">The effects for which the combined uptime is to be computed.</param>
        /// <param name="value">The value of special effects when procced, used to compute the value in returned WeightedStat.</param>
        public static WeightedStat[] GetAverageCombinedUptimeCombinationsMultiplicative(SpecialEffect[] effects, float[] triggerInterval, float[] triggerChance, float[] offset, float[] scale, float attackSpeed, float fightDuration, float[] value)
        {
            // CombinedAverageUptime = integrate_0..fightDuration prod_i Uptime[i](t) dt

            // initialize data, translate into interval time
            Parameters p = new Parameters(effects, triggerInterval, triggerChance, offset, attackSpeed, scale, value, fightDuration);

            // integrate using adaptive Simspon's method
            // if there's only one nonstationary use noncombination solution for efficiency
            if (p.N > 1)
            {
                AdaptiveSimpsonsMethodCombinations(p, fightDuration, 0.001f, maxRecursionDepth);
            }
            else if (p.N == 1)
            {
                p.partialIntegral[0, 1] = fightDuration * p.k[0] * p.effects[0].GetAverageUptime(triggerInterval[p.I], triggerChance[p.I], attackSpeed, fightDuration);
                p.partialIntegral[0, 0] = fightDuration - p.partialIntegral[0, 1];
            }

            WeightedStat[] result = new WeightedStat[1 << effects.Length];

            int mask = p.NC - 1;
            for (int i = 0; i < (1 << effects.Length); i++)
            {
                if (p.N > 0)
                {
                    result[i].Chance = p.partialIntegral[0, i & mask] / fightDuration;
                }
                else
                {
                    result[i].Chance = 1.0f;
                }
                for (int j = 0; j < effects.Length; j++)
                {
                    if ((i & (1 << j)) != 0)
                    {
                        result[i].Value = (1 + result[i].Value) * (1 + p.v[j]) - 1;
                        if (j >= p.N)
                        {
                            result[i].Chance *= p.uptime[j];
                        }
                    }
                    else
                    {
                        if (j >= p.N)
                        {
                            result[i].Chance *= (1.0f - p.uptime[j]);
                        }
                    }
                }
            }

            return result;
        }

        private static float AdaptiveSimpsonsAux(Parameters p, float a, float b, float epsilon, float S, float fa, float fb, float fc, int bottom)
        {
            float c = (a + b) / 2;
            float h = b - a;
            float d = (a + c) / 2;
            float e = (c + b) / 2;
            float fd = GetCombinedUptime(p, d);
            float fe = GetCombinedUptime(p, e);
            float Sleft = (h / 12) * (fa + 4 * fd + fc);
            float Sright = (h / 12) * (fc + 4 * fe + fb);
            float S2 = Sleft + Sright;
			if (bottom <= 0 || (h < 10 && Math.Abs(S2 - S) <= 15 * epsilon))
            {
                return S2 + (S2 - S) / 15;
            }
            return AdaptiveSimpsonsAux(p, a, c, epsilon / 2, Sleft, fa, fc, fd, bottom - 1) +
                   AdaptiveSimpsonsAux(p, c, b, epsilon / 2, Sright, fc, fb, fe, bottom - 1);
        }

        private static void AdaptiveSimpsonsAuxCombinations(Parameters p, float a, float b, float epsilon, int S, int fa, int fb, int fc, int bottom, int level)
        {
            float c = (a + b) / 2;
            float h = b - a;
            float d = (a + c) / 2;
            float e = (c + b) / 2;
            int fd = 1 + 2 * level;
            GetCombinedUptimeCombinations(p, d, fd);
            int fe = 2 + 2 * level;
            GetCombinedUptimeCombinations(p, e, fe);
            int Sleft = 2 * level - 1;
            int Sright = 2 * level;
            bool terminate = true;
            for (int i = 0; i < p.NC; i++)
            {
                p.partialIntegral[Sleft, i] = (h / 12) * (p.combinedUptime[fa, i] + 4 * p.combinedUptime[fd, i] + p.combinedUptime[fc, i]);
                p.partialIntegral[Sright, i] = (h / 12) * (p.combinedUptime[fc, i] + 4 * p.combinedUptime[fe, i] + p.combinedUptime[fb, i]);
                if (!(bottom <= 0 || (h < 10 && Math.Abs(p.partialIntegral[Sleft, i] + p.partialIntegral[Sright, i] - p.partialIntegral[S, i]) <= 15 * epsilon)))
                {
                    terminate = false;
                }
            }
            if (terminate)
            {
                for (int i = 0; i < p.NC; i++)
                {
                    p.partialIntegral[S, i] = (p.partialIntegral[Sleft, i] + p.partialIntegral[Sright, i]) * 16.0f / 15.0f - p.partialIntegral[S, i] / 15.0f;
                }
            }
            else
            {
                AdaptiveSimpsonsAuxCombinations(p, a, c, epsilon / 2, Sleft, fa, fc, fd, bottom - 1, level + 1);
                AdaptiveSimpsonsAuxCombinations(p, c, b, epsilon / 2, Sright, fc, fb, fe, bottom - 1, level + 1);
                for (int i = 0; i < p.NC; i++)
                {
                    p.partialIntegral[S, i] = p.partialIntegral[Sleft, i] + p.partialIntegral[Sright, i];
                }
            }
        }

        private static float AdaptiveSimpsonsMethod(Parameters p, float fightDuration, float epsilon, int maxRecursionDepth)
        {
            float a = 0.0f;
            float b = fightDuration;
            float c = (a + b) / 2;
            float h = b - a;
            float fa = GetCombinedUptime(p, a);
            float fb = GetCombinedUptime(p, b);
            float fc = GetCombinedUptime(p, c);
            float S = (h / 6) * (fa + 4 * fc + fb);
            return AdaptiveSimpsonsAux(p, a, b, epsilon, S, fa, fb, fc, maxRecursionDepth); 
        }

        private static void AdaptiveSimpsonsMethodCombinations(Parameters p, float fightDuration, float epsilon, int maxRecursionDepth)
        {
            float a = 0.0f;
            float b = fightDuration;
            float c = (a + b) / 2;
            float h = b - a;
            int fa = 0;
            GetCombinedUptimeCombinations(p, a, fa);
            int fb = 1;
            GetCombinedUptimeCombinations(p, b, fb);
            int fc = 2;
            GetCombinedUptimeCombinations(p, c, fc);
            int S = 0;
            for (int i = 0; i < p.NC; i++)
            {
                p.partialIntegral[0, i] = (h / 6) * (p.combinedUptime[fa, i] + 4 * p.combinedUptime[fc, i] + p.combinedUptime[fb, i]);
            }
            AdaptiveSimpsonsAuxCombinations(p, a, b, epsilon, S, fa, fb, fc, maxRecursionDepth, 1);
        }

        private struct RecursionData
        {
            public float a;
            public float b;
            public int S;
            public int fa;
            public int fb;
            public int fc;
            public int level;
            public float epsilon;
            public bool combine;
        }

        private static void AdaptiveSimpsonsMethodCombinations2(Parameters p, float fightDuration, float epsilon, int maxRecursionDepth)
        {
            float a = 0.0f;
            float b = fightDuration;
            float c = (a + b) / 2;
            float h = b - a;
            int fa = 0;
            GetCombinedUptimeCombinations(p, a, fa);
            int fb = 1;
            GetCombinedUptimeCombinations(p, b, fb);
            int fc = 2;
            GetCombinedUptimeCombinations(p, c, fc);
            int S = 0;
            for (int i = 0; i < (1 << p.effects.Length); i++)
            {
                p.partialIntegral[0, i] = (h / 6) * (p.combinedUptime[fa, i] + 4 * p.combinedUptime[fc, i] + p.combinedUptime[fb, i]);
            }
            RecursionData[] stack = new RecursionData[2 * maxRecursionDepth];

            int stackIndex = 0;
            stack[stackIndex].a = a;
            stack[stackIndex].b = b;
            stack[stackIndex].fa = fa;
            stack[stackIndex].fb = fb;
            stack[stackIndex].fc = fc;
            stack[stackIndex].S = S;
            stack[stackIndex].level = 1;
            stack[stackIndex].combine = false;
            stack[stackIndex].epsilon = epsilon;
            while (stackIndex >= 0)
            {
                if (stack[stackIndex].combine)
                {
                    // we just computed both subintervals and we have to sum them up into S
                    for (int i = 0; i < (1 << p.effects.Length); i++)
                    {
                        int Sleft = 2 * stack[stackIndex].level - 1;
                        int Sright = 2 * stack[stackIndex].level;
                        p.partialIntegral[stack[stackIndex].S, i] = p.partialIntegral[Sleft, i] + p.partialIntegral[Sright, i];
                    }
                    // move down in stack
                    stackIndex--;
                }
                else
                {
                    c = (stack[stackIndex].a + stack[stackIndex].b) / 2;
                    h = stack[stackIndex].b - stack[stackIndex].a;
                    float d = (stack[stackIndex].a + c) / 2;
                    float e = (c + stack[stackIndex].b) / 2;
                    int fd = 1 + 2 * stack[stackIndex].level;
                    GetCombinedUptimeCombinations(p, d, fd);
                    int fe = 2 + 2 * stack[stackIndex].level;
                    GetCombinedUptimeCombinations(p, e, fe);
                    int Sleft = 2 * stack[stackIndex].level - 1;
                    int Sright = 2 * stack[stackIndex].level;
                    bool terminate = true;
                    for (int i = 0; i < (1 << p.effects.Length); i++)
                    {
                        p.partialIntegral[Sleft, i] = (h / 12) * (p.combinedUptime[stack[stackIndex].fa, i] + 4 * p.combinedUptime[fd, i] + p.combinedUptime[stack[stackIndex].fc, i]);
                        p.partialIntegral[Sright, i] = (h / 12) * (p.combinedUptime[stack[stackIndex].fc, i] + 4 * p.combinedUptime[fe, i] + p.combinedUptime[stack[stackIndex].fb, i]);
                        if (!(maxRecursionDepth + 1 - stack[stackIndex].level <= 0 || (h < 10 && Math.Abs(p.partialIntegral[Sleft, i] + p.partialIntegral[Sright, i] - p.partialIntegral[stack[stackIndex].S, i]) <= 15 * stack[stackIndex].epsilon)))
                        {
                            terminate = false;
                        }
                    }
                    if (terminate)
                    {
                        // terminal case, combine and drop stack
                        for (int i = 0; i < (1 << p.effects.Length); i++)
                        {
                            p.partialIntegral[stack[stackIndex].S, i] = (p.partialIntegral[Sleft, i] + p.partialIntegral[Sright, i]) * 16.0f / 15.0f - p.partialIntegral[stack[stackIndex].S, i] / 15.0f;
                        }
                        stackIndex--;
                    }
                    else
                    {
                        // we'll need to go deeper, put children on stack and set us to combine after
                        stack[stackIndex].combine = true;
                        stack[stackIndex + 2].a = stack[stackIndex].a;
                        stack[stackIndex + 2].b = c;
                        stack[stackIndex + 2].fa = stack[stackIndex].fa;
                        stack[stackIndex + 2].fb = stack[stackIndex].fc;
                        stack[stackIndex + 2].fc = fd;
                        stack[stackIndex + 2].S = Sleft;
                        stack[stackIndex + 2].level = stack[stackIndex].level + 1;
                        stack[stackIndex + 2].epsilon = stack[stackIndex].epsilon / 2;
                        stack[stackIndex + 2].combine = false;
                        stack[stackIndex + 1].a = c;
                        stack[stackIndex + 1].b = stack[stackIndex].b;
                        stack[stackIndex + 1].fa = stack[stackIndex].fc;
                        stack[stackIndex + 1].fb = stack[stackIndex].fb;
                        stack[stackIndex + 1].fc = fe;
                        stack[stackIndex + 1].S = Sright;
                        stack[stackIndex + 1].level = stack[stackIndex].level + 1;
                        stack[stackIndex + 1].epsilon = stack[stackIndex].epsilon / 2;
                        stack[stackIndex + 1].combine = false;
                        stackIndex += 2;
                    }
                }
            }
        }

        private static float GetCombinedUptime(Parameters p, float t)
        {
            // Uptime(x) = sum_r=0..inf Ibeta(r+1, x - r * cooldown, p) - Ibeta(r+1, x - duration - r * cooldown, p)
            // t := x * interval
            // Uptime(t) = sum_r=0..inf Ibeta(r+1, t / interval - r * cooldown / interval, p) - Ibeta(r+1, t / interval - duration / interval - r * cooldown / interval, p)

            float combinedUptime = 1.0f;

            for (int i = 0; i < p.effects.Length; i++)
            {
                float x = t / p.triggerInterval[i] - p.o[i];

                float uptime = 0.0f;
                int r = 1;
                while (x > 0)
                {
                    uptime += p.Ibeta(r, x, p.p[i]);
                    float xd = x - p.d[i];
                    if (xd > 0)
                    {
                        uptime -= p.Ibeta(r, xd, p.p[i]);
                    }
                    r++;
                    x -= p.c[i];
                }
                combinedUptime *= uptime;
            }

            return combinedUptime;
        }

        private static void GetCombinedUptimeCombinations(Parameters p, float t, int index)
        {
            // Up time(x) = sum_r=0..inf Ibeta(r+1, x - r * cooldown, p) - Ibeta(r+1, x - duration - r * cooldown, p)
            // t := x * interval
            // Uptime(t) = sum_r=0..inf Ibeta(r+1, t / interval - r * cooldown / interval, p) - Ibeta(r+1, t / interval - duration / interval - r * cooldown / interval, p)

            for (int i = 0; i < p.N; i++)
            {
                if (p.triggerInterval[i] == 0.0f)
                {
                    float x = t - p.o[i]; // on use effects start straight on offset and then every cooldown
                    if (x >= 0 && x % p.c[i] < p.d[i])
                    {
                        p.uptime[i] = 1.0f;
                    }
                    else
                    {
                        p.uptime[i] = 0.0f;
                    }
                }
                else
                {
                    float x = t / p.triggerInterval[i] - p.o[i];

                    if (p.c[i] == 0.0f)
                    {
                        // support for cooldown = 0
                        if (p.d[i] <= 1.0f)
                        {
                            p.uptime[i] = p.d[i] * p.p[i];
                        }
                        else
                        {
                            p.uptime[i] = 1.0f - (float)Math.Pow(1f - p.p[i], Math.Min(x, p.d[i]));
                        }
                    }
                    else
                    {
                        // normal case for duration < cooldown
                        p.uptime[i] = 0.0f;
                        int r = 1;
                        while (x > 0)
                        {
                            p.uptime[i] += p.Ibeta(r, x, p.p[i]);
                            float xd = x - p.d[i];
                            if (xd > 0)
                            {
                                p.uptime[i] -= p.Ibeta(r, xd, p.p[i]);
                            }
                            r++;
                            x -= p.c[i];
                        }
                    }
                }
            }

            for (int i = 0; i < p.NC; i++)
            {
                p.combinedUptime[index, i] = 1.0f;
                for (int j = 0; j < p.N; j++)
                {
					if ((i & (1 << j)) == 0)
					{
						p.combinedUptime[index, i] *= (1.0f - p.uptime[j] * p.k[j]);
					}
					else
					{
						p.combinedUptime[index, i] *= p.uptime[j] * p.k[j];
					}
				}
            }
        }
    }
}
