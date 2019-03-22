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
using System.Windows.Data;

namespace Rawr.UI
{
    public partial class GemmingTemplates : ChildWindow
    {

        private Action<Item> gemCallback;
        private Action<Item> metaCallback;

        private bool isLoading;

        private Dictionary<string, CheckBox> templateGroupChecks;
        private Dictionary<string, StackPanel> templateItems;
        private Character character;
        public GemmingTemplates(Character c)
        {
            isLoading = true;
            character = c;
            InitializeComponent();            
            
            GemmingsShownNum.Value = Properties.GeneralSettings.Default.CountGemmingsShown;

            ComparisonGemList.Slot = CharacterSlot.Gems;
            ComparisonGemList.Character = c;
            ComparisonGemList.SelectedItemChanged += new EventHandler(ComparisonGemList_SelectedItemChanged);
            
            ComparisonMetaList.Slot = CharacterSlot.Metas;
            ComparisonMetaList.Character = c;
            ComparisonMetaList.SelectedItemChanged += new EventHandler(ComparisonMetaList_SelectedItemChanged);

            templateItems = new Dictionary<string, StackPanel>();
            templateItems["Custom"] = CustomStack;
            templateGroupChecks = new Dictionary<string, CheckBox>();
            templateGroupChecks["Custom"] = CustomCheck;
           
            foreach (GemmingTemplate template in character.CurrentGemmingTemplates)
            {
                GemmingTemplateItem templateItem = new GemmingTemplateItem(this, template);
                if (!templateItems.ContainsKey(template.Group))
                {
                    Expander groupExpander = new Expander();
                    CheckBox groupCheckBox = new CheckBox();
                    groupCheckBox.Content = template.Group;
                    groupCheckBox.Checked += new RoutedEventHandler(groupCheckBox_Checked);
                    groupCheckBox.Indeterminate += new RoutedEventHandler(groupCheckBox_Checked);
                    groupCheckBox.Unchecked += new RoutedEventHandler(groupCheckBox_Checked);
                    templateGroupChecks[template.Group] = groupCheckBox;
                    groupExpander.Header = groupCheckBox;
                    GroupStack.Children.Add(groupExpander);

                    StackPanel stack = new StackPanel();
                    groupExpander.Content = stack;
                    templateItems[template.Group] = stack;
                }              
                templateItems[template.Group].Children.Add(templateItem);
            }
            foreach (GemmingTemplate template in character.CustomGemmingTemplates)
            {
                GemmingTemplateItem templateItem = new GemmingTemplateItem(this, template);
                templateItems[template.Group].Children.Add(templateItem);
            }
            isLoading = false;
            foreach (string group in templateGroupChecks.Keys) UpdateGroupChecked(group);
        }

        private void ComparisonMetaList_SelectedItemChanged(object sender, EventArgs e)
        {
            if (metaCallback != null) metaCallback(ComparisonMetaList.SelectedItem);
        }

        private void ComparisonGemList_SelectedItemChanged(object sender, EventArgs e)
        {
            if (gemCallback != null) gemCallback(ComparisonGemList.SelectedItem);
        }

        private void groupCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!isLoading)
            {
                CheckBox check = sender as CheckBox;
                bool enabled = check.IsChecked.GetValueOrDefault();
                foreach (UIElement element in templateItems[check.Content.ToString()].Children)
                {
                    GemmingTemplateItem item = element as GemmingTemplateItem;
                    item.template.Enabled = enabled;
                }
            }
        }

        public void UpdateGroupChecked(string group)
        {
            isLoading = true;

            bool someChecked = false;
            bool allChecked = true;
            foreach (UIElement element in templateItems[group].Children)
            {
                GemmingTemplateItem item = element as GemmingTemplateItem;
                someChecked = item.template.Enabled || someChecked;
                allChecked = item.template.Enabled && allChecked;
            }
            if (allChecked) templateGroupChecks[group].IsChecked = true;
            else if (someChecked) templateGroupChecks[group].IsChecked = null;
            else templateGroupChecks[group].IsChecked = false;
            isLoading = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ItemCache.OnItemsChanged();
            this.DialogResult = true;
        }

        public void GemButtonClick(Item gem, Control relativeTo, Action<Item> callback)
        {
            gemCallback = null;
            ComparisonGemList.SelectedItem = gem;

            GeneralTransform gt = relativeTo.TransformToVisual(LayoutRoot);
            Point offset = gt.Transform(new Point(relativeTo.ActualWidth + 4, 0));
            GemPopup.VerticalOffset = offset.Y;
            GemPopup.HorizontalOffset = offset.X;

            ComparisonGemList.Measure(App.Current.RootVisual.RenderSize);

            GeneralTransform transform = relativeTo.TransformToVisual(App.Current.RootVisual);
            double distBetweenBottomOfPopupAndBottomOfWindow =
                App.Current.RootVisual.DesiredSize.Height -
                transform.Transform(new Point(0, ComparisonGemList.DesiredSize.Height)).Y;
            if (distBetweenBottomOfPopupAndBottomOfWindow < 0)
            {
                GemPopup.VerticalOffset += distBetweenBottomOfPopupAndBottomOfWindow;
            }

            ComparisonGemList.IsShown = true;
            GemPopup.IsOpen = true;
            ComparisonGemList.Focus();
            gemCallback = callback;
        }

        public void MetaButtonClick(Item meta, Control relativeTo, Action<Item> callback)
        {
            metaCallback = null;
            ComparisonMetaList.SelectedItem = meta;

            GeneralTransform gt = relativeTo.TransformToVisual(LayoutRoot);
            Point offset = gt.Transform(new Point(relativeTo.ActualWidth + 4, 0));
            MetaPopup.VerticalOffset = offset.Y;
            MetaPopup.HorizontalOffset = offset.X;

            ComparisonMetaList.Measure(App.Current.RootVisual.RenderSize);

            GeneralTransform transform = relativeTo.TransformToVisual(App.Current.RootVisual);
            double distBetweenBottomOfPopupAndBottomOfWindow =
                App.Current.RootVisual.DesiredSize.Height -
                transform.Transform(new Point(0, ComparisonMetaList.DesiredSize.Height)).Y;
            if (distBetweenBottomOfPopupAndBottomOfWindow < 0)
            {
                MetaPopup.VerticalOffset += distBetweenBottomOfPopupAndBottomOfWindow;
            }

            ComparisonMetaList.IsShown = true;
            MetaPopup.IsOpen = true;
            ComparisonMetaList.Focus();
            metaCallback = callback;
        }

        private void GemmingsShownChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Properties.GeneralSettings.Default.CountGemmingsShown = (int)e.NewValue;
        }

        private void AddTemplate(object sender, RoutedEventArgs e)
        {
            GemmingTemplate newTemplate = new GemmingTemplate() { Group = "Custom", Enabled = true, Model = Calculations.Instance.Name };
            character.CustomGemmingTemplates.Add(newTemplate);
            
            GemmingTemplateItem newItem = new GemmingTemplateItem(this, newTemplate);
            CustomStack.Children.Add(newItem);
            UpdateGroupChecked(newTemplate.Group);
        }

        public void RemoveTemplate(GemmingTemplateItem item)
        {
            if (item.isCustom)
            {
                CustomStack.Children.Remove(item);
                character.CustomGemmingTemplates.Remove(item.template);
                UpdateGroupChecked(item.template.Group);
            }
        }
    }
}