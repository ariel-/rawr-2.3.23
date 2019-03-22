using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Mage
{
    class FSRCalc
    {
        /*private static Dictionary<string, Dictionary<float, float>> Cache = new Dictionary<string, Dictionary<float, float>> ();

        public static bool TryGetCachedOO5SR(string spell, float casttimeHash, out float oo5sr)
        {
            oo5sr = 0;
            Dictionary<float, float> valueCache;
            if (Cache.TryGetValue(spell, out valueCache))
            {
                return valueCache.TryGetValue(casttimeHash, out oo5sr);
            }
            return false;
        }*/

        public FSRCalc()
        {
            ManaSpentTimestamp = new List<float> ();
            ChannelDuration = new List<float> ();
        }

        public FSRCalc(int capacity)
        {
            ManaSpentTimestamp = new List<float>(capacity);
            ChannelDuration = new List<float>(capacity);
        }

        public List<float> ManaSpentTimestamp;
        public List<float> ChannelDuration;
        public float Duration;

        public void AddSpell(float duration, float lag, bool channel)
        {
            if (channel)
            {
                ManaSpentTimestamp.Add(Duration + lag);
                ChannelDuration.Add(duration);
            }
            else
            {
                ManaSpentTimestamp.Add(Duration + duration + lag);
                ChannelDuration.Add(0);
            }
            Duration += duration + lag;
        }

        public void AddPause(float duration)
        {
            Duration += duration;
        }

        private float TimeDiff(int i1, int i2, int N)
        {
            /*float ret = 0;
            while (i1 < 0)
            {
                i1 += N;
                ret += Duration;
            }
            ret += ManaSpentTimestamp[i2] - ManaSpentTimestamp[i1];
            return ret;*/
            //int i1p = ((i1 % N) + N) % N;
            //int i1p = i1 % N + ((i1 < 0 || (i1 % N == 0)) ? N : 0);
            int k = i1 / N;
            int m = k * N;
            //return ManaSpentTimestamp[i2] - ManaSpentTimestamp[i1 - k * N + ((i1 < 0 && (i1 % N != 0)) ? N : 0)] + Duration * (- k + ((i1 < 0 && (i1 % N != 0)) ? 1 : 0));
            if (i1 < 0 && m != i1)
            {
                return ManaSpentTimestamp[i2] - ManaSpentTimestamp[i1 - m + N] + Duration * (-k + 1);
            }
            else
                return ManaSpentTimestamp[i2] - ManaSpentTimestamp[i1 - m] - Duration * k;
        }

        public float CalculateOO5SR(float clearcastingChance/*, string spell, float casttimeHash*/)
        {
            int N = ManaSpentTimestamp.Count;
            float c;
            float ret = 0;
            for (int a = 0; a < N; a++)
            {
                c = 1 - clearcastingChance;
                //int b = a - 1;
                float t;
                float endstamp = ManaSpentTimestamp[a];
                int k = 0;
                int bb = a - 1;
                if (bb < 0)
                {
                    bb += N;
                    k += 1;
                }
                float bbstamp = ManaSpentTimestamp[bb];
                float t0 = endstamp - bbstamp + k * Duration - ChannelDuration[bb];
                ret += ChannelDuration[bb];
                do
                {
                    t = endstamp - bbstamp + k * Duration - 5;
                    if (t < 0)
                        ret += t0 * c;
                    else if (t < t0)
                        ret += (t0 - t) * c;
                    c *= clearcastingChance;
                    //b -= 1;
                    bb -= 1;
                    if (bb < 0)
                    {
                        bb += N;
                        k += 1;
                    }
                    bbstamp = ManaSpentTimestamp[bb];
                } while (t < t0);
            }
            ret = (Duration - ret) / Duration;

            // unoptimized version, slightly more readable
            // main idea, compute time spent in FSR for each segment between potential mana spendings
            // compute partial contributions based on probability that index b was the last time mana was spent
            /*int N = ManaSpentTimestamp.Count;
            float c;
            float ret = 0;
            for (int a = 0; a < N; a++)
            {
                c = 1 - clearcastingChance;
                int b = a - 1;
                float t;
                float t0 = TimeDiff(b, a, N) - ChannelDuration[((b % N) + N) % N];
                ret += ChannelDuration[(((a - 1) % N) + N) % N];
                do
	            {
	                t = TimeDiff(b, a, N) - 5;
                    if (t < 0)
                        ret += t0 * c;
                    else if (t < t0)
                        ret += (t0 - t) * c;
                    c *= clearcastingChance;
                    b -= 1;
                } while (t < t0);
            }
            ret = (Duration - ret) / Duration;*/

            // cache result
            /*Dictionary<float, float> valueCache;
            if (!Cache.TryGetValue(spell, out valueCache))
            {
                valueCache = new Dictionary<float, float>();
                Cache[spell] = valueCache;
            }
            valueCache[casttimeHash] = ret;*/

            return ret;
        }

        public static float CalculateSimpleOO5SR(float clearcastingChance, float duration, float lag, bool channel)
        {
            float timestamp;
            float channelDuration = 0;
            if (channel)
            {
                timestamp = lag;
                channelDuration = duration;
            }
            else
            {
                timestamp = duration + lag;
            }
            duration += lag;
            float c;
            float ret = 0;

            c = 1 - clearcastingChance;
            float t;
            int k = 1;
            float t0 = k * duration - channelDuration;
            ret += channelDuration;
            do
            {
                t = k * duration - 5;
                if (t < 0)
                    ret += t0 * c;
                else if (t < t0)
                    ret += (t0 - t) * c;
                c *= clearcastingChance;
                k += 1;
            } while (t < t0);

            ret = (duration - ret) / duration;

            return ret;
        }
    }
}
