using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace gearit.xna
{
	class ResolutionChanger : GameScreen, IDemoScreen
	{
		private int _width;
		private int _height;

		#region IDemoScreen Members

		public string GetTitle()
		{
			return (string.Empty);
		}

		public string GetDetails()
		{
			return (string.Empty);
		}

		#endregion

		public ResolutionChanger(int width, int height) : base(false)
		{
			_width = width;
			_height = height;
		}

		public void LoadMenu()
		{
			base.LoadContent();
		}

		public override void Update(GameTime gameTime)
		{
			ScreenManager.SetResolutionScreen(_width, _height);
			this.ExitScreen();
			base.Update(gameTime);
		}
	}
}
