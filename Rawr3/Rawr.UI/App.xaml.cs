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
    public partial class App : Application
    {
#if !SILVERLIGHT
        public new static App Current
        {
            get
            {
                return Application.Current as App;
            }
        }

        public Grid RootVisual
        {
            get
            {
                return (Grid)MainWindow.Content;
            }
            set
            {
                MainWindow.Content = value;
            }
        }

        public bool IsRunningOutOfBrowser
        {
            get
            {
                return true;
            }
        }
#endif

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
#if SILVERLIGHT
            this.UnhandledException += this.Application_UnhandledException;
            this.CheckAndDownloadUpdateCompleted += new CheckAndDownloadUpdateCompletedEventHandler(App_CheckAndDownloadUpdateCompleted);
#else
            this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
#endif

			//_timerUpdates = new System.Threading.Timer(_timerUpdates_Tick, null, 0, 60 * 60 * 1000);
			InitializeComponent();
        }

		//private System.Threading.Timer _timerUpdates;
		//private void _timerUpdates_Tick(object state)
		//{
		//	Application.Current.RootVisual.Dispatcher.BeginInvoke(SayCheckingForUpdates);
		//}

		//private void SayCheckingForUpdates()
		//{
		//	MessageBox.Show("Checking for updates...");
		//}

		private void Application_Startup(object sender, StartupEventArgs e)
        {
#if SILVERLIGHT
            Properties.NetworkSettings.UseAspx = e.InitParams.ContainsKey("UseAspx");
#endif
            Grid g = new Grid();
            LoadScreen ls = new LoadScreen();
            g.Children.Add(ls);
            RootVisual = g;
            ls.StartLoading(new EventHandler(LoadFinished));
        }

        private void LoadFinished(object sender, EventArgs e)
        {
            Grid g;
            g = RootVisual as Grid;
            g.Children.RemoveAt(0);
            g.Children.Add(new MainPage());
#if SILVERLIGHT		
			this.CheckAndDownloadUpdateAsync();
#endif
		}

#if SILVERLIGHT
		private void App_CheckAndDownloadUpdateCompleted(object sender, CheckAndDownloadUpdateCompletedEventArgs e)
		{
			if (e.UpdateAvailable)
				MessageBox.Show("A new version of Rawr has automatically been downloaded and installed! Relaunch Rawr, at your leisure, to use it!", "New version installed", MessageBoxButton.OK);
		}
#endif

		private void Application_Exit(object sender, EventArgs e)
        {
            LoadScreen.SaveFiles();
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            
        }

#if SILVERLIGHT
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }
        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
#endif
    }
}
