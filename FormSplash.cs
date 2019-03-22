using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace Rawr
{
    public partial class FormSplash : Form
    {
        public FormSplash()
        {
            InitializeComponent();
        }

		private Image _cachedSplash = null;
		public Image CachedSplash
		{
			get
			{
				if (_cachedSplash == null)
				{
					_cachedSplash = new Bitmap(500, 350);
					Graphics g = Graphics.FromImage(_cachedSplash);
					g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
					g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
					g.DrawImage(this.BackgroundImage, 0, 0, 500, 350);

					int x = 16;
					int y = 302;
					Image icon = null;
					string name, version = string.Empty;
					Font fontName = new Font("Arial Narrow", 9.75f, FontStyle.Regular, GraphicsUnit.Point);
					Font fontVersion = new Font("Arial Narrow", 9f, FontStyle.Italic, GraphicsUnit.Point);
					Font fontVersionBig = new Font("Arial Narrow", 9.75f, FontStyle.Italic, GraphicsUnit.Point);
					Brush brushName = new SolidBrush(Color.FromArgb(224, 224, 224, 255));
					Brush brushVersion = new SolidBrush(Color.FromArgb(192, 224, 224, 255));
					StringFormat formatRightAlign = new StringFormat();
					formatRightAlign.Alignment = StringAlignment.Far;

					version = Assembly.GetCallingAssembly().GetName().Version.ToString();
					while (version.EndsWith(".0")) version = version.Substring(0, version.Length - 2);
					g.DrawString("UI v" + version, fontVersionBig, brushName, 484, 144, formatRightAlign);
					version = typeof(CalculationsBase).Assembly.GetName().Version.ToString();
					while (version.EndsWith(".0")) version = version.Substring(0, version.Length - 2);
					g.DrawString("Base v" + version, fontVersionBig, brushName, 484, 160, formatRightAlign);

					foreach (var modelKvp in Calculations.Models)
					{
						name = modelKvp.Key;
						version = "v" + modelKvp.Value.Assembly.GetName().Version.ToString();
						while (version.EndsWith(".0")) version = version.Substring(0, version.Length - 2);
						icon = ItemIcons.GetItemIcon(Calculations.ModelIcons[modelKvp.Key], true);
						if (icon != null)
						{
							g.DrawImage(icon, x, y, 32, 32);
						}
						g.DrawImage(Rawr.Properties.Resources.Rawr_Splash_ModelBackground, x + 30, y - 8);
						g.DrawString(name, fontName, brushName, x + 34, y);
						g.DrawString(version, fontVersion, brushVersion, x + 34, y + 16);

						x += 93;
						if (x > 415)
						{
							x = 16;
							y -= 40;
						}
					}
				}
				return _cachedSplash;
			}
		}

		private void FormSplash_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawImageUnscaled(CachedSplash, 0, 0);
		}
    }
}