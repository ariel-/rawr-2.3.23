using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;

namespace Rawr
{
	public partial class TalentPicker : UserControl
	{
		public static readonly string SavedFilePath = 
            Path.Combine(
                Path.Combine(
                    Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), 
                    "Data"),
                "Talents.xml");

		public TalentPicker()
		{
            try
			{
				LoadTalentSpecs();
			}
			catch { }
		    try
			{
				InitializeComponent();
			}
			catch { }
		    try
			{
				this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			}
			catch { }
		}

        private SavedTalentSpecList _savedTalents;
        private void LoadTalentSpecs()
        {
            try
            {
                if (File.Exists(SavedFilePath))
                {
                    using (StreamReader reader = new StreamReader(SavedFilePath, Encoding.UTF8))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(SavedTalentSpecList));
                        _savedTalents = (SavedTalentSpecList)serializer.Deserialize(reader);
                        reader.Close();
                    }
                }
            }
            catch (Exception)
            {
                ;
            }
            if (_savedTalents == null)
            {
                _savedTalents = new SavedTalentSpecList(10);
            }
        }
        private void SaveTalentSpecs()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(SavedFilePath, false, Encoding.UTF8))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SavedTalentSpecList));
                    serializer.Serialize(writer, _savedTalents);
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
                Log.Write(ex.StackTrace);
            }
        }

		private Character _character = null;
		public Character Character
		{
			get { return _character; }
			set
			{
				if (_character != null)
				{
					_character.ClassChanged -= new EventHandler(_character_ClassChanged);
				}
				_character = value;
				if (_character != null)
				{
					_character.ClassChanged += new EventHandler(_character_ClassChanged);
                    talentTree1.CharacterClass = value.Class;
                    talentTree2.CharacterClass = value.Class;
                    talentTree3.CharacterClass = value.Class;
                    Talents = _character.CurrentTalents;
                    UpdateSavedTalents();
				}
			}
		}

		private TalentsBase _talents = null;
		public TalentsBase Talents
		{
			get { return _talents; }
			set
			{
                _talents = value;
                if (_character != null) _character.CurrentTalents = value;
				UpdateTrees();
			}
		}

        private int _tree1Count;
        private int _tree2Count;
        private int _tree3Count;

		void _character_ClassChanged(object sender, EventArgs e)
        {
            Talents = _character.CurrentTalents;
            UpdateSavedTalents();
		}


        public SavedTalentSpec CustomSpec { get; set; }
        public List<SavedTalentSpec> SpecsFor(CharacterClass charClass)
        {
            List<SavedTalentSpec> classTalents = new List<SavedTalentSpec>();
            foreach (SavedTalentSpec spec in _savedTalents)
            {
                if (spec.Class == _character.Class)
                {
                    classTalents.Add(spec);
                }
            }
            if (((SavedTalentSpec)comboBoxTalentSpec.SelectedItem).Spec == null)
            {
                CustomSpec = new SavedTalentSpec("Custom", _talents, _tree1Count, _tree2Count, _tree3Count);
                classTalents.Add(CustomSpec);
            }
            return classTalents;
        }

        public SavedTalentSpec CurrentSpec()
        {
            if (comboBoxTalentSpec.SelectedItem == null) return CustomSpec;
            else if (((SavedTalentSpec)comboBoxTalentSpec.SelectedItem).Spec == null) return CustomSpec;
            else return (SavedTalentSpec)comboBoxTalentSpec.SelectedItem;
        }

        private bool _updateSaved = false;
        private void UpdateSavedTalents()
        {
            if (_character != null)
            {
                List<SavedTalentSpec> classTalents = new List<SavedTalentSpec>();
                SavedTalentSpec current = null;
                foreach (SavedTalentSpec spec in _savedTalents)
                {
                    if (spec.Class == _character.Class) { 
                        classTalents.Add(spec);
                        if (spec.Equals(_talents)) current = spec;
                    }
                }
                if (current == null)
                {
                    current = new SavedTalentSpec("Custom", null, _tree1Count, _tree2Count, _tree3Count);
                    classTalents.Add(current);
                }
                _updateSaved = true;
                comboBoxTalentSpec.DataSource = classTalents;
                comboBoxTalentSpec.SelectedItem = current;
                _updateSaved = false;
            }
        }

		private List<string> _treeNames = new List<string>();
		private void UpdateTrees()
		{
			ClearTalentPickerItems();
			if (Talents != null)
			{
                _treeNames = new List<string>((string[])Talents.GetType().GetField("TreeNames").GetValue(Talents));
                talentTree1.CharacterClass = _character.Class;
                talentTree1.TreeName = _treeNames[0];
                tabPageTree1.Text = _treeNames[0];
                talentTree2.CharacterClass = _character.Class;
                talentTree2.TreeName = _treeNames[1];
                tabPageTree2.Text = _treeNames[1];
                talentTree3.CharacterClass = _character.Class;
                talentTree3.TreeName = _treeNames[2];
                tabPageTree3.Text = _treeNames[2];

                TalentTree currentTree;

                List<GlyphDataAttribute> majors = new List<GlyphDataAttribute>();
                List<GlyphDataAttribute> minors = new List<GlyphDataAttribute>();
                List<string> relevant = Calculations.GetModel(Character.CurrentModel).GetRelevantGlyphs();

				foreach (PropertyInfo pi in Talents.GetType().GetProperties())
				{
					TalentDataAttribute[] talentDatas = pi.GetCustomAttributes(typeof(TalentDataAttribute), true) as TalentDataAttribute[];
					if (talentDatas.Length > 0)
                    {
                        TalentDataAttribute talentData = talentDatas[0];
                        switch (talentData.Tree)
                        {
                            case 0: currentTree = talentTree1; break;
                            case 1: currentTree = talentTree2; break;
                            default: currentTree = talentTree3; break;
                        }
                        TalentItem item = new TalentItem(currentTree, talentData.Name, talentData.Row - 1, talentData.Column - 1, talentData.Index, LineWrapDescription(talentData.Description), talentData.Icon,
                            (int)pi.GetValue(Talents, null), talentData.MaxPoints, talentData.Prerequisite >= 0 ? _talentPickerItems[talentData.Prerequisite] : null);
                        _talentPickerItems[talentData.Index] = item;
                        currentTree.AddTalent(item);

						item.CurrentRankChanged += new EventHandler(item_CurrentRankChanged);
					}

                    GlyphDataAttribute[] glyphDatas = pi.GetCustomAttributes(typeof(GlyphDataAttribute), true) as GlyphDataAttribute[];
                    if (glyphDatas.Length > 0)
                    {
                        GlyphDataAttribute glyphData = glyphDatas[0];
                        if (glyphData.Major) majors.Add(glyphData);
                        else minors.Add(glyphData);
                    }
				}

                tabPageGlyphs.SuspendLayout();

                foreach (Control c in grpMajorGlyph.Controls) c.Dispose();
                grpMajorGlyph.Controls.Clear();

                int count = 0;
                foreach (GlyphDataAttribute glyph in majors)
                {
                    if (relevant == null || relevant.Contains(glyph.Name))
                    {
                        CheckBox cb = new CheckBox();
                        grpMajorGlyph.Controls.Add(cb);
                        cb.AutoSize = true;
                        cb.Text = glyph.Name;
                        cb.Tag = glyph.Index;
                        cb.Checked = Talents.GlyphData[glyph.Index];
                        cb.UseVisualStyleBackColor = true;
                        cb.Location = new Point(8, 17 + 21 * count);
                        cb.CheckedChanged += new EventHandler(this.glyph_ValueChanged);
                        tooltipGlyph.SetToolTip(cb, glyph.Description);
                        count++;
                    }
                }
                if (count == 0) grpMajorGlyph.Hide();
                else grpMajorGlyph.Show();
                count = 0;

                foreach (Control c in grpMinorGlyph.Controls) c.Dispose();
                grpMinorGlyph.Controls.Clear();

                foreach (GlyphDataAttribute glyph in minors)
                {
                    if (relevant == null || relevant.Contains(glyph.Name))
                    {
                        CheckBox cb = new CheckBox();
                        grpMinorGlyph.Controls.Add(cb);
                        cb.AutoSize = true;
                        cb.Text = glyph.Name;
                        cb.Tag = glyph.Index;
                        cb.Checked = Talents.GlyphData[glyph.Index];
                        cb.UseVisualStyleBackColor = true;
                        cb.Location = new Point(6, 19 + 23 * count);
                        cb.CheckedChanged += new EventHandler(this.glyph_ValueChanged);
                        tooltipGlyph.SetToolTip(cb, glyph.Description);
                        count++;
                    }
                }

                if (count == 0) grpMinorGlyph.Hide();
                else grpMinorGlyph.Show();

                tabPageGlyphs.ResumeLayout();

                talentTree1.Redraw();
                talentTree2.Redraw();
                talentTree3.Redraw();
                item_CurrentRankChanged(null, null);
			}
		}

		private string[] LineWrapDescription(string[] descriptions)
		{
			List<string> wrappedDescriptions = new List<string>();
			foreach (string description in descriptions)
			{
				string[] lines = description.Replace("\t","").Split(new string[] { "\r\n" }, StringSplitOptions.None);
				List<string> wrappedLines = new List<string>();
				foreach (string line in lines)
				{
					string lineRemaining = line;
					StringBuilder sbLine = new StringBuilder();
					while (lineRemaining.Length > 70)
					{
						sbLine.Append(lineRemaining.Substring(0, lineRemaining.LastIndexOf(' ', 70)) + "\r\n");
						lineRemaining = lineRemaining.Substring(lineRemaining.LastIndexOf(' ', 70) + 1);
					}
					sbLine.Append(lineRemaining);
					wrappedLines.Add(sbLine.ToString());
				}
				wrappedDescriptions.Add(string.Join("\r\n", wrappedLines.ToArray()));
			}
			return wrappedDescriptions.ToArray();
		}

        private TalentItem[] _talentPickerItems = new TalentItem[100];
        private void ClearTalentPickerItems()
        {
            foreach (TalentItem item in _talentPickerItems)
            {
                if (item != null)
                {
                    item.CurrentRankChanged -= new EventHandler(item_CurrentRankChanged);
                }
            }
            _talentPickerItems = new TalentItem[100];
            talentTree1.Reset();
            talentTree2.Reset();
            talentTree3.Reset();
        }

        private void glyph_ValueChanged(object sender, EventArgs e) {
            if (Character.IsLoading) return;
            Talents.GlyphData[(int)((CheckBox)sender).Tag] = ((CheckBox)sender).Checked;
            UpdateSavedTalents();
            _character.OnTalentChange();
            _character.OnCalculationsInvalidated();
        }

        private void item_CurrentRankChanged(object sender, EventArgs e)
        {
			TalentItem item = sender as TalentItem;

            _tree1Count = talentTree1.TotalPoints();
            _tree2Count = talentTree2.TotalPoints();
            _tree3Count = talentTree3.TotalPoints();
			if (!Character.IsLoading)
			{
				if (item != null)
				{
					Talents.Data[item.Index] = item.CurrentRank;
				}
				UpdateSavedTalents();
                _character.OnTalentChange();
				_character.OnCalculationsInvalidated();
			}
			if( _tree1Count + _tree2Count + _tree3Count < _character.Level-9 )
				comboBoxTalentSpec.BackColor = Color.FromArgb(255,255,128);
			else if ( _tree1Count + _tree2Count + _tree3Count > _character.Level-9 )
				comboBoxTalentSpec.BackColor = Color.FromArgb(255,128,128);
			else
				comboBoxTalentSpec.BackColor = Color.White;
        }

        private void comboBoxTalentSpec_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((SavedTalentSpec)comboBoxTalentSpec.SelectedItem).Spec == null)
            {
                talentSpecButton.Text = "Save";
            }
            else
            {
                talentSpecButton.Text = "Delete";
                if (!_updateSaved) Talents = ((SavedTalentSpec)comboBoxTalentSpec.SelectedItem).TalentSpec();
            }
        }

        private void talentSpecButton_Click(object sender, EventArgs e)
        {
            if (((SavedTalentSpec)comboBoxTalentSpec.SelectedItem).Spec == null)
            {
                List<SavedTalentSpec> classTalents = new List<SavedTalentSpec>();
                foreach (SavedTalentSpec spec in _savedTalents)
                {
                    if (spec.Class == _character.Class) classTalents.Add(spec);
                }
                FormSaveTalentSpec form = new FormSaveTalentSpec(classTalents);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    SavedTalentSpec spec = form.TalentSpec();
                    String specName = form.TalentSpecName();
                    if (spec == null)
                    {
                        spec = new SavedTalentSpec(specName, _talents, _tree1Count, _tree2Count, _tree3Count);
                        _savedTalents.Add(spec);
                    }
                    else spec.Spec = _talents.ToString();
                    UpdateSavedTalents();
                    SaveTalentSpecs();
                    _character.OnCalculationsInvalidated();
                }
                form.Dispose();
            }
            else
            {
                _savedTalents.Remove((SavedTalentSpec)comboBoxTalentSpec.SelectedItem);
                UpdateSavedTalents();
                SaveTalentSpecs();
                _character.OnCalculationsInvalidated();
            }
        }
	}
}
