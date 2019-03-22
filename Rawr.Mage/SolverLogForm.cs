using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Rawr.Mage
{
    public partial class SolverLogForm : Form
    {
        private static SolverLogForm instance;
        public static SolverLogForm Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SolverLogForm();
                }
                return instance;
            }
        }

        public SolverLogForm()
        {
            InitializeComponent();

            CalculationsMage.AdvancedSolverChanged += new EventHandler(CalculationsMage_AdvancedSolverChanged);
            CalculationsMage.AdvancedSolverLogUpdated += new EventHandler(CalculationsMage_AdvancedSolverLogUpdated);
        }

        void CalculationsMage_AdvancedSolverLogUpdated(object sender, EventArgs e)
        {
            UpdateTextBox();
        }

        private IAsyncResult updateEvent = null;

        public void UpdateTextBox()
        {
            if (textBoxLog.Visible)
            {
                if (textBoxLog.InvokeRequired)
                {
                    // if the last update didn't go through yet then there is no point in calling again
                    // add synchronization mechanisms if needed
                    if (updateEvent == null || updateEvent.IsCompleted)
                    {
                        updateEvent = textBoxLog.BeginInvoke((MethodInvoker)delegate
                        {
                            textBoxLog.Text = CalculationsMage.AdvancedSolverLog;
                            textBoxLog.Select(textBoxLog.Text.Length, 0);
                            textBoxLog.ScrollToCaret();
                        });
                    }
                }
                else
                {
                    updateEvent = null;
                    textBoxLog.Text = CalculationsMage.AdvancedSolverLog;
                    textBoxLog.Select(textBoxLog.Text.Length, 0);
                    textBoxLog.ScrollToCaret();
                }
            }
        }

        void CalculationsMage_AdvancedSolverChanged(object sender, EventArgs e)
        {
            UpdateCancelButton();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            CalculationsMage.CancelAsync();
        }

        private void SolverLogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void UpdateCancelButton()
        {
            if (Visible)
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        buttonCancel.Enabled = !CalculationsMage.IsSolverEnabled(null);
                    });
                }
                else
                {
                    buttonCancel.Enabled = !CalculationsMage.IsSolverEnabled(null);
                }
            }
        }

        private void SolverLogForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                UpdateTextBox();
                UpdateCancelButton();
            }
        }
    }
}
