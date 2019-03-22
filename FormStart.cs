using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Rawr
{
	public partial class FormStart : Form
	{
		private int _currentTab = 0;
		private FormMain _formMain;
		private WebRequestWrapper _webRequestWrapper = new WebRequestWrapper();
		string[] _recentCharacters;
		public FormStart(FormMain formMain)
		{
			InitializeComponent();
			labelVersionHistory.Text = @"v2.3.23.0
 - Fix for default data of a few items.
 - Added a few buffs and reorganized the grouping of several buffs.
 - Fix for base agility on taurens being off by 1.
 - The optimizer will now consider items which have been marked available and are equippable, even if their item type is filtered out.
 - Rawr.Hunter: Fix for the ability to select Hunter's Mark. Fix for a crash with Pet buffs and talents.
 - Rawr.ShadowPriest: Fix for the bug with +hit calculations.
 - Rawr.Elemental: Fix for a bug involving DPS Fire Totems without 4T10.
 - Rawr.DPSDK: Added some ArPen-based default gemming templates. Improvements to the way the optimal rotation is detected. Fix for nested special effects producing inflated DPS. Added Resilience and Spell Penetration as optimizable stats. Fix for Cinderglacier being overvalued.
 - Rawr.TankDK: Improvements to the way the optimal rotation is detected. Added Resilience and Spell Penetration as optimizable stats.
 - Rawr.Moonkin: Fix for 2T10 having value without Omen of Clarity.
 - Rawr.Rogue: Added an option to only use the Custom Rotation when optiming. This greatly speeds up the optimization for those who know the best rotation.";
			labelVersionHistory.Height = 460;

			this.DoubleBuffered = true;
			_formMain = formMain;

			string version = "v" + System.Reflection.Assembly.GetCallingAssembly().GetName().Version.ToString();
			while (version.EndsWith(".0")) version = version.Substring(0, version.Length - 2);
			labelVersion.Text = version;
			checkBoxShowAtLaunch.Checked = Properties.Recent.Default.ShowStartPage;

			_recentCharacters = _formMain.ConfigRecentCharacters.Clone() as string[];
			Array.Reverse(_recentCharacters);
			int recentCount = _recentCharacters.Length;
			labelRecentCharacters.Visible = recentCount > 0;
			linkLabelRecentCharacter1.Visible = recentCount > 0;
			if (recentCount > 0)
				linkLabelRecentCharacter1.Text =
					System.IO.Path.GetFileNameWithoutExtension(_recentCharacters[0]);
			linkLabelRecentCharacter2.Visible = recentCount > 1;
			if (recentCount > 1)
				linkLabelRecentCharacter2.Text =
					System.IO.Path.GetFileNameWithoutExtension(_recentCharacters[1]);
			linkLabelRecentCharacter3.Visible = recentCount > 2;
			if (recentCount > 2)
				linkLabelRecentCharacter3.Text =
					System.IO.Path.GetFileNameWithoutExtension(_recentCharacters[2]);
			linkLabelRecentCharacter4.Visible = recentCount > 3;
			if (recentCount > 3)
				linkLabelRecentCharacter4.Text =
					System.IO.Path.GetFileNameWithoutExtension(_recentCharacters[3]);
			linkLabelRecentCharacter5.Visible = recentCount > 4;
			if (recentCount > 4)
				linkLabelRecentCharacter5.Text =
					System.IO.Path.GetFileNameWithoutExtension(_recentCharacters[4]);

			ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadDidYouKnows));
			ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadKnownIssues));
		}

		private void DownloadDidYouKnows(object ignored)
		{
			string dyk = _webRequestWrapper.GetRandomDidYouKnow();
			try
			{
				if (this.Visible || this.Disposing || this.IsDisposed)
					this.Invoke(new WaitCallback(DisplayDidYouKnow), dyk);
			}
			catch { }
		}

		private void DisplayDidYouKnow(object dyk)
		{
			if (this.Visible || this.Disposing || this.IsDisposed)
				labelDidYouKnow.Text = dyk.ToString().Replace("#39;","\'");
		}

		private void DownloadKnownIssues(object ignored)
		{
			string ki = _webRequestWrapper.GetKnownIssues();
			try
			{
				if (this.Visible || this.Disposing || this.IsDisposed)
					this.Invoke(new WaitCallback(DisplayKnownIssues), ki);
			}
			catch { }
		}

		private void DisplayKnownIssues(object ki)
		{
			if (this.Visible || this.Disposing || this.IsDisposed)
				labelKnownIssues.Text = ki.ToString();
		}

		private void panelDidYouKnowTab_Click(object sender, EventArgs e)
		{
			if (_currentTab != 0)
			{
				_currentTab = 0;
				panelTabBackground.BackgroundImage = Properties.Resources.RawrStart_1;
				panelDidYouKnow.Visible = true;
				panelDidYouKnow.Cursor = Cursors.Default;
				panelVersionHistory.Visible = panelKnownIssues.Visible = false;
				panelVersionHistory.Cursor = panelKnownIssues.Cursor = Cursors.Hand;
			}
		}

		private void panelVersionHistoryTab_Click(object sender, EventArgs e)
		{
			if (_currentTab != 1)
			{
				_currentTab = 1;
				panelTabBackground.BackgroundImage = Properties.Resources.RawrStart_2;
				panelVersionHistory.Visible = true;
				panelVersionHistory.Cursor = Cursors.Default;
				panelDidYouKnow.Visible = panelKnownIssues.Visible = false;
				panelDidYouKnow.Cursor = panelKnownIssues.Cursor = Cursors.Hand;
			}
		}

		private void panelKnownIssuesTab_Click(object sender, EventArgs e)
		{
			if (_currentTab != 2)
			{
				_currentTab = 2;
				panelTabBackground.BackgroundImage = Properties.Resources.RawrStart_3;
				panelKnownIssues.Visible = true;
				panelKnownIssues.Cursor = Cursors.Default;
				panelDidYouKnow.Visible = panelVersionHistory.Visible = false;
				panelDidYouKnow.Cursor = panelVersionHistory.Cursor = Cursors.Hand;
			}
		}

		private void linkLabelCreateNew_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (_formMain.NewCharacter()) this.Close();
		}

		private void linkLabelLoadArmory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (_formMain.LoadCharacterFromArmory()) this.Close();
		}

		private void linkLabelOpenSaved_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (_formMain.OpenCharacter()) this.Close();
		}

		private void linkLabelRecentCharacter1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			_formMain.LoadSavedCharacter(_recentCharacters[0]);
			this.Close();
		}

		private void linkLabelRecentCharacter2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			_formMain.LoadSavedCharacter(_recentCharacters[1]);
			this.Close();
		}

		private void linkLabelRecentCharacter3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			_formMain.LoadSavedCharacter(_recentCharacters[2]);
			this.Close();
		}

		private void linkLabelRecentCharacter4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			_formMain.LoadSavedCharacter(_recentCharacters[3]);
			this.Close();
		}

		private void linkLabelRecentCharacter5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			_formMain.LoadSavedCharacter(_recentCharacters[4]);
			this.Close();
		}

		private void panelTourOfRawr_Click(object sender, EventArgs e) //Tour of Rawr
		{ linkLabelHelp3_LinkClicked(null, null); }

		private void linkLabelHelp1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) //Rawr Website
		{ Help.ShowHelp(null, "http://rawr.codeplex.com/"); }

		private void linkLabelHelp2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) //Donate
		{ Help.ShowHelp(null, "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=2451163"); }

		private void linkLabelHelp3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) //Tour of Rawr
		{ Help.ShowHelp(null, "http://www.youtube.com/watch?v=OjRM5SUoOoQ"); }

		private void linkLabelHelp4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) //Gemmings
		{ Help.ShowHelp(null, "http://www.codeplex.com/Rawr/Wiki/View.aspx?title=Gemmings"); }

		private void linkLabelHelp5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) //Gear Optimization
		{ Help.ShowHelp(null, "http://www.codeplex.com/Rawr/Wiki/View.aspx?title=GearOptimization"); }

		private void linkLabelHelp6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) //Batch Tools
		{ Help.ShowHelp(null, "http://www.codeplex.com/Rawr/Wiki/View.aspx?title=BatchTools"); }

		private void linkLabelHelp7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) //Item Filtering
		{ Help.ShowHelp(null, "http://www.codeplex.com/Rawr/Wiki/View.aspx?title=ItemFiltering"); }

		private void checkBoxShowAtLaunch_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Recent.Default.ShowStartPage = checkBoxShowAtLaunch.Checked;
		}
	}
}
