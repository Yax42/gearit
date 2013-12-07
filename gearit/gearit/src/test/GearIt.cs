using System;
using Microsoft.Xna.Framework;
using gearit.xna;
using GUI;

namespace gearit.src.utility
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GearIt : Game
    {
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
 
        }
    }
}