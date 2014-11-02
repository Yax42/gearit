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
using GUI;
using System.Net.Sockets;
using gearit.src.GUI;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using gearit.src.output;
using gearit.src.Network;
using System.Diagnostics;
using Lidgren.Network;
using gearit.src.GUI.Picker;
using gearit.src.game;

namespace gearit.src.gui
{
    /// <summary>
    /// List server / interaction with Game Master server / Connexion
    /// </summary>

	class MenuSolo : GameScreen, IDemoScreen
	{
		public MenuSolo() : base(false)
		{
		}
		public override void LoadContent()
		{
			base.LoadContent();

			ScreenManager.Instance.AddScreen(new ScreenPickManager(true, true,
				delegate()
				{
						ScreenManager.Instance.AddScreen(new GearitGame(ScreenPickManager.RobotPath, ScreenPickManager.MapPath));
				}));
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
		}
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
		}

        public string GetTitle()
        {
            return "Solo";
        }

        public string GetDetails()
        {
            return "Solo menu";
        }
	}
}
