using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Cat
{
	public class StatsCat : Stats
	{
		public float BonusMangleDamageMultiplier { get; set; }
		public float BonusRakeDamageMultiplier { get; set; }
		public float BonusShredDamageMultiplier { get; set; }
		public float BonusFerociousBiteDamageMultiplier { get; set; }
		public float BonusEnergyOnTigersFury { get; set; }
		public float RakeCostReduction { get; set; }
		public float ShredCostReduction { get; set; }
		public float BonusCPOnCrit { get; set; }
		public float FinisherEnergyOnAvoid { get; set; }
		
		public WeightedStat[] TemporaryArPenRatingUptimes { get; set; }
		public WeightedStat[] TemporaryCritRatingUptimes { get; set; }
	}
}
