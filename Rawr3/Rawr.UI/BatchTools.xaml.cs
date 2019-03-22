using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Rawr.Properties;
using Microsoft.Win32;
#if !SILVERLIGHT
using System.Windows.Navigation;
using Microsoft.Windows.Controls;
#endif
using System.IO;

namespace Rawr.UI
{
	/// <summary>
	/// Interaction logic for BatchTools.xaml
	/// </summary>
	public partial class BatchTools : UserControl
	{
		public static BatchTools Instance { get; set; }
		internal Rawr.BatchTools batchTools;
		private bool _unsavedChanges;
		private string _filePath;

		public BatchTools()
		{
			InitializeComponent();

			batchTools = new Rawr.BatchTools();
			batchTools.OverrideRegem = OptimizerSettings.Default.OverrideRegem;
			batchTools.OverrideReenchant = OptimizerSettings.Default.OverrideReenchant;
			batchTools.Thoroughness = OptimizerSettings.Default.Thoroughness;
			batchTools.GreedyOptimizationMethod = OptimizerSettings.Default.GreedyOptimizationMethod;
			batchTools.OptimizationMethod = OptimizerSettings.Default.OptimizationMethod;
			batchTools.TemplateGemsEnabled = OptimizerSettings.Default.TemplateGemsEnabled;

			batchTools.OperationCompleted += new EventHandler(batchTools_OperationCompleted);
			batchTools.StatusUpdated += new EventHandler(batchTools_StatusUpdated);
			batchTools.UpgradeListCompleted += new EventHandler(batchTools_UpgradeListCompleted);

			DataContext = batchTools;

			Instance = this;
		}

		void batchTools_UpgradeListCompleted(object sender, EventArgs e)
		{
			UpgradesComparison upgrades = new UpgradesComparison(batchTools.Upgrades, batchTools.CustomSubpoints);
			upgrades.Show();
		}

		void batchTools_StatusUpdated(object sender, EventArgs e)
		{
			StatusLabel.Content = batchTools.Status;
			StatusProgress.Value = batchTools.Progress;
		}

		void batchTools_OperationCompleted(object sender, EventArgs e)
		{
			ButtonCancel.IsEnabled = false;
		}

#if SILVERLIGHT
		/*Obsoleted by implementing MenuButton
		private void FileMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (FileMenu != null)
			{
				int newIndex = FileMenu.SelectedIndex;
				if (newIndex > 0)
				{
					FileMenu.IsDropDownOpen = false;
					FileMenu.SelectedIndex = 0;
					switch (newIndex)
					{
						case 1:
							New(null, null);
							break;
						case 2:
							Import(null, null);
							break;
						case 4:
							Close(null, null);
							break;
						default:
							new ErrorWindow() { Message = "Not yet implemented." }.Show();
							break;
					}
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
					switch (newIndex)
					{
						case 1:
							batchTools.SetAvailableGear(MainPage.Instance.Character.AvailableItems);
							break;
						case 2:
							batchTools.ReplaceUnavailableGear();
							break;
						case 3:
							ButtonCancel.IsEnabled = true;
							batchTools.Optimize();
							break;
						case 4:
							ButtonCancel.IsEnabled = true;
							batchTools.SingleItemUpgrade = batchTools.GetSingleItemUpgrade(SingleItemUpgrade.Text);
							batchTools.BuildUpgradeList();
							break;
						case 5:
							ButtonCancel.IsEnabled = true;
							batchTools.BatchOptimize();
							break;
						case 6:
							ButtonCancel.IsEnabled = true;
							batchTools.SingleItemUpgrade = batchTools.GetSingleItemUpgrade(SingleItemUpgrade.Text);
							batchTools.BuildBatchUpgradeList();
							break;
						case 7:
							ButtonCancel.IsEnabled = true;
							batchTools.ProgressiveOptimize();
							break;
						case 8:
							ButtonCancel.IsEnabled = true;
							batchTools.SingleItemUpgrade = batchTools.GetSingleItemUpgrade(SingleItemUpgrade.Text);
							batchTools.BuildProgressiveUpgradeList();
							break;
						case 10:
							batchTools.SaveCharacters();
							break;
						case 11:
							batchTools.SaveCharactersAsCopy();
							break;
						default:
							new ErrorWindow() { Message = "Not yet implemented." }.Show();
							break;
					}
				}
			}
		}*/
#endif

		private bool PromptToSaveBeforeClosing()
		{
			if (_unsavedChanges)
			{
#if SILVERLIGHT
				MessageBoxResult result = MessageBox.Show("Would you like to save the current batch list before closing it?", "Rawr - Save?", MessageBoxButton.OKCancel);
				switch (result)
				{
					case MessageBoxResult.Yes:
						Save(null, null);
						return !string.IsNullOrEmpty(_filePath);
					default:
						return true;
				}
#else
				MessageBoxResult result = MessageBox.Show("Would you like to save the current batch list before closing it?", "Rawr - Save?", MessageBoxButton.YesNoCancel);
				switch (result)
				{
					case MessageBoxResult.Yes:
						Save(null, null);
						return !string.IsNullOrEmpty(_filePath);
					case MessageBoxResult.No:
						return true;
					default:
						return false;
				}
#endif
			}
			else
				return true;
		}

		// based on http://mrpmorris.blogspot.com/2007/05/convert-absolute-path-to-relative-path.html
		private string RelativePath(string absolutePath, string relativeTo)
		{
			string[] relativeDirectories = absolutePath.Split(System.IO.Path.DirectorySeparatorChar);
			string[] absoluteDirectories = relativeTo.Split(System.IO.Path.DirectorySeparatorChar);
			//Get the shortest of the two paths
			int length = absoluteDirectories.Length < relativeDirectories.Length ? absoluteDirectories.Length : relativeDirectories.Length;
			//Use to determine where in the loop we exited
			int lastCommonRoot = -1;
			int index;
			//Find common root
			for (index = 0; index < length; index++)
				if (absoluteDirectories[index] == relativeDirectories[index])
					lastCommonRoot = index;
				else
					break;
			//If we didn't find a common prefix then throw
			if (lastCommonRoot == -1)
				return absolutePath;
			//Build up the relative path
			StringBuilder relativePath = new StringBuilder();
			//Add on the ..
			for (index = lastCommonRoot + 1; index < absoluteDirectories.Length; index++)
				if (absoluteDirectories[index].Length > 0)
					relativePath.Append(".." + System.IO.Path.DirectorySeparatorChar);
			//Add on the folders
			for (index = lastCommonRoot + 1; index < relativeDirectories.Length - 1; index++)
				relativePath.Append(relativeDirectories[index] + System.IO.Path.DirectorySeparatorChar);
			relativePath.Append(relativeDirectories[relativeDirectories.Length - 1]);
			return relativePath.ToString();
		}

		#region File Menu
		private void New(object sender, RoutedEventArgs e)
		{
			if (PromptToSaveBeforeClosing())
			{
				_unsavedChanges = false;
				_filePath = null;
				batchTools.BatchCharacterList = new BatchCharacterList();
			}
		}

		private void Import(object sender, RoutedEventArgs e)
		{
			if (PromptToSaveBeforeClosing())
			{
				OpenFileDialog dialog = new OpenFileDialog();
#if !SILVERLIGHT
				dialog.DefaultExt = ".xml";
#endif
				dialog.Filter = "Rawr Xml Character Files | *.xml";
				dialog.Multiselect = true;
				if (dialog.ShowDialog().GetValueOrDefault())
				{
					_unsavedChanges = true;
					_filePath = null;
					BatchCharacterList list = new BatchCharacterList();
#if SILVERLIGHT
					foreach (FileInfo file in dialog.Files)
					{
						// we can only securely read the file name
						using (StreamReader reader = file.OpenText())
						{
							list.Add(new BatchCharacter() { RelativePath = file.Name, Character = Character.LoadFromXml(reader.ReadToEnd()) });
						}
					}
#else
					foreach (string filename in dialog.FileNames)
					{
						list.Add(new BatchCharacter() { RelativePath = RelativePath(filename, AppDomain.CurrentDomain.BaseDirectory) });
					}
#endif
					batchTools.BatchCharacterList = list;
				}
			}
		}

		private void Open(object sender, RoutedEventArgs e)
		{
			if (PromptToSaveBeforeClosing())
			{
				OpenFileDialog dialog = new OpenFileDialog();
#if !SILVERLIGHT
				dialog.DefaultExt = ".xml";
#endif
				dialog.Filter = "Rawr Batch Files | *.xml";
				dialog.Multiselect = false;
				if (dialog.ShowDialog().GetValueOrDefault())
				{
#if SILVERLIGHT
					_filePath = dialog.File.FullName;
#else
					_filePath = dialog.FileName;
#endif
					batchTools.BatchCharacterList = BatchCharacterList.Load(_filePath);
				}
			}
		}

		private void Save(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(_filePath))
			{
				batchTools.BatchCharacterList.Save(_filePath);
				//FormMain.Instance.AddRecentCharacter(_filePath);
				_unsavedChanges = false;
			}
			else
			{
				SaveAs(sender, e);
			}
		}

		private void SaveAs(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.DefaultExt = ".xml";
			dialog.Filter = "Rawr Batch Files | *.xml";
			if (dialog.ShowDialog().GetValueOrDefault())
			{
#if SILVERLIGHT
				batchTools.BatchCharacterList.Save(dialog.OpenFile());
#else
				_filePath = dialog.FileName;
				batchTools.BatchCharacterList.Save(_filePath);
#endif
				//FormMain.Instance.AddRecentCharacter(_filePath);
				_unsavedChanges = false;
			}
		}

		public void Close(object sender, RoutedEventArgs e)
		{
			Instance = null;
			MainPage.Instance.UnloadBatchCharacter();
			App.Current.CloseWindow(this);
		}
		#endregion

		#region Tools Menu
		private void SetAvailableItems(object sender, RoutedEventArgs e)
		{
			batchTools.SetAvailableGear(MainPage.Instance.Character.AvailableItems);
		}

		private void ReplaceUnavailable(object sender, RoutedEventArgs e)
		{
			batchTools.ReplaceUnavailableGear();
		}

		private void Optimize(object sender, RoutedEventArgs e)
		{
			ButtonCancel.IsEnabled = true;
			batchTools.Optimize();
		}

		private void BuildUpgradeList(object sender, RoutedEventArgs e)
		{
			ButtonCancel.IsEnabled = true;
			batchTools.SingleItemUpgrade = batchTools.GetSingleItemUpgrade(SingleItemUpgrade.Text);
			batchTools.BuildUpgradeList();
		}

		private void BatchOptimize(object sender, RoutedEventArgs e)
		{
			ButtonCancel.IsEnabled = true;
			batchTools.BatchOptimize();
		}

		private void BuildBatchUpgradeList(object sender, RoutedEventArgs e)
		{
			ButtonCancel.IsEnabled = true;
			batchTools.SingleItemUpgrade = batchTools.GetSingleItemUpgrade(SingleItemUpgrade.Text);
			batchTools.BuildBatchUpgradeList();
		}

		private void ProgressiveOptimize(object sender, RoutedEventArgs e)
		{
			ButtonCancel.IsEnabled = true;
			batchTools.ProgressiveOptimize();
		}

		private void BuildProgressiveUpgradeList(object sender, RoutedEventArgs e)
		{
			ButtonCancel.IsEnabled = true;
			batchTools.SingleItemUpgrade = batchTools.GetSingleItemUpgrade(SingleItemUpgrade.Text);
			batchTools.BuildProgressiveUpgradeList();
		}

		private void SaveCharacters(object sender, RoutedEventArgs e)
		{
			batchTools.SaveCharacters();
		}

		private void SaveCharactersAs(object sender, RoutedEventArgs e)
		{
			batchTools.SaveCharactersAsCopy();
		}
		#endregion

		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (DataGrid.SelectedItems.Count == 1 && DataGrid.SelectedIndex < batchTools.BatchCharacterList.Count)
			{
				ButtonUp.IsEnabled = DataGrid.SelectedIndex > 0;
				ButtonDown.IsEnabled = DataGrid.SelectedIndex < batchTools.BatchCharacterList.Count - 1;
			}
			else
			{
				ButtonUp.IsEnabled = false;
				ButtonDown.IsEnabled = false;
			}
		}

		private void ButtonUp_Click(object sender, RoutedEventArgs e)
		{
			if (DataGrid.SelectedItems.Count == 1)
			{
				int index = DataGrid.SelectedIndex;
				if (index > 0)
				{
					BatchCharacter b = batchTools.BatchCharacterList[index - 1];
					batchTools.BatchCharacterList.RemoveAt(index - 1);
					batchTools.BatchCharacterList.Insert(index, b);
					DataGrid_SelectionChanged(null, null);
				}
			}
		}

		private void ButtonDown_Click(object sender, RoutedEventArgs e)
		{
			if (DataGrid.SelectedItems.Count == 1)
			{
				int index = DataGrid.SelectedIndex;
				if (index < batchTools.BatchCharacterList.Count - 1)
				{
					BatchCharacter b = batchTools.BatchCharacterList[index];
					batchTools.BatchCharacterList.RemoveAt(index);
					batchTools.BatchCharacterList.Insert(index + 1, b);
					DataGrid.SelectedIndex = index + 1;
					DataGrid_SelectionChanged(null, null);
				}
			}
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			batchTools.Cancel();
		}

		private void OpenButton_Click(object sender, RoutedEventArgs e)
		{
			// we need to begin edit to force addition of new row
			DataGrid.BeginEdit();
#if SILVERLIGHT
			BatchCharacter batchCharacter = (BatchCharacter)DataGrid.SelectedItem;
#else
			BatchCharacter batchCharacter = (BatchCharacter)DataGrid.CurrentItem;
#endif

			OpenFileDialog dialog = new OpenFileDialog();
#if !SILVERLIGHT
			dialog.DefaultExt = ".xml";
#endif
			dialog.Filter = "Rawr Xml Character Files | *.xml";
			dialog.Multiselect = false;
			if (dialog.ShowDialog().GetValueOrDefault())
			{
#if SILVERLIGHT
				batchCharacter.RelativePath = dialog.File.Name;
				using (StreamReader reader = dialog.File.OpenText())
				{
					batchCharacter.Character = Character.LoadFromXml(reader.ReadToEnd());
				}
#else
				batchCharacter.RelativePath = RelativePath(dialog.FileName, AppDomain.CurrentDomain.BaseDirectory);
#endif
			}
		}

		private void ShowButton_Click(object sender, RoutedEventArgs e)
		{
#if SILVERLIGHT
			BatchCharacter batchCharacter = (BatchCharacter)DataGrid.SelectedItem;
#else
			BatchCharacter batchCharacter = (BatchCharacter)DataGrid.CurrentItem;
#endif
			MainPage.Instance.LoadBatchCharacter(batchCharacter);
			App.Current.ShowWindow(MainPage.Instance);
		}

		private void DiffButton_Click(object sender, RoutedEventArgs e)
		{
#if SILVERLIGHT
			BatchCharacter character = (BatchCharacter)DataGrid.SelectedItem;
#else
			BatchCharacter character = (BatchCharacter)DataGrid.CurrentItem;
#endif
			Character before;
#if SILVERLIGHT
			before = character.OriginalCharacter;
#else
			using (StreamReader reader = new StreamReader(character.AbsolutePath))
			{
				before = Character.LoadFromXml(reader.ReadToEnd()); // load clean version for comparison
			}
#endif
			Character after = character.Character;

			OptimizerResults results = new OptimizerResults(before, after);
			results.Closed += (object csender, EventArgs ce) =>
				{
					if (!results.DialogResult.GetValueOrDefault())
					{
						// we don't want the new character, reload the old one
						Character _character = character.Character;
						_character.IsLoading = true;
						_character.SetItems(before);
						_character.ActiveBuffs = before.ActiveBuffs;
						//_character.CurrentTalents = before.CurrentTalents; // let's not play with talents for now
						_character.IsLoading = false;
						_character.OnCalculationsInvalidated();
						character.UnsavedChanges = false; // reset the dirty flag and update ui
					}
				};
			//results.SetOptimizerScores(character.Score, character.NewScore.GetValueOrDefault(character.Score));
			results.Show();
		}
	}
}
