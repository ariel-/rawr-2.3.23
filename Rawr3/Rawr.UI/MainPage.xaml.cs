using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
#if !SILVERLIGHT
using Microsoft.Win32;
#endif
using Rawr;
using System.Threading;

namespace Rawr.UI
{
	public partial class MainPage : UserControl
	{
		public static ItemTooltip Tooltip { get; private set; }
		public static MainPage Instance { get; private set; }

		private bool _unsavedChanges = false;

		private Status status;
		public Status Status
		{
			set { status = value; }
			get
			{
				if (status == null) status = new Status();
				return status;
			}
		}

		private ItemBrowser itemSearch = null;
		public ItemBrowser ItemSearch
		{
			get
			{
				if (itemSearch == null) itemSearch = new ItemBrowser();
				return itemSearch;
			}
		}

		private Character character;
		public Character Character
		{
			get { return character; }
			set
			{
				if (value != null)
				{
					if (character != null)
					{
						character.CalculationsInvalidated -= new EventHandler(character_CalculationsInvalidated);
						character.AvailableItemsChanged -= new EventHandler(character_AvailableItemsChanged);
						character.ClassChanged -= new EventHandler(character_ClassChanged);
					}

					character = value;
					if (_batchCharacter != null && _batchCharacter.Character != character)
					{
						// we're loading a character that is not a batch character
						_batchCharacter = null;
					}
					character.IsLoading = true;

					DataContext = character;
					Calculations.LoadModel(Calculations.Models[character.CurrentModel]);
					Calculations.ClearCache();
					Calculations.CalculationOptionsPanel.Character = character;
					HeadButton.Character = character;
					NeckButton.Character = character;
					ShoulderButton.Character = character;
					BackButton.Character = character;
					ChestButton.Character = character;
					ShirtButton.Character = character;
					TabardButton.Character = character;
					WristButton.Character = character;
					HandButton.Character = character;
					BeltButton.Character = character;
					LegButton.Character = character;
					FeetButton.Character = character;
					Ring1Button.Character = character;
					Ring2Button.Character = character;
					Trinket1Button.Character = character;
					Trinket2Button.Character = character;
					MainHandButton.Character = character;
					OffHandButton.Character = character;
					RangedButton.Character = character;
					TalentPicker.Character = character;

					BuffControl.Character = character;

					BossPicker.Character = character;

					character.CalculationsInvalidated += new EventHandler(character_CalculationsInvalidated);
					character.ClassChanged += new EventHandler(character_ClassChanged);
					character.AvailableItemsChanged += new EventHandler(character_AvailableItemsChanged);

					character_ClassChanged(this, EventArgs.Empty);

					if (character.LoadItemFilterEnabledOverride())
					{
						ItemCache.OnItemsChanged();
					}

					Character.IsLoading = false;
					character_CalculationsInvalidated(this, EventArgs.Empty);

					// it's important that ComparisonGraph hooks CalculationsInvalidated event after we do because
					// the reference calculation must happen first
					// it also triggers update graph when we change character so make sure we've already done 
					// the reference calculation
					ComparisonGraph.Character = character;
				}
			}
		}

		void character_AvailableItemsChanged(object sender, EventArgs e)
		{
			_unsavedChanges = true;
		}

		#region Batch Tools
		private Character _storedCharacter;
		private BatchCharacter _batchCharacter;
		public void BatchCharacterSaved(BatchCharacter character)
		{
			if (_batchCharacter == character)
			{
				//_unsavedChanges = false;
				//SetTitle();
			}
		}

		public void LoadBatchCharacter(BatchCharacter character)
		{
			if (character.Character != null)
			{
				if (_batchCharacter == null)
				{
					_storedCharacter = this.character;
					//_storedCharacterPath = _characterPath;
					//_storedUnsavedChanged = _unsavedChanges;
				}
				_batchCharacter = character;
				//_characterPath = character.AbsulutePath;
				EnsureItemsLoaded(character.Character.GetAllEquippedAndAvailableGearIds());
				//LoadCharacterIntoForm(character.Character, character.UnsavedChanges);
				Character = character.Character;
			}
		}

		public void UnloadBatchCharacter()
		{
			if (_batchCharacter != null)
			{
				_batchCharacter = null;
				//_characterPath = _storedCharacterPath;
				//LoadCharacterIntoForm(_storedCharacter, _storedUnsavedChanged);
				Character = _storedCharacter;
				_storedCharacter = null;
			}
		}
		#endregion

		private class AsyncCalculationResult
		{
			public CharacterCalculationsBase Calculations;
			public Dictionary<string, string> DisplayCalculationValues;
		}

		CharacterCalculationsBase referenceCalculation;
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
			if (result.DisplayCalculationValues != null && result.Calculations == referenceCalculation)
			{
				CalculationDisplay.SetCalculations(result.DisplayCalculationValues);
				// refresh chart
				ComparisonGraph.UpdateGraph();
				asyncCalculation = null;
			}
		}

		public void character_CalculationsInvalidated(object sender, EventArgs e)
		{
#if DEBUG
			DateTime start = DateTime.Now;
#endif
			this.Cursor = Cursors.Wait;
			if (asyncCalculation != null)
			{
				CharacterCalculationsBase oldCalcs = referenceCalculation;
				referenceCalculation = null;
				oldCalcs.CancelAsynchronousCharacterDisplayCalculation();
				asyncCalculation = null;
			}
			//_unsavedChanges = true;
			referenceCalculation = Calculations.GetCharacterCalculations(character, null, true, true, true);
			CalculationDisplay.SetCalculations(referenceCalculation.GetCharacterDisplayCalculationValues());
			UpdateDisplayCalculationValues(referenceCalculation.GetCharacterDisplayCalculationValues(), referenceCalculation);
			if (Character.PrimaryProfession == Profession.Blacksmithing || Character.SecondaryProfession == Profession.Blacksmithing)
			{
				//HandBSCheck.IsChecked = true;
				//WristBSCheck.IsChecked = true;
				HandBSCheck.IsEnabled = true;
				WristBSCheck.IsEnabled = true;
			}
			else
			{
				HandBSCheck.IsChecked = false;
				WristBSCheck.IsChecked = false;
				HandBSCheck.IsEnabled = false;
				WristBSCheck.IsEnabled = false;
			}
			if (referenceCalculation.RequiresAsynchronousDisplayCalculation)
			{
				asyncCalculation = AsyncOperationManager.CreateOperation(null);
				ThreadPool.QueueUserWorkItem(delegate
				{
					AsyncCalculationStart(referenceCalculation, asyncCalculation);
				});
			}
			this.Cursor = Cursors.Arrow;
#if DEBUG
			System.Diagnostics.Debug.WriteLine(string.Format("Finished MainPage CalculationsInvalidated: {0}ms", DateTime.Now.Subtract(start).TotalMilliseconds));
#endif
		}

		public void ProfChanged(object sender, SelectionChangedEventArgs e) {
			character_CalculationsInvalidated(null, null);
		}

		public void UpdateDisplayCalculationValues(Dictionary<string, string> displayCalculationValues, CharacterCalculationsBase _calculatedStats)
		{
			//calculationDisplay1.SetCalculations(displayCalculationValues);
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
			StatusText.Text = status;
		}

		public MainPage()
		{
			Instance = this;
			InitializeComponent();
			if (App.Current.IsRunningOutOfBrowser) OfflineInstallButton.Visibility = Visibility.Collapsed;

			// Assign the Version number to the status bar
			Assembly asm = Assembly.GetExecutingAssembly();
			string version = "Debug";
			if (asm.FullName != null) {
				string[] parts = asm.FullName.Split(',');
				version = parts[1];
				if (version.Contains("Version=")) { version = version.Replace("Version=",""); }
				if (version.Contains(" ")) { version = version.Replace(" ", ""); }
			}
			VersionText.Text = string.Format("Rawr b{0}", version);

			asyncCalculationCompleted = new SendOrPostCallback(AsyncCalculationCompleted);

			Tooltip = ItemTooltip;

			HeadButton.Slot = CharacterSlot.Head;
			NeckButton.Slot = CharacterSlot.Neck;
			ShoulderButton.Slot = CharacterSlot.Shoulders;
			BackButton.Slot = CharacterSlot.Back;
			ChestButton.Slot = CharacterSlot.Chest;
			ShirtButton.Slot = CharacterSlot.Shirt;
			TabardButton.Slot = CharacterSlot.Tabard;
			WristButton.Slot = CharacterSlot.Wrist;
			HandButton.Slot = CharacterSlot.Hands;
			BeltButton.Slot = CharacterSlot.Waist;
			LegButton.Slot = CharacterSlot.Legs;
			FeetButton.Slot = CharacterSlot.Feet;
			Ring1Button.Slot = CharacterSlot.Finger1;
			Ring2Button.Slot = CharacterSlot.Finger2;
			Trinket1Button.Slot = CharacterSlot.Trinket1;
			Trinket2Button.Slot = CharacterSlot.Trinket2;
			MainHandButton.Slot = CharacterSlot.MainHand;
			OffHandButton.Slot = CharacterSlot.OffHand;
			RangedButton.Slot = CharacterSlot.Ranged;

			ModelCombo.ItemsSource = Calculations.Models.Keys;
			Calculations.ModelChanged += new EventHandler(Calculations_ModelChanged);

			Character c = new Character();
			c.CurrentModel = ConfigModel;
			c.Class = Calculations.ModelClasses[c.CurrentModel];
			Character = c;

			Calculations.ModelChanging += new EventHandler(Calculations_ModelChanging);
			ItemCache.Instance.ItemsChanged += new EventHandler(ItemCacheInstance_ItemsChanged);

			StatusMessaging.Ready = true;
		}

		private string ConfigModel { get { return "Bear"; } }

		private void Calculations_ModelChanged(object sender, EventArgs e)
		{
			foreach (string item in ModelCombo.Items)
			{
				if (item == Character.CurrentModel)
				{
					ModelCombo.SelectedItem = item;
				}
			}
			if (CMP_ClassModel != null)
			{
				try
				{
					bool found = false;
					foreach (Rawr.UI.ClassModelPicker.ClassModelPickerItem i in CMP_ClassModel.Items)
					{
						foreach (Rawr.UI.ClassModelPicker.ClassModelPickerItem j in i.Items)
						{
							if (j.Header == Character.CurrentModel)
							{
								CMP_ClassModel.PrimaryItem = i;
								CMP_ClassModel.PrimaryItem.SelectedItem = j;
								CMP_ClassModel.TextBlockClassModelButtonPrimary.Text = CMP_ClassModel.PrimaryItem.Header;
								CMP_ClassModel.TextBlockClassModelButtonSecondary.Text = CMP_ClassModel.PrimaryItem.SelectedItem.Header;
								found = true;
								break;
							}
						}
						if (found) break;
					}
				}
				catch (Exception ex) {
					new ErrorWindow() { Message = string.Format("Message: {0}\r\nInner Exception: {1}\r\n\r\nStack Trace:\r\n{2}",
						ex.Message, ex.InnerException, ex.StackTrace) }.Show();
				}
			}
			UpdateRaceLimitations();
			OptionsView.Content = Calculations.CalculationOptionsPanel;
			if (!Character.IsLoading) Character = Character;
		}

		private void UpdateRaceLimitations()
		{
			foreach (ComboBoxItem i in RaceCombo.Items)
			{
				if (i.Content != null && i.Content.ToString() != ""
					&& GetClassAllowableRaces[Character.CurrentModel].Contains(i.Content.ToString())) {
					i.Visibility = Visibility.Visible;
				}else{
					i.Visibility = Visibility.Collapsed;
				}
			}
		}

		private void Calculations_ModelChanging(object sender, EventArgs e)
		{
			Character.SerializeCalculationOptions();
		}

		private void ModelCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!Character.IsLoading)
			{
				Character.CurrentModel = ModelCombo.SelectedItem as string;
				Character.Class = Calculations.ModelClasses[Character.CurrentModel];
				Calculations.LoadModel(Calculations.GetModel(Character.CurrentModel));
			}
		}
		
		private void ItemCacheInstance_ItemsChanged(object sender, EventArgs e)
		{
			Character.InvalidateItemInstances();
			Character.OnCalculationsInvalidated();
			if (!Character.IsLoading)
			{
				ComparisonGraph.UpdateGraph();
			}
		}

		private void InstallOffline(object sender, System.Windows.RoutedEventArgs e)
		{
#if SILVERLIGHT
			if (Application.Current.InstallState == InstallState.NotInstalled)
			{
				Application.Current.Install();
			}
#endif
		}

		private void PerformanceTest(object sender, System.Windows.RoutedEventArgs e)
		{
			DateTime start = DateTime.Now;
			for (int i = 0; i < 20000; i++)
				Calculations.GetCharacterCalculations(Character);
			TimeSpan ts = DateTime.Now.Subtract(start);
			MessageBox.Show(ts.ToString());
		}

		private void EnsureItemsLoaded(string[] ids)
		{
			List<Item> items = new List<Item>();
			for (int i = 0; i < ids.Length; i++)
			{
				string id = ids[i];
				if (id != null)
				{
					if (id.IndexOf('.') < 0 && ItemCache.ContainsItemId(int.Parse(id))) continue;
					string[] s = id.Split('.');
					Item newItem = Item.LoadFromId(int.Parse(s[0]), false, false, false);
					if (s.Length >= 4)
					{
						Item gem;
						if (s[1] != "*" && s[1] != "0") gem = Item.LoadFromId(int.Parse(s[1]), false, false, false);
						if (s[2] != "*" && s[2] != "0") gem = Item.LoadFromId(int.Parse(s[2]), false, false, false);
						if (s[3] != "*" && s[3] != "0") gem = Item.LoadFromId(int.Parse(s[3]), false, false, false);
					}
					if (newItem != null)
					{
						items.Add(newItem);
					}
				}
			}
			ItemCache.OnItemsChanged();
		}

		private void ItemsAreLoaded(Character character)
		{
			Status.Close();
			Character = character;
		}

		public void LoadCharacterFromArmory(string characterName, CharacterRegion region, string realm)
		{
			ArmoryLoadDialog armoryLoad = new ArmoryLoadDialog();
			armoryLoad.Closed += new EventHandler(armoryLoad_Closed);
			armoryLoad.Show();
			armoryLoad.Load(characterName, region, realm);
		}

		private void armoryLoad_Closed(object sender, EventArgs e)
		{
			ArmoryLoadDialog ald = sender as ArmoryLoadDialog;
			if (((ArmoryLoadDialog)sender).DialogResult.GetValueOrDefault(false))
			{
				Character character = ald.Character;
				// The removes force it to put those items at the end.
				// So we can use that for recall later on what was last used
				if (Rawr.Properties.RecentSettings.Default.RecentChars.Contains(character.Name)) {
					Rawr.Properties.RecentSettings.Default.RecentChars.Remove(character.Name);
				}
				if (Rawr.Properties.RecentSettings.Default.RecentServers.Contains(character.Realm)) {
					Rawr.Properties.RecentSettings.Default.RecentServers.Remove(character.Realm);
				}
				Rawr.Properties.RecentSettings.Default.RecentChars.Add(character.Name);
				Rawr.Properties.RecentSettings.Default.RecentServers.Add(character.Realm);
				Rawr.Properties.RecentSettings.Default.RecentRegion = character.Region.ToString();

				this.Character = character;
			}
		}

		private void armory_ResultReady(Character newChar)
		{
			if (newChar != null)
			{
				Character = newChar;
			}
			Status = null;
		}

		private void charprofLoad_Closed(object sender, EventArgs e)
		{
			CharProfLoadDialog cpld = sender as CharProfLoadDialog;
			if (((CharProfLoadDialog)sender).DialogResult.GetValueOrDefault(false))
			{
				Character character = cpld.Character;

				// So we can use that for recall later on what was last used
				Rawr.Properties.RecentSettings.Default.RecentCharProfiler = cpld.TB_FilePath.Text;

				this.Character = character;
			}
		}

		#region Menus
#if SILVERLIGHT
		/*Obsoleted by implementing MenuButton control
		private void FileMenu_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (FileMenu != null)
			{
				int newIndex = FileMenu.SelectedIndex;
				if (newIndex > 0)
				{
					FileMenu.IsDropDownOpen = false;
					FileMenu.SelectedIndex = 0;
					if (newIndex == 1) NewCharacter(null, null);
					else if (newIndex == 2) OpenCharacter(null, null);
					else if (newIndex == 3) SaveCharacter(null, null);
					else if (newIndex == 4) OpenSavedUpgradeList(null, null);
					else if (newIndex == 6) LoadFromArmory(null, null);
					//else if (newIndex == 7) LoadFromCharacterProfiler(null, null);
					else new ErrorWindow() { Message = "Not yet implemented." }.Show();
				}
			}
		}

		private void ToolsMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ToolsMenu != null)
			{
				int newIndex = ToolsMenu.SelectedIndex;
				if (newIndex > 0)
				{
					ToolsMenu.IsDropDownOpen = false;
					ToolsMenu.SelectedIndex = 0;
					if (newIndex == 1) ShowItemEditor(null, null);
					else if (newIndex == 2) ShowGemmingTemplates(null, null);
					else if (newIndex == 3) ShowItemRefinement(null, null);
					else if (newIndex == 4) ShowItemFilters(null, null);
					else if (newIndex == 6) ResetItemCost(null, null);
					else if (newIndex == 7) LoadItemCost(null, null);
					else if (newIndex == 8) SaveItemCost(null, null);
					else if (newIndex == 9) LoadEmblemOfFrostCost(null, null);
					else if (newIndex == 11) ShowOptimizer(null, null);
					else if (newIndex == 12) ShowBatchTools(null, null);
					else new ErrorWindow() { Message = "Not yet implemented." }.Show();
				}
			}
		}

		private void ImportMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ImportMenu != null)
			{
				int newIndex = ImportMenu.SelectedIndex;
				if (newIndex > 0)
				{
					ImportMenu.IsDropDownOpen = false;
					ImportMenu.SelectedIndex = 0;
					new ErrorWindow() { Message = "Not yet implemented." }.Show();
				}
			}
		}

		private void OptionsMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (OptionsMenu != null)
			{
				int newIndex = OptionsMenu.SelectedIndex;
				if (newIndex > 0)
				{
					OptionsMenu.IsDropDownOpen = false;
					OptionsMenu.SelectedIndex = 0;
					if (newIndex == 1) ShowOptions(null, null);
					else if (newIndex == 3) ResetItemCache(null, null);
					else if (newIndex == 4) ResetAllCaches(null, null);
					else new ErrorWindow() { Message = "Not yet implemented." }.Show();
				}
			}

		}

		private void HelpMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (HelpMenu != null)
			{
				int newIndex = HelpMenu.SelectedIndex;
				if (newIndex > 0)
				{
					HelpMenu.IsDropDownOpen = false;
					HelpMenu.SelectedIndex = 0;
					//if (newIndex == 1) ShowHelp("http://rawr.codeplex.com/documentation");
					//else if (newIndex == 2) ShowHelp("http://www.youtube.com/watch?v=OjRM5SUoOoQ");
					//else if (newIndex == 3) ShowHelp("http://rawr.codeplex.com/wikipage?title=Gemmings");
					//else if (newIndex == 4) ShowHelp("http://rawr.codeplex.com/wikipage?title=GearOptimization");
					//else if (newIndex == 5) ShowHelp("http://rawr.codeplex.com/wikipage?title=ItemFiltering");
					//else if (newIndex == 7) ShowHelp("http://rawr.codeplex.com/");
					//else if (newIndex == 8) ShowHelp("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=2451163");
					//else new ErrorWindow() { Message = "Not yet implemented." }.Show();
				}
			}
		}
		*/
#endif

		#region File Menu
		private void NewCharacter(object sender, RoutedEventArgs args)
		{
			if (_unsavedChanges) { NeedToSaveCharacter(); }
			Character c = new Character();
			c.CurrentModel = Character.CurrentModel;
			c.Class = Character.Class;
			c.Race = Character.Race;
			Character = c;
		}

		private void OpenCharacter(object sender, RoutedEventArgs args)
		{
			if (_unsavedChanges) { NeedToSaveCharacter(); }
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "character file (*.xml)|*.xml";
			if (ofd.ShowDialog().GetValueOrDefault(false))
			{
#if SILVERLIGHT
				using (StreamReader reader = ofd.File.OpenText())
#else
				using (StreamReader reader = new StreamReader(ofd.OpenFile()))
#endif
				{
					// TODO: we'll have to expand this considerably to get to Rawr2 functionality                    
					Character loadedCharacter = Character.LoadFromXml(reader.ReadToEnd());
					EnsureItemsLoaded(loadedCharacter.GetAllEquippedAndAvailableGearIds());
					Character = loadedCharacter;
				}
			}
		}

		public void NeedToSaveCharacter() {
			if (MessageBox.Show("There are unsaved changes to the current character, do you want to save them?\n\nWARNING: Selecting Cancel will lose the unsaved changes.",
				"Unsaved Changes Detected", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
			{
				SaveCharacter(null, null);
			}
		}
		private void SaveCharacter(object sender, RoutedEventArgs args)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "character file (*.xml)|*.xml";
			if (sfd.ShowDialog().GetValueOrDefault(false))
			{
				Character.Save(sfd.OpenFile());
				_unsavedChanges = false;
			}
		}

		private void OpenSavedUpgradeList(object sender, RoutedEventArgs args)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Rawr Upgrade List Files|*.xml";
			if (ofd.ShowDialog().GetValueOrDefault())
			{
#if SILVERLIGHT
				new UpgradesComparison(ofd.File.OpenText()).Show();
#else
				new UpgradesComparison(new StreamReader(ofd.OpenFile())).Show();
#endif
			}
		}

		private void LoadFromArmory(object sender, RoutedEventArgs args)
		{
			if (_unsavedChanges) { NeedToSaveCharacter(); }
			ArmoryLoadDialog armoryLoad = new ArmoryLoadDialog();
			armoryLoad.Closed += new EventHandler(armoryLoad_Closed);
			armoryLoad.Show();
		}

		private void LoadFromCharacterProfiler(object sender, RoutedEventArgs args)
		{
			if (_unsavedChanges) { NeedToSaveCharacter(); }
			CharProfLoadDialog charprofLoad = new CharProfLoadDialog();
			charprofLoad.Closed += new EventHandler(charprofLoad_Closed);
			charprofLoad.Show();
		}
		#endregion

		#region Tools Menu
		private void ShowItemEditor(object sender, RoutedEventArgs args)
		{
			ItemSearch.Show();
		}

		private void ShowGemmingTemplates(object sender, RoutedEventArgs args)
		{
			new GemmingTemplates(Character).Show();
		}

		private void ShowItemRefinement(object sender, RoutedEventArgs args)
		{
			new RelevantItemRefinement().Show();
		}

		private void ShowItemFilters(object sender, RoutedEventArgs args)
		{
			new EditItemFilter().Show();
		}

		private void ResetItemCost(object sender, RoutedEventArgs args)
		{
			ItemCache.ResetItemCost();
		}

		private void LoadItemCost(object sender, RoutedEventArgs args)
		{
			OpenFileDialog dialog = new OpenFileDialog();
#if !SILVERLIGHT
			dialog.DefaultExt = ".cost.xml";
#endif
			dialog.Filter = "Rawr Xml Item Cost Files | *.cost.xml";
			dialog.Multiselect = false;
			if (dialog.ShowDialog().GetValueOrDefault(false))
			{
#if SILVERLIGHT
				using (StreamReader reader = dialog.File.OpenText())
#else
				using (StreamReader reader = new StreamReader(dialog.OpenFile()))
#endif
				{
					ItemCache.LoadItemCost(reader);
				}
			}
		}

		private void SaveItemCost(object sender, RoutedEventArgs args)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.DefaultExt = ".cost.xml";
			dialog.Filter = "Rawr Xml Item Cost Files | *.cost.xml";
			if (dialog.ShowDialog().GetValueOrDefault(false))
			{
				this.Cursor = Cursors.Wait;
				using (StreamWriter writer = new StreamWriter(dialog.OpenFile()))
				{
					ItemCache.SaveItemCost(writer);
				}
				this.Cursor = Cursors.Arrow;
			}
		}

		private void LoadEmblemOfFrostCost(object sender, RoutedEventArgs args)
		{
			ItemCache.LoadTokenItemCost("Emblem of Frost");
		}

		private void ShowOptimizer(object sender, RoutedEventArgs args)
		{
			new OptimizeWindow(Character).Show();
		}

		private void ShowBatchTools(object sender, RoutedEventArgs args)
		{
#if SILVERLIGHT
			// in silverlight we want to reuse current batch tools
			if (BatchTools.Instance != null)
			{
				App.Current.ShowWindow(BatchTools.Instance);
			}
			else
			{
				App.Current.OpenNewWindow("Batch Tools", new BatchTools());
			}
#else
			App.Current.OpenNewWindow("Batch Tools", new BatchTools());
#endif
		}
		#endregion

		#region Import Menu
		#endregion

		#region Options Menu
		private void ShowOptions(object sender, RoutedEventArgs e)
		{
			new OptionsDialog().Show();
		}

		private void ResetAllCaches(object sender, RoutedEventArgs e)
		{
			ConfirmationWindow.ShowDialog("Are you sure you'd like to clear and redownload all caches?\r\n\r\nWARNING: This will also unload the current character, so be sure to save first!",
				new EventHandler(ResetAllCaches_Confirmation));
		}

		private void ResetAllCaches_Confirmation(object sender, EventArgs e)
		{
			if (((ConfirmationWindow)sender).DialogResult == true)
			{
				Character = new Character();
				new FileUtils(new string[] {
					"BuffCache.xml", 
					"EnchantCache.xml",
					"ItemCache.xml",
					"Talents.xml",
					"PetTalents.xml",
					"BuffSets.xml",
					"ItemSource.xml",
					"ItemFilter.xml",
					"Settings.xml"}).Delete();
				LoadScreen ls = new LoadScreen();
				(App.Current.RootVisual as Grid).Children.Add(ls);
				this.Visibility = Visibility.Collapsed;
				ls.StartLoading(new EventHandler(ResetCaches_Finished));
				Character = new Character();
			}
		}

		private void ResetItemCache(object sender, RoutedEventArgs e)
		{
			ConfirmationWindow.ShowDialog("Are you sure you'd like to clear and redownload the item cache?\r\n\r\nWARNING: This will also unload the current character, so be sure to save first!",
				new EventHandler(ResetItemCaches_Confirmation));
		}

		private void ResetItemCaches_Confirmation(object sender, EventArgs e)
		{
			if (((ConfirmationWindow)sender).DialogResult == true)
			{
				Character = new Character();
				new FileUtils("ItemCache.xml").Delete();
				LoadScreen ls = new LoadScreen();
				(App.Current.RootVisual as Grid).Children.Add(ls);
				this.Visibility = Visibility.Collapsed;
				ls.StartLoading(new EventHandler(ResetCaches_Finished));
				Character = new Character();
			}
		}

		private void ResetCaches_Finished(object sender, EventArgs e)
		{
			(App.Current.RootVisual as Grid).Children.Remove(sender as LoadScreen);
			this.Visibility = Visibility.Visible;
		}
		#endregion

		#region Help Menu
		private void ShowHelp(string uri)
		{
#if SILVERLIGHT
			System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(uri, UriKind.Absolute), "_blank");
#else
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(uri));
#endif
		}

		private void ShowRawrHelpPage(object sender, RoutedEventArgs args)
		{
			ShowHelp("http://rawr.codeplex.com/documentation");
		}

		private void ShowTourOfRawr(object sender, RoutedEventArgs args)
		{
			ShowHelp("http://www.youtube.com/watch?v=OjRM5SUoOoQ");
		}

		private void ShowGemmingsHelp(object sender, RoutedEventArgs args)
		{
			ShowHelp("http://rawr.codeplex.com/wikipage?title=Gemmings");
		}

		private void ShowGearOptimizationHelp(object sender, RoutedEventArgs args)
		{
			ShowHelp("http://rawr.codeplex.com/wikipage?title=GearOptimization");
		}

		private void ShowItemFilteringHelp(object sender, RoutedEventArgs args)
		{
			ShowHelp("http://rawr.codeplex.com/wikipage?title=ItemFiltering");
		}

		private void ShowRawrWebsite(object sender, RoutedEventArgs args)
		{
			ShowHelp("http://rawr.codeplex.com/");
		}

		private void ShowDonate(object sender, RoutedEventArgs args)
		{
			ShowHelp("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=2451163");
		}
		#endregion
		#endregion

		private void character_ClassChanged(object sender, EventArgs e)
		{
			ClassCombo.SelectedIndex = Character.ClassIndex;
		}

		private void ClassCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Character.ClassIndex = ClassCombo.SelectedIndex;
		}

		private static Dictionary<string, List<string>> classAllowableRaces = null;
		private static Dictionary<string, List<string>> GetClassAllowableRaces
		{
			get
			{
				if (classAllowableRaces == null)
				{
					classAllowableRaces = new Dictionary<string, List<string>>();
					classAllowableRaces.Add("DPSDK", new List<string>() { "Draenei", "Dwarf", "Gnome", "Human", "Night Elf", "Blood Elf", "Orc", "Tauren", "Troll", "Undead", });
					classAllowableRaces.Add("TankDK", new List<string>() { "Draenei", "Dwarf", "Gnome", "Human", "Night Elf", "Blood Elf", "Orc", "Tauren", "Troll", "Undead", });
					classAllowableRaces.Add("Bear", new List<string>() { /*"Draenei",*/ /*"Dwarf",*/ /*"Gnome",*/ /*"Human",*/ "Night Elf", /*"Blood Elf",*/ /*"Orc",*/ "Tauren", /*"Troll",*/ /*"Undead",*/ });
					classAllowableRaces.Add("Cat", new List<string>() { /*"Draenei",*/ /*"Dwarf",*/ /*"Gnome",*/ /*"Human",*/ "Night Elf", /*"Blood Elf",*/ /*"Orc",*/ "Tauren", /*"Troll",*/ /*"Undead",*/ });
					classAllowableRaces.Add("Moonkin", new List<string>() { /*"Draenei",*/ /*"Dwarf",*/ /*"Gnome",*/ /*"Human",*/ "Night Elf", /*"Blood Elf",*/ /*"Orc",*/ "Tauren", /*"Troll",*/ /*"Undead",*/ });
					classAllowableRaces.Add("Tree", new List<string>() { /*"Draenei",*/ /*"Dwarf",*/ /*"Gnome",*/ /*"Human",*/ "Night Elf", /*"Blood Elf",*/ /*"Orc",*/ "Tauren", /*"Troll",*/ /*"Undead",*/ });
					classAllowableRaces.Add("Hunter", new List<string>() { "Draenei", "Dwarf", /*"Gnome",*/ /*"Human",*/ "Night Elf", "Blood Elf", "Orc", "Tauren", "Troll", /*"Undead",*/ });
					classAllowableRaces.Add("Mage", new List<string>() { "Draenei", /*"Dwarf",*/ "Gnome", "Human", /*"Night Elf",*/ "Blood Elf", /*"Orc",*/ /*"Tauren",*/ "Troll", "Undead", });
					classAllowableRaces.Add("Healadin", new List<string>() { "Draenei", "Dwarf", /*"Gnome",*/ "Human", /*"Night Elf",*/ "Blood Elf", /*"Orc",*/ /*"Tauren",*/ /*"Troll",*/ /*"Undead",*/ });
					classAllowableRaces.Add("ProtPaladin", new List<string>() { "Draenei", "Dwarf", /*"Gnome",*/ "Human", /*"Night Elf",*/ "Blood Elf", /*"Orc",*/ /*"Tauren",*/ /*"Troll",*/ /*"Undead",*/ });
					classAllowableRaces.Add("Retribution", new List<string>() { "Draenei", "Dwarf", /*"Gnome",*/ "Human", /*"Night Elf",*/ "Blood Elf", /*"Orc",*/ /*"Tauren",*/ /*"Troll",*/ /*"Undead",*/ });
					classAllowableRaces.Add("HealPriest", new List<string>() { "Draenei", "Dwarf", /*"Gnome",*/ "Human", "Night Elf", "Blood Elf", /*"Orc",*/ /*"Tauren",*/ "Troll", "Undead", });
					classAllowableRaces.Add("ShadowPriest", new List<string>() { "Draenei", "Dwarf", /*"Gnome",*/ "Human", "Night Elf", "Blood Elf", /*"Orc",*/ /*"Tauren",*/ "Troll", "Undead", });
					classAllowableRaces.Add("Rogue", new List<string>() { /*"Draenei",*/ "Dwarf", "Gnome", "Human", "Night Elf", "Blood Elf", "Orc", /*"Tauren",*/ "Troll", "Undead", });
					classAllowableRaces.Add("Enhance", new List<string>() { "Draenei", /*"Dwarf",*/ /*"Gnome",*/ /*"Human",*/ /*"Night Elf",*/ /*"Blood Elf",*/ "Orc", "Tauren", "Troll", /*"Undead",*/ });
					classAllowableRaces.Add("Elemental", new List<string>() { "Draenei", /*"Dwarf",*/ /*"Gnome",*/ /*"Human",*/ /*"Night Elf",*/ /*"Blood Elf",*/ "Orc", "Tauren", "Troll", /*"Undead",*/ });
					classAllowableRaces.Add("RestoSham", new List<string>() { "Draenei", /*"Dwarf",*/ /*"Gnome",*/ /*"Human",*/ /*"Night Elf",*/ /*"Blood Elf",*/ "Orc", "Tauren", "Troll", /*"Undead",*/ });
					classAllowableRaces.Add("Warlock", new List<string>() { /*"Draenei",*/ /*"Dwarf",*/ "Gnome", "Human", /*"Night Elf",*/ "Blood Elf", "Orc", /*"Tauren",*/ /*"Troll",*/ "Undead", });
					classAllowableRaces.Add("DPSWarr", new List<string>() { "Draenei", "Dwarf", "Gnome", "Human", "Night Elf", /*"Blood Elf",*/ "Orc", "Tauren", "Troll", "Undead", });
					classAllowableRaces.Add("ProtWarr", new List<string>() { "Draenei", "Dwarf", "Gnome", "Human", "Night Elf", /*"Blood Elf",*/ "Orc", "Tauren", "Troll", "Undead", });
				}
				return classAllowableRaces;
			}
			set { classAllowableRaces = value; }
		}
	}
}
