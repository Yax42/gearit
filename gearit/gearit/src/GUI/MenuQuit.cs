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

namespace gearit.src.gui
{

	class MenuQuit : GameScreen, IDemoScreen
	{

		public MenuQuit() : base(true)
		{
		}

		public override void LoadContent()
		{
			base.LoadContent();

            ScreenManager.Exit();
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		

		private void HandleInput()
		{
			
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
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
