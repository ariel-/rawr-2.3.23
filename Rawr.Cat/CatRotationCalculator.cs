using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Cat
{
	public class CatRotationCalculator
	{
		public StatsCat Stats { get; set; }
		public float Duration { get; set; }
		public float CPPerCPG { get; set; }
		public bool MaintainMangle { get; set; }
		public bool GlyphOfShred { get; set; }
		public float AttackSpeed { get; set; }
		public float ChanceExtraCPPerHit { get; set; }
		public bool OmenOfClarity { get; set; }
		public float AvoidedAttacks { get; set; }
		public float CPGEnergyCostMultiplier { get; set; }
		public float ClearcastOnBleedChance { get; set; }

		//public float MangleDuration { get; set; }
		//public float RipDurationUptime { get; set; }
		//public float RipDuration { get; set; }
		//public float RakeDuration { get; set; }
		//public float SavageRoarBonusDuration { get; set; }
		public float BerserkDuration { get; set; }

		//public float MeleeDamage { get; set; }
		//public float MangleDamage { get; set; }
		//public float ShredDamage { get; set; }
		//public float RakeDamage { get; set; }
		//public float RipDamage { get; set; }
		//public float BiteBaseDamage { get; set; }
		//public float BiteCPDamage { get; set; }

		//public float MangleEnergy { get; set; }
		//public float ShredEnergy { get; set; }
		//public float RakeEnergy { get; set; }
		//public float RipEnergy { get; set; }
		//public float BiteEnergy { get; set; }
		//public float RoarEnergy { get; set; }

		public CatAbilityStats MeleeStats { get; set; }
		public CatAbilityStats MangleStats { get; set; }
		public CatAbilityStats ShredStats { get; set; }
		public CatAbilityStats RakeStats { get; set; }
		public CatAbilityStats RipStats { get; set; }
		public CatAbilityStats BiteStats { get; set; }
		public CatAbilityStats RoarStats { get; set; }

		private float[] _chanceExtraCP = new float[5];

		public CatRotationCalculator(StatsCat stats, float duration, float cpPerCPG, bool maintainMangle, 
			float berserkDuration, float attackSpeed, bool omenOfClarity, bool glyphOfShred, float avoidedAttacks, 
			float chanceExtraCPPerHit, float cpgEnergyCostMultiplier, float clearcastOnBleedChance, 
			CatAbilityStats meleeStats, CatAbilityStats mangleStats, CatAbilityStats shredStats, CatAbilityStats rakeStats, 
			CatAbilityStats ripStats, CatAbilityStats biteStats, CatAbilityStats roarStats)
		{
			Stats = stats;
			Duration = duration;
			CPPerCPG = cpPerCPG;
			MaintainMangle = maintainMangle;
			AttackSpeed = attackSpeed;
			OmenOfClarity = omenOfClarity;
			GlyphOfShred = glyphOfShred;
			AvoidedAttacks = avoidedAttacks;
			ChanceExtraCPPerHit = chanceExtraCPPerHit;
			CPGEnergyCostMultiplier = cpgEnergyCostMultiplier;
			ClearcastOnBleedChance = clearcastOnBleedChance;
			BerserkDuration = berserkDuration;

			MeleeStats = meleeStats;
			MangleStats = mangleStats;
			ShredStats = shredStats;
			RakeStats = rakeStats;
			RipStats = ripStats;
			BiteStats = biteStats;
			RoarStats = roarStats;

			//MangleDuration = mangleDuration;
			//RipDurationUptime = ripDurationUptime;
			//RipDuration = ripDuration;
			//RakeDuration = rakeDuration;
			//SavageRoarBonusDuration = savageRoarBonusDuration;
			
			//MeleeDamage = meleeDamage;
			//MangleDamage = mangleDamage;
			//ShredDamage = shredDamage;
			//RakeDamage = rakeDamage;
			//RipDamage = ripDamage;
			//BiteBaseDamage = biteBaseDamage;
			//BiteCPDamage = biteCPDamage;
			
			//MangleEnergy = mangleEnergy;
			//ShredEnergy = shredEnergy;
			//RakeEnergy = rakeEnergy;
			//RipEnergy = ripEnergy;
			//BiteEnergy = biteEnergy;
			//RoarEnergy = roarEnergy;

			float c = chanceExtraCPPerHit, h = (1f - chanceExtraCPPerHit);
			_chanceExtraCP[0] = c;
			_chanceExtraCP[1] = c*h;
			_chanceExtraCP[2] = c*c+c*h*h;
			_chanceExtraCP[3] = 2*c*c*h+c*h*h*h;
			_chanceExtraCP[4] = c*c*c+3*c*c*h*h+c*h*h*h*h;

			//_chanceExactCP[0] = h;
			//_chanceExactCP[1] = c+h*h;
			//_chanceExactCP[2] = 2*c*h+h*h*h;
			//_chanceExactCP[3] = c*c+3*c*h*h+h*h*h*h;
			//_chanceExactCP[4] = 3*c*c*h+4*c*h*h*h+h*h*h*h*h;

			//float total0 = _chanceExactCP[0] + _chanceExtraCP[0];
			//float total1 = _chanceExactCP[1] + _chanceExtraCP[1];
			//float total2 = _chanceExactCP[2] + _chanceExtraCP[2];
			//float total3 = _chanceExactCP[3] + _chanceExtraCP[3];
			//float total4 = _chanceExactCP[4] + _chanceExtraCP[4];

			//ToString();
		}

		public CatRotationCalculation GetRotationCalculations(bool useRake, bool useShred, bool useRip, int biteCP, int roarCP)
		{
			float totalEnergyAvailable = 100f + (10f * Duration);
			totalEnergyAvailable += ((float)Math.Ceiling((Duration - 10f) / (30f - Stats.TigersFuryCooldownReduction)) * Stats.BonusEnergyOnTigersFury);
			if (BerserkDuration > 0)
				totalEnergyAvailable += (float)Math.Ceiling((Duration - 10f) / 180f ) * (BerserkDuration + 7f) * 10f; //Assume 70 energy when you activate Berserk
			if (OmenOfClarity)
			{
				float oocProcs = ((3.5f * (Duration / 60f)) / AttackSpeed) * (1f - AvoidedAttacks); //Counts all OOCs as being used on the CPG. Should be made more accurate than that, but that's close at least
				if (ClearcastOnBleedChance > 0)
				{
					float dotTicks = (1f / 3f + 1f / 2f) * Duration;
					oocProcs += dotTicks * ClearcastOnBleedChance;
				}
				float cpgEnergyRaw = (useShred ? ShredStats.EnergyCost : MangleStats.EnergyCost) / CPGEnergyCostMultiplier;
				totalEnergyAvailable += oocProcs * (cpgEnergyRaw * (1f - AvoidedAttacks) + cpgEnergyRaw * AvoidedAttacks * 0.2f);
			}
			
			float totalCPAvailable = 0f;
			float averageGCD = 1f / (1f - AvoidedAttacks);
			float ripDurationUptime = RipStats.DurationUptime + (GlyphOfShred && useShred ? 6f : 0f);
			float ripDurationAverage = RipStats.DurationAverage + (GlyphOfShred && useShred ? 6f : 0f);
			float averageFinisherCP = 5f + _chanceExtraCP[4];
			
			#region Melee
			float meleeCount = Duration / AttackSpeed;
			#endregion

			#region Rake
			float rakeCount = 0;
			float rakeTotalEnergy = 0;
			float rakeCP = 0;
			if (useRake)
			{
				//When maintaining Mangle, lose 2 GCDs at the start of the fight to Mangle, Roar.
				float durationRakeable = Duration -(MaintainMangle ? 2f * averageGCD : 0f);
				//Lose some time due to Rip/Rake conflicts
				float rakeRipConflict = (1f / ripDurationAverage) * 0.5f * averageGCD;

				rakeCount = durationRakeable / (RakeStats.DurationAverage + rakeRipConflict);
				rakeTotalEnergy = rakeCount * RakeStats.EnergyCost;
				rakeCP = rakeCount * CPPerCPG;
				totalCPAvailable += rakeCP;
				totalEnergyAvailable -= rakeTotalEnergy;
			}
			#endregion

			#region Mangle
			float mangleCount = 0f;
			float mangleTotalEnergy = 0f;
			float mangleCP = 0f;
			if (MaintainMangle)
			{
				//Lose some time due to Mangle/Rake and Mangle/Rip conflicts
				float mangleRakeConflict = (1f / RakeStats.DurationAverage) * 0.5f * averageGCD;
				float mangleRipConflict = (1f / ripDurationAverage) * 0.5f * averageGCD;

				mangleCount = Duration / (MangleStats.DurationAverage - mangleRakeConflict - mangleRipConflict);
				mangleTotalEnergy = mangleCount * MangleStats.EnergyCost;
				mangleCP = mangleCount * CPPerCPG;
				totalCPAvailable += mangleCP;
				totalEnergyAvailable -= mangleTotalEnergy;
			}
			#endregion

			#region Combo Point Generator
			float cpgCount = 0f;
			float cpgEnergy = useShred ? ShredStats.EnergyCost : MangleStats.EnergyCost;
			float shredCount = 0f;
			#endregion

			#region Savage Roar
			float averageRoarCP = ((float)roarCP + 1f) * _chanceExtraCP[roarCP - 1]
				+ ((float)roarCP) * (1f - _chanceExtraCP[roarCP - 1]);

			//Lose some time due to Roar/Rake, Roar/Mangle, and Roar/Rip conflicts
			float roarRakeConflict = (1f / RakeStats.DurationAverage) * 0.5f * averageGCD;
			float roarMangleConflict = (1f / MangleStats.DurationAverage) * 0.5f * averageGCD;
			float roarRipConflict = (1f / ripDurationAverage) * 0.5f * (averageGCD * averageFinisherCP / CPPerCPG);

			float roarDuration = RoarStats.DurationAverage + 5f * Math.Min(5f, averageRoarCP)
				- roarRakeConflict - roarMangleConflict - roarRipConflict;
			float roarCount = Duration / roarDuration;
			float roarTotalEnergy = roarCount * RoarStats.EnergyCost;
			float roarCPRequired = roarCount * averageRoarCP;
			if (totalCPAvailable < roarCPRequired)
			{
				float cpToGenerate = roarCPRequired - totalCPAvailable;
				float cpgToUse = cpToGenerate / CPPerCPG;
				cpgCount += cpgToUse;
				totalEnergyAvailable -= cpgToUse * cpgEnergy;
				totalCPAvailable += cpToGenerate;
			}
			totalCPAvailable -= roarCPRequired;
			totalEnergyAvailable -= roarTotalEnergy;
			#endregion

			#region Damage Finishers
			float ripCount = 0f;
			float biteCount = 0f;
			if (useRip)
			{
				#region Rip
				//Lose GCDs at the start of the fight to Mangle/Rake, Roar, and enough CPGs to get 5CPG.
				float durationRipable = Duration - 2f * averageGCD - (averageGCD * (averageFinisherCP / CPPerCPG));


				float ripCountMax = durationRipable / ripDurationAverage;
				float ripsFromAvailableCP = Math.Min(ripCountMax, totalCPAvailable / averageFinisherCP);
				ripCount += ripsFromAvailableCP;
				totalCPAvailable -= averageFinisherCP * ripsFromAvailableCP;
				totalEnergyAvailable -= RipStats.EnergyCost * ripsFromAvailableCP;

				float ripCycleEnergy = (averageFinisherCP / CPPerCPG) * cpgEnergy + RipStats.EnergyCost;
				float ripsFromNewCP = Math.Min(ripCountMax - ripsFromAvailableCP, totalEnergyAvailable / ripCycleEnergy);

				ripCount += ripsFromNewCP;
				cpgCount += (averageFinisherCP / CPPerCPG) * ripsFromNewCP;
				totalEnergyAvailable -= ripCycleEnergy * ripsFromNewCP;
				#endregion
			}
			if (biteCP > 0)
			{
				#region Ferocious Bite
				float averageBiteCP = ((float)biteCP + 1f) * _chanceExtraCP[biteCP - 1]
				+ ((float)biteCP) * (1f - _chanceExtraCP[biteCP - 1]);
				float bitesFromAvailableCP = totalCPAvailable / averageBiteCP;
				biteCount += bitesFromAvailableCP;
				totalCPAvailable = 0;
				totalEnergyAvailable -= BiteStats.EnergyCost * bitesFromAvailableCP;

				float biteCycleEnergy = (averageBiteCP / CPPerCPG) * cpgEnergy + BiteStats.EnergyCost;
				float bitesFromNewCP = totalEnergyAvailable / biteCycleEnergy;

				biteCount += bitesFromNewCP;
				cpgCount += bitesFromNewCP * (averageBiteCP / CPPerCPG);
				totalEnergyAvailable = 0f;
				#endregion
			}
			#endregion

			#region Extra Energy turned into Combo Point Generators
			if (totalEnergyAvailable > 0)
			{
				cpgCount += totalEnergyAvailable / cpgEnergy;
				totalEnergyAvailable = 0f;
			}
			#endregion

			#region Damage Totals
			if (useShred) shredCount += cpgCount;
			else mangleCount += cpgCount;
			
			float meleeDamageTotal = meleeCount * MeleeStats.DamagePerSwing;
			float mangleDamageTotal = mangleCount * MangleStats.DamagePerSwing;
			float rakeDamageTotal = rakeCount * RakeStats.DamagePerSwing;
			float shredDamageTotal = shredCount * ShredStats.DamagePerSwing;
			float ripDamageTotal = ripCount * RipStats.DamagePerSwing * (ripDurationUptime / 12f);
			float biteDamageTotal = biteCount * (BiteStats.DamagePerSwing + BiteStats.DamagePerSwingPerCP * biteCP);

			float damageTotal = meleeDamageTotal + mangleDamageTotal + rakeDamageTotal + shredDamageTotal + ripDamageTotal + biteDamageTotal;
			#endregion

			//StringBuilder rotationName = new StringBuilder();
			//if (MaintainMangle || !useShred) rotationName.Append("Mangle+");
			//if (useRake) rotationName.Append("Rake+");
			//if (useShred) rotationName.Append("Shred+");
			//if (useRip) rotationName.Append("Rip+");
			//if (biteCP>0) rotationName.AppendFormat("Bite{0}+", biteCP);
			//rotationName.Append("Roar" + roarCP.ToString());
			
			return new CatRotationCalculation()
			{
				//Name = rotationName.ToString(),
				DPS = damageTotal / Duration,
				TotalDamage = damageTotal,
				
				MeleeCount = meleeCount,
				MangleCount = mangleCount,
				RakeCount = rakeCount,
				ShredCount = shredCount,
				RipCount = ripCount,
				BiteCount = biteCount,
				RoarCount = roarCount,

				//MeleeDamageTotal = meleeDamageTotal,
				//MangleDamageTotal = mangleDamageTotal,
				//RakeDamageTotal = rakeDamageTotal,
				//ShredDamageTotal = shredDamageTotal,
				//RipDamageTotal = ripDamageTotal,
				//BiteDamageTotal = biteDamageTotal,

				RoarCP = roarCP,
				BiteCP = biteCP,
			};

			//List<string> rotationName = new List<string>();
			//if (MaintainMangle || !useShred) rotationName.Add("Mangle");
			//if (useShred) rotationName.Add("Shred");
			//if (useRip) rotationName.Add("Rip");
			//if (useFerociousBite) rotationName.Add("Bite");
			//rotationName.Add("Roar" + roarCP.ToString());
			
			//return new CatRotationCalculation()
			//{ 
			//    Name = string.Join(" + ", rotationName.ToArray()),
			//    DPS = damageTotal / Duration,
				
			//    MeleeDamageTotal = meleeDamageTotal,
			//    MangleDamageTotal = mangleDamageTotal,
			//    RakeDamageTotal = rakeDamageTotal,
			//    ShredDamageTotal = shredDamageTotal,
			//    RipDamageTotal = ripDamageTotal,
			//    BiteDamageTotal = biteDamageTotal,
			//    DamageTotal = damageTotal,

			//    RoarCP = roarCP,
			//};
		}

		public class CatRotationCalculation
		{
			public float DPS { get; set; }
			public float TotalDamage { get; set; }
			
			//public Stats Stats { get; set; }
			//public float Duration { get; set; }
			//public float CPPerCPG { get; set; }
			//public bool MaintainMangle { get; set; }
			//public float MangleDuration { get; set; }
			//public float RipDuration { get; set; }
			//public float AttackSpeed { get; set; }
			//public bool OmenOfClarity { get; set; }

			//public float MeleeDamage { get; set; }
			//public float MangleDamage { get; set; }
			//public float ShredDamage { get; set; }
			//public float RakeDamage { get; set; }
			//public float RipDamage { get; set; }
			//public float BiteDamage { get; set; }

			//public float MangleEnergy { get; set; }
			//public float ShredEnergy { get; set; }
			//public float RakeEnergy { get; set; }
			//public float RipEnergy { get; set; }
			//public float BiteEnergy { get; set; }
			//public float RoarEnergy { get; set; }

			//public float MeleeDamageTotal { get; set; }
			//public float MangleDamageTotal { get; set; }
			//public float RakeDamageTotal { get; set; }
			//public float ShredDamageTotal { get; set; }
			//public float RipDamageTotal { get; set; }
			//public float BiteDamageTotal { get; set; }


			public float MeleeCount { get; set; }
			public float MangleCount { get; set; }
			public float ShredCount { get; set; }
			public float RakeCount { get; set; }
			public float RipCount { get; set; }
			public float BiteCount { get; set; }
			public float RoarCount { get; set; }

			public int RoarCP { get; set; }
			public int BiteCP { get; set; }

			public override string ToString()
			{
				StringBuilder rotation = new StringBuilder();
				if (MangleCount > 0) rotation.Append("Ma ");
				if (RakeCount > 0) rotation.Append("Ra ");
				if (ShredCount > 0) rotation.Append("Sh ");
				if (RipCount > 0) rotation.Append("Ri ");
				if (BiteCount > 0) rotation.AppendFormat("FB{0} ", BiteCP);
				rotation.Append("Ro" + RoarCP.ToString());

				rotation.AppendFormat("*Keep {0}cp Savage Roar up.\r\n", RoarCP);
				if (MangleCount > 0) rotation.Append("Keep Mangle up.\r\n");
				if (RakeCount > 0) rotation.Append("Keep Rake up.\r\n");
				if (RipCount > 0) rotation.Append("Keep 5cp Rip up.\r\n");
				if (BiteCount > 0) rotation.AppendFormat("Use {0}cp Ferocious Bites to spend extra combo points.\r\n", BiteCP);
				if (ShredCount > 0) rotation.Append("Use Shred for combo points.");
				else rotation.Append("Use Mangle for combo points.");

				return rotation.ToString();
			}
		}
	}
}
