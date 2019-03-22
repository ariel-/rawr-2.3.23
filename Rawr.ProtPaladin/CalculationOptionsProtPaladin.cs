using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Rawr.ProtPaladin
{
#if !SILVERLIGHT
	[Serializable]
#endif
	public class CalculationOptionsProtPaladin : ICalculationOptionBase, INotifyPropertyChanged {

        private int _TargetLevel = 83;
        public int TargetLevel
        {
            get { return _TargetLevel; }
            set { _TargetLevel = value; OnPropertyChanged("TargetLevel"); }
        }

        private int _TargetArmor = (int)StatConversion.NPC_ARMOR[83 - 80];
        public int TargetArmor
        {
            get { return _TargetArmor; }
            set { _TargetArmor = value; OnPropertyChanged("TargetArmor"); }
        }

        private int _BossAttackValue = 80000;
        public int BossAttackValue
        {
            get { return _BossAttackValue; }
            set { _BossAttackValue = value; OnPropertyChanged("BossAttackValue"); }
        }

        private float _BossAttackSpeed = 2.0f;
        public float BossAttackSpeed
        {
            get { return _BossAttackSpeed; }
            set { _BossAttackSpeed = value; OnPropertyChanged("BossAttackSpeed"); }
        }

		private int _BossAttackValueMagic = 20000;
        public int BossAttackValueMagic
        {
            get { return _BossAttackValueMagic; }
            set { _BossAttackValueMagic = value; OnPropertyChanged("BossAttackValueMagic"); }
        }

		private float _BossAttackSpeedMagic = 1.0f;
        public float BossAttackSpeedMagic
        {
            get { return _BossAttackSpeedMagic; }
            set { _BossAttackSpeedMagic = value; OnPropertyChanged("BossAttackSpeedMagic"); }
        }

		private bool _UseParryHaste = false;
        public bool UseParryHaste
        {
            get { return _UseParryHaste; }
            set { _UseParryHaste = value; OnPropertyChanged("UseParryHaste"); }
        }

        private float _ThreatScale = 10.0f;
		public float ThreatScale
        {
            get { return _ThreatScale; }
            set { _ThreatScale = value; OnPropertyChanged("ThreatScale"); }
        }

        private float _MitigationScale = 17000f;
        public float MitigationScale
        {
            get { return _MitigationScale; }
            set { _MitigationScale = value; OnPropertyChanged("MitigationScale"); }
        }

		private int _RankingMode = 1;
        public int RankingMode
        {
            get { return _RankingMode; }
            set { _RankingMode = value; OnPropertyChanged("RankingMode"); }
        }

        private bool _UseHolyShield = true;
        public bool UseHolyShield
        {
            get { return _UseHolyShield; }
            set { _UseHolyShield = value; OnPropertyChanged("UseHolyShield"); }
        }

        private string _SealChoice = "Seal of Vengeance";
        public string SealChoice
        {
            get { return _SealChoice; }
            set { _SealChoice = value; OnPropertyChanged("SealChoice"); }
        }

		private string _TargetType = "Unspecified";
        public string TargetType
        {
            get { return _TargetType; }
            set { _TargetType = value; OnPropertyChanged("TargetType"); }
        }

		private string _MagicDamageType = "None";
        public string MagicDamageType
        {
            get { return _MagicDamageType; }
            set { _MagicDamageType = value; OnPropertyChanged("MagicDamageType"); }
        }

        private string _TrinketOnUseHandling = "Ignore";
        public string TrinketOnUseHandling
        {
            get { return _TrinketOnUseHandling; }
            set { _TrinketOnUseHandling = value; OnPropertyChanged("TrinketOnUseHandling"); }
        }

		private PaladinTalents _talents = null;
        public PaladinTalents talents
        {
            get { return _talents; }
            set { _talents = value; OnPropertyChanged("talents"); }
        }

        private bool _PTRMode = false;
        public bool PTRMode
        {
            get { return _PTRMode; }
            set { _PTRMode = value; OnPropertyChanged("PTRMode"); }
        }

        private int _survivalSoftCap = 300000;
        public int SurvivalSoftCap
        {
            get { return _survivalSoftCap; }
            set { _survivalSoftCap = value; OnPropertyChanged("SurvivalSoftCap"); }
        }

        #region ICalculationOptionBase members
        public string GetXml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsProtPaladin));
            StringBuilder xml = new StringBuilder();
            System.IO.StringWriter writer = new System.IO.StringWriter(xml);
            serializer.Serialize(writer, this);
            return xml.ToString();
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
        #endregion
	}
}
