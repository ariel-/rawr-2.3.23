using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.DPSDK
{
    public class Rotation
    {
        public Type curRotationType = Type.Custom;
        

        private CalculationOptionsDPSDK.Presence _presence = CalculationOptionsDPSDK.Presence.Blood;
        public CalculationOptionsDPSDK.Presence presence
        {
            get { return _presence; }
            set { _presence = value; }
        }

        private int _presenceByIndex = 0;
        public int PresenceByIndex
        {
            get { return _presenceByIndex; }
            set
            {
                _presenceByIndex = value;
                if (_presenceByIndex == 0) presence = CalculationOptionsDPSDK.Presence.Blood;
                if (_presenceByIndex == 1) presence = CalculationOptionsDPSDK.Presence.Unholy;
                if (_presenceByIndex == 2) presence = CalculationOptionsDPSDK.Presence.Frost;
            }
        }
        private double _curRotationDuration = 0f;  // rotation duration in seconds
        public double CurRotationDuration
        {
            get { return _curRotationDuration; }
            set { _curRotationDuration = value; }
        }

        //disease info
        private double _avgDiseaseMult = 0f;

        public double AvgDiseaseMult
        {
            get { return _avgDiseaseMult; }
            set { _avgDiseaseMult = value; }
        }
        private double _numDisease = 0f;

        public double NumDisease
        {
            get { return _numDisease; }
            set { _numDisease = value; }
        }
        private double _diseaseUptime = 0f;

        public double DiseaseUptime
        {
            get { return _diseaseUptime; }
            set { _diseaseUptime = value; }
        }
        private double _gargoyleDuration = 0f;

        public double GargoyleDuration
        {
            get { return _gargoyleDuration; }
            set { _gargoyleDuration = value; }
        }
        
        private double _deathCoil = 0f;

        public double DeathCoil
        {
            get { return _deathCoil; }
            set { _deathCoil = value; }
        }
        private double _icyTouch = 0f;

        public double IcyTouch
        {
            get { return _icyTouch; }
            set { _icyTouch = value; }
        }
        private double _plagueStrike = 0f;

        public double PlagueStrike
        {
            get { return _plagueStrike; }
            set { _plagueStrike = value; }
        }
        private double _frostFever = 0f;

        public double FrostFever
        {
            get { return _frostFever; }
            set { _frostFever = value; }
        }
        private double _bloodPlague = 0f;

        public double BloodPlague
        {
            get { return _bloodPlague; }
            set { _bloodPlague = value; }
        }
        private double _scourgeStrike = 0f;

        public double ScourgeStrike
        {
            get { return _scourgeStrike; }
            set { _scourgeStrike = value; }
        }
        private double _frostStrike = 0f;

        public double FrostStrike
        {
            get { return _frostStrike; }
            set { _frostStrike = value; }
        }
        private double _howlingBlast = 0f;

        public double HowlingBlast
        {
            get { return _howlingBlast; }
            set { _howlingBlast = value; }
        }
        private double _obliterate = 0f;

        public double Obliterate
        {
            get { return _obliterate; }
            set { _obliterate = value; }
        }
        private double _deathStrike = 0f;

        public double DeathStrike
        {
            get { return _deathStrike; }
            set { _deathStrike = value; }
        }
        private double _bloodStrike = 0f;

        public double BloodStrike
        {
            get { return _bloodStrike; }
            set { _bloodStrike = value; }
        }
        private double _bloodBoil = 0f;

        private double _KMFS;
        public double KMFS
        {
            get { return _KMFS; }
            set { _KMFS = value; }
        }

        private double _KMRime;
        public double KMRime
        {
            get { return _KMRime; }
            set { _KMRime = value; }
        }

        private double _FFTick;
        public double FFTick
        {
            get { return _FFTick; }
            set { _FFTick = value; }
        }

        private double _BPTick;
        public double BPTick
        {
            get { return _BPTick; }
            set { _BPTick = value; }
        }

        public double BloodBoil
        {
            get { return _bloodBoil; }
            set { _bloodBoil = value; }
        }
        private double _heartStrike = 0f;

        public double HeartStrike
        {
            get { return _heartStrike; }
            set { _heartStrike = value; }
        }
        private double _dancingRuneWeapon = 0f;

        public double DancingRuneWeapon
        {
            get { return _dancingRuneWeapon; }
            set { _dancingRuneWeapon = value; }
        }
        private double _horn = 0f;

        public double Horn
        {
            get { return _horn; }
            set { _horn = value; }
        }
        private double _ghoulFrenzy = 0f;

        public double GhoulFrenzy
        {
            get { return _ghoulFrenzy; }
            set { _ghoulFrenzy = value; }
        }
        private double _pestilence = 0f;

        public double Pestilence
        {
            get { return _pestilence; }
            set { _pestilence = value; }
        }

        private Boolean _managedRP = false;

        public Boolean ManagedRP
        {
            get { return _managedRP; }
            set { _managedRP = value; }
        }
        private Boolean _pTRCalcs = false;

        public Boolean PTRCalcs
        {
            get { return _pTRCalcs; }
            set { _pTRCalcs = value; }
        }
        private double _gCDTime;

        public double GCDTime
        {
            get { return _gCDTime; }
            set { _gCDTime = value; }
        }
        private double _rP;

        public double RP
        {
            get { return _rP; }
            set { _rP = value; }
        }

        private double _weight;
        public double Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        public void applyWeight(double weight)
        {
            Weight = weight;
            AvgDiseaseMult *= weight;
            BloodBoil *= weight;
            BloodPlague *= weight;
            BloodStrike *= weight;
            BPTick *= weight;
            this.DeathCoil *= weight;
            this.DeathStrike *= weight;
            this.FFTick *= weight;
            this.FrostFever *= weight;
            this.FrostStrike *= weight;
            this.GhoulFrenzy *= weight;
            this.HeartStrike *= weight;
            this.Horn *= weight;
            this.HowlingBlast *= weight;
            this.IcyTouch *= weight;
            this.KMFS *= weight;
            this.KMRime *= weight;
            this.Obliterate *= weight;
            this.Pestilence *= weight;
            this.PlagueStrike *= weight;
            this.ScourgeStrike *= weight;
            this.NumDisease *= weight;
        }

        public void addRotation(Rotation other)
        {
            this.Weight += other.Weight;
            this.AvgDiseaseMult += other.AvgDiseaseMult;
            this.BloodBoil += other.BloodBoil;
            this.BloodPlague += other.BloodPlague;
            this.BloodStrike += other.BloodStrike;
            this.BPTick += other.BPTick;
            this.DancingRuneWeapon += other.DancingRuneWeapon;
            this.DeathCoil += other.DeathCoil;
            this.DeathStrike += other.DeathStrike;
            this.FFTick += other.FFTick;
            this.FrostFever += other.FrostFever;
            this.FrostStrike += other.FrostStrike;
            this.GhoulFrenzy += other.GhoulFrenzy;
            this.HeartStrike += other.HeartStrike;
            this.Horn += other.Horn;
            this.HowlingBlast += other.HowlingBlast;
            this.IcyTouch += other.IcyTouch;
            this.KMFS += other.KMFS;
            this.KMRime += other.KMRime;
            this.NumDisease += other.NumDisease;
            this.Obliterate += other.Obliterate;
            this.Pestilence += other.Pestilence;
            this.PlagueStrike += other.PlagueStrike;
            this.ScourgeStrike += other.ScourgeStrike;
        }

        public void copyRotation(Rotation other)
        {
            this.Weight = other.Weight;
            this.CurRotationDuration = other.CurRotationDuration;
            this.AvgDiseaseMult = other.AvgDiseaseMult;
            this.BloodBoil = other.BloodBoil;
            this.BloodPlague = other.BloodPlague;
            this.BloodStrike = other.BloodStrike;
            this.BPTick = other.BPTick;
            this.DancingRuneWeapon = other.DancingRuneWeapon;
            this.DeathCoil = other.DeathCoil;
            this.DeathStrike = other.DeathStrike;
            this.FFTick = other.FFTick;
            this.FrostFever = other.FrostFever;
            this.FrostStrike = other.FrostStrike;
            this.GhoulFrenzy = other.GhoulFrenzy;
            this.HeartStrike = other.HeartStrike;
            this.Horn = other.Horn;
            this.HowlingBlast = other.HowlingBlast;
            this.IcyTouch = other.IcyTouch;
            this.KMFS = other.KMFS;
            this.KMRime = other.KMRime;
            this.NumDisease = other.NumDisease;
            this.Obliterate = other.Obliterate;
            this.Pestilence = other.Pestilence;
            this.PlagueStrike = other.PlagueStrike;
            this.ScourgeStrike = other.ScourgeStrike;
            this.presence = other.presence;
            this.NumDisease = other.NumDisease;
        }

        public enum Type
        {            
            Custom, Blood, Frost, Unholy, Unknown,
        }

        public Rotation()
        {

            setRotation(Type.Unholy);
        }

        public Type GetRotationType(DeathKnightTalents t)
        {
            curRotationType = Type.Custom;
            const int indexBlood = 0; // start index of Blood Talents.
            const int indexFrost = 28; // start index of Frost Talents.
            const int indexUnholy = indexFrost + 29; // start index of Unholy Talents.
            int[] TalentCounter = new int[4];
            int index = indexBlood;
            foreach (int i in t.Data)
            {
                if (i > 0)
                {
                    // Blood
                    if (index < indexFrost)
                        TalentCounter[(int)Rotation.Type.Blood]++;
                    // Frost
                    else if ((indexFrost <= index) && (index < indexUnholy))
                    {
                        TalentCounter[(int)Rotation.Type.Frost]++;
                    }
                    // Unholy
                    else if (index >= indexUnholy)
                    {
                        TalentCounter[(int)Rotation.Type.Unholy]++;
                    }
                }
                index++;
            }
            if ((TalentCounter[(int)Rotation.Type.Blood] > TalentCounter[(int)Rotation.Type.Frost]) && (TalentCounter[(int)Rotation.Type.Blood] > TalentCounter[(int)Rotation.Type.Unholy]))
            {
                // Blood
                curRotationType = Rotation.Type.Blood;
            }
            else if ((TalentCounter[(int)Rotation.Type.Frost] > TalentCounter[(int)Rotation.Type.Blood]) && (TalentCounter[(int)Rotation.Type.Frost] > TalentCounter[(int)Rotation.Type.Unholy]))
            {
                // Frost
                curRotationType = Rotation.Type.Frost;
            }
            else if ((TalentCounter[(int)Rotation.Type.Unholy] > TalentCounter[(int)Rotation.Type.Frost]) && (TalentCounter[(int)Rotation.Type.Unholy] > TalentCounter[(int)Rotation.Type.Blood]))
            {
                // Unholy
                curRotationType = Rotation.Type.Unholy;
            }
            return curRotationType;
        }

        public float getMeleeSpecialsPerSecond()
        {
            double temp;
            temp = PlagueStrike + ScourgeStrike + FrostStrike + Obliterate + DeathStrike + BloodStrike + HeartStrike;
            temp = temp / CurRotationDuration;
            return (float)temp;
        }

        public float getSpellSpecialsPerSecond()
        {
            float temp;
            temp = (float)(DeathCoil + IcyTouch + HowlingBlast);
            temp = temp / (float)CurRotationDuration;
            return temp;
        }


        public float getRP(DeathKnightTalents talents, Character character)
        {
            bool fourT7 = character.ActiveBuffsContains("Scourgeborne Battlegear 4 Piece Bonus");
            bool GlyphofIT = talents.GlyphofIcyTouch;
            bool GlyphofFS = talents.GlyphofFrostStrike;

            RP = ((15 + (fourT7 ? 5 : 0) + 2.5f * talents.ChillOfTheGrave) * (Obliterate)) +
                ((15 + (fourT7 ? 5 : 0) + 2.5f * talents.Dirge) * (ScourgeStrike)) +
                ((15 + (fourT7 ? 5 : 0) + 2.5f * talents.Dirge) * (DeathStrike)) +
                ((10 + 2.5f * talents.Dirge) * (PlagueStrike)) +
                (10 * (BloodStrike + HeartStrike + Pestilence + GhoulFrenzy)) +
                ((10 + 2.5f * talents.ChillOfTheGrave) * (IcyTouch)) +
                ((15 + 2.5f * talents.ChillOfTheGrave) * HowlingBlast) +
                (10 * Horn) +
                ((CurRotationDuration / 5f)*talents.Butchery);

            if (ManagedRP)
            {
                RP = manageRPDumping(talents, (float)RP);
            }
            else
            {
                RP -= ((40 * DeathCoil) +
                    ((GlyphofFS ? 32 : 40) * FrostStrike));
            }
            return (float)RP;
        }

        public float manageRPDumping(DeathKnightTalents talents, float RP)
        {
			if (talents.DancingRuneWeapon > 0f)
			{
				RP -= (float)CurRotationDuration * (60f / 90f);
			}
			if (talents.SummonGargoyle > 0f)
			{
                RP -= (float)(CurRotationDuration * (60f / 180f));
			}
			if (talents.FrostStrike > 0f)
			{
				FrostStrike = RP / (talents.GlyphofFrostStrike ? 32f : 40f);
				DeathCoil = 0f;
				RP = 0f;
			}
			else
			{
				DeathCoil = RP / 40f;
				FrostStrike = 0f;
				RP = 0f;
			}
            return RP;
        }

        public float getGCDTime()
        {
            if (presence.Equals(CalculationOptionsDPSDK.Presence.Unholy))
            {
                GCDTime = DeathCoil + IcyTouch + PlagueStrike + ScourgeStrike +
                    FrostStrike + HowlingBlast + Obliterate + DeathStrike + BloodStrike +
                    HeartStrike + Horn + Pestilence + GhoulFrenzy;
            }
            else if (presence.Equals(CalculationOptionsDPSDK.Presence.Blood))
            {
                GCDTime = 1.5f * (PlagueStrike + ScourgeStrike + FrostStrike + Obliterate + DeathStrike +
                    BloodStrike + HeartStrike);
                GCDTime += 1.5f * (DeathCoil + IcyTouch + HowlingBlast + Horn + Pestilence + GhoulFrenzy);
                // this does not currently account for haste, and I don't think it is possible in the current design.
            }
            return (float)GCDTime;
        }

        public void setRotation(Type t)
        {
            curRotationType = t;
            switch (curRotationType)
            {
                case Type.Blood:
                    NumDisease = 2f;
                    DiseaseUptime = 100f;
                    DeathCoil = 3f;
                    IcyTouch = 1f;
                    PlagueStrike = 1f;
                    ScourgeStrike = 0f;
                    ManagedRP = true;
                    Horn = 0f;
                    FrostStrike = 0f;
                    HowlingBlast = 0f;
                    Obliterate = 0f;
                    DeathStrike = 2f;
                    BloodStrike = 0f;
                    HeartStrike = 6f;
                    CurRotationDuration = 360f;
                    DancingRuneWeapon = 90f;
                    GargoyleDuration = 0f;
                    DeathStrike = 2f;
                    GhoulFrenzy = 0f;
                    Pestilence = 0f;
                    presence = CalculationOptionsDPSDK.Presence.Blood;
                    break;
                case Type.Frost:
                    NumDisease = 2f;
                    DiseaseUptime = 100f;
                    DeathCoil = 0f;
                    IcyTouch = 1f;
                    PlagueStrike = 1f;
                    ScourgeStrike = 0f;
                    ManagedRP = true;
                    Horn = 0f;
                    FrostStrike = 3f;
                    HowlingBlast = 0f;
                    Obliterate = 4f;
                    BloodStrike = 2f;
                    HeartStrike = 0f;
                    DancingRuneWeapon = 0f;
                    CurRotationDuration = 360f;
                    GargoyleDuration = 0f;
                    DeathStrike = 0f;
					Pestilence = 0f;
                    GhoulFrenzy = 0f;
                    presence = CalculationOptionsDPSDK.Presence.Blood;
                    break;
                case Type.Unholy:
                    NumDisease = 3f;
                    DiseaseUptime = 100f;
                    DeathCoil = 3.5f;
                    IcyTouch = 1f;
                    PlagueStrike = 1f;
                    ScourgeStrike = 4f;
                    ManagedRP = true;
                    FrostStrike = 0f;
                    HowlingBlast = 0f;
                    Obliterate = 0f;
                    BloodStrike = 2f;
                    HeartStrike = 0f;
                    DancingRuneWeapon = 0f;
                    CurRotationDuration = 360f;
                    Horn = 1f;
                    GargoyleDuration = 30f;
                    DeathStrike = 0f;
                    GhoulFrenzy = 0f;
                    Pestilence = 0f;
                    presence = CalculationOptionsDPSDK.Presence.Blood;
                    break;
                case Type.Custom:
                   /* numDisease = 0f;
                    diseaseUptime = 0f;
                    DeathCoil = 0f;
                    IcyTouch = 0f;
                    PlagueStrike = 0f;
                    ScourgeStrike = 0f;
                    UnholyBlight = 0f;
                    FrostStrike = 0f;
                    HowlingBlast = 0f;
                    Obliterate = 0f;
                    BloodStrike = 0f;
                    HeartStrike = 0f;
                    DancingRuneWeapon = 0f;
                    curRotationDuration = 0f;
                    GargoyleDuration = 0f;
                    presence = CalculationOptionsDPSDK.Presence.Blood;*/
                    break;
            }
        }
    }
}
