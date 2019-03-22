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

namespace Rawr.UI
{
    public partial class RelevantItemRefinement : ChildWindow
    {

        private List<ItemType> modelRelevant;
        private List<ItemType> userRelevant;

        private List<CheckBox> checkBoxes;

        public RelevantItemRefinement()
        {
            InitializeComponent();

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
        }

        public void UpdateBoxes()
        {
            modelRelevant = Calculations.Instance.RelevantItemTypes;
            userRelevant = ItemFilter.GetRelevantItemTypesList(Calculations.Instance);
            foreach (CheckBox box in checkBoxes)
            {
                if (box == CheckBoxRelic)
                {
                    box.IsEnabled = modelRelevant.Contains(ItemType.Libram) || modelRelevant.Contains(ItemType.Idol)
                      || modelRelevant.Contains(ItemType.Totem) || modelRelevant.Contains(ItemType.Sigil);
                    box.IsChecked = userRelevant.Contains(ItemType.Libram) || userRelevant.Contains(ItemType.Idol)
                        || userRelevant.Contains(ItemType.Totem) || userRelevant.Contains(ItemType.Sigil);
                }
                else
                {
                    box.IsEnabled = modelRelevant.Contains((ItemType)Enum.Parse(typeof(ItemType), (string)box.Tag, true));
                    box.IsChecked = userRelevant.Contains((ItemType)Enum.Parse(typeof(ItemType), (string)box.Tag, true));
                }
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            userRelevant.Clear();
            modelRelevant = Calculations.Instance.RelevantItemTypes;
            foreach (CheckBox box in checkBoxes)
            {
                if (box.IsChecked.GetValueOrDefault(false) && box.IsEnabled)
                {
                    if (box == CheckBoxRelic)
                    {
                        if (modelRelevant.Contains(ItemType.Libram)) userRelevant.Add(ItemType.Libram);
                        if (modelRelevant.Contains(ItemType.Totem)) userRelevant.Add(ItemType.Totem);
                        if (modelRelevant.Contains(ItemType.Idol)) userRelevant.Add(ItemType.Idol);
                        if (modelRelevant.Contains(ItemType.Sigil)) userRelevant.Add(ItemType.Sigil);
                    }
                    else
                    {
                        userRelevant.Add((ItemType)Enum.Parse(typeof(ItemType), (string)box.Tag, true));
                    }
                }
            }
            ItemCache.OnItemsChanged();
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

