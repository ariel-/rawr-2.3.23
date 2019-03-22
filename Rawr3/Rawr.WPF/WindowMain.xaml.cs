using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Rawr.UI;

namespace Rawr.WPF
{
	/// <summary>
	/// Interaction logic for WindowMain.xaml
	/// </summary>
	public partial class WindowMain : Window
	{
		public WindowMain()
		{
			//Application.Current.Exit += new ExitEventHandler(Current_Exit);
			InitializeComponent();
			LoadScreen.StartLoading(new EventHandler(LoadFinished));
		}

		void Current_Exit(object sender, ExitEventArgs e)
		{
			LoadScreen.SaveFiles();
		}

		private void LoadFinished(object sender, EventArgs e)
		{
			LoadScreen.Visibility = System.Windows.Visibility.Collapsed;
			RootVisual.Children.Add(new MainPage());
		}
	}
}
