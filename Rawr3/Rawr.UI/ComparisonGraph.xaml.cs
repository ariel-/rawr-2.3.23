using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Rawr.Optimizer;

namespace Rawr.UI
{
    public partial class ComparisonGraph : UserControl
    {
#if !SILVERLIGHT
        private ItemContextMenu ItemContextMenu = new ItemContextMenu();
#endif

        private CharacterSlot slot;
        public CharacterSlot Slot
        {
            get { return slot; }
            set
            {                
                slot = value;
            }
        }

        private Character character;
        public Character Character
        {
            get { return character; }
            set
            {
                character = value;
                if (comparisonItems != null)
                {
                    foreach (ComparisonGraphItem cgi in comparisonItems) cgi.Character = character;
                }
            }
        }

        private ComparisonSort sort;
        public ComparisonSort Sort
        {
            get { return sort; }
            set
            {
                sort = value;
                RefreshCalcs();
            }
        }

        public int CompareItemCalculations(ComparisonCalculationBase a, ComparisonCalculationBase b)
        {
            if (Sort == ComparisonSort.Overall)
            {
                if (b.OverallPoints != a.OverallPoints)
                    return b.OverallPoints.CompareTo(a.OverallPoints);
                if (b.Equipped != a.Equipped)
                    return b.Equipped.CompareTo(a.Equipped);
                if (b.PartEquipped != a.PartEquipped)
                    return b.PartEquipped.CompareTo(a.PartEquipped);
                return a.Name.CompareTo(b.Name);
            }
            else if (Sort == ComparisonSort.Alphabetical)
            {
                if (a.Name != b.Name)
                    return a.Name.CompareTo(b.Name);
                if (b.OverallPoints != a.OverallPoints)
                    return b.OverallPoints.CompareTo(a.OverallPoints);
                if (b.PartEquipped != a.PartEquipped)
                    return b.PartEquipped.CompareTo(a.PartEquipped);
                return b.Equipped.CompareTo(a.Equipped);
            }
            else
            {
                int subPointCompare = b.SubPoints[(int)Sort].CompareTo(a.SubPoints[(int)Sort]);
                if (subPointCompare != 0)
                    return subPointCompare;
                if (b.OverallPoints != a.OverallPoints)
                    return b.OverallPoints.CompareTo(a.OverallPoints);
                if (b.Equipped != a.Equipped)
                    return b.Equipped.CompareTo(a.Equipped);
                if (b.PartEquipped != a.PartEquipped)
                    return b.PartEquipped.CompareTo(a.PartEquipped);
                return a.Name.CompareTo(b.Name);
            }
        }

        private DisplayMode mode;
        public DisplayMode Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                if (mode == DisplayMode.Overall)
                {
                    Dictionary<string, Color> overallLegend = new Dictionary<string, Color>();
                    overallLegend["Overall"] = Colors.Purple;
                    LegendItems = overallLegend;
                }
            }
        }

        private ComparisonCalculationBase[] currentCalcs;

        private float minScale = 0;
        private float maxScale = 2;

        private Dictionary<string, Color> legendItems;
        public Dictionary<string, Color> LegendItems
        {
            get { return legendItems; }
            set
            {
                if (legendItems != value)
                {
                    legendItems = value;
                    if (legendItems != null)
                    {
                        if (comparisonItems != null)
                        {
                            foreach (ComparisonGraphItem item in comparisonItems) item.SetColors(legendItems.Values);
                        }
                        /*LegendStack.Children.Clear();
                        LegendStack.Visibility = Visibility.Collapsed;
                        foreach (KeyValuePair<string, Color> kvp in legendItems)
                        {
                            LegendStack.Children.Add(new ComparisonGraphBar() { Color = kvp.Value, Title = kvp.Key });
                        }*/
                    }
                }
            }
        }

        private TextBlock[] AxisLabels;
        private Rectangle[] AxisLines;
        public ComparisonGraph()
        {
            // Required to initialize variables
            InitializeComponent();

            mode = DisplayMode.Subpoints;
            sort = ComparisonSort.Overall;

            AxisLabels = new TextBlock[9];
            AxisLabels[0] = AxisLabel1; AxisLabels[1] = AxisLabel2; AxisLabels[2] = AxisLabel3;
            AxisLabels[3] = AxisLabel4; AxisLabels[4] = AxisLabel5; AxisLabels[5] = AxisLabel6;
            AxisLabels[6] = AxisLabel7; AxisLabels[7] = AxisLabel8; AxisLabels[8] = AxisLabel9;

            AxisLines = new Rectangle[9];
            AxisLines[0] = AxisLine1; AxisLines[1] = AxisLine2; AxisLines[2] = AxisLine3;
            AxisLines[3] = AxisLine4; AxisLines[4] = AxisLine5; AxisLines[5] = AxisLine6;
            AxisLines[6] = AxisLine7; AxisLines[7] = AxisLine8; AxisLines[8] = AxisLine9;

        }

        private List<ComparisonGraphItem> comparisonItems;

        public void RefreshCalcs()
        {
            DisplayCalcs(currentCalcs);
        }

        public void DisplayCalcs(ComparisonCalculationBase[] calcs)
        {
            if (calcs == null) return;
            currentCalcs = calcs;
            if (comparisonItems == null) comparisonItems = new List<ComparisonGraphItem>();
            int i = 0;
#if DEBUG
            DateTime dtA = DateTime.Now;
#endif

            minScale = 0; maxScale = 0;
            foreach (ComparisonCalculationBase c in calcs)
            {
                if (c == null) continue;
                float min = 0f, max = 0f;
                if (Mode == DisplayMode.Overall)
                {
                    if (c.OverallPoints < 0) min += c.OverallPoints;
                    else max += c.OverallPoints;
                }
                else
                {
                    foreach (float f in c.SubPoints)
                    {
                        if (f < 0) min += f;
                        else max += f;
                    }
                }
                if (min < minScale) minScale = min;
                if (max > maxScale) maxScale = max;
            }
            if (minScale > -.01f) minScale = 0f;
            if (maxScale < .01f) maxScale = 0f;
            if (maxScale == 0f && minScale == 0f) maxScale = 2f;

            float totalScale = maxScale - minScale;

            float roundTo = 2f;
            if (totalScale >= 10) roundTo = (int)Math.Pow(10, Math.Floor(Math.Log10(totalScale) - .3f));
            totalScale = roundTo * (float)Math.Ceiling(totalScale / roundTo);

            minScale = -(float)Math.Ceiling(-minScale / totalScale * 8f) * (totalScale / 8f);
            maxScale = (float)Math.Ceiling(maxScale / totalScale * 8f) * (totalScale / 8f);
            if (maxScale - minScale > totalScale)
            {
                totalScale = maxScale - minScale; roundTo = 2f;
                if (totalScale >= 10) roundTo = (int)Math.Pow(10, Math.Floor(Math.Log10(totalScale) - .3f));
                totalScale = roundTo * (float)Math.Ceiling(totalScale / roundTo);

                minScale = -(float)Math.Ceiling(-minScale / totalScale * 8f) * (totalScale / 8f);
                maxScale = (float)Math.Ceiling(maxScale / totalScale * 8f) * (totalScale / 8f);
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("DisplayCalcs Phase A: {0}ms", DateTime.Now.Subtract(dtA).TotalMilliseconds));
            DateTime dtB = DateTime.Now;
#endif

            ChangedSize(this, null);
            IOrderedEnumerable<ComparisonCalculationBase> orderedCalcs;
            if (Sort == ComparisonSort.Alphabetical) 
                orderedCalcs = calcs.OrderBy(calc => calc == null ? "" : calc.Name);
            else if (Sort == ComparisonSort.Overall)
                orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.OverallPoints);
            else if ((int)Sort >= calcs[0].SubPoints.Length)
                // It's a combination one
                if        (calcs[0].SubPoints.Length == 3) {
                    switch ((int)Sort) {
                        case 3: { orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[0] + calc.SubPoints[1]); break; }
                        case 4: { orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[0] + calc.SubPoints[2]); break; }
                        default:{ orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[1] + calc.SubPoints[2]); break; }
                    }
                } else if (calcs[0].SubPoints.Length == 4) {
                    switch ((int)Sort) {
                        case 3: { orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[0] + calc.SubPoints[1]); break; }
                        case 4: { orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[0] + calc.SubPoints[2]); break; }
                        case 5: { orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[0] + calc.SubPoints[3]); break; }
                        case 6: { orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[1] + calc.SubPoints[2]); break; }
                        case 7: { orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[1] + calc.SubPoints[2]); break; }
                        default:{ orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[2] + calc.SubPoints[3]); break; }
                    }
                }
                else /*if (calcs[0].SubPoints.Length == 5)*/ {
                    switch ((int)Sort) {
                        case 3: { orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[0] + calc.SubPoints[1]); break; }
                        case 4: { orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[0] + calc.SubPoints[2]); break; }
                        case 5: { orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[0] + calc.SubPoints[3]); break; }
                        case 6: { orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[0] + calc.SubPoints[4]); break; }
                        case 7: { orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[1] + calc.SubPoints[2]); break; }
                        case 8: { orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[1] + calc.SubPoints[3]); break; }
                        case 9: { orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[1] + calc.SubPoints[4]); break; }
                        case 10:{ orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[2] + calc.SubPoints[3]); break; }
                        case 11:{ orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[2] + calc.SubPoints[4]); break; }
                        default:{ orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[3] + calc.SubPoints[4]); break; }
                    }
                }
            else
                orderedCalcs = calcs.OrderByDescending(calc => calc == null ? 0 : calc.SubPoints[(int)Sort]);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("DisplayCalcs Phase B: {0}ms", DateTime.Now.Subtract(dtB).TotalMilliseconds));
            DateTime dtC = DateTime.Now;
#endif
            foreach (ComparisonCalculationBase c in orderedCalcs)
            {
                if (c == null) continue;
                ComparisonGraphItem item;
                if (i >= comparisonItems.Count)
                {
                    item = new ComparisonGraphItem(LegendItems.Values);
                    item.Character = Character;
                    item.Slot = Slot;
                    comparisonItems.Add(item);
                    ItemStack.Children.Add(item);
                    item.NameGrid.MouseEnter += new MouseEventHandler(NameGrid_MouseEnter);
                    item.NameGrid.MouseLeave += new MouseEventHandler(NameGrid_MouseLeave);

//#if SILVERLIGHT
//                    item.NameGrid.MouseLeftButtonDown += new MouseButtonEventHandler(NameGrid_MouseLeftButtonDown);
//#else
//                    item.NameGrid.ContextMenu = ItemContextMenu;
//                    item.NameGrid.ContextMenuOpening += new ContextMenuEventHandler(NameGrid_ContextMenuOpening);
//#endif
                }
                else item = comparisonItems[i];
                bool isGem = c.Item != null && c.Item.IsGem;
                string s = c.Name;
                if (c.Item != null && c.Item.ItemLevel != 0 && !isGem) { s += string.Format(" [{0}]", c.Item.ItemLevel); }
                item.Title = s;
                item.Equipped = c.Equipped;
                item.PartEquipped = c.PartEquipped;
                item.OtherItem = c.Item;
                item.ItemInstance = c.ItemInstance;
                item.MinScale = minScale;
                item.MaxScale = maxScale;
                item.Slot = Slot;

                item.NameGrid.Tag = c;

                // set visibility first, otherwise ActualWidth is 0 and it will fail to reset colors
                item.Visibility = Visibility.Visible;
                if (Mode == DisplayMode.Overall) { item[0] = c.OverallPoints; }
                else { for (int j = 0; j < c.SubPoints.Length; j++) item[j] = c.SubPoints[j]; }
                
                i++;
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("DisplayCalcs Phase C: {0}ms", DateTime.Now.Subtract(dtC).TotalMilliseconds));
            DateTime dtD = DateTime.Now;
#endif
            for (; i < comparisonItems.Count; i++) comparisonItems[i].Visibility = Visibility.Collapsed;
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("DisplayCalcs Phase D: {0}ms", DateTime.Now.Subtract(dtD).TotalMilliseconds));
            System.Diagnostics.Debug.WriteLine(string.Format("Finished DisplayCalcs: Total {0}ms", DateTime.Now.Subtract(dtA).TotalMilliseconds));
#endif
        }

//#if SILVERLIGHT
//        private void NameGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
//        {
//            Grid senderGrid = sender as Grid;
//            ComparisonCalculationBase calc = senderGrid.Tag as ComparisonCalculationBase;
//            if (calc.ItemInstance != null)
//            {
//                MainPage.Tooltip.Hide();
//                Point mousePoint = e.GetPosition(senderGrid);
//                //ItemContextMenu.SelectedItemInstance = calc.ItemInstance;
//                //ItemContextMenu.Show(senderGrid, mousePoint.X + 4, mousePoint.Y + 4);
//                ContextMenuItem.HorizontalOffset = mousePoint.X + 4;
//                ContextMenuItem.VerticalOffset = mousePoint.Y + 4;
//                ContextMenuItem.IsOpen = true;
//            }
//            e.Handled = true;
//        }
//#else
//        private void NameGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
//        {
//            Grid senderGrid = sender as Grid;
//            ComparisonCalculationBase calc = senderGrid.Tag as ComparisonCalculationBase;
//            if (calc.ItemInstance != null)
//            {
//                MainPage.Tooltip.Hide();
//                ItemContextMenu.SelectedItemInstance = calc.ItemInstance;
//            }
//            else
//            {
//                e.Handled = true;
//            }
//        }
//#endif

        private void NameGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            MainPage.Tooltip.Hide();
        }

        private void NameGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            ComparisonCalculationBase calc = ((Grid)sender).Tag as ComparisonCalculationBase;
            if ((calc.Description != null || calc.ItemInstance != null || calc.Item != null) && (!ContextMenuService.GetContextMenu(((Grid)sender)).IsOpen))
            {
                if      (calc.ItemInstance != null) MainPage.Tooltip.ItemInstance = calc.ItemInstance;
                else if (calc.Item         != null) MainPage.Tooltip.Item = calc.Item;
                else MainPage.Tooltip.CurrentString = calc.Name + "|" + calc.Description;
                if (calc is ComparisonCalculationUpgrades)
                {
                    ComparisonCalculationUpgrades upgrades = calc as ComparisonCalculationUpgrades;
                    MainPage.Tooltip.CharacterItems = upgrades.CharacterItems;
                }
                MainPage.Tooltip.Show((Grid)sender, 143, 2);
            }
        }

        private void ChangedSize(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (ItemStack.ActualWidth > 100)
            {
                int negTicks = (int)(-minScale / (maxScale - minScale) * 8);
                int posTicks = (int)(maxScale / (maxScale - minScale) * 8);

                int j = 0;
                for (float i = minScale; i <= maxScale; i += (maxScale - minScale) / 8, j++)
                {
                    if (i == 0)
                    {
                        AxisLabels[j].Foreground = new SolidColorBrush(Colors.Black);
                        AxisLines[j].Fill = new SolidColorBrush(Colors.Black);
                    }
                    else if (Math.Abs(negTicks - j) % 2 == 1)
                    {
                        AxisLabels[j].Foreground = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0));
                        AxisLines[j].Fill = new SolidColorBrush(Color.FromArgb(25, 0, 0, 0));
                    }
                    else
                    {
                        AxisLabels[j].Foreground = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0));
                        AxisLines[j].Fill = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));
                    }
                    AxisLabels[j].Text = Math.Round(i, 2).ToString();
                }
                if ((int)NameColumn.Width.Value != Rawr.Properties.GeneralSettings.Default.ItemNameWidthSetting * 20)
                {
                    // Reset the width of the name section
                    //ColumnsGrid.ColumnDefinitions[0].Width
                    int width = 142 + Rawr.Properties.GeneralSettings.Default.ItemNameWidthSetting * 20;
                    NameColumn.Width = new GridLength(width, GridUnitType.Pixel);
                    ChartTopLine.Margin = new Thickness(width, 24, 0, 0);
                }
            }
        }

        public enum DisplayMode
        {
            Subpoints,
            Overall
        }

        #region Selecting an Item: find, select, redraw, scroll to

        public int SelectedItemIndex
        {
            get { return _selItemIndex; }
            set
            {
                if (_selItemIndex != value)
                {
                    // change
                    _selItemIndex = value;

                    // redraw
                    Redraw();

                    // scroll to 
                    ScrollTo(value);
                }
            }
        }
        private int _selItemIndex = -1;

        public void Redraw()
        {
            // cleanup
            //if (_prerenderedGraph != null) { _prerenderedGraph.Dispose(); }
            //_prerenderedGraph = null;

            // get it
            //PrerenderedGraph.ToString();

            // mark for next update
            //this.Invalidate();

            foreach (ComparisonGraphItem cgi in comparisonItems) {
                cgi.HighLightedRect.Visibility = Visibility.Collapsed;
            }
        }

        public int FindItem(string sText, int nStartIndex)
        {
            if (!string.IsNullOrEmpty(sText))
            {
                for (int i = nStartIndex; i < comparisonItems.Count; i++)
                {
                    if (comparisonItems[i].TextLabel.Text.IndexOf(sText, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        return i;
                    }
                }
            }

            // none
            return -1;
        }

        public void ScrollTo(int nIndex)
        {
            if (nIndex >= 0 && nIndex < comparisonItems.Count)
            {
                ScrollViewer ScrollBar = this.Parent as ScrollViewer;
                double height = comparisonItems[0].ActualHeight + 4;

                // offset in prerendered zone
                var nOffset = 24 + nIndex * height;

                // viewport start/end Y
                var start = ScrollBar.VerticalOffset;// Value;
                var end = start + ScrollBar.ActualHeight;
                double scroll = 0;

                // check if visible
                if (nOffset < start || nOffset > end)
                {
                    // nice delta
                    var nice = (ScrollBar.ActualHeight / 2) - (height / 2);

                    // calc new offset
                    scroll = Math.Max(nOffset - nice, 0/*ScrollBar.Minimum*/);
                    scroll = Math.Min(scroll, this.ActualHeight/*Maximum*/);

                    // change it
                    //ScrollBar.VerticalOffset = scroll;// Value
                    ScrollBar.ScrollToVerticalOffset(scroll);
                    //this.Invalidate();
                }
                comparisonItems[nIndex].HighLightedRect.Visibility = Visibility.Visible;
                /*new ErrorWindow()
                {
                    Message = string.Format("VO: {0}\r\nSB Height: {1}\r\nthis Height: {2}\r\nOffset: {3}\r\nStart: {4}\r\nEnd: {5}\r\nScroll: {6}",
                        ScrollBar.VerticalOffset, ScrollBar.ActualHeight, this.ActualHeight, nOffset, start, end, scroll)
                }.Show();*/
            }
        }

        #endregion // Selecting an Item: find, select, redraw, scroll to
    }

    public enum ComparisonSort
    {
        //SubPoints will be their index, such as 0 or 1
        Overall = -1,
        Alphabetical = -2
    }
}