using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Rawr.Forms
{
	public partial class Status : Form
	{
        private const string OVERALL_PROGRESS = "{0} of {1} Tasks have completed successfully";
        private const int MIN_HEIGHT_EXPANDED = 327;
        private const int MIN_HEIGHT_COLLAPSED = 130;
        private const int MIN_WIDTH = 440;
        private bool _Expanded = true;
		private List<StatusEventArgs> _StatusUpdates;
        private List<StatusErrorEventArgs> _StatusErrors;
        private int _LastExpandedHeight;
        private bool _AllowedToClose;
        public bool CancelPending { get; set; }

		public Status()
		{
            InitializeComponent();
            _LastExpandedHeight = 392;
            _AllowedToClose = false;
			_StatusUpdates = new List<StatusEventArgs>();
            _StatusErrors = new List<StatusErrorEventArgs>();
			StatusMessaging.StatusUpdate += new StatusMessaging.StatusUpdateDelegate(StatusMessaging_StatusUpdate);
            StatusMessaging.StatusError += new StatusMessaging.StatusErrorDelegate(StatusMessaging_StatusError);
		}

        public bool AllowedToClose
        {
            get { return _AllowedToClose; }
            set { _AllowedToClose = value; }
        }
        public bool HasErrors
        {
            get { return _StatusErrors.Count > 0; }
        }

        public void SwitchToErrorTab()
        {
            this.tabControl1.SelectedTab = this.tabControl1.TabPages[1];
        }

        private void StatusMessaging_StatusError(StatusErrorEventArgs args)
        {
            if (!this.IsDisposed && this.Visible)
            {
                _StatusErrors.Add(args);
                Invoke((RefreshErrorListDelegate)RefreshErrorList, args);
                //InvokeHelper.BeginInvoke(this, "RefreshErrorList", new object[] { args });
            }
        }

        private void StatusMessaging_StatusUpdate(StatusEventArgs args)
		{
			if (!this.IsDisposed)
			{
				bool found = false;
				if (args != null && !String.IsNullOrEmpty(args.Key))
				{
					for (int i = 0; i < _StatusUpdates.Count; i++)
					{
						if (_StatusUpdates[i].Key == args.Key)
						{
							//overwrite old status from that key
							_StatusUpdates[i] = args;
							found = true;
							break;
						}
					}
					if (!found)
					{
						_StatusUpdates.Add(args);
					}
				}

                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)RefreshTaskList);
                    //InvokeHelper.BeginInvoke(this, "RefreshTaskList", null);
                }
                else
                {
                    RefreshTaskList();
                }
			}
		}

        private delegate void RefreshErrorListDelegate(StatusErrorEventArgs args);
        private void RefreshErrorList(StatusErrorEventArgs args)
        {
            ListViewItem newError = new ListViewItem(new string[] { args.Key, args.FriendlyMessage });
            newError.Tag = args;
            ErrorListView.Items.Add(newError);
        }

		private void RefreshTaskList()
		{
			int doneCount = 0;
			for (int i = 0; i < _StatusUpdates.Count; i++)
			{
				if (_StatusUpdates[i].Done)
				{
					doneCount++;
				}
                if (TaskListView.Items.Count > i)
                {
                    // Insert a check to prevent unnecessary refreshing of identical content
                    if (!TaskListView.Items[i].SubItems["Description"].Text.Equals(_StatusUpdates[i].Description))
                    {
                        TaskListView.Items[i].SubItems["Description"].Text = _StatusUpdates[i].Description;
                    }
                }
                else
                {
                    ListViewItem item = new ListViewItem(new string[] { _StatusUpdates[i].Key, _StatusUpdates[i].Description });
                    item.SubItems[0].Name = "Key";
                    item.SubItems[1].Name = "Description";
                    TaskListView.Items.Add(item);
                }
			}
			label1.Text = string.Format(OVERALL_PROGRESS,doneCount, _StatusUpdates.Count);
			progressBar1.Value = Convert.ToInt32(decimal.Round(((decimal)doneCount / (decimal)_StatusUpdates.Count) * 100));
		}

		private void ShowHideDetails_Click(object sender, EventArgs e)
		{
			_Expanded = !_Expanded;
			if (_Expanded)
			{
                this.MinimumSize = new Size(MIN_WIDTH, MIN_HEIGHT_EXPANDED);
                this.MaximumSize = new Size(0, 0);
				this.Size = new Size(this.Size.Width,_LastExpandedHeight);
				ShowHideDetails.Text = "<< Details";
			}
			else
			{
                _LastExpandedHeight = this.Size.Height;
                this.MinimumSize = new Size(MIN_WIDTH, MIN_HEIGHT_COLLAPSED);
                this.MaximumSize = new Size(700, MIN_HEIGHT_COLLAPSED);
				this.Size = new Size(this.Size.Width,MIN_HEIGHT_COLLAPSED);
				ShowHideDetails.Text = "Details >>";
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			StatusMessaging.StatusUpdate -= new StatusMessaging.StatusUpdateDelegate(StatusMessaging_StatusUpdate);
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

        private void ErrorListView_DoubleClick(object sender, EventArgs e)
        {
            if (ErrorListView.SelectedIndices.Count > 0)
            {
                ListViewItem item = ErrorListView.Items[ErrorListView.SelectedIndices[0]];
                ErrorReport dialog = new ErrorReport(item.Tag as StatusErrorEventArgs);
                dialog.ShowDialog(this);
                dialog.Dispose();
            }
        }

        private void Status_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AllowedToClose)
            {
                e.Cancel = true;
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            CancelPending = true;
        }
	}
}
