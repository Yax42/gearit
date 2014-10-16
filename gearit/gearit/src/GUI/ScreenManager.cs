using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using System.Threading;
using System;
using Squid;
using gearit.src.GUI;
using gearit.src;
using gearit.src.editor.map;
using gearit.src.editor.robot;
using gearit.src.GUI.OptionsMenu;
using gearit.src.GUI.Picker;

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
		static public int BLOCKED_FPS = 60;
        // Be safe boys !

		public static ScreenManager Instance { get; private set; }
        private Mutex mutex;

		private AssetCreator _assetCreator;
		private ContentManager _contentManager;
		public GraphicsDeviceManager _graphics {get; set;}
        public BasicEffect BasicEffect;

		private bool _isInitialized;
		private LineBatch _lineBatch;

		private List<GameScreen> _screens;
		private List<GameScreen> _screensTemp;

		private SpriteBatch _spriteBatch;

        public bool fpsIsLocked;

        private TimeSpan _elapsedTime = TimeSpan.Zero;
		private int _frameCounter;
		private int _frameRate;

		private Desktop _desktop;
		private Label _label;
		private int _ms_elapsed;
		private int _duration;

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
			Instance = this;
            // When drawing in other threads
            mutex = new Mutex();
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
            _graphics.SynchronizeWithVerticalRetrace = false;
			//game.Components.Add(new FrameRateCounter(this));
			fpsLock();

			#region StaticInstance
			new MapEditor();
			new RobotEditor();
			new OptionsMenu(this);
			new ScreenPickRobot();
			new ScreenPickMap();
			#endregion

		}

        public void fpsUnlock()
        {
            //débloquer les fps
            this.fpsIsLocked = false;
			Game.IsFixedTimeStep = false;
            _graphics.ApplyChanges();
        }

        public void fpsLock()
        {
            this.fpsIsLocked = true;
			Game.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 1000 / BLOCKED_FPS);
			Game.IsFixedTimeStep = true;
            _graphics.ApplyChanges();
        }

		public void Message(string msg, int duration = 2000)
		{
			_duration = duration;
			_label.Visible = true;
			_label.Text = msg;
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

        public void beginDrawing()
        {
            mutex.WaitOne();
        }

        public void stopDrawing()
        {
            mutex.ReleaseMutex();
        }

		/// <summary>
		/// Initializes the screen manager component.
		/// </summary>
		public override void Initialize()
		{
			_spriteFonts = new SpriteFonts(_contentManager);
			base.Initialize();

			_isInitialized = true;

			_desktop = new Desktop();
            _desktop.Size = new Squid.Point(Width, Height);
            _desktop.Position = new Squid.Point(0, 0);
			_label = new Label();
			_label.Size = _desktop.Size - new Squid.Point(0, 100) ;
			_label.Visible = false;
			_label.Parent = _desktop;
			_label.Style = "message";

			ChatBox.init(this);
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

            // Because XNA sucks
            BasicEffect = new BasicEffect(GraphicsDevice);
            BasicEffect.VertexColorEnabled = true;
            BasicEffect.TextureEnabled = false;
            BasicEffect.Projection = Matrix.CreateOrthographicOffCenter
               (0, GraphicsDevice.Viewport.Width,     // left, right
                GraphicsDevice.Viewport.Height, 0,    // bottom, top
                0, 1); 
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
			_screens.Clear();
		}

        public void CountFPS(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;

			if (_elapsedTime <= TimeSpan.FromSeconds(1)) return;

			_elapsedTime -= TimeSpan.FromSeconds(1);
			_frameRate = _frameCounter;
			_frameCounter = 0;
        }

        public int getFPS()
        {
            return (_frameRate);
        }


		/// <summary>
		/// Allows each screen to run logic.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
            CountFPS(gameTime);

            beginDrawing();

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

			_ms_elapsed += gameTime.ElapsedGameTime.Milliseconds;
			if (_label.Visible && _ms_elapsed > _duration)
				_label.Visible = false;
			_desktop.Update();

			ChatBox.Update();

            stopDrawing();
		}

		/// <summary>
		/// Tells each screen to draw itself.
		/// </summary>
		public override void Draw(GameTime gameTime)
		{
            _frameCounter++;

            beginDrawing();

			// Remove if problem with Squid
			GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.White);

			foreach (GameScreen screen in _screens)
			{
				if (screen.ScreenState == ScreenState.Hidden)
					continue;

				screen.Draw(gameTime);
			}

			_desktop.Draw();
			ChatBox.getDesktop().Draw();

            stopDrawing();
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

		public void BackToMainMenu()
		{
			while (_screens.Count > 1)
				RemoveLast();
		}

		public void ResetTo(GameScreen screen)
		{
			BackToMainMenu();
			AddScreen(screen);
		}

		/// <summary>
		/// Adds a new screen to the screen manager.
		/// </summary>
		public void AddScreen(GameScreen screen)
		{
			screen.ScreenManager = this;
			screen.IsExiting = false;

			// If we have a graphics device, tell the screen to load content.
			if (_isInitialized && !screen.is_initialized)
				screen.LoadContent();

			_screens.Add(screen);
			UpdatePriority();
		}

		public void RemoveLast()
		{
			_screens[_screens.Count - 1].UnloadContent();
			_screens.RemoveAt(_screens.Count - 1);
		}

		public void ReplaceLast(GameScreen screen)
		{
			RemoveLast();
			AddScreen(screen);
		}

		public bool MenuVisible
		{
			get
			{
				return _screens[_screens.Count - 1].VisibleMenu;
			}
		}

		public bool IsEmpty()
		{
			return _screens.Count == 0;
		}

		public void RemoveScreen(GameScreen screen)
		{
			// If we have a graphics device, tell the screen to unload content.
			int target = _screens.IndexOf(screen);
			for (int i = _screens.Count - 1; i <= target; i++)
			{
				if (_isInitialized && _screens[i].is_initialized)
				{
					_screens[i].UnloadContent();
				}
				_screensTemp.Remove(_screens[i]);
				_screens.RemoveAt(i);
			}
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