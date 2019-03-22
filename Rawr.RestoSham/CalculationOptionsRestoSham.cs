using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Rawr.RestoSham
{
	#region Calculations, do not edit.
	class NumericField
	{
		public NumericField(string szName, float min, float max, bool bzero)
		{
			PropertyName = szName;
			MinValue = min;
			MaxValue = max;
			CanBeZero = bzero;
		}

		public string PropertyName = string.Empty;
		public float MinValue = float.MinValue;
		public float MaxValue = float.MaxValue;
		public bool CanBeZero = false;
	}
	#endregion

#if !SILVERLIGHT
	[Serializable]
#endif
	public class CalculationOptionsRestoSham : ICalculationOptionBase, INotifyPropertyChanged
	{
		#region GetXml strings
		public string GetXml()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsRestoSham));
			StringBuilder xml = new StringBuilder();
			System.IO.StringWriter writer = new System.IO.StringWriter(xml);
			serializer.Serialize(writer, this);
			return xml.ToString();
		}
		#endregion
		#region Defaults for variables
		/// <summary>
		/// Fight length, in minutes.
		/// </summary>
        private float _FightLength = 5.0f;
        public float FightLength { get { return _FightLength; } set { _FightLength = value; OnPropertyChanged("FightLength"); } }

		/// <summary>
		/// Whether a Mana Tide totem is placed every time the cooldown is up.
		/// </summary>
        private bool _ManaTideEveryCD = true;
        public bool ManaTideEveryCD { get { return _ManaTideEveryCD; } set { _ManaTideEveryCD = value; OnPropertyChanged("ManaTideEveryCD"); } }

        /// <summary>
        /// Count of Innervates you get.
        /// </summary>
        private float _Innervates = 0f;
        public float Innervates { get { return _Innervates; } set { _Innervates = value; OnPropertyChanged("Innervates"); } }

        /// <summary>
		/// Whether we keep Water Shield up or not (could use Earth Shield during some fights).
		/// </summary>
        private bool _WaterShield = true;
        public bool WaterShield { get { return _WaterShield; } set { _WaterShield = value; OnPropertyChanged("WaterShield"); } }

		/// <summary>
		/// Earth shield use.
		/// </summary>
        private bool _EarthShield = true;
        public bool EarthShield { get { return _EarthShield; } set { _EarthShield = value; OnPropertyChanged("EarthShield"); } }

		/// <summary>
		/// Your style of healing.
		/// </summary>
        private string _BurstStyle = "RT+HW";
        public string BurstStyle { get { return _BurstStyle; } set { _BurstStyle = value; OnPropertyChanged("BurstStyle"); } }

		/// <summary>
		/// Your style of healing.
		/// </summary>
        private string _SustStyle = "RT+CH";
        public string SustStyle { get { return _SustStyle; } set { _SustStyle = value; OnPropertyChanged("SustStyle"); } }

        /// <summary>
        /// Targets.
        /// </summary>
        private string _Targets = "Raid";
        public string Targets { get { return _Targets; } set { _Targets = value; OnPropertyChanged("Targets"); } }

        /// <summary>
		/// Will you or someone else use heroism?
		/// </summary>
        private string _Heroism = "Me";
        public string Heroism { get { return _Heroism; } set { _Heroism = value; OnPropertyChanged("Heroism"); } }

		/// <summary>
		/// The percentage of time that Replenishment is "up".
		/// </summary>
        private float _ReplenishmentPercentage = 90f;
        public float ReplenishmentPercentage { get { return _ReplenishmentPercentage; } set { _ReplenishmentPercentage = value; OnPropertyChanged("ReplenishmentPercentage"); } }

		/// <summary>
		/// The number of times you use Cleanse Spirit.
		/// </summary>
        private float _Decurse = 0f;
        public float Decurse { get { return _Decurse; } set { _Decurse = value; OnPropertyChanged("Decurse"); } }

        /// <summary>
        /// The latency in milliseconds.
        /// </summary>
        private float _Latency = 60f;
        public float Latency { get { return _Latency; } set { _Latency = value; OnPropertyChanged("Latency"); } }

        /// <summary>
        /// The percentage of usefulness survival is to you.
        /// </summary>
        private float _SurvivalPerc = 2f;
        public float SurvivalPerc { get { return _SurvivalPerc; } set { _SurvivalPerc = value; OnPropertyChanged("SurvivalPerc"); } }

        /// <summary>
        /// The percentage of time you are active.
        /// </summary>
        private float _ActivityPerc = 85f;
        public float ActivityPerc { get { return _ActivityPerc; } set { _ActivityPerc = value; OnPropertyChanged("ActivityPerc"); } }

        /// <summary>
        /// The number of times water shield pops per minute.
        /// </summary>
        private float _WSPops = 0f;
        public float WSPops { get { return _WSPops; } set { _WSPops = value; OnPropertyChanged("WSPops"); } }
		#endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
        }
        #endregion
    }
}
