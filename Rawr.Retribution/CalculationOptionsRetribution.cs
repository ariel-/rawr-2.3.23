using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Rawr.Retribution
{
    public class CalculationOptionsRetribution : ICalculationOptionBase, INotifyPropertyChanged
    {

        public string GetXml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsRetribution));
            StringBuilder xml = new StringBuilder();
            System.IO.StringWriter writer = new System.IO.StringWriter(xml);
            serializer.Serialize(writer, this);
            return xml.ToString();
        }

        public CalculationOptionsRetribution()
        {
            // Tab - Fight Parameters
            fightLength = 5f;
            mob = MobType.Humanoid;
            seal = SealOf.Vengeance;
            targetLevel = 83;
            timeUnder20 = .18f;
            stackTrinketReset = 0;
            targets = 1f;
            inFront = 0f;
            consEff = 1f;
            bloodlust = true;
            hoREff = 0f;
            targetSwitches = 0f;

            // Tab - Rotation
            simulateRotation = true;
            // Tab - Rotation - FCFS
            rotations = new List<Ability[]>();
            delay = .05f;
            wait = .05f;
            // Tab - Rotation - Effective CD's
            SetEffectiveAbilityCooldown(Ability.Judgement, 7.1f);
            SetEffectiveAbilityCooldown(Ability.CrusaderStrike, 7.1f);
            SetEffectiveAbilityCooldown(Ability.DivineStorm, 10.5f);
            SetEffectiveAbilityCooldown(Ability.Consecration, 10.5f);
            SetEffectiveAbilityCooldown(Ability.Exorcism, 18f);

            SetEffectiveAbilityCooldownAfter20PercentHealth(Ability.Judgement, 7.1f);
            SetEffectiveAbilityCooldownAfter20PercentHealth(Ability.CrusaderStrike, 7.1f);
            SetEffectiveAbilityCooldownAfter20PercentHealth(Ability.DivineStorm, 12.5f);
            SetEffectiveAbilityCooldownAfter20PercentHealth(Ability.Consecration, 12.5f);
            SetEffectiveAbilityCooldownAfter20PercentHealth(Ability.Exorcism, 25f);
            SetEffectiveAbilityCooldownAfter20PercentHealth(Ability.HammerOfWrath, 6.4f);

            // Tab - Misc
            experimental = "";
        }

        public CalculationOptionsRetribution Clone()
        {
            CalculationOptionsRetribution clone = new CalculationOptionsRetribution();

            // Tab - Fight Parameters
            clone.FightLength = FightLength;
            clone.Mob = Mob;
            clone.Seal = Seal;
            clone.TargetLevel = TargetLevel;
            clone.TimeUnder20 = TimeUnder20;
            clone.StackTrinketReset = StackTrinketReset;
            clone.Targets = Targets;
            clone.InFront = InFront;
            clone.ConsEff = ConsEff;
            clone.Bloodlust = Bloodlust;
            clone.HoREff = HoREff;
            clone.TargetSwitches = TargetSwitches;

            // Tab - Rotation
            clone.SimulateRotation = SimulateRotation;

            // Tab - Rotation - FCFS
            clone.Rotations = new List<Ability[]>();
            foreach (Ability[] rotation in Rotations)
                clone.Rotations.Add((Ability[])rotation.Clone());

            clone.Delay = Delay;
            clone.Wait = Wait;
            // Tab - Rotation - Effective CD's
            clone.JudgeCD = JudgeCD;
            clone.CSCD = CSCD;
            clone.DSCD = DSCD;
            clone.ConsCD = ConsCD;
            clone.ExoCD = ExoCD;

            clone.JudgeCD20 = JudgeCD20;
            clone.CSCD20 = CSCD20;
            clone.DSCD20 = DSCD20;
            clone.ConsCD20 = ConsCD20;
            clone.ExoCD20 = ExoCD20;
            clone.HoWCD20 = HoWCD20;

            // Tab - Misc
            clone.Experimental = experimental;

            return clone;
        }

        #region Property 'CacheVars'

        // Tab - Fight Parameters
        private float fightLength;
        public float FightLength
        {
            get { return fightLength; }
            set { fightLength = value; OnPropertyChanged("FightLength"); }
        }

        private MobType mob;
        public MobType Mob
        {
            get { return mob; }
            set { mob = value; OnPropertyChanged("Mob"); }
        }

        private SealOf seal;
        public SealOf Seal
        {
            get { return seal; }
            set { seal = value; OnPropertyChanged("Seal"); }
        }

        private int targetLevel;
        public int TargetLevel
        {
            get { return targetLevel; }
            set { targetLevel = value; OnPropertyChanged("TargetLevel"); }
        }

        private float timeUnder20;
        public float TimeUnder20
        {
            get { return timeUnder20; }
            set { timeUnder20 = value; OnPropertyChanged("TimeUnder20"); }
        }

        private int stackTrinketReset;
        public int StackTrinketReset
        {
            get { return stackTrinketReset; }
            set { stackTrinketReset = value; OnPropertyChanged("StackTrinketReset"); }
        }

        private float targets;
        public float Targets
        {
            get { return targets; }
            set { targets = value; OnPropertyChanged("Targets"); }
        }

        private float inFront;
        public float InFront
        {
            get { return inFront; }
            set { inFront = value; OnPropertyChanged("InFront"); }
        }

        private float consEff;
        public float ConsEff
        {
            get { return consEff; }
            set { consEff = value; OnPropertyChanged("ConsEff"); }
        }

        private bool bloodlust;
        public bool Bloodlust
        {
            get { return bloodlust; }
            set { bloodlust = value; OnPropertyChanged("Bloodlust"); }
        }

        private float hoREff;
        public float HoREff
        {
            get { return hoREff; }
            set { hoREff = value; OnPropertyChanged("HoREff"); }
        }

        private float targetSwitches;
        public float TargetSwitches
        {
            get { return targetSwitches; }
            set { targetSwitches = value; OnPropertyChanged("TargetSwitches"); }
        }

        // Tab - Rotation
        private bool simulateRotation;    // FCFS Simulator (true) or Effective CD's (false)
        public bool SimulateRotation
        {
            get { return simulateRotation; }
            set { simulateRotation = value; OnPropertyChanged("SimulateRotation"); OnPropertyChanged("EffectiveCD"); }
        }

        // Tab - Rotation - FCFS
        private List<Ability[]> rotations;
        public List<Ability[]> Rotations
        {
            get { return rotations; }
            set { rotations = value; OnPropertyChanged("Rotations"); }
        }

        private float delay;
        public float Delay
        {
            get { return delay; }
            set { delay = value; OnPropertyChanged("Delay"); }
        }

        private float wait;
        public float Wait
        {
            get { return wait; }
            set { wait = value; OnPropertyChanged("Wait"); }
        }

        // Tab - Rotation - Effective CD's
        [XmlIgnore]
        public bool EffectiveCD
        {
            get { return !SimulateRotation; }
            set { SimulateRotation = !value; OnPropertyChanged("SimulateRotation"); OnPropertyChanged("EffectiveCD"); }
        }

        private float[] effectiveAbilityCooldowns = new float[(int)Ability.Last + 1];
        private float[] effectiveAbilityCooldownsAfter20PercentHealth = new float[(int)Ability.Last + 1];

        public float JudgeCD
        {
            get { return GetEffectiveAbilityCooldown(Ability.Judgement); }
            set 
            { 
                SetEffectiveAbilityCooldown(Ability.Judgement, value); 
                OnPropertyChanged("JudgeCD"); 
            }
        }

        public float CSCD
        {
            get { return GetEffectiveAbilityCooldown(Ability.CrusaderStrike); }
            set 
            {
                SetEffectiveAbilityCooldown(Ability.CrusaderStrike, value);
                OnPropertyChanged("CSCD"); 
            }
        }

        public float DSCD
        {
            get { return GetEffectiveAbilityCooldown(Ability.DivineStorm); }
            set 
            {
                SetEffectiveAbilityCooldown(Ability.DivineStorm, value);
                OnPropertyChanged("DSCD"); 
            }
        }

        public float ConsCD
        {
            get { return GetEffectiveAbilityCooldown(Ability.Consecration); }
            set 
            { 
                SetEffectiveAbilityCooldown(Ability.Consecration, value);
                OnPropertyChanged("ConsCD"); 
            }
        }

        public float ExoCD
        {
            get { return GetEffectiveAbilityCooldown(Ability.Exorcism); }
            set 
            { 
                SetEffectiveAbilityCooldown(Ability.Exorcism, value); 
                OnPropertyChanged("ExoCD"); 
            }
        }

        public float JudgeCD20
        {
            get { return GetEffectiveAbilityCooldownAfter20PercentHealth(Ability.Judgement); }
            set 
            { 
                SetEffectiveAbilityCooldownAfter20PercentHealth(Ability.Judgement, value); 
                OnPropertyChanged("JudgeCD20"); 
            }
        }

        public float CSCD20
        {
            get { return GetEffectiveAbilityCooldownAfter20PercentHealth(Ability.CrusaderStrike); }
            set 
            { 
                SetEffectiveAbilityCooldownAfter20PercentHealth(Ability.CrusaderStrike, value); 
                OnPropertyChanged("CSCD20"); 
            }
        }

        public float DSCD20
        {
            get { return GetEffectiveAbilityCooldownAfter20PercentHealth(Ability.DivineStorm); }
            set 
            { 
                SetEffectiveAbilityCooldownAfter20PercentHealth(Ability.DivineStorm, value); 
                OnPropertyChanged("DSCD20"); 
            }
        }

        public float ConsCD20
        {
            get { return GetEffectiveAbilityCooldownAfter20PercentHealth(Ability.Consecration); }
            set 
            { 
                SetEffectiveAbilityCooldownAfter20PercentHealth(Ability.Consecration, value); 
                OnPropertyChanged("ConsCD20"); 
            }
        }

        public float ExoCD20
        {
            get { return GetEffectiveAbilityCooldownAfter20PercentHealth(Ability.Exorcism); }
            set 
            { 
                SetEffectiveAbilityCooldownAfter20PercentHealth(Ability.Exorcism, value); 
                OnPropertyChanged("ExoCD20"); 
            }
        }

        public float HoWCD20
        {
            get { return GetEffectiveAbilityCooldownAfter20PercentHealth(Ability.HammerOfWrath); }
            set 
            { 
                SetEffectiveAbilityCooldownAfter20PercentHealth(Ability.HammerOfWrath, value); 
                OnPropertyChanged("HoWCD20"); 
            }
        }

        // Tab - Misc
        private string experimental;
        public string Experimental
        {
            get { return experimental; }
            set { experimental = value; OnPropertyChanged("Experimental"); }
        }

        #endregion

        public float GetEffectiveAbilityCooldown(Ability ability)
        {
            return effectiveAbilityCooldowns[(int)ability];
        }

        public float GetEffectiveAbilityCooldownAfter20PercentHealth(Ability ability)
        {
            return effectiveAbilityCooldownsAfter20PercentHealth[(int)ability];
        }

        private void SetEffectiveAbilityCooldown(Ability ability, float effectiveCooldown)
        {
            effectiveAbilityCooldowns[(int)ability] = effectiveCooldown;
        }

        private void SetEffectiveAbilityCooldownAfter20PercentHealth(Ability ability, float effectiveCooldown)
        {
            effectiveAbilityCooldownsAfter20PercentHealth[(int)ability] = effectiveCooldown;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
        }
        #endregion
    }
}