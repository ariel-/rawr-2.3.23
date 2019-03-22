using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Rawr
{
    public static class ScrollHook
    {
        public static void hookRec(Control c)
        {
            c.MouseClick += FixScroll;
            foreach (Control child in c.Controls)
            {
                hookRec(child);
            }
        }

        public static void unhookRec(Control c)
        {
            c.MouseClick -= FixScroll;
            foreach (Control child in c.Controls)
            {
                unhookRec(child);
            }
        }

        private static void FixScroll(object sender, MouseEventArgs e)
        {
            Control c = sender as Control;
            if (c != null)
            {
                c.Focus();
            }
        }
    }
}
