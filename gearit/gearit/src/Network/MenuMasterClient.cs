using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using gearit.xna;
using GUI2;
using gearit.src.utility;
using gearit.src.GUI;
using GUI;

namespace gearit.src
{
	class MenuMasterClient : Desktop
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

		public static MenuMasterClient Instance { set; get; }

		public MenuMasterClient(ScreenManager ScreenManager)
		{
			Instance = this;

			#region main

			#region init

			_ScreenManager = ScreenManager;

			padding_x = 0;

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

			Label lb;
			label_name = new Label();
			lb = label_name;
			lb.Text = "TEXTE_1";
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuTitle";
			lb.Parent = this;

			y += lb.Size.y + PADDING;

			#region piecedrop

			Button btn;
			//// Circle and Pipe
			// Circle
			btn = rb_ball;
			btn.Text = "EXIT";
			btn.Style = "button";
			btn.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.Cursor = Cursors.Move;
			btn.Tag = BType.Ball;
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
					//ScreenMainMenu.GoBack = true;
			};

			btn = rb_wall;
			btn.Text = "Wall";
			btn.Style = "button";
			btn.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(MENU_WIDTH / 2 + 1, y);
			btn.Parent = this;
			btn.Cursor = Cursors.Move;
			btn.Tag = BType.Wall;

			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				rb_wall.Checked = !rb_wall.Checked;
				if (rb_wall.Checked)
					saveMap();
			};

			y += btn.Size.y + 2; 

			#endregion

			y += ITEM_HEIGHT + PADDING;

			#region Helper
			helper.Text =
			"Help (F1)\n" +
			"----------------------------------------------------------------------------------------\n" +
			"";


			helper.Size = new Squid.Point(370, 280);
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
			btn.Style = "button";
			btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				swapHelp();
			};

			y -= btn.Size.y + PADDING;

			#endregion

			#endregion
		}

		public void Update()
		{
			base.Update();
			if (Input.Exit)
			{
				if (_messageBoxSave != null)
				{
					_messageBoxSave.cancel();
					_messageBoxSave = null;
				}
			}
		}

		//---------------------SAVE&LOAD-----------------------------

		public void saveMap()
		{
			_messageBoxSave = new MessageBoxSave(this, "TEXT_2", safeSaveMap, setFocus, "map");
		}

		private MessageBoxSave _messageBoxSave = null;
		public void saveasMap(bool mustExit = false)
		{
			_messageBoxSave = new MessageBoxSave(this, "TEXT_2", safeSaveMap, setFocus, "map");
		}

		public void safeSaveMap(string name, bool mustExit = false)
		{
			setFocus(false);
			// some actions
		}


		public void updateButtonMapName()
		{
			label_name.Text = "TEXT_3";
		}

		//--------------------------------------------------------------------

		public bool hasFocus()
		{
			return (_has_focus);
		}


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
