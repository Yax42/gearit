using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using gearit.xna;

namespace gearit.src.utility
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GearIt : Game
    {
        // TODO: Update values.
        const int maxGamers = 4;
        const int maxLocalGamers = 2;

        private NetworkSession networkSession;

        private PacketWriter packetWriter;
        private PacketReader packetReader;

        public GearIt()
        {
            Window.Title = "GearIt - Prototype";
            IsFixedTimeStep = true;
            Content.RootDirectory = "Content";

            //new-up components and add to Game.Components
            ScreenManager = new ScreenManager(this);
            Components.Add(ScreenManager);

            FrameRateCounter frameRateCounter = new FrameRateCounter(ScreenManager);
            frameRateCounter.DrawOrder = 101;
            Components.Add(frameRateCounter);

            packetWriter = new PacketWriter();
            packetReader = new PacketReader();
        }

        public ScreenManager ScreenManager { get; set; }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            ScreenManager.AddScreen(new BackgroundScreen());
            ScreenManager.AddScreen(new LogoScreen(TimeSpan.FromSeconds(2.0)));
            LoadMenu();
        }

        protected void LoadMenu()
        {
            MainMenu main = new MainMenu(ScreenManager);
            main.LoadContent();
        }

        /// <summary>
        /// Allows the game to run logic.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Input ?
            if (networkSession == null)
            {
                // If we are not in a network session, update the
                // menu screen that will let us create or join one.
                UpdateMenuScreen();
            }
            else
            {
                // If we are in a network session, update it.
                UpdateNetworkSession();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Menu screen provides options to create or join network sessions.
        /// </summary>
        private void UpdateMenuScreen()
        {
            // TODO: Sign-in / Create session / Join Session.
        }

        /// <summary>
        /// Updates the state of the network session, moving the robots
        /// around and synchronizing their state over the network.
        /// </summary>
        private void UpdateNetworkSession()
        {
            // TODO: For each local players (only one?), read input and send
            // them to the server.

            // TODO: If host of party, update all robots and send positions to
            // all players.
        }
    }
}
