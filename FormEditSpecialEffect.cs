using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr
{
    public partial class FormEditSpecialEffect : Form
    {

        public Stats Stats { get; set; }
        public Trigger Trigger { get; set; }
        public float Duration { get; set; }
        public float Cooldown { get; set; }
        public float Chance { get; set; }
        public int Stacks { get; set; }

        public FormEditSpecialEffect()
            : this(new Stats(), Trigger.MeleeHit, 20f, 120f, 1f, 1)
        {
            Text = "Add Special Effect";
        }

        public FormEditSpecialEffect(Stats stats, Trigger trigger, float duration, float cooldown, float chance, int stacks)
        {
            Stats = stats.Clone();
            
            InitializeComponent();

            propertyGridStats.SelectedObject = Stats;
            cmbTrigger.DataSource = Enum.GetNames(typeof(Trigger));

            cmbTrigger.SelectedIndex = (int)trigger;
            nudDuration.Value = (decimal)duration;
            nudCooldown.Value = (decimal)cooldown;
            nudStacks.Value = (decimal)stacks;

            if (chance < 0)
            {
                nudChance.Value = (decimal)-chance;
                cmbPPM.SelectedIndex = 1;
            }
            else
            {
                nudChance.Value = (decimal)(chance * 100f);
                cmbPPM.SelectedIndex = 0;
            }
        }

        private void butOkay_Click(object sender, EventArgs e)
        {
            Trigger = (Trigger)cmbTrigger.SelectedIndex;
            Duration = (float)nudDuration.Value;
            Cooldown = (float)nudCooldown.Value;
            Stacks = (int)nudStacks.Value;
            if (cmbPPM.SelectedIndex == 1) Chance = -(float)nudChance.Value;
            else Chance = (float)nudChance.Value / 100f;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}
