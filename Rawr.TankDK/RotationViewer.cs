using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr.TankDK
{
    public partial class RotationViewer : Form
    {
        public Rotation rotation;
        public DeathKnightTalents talents;
        private CalculationOptionsTankDK calcOpts;
        private Character character;

        public RotationViewer(CalculationOptionsTankDK opts, Character ch)
        {
            InitializeComponent();
            this.calcOpts = opts;
            this.rotation = calcOpts.m_Rotation;
            this.character = ch.Clone();
            talents = character.DeathKnightTalents;

            txtBS.KeyUp += new KeyEventHandler(txtBS_KeyUp);
            txtDC.KeyUp += new KeyEventHandler(txtDC_KeyUp);
            txtDuration.KeyUp += new KeyEventHandler(txtDuration_KeyUp);
            txtFS.KeyUp += new KeyEventHandler(txtFS_KeyUp);
            txtHB.KeyUp += new KeyEventHandler(txtHB_KeyUp);
            txtHS.KeyUp += new KeyEventHandler(txtHS_KeyUp);
            txtIT.KeyUp += new KeyEventHandler(txtIT_KeyUp);
            txtNumDisease.KeyUp += new KeyEventHandler(txtNumDisease_KeyUp);
            txtOblit.KeyUp += new KeyEventHandler(txtOblit_KeyUp);
            txtPS.KeyUp += new KeyEventHandler(txtPS_KeyUp);
            txtSS.KeyUp += new KeyEventHandler(txtSS_KeyUp);
            txtPest.KeyUp += new KeyEventHandler(txtPest_KeyUp);
            txtUptime.KeyUp += new KeyEventHandler(txtUptime_KeyUp);
            txtDS.KeyUp += new KeyEventHandler(txtDS_KeyUp);
            txtHoW.KeyUp += new KeyEventHandler(txtHoW_KeyUp);
            txtRS.KeyUp += new KeyEventHandler(txtRS_KeyUp);
            txtBB.KeyUp += new KeyEventHandler(txtBB_KeyUp);
            txtDnD.KeyUp += new KeyEventHandler(txtDnD_KeyUp);

            UpdateStrikes();
        }

        void UpdateStrikes()
        {
            // Update the UI by disabling those strikes that we don't have access to.
            #region Blood
            // Heart Strike
            if (talents.HeartStrike == 0)
            {
                txtHS.Enabled = false;
                txtHS.Text = "0";
            }
            else
            {
                txtHS.Enabled = true;
            }
            // DRW
            if (talents.DancingRuneWeapon == 0)
            {
                //txtDRW.Enabled = false;
                //txtDRW.Text = "0";
            }
            #endregion
            #region Frost
            // Frost Strike
            if (talents.FrostStrike == 0)
            {
                txtFS.Enabled = false;
                txtFS.Text = "0";
            }
            else
            {
                txtFS.Enabled = true;

            }
            // Howling Blast
            if (talents.HowlingBlast == 0)
            {
                txtHB.Enabled = false;
                txtHB.Text = "0";
            }
            else
            {
                txtHB.Enabled = true;
            }
            #endregion
            #region Unholy
            // Corpse Explosion
            if (talents.CorpseExplosion == 0)
            {
                //txtCE.Enabled = false;
            }
            // Scourge Strike
            if (talents.ScourgeStrike == 0)
            {
                txtSS.Enabled = false;
                txtSS.Text = "0";
            }
            else
            {
                txtSS.Enabled = true;
            }
            // Gargoyle
            if (talents.HeartStrike == 0)
            {
                // txtGar.Enabled = false;
            }
            #endregion
        }

        void updateLabels()
        {
            float GCD = rotation.getRotationDuration();
            totalGCDs.Text = GCD + " seconds";
            if (GCD > rotation.curRotationDuration)
            {
                totalGCDs.ForeColor = Color.Red;
            }
            else totalGCDs.ForeColor = Color.Black;
            float RP = rotation.getRP(talents, character);
            netRP.Text = RP + " runic power";
            if (RP < 0)
            {
                netRP.ForeColor = Color.Red;
            }
            else netRP.ForeColor = Color.Black;
        }

        private void loadCurRotation()
        {
            txtBS.Text = rotation.BloodStrike.ToString();
            txtDC.Text = rotation.DeathCoil.ToString();
            txtDuration.Text = rotation.curRotationDuration.ToString();
            txtFS.Text = rotation.FrostStrike.ToString();
            txtHB.Text = rotation.HowlingBlast.ToString();
            txtHS.Text = rotation.HeartStrike.ToString();
            txtIT.Text = rotation.IcyTouch.ToString();
            txtNumDisease.Text = rotation.numDisease.ToString();
            txtOblit.Text = rotation.Obliterate.ToString();
            txtDS.Text = rotation.DeathStrike.ToString();
            txtPS.Text = rotation.PlagueStrike.ToString();
            txtSS.Text = rotation.ScourgeStrike.ToString();
            txtPest.Text = rotation.Pestilence.ToString();
            txtUptime.Text = rotation.diseaseUptime.ToString();
            txtHoW.Text = rotation.HornOfWinter.ToString();
            txtRS.Text = rotation.RuneStrike.ToString();
            txtBB.Text = rotation.BloodBoil.ToString();
            txtDnD.Text = rotation.DeathNDecay.ToString();
            cbManagedRP.Checked = rotation.managedRP;
            UpdateStrikes();
            updateLabels();
        }

        private void lockTxt()
        {
            txtBS.ReadOnly = true;
            txtDC.ReadOnly = true;
            txtDuration.ReadOnly = true;
            txtFS.ReadOnly = true;
            txtHB.ReadOnly = true;
            txtHS.ReadOnly = true;
            txtIT.ReadOnly = true;
            txtNumDisease.ReadOnly = true;
            txtOblit.ReadOnly = true;
            txtPS.ReadOnly = true;
            txtSS.ReadOnly = true;
            txtPest.ReadOnly = true;
            txtUptime.ReadOnly = true;
            updateLabels();
        }

        private void unLockTxt()
        {
            txtBS.ReadOnly = false;
            txtDC.ReadOnly = false;
            txtDuration.ReadOnly = false;
            txtFS.ReadOnly = false;
            txtHB.ReadOnly = false;
            txtHS.ReadOnly = false;
            txtIT.ReadOnly = false;
            txtNumDisease.ReadOnly = false;
            txtOblit.ReadOnly = false;
            txtPS.ReadOnly = false;
            txtSS.ReadOnly = false;
            txtPest.ReadOnly = false;
            txtUptime.ReadOnly = false;
            updateLabels();
        }

        private void RotationViewer_Load(object sender, EventArgs e)
        {
            // ensure that rotation actually means something.
            if (null == rotation)
            {
                return;
//                rotation = new Rotation();
//                rotation.setRotation(Rotation.Type.Frost);
            }
            if (rotation.curRotationType == Rotation.Type.Blood)
            {
                rbBlood.Checked = true;
            }
            else if (rotation.curRotationType == Rotation.Type.Custom)
            {
                rbCustom.Checked = true;
            }
            else if (rotation.curRotationType == Rotation.Type.Frost)
            {
                rbFrost.Checked = true;
            }
            else if (rotation.curRotationType == Rotation.Type.Unholy)
            {
                rbUnholy.Checked = true;
            }
            updateLabels();
        }

        private void rbUnholy_CheckedChanged(object sender, EventArgs e)
        {
            if (rbUnholy.Checked)
            {
                unLockTxt();
                rotation.setRotation(Rotation.Type.Unholy);
                loadCurRotation();
            }
            updateLabels();
        }

        private void rbFrost_CheckedChanged(object sender, EventArgs e)
        {
            if (rbFrost.Checked)
            {
                unLockTxt();
                rotation.setRotation(Rotation.Type.Frost);
                loadCurRotation();
            }
            updateLabels();
        }

        private void rbCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCustom.Checked)
            {
                unLockTxt();
                rotation.setRotation(Rotation.Type.Custom);
                loadCurRotation();
            }
            updateLabels();
        }

        private void rbBlood_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBlood.Checked)
            {
                unLockTxt();
                rotation.setRotation(Rotation.Type.Blood);
                loadCurRotation();
            }
            updateLabels();
        }

        private void cbManagedRP_CheckedChanged(object sender, EventArgs e)
        {
            // On the RP abilities need to become readonly.
            txtFS.ReadOnly = cbManagedRP.Checked;
            txtRS.ReadOnly = cbManagedRP.Checked;
            txtDC.ReadOnly = cbManagedRP.Checked;
            rotation.managedRP = cbManagedRP.Checked;
            updateLabels();
            txtFS.Text = rotation.FrostStrike.ToString();
            txtDC.Text = rotation.DeathCoil.ToString();
            txtRS.Text = rotation.RuneStrike.ToString();
        }


        //text changes, commits VALID entries into memory
        #region Text Changes

        void txtDC_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.DeathCoil = f;
            }
            catch
            {
                rotation.DeathCoil = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtBS_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.BloodStrike = f;
            }
            catch
            {
                rotation.BloodStrike = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtDuration_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.curRotationDuration = f;
            }
            catch
            {
                rotation.curRotationDuration = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtFS_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.FrostStrike = f;
            }
            catch
            {
                rotation.FrostStrike = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtHB_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.HowlingBlast = f;
            }
            catch
            {
                rotation.HowlingBlast = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtHS_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.HeartStrike = f;
            }
            catch
            {
                rotation.HeartStrike = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtIT_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.IcyTouch = f;
            }
            catch
            {
                rotation.IcyTouch = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtNumDisease_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.numDisease = f;
            }
            catch
            {
                rotation.numDisease = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtOblit_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.Obliterate = f;
            }
            catch
            {
                rotation.Obliterate = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtDS_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.DeathStrike = f;
            }
            catch
            {
                rotation.DeathStrike = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtHoW_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.HornOfWinter = f;
            }
            catch
            {
                rotation.HornOfWinter = 0f;
                t.Text = "0";
            }
            updateLabels();
        }
        void txtPS_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.PlagueStrike = f;
            }
            catch
            {
                rotation.PlagueStrike = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtSS_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.ScourgeStrike = f;
            }
            catch
            {
                rotation.ScourgeStrike = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtPest_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.Pestilence = f;
            }
            catch
            {
                rotation.Pestilence = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtUptime_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.diseaseUptime = f;
            }
            catch
            {
                rotation.diseaseUptime = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtRS_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.RuneStrike = f;
            }
            catch
            {
                rotation.RuneStrike = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtBB_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.BloodBoil = f;
            }
            catch
            {
                rotation.BloodBoil = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtDnD_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.DeathNDecay = f;
            }
            catch
            {
                rotation.DeathNDecay = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        private void txtPest_TextChanged(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;
                rotation.Pestilence = f;
            }
            catch
            {
                rotation.Pestilence = 0f;
                t.Text = "0";
            }
            updateLabels();
        }
        #endregion

    }
}
