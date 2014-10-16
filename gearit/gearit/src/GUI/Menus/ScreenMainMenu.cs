using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.GUI;
using Squid;
using gearit.src.game;
using gearit.src.Network;
using gearit.src.GeneticAlgorithm;
using gearit.src;
using gearit.src.gui;
using gearit.src.GUI.Picker;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.editor.map;
using gearit.src.editor.robot;
using gearit.src.GUI.OptionsMenu;
using gearit.src.GUI.Tools;

namespace GUI
{
	class ScreenMainMenu : GameScreen
	{
		private static bool _GoBack = false;
		public static bool GoBack
		{
			get { return _GoBack; }
			set{_GoBack = value; }
		}

		public enum Animation
		{
			Chillout,
			ShowMainMenu,
			HideMainMenu,
			ToggleMenu,
			ShowMenu
		}	

		// Properties
		static public int MENU_WIDTH = 300;
		static public int MENU_LIST_WIDTH = 200;
		static public int HEIGHT_TITLE = 35;
		static public int DELAY_ANIMATION = 200;
		static public int RESERVED_HEIGHT = HEIGHT_TITLE * 2;

		// Gui
		public int AnimationElapsedTime = 0;
		public Animation CurrentAnimation = Animation.Chillout;
		private GameScreen _current_screen = null;
		private GameScreen _old_screen = null;
		private Color STRIPE_COLOR = Color.White;
		private Desktop _dk_main_menu;
		private Control main_menu;
		private Control list_container;
		private Label tb_title;
		private Desktop dk_listbox;
		private ListBox menu_listbox;

		// List of item menu
		private MyGame _Gearit;
		private GearitGame _game;
		private NetworkClientGame _networkGame;
		private MenuPlay _play;
		private MenuQuit _quit;
		private ScreenPickManager _soloGame;

		public ScreenMainMenu() : base(false)
		{
			DrawPriority = 999;
		}

		public override void LoadContent()
		{
			base.LoadContent();

			VisibleMenu = true;
			_dk_main_menu = new Desktop();

			#region main

			_dk_main_menu.Position = new Squid.Point(0, 0);
			_dk_main_menu.Size = new Squid.Point(MENU_WIDTH, ScreenManager.Height);

			// MainMenu Container
			main_menu = new Control();
			main_menu.Position = new Squid.Point(0, HEIGHT_TITLE);
			main_menu.Size = new Squid.Point(MENU_WIDTH, ScreenManager.Height);
			main_menu.Style = "mainMenu";
			main_menu.Parent = _dk_main_menu;

			// MainMenu List Container
			list_container = new Control();
			list_container.Position = new Squid.Point(MENU_WIDTH - MENU_LIST_WIDTH, 0);
			list_container.Size = new Squid.Point(MENU_LIST_WIDTH, (int)(ScreenManager.Height / 1.5f));
			list_container.Style = "mainMenuListContainer";
			list_container.Parent = _dk_main_menu;

			// Private desktop
			dk_listbox = new Desktop();
			dk_listbox.Position = list_container.Position;
			dk_listbox.Size = list_container.Size;

			// MainMenu List
			menu_listbox = new ListBox();
			menu_listbox.Position = new Squid.Point(0, HEIGHT_TITLE * 3);
			menu_listbox.Size = new Squid.Point(MENU_LIST_WIDTH, list_container.Size.y - HEIGHT_TITLE * 2);
            menu_listbox.Scrollbar.Size = new Squid.Point(0, 0);
			menu_listbox.Style = "mainMenuList";
			menu_listbox.Parent = dk_listbox;

			// Title
			tb_title = new Label();
			tb_title.Position = new Squid.Point(0, HEIGHT_TITLE + 7);
			tb_title.Size = new Squid.Point(MENU_LIST_WIDTH, HEIGHT_TITLE);
			tb_title.Style = "titleMainMenu";
			tb_title.Parent = dk_listbox;

			_Gearit = new MyGame();
			_game = new GearitGame();
			_networkGame = new NetworkClientGame();
			_play = new MenuPlay();
			_quit = new MenuQuit();
			_soloGame = new ScreenPickManager(_game);

			// Add ItemMenu
			addMenuItem(_play, _play.GetTitle().ToUpper());
			addMenuItem(RobotEditor.Instance, RobotEditor.Instance.GetTitle().ToUpper());
			addMenuItem(MapEditor.Instance, MapEditor.Instance.GetTitle().ToUpper());
			addMenuItem(OptionsMenu.Instance , OptionsMenu.Instance.GetTitle().ToUpper());
			addMenuSeparator();
			addMenuItem(_game, "DEFAULT GAME");
			addMenuItem(_networkGame, "NETWORK GAME");
			addMenuItem(_soloGame, "SOLO");

			addMenuSeparator();

			addMenuItem(_quit, _quit.GetTitle().ToUpper());

			//menu_listbox.Items[1].Click(0);

			#endregion
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			_dk_main_menu.Update();
			dk_listbox.Update();

			if (Input.Exit)
			{
				if (CurrentAnimation == Animation.HideMainMenu || CurrentAnimation == Animation.ShowMainMenu)
				{
					CurrentAnimation = CurrentAnimation == Animation.ShowMainMenu ? Animation.HideMainMenu : Animation.ShowMainMenu;
					AnimationElapsedTime = DELAY_ANIMATION - AnimationElapsedTime;
				}
				else
					CurrentAnimation = VisibleMenu ? Animation.HideMainMenu : Animation.ShowMainMenu;
				dk_listbox.Enabled = false;
			}

			// Animate
			if (CurrentAnimation != Animation.Chillout)
				manageAnimation(gameTime);
		}

		public override void positionChanged(int x, int y)
		{
			base.positionChanged(x, y);

			_dk_main_menu.Position = new Squid.Point(x, y);
			dk_listbox.Position = new Squid.Point(x + MENU_WIDTH - MENU_LIST_WIDTH, 0);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			_dk_main_menu.Draw();

			drawTitleStrip();

			Color color;
			tb_title.Text = "GEARIT";
			// Draw stripe listview
			for (int i = 0; i < menu_listbox.Items.Count; ++i)
			{
				ListBoxItem item = menu_listbox.Items[i];
				if (item.Text == "")
					continue;
				if (item.State > ControlState.Default && item.State != ControlState.Disabled)
				{
					tb_title.Text = item.Text;
					color = Theme.CurrentTheme.Primitive;
				}
				else
					color = STRIPE_COLOR;
				drawStripes(item.Location.x + 8, item.Location.y + 6, item.Size.x - 8, 25, color);
			}

			dk_listbox.Draw();
		}

		public void drawTitleStrip()
		{
			// Setup graphics
			RasterizerState _rasterizerState = new RasterizerState() { ScissorTestEnable = true };
			ScreenManager.Instance.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, _rasterizerState);
            ScreenManager.Instance.BasicEffect.CurrentTechnique.Passes[0].Apply(); // don't worry be happy

			setBaseStrip(HEIGHT_TITLE);
			int x = MENU_WIDTH - MENU_LIST_WIDTH - BASE_STRIP_WIDTH / 2 - ((BASE_STRIP_WIDTH * 2 + 1) * 5) - 2;
			ScreenManager.Instance.GraphicsDevice.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle(0, HEIGHT_TITLE, MENU_WIDTH - MENU_LIST_WIDTH, HEIGHT_TITLE);
			drawStripes(x, HEIGHT_TITLE - 1, 6, STRIPE_COLOR);
			x += ((BASE_STRIP_WIDTH * 2 + 1) * 3) + BASE_STRIP_WIDTH;
			ScreenManager.Instance.GraphicsDevice.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, ScreenManager.Width, ScreenManager.Height);
			drawStripes(x, HEIGHT_TITLE - 1, (int) (MENU_LIST_WIDTH * 1.7f), HEIGHT_TITLE, Theme.CurrentTheme.Grayie);
			int nb = drawStripes(x + 1, HEIGHT_TITLE - 1, (int) (MENU_LIST_WIDTH * 1.7f), HEIGHT_TITLE, Theme.CurrentTheme.Grayie);
			ScreenManager.Instance.GraphicsDevice.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle(MENU_WIDTH - MENU_LIST_WIDTH, HEIGHT_TITLE, MENU_LIST_WIDTH, HEIGHT_TITLE);
			drawStripes(x + 1, HEIGHT_TITLE - 1, (int) (MENU_LIST_WIDTH * 1.7f), HEIGHT_TITLE, STRIPE_COLOR);
			ScreenManager.Instance.GraphicsDevice.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, ScreenManager.Width, ScreenManager.Height);
			x += nb * (BASE_STRIP_WIDTH * 2 + 1);
			drawStripes(x, HEIGHT_TITLE - 1, 8, Theme.CurrentTheme.Primitive);

			ScreenManager.Instance.SpriteBatch.End();
		}

		public void setBaseStrip(int height)
		{
			BASE_STRIP_HEIGHT = height;
			BASE_STRIP_SPACE = BASE_STRIP_HEIGHT;
			BASE_STRIP_WIDTH = (int) (BASE_STRIP_HEIGHT * 0.2);
		}

		private int BASE_STRIP_WIDTH = 7;
		private int BASE_STRIP_HEIGHT = 35;
		private int BASE_STRIP_SPACE = 35;
		private int drawStripes(int x, int y, int width, int height, Color color)
		{
			setBaseStrip(height);
			return (drawStripes(x, y, (width - (BASE_STRIP_SPACE - BASE_STRIP_WIDTH))/ (BASE_STRIP_WIDTH * 2 + 1), color));
		}

		private int drawStripes(int x, int y, int number, Color color)
		{
            VertexPositionColor[] verts = new VertexPositionColor[number * 6];

			int pos = 0;
			for (int i = 0; i < number; ++i)
			{
				verts[pos] = new VertexPositionColor(new Vector3(x, y, 0), color);
				verts[pos + 5] = new VertexPositionColor(new Vector3(x, y, 0), color);
				x += BASE_STRIP_WIDTH;
				verts[++pos] = new VertexPositionColor(new Vector3(x, y, 0), color);
				y += BASE_STRIP_HEIGHT;
				x += BASE_STRIP_SPACE;
				verts[++pos] = new VertexPositionColor(new Vector3(x, y, 0), color);
				verts[++pos] = new VertexPositionColor(new Vector3(x, y, 0), color);
				x -= BASE_STRIP_WIDTH;
				verts[++pos] = new VertexPositionColor(new Vector3(x, y, 0), color);
				y -= BASE_STRIP_HEIGHT;
				x = x - BASE_STRIP_SPACE + (int) (BASE_STRIP_WIDTH * 2 + 1);
				pos += 2;
			}

            ScreenManager.Instance.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, verts, 0, number * 2);

			return (number);
		}

		public void goBack()
		{
			if (ScreenManager.IsEmpty())
			{
				ScreenManager.Exit();
				return;
			}
			ScreenManager.RemoveLast();
			menu_listbox.SelectedItem.Selected = false;
			//Visible = true;
		}

		public void addMenuSeparator()
		{
			ListBoxItem item = new ListBoxItem();
			item.Text = "";
			item.BBCodeEnabled = true;
			item.Size = new Squid.Point(MENU_WIDTH, 24);
			menu_listbox.Items.Add(item);
		}

		public void addMenuItem(GameScreen screen, string title)
		{
			ListBoxItem item = new ListBoxItem();
			item.Text = title;
			item.BBCodeEnabled = true;
			item.Size = new Squid.Point(MENU_WIDTH, 42);
			item.Style = "itemMainMenu";
			menu_listbox.Items.Add(item);

			// Callback
			item.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				_old_screen = _current_screen;
				_current_screen = screen;
				ScreenManager.AddScreen(screen);

				if (_old_screen != null)
					CurrentAnimation = Animation.ToggleMenu;
				else
					CurrentAnimation = Animation.ShowMenu;
			};
		}

		public override int getMenuWidth()
		{
			return MENU_WIDTH;
		}

		public void manageAnimation(GameTime gameTime)
		{
			AnimationElapsedTime += gameTime.ElapsedGameTime.Milliseconds;

			if (CurrentAnimation == Animation.HideMainMenu)
			{
				float posx = (float)AnimationElapsedTime / DELAY_ANIMATION * MENU_WIDTH;
				positionChanged((int)-posx, 0);

				if (AnimationElapsedTime >= DELAY_ANIMATION)
				{
					VisibleMenu = false;
					positionChanged(-MENU_WIDTH, 0);
				}
			}
			else if (CurrentAnimation == Animation.ShowMainMenu)
			{
				float posx = (float)AnimationElapsedTime / DELAY_ANIMATION * MENU_WIDTH;
				positionChanged((int)posx - MENU_WIDTH, 0);

				if (AnimationElapsedTime >= DELAY_ANIMATION)
				{
					positionChanged(0, 0);
					VisibleMenu = true;
					dk_listbox.Enabled = true;
				}
			}
			else if (CurrentAnimation == Animation.ShowMenu)
			{
				float posx = (float)AnimationElapsedTime / DELAY_ANIMATION * _current_screen.getMenuWidth();
				_current_screen.positionChanged(MENU_WIDTH - _current_screen.getMenuWidth() + (int) posx, HEIGHT_TITLE * 2);

				if (AnimationElapsedTime >= DELAY_ANIMATION)
				{
					_current_screen.positionChanged(MENU_WIDTH, HEIGHT_TITLE * 2);
					_current_screen.VisibleMenu = true;
				}
			}
			else if (CurrentAnimation == Animation.ToggleMenu)
			{
				float posx = (float)AnimationElapsedTime / DELAY_ANIMATION * _old_screen.getMenuWidth();
				_old_screen.positionChanged(MENU_WIDTH - (int) posx, HEIGHT_TITLE * 2);

				if (AnimationElapsedTime >= DELAY_ANIMATION)
				{
					AnimationElapsedTime = 0;
					CurrentAnimation = Animation.ShowMenu;
					_old_screen.VisibleMenu = false;
					ScreenManager.Instance.RemoveScreen(_old_screen);
				}
			}

			if ((CurrentAnimation == Animation.ShowMainMenu || CurrentAnimation == Animation.HideMainMenu) && _current_screen != null)
				_current_screen.positionChanged(dk_listbox.Position.x + dk_listbox.Size.x, HEIGHT_TITLE * 2);


			if (AnimationElapsedTime >= DELAY_ANIMATION)
			{
				CurrentAnimation = Animation.Chillout;
				AnimationElapsedTime = 0;
			}
		}
	}
}
