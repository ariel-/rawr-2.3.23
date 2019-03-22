using System;
using System.Drawing;

namespace Rawr.UserControls.Options
{
	internal interface IOptions
	{
		void Save();
		void Cancel();
		bool HasValidationErrors();
		string DisplayName { get; }
		string TreePosition { get; }
		Image MenuIcon{get;}
	}
}
