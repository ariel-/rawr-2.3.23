using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Bear
{
	public class BearRotationCalculator
	{
		public float MeleeDamage { get; set; }
		public float MaulDamage { get; set; }
		public float MangleDamage { get; set; }
		public float SwipeDamage { get; set; }
		public float FaerieFireDamage { get; set; }
		public float LacerateDamage { get; set; }
		public float LacerateDotDamage { get; set; }

		public float MeleeThreat { get; set; }
		public float MaulThreat { get; set; }
		public float MangleThreat { get; set; }
		public float SwipeThreat { get; set; }
		public float FaerieFireThreat { get; set; }
		public float LacerateThreat { get; set; }
		public float LacerateDotThreat { get; set; }

		public float MangleCooldown { get; set; }
		public float MeleeSpeed { get; set; }

		public BearRotationCalculator(float meleeDamage, float maulDamage, float mangleDamage, float swipeDamage, float faerieFireDamage,
			float lacerateDamage, float lacerateDotDamage, float meleeThreat, float maulThreat, float mangleThreat, float swipeThreat,
			float faerieFireThreat, float lacerateThreat, float lacerateDotThreat, float mangleCooldown, float meleeSpeed)
		{
			MeleeDamage = meleeDamage;
			MaulDamage = maulDamage;
			MangleDamage = mangleDamage;
			SwipeDamage = swipeDamage;
			FaerieFireDamage = faerieFireDamage;
			LacerateDamage = lacerateDamage;
			LacerateDotDamage = lacerateDotDamage;

			MeleeThreat = meleeThreat;
			MaulThreat = maulThreat;
			MangleThreat = mangleThreat;
			SwipeThreat = swipeThreat;
			FaerieFireThreat = faerieFireThreat;
			LacerateThreat = lacerateThreat;
			LacerateDotThreat = lacerateDotThreat;

			MangleCooldown = mangleCooldown;
			MeleeSpeed = meleeSpeed;
		}

		private enum BearAttack
		{
			None,
			Mangle,
			Swipe,
			FaerieFire,
			Lacerate
		}

		/// <summary>
		/// Gets calculations about a bear rotation
		/// </summary>
		/// <param name="useMaul">Null = no auto or maul, false = auto, true = maul</param>
		/// <param name="useMangle">Whether to use Mangle</param>
		/// <param name="useSwipe">Whether to use Swipe</param>
		/// <param name="useFaerieFire">Whether to use Faerie Fire (Feral)</param>
		/// <param name="useLacerate">Whether to use Lacerate</param>
		public BearRotationCalculation GetRotationCalculations(bool? useMaul, bool useMangle, bool useSwipe, bool useFaerieFire, bool useLacerate)
		{
			//Dictionary<BearAttack, int> attackCounts = new Dictionary<BearAttack, int>();
			//attackCounts[BearAttack.Mangle] = attackCounts[BearAttack.FaerieFire] = 
			//    attackCounts[BearAttack.Lacerate] = attackCounts[BearAttack.Swipe] = 0;

			//float rotationDuration = 0f;
			//if (useMangle)
			//{
			//    int mangleCooldown = (int)Math.Round(MangleCooldown * 10f);
			//    if (mangleCooldown == 60)
			//    { //0 iMangle - MFLSMFSS/12
			//        rotationDuration = 12f;
					
			//        //M slots
			//        attackCounts[BearAttack.Mangle] = 2;

			//        //F slots
			//        attackCounts[useFaerieFire ? BearAttack.FaerieFire :
			//            (useSwipe ? BearAttack.Swipe :
			//            (useLacerate ? BearAttack.Lacerate :
			//            BearAttack.None))] += 2;

			//        //L slots
			//        attackCounts[useLacerate ? BearAttack.Lacerate :
			//            (useSwipe ? BearAttack.Swipe :
			//            BearAttack.None)] += 1;

			//        //S slots
			//        attackCounts[useSwipe ? BearAttack.Swipe :
			//            (useLacerate ? BearAttack.Lacerate :
			//            BearAttack.None)] += 3;
			//    }
			//    if (mangleCooldown == 55 || mangleCooldown == 50)
			//    { //1 or 2 iMangle - MFLSMFSS/11 WRONG
			//        rotationDuration = 11f;

			//        //M slots
			//        attackCounts[BearAttack.Mangle] = 2;

			//        //F slots
			//        attackCounts[useFaerieFire ? BearAttack.FaerieFire :
			//            (useSwipe ? BearAttack.Swipe :
			//            (useLacerate ? BearAttack.Lacerate :
			//            BearAttack.None))] += 2;

			//        //L slots
			//        attackCounts[useLacerate ? BearAttack.Lacerate :
			//            (useSwipe ? BearAttack.Swipe :
			//            BearAttack.None)] += 1;

			//        //S slots
			//        attackCounts[useSwipe ? BearAttack.Swipe :
			//            (useLacerate ? BearAttack.Lacerate :
			//            BearAttack.None)] += 3;
			//    }
			//    if (mangleCooldown == 45)
			//    {
			//        if (useFaerieFire)
			//        { //3 iMangle, with FF - MFLMSFMSLMFSMSFMLSMFSMLFMSS/40.5
			//            rotationDuration = 40.5f;

			//            //M slots
			//            attackCounts[BearAttack.Mangle] = 9;

			//            //F slots
			//            attackCounts[useFaerieFire ? BearAttack.FaerieFire :
			//                (useSwipe ? BearAttack.Swipe :
			//                (useLacerate ? BearAttack.Lacerate :
			//                BearAttack.None))] += 6;

			//            //L slots
			//            attackCounts[useLacerate ? BearAttack.Lacerate :
			//                (useSwipe ? BearAttack.Swipe :
			//                BearAttack.None)] += 4;

			//            //S slots
			//            attackCounts[useSwipe ? BearAttack.Swipe :
			//                (useLacerate ? BearAttack.Lacerate :
			//                BearAttack.None)] += 8;
			//        }
			//        else
			//        { //3 iMangle, without FF - MLSMSSMSLMSSMSS/22.5
			//            rotationDuration = 22.5f;

			//            //M slots
			//            attackCounts[BearAttack.Mangle] = 5;

			//            //L slots
			//            attackCounts[useLacerate ? BearAttack.Lacerate :
			//                (useSwipe ? BearAttack.Swipe :
			//                BearAttack.None)] += 2;

			//            //S slots
			//            attackCounts[useSwipe ? BearAttack.Swipe :
			//                (useLacerate ? BearAttack.Lacerate :
			//                BearAttack.None)] += 8;
			//        }
			//    }
			//}
			//else
			//{
			//    if (useFaerieFire)
			//    { //No Mangle, with FF - FLSSFSSS/12
					
			//    }
			//    else
			//    {
			//    }
			//}



			//36 GCDs, 54 sec
			BearAttack[] attacks = new BearAttack[36];

			if (useMangle)
			{
				for (int i = 0; i < attacks.Length; i++)
				{
					if (attacks[i] == BearAttack.None)
					{
						attacks[i] = BearAttack.Mangle;
						i += (int)Math.Ceiling((float)MangleCooldown / 1.5f) - 1;
					}
				}
			}

			if (useFaerieFire)
			{
				for (int i = 0; i < attacks.Length; i++)
				{
					if (attacks[i] == BearAttack.None)
					{
						attacks[i] = BearAttack.FaerieFire;
						i += 3;
					}
				}
			}

			if (useLacerate)
			{
				for (int i = 0; i < attacks.Length; i++)
				{
					if (attacks[i] == BearAttack.None)
					{
						attacks[i] = BearAttack.Lacerate;
						if (useSwipe)
							i += 4;
					}
				}
			}

			if (useSwipe)
			{
				for (int i = 0; i < attacks.Length; i++)
				{
					if (attacks[i] == BearAttack.None)
					{
						attacks[i] = BearAttack.Swipe;
					}
				}
			}

			int countMangle = 0, countFaerieFire = 0, countLacerate = 0, countSwipe = 0, countLacerateDot = useLacerate ? 18 : 0;
			for (int i = 0; i < attacks.Length; i++)
			{
				switch (attacks[i])
				{
					case BearAttack.Mangle: countMangle++; break;
					case BearAttack.Swipe: countSwipe++; break;
					case BearAttack.FaerieFire: countFaerieFire++; break;
					case BearAttack.Lacerate: countLacerate++; break;
				}
			}

			float attackDPS = (countMangle * MangleDamage + countSwipe * SwipeDamage + countFaerieFire * FaerieFireDamage +
				countLacerate * LacerateDamage + countLacerateDot * LacerateDotDamage) / 54f;
			float attackTPS = (countMangle * MangleThreat + countSwipe * SwipeThreat + countFaerieFire * FaerieFireThreat +
							countLacerate * LacerateThreat + countLacerateDot * LacerateDotThreat) / 54f;

			if (useMaul.HasValue && !useMaul.Value)
			{ //Just melee attacks, no mauls
				attackDPS += (MeleeDamage / MeleeSpeed);
				attackTPS += (MeleeThreat / MeleeSpeed);
			}
			else if (useMaul.HasValue && useMaul.Value)
			{ //Use mauls
				attackDPS += (MaulDamage / MeleeSpeed);
				attackTPS += (MaulThreat / MeleeSpeed);
			}

			StringBuilder strRotationName = new StringBuilder();
			if (useMaul.HasValue && !useMaul.Value) strRotationName.Append("Melee+");
			if (useMaul.HasValue && useMaul.Value) strRotationName.Append("Maul+");
			if (useMangle) strRotationName.Append("Mangle+");
			if (useSwipe) strRotationName.Append("Swipe+");
			if (useLacerate) strRotationName.Append("Lacerate+");
			if (useFaerieFire) strRotationName.Append("FaerieFire+");
			if (strRotationName.Length == 0) strRotationName.Append("None");
			else strRotationName.Length--;

			return new BearRotationCalculation() { Name = strRotationName.ToString(), DPS = attackDPS, TPS = attackTPS };
		}

		public class BearRotationCalculation
		{
			public string Name { get; set; }
			public float DPS { get; set; }
			public float TPS { get; set; }
		}
	}
}
