using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Rogue
{
    public abstract class RogueAbilityStats
    {
        public float DamagePerHit { get; set; }
        public float DamagePerSwing { get; set; }
        public float EnergyCost { get; set; }
        public float DurationUptime { get; set; }
        public float DurationAverage { get; set; }
        public float DamagePerHitPerCP { get; set; }
        public float DamagePerSwingPerCP { get; set; }
        public float DurationPerCP { get; set; }
        public Item Weapon { get; set; }
        public float CritChance { get; set; }
        public float CPPerSwing { get; set; }

        public abstract string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration);
    }

    public class RogueMHStats : RogueAbilityStats
    {
        public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
        {
            float damageDone = useCount * (DamagePerSwing + cp * DamagePerSwingPerCP);
            string stats = string.Format("{0:F0}x   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Swing:  {3}\r\nDamage Per Hit:  {4}",
                useCount, damageDone, damageDone / totalDamage, DamagePerSwing, DamagePerHit);
            return stats;
        }
    }

    public class RogueOHStats : RogueAbilityStats
    {
        public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
        {
            float damageDone = useCount * (DamagePerSwing + cp * DamagePerSwingPerCP);
            string stats = string.Format("{0:F0}x   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Swing:  {3}\r\nDamage Per Hit:  {4}",
                useCount, damageDone, damageDone / totalDamage, DamagePerSwing, DamagePerHit);
            return stats;
        }
    }

    public class RogueBackstabStats : RogueAbilityStats
    {
        public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
        {
            float damageDone = useCount * (DamagePerSwing + cp * DamagePerSwingPerCP);
            string stats = string.Format("{5:F1} DPE   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Swing:  {3}\r\nDamage Per Hit:  {4}\r\nDamage Per Energy:  {5}",
                useCount, damageDone, damageDone / totalDamage, DamagePerSwing * chanceNonAvoided, DamagePerHit, DamagePerSwing / EnergyCost);
            return stats;
        }
    }

    public class RogueHemoStats : RogueAbilityStats
    {
        public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
        {
            float damageDone = useCount * (DamagePerSwing + cp * DamagePerSwingPerCP);
            string stats = string.Format("{5:F1} DPE   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Swing:  {3}\r\nDamage Per Hit:  {4}\r\nDamage Per Energy:  {5}",
                useCount, damageDone, damageDone / totalDamage, DamagePerSwing * chanceNonAvoided, DamagePerHit, DamagePerSwing / EnergyCost);
            return stats;
        }
    }

    public class RogueMutiStats : RogueAbilityStats
    {
        public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
        {
            float damageDone = useCount * (DamagePerSwing + cp * DamagePerSwingPerCP);
            string stats = string.Format("{5:F1} DPE   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Swing:  {3}\r\nDamage Per Hit:  {4}\r\nDamage Per Energy:  {5}",
                useCount, damageDone, damageDone / totalDamage, DamagePerSwing * chanceNonAvoided, DamagePerHit, DamagePerSwing / EnergyCost);
            return stats;
        }
    }

    public class RogueSStrikeStats : RogueAbilityStats
    {
        public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
        {
            float damageDone = useCount * (DamagePerSwing + cp * DamagePerSwingPerCP);
            string stats = string.Format("{5:F1} DPE   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Swing:  {3}\r\nDamage Per Hit:  {4}\r\nDamage Per Energy:  {5}",
                useCount, damageDone, damageDone / totalDamage, DamagePerSwing * chanceNonAvoided, DamagePerHit, DamagePerSwing / EnergyCost);
            return stats;
        }
    }

    public class RogueRuptStats : RogueAbilityStats
    {
        public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
        {
            float damageDone = useCount * (DamagePerSwing + cp * DamagePerSwingPerCP);
            string stats = string.Format("{5:F1} DPE   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Swing:  {3}\r\nDamage Per Hit:  {4}\r\nDamage Per Energy:  {5}\r\nDuration:  {6}sec\r\nUptime:  {7:P}",
                useCount, damageDone, damageDone / totalDamage, DamagePerSwing * chanceNonAvoided, DamagePerHit, DamagePerSwing / EnergyCost, DurationUptime + (cp * DurationPerCP), ((DurationUptime + (cp * DurationPerCP)) * useCount) / totalDuration);
            return stats;
        }
    }

    public class RogueEnvenomStats : RogueAbilityStats
    {
        public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
        {
            float damagePerSwing = (DamagePerSwing + cp * DamagePerSwingPerCP);
            float damagePerHit = (DamagePerHit + cp * DamagePerHitPerCP);
            float damageDone = useCount * damagePerSwing;
            float dpe1 = (DamagePerSwing + 1f * DamagePerSwingPerCP) / EnergyCost;
            float dpe2 = (DamagePerSwing + 2f * DamagePerSwingPerCP) / EnergyCost;
            float dpe3 = (DamagePerSwing + 3f * DamagePerSwingPerCP) / EnergyCost;
            float dpe4 = (DamagePerSwing + 4f * DamagePerSwingPerCP) / EnergyCost;
            float dpe5 = (DamagePerSwing + 5f * DamagePerSwingPerCP) / EnergyCost;

            string stats = string.Format("{5:F1} DPE   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Swing:  {3}\r\nDamage Per Hit:  {4}\r\nDamage Per Energy:  {5}\r\n1cp DPE:  {6}\r\n2cp DPE:  {7}\r\n3cp DPE:  {8}\r\n4cp DPE:  {9}\r\n5cp DPE:  {10}\r\n",
                useCount, damageDone, damageDone / totalDamage, damagePerSwing * chanceNonAvoided, damagePerHit, damagePerSwing / EnergyCost, dpe1, dpe2, dpe3, dpe4, dpe5);
            return stats;
        }
    }

    public class RogueEvisStats : RogueAbilityStats
    {
        public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
        {
            float damagePerSwing = (DamagePerSwing + cp * DamagePerSwingPerCP);
            float damagePerHit = (DamagePerHit + cp * DamagePerHitPerCP);
            float damageDone = useCount * damagePerSwing;
            float dpe1 = (DamagePerSwing + 1f * DamagePerSwingPerCP) / EnergyCost;
            float dpe2 = (DamagePerSwing + 2f * DamagePerSwingPerCP) / EnergyCost;
            float dpe3 = (DamagePerSwing + 3f * DamagePerSwingPerCP) / EnergyCost;
            float dpe4 = (DamagePerSwing + 4f * DamagePerSwingPerCP) / EnergyCost;
            float dpe5 = (DamagePerSwing + 5f * DamagePerSwingPerCP) / EnergyCost;

            string stats = string.Format("{5:F1} DPE   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Swing:  {3}\r\nDamage Per Hit:  {4}\r\nDamage Per Energy:  {5}\r\n1cp DPE:  {6}\r\n2cp DPE:  {7}\r\n3cp DPE:  {8}\r\n4cp DPE:  {9}\r\n5cp DPE:  {10}\r\n",
                useCount, damageDone, damageDone / totalDamage, damagePerSwing * chanceNonAvoided, damagePerHit, damagePerSwing / EnergyCost, dpe1, dpe2, dpe3, dpe4, dpe5);
            return stats;
        }
    }

    public class RogueSnDStats : RogueAbilityStats
    {
        public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
        {
            string usage = string.Format("{0}x*Use Count: {0}",
                useCount);
            string stats = string.Format("{0}sec, {1}cp*Duration: {0}\r\nTarget Combo Points: {1}%",
                DurationUptime + (cp * DurationPerCP), cp);
            return usage + stats;
        }
    }

    public class RogueIPStats : RogueAbilityStats
    {
        public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
        {
            float damageDone = useCount * (DamagePerSwing + cp * DamagePerSwingPerCP);
            string stats = string.Format("{0}x   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Swing:  {3}\r\nDamage Per Hit:  {4}",
                useCount, damageDone, damageDone / totalDamage, DamagePerSwing * chanceNonAvoided, DamagePerHit);
            return stats;
        }
    }

    public class RogueDPStats : RogueAbilityStats
    {
        public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
        {
            float damageDone = useCount * (DamagePerSwing + cp * DamagePerSwingPerCP);
            string stats = string.Format("{0}x   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Swing:  {3}\r\nDamage Per Hit:  {4}",
                useCount, damageDone, damageDone / totalDamage, DamagePerSwing * chanceNonAvoided, DamagePerHit);
            return stats;
        }
    }

    public class RogueWPStats : RogueAbilityStats
    {
        public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
        {
            float damageDone = useCount * (DamagePerSwing + cp * DamagePerSwingPerCP);
            string stats = string.Format("{0}x   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Swing:  {3}\r\nDamage Per Hit:  {4}",
                useCount, damageDone, damageDone / totalDamage, DamagePerSwing * chanceNonAvoided, DamagePerHit);
            return stats;
        }   
    }

    public class RogueAPStats : RogueAbilityStats
    {
        public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
        {
            float damageDone = useCount * (DamagePerSwing + cp * DamagePerSwingPerCP);
            string stats = string.Format("{0}x   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Swing:  {3}\r\nDamage Per Hit:  {4}",
                useCount, damageDone, damageDone / totalDamage, DamagePerSwing * chanceNonAvoided, DamagePerHit);
            return stats;
        }
    }
}