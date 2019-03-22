using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Rawr.Bear
{
#if !SILVERLIGHT
	[Serializable]
#endif
	public class CalculationOptionsBear : ICalculationOptionBase, INotifyPropertyChanged
	{
		public string GetXml()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsBear));
			StringBuilder xml = new StringBuilder();
			System.IO.StringWriter writer = new System.IO.StringWriter(xml);
			serializer.Serialize(writer, this);
			return xml.ToString();
        }

        #region Rating Customization
        private float _threatScale = 10f;
        public float ThreatScale
        {
            get { return _threatScale; }
            set { if (_threatScale != value) { _threatScale = value; OnPropertyChanged("ThreatScale"); } }
        }
        private int _survivalSoftCap = 160000;
        public int SurvivalSoftCap
        {
            get { return _survivalSoftCap; }
            set { if (_survivalSoftCap != value) { _survivalSoftCap = value; OnPropertyChanged("SurvivalSoftCap"); } }
        }
        private float _temporarySurvivalScale = 1f;
		public float TemporarySurvivalScale
		{
			get { return _temporarySurvivalScale; }
			set { if (_temporarySurvivalScale != value) { _temporarySurvivalScale = value; OnPropertyChanged("TemporarySurvivalScale"); } }
		}
        #endregion

        #region Target Parameters
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
        private int _targetDamage = 65000;
        public int TargetDamage
        {
            get { return _targetDamage; }
            set { if (_targetDamage != value) { _targetDamage = value; OnPropertyChanged("TargetDamage"); } }
        }
        private float _targetAttackSpeed = 2.0f;
        public float TargetAttackSpeed
        {
            get { return _targetAttackSpeed; }
            set { if (_targetAttackSpeed != value) { _targetAttackSpeed = value; OnPropertyChanged("TargetAttackSpeed"); } }
        }
        private bool _targetParryHastes = true;
        public bool TargetParryHastes
        {
            get { return _targetParryHastes; }
            set { if (_targetParryHastes != value) { _targetParryHastes = value; OnPropertyChanged("TargetParryHastes"); } }
        }
        #endregion

        #region Custom Rotation
        private bool _customUseMaul = false;
        public bool CustomUseMaul
        {
            get { return _customUseMaul; }
            set { if (_customUseMaul != value) { _customUseMaul = value; OnPropertyChanged("CustomUseMaul"); } }
        }
        private bool _customUseMangle = false;
        public bool CustomUseMangle
        {
            get { return _customUseMangle; }
            set { if (_customUseMangle != value) { _customUseMangle = value; OnPropertyChanged("CustomUseMangle"); } }
        }
        private bool _customUseSwipe = false;
        public bool CustomUseSwipe
        {
            get { return _customUseSwipe; }
            set { if (_customUseSwipe != value) { _customUseSwipe = value; OnPropertyChanged("CustomUseSwipe"); } }
        }
        private bool _customUseFaerieFire = false;
        public bool CustomUseFaerieFire
        {
            get { return _customUseFaerieFire; }
            set { if (_customUseFaerieFire != value) { _customUseFaerieFire = value; OnPropertyChanged("CustomUseFaerieFire"); } }
        }
        private bool _customUseLacerate = false;
        public bool CustomUseLacerate
        {
            get { return _customUseLacerate; }
            set { if (_customUseLacerate != value) { _customUseLacerate = value; OnPropertyChanged("CustomUseLacerate"); } }
        }
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
