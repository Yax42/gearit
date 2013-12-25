using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;

namespace gearit.xna
{
    /// <summary>
    /// The screen manager is a component which manages one or more GameScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        private AssetCreator _assetCreator;
        private ContentManager _contentManager;
        private GraphicsDeviceManager _graphics;

        private bool _isInitialized;
        private LineBatch _lineBatch;

        private List<GameScreen> _screens;
        private List<GameScreen> _screensTemp;

        private SpriteBatch _spriteBatch;

        /// <summary>
        /// Contains all the fonts avaliable for use.
        /// </summary>
        private SpriteFonts _spriteFonts;

        private List<RenderTarget2D> _transitions;

        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public ScreenManager(Game game)
            : base(game)
        {
            // we must set EnabledGestures before we can query for them, but
            // we don't assume the game wants to read them.
            _contentManager = game.Content;
            _contentManager.RootDirectory = "Content";

            _screens = new List<GameScreen>();
            _screensTemp = new List<GameScreen>();
            _transitions = new List<RenderTarget2D>();
            _graphics = new GraphicsDeviceManager(game);
            _graphics.PreferMultiSampling = true;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);
            _graphics.IsFullScreen = false;
        }

        /// <summary>
        /// Return Width
        /// </summary>
        public int Width
        {
            get { return _graphics.PreferredBackBufferWidth; }
        }

        /// <summary>
        /// Return Height
        /// </summary>
        public int Height
        {
            get { return _graphics.PreferredBackBufferHeight; }
        }

        /// <summary>
        /// Set Resolution
        /// </summary>
        public void SetResolutionScreen(int width, int height)
        {
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            
            _graphics.ApplyChanges();
        }

        /// <summary>
        /// Return FullScreen mode
        /// </summary>
        public bool IsFullScreen
        {
            get { return _graphics.IsFullScreen; }
        }

        /// <summary>
        /// Active FullScreen mode
        /// </summary>
        public void activeFullScreen()
        {
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }

        /// <summary>
        /// Desactive FullScreen mode
        /// </summary>
        public void deactivFullScreen()
        {
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
        }

        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        public LineBatch LineBatch
        {
            get { return _lineBatch; }
        }

        public ContentManager Content
        {
            get { return _contentManager; }
        }

        public SpriteFonts Fonts
        {
            get { return _spriteFonts; }
        }

        public AssetCreator Assets
        {
            get { return _assetCreator; }
        }

        /// <summary>
        /// Initializes the screen manager component.
        /// </summary>
        public override void Initialize()
        {
            _spriteFonts = new SpriteFonts(_contentManager);
            base.Initialize();

            _isInitialized = true;
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _lineBatch = new LineBatch(GraphicsDevice);
            _assetCreator = new AssetCreator(GraphicsDevice);
            _assetCreator.LoadContent(_contentManager);

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in _screens)
            {
                screen.LoadContent();
            }
        }

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in _screens)
            {
                screen.UnloadContent();
            }
        }

        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Update input
            Input.update();

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            _screensTemp.Clear();

            foreach (GameScreen screen in _screens)
                _screensTemp.Add(screen);

            // Loop as long as there are screens waiting to be updated.
            while (_screensTemp.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = _screensTemp[_screensTemp.Count - 1];

                _screensTemp.RemoveAt(_screensTemp.Count - 1);
                screen.Update(gameTime);
            }
        }

        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Remove if problem with Squid
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Gray);

            foreach (GameScreen screen in _screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }

        /// <summary>
        /// Sort the list by Draw priority
        /// </summary>
        public void UpdatePriority()
        {
            _screens.Sort(
                delegate(GameScreen p1, GameScreen p2)
                {
                    return p1.DrawPriority.CompareTo(p2.DrawPriority);
                }
            );
        }

        public void Exit()
        {
            Game.Exit();
        }

        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (_isInitialized)
                screen.LoadContent();

            _screens.Add(screen);
            UpdatePriority();
        }

        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (_isInitialized)
            {
                screen.UnloadContent();
            }

            _screens.Remove(screen);
            _screensTemp.Remove(screen);
        }

        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public GameScreen[] GetScreens()
        {
            return _screens.ToArray();
        }
    }
}