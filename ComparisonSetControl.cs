using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Rawr
{
    public partial class ComparisonSetControl : UserControl, IFormItemSelectionProvider
    {

        public ComparisonSetControl(FormItemComparison itemComparison)
        {
            InitializeComponent();

            _itemComparison = itemComparison;
            _deltaCharacter = new Character();

            _deltaCharacter.CalculationsInvalidated += new EventHandler(ItemChanged);

            itemButtonBack.Character = _deltaCharacter;
            itemButtonChest.Character = _deltaCharacter;
            itemButtonFeet.Character = _deltaCharacter;
            itemButtonFinger1.Character = _deltaCharacter;
            itemButtonFinger2.Character = _deltaCharacter;
            itemButtonHands.Character = _deltaCharacter;
            itemButtonHead.Character = _deltaCharacter;
            itemButtonLegs.Character = _deltaCharacter;
            itemButtonMainHand.Character = _deltaCharacter;
            itemButtonNeck.Character = _deltaCharacter;
            itemButtonOffHand.Character = _deltaCharacter;
            itemButtonRanged.Character = _deltaCharacter;
            itemButtonShirt.Character = _deltaCharacter;
            itemButtonShoulders.Character = _deltaCharacter;
            itemButtonTabard.Character = _deltaCharacter;
            itemButtonTrinket1.Character = _deltaCharacter;
            itemButtonTrinket2.Character = _deltaCharacter;
            itemButtonWaist.Character = _deltaCharacter;
            itemButtonWrist.Character = _deltaCharacter;

            itemButtonBack.FormItemSelection = this;
            itemButtonChest.FormItemSelection = this;
            itemButtonFeet.FormItemSelection = this;
            itemButtonFinger1.FormItemSelection = this;
            itemButtonFinger2.FormItemSelection = this;
            itemButtonHands.FormItemSelection = this;
            itemButtonHead.FormItemSelection = this;
            itemButtonLegs.FormItemSelection = this;
            itemButtonMainHand.FormItemSelection = this;
            itemButtonNeck.FormItemSelection = this;
            itemButtonOffHand.FormItemSelection = this;
            itemButtonRanged.FormItemSelection = this;
            itemButtonShirt.FormItemSelection = this;
            itemButtonShoulders.FormItemSelection = this;
            itemButtonTabard.FormItemSelection = this;
            itemButtonTrinket1.FormItemSelection = this;
            itemButtonTrinket2.FormItemSelection = this;
            itemButtonWaist.FormItemSelection = this;
            itemButtonWrist.FormItemSelection = this;

            ItemChanged(this, EventArgs.Empty);
        }

        private Character _deltaCharacter;
        private FormItemComparison _itemComparison;
        public Character BaseCharacter
        {
            get
            {
                return _itemComparison.Character;
            }
        }

        private Character _compositeCharacter;
        public Character CompositeCharacter
        {
            get
            {
                if (_compositeCharacter == null)
                {
                    if (BaseCharacter != null)
                    {
                        _compositeCharacter = BaseCharacter.Clone();
                        foreach (CharacterSlot cs in Character.CharacterSlots)
                        {
                            if (_deltaCharacter[cs] != null)
                            {
                                _compositeCharacter[cs] = _deltaCharacter[cs];
                            }
                        }
                    }
                    else return null;
                }
                return _compositeCharacter;
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

        private CharacterCalculationsBase _currentCalculations;
        public CharacterCalculationsBase CurrentCalculations
        {
            get
            {
                if (_currentCalculations == null)
                {
                    UpdateCalculations(this, EventArgs.Empty);
                }
                return _currentCalculations;
            }
            set { _currentCalculations = value; }
        }

        public event EventHandler CalculationsInvalidated;

        public void UpdateCalculations(object sender, EventArgs e)
        {
            _compositeCharacter = null;
            _currentCalculations = null;
            if (CompositeCharacter != null)
            {
                FormItemSelection.Character = CompositeCharacter;
                _currentCalculations = Calculations.GetCharacterCalculations(CompositeCharacter, null, true, true, true);
                FormItemSelection.CurrentCalculations = _currentCalculations;
                if (CalculationsInvalidated != null) CalculationsInvalidated(this, EventArgs.Empty);
            }
        }

        private void ItemChanged(object sender, EventArgs e)
        {
            itemButtonBack.UpdateSelectedItem();
            itemButtonChest.UpdateSelectedItem();
            itemButtonFeet.UpdateSelectedItem();
            itemButtonFinger1.UpdateSelectedItem();
            itemButtonFinger2.UpdateSelectedItem();
            itemButtonHands.UpdateSelectedItem();
            itemButtonHead.UpdateSelectedItem();
            itemButtonLegs.UpdateSelectedItem();
            itemButtonMainHand.UpdateSelectedItem();
            itemButtonNeck.UpdateSelectedItem();
            itemButtonOffHand.UpdateSelectedItem();
            itemButtonRanged.UpdateSelectedItem();
            itemButtonShirt.UpdateSelectedItem();
            itemButtonShoulders.UpdateSelectedItem();
            itemButtonTabard.UpdateSelectedItem();
            itemButtonTrinket1.UpdateSelectedItem();
            itemButtonTrinket2.UpdateSelectedItem();
            itemButtonWaist.UpdateSelectedItem();
            itemButtonWrist.UpdateSelectedItem();

            UpdateCalculations(this, EventArgs.Empty);
        }

    }
}
