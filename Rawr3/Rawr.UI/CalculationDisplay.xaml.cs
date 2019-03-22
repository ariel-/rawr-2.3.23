using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rawr.UI
{
	public partial class CalculationDisplay : UserControl
	{

        public CalculationDisplay()
		{
			InitializeComponent();
            BuildControls();
            TheScrollViewer.SetIsMouseWheelScrollingEnabled(true);
            Calculations.ModelChanged += new EventHandler(Calculations_ModelChanged);
		}

        void Calculations_ModelChanged(object sender, EventArgs e)
        {
            BuildControls();
        }

        Dictionary<string, AccordionItem> AccordionItems = new Dictionary<string, AccordionItem>();
        Dictionary<string, TextBlock> LabelLabels = new Dictionary<string, TextBlock>();
        Dictionary<string, TextBlock> ValueLabels = new Dictionary<string, TextBlock>();
        private void BuildControls()
        {
            CalculationsAccordion.Items.Clear();
            AccordionItems.Clear();
            LabelLabels.Clear();
            ValueLabels.Clear();

            string[] displayLabelConfigurationStrings = null;
            if (Calculations.Instance != null)
            {
                displayLabelConfigurationStrings = Calculations.CharacterDisplayCalculationLabels;
            }
            else
            {
                displayLabelConfigurationStrings = new string[0];
            }
            foreach (string displayLabelConfigurationString in displayLabelConfigurationStrings)
            {
                string[] displayLabelConfigurationSplit = displayLabelConfigurationString.Split(':');
                string group = displayLabelConfigurationSplit[0];
                if (!AccordionItems.ContainsKey(group))
                {
                    AccordionItem accordionItem = new AccordionItem();
                    accordionItem.Header = group;
                    accordionItem.Tag = group;
                    accordionItem.IsSelected = true;
                    StackPanel accordionSp = new StackPanel();
                    accordionSp.Margin = new Thickness(4);
                    accordionItem.Content = accordionSp;

                    AccordionItems[group] = accordionItem;
                    CalculationsAccordion.Items.Add(accordionItem);
                }

                string name = displayLabelConfigurationSplit[1];
                string[] nameSplit = name.Split('*');
                name = nameSplit[0];

                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                TextBlock labelLabel = new TextBlock();
                labelLabel.Width = 160;
                labelLabel.Text = name + (nameSplit.Length > 1 ? ": *" : ":");
                sp.Children.Add(labelLabel);
                LabelLabels[name] = labelLabel;
                if (nameSplit.Length > 1)
                {
                    ToolTipService.SetToolTip(labelLabel,  nameSplit[1]);
                }

                TextBlock labelValue = new TextBlock();
                ValueLabels[name] = labelValue;
                sp.Children.Add(labelValue);

                ((StackPanel)AccordionItems[group].Content).Children.Add(sp);
            }
        }

        public void SetCalculations(Dictionary<string, string> displayCalculationValues)
        {
            foreach (KeyValuePair<string, string> kvp in displayCalculationValues)
            {
                string[] valueSplit = kvp.Value.Split('*');
                string value = valueSplit[0];
                TextBlock label;
                if (ValueLabels.TryGetValue(kvp.Key, out label))
                {
                    if (valueSplit.Length > 1)
                    {
                        label.Text = value + " *";
                        ToolTipService.SetToolTip(label, valueSplit[1]);
                    }
                    else
                    {
                        label.Text = value;
                        ToolTipService.SetToolTip(label, null);
                    }
                }
            }
        }

	}
}