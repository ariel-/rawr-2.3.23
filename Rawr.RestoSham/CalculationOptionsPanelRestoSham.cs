using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Xml.Serialization;

namespace Rawr.RestoSham
{
	public partial class CalculationOptionsPanelRestoSham : CalculationOptionsPanelBase
	{
		private bool _bLoading = false;

		public CalculationOptionsPanelRestoSham()
		{
			InitializeComponent();

            txtFightLength.Tag = new NumericField("FightLength", 1f, 20f, false);
            txtLatency.Tag = new NumericField("Latency", 0f, 500f, true);
            txtCleanse.Tag = new NumericField("Decurse", 1f, 300f, true);
            txtInnervates.Tag = new NumericField("Innervates", 0f, 100f, true);
            txtWSPops.Tag = new NumericField("WSPops", 0f, 20f, true);
            tbReplenishment.Tag = new NumericField("ReplenishmentPercentage", 0f, 100f, true);
            tbSurvival.Tag = new NumericField("SurvivalPerc", 0f, 100f, true);
            tbActivity.Tag = new NumericField("ActivityPerc", 1f, 100f, false);
		}

		protected override void LoadCalculationOptions()
		{
			if (Character.CalculationOptions == null)
				Character.CalculationOptions = new CalculationOptionsRestoSham();
			CalculationOptionsRestoSham options = Character.CalculationOptions as CalculationOptionsRestoSham;

			_bLoading = true;

			#region General tab page:
			txtFightLength.Text = options.FightLength.ToString();
            chkManaTide.Checked = options.ManaTideEveryCD;
			chkWaterShield.Checked = options.WaterShield;
			chkEarthShield.Checked = options.EarthShield;
			txtCleanse.Text = options.Decurse.ToString();
			cboBurstStyle.Text = options.BurstStyle.ToString();
			cboSustStyle.Text = options.SustStyle.ToString();
			cboHeroism.Text = options.Heroism.ToString();
            txtInnervates.Text = options.Innervates.ToString();
            cboTargets.Text = options.Targets.ToString();
            txtLatency.Text = options.Latency.ToString();
            txtWSPops.Text = options.WSPops.ToString();
            #region The track bars
			tbReplenishment.Value = (Int32)options.ReplenishmentPercentage;
            UpdateTrackBarLabel(tbReplenishment);
            tbSurvival.Value = (Int32)options.SurvivalPerc;
            UpdateTrackBarLabel(tbSurvival);
            tbActivity.Value = (Int32)options.ActivityPerc;
            UpdateTrackBarLabel(tbActivity);
			#endregion
			#endregion

			_bLoading = false;
		}
		#region Text box handling
		private void numericTextBox_Validated(object sender, EventArgs e)
		{
			if (_bLoading || Character == null)
				return;

			Control txtBox = sender as Control;
			if (txtBox.Tag == null)
				return;
			NumericField info = txtBox.Tag as NumericField;

			this[info.PropertyName] = float.Parse(txtBox.Text);
			Character.OnCalculationsInvalidated();
		}
		#endregion

		#region Validation on Boxes and Text Handling
		private void numericTextBox_Validating(object sender, CancelEventArgs e)
		{
			Control txtBox = sender as Control;
			if (txtBox.Tag == null)
				return;
			NumericField info = txtBox.Tag as NumericField;

			float f;
			string szError = string.Empty;
			if (!float.TryParse(txtBox.Text, out f))
				szError = "Please enter a numeric value";
			else
			{
				if (f < info.MinValue || f > info.MaxValue)
					if (f == 0f && !info.CanBeZero)
					{
						if (info.MinValue == float.MinValue)
						{
							if (info.MaxValue == float.MaxValue)
								szError = "Please enter a numeric value";
							else
								szError = "Please enter a number less than " + info.MaxValue.ToString();
						}
						else
						{
							if (info.MaxValue == float.MaxValue)
								szError = "Please enter a number larger than " + info.MinValue.ToString();
							else
								szError = "Please enter a number between " + info.MinValue.ToString() + " and " +
										  info.MaxValue.ToString();
						}
					}
			}

			if (!string.IsNullOrEmpty(szError))
			{
				e.Cancel = true;
				errorRestoSham.SetError(sender as Control, szError);
			}
		}


		public object this[string szFieldName]
		{
			get
			{
				CalculationOptionsRestoSham options = Character.CalculationOptions as CalculationOptionsRestoSham;
				Type t = options.GetType();
                PropertyInfo property = t.GetProperty(szFieldName);
				if (property != null)
					return property.GetValue(options, null);

				return null;
			}
			set
			{
				CalculationOptionsRestoSham options = Character.CalculationOptions as CalculationOptionsRestoSham;
				Type t = options.GetType();
                PropertyInfo property = t.GetProperty(szFieldName);
				if (property != null)
                    property.SetValue(options, value, null);
			}
		}
		#endregion

		#region CheckBox Handling
		private void chkManaTide_CheckedChanged(object sender, EventArgs e)
		{
			if (!_bLoading)
			{
				this["ManaTideEveryCD"] = chkManaTide.Checked;
				Character.OnCalculationsInvalidated();
			}
		}
		private void chkEarthShield_CheckedChanged(object sender, EventArgs e)
		{
			if (!_bLoading)
			{
				this["EarthShield"] = chkEarthShield.Checked;
				Character.OnCalculationsInvalidated();
			}
		}

		private void chkWaterShield_CheckedChanged(object sender, EventArgs e)
		{
			if (!_bLoading)
			{
				this["WaterShield"] = chkWaterShield.Checked;
				Character.OnCalculationsInvalidated();
			}
		}
		#endregion

		#region Trackbar Handling
		private void OnTrackBarScroll(object sender, EventArgs e)
		{
			TrackBar trackBar = sender as TrackBar;
			if (trackBar.Tag == null)
				return;

			// For now, only update the labels of track bars we think are percentages.
			if (trackBar.Minimum == 0 && trackBar.Maximum == 100)
				UpdateTrackBarLabel(trackBar);

			NumericField f = trackBar.Tag as NumericField;
			if (trackBar.Value == 0 && !f.CanBeZero)
			{
				errorRestoSham.SetError(sender as Control, "Value cannot be zero.");
				return;
			}

			if (trackBar.Value > f.MaxValue || trackBar.Value < f.MinValue)
			{
				string err = string.Format("Value must be between {0} and {1}.", f.MinValue, f.MaxValue);
				errorRestoSham.SetError(sender as Control, err);
				return;
			}

			this[f.PropertyName] = trackBar.Value;
			Character.OnCalculationsInvalidated();
		}

		private void UpdateTrackBarLabel(TrackBar trackBar)
		{
			Control[] sr = trackBar.Parent.Controls.Find(string.Format("{0}_Label", trackBar.Name), true);
			if (sr == null || sr.Length != 1)
				return;

			Label l = sr[0] as Label;
			if (l == null)
				return;

			System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(
				"\\(\\d*%\\)",
				System.Text.RegularExpressions.RegexOptions.CultureInvariant
				| System.Text.RegularExpressions.RegexOptions.Compiled
			);

			l.Text = re.Replace(l.Text, string.Format("({0}%)", trackBar.Value));
		}
		#endregion

		#region Combo Box and other Text Box Handling
		private void cboBurstStyle_TextChanged(object sender, EventArgs e)
		{
			if (!_bLoading)
			{
				this["BurstStyle"] = cboBurstStyle.Text;
				Character.OnCalculationsInvalidated();
			}
		}
		private void cboSustStyle_TextChanged(object sender, EventArgs e)
		{
			if (!_bLoading)
			{
				this["SustStyle"] = cboSustStyle.Text;
				Character.OnCalculationsInvalidated();
			}
		}
		private void cboHeroism_TextChanged(object sender, EventArgs e)
		{
			if (!_bLoading)
			{
				this["Heroism"] = cboHeroism.Text;
				Character.OnCalculationsInvalidated();
			}
		}
        private void cboDamageReceivers_TextChanged(object sender, EventArgs e)
        {
            if (!_bLoading)
            {
                this["Targets"] = cboTargets.Text;
                Character.OnCalculationsInvalidated();
            }
        }
        #endregion


	}
}
