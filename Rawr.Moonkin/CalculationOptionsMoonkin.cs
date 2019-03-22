using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Rawr.Moonkin
{
#if !SILVERLIGHT
	[Serializable]
#endif
	public class CalculationOptionsMoonkin : ICalculationOptionBase, INotifyPropertyChanged
	{
		public string GetXml()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsMoonkin));
			StringBuilder xml = new StringBuilder();
			System.IO.StringWriter writer = new System.IO.StringWriter(xml);
			serializer.Serialize(writer, this);
			return xml.ToString();
		}

		private int targetLevel = 83;
		public int TargetLevel
		{
			get { return targetLevel; }
			set { targetLevel = value; OnPropertyChanged("TargetLevel"); }
		}

		private float latency = 0.2f;
		public float Latency
		{
			get { return latency; }
			set { latency = value; OnPropertyChanged("Latency"); }
		}
		public float ExternalLatency
		{
			get { return latency * 1000f; }
			set { latency = value / 1000f; OnPropertyChanged("Latency"); }
		}

		private float fightLength = 5;
		public float FightLength
		{
			get { return fightLength; }
			set { fightLength = value; OnPropertyChanged("FightLength"); }
		}

		private bool innervate = false;
		public bool Innervate
		{
			get { return innervate; }
			set { innervate = value; OnPropertyChanged("Innervate"); }
		}

		private float innervateDelay = 1;
		public float InnervateDelay
		{
			get { return innervateDelay; }
			set { innervateDelay = value; OnPropertyChanged("InnervateDelay"); }
		}

		private string aldorScryer = "Aldor";
		public string AldorScryer
		{
			get { return aldorScryer; }
			set { aldorScryer = value; OnPropertyChanged("AldorScryer"); }
		}

		private float replenishmentUptime = 1.0f;
		public float ReplenishmentUptime
		{
			get { return replenishmentUptime; }
			set { replenishmentUptime = value; OnPropertyChanged("ReplenishmentUptime"); }
		}

		private float treantLifespan = 1.0f;
		public float TreantLifespan
		{
			get { return treantLifespan; }
			set { treantLifespan = value; OnPropertyChanged("TreantLifespan"); }
		}

		private string userRotation = "None";
		public string UserRotation
		{
			get { return userRotation; }
			set { userRotation = value; OnPropertyChanged("UserRotation"); }
		}

        private bool ptrMode = false;
        public bool PTRMode
        {
            get { return ptrMode; }
            set { ptrMode = value; OnPropertyChanged("PTRMode"); }
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
