using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using gearit.src.utility.Menu;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using gearit.src.editor.robot;
using FarseerPhysics.Factories;
using gearit.src.map;
using gearit.src.editor.map.action;
using gearit.src.editor.map;
using FarseerPhysics.DebugViews;
using Squid;
using gearit.src.GUI;
using gearit.src.GUI.Picker;
using gearit.src.Network;

namespace gearit.src.gui
{

	class MenuConnectIP : GameScreen, IDemoScreen
	{

		private ScreenPickManager picker;
		private Desktop dk;
		private MessageBoxInput msgbox;

		public MenuConnectIP() : base(true)
		{
		}

		public override void LoadContent()
		{
			base.LoadContent();

			dk = new Desktop();
			dk.Position = new Squid.Point(0, 0);
			dk.Size = new Squid.Point(ScreenManager.Instance.Width, ScreenManager.Instance.Height);
			msgbox = new MessageBoxInput(dk, "Connect To", connectTo);
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
		}

		public void connectTo(string ip)
		{
			picker = new ScreenPickManager(false, true,
					delegate()
					{
						ScreenManager.AddScreen(new NetworkClientGame(ScreenPickManager.RobotPath, ip));
						picker = null;
					});
					ScreenManager.Instance.AddScreen(picker);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			dk.Update();
		}

		

		private void HandleInput()
		{
			
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			dk.Draw();
		}

        public string GetTitle()
        {
            return "Quit";
        }

        public string GetDetails()
        {
            return "Quit menu";
        }
	}
}
