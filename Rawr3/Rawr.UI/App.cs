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
using System.ComponentModel;

namespace Rawr.UI
{
    public class App : Application, INotifyPropertyChanged
    {
#if !SILVERLIGHT
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

        public static object GetFocusedElement()
        {
            return FocusManager.GetFocusedElement(Application.Current.MainWindow);
        }
#else
        public static object GetFocusedElement()
        {
            return FocusManager.GetFocusedElement();
        }
#endif

        public new static App Current
        {
            get
            {
                return Application.Current as App;
            }
        }

		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}
		#endregion

		private string _loadProgress = "";
		public string LoadProgress
		{
			get { return _loadProgress; }
			set
			{
				_loadProgress = value;
				OnPropertyChanged("LoadProgress");
			}
		}

		public void WriteLoadProgress(string message)
		{
			LoadProgress += "\r\n" + message;
		}

		public void ClearLoadProgress()
		{
			LoadProgress = string.Empty;
		}

        public virtual void OpenNewWindow(string title, Control control)
        {
        }

        public virtual void ShowWindow(Control control)
        {
        }

        public virtual void CloseWindow(Control control)
        {
        }
	}
}
