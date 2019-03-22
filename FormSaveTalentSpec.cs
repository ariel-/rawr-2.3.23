using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr
{
    public partial class FormSaveTalentSpec : Form
    {

        public FormSaveTalentSpec() : this(null) { ; }

        public FormSaveTalentSpec(ICollection<SavedTalentSpec> specs)
        {
            InitializeComponent();
            talentSpecsCombo.DataSource = specs;
        }

        public SavedTalentSpec TalentSpec()
        {
            return (SavedTalentSpec)talentSpecsCombo.SelectedItem;
        }

        public string TalentSpecName()
        {
            return talentSpecsCombo.Text;
        }
    }
}
