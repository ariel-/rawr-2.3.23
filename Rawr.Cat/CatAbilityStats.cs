using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Cat
{
	public abstract class CatAbilityStats
	{
		public float DamagePerHit { get; set; }
		public float DamagePerSwing { get; set; }
		public float EnergyCost { get; set; }
		public float DurationUptime { get; set; }
		public float DurationAverage { get; set; }
		public float DamagePerHitPerCP { get; set; }
		public float DamagePerSwingPerCP { get; set; }
		public float DurationPerCP { get; set; }
		
		public abstract string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration);
	}

	public class CatMeleeStats : CatAbilityStats
	{
		public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
		{
			float damageDone = useCount * (DamagePerSwing + cp * DamagePerSwingPerCP);
			string stats = string.Format("{0:F0}x   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Swing:  {3}\r\nDamage Per Hit:  {4}",
				useCount, damageDone, damageDone / totalDamage, DamagePerSwing, DamagePerHit);
			return stats;
		}
	}

	public class CatMangleStats : CatAbilityStats
	{
		public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
		{
			float damageDone = useCount * (DamagePerSwing + cp * DamagePerSwingPerCP);
			string stats = string.Format("{5:F1} DPE   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Landed Swing:  {3}\r\nDamage Per Hit:  {4}\r\nDamage Per Energy:  {5}",
				useCount, damageDone, damageDone / totalDamage, DamagePerSwing * chanceNonAvoided, DamagePerHit, DamagePerSwing / EnergyCost);
			return stats;
		}
	}

	public class CatShredStats : CatAbilityStats
	{
		public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
		{
			float damageDone = useCount * (DamagePerSwing + cp * DamagePerSwingPerCP);
			string stats = string.Format("{5:F1} DPE   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Landed Swing:  {3}\r\nDamage Per Hit:  {4}\r\nDamage Per Energy:  {5}",
				useCount, damageDone, damageDone / totalDamage, DamagePerSwing * chanceNonAvoided, DamagePerHit, DamagePerSwing / EnergyCost);
			return stats;
		}
	}

	public class CatRakeStats : CatAbilityStats
	{
		public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
		{
			float damageDone = useCount * (DamagePerSwing + cp * DamagePerSwingPerCP);
			string stats = string.Format("{5:F1} DPE   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Landed Swing:  {3}\r\nDamage Per Hit:  {4}\r\nDamage Per Energy:  {5}\r\nDuration:  {6}sec\r\nUptime:  {7:P}",
				useCount, damageDone, damageDone / totalDamage, DamagePerSwing * chanceNonAvoided, DamagePerHit, DamagePerSwing / EnergyCost, DurationUptime, (DurationUptime * useCount) / totalDuration);
			return stats;
		}
	}

	public class CatRipStats : CatAbilityStats
	{
		public override string GetStatsTexts(float useCount, float cp, float totalDamage, float chanceNonAvoided, float totalDuration)
		{
			float damageDone = useCount * (DamagePerSwing + cp * DamagePerSwingPerCP);
			string stats = string.Format("{5:F1} DPE   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Landed Swing:  {3}\r\nDamage Per Hit:  {4}\r\nDamage Per Energy:  {5}\r\nDuration:  {6}sec\r\nUptime:  {7:P}",
				useCount, damageDone, damageDone / totalDamage, DamagePerSwing * chanceNonAvoided, DamagePerHit, DamagePerSwing / EnergyCost, DurationUptime, (DurationUptime * useCount) / totalDuration);
			return stats;
		}
	}

	public class CatBiteStats : CatAbilityStats
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

			string stats = string.Format("{5:F1} DPE   ({2:P1})*Use Count:  {0}\r\nDamage Done:  {1}\r\n% of Total Damage:  {2:P}\r\nDamage Per Landed Swing:  {3}\r\nDamage Per Hit:  {4}\r\nDamage Per Energy:  {5}\r\n1cp DPE:  {6}\r\n2cp DPE:  {7}\r\n3cp DPE:  {8}\r\n4cp DPE:  {9}\r\n5cp DPE:  {10}\r\n",
				useCount, damageDone, damageDone / totalDamage, damagePerSwing * chanceNonAvoided, damagePerHit, damagePerSwing / EnergyCost, dpe1, dpe2, dpe3, dpe4, dpe5);
			return stats;
		}
	}

	public class CatRoarStats : CatAbilityStats
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
}
