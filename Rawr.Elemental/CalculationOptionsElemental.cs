using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Rawr.Elemental
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class CalculationOptionsElemental : ICalculationOptionBase, INotifyPropertyChanged
    {
        public string GetXml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsElemental));
            StringBuilder xml = new StringBuilder();
            System.IO.StringWriter writer = new System.IO.StringWriter(xml);
            serializer.Serialize(writer, this);
            return xml.ToString();
        }

        [XmlIgnore]
        public CharacterCalculationsElemental calculatedStats = null;

#if !RAWR3 && !SILVERLIGHT // BossHandler Options
        private int _FightDuration = 300; //5 Minutes
        public int FightDuration
        {
            get { return _FightDuration; }
            set { _FightDuration = value; OnPropertyChanged("FightDuration"); }
        }
#endif

        private int _BSRatio = 75; // goes from 0 to 100
        public int BSRatio
        {
            get { return _BSRatio; }
            set { _BSRatio = value; OnPropertyChanged("BSRatio"); }
        }

        private bool _UseThunderstorm = true;
        public bool UseThunderstorm
        {
            get { return _UseThunderstorm; }
            set { _UseThunderstorm = value; OnPropertyChanged("UseThunderstorm"); }
        }

        private int _rotationType = 0;
        public int RotationType
        {
            get { return _rotationType; }
            set { _rotationType = value; OnPropertyChanged("RotationType"); }
        }

        //latency in s
        private float _LatencyGcd = .15f;
        public float LatencyGcd
        {
            get { return _LatencyGcd; }
            set { _LatencyGcd = value; OnPropertyChanged("LatencyGcd"); }
        }

        private float _LatencyCast = .075f;
        public float LatencyCast
        {
            get { return _LatencyCast; }
            set { _LatencyCast = value; OnPropertyChanged("LatencyCast"); }
        }

        private int _NumberOfTargets = 1;
        public int NumberOfTargets
        {
            get { return _NumberOfTargets; }
            set { _NumberOfTargets = value; OnPropertyChanged("NumberOfTargets"); }
        }

        private bool _UseFireNova = true;
        public bool UseFireNova
        {
            get { return _UseFireNova; }
            set { _UseFireNova = value; OnPropertyChanged("UseFireNova"); }
        }

        private bool _UseChainLightning = true;
        public bool UseChainLightning
        {
            get { return _UseChainLightning; }
            set { _UseChainLightning = value; OnPropertyChanged("UseChainLightning"); }
        }
        
        private bool _UseDpsTotem = false;
        public bool UseDpsTotem
        {
            get { return _UseDpsTotem; }
            set { _UseDpsTotem = value; OnPropertyChanged("UseDpsTotem"); }
        }

        private bool _UseFireEle = false;
        public bool UseFireEle
        {
            get { return _UseFireEle; }
            set { _UseFireEle = value; OnPropertyChanged("UseFireEle"); }
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
