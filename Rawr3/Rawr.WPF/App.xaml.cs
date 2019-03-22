using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Rawr.UI;

namespace Rawr.WPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Rawr.UI.App
	{

		public App()
		{
            //this.Exit += this.Application_Exit;
			
            InitializeComponent();

			//this.MainWindow = new WindowMain();
		}


		/*private void App_CheckAndDownloadUpdateCompleted(object sender, CheckAndDownloadUpdateCompletedEventArgs e)
		{
			if (e.UpdateAvailable)
				MessageBox.Show("A new version of Rawr has automatically been downloaded and installed! Relaunch Rawr, at your leisure, to use it!", "New version installed", MessageBoxButton.OK);
		}

		private void Application_Exit(object sender, EventArgs e)
		{
			LoadScreen.SaveFiles();
		}*/

        public override void OpenNewWindow(string title, System.Windows.Controls.Control control)
        {
            WindowChild window = new WindowChild();
            window.RootVisual.Children.Add(control);
            window.Title = title;
            window.Show();
        }
	}
}
