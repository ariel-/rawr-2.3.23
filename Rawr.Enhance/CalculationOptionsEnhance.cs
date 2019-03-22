using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Rawr.Enhance
{
#if !SILVERLIGHT
	[Serializable]
#endif
    public class CalculationOptionsEnhance : ICalculationOptionBase, INotifyPropertyChanged
	{
        // Boss parameters
        private BossHandler _boss = new BossHandler();
        private string _bossName = "Custom";
        private int _targetLevel = 83;
        private float _targetArmor = StatConversion.NPC_ARMOR[83 - 80];
        private bool _inBack = true;
        private int _inBackPerc = 100;

        // General parameters
        private int _averageLag = 250;
        private int _reactionTime = 250;
        private string _mainhandImbue = "Windfury";
        private string _offhandImbue = "Flametongue";
        private string _calculationToGraph = "DPS Rating";
        private float _fightLength = 7.0f;
        private float _fightLengthMultiplier = 5.0f;
        private float _targetFireResistance = 0;
        private float _targetNatureResistance = 0;
        private float _minManaSR = 1250;
        private bool _multipleTargets = false;
        private int _additionalTargets = 2;
        private float _additionalTargetPercent = 0.25f;
        private bool _baseStatOption = true;
        private bool _useMana = false;
        private bool _showExportMessageBox = true;
        private bool[] _statsList = new bool[] { true, true, true, true, true, true, true, true, true, true };
        private SerializableDictionary<EnhanceAbility, Priority> _priorityList;
        // this can be read from multiple threads, make sure all modifications are only done from
        // when no calculations are going on so we can avoid locking
        // make sure it is updated whenever changes to _priorityList are made
        private List<KeyValuePair<EnhanceAbility, Priority>> _sortedList;

        public CalculationOptionsEnhance()
        {
            _priorityList = SetPriorityDefaults();
            SortPriorities();
        }

        #region Getter/Setter
        public string BossName { get { return _bossName; } set { _bossName = value; OnPropertyChanged("BossName"); } }
        public int TargetLevel { get { return _targetLevel; } set { _targetLevel = value; OnPropertyChanged("TargetLevel"); } }
        public float TargetArmor { get { return _targetArmor; } set { _targetArmor = value; OnPropertyChanged("TargetArmor"); } }
        public bool InBack { get { return _inBack; } set { _inBack = value; OnPropertyChanged("InBack"); } }
        public int InBackPerc { get { return _inBackPerc; } set { _inBackPerc = value; OnPropertyChanged("InBackPerc"); } }
        public int AverageLag { get { return _averageLag; } set { _averageLag = value; OnPropertyChanged("AverageLag"); } }
        public int ReactionTime { get { return _reactionTime; } set { _reactionTime = value; OnPropertyChanged("ReactionTime"); } }
        public float MinManaSR { get { return _minManaSR; } set { _minManaSR = value; OnPropertyChanged("MinManaSR"); } }
        public string MainhandImbue { get { return _mainhandImbue; } set { _mainhandImbue = value; OnPropertyChanged("MainhandImbue"); } }
        public string OffhandImbue { get { return _offhandImbue; } set { _offhandImbue = value; OnPropertyChanged("OffhandImbue"); } }
        public string CalculationToGraph { get { return _calculationToGraph; } set { _calculationToGraph = value; OnPropertyChanged("CalculationToGraph"); } }
        public float FightLength { get { return _fightLength; } set { _fightLength = value; OnPropertyChanged("FightLength"); } }
        public float FightLengthMultiplier { get { return _fightLengthMultiplier; } set { _fightLengthMultiplier = value; OnPropertyChanged("FightLengthMultiplier"); } }
        public float TargetFireResistance { get { return _targetFireResistance; } set { _targetFireResistance = value; OnPropertyChanged("TargetFireResistance"); } }
        public float TargetNatureResistance { get { return _targetNatureResistance; } set { _targetNatureResistance = value; OnPropertyChanged("TargetNatureResistance"); } }
        public bool Magma { get { return PriorityInUse(EnhanceAbility.MagmaTotem); } }
        public bool Searing { get { return PriorityInUse(EnhanceAbility.SearingTotem); } }
        public bool FireElemental { get { return PriorityInUse(EnhanceAbility.FireElemental); } }
        public bool BaseStatOption { get { return _baseStatOption; } set { _baseStatOption = value; OnPropertyChanged("BaseStatOption"); } }
        //public bool UseMana { get { return _useMana; } set { _useMana = value; OnPropertyChanged("UseMana"); } }
        public bool UseMana { get { return _useMana; } set { _useMana = false; OnPropertyChanged("UseMana"); } }
        public bool ShowExportMessageBox { get { return _showExportMessageBox; } set { _showExportMessageBox = value; OnPropertyChanged("ShowExportMessageBox"); } }
        public bool MultipleTargets { get { return _multipleTargets; } set { _multipleTargets = value; OnPropertyChanged("MultipleTargets"); } }
        public int AdditionalTargets { get { return _additionalTargets; } set { _additionalTargets = value; OnPropertyChanged("AdditionalTargets"); } }
        public float AdditionalTargetPercent { get { return _additionalTargetPercent; } set { _additionalTargetPercent = value; OnPropertyChanged("AdditionalTargetPercent"); } }
        public bool[] StatsList { get { return _statsList; } set { _statsList = value; OnPropertyChanged("StatsList"); } }
        public SerializableDictionary<EnhanceAbility, Priority> PriorityList { get { return _priorityList; } set { _priorityList = value; SortPriorities(); OnPropertyChanged("PriorityList"); } }
        
        public string GetXml()
        {
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsEnhance));
            StringBuilder xml = new StringBuilder();
            System.IO.StringWriter writer = new System.IO.StringWriter(xml);
            serializer.Serialize(writer, this);
            return xml.ToString();
        }
        #endregion

        public void SetBoss(BossHandler boss) 
        {
            _boss = boss;
            _bossName = boss.Name;
            _targetLevel = boss.Level;
            _targetArmor = (int)boss.Armor;
            _fightLength = boss.BerserkTimer / 60f;
            _inBack = ((_inBackPerc = (int)(boss.InBackPerc_Melee * 100f)) != 0);
            _targetFireResistance = (float)boss.Resistance(ItemDamageType.Fire);
            _targetNatureResistance = (float)boss.Resistance(ItemDamageType.Nature);
            //_additionalTargets = (int)boss.MaxNumTargets - 1;
            {
                float value = 0;
                foreach (TargetGroup tg in boss.Targets)
                {
                    if (tg.Chance <= 0 || tg.Frequency <= 0 || tg.Duration <= 0) continue;
                    value += tg.Frequency / boss.BerserkTimer * tg.Duration / 1000f;
                }
                float num = value / boss.Targets.Count;
                _additionalTargets = (int)num - 1;
            }
            //_additionalTargetPercent = (float)boss.MultiTargsPerc;
            {
                float time = 0;
                foreach (TargetGroup tg in boss.Targets)
                {
                    if (tg.Chance <= 0 || tg.Frequency <= 0 || tg.Duration <= 0) continue;
                    time += tg.Frequency / boss.BerserkTimer * tg.Duration / 1000f;
                }
                float perc = time / boss.BerserkTimer;
                _additionalTargetPercent = Math.Max(0f, Math.Min(1f, perc));
            }
            _multipleTargets = _additionalTargets > 0 && _additionalTargetPercent > 0;
        }

        public bool PriorityInUse(EnhanceAbility abilityType)
        {
            Priority p;
            if (_priorityList.TryGetValue(abilityType, out p))
            {
                return p.Checked;
            }
            return false;
        }

        public Priority GetAbilityPriority(EnhanceAbility abilityType)
        {
            return _priorityList[abilityType];
        }

        public int GetAbilityPriorityValue(EnhanceAbility abilityType)
        {
            int priority = 0;
            foreach (KeyValuePair<EnhanceAbility, Priority> kvp in _sortedList)
            {
                Priority p = kvp.Value;
                if (p.Checked && p.PriorityValue > 0) 
                    priority++;
                if (p.AbilityType == abilityType)
                    return p.Checked ? priority : 0;
            }
            return -1;
        }

        public void SetAbilityPriority(EnhanceAbility abilityType, Priority priority)
        {
            Priority value = GetAbilityPriority(abilityType);
            if (value.AbilityType != EnhanceAbility.None)
                _priorityList.Remove(abilityType);
            if (priority.PriorityValue > 0)
                _priorityList.Add(abilityType, priority);
            // update the sorted list
            SortPriorities();
        }

        public int ActivePriorities()
        {
            int activePriorities = 0;
            foreach (Priority p in _priorityList.Values)
            {
                if (p.Checked && p.PriorityValue > 0) activePriorities++;
            }
            return activePriorities;
        }

        // make sure not to call this from calculations, this should be setup before calculations start
        public void SortPriorities()
        {
            _sortedList = new List<KeyValuePair<EnhanceAbility, Priority>>(_priorityList);
            _sortedList.Sort(
                delegate(KeyValuePair<EnhanceAbility, Priority> firstPair, KeyValuePair<EnhanceAbility, Priority> nextPair)
                {
                    return firstPair.Value.PriorityValue.CompareTo(nextPair.Value.PriorityValue);
                }
            );
        }

        public static SerializableDictionary<EnhanceAbility, Priority> SetPriorityDefaults()
        {
            SerializableDictionary<EnhanceAbility, Priority> priorityList = new SerializableDictionary<EnhanceAbility, Priority>();
            if (priorityList.Count == 0)
            {
                int priority = 0;
                priorityList.Add(EnhanceAbility.ShamanisticRage, new Priority("Shamanistic Rage", EnhanceAbility.ShamanisticRage, "Use Shamanistic Rage", true, ++priority, "SR"));
                priorityList.Add(EnhanceAbility.FeralSpirits, new Priority("Feral Spirits", EnhanceAbility.FeralSpirits, "Use Feral Sprirts", true, ++priority, "SW"));
                priorityList.Add(EnhanceAbility.LightningBolt, new Priority("Lightning Bolt on 5 stacks of MW", EnhanceAbility.LightningBolt, "Use Lightning Bolt when you have 5 stacks of Maelstrom Weapon", true, ++priority, "MW5_LB"));
                priorityList.Add(EnhanceAbility.FireElemental, new Priority("Fire Elemental", EnhanceAbility.FireElemental, "Drop Fire Elemental Totem", false, ++priority, "FE"));
                priorityList.Add(EnhanceAbility.MagmaTotem, new Priority("Magma Totem", EnhanceAbility.MagmaTotem, "Refresh Magma Totem", true, ++priority, "MT"));
                priorityList.Add(EnhanceAbility.FlameShock, new Priority("Flame Shock", EnhanceAbility.FlameShock, "Use Flame Shock if no Flame Shock debuff on target", true, ++priority, "FS"));
                // priorityList.Add(EnhanceAbility.EarthShock, new Priority("Earth Shock if SS debuff", EnhanceAbility.EarthShock, "Use Earth Shock if Stormstrike debuff on target", true, ++priority, "ES_SS"));
                //       priorityList.Add(new Priority("Lava Lash if Quaking Earth", EnhanceAbility.LavaLash, "Use Lava Lash if Volcanic Fury buff about to run out", false, ++priority, "LL_QE"));
                priorityList.Add(EnhanceAbility.StormStrike, new Priority("Stormstrike", EnhanceAbility.StormStrike, "Use Stormstrike", true, ++priority, "SS"));
                priorityList.Add(EnhanceAbility.EarthShock, new Priority("Earth Shock", EnhanceAbility.EarthShock, "Use Earth Shock", true, ++priority, "ES"));
                priorityList.Add(EnhanceAbility.LavaLash, new Priority("Lava Lash", EnhanceAbility.LavaLash, "Use Lava Lash", true, ++priority, "LL"));
                priorityList.Add(EnhanceAbility.FireNova, new Priority("Fire Nova", EnhanceAbility.FireNova, "Use Fire Nova", true, ++priority, "FN"));
                priorityList.Add(EnhanceAbility.SearingTotem, new Priority("Searing Totem", EnhanceAbility.SearingTotem, "Refresh Searing Totem", false, ++priority, "ST"));
                priorityList.Add(EnhanceAbility.LightningShield, new Priority("Lightning Shield", EnhanceAbility.LightningShield, "Refresh Lightning Shield", true, ++priority, "LS"));
                priorityList.Add(EnhanceAbility.RefreshTotems, new Priority("Refresh Totems", EnhanceAbility.RefreshTotems, "Refresh Totems", true, ++priority, ""));
            }
            return priorityList;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
        #endregion
	}
}
