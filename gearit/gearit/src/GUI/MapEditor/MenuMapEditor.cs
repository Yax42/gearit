using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using gearit.xna;
using GUI;
using gearit.src.robot;
using gearit.src.utility;
using System.Globalization;
using gearit.src.editor.map.action;
using gearit.src.GUI;
using gearit.src.editor.map;

namespace gearit.src.map.robot
{
	class MenuMapEditor : Desktop
	{
		// Propertie
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

		private ScreenManager _ScreenManager;
		//private ListBox menu_listbox = new ListBox();
		private Control background = new Control();
		private Button rb_wall = new Button();
		private Button rb_ball = new Button();
		private Button Help_btn;

		private TextBox helper = new TextBox();

		enum BType
		{
			Ball,
			Wall
		};

		public static MenuMapEditor Instance { set; get; }

		public MenuMapEditor(ScreenManager ScreenManager)
		{
			Instance = this;

			#region main

			#region init

			_ScreenManager = ScreenManager;

			padding_x = MapEditor.Instance.VisibleMenu ? MainMenu.MENU_WIDTH : 0;

			// MapEditor.Instance.VisibleMenu = true;
			//ShowCursor = true;
			Position = new Squid.Point(padding_x, 0);

			// Full width to get the cursor propagation
			Size = new Squid.Point(ScreenManager.Width - padding_x, ScreenManager.Height);

			int y = 0;

			#endregion

			// Background
			background.Parent = this;
			background.Style = "menu";
			background.Position = new Point(0, y);
			background.Size = new Point(MENU_WIDTH, _ScreenManager.Height);

			// Drop callback
			AllowDrop = true;
			DragDrop += delegate(Control sender, DragDropEventArgs e)
			{
				if (e.DraggedControl.Tag.Equals(BType.Wall))
					MapEditor.Instance.doAction(ActionTypes.CREATE_WALL);
				else
					MapEditor.Instance.doAction(ActionTypes.CREATE_BALL);

			};

			//// Tools section
			// Auto select menu tools
			Label lb;

			// Title
			/*
			Label lb = new Label();
			lb.Text = "Pieces";
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuTitle";
			lb.Parent = this;

			y += lb.Size.y + PADDING;
			*/
			label_name = new Label();
			lb = label_name;
			lb.Text = "~ " + MapEditor.Instance.NamePath + " ~";
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuTitle";
			lb.Parent = this;

			y += lb.Size.y + PADDING;

			#region piecedrop

			Button btn;
			/*
			btn = new Button();
			btn.Text = "Object";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.MouseDrag += dragPiece;
			btn.Cursor = Cursors.Move;
			btn.Tooltip = "(W)";
			btn.Checked = false;
			y += btn.Size.y + PADDING;
			*/

			//// Circle and Pipe
			// Circle
			btn = rb_ball;
			btn.Text = "Ball";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.Cursor = Cursors.Move;
			btn.Tooltip = "(A)";
			btn.Tag = BType.Ball;
			btn.MouseDrag += dragPiece;

			btn = rb_wall;
			btn.Text = "Wall";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(MENU_WIDTH / 2 + 1, y);
			btn.Parent = this;
			btn.Cursor = Cursors.Move;
			btn.Tooltip = "(A)";
			btn.Tag = BType.Wall;
			btn.MouseDrag += dragPiece;

			y += btn.Size.y + 2; 

			//Callback
			/*
			rb_ball.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				if (!rb_ball.Checked)
					swap_type();
			};

			rb_wall.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				if (!rb_wall.Checked)
					swap_type();
			};
			*/

			#endregion

			y += ITEM_HEIGHT + PADDING;



			#region Helper
			helper.Text =
			"Help (F1)\n" +
			"----------------------------------------------------------------------------------------\n" +
			"Select piece A.......................................(left click)\n" +
			"Select piece B.......................................(shift+left click)\n" +
			"Select spot S.........................................[automatically\n" +
			" selected by selecting both of the pieces it's linked to]\n" +
			"Move piece A........................................(right click)\n" +
			"Delete piece A.......................................(R)\n" +
			"Delete spot S.........................................(shift+R)\n" +
			"Move A to S anchor...............................(shift+right click)\n" +
			"Resize A................................................(S)\n" +
			"Switch piece type pT.............................(A)\n" +
			"Switch spot type sT...............................(shift+A)\n" +
			"Create a pT and link it to A with a sT....(W)\n" +
			"Link A and B with a sT..........................(shfit+W)\n" +
			"Move camera........................................(scroll click)\n" +
			"Zoom/Unzoom.......................................(scrolling)\n" +
			"Undo......................................................(ctrl+Z)\n" +
			"Redo......................................................(ctrl+Y)\n" +
			"Show/Hide A..........................................(E)\n" +
			"Show every pieces.................................(space)\n" +
			"Save.......................................................(ctrl+S)\n" +
			"Save as...................................................(ctrl+shift+S)\n" +
			"Load.......................................................(ctrl+D)\n" +
			"";


			helper.Size = new Squid.Point(370, 410);
			helper.Position = new Squid.Point(ScreenManager.Width - helper.Size.x, 0);
			helper.Style = "messagebox";
			helper.Parent = this;
			helper.Enabled = false;
			helper.Visible = false;
			#endregion

			#region action

			//-----------------------------------------------

			y = ScreenManager.Height - ITEM_HEIGHT;

			Help_btn = new Button();
			btn = Help_btn;
			btn.Text = "Help (F1)";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				swapHelp();
			};

			y -= btn.Size.y + PADDING;

			// Load
			btn = new Button();
			btn.Text = "Load";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH / 3 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.Tooltip = "(ctrl+D)";
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				loadMap();
			};

			btn = new Button();
			btn.Text = "Save";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH / 3 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(MENU_WIDTH / 3 + 1, y);
			btn.Parent = this;
			btn.Tooltip = "(ctrl+S)";
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				saveMap();
			};

			btn = new Button();
			btn.Text = "Save as";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH / 3 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(MENU_WIDTH * 2 / 3 + 1, y);
			btn.Parent = this;
			btn.Tooltip = "(ctrl+shift+S)";
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				saveasMap();
			};

			y -= btn.Size.y / 2+ PADDING;

			lb = new Label();
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT / 2);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuTitle";
			lb.Parent = this;
			y += ITEM_HEIGHT / 2 + PADDING;

			#endregion

			#endregion
		}

		public void saveMap()
		{
			//MapEditor.Instance.doAction(ActionTypes.SAVE_ROBOT);
			if (MapEditor.Instance.NamePath == "")
			{
				setFocus(true);
				new MessageBoxSave(this, MapEditor.Instance.Map.Name, safeSaveMap);
			}
			else
				MapEditor.Instance.doAction(ActionTypes.SAVE);
		}
		public void saveasMap()
		{
			setFocus(true);
			new MessageBoxSave(this, MapEditor.Instance.Map.Name, safeSaveMap);
		}

		public void safeSaveMap(string name)
		{
			setFocus(false);
			if (name == "")
				return;
			MapEditor.Instance.NamePath = name;
			MapEditor.Instance.doAction(ActionTypes.SAVE);
			label_name.Text = "~ " + MapEditor.Instance.NamePath + " ~";
		}

		public void loadMap()
		{
			setFocus(true);
			new MessageBoxLoad(@"data/robot/", ".gir", this, safeLoadMap);
		}

		public void safeLoadMap(string name)
		{
			setFocus(false);
			MapEditor.Instance.NamePath = name;
			MapEditor.Instance.doAction(ActionTypes.LOAD);
			label_name.Text = "~ " + MapEditor.Instance.NamePath + " ~";
		}

		void dragPiece(Control sender, MouseEventArgs e)
		{
			ImageControl img = new ImageControl();

			img.Texture = "RobotEditor/revolute.png";

			img.Size = new Squid.Point(32, 32);
			img.Position = new Point((int) Input.position().X - 16 - padding_x, (int) Input.position().Y - 16);
			img.Style = "itemMainMenu";
			img.Tag = sender.Tag;

			DoDragDrop(img);
		}

		public bool hasFocus()
		{
			if (_has_focus)
				return (true);

			int padding = MapEditor.Instance.VisibleMenu ? MainMenu.MENU_WIDTH : 0;

			bool menu_has_focus = background.Position.x + padding <= Input.position().X &&
				background.Position.x + background.Size.x + padding >= Input.position().X &&
				background.Position.y <= Input.position().Y &&
				background.Position.y + background.Size.y >= Input.position().Y;

			return (menu_has_focus);
		}

		/*
		public void swap_type()
		{
			rb_ball.Checked = !rb_ball.Checked;
			rb_wall.Checked = !rb_wall.Checked;
		}
		*/

		public void swapHelp()
		{
			helper.Visible = !helper.Visible;
			Help_btn.Checked = helper.Visible;
		}

		public void setFocus(bool focus)
		{
			_has_focus = focus;
		}
	}
}
