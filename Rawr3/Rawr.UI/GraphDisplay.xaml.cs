using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace Rawr.UI
{
    public partial class GraphDisplay : UserControl
    {

        //public enum Graph : int
        //{   
        //    Gear = 0,
        //    Enchants = 1,
        //    Gems = 2,
        //    Buffs = 3,
        //    TalentSpecs = 5,
        //    Talents = 6,
        //    Glyphs = 7,
        //    RelativeStatValue = 8,
        //    Custom = 10
        //}

        //#region Getters/Setters
        //private Graph currentGraph;
        //public Graph CurrentGraph
        //{
        //    get { return currentGraph; }
        //    set
        //    {
        //        currentGraph = value;
        //        UpdateGraph();
        //    }
        //}

        //private CharacterSlot gear;
        //public CharacterSlot Gear
        //{
        //    get { return gear; }
        //    set
        //    {
        //        gear = value;
        //        ComparisonGraph.Slot = gear;
        //        UpdateGraph();
        //    }
        //}

        //private ItemSlot enchant;
        //public ItemSlot Enchant
        //{
        //    get { return enchant; }
        //    set
        //    {
        //        enchant = value;
        //        UpdateGraph();
        //    }
        //}

        //private CharacterSlot gem;
        //public CharacterSlot Gem
        //{
        //    get { return gem; }
        //    set
        //    {
        //        gem = value;
        //        UpdateGraph();
        //    }
        //}

        //private BuffGroup allBuffs;
        //public BuffGroup AllBuffs
        //{
        //    get { return allBuffs; }
        //    set
        //    {
        //        allBuffs = value;
        //        UpdateGraph();
        //    }
        //}

        //private int custom;
        //public int Custom
        //{
        //    get { return custom; }
        //    set
        //    {
        //        custom = value;
        //        UpdateGraph();
        //    }
        //}

        private string _currentGraph = "Gear|Head";
        public string CurrentGraph
        {
            get { return _currentGraph; }
            set
            {
                _currentGraph = value;
                UpdateGraph();
            }
        }

        private Character _character;
        public Character Character
        {
            get { return _character; }
            set
            {
                if (_character != null)
                {
                    _character.CalculationsInvalidated -= new EventHandler(character_CalculationsInvalidated);
                    _character.ClassChanged -= new EventHandler(character_ModelChanged);
                }
                _character = value;
                _character.CalculationsInvalidated += new EventHandler(character_CalculationsInvalidated);
                _character.ClassChanged += new EventHandler(character_ModelChanged);
                ComparisonGraph.Character = _character;
                //CustomCombo.ItemsSource = new List<string>(Calculations.CustomChartNames);
                //CustomCombo.SelectedIndex = Calculations.CustomChartNames.Length > 0 ? 0 : -1;
                SetCustomSubpoints(Character.CurrentCalculations.SubPointNameColors.Keys.ToArray());
                UpdateBoxes();
                UpdateGraph();
            }
        }
        //#endregion

        public void character_CalculationsInvalidated(object sender, EventArgs e)
        {
            UpdateBoxes();
            UpdateGraph();
        }
        public void character_ModelChanged(object sender, EventArgs e)
        {
            SetCustomSubpoints(Character.CurrentCalculations.SubPointNameColors.Keys.ToArray());
            UpdateBoxes();
            UpdateGraph();
        }

        private bool _loading;
        public GraphDisplay()
        {
            _loading = true;
            // Required to initialize variables
            InitializeComponent();
            ChartPicker1.GraphDisplay = this;
#if SILVERLIGHT
            GraphScroll.SetIsMouseWheelScrollingEnabled(true);
#endif

            #region Filter Side-bar
#if !DEBUG // Remove when the Filters on the Side work
            // This keeps the right edge from looking indented for no reason
            FilterAccordion.Margin = new Thickness(0);        
#endif

            SV_Bind.SetIsMouseWheelScrollingEnabled(true);
            SV_Prof.SetIsMouseWheelScrollingEnabled(true);
            SV_iLevel.SetIsMouseWheelScrollingEnabled(true);
            SV_Type.SetIsMouseWheelScrollingEnabled(true);
            SV_Gems.SetIsMouseWheelScrollingEnabled(true);

            // From Refine Types of Items Listed
            checkBoxes = new List<CheckBox>();

            checkBoxes.Add(CheckBoxPlate);
            checkBoxes.Add(CheckBoxMail);
            checkBoxes.Add(CheckBoxLeather);
            checkBoxes.Add(CheckBoxCloth);

            checkBoxes.Add(CheckBoxDagger);
            checkBoxes.Add(CheckBoxFistWeapon);
            checkBoxes.Add(CheckBoxOneHandedAxe);
            checkBoxes.Add(CheckBoxOneHandedMace);
            checkBoxes.Add(CheckBoxOneHandedSword);

            checkBoxes.Add(CheckBoxStaff);
            checkBoxes.Add(CheckBoxPolearm);
            checkBoxes.Add(CheckBoxTwoHandedAxe);
            checkBoxes.Add(CheckBoxTwoHandedMace);
            checkBoxes.Add(CheckBoxTwoHandedSword);

            checkBoxes.Add(CheckBoxBow);
            checkBoxes.Add(CheckBoxCrossBow);
            checkBoxes.Add(CheckBoxGun);
            checkBoxes.Add(CheckBoxThrown);
            checkBoxes.Add(CheckBoxRelic);
            checkBoxes.Add(CheckBoxWand);

            checkBoxes.Add(CheckBoxShield);
            checkBoxes.Add(CheckBoxMisc);

            UpdateBoxes();

            FilterTree.ItemsSource = ItemFilter.FilterList.FilterList;
            // Do an initial hide
            ButtonExpand_Click(null, null);
            FilterPopup.Visibility = Visibility.Collapsed; // The actual button will be removed later, just hiding it for now
            AI_Source.IsSelected = true;
            AI_Type.IsSelected = AI_Bind.IsSelected = AI_Prof.IsSelected = AI_iLevel.IsSelected = AI_Gems.IsSelected = false;
            FilterAccordion.SelectionMode = AccordionSelectionMode.One;
            #endregion

            _loading = false;
            UpdateGraph();
        }

        private string ConvertBuffSelector(string buffGroup) {
            switch (buffGroup) {
                case "Raid Buffs":        {
                    return 
                        // Stats
                          "Agility and Strength|Intellect|Spirit|Stamina|Stat Add|Stat Multiplier"
                        + "|Health|Attack Power|Attack Power (%)|Ranged Attack Power|Spell Power|Haste (%)|Spell Haste|Physical Haste"
                        // Offensive
                        + "|Damage (%)|Bleed Damage"
                        + "|Physical Critical Strike Chance|Spell Critical Strike Chance|Focus Magic, Spell Critical Strike Chance"
                        // Defensive
                        + "|Armor|Damage Reduction (Major %)|Damage Reduction (Minor %)|Resistance"
                        // Healing
                        + "|Healing Received (%)"
                        // Sustaining
                        + "|Replenishment|Mana Regeneration|Mana Restore|Burst Mana Regeneration"
                        // Other
                        + "|Racial Buffs|Class Buffs"
                        + "|Temp Power Boost";
                }
                case "Raid Debuffs": {
                    return
                        // Ailment
                           "Boss Attack Speed"
                        // Stat Reduction
                        + "|Melee Hit Chance Reduction|Spell Hit Taken|Critical Strike Chance Taken|Spell Critical Strike Taken"
                        + "|Armor (Major)|Armor (Minor)"
                        // Vulvernability
                        + "|Physical Vulnerability|Spell Sensitivity"
                        + "|Disease Damage Taken|Spell Damage Taken";
                }
                case "Current": { return "Current"; }
                default: { return buffGroup; }
            }
        }

        public void UpdateGraph()
        {
            if (_loading || Calculations.Instance == null || Character == null) return;
            string[] parts = CurrentGraph.Split('|');
            switch (parts[0])
            {
                case "Gear":                UpdateGraphGear(parts[1]); break;
                case "Enchants":            UpdateGraphEnchants(parts[1]); break;
                case "Gems":                UpdateGraphGems(parts[1]); break;
                case "Buffs":               UpdateGraphBuffs(parts[1]); break;
                case "Talents and Glyphs":  UpdateGraphTalents(parts[1]); break;
                case "Equipped":            UpdateGraphEquipped(parts[1]); break;
                case "Available":           UpdateGraphAvailable(parts[1]); break;
                case "Direct Upgrades":     UpdateGraphDirectUpgrades(parts[1]); break;
                case "Stat Values":         UpdateGraphStatValues(parts[1]); break;
                default:                    UpdateGraphModelSpecific(parts[1]); break;
            }
        }

        private void SetGraphControl(Control graphControl)
        {
            if (GraphScroll.Content != graphControl)
            {
                GraphScroll.Content = graphControl;
            }
        }

        private int _calculationCount = 0;
        private ComparisonCalculationBase[] _itemCalculations = null;
        private AutoResetEvent _autoResetEvent = null;
        private CharacterSlot _characterSlot = CharacterSlot.AutoSelect;

        #region Update Graph
        private void UpdateGraphGear(string subgraph)
        {
            SetGraphControl(ComparisonGraph);
            _characterSlot = (CharacterSlot)Enum.Parse(typeof(CharacterSlot), subgraph.Replace(" ", ""), true);
            ComparisonGraph.Slot = _characterSlot;
            bool seenEquippedItem = (Character[_characterSlot] == null);

            Calculations.ClearCache();
            List<ItemInstance> relevantItemInstances = Character.GetRelevantItemInstances(_characterSlot);
            _itemCalculations = new ComparisonCalculationBase[relevantItemInstances.Count];
            _calculationCount = 0;
            _autoResetEvent = new AutoResetEvent(false);

#if DEBUG
            System.Diagnostics.Debug.WriteLine("Starting Comparison Calculations");
            DateTime start = DateTime.Now;
#endif
            if (relevantItemInstances.Count > 0)
            {
                foreach (ItemInstance item in relevantItemInstances)
                {
                    if (!seenEquippedItem && Character[_characterSlot].Equals(item)) seenEquippedItem = true;
                    ThreadPool.QueueUserWorkItem(GetItemInstanceCalculations, item);
                }
                _autoResetEvent.WaitOne();
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Finished Comparison Calculations: Total " + DateTime.Now.Subtract(start).TotalMilliseconds.ToString() + "ms");
#endif

            List<ComparisonCalculationBase> listItemCalculations = new List<ComparisonCalculationBase>(_itemCalculations);
            if (!seenEquippedItem) listItemCalculations.Add(Calculations.GetItemCalculations(Character[_characterSlot], Character, _characterSlot));
            _itemCalculations = FilterTopXGemmings(listItemCalculations);


            CGL_Legend.LegendItems = Calculations.SubPointNameColors;
            ComparisonGraph.LegendItems = Calculations.SubPointNameColors;
            ComparisonGraph.Mode = ComparisonGraph.DisplayMode.Subpoints;
            ComparisonGraph.DisplayCalcs(_itemCalculations);
        }

        private void GetItemInstanceCalculations(object item)
        {
            ComparisonCalculationBase result = Calculations.GetItemCalculations((ItemInstance)item, Character, _characterSlot);
            _itemCalculations[Interlocked.Increment(ref _calculationCount) - 1] = result;
            if (_calculationCount == _itemCalculations.Length) _autoResetEvent.Set();
        }
        
        private void UpdateGraphEnchants(string subgraph)
        {
            SetGraphControl(ComparisonGraph);
            ItemSlot slot = (ItemSlot)Enum.Parse(typeof(ItemSlot), subgraph.Replace(" 1", "").Replace(" 2", "").Replace(" ", ""), true);
            CGL_Legend.LegendItems = Calculations.SubPointNameColors;
            ComparisonGraph.LegendItems = Calculations.SubPointNameColors;
            ComparisonGraph.Mode = ComparisonGraph.DisplayMode.Subpoints;
            ComparisonGraph.DisplayCalcs(Calculations.GetEnchantCalculations(slot, Character, Calculations.GetCharacterCalculations(Character), false).ToArray());
        }

        private void UpdateGraphGems(string subgraph)
        {
            SetGraphControl(ComparisonGraph);
            CharacterSlot cslot = CharacterSlot.Gems;
            ItemSlot islot = ItemSlot.None;
            switch (subgraph)
            {
                case "Red":     islot = ItemSlot.Red; break;
                case "Yellow":  islot = ItemSlot.Yellow; break;
                case "Blue":    islot = ItemSlot.Blue; break;
                case "Meta":    cslot = CharacterSlot.Metas; break;
            }
            Calculations.ClearCache();
            Character.ClearRelevantGems(); // we need to reset relevant items for gems to allow colour selection
            List<Item> relevantItems = Character.GetRelevantItems(cslot, islot);
            _itemCalculations = new ComparisonCalculationBase[relevantItems.Count];
            _calculationCount = 0;
            _autoResetEvent = new AutoResetEvent(false);

            if (relevantItems.Count > 0)
            {
                foreach (Item item in relevantItems)
                {
                    ThreadPool.QueueUserWorkItem(GetItemCalculations, item);
                }
                _autoResetEvent.WaitOne();
            }

            _itemCalculations = FilterTopXGemmings(new List<ComparisonCalculationBase>(_itemCalculations));

            CGL_Legend.LegendItems = Calculations.SubPointNameColors;
            ComparisonGraph.LegendItems = Calculations.SubPointNameColors;
            ComparisonGraph.Mode = ComparisonGraph.DisplayMode.Subpoints;
            ComparisonGraph.DisplayCalcs(_itemCalculations);
        }

        private void GetItemCalculations(object item)
        {
            ComparisonCalculationBase result = Calculations.GetItemCalculations((Item)item, Character, _characterSlot);
            _itemCalculations[Interlocked.Increment(ref _calculationCount) - 1] = result;
            if (_calculationCount == _itemCalculations.Length) _autoResetEvent.Set();
        }

        private void UpdateGraphBuffs(string subgraph)
        {
            SetGraphControl(ComparisonGraph);
            CGL_Legend.LegendItems = Calculations.SubPointNameColors;
            ComparisonGraph.LegendItems = Calculations.SubPointNameColors;
            ComparisonGraph.Mode = ComparisonGraph.DisplayMode.Subpoints;
            ComparisonGraph.DisplayCalcs(Calculations.GetBuffCalculations(Character, 
                Calculations.GetCharacterCalculations(Character), ConvertBuffSelector(subgraph)).ToArray());
        }

        private void UpdateGraphTalents(string subgraph)
        {
            SetGraphControl(ComparisonGraph);
            if        (subgraph == "Individual Talents") {
                #region
                List<ComparisonCalculationBase> talentCalculations = new List<ComparisonCalculationBase>();
                Character newChar = Character.Clone();
                CharacterCalculationsBase currentCalc = Calculations.GetCharacterCalculations(Character, null, false, true, false); ;
                CharacterCalculationsBase newCalc;
                ComparisonCalculationBase compare;

                foreach (PropertyInfo pi in Character.CurrentTalents.GetType().GetProperties())
                {
                    TalentDataAttribute[] talentDatas = pi.GetCustomAttributes(typeof(TalentDataAttribute), true) as TalentDataAttribute[];
                    int orig;
                    if (talentDatas.Length > 0)
                    {
                        TalentDataAttribute talentData = talentDatas[0];
                        orig = Character.CurrentTalents.Data[talentData.Index];
                        if (talentData.MaxPoints == (int)pi.GetValue(Character.CurrentTalents, null))
                        {
                            newChar.CurrentTalents.Data[talentData.Index]--;
                            newCalc = Calculations.GetCharacterCalculations(newChar, null, false, true, false);
                            compare = Calculations.GetCharacterComparisonCalculations(newCalc, currentCalc, talentData.Name, talentData.MaxPoints == orig, orig != 0 && orig != talentData.MaxPoints);
                        }
                        else
                        {
                            newChar.CurrentTalents.Data[talentData.Index]++;
                            newCalc = Calculations.GetCharacterCalculations(newChar, null, false, true, false);
                            compare = Calculations.GetCharacterComparisonCalculations(currentCalc, newCalc, talentData.Name, talentData.MaxPoints == orig, orig != 0 && orig != talentData.MaxPoints);
                        }
                        string text = string.Format("Current Rank {0}/{1}\r\n\r\n", orig, talentData.MaxPoints);
                        if (orig == 0)
                        {
                            // We originally didn't have it, so first rank is next rank
                            text += "Next Rank:\r\n";
                            text += talentData.Description[0];
                        }
                        else if (orig >= talentData.MaxPoints)
                        {
                            // We originally were at max, so there isn't a next rank, just show the capped one
                            text += talentData.Description[talentData.MaxPoints - 1];
                        }
                        else
                        {
                            // We aren't at 0 or MaxPoints originally, so it's just a point in between
                            text += talentData.Description[orig - 1];
                            text += "\r\n\r\nNext Rank:\r\n";
                            text += talentData.Description[orig];
                        }
                        compare.Description = text;
                        compare.Item = null;
                        talentCalculations.Add(compare);
                        newChar.CurrentTalents.Data[talentData.Index] = orig;
                    }
                }
                CGL_Legend.LegendItems = Calculations.SubPointNameColors;
                ComparisonGraph.LegendItems = Calculations.SubPointNameColors;
                ComparisonGraph.Mode = ComparisonGraph.DisplayMode.Subpoints;
                ComparisonGraph.DisplayCalcs(talentCalculations.ToArray());
                #endregion
            } else if (subgraph == "Individual Talents (Full)") {
                #region
                List<ComparisonCalculationBase> talentCalculations = new List<ComparisonCalculationBase>();
                Character newCharEmpty = Character.Clone();
                Character newCharFull = Character.Clone();
                CharacterCalculationsBase currentCalc = Calculations.GetCharacterCalculations(Character, null, false, true, false); ;
                CharacterCalculationsBase newCalcEmpty, newCalcFull;
                ComparisonCalculationBase compare;

                foreach (PropertyInfo pi in Character.CurrentTalents.GetType().GetProperties())
                {
                    TalentDataAttribute[] talentDatas = pi.GetCustomAttributes(typeof(TalentDataAttribute), true) as TalentDataAttribute[];
                    int orig;
                    if (talentDatas.Length > 0) {
                        TalentDataAttribute talentData = talentDatas[0];
                        orig = Character.CurrentTalents.Data[talentData.Index];
                        //
                        newCharEmpty.CurrentTalents.Data[talentData.Index] = 0;
                        newCalcEmpty = Calculations.GetCharacterCalculations(newCharEmpty, null, false, true, false);
                        //
                        newCharFull.CurrentTalents.Data[talentData.Index] = talentData.MaxPoints - 1;
                        newCalcFull = Calculations.GetCharacterCalculations(newCharFull, null, false, true, false);
                        //
                        compare = Calculations.GetCharacterComparisonCalculations(newCalcEmpty, newCalcFull, talentData.Name, talentData.MaxPoints == orig, orig != 0 && orig != talentData.MaxPoints);
                        //
                        string text = string.Format("Current Rank {0}/{1}\r\n\r\n", orig, talentData.MaxPoints);
                        text += talentData.Description[talentData.MaxPoints - 1];
                        compare.Description = text;
                        compare.Item = null;
                        talentCalculations.Add(compare);
                        newCharEmpty.CurrentTalents.Data[talentData.Index] = orig;
                        newCharFull.CurrentTalents.Data[talentData.Index] = orig;
                    }
                }
                CGL_Legend.LegendItems = Calculations.SubPointNameColors;
                ComparisonGraph.LegendItems = Calculations.SubPointNameColors;
                ComparisonGraph.Mode = ComparisonGraph.DisplayMode.Subpoints;
                ComparisonGraph.DisplayCalcs(talentCalculations.ToArray());
                #endregion
            } else if (subgraph == "Talent Specs") {
                #region
                List<ComparisonCalculationBase> talentCalculations = new List<ComparisonCalculationBase>();
                Character newChar = Character.Clone();

                TalentsBase nothing = Character.CurrentTalents.Clone();
                for (int i = 0; i < nothing.Data.Length; i++) nothing.Data[i] = 0;
                for (int i = 0; i < nothing.GlyphData.Length; i++) nothing.GlyphData[i] = false;
                newChar.CurrentTalents = nothing;

                CharacterCalculationsBase baseCalc = Calculations.GetCharacterCalculations(newChar, null, false, true, true);
                CharacterCalculationsBase newCalc;
                ComparisonCalculationBase compare;

                bool same, found = false;
                foreach (SavedTalentSpec sts in SavedTalentSpec.SpecsFor(Character.Class))
                {
                    same = false;
                    if (sts.Equals(Character.CurrentTalents)) same = true;
                    newChar.CurrentTalents = sts.TalentSpec();
                    newCalc = Calculations.GetCharacterCalculations(newChar, null, false, true, true);
                    compare = Calculations.GetCharacterComparisonCalculations(baseCalc, newCalc, sts.Name, same, false);
                    compare.Item = null;
                    compare.Name = sts.ToString();
                    compare.Description = sts.Spec;
                    talentCalculations.Add(compare);
                    found = found || same;
                }
                if (!found)
                {
                    newCalc = Calculations.GetCharacterCalculations(Character, null, false, true, true);
                    compare = Calculations.GetCharacterComparisonCalculations(baseCalc, newCalc, "Custom", true, false);
                    talentCalculations.Add(compare);
                }
                CGL_Legend.LegendItems = Calculations.SubPointNameColors;
                ComparisonGraph.LegendItems = Calculations.SubPointNameColors;
                ComparisonGraph.Mode = ComparisonGraph.DisplayMode.Subpoints;
                ComparisonGraph.DisplayCalcs(talentCalculations.ToArray());
                #endregion
            } else if (subgraph == "Glyphs") {
                #region
                List<ComparisonCalculationBase> glyphCalculations = new List<ComparisonCalculationBase>();
                Character newChar = Character.Clone();
                CharacterCalculationsBase currentCalc = Calculations.GetCharacterCalculations(Character, null, false, true, false);
                CharacterCalculationsBase newCalc;
                ComparisonCalculationBase compare;
                List<string> relevant = Calculations.GetModel(Character.CurrentModel).GetRelevantGlyphs();

                foreach (PropertyInfo pi in Character.CurrentTalents.GetType().GetProperties())
                {
                    GlyphDataAttribute[] glyphDatas = pi.GetCustomAttributes(typeof(GlyphDataAttribute), true) as GlyphDataAttribute[];
                    bool orig;
                    if (glyphDatas.Length > 0)
                    {
                        GlyphDataAttribute glyphData = glyphDatas[0];
                        if (relevant == null || relevant.Contains(glyphData.Name))
                        {
                            orig = Character.CurrentTalents.GlyphData[glyphData.Index];
                            if (orig)
                            {
                                newChar.CurrentTalents.GlyphData[glyphData.Index] = false;
                                newCalc = Calculations.GetCharacterCalculations(newChar, null, false, true, false);
                                compare = Calculations.GetCharacterComparisonCalculations(newCalc, currentCalc, glyphData.Name, orig, false);
                            }
                            else
                            {
                                newChar.CurrentTalents.GlyphData[glyphData.Index] = true;
                                newCalc = Calculations.GetCharacterCalculations(newChar, null, false, true, false);
                                compare = Calculations.GetCharacterComparisonCalculations(currentCalc, newCalc, glyphData.Name, orig, false);
                            }
                            // JOTHAY: WTB Tooltips that show info on these charts
                            compare.Description = glyphData.Description;
                            compare.Item = null;
                            glyphCalculations.Add(compare);
                            newChar.CurrentTalents.GlyphData[glyphData.Index] = orig;
                        }
                    }
                }
                CGL_Legend.LegendItems = Calculations.SubPointNameColors;
                ComparisonGraph.LegendItems = Calculations.SubPointNameColors;
                ComparisonGraph.Mode = ComparisonGraph.DisplayMode.Subpoints;
                ComparisonGraph.DisplayCalcs(glyphCalculations.ToArray());
                #endregion
            }
        }

        private void UpdateGraphEquipped(string subgraph)
        {
            SetGraphControl(ComparisonGraph);
            CGL_Legend.LegendItems = Calculations.SubPointNameColors;
            ComparisonGraph.LegendItems = Calculations.SubPointNameColors;
            ComparisonGraph.Mode = ComparisonGraph.DisplayMode.Subpoints;
            List<ComparisonCalculationBase> itemCalculations = new List<ComparisonCalculationBase>();

            if (subgraph == "Gear" || subgraph == "All")
            {
                CharacterSlot[] slots = new CharacterSlot[]
                {
                     CharacterSlot.Back, CharacterSlot.Chest, CharacterSlot.Feet, CharacterSlot.Finger1,
                     CharacterSlot.Finger2, CharacterSlot.Hands, CharacterSlot.Head, CharacterSlot.Legs,
                     CharacterSlot.MainHand, CharacterSlot.Neck, CharacterSlot.OffHand, CharacterSlot.Projectile,
                     CharacterSlot.ProjectileBag, CharacterSlot.Ranged, CharacterSlot.Shoulders,
                     CharacterSlot.Trinket1, CharacterSlot.Trinket2, CharacterSlot.Waist, CharacterSlot.Wrist
                };
                foreach (CharacterSlot slot in slots)
                {
                    ItemInstance item = Character[slot];
                    if (item != null)
                    {
                        itemCalculations.Add(Calculations.GetItemCalculations(item, Character, slot));
                    }
                }
            }
            if (subgraph == "Enchants" || subgraph == "All")
            {
                ItemSlot[] slots = new ItemSlot[]
                {
                     ItemSlot.Back, ItemSlot.Chest, ItemSlot.Feet, ItemSlot.Finger,
                     ItemSlot.Hands, ItemSlot.Head, ItemSlot.Legs,
                     ItemSlot.MainHand, ItemSlot.OffHand, ItemSlot.Ranged, ItemSlot.Shoulders,
                     ItemSlot.Waist, ItemSlot.Wrist
                };
                
                foreach (ItemSlot slot in slots)
                    foreach (ComparisonCalculationBase calc in Calculations.GetEnchantCalculations(slot, Character, Calculations.GetCharacterCalculations(Character), true))
                        itemCalculations.Add(calc);
            }
            if (subgraph == "Buffs" || subgraph == "All")
            {
                itemCalculations.AddRange(Calculations.GetBuffCalculations(Character, Calculations.GetCharacterCalculations(Character), ConvertBuffSelector("Current")));
                ComparisonGraph.DisplayCalcs(itemCalculations.ToArray());
            }
            // Now Push the results to the screen
            ComparisonGraph.DisplayCalcs(itemCalculations.ToArray());
        }

        private void UpdateGraphAvailable(string subgraph)
        {
            SetGraphControl(ComparisonGraph);
            if (subgraph == "Gear")
            {
                foreach (string availableItem in Character.AvailableItems)
                {
                    availableItem.ToString();
                }

                ComparisonCalculationBase calc = Calculations.CreateNewComparisonCalculation();
                calc.Name = "Chart Not Yet Implemented";
                ComparisonGraph.DisplayCalcs(new ComparisonCalculationBase[] { calc });
            }
            else if (subgraph == "Enchants")
            {
                ComparisonCalculationBase calc = Calculations.CreateNewComparisonCalculation();
                calc.Name = "Chart Not Yet Implemented";
                ComparisonGraph.DisplayCalcs(new ComparisonCalculationBase[] { calc });
            }
        }

        private void UpdateGraphDirectUpgrades(string subgraph)
        {
            SetGraphControl(ComparisonGraph);
            if (subgraph == "Gear")
            {
                UpdateGraphDirectUpgradesGear(false);
            }
            else if (subgraph == "Gear / Cost")
            {
                UpdateGraphDirectUpgradesGear(true);
            }
            else if (subgraph == "Enchants")
            {
                ComparisonCalculationBase calc = Calculations.CreateNewComparisonCalculation();
                calc.Name = "Chart Not Yet Implemented";
                ComparisonGraph.DisplayCalcs(new ComparisonCalculationBase[] { calc });
            }
        }

        private void UpdateGraphDirectUpgradesGear(bool divideByCost)
        {
            List<ComparisonCalculationBase> itemCalculations = new List<ComparisonCalculationBase>();
            Dictionary<ItemSlot, CharacterSlot> slotMap = new Dictionary<ItemSlot, CharacterSlot>();
            if (Character != null)
            {
                Dictionary<string, ItemInstance> items = new Dictionary<string, ItemInstance>();

                float Finger1 = (Character[CharacterSlot.Finger1] == null ? 0 : Calculations.GetItemCalculations(
                    Character[CharacterSlot.Finger1], Character, CharacterSlot.Finger1).OverallPoints);
                float Finger2 = (Character[CharacterSlot.Finger2] == null ? 0 : Calculations.GetItemCalculations(
                    Character[CharacterSlot.Finger2], Character, CharacterSlot.Finger2).OverallPoints);

                float Trinket1 = (Character[CharacterSlot.Trinket1] == null ? 0 : Calculations.GetItemCalculations(
                    Character[CharacterSlot.Trinket1], Character, CharacterSlot.Trinket1).OverallPoints);
                float Trinket2 = (Character[CharacterSlot.Trinket2] == null ? 0 : Calculations.GetItemCalculations(
                    Character[CharacterSlot.Trinket2], Character, CharacterSlot.Trinket2).OverallPoints);

                if (Finger2 < Finger1)
                {
                    slotMap[ItemSlot.Finger] = CharacterSlot.Finger2;
                }

                if (Trinket2 < Trinket1)
                {
                    slotMap[ItemSlot.Trinket] = CharacterSlot.Trinket2;
                }

                float MainHand = (Character[CharacterSlot.MainHand] == null ? 0 : Calculations.GetItemCalculations(
                    Character[CharacterSlot.MainHand], Character, CharacterSlot.MainHand).OverallPoints);
                float OffHand = (Character[CharacterSlot.OffHand] == null ? 0 : Calculations.GetItemCalculations(
                    Character[CharacterSlot.OffHand], Character, CharacterSlot.OffHand).OverallPoints);

                if (MainHand > OffHand)
                {
                    slotMap[ItemSlot.OneHand] = CharacterSlot.OffHand;

                }

                foreach (KeyValuePair<ItemSlot, CharacterSlot> kvp in Item.DefaultSlotMap)
                {
                    try
                    {
                        ItemSlot iSlot = kvp.Key;
                        CharacterSlot slot;

                        if (slotMap.ContainsKey(iSlot))
                        {
                            slot = slotMap[iSlot];
                        }
                        else
                        {
                            slot = kvp.Value;
                        }
                        if (slot != CharacterSlot.None)
                        {
                            ComparisonCalculationBase slotCalc;
                            ItemInstance currentItem = Character[slot];
                            if (currentItem == null)
                                slotCalc = Calculations.CreateNewComparisonCalculation();
                            else
                                slotCalc = Calculations.GetItemCalculations(currentItem, Character, slot);

                            foreach (ItemInstance item in Character.GetRelevantItemInstances(slot))
                            {
                                if (!items.ContainsKey(item.GemmedId) && (currentItem == null || currentItem.GemmedId != item.GemmedId))
                                {
                                    if (currentItem != null && currentItem.Item.Unique)
                                    {
                                        CharacterSlot otherSlot = CharacterSlot.None;
                                        switch (slot)
                                        {
                                            case CharacterSlot.Finger1:
                                                otherSlot = CharacterSlot.Finger2;
                                                break;
                                            case CharacterSlot.Finger2:
                                                otherSlot = CharacterSlot.Finger1;
                                                break;
                                            case CharacterSlot.Trinket1:
                                                otherSlot = CharacterSlot.Trinket2;
                                                break;
                                            case CharacterSlot.Trinket2:
                                                otherSlot = CharacterSlot.Trinket1;
                                                break;
                                            case CharacterSlot.MainHand:
                                                otherSlot = CharacterSlot.OffHand;
                                                break;
                                            case CharacterSlot.OffHand:
                                                otherSlot = CharacterSlot.MainHand;
                                                break;
                                        }
                                        if (otherSlot != CharacterSlot.None && Character[otherSlot] != null && Character[otherSlot].Id == item.Id)
                                        {
                                            continue;
                                        }

                                    }

                                    if (!divideByCost || item.Item.Cost > 0.0f)
                                    {
                                        ComparisonCalculationBase itemCalc = Calculations.GetItemCalculations(item, Character, slot);
                                        //bool include = false;
                                        //for (int i = 0; i < itemCalc.SubPoints.Length; i++)
                                        //{
                                        //    itemCalc.SubPoints[i] -= slotCalc.SubPoints[i];
                                        //    include |= itemCalc.SubPoints[i] > 0;
                                        //}
                                        //itemCalc.OverallPoints -= slotCalc.OverallPoints;
                                        //if ( itemCalc.OverallPoints > 0)
                                        //{
                                        //    itemCalculations.Add(itemCalc);
                                        //}

                                        float difference = itemCalc.OverallPoints - slotCalc.OverallPoints;
                                        if (difference > 0)
                                        {
                                            itemCalc.SubPoints = new float[itemCalc.SubPoints.Length];
                                            if (divideByCost)
                                            {
                                                itemCalc.OverallPoints = difference / item.Item.Cost;
                                            }
                                            else
                                            {
                                                itemCalc.OverallPoints = difference;
                                            }
                                            itemCalculations.Add(itemCalc);
                                        }
                                    }

                                    items[item.GemmedId] = item;
                                }
                            }
                        }

                    }
                    catch (Exception)
                    {
                    }
                }
            }
            //ComparisonGraph.RoundValues = true;
            //ComparisonGraph.CustomRendered = false;
            //ComparisonGraph.DisplayMode = ComparisonGraph.GraphDisplayMode.Overall;
            //ComparisonGraph.ItemCalculations = FilterTopXGemmings(itemCalculations);
            //ComparisonGraph.EquipSlot = CharacterSlot.AutoSelect;
            //ComparisonGraph.SlotMap = slotMap;
            //_characterSlot = CharacterSlot.None;

            //Dictionary<string, Color> overall = new Dictionary<string,Color>();
            //overall.Add("Overall", Colors.Purple);
            //ComparisonGraph.LegendItems = overall;
            ComparisonGraph.Mode = ComparisonGraph.DisplayMode.Overall;
            ComparisonGraph.DisplayCalcs(FilterTopXGemmings(itemCalculations));
        }

        private void UpdateGraphStatValues(string subgraph)
        {
            SetGraphControl(ComparisonGraph);
            if (subgraph == "Relative Stat Values")
            {
                CGL_Legend.LegendItems = Calculations.SubPointNameColors;
                ComparisonGraph.LegendItems = Calculations.SubPointNameColors;
                ComparisonGraph.Mode = ComparisonGraph.DisplayMode.Subpoints;
                ComparisonGraph.DisplayCalcs(CalculationsBase.GetRelativeStatValues(Character));
            }
        }

        private void UpdateGraphModelSpecific(string subgraph)
        {
            Control control = Calculations.GetCustomChartControl(subgraph);
            if (control != null)
            {
                SetGraphControl(control);
                Calculations.UpdateCustomChartData(Character, subgraph, control);
            }
            else
            {
                SetGraphControl(ComparisonGraph);
                var calcs = Calculations.GetCustomChartData(Character, subgraph); // should be called before requesting SubPointNameColors because some models swap colors depending on subgraph, ideally an api would be changed that allows query of subpoints based on chart name
                CGL_Legend.LegendItems = Calculations.SubPointNameColors;
                ComparisonGraph.LegendItems = Calculations.SubPointNameColors;
                ComparisonGraph.Mode = ComparisonGraph.DisplayMode.Subpoints;
                ComparisonGraph.DisplayCalcs(calcs);
            }
        }
        #endregion

        // takes a list of items and returns a sorted filtered array of the top X gemmings
        private ComparisonCalculationBase[] FilterTopXGemmings(List<ComparisonCalculationBase> listItemCalculations)
        {
            List<ComparisonCalculationBase> filteredItemCalculations = new List<ComparisonCalculationBase>();
            int maxGemmings = Properties.GeneralSettings.Default.CountGemmingsShown;

            listItemCalculations.Sort(ComparisonGraph.CompareItemCalculations);
            
            Dictionary<int, int> countItem = new Dictionary<int, int>();
            foreach (ComparisonCalculationBase itemCalculation in listItemCalculations)
            {
                int itemId = 0;
                if (itemCalculation.ItemInstance != null)
                {
                    itemId = itemCalculation.ItemInstance.Id;
                }
                else if (itemCalculation.Item != null)
                {
                    itemId = itemCalculation.Item.Id;
                }
                if (!countItem.ContainsKey(itemId)) countItem.Add(itemId, 0);
                if (countItem[itemId]++ < maxGemmings ||
                    itemCalculation.Equipped || itemCalculation.ItemInstance.ForceDisplay)
                {
                    //Debug.Print("Itemid: " + itemId + " Instances of that item : " + countItem[itemId]);
                    filteredItemCalculations.Add(itemCalculation);
                }
            }
            return filteredItemCalculations.ToArray();
        }

        private IEnumerable<string> customSubpoints;
        private void SetCustomSubpoints(IEnumerable<string> subpoints)
        {
            if (subpoints == null) customSubpoints = new string[0];
            else customSubpoints = subpoints;

            if (customSubpoints.Count<string>() == 3) {
                // Add combination sorts, like Mit+Surv(noThreat) or Mit+Threat(noSurv)
                List<string> newCustom = customSubpoints.ToList();
                newCustom.Add(newCustom[0] + "+" + newCustom[1]);
                newCustom.Add(newCustom[0] + "+" + newCustom[2]);
                newCustom.Add(newCustom[1] + "+" + newCustom[2]);
                customSubpoints = newCustom;
            } else if (customSubpoints.Count<string>() == 4) {
                // Add combination sorts, like Mit+Surv(noThreat) or Mit+Threat(noSurv)
                List<string> newCustom = customSubpoints.ToList();
                newCustom.Add(newCustom[0] + "+" + newCustom[1]);
                newCustom.Add(newCustom[0] + "+" + newCustom[2]);
                newCustom.Add(newCustom[0] + "+" + newCustom[3]);
                newCustom.Add(newCustom[1] + "+" + newCustom[2]);
                newCustom.Add(newCustom[1] + "+" + newCustom[3]);
                newCustom.Add(newCustom[2] + "+" + newCustom[3]);
                customSubpoints = newCustom;
            } else if (customSubpoints.Count<string>() == 5) {
                // Add combination sorts, like Mit+Surv(noThreat) or Mit+Threat(noSurv)
                List<string> newCustom = customSubpoints.ToList();
                newCustom.Add(newCustom[0] + "+" + newCustom[1]);
                newCustom.Add(newCustom[0] + "+" + newCustom[2]);
                newCustom.Add(newCustom[0] + "+" + newCustom[3]);
                newCustom.Add(newCustom[0] + "+" + newCustom[4]);
                newCustom.Add(newCustom[1] + "+" + newCustom[2]);
                newCustom.Add(newCustom[1] + "+" + newCustom[3]);
                newCustom.Add(newCustom[1] + "+" + newCustom[4]);
                newCustom.Add(newCustom[2] + "+" + newCustom[3]);
                newCustom.Add(newCustom[2] + "+" + newCustom[4]);
                newCustom.Add(newCustom[3] + "+" + newCustom[4]);
                customSubpoints = newCustom;
            }

            List<string> sortOptionsList = new List<string>();
            sortOptionsList.Add("Alphabetical");
            sortOptionsList.Add("Overall");
            foreach (string s in customSubpoints) sortOptionsList.Add(s);
            if (SortCombo.Items.Count == 2) { SortCombo.Items.Clear(); } // Then it's a default list and needs to be removed
            else { SortCombo.SelectedItem = "Overall"; } // There's a valid list in place and we need to enforce Overall as selected instead of a subpoint
            SortCombo.ItemsSource = sortOptionsList;
            SortCombo.SelectedItem = "Overall";
        }

        private void SortChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortCombo != null && SortCombo.SelectedIndex != -1)
                ComparisonGraph.Sort = (ComparisonSort)(SortCombo.SelectedIndex - 2);
        }

        private void TB_LiveFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // search next selected item
                int sel = ComparisonGraph.FindItem(TB_LiveFilter.Text, 0/*ComparisonGraph.SelectedItemIndex + 1*/);

                // change
                ComparisonGraph.SelectedItemIndex = sel;

                // if N/A - ding
                TB_LiveFilter.Background = new SolidColorBrush(((sel < 0) ? Color.FromArgb(255, 255, 218, 248) : Colors.White));

                // Weird Focus issue
                //toolStripItemComparison.Focus();
                //txtFilterBox.Focus();

                // we handled it fine
                e.Handled = true;
            }
            else
            {
                if (TB_LiveFilter.Background != new SolidColorBrush(Colors.White))
                    TB_LiveFilter.Background = new SolidColorBrush(Colors.White);
                // Weird Focus issue
                //TB_LiveFilter.Focus();
            }
        }

        #region Filter Side-bar
        // From GridSplitter
        DateTime lastClicked;
        GridLength cachedColumnWidth;
        void GridSplitter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            // The Poor Man's DoubleClick:
            if (DateTime.Now.Subtract(lastClicked).TotalMilliseconds < 500) {
                // What column is it in?
                int colIndex = (int)FilterSplitter.GetValue(Grid.ColumnProperty);
                // And what grid are we talking about?
                Grid grid = FilterSplitter.Parent as Grid;
                Debug.Assert(grid.ColumnDefinitions.Count > 0);
                // Cool...now look at the column definition:
                GridLength gridLength = grid.ColumnDefinitions[colIndex].Width;
                // In order to see if we've already slammed the splitter up against the side:
                GridLength splitterWidth = new GridLength(10/*splitter.Width*/);
                if (gridLength.Equals(splitterWidth)) {
                    // Already collapsed..restore:
                    grid.ColumnDefinitions[colIndex].Width = cachedColumnWidth;
                } else {
                    // Collapse the column:
                    cachedColumnWidth = gridLength;
                    grid.ColumnDefinitions[colIndex].Width = splitterWidth;
                }
            }
            lastClicked = DateTime.Now;
        }
        private void ButtonExpand_Click(object sender, RoutedEventArgs e)
        {
            // What column is it in?
            int colIndex = (int)FilterSplitter.GetValue(Grid.ColumnProperty);
            // And what grid are we talking about?
            Grid grid = FilterSplitter.Parent as Grid;
            Debug.Assert(grid.ColumnDefinitions.Count > 0);
            // Cool...now look at the column definition:
            GridLength gridLength = grid.ColumnDefinitions[colIndex].Width;
            // In order to see if we've already slammed the splitter up against the side:
            GridLength splitterWidth = new GridLength(10/*splitter.Width*/);
            if (gridLength.Equals(splitterWidth)) {
                // Already collapsed..restore:
                grid.ColumnDefinitions[colIndex].Width = cachedColumnWidth;
            } else {
                // Collapse the column:
                cachedColumnWidth = gridLength;
                grid.ColumnDefinitions[colIndex].Width = splitterWidth;
            }
        }

        // From Items By Source
        private void ShowItemFilters(object sender, RoutedEventArgs args)
        {
            new EditItemFilter().Show();
        }
        private void BT_SourceFilters_Reset_Click(object sender, RoutedEventArgs e) { SourceFilters_Helper(true, true); }
        private void BT_SourceFilters_CheckAll_Click(object sender, RoutedEventArgs e) { SourceFilters_Helper(true); }
        private void BT_SourceFilters_UnCheckAll_Click(object sender, RoutedEventArgs e) { SourceFilters_Helper(false); }
        private void SourceFilters_Helper(bool Checked, bool Reset=false) {
            if (Reset) { Checked = true; }
            List<object> sourceFiltersItems = FilterTree.Items.ToList();
            object last = sourceFiltersItems.Last<object>();
            sourceFiltersItems.RemoveAt(sourceFiltersItems.Count-1);
            ItemFilterOther TheOtherOne = last as ItemFilterOther;
            foreach(ItemFilterRegex c in sourceFiltersItems) {
                if (c.Enabled != Checked) {
                    c.Enabled = Checked;
                }
                if (Reset && c.Name.Contains("Disable")) { c.Enabled = false; }
            }
            TheOtherOne.Enabled = Checked;
        }

        // From Refine Types of Items Listed
        private List<ItemType> modelRelevant;
        private List<ItemType> userRelevant;
        private List<CheckBox> checkBoxes;
        public void UpdateBoxes()
        {
            if (Calculations.Instance == null || Calculations.Instance.RelevantItemTypes == null) return;
            modelRelevant = Calculations.Instance != null && Calculations.Instance.RelevantItemTypes != null ? Calculations.Instance.RelevantItemTypes : new List<ItemType>();
            userRelevant = Calculations.Instance != null ? ItemFilter.GetRelevantItemTypesList(Calculations.Instance) : new List<ItemType>();
            foreach (CheckBox box in checkBoxes) {
                if (box == CheckBoxRelic) {
                    box.IsEnabled = modelRelevant.Contains(ItemType.Libram) || modelRelevant.Contains(ItemType.Idol)
                       || modelRelevant.Contains(ItemType.Totem) ||modelRelevant.Contains(ItemType.Sigil);
                    box.IsChecked = userRelevant.Contains(ItemType.Libram) || userRelevant.Contains(ItemType.Idol)
                        || userRelevant.Contains(ItemType.Totem) || userRelevant.Contains(ItemType.Sigil);
                } else {
                    box.IsEnabled = modelRelevant.Contains((ItemType)Enum.Parse(typeof(ItemType), (string)box.Tag, true));
                    box.IsChecked = userRelevant.Contains((ItemType)Enum.Parse(typeof(ItemType), (string)box.Tag, true));
                }
            }
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e) {
            if (_loading) return;
            userRelevant.Clear();
            modelRelevant = Calculations.Instance.RelevantItemTypes;
            foreach (CheckBox box in checkBoxes) {
                if (box.IsChecked.GetValueOrDefault(false) && box.IsEnabled) {
                    if (box == CheckBoxRelic) {
                        if (modelRelevant.Contains(ItemType.Libram)) userRelevant.Add(ItemType.Libram);
                        if (modelRelevant.Contains(ItemType.Totem)) userRelevant.Add(ItemType.Totem);
                        if (modelRelevant.Contains(ItemType.Idol)) userRelevant.Add(ItemType.Idol);
                        if (modelRelevant.Contains(ItemType.Sigil)) userRelevant.Add(ItemType.Sigil);
                    } else {
                        userRelevant.Add((ItemType)Enum.Parse(typeof(ItemType), (string)box.Tag, true));
                    }
                }
            }
            ItemCache.OnItemsChanged();
            //this.DialogResult = true;
        }

        // From Professions
        public void UpdateProfFilters() {

        }
        private void BT_ApplyProfFilter_Click(object sender, RoutedEventArgs e) {

        }
        #endregion

        #region Export Options
        private void CopyCSVDataToClipboard(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(GetChartDataCSV());
        }

        private void ExportToImage(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".png";
            dialog.Filter = "PNG|*.png|GIF|*.gif|JPG|*.jpg|BMP|*.bmp";
            if (dialog.ShowDialog().GetValueOrDefault(false) /*== DialogResult.OK*/)
            {
                try
                {
                    // JOTHAY NOTE TODO: I couldn't find the supported matching class for ImageFormat
                    /*ImageFormat imgFormat = ImageFormat.Bmp;
                    if (dialog.SafeFileName.EndsWith(".png")) imgFormat = ImageFormat.Png;
                    else if (dialog.SafeFileName.EndsWith(".gif")) imgFormat = ImageFormat.Gif;
                    else if (dialog.SafeFileName.EndsWith(".jpg") || dialog.SafeFileName.EndsWith(".jpeg")) imgFormat = ImageFormat.Jpeg;
                    ComparisonGraph.PrerenderedGraph.Save(dialog.SafeFileName, imgFormat);*/
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void ExportToCSV(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".csv";
            dialog.Filter = "Comma Separated Values | *.csv";
            if (dialog.ShowDialog().GetValueOrDefault(false) /*== DialogResult.OK*/)
            {
                try
                {
                    using (StreamWriter writer = File.CreateText(dialog.SafeFileName))
                    {
                        writer.Write(GetChartDataCSV());
                        writer.Flush();
                        writer.Close();
                        writer.Dispose();
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private string GetChartDataCSV()
        {
            StringBuilder sb = new StringBuilder("Name,Equipped,Slot,Gem1,Gem2,Gem3,Enchant,Source,ItemId,GemmedId,Overall");
            foreach (string subPointName in Calculations.SubPointNameColors.Keys)
            {
                sb.AppendFormat(",{0}", subPointName);
            }
            sb.AppendLine();
            foreach (ComparisonCalculationBase comparisonCalculation in _itemCalculations)
            {
                ItemInstance itemInstance = comparisonCalculation.ItemInstance;
                Item item = comparisonCalculation.Item;
                if (itemInstance != null)
                {
                    sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                        itemInstance.Item.Name.Replace(',', ';'),
                        comparisonCalculation.Equipped,
                        itemInstance.Slot,
                        itemInstance.Gem1 != null ? itemInstance.Gem1.Name : null,
                        itemInstance.Gem2 != null ? itemInstance.Gem2.Name : null,
                        itemInstance.Gem3 != null ? itemInstance.Gem3.Name : null,
                        itemInstance.Enchant.Name,
                        itemInstance.Item.LocationInfo[0].Description.Split(',')[0] + (itemInstance.Item.LocationInfo[1]!=null ? "|" + itemInstance.Item.LocationInfo[1].Description.Split(',')[0] : ""),
                        itemInstance.Id,
                        itemInstance.GemmedId,
                        comparisonCalculation.OverallPoints);
                    foreach (float subPoint in comparisonCalculation.SubPoints)
                        sb.AppendFormat(",{0}", subPoint);
                    sb.AppendLine();
                }
                else if (item != null)
                {
                    sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                        item.Name.Replace(',', ';'),
                        comparisonCalculation.Equipped,
                        item.Slot,
                        null,
                        null,
                        null,
                        null,
                        item.LocationInfo[0].Description.Split(',')[0] + (item.LocationInfo[1] != null ? "|" + item.LocationInfo[1].Description.Split(',')[0] : ""),
                        item.Id,
                        null,
                        comparisonCalculation.OverallPoints);
                    foreach (float subPoint in comparisonCalculation.SubPoints)
                        sb.AppendFormat(",{0}", subPoint);
                    sb.AppendLine();
                }
                else
                {
                    sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                        comparisonCalculation.Name.Replace(',', ';'),
                        comparisonCalculation.Equipped,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        comparisonCalculation.OverallPoints);
                    foreach (float subPoint in comparisonCalculation.SubPoints)
                        sb.AppendFormat(",{0}", subPoint);
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
        #endregion
    }
}