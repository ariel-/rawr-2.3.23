using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Rawr
{
    public class GemmingTemplate : INotifyPropertyChanged
	{
		[XmlElement("Id")]
		public string _id;
		[XmlIgnore]
		public string Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[XmlElement("Model")]
		public string _model;
		[XmlIgnore]
		public string Model
		{
			get { return _model; }
			set { _model = value; }
		}

		[XmlElement("Enabled")]
		public bool _enabled;
		[XmlIgnore]
		public bool Enabled
		{
			get { return _enabled; }
			set 
            {
                _enabled = value;
                OnPropertyChanged("Enabled");
            }
		}

		[XmlElement("Group")]
		public string _group;
		[XmlIgnore]
		public string Group
		{
			get { return _group; }
			set { _group = value; }
		}

		[XmlElement("RedId")]
		public int _redId;
		[XmlIgnore]
		public int RedId
		{
			get { return _redId; }
			set { _redId = value; }
		}

        [XmlIgnore]
        private Item _redGem;
        [XmlIgnore]
        public Item RedGem
        {
            get
            {
                if (RedId == 0) return null;
                if (_redGem == null || _redGem.Id != RedId)
                {
                    _redGem = Item.LoadFromId(RedId, false, true, true);
                }
                return _redGem;
            }
            set
            {
                if (value == null)
                    RedId = 0;
                else
                    RedId = value.Id;
                _redGem = value;
                OnPropertyChanged("RedGem");
            }
        }

		[XmlElement("YellowId")]
		public int _yellowId;
		[XmlIgnore]
		public int YellowId
		{
			get { return _yellowId; }
			set { _yellowId = value; }
		}

        [XmlIgnore]
        private Item _yellowGem;
        [XmlIgnore]
        public Item YellowGem
        {
            get
            {
                if (YellowId == 0) return null;
                if (_yellowGem == null || _yellowGem.Id != YellowId)
                {
					_yellowGem = Item.LoadFromId(YellowId, false, true, true);
                }
                return _yellowGem;
            }
            set
            {
                if (value == null)
                    YellowId = 0;
                else
                    YellowId = value.Id;
                _yellowGem = value;
                OnPropertyChanged("YellowGem");
            }
        }

		[XmlElement("BlueId")]
		public int _blueId;
		[XmlIgnore]
		public int BlueId
		{
			get { return _blueId; }
			set { _blueId = value; }
		}

        [XmlIgnore]
        private Item _blueGem;
        [XmlIgnore]
        public Item BlueGem
        {
            get
            {
                if (BlueId == 0) return null;
                if (_blueGem == null || _blueGem.Id != BlueId)
                {
					_blueGem = Item.LoadFromId(BlueId, false, true, true);
                }
                return _blueGem;
            }
            set
            {
                if (value == null)
                    BlueId = 0;
                else
                    BlueId = value.Id;
                _blueGem = value;
                OnPropertyChanged("BlueGem");
            }
        }

		[XmlElement("MetaId")]
		public int _metaId;
		[XmlIgnore]
		public int MetaId
		{
			get { return _metaId; }
			set { _metaId = value; }
		}

        [XmlIgnore]
        private Item _metaGem;
        [XmlIgnore]
        public Item MetaGem
        {
            get
            {
                if (MetaId == 0) return null;
                if (_metaGem == null || _metaGem.Id != MetaId)
                {
					_metaGem = Item.LoadFromId(MetaId, false, true, true);
                }
                return _metaGem;
            }
            set
            {
                if (value == null)
                    MetaId = 0;
                else
                    MetaId = value.Id;
                _metaGem = value;
                OnPropertyChanged("MetaGem");
            }
        }

		[XmlElement("PrismaticId")]
		public int _prismaticId;
		[XmlIgnore]
		public int PrismaticId
		{
			get { return _prismaticId; }
			set { _prismaticId = value; }
		}

        [XmlIgnore]
        private Item _prismaticGem;
        [XmlIgnore]
        public Item PrismaticGem
        {
            get
            {
                if (PrismaticId == 0) return null;
                if (_prismaticGem == null || _prismaticGem.Id != PrismaticId)
                {
					_prismaticGem = Item.LoadFromId(PrismaticId, false, true, true);
                }
                return _prismaticGem;
            }
            set
            {
                if (value == null)
                    PrismaticId = 0;
                else
                    PrismaticId = value.Id;
                _prismaticGem = value;
                OnPropertyChanged("PrismaticGem");
            }
        }

		public ItemInstance GetItemInstance(Item item, Enchant enchant, bool blacksmithingSocket)
		{
			if (item == null) return null;
			Item gem1 = null;
            Item gem2 = null;
            Item gem3 = null;
			switch (item.SocketColor1)
			{
				case ItemSlot.Meta: gem1 = MetaGem; break;
                case ItemSlot.Red: gem1 = RedGem; break;
				case ItemSlot.Yellow: gem1 = YellowGem; break;
				case ItemSlot.Blue: gem1 = BlueGem; break;
				case ItemSlot.Prismatic: gem1 = PrismaticGem; break;
                case ItemSlot.None: 
                    if (blacksmithingSocket)
                    {
                        gem1 = PrismaticGem;
                        blacksmithingSocket = false;
                    }
                    break;
			}
			switch (item.SocketColor2)
			{
				case ItemSlot.Meta: gem2 = MetaGem; break;
				case ItemSlot.Red: gem2 = RedGem; break;
				case ItemSlot.Yellow: gem2 = YellowGem; break;
				case ItemSlot.Blue: gem2 = BlueGem; break;
				case ItemSlot.Prismatic: gem2 = PrismaticGem; break;
                case ItemSlot.None:
                    if (blacksmithingSocket)
                    {
                        gem2 = PrismaticGem;
                        blacksmithingSocket = false;
                    }
                    break;
            }
			switch (item.SocketColor3)
			{
				case ItemSlot.Meta: gem3 = MetaGem; break;
				case ItemSlot.Red: gem3 = RedGem; break;
				case ItemSlot.Yellow: gem3 = YellowGem; break;
				case ItemSlot.Blue: gem3 = BlueGem; break;
				case ItemSlot.Prismatic: gem3 = PrismaticGem; break;
                case ItemSlot.None:
                    if (blacksmithingSocket)
                    {
                        gem3 = PrismaticGem;
                        blacksmithingSocket = false;
                    }
                    break;
            }
			return new ItemInstance(item, gem1, gem2, gem3, enchant);
		}

        private static Dictionary<string, List<GemmingTemplate>> _allTemplates = new Dictionary<string, List<GemmingTemplate>>();
        public static Dictionary<string, List<GemmingTemplate>> AllTemplates
        {
            get { return _allTemplates; }
        }

        public static List<GemmingTemplate> CurrentTemplates
        {
            get
            {
                List<GemmingTemplate> list;
                if (!AllTemplates.TryGetValue(Calculations.Instance.Name, out list))
                {
                    list = new List<GemmingTemplate>(Calculations.Instance.DefaultGemmingTemplates);
                    AllTemplates[Calculations.Instance.Name] = list;
                }
                return list;
            }
        }

        #region INotifyPropertyChanged Members
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
	}
}
