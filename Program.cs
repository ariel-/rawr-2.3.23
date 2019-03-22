using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Rawr
{
	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
#if !DEBUG
			try
			{
#endif
				bool bAppFirstInstance;
				//use the app domain base directory to allow a second copy running in a different folder.
				System.Threading.Mutex oMutex = new System.Threading.Mutex(true, "Global\\Rawr-" + AppDomain.CurrentDomain.BaseDirectory.Replace('\\','|'), out bAppFirstInstance);
				if (bAppFirstInstance)
				{
					//RawrCatIntro();
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(new FormMain());
				}
				else
				{
					MessageBox.Show("Rawr is already running and cannot be opened a second time.");
				}
				//In release mode, the GC will collect oMutex right after it is created since it is not referenced anywhere else.
				//This line prevents the GC from collecting the object until the App closes.
				GC.KeepAlive(oMutex);
#if !DEBUG
			}
			catch (Exception ex)
			{
			   MessageBox.Show(GetErrorMessage(ex), "Rawr Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
#endif
		}

		private static string GetErrorMessage(Exception ex)
		{
			string error = "\r\n\r\n\r\n" + ex.Message + "\r\n\r\n" + ex.StackTrace;
			if (error.Contains("Could not load file or assembly 'Rawr.Base") ||
				error.Contains("Rawr.FormMain.LoadModel(String"))
			{
				return "Rawr was unable to find its required data files. Please make sure that you have fully unzipped Rawr to a location that you have full permissions to (such as My Documents). " +
					"If you still have this problem, please copy (CTRL-C) and paste this into an e-mail to WarcraftRawr@gmail.com. Thanks!" + error;
			}
			if (error.Contains("Error creating window handle."))
			{
				return "Rawr encounted a serious error caused by an old version of the .NET Framework. Please download and install the latest version of the .NET Framework from Microsoft (http://www.microsoft.com/Net/Download.aspx)." +
					"If you still have this problem, please copy (CTRL-C) and paste this into an e-mail to WarcraftRawr@gmail.com. Thanks!" + error;
			}
			if (error.Contains("Verdana"))
			{
				return "Rawr encountered a serious error caused by a corrupted copy of the 'Verdana' font. Try reinstalling .NET Framework 3.5SP1 (http://www.microsoft.com/Net/Download.aspx), or getting a copy from a friend. " +
					"If you still have this problem, please copy (CTRL-C) and paste this into an e-mail to WarcraftRawr@gmail.com. Thanks!" + error;
			}
			if (error.Contains("Segou UI"))
			{
				return "Rawr encountered a serious error caused by a corrupted copy of the 'Segou UI' font. Try reinstalling .NET Framework 3.5SP1 (http://www.microsoft.com/Net/Download.aspx), or getting a copy from a friend. " +
					"If you still have this problem, please copy (CTRL-C) and paste this into an e-mail to WarcraftRawr@gmail.com. Thanks!" + error;
			}
			if (error.Contains("Configuration system failed"))
			{
				return "Rawr encountered a serious error caused by a corrupted user settings file. Please try clearing your local settings by closing Rawr, and then deleting any folders whose name starts with 'Rawr' at C:\\Documents and Settings\\[User]\\AppData\\Local\\ or C:\\Users\\[User]\\AppData\\Local\\. You will need to display hidden folders to find those folders. " +
					"If you still have this problem, please copy (CTRL-C) and paste this into an e-mail to WarcraftRawr@gmail.com. Thanks!" + error;
			}
			return "Rawr encountered a serious error. Please copy (CTRL-C) and paste this into an e-mail to WarcraftRawr@gmail.com. Thanks!" + error;
		}
	}
}

//cold ground below...
//dissatisfied clouds above...