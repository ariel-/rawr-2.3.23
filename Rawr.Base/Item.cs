using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.ComponentModel;
#if RAWR3
using System.Linq;
#else
using System.Windows.Forms;
#endif
using System.Net;

namespace Rawr
{
	#region Item
	
	public class Item : IComparable<Item>
	{
		[XmlElement("ItemLevel")]
		public int _itemLevel;

        [XmlElement("DisplayId")]
        public int _displayId;

        [XmlElement("DisplaySlot")]
        public int _displaySlot;

        [XmlElement("IconPath")]
		public string _iconPath;

		[XmlElement("Stats")]
		public Stats _stats = new Stats();

		[XmlElement("Quality")]
		public ItemQuality _quality;

		[DefaultValueAttribute("")]
		[XmlElement("SetName")]
		public string _setName;
		
		[XmlElement("Type")]
		public ItemType _type = ItemType.None;

		[XmlElement("Faction")]
		public ItemFaction _faction = ItemFaction.Neutral;
		
		[DefaultValueAttribute(0)]
		[XmlElement("MinDamage")]
		public int _minDamage = 0;

		[DefaultValueAttribute(0)]
		[XmlElement("MaxDamage")]
		public int _maxDamage = 0;

		[DefaultValueAttribute(0)]
		[XmlElement("DamageType")]
		public ItemDamageType _damageType = ItemDamageType.Physical;

		[DefaultValueAttribute(0)]
		[XmlElement("Speed")]
		public float _speed = 0f;

		[DefaultValueAttribute("")]
		[XmlElement("RequiredClasses")]
		public string _requiredClasses;

		[DefaultValueAttribute(false)]
		[XmlElement("Unique")]
		public bool _unique;

        /// <summary>
        /// List of Ids that cannot be used together with this item (other than this item).
        /// Unique should be set to true if this is not empty.
        /// </summary>
        [XmlElement("UniqueId")]
        public List<int> UniqueId { get; set; }

        [DefaultValueAttribute(BindsOn.None)]
        [XmlElement("Bind")]
        public BindsOn _bind;

		[DefaultValueAttribute(0)]
		[XmlElement("SocketColor1")]
		public ItemSlot _socketColor1;

		[DefaultValueAttribute(0)]
		[XmlElement("SocketColor2")]
		public ItemSlot _socketColor2;

		[DefaultValueAttribute(0)]
		[XmlElement("SocketColor3")]
		public ItemSlot _socketColor3;

		[XmlElement("SocketBonus")]
		public Stats _socketBonus = new Stats();

        [XmlElement("LocalizedName")]
        public string _localizedName;
        
        public ItemLocation[] LocationInfo {
			get { return LocationFactory.Lookup(Id); }
		}

        /// <summary>
        /// Cost of acquiring the item (i.e. badges, dkp, gold, etc.)
        /// </summary>
        [DefaultValueAttribute(0.0f)]
        public float Cost { get; set; }

        [XmlIgnore]
        public DateTime LastChange { get; set; }

        public void InvalidateCachedData()
        {
            Stats.InvalidateSparseData();
            SocketBonus.InvalidateSparseData();
            LastChange = DateTime.Now;
        }

        [XmlIgnore]
        public bool Invalid { get; set; }

        public void Delete()
        {
            Invalid = true;
        }

        private string _name;
        [XmlIgnore]
		public string Name
		{
			get 
            {
                if (_localizedName != null && !Rawr.Properties.GeneralSettings.Default.Locale.Equals("en"))
                {
                    return _localizedName;
                }
                else
                {
                    return _name;
                }
            }
			set
            {
                _name = value;
                UpdateGemInformation();
            }
		}

        [XmlElement("Name")]
        public string EnglishName
        {
            get
            { 
                return _name; 
            }
            set
            { 
                _name = value;
                UpdateGemInformation();
            }
        }

        private int _id;
        public int Id
		{
			get 
            {
                return _id; 
            }
			set
			{
				_id = value;
                UpdateGemInformation();
			}
		}

		[XmlIgnore]
		public int ItemLevel
		{
			get { return _itemLevel; }
			set
			{
				_itemLevel = value;
			}
		}
        [XmlIgnore]
        public int DisplayId
        {
            get { return _displayId; }
            set
            {
                _displayId = value;
            }
        }
        [XmlIgnore]
        public int DisplaySlot
        {
            get { return _displaySlot; }
            set
            {
                _displaySlot = value;
            }
        }
        [XmlIgnore]
		public string IconPath
		{
			get { return (_iconPath == null ? null : _iconPath.ToLower(System.Globalization.CultureInfo.InvariantCulture)); }
			set { _iconPath = value.ToLower(System.Globalization.CultureInfo.InvariantCulture); }
		}

        private ItemSlot _slot;
		public ItemSlot Slot
		{
			get 
            { 
                return _slot; 
            }
			set 
            {
                _slot = value;
                UpdateGemInformation();
            }
		}

		/// <summary>
		/// String version of Slot, to facilitate databinding.
		/// </summary>
		[XmlIgnore]
		public string SlotString
		{
			get 
            { 
                return _slot.ToString(); 
            }
			set 
            { 
                _slot = (ItemSlot)Enum.Parse(typeof(ItemSlot), value, false);
                UpdateGemInformation();
            }
		}

		[XmlIgnore]
		public Stats Stats
		{
			get { return _stats; }
			set { _stats = value; }
		}
		[XmlIgnore]
		public ItemQuality Quality
		{
			get
			{
				return _quality;
			}
			set
			{
				_quality = value;
			}
		}
		[XmlIgnore]
		public string SetName
		{
			get
			{
				return _setName;
			}
			set
			{
				_setName = value;
			}
		}
		[XmlIgnore]
		public ItemFaction Faction
		{
			get
			{
				return _faction;
			}
			set
			{
				_faction = value;
			}
		}

        public bool FitsFaction(CharacterRace race)
        {
            bool fitsFaction = true;
            if (Faction != ItemFaction.Neutral)
            {
                switch (race)
                {
                    case CharacterRace.Draenei:
                    case CharacterRace.Dwarf:
                    case CharacterRace.Gnome:
                    case CharacterRace.Human:
                    case CharacterRace.NightElf:
                        fitsFaction &= Faction == ItemFaction.Alliance;
                        break;

                    default:
                        fitsFaction &= Faction == ItemFaction.Horde;
                        break;
                }
            }
            return fitsFaction;
        }

		/// <summary>
		/// String version of Faction, to facilitate databinding
		/// </summary>
		[XmlIgnore]
		public string FactionString
		{
			get { return _faction.ToString(); }
			set { _faction = (ItemFaction)Enum.Parse(typeof(ItemFaction), value, false); }
		}
		[XmlIgnore]
		public ItemType Type {
			get { return _type; }
			set { _type = value; }
		}
		/// <summary>
		/// String version of Type, to facilitate databinding
		/// </summary>
		[XmlIgnore]
		public string TypeString {
			get { return _type.ToString(); }
			set { _type = (ItemType)Enum.Parse(typeof(ItemType), value, false); }
		}
		[XmlIgnore]
		public int MinDamage {
			get { return _minDamage; }
			set { _minDamage = value; }
		}
		[XmlIgnore]
		public int MaxDamage {
			get { return _maxDamage; }
			set { _maxDamage = value; }
		}
		[XmlIgnore]
		public ItemDamageType DamageType {
			get { return _damageType; }
			set { _damageType = value; }
		}
		[XmlIgnore]
		public float Speed {
			get { return _speed; }
			set { _speed = value; }
		}
		[XmlIgnore]
		public float DPS {
			get {
				if (Speed == 0f) return 0f;
				else return ((float)(MinDamage + MaxDamage) * 0.5f) / Speed;
			}
		}
		[XmlIgnore]
		public string RequiredClasses {
			get { return _requiredClasses; }
			set { _requiredClasses = value; }
		}
		[XmlIgnore]
		public bool Unique {
			get { return _unique; }
			set { _unique = value; }
		}
		[XmlIgnore]
        public BindsOn Bind
        {
            get { return _bind; }
            set { _bind = value; }
        }
        [XmlIgnore]
		public ItemSlot SocketColor1
		{
			get { return _socketColor1; }
			set { _socketColor1 = value; }
		}
		[XmlIgnore]
		public ItemSlot SocketColor2
		{
			get { return _socketColor2; }
			set { _socketColor2 = value; }
		}
		[XmlIgnore]
		public ItemSlot SocketColor3
		{
			get { return _socketColor3; }
			set { _socketColor3 = value; }
		}
		public ItemSlot GetSocketColor(int index)
		{
			switch (index)
			{
				case 1:
					return SocketColor1;
				case 2:
					return SocketColor2;
				case 3:
					return SocketColor3;
				default:
					return ItemSlot.None;
			}
		}
		[XmlIgnore]
		public string SocketColor1String
		{
			get { return _socketColor1.ToString(); }
			set { _socketColor1 = (ItemSlot)Enum.Parse(typeof(ItemSlot), value, false); }
		}
		[XmlIgnore]
		public string SocketColor2String
		{
			get { return _socketColor2.ToString(); }
			set { _socketColor2 = (ItemSlot)Enum.Parse(typeof(ItemSlot), value, false); }
		}
		[XmlIgnore]
		public string SocketColor3String
		{
			get { return _socketColor3.ToString(); }
			set { _socketColor3 = (ItemSlot)Enum.Parse(typeof(ItemSlot), value, false); }
		}
		[XmlIgnore]
		public Stats SocketBonus
		{
			get { return _socketBonus; }
			set { _socketBonus = value; }
		}
        [XmlIgnore]
        public string LocalizedName
        {
            get { return _localizedName; }
            set { _localizedName = value; }
        }

		public static ItemSlot GetItemSlotByCharacterSlot(CharacterSlot slot)
		{
			switch (slot)
			{
				case CharacterSlot.Projectile: return ItemSlot.Projectile;
				case CharacterSlot.Head: return ItemSlot.Head;
				case CharacterSlot.Neck: return ItemSlot.Neck;
				case CharacterSlot.Shoulders: return ItemSlot.Shoulders;
				case CharacterSlot.Chest: return ItemSlot.Chest;
				case CharacterSlot.Waist: return ItemSlot.Waist;
				case CharacterSlot.Legs: return ItemSlot.Legs;
				case CharacterSlot.Feet: return ItemSlot.Feet;
				case CharacterSlot.Wrist: return ItemSlot.Wrist;
				case CharacterSlot.Hands: return ItemSlot.Hands;
				case CharacterSlot.Finger1: return ItemSlot.Finger;
				case CharacterSlot.Finger2: return ItemSlot.Finger;
				case CharacterSlot.Trinket1: return ItemSlot.Trinket;
				case CharacterSlot.Trinket2: return ItemSlot.Trinket;
				case CharacterSlot.Back: return ItemSlot.Back;
				case CharacterSlot.MainHand: return ItemSlot.MainHand;
				case CharacterSlot.OffHand: return ItemSlot.OffHand;
				case CharacterSlot.Ranged: return ItemSlot.Ranged;
				case CharacterSlot.ProjectileBag: return ItemSlot.ProjectileBag;
                //case CharacterSlot.ExtraWristSocket: return ItemSlot.Prismatic;
                //case CharacterSlot.ExtraHandsSocket: return ItemSlot.Prismatic;
                //case CharacterSlot.ExtraWaistSocket: return ItemSlot.Prismatic;
				case CharacterSlot.Tabard: return ItemSlot.Tabard;
				case CharacterSlot.Shirt: return ItemSlot.Shirt;
				case CharacterSlot.Gems: return ItemSlot.Prismatic;
				case CharacterSlot.Metas: return ItemSlot.Meta;
				default: return ItemSlot.None;
			}
		}

        private bool _isGem;
		public bool IsGem
		{
			get
			{
				return _isGem;
			}
		}

        private bool _isRedGem;
        public bool IsRedGem
        {
            get
            {
                return _isRedGem;
            }
        }

        private bool _isYellowGem;
        public bool IsYellowGem
        {
            get
            {
                return _isYellowGem;
            }
        }

        private bool _isBlueGem;
        public bool IsBlueGem
        {
            get
            {
                return _isBlueGem;
            }
        }

        private bool _isJewelersGem;
		public bool IsJewelersGem
		{
			get
			{
                return _isJewelersGem;
			}
		}

        public bool IsLimitedGem
        {
            get
            {
                return _isGem && (_isJewelersGem || Unique);
            }
        }

        internal static bool IsJewelersGemId(int id)
        {
            return (id == 42142 || id == 36766 || id == 42148 || id == 42143 || id == 42152 || id == 42153 || id == 42146 || id == 42158 || id == 42154 || id == 42150 || id == 42156 || id == 42144 || id == 42149 || id == 36767 || id == 42145 || id == 42155 || id == 42151 || id == 42157);
        }

        private void UpdateGemInformation()
        {
            _isGem = Slot == ItemSlot.Meta || Slot == ItemSlot.Blue || Slot == ItemSlot.Green || Slot == ItemSlot.Orange || Slot == ItemSlot.Prismatic || Slot == ItemSlot.Purple || Slot == ItemSlot.Red || Slot == ItemSlot.Yellow;
            _isJewelersGem = IsJewelersGemId(Id);
            _isRedGem = _isGem && Item.GemMatchesSlot(this, ItemSlot.Red);
            _isYellowGem = _isGem && Item.GemMatchesSlot(this, ItemSlot.Yellow);
            _isBlueGem = _isGem && Item.GemMatchesSlot(this, ItemSlot.Blue);
        }

		public Item() { }
		public Item(string name, ItemQuality quality, ItemType type, int id, string iconPath, ItemSlot slot, string setName, bool unique, Stats stats, Stats socketBonus, ItemSlot socketColor1, ItemSlot socketColor2, ItemSlot socketColor3, int minDamage, int maxDamage, ItemDamageType damageType, float speed, string requiredClasses)
		{
			_name = name;
			_id = id;
			_iconPath = iconPath;
			_slot = slot;
			_stats = stats;
			_socketBonus = socketBonus;
			_socketColor1 = socketColor1;
			_socketColor2 = socketColor2;
			_socketColor3 = socketColor3;
			_setName = setName;
			_quality = quality;
			_type = type;
			_minDamage = minDamage;
			_maxDamage = maxDamage;
			_damageType = damageType;
			_speed = speed;
			_requiredClasses = requiredClasses;
			_unique = unique;
            UpdateGemInformation();
		}

		public Item Clone()
		{
			return new Item()
			{
				Name = this.Name,
				Quality = this.Quality,
				Id = this.Id,
				IconPath = this.IconPath,
				Slot = this.Slot,
				Stats = this.Stats.Clone(),
				SocketBonus = this.SocketBonus.Clone(),
				SocketColor1 = this.SocketColor1,
				SocketColor2 = this.SocketColor2,
				SocketColor3 = this.SocketColor3,
				SetName = this.SetName,
				Type = this.Type,
				MinDamage = this.MinDamage,
				MaxDamage = this.MaxDamage,
				DamageType = this.DamageType,
				Speed = this.Speed,
				RequiredClasses = this.RequiredClasses,
				Unique = this.Unique,
                UniqueId = new List<int>(this.UniqueId),
                LocalizedName = this.LocalizedName
			};
		}

		public override string ToString()
		{
			return (!string.IsNullOrEmpty(this.Name) ? this.Name : this.Id.ToString("00000")) + ": " + 
                (this.Bind != BindsOn.None ? (this.Bind + " ") : string.Empty) +
                this.Stats.ToString();
		}

		public static bool GemMatchesSlot(Item gem, ItemSlot slotColor)
		{
			switch (slotColor)
			{
				case ItemSlot.Red:
					return gem != null && (gem.Slot == ItemSlot.Red || gem.Slot == ItemSlot.Orange || gem.Slot == ItemSlot.Purple || gem.Slot == ItemSlot.Prismatic);
				case ItemSlot.Yellow:
                    return gem != null && (gem.Slot == ItemSlot.Yellow || gem.Slot == ItemSlot.Orange || gem.Slot == ItemSlot.Green || gem.Slot == ItemSlot.Prismatic);
				case ItemSlot.Blue:
                    return gem != null && (gem.Slot == ItemSlot.Blue || gem.Slot == ItemSlot.Green || gem.Slot == ItemSlot.Purple || gem.Slot == ItemSlot.Prismatic);
				case ItemSlot.Meta:
					return gem != null && (gem.Slot == ItemSlot.Meta);
				default:
                    return gem == null || gem.Slot != ItemSlot.Meta;
			}
        }

#if RAWR3
        public static Dictionary<ItemSlot, CharacterSlot> DefaultSlotMap { get; private set; }
		static Item()
		{
            Dictionary<ItemSlot, CharacterSlot> list = new Dictionary<ItemSlot, CharacterSlot>();
#else

		public static SortedList<ItemSlot, CharacterSlot> DefaultSlotMap { get; private set; }
		static Item()
		{
            SortedList<ItemSlot, CharacterSlot> list = new SortedList<ItemSlot, CharacterSlot>();
#endif
            foreach (ItemSlot iSlot in EnumHelper.GetValues(typeof(ItemSlot)))
			{
				list[iSlot] = CharacterSlot.None;
			}
			list[ItemSlot.Head] = CharacterSlot.Head;
			list[ItemSlot.Neck] = CharacterSlot.Neck;
			list[ItemSlot.Shoulders] = CharacterSlot.Shoulders;
			list[ItemSlot.Back] = CharacterSlot.Back;
			list[ItemSlot.Chest] = CharacterSlot.Chest;
			list[ItemSlot.Shirt] = CharacterSlot.Shirt;
			list[ItemSlot.Tabard] = CharacterSlot.Tabard;
			list[ItemSlot.Wrist] = CharacterSlot.Wrist;
			list[ItemSlot.Hands] = CharacterSlot.Hands;
			list[ItemSlot.Waist] = CharacterSlot.Waist;
			list[ItemSlot.Legs] = CharacterSlot.Legs;
			list[ItemSlot.Feet] = CharacterSlot.Feet;
			list[ItemSlot.Finger] = CharacterSlot.Finger1;
			list[ItemSlot.Trinket] = CharacterSlot.Trinket1;
			list[ItemSlot.OneHand] = CharacterSlot.MainHand;
			list[ItemSlot.TwoHand] = CharacterSlot.MainHand;
			list[ItemSlot.MainHand] = CharacterSlot.MainHand;
			list[ItemSlot.OffHand] = CharacterSlot.OffHand;
			list[ItemSlot.Ranged] = CharacterSlot.Ranged;
			list[ItemSlot.Projectile] = CharacterSlot.Projectile;
			list[ItemSlot.ProjectileBag] = CharacterSlot.ProjectileBag;
#if RAWR3
            list.OrderBy(kvp => (int)kvp.Key);
#else
            list.TrimExcess();
#endif
			DefaultSlotMap = list;
		}

		public bool FitsInSlot(CharacterSlot charSlot)
		{
			//And if I fell with all the strength I held inside...
			switch (charSlot)
			{
				case CharacterSlot.Head:
					return this.Slot == ItemSlot.Head;
				case CharacterSlot.Neck:
					return this.Slot == ItemSlot.Neck;
				case CharacterSlot.Shoulders:
					return this.Slot == ItemSlot.Shoulders;
				case CharacterSlot.Back:
					return this.Slot == ItemSlot.Back;
				case CharacterSlot.Chest:
					return this.Slot == ItemSlot.Chest;
				case CharacterSlot.Shirt:
					return this.Slot == ItemSlot.Shirt;
				case CharacterSlot.Tabard:
					return this.Slot == ItemSlot.Tabard;
				case CharacterSlot.Wrist:
					return this.Slot == ItemSlot.Wrist;
				case CharacterSlot.Hands:
					return this.Slot == ItemSlot.Hands;
				case CharacterSlot.Waist:
					return this.Slot == ItemSlot.Waist;
				case CharacterSlot.Legs:
					return this.Slot == ItemSlot.Legs;
				case CharacterSlot.Feet:
					return this.Slot == ItemSlot.Feet;
				case CharacterSlot.Finger1:
				case CharacterSlot.Finger2:
					return this.Slot == ItemSlot.Finger;
				case CharacterSlot.Trinket1:
				case CharacterSlot.Trinket2:
					return this.Slot == ItemSlot.Trinket;
				case CharacterSlot.MainHand:
					return this.Slot == ItemSlot.TwoHand || this.Slot == ItemSlot.OneHand || this.Slot == ItemSlot.MainHand;
				case CharacterSlot.OffHand:
					return this.Slot == ItemSlot.OneHand || this.Slot == ItemSlot.OffHand;
				case CharacterSlot.Ranged:
					return this.Slot == ItemSlot.Ranged;
				case CharacterSlot.Projectile:
					return this.Slot == ItemSlot.Projectile;
				case CharacterSlot.ProjectileBag:
					return this.Slot == ItemSlot.ProjectileBag;
                //case CharacterSlot.ExtraWristSocket:
                //case CharacterSlot.ExtraHandsSocket:
                //case CharacterSlot.ExtraWaistSocket:
				case CharacterSlot.Gems:
					return this.Slot == ItemSlot.Red || this.Slot == ItemSlot.Blue || this.Slot == ItemSlot.Yellow
						|| this.Slot == ItemSlot.Purple || this.Slot == ItemSlot.Green || this.Slot == ItemSlot.Orange
						|| this.Slot == ItemSlot.Prismatic;
				case CharacterSlot.Metas:
					return this.Slot == ItemSlot.Meta;
				default:
					return false;
			}
			//I wouldn't be out here... alone tonight
		}

        public bool FitsInSlot(CharacterSlot charSlot, Character character)
        {
            return Calculations.ItemFitsInSlot(this, character, charSlot, false);
        }

		public bool FitsInSlot(CharacterSlot charSlot, Character character, bool ignoreUnique)
		{
			return Calculations.ItemFitsInSlot(this, character, charSlot, ignoreUnique);
		}

		public bool MeetsRequirements(Character character)
		{
			bool temp;
			return MeetsRequirements(character, out temp);
		}

        public static bool ItemsAreConsideredUniqueEqual(Item itema, Item itemb)
        {
            return (object)itema != null && (object)itemb != null && itema.Unique && (itema.Id == itemb.Id || (itema.UniqueId != null && itema.UniqueId.Contains(itemb.Id)));
        }

        public static bool OptimizerManagedVolatiliy { get; set; }

		/// <summary>
		/// Checks whether item meets the requirements to activate its stats.
		/// </summary>
		/// <param name="character">Character for which we are checking the requirements.</param>
		/// <param name="volatileRequirements">This is set to true for items that depend on requirements not local to the item itself.</param>
		/// <returns>True if the item meets the requirements.</returns>
		public bool MeetsRequirements(Character character, out bool volatileRequirements)
		{
			volatileRequirements = false;
			bool meetsRequirements = true;

			if (this.Slot == ItemSlot.Meta)
			{
				#region Metagem Requirements
				volatileRequirements = true;
                if (character == null
#if !RAWR3 && !SILVERLIGHT
                    || !character.EnforceGemRequirements
#else
                    || !Rawr.Properties.GeneralSettings.Default.EnforceGemRequirements
                    || !Rawr.Properties.GeneralSettings.Default.EnforceGemRequirements_Meta
#endif
                    ) { return true; }

				int redGems = 0, yellowGems = 0, blueGems = 0;
				if (character != null)
				{
					/*redGems = character.GetGemColorCount(ItemSlot.Red);
					yellowGems = character.GetGemColorCount(ItemSlot.Yellow);
					blueGems = character.GetGemColorCount(ItemSlot.Blue);*/
					redGems = character.RedGemCount;
					yellowGems = character.YellowGemCount;
					blueGems = character.BlueGemCount;
				}

				//TODO: Make this dynamic, by loading the gem requirements from the armory
				switch (this.Id)
				{
					case 25899:
					case 25890:
					case 25901:
					case 32409:
					case 32410:
						volatileRequirements = true; //2 of each
						meetsRequirements = redGems >= 2 && yellowGems >= 2 && blueGems >= 2;
						break;
					case 41307:
					case 41401:
					case 41400:
					case 41375:
					case 44078:
					case 44089:
					case 41382:
					case 41398:
						volatileRequirements = true; //1 of each
						meetsRequirements = redGems >= 1 && yellowGems >= 1 && blueGems >= 1;
						break;
					case 25897:
						volatileRequirements = true; //More reds than blues
						meetsRequirements = redGems > blueGems;
						break;
					case 25895:
						volatileRequirements = true; //More reds than yellows
						meetsRequirements = redGems > yellowGems;
						break;
					case 25893:
					case 32640:
						volatileRequirements = true; //More blues than yellows
						meetsRequirements = blueGems > yellowGems;
						break;
					case 34220:
					case 41285:
						volatileRequirements = true; //2 blues
						meetsRequirements = blueGems >= 2;
						break;
					case 41376:
						volatileRequirements = true; //2 reds
						meetsRequirements = redGems >= 2;
						break;
					case 25896:
					case 44087:
						volatileRequirements = true; //3 blues
						meetsRequirements = blueGems >= 3;
						break;
					case 25898:
						volatileRequirements = true; //5 blues
						meetsRequirements = blueGems >= 5;
						break;
					case 32641:
						volatileRequirements = true; //exactly 3 yellows
						meetsRequirements = yellowGems == 3;
						break;
					case 41333:
						volatileRequirements = true; //3 red
						meetsRequirements = redGems >= 3;
						break;
					case 25894:
					case 28557:
					case 28556:
					case 41339:
					case 44076:
						volatileRequirements = true; //2 yellows, 1 red
						meetsRequirements = yellowGems >= 2 && redGems >= 1;
						break;
					case 35501:
					case 44088:
						volatileRequirements = true; //1 yellow, 2 blue
						meetsRequirements = yellowGems >= 1 && blueGems >= 2;
						break;
					case 41378:
					case 44084:
					case 41381:
						volatileRequirements = true; //2 yellow, 1 blue
						meetsRequirements = yellowGems >= 2 && blueGems >= 1;
						break;
					case 41380:
					case 41377:
					case 44082:
					case 41385:
						volatileRequirements = true; //2 blue, 1 red
						meetsRequirements = blueGems >= 2 && redGems >= 1;
						break;
					case 41335:
					case 41389:
						volatileRequirements = true; //2 red, 1 yellow
						meetsRequirements = redGems >= 2 && yellowGems >= 1;
						break;
					case 41379:
					case 44081:
					case 41396:
					case 41395:
						volatileRequirements = true; //2 red, 1 blue
						meetsRequirements = redGems >= 2 && blueGems >= 1;
						break;
					default:
						meetsRequirements = true;
						break;
				}
				#endregion
			}
            else if (!OptimizerManagedVolatiliy)
			{
				#region Gem Requirements
				if (IsJewelersGem)
				{
					volatileRequirements = true;
					if (character == null
#if !RAWR3 && !SILVERLIGHT
                    || !character.EnforceGemRequirements
#else
                    || !Rawr.Properties.GeneralSettings.Default.EnforceGemRequirements
                    || !Rawr.Properties.GeneralSettings.Default.EnforceGemRequirements_JC
#endif
                    ) { return true; }
					meetsRequirements = character.JewelersGemCount <= 3;
				}
				else if (Unique)
				{
					volatileRequirements = true;
                    if (character == null
#if !RAWR3 && !SILVERLIGHT
                    || !character.EnforceGemRequirements
#else
                    || !Rawr.Properties.GeneralSettings.Default.EnforceGemRequirements
                    || !Rawr.Properties.GeneralSettings.Default.EnforceGemRequirements_Unique
#endif
                    ) { return true; }
					meetsRequirements = character.GetGemIdCount(Id) <= 1;
				}
				else
				{
					volatileRequirements = false;
					meetsRequirements = true;
				}
				#endregion
			}

			return meetsRequirements;
		}


		public static Item LoadFromId(int id) { return LoadFromId(id, false, true, false); }
        public static Item LoadFromId(int id, bool forceRefresh, bool raiseEvent, bool useWowhead) { return LoadFromId(id, forceRefresh, raiseEvent, useWowhead, Rawr.Properties.GeneralSettings.Default.Locale); }
        public static Item LoadFromId(int id, bool forceRefresh, bool raiseEvent, bool useWowhead, string locale) { return LoadFromId(id, forceRefresh, raiseEvent, useWowhead, locale, "www"); }
        public static Item LoadFromId(int id, bool forceRefresh, bool raiseEvent, bool useWowhead, string locale, string wowheadSite)
		{
			Item cachedItem = ItemCache.FindItemById(id);
            #if DEBUG
            string oldItemStats = "";
            string oldItemSource = "";
            ItemLocation[] oldItemLoc = null;
            if (cachedItem != null && forceRefresh){
                oldItemStats  = cachedItem.ToString().Split(':')[1];
                oldItemLoc    = cachedItem.LocationInfo;
                oldItemSource = (cachedItem.LocationInfo[0].Description
                    + (cachedItem.LocationInfo[1] != null ? " and" + cachedItem.LocationInfo[1].Description.Replace("Purchasable with", "") : ""));
            }
            #endif

            if (cachedItem != null && !forceRefresh) return cachedItem;
			else
			{
#if RAWR3
				ElitistArmoryService armoryService = new ElitistArmoryService();
				armoryService.GetItemCompleted += new EventHandler<EventArgs<Item>>(armoryService_GetItemCompleted);
				armoryService.GetItemAsync(id);
				
				if (cachedItem != null) return cachedItem;
                else
                {
                    Item tempItem = new Item();
                    tempItem.Name = "[Downloading from Armory]";
                    tempItem.Id = id;
                    ItemCache.AddItem(tempItem, raiseEvent);
                    return tempItem;
                }
#else
				Item newItem = useWowhead ? Wowhead.GetItem(wowheadSite, id.ToString(), false) : Armory.GetItem(id);
                if (newItem != null)
                {
                    if (!locale.Equals("en"))
                    {
                        WebRequestWrapper wrw = new WebRequestWrapper();
                        if (locale.Equals("zhTW"))
                            newItem.LocalizedName = wrw.GetNameFromArmory(id, "tw");
                        else if (locale.Equals("zhCN"))
                            newItem.LocalizedName = wrw.GetNameFromArmory(id, "cn");
                        else if (locale.Equals("kr"))
                            newItem.LocalizedName = wrw.GetNameFromArmory(id, "kr");
                        else
                        {
                            Item localItem = Wowhead.GetItem(id, false, locale);
                            if (localItem != null)
                                newItem.LocalizedName = localItem.Name;
                        }
                    }
					if (!useWowhead && newItem.SocketBonus.ToString() == "")
					{
						Item oldItem = ItemCache.FindItemById(id);
						if (oldItem != null)
						{ //If we loaded from the Armory, and got no socket bonus, and we used to 
							//have a socket bonus, just reuse the old one; Armory has been bugged for a while now
							//to not show socket bonuses.
							newItem.SocketBonus = oldItem.SocketBonus.Clone();
						}
					}
					if (useWowhead && newItem.Faction == ItemFaction.Neutral)
					{
						Item oldItem = ItemCache.FindItemById(id);
						if (oldItem != null)
						{ //If we loaded from Wowhead, and got no faction, and we used to 
							//have a faction, just reuse the old one; Wowhead doesn't support 
							//faction-specificity yet
							newItem.Faction = oldItem.Faction;
						}
					}
                    if (cachedItem != null)
                    {
                        // preserve Cost information
                        newItem.Cost = cachedItem.Cost;
                    }
                    #if DEBUG
                    string newItemStats = newItem.ToString().Split(':')[1];
                    string newItemSource = newItem.LocationInfo[0].Description
                        + (newItem.LocationInfo[1] != null ? " and" + newItem.LocationInfo[1].Description.Replace("Purchasable with", "") : "");

                    if (!Rawr.Properties.GeneralSettings.Default.UseMultithreading
                        && (oldItemStats != newItemStats || oldItemSource != newItemSource))
                    {
                        int locationCreated = oldItemSource.IndexOf("Created via");
                        int locationCrafted = newItemSource.IndexOf("Crafted");

                        if (oldItemSource != "Unknown Location, please refresh" &&
                            oldItemSource != "Armory reports no source, try Wowhead" &&
                            ((locationCreated != 0) || (locationCrafted != 0)))
                        {
                            switch (MessageBox.Show(
                                "Do you want to use the New Data and overwrite the Old?"
                                + "\r\n\r\n"
                                + (cachedItem != null ? "[" + cachedItem.Id + "] " + cachedItem.Name
                                : (newItem != null ? "[" + newItem.Id + "] " + newItem.Name
                                : ""))
                                + "\r\n\r\n"
                                + "Old Data:\r\n"
                                + "Stats:\r\n" + oldItemStats
                                + "\r\n"
                                + "Source: " + oldItemSource
                                + "\r\n\r\n"
                                + "New Data:\r\n"
                                + "Stats:\r\n" + newItemStats
                                + "\r\n"
                                + "Source: " + newItemSource
                                , "Update Item Cache",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1))
                            {
                                case DialogResult.Yes: { break; } // Do Nothing, go to AddItem
                                default:
                                    {
                                        // User wants to keep orig data
                                        LocationFactory.Add(cachedItem.Id.ToString(), oldItemLoc, true);
                                        return ItemCache.FindItemById(id);
                                    }
                            }
                        }
                    }
                    #endif
                    ItemCache.AddItem(newItem, raiseEvent);
                }
				return ItemCache.FindItemById(id);
#endif
			}
		}

#if RAWR3
		private static void armoryService_GetItemCompleted(object sender, EventArgs<Item> e)
		{
			if (e.Value != null)
				ItemCache.AddItem(e.Value, true);
			((ElitistArmoryService)sender).GetItemCompleted -= new EventHandler<EventArgs<Item>>(armoryService_GetItemCompleted);
		}
#endif

		/// <summary>
		/// Used by optimizer to cache dictionary search result
		/// </summary>
        [XmlIgnore]
        internal Optimizer.ItemAvailabilityInformation AvailabilityInformation;

		#region IComparable<Item> Members

		public int CompareTo(Item other)
		{
			return ToString().CompareTo(other.ToString());
		}

		#endregion
	}
	#endregion

	#region ItemInstance
    // to make our lives more tolerable, ItemInstance is exactly what it implies
    // it is a single instance of an item, it is not to be shared between multiple characters
    // or whatever, at least if you don't know what you are doing
    // if for whatever reason you reuse the same instance treat it as read only
	
	public class ItemInstance : IComparable<ItemInstance>
	{
		[XmlElement("Id")]
		public int _id;
		[DefaultValueAttribute(0)]
		[XmlElement("Gem1Id")]
		public int _gem1Id;
		[DefaultValueAttribute(0)]
		[XmlElement("Gem2Id")]
		public int _gem2Id;
		[DefaultValueAttribute(0)]
		[XmlElement("Gem3Id")]
		public int _gem3Id;
		[DefaultValueAttribute(0)]
		[XmlElement("EnchantId")]
		public int _enchantId;

		[XmlIgnore]
		public int Id
		{
			get { return _id; }
            set
            {
                _id = value; OnIdsChanged();
            }
		}
		[XmlIgnore]
		public int Gem1Id
		{
			get { return _gem1Id; }
            set
            {
                _gem1Id = value; OnIdsChanged();
            }
		}
		[XmlIgnore]
		public int Gem2Id
		{
			get { return _gem2Id; }
            set
            {
                _gem2Id = value; OnIdsChanged();
            }
		}
		[XmlIgnore]
		public int Gem3Id
		{
			get { return _gem3Id; }
            set
            {
                _gem3Id = value; OnIdsChanged();
            }
		}
		[XmlIgnore]
		public int EnchantId
		{
			get { return _enchantId; }
            set
            {
                _enchantId = value; OnIdsChanged();
            }
		}

        private void UpdateJewelerCount()
        {
            int jewelerCount = 0;
            if (Item.IsJewelersGemId(_gem1Id)) jewelerCount++;
            if (Item.IsJewelersGemId(_gem2Id)) jewelerCount++;
            if (Item.IsJewelersGemId(_gem3Id)) jewelerCount++;
            JewelerCount = jewelerCount;
        }

        [XmlIgnore]
        public int JewelerCount { get; private set; }

        [DefaultValueAttribute(false)]
        public bool ForceDisplay { get; set; }

        public event EventHandler IdsChanged;
        private void OnIdsChanged()
        {
            _gemmedId = string.Empty;
            InvalidateCachedData();
            UpdateJewelerCount();
            if (IdsChanged != null) IdsChanged(this, null);
        }

        [XmlIgnore]
        private Item _itemCached = null;
        [XmlIgnore]
        public Item Item
        {
            get
            {
                if (Id <= 0) return null;
                if (_itemCached == null || _itemCached.Id != Id || _itemCached.Invalid)
                {
                    _itemCached = Item.LoadFromId(Id);
                }
                return _itemCached;
            }
            set
            {
                _itemCached = value;
                if (value == null)
                    Id = 0;
                else
                    Id = value.Id;
            }
        }

		[XmlIgnore]
		private Item _gem1Cached = null;
		[XmlIgnore]
		public Item Gem1
		{
			get
			{
				if (Gem1Id == 0) return null;
				if (_gem1Cached == null || _gem1Cached.Id != Gem1Id || _gem1Cached.Invalid)
				{
					_gem1Cached = Item.LoadFromId(Gem1Id);
				}
				return _gem1Cached;
			}
			set
			{
                _gem1Cached = value;
                if (value == null)
					Gem1Id = 0;
				else
					Gem1Id = value.Id;
            }
		}

		[XmlIgnore]
		private Item _gem2Cached = null;
		[XmlIgnore]
		public Item Gem2
		{
			get
			{
				if (Gem2Id == 0) return null;
				if (_gem2Cached == null || _gem2Cached.Id != Gem2Id || _gem2Cached.Invalid)
				{
					_gem2Cached = Item.LoadFromId(Gem2Id);
				}
				return _gem2Cached;
			}
			set
			{
                _gem2Cached = value;
                if (value == null)
					Gem2Id = 0;
				else
					Gem2Id = value.Id;
            }
		}

		[XmlIgnore]
		private Item _gem3Cached = null;
		[XmlIgnore]
		public Item Gem3
		{
			get
			{
				if (Gem3Id == 0) return null;
				if (_gem3Cached == null || _gem3Cached.Id != Gem3Id || _gem3Cached.Invalid)
				{
					_gem3Cached = Item.LoadFromId(Gem3Id);
				}
				return _gem3Cached;
			}
			set
			{
                _gem3Cached = value;
                if (value == null)
					Gem3Id = 0;
				else
					Gem3Id = value.Id;
            }
		}

		[XmlIgnore]
		private Enchant _enchantCached = null;
		[XmlIgnore]
		public Enchant Enchant
		{
		    get
		    {
		        if (_enchantCached == null || _enchantCached.Id != EnchantId)
		        {
		            _enchantCached = Enchant.FindEnchant(EnchantId, Item != null ? Item.Slot : ItemSlot.None, null);
		        }
		        return _enchantCached;
		    }
		    set
		    {
                _enchantCached = value;
                if (value == null)
		            EnchantId = 0;
		        else
                    EnchantId = value.Id;
            }
		}

		// 1-based index
		public Item GetGem(int index)
		{
			switch (index)
			{
				case 1:
					return Gem1;
				case 2:
					return Gem2;
				case 3:
					return Gem3;
				default:
					return null;
			}
		}

		public void SetGem(int index, Item value)
		{
			switch (index)
			{
				case 1:
					Gem1 = value;
					break;
				case 2:
					Gem2 = value;
					break;
				case 3:
					Gem3 = value;
					break;
			}
		}

        [XmlIgnore]
		private string _gemmedId = string.Empty;
		[XmlIgnore]
		public string GemmedId
		{
			get
			{
				if (_gemmedId.Length == 0) // _gemmedId is never null
				{
					_gemmedId = string.Format("{0}.{1}.{2}.{3}.{4}", Id, Gem1Id, Gem2Id, Gem3Id, EnchantId);
				}
				return _gemmedId;
			}
            set
            {
                if (value == null) _gemmedId = string.Empty;
                else _gemmedId = value;
                string[] ids = _gemmedId.Split('.');
                _id = int.Parse(ids[0]);
                _gem1Id = ids.Length > 1 ? int.Parse(ids[1]) : 0;
                _gem2Id = ids.Length > 2 ? int.Parse(ids[2]) : 0;
                _gem3Id = ids.Length > 3 ? int.Parse(ids[3]) : 0;
                _enchantId = ids.Length > 4 ? int.Parse(ids[4]) : 0;
                OnIdsChanged();
            }
		}
		
		public ItemInstance() { }
		public ItemInstance(string gemmedId)
		{
			string[] ids = gemmedId.Split('.');
			_id = int.Parse(ids[0]);
			_gem1Id = ids.Length > 1 ? int.Parse(ids[1]) : 0;
			_gem2Id = ids.Length > 2 ? int.Parse(ids[2]) : 0;
			_gem3Id = ids.Length > 3 ? int.Parse(ids[3]) : 0;
			_enchantId = ids.Length > 4 ? int.Parse(ids[4]) : 0;
            UpdateJewelerCount();
		}
		public ItemInstance(int id, int gem1Id, int gem2Id, int gem3Id, int enchantId)
		{
			_id = id;
			_gem1Id = gem1Id;
			_gem2Id = gem2Id;
			_gem3Id = gem3Id;
			_enchantId = enchantId;
            UpdateJewelerCount();
        }
        public ItemInstance(Item item, Item gem1, Item gem2, Item gem3, Enchant enchant)
        {
            // this code path is used a lot, optimize for performance
            _itemCached = item;
            _gem1Cached = gem1;
            _gem2Cached = gem2;
            _gem3Cached = gem3;
            _enchantCached = enchant;
            _id = item != null ? item.Id : 0;
            _gem1Id = gem1 != null ? gem1.Id : 0;
            _gem2Id = gem2 != null ? gem2.Id : 0;
            _gem3Id = gem3 != null ? gem3.Id : 0;
            _enchantId = enchant != null ? enchant.Id : 0;
            OnIdsChanged();
        }

		public ItemInstance Clone()
		{
			return new ItemInstance()
			{
				Item = this.Item,
				Gem1 = this.Gem1,
				Gem2 = this.Gem2,
				Gem3 = this.Gem3,
				Enchant = this.Enchant
			};
		}

		public override string ToString()
		{
			string summary = this.Item.Name + ": ";
			summary += this.GetTotalStats().ToString();
			//summary += Stats.ToString();
			//summary += Sockets.ToString();
			if (summary.EndsWith(", ")) summary = summary.Substring(0, summary.Length - 2);

			if ((Item.SocketColor1 != ItemSlot.None && Gem1Id == 0) ||
				(Item.SocketColor2 != ItemSlot.None && Gem2Id == 0) ||
				(Item.SocketColor3 != ItemSlot.None && Gem3Id == 0))
				summary += " [EMPTY SOCKETS]";

			return summary;
		}

        public bool MatchesSocketBonus
        {
            get
            {
                Item item = Item;
                return Item.GemMatchesSlot(Gem1, item.SocketColor1) &&
                       Item.GemMatchesSlot(Gem2, item.SocketColor2) &&
                       Item.GemMatchesSlot(Gem3, item.SocketColor3);
            }
        }

        public int DisplayId
        {
            get
            {
                return Item == null ? 0 : Item.DisplayId;
            }
        }

        public int DisplaySlot
        {
            get
            {
                return Item == null ? 0 : Item.DisplaySlot;
            }
        }

        // caching policy: cache total stats only for items that don't have global requirements
		// value should not change if it relies on data other than from this item
		// assume there is no stat editing happening in code other than in item editor
		// invalidate on id changes, invalidate when item is opened for editing
		// invalidate all items when any gem is opened for editing
		// invalidate 
		private Stats cachedTotalStats = null;
        private DateTime cachedTime; 
		public void InvalidateCachedData()
		{
			cachedTotalStats = null;
		}

        public Stats GetTotalStats() { return AccumulateTotalStats(null, null); }
        public Stats GetTotalStats(Character character) { return AccumulateTotalStats(character, null); }
#if SILVERLIGHT
        public Stats AccumulateTotalStats(Character character, Stats unsafeStatsAccumulator)
#else
        public unsafe Stats AccumulateTotalStats(Character character, Stats unsafeStatsAccumulator)
#endif
		{
            Item item = Item;
            if ((object)cachedTotalStats != null && item.LastChange <= cachedTime)
            {
                if ((object)unsafeStatsAccumulator != null)
                {
                    unsafeStatsAccumulator.AccumulateUnsafe(cachedTotalStats);
                }
                return cachedTotalStats;
            }
            Item g1 = Gem1;
            Item g2 = Gem2;
            Item g3 = Gem3;
            Enchant enchant = Enchant;
            bool volatileGem = false, volatileItem = false;
            bool gem1 = false;
            bool gem2 = false;
            bool gem3 = false;
            bool eligibleForSocketBonus = Item.GemMatchesSlot(g1, item.SocketColor1) &&
                                            Item.GemMatchesSlot(g2, item.SocketColor2) &&
                                            Item.GemMatchesSlot(g3, item.SocketColor3);
            if (g1 != null && g1.MeetsRequirements(character, out volatileGem)) gem1 = true;
            volatileItem = volatileItem || volatileGem;
            if (g2 != null && g2.MeetsRequirements(character, out volatileGem)) gem2 = true;
            volatileItem = volatileItem || volatileGem;
            if (g3 != null && g3.MeetsRequirements(character, out volatileGem)) gem3 = true;
            volatileItem = volatileItem || volatileGem;
            if (volatileItem && unsafeStatsAccumulator != null)
            {
                unsafeStatsAccumulator.AccumulateUnsafe(item.Stats, true);
                if (gem1) unsafeStatsAccumulator.AccumulateUnsafe(g1.Stats, true);
                if (gem2) unsafeStatsAccumulator.AccumulateUnsafe(g2.Stats, true);
                if (gem3) unsafeStatsAccumulator.AccumulateUnsafe(g3.Stats, true);
                if (eligibleForSocketBonus) unsafeStatsAccumulator.AccumulateUnsafe(item.SocketBonus, true);
                bool eligibleForEnchant = false;
                if (enchant.Slot == ItemSlot.OneHand)
                {
                    eligibleForEnchant = (this.Slot == ItemSlot.OneHand ||
                                        (this.Slot == ItemSlot.OffHand &&
                                            this.Type != ItemType.Shield &&
                                            this.Type != ItemType.None) ||
                                        this.Slot == ItemSlot.MainHand ||
                                        this.Slot == ItemSlot.TwoHand);
                }
                else if (enchant.Slot == ItemSlot.OffHand)
                {
                    eligibleForEnchant = this.Type == ItemType.Shield;
                }
                else
                {
                    eligibleForEnchant = (enchant.Slot == this.Slot);
                }
                if (eligibleForEnchant) unsafeStatsAccumulator.AccumulateUnsafe(enchant.Stats, true);
                return null;
            }
            else
            {
                Stats totalItemStats = new Stats();
#if !SILVERLIGHT
                fixed (float* pRawAdditiveData = totalItemStats._rawAdditiveData, pRawMultiplicativeData = totalItemStats._rawMultiplicativeData, pRawNoStackData = totalItemStats._rawNoStackData)
                {
                    totalItemStats.BeginUnsafe(pRawAdditiveData, pRawMultiplicativeData, pRawNoStackData);
#endif
                    totalItemStats.AccumulateUnsafe(item.Stats, true);
                    if (gem1) totalItemStats.AccumulateUnsafe(g1.Stats, true);
                    if (gem2) totalItemStats.AccumulateUnsafe(g2.Stats, true);
                    if (gem3) totalItemStats.AccumulateUnsafe(g3.Stats, true);
                    if (eligibleForSocketBonus) totalItemStats.AccumulateUnsafe(item.SocketBonus, true);
                    bool eligibleForEnchant = false;
                    if (enchant.Slot == ItemSlot.OneHand)
                    {
                        eligibleForEnchant = (this.Slot == ItemSlot.OneHand ||
                                            (this.Slot == ItemSlot.OffHand &&
                                                this.Type != ItemType.Shield &&
                                                this.Type != ItemType.None) ||
                                            this.Slot == ItemSlot.MainHand ||
                                            this.Slot == ItemSlot.TwoHand);
                    }
                    else if (enchant.Slot == ItemSlot.OffHand)
                    {
                        eligibleForEnchant = this.Type == ItemType.Shield;
                    }
                    else
                    {
                        eligibleForEnchant = (enchant.Slot == this.Slot);
                    }
                    if (eligibleForEnchant) totalItemStats.AccumulateUnsafe(enchant.Stats, true);
                    if (!volatileItem)
                    {
                        cachedTime = DateTime.Now;
                        cachedTotalStats = totalItemStats;
                        cachedTotalStats.GenerateSparseData();
                        if (unsafeStatsAccumulator != null)
                        {
                            unsafeStatsAccumulator.AccumulateUnsafe(cachedTotalStats);
                        }
                    }
#if !SILVERLIGHT

                    totalItemStats.EndUnsafe();
                }
#endif
                return totalItemStats;
            }
		}


		public static ItemInstance LoadFromId(string gemmedId)
		{
			if (string.IsNullOrEmpty(gemmedId))
				return null;
			return new ItemInstance(gemmedId);
		}

		#region IComparable<Item> Members

		public int CompareTo(ItemInstance other)
		{
			return GemmedId.CompareTo(other.GemmedId);
		}

		#endregion

        public bool FitsInSlot(CharacterSlot characterSlot)
        {
            return Item.FitsInSlot(characterSlot);
        }

        // helper functions to minimize fixing of models
        [XmlIgnore]
        public ItemSlot Slot
        {
            get
            {
                if (Item == null) return ItemSlot.None;
                return Item.Slot;
            }
        }

        [XmlIgnore]
        public ItemType Type
        {
            get
            {
                if (Item == null) return ItemType.None;
                return Item.Type;
            }
        }

        [XmlIgnore]
        public int MinDamage
        {
            get
            {
                if (Item == null) return 0;
                return Item.MinDamage;
            }
        }

        [XmlIgnore]
        public int MaxDamage
        {
            get
            {
                if (Item == null) return 0;
                return Item.MaxDamage;
            }
        }

        [XmlIgnore]
        public ItemDamageType DamageType
        {
            get
            {
                if (Item == null) return ItemDamageType.Physical;
                return Item.DamageType;
            }
        }

        [XmlIgnore]
        public float Speed
        {
            get
            {
                if (Item == null) return 0;
                return Item.Speed;
            }
        }

        public static bool operator ==(ItemInstance a, ItemInstance b)
        {
            if ((object)b == null || (object)a == null) return (object)a == (object)b;
            return a.GemmedId == b.GemmedId;
        }

        public static bool operator !=(ItemInstance a, ItemInstance b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            ItemInstance o = obj as ItemInstance;
            if ((object)o != null)
            {
                return GemmedId == o.GemmedId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GemmedId.GetHashCode();
        }
    }

	#endregion

    [GenerateSerializer]
	public class ItemList : List<Item>
    {
        public ItemList() : base() { }
        public ItemList(IEnumerable<Item> collection) : base(collection) { }
    }
}
