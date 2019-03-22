using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Rawr.HealPriest
{
    public enum eRole
    {
        AUTO_Tank, AUTO_Raid, Greater_Heal, Flash_Heal, CoH_PoH, Holy_Tank, Holy_Raid,
        Disc_Tank_GH, Disc_Tank_FH, Disc_Raid, CUSTOM, Holy_Raid_Renew
    };

#if !SILVERLIGHT
    [Serializable]
#endif
    public class CalculationOptionsHealPriest : ICalculationOptionBase, INotifyPropertyChanged
    {
        public string GetXml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsHealPriest));
            StringBuilder xml = new StringBuilder();
            System.IO.StringWriter writer = new System.IO.StringWriter(xml);
            serializer.Serialize(writer, this);
            return xml.ToString();
        }

        private eRole _Role = eRole.AUTO_Tank;
        public eRole Role { get { return _Role; } set { _Role = value; OnPropertyChanged("Role"); } }
        private int _Rotation = 0;// LEGACY
        public int Rotation { get { return _Rotation; } set { _Rotation = value; OnPropertyChanged("Rotation"); } }// LEGACY
        private float _FSRRatio = 93f;
        public float FSRRatio { get { return _FSRRatio; } set { _FSRRatio = value; OnPropertyChanged("FSRRatio"); } }
        private float _FightLengthSeconds = 480f;
        public float FightLengthSeconds { get { return _FightLengthSeconds; } set { _FightLengthSeconds = value; OnPropertyChanged("FightLengthSeconds"); } }
        private float _Serendipity = 75f;
        public float Serendipity { get { return _Serendipity; } set { _Serendipity = value; OnPropertyChanged("Serendipity"); } }
        private float _Replenishment = 75f;
        public float Replenishment { get { return _Replenishment; } set { _Replenishment = value; OnPropertyChanged("Replenishment"); } }
        private float _Shadowfiend = 100f;
        public float Shadowfiend { get { return _Shadowfiend; } set { _Shadowfiend = value; OnPropertyChanged("Shadowfiend"); } }
        private float _Survivability = 2f;
        public float Survivability { get { return _Survivability; } set { _Survivability = value; OnPropertyChanged("Survivability"); } }
        private float _Rapture = 25f;
        public float Rapture { get { return _Rapture; } set { _Rapture = value; OnPropertyChanged("Rapture"); } }
        private float _TestOfFaith = 25f;
        public float TestOfFaith { get { return _TestOfFaith; } set { _TestOfFaith = value; OnPropertyChanged("TestOfFaith"); } }
        private bool _ModelProcs = true;
        public bool ModelProcs { get { return _ModelProcs; } set { _ModelProcs = value; OnPropertyChanged("ModelProcs"); } }

        private int _FlashHealCast = 0;
        public int FlashHealCast { get { return _FlashHealCast; } set { _FlashHealCast = value; OnPropertyChanged("FlashHealCast"); } }
        private int _BindingHealCast = 0;
        public int BindingHealCast { get { return _BindingHealCast; } set { _BindingHealCast = value; OnPropertyChanged("BindingHealCast"); } }
        private int _GreaterHealCast = 0;
        public int GreaterHealCast { get { return _GreaterHealCast; } set { _GreaterHealCast = value; OnPropertyChanged("GreaterHealCast"); } }
        private int _PenanceCast = 0;
        public int PenanceCast { get { return _PenanceCast; } set { _PenanceCast = value; OnPropertyChanged("PenanceCast"); } }
        private int _RenewCast = 0;
        public int RenewCast { get { return _RenewCast; } set { _RenewCast = value; OnPropertyChanged("RenewCast"); } }
        private int _RenewTicks = 0;
        public int RenewTicks { get { return _RenewTicks; } set { _RenewTicks = value; OnPropertyChanged("RenewTicks"); } }
        private int _ProMCast = 0;
        public int ProMCast { get { return _ProMCast; } set { _ProMCast = value; OnPropertyChanged("ProMCast"); } }
        private int _ProMTicks = 0;
        public int ProMTicks { get { return _ProMTicks; } set { _ProMTicks = value; OnPropertyChanged("ProMTicks"); } }
        private int _PoHCast = 0;
        public int PoHCast { get { return _PoHCast; } set { _PoHCast = value; OnPropertyChanged("PoHCast"); } }
        private int _PWSCast = 0;
        public int PWSCast { get { return _PWSCast; } set { _PWSCast = value; OnPropertyChanged("PWSCast"); } }
        private int _CoHCast = 0;
        public int CoHCast { get { return _CoHCast; } set { _CoHCast = value; OnPropertyChanged("CoHCast"); } }
        private int _HolyNovaCast = 0;
        public int HolyNovaCast { get { return _HolyNovaCast; } set { _HolyNovaCast = value; OnPropertyChanged("HolyNovaCast"); } }
        private int _DivineHymnCast = 0;
        public int DivineHymnCast { get { return _DivineHymnCast; } set { _DivineHymnCast = value; OnPropertyChanged("DivineHymnCast"); } }
        private int _DispelCast = 0;
        public int DispelCast { get { return _DispelCast; } set { _DispelCast = value; OnPropertyChanged("DispelCast"); } }
        private int _MDCast = 0;
        public int MDCast { get { return _MDCast; } set { _MDCast = value; OnPropertyChanged("MDCast"); } }

        #region Obsolete, to be Removed
        private static readonly List<int> manaAmt = new List<int>() { 0, 1800, 2200, 2400, 4300 };
        private int _ManaPot = 4;
        public int ManaPot { get { return _ManaPot; } set { _ManaPot = value; OnPropertyChanged("ManaPot"); } }
        public int ManaAmt { get { return manaAmt[ManaPot]; } }
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
