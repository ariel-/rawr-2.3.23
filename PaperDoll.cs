using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Rawr
{
    public partial class PaperDoll : UserControl
    {
        private Character _character = null;
        private Color _defaultColor = SystemColors.Control;
        private bool _loadingCharacter = false;

        public PaperDoll()
        {
            InitializeComponent();
            _character = new Character();
            itemButtonHead.SelectedItem = itemButtonWrist.SelectedItem =
            itemButtonBack.SelectedItem = itemButtonChest.SelectedItem = itemButtonFeet.SelectedItem =
            itemButtonFinger1.SelectedItem = itemButtonFinger2.SelectedItem = itemButtonHands.SelectedItem =
            itemButtonHead.SelectedItem = itemButtonRanged.SelectedItem = itemButtonLegs.SelectedItem =
            itemButtonShoulders.SelectedItem = itemButtonMainHand.SelectedItem = itemButtonOffHand.SelectedItem =
            itemButtonProjectile.SelectedItemInstance = itemButtonProjectileBag.SelectedItemInstance =
            itemButtonNeck.SelectedItemInstance = itemButtonShirt.SelectedItemInstance = itemButtonWaist.SelectedItemInstance =
            itemButtonTabard.SelectedItemInstance = itemButtonTrinket1.SelectedItemInstance = itemButtonTrinket2.SelectedItemInstance = new ItemInstance();
        }

        #region ItemSlots
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Head
        {
            get { return Character.Head ?? new ItemInstance(); }
            set { itemButtonHead.SelectedItem = value; itemButtonHead.UpdateSelectedItem(); }
        }

        public bool HeadHidden
        {
            get { return itemButtonHead.ItemHidden; }
            set { itemButtonHead.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Neck
        {
            get { return Character.Neck ?? new ItemInstance(); }
            set { itemButtonNeck.SelectedItemInstance = value; itemButtonNeck.UpdateSelectedItem(); }
        }

        public bool NeckHidden
        {
            get { return itemButtonNeck.ItemHidden; }
            set { itemButtonNeck.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Shoulders
        {
            get { return Character.Shoulders ?? new ItemInstance(); }
            set { itemButtonShoulders.SelectedItem = value; itemButtonShoulders.UpdateSelectedItem(); }
        }

        public bool ShouldersHidden
        {
            get { return itemButtonShoulders.ItemHidden; }
            set { itemButtonShoulders.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Back
        {
            get { return Character.Back ?? new ItemInstance(); }
            set { itemButtonBack.SelectedItem = value; itemButtonBack.UpdateSelectedItem(); }
        }

        public bool BackHidden
        {
            get { return itemButtonBack.ItemHidden; }
            set { itemButtonBack.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Chest
        {
            get { return Character.Chest ?? new ItemInstance(); }
            set { itemButtonChest.SelectedItem = value; itemButtonChest.UpdateSelectedItem(); }
        }

        public bool ChestHidden
        {
            get { return itemButtonChest.ItemHidden; }
            set { itemButtonChest.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Shirt
        {
            get { return Character.Shirt ?? new ItemInstance(); }
            set { itemButtonShirt.SelectedItemInstance = value; itemButtonShirt.UpdateSelectedItem(); }
        }

        public bool ShirtHidden
        {
            get { return itemButtonShirt.ItemHidden; }
            set { itemButtonShirt.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Tabard
        {
            get { return Character.Tabard ?? new ItemInstance(); }
            set { itemButtonTabard.SelectedItemInstance = value; itemButtonTabard.UpdateSelectedItem(); }
        }

        public bool TabardHidden
        {
            get { return itemButtonTabard.ItemHidden; }
            set { itemButtonTabard.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Wrist
        {
            get { return Character.Wrist ?? new ItemInstance(); }
            set { itemButtonWrist.SelectedItem = value; itemButtonWrist.UpdateSelectedItem(); }
        }

        public bool WristHidden
        {
            get { return itemButtonWrist.ItemHidden; }
            set { itemButtonWrist.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance MainHand
        {
            get { return Character.MainHand ?? new ItemInstance(); }
            set { itemButtonMainHand.SelectedItem = value; itemButtonMainHand.UpdateSelectedItem(); }
        }

        public bool MainHandHidden
        {
            get { return itemButtonMainHand.ItemHidden; }
            set { itemButtonMainHand.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance OffHand
        {
            get { return Character.OffHand ?? new ItemInstance(); }
            set { itemButtonOffHand.SelectedItem = value; itemButtonOffHand.UpdateSelectedItem(); }
        }

        public bool OffHandHidden
        {
            get { return itemButtonOffHand.ItemHidden; }
            set { itemButtonOffHand.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Ranged
        {
            get { return Character.Ranged ?? new ItemInstance(); }
            set { itemButtonRanged.SelectedItem = value; itemButtonRanged.UpdateSelectedItem(); }
        }

        public bool RangedHidden
        {
            get { return itemButtonRanged.ItemHidden; }
            set { itemButtonRanged.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Projectile
        {
            get { return Character.Projectile ?? new ItemInstance(); }
            set { itemButtonProjectile.SelectedItemInstance = value; itemButtonProjectile.UpdateSelectedItem(); }
        }

        public bool ProjectileHidden
        {
            get { return itemButtonProjectile.ItemHidden; }
            set { itemButtonProjectile.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance ProjectileBag
        {
            get { return Character.ProjectileBag ?? new ItemInstance(); }
            set { itemButtonProjectileBag.SelectedItemInstance = value; itemButtonProjectileBag.UpdateSelectedItem(); }
        }

        public bool ProjectileBagHidden
        {
            get { return itemButtonProjectileBag.ItemHidden; }
            set { itemButtonProjectileBag.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Hands
        {
            get { return Character.Hands ?? new ItemInstance(); }
            set { itemButtonHands.SelectedItem = value; itemButtonHands.UpdateSelectedItem(); }
        }

        public bool HandsHidden
        {
            get { return itemButtonHands.ItemHidden; }
            set { itemButtonHands.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Waist
        {
            get { return Character.Waist ?? new ItemInstance(); }
            set { itemButtonWaist.SelectedItemInstance = value; itemButtonWaist.UpdateSelectedItem(); }
        }

        public bool WaistHidden
        {
            get { return itemButtonWaist.ItemHidden; }
            set { itemButtonWaist.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Legs
        {
            get { return Character.Legs ?? new ItemInstance(); }
            set { itemButtonLegs.SelectedItem = value; itemButtonLegs.UpdateSelectedItem(); }
        }

        public bool LegsHidden
        {
            get { return itemButtonLegs.ItemHidden; }
            set { itemButtonLegs.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Feet
        {
            get { return Character.Feet ?? new ItemInstance(); }
            set { itemButtonFeet.SelectedItem = value; itemButtonFeet.UpdateSelectedItem(); }
        }

        public bool FeetHidden
        {
            get { return itemButtonFeet.ItemHidden; }
            set { itemButtonFeet.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Finger1
        {
            get { return Character.Finger1 ?? new ItemInstance(); }
            set { itemButtonFinger1.SelectedItem = value; itemButtonFinger1.UpdateSelectedItem(); }
        }

        public bool Finger1Hidden
        {
            get { return itemButtonFinger1.ItemHidden; }
            set { itemButtonFinger1.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Finger2
        {
            get { return Character.Finger2 ?? new ItemInstance(); }
            set { itemButtonFinger2.SelectedItem = value; itemButtonFinger2.UpdateSelectedItem(); }
        }

        public bool Finger2Hidden
        {
            get { return itemButtonFinger2.ItemHidden; }
            set { itemButtonFinger2.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Trinket1
        {
            get { return Character.Trinket1 ?? new ItemInstance(); }
            set { itemButtonTrinket1.SelectedItemInstance = value; itemButtonTrinket1.UpdateSelectedItem(); }
        }

        public bool Trinket1Hidden
        {
            get { return itemButtonTrinket1.ItemHidden; }
            set { itemButtonTrinket1.ItemHidden = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ItemInstance Trinket2
        {
            get { return Character.Trinket2 ?? new ItemInstance(); }
            set { itemButtonTrinket2.SelectedItemInstance = value; itemButtonTrinket2.UpdateSelectedItem(); }
        }

        public bool Trinket2Hidden
        {
            get { return itemButtonTrinket2.ItemHidden; }
            set { itemButtonTrinket2.ItemHidden = value; }
        }
        #endregion

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Character Character
        {
            get
            {
                if (_character == null)
                {
                    Character character = new Character();
                    Character = character;
                }
                return _character;
            }
            set
            {
                if (_character != null)
                {
                    _character.CalculationsInvalidated -= new EventHandler(_character_ItemsChanged);
                }
                _character = value;
                if (_character != null)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _character.IsLoading = true; // we do not need ItemsChanged event triggering until we call OnItemsChanged at the end

                    Character.CurrentModel = null;

                    //Calculations.CalculationOptionsPanel.Character = _character;
                    itemButtonBack.Character = itemButtonChest.Character = itemButtonFeet.Character =
                        itemButtonFinger1.Character = itemButtonFinger2.Character = itemButtonHands.Character =
                        itemButtonHead.Character = itemButtonRanged.Character = itemButtonLegs.Character =
                        itemButtonNeck.Character = itemButtonShirt.Character = itemButtonShoulders.Character =
                        itemButtonTabard.Character = itemButtonTrinket1.Character = itemButtonTrinket2.Character =
                        itemButtonWaist.Character = itemButtonMainHand.Character = itemButtonOffHand.Character =
                        itemButtonProjectile.Character = itemButtonProjectileBag.Character = itemButtonWrist.Character = _character;

                    _character.CalculationsInvalidated += new EventHandler(_character_ItemsChanged);
                    _loadingCharacter = true;

                    textBoxName.Text = Character.Name;
                    textBoxRealm.Text = Character.Realm;
                    comboBoxRegion.Text = Character.Region.ToString();
                    comboBoxRace.Text = Character.Race.ToString();
                    checkBoxEnforceGemRequirements.Checked = Character.EnforceGemRequirements;
                    {
                        comboBoxClass.Text = Character.Class.ToString();
                    }
                    _loadingCharacter = false;
                    _character.IsLoading = false;
                    _character_ItemsChanged(_character, EventArgs.Empty); // this way it won't cause extra invalidations for other listeners of the event
                    this.Cursor = Cursors.Default;
                }
            }
        }

        void _character_ItemsChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                Invoke((EventHandler)_character_ItemsChanged, sender, e);
                return;
            }
            this.Cursor = Cursors.WaitCursor;
            if (!_loadingCharacter)
            {
                itemButtonBack.UpdateSelectedItem(); itemButtonChest.UpdateSelectedItem(); itemButtonFeet.UpdateSelectedItem();
                itemButtonFinger1.UpdateSelectedItem(); itemButtonFinger2.UpdateSelectedItem(); itemButtonHands.UpdateSelectedItem();
                itemButtonHead.UpdateSelectedItem(); itemButtonRanged.UpdateSelectedItem(); itemButtonLegs.UpdateSelectedItem();
                itemButtonNeck.UpdateSelectedItem(); itemButtonShirt.UpdateSelectedItem(); itemButtonShoulders.UpdateSelectedItem();
                itemButtonTabard.UpdateSelectedItem(); itemButtonTrinket1.UpdateSelectedItem(); itemButtonTrinket2.UpdateSelectedItem();
                itemButtonWaist.UpdateSelectedItem(); itemButtonMainHand.UpdateSelectedItem(); itemButtonOffHand.UpdateSelectedItem();
                itemButtonProjectile.UpdateSelectedItem(); itemButtonProjectileBag.UpdateSelectedItem(); itemButtonWrist.UpdateSelectedItem();
            }
            if (_character.IsMetaGemActive)
                itemButtonHead.BackColor = _defaultColor;
            else
                itemButtonHead.BackColor = Color.Red;
            this.Cursor = Cursors.Default;
        }

        public void SetResultText(String message)
        {
            labResults.Text = message;
        }
    }
}
