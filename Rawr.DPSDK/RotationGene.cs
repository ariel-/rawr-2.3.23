using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.DPSDK
{
    class RotationGene
    {

        private Rotation _rotation;
        public Rotation Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public RotationGene(int availableBlood, int availableFrost, int availableUnholy, DeathKnightTalents talents)
        {
            Rotation = new Rotation();
            Rotation.curRotationType = Rotation.Type.Custom;
            Rotation.ManagedRP = true;
            Random rand = new Random((int)(CalculationsDPSDK.hawut+0.5d));
            CalculationsDPSDK.hawut -= rand.NextDouble() * DateTime.Now.ToOADate() / 1000000;
            double nextRand;
            Rotation.presence = (rand.NextDouble() > .5 ? CalculationOptionsDPSDK.Presence.Blood : CalculationOptionsDPSDK.Presence.Unholy);
            nextRand = rand.NextDouble();
            #region blood runes
            if (nextRand > 0.5d && availableBlood > 0d)
            {
                Rotation.Pestilence = 1;
                availableBlood--;
            }
            nextRand = rand.NextDouble();
            if (nextRand < 1d / 3d)
            {
                Rotation.BloodStrike = availableBlood;
                Rotation.HeartStrike = 0;
                Rotation.BloodBoil = 0;
            }
            else if (nextRand < 2d / 3d)
            {
                Rotation.BloodStrike = 0;
                Rotation.HeartStrike = availableBlood;
                Rotation.BloodBoil = 0;
            }
            else
            {
                Rotation.BloodStrike = 0;
                Rotation.HeartStrike = 0;
                Rotation.BloodBoil = availableBlood;
            }
            #endregion

            #region frost and unholy runes
            nextRand = rand.NextDouble();
            if (nextRand > 0.5d && availableFrost > 0d)
            {
                Rotation.IcyTouch = 1;
                availableFrost--;
            }
            else
            {
                Rotation.IcyTouch = 0;
            }
            nextRand = rand.NextDouble();
            if (nextRand > 0.5d && availableUnholy > 0d)
            {
                Rotation.PlagueStrike = 1;
                availableUnholy--;
            }
            else
            {
                Rotation.PlagueStrike = 0;
            }
            nextRand = rand.NextDouble();
            if (nextRand < .25d)
            {
                Rotation.DeathStrike = (availableUnholy + availableFrost) / 2f;
                Rotation.ScourgeStrike = 0;
                Rotation.Obliterate = 0;
            }
            else if (nextRand < .5d)
            {
                Rotation.DeathStrike = 0;
                Rotation.ScourgeStrike = (availableFrost + availableUnholy) / 2f;
                Rotation.Obliterate = 0;
            }
            else if (nextRand < .75d)
            {
                Rotation.DeathStrike = 0;
                Rotation.ScourgeStrike = 0;
                Rotation.Obliterate = (availableUnholy + availableFrost) / 2f;
            }
            else
            {
                Rotation.DeathStrike = 0;
                Rotation.ScourgeStrike = 0;
                Rotation.Obliterate = 0;
                Rotation.PlagueStrike += availableUnholy;
                Rotation.IcyTouch += availableFrost;
            }
            #endregion
            Rotation.AvgDiseaseMult = (Rotation.IcyTouch > 0 ? 1 : 0) + (Rotation.PlagueStrike > 0 ? 1 : 0);
            if (talents.CryptFever > 0 && (Rotation.IcyTouch > 0 || Rotation.PlagueStrike > 0))
            {
                Rotation.AvgDiseaseMult++;
            }
            double minDuration = availableBlood + availableFrost + availableUnholy;
            minDuration /= 6;
            minDuration *= 10 - (Rotation.presence == CalculationOptionsDPSDK.Presence.Unholy ? talents.ImprovedUnholyPresence * .5 : 0);
        }

        public RotationGene(RotationGene father, RotationGene mother)
        {
        }
        public void MutateGene()
        {
        }
    }
}
