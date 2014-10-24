using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using gearit.xna;
using GUI;
using gearit.src.utility;
using gearit.src.map;
using FarseerPhysics.Dynamics;
using gearit.src.editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.robot;
using gearit.src.GUI.Picker;

namespace gearit.src.GUI
{
	class MenuPickItem : Desktop
	{
		// Propertie
		static public int MI_WIDTH = 150;
		static public int MI_HEIGHT = 150;
		static public int MI_SEPARATOR = 4;


		static public int MENU_WIDTH = 220;
		static public int TOOLS_HEIGHT = 32;
		static public int PIECE_HEIGHT = 400;
		static public int SPOT_HEIGHT = 400;
		static public int ITEM_HEIGHT = 30;
		static public int PADDING = 2;
		static public int PADDING_NODE = 24;
		static public int HEIGHT_NODE = 50;

		//// Gui
		private bool _has_focus = false;
		private int padding_x;

		// Tools
//		private ListBox lb_jointure;
		private Label label_name;

		private EditorCamera Camera;
		private ScreenManager ScreenManager;
		//private ListBox menu_listbox = new ListBox();
		private Button rb_exit = new Button();
		private string[] Files;
		private Map[] Maps;
		private Robot[] Robots;
		private bool IsMap;
		//private World[] Worlds;

		//return values
		public static string Result = null;


		//private DrawGame DrawGame;


		public MenuPickItem(ScreenManager screenManager, bool isMap)
		{
			IsMap = IsMap;

			ScreenManager = screenManager;
			int x;
			int y;

			#region init

			padding_x = 0;

			// MapEditor.Instance.VisibleMenu = true;
			//ShowCursor = true;
			Position = new Squid.Point(padding_x, 0);

			// Full width to get the cursor propagation
			Size = new Squid.Point(ScreenManager.Width - padding_x, ScreenManager.Height);

			#endregion

			y = ScreenManager.Height - ITEM_HEIGHT;
			var btn = rb_exit;
			btn.Text = "EXIT";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.Cursor = Cursors.Move;
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				ScreenPickManager.Exit = true;
			};

			Camera = new EditorCamera(new Viewport(0, 0, MI_WIDTH, MI_HEIGHT));
			//DrawGame = new DrawGame(ScreenManager.GraphicsDevice);
			/****/
			if (isMap)
				GenerateItems("data/map/", ".gim");
			else
				GenerateItems("data/robot/", ".gir");
			/****/

		}

		private void GenerateItems(string dir, string type)
		{
			Files = FileManager.GetFiles(dir, type);
			Maps = new Map[Files.Count()];
			int x = 0;
			int y = 0;
			World w = new World(Vector2.Zero);
			SerializerHelper.World = w;
			for (int i = 0; i < Files.Count(); i++)
			{
				if (IsMap && false)
				{
					Maps[i] = (Map)Serializer.DeserializeItem(Files[i]);
					if (Maps[i] == null)
						continue;
				}
				Button btn = new Button();
				btn.Text = Files[i].Split("/".ToCharArray()).Last();
				btn.Style = "itemPickButton";
				btn.Size = new Squid.Point(MI_WIDTH, MI_HEIGHT);
				btn.Position = new Squid.Point(x, y);
				btn.Parent = this;
				btn.Cursor = Cursors.Move;
				string str = Files[i];
				btn.MouseClick += delegate(Control snd, MouseEventArgs e)
				{
					Result = str;
				};

				x += MI_WIDTH + MI_SEPARATOR;
				if (x + MI_WIDTH > ScreenManager.Width)
				{
					x = 0;
					y += MI_HEIGHT + MI_SEPARATOR;
				}
			}
		}

		public void Update()
		{
			base.Update();
			//Camera.input();
			//Camera.update();
		}

		public bool hasFocus()
		{
			return (_has_focus);
		}

		public void setFocus(bool focus)
		{
			_has_focus = focus;
		}

		public void DrawItems(DrawGame DrawGame)
		{
				return;
			int x = 0;
			int y = 0;

			for (int i = 0; i < Files.Count(); i++)
			{


				DrawGame.BeginPrimitive(Camera,
					Matrix.CreateOrthographic(100, 100, 0, 1));
					/*Matrix.CreateOrthographicOffCenter(
					(0),
					(MI_WIDTH),
					(MI_HEIGHT),
					(0),
					0f, 1f));*/

				if (Maps[i] != null)
					Maps[i].DrawDebug(DrawGame);
				DrawGame.End();

				x += MI_WIDTH + MI_SEPARATOR;
				if (x + MI_WIDTH > ScreenManager.Width)
				{
					x = 0;
					y += MI_HEIGHT + MI_SEPARATOR;
				}
			}
		}
	}
}