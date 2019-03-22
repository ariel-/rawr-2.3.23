using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr.Forms
{
    public partial class ErrorReport : Form
    {
        private StatusErrorEventArgs _errorToDisplay;

        public ErrorReport(StatusErrorEventArgs errorToDisplay)
        {
            InitializeComponent();
            _errorToDisplay = errorToDisplay;
        }

        private void ErrorReport_Load(object sender, EventArgs e)
        {
            if (_errorToDisplay != null)
            {
                StepName.Text = _errorToDisplay.Key;
                Description.Text = _errorToDisplay.FriendlyMessage;
                StepExceptionMessage.Text = _errorToDisplay.Ex == null ? "No Exception" : _errorToDisplay.Ex.Message;
            }
            if (WebRequestWrapper.LastWasFatalError)
            {
                WebClientExceptionMessage.Text = WebRequestWrapper.FatalError.Message;
            }
            else if (WebRequestWrapper.Error != null)
            {
                WebClientExceptionMessage.Text = WebRequestWrapper.Error.Message;
            }
        }

        private void CopyToClipboard_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Step Name: ");
            sb.Append(_errorToDisplay.Key);
            sb.Append(Environment.NewLine);
            sb.Append("Description: ");
            sb.Append(_errorToDisplay.FriendlyMessage);
            sb.Append(Environment.NewLine);
            if (_errorToDisplay.Ex != null)
            {
                Exception temp = _errorToDisplay.Ex;
                while (temp != null)
                {
                    sb.Append(temp.Message);
                    sb.Append(Environment.NewLine);
                    sb.Append(temp.StackTrace);
                    sb.Append(Environment.NewLine);
                    temp = temp.InnerException;
                }
            }
            if (WebRequestWrapper.LastWasFatalError)
            {
                Exception temp = WebRequestWrapper.FatalError;
                while (temp != null)
                {
                    sb.Append(temp.Message);
                    sb.Append(Environment.NewLine);
                    sb.Append(temp.StackTrace);
                    sb.Append(Environment.NewLine);
                    temp = temp.InnerException;
                }
            }
			try
			{
				Clipboard.SetText(sb.ToString(), TextDataFormat.Text);
			}
			catch { }
            MessageBox.Show("Information has been copied to the clipboard.", "Copy to Clipboard",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
    }
}
