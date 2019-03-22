using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using System.Collections.Generic;

namespace Rawr
{
    public class ItemToolTip : ToolTip
    {
        private static ItemToolTip _instance = null;
        public static ItemToolTip Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ItemToolTip();
                }
                return _instance;
            }
        }

		private Character _character = null;
		public Character Character
		{
			get { return _character; }
			set
			{
                if (_character != value)
                {
                    if (_character != null)
                    {
                        _character.CalculationsInvalidated -= new EventHandler(CharacterItemCache_ItemsChanged);
                    }
                    _character = value;
                    if (_character != null)
                    {
                        _character.CalculationsInvalidated += new EventHandler(CharacterItemCache_ItemsChanged);
                    }
                }
			}
		}

        private bool resetItem = false;
        public ItemToolTip()
        {
            AutomaticDelay = 0;
            AutoPopDelay = 0;
            Draw += new DrawToolTipEventHandler(ItemToolTip_Draw);
            InitialDelay = 0;
            OwnerDraw = true;
            Popup += new PopupEventHandler(ItemToolTip_Popup);
            ReshowDelay = 0;
            UseAnimation = true;
            UseFading = true;
            LoadGraphicsObjects();

			ItemCache.Instance.ItemsChanged += new EventHandler(CharacterItemCache_ItemsChanged);
        }

        private void CharacterItemCache_ItemsChanged(object sender, EventArgs e)
        {
            //with the way this event can be fired, it can set the current item null while the item image is being generated and cause errors
            resetItem = true;
        }

        private ItemInstance _currentItemInstance = null;
        private ItemInstance CurrentItemInstance
        {
            get { return _currentItemInstance; }
            set
            {
                if (resetItem)
                {
                    _currentItemInstance = null;
                    resetItem = false;
                }
                if (_currentItemInstance != value)
                {
                    _currentItemInstance = value;
                    _currentItem = _currentItemInstance.Item;
                    if (_cachedToolTipImage != null)
                    {
                        _cachedToolTipImage.Dispose();
                        _cachedToolTipImage = null;
                    }
                }
            }
		}

        private string _currentString = null;
        private string CurrentString
        {
            get { return _currentString; }
            set
            {
                if (resetItem)
                {
                    _currentString = null;
                    resetItem = false;
                }
                if (_currentString != value)
                {
                    _currentString = value;
                    if (_cachedToolTipImage != null)
                    {
                        _cachedToolTipImage.Dispose();
                        _cachedToolTipImage = null;
                    }
                }
            }
        }

        private CharacterSlot _currentSlot = CharacterSlot.None;
		private CharacterSlot CurrentSlot
		{
			get { return _currentSlot; }
			set
			{
				if (resetItem)
				{
					_currentSlot = CharacterSlot.None;
					resetItem = false;
				}
				if (_currentSlot != value)
				{
					_currentSlot = value;
					if (_cachedToolTipImage != null)
					{
						_cachedToolTipImage.Dispose();
						_cachedToolTipImage = null;
					}
				}
			}
		}

        private Item _currentItem = null;
        private Item CurrentItem
        {
            get { return _currentItem; }
            set
            {
                if (resetItem)
                {
                    _currentItem = null;
                    resetItem = false;
                }
                if (_currentItem != value)
                {
                    _currentItem = value;
                    _currentItemInstance = null;
                    if (_cachedToolTipImage != null)
                    {
                        _cachedToolTipImage.Dispose();
                        _cachedToolTipImage = null;
                    }
                }
            }
        }

        private ItemInstance[] CurrentCharacterItems { get; set; }

        private Font _fontName = null;
        private Font _fontStats = null;
        private Font _fontStatsSmall = null;
        private Font _fontTinyName = null;

        private void LoadGraphicsObjects()
        {
            Debug.Assert(_fontName == null);
            Debug.Assert(_fontTinyName == null);
            Debug.Assert(_fontStats == null);
            Debug.Assert(_fontStatsSmall == null);

            _fontName = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((0)));
            _fontTinyName = new Font("Microsoft Sans Serif", 6.00F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
			_fontStats = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
			_fontStatsSmall = new Font("Microsoft Sans Serif", 7.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
		}

        protected Image _cachedToolTipImage = null;

        private static Graphics _dummyBitmap = Graphics.FromImage(new Bitmap(1, 1));

        public static string Bind2Text( BindsOn bind )
        {
            // TODO: add localization here? or we better stick with invariant EN, so we'll use this function from XML parsing too
            if (bind == BindsOn.BoA) return "Binds to account";
            if (bind == BindsOn.BoE) return "Binds when equipped";
            if (bind == BindsOn.BoP) return "Binds when picked up";
            if (bind == BindsOn.BoU) return "Binds when used";
            if (bind == BindsOn.None) return "Doesn't Bind or No Bind Data";

            return string.Empty;
        }

        protected Image CachedToolTipImage
        {
            get
            {
                // ===== For item tooltips =====
                if (_cachedToolTipImage == null && _currentItem != null)
                {
                    lock (_currentItem)
                    {
                        if (_currentItem != null)
                        {
                            #region Figure out the size of the tooltip needed
                            bool hasSockets = _currentItem.SocketColor1 != ItemSlot.None ||
                                              _currentItem.SocketColor2 != ItemSlot.None ||
                                              _currentItem.SocketColor3 != ItemSlot.None;
                            if (_currentItemInstance != null && (_currentItemInstance.Gem1 != null ||
                                                                 _currentItemInstance.Gem2 != null ||
                                                                 _currentItemInstance.Gem3 != null))
                            {
                                hasSockets = true;
                            }

                            Stats relevantStats = Calculations.GetRelevantStats(_currentItem.Stats);
                            var positiveStats = relevantStats.Values(x => x != 0);

                            #region Calculate statHeight
                            var xGrid = new
                            {
                                initial = 2,
                                step = 103,
                                max = 218,
                                end = 307
                            };

                            var yGrid = new
                            {
                                initial = 21,
                                step = 17
                            };

                            int xPos = xGrid.initial;
                            int yPos = yGrid.initial;

                            #region Item ID's Section

                            // 8.jan.2010: all ID's (including binding info) goes separate line, as stats
                            var typesText = string.Empty;

                            if (Properties.GeneralSettings.Default.DisplayExtraItemInfo) {
                                                                                    typesText += string.Format("[{0}] ", CurrentItem.Id);
                                if (CurrentItem.ItemLevel > 0) {                    typesText += string.Format("[{0}] ", CurrentItem.ItemLevel); }
                                if (CurrentItem.Bind != BindsOn.None) {             typesText += string.Format("[{0}] ", CurrentItem.Bind); }
                                if (!string.IsNullOrEmpty(CurrentItem.SlotString)) {typesText += string.Format("[{0}] ", CurrentItem.SlotString); }
                                if (CurrentItem.Type != ItemType.None) {            typesText += string.Format("[{0}] ", CurrentItem.Type); }
                            }else{
                                // Just do the iLevel so we can differentiate the Heroic vs non-Heroic versions of items
                                if (CurrentItem.ItemLevel > 0) { typesText += string.Format(" [{0}]", CurrentItem.ItemLevel); }
                            }

                            // check if we added some and join the text for drawing
                            if (Properties.GeneralSettings.Default.DisplayExtraItemInfo && !string.IsNullOrEmpty(typesText)) { yPos += yGrid.step; }

                            #endregion // Item ID's Section

                            List<string> statsList = new List<string>();

                            foreach (System.Reflection.PropertyInfo info in positiveStats.Keys)
                            {
                                float value = positiveStats[info];
                                if (Stats.IsPercentage(info)) value *= 100;
                                value = (float)Math.Round(value * 100f) / 100f;
                                string text = string.Format("{0}{1}", value, Extensions.DisplayName(info));
                                statsList.Add(text.Replace("Armor Penetration Rating","ArP Rating"));
                                float width = _dummyBitmap.MeasureString(text, _fontStats).Width;
                                if (xPos + width > xGrid.end && xPos != xGrid.initial)
                                {
                                    xPos = xGrid.initial;
                                    yPos += yGrid.step;
                                }
                                // Force stats that are in the 1st column that would STILL fall off to Text Wrap
                                if (xPos + width > xGrid.end && xPos == xGrid.initial)
                                {
                                    xPos = xGrid.initial;
                                    yPos += yGrid.step;
                                }
                                // once we write on a line store where the next line would be
                                // (alternatively we could put the yPos update code below this at the top of the loop)
                                int steps = (int)Math.Ceiling(width / xGrid.step);
                                xPos += steps * xGrid.step;
                                if (xPos > xGrid.max)
                                {
                                    xPos = xGrid.initial;
                                    yPos += yGrid.step;
                                }
                            }

                            #region Weapon DPS
                            if (_currentItem.DPS > 0)
                            {
                                float dps = (float)Math.Round(_currentItem.DPS * 100f) / 100f;
                                string text = dps + " DPS";
                                float width = _dummyBitmap.MeasureString(text, _fontStats).Width;
                                if (xPos + width > xGrid.end)
                                {
                                    xPos = xGrid.initial;
                                    yPos += yGrid.step;
                                }
                                statsList.Add(text);
                                xPos += (int)Math.Ceiling(width / xGrid.step) * xGrid.step;
                                if (xPos > xGrid.max)
                                {
                                    xPos = xGrid.initial;
                                    yPos += yGrid.step;
                                }

                                text = _currentItem.Speed + " Speed";
                                width = _dummyBitmap.MeasureString(_currentItem.Speed + " Speed", _fontStats).Width;
                                if (xPos + width > xGrid.end)
                                {
                                    xPos = xGrid.initial;
                                    yPos += yGrid.step;
                                }
                                statsList.Add(text);
                                xPos += (int)Math.Ceiling(width / xGrid.step) * xGrid.step;
                                if (xPos > xGrid.max)
                                {
                                    xPos = xGrid.initial;
                                    yPos += yGrid.step;
                                }
                            }
                            #endregion

                            foreach (SpecialEffect effect in relevantStats.SpecialEffects())
                            {
                                string text = effect.ToString();
                                statsList.Add(text);
                                float width = _dummyBitmap.MeasureString(text, _fontStats).Width;
                                // Force stats that are in a 2nd or 3rd column to a new line if they'd fall off the tooltip
                                if (xPos + width > xGrid.end && xPos != xGrid.initial)
                                {
                                    xPos = xGrid.initial;
                                    yPos += yGrid.step;
                                }
                                // Force stats that are in the 1st column that would STILL fall off to Text Wrap
                                if (xPos + width > xGrid.end && xPos == xGrid.initial)
                                {
                                    yPos += yGrid.step;
                                }
                                // once we write on a line store where the next line would be
                                // (alternatively we could put the yPos update code below this at the top of the loop)
                                int steps = (int)Math.Ceiling(width / xGrid.step);
                                xPos += steps * xGrid.step;
                                if (xPos > xGrid.max)
                                {
                                    xPos = xGrid.initial;
                                    yPos += yGrid.step;
                                }
                            }
                            #endregion

                            int statHeight = (xPos > xGrid.initial) ? yPos : Math.Max(0, yPos - yGrid.step);

                            #region Location Section
                            int extraLocation = 0;
                            string location = "Unknown source";
                            if (_currentItem.Quality != ItemQuality.Temp) {
                                if (_currentItem.LocationInfo != null) {
                                    location =  _currentItem.LocationInfo[0].Description;
                                    /*if (_currentItem.LocationInfo[1] != null) {
                                        location += " and" + _currentItem.LocationInfo[1].Description.Replace("Purchasable with", "");
                                        //extraLocation += 13;
                                    }*/
                                }
                                SizeF locationSize = _dummyBitmap.MeasureString(location, _fontStats);
                                if (locationSize.Width > 300) {
                                    extraLocation += (int)locationSize.Height;
                                    int index = (location.Length > 20) ? location.IndexOf(" in ", 20) : -1;
                                    index = (index == -1 ? location.IndexOf(" and ") : index);
                                    index = (index == -1 ? -1 : index + 4);
                                    if (index != -1) { location = location.Insert(index, "\r\n    "); }
                                }
                            }
                            #endregion

                            #region Enchant Section
                            int extraEnchant = 0;
                            string enchant = "No Enchant";
                            if (_currentItem.Quality != ItemQuality.Temp && !CurrentItem.IsGem)
                            {
                                if (CurrentItemInstance != null && CurrentItemInstance.Enchant != null)
                                {
                                    enchant = CurrentItemInstance.Enchant.ToString();
                                }
                                SizeF enchantsize = _dummyBitmap.MeasureString(enchant, _fontStats);
                                if (enchantsize.Width > 300)
                                {
                                    extraEnchant = (int)enchantsize.Height;
                                    int index = enchant.IndexOf(": ");
                                    index = (index == -1 ? -1 : index + 2);
                                    if (index != -1) { enchant = enchant.Insert(index, "\r\n    "); }
                                }
                            }
                            #endregion

                            #region Gem Section
                            int gemInfoSize = 0;
                            int gemNamesSize = 0;
                            if (_currentItem.Quality != ItemQuality.Temp) {
                                if (CurrentItem.IsGem) {
                                    SizeF gem_info_size = _dummyBitmap.MeasureString("Matches:", _fontStats);
                                    gemInfoSize = (int)gem_info_size.Height;
                                }else if (Rawr.Properties.GeneralSettings.Default.DisplayGemNames && !CurrentItem.IsGem && hasSockets) {
                                    SizeF gem_name_size = _dummyBitmap.MeasureString("Name of Gem", _fontStats);
                                    for (int i = 0; i < 3; i++) {
                                        Item gem = null;
                                        if (CurrentItemInstance != null) gem = (i == 0 ? CurrentItemInstance.Gem1 : (i == 1 ? CurrentItemInstance.Gem2 : CurrentItemInstance.Gem3));
                                        if (gem != null) { gemNamesSize += (int)gem_name_size.Height; }
                                    }
                                }
                            }
                            #endregion

                            #region Extra Items Section (like in Build Upgrade List)
                            int numTinyItems = 0;
                            int tinyItemSize = 18;
                            if (CurrentCharacterItems != null)
                            {
                                for (int slot = 0; slot < Character.SlotCount; slot++)
                                {
                                    if (CurrentCharacterItems[slot] != null)
                                    {
                                        if (_currentItemInstance == null || CurrentCharacterItems[slot].GemmedId != _currentItemInstance.GemmedId)
                                            numTinyItems++;
                                    }
                                }
                            }
                            #endregion

                            #endregion

                            _cachedToolTipImage = new Bitmap(309,
                                (hasSockets ? 78 : 20) + statHeight                                   // Stats
                                //+ (!string.IsNullOrEmpty(typesText) ? 13 : 0)                         // ID's info
                                + (_currentItem.Quality != ItemQuality.Temp ? 13 + extraLocation : 0) // Location
                                + (_currentItem.Quality != ItemQuality.Temp && !CurrentItem.IsGem ? 13 + extraEnchant  : 0) // Enchant
                                + gemInfoSize + gemNamesSize                                          // Gems
                                + numTinyItems * tinyItemSize,                                        // Tiny Items, like in Build Upgrade list
                                PixelFormat.Format32bppArgb);

                            #region Setup for Background of the tooltip
                            Graphics g = Graphics.FromImage(_cachedToolTipImage);
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.TextRenderingHint = TextRenderingHint.AntiAlias;

                            Rectangle rectBorder =
                                new Rectangle(0, 0, _cachedToolTipImage.Width - 1, _cachedToolTipImage.Height - 1);
                            LinearGradientBrush lgBrush = new LinearGradientBrush(rectBorder,
                                                                    Color.FromArgb(212, 212, 255), Color.FromArgb(192, 192, 255),
                                                                    90);
                            g.FillRectangle(lgBrush, rectBorder);
                            lgBrush.Dispose();
                            lgBrush = null;

                            Pen pen = new Pen(Color.FromArgb(128, 0, 0, 0));
                            g.DrawRectangle(pen, rectBorder);
                            pen.Dispose();
                            pen = null;
                            #endregion

                            #region Item Name and Optionalized Add-Ons (iLevel, etc)
                            { // hide the name brush scope to reduce bugs
                                Brush nameBrush = null;
                                switch (_currentItem.Quality)
                                {
                                    case ItemQuality.Common:
                                    case ItemQuality.Temp:
                                        nameBrush = SystemBrushes.InfoText;
                                        break;
                                    case ItemQuality.Epic:
                                        nameBrush = Brushes.Purple;
                                        break;
                                    case ItemQuality.Legendary:
                                        nameBrush = Brushes.Orange;
                                        break;
                                    case ItemQuality.Heirloom:
                                    case ItemQuality.Artifact:
                                        nameBrush = Brushes.Gold;
                                        break;
                                    case ItemQuality.Poor:
                                        nameBrush = Brushes.Gray;
                                        break;
                                    case ItemQuality.Rare:
                                        nameBrush = Brushes.Blue;
                                        break;
                                    case ItemQuality.Uncommon:
                                        nameBrush = Brushes.Green;
                                        break;
                                }
                                g.DrawString(CurrentItem.Name, _fontName, nameBrush, 2, 4);
                                if (!Properties.GeneralSettings.Default.DisplayExtraItemInfo && !string.IsNullOrEmpty(typesText)) {
                                    float nameWidth = _dummyBitmap.MeasureString(CurrentItem.Name, _fontStats).Width;
                                    g.DrawString(typesText, _fontName, SystemBrushes.GrayText, 4 + nameWidth, 4);
                                }
                            }

                            #endregion

                            xPos = xGrid.initial;
                            yPos = yGrid.initial;

                            // draw extra item info (if option is checked)
                            if (Properties.GeneralSettings.Default.DisplayExtraItemInfo && !string.IsNullOrEmpty(typesText)) {
                                g.DrawString(typesText, _fontName, SystemBrushes.GrayText, xPos, yPos);
                                yPos += yGrid.step;
                            }

                            #region Stats Section
                            foreach (string statText in statsList)
                            {
                                string text = statText;
                                float width = g.MeasureString(statText, _fontStats).Width;
                                // Force stats that are in a 2nd or 3rd column to a new line if they'd fall off the tooltip
                                if (xPos + width > xGrid.end && xPos != xGrid.initial)
                                {
                                    xPos = xGrid.initial;
                                    yPos += yGrid.step;
                                }
                                // Force stats that are in the 1st column that would STILL fall off to Text Wrap
                                if (xPos + width > xGrid.end && xPos == xGrid.initial) 
                                {
                                    int index = (text.Length > 47) ? (text.IndexOf(" ", 47) + 1) : text.Length;
                                    text = text.Insert(index, "\r\n   "); 
                                }
                                g.DrawString(text, _fontStats, SystemBrushes.InfoText, xPos, yPos);
                                if (xPos + width > xGrid.end && xPos == xGrid.initial) { yPos += yGrid.step; }
                                // once we write on a line store where the next line would be
                                // (alternatively we could put the yPos update code below this at the top of the loop)
                                xPos += (int)Math.Ceiling(width / xGrid.step) * xGrid.step;
                                if (xPos > xGrid.max)
                                {
                                    xPos = xGrid.initial;
                                    yPos += yGrid.step;
                                }
                            }
                            #endregion

                            // this is the next clean/empty line after we wrote the stats
                            if (xPos > xGrid.initial) { yPos += yGrid.step; }

                            #region Sockets Section
                            if (hasSockets)
                            {
                                int gemNameHeight = 0, gemUsedSlot = 0;

                                for (int i = 0; i < 3; i++)
                                {
                                    ItemSlot slotColor = (i == 0
                                                                   ? _currentItem.SocketColor1
                                                                   :
                                                               (i == 1 ? _currentItem.SocketColor2 : _currentItem.SocketColor3));
                                    if (slotColor == ItemSlot.None && _currentItemInstance != null && _currentItemInstance.GetGem(i + 1) != null)
                                    {
                                        slotColor = ItemSlot.Prismatic;
                                    }
                                    if (slotColor != ItemSlot.None)
                                    {
                                        Rectangle rectGemBorder = new Rectangle(3 + (103 * (gemUsedSlot)), 20 + statHeight, 35, 35);

                                        // seek to next one
                                        gemUsedSlot++;

                                        Brush brushGemBorder = Brushes.Silver;
                                        switch (slotColor) {
                                            case ItemSlot.Red:      brushGemBorder = Brushes.Red;    break;
                                            case ItemSlot.Yellow:   brushGemBorder = Brushes.Yellow; break;
                                            case ItemSlot.Blue:     brushGemBorder = Brushes.Blue;   break;
                                            case ItemSlot.Prismatic:brushGemBorder = Brushes.Silver; break;
                                        }
                                        g.FillRectangle(brushGemBorder, rectGemBorder);
                                        brushGemBorder = null;

                                        Item gem = null;
                                        if (_currentItemInstance != null) gem = (i == 0 ? _currentItemInstance.Gem1 : (i == 1 ? _currentItemInstance.Gem2 : _currentItemInstance.Gem3));
                                        if (gem != null)
                                        {
                                            Image icon = ItemIcons.GetItemIcon(gem, true);
                                            if (icon != null)
                                            {
                                                g.DrawImageUnscaled(icon, rectGemBorder.X + 2, rectGemBorder.Y + 2);
                                                //icon.Dispose(); // these are cached images, I believe they should not be disposed
                                                icon = null;
                                            }

                                            bool active = true;
                                            if (Character != null)
                                            {
                                                if (Character.IsEquipped(CurrentItemInstance))
                                                {
                                                    active = gem.MeetsRequirements(Character);
                                                }
                                                else
                                                {
                                                    CharacterSlot slotToEquip = CurrentSlot;
                                                    if (slotToEquip == CharacterSlot.None)
                                                        slotToEquip = Character.GetCharacterSlotByItemSlot(CurrentItemInstance.Slot);
                                                    Character characterWithItemEquipped = Character.Clone();
                                                    characterWithItemEquipped[slotToEquip] = CurrentItemInstance;
                                                    active = gem.MeetsRequirements(characterWithItemEquipped);
                                                }
                                            }

                                            string[] stats = gem.Stats.ToString().Split(',');

                                            // Fixes a text too big issue
                                            if (stats.Length > 0) {
                                                for (int l = 0; l < stats.Length; l++) {
                                                    if (stats[l].Contains("Armor Penetration")) {
                                                        stats[l] = stats[l].Replace("Armor Penetration", "ArP");
                                                    }
                                                }
                                            }

                                            switch (stats.Length)
                                            {
                                                case 1:
                                                    g.DrawString(stats[0].Trim(), _fontStats, active ? SystemBrushes.InfoText : SystemBrushes.GrayText,
                                                        rectGemBorder.X + 39, rectGemBorder.Y + 3);
                                                    break;

                                                case 2:
                                                    g.DrawString(stats[0].Trim(), _fontStats, active ? SystemBrushes.InfoText : SystemBrushes.GrayText,
                                                        rectGemBorder.X + 39, rectGemBorder.Y + 3);
                                                    g.DrawString(stats[1].Trim(), _fontStats, active ? SystemBrushes.InfoText : SystemBrushes.GrayText,
                                                        rectGemBorder.X + 39, rectGemBorder.Y + 20);
                                                    break;

                                                case 3:
                                                    g.DrawString(stats[0].Trim(), _fontStatsSmall, active ? SystemBrushes.InfoText : SystemBrushes.GrayText,
                                                        rectGemBorder.X + 39, rectGemBorder.Y + 0);
                                                    g.DrawString(stats[1].Trim(), _fontStatsSmall, active ? SystemBrushes.InfoText : SystemBrushes.GrayText,
                                                        rectGemBorder.X + 39, rectGemBorder.Y + 12);
                                                    g.DrawString(stats[2].Trim(), _fontStatsSmall, active ? SystemBrushes.InfoText : SystemBrushes.GrayText,
                                                        rectGemBorder.X + 39, rectGemBorder.Y + 24);
                                                    break;
                                                case 4:
                                                case 5:
                                                    g.DrawString(stats[0].Trim(), _fontStatsSmall, active ? SystemBrushes.InfoText : SystemBrushes.GrayText,
                                                        rectGemBorder.X + 39, rectGemBorder.Y + 0);
                                                    g.DrawString(stats[1].Trim(), _fontStatsSmall, active ? SystemBrushes.InfoText : SystemBrushes.GrayText,
                                                        rectGemBorder.X + 39, rectGemBorder.Y + 12);
                                                    g.DrawString(stats[2].Trim() + "...", _fontStatsSmall, active ? SystemBrushes.InfoText : SystemBrushes.GrayText,
                                                        rectGemBorder.X + 39, rectGemBorder.Y + 24);
                                                    break;
                                            }

                                            if (!active)
                                            {
                                                SolidBrush solidBrush = new SolidBrush(Color.FromArgb(128, Color.Silver));
                                                g.FillRectangle(solidBrush, rectGemBorder);
                                                solidBrush.Dispose();
                                            }
                                            if (Rawr.Properties.GeneralSettings.Default.DisplayGemNames && !CurrentItem.IsGem)
                                            {
                                                SizeF gemNameSize = g.MeasureString("- " + gem.Name, _fontStats);
                                                g.DrawString("- " + gem.Name, _fontStats, SystemBrushes.InfoText, 2, 63 + statHeight + gemNameHeight);
                                                gemNameHeight += (int)gemNameSize.Height;
                                            }
                                        }
                                    }
                                }

                                Brush brushBonus = SystemBrushes.InfoText;
                                if (_currentItemInstance == null || !Item.GemMatchesSlot(_currentItemInstance.Gem1, _currentItem.SocketColor1) ||
                                    !Item.GemMatchesSlot(_currentItemInstance.Gem2, _currentItem.SocketColor2) ||
                                    !Item.GemMatchesSlot(_currentItemInstance.Gem3, _currentItem.SocketColor3))
                                    brushBonus = SystemBrushes.GrayText;

                                g.DrawString(
                                    "Socket Bonus: " +
                                    (CurrentItem.SocketBonus.ToString().Length == 0
                                         ? "None"
                                         : CurrentItem.SocketBonus.ToString()),
                                    _fontStats, brushBonus, 2, 63 + statHeight + gemNameHeight);

                                // update cursor position (the magic number 63 comes from right above)
                                SizeF socket_info_size = g.MeasureString("Socket Bonus:", _fontStats);
                                yPos = 63 + statHeight + gemNameHeight + ((int)socket_info_size.Height);
                            }
                            // draw information about gem color (for dummies like me)
                            if (CurrentItem.IsGem)
                            {
                                // create a string that gives us some information about the gem
                                string colors = "Matches: ";
                                if (Item.GemMatchesSlot(CurrentItem, ItemSlot.Meta))
                                    colors += "Meta ";
                                if (Item.GemMatchesSlot(CurrentItem, ItemSlot.Red))
                                    colors += "Red ";
                                if (Item.GemMatchesSlot(CurrentItem, ItemSlot.Yellow))
                                    colors += "Yellow ";
                                if (Item.GemMatchesSlot(CurrentItem, ItemSlot.Blue))
                                    colors += "Blue ";
                                // put the info on the tooltip
                                Brush gemBrush = new SolidBrush(Color.Gray);
                                g.DrawString(colors, _fontStats, gemBrush, 2, yPos);
                                gemBrush.Dispose();
                                // update our cursor position
                                SizeF gem_info_size = g.MeasureString(colors, _fontStats);
                                yPos += (int)gem_info_size.Height;
                            }
                            #endregion

                            if (_currentItem.Quality != ItemQuality.Temp)
                            {
                                PointF draw_pos = new PointF(2, yPos);
                                //Rectangle textRec = new Rectangle(2, (hasSockets ? 76 : 18) + statHeight + 4, _cachedToolTipImage.Width - 4, (int)locationSize.Height + extraLocation);
                                g.DrawString(location, _fontStats, SystemBrushes.InfoText, draw_pos);
                                // update yPos here
                                SizeF quality_size = g.MeasureString(location, _fontStats);
                                yPos += (int)quality_size.Height;

                                if (!CurrentItem.IsGem) {
                                    draw_pos = new PointF(2, yPos);
                                    g.DrawString(enchant, _fontStats, SystemBrushes.InfoText, draw_pos);
                                    // update yPos here
                                    quality_size = g.MeasureString(enchant, _fontStats);
                                    yPos += (int)quality_size.Height;
                                }
                            }

                            xPos = 2;
                            //yPos = (hasSockets ? 76 : 18) + statHeight + 4 + (int)locationSize.Height + extraLocation + 2;

                            #region Extra Item Lists (like in Build Upgrade List)
                            if (CurrentCharacterItems != null)
                            {
                                for (int slot = 0; slot < Character.SlotCount; slot++)
                                {
                                    ItemInstance tinyItem = CurrentCharacterItems[slot];
                                    if (tinyItem != null && (_currentItemInstance == null || tinyItem.GemmedId != _currentItemInstance.GemmedId))
                                    {
                                        Image icon = ItemIcons.GetItemIcon(tinyItem.Item, true);
                                        if (icon != null)
                                        {
                                            g.DrawImage(icon, xPos + 1, yPos + 1, 16, 16);
                                            icon.Dispose();
                                            xPos += tinyItemSize;
                                        }
                                        for (int i = 1; i <= 3; i++)
                                        {
                                            Item gem = tinyItem.GetGem(i);
                                            if (gem != null)
                                            {
                                                icon = ItemIcons.GetItemIcon(gem, true);
                                                if (icon != null)
                                                {
                                                    g.DrawImage(icon, xPos + 1, yPos + 1, 16, 16);
                                                    icon.Dispose();
                                                    xPos += tinyItemSize;
                                                }
                                            }
                                        }

                                        xPos = 2;

                                        { // hide the name brush scope to reduce bugs
                                            Brush nameBrush = null;
                                            switch (tinyItem.Item.Quality)
                                            {
                                                case ItemQuality.Epic:
                                                    nameBrush = Brushes.Purple;
                                                    break;
                                                case ItemQuality.Legendary:
                                                    nameBrush = Brushes.Orange;
                                                    break;
                                                case ItemQuality.Poor:
                                                    nameBrush = Brushes.Gray;
                                                    break;
                                                case ItemQuality.Rare:
                                                    nameBrush = Brushes.Blue;
                                                    break;
                                                case ItemQuality.Uncommon:
                                                    nameBrush = Brushes.Green;
                                                    break;
                                                case ItemQuality.Heirloom:
                                                case ItemQuality.Artifact:
                                                    nameBrush = Brushes.Gold;
                                                    break;
                                                default:
                                                    nameBrush = SystemBrushes.InfoText;
                                                    break;
                                            }
                                            string label = tinyItem.Item.Name;
                                            Enchant tinyEnchant = CurrentCharacterItems[slot].Enchant;
                                            if (tinyEnchant != null && tinyEnchant.Id != 0) label = label + " (" + tinyEnchant.ToString() + ")";
                                            g.DrawString(label, _fontTinyName, nameBrush, xPos + 76, yPos + 1);
                                            yPos += tinyItemSize;
                                        }
                                    }
                                }
                            }
                            #endregion

                            g.Dispose();
                            g = null;
                        }
                    }
                // ===== For non-item tooltips =====
                } else if (_cachedToolTipImage == null && CurrentString != null && CurrentString != "") {
                    // Discover the minimum size needed
                    string Title = CurrentString.Split('|')[0].Trim();
                    string Desc  = CurrentString.Split('|')[1].Trim();

                    int extraSize = 0;
                    int SizeofanExtraLine = 13;
                    int countExtraLines = 1;
                    int widthCharCount = 47;
                    int nextIndex = 0, lastIndex = 0;
                    int loopcount = 0;

                    while (lastIndex + widthCharCount < Desc.Length && loopcount < 50)
                    {
                        string sub = Desc.Substring(lastIndex + 2, Math.Min(lastIndex + widthCharCount, Desc.Length - lastIndex) - 2);
                        if (sub.Contains("\r\n")) {
                            lastIndex = lastIndex + sub.IndexOf("\r\n") + 2;
                            continue;
                        }
                        nextIndex = (Desc.Length > lastIndex + widthCharCount) ? Desc.IndexOf(" ", lastIndex + widthCharCount) : -1;
                        if (nextIndex > 0) {
                            Desc = Desc.Insert(nextIndex + 1, "\r\n");
                            lastIndex = nextIndex + 1;
                            countExtraLines++;
                        } else {
                            lastIndex = Desc.Length;
                        }
                        loopcount++;
                    }

                    countExtraLines = Math.Max(countExtraLines, Desc.Split('\n').Length);
                    extraSize = countExtraLines * SizeofanExtraLine;

                    // Generate the Base Tooltip Image for the screen
                    _cachedToolTipImage = new Bitmap(309, 13 + extraSize + 13, PixelFormat.Format32bppArgb);

                    #region Setup for Background of the tooltip
                    Graphics g = Graphics.FromImage(_cachedToolTipImage);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;

                    Rectangle rectBorder =
                        new Rectangle(0, 0, _cachedToolTipImage.Width - 1, _cachedToolTipImage.Height - 1);
                    LinearGradientBrush lgBrush = new LinearGradientBrush(rectBorder,
                                                            Color.FromArgb(212, 212, 255), Color.FromArgb(192, 192, 255),
                                                            90);
                    g.FillRectangle(lgBrush, rectBorder);
                    lgBrush.Dispose();
                    lgBrush = null;

                    Pen pen = new Pen(Color.FromArgb(128, 0, 0, 0));
                    g.DrawRectangle(pen, rectBorder);
                    pen.Dispose();
                    pen = null;
                    #endregion

                    // Draw the tooltip Title text
                    g.DrawString(Title, new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point, 0), Brushes.Black, 2, 4);

                    // Draw the tooltip text
                    g.DrawString(Desc, _fontName, SystemBrushes.InfoText, 2, 4 + 16);

                    // JOTHAY NOTE: Try to add the info for desc level, like with talents: 2/5, maybe even a next level add-on
                }
                if (resetItem) {
                    CurrentItemInstance = null;
                    resetItem = false;
                }
                return _cachedToolTipImage;
            }
        }

        public void Show(Character character, string desc, IWin32Window window, Point point)
        {
            Character = character;
            CurrentItem = null;
            CurrentCharacterItems = null;
            CurrentSlot = CharacterSlot.None;
            CurrentString = desc/*.Split('|')[1]*/;
            if (CachedToolTipImage != null)
            {
                base.Show(desc/*.Split('|')[0]*/, window, point);
            }
        }

        public void Show(Character character, Item item, IWin32Window window, Point point)
        {
            Show(character, item, null, CharacterSlot.None, window, point);
        }

        public void Show(Character character, Item item, ItemInstance[] characterItems, CharacterSlot slot, IWin32Window window, Point point)
		{
            Character = character;
			CurrentItem = item;
            CurrentCharacterItems = characterItems;
			CurrentSlot = slot;            
            if (CachedToolTipImage != null && CurrentItem != null)
            {
                base.Show(item.Name, window, point);
            }
		}

        public void Show(Character character, ItemInstance item, IWin32Window window, Point point)
        {
            Show(character, item, null, CharacterSlot.None, window, point);
        }

        public void Show(Character character, ItemInstance item, ItemInstance[] characterItems, CharacterSlot slot, IWin32Window window, Point point)
        {
            Character = character;
            CurrentItemInstance = item;
            CurrentCharacterItems = characterItems;
			CurrentSlot = slot;
            if (CachedToolTipImage != null && CurrentItemInstance != null && item.Item != null)
            {
                 base.Show(item.Item.Name, window, point);
            }
        }

        private void ItemToolTip_Popup(object sender, PopupEventArgs e)
        {
			e.ToolTipSize = CachedToolTipImage.Size;
		}

        private void ItemToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            if (CachedToolTipImage != null)
                //the emotion opens up swallow you
                e.Graphics.DrawImageUnscaled(CachedToolTipImage, 0, 0);
        }
    }
}