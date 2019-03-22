using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Rawr.TankDK
{
#if !SILVERLIGHT
	[Serializable]
#endif
	public class CalculationOptionsTankDK : ICalculationOptionBase, INotifyPropertyChanged
	{
        #region Options
        private int _TargetLevel = 83;
        public int TargetLevel
        {
            get 
            {
                if ( _TargetLevel < 80 || _TargetLevel > 83)
                {
                    _TargetLevel = 83;
                }
                return _TargetLevel; 
            }
            set { _TargetLevel = value; OnPropertyChanged("TargetLevel"); }
        }
        private float _ThreatWeight = 1;
        public float ThreatWeight
        {
            get 
            {
                if ( _ThreatWeight < 0)
                {
                    _ThreatWeight = 1f;
                }
                return _ThreatWeight; 
            }
            set { _ThreatWeight = value; OnPropertyChanged("ThreatWeight"); }
        }
        private float _SurvivalWeight = 1;
        public float SurvivalWeight
        {
            get
            {
                if (_SurvivalWeight < 0)
                {
                    _SurvivalWeight = 1f;
                }
                return _SurvivalWeight;
            }
            set { _SurvivalWeight = value; OnPropertyChanged("SurvivalWeight"); }
        }
        private float _MitigationWeight = 4;
        public float MitigationWeight
        {
            get 
            {
                if (_MitigationWeight < 0)
                {
                    _MitigationWeight = 1f;
                }
                return _MitigationWeight; 
            }
            set { _MitigationWeight = value; OnPropertyChanged("MitigationWeight"); }
        }
        #region Physical Damage
        private uint _IncomingDamage = 60000u;
        public uint IncomingDamage
        {
            get 
            {
                if ( _IncomingDamage <= 0)
                {
                    _IncomingDamage = 10000u;
                }
                return _IncomingDamage; 
            }
            set { _IncomingDamage = value; OnPropertyChanged("IncomingDamage"); }
        }
        private float _BossAttackSpeed = 2f;
        public float BossAttackSpeed
        {
            get
            {
                if (_BossAttackSpeed < 0f)
                {
                    _BossAttackSpeed = 2f;
                }
                return _BossAttackSpeed;
            }
            set { _BossAttackSpeed = value; OnPropertyChanged("BossAttackSpeed"); }
        }
        #endregion
        #region Magic Damage
        private uint _IncomingMagicDamage = 15000;
        public uint IncomingMagicDamage
        {
            get
            {
                if (_IncomingMagicDamage <= 0)
                {
                    _IncomingMagicDamage = 0;
                }
                return _IncomingMagicDamage;
            }
            set { _IncomingMagicDamage = value; OnPropertyChanged("IncomingMagicDamage"); }
        }
        private float _IncomingFromMagicFrequency = 20;
        public float IncomingFromMagicFrequency
        {
            get 
            {
                if (_IncomingFromMagicFrequency < 0)
                {
                    _IncomingFromMagicFrequency = .0f;
                }
                return _IncomingFromMagicFrequency; 
            }
            set { _IncomingFromMagicFrequency = value; OnPropertyChanged("numIncFromMagicFrequency"); }
        }
        #endregion
        #region Bleed Damage
        private uint _IncomingBleedDamage = 4000;
        public uint IncomingBleedDamage
        {
            get
            {
                if (_IncomingBleedDamage <= 0)
                {
                    _IncomingBleedDamage = 0;
                }
                return _IncomingBleedDamage;
            }
            set { _IncomingBleedDamage = value; OnPropertyChanged("IncomingMagicDamage"); }
        }
        private float _IncomingBleedTickFrequency = 2;
        public float BleedTickFrequency
        {
            get
            {
                if (_IncomingBleedTickFrequency < 0)
                {
                    _IncomingBleedTickFrequency = .0f;
                }
                return _IncomingBleedTickFrequency;
            }
            set { _IncomingBleedTickFrequency = value; OnPropertyChanged("BleedTickFrequency"); }
        }
        #endregion

        private float _BossArmor = StatConversion.NPC_ARMOR[83 - 80];
        public float BossArmor 
        {
            get 
            {
                if (_BossArmor < 0)
                {
                    _BossArmor = StatConversion.NPC_ARMOR[83 - 80];
                }
                return _BossArmor; 
            }
            set { _BossArmor = value; OnPropertyChanged("BossArmor"); }
        }
		public bool Bloodlust = false;

        private float _fightLength = 10;
        public float FightLength
        {
            get 
            {
                if ( _fightLength < 1f)
                {
                    _fightLength = 10f;
                }
                return _fightLength; 
            }
            set { _fightLength = value; OnPropertyChanged("FightLength"); }
        }

        private uint _uNumberTargets = 1;
        public uint uNumberTargets
        {
            get 
            {
                if ( _uNumberTargets < 1)
                {
                    _uNumberTargets = 1;
                }
                return _uNumberTargets; 
            }
            set { _uNumberTargets = value; OnPropertyChanged("uNumberTargets"); }
        }
        private CalculationType _cType ;
        public CalculationType cType
        {
            get 
            {
                if (_cType != CalculationType.Burst 
                    && _cType != CalculationType.SMT)
                {
                    _cType = CalculationType.SMT;
                }
                return _cType; 
            }
            set { _cType = value; OnPropertyChanged("cType"); }
        }

        private bool _m_bExperimental = false;
        public bool bExperimental
        {
            get
            {
                return _m_bExperimental;
            }
            set { _m_bExperimental = value; OnPropertyChanged("Experimental"); }
        }

        private bool _m_AdditiveMitigation = false;
        public bool AdditiveMitigation
        {
            get
            {
                return _m_AdditiveMitigation;
            }
            set { _m_AdditiveMitigation = value; OnPropertyChanged("AdditiveMitigation"); }
        }

        private bool _m_bEnableParryHaste = true;
        public bool bParryHaste
        {
            get
            {
                return _m_bEnableParryHaste;
            }
            set { _m_bEnableParryHaste = value; OnPropertyChanged("EnableParryHaste"); }
        }

        private bool _m_bUseOnUseAbilities = true;
        public bool bUseOnUseAbilities
        {
            get
            {
                return _m_bUseOnUseAbilities;
            }
            set { _m_bUseOnUseAbilities = value; OnPropertyChanged("UseOnUseAbilities"); }
        }
        #endregion

        #region Rotation

        private Rotation _m_rotation;
        public Rotation m_Rotation
        {
            get
            {
                if (null == _m_rotation || null == _m_rotation.tTalents || talents != _m_rotation.tTalents)
                {
                    _m_rotation = new Rotation(talents);
                }
                return _m_rotation;
            }
            set { _m_rotation = value; }
        }

        [XmlIgnore]
        public float CurRotationDuration
        {
            get { return m_Rotation.curRotationDuration; }
            set { m_Rotation.curRotationDuration = value; OnPropertyChanged("CurRotationDuration"); }
        }
        // Single Rune, Single Target
        [XmlIgnore]
        public float IcyTouch
        {
            get { return m_Rotation.IcyTouch; }
            set { m_Rotation.IcyTouch = value; OnPropertyChanged("IcyTouch"); }
        }
        [XmlIgnore]
        public float PlagueStrike
        {
            get { return m_Rotation.PlagueStrike; }
            set { m_Rotation.PlagueStrike = value; OnPropertyChanged("PlagueStrike"); }
        }
        [XmlIgnore]
        public float BloodStrike
        {
            get { return m_Rotation.BloodStrike; }
            set { m_Rotation.BloodStrike = value; OnPropertyChanged("BloodStrike"); }
        }
        [XmlIgnore]
        public float HeartStrike
        {
            get { return m_Rotation.HeartStrike; }
            set { m_Rotation.HeartStrike = value; OnPropertyChanged("HeartStrike"); }
        }
        // Multi Rune, Single Target
        [XmlIgnore]
        public float DeathStrike
        {
            get { return m_Rotation.DeathStrike; }
            set { m_Rotation.DeathStrike = value; OnPropertyChanged("DeathStrike"); }
        }
        [XmlIgnore]
        public float Obliterate
        {
            get { return m_Rotation.Obliterate; }
            set { m_Rotation.Obliterate = value; OnPropertyChanged("Obliterate"); }
        }
        [XmlIgnore]
        public float ScourgeStrike
        {
            get { return m_Rotation.ScourgeStrike; }
            set { m_Rotation.ScourgeStrike = value; OnPropertyChanged("ScourgeStrike"); }
        }
        
        // Multi Target
        [XmlIgnore]
        public float Pestilence
        {
            get { return m_Rotation.Pestilence; }
            set { m_Rotation.Pestilence = value; OnPropertyChanged("Pestilence"); }
        }
        [XmlIgnore]
        public float HowlingBlast
        {
            get { return m_Rotation.HowlingBlast; }
            set { m_Rotation.HowlingBlast = value; OnPropertyChanged("HowlingBlast"); }
        }
        [XmlIgnore]
        public float DeathNDecay
        {
            get { return m_Rotation.DeathNDecay; }
            set { m_Rotation.DeathNDecay = value; OnPropertyChanged("DeathNDecay"); }
        }
        [XmlIgnore]
        public float BloodBoil
        {
            get { return m_Rotation.BloodBoil; }
            set { m_Rotation.BloodBoil = value; OnPropertyChanged("BloodBoil"); }
        }

        // RP
        [XmlIgnore]
        public float RuneStrike
        {
            get { return m_Rotation.RuneStrike; }
            set { m_Rotation.RuneStrike = value; OnPropertyChanged("RuneStrike"); }
        }
        [XmlIgnore]
        public float DeathCoil
        {
            get { return m_Rotation.DeathCoil; }
            set { m_Rotation.DeathCoil = value; OnPropertyChanged("DeathCoil"); }
        }
        [XmlIgnore]
        public float FrostStrike
        {
            get { return m_Rotation.FrostStrike; }
            set { m_Rotation.FrostStrike = value; OnPropertyChanged("FrostStrike"); }
        }
        // Other GCD
        [XmlIgnore]
        public float HornOfWinter
        {
            get { return m_Rotation.HornOfWinter; }
            set { m_Rotation.HornOfWinter = value; OnPropertyChanged("HornOfWinter"); }
        }
        #endregion

        #region BossHandler
        private BossHandler _hCurrentBoss = new Bosses.LordMarrowgar()[(int)BossHandler.Versions.V_10N];
        public BossHandler hCurrentBoss
        {
            get
            {
                return _hCurrentBoss;
            }
            set { _hCurrentBoss = value; OnPropertyChanged("BossHandler"); }
        }
        #endregion

        public DeathKnightTalents talents;

        #region XML IO
        public string GetXml()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsTankDK));
			StringBuilder xml = new StringBuilder();
			System.IO.StringWriter writer = new System.IO.StringWriter(xml);
			serializer.Serialize(writer, this);
			return xml.ToString();
        }
        #endregion
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion
	}
}
