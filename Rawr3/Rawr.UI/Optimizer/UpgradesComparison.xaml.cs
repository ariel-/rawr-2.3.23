using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Rawr.Optimizer;
using System.IO;
using System.Xml.Serialization;
#if !SILVERLIGHT
using Microsoft.Win32;
#endif

namespace Rawr.UI
{
    public partial class UpgradesComparison : ChildWindow
    {
        private Dictionary<string, ComparisonCalculationUpgrades[]> itemCalculations;
        private string[] customSubpoints;

        private CharacterSlot slot;
        public CharacterSlot Slot
        {
            get { return slot; }
            set
            {
                slot = value;
                UpdateGraph();
            }
        }

        public UpgradesComparison(TextReader reader)
        {
            InitializeComponent();
            Graph.Mode = ComparisonGraph.DisplayMode.Overall;
            Graph.Character = MainPage.Instance.Character;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SerializationData));
                SerializationData data = (SerializationData)serializer.Deserialize(reader);

                SetCustomSubpoints(data.CustomSubpoints);
                itemCalculations = new Dictionary<string, ComparisonCalculationUpgrades[]>();
                for (int i = 0; i < data.Keys.Count; i++)
                {
                    itemCalculations[data.Keys[i]] = data.ItemCalculations[i];
                }

                SlotCombo.SelectedIndex = 0;
            }
            catch (Exception /*e*/)
            {
                Close();
                MessageBox.Show("The chosen file is not a Rawr Upgrade List.", "Unable to Load Upgrade List", MessageBoxButton.OK);
            }
            finally { reader.Dispose(); }
        }

        public UpgradesComparison(Dictionary<CharacterSlot, List<ComparisonCalculationUpgrades>> upgrades, string[] customSubpoints)
        {
            InitializeComponent();
            Graph.Mode = ComparisonGraph.DisplayMode.Overall;
            Graph.Character = MainPage.Instance.Character;

            SetCustomSubpoints(customSubpoints);

            itemCalculations = new Dictionary<string, ComparisonCalculationUpgrades[]>();
            List<ComparisonCalculationUpgrades> all = new List<ComparisonCalculationUpgrades>();
            foreach (KeyValuePair<CharacterSlot, List<ComparisonCalculationUpgrades>> kvp in upgrades)
            {
                all.AddRange(kvp.Value);
                itemCalculations["Gear." + kvp.Key.ToString()] = kvp.Value.ToArray();
            }
            itemCalculations["Gear.All"] = all.ToArray();

            SlotCombo.SelectedIndex = 0;
        }

        private void SetCustomSubpoints(string[] subpoints)
        {
            if (subpoints == null) customSubpoints = new string[0];
            else customSubpoints = subpoints;

            List<string> sortOptionsList = new List<string>();
            sortOptionsList.Add("Alphabetical");
            sortOptionsList.Add("Overall");
            foreach (string s in customSubpoints) sortOptionsList.Add(s);
            SortCombo.ItemsSource = sortOptionsList;
            SortCombo.SelectedIndex = 1;
        }

        public void UpdateGraph()
        {
            if (Slot == CharacterSlot.AutoSelect) Graph.DisplayCalcs(itemCalculations["Gear.All"]);
            else Graph.DisplayCalcs(itemCalculations["Gear." + Slot.ToString()]);
        }

        private void SlotChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SlotCombo.SelectedIndex == 0) Slot = CharacterSlot.AutoSelect;
            else Slot = (CharacterSlot)SlotCombo.SelectedIndex;
        }

        private void SortChanged(object sender, SelectionChangedEventArgs e)
        {
            Graph.Sort = (ComparisonSort)(SortCombo.SelectedIndex - 2);
        }

        public class SerializationData
        {
            public string[] CustomSubpoints;
            public List<string> Keys;
            public List<ComparisonCalculationUpgrades[]> ItemCalculations;
        }

        private void SaveButton_Clicked(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Rawr Upgrade List Files|*.xml";
            sfd.DefaultExt = "*.xml";
            if (sfd.ShowDialog().GetValueOrDefault())
            {
                SerializationData data = new SerializationData();
                data.CustomSubpoints = customSubpoints;
                data.Keys = new List<string>(itemCalculations.Keys);
                data.ItemCalculations = new List<ComparisonCalculationUpgrades[]>(itemCalculations.Values);

                XmlSerializer serializer = new XmlSerializer(typeof(SerializationData));
                using (Stream fileStream = sfd.OpenFile()) serializer.Serialize(fileStream, data);
            }
        }
    }
}
