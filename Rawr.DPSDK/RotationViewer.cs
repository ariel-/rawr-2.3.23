using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr.DPSDK
{
    public partial class RotationViewer : Form
    {
        public Rotation rotation;
        public DeathKnightTalents talents;
        CalculationOptionsDPSDK calcOpts;
        Character character;

        public RotationViewer(CalculationOptionsDPSDK calcOpts, Character character)
        {
            InitializeComponent();
            this.calcOpts = calcOpts;
            rotation = calcOpts.rotation;
            talents = character.DeathKnightTalents;
            this.character = character;

         //   cbPresence.Items.Clear();
         //   cbPresence.Items.Add(CalculationOptionsDPSDK.Presence.Blood.ToString());
         //   cbPresence.Items.Add(CalculationOptionsDPSDK.Presence.Unholy.ToString());

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
            txtGF.KeyUp += new KeyEventHandler(txtGF_KeyUp);
            txtUptime.KeyUp += new KeyEventHandler(txtUptime_KeyUp);
            txtDS.KeyUp += new KeyEventHandler(txtDS_KeyUp);
            txtHoW.KeyUp += new KeyEventHandler(txtHoW_KeyUp);
        }

        void updateLabels()
        {
            rotation.getGCDTime();
            totalGCDs.Text = rotation.getGCDTime() + " seconds";
            if (rotation.getGCDTime() > rotation.CurRotationDuration)
            {
                totalGCDs.ForeColor = Color.Red;
            }
            else totalGCDs.ForeColor = Color.Black;
            netRP.Text = rotation.getRP(talents, character) + " runic power";
            if (rotation.getRP(talents, character) < 0)
            {
                netRP.ForeColor = Color.Red;
            }
            else netRP.ForeColor = Color.Black;

            txtFS.Text = rotation.FrostStrike.ToString();
            txtDC.Text = rotation.DeathCoil.ToString();
        }

        private void loadCurRotation()
        {
            txtBS.Text = rotation.BloodStrike.ToString();
            txtDC.Text = rotation.DeathCoil.ToString();
            txtDuration.Text = rotation.CurRotationDuration.ToString();
            txtFS.Text = rotation.FrostStrike.ToString();
            txtHB.Text = rotation.HowlingBlast.ToString();
            txtHS.Text = rotation.HeartStrike.ToString();
            txtIT.Text = rotation.IcyTouch.ToString();
            txtNumDisease.Text = rotation.NumDisease.ToString();
            txtOblit.Text = rotation.Obliterate.ToString();
            txtDS.Text = rotation.DeathStrike.ToString();
            txtPS.Text = rotation.PlagueStrike.ToString();
            txtSS.Text = rotation.ScourgeStrike.ToString();
            txtPest.Text = rotation.Pestilence.ToString();
            txtUptime.Text = rotation.DiseaseUptime.ToString();
            txtHoW.Text = rotation.Horn.ToString();
            cbManagedRP.Checked = rotation.ManagedRP;
            txtGF.Text = rotation.GhoulFrenzy.ToString();
            if (rotation.presence == CalculationOptionsDPSDK.Presence.Unholy)
            {
                rbUnholyPresence.Checked = true;
            }
            else if (rotation.presence == CalculationOptionsDPSDK.Presence.Blood)
            {
                rbBloodPresence.Checked = true;
            }
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
        private void rbUnholyPresence_CheckedChanged(object sender, EventArgs e)
        {
            if (rbUnholyPresence.Checked)
            {
                rotation.presence = CalculationOptionsDPSDK.Presence.Unholy;
            }
            updateLabels();
        }
        private void rbBloodPresence_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBloodPresence.Checked)
            {
                rotation.presence = CalculationOptionsDPSDK.Presence.Blood;
            }
            updateLabels();
        }

        //text changes, commits VALID entries into memory

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
                rotation.CurRotationDuration = f;
            }
            catch
            {
                rotation.CurRotationDuration = 0f;
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
                rotation.NumDisease = f;
            }
            catch
            {
                rotation.NumDisease = 0f;
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
                rotation.Horn = f;
            }
            catch
            {
                rotation.Horn = 0f;
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
                rotation.DiseaseUptime = f;
            }
            catch
            {
                rotation.DiseaseUptime = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        void txtGF_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d= double.Parse(t.Text);
                float f= (float)d;
                rotation.GhoulFrenzy = f;
            }
            catch
            {
                rotation.GhoulFrenzy = 0f;
                t.Text = "0";
            }
        }

        void txtGargoyleDuration_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                double d = double.Parse(t.Text);
                float f = (float)d;

                if (f < 0f)
                {
                    f = 0f;
                }
                else if (f > 30f)
                {
                    f = 30f;
                }
                t.Text = f.ToString();

                rotation.GargoyleDuration = f;
            }
            catch
            {
                rotation.GargoyleDuration = 0f;
                t.Text = "0";
            }
            updateLabels();
        }

        private void cbManagedRP_CheckedChanged(object sender, EventArgs e)
        {
                txtFS.ReadOnly = cbManagedRP.Checked;
                txtDC.ReadOnly = cbManagedRP.Checked;
                rotation.ManagedRP = cbManagedRP.Checked;
                updateLabels();
                txtFS.Text = rotation.FrostStrike.ToString();
                txtDC.Text = rotation.DeathCoil.ToString();
        }
    }
}
