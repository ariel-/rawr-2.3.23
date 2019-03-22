using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rawr.Warlock {
    public partial class TextInputDialog : Form {

        public static string Show(string title, string message, string start) {
            TextInputDialog d = new TextInputDialog(title, message, start);
            d.ShowDialog();
            if (d.DialogResult == DialogResult.OK) {
                return d.inputBox.Text;
            } else {
                return null;
            }
        }

        public TextInputDialog(string title, string message, string start) {

            InitializeComponent();
            Text = title;
            MessageLabel.Text = message;
            inputBox.Text = start;
        }
    }
}
