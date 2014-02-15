using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace gearit.xna
{
	class MainOptions : MenuScreen, IDemoScreen
	{
		private VideoOption _video;
		#region IDemoScreen Members

		public string GetTitle()
		{
			return "Options";
		}

		public string GetDetails()
		{
			return (string.Empty);
		}

		#endregion

		public MainOptions(string menuTitle, ScreenManager manager)
			: base(menuTitle)
		{
			ScreenManager = manager;
		}

		public void LoadContent()
		{
			base.LoadContent();

			_video = new VideoOption("Video Option", ScreenManager);
			_video.LoadMenu();

			AddMenuItem("Video", EntryType.Screen, _video);
			AddMenuItem("Audio", EntryType.Separator, null);
			AddMenuItem("", EntryType.Separator, null);
			AddMenuItem("Retour", EntryType.Cancel, null);
		}
	}
}
