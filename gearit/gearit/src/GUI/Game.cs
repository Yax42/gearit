	using System;
using Microsoft.Xna.Framework;
using Squid;
using GUI;
using gearit.src.server;
using gearit.src.Network;
using gearit.xna;
using gearit.src.GUI.Tools;

namespace SquidXNA
{
	/// <summary>
	/// Game class
	/// </summary>
	public class Game : Microsoft.Xna.Framework.Game
	{
		public gearit.xna.ScreenManager ScreenManager;

		public Game()
		{
			IsMouseVisible = true;

			Content.RootDirectory = "Content";

			this.IsFixedTimeStep = false;

			ScreenManager = new gearit.xna.ScreenManager(this);
			//ScreenManager.SetResolutionScreen(1280, 700);
			// Arthur: I modified the default screen resolution so it matches a supported resolution.
			ScreenManager.SetResolutionScreen(1280, 720);
			Components.Add(ScreenManager);

			//Components.Add(new FrameRateCounter(ScreenManager));

			this.Window.Title = "Gear It!";
		}

		protected override void Initialize()
		{
			GuiHost.Renderer = new RendererXNA(this);

			InputManager input = new InputManager(this);
			Components.Add(input);

			ScreenManager.Content.RootDirectory = "Content";
			Theme.init();
			Theme.setTheme("Red");
			base.Initialize();
			ScreenManager.AddScreen(new ScreenMainMenu());

            // To remove
            NetworkServer.Start(INetwork.SERVER_PORT);
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GuiHost.TimeElapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			base.Draw(gameTime);
		}

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            // Stop the threads
            NetworkServer.Instance.Stop();
        }
	}
}
