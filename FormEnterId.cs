using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Rawr
{
    public partial class FormEnterId : Form
    {
        public int Value
        {
            get 
            {
                String input = textItemId.Text;

                Regex wowhead = new Regex(@"http://www.wowhead.com/\?item=([-+]?\d+)");
                Match m = wowhead.Match(input);

                if (m.Success)
                {
                    return int.Parse(m.Groups[1].Value);
                }

                Regex wowheadptr = new Regex(@"http://ptr.wowhead.com/\?item=([-+]?\d+)");
                m = wowheadptr.Match(input);

                if (m.Success)
                {
                    return int.Parse(m.Groups[1].Value);
                }

                Regex thottbot = new Regex(@"http://thottbot.com/i([-+]?\d+)");
                m = thottbot.Match(input);

                if (m.Success)
                {
                    return int.Parse(m.Groups[1].Value);
                }

                Regex numeric = new Regex(@"([-+]?\d+)");
                m = numeric.Match(input);

                if (m.Success)
                {
                    return int.Parse(m.Groups[1].Value);
                }

                return 0;
            }
        }

        public string ItemName
        {
            get { return textItemId.Text; }
        }

		public bool UseArmory
		{
			get { return cbArmory.Checked; }
		}

        public bool UseWowhead
        {
            get { return cbWowhead.Checked; }
        }

        public FormEnterId()
        {
            InitializeComponent();
        }

        private void FormEnterId_Load(object sender, EventArgs e)
        {
            textItemId.Text = "";
            this.ActiveControl = textItemId;
        }
    }
}