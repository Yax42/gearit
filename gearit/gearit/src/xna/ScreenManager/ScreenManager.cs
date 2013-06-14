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

        private InputHelper _input;
        private bool _isInitialized;
        private LineBatch _lineBatch;

        private List<GameScreen> _screens;
        private List<GameScreen> _screensToUpdate;

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
            _input = new InputHelper(this);

            _screens = new List<GameScreen>();
            _screensToUpdate = new List<GameScreen>();
            _transitions = new List<RenderTarget2D>();
            _graphics = new GraphicsDeviceManager(game);
            _graphics.PreferMultiSampling = true;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);
            _graphics.IsFullScreen = false;
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
            _input.LoadContent();

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
            // Read the keyboard and gamepad.
            _input.Update(gameTime);

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            _screensToUpdate.Clear();

            foreach (GameScreen screen in _screens)
            {
                _screensToUpdate.Add(screen);
            }

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (_screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = _screensToUpdate[_screensToUpdate.Count - 1];

                _screensToUpdate.RemoveAt(_screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        _input.ShowCursor = screen.HasCursor;
                        _input.EnableVirtualStick = screen.HasVirtualStick;
                        screen.HandleInput(_input, gameTime);
                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                    {
                        coveredByOtherScreen = true;
                    }
                }
            }
        }

        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            int transitionCount = 0;
            foreach (GameScreen screen in _screens)
            {
                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.TransitionOff)
                {
                    ++transitionCount;
                    if (_transitions.Count < transitionCount)
                    {
                        PresentationParameters _pp = GraphicsDevice.PresentationParameters;
                        _transitions.Add(new RenderTarget2D(GraphicsDevice, _pp.BackBufferWidth, _pp.BackBufferHeight,
                                                            false,
                                                            SurfaceFormat.Color, _pp.DepthStencilFormat,
                                                            _pp.MultiSampleCount,
                                                            RenderTargetUsage.DiscardContents));
                    }
                    GraphicsDevice.SetRenderTarget(_transitions[transitionCount - 1]);
                    GraphicsDevice.Clear(Color.Transparent);
                    screen.Draw(gameTime);
                    GraphicsDevice.SetRenderTarget(null);
                }
            }

            GraphicsDevice.Clear(Color.Black);

            transitionCount = 0;
            foreach (GameScreen screen in _screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                {
                    continue;
                }

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.TransitionOff)
                {
                    _spriteBatch.Begin(0, BlendState.AlphaBlend);
                    _spriteBatch.Draw(_transitions[transitionCount], Vector2.Zero, Color.White * screen.TransitionAlpha);
                    _spriteBatch.End();

                    ++transitionCount;
                }
                else
                {
                    screen.Draw(gameTime);
                }
            }
            _input.Draw();
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
            {
                screen.LoadContent();
            }

            _screens.Add(screen);
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
            _screensToUpdate.Remove(screen);
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