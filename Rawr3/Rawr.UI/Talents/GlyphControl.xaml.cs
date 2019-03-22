using System;
using System.Reflection;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rawr.UI
{
	public partial class GlyphControl : UserControl
	{

        public EventHandler TalentsChanged;

        private TalentsBase talents;
        private Character character;
        public Character Character
        {
            get { return character; }
            set
            {
                character = value;
                talents = character.CurrentTalents;
                MajorStack.Children.Clear();
                MinorStack.Children.Clear();

                List<string> relevant = Calculations.GetModel(Character.CurrentModel).GetRelevantGlyphs();

                foreach (PropertyInfo pi in talents.GetType().GetProperties())
                {
                    GlyphDataAttribute[] glyphDatas = pi.GetCustomAttributes(typeof(GlyphDataAttribute), true) as GlyphDataAttribute[];
                    if (glyphDatas.Length > 0)
                    {
                        GlyphDataAttribute glyphData = glyphDatas[0];
                        if (relevant == null || relevant.Contains(glyphData.Name))
                        {
                            CheckBox cb = new CheckBox();
                            cb.Content = glyphData.Name;
                            ToolTipService.SetToolTip(cb, glyphData.Description);
                            cb.Tag = glyphData.Index;
                            cb.IsChecked = talents.GlyphData[glyphData.Index];
                            cb.Checked += new RoutedEventHandler(cb_Checked);
                            cb.Unchecked += new RoutedEventHandler(cb_Checked);
                            if (glyphData.Major) MajorStack.Children.Add(cb);
                            else MinorStack.Children.Add(cb);
                        }
                    }
                }
            }
        }

        void cb_Checked(object sender, RoutedEventArgs e)
        {
            int index = (int)((CheckBox)sender).Tag;
            talents.GlyphData[index] = (bool)((CheckBox)sender).IsChecked;
            if (TalentsChanged != null) TalentsChanged.Invoke(this, EventArgs.Empty);
        }

		public GlyphControl()
		{
			// Required to initialize variables
			InitializeComponent();
            TheScroll.SetIsMouseWheelScrollingEnabled(true);
		}
	}
}