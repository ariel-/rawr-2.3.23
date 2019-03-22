using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Rawr
{
	public partial class GemmingTemplateControl : UserControl
	{
		public GemmingTemplateControl()
		{
			InitializeComponent();
		}

		private GemmingTemplate _gemmingTemplate;
		public GemmingTemplate GemmingTemplate
		{
			get { return _gemmingTemplate; }
			set
			{
				_gemmingTemplate = value;
				ReloadData();
			}
		}

		private void ReloadData()
		{
			try
			{
				_loading = true;
				bool isCustom = _gemmingTemplate.Group == "Custom";
				buttonDelete.Visible = isCustom;
				checkBoxEnabled.Checked = _gemmingTemplate.Enabled;
				itemButtonRed.SelectedItem = _gemmingTemplate.RedGem;
				itemButtonYellow.SelectedItem = _gemmingTemplate.YellowGem;
				itemButtonBlue.SelectedItem = _gemmingTemplate.BlueGem;
				itemButtonPrismatic.SelectedItem = _gemmingTemplate.PrismaticGem;
				itemButtonMeta.SelectedItem = _gemmingTemplate.MetaGem;
				itemButtonRed.ReadOnly = itemButtonYellow.ReadOnly = itemButtonBlue.ReadOnly =
					itemButtonPrismatic.ReadOnly = itemButtonMeta.ReadOnly = !isCustom;
			}
			finally
			{
				_loading = false;
			}
		}

		private void itemButton_SelectedItemChanged(object sender, EventArgs e)
		{
			if (!_loading)
			{
				_gemmingTemplate.RedGem = itemButtonRed.SelectedItem;
				_gemmingTemplate.YellowGem = itemButtonYellow.SelectedItem;
				_gemmingTemplate.BlueGem = itemButtonBlue.SelectedItem;
				_gemmingTemplate.PrismaticGem = itemButtonPrismatic.SelectedItem;
				_gemmingTemplate.MetaGem = itemButtonMeta.SelectedItem;
			}
		}

		public bool GemmingTemplateEnabled
		{
			get { return checkBoxEnabled.Checked; }
			set
			{
				try
				{
					_loading = true;
					checkBoxEnabled.Checked = value;
				}
				finally
				{
					_loading = false;
				}
			}
		}

		private bool _loading = false;
		public event EventHandler GemmingTemplateEnabledChanged;
		private void checkBoxEnabled_CheckedChanged(object sender, EventArgs e)
		{
			_gemmingTemplate.Enabled = checkBoxEnabled.Checked;
			if (!_loading && GemmingTemplateEnabledChanged != null) GemmingTemplateEnabledChanged(this, EventArgs.Empty);
		}

		public event EventHandler DeleteClicked;
		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (DeleteClicked != null) DeleteClicked(this, EventArgs.Empty);
		}

	}
}
