using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Rawr.Rogue
{
#if !SILVERLIGHT
	[Serializable]
#endif
	public class CalculationOptionsRogue :  ICalculationOptionBase, INotifyPropertyChanged
    {
        public string GetXml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsRogue));
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
        private bool _targetPoisonable = true;
        public bool TargetPoisonable
        {
            get { return _targetPoisonable; }
            set { if (_targetPoisonable != value) { _targetPoisonable = value; OnPropertyChanged("TargetPoisonable"); } }
        }
        private bool _bleedIsUp = true;
        public bool BleedIsUp
        {
            get { return _bleedIsUp; }
            set { if (_bleedIsUp != value) { _bleedIsUp = value; OnPropertyChanged("BleedIsUp"); } }
        }
        private int _duration = 300;
        public int Duration
        {
            get { return _duration; }
            set { if (_duration != value) { _duration = value; OnPropertyChanged("Duration"); } }
        }
        private int _lagVariance = 200;
        public int LagVariance
        {
            get { return _lagVariance; }
            set { if (_lagVariance != value) { _lagVariance = value; OnPropertyChanged("LagVariance"); } }
        }
        private int _customCPG = 1;
        public int CustomCPG
        {
            get { return _customCPG; }
            set { if (_customCPG != value) { _customCPG = value; OnPropertyChanged("CustomCPG"); } }
        }
        private bool _forceCustom = false;
        public bool ForceCustom
        {
            get { return _forceCustom; }
            set { if (_forceCustom != value) { _forceCustom = value; OnPropertyChanged("ForceCustom"); } }
        }
        private bool _customUseRupt = false;
        public bool CustomUseRupt
        {
            get { return _customUseRupt; }
            set { if (_customUseRupt != value) { _customUseRupt = value; OnPropertyChanged("CustomUseRupt"); } }
        }
        private bool _customUseExpose = false;
        public bool CustomUseExpose
        {
            get { return _customUseExpose; }
            set { if (_customUseExpose != value) { _customUseExpose = value; OnPropertyChanged("CustomUseExpose"); } }
        }
        private bool _customUseTotT = false;
        public bool CustomUseTotT
        {
            get { return _customUseTotT; }
            set { if (_customUseTotT != value) { _customUseTotT = value; OnPropertyChanged("CustomUseTotT"); } }
        }
        private int _customFinisher = 1;
        public int CustomFinisher
        {
            get { return _customFinisher; }
            set { if (_customFinisher != value) { _customFinisher = value; OnPropertyChanged("CustomFinisher"); } }
        }
        private int _customCPFinisher = 5;
        public int CustomCPFinisher
        {
            get { return _customCPFinisher; }
            set { if (_customCPFinisher != value) { _customCPFinisher = value; OnPropertyChanged("CustomCPFinisher"); } }
        }
        private int _customCPSnD = 5;
        public int CustomCPSnD
        {
            get { return _customCPSnD; }
            set { if (_customCPSnD != value) { _customCPSnD = value; OnPropertyChanged("CustomCPSnD"); } }
        }
        private int _customMHPoison = 1;
        public int CustomMHPoison
        {
            get { return _customMHPoison; }
            set { if (_customMHPoison != value) { _customMHPoison = value; OnPropertyChanged("CustomMHPoison"); } }
        }
        private int _customOHPoison = 2;
        public int CustomOHPoison
        {
            get { return _customOHPoison; }
            set { if (_customOHPoison != value) { _customOHPoison = value; OnPropertyChanged("CustomOHPoison"); } }
        }
        private bool _enableMuti = true;
        public bool EnableMuti
        {
            get { return _enableMuti; }
            set { if (_enableMuti != value) { _enableMuti = value; OnPropertyChanged("EnableMuti"); } }
        }
        private bool _enableSS = true;
        public bool EnableSS
        {
            get { return _enableSS; }
            set { if (_enableSS != value) { _enableSS = value; OnPropertyChanged("EnableSS"); } }
        }
        private bool _enableBS = true;
        public bool EnableBS
        {
            get { return _enableBS; }
            set { if (_enableBS != value) { _enableBS = value; OnPropertyChanged("EnableBS"); } }
        }
        private bool _enableHemo = true;
        public bool EnableHemo
        {
            get { return _enableHemo; }
            set { if (_enableHemo != value) { _enableHemo = value; OnPropertyChanged("EnableHemo"); } }
        }
        private bool _enableIP = true;
        public bool EnableIP
        {
            get { return _enableIP; }
            set { if (_enableIP != value) { _enableIP = value; OnPropertyChanged("EnableIP"); } }
        }
        private bool _enableDP = true;
        public bool EnableDP
        {
            get { return _enableDP; }
            set { if (_enableDP != value) { _enableDP = value; OnPropertyChanged("EnableDP"); } }
        }
        private bool _enableWP = true;
        public bool EnableWP
        {
            get { return _enableWP; }
            set { if (_enableWP != value) { _enableWP = value; OnPropertyChanged("EnableWP"); } }
        }
        private bool _enableRupt = true;
        public bool EnableRupt
        {
            get { return _enableRupt; }
            set { if (_enableRupt != value) { _enableRupt = value; OnPropertyChanged("EnableRupt"); } }
        }
        private bool _enableEvis = true;
        public bool EnableEvis
        {
            get { return _enableEvis; }
            set { if (_enableEvis != value) { _enableEvis = value; OnPropertyChanged("EnableEvis"); } }
        }
        private bool _enableEnvenom = true;
        public bool EnableEnvenom
        {
            get { return _enableEnvenom; }
            set { if (_enableEnvenom != value) { _enableEnvenom = value; OnPropertyChanged("EnableEnvenom"); } }
        }
        private float _trinketOffset = 0f;
        public float TrinketOffset
        {
            get { return _trinketOffset; }
            set { if (_trinketOffset != value) { _trinketOffset = value; OnPropertyChanged("TrinketOffset"); } }
        }
        private bool _PTRMode = false;
        public bool PTRMode
        {
            get { return _PTRMode; }
            set { if (_PTRMode != value) { _PTRMode = value; OnPropertyChanged("PTRMode"); } }
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