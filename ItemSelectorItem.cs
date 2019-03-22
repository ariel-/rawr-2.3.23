using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Rawr
{
	public partial class ItemSelectorItem : UserControl
	{
		public ItemSelectorItem()
		{
			this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			this.UpdateStyles();

			InitializeComponent();
			this.Dock = DockStyle.Top;

			////Ugh, this is sloppy, but I'm tired at the moment.
			//foreach (Control a in this.Controls)
			//{
			//    a.MouseMove += new MouseEventHandler(ItemSelectorItem_MouseEnterLeave);
			//    a.MouseLeave += new EventHandler(ItemSelectorItem_MouseEnterLeave);
			//    a.MouseClick += new MouseEventHandler(ItemSelectorItem_MouseClick);
			//    foreach (Control b in a.Controls)
			//    {
			//        b.MouseMove += new MouseEventHandler(ItemSelectorItem_MouseEnterLeave);
			//        b.MouseLeave += new EventHandler(ItemSelectorItem_MouseEnterLeave);
			//        b.MouseClick += new MouseEventHandler(ItemSelectorItem_MouseClick);
			//        foreach (Control c in b.Controls)
			//        {
			//            c.MouseMove += new MouseEventHandler(ItemSelectorItem_MouseEnterLeave);
			//            c.MouseLeave += new EventHandler(ItemSelectorItem_MouseEnterLeave);
			//            c.MouseClick += new MouseEventHandler(ItemSelectorItem_MouseClick);
			//        }
			//    }
			//}
			this.MouseMove += new MouseEventHandler(ItemSelectorItem_MouseEnterLeave);
			this.MouseLeave += new EventHandler(ItemSelectorItem_MouseEnterLeave);
			this.MouseClick += new MouseEventHandler(ItemSelectorItem_MouseClick);
			Calculations.ModelChanged += new EventHandler(Calculations_ModelChanged);
			//CreateSubPointPanels();
		}

		void Calculations_ModelChanged(object sender, EventArgs e)
		{
			Invalidate();
			//CreateSubPointPanels();
		}

		void ItemSelectorItem_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
                if (_itemCalculation.Item.Id == 0)
                {
					if (_itemCalculation.Item.Name == "Empty")
	                    (FindForm() as FormItemSelection).Select((ItemInstance)null);
					else
						(FindForm() as FormItemSelection).Select((Item)null);
				}
                else if (_itemCalculation.ItemInstance != null)
                {
                    (FindForm() as FormItemSelection).Select(_itemCalculation.ItemInstance);
                }
                else
                {
                    (FindForm() as FormItemSelection).Select(_itemCalculation.Item);
                }
			}
			else if (e.Button == MouseButtons.Right)
			{
				if (ItemCalculation.ItemInstance != null)
					ItemContextualMenu.Instance.Show(Character, ItemCalculation.ItemInstance, CharacterSlot.None, !ItemCalculation.Equipped);
				else
				{
					ItemInstance instance = GemmingTemplate.CurrentTemplates[0].GetItemInstance(ItemCalculation.Item, null, false);
					ItemContextualMenu.Instance.Show(Character, instance, CharacterSlot.None, !ItemCalculation.Equipped);
				}
			}
		}

		private bool _tooltipShown = false;
		Rectangle rectMouseTest;
		void ItemSelectorItem_MouseEnterLeave(object sender, MouseEventArgs e) { ItemSelectorItem_MouseEnterLeave(sender, (EventArgs)e); }
		void ItemSelectorItem_MouseEnterLeave(object sender, EventArgs e)
		{
			rectMouseTest = new Rectangle(2, 2, this.Width - 4, this.Height - 4);
			if (rectMouseTest.Contains(this.PointToClient(Control.MousePosition)))
				ShowToolTip();
			else
				HideToolTip();
		}

		public void HideToolTip()
		{
			if (_tooltipShown && !IsDisposed)
			{
				_tooltipShown = false;
				//UpdateBackColors();
				Invalidate();
			}
		}

		public void ShowToolTip()
		{
			if (!_tooltipShown && !IsDisposed)
			{
				_tooltipShown = true;
				int tipX = this.Width + 20;
				if (Parent.PointToScreen(Location).X + tipX + 249 > System.Windows.Forms.Screen.GetWorkingArea(this).Right)
					tipX = -249;
                if (_itemCalculation.ItemInstance != null)
                {
                    ItemToolTip.Instance.Show(Character, _itemCalculation.ItemInstance.Id == 0 ? null : _itemCalculation.ItemInstance, null, CharacterSlot, this, new Point(tipX, 0));
                }
                else
                {
					ItemToolTip.Instance.Show(Character, _itemCalculation.Item.Id == 0 ? null : _itemCalculation.Item, null, CharacterSlot, this, new Point(tipX, 0));
                }

				//UpdateBackColors();
				Invalidate();
				
				foreach (ItemSelectorItem ctrl in Parent.Controls)
					if (ctrl != this && !ctrl.IsDisposed)
						ctrl.HideToolTip();
			}
		}

		private new void Invalidate()
		{
			_prerenderedImage = null;
			base.Invalidate();
		}

		private static Brush _brushEquipped = null;
		public static Brush BrushEquipped
		{
			get
			{
				if (_brushEquipped == null)
					_brushEquipped = new SolidBrush(Color.FromArgb(212, 212, 255));
				return _brushEquipped;
			}
		}

		private static Brush _brushHighlight = null;
		public static Brush BrushHighlight
		{
			get
			{
				if (_brushHighlight == null)
					_brushHighlight = new SolidBrush(Color.FromArgb(192, 192, 255));
				return _brushHighlight;
			}
		}

		private static Brush _brushHighlightBorder = null;
		public static Brush BrushHighlightBorder
		{
			get
			{
				if (_brushHighlightBorder == null)
					_brushHighlightBorder = new SolidBrush(Color.FromArgb(128, 128, 255));
				return _brushHighlightBorder;
			}
		}

		private static Brush _brushMeta = null;
		public static Brush BrushMeta
		{
			get
			{
				if (_brushMeta == null)
					_brushMeta = new SolidBrush(Color.Silver);
				return _brushMeta;
			}
		}

		private static Brush _brushRed = null;
		public static Brush BrushRed
		{
			get
			{
				if (_brushRed == null)
					_brushRed = new SolidBrush(Color.Red);
				return _brushRed;
			}
		}

		private static Brush _brushBlue = null;
		public static Brush BrushBlue
		{
			get
			{
				if (_brushBlue == null)
					_brushBlue = new SolidBrush(Color.Blue);
				return _brushBlue;
			}
		}

		private static Brush _brushYellow = null;
		public static Brush BrushYellow
		{
			get
			{
				if (_brushYellow == null)
					_brushYellow = new SolidBrush(Color.Yellow);
				return _brushYellow;
			}
		}

		private static StringFormat _stringFormatItemName = null;
		public static StringFormat StringFormatItemName
		{
			get
			{
				if (_stringFormatItemName == null)
				{
					_stringFormatItemName = new StringFormat();
					_stringFormatItemName.Alignment = StringAlignment.Near;
					_stringFormatItemName.LineAlignment = StringAlignment.Center;
				}
				return _stringFormatItemName;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			try
			{
				e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
				e.Graphics.DrawImageUnscaled(PrerenderedImage, 0, 0);
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}

		private bool _isEnchant = false;
		public bool IsEnchant
		{
			get { return _isEnchant; }
			set
			{
				_isEnchant = value;
				Height = value ? 24 : 43;
			}
		}

		private Bitmap _prerenderedImage = null;
		public Bitmap PrerenderedImage
		{
			get
			{
				try
				{
					if (_prerenderedImage == null)
					{
						_prerenderedImage = new Bitmap(this.Width, this.Height);
						Graphics g = Graphics.FromImage(_prerenderedImage);
						g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
						g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

						if (_tooltipShown)
						{
							g.FillRectangle(BrushHighlightBorder, 2, 2, this.Width - 4, this.Height - 2);
							g.FillRectangle(BrushHighlight, 3, 3, this.Width - 6, this.Height - 4);
						}
						else if (_itemCalculation.Equipped)
							g.FillRectangle(BrushEquipped, 2, 2, this.Width - 4, this.Height - 4);

						if (!string.IsNullOrEmpty(_itemCalculation.Item.IconPath))
							g.DrawImageUnscaled(ItemIcons.GetItemIcon(_itemCalculation.Item, true), 5, 5);
						int gemCount;
                        if (_itemCalculation.ItemInstance != null)
                        {
                            gemCount = (_itemCalculation.ItemInstance.Gem1Id == 0 ? 0 : 1) +
                                (_itemCalculation.ItemInstance.Gem2Id == 0 ? 0 : 1) +
                                    (_itemCalculation.ItemInstance.Gem3Id == 0 ? 0 : 1);
                        }
                        else
                        {
                            gemCount = (_itemCalculation.Item.SocketColor1 == ItemSlot.None ? 0 : 1) +
                                (_itemCalculation.Item.SocketColor2 == ItemSlot.None ? 0 : 1) +
                                    (_itemCalculation.Item.SocketColor3 == ItemSlot.None ? 0 : 1);
                        }
						if (IsEnchant)
							g.DrawString(_itemCalculation.Item.Name, this.Font, new SolidBrush(this.ForeColor),
								new RectangleF(4, 0, Math.Max(0, this.Width - 14 - (gemCount * 31)), Math.Max(0, this.Height - 3)), StringFormatItemName);
						else
							g.DrawString(_itemCalculation.Item.Name, this.Font, new SolidBrush(this.ForeColor),
								new RectangleF(41, 0, Math.Max(0, this.Width - 49 - (gemCount * 31)), Math.Max(0, this.Height - 3)), StringFormatItemName);

						if ((_itemCalculation.ItemInstance != null && _itemCalculation.ItemInstance.Gem1Id != 0) || _itemCalculation.Item.SocketColor1 != ItemSlot.None)
						{
							switch (_itemCalculation.Item.SocketColor1)
							{
								case ItemSlot.Meta:
									g.FillRectangle(BrushMeta, new Rectangle(Math.Max(0, this.Width - 3 - (gemCount * 31)), 8, 26, 26));
									break;
								case ItemSlot.Red:
									g.FillRectangle(BrushRed, new Rectangle(Math.Max(0, this.Width - 3 - (gemCount * 31)), 8, 26, 26));
									break;
								case ItemSlot.Yellow:
									g.FillRectangle(BrushYellow, new Rectangle(Math.Max(0, this.Width - 3 - (gemCount * 31)), 8, 26, 26));
									break;
								case ItemSlot.Blue:
									g.FillRectangle(BrushBlue, new Rectangle(Math.Max(0, this.Width - 3 - (gemCount * 31)), 8, 26, 26));
									break;
							}
							if (_itemCalculation.ItemInstance != null && _itemCalculation.ItemInstance.Gem1 != null)
								g.DrawImage(ItemIcons.GetItemIcon(_itemCalculation.ItemInstance.Gem1, true),
									new Rectangle(this.Width - 2 - (gemCount * 31), 9, 24, 24));
							gemCount--;
						}

                        if ((_itemCalculation.ItemInstance != null && _itemCalculation.ItemInstance.Gem2Id != 0) || _itemCalculation.Item.SocketColor2 != ItemSlot.None)
						{
							switch (_itemCalculation.Item.SocketColor2)
							{
								case ItemSlot.Meta:
									g.FillRectangle(BrushMeta, new Rectangle(Math.Max(0, this.Width - 3 - (gemCount * 31)), 8, 26, 26));
									break;
								case ItemSlot.Red:
									g.FillRectangle(BrushRed, new Rectangle(Math.Max(0, this.Width - 3 - (gemCount * 31)), 8, 26, 26));
									break;
								case ItemSlot.Yellow:
									g.FillRectangle(BrushYellow, new Rectangle(Math.Max(0, this.Width - 3 - (gemCount * 31)), 8, 26, 26));
									break;
								case ItemSlot.Blue:
									g.FillRectangle(BrushBlue, new Rectangle(Math.Max(0, this.Width - 3 - (gemCount * 31)), 8, 26, 26));
									break;
							}
							if (_itemCalculation.ItemInstance != null && _itemCalculation.ItemInstance.Gem2 != null)
								g.DrawImage(ItemIcons.GetItemIcon(_itemCalculation.ItemInstance.Gem2, true),
									new Rectangle(this.Width - 2 - (gemCount * 31), 9, 24, 24));
							gemCount--;
						}

                        if ((_itemCalculation.ItemInstance != null && _itemCalculation.ItemInstance.Gem3Id != 0) || _itemCalculation.Item.SocketColor3 != ItemSlot.None)
						{
							switch (_itemCalculation.Item.SocketColor3)
							{
								case ItemSlot.Meta:
									g.FillRectangle(BrushMeta, new Rectangle(Math.Max(0, this.Width - 3 - (gemCount * 31)), 8, 26, 26));
									break;
								case ItemSlot.Red:
									g.FillRectangle(BrushRed, new Rectangle(Math.Max(0, this.Width - 3 - (gemCount * 31)), 8, 26, 26));
									break;
								case ItemSlot.Yellow:
									g.FillRectangle(BrushYellow, new Rectangle(Math.Max(0, this.Width - 3 - (gemCount * 31)), 8, 26, 26));
									break;
								case ItemSlot.Blue:
									g.FillRectangle(BrushBlue, new Rectangle(Math.Max(0, this.Width - 3 - (gemCount * 31)), 8, 26, 26));
									break;
							}
							if (_itemCalculation.ItemInstance != null && _itemCalculation.ItemInstance.Gem3 != null)
								g.DrawImage(ItemIcons.GetItemIcon(_itemCalculation.ItemInstance.Gem3, true),
									new Rectangle(this.Width - 2 - (gemCount * 31), 9, 24, 24));
							gemCount--;
						}

						float maxWidth = this.Width - 10;
						int startX = 5;
						int sort = (int)_sort;
						int subPointWidth = 0;
						Color[] subPointColors = new Color[Calculations.SubPointNameColors.Values.Count];
						Calculations.SubPointNameColors.Values.CopyTo(subPointColors, 0);
						for (int i = 0; i < _itemCalculation.SubPoints.Length; i++)
						{
							if (sort == i || sort < 0)
							{
								subPointWidth = (int)Math.Floor((float)maxWidth * (_itemCalculation.SubPoints[i] / _maxRating));
								g.FillRectangle(new SolidBrush(subPointColors[i]), new Rectangle(Math.Max(0, startX), Math.Max(0, this.Height - 5), Math.Max(0, subPointWidth), 2));
								startX += subPointWidth;
							}
						}
					}
					return _prerenderedImage;
				}
				catch (Exception ex)
				{
					ex.ToString();
					return null;
				}
			}
		}

		//private void UpdateBackColors()
		//{
		//    if (_tooltipShown)
		//    {
		//        panelBorder.BackColor = Color.FromArgb(128, 128, 255);
		//        panelLeft.BackColor = panelCenter.BackColor = panelRight.BackColor =
		//            panelBottom.BackColor = Color.FromArgb(192, 192, 255);
		//    }
		//    else
		//    {
		//        panelBorder.BackColor = panelLeft.BackColor = panelCenter.BackColor = panelRight.BackColor =
		//            panelBottom.BackColor = (_itemCalculation.Equipped ? Color.FromArgb(212, 212, 255) : SystemColors.Control);
		//    }
		//}

		private Character _character;
		public Character Character
		{
			get { return _character; }
			set { _character = value; }
		}

		private CharacterSlot _characterSlot;
		public CharacterSlot CharacterSlot
		{
			get { return _characterSlot; }
			set { _characterSlot = value; }
		}

		private ComparisonCalculationBase _itemCalculation;
		public ComparisonCalculationBase ItemCalculation
		{
			get { return _itemCalculation; }
			set
			{
				if (_itemCalculation != value)
				{
					_itemCalculation = value;
					_sort = (ComparisonGraph.ComparisonSort)(-1);
					Invalidate();
					
					
					//UpdateBackColors();
					//Item item = _itemCalculation.Item;
					//pictureBoxIcon.Image = ItemIcons.GetItemIcon(item.IconPath);
					//labelName.Text = item.Name;
					//panelRight.Visible = item.Gem1 != null || item.Gem2 != null || item.Gem3 != null;
					//panelRight.Width = (item.Gem3 != null ? 98 : (item.Gem2 != null ? 67 : 36));
					//switch (item.Sockets.Color1)
					//{
					//    case ItemSlot.Meta:
					//        panelGem1.BackColor = Color.Silver;
					//        break;
					//    case ItemSlot.Red:
					//        panelGem1.BackColor = Color.Red;
					//        break;
					//    case ItemSlot.Yellow:
					//        panelGem1.BackColor = Color.Yellow;
					//        break;
					//    case ItemSlot.Blue:
					//        panelGem1.BackColor = Color.Blue;
					//        break;
					//}
					//switch (item.Sockets.Color2)
					//{
					//    case ItemSlot.Meta:
					//        panelGem2.BackColor = Color.Silver;
					//        break;
					//    case ItemSlot.Red:
					//        panelGem2.BackColor = Color.Red;
					//        break;
					//    case ItemSlot.Yellow:
					//        panelGem2.BackColor = Color.Yellow;
					//        break;
					//    case ItemSlot.Blue:
					//        panelGem2.BackColor = Color.Blue;
					//        break;
					//}
					//switch (item.Sockets.Color3)
					//{
					//    case ItemSlot.Meta:
					//        panelGem3.BackColor = Color.Silver;
					//        break;
					//    case ItemSlot.Red:
					//        panelGem3.BackColor = Color.Red;
					//        break;
					//    case ItemSlot.Yellow:
					//        panelGem3.BackColor = Color.Yellow;
					//        break;
					//    case ItemSlot.Blue:
					//        panelGem3.BackColor = Color.Blue;
					//        break;
					//}
					//if (item.Gem1 != null) pictureBoxGem1.Image = ItemIcons.GetItemIcon(item.Gem1);
					//if (item.Gem2 != null) pictureBoxGem2.Image = ItemIcons.GetItemIcon(item.Gem2);
					//if (item.Gem3 != null) pictureBoxGem3.Image = ItemIcons.GetItemIcon(item.Gem3);
				}
			}
		}

		private ComparisonGraph.ComparisonSort _sort = (ComparisonGraph.ComparisonSort)(-1);
		public ComparisonGraph.ComparisonSort Sort
		{
			get { return _sort; }
			set
			{
				if (_sort != value)
				{
					_sort = value;
					Invalidate();
					//foreach (Panel panelSubPoint in panelBottom.Controls)
					//{
					//    panelSubPoint.Visible = (int)_sort == (int)panelSubPoint.Tag || _sort == ComparisonGraph.ComparisonSort.Overall || _sort == ComparisonGraph.ComparisonSort.Alphabetical;
					//}
				}
			}
		}

		//private void CreateSubPointPanels()
		//{
		//    panelBottom.SuspendLayout();
		//    panelBottom.Controls.Clear();
		//    foreach (KeyValuePair<string, Color> subPointNameColors in Calculations.SubPointNameColors)
		//    {
		//        Panel panelSubPoint = new Panel();
		//        panelSubPoint.BackColor = subPointNameColors.Value;
		//        panelSubPoint.Tag = panelBottom.Controls.Count;
		//        panelSubPoint.Dock = DockStyle.Left;
		//        panelBottom.Controls.Add(panelSubPoint);
		//        panelSubPoint.BringToFront();
		//    }
		//    panelBottom.ResumeLayout();
		//    SetMaxRating(_lastMaxRating);
		//}

		private float _maxRating = 100f;
		public float MaxRating
		{
			get { return _maxRating; }
			set
			{
				if (_maxRating != value)
				{
					_maxRating = value;
					if (_maxRating == 0) _maxRating = 1;
					this.Invalidate();
				}
			}
		}
		//public void SetMaxRating(float maxRating)
		//{
		//    _lastMaxRating = maxRating;
		//    if (_itemCalculation != null)
		//        foreach (Panel panelSubPoint in panelBottom.Controls)
		//            try
		//            {
		//                panelSubPoint.Width = (int)Math.Floor((float)(panelBottom.Width - panelBottom.Padding.Horizontal) * (_itemCalculation.SubPoints[(int)panelSubPoint.Tag] / maxRating));
		//            }
		//            catch { }
		//}

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            Calculations.ModelChanged -= new EventHandler(Calculations_ModelChanged);
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
	}
}
