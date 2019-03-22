using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Bear
{
	public class StatsBear : Stats
	{
		public float BonusMangleDamageMultiplier { get; set; }
		public float BonusMaulDamageMultiplier { get; set; }
		public float BonusEnrageDamageMultiplier { get; set; }
		public float MangleCooldownReduction { get; set; }
		public float BonusSwipeDamageMultiplier { get; set; }

		public WeightedStat[] TemporaryArmorUptimes { get; set; }

	}
}
