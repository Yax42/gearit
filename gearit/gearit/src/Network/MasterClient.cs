using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;
using gearit.src.server;
using gearit.src.output;

namespace gearit.src
{
	class MasterClient : GameScreen, IDemoScreen
	{
		//private DrawGame _draw_game;
		private const int PropertiesMenuSize = 40;

		//Action
		public static MasterClient Instance { set; get; }

        //server + client
        public InGameServer server;
        public InGameClient client;

		#region IDemoScreen Members

		public string GetTitle()
		{
			return "Multi-player online";
		}

		public string GetDetails()
		{
			return "";
		}

		#endregion

		public MasterClient() : base(true)
		{
			TransitionOnTime = TimeSpan.FromSeconds(0.75);
			TransitionOffTime = TimeSpan.FromSeconds(0.75);
			HasCursor = true;
			Instance = this;
            server = new InGameServer(25552);
            client = new InGameClient(25552, "127.0.0.1");
		}

        ~MasterClient()
        {
            client.Stop();
            server.Stop();
        }


		public override void LoadContent()
		{
			base.LoadContent();
			new MenuMasterClient(ScreenManager);

			ScreenManager.Game.ResetElapsedTime();
			HasVirtualStick = true;

			//_draw_game = new DrawGame(ScreenManager.GraphicsDevice);
            // Start Server
            OutputManager.LogNetwork("Server launch");
            server.Start();
            System.Threading.Thread.Sleep(50);
            
            //Start Client
            OutputManager.LogNetwork("Client launch");
            client.Start();
            System.Threading.Thread.Sleep(50);
		}

		public override void Update(GameTime gameTime)
		{
			MenuMasterClient.Instance.Update();
			base.Update(gameTime);
		}


		public override void Draw(GameTime gameTime)
		{
			//ScreenManager.GraphicsDevice.Clear(Color.LightSeaGreen);
			//_draw_game.Begin(_camera);

			//_draw_game.End();
			base.Draw(gameTime);
			MenuMasterClient.Instance.Draw();
		}

        public override void UnloadContent()
        {
            client.Stop();
            server.Stop();
            base.UnloadContent();
        }
	}
}
