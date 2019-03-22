using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rawr.UI
{
    public partial class WelcomeWindow : ChildWindow
    {
		public WelcomeWindow()
        {
            InitializeComponent();
			MessageText.Text = @"Welcome to the Rawr3 Beta!

First off, we'd like to say that as a development team we are proud to bring you Rawr in a web based application which works on multiple platforms. It's been alot of late nights and hard work from everyone on the team to make this happen.

This is the public beta for Rawr3. We ask you to keep the following in mind as you use it:

  a) This is a beta. There are tons of things which are going to be broken. Find them and report them on our Issue Tracker as bugs using the formatting linked below.
  b) Use the current release (Rawr 2.3.x) version as well. If Rawr3 is broken for your class/spec, Rawr2 isn't going away for a while still, please use it.
  c) This is a beta. There are TONS of things which are going to be broken. Please report issues that you find/have with the appropriate formatting (linked below).
  d) A feature request and a bug report are two different things. Know the difference.
  e) This is a beta. There will be bugs, and your browser might even crash. If you don't want to help beta test it, then don't. If you do, then realize that it's not perfect... yet.";
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Rawr.Properties.GeneralSettings.Default.WelcomeScreenSeen = true;
            this.DialogResult = true;
        }
    }
}

