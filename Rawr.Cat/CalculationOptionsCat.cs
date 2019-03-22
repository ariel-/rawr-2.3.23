using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Rawr.Cat
{
#if !SILVERLIGHT
	[Serializable]
#endif
    public class CalculationOptionsCat : 
        ICalculationOptionBase, 
        INotifyPropertyChanged,
        ICharacterCalculationOptions
	{
		public string GetXml()
		{
			System.Xml.Serialization.XmlSerializer serializer =
				new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsCat));
			StringBuilder xml = new StringBuilder();
			System.IO.StringWriter writer = new System.IO.StringWriter(xml);
			serializer.Serialize(writer, this);
			return xml.ToString();
		}


        private int _targetLevel = 83;
        public int TargetLevel
        {
            get { return _targetLevel; }
            set { if (_targetLevel != value) { _targetLevel = value; OnPropertyChanged("TargetLevel"); } }
        }
        private int _targetArmor = (int)StatConversion.NPC_ARMOR[83 - 80];
        public int TargetArmor
        {
            get { return _targetArmor; }
            set { if (_targetArmor != value) { _targetArmor = value; OnPropertyChanged("TargetArmor"); } }
        }
        private bool _customUseShred = false;
        public bool CustomUseShred
        {
            get { return _customUseShred; }
            set { if (_customUseShred != value) { _customUseShred = value; OnPropertyChanged("CustomUseShred"); } }
        }
        private bool _customUseRip = false;
        public bool CustomUseRip
        {
            get { return _customUseRip; }
            set { if (_customUseRip != value) { _customUseRip = value; OnPropertyChanged("CustomUseRip"); } }
        }
        private bool _customUseRake = false;
        public bool CustomUseRake
        {
            get { return _customUseRake; }
            set { if (_customUseRake != value) { _customUseRake = value; OnPropertyChanged("CustomUseRake"); } }
        }
        private int _customCPFerociousBite = 0;
        public int CustomCPFerociousBite
        {
            get { return _customCPFerociousBite; }
            set { if (_customCPFerociousBite != value) { _customCPFerociousBite = value; OnPropertyChanged("CustomCPFerociousBite"); } }
        }
        private int _customCPSavageRoar = 5;
        public int CustomCPSavageRoar
        {
            get { return _customCPSavageRoar; }
            set { if (_customCPSavageRoar != value) { _customCPSavageRoar = value; OnPropertyChanged("CustomCPSavageRoar"); } }
        }
        private int _duration = 300;
        public int Duration
        {
            get { return _duration; }
            set { if (_duration != value) { _duration = value; OnPropertyChanged("Duration"); } }
        }
        private float _trinketOffset = 0f;
        public float TrinketOffset
        {
            get { return _trinketOffset; }
            set { if (_trinketOffset != value) { _trinketOffset = value; OnPropertyChanged("TrinketOffset"); } }
        }
        private int _lagVariance = 200;
        public int LagVariance
        {
            get { return _lagVariance; }
            set { if (_lagVariance != value) { _lagVariance = value; OnPropertyChanged("LagVariance"); } }
        }

        private Character _character;
        [XmlIgnore]
        public Character Character
        {
            get
            {
                return _character;
            }
            set
            {
                _character = value;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            if (Character != null)
            {
                Character.OnCalculationsInvalidated();
            }
        }

        #endregion
	}
}
