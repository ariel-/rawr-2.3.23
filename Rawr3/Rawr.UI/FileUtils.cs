using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Collections.Generic;
using System.Windows.Resources;

namespace Rawr.UI
{
    public class FileUtils
    {
        public EventHandler StreamReady;
		public EventHandler ProgressUpdated;
        public List<string> Filenames { get; private set; }
        public List<string> FilesToDownload { get; private set; }
		public int Progress { get; private set; }
		public string Status { get; private set; }

        public FileUtils(string filename)
        {
			Filenames = new List<string>(new string[] { filename });
        }

		public FileUtils(string[] filenames)
		{
			Filenames = new List<string>(filenames);
		}

		public FileUtils(string[] filenames, EventHandler progressUpdated)
		{
			ProgressUpdated += progressUpdated;
			Filenames = new List<string>(filenames);
		}

		public void DownloadIfNotExists(EventHandler callback)
        {
            StreamReady += callback;
			FilesToDownload = new List<string>();
			foreach (string file in Filenames)
			{
#if !SILVERLIGHT
            if (!File.Exists(file))
#else
				if (!IsolatedStorageFile.GetUserStoreForApplication().FileExists(file))
#endif
					FilesToDownload.Add(file);
			}

			if (FilesToDownload.Count == 0)
			{
				//We have all the files, just fire the ready event
				if (StreamReady != null) StreamReady.Invoke(this, EventArgs.Empty);
			}
			else
			{
				//We need to download at least one of the files
#if !SILVERLIGHT
				Uri url = new Uri(@"http://wowrawr.com/ClientBin/DefaultDataFiles.zip", UriKind.Absolute);
#else
				Uri url = new Uri("DefaultDataFiles.zip", UriKind.Relative);
#endif
				WebClient client = new WebClient();
				client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
				client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
				client.OpenReadAsync(url);

				UpdateProgress(0, "Downloading default data files...");
			}
        }

		public void Delete()
		{
			foreach (string file in Filenames)
			{
#if !SILVERLIGHT
				if (File.Exists(file))
				{
					File.Delete(file);
				}
#else
				if (IsolatedStorageFile.GetUserStoreForApplication().FileExists(file))
				{
					IsolatedStorageFile.GetUserStoreForApplication().DeleteFile(file);
				}
#endif
			}
		}

		void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			UpdateProgress(e.ProgressPercentage, 
				string.Format("Downloading default data files ({0}% complete)", e.ProgressPercentage));
		}

		void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
		{
			if (e.Error == null)
			{
				StreamResourceInfo zipStream = new StreamResourceInfo(e.Result as Stream, null);
				foreach (string file in FilesToDownload)
				{
					UpdateProgress(0, "Decompressing " + file + "...");
					Uri part = new Uri(file, UriKind.Relative);
#if !SILVERLIGHT
                    System.IO.Packaging.Package zipFile = System.IO.Packaging.ZipPackage.Open(zipStream.Stream, FileMode.Open);
                    System.IO.Packaging.PackagePart thePart = zipFile.GetPart(part);
                    StreamReader sr = new StreamReader(thePart.GetStream());
                    string unzippedFile = sr.ReadToEnd();
					StreamWriter writer = new StreamWriter(file);
					writer.Write(unzippedFile);
					writer.Close();
#else
                    // This reading method only works in Silverlight due to the GetResourceStream not existing with 2 arguments in
                    // regular .Net-land
					StreamResourceInfo fileStream = Application.GetResourceStream(zipStream, part);
					StreamReader sr = new StreamReader(fileStream.Stream);
					string unzippedFile = sr.ReadToEnd();
					//Write it in a special way when using IsolatedStorage, due to IsolatedStorage
					//having a huge performance issue when writing small chunks
					IsolatedStorageFileStream isfs = GetFileStream(file, true);
					
					char[] charBuffer = unzippedFile.ToCharArray();
					int fileSize = charBuffer.Length;
					byte[] byteBuffer = new byte[fileSize];
					for (int i = 0; i < fileSize; i++) byteBuffer[i] = (byte)charBuffer[i];
					isfs.Write(byteBuffer, 0, fileSize);
					isfs.Close();
#endif

                    UpdateProgress(0, "Finished " + file + "...");
				}
			}
			else 
				(App.Current as App).WriteLoadProgress(e.Error.Message);
			if (StreamReady != null) StreamReady.Invoke(this, EventArgs.Empty);
		}

		private void UpdateProgress(int progress, string status)
		{
			if (progress != 0) Progress = progress;
			if (status != null) Status = status;
			if (ProgressUpdated != null) ProgressUpdated(this, EventArgs.Empty);
		}

#if !SILVERLIGHT
        public static FileStream GetFileStream(string filename, bool write)
        {
            return new FileStream(filename, write ? FileMode.Create : FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }
#else
		public static IsolatedStorageFileStream GetFileStream(string filename, bool write)
        {
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            if (write) return new IsolatedStorageFileStream(filename, FileMode.Create, isf);
            else return new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, isf);
        }
#endif

		public static bool HasQuota(int kilobytes)
		{
#if !SILVERLIGHT 
            return true;
#else
			using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
			{
				return isf.Quota >= kilobytes * 1024;
			}
#endif
		}

		public static bool EnsureQuota(int kilobytes)
		{
#if !SILVERLIGHT
            return true;
#else
			using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
			{
				long newSpace = kilobytes * 1024;
				try
				{
					return isf.IncreaseQuotaTo(newSpace);
				}
				catch { }
			}
			return false;
#endif
		}
    }
}
