using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Rawr.Forms;
using Rawr.UserControls;
using System.IO;
using System.Threading;
using System.Drawing.Imaging;

namespace Rawr
{
    public partial class FormMain : Form, IFormItemSelectionProvider
    {

        private string _storedCharacterPath;
        private bool _storedUnsavedChanged;
        private Character _storedCharacter;
        private BatchCharacter _batchCharacter;

        private FormSplash _splash = new FormSplash();
        private string _characterPath = "";
        private bool _unsavedChanges = false;
        private CharacterCalculationsBase _calculatedStats = null;
        private List<ToolStripMenuItem> _recentCharacterMenuItems = new List<ToolStripMenuItem>();
        private bool _loadingCharacter = false;
        private Character _character = null;
        private List<ToolStripMenuItem> _customChartMenuItems = new List<ToolStripMenuItem>();
        private Status _statusForm;
        private string _formatWindowTitle = "Rawr v{0}";
        private Color _defaultColor = Color.White;
        private System.Threading.Timer _timerCheckForUpdates;

        // we want this access so we can check cancel request from workers
        public Status Status { get { return _statusForm;  } }

        private FormRelevantItemRefinement _itemRefinement;
        public FormRelevantItemRefinement ItemRefinement
        {
            get
            {
                if (_itemRefinement == null || _itemRefinement.IsDisposed)
                    _itemRefinement = new FormRelevantItemRefinement(null);
                return _itemRefinement;
            }
        }

        private FormItemComparison _itemComparison;

        private FormItemFilter _formItemFilter;
        public FormItemFilter FormItemFilter
        {
            get
            {
                if (_formItemFilter == null || _formItemFilter.IsDisposed)
                    _formItemFilter = new FormItemFilter();
                return _formItemFilter;
            }
        }

        private FormItemSelection _formItemSelection;
        public FormItemSelection FormItemSelection
        {
            get
            {
                if (_formItemSelection == null || _formItemSelection.IsDisposed)
                    _formItemSelection = new FormItemSelection();
                return _formItemSelection;
            }
        }

        public TalentPicker TalentPicker { get { return talentPicker1; } }

        private ItemFilterTreeView itemFilterTreeView;

        private static FormMain _instance;
        public static FormMain Instance { get { return FormMain._instance; } }
        public FormMain()
        {
            _instance = this;
            _splash.Show();
            _statusForm = new Status();
            Application.DoEvents();

            Version version = System.Reflection.Assembly.GetCallingAssembly().GetName().Version;
            _formatWindowTitle = string.Format(_formatWindowTitle, version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Build.ToString());

            asyncCalculationStart = new AsynchronousDisplayCalculationDelegate(AsyncCalculationStart);
            asyncCalculationCompleted = new SendOrPostCallback(AsyncCalculationCompleted);
            Rawr.UserControls.Options.GeneralSettings.HideProfessionsChanged += new EventHandler(GeneralSettings_HideProfessionsChanged);
            
            LoadModel(ConfigModel);
            InitializeComponent();
            _defaultColor = itemButtonHead.BackColor;
            if (Type.GetType("Mono.Runtime") != null)
                copyDataToClipboardToolStripMenuItem.Text += " (Doesn't work under Mono)";
            Application.DoEvents();

            Rectangle bounds = ConfigBounds;
            if (bounds.Width >= this.MinimumSize.Width && bounds.Height >= this.MinimumSize.Height)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Bounds = bounds;
            }
            
            Image icon = ItemIcons.GetItemIcon(Calculations.ModelIcons[ConfigModel], true);
            if (icon != null)
            {
                this.Icon = Icon.FromHandle((icon as Bitmap).GetHicon());
            }
            UpdateRecentCharacterMenuItems();

            //ToolStripMenuItem modelsToolStripMenuItem = new ToolStripMenuItem("Models");
            //menuStripMain.Items.Add(modelsToolStripMenuItem);
            //foreach (KeyValuePair<string, Type> kvp in Calculations.Models)
            //{
            //    ToolStripMenuItem modelToolStripMenuItem = new ToolStripMenuItem(kvp.Key);
            //    modelToolStripMenuItem.Click += new EventHandler(modelToolStripMenuItem_Click);
            //    modelToolStripMenuItem.Checked = kvp.Value == Calculations.Instance.GetType();
            //    modelToolStripMenuItem.Tag = kvp;
            //    modelsToolStripMenuItem.DropDownItems.Add(modelToolStripMenuItem);
            //}

            this.Shown += new EventHandler(FormMain_Shown);
            ItemCache.Instance.ItemsChanged += new EventHandler(ItemCache_ItemsChanged);
            Calculations.ModelChanging += new EventHandler(Calculations_ModelChanging);
            Calculations.ModelChanged += new EventHandler(Calculations_ModelChanged);
            // at this point there is no character
            _character = new Character();
            _character.CurrentModel = ConfigModel;
            _character.Class = Calculations.ModelClasses[_character.CurrentModel];
            _characterPath = string.Empty;
            _unsavedChanges = false;
            // we didn't actually set up the character yet
            // model change will force it to reload and set up all needed events and everything
            Calculations_ModelChanged(null, null);

            _loadingCharacter = true;
            sortToolStripMenuItem_Click(overallToolStripMenuItem, EventArgs.Empty);
            slotToolStripMenuItem_Click(headToolStripMenuItem, EventArgs.Empty);
            _loadingCharacter = false;

            itemFilterTreeView = new ItemFilterTreeView();
            itemFilterTreeView.EditMode = false;
            itemFilterTreeView.BorderStyle = BorderStyle.None;
            itemFilterTreeView.Size = new Size(275, 400);

            ToolStripDropDown dropDown = new ToolStripDropDown();
            dropDown.Items.Add(new ToolStripControlHost(itemFilterTreeView));
            toolStripDropDownButtonFilter.DropDown = dropDown;
        }

        private bool _checkForUpdatesEnabled = true;
        void _timerCheckForUpdates_Callback(object data)
        {
            if (_checkForUpdatesEnabled)
            {
                string latestVersion = new Rawr.WebRequestWrapper().GetBetaVersionString();
                if (!string.IsNullOrEmpty(latestVersion))
                {
                    string currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    if (currentVersion != latestVersion)
                    {
                        _checkForUpdatesEnabled = false;
                        DialogResult result = MessageBox.Show(string.Format("A new version of Rawr has been released, version {0}! Would you like to go to the Rawr site to download the new version?",
                            latestVersion), "New Version Released!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (result == DialogResult.Yes)
                        {
                            Help.ShowHelp(null, "http://rawr.codeplex.com/");
                        }
                    }
                }
            }
        }

        void Calculations_ModelChanging(object sender, EventArgs e)
        {
            Character.SerializeCalculationOptions();
        }

        public Character Character
        {
            get
            {
                if (_character == null)
                {
                    Character character = new Character();
                    character.CurrentModel = ConfigModel;
                    character.Class = Calculations.ModelClasses[character.CurrentModel];
                    Character = character;
                    _characterPath = string.Empty;
                    _unsavedChanges = false;
                }
                return _character;
            }
            set
            {
                if (_character != null)
                {
                    _character.ClassChanged -= new EventHandler(_character_ClassChanged);
                    _character.CalculationsInvalidated -= new EventHandler(_character_ItemsChanged);
                    _character.AvailableItemsChanged -= new EventHandler(_character_AvailableItemsChanged);
                }
                _character = value;
                if (_batchCharacter != null && _batchCharacter.Character != _character)
                {
                    // we're loading a character that is not a batch character
                    _batchCharacter = null;
                }
                if (_character != null)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _character.IsLoading = true; // we do not need ItemsChanged event triggering until we call OnItemsChanged at the end

                    Character.CurrentModel = null;
                    
                    Calculations.CalculationOptionsPanel.Character = _character;
                    ItemToolTip.Instance.Character = FormItemSelection.Character = talentPicker1.Character =
                        ItemEnchantContextualMenu.Instance.Character = ItemContextualMenu.Instance.Character = buffSelector1.Character = itemComparison1.Character = 
                        itemButtonBack.Character = itemButtonChest.Character = itemButtonFeet.Character =
                        itemButtonFinger1.Character = itemButtonFinger2.Character = itemButtonHands.Character =
                        itemButtonHead.Character = itemButtonRanged.Character = itemButtonLegs.Character =
                        itemButtonNeck.Character = itemButtonShirt.Character = itemButtonShoulders.Character =
                        itemButtonTabard.Character = itemButtonTrinket1.Character = itemButtonTrinket2.Character =
                        itemButtonWaist.Character = itemButtonMainHand.Character = itemButtonOffHand.Character =
                        itemButtonProjectile.Character = itemButtonProjectileBag.Character = itemButtonWrist.Character = _character;
                    //Ahhh ahhh ahhh ahhh ahhh ahhh ahhh ahhh...

                    if (_itemComparison != null && !_itemComparison.IsDisposed) _itemComparison.Character = _character;

                    _character.ClassChanged += new EventHandler(_character_ClassChanged);
                    _character.CalculationsInvalidated += new EventHandler(_character_ItemsChanged);
                    _character.AvailableItemsChanged += new EventHandler(_character_AvailableItemsChanged);
                    _loadingCharacter = true;

                    textBoxName.Text = Character.Name;
                    textBoxRealm.Text = Character.Realm;
                    comboBoxRegion.Text = Character.Region.ToString();
                    comboBoxRace.Text = Character.Race.ToString();
                    comboBoxProfession1.Text = Character.PrimaryProfession.ToString();
                    comboBoxProfession2.Text = Character.SecondaryProfession.ToString();
                    checkBoxEnforceGemRequirements.Checked = Character.EnforceGemRequirements;
                    checkBoxWaistBlacksmithingSocket.Checked = Character.WaistBlacksmithingSocketEnabled;
                    checkBoxWristBlacksmithingSocket.Checked = Character.WristBlacksmithingSocketEnabled;
                    checkBoxHandsBlacksmithingSocket.Checked = Character.HandsBlacksmithingSocketEnabled;
                    if (comboBoxClass.Text != Character.Class.ToString())
                    {
                        comboBoxClass.Text = Character.Class.ToString();
                        _character_ClassChanged(null, null);
                    }
                    if (_character.LoadItemFilterEnabledOverride())
                    {
                        itemFilterTreeView.GenerateNodes();
                        ItemCache.OnItemsChanged();
                    }

                    //if (_itemComparison != null && !_itemComparison.IsDisposed)
                    //{
                    //    _itemComparison.Hide();
                    //    _itemComparison.Dispose();
                    //}
                    _loadingCharacter = false;
                    UpdateProfessionControls();
                    _character.IsLoading = false;
                    //_character.OnCalculationsInvalidated(); nothing actually changed on the character, we just need calculations
                    _character_ItemsChanged(_character, EventArgs.Empty); // this way it won't cause extra invalidations for other listeners of the event
                }
            }
        }

        void _character_ClassChanged(object sender, EventArgs e)
        {
            _unsavedChanges = true;
            this.Cursor = Cursors.WaitCursor;
            string oldModel = (string)comboBoxModel.SelectedValue;
            if (string.IsNullOrEmpty(oldModel)) oldModel = ConfigModel;
            comboBoxModel.Items.Clear();
            List<string> items = new List<string>();
            foreach (KeyValuePair<string, CharacterClass> kvp in Calculations.ModelClasses)
            {
                if (kvp.Value == _character.Class)
                {
                    items.Add(kvp.Key);
                }
            }
            comboBoxModel.Items.AddRange(items.ToArray());
            if (items.Contains(oldModel)) comboBoxModel.SelectedIndex = items.IndexOf(oldModel);
            else if (comboBoxModel.Items.Count > 0) comboBoxModel.SelectedIndex = 0;
            this.Cursor = Cursors.Default;
        }

        private void SetTitle()
        {
            StringBuilder sb = new StringBuilder(_formatWindowTitle);
            if (_character != null && !String.IsNullOrEmpty(_character.Name))
            {
                sb.Append(" - ");
                sb.Append(_character.Name);
            }
            if (!String.IsNullOrEmpty(_characterPath))
            {
                sb.Append(" - ");
                sb.Append(Path.GetFileName(_characterPath));
                if (_unsavedChanges)
                {
                    sb.Append("*");
                }
            }
            this.Text = sb.ToString();
        }

        void _character_AvailableItemsChanged(object sender, EventArgs e)
        {
            _unsavedChanges = true;
        }

        //private void ItemEnchantsChanged()
        //{
        //    _loadingCharacter = true;
        //    comboBoxEnchantBack.SelectedItem = Character.BackEnchant;
        //    comboBoxEnchantChest.SelectedItem = Character.ChestEnchant;
        //    comboBoxEnchantFeet.SelectedItem = Character.FeetEnchant;
        //    comboBoxEnchantFinger1.SelectedItem = Character.Finger1Enchant;
        //    comboBoxEnchantFinger2.SelectedItem = Character.Finger2Enchant;
        //    comboBoxEnchantHands.SelectedItem = Character.HandsEnchant;
        //    comboBoxEnchantHead.SelectedItem = Character.HeadEnchant;
        //    comboBoxEnchantLegs.SelectedItem = Character.LegsEnchant;
        //    comboBoxEnchantShoulders.SelectedItem = Character.ShouldersEnchant;
        //    comboBoxEnchantMainHand.SelectedItem = Character.MainHandEnchant;
        //    comboBoxEnchantOffHand.SelectedItem = Character.OffHandEnchant;
        //    comboBoxEnchantRanged.SelectedItem = Character.RangedEnchant;
        //    comboBoxEnchantWrists.SelectedItem = Character.WristEnchant;
        //    _loadingCharacter = false;
        //}

        private delegate void AsynchronousDisplayCalculationDelegate(CharacterCalculationsBase calculations, AsyncOperation asyncCalculation);

        private class AsyncCalculationResult
        {
            public CharacterCalculationsBase Calculations;
            public Dictionary<string, string> DisplayCalculationValues;
        }

        AsynchronousDisplayCalculationDelegate asyncCalculationStart;
        SendOrPostCallback asyncCalculationCompleted;
        AsyncOperation asyncCalculation;

        private void AsyncCalculationStart(CharacterCalculationsBase calculations, AsyncOperation asyncCalculation)
        {
            Dictionary<string, string> result = calculations.GetAsynchronousCharacterDisplayCalculationValues();
            asyncCalculation.PostOperationCompleted(asyncCalculationCompleted, new AsyncCalculationResult() { Calculations = calculations, DisplayCalculationValues = result });
        }

        private void AsyncCalculationCompleted(object arg)
        {
            AsyncCalculationResult result = (AsyncCalculationResult)arg;
            if (result.DisplayCalculationValues != null && result.Calculations == _calculatedStats)
            {
                UpdateDisplayCalculationValues(result.DisplayCalculationValues);
                // refresh chart if it's custom chart
                foreach (ToolStripItem item in toolStripDropDownButtonSlot.DropDownItems)
                {
                    if (item is ToolStripMenuItem && (item as ToolStripMenuItem).Checked && item.Tag != null)
                    {
                        itemComparison1.DisplayMode = ComparisonGraph.GraphDisplayMode.Subpoints;
                        string[] tag = item.Tag.ToString().Split('.');
                        switch (tag[0])
                        {
                            case "Custom":
                                itemComparison1.LoadCustomChart(tag[1]);
                                break;
                            case "CustomRendered":
                                itemComparison1.LoadCustomRenderedChart(tag[1]);
                                break;
                        }
                        break;
                    }
                }
                asyncCalculation = null;
            }
        }

        void _character_ItemsChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                Invoke((EventHandler)_character_ItemsChanged, sender, e);
                //InvokeHelper.Invoke(this, "_character_ItemsChanged", new object[] { sender, e });
                return;
            }
            this.Cursor = Cursors.WaitCursor;
            if (asyncCalculation != null)
            {
                CharacterCalculationsBase oldCalcs = _calculatedStats;
                _calculatedStats = null;
                oldCalcs.CancelAsynchronousCharacterDisplayCalculation();
                asyncCalculation = null;
            }			
            _unsavedChanges = true;

            //itemButtonOffHand.Enabled = _character.MainHand == null || _character.MainHand.Slot != ItemSlot.TwoHand;
            if (!_loadingCharacter)
            {
                itemButtonBack.UpdateSelectedItem(); itemButtonChest.UpdateSelectedItem(); itemButtonFeet.UpdateSelectedItem();
                itemButtonFinger1.UpdateSelectedItem(); itemButtonFinger2.UpdateSelectedItem(); itemButtonHands.UpdateSelectedItem();
                itemButtonHead.UpdateSelectedItem(); itemButtonRanged.UpdateSelectedItem(); itemButtonLegs.UpdateSelectedItem();
                itemButtonNeck.UpdateSelectedItem(); itemButtonShirt.UpdateSelectedItem(); itemButtonShoulders.UpdateSelectedItem();
                itemButtonTabard.UpdateSelectedItem(); itemButtonTrinket1.UpdateSelectedItem(); itemButtonTrinket2.UpdateSelectedItem();
                itemButtonWaist.UpdateSelectedItem(); itemButtonMainHand.UpdateSelectedItem(); itemButtonOffHand.UpdateSelectedItem();
                itemButtonProjectile.UpdateSelectedItem(); itemButtonProjectileBag.UpdateSelectedItem(); itemButtonWrist.UpdateSelectedItem();
                //ItemEnchantsChanged();
            }

            //and the clouds above move closer / looking so dissatisfied
            Calculations.ClearCache();
            CharacterCalculationsBase calcs = Calculations.GetCharacterCalculations(Character, null, true, true, true);
            _calculatedStats = calcs;

            FormItemSelection.CurrentCalculations = calcs;
            UpdateDisplayCalculationValues(calcs.GetCharacterDisplayCalculationValues());
            if (Character.IsMetaGemActive)
                itemButtonHead.BackColor = _defaultColor;
            else
                itemButtonHead.BackColor = Color.Red;
            LoadComparisonData();
            if (calcs.RequiresAsynchronousDisplayCalculation)
            {
                asyncCalculation = AsyncOperationManager.CreateOperation(null);
                asyncCalculationStart.BeginInvoke(calcs, asyncCalculation, null, null);
            }

            this.Cursor = Cursors.Default;
            //and the ground below grew colder / as they put you down inside
        }

        public void UpdateDisplayCalculationValues(Dictionary<string, string> displayCalculationValues)
        {
            calculationDisplay1.SetCalculations(displayCalculationValues);
            string status;
            if (!displayCalculationValues.TryGetValue("Status", out status))
            {
                int i = 0;
                status = "Overall: " + Math.Round(_calculatedStats.OverallPoints);
                foreach (KeyValuePair<string, Color> kvp in Calculations.SubPointNameColors)
                {
                    status += ", " + kvp.Key + ": " + Math.Round(_calculatedStats.SubPoints[i]);
                    i++;
                }
                //status = "Rawr version " + typeof(Calculations).Assembly.GetName().Version.ToString();
            }
            toolStripStatusLabel.Text = status;
        }

        public void LoadModel(string displayName)
        {
            try
            {
                Calculations.LoadModel(Calculations.Models[displayName]);
            }
            finally
            {
                this.ConfigModel = displayName;
                Image icon = ItemIcons.GetItemIcon(Calculations.ModelIcons[displayName], true);
                if (icon != null)
                {
                    this.Icon = Icon.FromHandle((icon as Bitmap).GetHicon());
                }
            }
        }

        public string ConfigModel
        {
            get
            {
                return Calculations.ValidModel(Properties.Recent.Default.RecentModel);
            }
            set { Properties.Recent.Default.RecentModel = value; }
        }

        public Rectangle ConfigBounds
        {
            get
            {
                return new Rectangle(Properties.Recent.Default.WindowLocation,
                    Properties.Recent.Default.WindowSize);
            }
            set 
            { 
                Properties.Recent.Default.WindowLocation = value.Location; 
                Properties.Recent.Default.WindowSize = value.Size; 
            }
        }

        public enum FileType
        {
            Character,
            Batch
        }

        public FileType GetFileType(string file)
        {
            StreamReader reader = new StreamReader(file);
            reader.ReadLine(); // xml declaration
            string root = reader.ReadLine();
            if (root.StartsWith("<Character"))
            {
                return FileType.Character;
            }
            else if (root.StartsWith("<ArrayOfBatchCharacter"))
            {
                return FileType.Batch;
            }
            // otherwise assume Character, it won't load anyway
            return FileType.Character;
        }

        public string[] ConfigRecentCharacters
        {
            get
            {
                string recentCharacters = Properties.Recent.Default.RecentFiles;
                if (string.IsNullOrEmpty(recentCharacters))
                {
                    return new string[0];
                }
                else
                {
                    return recentCharacters.Split(';');
                }
            }
            set { Properties.Recent.Default.RecentFiles = string.Join(";", value); }
        }

        private delegate void AddRecentCharacterDelegate(string character);
        public void AddRecentCharacter(string character)
        {
            List<string> recentCharacters = new List<string>(ConfigRecentCharacters);
            recentCharacters.Remove(character);
            recentCharacters.Add(character);
            while (recentCharacters.Count > 8)
                recentCharacters.RemoveRange(0, recentCharacters.Count - 8);
            ConfigRecentCharacters = recentCharacters.ToArray();
            UpdateRecentCharacterMenuItems();
        }
        
        public void UpdateRecentCharacterMenuItems()
        {
            foreach (ToolStripMenuItem item in _recentCharacterMenuItems)
            {
                fileToolStripMenuItem.DropDownItems.Remove(item);
                item.Dispose();
            }
            _recentCharacterMenuItems.Clear();
            foreach (string recentCharacter in ConfigRecentCharacters)
            {
                string fileName = System.IO.Path.GetFileName(recentCharacter);
                ToolStripMenuItem recentCharacterMenuItem = new ToolStripMenuItem(fileName);
                recentCharacterMenuItem.Tag = recentCharacter;
                recentCharacterMenuItem.Click += new EventHandler(recentCharacterMenuItem_Click);
                _recentCharacterMenuItems.Add(recentCharacterMenuItem);
                fileToolStripMenuItem.DropDownItems.Insert(6, recentCharacterMenuItem);
            }
        }

        public void UpdateCustomChartMenuItems()
        {
            foreach (ToolStripMenuItem item in _customChartMenuItems)
            {
                toolStripDropDownButtonSlot.DropDownItems.Remove(item);
                item.Dispose();
            }
            _customChartMenuItems.Clear();
            foreach (string chartName in Calculations.CustomChartNames)
            {
                ToolStripMenuItem customChartMenuItem = new ToolStripMenuItem(chartName);
                customChartMenuItem.Tag = "Custom." + chartName;
                customChartMenuItem.Click += new EventHandler(slotToolStripMenuItem_Click);
                _customChartMenuItems.Add(customChartMenuItem);
                toolStripDropDownButtonSlot.DropDownItems.Add(customChartMenuItem);
            }
            foreach (string chartName in Calculations.CustomRenderedChartNames)
            {
                ToolStripMenuItem customChartMenuItem = new ToolStripMenuItem(chartName);
                customChartMenuItem.Tag = "CustomRendered." + chartName;
                customChartMenuItem.Click += new EventHandler(slotToolStripMenuItem_Click);
                _customChartMenuItems.Add(customChartMenuItem);
                toolStripDropDownButtonSlot.DropDownItems.Add(customChartMenuItem);
            }
        }

        void recentCharacterMenuItem_Click(object sender, EventArgs e)
        {
            if (PromptToSaveBeforeClosing())
            {
                LoadSavedCharacter((sender as ToolStripMenuItem).Tag.ToString());
            }
        }

        //private void modelToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    ToolStripMenuItem modelToolStripMenuItem = sender as ToolStripMenuItem;
        //    if (!modelToolStripMenuItem.Checked)
        //    {
        //        foreach (ToolStripMenuItem item in _customChartMenuItems)
        //            if (item.Checked)
        //                slotToolStripMenuItem_Click(toolStripDropDownButtonSlot.DropDownItems[1], null);

        //        foreach (ToolStripMenuItem item in (modelToolStripMenuItem.OwnerItem as ToolStripMenuItem).DropDownItems)
        //            item.Checked = item == modelToolStripMenuItem;
        //        KeyValuePair<string, Type> kvpModel = (KeyValuePair<string, Type>)modelToolStripMenuItem.Tag;
        //        Image icon = ItemIcons.GetItemIcon(Calculations.ModelIcons[kvpModel.Key], true);
        //        if (icon != null)
        //        {
        //            this.Icon = Icon.FromHandle((icon as Bitmap).GetHicon());
        //        }
        //        this.LoadModel(kvpModel.Key);
        //    }
        //}

        private void Calculations_ModelChanged(object sender, EventArgs e)
        {
            bool unsavedChanges = _unsavedChanges;

            Character.CurrentModel = null;

            UpdateCustomChartMenuItems();

            toolStripDropDownButtonSort.DropDownItems.Clear();
            toolStripDropDownButtonSort.DropDownItems.Add(overallToolStripMenuItem);
            toolStripDropDownButtonSort.DropDownItems.Add(alphabeticalToolStripMenuItem);
            foreach (string name in Calculations.SubPointNameColors.Keys)
            {
                ToolStripMenuItem toolStripMenuItemSubPoint = new ToolStripMenuItem(name);
                toolStripMenuItemSubPoint.Tag = toolStripDropDownButtonSort.DropDownItems.Count - 2;
                toolStripMenuItemSubPoint.Click += new System.EventHandler(this.sortToolStripMenuItem_Click);
                toolStripDropDownButtonSort.DropDownItems.Add(toolStripMenuItemSubPoint);
            }

            Calculations.CalculationOptionsPanel.Dock = DockStyle.Fill;
            tabPageOptions.Controls.Clear();
            tabPageOptions.Controls.Add(Calculations.CalculationOptionsPanel);
            
            itemButtonProjectile.Visible = itemButtonProjectileBag.Visible = Calculations.CanUseAmmo;
            _loadingCharacter = true; // no need to load the comparison charts for this, it's done when reloading the character
            ItemRefinement.resetLists();
            ItemCache.OnItemsChanged();
            _loadingCharacter = false;
            Character = Character; //Reload the character
            _unsavedChanges = unsavedChanges;
        }

        void FormMain_Shown(object sender, EventArgs e)
        {
            _splash.Close();
            _splash.Dispose();
            SetTitle();

            // reset filter regex
            ItemFilterRegex.RegexCompiled = true;

            // compile regex and save files in background
            ThreadPool.QueueUserWorkItem((object state) =>
            {
                Thread.Sleep(1000); // wait a bit while windows are still drawing so it doesn't look laggy

                ItemFilter.Compile();
                Buff.SaveBuffs();
                Enchant.SaveEnchants();
            });

            if (!Properties.GeneralSettings.Default.SeenRawr3Note)
            {
                MessageBox.Show(
@"We're pleased to announce that, after long last, Rawr3 has entered public beta. You're still welcome to continue using Rawr2 (that's what you're using right now), but we urge you to try out Rawr3, and enjoy all the new features and benefits. Rawr3 is a port of Rawr to Silverlight, which means:

    - You can run Rawr3 in your web browser.
    - No need to download or install anything.
    - It runs on Mac OS X (Intel). Welcome to Rawr, Mac users!
    - You can optionally install it locally with 2 clicks from the web version, if you want to have it locally for offline use.
    - Lots more.

So give Rawr3 a try today! Get started at: http://elitistjerks.com/rawr.php
Please remember that it's still a beta, though, so lots of things are likely to be buggy or incomplete!",
                "A Message from the Rawr Development Team");
                Properties.GeneralSettings.Default.SeenRawr3Note = true;
                Properties.GeneralSettings.Default.Save();
            }

            //if (Properties.Recent.Default.SeenIntroVersion < INTRO_VERSION)
            //{
            //    Properties.Recent.Default.SeenIntroVersion = INTRO_VERSION;
            //    MessageBox.Show(INTRO_TEXT);
            //}
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Character.ToString();//Load the saved character

            StatusMessaging.Ready = true;
            _timerCheckForUpdates = new System.Threading.Timer(new System.Threading.TimerCallback(_timerCheckForUpdates_Callback));
            _timerCheckForUpdates.Change(3000, 1000 * 60 * 60 * 8); //Check for updates 3 sec after the form loads, and then again every 8 hours

            if (Properties.Recent.Default.ShowStartPage)
                ShowStartPage();
        }

        private void ShowStartPage()
        {
            FormStart formStart = new FormStart(this);
            formStart.Left = this.Left + this.Width / 2 - formStart.Width / 2;
            formStart.Top = this.Top + this.Height / 2 - formStart.Height / 2;
            formStart.Show(this);
        }

        void ItemCache_ItemsChanged(object sender, EventArgs e)
        {
            // when item is deleted from item cache we have to make sure to update
            // all items needed by current character (essentially we have to prevent the item from
            // being deleted)
            if (!_loadingCharacter)
            {
                _loadingCharacter = true; // suppress item changed event
                EnsureItemsLoaded(_character.GetAllEquippedAndAvailableGearIds());
                _loadingCharacter = false;
            }
            if (this.InvokeRequired)
            {
                if (_loadingCharacter)
                {
                    Character.InvalidateItemInstances();
                }
                else
                {
                    BeginInvoke((EventHandler)ItemCache_ItemsChanged, sender, e);
                }
            }
            else
            {
                Character.InvalidateItemInstances();
                if (!_loadingCharacter)
                {
                    LoadComparisonData();
                }
            }
        }

        void refineEquipmentParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            ItemRefinement.updateBoxes();
            if (ItemRefinement.ShowDialog(this) == DialogResult.OK)
            {
                ItemFilter.Save(GetItemFilterFilePath());
            }
        }

        void defaultGemControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            FormGemmingTemplates GemControl = new FormGemmingTemplates();
            GemControl.ShowDialog(this);
            this.Cursor = Cursors.Default;
            List<GemmingTemplate> copy = new List<GemmingTemplate>(Character.CustomGemmingTemplates);
            if (GemControl.DialogResult.Equals(DialogResult.OK))
            {
                ItemCache.OnItemsChanged();
            }
            else
            {
                Character.CustomGemmingTemplates = copy;
            }
        }
        
        private void editItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormItemEditor itemEditor = new FormItemEditor(Character);
            itemEditor.ShowDialog(this);
            ItemCache.OnItemsChanged();
        }

        //{
        //    OpenItemEditor();
        //}

        //public void OpenItemEditor() { OpenItemEditor(null); }
        //public void OpenItemEditor(Item selectedItem)
        //{
        //    this.Invoke(new OpenItemEditorDel(_openItemEditor), selectedItem);
        //}

        //private delegate void OpenItemEditorDel(Item selectedItem);
        //private void _openItemEditor(Item selectedItem)
        //{
        //    FormItemEditor itemEditor = new FormItemEditor(Character);
        //    if (selectedItem != null) itemEditor.SelectItem(selectedItem, true);
        //    itemEditor.ShowDialog(this);
        //    ItemCache.OnItemsChanged();
        //}

        #region File Commands
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewCharacter();
        }

        public bool NewCharacter()
        {
            bool ret = false;
            if (PromptToSaveBeforeClosing())
            {
                _characterPath = null;
                LoadCharacterIntoForm(new Character());
                ret = true;
            }
            return ret;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenCharacter();
        }

        public bool OpenCharacter()
        {
            bool ret = false;
            if (PromptToSaveBeforeClosing())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ".xml";
                dialog.Filter = "Rawr Xml Character Files | *.xml";
                dialog.Multiselect = false;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    LoadSavedCharacter(dialog.FileName);
                    ret = true;
                }
                dialog.Dispose();
            }
            return ret;
        }

        private void LoadCharacterIntoForm(Character character)
        {
            LoadCharacterIntoForm(character, false);
        }

        private void LoadCharacterIntoForm(Character character, bool unsavedChanges)
        {
            string characterModel = character.CurrentModel;
            // if the current character is already using the target model (majority case), then we can skip this whole mess
            if (Character.CurrentModel != characterModel)
            {
                Character c = new Character();
                // set the race/class/model of target character to minimize model swaps
                c.Class = character.Class;
                c.Race = character.Race;
                c.CurrentModel = characterModel;
                // now load blank character and force a model change
                // TODO: this can probably be optimized still, don't need to do charts etc for the blank character
                Character = c;
                LoadModel(characterModel);
            }
            // now load the character without poluting it with previous model
            Character = character;
            _unsavedChanges = unsavedChanges;
            SetTitle();
            comboBoxModel.SelectedItem = characterModel;
        }

        public void BatchCharacterSaved(BatchCharacter character)
        {
            if (_batchCharacter == character)
            {
                _unsavedChanges = false;
                SetTitle();
            }
        }

        public void LoadBatchCharacter(BatchCharacter character)
        {
            if (character.Character != null)
            {
                if (_batchCharacter == null)
                {
                    _storedCharacter = _character;
                    _storedCharacterPath = _characterPath;
                    _storedUnsavedChanged = _unsavedChanges;
                }
                _batchCharacter = character;
                _characterPath = character.AbsolutePath;
                EnsureItemsLoaded(character.Character.GetAllEquippedAndAvailableGearIds());
                LoadCharacterIntoForm(character.Character, character.UnsavedChanges);
            }
        }

        public void UnloadBatchCharacter()
        {
            if (_batchCharacter != null)
            {
                _batchCharacter = null;
                _characterPath = _storedCharacterPath;
                LoadCharacterIntoForm(_storedCharacter, _storedUnsavedChanged);
                _storedCharacter = null;
            }
        }

        public void LoadSavedCharacter(string path)
        {
            if (!File.Exists(path)) {
                MessageBox.Show("That file no longer exists.\n\nRawr will now skip the remainder of the attempt to open the file",
                    "Error Opening File",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            switch (GetFileType(path))
            {
                case FileType.Character:
                    StartProcessing();
                    BackgroundWorker bw = new BackgroundWorker();
                    bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_LoadSavedCharacterComplete);
                    bw.DoWork += new DoWorkEventHandler(bw_LoadSavedCharacter);
                    bw.RunWorkerAsync(path);
                    break;
                case FileType.Batch:
                    FormBatchTools form = new FormBatchTools(this);
                    AddRecentCharacter(path);
                    form.batchTools.BatchCharacterList = BatchCharacterList.Load(path);
                    form.batchCharacterListBindingSource.DataSource = form.batchTools.BatchCharacterList;
                    form.Show();
                    break;
            }
        }

        void bw_LoadSavedCharacter(object sender, DoWorkEventArgs e)
        {
            WebRequestWrapper.ResetFatalErrorIndicator();
            StatusMessaging.UpdateStatus("Loading Character", "Loading Saved Character");
            StatusMessaging.UpdateStatus("Update Item Cache", "Queued");
            StatusMessaging.UpdateStatus("Cache Item Icons", "Queued");
            _loadingCharacter = true; // suppress item changed event
            Character character = Character.Load(e.Argument as string);
            _loadingCharacter = false;
            StatusMessaging.UpdateStatusFinished("Loading Character");
            if (character != null)
            {
                _loadingCharacter = true; // suppress item changed event
                this.EnsureItemsLoaded(character.GetAllEquippedAndAvailableGearIds());
                _loadingCharacter = false;
                _characterPath = e.Argument as string;
                Invoke((AddRecentCharacterDelegate)AddRecentCharacter, e.Argument);
                //InvokeHelper.Invoke(this, "AddRecentCharacter", new object[] { e.Argument});
                e.Result = character;
            }
        }

        void bw_LoadSavedCharacterComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null) {
                MessageBox.Show(e.Error.Message, "Error loading Saved Character file");
            } else {
                //Load Character into UI
                LoadCharacterIntoForm(e.Result as Character);
            }
            FinishedProcessing();
        }

        private void loadFromArmoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadCharacterFromArmory();
        }

        public bool LoadCharacterFromArmory()
        {
            bool ret = false;
            if (PromptToSaveBeforeClosing())
            {
                FormEnterNameRealm form = new FormEnterNameRealm();
                form.Icon = this.Icon;
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    // The removes force it to put those items at the end.
                    // So we can use that for recall later on what was last used
                    if (Rawr.Properties.Recent.Default.RecentChars.Contains(form.textBoxName.Text)) {
                        Rawr.Properties.Recent.Default.RecentChars.Remove(form.textBoxName.Text);
                    }
                    if (Rawr.Properties.Recent.Default.RecentServers.Contains(form.textBoxRealm.Text)) {
                        Rawr.Properties.Recent.Default.RecentServers.Remove(form.textBoxRealm.Text);
                    }
                    Rawr.Properties.Recent.Default.RecentChars.Add(form.textBoxName.Text);
                    Rawr.Properties.Recent.Default.RecentServers.Add(form.textBoxRealm.Text);
                    Rawr.Properties.Recent.Default.RecentRegion = form.comboBoxRegion.Text;
                    //
                    StartProcessing();
                    BackgroundWorker bw = new BackgroundWorker();
                    bw.DoWork += new DoWorkEventHandler(bw_ArmoryGetCharacter);
                    bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_ArmoryGetCharacterComplete);
                    bw.RunWorkerAsync(new string[] { form.CharacterName, form.Realm, form.ArmoryRegion.ToString() });
                    ret = true;
                }
                form.Dispose();
            }
            return ret;
        }

        void bw_ArmoryGetCharacter(object sender, DoWorkEventArgs e)
        {
            string[] args = e.Argument as string[];
            //just accessing the UI elements from off thread is ok, its changing them thats bad.
            CharacterRegion region = (CharacterRegion)Enum.Parse(typeof(CharacterRegion),args[2]);
            e.Result = this.GetCharacterFromArmory(args[1], args[0], region);
            _characterPath = "";  
        }

        void bw_ArmoryGetCharacterComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // TODO: log this to the status screen.
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Result != null)
            {
                Character character = e.Result as Character;
                LoadCharacterIntoForm(character);
                _unsavedChanges = true;
            }
            FinishedProcessing();
        }

        private void reloadCurrentCharacterFromArmoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CharacterRegion region = (CharacterRegion)Enum.Parse(typeof(CharacterRegion), comboBoxRegion.SelectedItem.ToString());
            if (String.IsNullOrEmpty(Character.Name) || String.IsNullOrEmpty(Character.Realm))
            {
                MessageBox.Show("A valid character has not been loaded, unable to reload.", 
                    "No Character Loaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (MessageBox.Show("Confirm reloading " + textBoxName.Text + " from the " + textBoxRealm.Text + "@" + region + " realm ",
                "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                StartProcessing();
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(bw_ArmoryReloadCharacter);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_ArmoryGetCharacterReloadComplete);
                bw.RunWorkerAsync(Character);
            }
        }

        void bw_ArmoryReloadCharacter(object sender, DoWorkEventArgs e)
        {
            Character character = e.Argument as Character;
            e.Result = this.ReloadCharacterFromArmory(character);
        }

        void bw_ArmoryGetCharacterReloadComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // TODO: log this to the status screen.
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                Character character = e.Result as Character;
                this.Character = character;
                _unsavedChanges = true;
            }
            FinishedProcessing();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_characterPath))
            {
                this.Cursor = Cursors.WaitCursor;
                Character.Save(_characterPath);
                if (_batchCharacter != null)
                {
                    _batchCharacter.UnsavedChanges = false;
                }
                _unsavedChanges = false;
                AddRecentCharacter(_characterPath);
                SetTitle();
                this.Cursor = Cursors.Default;
            }
            else
            {
                saveAsToolStripMenuItem_Click(null, null);
            }            
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".xml";
            dialog.Filter = "Rawr Xml Character Files | *.xml";
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                Character.Save(dialog.FileName);
                _characterPath = dialog.FileName;
                _unsavedChanges = false;
                AddRecentCharacter(_characterPath);
                SetTitle();
                this.Cursor = Cursors.Default;
            }
            dialog.Dispose();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void SaveSettingsAndCaches()
        {
            try
            {
                Properties.Recent.Default.Save();
                ItemCache.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Rawr: " + ex.Message );
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !PromptToSaveBeforeClosing();
            ConfigBounds = this.Bounds;
            SaveSettingsAndCaches();
        }

        private bool PromptToSaveBeforeClosing()
        {
            if (_unsavedChanges)
            {
                DialogResult result = MessageBox.Show("Would you like to save the current character before closing it?", "Rawr - Save?", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.Yes:
                        saveToolStripMenuItem_Click(null, null);
                        return !string.IsNullOrEmpty(_characterPath);
                    case DialogResult.No:
                        return true;
                    default:
                        return false;
                }
            }
            else
                return true;
        }
        #endregion

        private void comboBoxEnchant_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (!_loadingCharacter)
            //{   //If I was in World War II, they'd call me S-
            //    Character.BackEnchant = comboBoxEnchantBack.SelectedItem as Enchant;
            //    Character.ChestEnchant = comboBoxEnchantChest.SelectedItem as Enchant;
            //    Character.FeetEnchant = comboBoxEnchantFeet.SelectedItem as Enchant;
            //    Character.Finger1Enchant = comboBoxEnchantFinger1.SelectedItem as Enchant;
            //    Character.Finger2Enchant = comboBoxEnchantFinger2.SelectedItem as Enchant;
            //    Character.HandsEnchant = comboBoxEnchantHands.SelectedItem as Enchant;
            //    Character.HeadEnchant = comboBoxEnchantHead.SelectedItem as Enchant;
            //    Character.LegsEnchant = comboBoxEnchantLegs.SelectedItem as Enchant;
            //    Character.ShouldersEnchant = comboBoxEnchantShoulders.SelectedItem as Enchant;
            //    Character.MainHandEnchant = comboBoxEnchantMainHand.SelectedItem as Enchant;
            //    Character.OffHandEnchant = comboBoxEnchantOffHand.SelectedItem as Enchant;
            //    Character.RangedEnchant = comboBoxEnchantRanged.SelectedItem as Enchant;
            //    Character.WristEnchant = comboBoxEnchantWrists.SelectedItem as Enchant;
            //    Character.OnCalculationsInvalidated();
            //}   //...Fire!
        }

        private void comboBoxRace_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loadingCharacter)
            {
                Character.Race = (CharacterRace)Enum.Parse(typeof(CharacterRace), comboBoxRace.Text);
                Character.OnCalculationsInvalidated();
            }
        }

        private void comboBoxRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loadingCharacter)
            {
                Character.Region = (CharacterRegion)Enum.Parse(typeof(CharacterRegion), comboBoxRegion.Text);
                Character.OnCalculationsInvalidated();
            }
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            if (!_loadingCharacter)
            {
                Character.Name = textBoxName.Text;
                _unsavedChanges = true;
            }
        }

        private void textBoxRealm_TextChanged(object sender, EventArgs e)
        {
            if (!_loadingCharacter)
            {
                Character.Realm = textBoxRealm.Text;
                _unsavedChanges = true;
            }
        }

        private void comboBoxProfession1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loadingCharacter)
            {
                Character.PrimaryProfession = Profs.StringToProfession(comboBoxProfession1.Text);
                Character.OnCalculationsInvalidated();
                UpdateProfessionControls();
                _unsavedChanges = true;
            }
        }

        private void comboBoxProfession2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loadingCharacter)
            {
                Character.SecondaryProfession = Profs.StringToProfession(comboBoxProfession2.Text);
                Character.OnCalculationsInvalidated();
                UpdateProfessionControls();
                _unsavedChanges = true;
            }
        }

        void GeneralSettings_HideProfessionsChanged(object sender, EventArgs e)
        {
            UpdateProfessionControls();
        }

        /// <summary>
        /// This routine updates controls on the main form based on the character's professions
        /// eg: whether to allow or disallow Blacksmithing sockets 
        /// only acts if global option to hide professions is enabled
        /// </summary>
        private void UpdateProfessionControls()
        {
            // set defaults
            if (!_loadingCharacter)
            {
                checkBoxHandsBlacksmithingSocket.Enabled = true;
                checkBoxWristBlacksmithingSocket.Enabled = true;
                if (Rawr.Properties.GeneralSettings.Default.HideProfEnchants)
                {
                    if (!Character.HasProfession(Profession.Blacksmithing))
                    {
                        checkBoxHandsBlacksmithingSocket.Enabled = false;
                        checkBoxWristBlacksmithingSocket.Enabled = false;
                        checkBoxHandsBlacksmithingSocket.Checked = false;
                        checkBoxWristBlacksmithingSocket.Checked = false;
                    }
                    // any other profession checks go here
                }
                Buff.InvalidateBuffs(); // forces rebuild of buff list
                buffSelector1.RebuildControls();
            }
        }
        
        private void copyCharacterStatsToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int slotNum in Enum.GetValues(typeof(CharacterSlot)))
            {
                CharacterSlot slot = (CharacterSlot)(slotNum);
                ItemInstance item = Character[slot];
                if (item != null)
                {
                    sb.AppendFormat("{0}\t{1}", slot,item.Item.Name);
                    sb.AppendLine();
                }
            }

            sb.AppendLine(Calculations.GetCharacterStatsString(Character));

            try
            {
                Clipboard.SetText(sb.ToString(), TextDataFormat.Text);
            }
            catch { }
            if (Type.GetType("Mono.Runtime") != null)
            {
                //Clipboard isn't working
                System.IO.File.WriteAllText("stats.txt", sb.ToString());
                MessageBox.Show("Mono doesn't support modifying the clipboard, so stats have been saved to a 'stats.txt' file instead.");
            }
        }

        private void slotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            ToolStripMenuItem clickedMenuItem = (ToolStripMenuItem)sender;
            string selectedTag = clickedMenuItem.Tag.ToString();

            if (gearToolStripMenuItem.DropDownItems.Contains(clickedMenuItem) ||
                toolStripDropDownButtonSubSlotGear.DropDownItems.Contains(clickedMenuItem) ||
                clickedMenuItem == gearToolStripMenuItem)
            {
                gearToolStripMenuItem.Tag = selectedTag;
                if (clickedMenuItem != gearToolStripMenuItem)
                {
                    gearToolStripMenuItem.Text = "Gear > " + clickedMenuItem.Text;
                    toolStripDropDownButtonSubSlotGear.Text = clickedMenuItem.Text;
                }

                toolStripDropDownButtonSlot.Text = "Slot: Gear";
                toolStripDropDownButtonSubSlotGear.Visible = true;
                toolStripDropDownButtonSubSlotEnchants.Visible = toolStripDropDownButtonSubSlotGems.Visible =
                    toolStripDropDownButtonSubSlotBuffs.Visible = false;
            }
            else if (enchantsToolStripMenuItem.DropDownItems.Contains(clickedMenuItem) ||
                toolStripDropDownButtonSubSlotEnchants.DropDownItems.Contains(clickedMenuItem) ||
                clickedMenuItem == enchantsToolStripMenuItem)
            {
                enchantsToolStripMenuItem.Tag = selectedTag;
                if (clickedMenuItem != enchantsToolStripMenuItem)
                {
                    enchantsToolStripMenuItem.Text = "Enchants > " + clickedMenuItem.Text;
                    toolStripDropDownButtonSubSlotEnchants.Text = clickedMenuItem.Text;
                }

                toolStripDropDownButtonSlot.Text = "Slot: Enchants";
                toolStripDropDownButtonSubSlotEnchants.Visible = true;
                toolStripDropDownButtonSubSlotGear.Visible = toolStripDropDownButtonSubSlotGems.Visible =
                    toolStripDropDownButtonSubSlotBuffs.Visible = false;
            }
            else if (gemsToolStripMenuItem.DropDownItems.Contains(clickedMenuItem) ||
                normalToolStripMenuItem.DropDownItems.Contains(clickedMenuItem) || 
                toolStripDropDownButtonSubSlotGems.DropDownItems.Contains(clickedMenuItem) ||
                toolStripMenuItemNormalGems.DropDownItems.Contains(clickedMenuItem) ||
                clickedMenuItem == gemsToolStripMenuItem)
            {
                gemsToolStripMenuItem.Tag = selectedTag;
                if (clickedMenuItem != gemsToolStripMenuItem)
                {
                    gemsToolStripMenuItem.Text = "Gems > " + clickedMenuItem.Text;
                    toolStripDropDownButtonSubSlotGems.Text = clickedMenuItem.Text;
                }

                toolStripDropDownButtonSlot.Text = "Slot: Gems";
                toolStripDropDownButtonSubSlotGems.Visible = true;
                toolStripDropDownButtonSubSlotGear.Visible = toolStripDropDownButtonSubSlotEnchants.Visible =
                    toolStripDropDownButtonSubSlotBuffs.Visible = false;
            }
            else if (buffsToolStripMenuItem.DropDownItems.Contains(clickedMenuItem) ||
                toolStripDropDownButtonSubSlotBuffs.DropDownItems.Contains(clickedMenuItem) ||
                clickedMenuItem == buffsToolStripMenuItem)
            {
                buffsToolStripMenuItem.Tag = selectedTag;
                if (clickedMenuItem != buffsToolStripMenuItem)
                {
                    buffsToolStripMenuItem.Text = "Buffs > " + clickedMenuItem.Text;
                    toolStripDropDownButtonSubSlotBuffs.Text = clickedMenuItem.Text;
                }

                toolStripDropDownButtonSlot.Text = "Slot: Buffs";
                toolStripDropDownButtonSubSlotBuffs.Visible = true;
                toolStripDropDownButtonSubSlotGear.Visible = toolStripDropDownButtonSubSlotEnchants.Visible =
                    toolStripDropDownButtonSubSlotGems.Visible = false;
            }
            else
            {
                toolStripDropDownButtonSlot.Text = "Slot: " + clickedMenuItem.Text;
                toolStripDropDownButtonSubSlotGear.Visible = toolStripDropDownButtonSubSlotEnchants.Visible = 
                    toolStripDropDownButtonSubSlotGems.Visible = toolStripDropDownButtonSubSlotBuffs.Visible = false;
            }

            foreach (ToolStripItem item in toolStripDropDownButtonSlot.DropDownItems)
            {
                ToolStripMenuItem menuItem = item as ToolStripMenuItem;
                if (menuItem != null)
                {
                    menuItem.Checked = (string)menuItem.Tag == selectedTag;
                    //if (menuItem.Checked)
                    //{
                    //    toolStripDropDownButtonSlot.Text = "Slot: " + menuItem.Text;
                    //}
                }
            }
            foreach (ToolStripMenuItem menuItem in gearToolStripMenuItem.DropDownItems)
                menuItem.Checked = (string)menuItem.Tag == selectedTag;
            foreach (ToolStripMenuItem menuItem in enchantsToolStripMenuItem.DropDownItems)
                menuItem.Checked = (string)menuItem.Tag == selectedTag;
            foreach (ToolStripMenuItem menuItem in gemsToolStripMenuItem.DropDownItems)
                menuItem.Checked = (string)menuItem.Tag == selectedTag;
            foreach (ToolStripMenuItem menuItem in buffsToolStripMenuItem.DropDownItems)
                menuItem.Checked = (string)menuItem.Tag == selectedTag;
            foreach (ToolStripMenuItem menuItem in toolStripDropDownButtonSubSlotGear.DropDownItems)
                menuItem.Checked = (string)menuItem.Tag == selectedTag;
            foreach (ToolStripMenuItem menuItem in toolStripDropDownButtonSubSlotEnchants.DropDownItems)
                menuItem.Checked = (string)menuItem.Tag == selectedTag;
            foreach (ToolStripMenuItem menuItem in toolStripDropDownButtonSubSlotGems.DropDownItems)
                menuItem.Checked = (string)menuItem.Tag == selectedTag;
            foreach (ToolStripMenuItem menuItem in toolStripDropDownButtonSubSlotBuffs.DropDownItems)
                menuItem.Checked = (string)menuItem.Tag == selectedTag;

            toolStripDropDownButtonSlot.DropDown.Close(ToolStripDropDownCloseReason.ItemClicked);

            if (!_loadingCharacter)
                LoadComparisonData(selectedTag);
            this.Cursor = Cursors.Default;
        }

        private string _currentChartTag = "Gear.Head";
        private void LoadComparisonData() { LoadComparisonData(_currentChartTag); }
        private void LoadComparisonData(string chartTag)
        {
            _currentChartTag = chartTag;
            itemComparison1.DisplayMode = ComparisonGraph.GraphDisplayMode.Subpoints;
            string[] tag = chartTag.Split('.');
            copyPawnStringToClipboardToolStripMenuItem.Visible = viewUpgradesOnLootRankToolStripMenuItem.Visible =
                viewUpgradesOnWowheadToolStripMenuItem.Visible = labelRelativeStatValuesWarning.Visible =
                tag[0] == "Relative Stat Values";
            panelStatGraph.Visible = tag[0] == "Stat Graph";
            copyEnhSimConfigToClipboardToolStripMenuItem.Visible = _character.CurrentModel == "Enhance";
            switch (tag[0])
            {
                case "Gear":
                    itemComparison1.LoadGearBySlot((CharacterSlot)Enum.Parse(typeof(CharacterSlot), tag[1]));
                    break;
 
                case "Gems":
                    itemComparison1.LoadGearBySlot((CharacterSlot)Enum.Parse(typeof(CharacterSlot), tag[1]), (ItemSlot)Enum.Parse(typeof(ItemSlot), tag[2]));
                    break;

                case "Enchants":
                    itemComparison1.LoadEnchantsBySlot((ItemSlot)Enum.Parse(typeof(ItemSlot), tag[1]), _calculatedStats);
                    break;

                case "Buffs":
                    itemComparison1.LoadBuffs(_calculatedStats, tag[1]);
                    break;

                case "Current Gear/Enchants/Buffs":
                    itemComparison1.LoadCurrentGearEnchantsBuffs(_calculatedStats);
                    break;

                case "Direct Upgrades":
                    itemComparison1.LoadAvailableGear(_calculatedStats, false);
                    break;

                case "Direct Upgrades / Cost":
                    itemComparison1.LoadAvailableGear(_calculatedStats, true);
                    break;

                case "Talent Specs":
                    itemComparison1.LoadTalentSpecs(talentPicker1);
                    break;

                case "Talents":
                    itemComparison1.LoadTalents();
                    break;

                case "Glyphs":
                    itemComparison1.LoadGlyphs();
                    break;

                case "Relative Stat Values":
                    itemComparison1.LoadRelativeStatValues();
                    break;

                case "Stat Graph":
                    itemComparison1.LoadCustomRenderedChart(tag[0]);
                    break;

                case "Custom":
                    itemComparison1.LoadCustomChart(tag[1]);
                    break;

                case "CustomRendered":
                    itemComparison1.LoadCustomRenderedChart(tag[1]);
                    break;
            }
        }

        private void sortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            ComparisonGraph.ComparisonSort sort = ComparisonGraph.ComparisonSort.Overall;
            foreach (ToolStripItem item in toolStripDropDownButtonSort.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                {
                    (item as ToolStripMenuItem).Checked = item == sender;
                    if ((item as ToolStripMenuItem).Checked)
                    {
                        toolStripDropDownButtonSort.Text = "Sort: " + item.Text;
                        sort = (ComparisonGraph.ComparisonSort)((int)item.Tag);
                    }
                }
            }
            itemComparison1.Sort = sort;
            this.Cursor = Cursors.Default;
        }

        public void RunPossibleUpgradesFromArmory( CharacterSlot slot )
        {
            StartProcessing();
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_GetArmoryUpgrades);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_StatusCompleted);
            bw.RunWorkerAsync(slot);
        }

        private void loadPossibleUpgradesFromArmoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunPossibleUpgradesFromArmory(CharacterSlot.None);
        }

        void bw_GetArmoryUpgrades(object sender, DoWorkEventArgs e)
        {
            this.GetArmoryUpgrades( (CharacterSlot)e.Argument );
        }

        public void RunPossibleUpgradesFromWowhead(CharacterSlot slot)
        {
            StartProcessing();
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_GetWowheadUpgrades);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_StatusCompleted);
            bw.RunWorkerAsync(slot);
        }

        private void loadPossibleUpgradesFromWowheadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunPossibleUpgradesFromWowhead(CharacterSlot.None);
        }

        void bw_GetWowheadUpgrades(object sender, DoWorkEventArgs e)
        {
            this.GetWowheadUpgrades((CharacterSlot)e.Argument, usePTRDataToolStripMenuItem.Checked);
        }

        public bool IsUsingPTR()
        {
            return usePTRDataToolStripMenuItem.Checked;
        }

        private void StartProcessing()
        {
            Cursor = Cursors.WaitCursor;
            if (_statusForm == null || _statusForm.IsDisposed)
            {
                _statusForm = new Status();
            }
            _statusForm.Show(this);
            menuStripMain.Enabled = false;
            ItemContextualMenu.Instance.Enabled = false;
            FormItemSelection.Enabled = false;
        }

        private void FinishedProcessing()
        {
            if (_statusForm != null && !_statusForm.IsDisposed)
            {
                _statusForm.AllowedToClose = true;
                if (_statusForm.HasErrors)
                {
                    _statusForm.SwitchToErrorTab();
                }
                else
                {
                    _statusForm.Close();
                    _statusForm.Dispose();  
                }
            }
            ItemContextualMenu.Instance.Enabled = true;
            menuStripMain.Enabled = true;
            FormItemSelection.Enabled = true;
            this.Cursor = Cursors.Default;
        }

        public void RunItemCacheArmoryUpdate(CharacterSlot slot)
        {
            if (slot != CharacterSlot.None || ConfirmUpdateItemCache())
            {
                StartProcessing();
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(bw_UpdateItemCacheArmory);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_StatusCompleted);
                bw.RunWorkerAsync( slot );
            }
        }

        private void updateItemCacheArmoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunItemCacheArmoryUpdate(CharacterSlot.None);
        }

        private bool ConfirmUpdateItemCache()
        {
            return MessageBox.Show("Are you sure you would like to update the item cache? This process takes significant time, and the default item cache is fully updated as of the time of release. This does not add any new items, it only updates the data about items already in your itemcache.",
                "Update Item Cache?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        void bw_UpdateItemCacheArmory(object sender, DoWorkEventArgs e)
        {
            // check for slot parameter
            var slot = (e.Argument != null && e.Argument is CharacterSlot
                                      ? (CharacterSlot) e.Argument
                                      : CharacterSlot.None);

            // fire 
            this.UpdateItemCacheArmory(slot, (ModifierKeys & Keys.ShiftKey) != 0 );
        }

        public void RunItemCacheWowheadUpdate(CharacterSlot slot)
        {
            if (slot != CharacterSlot.None || ConfirmUpdateItemCache())
            {
                StartProcessing();
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(bw_UpdateItemCacheWowhead);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_StatusCompleted);
                bw.RunWorkerAsync( slot );
            }
        }

        private void updateItemCacheWowheadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunItemCacheWowheadUpdate( CharacterSlot.None );
        }

        void bw_UpdateItemCacheWowhead(object sender, DoWorkEventArgs e)
        {
            // check for slot parameter
            var slot = (e.Argument != null && e.Argument is CharacterSlot
                                      ? (CharacterSlot)e.Argument
                                      : CharacterSlot.None);

            // fire 
            this.UpdateItemCacheWowhead(slot, (ModifierKeys & Keys.Shift) != 0 );
        }

        void bw_ImportWowheadFilter(object sender, DoWorkEventArgs e)
        {
            this.ImportWowheadFilter((string)e.Argument, usePTRDataToolStripMenuItem.Checked);
        }

        void bw_StatusCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Error processing request: " + e.Error.Message);
            }
            FinishedProcessing();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options options = new Options();
            options.ShowDialog(this);
            options.Dispose();
        }

        private void optimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormOptimize optimize = new FormOptimize(Character);
            optimize.ShowDialog(this);
            if (optimize.ShowUpgradeComparison)
            {
                FormUpgradeComparison.Instance.Show();
                FormUpgradeComparison.Instance.BringToFront();
            }
            optimize.Dispose();
        }

        public static string UpdateCacheStatusKey( CharacterSlot slot, bool bWowhead )
        {
            return string.Format("Update {0} from {1}",
                                 slot == CharacterSlot.None ? "All Items" : slot.ToString(),
                                 bWowhead ? "Wowhead" : "Armory"
                );
        }

        public void UpdateItemCacheArmory( CharacterSlot slot, bool bOnlyNonLocalized )
        {
            WebRequestWrapper.ResetFatalErrorIndicator();
            StatusMessaging.UpdateStatus(UpdateCacheStatusKey(slot, false), "Beginning Update");
            StatusMessaging.UpdateStatus("Cache Item Icons", "Not Started");
            StringBuilder sbChanges = new StringBuilder();
            ItemUpdater updater = new ItemUpdater(Rawr.Properties.GeneralSettings.Default.UseMultithreading, true, false, 1, UpgradeCancelPending );
            int skippedItems = 0;

            // get list of the items to be updated
            Item[] allItems = ItemsForUpdate(slot);

            // an index of added items
            var addedItems = 0;

            foreach (Item item in allItems)
            {
                if (item.Id < 90000)
                {
                    if (!bOnlyNonLocalized || string.IsNullOrEmpty(item.LocalizedName))
                    {
                        updater.AddItem(addedItems++, item);
                    }
                }
                else
                {
                    skippedItems++;
                }
            }

            updater.FinishAdding();

            while (!updater.Done)
            {
                StatusMessaging.UpdateStatus(UpdateCacheStatusKey(slot, false), "Updating " + (skippedItems + updater.ItemsDone) + " of " + updater.ItemsToDo + " items");
                Thread.Sleep(1000);
            }

            for (int i = 0; i < allItems.Length; i++)
            {
                Item item = allItems[i];
                Item newItem = updater[i];
                
                if (item.Id < 90000 && newItem != null)
                {
                    string before = item.Stats.ToString();
                    string after = newItem.Stats.ToString();
                    if (before != after)
                    {
                        sbChanges.AppendFormat("[{0}] {1}\r\n", item.Id, item.Name);
                        sbChanges.AppendFormat("BEFORE: {0}\r\n", before);
                        sbChanges.AppendFormat("AFTER: {0}\r\n\r\n", after);
                    }
                }
            }
#if DEBUG
            if (sbChanges.Length > 0)
            {
                ScrollableMessageBox msgBox = new ScrollableMessageBox();
                msgBox.Show(sbChanges.ToString());
            }
#endif
            StatusMessaging.UpdateStatusFinished(UpdateCacheStatusKey(slot, false));
            ItemIcons.CacheAllIcons(ItemCache.AllItems);
            ItemCache.OnItemsChanged();
            _character.InvalidateItemInstances();

            // save stuff
            SaveSettingsAndCaches();
        }

        public Item[] ItemsForUpdate( CharacterSlot slot )
        {
            // unknown? update everything
            if (slot == CharacterSlot.None)
                return ItemCache.AllItems;

            // get relevant items
            var list = Character.GetRelevantItems(slot);

            // fix the currently equipped: it might be non-relevant...
            var equipped = Character[slot];
            if (equipped != null && equipped.Item != null)
            {
                // check if it is
                if ( !list.Contains(equipped.Item) )
                    list.Add( equipped.Item );
            }

            // retval
            return list.ToArray();
        }

        public void UpdateItemCacheWowhead(CharacterSlot slot, bool bOnlyNonLocalized )
        {
            WebRequestWrapper.ResetFatalErrorIndicator();
            StatusMessaging.UpdateStatus(UpdateCacheStatusKey(slot, true), "Beginning Update");
            StatusMessaging.UpdateStatus("Cache Item Icons", "Not Started");
            StringBuilder sbChanges = new StringBuilder();

            bool multithreaded = Rawr.Properties.GeneralSettings.Default.UseMultithreading;
            ItemUpdater updater = new ItemUpdater(multithreaded, false, usePTRDataToolStripMenuItem.Checked, 20, UpgradeCancelPending);
            int skippedItems = 0;

            // get list of the items to be updated
            var allItems = ItemsForUpdate( slot ); 

            // an index of added items
            var addedItems = 0;

            foreach (Item item in allItems )
            {
                if (item.Id < 90000)
                {
                    if (!bOnlyNonLocalized || string.IsNullOrEmpty(item.LocalizedName))
                    {
                        updater.AddItem(addedItems++, item);
                        if (!multithreaded)
                        {
                            StatusMessaging.UpdateStatus(UpdateCacheStatusKey(slot, true), "Updating " + (skippedItems + addedItems) + " of " + allItems.Length + " items");
                            if (UpgradeCancelPending())
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    skippedItems++;
                }
            }

            updater.FinishAdding();

            while (!updater.Done)
            {
                StatusMessaging.UpdateStatus(UpdateCacheStatusKey(slot, true), "Updating " + (skippedItems + updater.ItemsDone) + " of " + updater.ItemsToDo + " items");
                Thread.Sleep(1000);
            }
            
            for (int i = 0; i < allItems.Length; i++)
            {
                Item item = allItems[i];
                Item newItem = updater[i];
                
                if (item.Id < 90000 && newItem != null)
                {
                    string before = item.Stats.ToString();
                    string after = newItem.Stats.ToString();
                    if (before != after)
                    {
                        sbChanges.AppendFormat("[{0}] {1}\r\n", item.Id, item.Name);
                        sbChanges.AppendFormat("BEFORE: {0}\r\n", before);
                        sbChanges.AppendFormat("AFTER: {0}\r\n\r\n", after);
                    }
                }
            }
#if DEBUG
            if (sbChanges.Length > 0)
            {
                ScrollableMessageBox msgBox = new ScrollableMessageBox();
                msgBox.Show(sbChanges.ToString());
            }
#endif
            StatusMessaging.UpdateStatusFinished(UpdateCacheStatusKey(slot, true));
            ItemIcons.CacheAllIcons(ItemCache.AllItems);
            ItemCache.OnItemsChanged();
            _character.InvalidateItemInstances();

            // save stuff
            SaveSettingsAndCaches();
        }

        public void ImportWowheadFilter(string filter, bool usePTR)
        {
            WebRequestWrapper.ResetFatalErrorIndicator();
            StatusMessaging.UpdateStatus("ImportWowheadFilter", "Importing Items From Wowhead");
            Wowhead.ImportItemsFromWowhead(filter, usePTR);
            ItemCache.OnItemsChanged();
            StatusMessaging.UpdateStatusFinished("ImportWowheadFilter");
        }

        public void GetArmoryUpgrades( CharacterSlot slot )
        {
            WebRequestWrapper.ResetFatalErrorIndicator();
            StatusMessaging.UpdateStatus(LoadUpgradesStatusKey(slot, false), "Getting Armory Updates");
            Armory.LoadUpgradesFromArmory( Character, slot, UpgradeCancelPending );
            ItemCache.OnItemsChanged();
            StatusMessaging.UpdateStatusFinished(LoadUpgradesStatusKey(slot, false));
        }

        private bool UpgradeCancelPending()
        {
            return Status != null && Status.CancelPending;
        }

        public static string LoadUpgradesStatusKey(CharacterSlot slot, bool bWowhead)
        {
            return string.Format("Loading {0} Upgrades from {1}",
                                 slot == CharacterSlot.None ? "All Items" : slot.ToString(),
                                 bWowhead ? "Wowhead" : "Armory"
                );
        }

        public void GetWowheadUpgrades(CharacterSlot slot, bool usePTR)
        {
            WebRequestWrapper.ResetFatalErrorIndicator();
            StatusMessaging.UpdateStatus(LoadUpgradesStatusKey(slot,true), "Getting Wowhead Updates");
            Wowhead.LoadUpgradesFromWowhead(Character, slot, usePTR, UpgradeCancelPending);
            ItemCache.OnItemsChanged();
            StatusMessaging.UpdateStatusFinished(LoadUpgradesStatusKey(slot, true));
        }

        public Character ReloadCharacterFromArmory(Character character)
        {
            WebRequestWrapper.ResetFatalErrorIndicator();
            Character reload = GetCharacterFromArmory(character.Realm, character.Name, character.Region);
            if (reload != null)
            {
                this.Invoke(new ReloadCharacterFromArmoryUpdateDelegate(this.ReloadCharacterFromCharacterProfilerUpdate), character, reload);
            }
            return character;
        }

        public delegate void ReloadCharacterFromArmoryUpdateDelegate(Character character, Character reload);
        public void ReloadCharacterFromArmoryUpdate(Character character, Character reload)
        {
            //load values for gear from armory into original character
            character.SetItems(reload, true, true);
            character.AssignAllTalentsFromCharacter(reload, false);
        }

        public Character GetCharacterFromArmory(string realm, string name, CharacterRegion region)
        {
            WebRequestWrapper.ResetFatalErrorIndicator();
            StatusMessaging.UpdateStatus("Get Character From Armory", " Downloading Character Definition");
            StatusMessaging.UpdateStatus("Update Item Cache", "Queued");
            StatusMessaging.UpdateStatus("Cache Item Icons", "Queued");
            string[] itemsOnChar;
            Character character = Armory.GetCharacter(region, realm, name, out itemsOnChar);
            StatusMessaging.UpdateStatusFinished("Get Character From Armory");
            if (itemsOnChar != null) {
                _loadingCharacter = true; // suppress item changed event
                EnsureItemsLoaded(itemsOnChar);
                _loadingCharacter = false;
            } else {
                StatusMessaging.UpdateStatusFinished("Update Item Cache");
                StatusMessaging.UpdateStatusFinished("Cache Item Icons");

            }
            return character;
        }

        private void EnsureItemsLoaded(string[] ids)
        {
            List<Item> items = new List<Item>();
            bool cacheChanged = false;
            for (int i = 0; i < ids.Length; i++)
            {
                StatusMessaging.UpdateStatus("Update Item Cache", string.Format("Checking Item Cache for Definitions - {0} of {1}", i, ids.Length));
                string id = ids[i];
                if (id != null)
                {
                    if (id.IndexOf('.') < 0 && ItemCache.ContainsItemId(int.Parse(id))) continue;                    
                    string[] s = id.Split('.');
                    Item newItem = null;
                    if (!ItemCache.ContainsItemId(int.Parse(s[0])))
                    {
                        newItem = Item.LoadFromId(int.Parse(s[0]), false, false, false);
                        cacheChanged = true;
                    }
                    if (s.Length >= 4)
                    {
                        Item gem;
                        for (int g = 1; g <= 3; g++)
                        {
                            if (s[g] != "*" && s[g] != "0")
                            {
                                if (!ItemCache.ContainsItemId(int.Parse(s[g])))
                                {
                                    gem = Item.LoadFromId(int.Parse(s[g]), false, false, false);
                                    cacheChanged = true;
                                }
                            }
                        }
                    }
                    if (newItem != null)
                    {
                        items.Add(newItem);
                    }
                }
            }
            StatusMessaging.UpdateStatusFinished("Update Item Cache");
            ItemIcons.CacheAllIcons(items.ToArray());
            if (cacheChanged)
            {
                ItemCache.OnItemsChanged();
            }
        }

        private void BatchToolsToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            FormBatchTools form = new FormBatchTools(this);
            form.Show();            
        }

        private void loadFromCharacterProfilerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PromptToSaveBeforeClosing())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ".lua";
                dialog.Filter = "Character Profiler Saved Variables Files | *.lua";
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        CharacterProfilerData characterList = new CharacterProfilerData(dialog.FileName);

                        FormChooseCharacter form = new FormChooseCharacter(characterList);

                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            StartProcessing();
                            BackgroundWorker bw = new BackgroundWorker();
                            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_LoadCharacterProfilerComplete);
                            bw.DoWork += new DoWorkEventHandler(bw_LoadCharacterProfiler);
                            bw.RunWorkerAsync(form.Character);
                        }

                        form.Dispose();
                    }
                    catch (InvalidDataException ex)
                    {
                        MessageBox.Show("Unable to parse saved variable file: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading saved variable file: " + ex.Message);
                    }
                }
                dialog.Dispose();
            }
        }

        void bw_LoadCharacterProfiler(object sender, DoWorkEventArgs e)
        {
            WebRequestWrapper.ResetFatalErrorIndicator();
            StatusMessaging.UpdateStatus("Loading Character", "Loading Saved Character");
            StatusMessaging.UpdateStatus("Update Item Cache", "Queued");
            StatusMessaging.UpdateStatus("Cache Item Icons", "Queued");
            CharacterProfilerCharacter characterProfilerChoice = e.Argument as CharacterProfilerCharacter;
            characterProfilerChoice.loadFromInfo();
            StatusMessaging.UpdateStatusFinished("Loading Character");
            if (characterProfilerChoice != null)
            {
                _loadingCharacter = true; // suppress item changed event
                this.EnsureItemsLoaded(characterProfilerChoice.Character.GetAllEquippedAndAvailableGearIds());
                _loadingCharacter = false;
                _characterPath = null;
                e.Result = characterProfilerChoice.Character;
            }
        }

        void bw_LoadCharacterProfilerComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                //Load Character into UI
                LoadCharacterIntoForm(e.Result as Character);
                FinishedProcessing();
            }
        }

        private void comboBoxModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            LoadModel((string)comboBoxModel.SelectedItem);
            this.Cursor = Cursors.Default;
        }

        private void comboBoxClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loadingCharacter && _character != null)
            {
                this.Cursor = Cursors.WaitCursor;
                Character.Class = (CharacterClass)Enum.Parse(typeof(CharacterClass), comboBoxClass.Text);
                this.Cursor = Cursors.Default;
            }
        }

        private void checkBoxEnforceGemRequirements_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loadingCharacter && _character != null)
            {
                Character.EnforceGemRequirements = checkBoxEnforceGemRequirements.Checked;
                Character.OnCalculationsInvalidated();
            }
        }

        private void toolStripMenuItemItemFilterEditor_Click(object sender, EventArgs e)
        {
            // in order to preserve which filters are enabled we have to save the filters before initiating the edit
            ItemFilter.Save(GetItemFilterFilePath());
            FormItemFilter.itemFilterTreeView.GenerateNodes();
            if (FormItemFilter.ShowDialog(this) == DialogResult.OK)
            {
                ItemFilter.Save(GetItemFilterFilePath());
                itemFilterTreeView.GenerateNodes();
                ItemCache.OnItemsChanged();
            }
            else
            {
                ItemFilter.Load(GetItemFilterFilePath());
                itemFilterTreeView.GenerateNodes(); // you have to rebuild dropdown, because reloading item filters from file makes Tags on drop down menu invalid
            }
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == (Keys)192 && e.Modifiers == (Keys.Alt | Keys.Control))
            {
                statGraphToolStripMenuItem.Visible = true;
            }
#if DEBUG
            if (e.KeyCode == (Keys)192 && e.Modifiers == Keys.Alt)
            {
                ToString(); //Breakpoint Here

                foreach (Item item in ItemCache.AllItems)
                {
                    if (item.LocationInfo[0].Description.Contains("Wowhead lacks"))
                        Item.LoadFromId(item.Id, true, false, false);
                }

                ItemCache.OnItemsChanged();
                
                ToString();
            }
#endif
        }

        public class LoadCharacterProfileArguments
        {
            public Character character;
            public CharacterProfilerCharacter characterProfilerCharacter;
        }

        private void reloadInvetoryFromCharacterProfilerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CharacterRegion region = (CharacterRegion)Enum.Parse(typeof(CharacterRegion), comboBoxRegion.SelectedItem.ToString());
            if (String.IsNullOrEmpty(Character.Name) || String.IsNullOrEmpty(Character.Realm)) {
                MessageBox.Show("A valid character has not been loaded, unable to reload.",
                    "No Character Loaded", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ".lua";
                dialog.Filter = "Character Profiler Saved Variables Files | *.lua";
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        CharacterProfilerData characterList = new CharacterProfilerData(dialog.FileName);

                        bool found_character = false;
                        for (int r=0; r<characterList.Realms.Count; r++)
                        {
                            if (characterList.Realms[r].Name == this.Character._realm)
                            {
                                for (int c=0; c<characterList.Realms[r].Characters.Count; c++)
                                {
                                    if (characterList.Realms[r].Characters[c].Name == this.Character._name)
                                    {
                                        StartProcessing();
                                        BackgroundWorker bw = new BackgroundWorker();
                                        bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_ReloadCharacterProfilerComplete);
                                        bw.DoWork += new DoWorkEventHandler(bw_ReloadCharacterProfiler);
                                        LoadCharacterProfileArguments args = new LoadCharacterProfileArguments();
                                        args.character = Character;
                                        args.characterProfilerCharacter = characterList.Realms[r].Characters[c];
                                        bw.RunWorkerAsync(args);
                                        // we found the character stop searching
                                        found_character = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!found_character)
                        {
                            string error_msg = string.Format("{0} of {1} was not found in the Character Profiler Data.", this.Character._name, this.Character._realm);
                            MessageBox.Show(error_msg, "Character Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (InvalidDataException ex)
                    {
                        MessageBox.Show("Unable to parse saved variable file: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading saved variable file: " + ex.Message);
                    }
                }
                dialog.Dispose();
            }
        }

        void bw_ReloadCharacterProfiler(object sender, DoWorkEventArgs e)
        {
            LoadCharacterProfileArguments args = e.Argument as LoadCharacterProfileArguments;
            e.Result = this.ReloadCharacterFromCharacterProfiler(args.character, args.characterProfilerCharacter);
        }

        void bw_ReloadCharacterProfilerComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // TODO: log this to the status screen.
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                Character character = e.Result as Character;
                LoadCharacterIntoForm(character);
                _unsavedChanges = true;
            }
            FinishedProcessing();
        }

        public Character ReloadCharacterFromCharacterProfiler(Character character, CharacterProfilerCharacter characterProfilerCharacter)
        {
            WebRequestWrapper.ResetFatalErrorIndicator();
            Character reload = GetCharacterFromCharacterProfiler(characterProfilerCharacter);
            if (reload != null)
            {
                this.Invoke(new ReloadCharacterFromCharacterProfilerUpdateDelegate(this.ReloadCharacterFromCharacterProfilerUpdate), character, reload);
            }
            return character;
        }

        public delegate void ReloadCharacterFromCharacterProfilerUpdateDelegate(Character character, Character reload);
        public void ReloadCharacterFromCharacterProfilerUpdate(Character character, Character reload)
        {
            //load values for gear from armory into original character
            character.IsLoading = true;
            character.SetItems(reload, true, true);
            foreach (string existingAvailableItem in character.AvailableItems)
            {
                string itemId = existingAvailableItem.Split('.')[0];
                if (reload.AvailableItems.Contains(itemId)) reload.AvailableItems.Remove(itemId);
            }
            character.AvailableItems.AddRange(reload.AvailableItems);
            character.AssignAllTalentsFromCharacter(reload, false);
            character.PrimaryProfession = reload.PrimaryProfession;
            character.SecondaryProfession = reload.SecondaryProfession;
            #region Hunter Pets if a Hunter
            if (character.Class == CharacterClass.Hunter)
            {
                // Pull Pet(s) Info if you are a Hunter
                List<ArmoryPet> pets = Armory.GetPet(character.Region, character.Realm, character.Name);
                if (pets != null) { character.ArmoryPets = pets; }
            }
            #endregion
            character.IsLoading = false;
            character.OnCalculationsInvalidated();
        }

        public Character GetCharacterFromCharacterProfiler(CharacterProfilerCharacter characterProfilerCharacter)
        {
            WebRequestWrapper.ResetFatalErrorIndicator();
            StatusMessaging.UpdateStatus("Load Character", " Loading Character");
            StatusMessaging.UpdateStatus("Update Item Cache", "Queued");
            StatusMessaging.UpdateStatus("Cache Item Icons", "Queued");
            characterProfilerCharacter.loadFromInfo();
            Character character = characterProfilerCharacter.Character;
            StatusMessaging.UpdateStatusFinished("Load Character");
            if (characterProfilerCharacter.Character != null)
            {
                _loadingCharacter = true; // suppress item changed event
                EnsureItemsLoaded(characterProfilerCharacter.Character.GetAllEquippedAndAvailableGearIds());
                _loadingCharacter = false;
            }
            else
            {
                StatusMessaging.UpdateStatusFinished("Update Item Cache");
                StatusMessaging.UpdateStatusFinished("Cache Item Icons");

            }
            return character;
        }

        private void checkBoxWristBlacksmithingSocket_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loadingCharacter && _character != null)
            {
                Character.WristBlacksmithingSocketEnabled = checkBoxWristBlacksmithingSocket.Checked;
            }
        }

        private void checkBoxHandsBlacksmithingSocket_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loadingCharacter && _character != null)
            {
                Character.HandsBlacksmithingSocketEnabled = checkBoxHandsBlacksmithingSocket.Checked;
            }
        }

        private void checkBoxWaistBlacksmithingSocket_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loadingCharacter && _character != null)
            {
                Character.WaistBlacksmithingSocketEnabled = checkBoxWaistBlacksmithingSocket.Checked;
            }
        }

        private void rawrHelpPageToolStripMenuItem_Click(object sender, EventArgs e)
        { Help.ShowHelp(null, "http://www.codeplex.com/Rawr/Wiki/View.aspx?title=Help"); }

        private void tourOfRawrToolStripMenuItem_Click(object sender, EventArgs e)
        { Help.ShowHelp(null, "http://www.youtube.com/watch?v=OjRM5SUoOoQ"); }

        private void gemmingsToolStripMenuItem_Click(object sender, EventArgs e)
        { Help.ShowHelp(null, "http://www.codeplex.com/Rawr/Wiki/View.aspx?title=Gemmings"); }

        private void gearOptimizationToolStripMenuItem_Click(object sender, EventArgs e)
        { Help.ShowHelp(null, "http://www.codeplex.com/Rawr/Wiki/View.aspx?title=GearOptimization"); }

        private void batchToolsToolStripMenuItem_Click(object sender, EventArgs e)
        { Help.ShowHelp(null, "http://www.codeplex.com/Rawr/Wiki/View.aspx?title=BatchTools"); }

        private void itemFilteringToolStripMenuItem_Click(object sender, EventArgs e)
        { Help.ShowHelp(null, "http://www.codeplex.com/Rawr/Wiki/View.aspx?title=ItemFiltering"); }

        private void rawrWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        { Help.ShowHelp(null, "http://rawr.codeplex.com/"); }

        private void donateToolStripMenuItem_Click(object sender, EventArgs e)
        { Help.ShowHelp(null, "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=2451163"); }

        private void upgradeListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".xml";
            dialog.Filter = "Rawr Upgrade List Files | *.xml";
            dialog.Multiselect = false;
            if (dialog.ShowDialog(this) == DialogResult.OK && 
                FormUpgradeComparison.Instance.LoadFile(dialog.FileName))
            {
                FormUpgradeComparison.Instance.Show();
                FormUpgradeComparison.Instance.BringToFront();
            }
            dialog.Dispose();
        }

        #region Exports
        private void viewUpgradesOnWowheadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_loadingCharacter && _character != null)
                Help.ShowHelp(null, Wowhead.GetWowheadWeightedReportURL(_character));
        }

        private void copyPawnStringToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_loadingCharacter && _character != null)
                Exports.CopyPawnString(_character);
        }

        private void viewUpgradesOnLootRankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_loadingCharacter && _character != null)
                Help.ShowHelp(null, Exports.GetLootRankURL(_character));
        }

        private string GetChartDataCSV()
        {
            StringBuilder sb = new StringBuilder("Name,Equipped,Slot,Gem1,Gem2,Gem3,Enchant,Source,ItemId,GemmedId,Overall");
            foreach (string subPointName in Calculations.SubPointNameColors.Keys)
            {
                sb.AppendFormat(",{0}", subPointName);
            }
            sb.AppendLine();
            foreach (ComparisonCalculationBase comparisonCalculation in itemComparison1.ComparisonGraph.ItemCalculations)
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

        private void copyDataToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(GetChartDataCSV(), TextDataFormat.Text);
            }
            catch { }
        }

        private void exportToImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".png";
            dialog.Filter = "PNG|*.png|GIF|*.gif|JPG|*.jpg|BMP|*.bmp";
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    ImageFormat imgFormat = ImageFormat.Bmp;
                    if (dialog.FileName.EndsWith(".png")) imgFormat = ImageFormat.Png;
                    else if (dialog.FileName.EndsWith(".gif")) imgFormat = ImageFormat.Gif;
                    else if (dialog.FileName.EndsWith(".jpg") || dialog.FileName.EndsWith(".jpeg")) imgFormat = ImageFormat.Jpeg;
                    itemComparison1.ComparisonGraph.PrerenderedGraph.Save(dialog.FileName, imgFormat);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            dialog.Dispose();
        }

        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".csv";
            dialog.Filter = "Comma Separated Values | *.csv";
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter writer = System.IO.File.CreateText(dialog.FileName))
                    {
                        writer.Write(GetChartDataCSV());
                        writer.Flush();
                        writer.Close();
                        writer.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            dialog.Dispose();
        }

        private void copyEnhSimConfigToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Type formOptionsPanel = Calculations.CalculationOptionsPanel.GetType();
            if (formOptionsPanel.FullName == "Rawr.Enhance.CalculationOptionsPanelEnhance")
            {
                MethodInfo exportMethod = formOptionsPanel.GetMethod("Export");
                if (exportMethod != null)
                {
                    exportMethod.Invoke(Calculations.CalculationOptionsPanel, null);
                }
            }
        }

        #endregion

        private void startPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowStartPage();
        }

        private void updateFromWowheadFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormEnterWowheadFilter filterForm = new FormEnterWowheadFilter();
            if (filterForm.ShowDialog() == DialogResult.OK)
            {
                StartProcessing();
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(bw_ImportWowheadFilter);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_StatusCompleted);
                bw.RunWorkerAsync(filterForm.WowheadFilter);
            }
        }

        private void itemComparisonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_itemComparison == null || _itemComparison.IsDisposed)
            {
                _itemComparison = new FormItemComparison(Character);
            }
            _itemComparison.Show();
        }

        private void buttonStatGraph_Click(object sender, EventArgs e)
        {
            try
            {
                Calculations.Instance.StatGraphRenderer.MinimumX = int.Parse(textBox1.Text);
                Calculations.Instance.StatGraphRenderer.MaximumX = int.Parse(textBox2.Text);
                Calculations.Instance.StatGraphRenderer.GranularityX = int.Parse(textBox3.Text);
                Calculations.Instance.StatGraphRenderer.StatX = textBox4.Text;
                itemComparison1.Invalidate();
                itemComparison1.Refresh();
                itemComparison1.LoadCustomRenderedChart("Stat Graph");
            }
            catch { }
        }

        private void saveBuffsToolstripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".buffs.xml";
            dialog.Filter = "Rawr Xml Buff Files | *.buffs.xml";
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                Character.SaveBuffs(dialog.FileName);
                this.Cursor = Cursors.Default;
            }
            dialog.Dispose();
        }

        private void loadBuffsToolstripMenuItem_Click(object sender, EventArgs e)
        {
            if (PromptToSaveBeforeClosing())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ".buffs.xml";
                dialog.Filter = "Rawr Xml Buff Files | *.buffs.xml";
                dialog.Multiselect = false;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    Character.LoadBuffsFromXml(dialog.FileName);
                }
                dialog.Dispose();
            }
        }

        private void maleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowWowhead3DModelURL(Character, true);
        }

        private void femaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowWowhead3DModelURL(Character, false);
        }

        private void maleJavaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowWowhead3DModelJava(Character, true);
        }

        private void femaleJavaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowWowhead3DModelJava(Character, false);
        }
        
        private void ShowWowhead3DModelURL(Character character, bool maleModel)
        {
            bool missingDisplayID = false;
            StringBuilder URL = new StringBuilder("http://static.wowhead.com/modelviewer/ModelView.swf?model=");
            URL.Append(character.Race.ToString().ToLower());
            URL.Append(maleModel ? "male" : "female");
            URL.Append("&modelType=16&ha=0&hc=0&fa=0&sk=0&fh=0&fc=0&contentPath=http://static.wowhead.com/modelviewer/&blur=1&equipList=");
            foreach (ItemInstance item in character.GetItems())
            {
                if (item != null && (item.Slot != ItemSlot.Neck && item.Slot != ItemSlot.Shirt && item.Slot != ItemSlot.Tabard &&
                    item.Slot != ItemSlot.Trinket && item.Slot != ItemSlot.Finger && item.Slot != ItemSlot.Projectile && item.Slot != ItemSlot.ProjectileBag ||
                    (item.Slot == ItemSlot.Ranged && (item.Type == ItemType.Bow || item.Type == ItemType.Crossbow || item.Type == ItemType.Thrown || item.Type == ItemType.Wand))))
                {
                   if (item.DisplayId == 0 || item.DisplaySlot == 0)
                        missingDisplayID = true;
                    URL.Append(item.DisplaySlot + ",");
                    URL.Append(item.DisplayId + ",");
                }
            }
            if (missingDisplayID)
                MessageBox.Show("One or more of your equipped items has a missing Display Information.\r\nPlease update your item cache from Wowhead to try to fix this problem.");
            else
                Help.ShowHelp(null, URL.ToString().TrimEnd(','));
        }

        public void ShowWowhead3DModelJava(Character character, bool maleModel)
        {
            bool missingDisplayID = false;
            StringBuilder URL = new StringBuilder("<html><head><title>Wowhead 3D Character Model in Java</title></head><body>");
            URL.Append("<applet id=\"3dviewer-java\" code=\"org.jdesktop.applet.util.JNLPAppletLauncher\" ");
            URL.Append("width=\"600\" height=\"400\" ");
            URL.Append("archive=\"http://static.wowhead.com/modelviewer/applet-launcher.jar,");
            URL.Append("http://download.java.net/media/jogl/builds/archive/jsr-231-webstart-current/jogl.jar,");
            URL.Append("http://download.java.net/media/gluegen/webstart/gluegen-rt.jar,");
            URL.Append("http://download.java.net/media/java3d/webstart/release/vecmath/latest/vecmath.jar,");
            URL.Append("http://static.wowhead.com/modelviewer/ModelView510.jar\">");
            URL.Append("<param name=\"jnlp_href\" value=\"http://static.wowhead.com/modelviewer/ModelView.jnlp\">");
            URL.Append("<param name=\"codebase_lookup\" value=\"false\">");
            URL.Append("<param name=\"cache_option\" value=\"no\">");
            URL.Append("<param name=\"subapplet.classname\" value=\"modelview.ModelViewerApplet\">");
            URL.Append("<param name=\"subapplet.displayname\" value=\"Model Viewer Applet\">");
            URL.Append("<param name=\"progressbar\" value=\"true\">");
            URL.Append("<param name=\"jnlpNumExtensions\" value=\"1\">");
            URL.Append("<param name=\"jnlpExtension1\" value=\"http://download.java.net/media/jogl/builds/archive/jsr-231-webstart-current/jogl.jnlp\">");
            URL.Append("<param name=\"contentPath\" value=\"http://static.wowhead.com/modelviewer/\">");
            URL.Append("<param name=\"model\" value=\"");
            URL.Append(character.Race.ToString().ToLower());
            URL.Append(maleModel ? "male" : "female");
            URL.Append("\">");
            URL.Append("<param name=\"modelType\" value=\"16\">");
            URL.Append("<param name=\"equipList\" value=\"");
            foreach (ItemInstance item in character.GetItems())
            {
                if (item != null && (item.Slot != ItemSlot.Neck && item.Slot != ItemSlot.Shirt && item.Slot != ItemSlot.Tabard &&
                    item.Slot != ItemSlot.Trinket && item.Slot != ItemSlot.Finger && item.Slot != ItemSlot.Projectile && item.Slot != ItemSlot.ProjectileBag ||
                    (item.Slot == ItemSlot.Ranged && (item.Type == ItemType.Bow || item.Type == ItemType.Crossbow || item.Type == ItemType.Thrown || item.Type == ItemType.Wand))))
                {
                    if (item.DisplayId == 0 || item.DisplaySlot == 0)
                        missingDisplayID = true;
                    URL.Append(item.DisplaySlot + ",");
                    URL.Append(item.DisplayId + ",");
                }
            }
            URL.Remove(URL.Length - 1, 1); // removes trailing ,
            URL.Append("\">");
            URL.Append("<param name=\"bgColor\" value=\"#181818\">");
            URL.Append("</applet></body></html>");

            if (missingDisplayID)
                MessageBox.Show("One or more of your equipped items has a missing Display Information.\r\nPlease update your item cache from Wowhead to try to fix this problem.");
            else
            {
                string path = Application.ExecutablePath;
                string fileName = path.Substring(0, path.LastIndexOf('\\')) + "\\data\\java3dModel.html";
                try
                {
                    System.IO.FileInfo file = new System.IO.FileInfo(fileName);
                    System.IO.StreamWriter sw = file.CreateText();
                    sw.WriteLine(URL.ToString());
                    sw.Close();
                    Help.ShowHelp(null, "file:///" + fileName);
                }
                catch
                {
                    MessageBox.Show("Failed to generate 3d Java html file.");
                }
            }
        }

        private void disableAllBuffsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buffSelector1.DisableAllBuffs();
        }

        private void resetItemCostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ItemCache.ResetItemCost();
        }

        private void loadItemCostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".cost.xml";
            dialog.Filter = "Rawr Xml Item Cost Files | *.cost.xml";
            dialog.Multiselect = false;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ItemCache.LoadItemCost(new StreamReader(dialog.FileName));
            }
            dialog.Dispose();
        }

        private void saveItemCostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".cost.xml";
            dialog.Filter = "Rawr Xml Item Cost Files | *.cost.xml";
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                ItemCache.SaveItemCost(new StreamWriter(dialog.FileName));
                this.Cursor = Cursors.Default;
            }
            dialog.Dispose();
        }

        private void loadEmblemOfFrostItemCostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ItemCache.LoadTokenItemCost("Emblem of Frost");
        }

        private void loadEmblemOfTriumphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ItemCache.LoadTokenItemCost("Emblem of Triumph");
        }

        private void loadEmblemOfConquestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ItemCache.LoadTokenItemCost("Emblem of Conquest");
        }

        private void loadEmblemOfValorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ItemCache.LoadTokenItemCost("Emblem of Valor");
        }

        private void loadEmblemOfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ItemCache.LoadTokenItemCost("Emblem of Heroism");
        }
        
        private void txtFilterBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter) {
                // search next selected item
                var sel = itemComparison1.ComparisonGraph.FindItem(txtFilterBox.Text, itemComparison1.ComparisonGraph.SelectedItemIndex + 1);

                // change
                itemComparison1.ComparisonGraph.SelectedItemIndex = sel;

                // if N/A - ding
                txtFilterBox.BackColor = (sel < 0) ? Color.LightPink : Color.Empty;

                // Weird Focus issue
                toolStripItemComparison.Focus();
                //txtFilterBox.Focus();

                // we handled it fine
                e.Handled = true;
            } else {
                if (txtFilterBox.BackColor != Color.Empty)
                    txtFilterBox.BackColor = Color.Empty;
                // Weird Focus issue
                txtFilterBox.Focus();
            }
        }

        private void txtFilterBox_Leave(object sender, EventArgs e)
        {
            EventArgs a = e;
            // Weird Focus issue
            toolStripItemComparison.Focus();
            toolStripItemComparison.Focus();
            //txtFilterBox.Focus();
        }

        private string GetItemFilterFilePath()
        {
            return Path.Combine("Data", "ItemFilter.xml");
        }

    }
}
