using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Rawr.DPSWarr.Markov;
using Rawr.Base;
using Rawr.Bosses;

namespace Rawr.DPSWarr.Markov
{
    class BloodSurgeMatrix
    {
        private double[] _procChances;
        private double _proc1;
        private double _proc2;

        public double ProcChances(int index)
        {
            if (index < 0 || index > 2) return 0;
            return _procChances[index];
        }
        
        /// <summary>
        /// Constructor for the Matrix
        /// </summary>
        /// <param name="proc1">The chance that Bloodsurge procs (ie 20%)</param>
        /// <param name="proc2">The percentage of Bloodsurge procs that are hasted (ie 20%)</param>
        public BloodSurgeMatrix(double proc1, double proc2)
        {
            _proc1 = proc1;
            _proc2 = proc2;

            _procChances = new double[] {1, 0, 0};
        }

        public void SetBaseChance(double procChance)
        {
            _procChances[0] = 1 - procChance;
            _procChances[1] = procChance - procChance * _proc2;
            _procChances[2] = procChance * _proc2;
        }

        /// <summary>
        /// Adds an ability to the matrix
        /// </summary>
        /// <param name="chance1">The chance that the attack lands</param>
        /// <param name="numActivates">The number of attacks made</param>
        public void AddAbility(double chanceLand, double numActivates)
        {
            double oldProcChance = _procChances[1] + _procChances[2];
            double newProcChance = _proc1 * chanceLand;
            double finalProc_any = 1 - (1 - oldProcChance) * Math.Pow(1 - newProcChance, numActivates);

            _procChances[0] = 1 - finalProc_any;
            _procChances[1] = finalProc_any - finalProc_any * _proc2;
            _procChances[2] = finalProc_any * _proc2;
        }

        /// <summary>
        /// Adds an ability to the matrix
        /// </summary>
        /// <param name="chance">The chance that the attack lands</param>
        public void AddAbility(double chance)
        {
            AddAbility(chance, 1);
            //double[] newChances = new double[] { 1 - chance2 - chance1, chance1, chance2 };
            //ResetMatrix(newChances);
        }

        private void ResetMatrix(double[] chances)
        {
            double[] ret = new double[3];
            
            ret[0] = _procChances[0] * chances[0];

            ret[1] = _procChances[1] * (chances[0] + chances[1]) + _procChances[0] * chances[1];
                        
            ret[2] = 1 - ret[1] - ret[0];

            _procChances = ret;
        }
    }
}
