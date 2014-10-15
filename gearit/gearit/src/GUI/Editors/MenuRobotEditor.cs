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
using gearit.src.GUI;
using System.Text.RegularExpressions;
using gearit.src.output;
using gearit.src.editor.robot;
using System.IO;
using gearit.src.GUI.Editors;

namespace gearit.src.GUI
{
    /// <summary>
    /// Menu specific for Robot Edition : lua script/properties/...
    /// </summary>
	class MenuRobotEditor : Desktop
	{
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
		private Button rb_revolute = new Button();
		private Label label_title;
		private Label label_name;
		private Label label_weight;
		private Label label_force;

		// Piece & Spot
		private ScreenManager _ScreenManager;
		private ListBox menu_listbox = new ListBox();
		private Control background = new Control();
		private Button rb_rod = new Button();
		private Button rb_wheel = new Button();
		private Button Help_btn;
		private Button Exit_btn;

		private Frame spot_container = new Frame();
		private TextBox spot_name;
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
		private Panel _panelScript = new Panel();
		public Panel PanelScript
		{
			get
			{
				return _panelScript;
			}
			set
			{
				_panelScript = value;
			}
		}

		private List<TreeNodeEvent> _nodes = new List<TreeNodeEvent>();
		public List<TreeNodeEvent> EventNodes
		{
			get
			{
				return _nodes;
			}
			set
			{
				_nodes = value;
			}
		}
		private Button btn_add_event = new Button();

		public static MenuRobotEditor Instance { set; get; }

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
			new ConditionModifierBox();

			#region main

			#region init

			_ScreenManager = ScreenManager;

			padding_x = RobotEditor.Instance.VisibleMenu ? ScreenMainMenu.MENU_WIDTH : 0;

			//ShowCursor = true;
			int padding_y = 152;
			Position = new Squid.Point(padding_x, padding_y);

			// Full width to get the cursor propagation
			Size = new Squid.Point(MENU_WIDTH, ScreenManager.Height - padding_y);

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
			label_title = new Label();
			lb = label_title;
			lb.Text = "ROBOT EDITOR";
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuTitle";
			lb.Parent = this;

			y += lb.Size.y + PADDING;

			label_name = new Label();
			lb = label_name;
			lb.Text = "~ " + RobotEditor.Instance.NamePath + " ~";
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuSubtitle";
			lb.Parent = this;

			y += lb.Size.y + PADDING;

			label_weight = new Label();
			lb = label_weight;
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuSubtitle";
			lb.Parent = this;

			y += lb.Size.y + PADDING;

			label_force = new Label();
			lb = label_force;
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuSubtitle";
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
			btn.Cursor = Cursors.Move;
			btn.Tooltip = "(A)";
			btn.Checked = true;

			btn = rb_rod;
			btn.Text = "Pipe";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(MENU_WIDTH / 2 + 1, y);
			btn.Parent = this;
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
			y += ITEM_HEIGHT + PADDING;

			#endregion

			#region piece

			// Piece section
			lb = new Label();
			lb.Text = "PIECE DATA";
			lb.TextColor = 255;
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuSubtitle";
			lb.Parent = this;

			y += lb.Size.y + PADDING;

			// Piece name
			lb = new Label();
			lb.Text = "Weight";
			lb.Size = new Squid.Point(70, ITEM_HEIGHT);
			lb.Position = new Squid.Point(8, y);
			lb.Style = "itemMenu";
			lb.Parent = this;

			piece_weight = newEditableTextBox();
			piece_weight.Text = "45";
			piece_weight.Size = new Squid.Point(124, ITEM_HEIGHT - PADDING * 3);
			piece_weight.Position = new Squid.Point(lb.Size.x + 8, y + PADDING + 1);
			piece_weight.Style = "textbox";
			piece_weight.Parent = this;
			//piece_weight.Enabled = false;

			piece_weight.Mode = TextBoxMode.Numeric;
			//piece_weight.TextCommit += delegate(object snd, EventArgs e)
			piece_weight.TextChanged += delegate(Control snd)
			{
				float res;
				if (!float.TryParse(((TextBox)snd).Text, out res))
					Piece.Weight = 0;
				else
					Piece.Weight = res;// float.Parse(((TextBox)snd).Text, CultureInfo.InvariantCulture.NumberFormat);
			};


//			spot_force.Parent = spot_container;

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
			#endregion

			#region Spot
			spot_container = new Frame();
			spot_container.Parent = this;
			spot_container.Position = new Squid.Point(0, y);
			spot_container.Size = new Squid.Point(Size.x, Size.y - y);

			y = 0;

			lb = new Label();
			lb.Text = "Spot data";
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuSubTitle";
			lb.Parent = spot_container;

			y += lb.Size.y + PADDING;

			// Spot name
			lb = new Label();
			lb.Text = "Name";
			lb.Size = new Squid.Point(70, ITEM_HEIGHT);
			lb.Position = new Squid.Point(8, y);
			lb.Style = "itemMenu";
			lb.Parent = spot_container;

			spot_name = newEditableTextBox();
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

			spot_force = newEditableTextBox();
			spot_force.Text = "8";
			spot_force.Size = new Squid.Point(124, ITEM_HEIGHT - PADDING * 3);
			spot_force.Position = new Squid.Point(lb.Size.x + 8, y + PADDING + 1);
			spot_force.Style = "textbox";
			spot_force.Parent = spot_container;
			spot_force.Mode = TextBoxMode.Numeric;
			spot_force.TextChanged += delegate(Control snd)
			{
				float res;
				if (!float.TryParse(((TextBox)snd).Text, out res))
					Spot.MaxForce = 0;
				else
					Spot.MaxForce = res;// float.Parse(((TextBox)snd).Text, CultureInfo.InvariantCulture.NumberFormat);
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

			#region Helper
			helper.Text =
			"Help (F1)\n" +
			"----------------------------------------------------------------------------------------\n" +
			"Select piece A.......................................(left click)\n" +
			"Select piece B.......................................(shift+left click)\n" +
			"Swap Selection.....................................(C)\n" +
			"Select spot S.........................................[automatically\n" +
			" selected by selecting both of the pieces it's linked to]\n" +
			"Move piece A........................................(right click)\n" +
			"Delete piece A.......................................(R)\n" +
			"Delete spot S.........................................(shift+R)\n" +
			"Move A to S anchor...............................(shift+right click)\n" +
			"Resize A................................................(S)\n" +
			//"Resize A and rotate B (Rods only).........(shift+S)\n" +
			"Switch piece type pT.............................(A)\n" +
			//"Switch spot type sT...............................(shift+A)\n" +
			"Create a pT linked to A.........................(W)\n" +
			"Create a pT linked to A in a close position....(ctrl+W)\n" +
			"Link A and B.........................................(shift+W)\n" +
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


			helper.Size = new Squid.Point(370, 430);
			helper.Position = new Squid.Point(ScreenManager.Width - helper.Size.x, 0);
			helper.Style = "messagebox";
			helper.Parent = this;
			helper.Enabled = false;
			helper.Visible = false;
			#endregion

			#region action


			/*
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
			*/

			//-----------------------------------------------
			y = ScreenManager.Height - ITEM_HEIGHT;

			Help_btn = new Button();
			btn = Help_btn;
			btn.Text = "Help (F1)";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(3 * MENU_WIDTH / 4 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(MENU_WIDTH / 4 + 1, y);
			btn.Parent = this;
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				swapHelp();
			};

			Exit_btn = new Button();
			btn = Exit_btn;
			btn.Text = "Exit";
			btn.Tooltip = "Escape";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH / 4 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				RobotEditor.Instance.doAction(ActionTypes.EXIT);
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
			btn.Tooltip = "(ctrl+shift+S)";
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				saveasRobot();
			};

			y -= btn.Size.y + PADDING;

			btn = new Button();
			btn.Text = "Edit script";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.Tooltip = "Toggle the script editor";
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				_panelScript.Visible = !_panelScript.Visible;
			};

			y -= btn.Size.y / 2+ PADDING;

			lb = new Label();
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT / 2);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuSubtitle";
			lb.Parent = this;
			y += ITEM_HEIGHT / 2 + PADDING;

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
			_panelScript.Style = "menu";
			_panelScript.Parent = this;

			_panelScript.VScroll.Margin = new Margin(0, 8, 8, 8);
			_panelScript.VScroll.Size = new Squid.Point(14, 10);
			_panelScript.VScroll.Slider.Style = "vscrollTrack";
			_panelScript.VScroll.Slider.Button.Style = "vscrollButton";
			_panelScript.VScroll.ButtonUp.Style = "vscrollUp";
			_panelScript.VScroll.ButtonUp.Size = new Squid.Point(10, 20);
			_panelScript.VScroll.ButtonDown.Style = "vscrollUp";
			_panelScript.VScroll.ButtonDown.Size = new Squid.Point(10, 20);
			_panelScript.VScroll.Slider.Margin = new Margin(0, 2, 0, 2);

			_panelScript.Visible = false;

			_panelScript.Position = new Point(padding_x + MENU_WIDTH, _ScreenManager.Height - ITEM_HEIGHT);
			_panelScript.Size = new Point(_ScreenManager.Width - padding_x - MENU_WIDTH, ITEM_HEIGHT);

			// Add event
			btn_add_event.Parent = _panelScript;
			btn_add_event.Size = new Point(_panelScript.Size.x, ITEM_HEIGHT);
			btn_add_event.Position = new Point(0, 0);
			btn_add_event.Text = "Add a new event";
			btn_add_event.Style = "addEventButton";
			btn_add_event.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				// Add new event
				_nodes.Add(new TreeNodeEvent(_panelScript.Size.x));

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

		private TextBox newEditableTextBox()
		{
			TextBox box = new TextBox();
			box.GotFocus += delegate(Control snd)
			{
				setFocus(true);
			};
			box.LostFocus += delegate(Control snd)
			{
				setFocus(false);
			};
			return box;
		}


		public bool isScriptValide()
		{
			foreach (var event_node in _nodes)
			{
				foreach (var action_node in event_node._nodes)
					if (!action_node.isValide())
					{
						MessageBox.Show(new Point(300, 150), "Script Error", "An action is link to the wrong spot.", MessageBoxButtons.OK, this);
						return (false);
					}

				if (!event_node.isValide())
				{
					MessageBox.Show(new Point(300, 150), "Script Error", "An event exist without binding.", MessageBoxButtons.OK, this);
					return (false);
				}
			}

			return (true);
		}

		//----------------SAVE&LOAD-------------------------------------------
		public bool saveRobot()
		{
			if (RobotEditor.Instance.NamePath == "")
			{
				setFocus(true);
				_messageBoxSave = new MessageBoxSave(this, RobotEditor.Instance.Robot.Name, safeSaveRobot, setFocus, "robot");
				return false;
			}
			else
			{
				RobotEditor.Instance.doAction(ActionTypes.SAVE_ROBOT);
				return true;
			}
		}

		private MessageBoxSave _messageBoxSave = null;
		public void saveasRobot(bool mustExit = false)
		{
			if (mustExit && RobotEditor.Instance.IsEmpty())
				ScreenMainMenu.GoBack = true;
			else
			{
				setFocus(true);
				_messageBoxSave = new MessageBoxSave(this, RobotEditor.Instance.Robot.Name, safeSaveRobot, setFocus, "robot", mustExit);
			}
		}

		public void safeSaveRobot(string name, bool mustExit = false)
		{
			setFocus(false);
			if (name == "")
				return;
			RobotEditor.Instance.NamePath = name;
			if (mustExit)
				ActionSaveRobot.MustExit = true;
			RobotEditor.Instance.doAction(ActionTypes.SAVE_ROBOT);
		}

		private MessageBoxLoad _messageBoxLoad = null;
		public void loadRobot()
		{
			setFocus(true);
			_messageBoxLoad = new MessageBoxLoad(@"data/robot/", ".gir", this, safeLoadRobot, setFocus);
		}

		public void safeLoadRobot(string name)
		{
			setFocus(false);
			RobotEditor.Instance.NamePath = name;
			RobotEditor.Instance.doAction(ActionTypes.LOAD_ROBOT);
		}

		public void updateButtonMapName()
		{
			label_name.Text = "~ " + RobotEditor.Instance.NamePath + " ~";
		}
		//-----------------------------------------------------------

		public void deleteEvent(TreeNodeEvent evt)
		{
			_nodes.Remove(evt);
			refreshScriptEditor();
		}

		public void refreshScriptEditor()
		{
			int y = ITEM_HEIGHT;

			// Remove all
			_panelScript.Content.Controls.Clear();
			// Readd button add event
			_panelScript.Content.Controls.Add(btn_add_event);

			foreach (TreeNodeEvent evt in _nodes)
			{
				_panelScript.Content.Controls.Add(evt);
				evt.Position = new Point(0, y);
				y += evt.Size.y;

				// Loop on all nodes of event
				if (evt.isOpen())
					foreach (TreeNodeAction act in evt._nodes)
					{
						_panelScript.Content.Controls.Add(act);
						act.Position = new Point(PADDING_NODE, y);
						y += act.Size.y;
					}
			}

			// Resize script editor container to show them all if we can
			if (y > _ScreenManager.Height / 2)
				y = _ScreenManager.Height / 2;

			_panelScript.Size = new Point(_panelScript.Size.x, y);
			_panelScript.Position = new Point(_panelScript.Position.x, _ScreenManager.Height - y);
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
			if (ActionLaunch.Running)
				return false;
			if (_has_focus)
				return (true);
			//else return false;
			int padding = RobotEditor.Instance.VisibleMenu ? ScreenMainMenu.MENU_WIDTH : 0;


			bool menu_has_focus = background.Position.x + padding <= Input.position().X &&
				background.Position.x + background.Size.x + padding >= Input.position().X &&
				background.Position.y <= Input.position().Y &&
				background.Position.y + background.Size.y >= Input.position().Y;

			bool script_has_focus = _panelScript.Visible && 
				_panelScript.Position.x + padding <= Input.position().X &&
				_panelScript.Position.x + _panelScript.Size.x + padding >= Input.position().X &&
				_panelScript.Position.y <= Input.position().Y &&
				_panelScript.Position.y + _panelScript.Size.y >= Input.position().Y;

			return (script_has_focus || menu_has_focus);
		}

		public void swap_jointure()
		{
			rb_revolute.Checked = !rb_revolute.Checked;
		}

		public void swap_pieces()
		{
			rb_wheel.Checked = !rb_wheel.Checked;
			rb_rod.Checked = !rb_rod.Checked;
		}

		public void Update(Piece piece, ISpot spot)
		{
			if (Input.Exit)
			{
				if (_messageBoxLoad != null)
				{
					_messageBoxLoad.cancel();
					_messageBoxLoad = null;
				}
				if (_messageBoxSave != null)
				{
					_messageBoxSave.cancel();
					_messageBoxSave = null;
				}
			}

			Piece = piece;
			Spot = spot;

			if (!_has_focus)
			{
				piece_weight.Text = piece.Weight.ToString();
			}

			label_weight.Text = "WEIGHT " + RobotEditor.Instance.Robot.Weight.ToString();
			label_force.Text = "FORCE " + RobotEditor.Instance.Robot.MaxForce.ToString();
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
				if (!_has_focus)
				{
					spot_name.Text = spot.Name;
					spot_force.Text = spot.MaxForce.ToString();
				}

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
