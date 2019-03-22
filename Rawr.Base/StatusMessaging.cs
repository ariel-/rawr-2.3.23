using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr
{
	public class StatusEventArgs : EventArgs
	{
		private string _key;
		private string _description;
		private bool _done;

		public StatusEventArgs(string key, string description)
		{
			_key = key;
			_description = description;
			_done = false;
		}

		public StatusEventArgs(string key, string description, bool done)
		{
			_key = key;
			_description = description;
			_done = done;
		}

		public string Key
		{
			get { return _key; }
		}

		public string Description
		{
			get { return _description; }
		}

		public bool Done
		{
			get { return _done; }
		}
	}

    public class StatusErrorEventArgs : EventArgs
    {
        private string _key;
        private Exception _ex;
        private string _friendlyMessage;

        public StatusErrorEventArgs(string key, Exception ex, string friendlyMessage)
        {
            _key = key;
            _ex = ex;
            _friendlyMessage = friendlyMessage;
        }

        public string Key
        {
            get { return _key; }
        }
        public Exception Ex
        {
            get { return _ex; }
        }

        public string FriendlyMessage
        {
            get { return _friendlyMessage; }
        }
    }

	public static class StatusMessaging
	{
		public static bool Ready = false;
        public delegate void StatusErrorDelegate(StatusErrorEventArgs args);
        public static event StatusErrorDelegate StatusError;

		public delegate void StatusUpdateDelegate(StatusEventArgs args);
		public static event StatusUpdateDelegate StatusUpdate;

		public static void UpdateStatus(string key, string description)
		{
			StatusUpdateDelegate del = StatusMessaging.StatusUpdate;
			if (Ready && del != null)
			{
				del(new StatusEventArgs(key, description));
			}
		}

        public static void ReportError(string key, Exception ex, string friendlyMessage)
        {
            StatusErrorDelegate del = StatusMessaging.StatusError;
            if (Ready && del != null)
            {
                del(new StatusErrorEventArgs(key, ex, friendlyMessage));
            }

        }

		public static void UpdateStatusFinished(string key)
		{
			StatusUpdateDelegate del = StatusMessaging.StatusUpdate;
			if (Ready && del != null)
			{
				del(new StatusEventArgs(key, "Done!",true));
			}
		}
	}
}
