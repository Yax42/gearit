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
using gearit.src.editor.robot.action;
using gearit.src.GUI.RobotEditor;
using gearit.src.GUI;

namespace gearit.src.editor.robot
{
	class MenuRobotEditor : Desktop
	{
		enum FPiece
		{
			Circle = 1,
			Pipe = 2,
			None
		}

		// Editor
		public Piece Piece {get; set;}
		public ISpot Spot {get; set;}

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
		private ListBox lb_jointure;
		private FPiece flag_piece;
		private Button rb_revolute = new Button();
		private Button rb_prismatic = new Button();
		private Label label_name;

		// Piece & Spot
		private ScreenManager _ScreenManager;
		private ListBox menu_listbox = new ListBox();
		private Control background = new Control();
		private Button rb_rod = new Button();
		private Button rb_wheel = new Button();
		private Button Help_btn;

		private Frame spot_container = new Frame();
		private TextBox spot_name = new TextBox();
		private TextBox spot_force = new TextBox();
		private Frame spot_distance_container = new Frame();
		private TextBox spot_distance = new TextBox();

		private Frame piece_rotation_container = new Frame();
		private TextBox piece_rotation = new TextBox();

		private TextBox piece_weight = new TextBox();
		private TextBox piece_size = new TextBox();
		private TextBox piece_x = new TextBox();
		private TextBox piece_y = new TextBox();
		private TextBox helper = new TextBox();

		// Script editor
		private Panel panel_script = new Panel();
		private List<TreeNodeEvent> _nodes = new List<TreeNodeEvent>();
		private Button btn_add_event = new Button();

		public static MenuRobotEditor Instance { set; get; }

		public bool IsPrismatic
		{
			get
			{
				return rb_prismatic.Checked;
			}
			set
			{
				rb_prismatic.Checked = value;
				rb_revolute.Checked = !value;
			}
		}

		public bool IsWheel
		{
			get
			{
				return rb_wheel.Checked;
			}
			set
			{
				rb_wheel.Checked = value;
				rb_rod.Checked = !value;
			}
		}

		public MenuRobotEditor(ScreenManager ScreenManager)
		{
			Instance = this;

			#region main

			#region init

			_ScreenManager = ScreenManager;

			padding_x = RobotEditor.Instance.VisibleMenu ? MainMenu.MENU_WIDTH : 0;

			// RobotEditor.Instance.VisibleMenu = true;
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
				RobotEditor.Instance.doAction(ActionTypes.CREATE_PIECE);
			};

			#region tools

			//// Tools section
			// Auto select menu tools
			flag_piece = FPiece.None;
			rb_prismatic.Checked = true;
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
			lb.Text = "~ " + RobotEditor.Instance.NamePath + " ~";
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuTitle";
			lb.Parent = this;

			y += lb.Size.y + PADDING;

			#region piecedrop
			Button btn = new Button();
			btn.Text = "Piece";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.MouseDrag += dragPiece;
			btn.Cursor = Cursors.Move;
			btn.Tooltip = "(W)";
			btn.Checked = false;
			y += btn.Size.y + PADDING;

			//// Circle and Pipe
			// Circle
			btn = rb_wheel;
			btn.Text = "Circle";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.Tag = FPiece.Circle;
			btn.Cursor = Cursors.Move;
			btn.Tooltip = "(A)";
			btn.Checked = true;

			btn = rb_rod;
			btn.Text = "Pipe";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(MENU_WIDTH / 2 + 1, y);
			btn.Parent = this;
			btn.Tag = FPiece.Pipe;
			btn.Cursor = Cursors.Move;
			btn.Tooltip = "(A)";

			y += btn.Size.y + 2; 

			//Callback
			rb_wheel.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				if (!rb_wheel.Checked)
					swap_pieces();
			};

			rb_rod.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				if (!rb_rod.Checked)
					swap_pieces();
			};

			#endregion

			#region jointuretype

			// Title
			//lb = new Label();
			//lb.Text = "Jointure";
			//lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			//lb.Position = new Squid.Point(0, y);
			//lb.Style = "itemMenuTitle";
			//lb.Parent = this;

			//y += lb.Size.y + PADDING;

			// Piece jointure type
			rb_revolute.Text = "Revolute";
			rb_revolute.Style = "itemMenuButton";
			rb_revolute.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
			rb_revolute.Position = new Squid.Point(0, y);
			rb_revolute.Parent = this;
			rb_revolute.Tooltip = "Circular motor (Shift+A)";

			rb_prismatic.Text = "Prismatic";
			rb_prismatic.Style = "itemMenuButton";
			rb_prismatic.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
			rb_prismatic.Position = new Squid.Point(MENU_WIDTH / 2 + 1, y);
			rb_prismatic.Parent = this;
			rb_prismatic.Tooltip = "Piston motor (Shift+A)";

			// Callback button
			rb_revolute.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				if (!rb_revolute.Checked)
					swap_jointure();
			};

			rb_prismatic.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				if (!rb_prismatic.Checked)
					swap_jointure();
			};
			y += ITEM_HEIGHT + PADDING;

			lb = new Label();
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT / 2);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuTitle";
			lb.Parent = this;
			y += ITEM_HEIGHT / 2 + PADDING;

			// Link
			btn = new Button();
			btn.Text = "Link (Shift+W)";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.Tooltip = "Link two pieces together with specified jointure";
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				RobotEditor.Instance.doAction(ActionTypes.LINK);
			};

			// Remove
			btn = new Button();
			btn.Text = "Delete (R)";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(MENU_WIDTH / 2 + 1, y);
			btn.Parent = this;
			btn.Tooltip = "Remove selected piece";
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				RobotEditor.Instance.doAction(ActionTypes.DELETE_PIECE);
			};

			y += btn.Size.y + PADDING;

			//-----------------------------------------------
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
				loadRobot();
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
				saveRobot();
			};

			btn = new Button();
			btn.Text = "Save as";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH / 3 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(MENU_WIDTH * 2 / 3 + 1, y);
			btn.Parent = this;
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				saveasRobot();
			};

			y += btn.Size.y + PADDING;

			btn = new Button();
			btn.Text = "Script Editor";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.Tooltip = "Toggle the script editor";
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				panel_script.Visible = !panel_script.Visible;
			};

			y += btn.Size.y + PADDING;

			Help_btn = new Button();
			btn = Help_btn;
			btn.Text = "Help";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				swapHelp();
			};

			y += btn.Size.y + PADDING;
			#endregion

			#endregion

			#region piece

			// Piece section
			lb = new Label();
			lb.Text = "Piece data";
			lb.TextColor = 255;
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuTitle";
			lb.Parent = this;

			y += lb.Size.y + PADDING;

			// Piece name
			lb = new Label();
			lb.Text = "Weight";
			lb.Size = new Squid.Point(70, ITEM_HEIGHT);
			lb.Position = new Squid.Point(8, y);
			lb.Style = "itemMenu";
			lb.Parent = this;

			piece_weight.Text = "45";
			piece_weight.Size = new Squid.Point(124, ITEM_HEIGHT - PADDING * 3);
			piece_weight.Position = new Squid.Point(lb.Size.x + 8, y + PADDING + 1);
			piece_weight.Style = "textbox";
			piece_weight.Parent = this;
			piece_weight.Enabled = false;

			y += ITEM_HEIGHT;

			// Piece size
			lb = new Label();
			lb.Text = "Size";
			lb.Size = new Squid.Point(70, ITEM_HEIGHT);
			lb.Position = new Squid.Point(8, y);
			lb.Style = "itemMenu";
			lb.Parent = this;

			piece_size.Text = "8";
			piece_size.Size = new Squid.Point(124, ITEM_HEIGHT - PADDING * 3);
			piece_size.Position = new Squid.Point(lb.Size.x + 8, y + PADDING + 1);
			piece_size.Style = "textbox";
			piece_size.Parent = this;
			piece_size.Mode = TextBoxMode.Numeric;
			piece_size.Enabled = false;

			y += ITEM_HEIGHT;
			
			// Piece position
			lb = new Label();
			lb.Text = "X";
			lb.Size = new Squid.Point(20, ITEM_HEIGHT);
			lb.Position = new Squid.Point(8, y);
			lb.Style = "itemMenu";
			lb.Parent = this;

			piece_x.Text = "15";
			piece_x.Size = new Squid.Point(67, ITEM_HEIGHT - PADDING * 3);
			piece_x.Position = new Squid.Point(lb.Position.x + lb.Size.x + PADDING, y + PADDING + 1);
			piece_x.Style = "textbox";
			piece_x.Enabled = false;
			piece_x.Parent = this;

			lb = new Label();
			lb.Text = "Y";
			lb.Size = new Squid.Point(20, ITEM_HEIGHT);
			lb.Position = new Squid.Point(piece_x.Position.x + piece_x.Size.x + PADDING * 3, y);
			lb.Style = "itemMenu";
			lb.Parent = this;

			piece_y.Text = "81";
			piece_y.Size = new Squid.Point(67, ITEM_HEIGHT - PADDING * 3);
			piece_y.Position = new Squid.Point(lb.Position.x + lb.Size.x + PADDING, y + PADDING + 1);
			piece_y.Style = "textbox";
			piece_y.Enabled = false;
			piece_y.Parent = this;

			y += ITEM_HEIGHT + PADDING;

			/* C'est quoi ça ? en tout cas ça semble ne pas marcher
			// Piece rotation
			piece_rotation_container.Size = new Squid.Point(Size.x, ITEM_HEIGHT);
			piece_rotation_container.Position = new Squid.Point(0, y);
			piece_rotation_container.Parent = this;

			lb = new Label();
			lb.Text = "Rotation";
			lb.Size = new Squid.Point(70, ITEM_HEIGHT);
			lb.Position = new Squid.Point(8, 0);
			//lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenu";
			lb.Parent = piece_rotation_container;

			piece_rotation.Text = "8.23";
			piece_rotation.Size = new Squid.Point(124, ITEM_HEIGHT - PADDING * 3);
			piece_rotation.Position = new Squid.Point(lb.Size.x + 8, PADDING + 1);
			piece_rotation.Style = "textbox";
			piece_rotation.Parent = piece_rotation_container;
			piece_rotation.Enabled = false;

			y += lb.Size.y + PADDING * 2;
			*/

			// Spot section
			spot_container = new Frame();
			spot_container.Parent = this;
			spot_container.Position = new Squid.Point(0, y);
			spot_container.Size = new Squid.Point(Size.x, Size.y - y);

			y = 0;

			lb = new Label();
			lb.Text = "Spot data";
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuTitle";
			lb.Parent = spot_container;

			y += lb.Size.y + PADDING;

			// Spot name
			lb = new Label();
			lb.Text = "Name";
			lb.Size = new Squid.Point(70, ITEM_HEIGHT);
			lb.Position = new Squid.Point(8, y);
			lb.Style = "itemMenu";
			lb.Parent = spot_container;

			spot_name.Text = "spotNumber";
			spot_name.Size = new Squid.Point(124, ITEM_HEIGHT - PADDING * 3);
			spot_name.Position = new Squid.Point(lb.Size.x + 8, y + PADDING + 1);
			spot_name.Style = "textbox";
			spot_name.Parent = spot_container;
			spot_name.TextChanged += delegate(Control snd)
			{
				Spot.Name = ((TextBox)snd).Text;
			};

			y += ITEM_HEIGHT;

			// Spot size
			lb = new Label();
			lb.Text = "Force";
			lb.Size = new Squid.Point(70, ITEM_HEIGHT);
			lb.Position = new Squid.Point(8, y);
			lb.Style = "itemMenu";
			lb.Parent = spot_container;

			spot_force.Text = "8";
			spot_force.Size = new Squid.Point(124, ITEM_HEIGHT - PADDING * 3);
			spot_force.Position = new Squid.Point(lb.Size.x + 8, y + PADDING + 1);
			spot_force.Style = "textbox";
			spot_force.Parent = spot_container;
			spot_force.Mode = TextBoxMode.Numeric;
			spot_force.TextChanged += delegate(Control snd)
			{
				if (((TextBox)snd).Text == "")
					Spot.Force = 0;
				else
					Spot.Force = float.Parse(((TextBox)snd).Text, CultureInfo.InvariantCulture.NumberFormat);
			};

			y += ITEM_HEIGHT;

			// Spot distance
			spot_distance_container.Size = new Squid.Point(Size.x, ITEM_HEIGHT);
			spot_distance_container.Position = new Squid.Point(0, y);
			spot_distance_container.Parent = spot_container;

			lb = new Label();
			lb.Text = "Distance";
			lb.Size = new Squid.Point(70, ITEM_HEIGHT);
			lb.Position = new Squid.Point(8, 0);
			lb.Style = "itemMenu";
			lb.Parent = spot_distance_container;

			spot_distance.Text = "8.23";
			spot_distance.Size = new Squid.Point(124, ITEM_HEIGHT - PADDING * 3);
			spot_distance.Position = new Squid.Point(lb.Size.x + 8, PADDING + 1);
			spot_distance.Style = "textbox";
			spot_distance.Parent = spot_distance_container;
			spot_distance.Enabled = false;

			#endregion

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


			helper.Size = new Squid.Point(400, 400);
			helper.Position = new Squid.Point(880, 0);
			helper.Style = "messagebox";
			helper.Parent = this;
			helper.Enabled = false;
			helper.Visible = true;

			#region action


			// Title
			/*
			lb = new Label();
			lb.Text = "Actions";
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuTitle";
			lb.Parent = this;
			*/

			#endregion

			#region script_editor

			// Panel script editor
			panel_script.Style = "menu";
			panel_script.Parent = this;

			panel_script.VScroll.Margin = new Margin(0, 8, 8, 8);
			panel_script.VScroll.Size = new Squid.Point(14, 10);
			panel_script.VScroll.Slider.Style = "vscrollTrack";
			panel_script.VScroll.Slider.Button.Style = "vscrollButton";
			panel_script.VScroll.ButtonUp.Style = "vscrollUp";
			panel_script.VScroll.ButtonUp.Size = new Squid.Point(10, 20);
			panel_script.VScroll.ButtonDown.Style = "vscrollUp";
			panel_script.VScroll.ButtonDown.Size = new Squid.Point(10, 20);
			panel_script.VScroll.Slider.Margin = new Margin(0, 2, 0, 2);

			panel_script.Position = new Point(padding_x + MENU_WIDTH, _ScreenManager.Height - ITEM_HEIGHT);
			panel_script.Size = new Point(_ScreenManager.Width - padding_x - MENU_WIDTH, ITEM_HEIGHT);

			// Add event
			btn_add_event.Parent = panel_script;
			btn_add_event.Size = new Point(panel_script.Size.x, ITEM_HEIGHT);
			btn_add_event.Position = new Point(0, 0);
			btn_add_event.Text = "Add a new event";
			btn_add_event.Style = "addEventButton";
			btn_add_event.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				// Add new event
				_nodes.Add(new TreeNodeEvent(panel_script.Size.x, this));

				// Refresh position
				refreshScriptEditor();
			};

			refreshScriptEditor();

			// Popup script editor - TO REMOVE
			btn_add_event.Click(42);

			#endregion

			/*
			TextArea ta = new TextArea();
			ta.Parent = this;
			ta.Position = new Point(100, 100);
			ta.Size = new Point(200, 200);
			*/

			#endregion
		}

		public void saveRobot()
		{
			//RobotEditor.Instance.doAction(ActionTypes.SAVE_ROBOT);
			if (RobotEditor.Instance.NamePath == "")
			{
				setFocus(true);
				new MessageBoxSave(this, RobotEditor.Instance.Robot.Name, safeSaveRobot);
			}
			else
				RobotEditor.Instance.doAction(ActionTypes.SAVE_ROBOT);
		}
		public void saveasRobot()
		{
			setFocus(true);
			new MessageBoxSave(this, RobotEditor.Instance.Robot.Name, safeSaveRobot);
		}

		public void safeSaveRobot(string name)
		{
			setFocus(false);
			RobotEditor.Instance.NamePath = name;
			RobotEditor.Instance.doAction(ActionTypes.SAVE_ROBOT);
			label_name.Text = "~ " + RobotEditor.Instance.NamePath + " ~";
		}

		public void loadRobot()
		{
			setFocus(true);
			new MessageBoxLoad(this, safeLoadRobot);
		}

		public void safeLoadRobot(string name)
		{
			setFocus(false);
			RobotEditor.Instance.NamePath = name;
			RobotEditor.Instance.doAction(ActionTypes.LOAD_ROBOT);
			label_name.Text = "~ " + RobotEditor.Instance.NamePath + " ~";
		}

		public void deleteEvent(TreeNodeEvent evt)
		{
			_nodes.Remove(evt);
			refreshScriptEditor();
		}

		public void refreshScriptEditor()
		{
			int y = ITEM_HEIGHT;

			// Remove all
			panel_script.Content.Controls.Clear();
			// Readd button add event
			panel_script.Content.Controls.Add(btn_add_event);

			foreach (TreeNodeEvent evt in _nodes)
			{
				panel_script.Content.Controls.Add(evt);
				evt.Position = new Point(0, y);
				y += evt.Size.y;

				// Loop on all nodes of event
				if (evt.isOpen())
					foreach (TreeNodeAction act in evt._nodes)
					{
						panel_script.Content.Controls.Add(act);
						act.Position = new Point(PADDING_NODE, y);
						y += act.Size.y;
					}
			}

			// Resize script editor container to show them all if we can
			if (y > _ScreenManager.Height / 2)
				y = _ScreenManager.Height / 2;

			panel_script.Size = new Point(panel_script.Size.x, y);
			panel_script.Position = new Point(panel_script.Position.x, _ScreenManager.Height - y);
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

		public void selectHeart()
		{
			RobotEditor.Instance.selectHeart();
		}

		public bool hasFocus()
		{
			if (_has_focus)
				return (true);

			int padding = RobotEditor.Instance.VisibleMenu ? MainMenu.MENU_WIDTH : 0;

			bool menu_has_focus = background.Position.x + padding <= Input.position().X &&
				background.Position.x + background.Size.x + padding >= Input.position().X &&
				background.Position.y <= Input.position().Y &&
				background.Position.y + background.Size.y >= Input.position().Y;

			bool script_has_focus = panel_script.Visible && 
				panel_script.Position.x + padding <= Input.position().X &&
				panel_script.Position.x + panel_script.Size.x + padding >= Input.position().X &&
				panel_script.Position.y <= Input.position().Y &&
				panel_script.Position.y + panel_script.Size.y >= Input.position().Y;

			return (script_has_focus || menu_has_focus);
		}

		public void swap_jointure()
		{
			rb_revolute.Checked = !rb_revolute.Checked;
			rb_prismatic.Checked = !rb_prismatic.Checked;
		}

		public void swap_pieces()
		{
			rb_wheel.Checked = !rb_wheel.Checked;
			rb_rod.Checked = !rb_rod.Checked;
		}

		public void Update(Piece piece, ISpot spot)
		{
			Piece = piece;
			Spot = spot;

			piece_weight.Text = piece.Weight.ToString();
			piece_size.Text = piece.getSize().ToString();
			piece_x.Text = piece.Position.X.ToString();
			piece_y.Text = piece.Position.Y.ToString();

			updateRod(piece);
			updateSpot(spot);
		}

		private void updateSpot(ISpot spot)
		{
			// Update ISpot
			bool visible = (spot != null);

			spot_container.Visible = visible;

			if (visible)
			{
				spot_name.Text = spot.Name;
				spot_force.Text = spot.Force.ToString();

				// Check if Prismatic
				if (spot.GetType() == typeof(PrismaticSpot))
				{
					spot_distance.Text = ((PrismaticSpot)spot).getSize().ToString();
					spot_distance_container.Visible = true;
				}
				else
					spot_distance_container.Visible = false;
			}
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

		private void updateRod(Piece piece)
		{
			// Update Rod if is
			if (piece.GetType() != typeof(Rod))
				piece_rotation_container.Visible = false;
			else
			{
				piece_rotation_container.Visible = true;
				piece_rotation.Text = ((Rod)piece).Rotation.ToString();
			}
		}
	}
}
