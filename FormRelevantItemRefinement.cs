using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr
{
    public partial class FormRelevantItemRefinement : Form
    {
        //_relevantItemTypes is used to carry info on which types
        // are utilized by the class period (as listed in instance.RelevantItemTypes)
        static private List<ItemType> _relevantItemTypes = null;

        //_RelevantItemTypes is a static piece of data which determines which item
        // types are being selected for use (have their boxes checked) Right Now. 
        static private List<ItemType> _RelevantItemTypes = null;

        //lists all the checkboxes in the form (used to track the types of items)
        private List<System.Windows.Forms.CheckBox> checkBoxes = new List<CheckBox>();

        public FormRelevantItemRefinement(Object Sender)
        {

            InitializeComponent();
            updateBoxes();

            checkBoxes.Add(this.checkBoxPlateArmor);
            checkBoxes.Add(this.checkBoxMailArmor);
            checkBoxes.Add(this.checkBoxLeatherArmor);
            checkBoxes.Add(this.checkBoxClothArmor);
            
            checkBoxes.Add(this.checkBoxDagger);
            checkBoxes.Add(this.checkBoxFistWeapon);
            checkBoxes.Add(this.checkBoxOneHandedAxe);
            checkBoxes.Add(this.checkBoxOneHandedMace);
            checkBoxes.Add(this.checkBoxOneHandedSword);

            checkBoxes.Add(this.checkBoxStaff);
            checkBoxes.Add(this.checkBoxPolearm);
            checkBoxes.Add(this.checkBoxTwoHandedAxe);
            checkBoxes.Add(this.checkBoxTwoHandedMace);
            checkBoxes.Add(this.checkBoxTwoHandedSword);

            checkBoxes.Add(this.checkBoxBow);
            checkBoxes.Add(this.checkBoxCrossbow);
            checkBoxes.Add(this.checkBoxGun);
            checkBoxes.Add(this.checkBoxThrown);
            checkBoxes.Add(this.checkBoxRelic);
            checkBoxes.Add(this.checkBoxWand);
            checkBoxes.Add(this.checkBoxShield);
            checkBoxes.Add(this.checkBoxMisc);
        }


        public void resetLists()
        {

            _relevantItemTypes = null;

            uncheckCheckBox();
            disableCheckBox();
        }

        public void updateBoxes()
        {
            if (_relevantItemTypes == null)
            {              
                _relevantItemTypes = Calculations.Instance.RelevantItemTypes;
            }
            _RelevantItemTypes = ItemFilter.GetRelevantItemTypesList(Calculations.Instance);
            foreach (CheckBox box in checkBoxes)
            {
                if (box.Equals(checkBoxRelic))
                {
                    box.Enabled = _relevantItemTypes.Contains(ItemType.Libram) || _relevantItemTypes.Contains(ItemType.Idol)
                        || _relevantItemTypes.Contains(ItemType.Totem) || _relevantItemTypes.Contains(ItemType.Sigil);
                    box.Checked = _RelevantItemTypes.Contains(ItemType.Libram) || _RelevantItemTypes.Contains(ItemType.Idol)
                        || _RelevantItemTypes.Contains(ItemType.Totem) || _RelevantItemTypes.Contains(ItemType.Sigil);
                }
                else
                {
                    box.Enabled = _relevantItemTypes.Contains((ItemType)box.Tag);
                    box.Checked = _RelevantItemTypes.Contains((ItemType)box.Tag);
                }
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            _RelevantItemTypes.Clear();
            if (_relevantItemTypes == null)
            {
                _relevantItemTypes = Calculations.Instance.RelevantItemTypes;
            }
            foreach (CheckBox box in checkBoxes)
            {
                if (box.Checked && box.Enabled)
                {
                    if (box.Equals(checkBoxRelic))
                    {
                        if (_relevantItemTypes.Contains(ItemType.Libram)) _RelevantItemTypes.Add(ItemType.Libram);
                        if (_relevantItemTypes.Contains(ItemType.Totem)) _RelevantItemTypes.Add(ItemType.Totem);
                        if (_relevantItemTypes.Contains(ItemType.Idol)) _RelevantItemTypes.Add(ItemType.Idol);
                        if (_relevantItemTypes.Contains(ItemType.Sigil)) _RelevantItemTypes.Add(ItemType.Sigil);
                    }
                    else
                    {
                        _RelevantItemTypes.Add((ItemType)box.Tag);
                    }
                }
            }
            ItemCache.OnItemsChanged();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.uncheckCheckBox();
            //it's a wash, ignore everything and make sure nothing stays checked
        }

        private void uncheckCheckBox()
        {
            foreach (CheckBox box in checkBoxes)
            {
                box.Checked = false;
            }
        }

        private void disableCheckBox()
        {
            foreach (CheckBox box in checkBoxes)
            {
                box.Enabled = false;
            }
        }
    }
}
        
   