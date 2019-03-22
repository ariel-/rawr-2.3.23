using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Rawr.DPSDK
{
#if !SILVERLIGHT
	[Serializable]
#endif
	public class CalculationOptionsDPSDK : ICalculationOptionBase, INotifyPropertyChanged
	{
		public string GetXml()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsDPSDK));
			StringBuilder xml = new StringBuilder();
			System.IO.StringWriter writer = new System.IO.StringWriter(xml);
			serializer.Serialize(writer, this);
			return xml.ToString();
		}

		public enum Presence
		{
			None = 0,
            Blood, 
            Unholy, 
            Frost
		}
		
		private float _GhoulUptime = 1f;
		public float GhoulUptime
		{
			get { return _GhoulUptime; }
			set { _GhoulUptime = value; OnPropertyChanged("GhoulUptime"); }
		}

        private Presence _Presence = Presence.None;
        public Presence CurrentPresence
        {
            get { return _Presence; }
            set { _Presence = value; }
        }
		
		private float _KMProcUsage = 1f;
		public float KMProcUsage
		{
			get { return _KMProcUsage; }
			set { _KMProcUsage = value; OnPropertyChanged("KMProcUsage"); }
		}
		
		private float _BloodwormsUptime = 0.25f;
		public float BloodwormsUptime
		{
			get { return _BloodwormsUptime; }
			set { _BloodwormsUptime = value; OnPropertyChanged("BloodwormsUptime"); }
		}
		
		private bool _Ghoul = true;
		public bool Ghoul
		{
			get { return _Ghoul; }
			set { _Ghoul = value; OnPropertyChanged("Ghoul"); }
		}
		
		private bool _EnforceMetagemRequirements = false;
		public bool EnforceMetagemRequirements
		{
			get {return _EnforceMetagemRequirements; }
			set { _EnforceMetagemRequirements = value; OnPropertyChanged("EnforceMetagemRequirements"); }
		}
		
		private float _FightLength = 6;
		public float FightLength
		{
			get {return _FightLength; }
			set { _FightLength = value; OnPropertyChanged("FightLength"); }
		}
		
		private int _TargetLevel = 83;
        public int TargetLevel
        {
            get { return _TargetLevel; }
            set { _TargetLevel = value; OnPropertyChanged("TargetLevel"); }
        }
		
		private int _BossArmor = (int)StatConversion.NPC_ARMOR[83 - 80];
		public int BossArmor
		{
			get {return _BossArmor; }
			set { _BossArmor = value; OnPropertyChanged("BossArmor"); }
		}
		
        private bool _getRefreshForReferenceCalcs = true;
        public bool GetRefreshForReferenceCalcs
        {
            get { return _getRefreshForReferenceCalcs; }
            set { _getRefreshForReferenceCalcs = value; }
        }

        private bool _getRefreshForDisplayCalcs = true;
        public bool GetRefreshForDisplayCalcs
        {
            get { return _getRefreshForDisplayCalcs; }
            set { _getRefreshForDisplayCalcs = value; }
        }

        private bool _getRefreshForSignificantChange = false;
        public bool GetRefreshForSignificantChange
        {
            get { return _getRefreshForSignificantChange; }
            set { _getRefreshForSignificantChange = value; }
        }


        private bool _m_bExperimental = false;
        public bool m_bExperimental
        {
            get { return _m_bExperimental; }
            set { _m_bExperimental = value; }
        }

		private Rotation _rotation = null;
		public Rotation rotation
		{
            get { if (_rotation == null) _rotation = new Rotation(); return _rotation; }
            set
            {
                if (_rotation == null) _rotation = new Rotation();
                _rotation.ManagedRP = value.ManagedRP;
                _rotation.AvgDiseaseMult = value.AvgDiseaseMult;
                _rotation.BloodPlague = value.BloodPlague;
                _rotation.BloodStrike = value.BloodStrike;
                _rotation.CurRotationDuration = value.CurRotationDuration;
                _rotation.curRotationType = value.curRotationType;
                _rotation.DancingRuneWeapon = value.DancingRuneWeapon;
                _rotation.DeathCoil = value.DeathCoil;
                _rotation.DeathStrike = value.DeathStrike;
                _rotation.DiseaseUptime = value.DiseaseUptime;
                _rotation.FrostFever = value.FrostFever;
                _rotation.FrostStrike = value.FrostStrike;
                _rotation.GargoyleDuration = value.GargoyleDuration;
                _rotation.GCDTime = value.GCDTime;
                _rotation.GhoulFrenzy = value.GhoulFrenzy;
                _rotation.HeartStrike = value.HeartStrike;
                _rotation.Horn = value.Horn;
                _rotation.HowlingBlast = value.HowlingBlast;
                _rotation.IcyTouch = value.IcyTouch;
                _rotation.NumDisease = value.NumDisease;
                _rotation.Obliterate = value.Obliterate;
                _rotation.Pestilence = value.Pestilence;
                _rotation.PlagueStrike = value.PlagueStrike;
                _rotation.presence = value.presence;
                _rotation.PTRCalcs = value.PTRCalcs;
                _rotation.RP = value.RP;
                _rotation.ScourgeStrike = value.ScourgeStrike;
                OnPropertyChanged("rotation"); }
		}
		
		private bool _TalentsSaved = false;
		public bool TalentsSaved
		{
			get { return _TalentsSaved; }
			set { _TalentsSaved = value; OnPropertyChanged("TalentsSaved"); }
		}

        private double _weightScale = .01;
        public double WeightScale
        {
            get { return _weightScale; }
            set { _weightScale = value; }
        }

		#region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(property)); }
        }
        #endregion
	}
}
