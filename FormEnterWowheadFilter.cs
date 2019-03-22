using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Rawr
{
    public partial class FormEnterWowheadFilter : Form
    {

        public FormEnterWowheadFilter()
        {
            InitializeComponent();
        }

        public string WowheadFilter
        {
            get
            {
                return textWowheadFilter.Text;
            }
        }

        private void FormEnterId_Load(object sender, EventArgs e)
        {
            textWowheadFilter.Text = "";
            this.ActiveControl = textWowheadFilter;
        }
    }
}