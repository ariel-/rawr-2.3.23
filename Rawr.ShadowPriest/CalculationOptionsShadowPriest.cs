using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Rawr.ShadowPriest
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class CalculationOptionsShadowPriest : ICalculationOptionBase, INotifyPropertyChanged
    {
        public string GetXml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsShadowPriest));
            StringBuilder xml = new StringBuilder();
            System.IO.StringWriter writer = new System.IO.StringWriter(xml);
            serializer.Serialize(writer, this);
            return xml.ToString();
        }

#if !RAWR3 && !SILVERLIGHT // Boss Handler Options
        private int _TargetLevel = 3;
        private float _FightLength = 6f;
        public int TargetLevel { get { return _TargetLevel; } set { _TargetLevel = value; OnPropertyChanged("TargetLevel"); } }
        public float FightLength { get { return _FightLength; } set { _FightLength = value; OnPropertyChanged("FightLength"); } }
#endif
        // These should be done with Boss Handler, but harder Mechanics so not changing it
        private float _MoveFrequency = 60f;
        private float _MoveDuration = 5f;
        public float MoveFrequency { get { return _MoveFrequency; } set { _MoveFrequency = value; OnPropertyChanged("MoveFrequency"); } }
        public float MoveDuration { get { return _MoveDuration; } set { _MoveDuration = value; OnPropertyChanged("MoveDuration"); } }

        private float _FSRRatio = 100f;
        private float _Delay = 350f;
        private float _Shadowfiend = 100f;
        private float _Replenishment = 100f;
        private float _JoW = 100f;
        private float _Survivability = 2f;
        private bool _PTR = false;
        private List<string> _SpellPriority = null;
        private int _ManaPot = 4; // TODO: Remove

        public float FSRRatio { get { return _FSRRatio; } set { _FSRRatio = value; OnPropertyChanged("FSRRatio"); } }
        public float Delay { get { return _Delay; } set { _Delay = value; OnPropertyChanged("Delay"); } }
        public float Shadowfiend { get { return _Shadowfiend; } set { _Shadowfiend = value; OnPropertyChanged("Shadowfiend"); } }
        public float Replenishment { get { return _Replenishment; } set { _Replenishment = value; OnPropertyChanged("Replenishment"); } }
        public float JoW { get { return _JoW; } set { _JoW = value; OnPropertyChanged("JoW"); } }
        public float Survivability { get { return _Survivability; } set { _Survivability = value; OnPropertyChanged("Survivability"); } }
        public bool PTR { get { return _PTR; } set { _PTR = value; OnPropertyChanged("PTR"); } }
        public List<string> SpellPriority { get { return _SpellPriority; } set { _SpellPriority = value; OnPropertyChanged("SpellPriority"); } }
        public int ManaPot { get { return _ManaPot; } set { _ManaPot = value; OnPropertyChanged("ManaPot"); } }

		private static readonly List<int> manaAmt = new List<int>() { 0, 1800, 2200, 2400, 4300 }; // TODO: Remove
        public int ManaAmt { get { return manaAmt[ManaPot]; } } // TODO: Remove

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
        }
        #endregion
    }
}
