using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Rawr.CustomControls;

namespace Rawr
{
	public partial class BuffSelector : UserControl
	{
        private ExtendedToolTipCheckBox disabledToolTipControl = null;
		//phobos and deimos in the air
		public BuffSelector()
		{
			InitializeComponent();
            if (!this.DesignMode)
			{
                // don't need to BuildControls here, they'll be created when we set the first character from RebuildControls
				//BuildControls();
				Calculations.ModelChanged += new EventHandler(Calculations_ModelChanged);
                Rawr.UserControls.Options.GeneralSettings.DisplayBuffChanged += new EventHandler(GeneralSettings_DisplayBuffChanged);
                Rawr.UserControls.Options.GeneralSettings.HideProfessionsChanged += new EventHandler(GeneralSettings_HideProfessionsChanged);
                Character.RaceChanged += new EventHandler(Character_RaceChanged);
                ScrollHook.hookRec(this);
			}
		}

        void BuffSelector_MouseMove(object sender, MouseEventArgs e)
        {
            Control child = (sender as Control).GetChildAtPoint(e.Location);
            if (child != null && child.Enabled == false)
            {
                if (disabledToolTipControl != child && child is ExtendedToolTipCheckBox)
                {
                    if (disabledToolTipControl != null)
                    {
                        disabledToolTipControl.ForceHideToolTip();
                    }
                    disabledToolTipControl = child as ExtendedToolTipCheckBox;
                    disabledToolTipControl.ForceShowToolTip();
                }
            }
            else if (disabledToolTipControl != null)
            {
                disabledToolTipControl.ForceHideToolTip();
                disabledToolTipControl = null;
            }
        }

		void Calculations_ModelChanged(object sender, EventArgs e)
		{
            RebuildControls();
		}

        void Character_RaceChanged(object sender, EventArgs e)
        {
            RebuildControls();
        }

        public void RebuildControls()
        {
            BuildControls();
            LoadBuffsFromCharacter();
            UpdateEnabledStates();
            ScrollHook.hookRec(this);
        }

        void GeneralSettings_DisplayBuffChanged(object sender, EventArgs e)
        {
            foreach (CheckBox checkbox in CheckBoxes.Values)
            {
                Buff buff = checkbox.Tag as Buff;
                if (Rawr.Properties.GeneralSettings.Default.DisplayBuffSource && buff.Source != null)
                    checkbox.Text = buff.Name + " (" + buff.Source + ")";
                else
                    checkbox.Text = buff.Name;
            }
            ScrollHook.hookRec(this);
        }

        void GeneralSettings_HideProfessionsChanged(object sender, EventArgs e)
        {
            RebuildControls();
        }

        public Dictionary<Buff, CheckBox> BuffCheckBoxes
        {
            get { return CheckBoxes; }
        }
        
        //i want to be free... from desolation and despair
		Dictionary<string, GroupBox> GroupBoxes = new Dictionary<string, GroupBox>();
		Dictionary<Buff, CheckBox> CheckBoxes = new Dictionary<Buff, CheckBox>();
		private void BuildControls()
		{
			this.Controls.Clear();
			this.GroupBoxes.Clear();
			this.CheckBoxes.Clear();
			this.SuspendLayout();
			foreach (Buff buff in Buff.RelevantBuffs)
			{
				if (!GroupBoxes.ContainsKey(buff.Group))
				{
					GroupBox groupBox = new GroupBox();
					groupBox.Text = buff.Group;
					groupBox.Tag = buff.Group;
					groupBox.Font = new Font(this.Font.FontFamily, 7f);
					groupBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
					groupBox.MouseMove += new MouseEventHandler(BuffSelector_MouseMove);
					groupBox.Dock = DockStyle.Top;
					GroupBoxes.Add(buff.Group, groupBox);
					this.Controls.Add(groupBox);
					groupBox.BringToFront();
				}
			}

			List<Buff> buffs = Buff.RelevantBuffs;

            foreach (Buff buff in buffs)
            {
                if (Character == null || !Rawr.Properties.GeneralSettings.Default.HideProfEnchants || (buff.Professions != null && Character.HasProfession(buff.Professions)))
                {
                    ExtendedToolTipCheckBox checkBox = new ExtendedToolTipCheckBox();
                    checkBox.Tag = buff;
                    if (Rawr.Properties.GeneralSettings.Default.DisplayBuffSource && buff.Source != null)
                        checkBox.Text = buff.Name + " (" + buff.Source + ")";
                    else
                        checkBox.Text = buff.Name;
                    checkBox.AutoSize = true;
                    checkBox.Font = this.Font;
                    checkBox.Dock = DockStyle.Top;
                    checkBox.ToolTipText = buff.Stats.ToString();
                    checkBox.CheckedChanged += new EventHandler(checkBoxBuff_CheckedChanged);
                    GroupBoxes[buff.Group].Controls.Add(checkBox);
                    checkBox.BringToFront();
                    // only add Draenei Heroic Presence buff if Alliance
                    /*if (buff.Name.Equals("Heroic Presence") && FormMain.Instance.IsHandleCreated)
                    {
                        if (FormMain.Instance.Character.Faction == CharacterFaction.Alliance)
                            CheckBoxes.Add(buff, checkBox);
                        else
                            checkBox.Enabled = false;
                    }
                    else
                        CheckBoxes.Add(buff, checkBox);*/
                    CheckBoxes.Add(buff, checkBox);

                    foreach (Buff improvement in buff.Improvements)
                    {
                        if (Character == null || !Rawr.Properties.GeneralSettings.Default.HideProfEnchants || Character.HasProfession(improvement.Professions))
                        {
                            ExtendedToolTipCheckBox checkBoxImprovement = new ExtendedToolTipCheckBox();
                            checkBoxImprovement.Tag = improvement;
                            if (Rawr.Properties.GeneralSettings.Default.DisplayBuffSource && improvement.Source != null)
                                checkBoxImprovement.Text = improvement.Name + " (" + improvement.Source + ")";
                            else
                                checkBoxImprovement.Text = improvement.Name;
                            checkBoxImprovement.Padding = new Padding(8 + checkBoxImprovement.Padding.Left,
                                checkBoxImprovement.Padding.Top, checkBoxImprovement.Padding.Right, checkBoxImprovement.Padding.Bottom);
                            checkBoxImprovement.AutoSize = true;
                            checkBoxImprovement.Font = this.Font;
                            checkBoxImprovement.Dock = DockStyle.Top;
                            checkBoxImprovement.ToolTipText = improvement.Stats.ToString();
                            checkBoxImprovement.CheckedChanged += new EventHandler(checkBoxBuff_CheckedChanged);
                            GroupBoxes[buff.Group].Controls.Add(checkBoxImprovement);
                            checkBoxImprovement.BringToFront();
                            CheckBoxes.Add(improvement, checkBoxImprovement);
                        }
                    }
                }
            }

			foreach (GroupBox groupBox in GroupBoxes.Values)
			{
                // Make sure not to index an empty array
                if (groupBox.Controls.Count > 0) {
                    groupBox.Height = groupBox.Controls[0].Bottom + 2;
                }
                // Hide empty group boxes
                else {
                    groupBox.Hide();
                }
			}
            ScrollHook.hookRec(this);
			this.ResumeLayout();
		}


		//when will this loneliness be over
		private bool _loadingBuffsFromCharacter = false;
		private Character _character = null;
		public Character Character
		{
			get { return _character; }
			set
			{
				if (_character != null)
				{
					_character.CalculationsInvalidated -= new EventHandler(Character_ItemsChanged);
				}
				_character = value;
				
				if (Character != null)
				{
					_character.CalculationsInvalidated += new EventHandler(Character_ItemsChanged);
					LoadBuffsFromCharacter();
				}

				UpdateEnabledStates();
			}
		}

		private void LoadBuffsFromCharacter()
		{
			_loadingBuffsFromCharacter = true;
			foreach (CheckBox checkBox in CheckBoxes.Values)
				checkBox.Checked = false;
			foreach (Buff buff in Character.ActiveBuffs)
			{
                if (buff != null)
                {
                    if (CheckBoxes.ContainsKey(buff))
                        CheckBoxes[buff].Checked = true;
                }
			}
			_loadingBuffsFromCharacter = false;
		}

		void Character_ItemsChanged(object sender, EventArgs e)
		{
			if (this.InvokeRequired)
			{
                Invoke((EventHandler)Character_ItemsChanged, sender, e); 
				//InvokeHelper.Invoke(this, "Character_ItemsChanged", new object[] { null, null });
				return;
			}
			LoadBuffsFromCharacter();
			UpdateEnabledStates();
		}


		//why can't we see
		private void checkBoxBuff_CheckedChanged(object sender, EventArgs e)
		{
			if (!_loadingBuffsFromCharacter && Character != null)
			{
				UpdateEnabledStates();
				UpdateCharacterBuffs();
				Character.OnCalculationsInvalidated();
			}
		}

		//that when we bleed
		private void UpdateCharacterBuffs()
		{
			List<Buff> activeBuffs = new List<Buff>();
			foreach (CheckBox checkBox in CheckBoxes.Values)
				if (checkBox.Checked && checkBox.Enabled)
					activeBuffs.Add((checkBox.Tag as Buff));
			Character.ActiveBuffs = activeBuffs;
		}

		//we bleed the same
		private void UpdateEnabledStates()
		{
			//If I was in World War II, they'd call me Spitfire!
			List<string> currentConflictNames = new List<string>();
			foreach (CheckBox checkBox in CheckBoxes.Values)
			{
				if (checkBox.Checked)
				{
					Buff buff = checkBox.Tag as Buff;
					foreach (string conflictName in buff.ConflictingBuffs)
						if (!string.IsNullOrEmpty(conflictName) && !currentConflictNames.Contains(conflictName))
							currentConflictNames.Add(conflictName);
				}
			}

			foreach (CheckBox checkBox in CheckBoxes.Values)
			{
				checkBox.Enabled = true;
				//Buff buff = checkBox.Tag as Buff;
                Buff buff = (Buff) checkBox.Tag;
                if (Character != null && buff.Name == "Heroic Presence") {
                    if (Character.Race == CharacterRace.Draenei ||
                        Character.Faction == CharacterFaction.Horde)
                    {
                        checkBox.Enabled = false;
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(buff.SetName))
				{
					checkBox.Enabled = false;
					continue;
				}
				if (string.IsNullOrEmpty(buff.Group))
				{
					checkBox.Enabled = CheckBoxes[GetParentBuff(buff)].Checked;
					if (!checkBox.Enabled) continue;
				}
				if (!checkBox.Checked)
				{
					foreach (string buffName in buff.ConflictingBuffs)
					{
						if (currentConflictNames.Contains(buffName))
						{
							checkBox.Enabled = false;
							break;
						}
					}
				}
			}

		}

		private Buff GetParentBuff(Buff childBuff)
		{
			foreach (Buff parentBuff in Buff.RelevantBuffs)
				if (parentBuff.Improvements.Contains(childBuff))
					return parentBuff;
			return null;
		}

        public void DisableAllBuffs()
        {
            _loadingBuffsFromCharacter = true;
            foreach (CheckBox checkBox in CheckBoxes.Values)
                checkBox.Checked = false;
            _loadingBuffsFromCharacter = false;
            UpdateCharacterBuffs();
            Character.OnCalculationsInvalidated();
        }
    }
}
