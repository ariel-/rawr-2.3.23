using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Rawr.Mage
{
    public partial class CooldownRestrictionsForm : Form
    {
        private Character character;
        public Character Character {
            get
            {
                return character;
            }
            set
            {
                character = value;
                CalculationOptionsMage calculationOptions = character.CalculationOptions as CalculationOptionsMage;
                if (calculationOptions.CooldownRestrictions != null)
                {
                    textBoxCooldownRestrictions.Text = Regex.Replace(calculationOptions.CooldownRestrictions, "(\r|\n)+", Environment.NewLine);
                }
            }
        }

        public CooldownRestrictionsForm()
        {
            InitializeComponent();
        }

        private void listBoxState_DoubleClick(object sender, EventArgs e)
        {
            string state = (string)listBoxState.SelectedItem;
            if (state != null)
            {
                textBoxCooldownRestrictions.SelectedText = state;
            }
        }

        private void buttonUpdateItems_Click(object sender, EventArgs e)
        {
            CalculationOptionsMage calculationOptions = Character.CalculationOptions as CalculationOptionsMage;
            calculationOptions.CooldownRestrictions = Regex.Replace(textBoxCooldownRestrictions.Text, "(\r|\n)+", Environment.NewLine);
            Character.OnCalculationsInvalidated();
        }

        private void textBoxCooldownRestrictions_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)))
            {
                string text = (string)e.Data.GetData(typeof(string));
                if (text != null)
                {
                    int index = textBoxCooldownRestrictions.GetCharIndexFromPosition(textBoxCooldownRestrictions.PointToClient(new Point(e.X, e.Y)));
                    if (index < textBoxCooldownRestrictions.Text.Length && textBoxCooldownRestrictions.Text[index] == '\n')
                    {
                        index++;
                    }
                    textBoxCooldownRestrictions.SelectionStart = index;
                    textBoxCooldownRestrictions.SelectionLength = 0;
                    textBoxCooldownRestrictions.SelectedText = text;
                }
            }
        }

        private Rectangle dragBoxFromMouseDown;
        private int indexOfItemUnderMouseToDrag;

        private void listBoxState_MouseDown(object sender, MouseEventArgs e)
        {
            indexOfItemUnderMouseToDrag = listBoxState.IndexFromPoint(e.X, e.Y);

            if (indexOfItemUnderMouseToDrag != ListBox.NoMatches)
            {
                Size dragSize = SystemInformation.DragSize;
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
            }
            else
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void listBoxState_MouseUp(object sender, MouseEventArgs e)
        {
            dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void listBoxState_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // if the mouse moves outside the rectangle start the drag
                if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    DragDropEffects dropEffect = listBoxState.DoDragDrop(listBoxState.Items[indexOfItemUnderMouseToDrag], DragDropEffects.Copy);
                }
            }
        }

        private void textBoxCooldownRestrictions_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(string)))
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
    }
}
