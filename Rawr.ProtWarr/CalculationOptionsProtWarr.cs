using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Rawr.ProtWarr
{
#if !SILVERLIGHT
	[Serializable]
#endif
	public class CalculationOptionsProtWarr : 
        ICalculationOptionBase,
        INotifyPropertyChanged
	{
		public string GetXml()
		{
			System.Xml.Serialization.XmlSerializer serializer =
				new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsProtWarr));
			StringBuilder xml = new StringBuilder();
			System.IO.StringWriter writer = new System.IO.StringWriter(xml);
			serializer.Serialize(writer, this);
			return xml.ToString();
		}

		public CalculationOptionsProtWarr()
		{
			TargetLevel = 83;
			TargetArmor = (int)StatConversion.NPC_ARMOR[83 - 80];
			BossAttackValue = 80000;
			BossAttackSpeed = 2.0f;
			UseParryHaste = false;
			ThreatScale = 8.0f;
			MitigationScale = 0.125f;
			RankingMode = 1;
			UseVigilance = true;
			VigilanceValue = 5000;
            HeroicStrikeFrequency = 0.9f;
		}
		
		private int _targetLevel;
		public int TargetLevel
		{
			get { return _targetLevel; }
			set { _targetLevel = value; OnPropertyChanged("TargetLevel"); }
		}

		private int _targetArmor;
		public int TargetArmor
		{
			get { return _targetArmor; }
			set { _targetArmor = value; OnPropertyChanged("TargetArmor"); }
		}

		private int _bossAttackValue;
		public int BossAttackValue
		{
			get { return _bossAttackValue; }
			set { _bossAttackValue = value; OnPropertyChanged("BossAttackValue"); }
		}

		private float _bossAttackSpeed;
		public float BossAttackSpeed
		{
			get { return _bossAttackSpeed; }
			set { _bossAttackSpeed = value; OnPropertyChanged("BossAttackSpeed"); }
		}

		private bool _useParryHaste;
		public bool UseParryHaste
		{
			get { return _useParryHaste; }
			set { _useParryHaste = value; OnPropertyChanged("UseParryHaste"); }
		}

		private float _threatScale;
		public float ThreatScale
		{
			get { return _threatScale; }
			set { _threatScale = value; OnPropertyChanged("ThreatScale"); }
		}

		private float _mitigationScale;
		public float MitigationScale
		{
			get { return _mitigationScale; }
			set { _mitigationScale = value; OnPropertyChanged("MitigationScale"); }
		}

		private int _rankingMode;
		public int RankingMode
		{
			get { return _rankingMode; }
			set { _rankingMode = value; OnPropertyChanged("RankingMode"); }
		}

		private bool _useVigilance;
		public bool UseVigilance
		{
			get { return _useVigilance; }
			set { _useVigilance = value; OnPropertyChanged("UseVigilance"); }
		}

		private int _vigilanceValue;
		public int VigilanceValue
		{
			get { return _vigilanceValue; }
			set { _vigilanceValue = value; OnPropertyChanged("VigilanceValue"); }
		}

        private float _heroicStrikeFrequency;
		public float HeroicStrikeFrequency
		{
            get { return _heroicStrikeFrequency; }
            set { _heroicStrikeFrequency = value; OnPropertyChanged("HeroicStrikeFrequency"); }
		}

		#region INotifyPropertyChanged Members
		private void OnPropertyChanged(string property)
		{
			if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

	}
}
