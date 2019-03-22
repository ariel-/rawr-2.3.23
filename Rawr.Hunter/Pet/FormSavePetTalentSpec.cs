using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr.Hunter
{
    public partial class FormSavePetTalentSpec : Form
    {

        public FormSavePetTalentSpec() : this(null) { ; }

        public FormSavePetTalentSpec(ICollection<SavedPetTalentSpec> specs)
        {
            InitializeComponent();
            talentSpecsCombo.DataSource = specs;
        }

        public SavedPetTalentSpec PetTalentSpec()
        {
            return (SavedPetTalentSpec)talentSpecsCombo.SelectedItem;
        }

        public string PetTalentSpecName()
        {
            return talentSpecsCombo.Text;
        }
    }
}
