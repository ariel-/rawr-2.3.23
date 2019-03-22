using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Retribution
{
    public class SimulatorAbility
    {

        public SimulatorAbility(int cooldown, int globalCooldown)
        {
			Cooldown = cooldown;
            GlobalCooldown = globalCooldown;
            FirstUse = -1;
            LastUse = -1;
            NextUse = 0;
        }

        public static int Wait { get; set; }
        public static int Delay { get; set; }

        public int GlobalCooldown { get; set; }
        public int Cooldown { get; set; }
        public int FirstUse { get; set; }
        public int LastUse { get; set; }
        public int NextUse { get; set; }
		public int Uses { get; set; }

        public void ResetCooldown(int currentTime)
        {
            // CD reset will be visible only after delay
            NextUse = Math.Min(NextUse, currentTime + Delay);
        }

        /// <summary>
        /// Uses the ability next time it's available.
        /// </summary>
        /// <param name="currentTime">Current time</param>
        /// <returns>GCD finish time</returns>
        public int UseAbility(int currentTime)
        {
			int usageTime = currentTime + Delay;

            if (FirstUse < 0) 
                FirstUse = usageTime;
            LastUse = usageTime;
            NextUse = usageTime + Cooldown;
            Uses++;
            return usageTime + GlobalCooldown;
        }
		
        public bool ShouldAbilityBeUsedNext(int currentTime)
        {
            return NextUse <= currentTime + Wait;
        }

		public bool CanAbilityBeUsedNow(int currentTime)
        {
            return currentTime >= NextUse;
        }

        public int GetNextUseTime(int currentTime)
        {
            return Math.Max(currentTime, NextUse);
        }

        public float EffectiveCooldown() 
        { 
            return  ((float)(LastUse - FirstUse)) / (Uses - 1); 
        }

    }
}