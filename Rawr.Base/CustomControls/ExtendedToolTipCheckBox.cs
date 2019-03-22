using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr.CustomControls
{
    public partial class ExtendedToolTipCheckBox : CheckBox
    {
        private ToolTip _ToolTip;
        private string _ToolTipText;

        public ExtendedToolTipCheckBox()
        {
            InitializeComponent();
            _ToolTip = new ToolTip();
            this.MouseLeave += new EventHandler(ExtendedToolTipCheckBox_MouseLeave);
            this.MouseHover += new EventHandler(ExtendedToolTipCheckBox_MouseHover);
        }

        void ExtendedToolTipCheckBox_MouseHover(object sender, EventArgs e)
        {
            ForceShowToolTip();
        }

        public void ForceShowToolTip()
        {
            if (!String.IsNullOrEmpty(_ToolTipText))
            {
                int x = PointToClient(MousePosition).X + 10;
                _ToolTip.Show(_ToolTipText, this, new Point(x, -10));
            }
        }


        public void ForceHideToolTip()
        {
            if (!String.IsNullOrEmpty(_ToolTipText))
            {
                _ToolTip.Hide(this);
            }
        }

        public string ToolTipText
        {
            get { return _ToolTipText; }
            set { _ToolTipText = value; }
        }

        private void ExtendedToolTipCheckBox_MouseLeave(object sender, EventArgs e)
        {
            ForceHideToolTip();
        }
    }
}
