using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace gearit.xna
{
	class VideoOption : MenuScreen, IDemoScreen
	{
		private ResolutionChanger _800;
		private ResolutionChanger _1024;
		private ResolutionChanger _1280;
		private ResolutionChanger _1680;
		private ResolutionChanger _1920;
		private FullScreen _fs;
		#region IDemoScreen Members

		public string GetTitle()
		{
			return "Graphique";
		}

		public string GetDetails()
		{
			/*StringBuilder sb = new StringBuilder();
			sb.AppendLine("");
			return sb.ToString();*/
			return (string.Empty);
		}

		#endregion

		public VideoOption(string menuTitle, ScreenManager manager)
			: base(menuTitle)
		{
			ScreenManager = manager;
		}

		public void LoadMenu()
		{
			base.LoadContent();
			_800 = new ResolutionChanger(800, 600);
			_1024 = new ResolutionChanger(1024, 768);
			_1280 = new ResolutionChanger(1280, 720);
			_1680 = new ResolutionChanger(1680, 1050);
			_1920 = new ResolutionChanger(1920, 1080);
			_fs = new FullScreen();

			AddMenuItem("800 * 600", EntryType.Screen, _800);
			AddMenuItem("1024 * 768", EntryType.Screen, _1024);
			AddMenuItem("1280 * 720", EntryType.Screen, _1280);
			AddMenuItem("1680 * 1050", EntryType.Screen, _1680);
			AddMenuItem("1920 * 1080", EntryType.Screen, _1920);
			AddMenuItem("", EntryType.Separator, null);
			AddMenuItem("On/Off Fullscreen", EntryType.Screen, _fs);
			AddMenuItem("", EntryType.Separator, null);
			AddMenuItem("Retour", EntryType.Cancel, null);
		}
	}
}
