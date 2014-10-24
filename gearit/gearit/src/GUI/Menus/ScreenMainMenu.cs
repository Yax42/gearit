using System;using System.Collections.Generic;
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
using gearit.src.xna.Sound;

namespace GUI
{
	class ScreenMainMenu : GameScreen
	{
		public enum Animation
		{
			Chillout,
			ShowMainMenu,
			HideMainMenu,
			ToggleMenu,
			ShowMenu
		}	

		public class AnimInfo
		{
			public Animation type;
			public int elapsedTime;

			public AnimInfo(Animation atype)
			{
				type = atype;
			}
		}

		// Properties
		static public int MENU_WIDTH = 300;
		static public int MENU_LIST_WIDTH = 200;
		static public int HEIGHT_TITLE = 35;
		static public int DELAY_ANIMATION = 200;
		static public int RESERVED_HEIGHT = HEIGHT_TITLE * 2;

		static public ScreenMainMenu Instance { get; private set; }
		// Gui
		private SamplerState _sampler;
		private RasterizerState _rasterizer;
		private List<AnimInfo> _animations = new List<AnimInfo>();
		private GameScreen _current_screen = null;
		private GameScreen _old_screen = null;
		private Color STRIPE_COLOR = Color.White;
		private Desktop _dk_main_menu;
		private Control main_menu;
		private Control list_container;
		private Label tb_title;
		private string str_title = "";
		private Desktop dk_listbox;
		private ListBox menu_listbox;

        // For the sound
		private ListBoxItem prev_item;
		private ListBoxItem inside_item;

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
			Instance = this;
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
			list_container.Size = new Squid.Point(MENU_LIST_WIDTH, (int)(ScreenManager.Height / 1.45f));
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

			_Gearit = new MyGame();
			_game = new GearitGame();
			_networkGame = new NetworkClientGame("bob", "data/robot/default.gir", "127.0.0.1");
			_play = new MenuPlay();
			_quit = new MenuQuit();
			_soloGame = new ScreenPickManager(_game, true, true,
				delegate()
				{
					ScreenManager.AddScreen(new GearitGame(ScreenPickManager.RobotPath, ScreenPickManager.MapPath));
				});


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

			menu_listbox.Items[0].Click(0);

			_rasterizer = new RasterizerState() { ScissorTestEnable = true };
			_sampler = new SamplerState();
			_sampler.Filter = TextureFilter.Anisotropic;

			#endregion
		}

		public bool CatchExit = true;
		public bool CatchExitLock
		{
			get
			{
				return CatchExit;
			}
			set
			{
				CatchExit = value;
				if (CatchExit)
					ChangeAnim(Animation.ShowMainMenu);
				else
					ChangeAnim(Animation.HideMainMenu);
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			_dk_main_menu.Update();
			dk_listbox.Update();

			if (CatchExit)
			{
				if (Input.Exit)
				{
					AnimInfo anim = _animations.Find(delegate(AnimInfo search) { return (search.type == Animation.ShowMainMenu || search.type == Animation.HideMainMenu); });
					if (anim == null)
						_animations.Add(new AnimInfo(VisibleMenu ? Animation.HideMainMenu : Animation.ShowMainMenu));
					else
					{
						anim.elapsedTime = DELAY_ANIMATION - anim.elapsedTime;
						anim.type = anim.type == Animation.ShowMainMenu ? Animation.HideMainMenu : Animation.ShowMainMenu;
					}
					//dk_listbox.Enabled = false;
				}
			}

			// Animate
			manageAnimation(gameTime);
		}

		void ChangeAnim(Animation animation)
		{
			AnimInfo anim = _animations.Find(delegate(AnimInfo search) { return (search.type == Animation.ShowMainMenu || search.type == Animation.HideMainMenu); });
			if (anim == null)
				_animations.Add(new AnimInfo(animation));
			else if (anim.type != animation)
			{
				anim.elapsedTime = DELAY_ANIMATION - anim.elapsedTime;
				anim.type = animation;
			}
		}

		public void NoSelect()
		{
			menu_listbox.SelectedItem = null;
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

            ScreenManager.Instance.BasicEffect.CurrentTechnique.Passes[0].Apply(); // don't worry be happy
			// Draw menu border
			Squid.Point menu_size = _current_screen.getMenuSize();
			if (_current_screen != null && menu_size.x != 0)
			{
				Squid.Point menu_pos = _current_screen.getMenuPosition();
				VertexPositionColor[] verts = new VertexPositionColor[3];
				verts[0] = new VertexPositionColor(new Vector3(menu_pos.x - 0.5f, menu_pos.y + menu_size.y - 0.5f, 0), Theme.CurrentTheme.Grayie);
				verts[1] = new VertexPositionColor(new Vector3(menu_pos.x - 0.5f + menu_size.x, menu_pos.y + menu_size.y - 0.5f, 0), Theme.CurrentTheme.Grayie);
				verts[2] = new VertexPositionColor(new Vector3(menu_pos.x - 0.5f, menu_pos.y + menu_size.y + HEIGHT_TITLE * 4 - 0.5f, 0), Theme.CurrentTheme.Grayie);
				ScreenManager.Instance.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, verts, 0, 1);
			}

			_dk_main_menu.Draw();

			// Setup graphics
			ScreenManager.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, _sampler, null, _rasterizer);
            ScreenManager.Instance.BasicEffect.CurrentTechnique.Passes[0].Apply(); // don't worry be happy

			Color color;
			str_title = "GEARIT";
			int nbSelected = 0;
			bool play_me = false;
			// Draw stripe listview
			for (int i = 0; i < menu_listbox.Items.Count; ++i)
			{
				ListBoxItem item = menu_listbox.Items[i];
				if (item.Text == "")
					continue;
				if (item.State > ControlState.Default && item.State != ControlState.Disabled)
				{
					if (inside_item != item && prev_item != item)
					{
						play_me = true;
						prev_item = item;
						nbSelected++;
					}
					str_title = item.Text;
					color = Theme.CurrentTheme.Primitive;
				}
				else
					color = STRIPE_COLOR;
				drawStripes(item.Location.x + 8, item.Location.y + 6, item.Size.x - 8, 25, color);
			}
			ScreenManager.Instance.SpriteBatch.End();

			if (play_me == true && nbSelected == 1)
				AudioManager.Instance.PlaySound("hover");
			dk_listbox.Draw();

			drawHeader();
		}

		public void drawHeader()
		{
			ScreenManager.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, _sampler, null, _rasterizer);
            ScreenManager.Instance.BasicEffect.CurrentTechnique.Passes[0].Apply(); // don't worry be happy

			int padding_title = 0;
			int padding_stripe = 0;
			if (_dk_main_menu.Position.x < 0)
			{
				padding_stripe = (-(int)(_dk_main_menu.Position.x / 1.4f));
				padding_title = (-(int)(_dk_main_menu.Position.x / 3));
			}
			setBaseStrip(HEIGHT_TITLE);
			int x = MENU_WIDTH - MENU_LIST_WIDTH - BASE_STRIP_WIDTH / 2 - ((BASE_STRIP_WIDTH * 2 + 1) * 5) - 9 - padding_stripe;
			int nb = drawStripes(x, HEIGHT_TITLE - 1, (int) (MENU_WIDTH * 1.4f), BASE_STRIP_HEIGHT, Theme.CurrentTheme.Grayie);
			ScreenManager.Instance.GraphicsDevice.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle(0, HEIGHT_TITLE - 1, _dk_main_menu.Position.x + _dk_main_menu.Size.x, HEIGHT_TITLE);
			drawStripes(x, HEIGHT_TITLE - 1, MENU_WIDTH + BASE_STRIP_SPACE, BASE_STRIP_HEIGHT, STRIPE_COLOR);
			x += ((BASE_STRIP_WIDTH * 2 + 1) * nb);
			ScreenManager.Instance.GraphicsDevice.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, ScreenManager.Width, ScreenManager.Height);
			drawStripes(x, HEIGHT_TITLE - 1, (int) (MENU_WIDTH * 0.4f), BASE_STRIP_HEIGHT, Theme.CurrentTheme.Primitive);

			ScreenManager.Instance.SpriteBatch.End();

			ScreenManager.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null);
			Squid.Point str_size = SpriteFonts.GetTextSize(str_title, ScreenManager.Fonts.TitleFont);
			ScreenManager.Instance.SpriteBatch.DrawString(ScreenManager.Fonts.TitleFont, str_title, new Vector2(MENU_WIDTH - MENU_LIST_WIDTH + MENU_LIST_WIDTH / 2 - str_size.x / 2 - padding_title, HEIGHT_TITLE + 2), Theme.CurrentTheme.Primitive);
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

		public void addMenuSeparator()
		{
			ListBoxItem item = new ListBoxItem();
			item.Text = "";
			item.Style = "panel";
			item.BBCodeEnabled = true;
			item.Size = new Squid.Point(MENU_WIDTH, 24);
			menu_listbox.Items.Add(item);
		}

		int nb_item = 0;
		int current_item_id = 0;
		public void addMenuItem(GameScreen screen, string title)
		{
			ListBoxItem item = new ListBoxItem();
			item.Text = title;
			item.BBCodeEnabled = true;
			item.Size = new Squid.Point(MENU_WIDTH, 42);
			item.Style = "itemMainMenu";
			menu_listbox.Items.Add(item);
			int item_id = nb_item++;

			// Callback
			item.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				AnimInfo info = _animations.Find(delegate(AnimInfo search) { return search.type == Animation.ShowMenu || search.type == Animation.ToggleMenu; });
				inside_item = menu_listbox.Items[current_item_id];
				AudioManager.Instance.PlaySound("click");
				if (screen == _current_screen || info != null)
				{
					if (info != null && screen != _current_screen)
						menu_listbox.Items[current_item_id].Click(0);
					return;
				}

				current_item_id = item_id;
				_old_screen = _current_screen;
				_current_screen = screen;
				ScreenManager.AddScreen(screen);
				_current_screen.positionChanged(-_current_screen.getMenuSize().x, 0);
				if (_old_screen != null)
					_animations.Add(new AnimInfo(Animation.ToggleMenu));
				else
					_animations.Add(new AnimInfo(Animation.ShowMenu));
			};
		}

		public override Squid.Point getMenuSize()
		{
			return (new Squid.Point(MENU_WIDTH, ScreenManager.Height));
		}

		public void manageAnimation(GameTime gameTime)
		{
			foreach (AnimInfo anim in _animations)
			{
				anim.elapsedTime += gameTime.ElapsedGameTime.Milliseconds;

				if (anim.type == Animation.HideMainMenu)
				{
					float posx = (float)anim.elapsedTime / DELAY_ANIMATION * MENU_WIDTH;
					positionChanged((int)-posx, 0);

					if (anim.elapsedTime >= DELAY_ANIMATION)
					{
						VisibleMenu = false;
						positionChanged(-MENU_WIDTH, 0);
					}
				}
				else if (anim.type == Animation.ShowMainMenu)
				{
					float posx = (float)anim.elapsedTime / DELAY_ANIMATION * MENU_WIDTH;
					positionChanged((int)posx - MENU_WIDTH, 0);

					if (anim.elapsedTime >= DELAY_ANIMATION)
					{
						positionChanged(0, 0);
						VisibleMenu = true;
						dk_listbox.Enabled = true;
					}
				}
				else if (anim.type  == Animation.ShowMenu)
				{
					float posx = (float)anim.elapsedTime / DELAY_ANIMATION * _current_screen.getMenuSize().x;
					_current_screen.positionChanged(dk_listbox.Position.x + dk_listbox.Size.x - _current_screen.getMenuSize().x + (int)posx + 1, HEIGHT_TITLE * 2 + 1);

					if (anim.elapsedTime >= DELAY_ANIMATION)
					{
						_current_screen.positionChanged(dk_listbox.Position.x + dk_listbox.Size.x + 1, HEIGHT_TITLE * 2 + 1);
						_current_screen.VisibleMenu = true;
					}
				}
				else if (anim.type == Animation.ToggleMenu)
				{
					float posx = (float)anim.elapsedTime / DELAY_ANIMATION * _old_screen.getMenuSize().x;
					_old_screen.positionChanged(dk_listbox.Position.x + dk_listbox.Size.x - (int)posx + 1, HEIGHT_TITLE * 2 + 1);

					if (anim.elapsedTime >= DELAY_ANIMATION)
					{
						anim.elapsedTime = 0;
						anim.type = Animation.ShowMenu;
						_old_screen.VisibleMenu = false;
						ScreenManager.Instance.RemoveScreen(_old_screen);
					}
				}

				if ((anim.type == Animation.ShowMainMenu || anim.type == Animation.HideMainMenu) && _current_screen != null && _animations.Find(delegate(AnimInfo search) { return search.type == Animation.ShowMenu || search.type == Animation.ToggleMenu; }) == null)
					_current_screen.positionChanged(dk_listbox.Position.x + dk_listbox.Size.x + 1, HEIGHT_TITLE * 2 + 1);

				if (anim.elapsedTime >= DELAY_ANIMATION)
					anim.type = Animation.Chillout;
			}

			_animations.RemoveAll(delegate(AnimInfo anim) { return anim.type == Animation.Chillout; });
		}
	}
}
