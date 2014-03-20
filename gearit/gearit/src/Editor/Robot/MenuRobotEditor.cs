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

		enum FJointure
		{
			Prismatic,
			Revolute
		}

		// Editor
		RobotEditor _robot_editor;
		Piece _piece;
		ISpot _spot;

		// Propertie
		static public int MENU_WIDTH = 200;
		static public int TOOLS_HEIGHT = 32;
		static public int PIECE_HEIGHT = 400;
		static public int SPOT_HEIGHT = 400;
		static public int ITEM_HEIGHT = 42;
		static public int PADDING = 4;

		//// Gui
		private bool _has_focus = false;

		// Tools
		private ListBox lb_jointure;
		private FPiece flag_piece;
		private FJointure flag_jointure;
		private Button rb_revolute = new Button();
		private Button rb_prismatic = new Button();

		// Piece & Spot
		private ScreenManager _ScreenManager;
		private ListBox menu_listbox = new ListBox();
		private Control background = new Control();

		private Frame spot_container = new Frame();
		private TextBox spot_name = new TextBox();
		private TextBox spot_force = new TextBox();
		private Frame spot_distance_container = new Frame();
		private TextBox spot_distance = new TextBox();

		private Frame piece_rotation_conainer = new Frame();
		private TextBox piece_rotation = new TextBox();

		private TextBox piece_weight = new TextBox();
		private TextBox piece_size = new TextBox();
		private TextBox piece_x = new TextBox();
		private TextBox piece_y = new TextBox();


		public MenuRobotEditor(ScreenManager ScreenManager, RobotEditor robot_editor)
		{
		   
			#region main

			#region init

			_ScreenManager = ScreenManager;
			_robot_editor = robot_editor;

			ShowCursor = true;
			Position = new Squid.Point(MainMenu.MENU_WIDTH, 0);

			// Full width to get the cursor propagation
			Size = new Squid.Point(ScreenManager.Width - MainMenu.MENU_WIDTH, ScreenManager.Height);

			int y = 0;

			#endregion

			// Background
			background.Parent = this;
			background.Style = "menu";
			background.Position = new Point(0, y);
			background.Size = new Point(MENU_WIDTH, _ScreenManager.Height);

			// Drop callback
			AllowDrop = true;
			DragDrop += dropPiece;

			#region tools

			//// Tools section
			// Auto select menu tools
			flag_piece = FPiece.None;
			flag_jointure = FJointure.Prismatic;
			rb_prismatic.Checked = true;

			// Title
			Label lb = new Label();
			lb.Text = "Pieces";
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuTitle";
			lb.Parent = this;

			y += lb.Size.y + PADDING;

			#region piecedrop

			//// Circle and Pipe
			// Circle
			Button btn = new Button();
			btn.Text = "Circle";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.Tag = FPiece.Circle;
			btn.MouseDrag += dragPiece;
			btn.Cursor = Cursors.Move;
			btn.Tooltip = "Drag to the specified location";

			y += btn.Size.y + 2;

			btn = new Button();
			btn.Text = "Pipe";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.Tag = FPiece.Pipe;
			btn.MouseDrag += dragPiece;
			btn.Cursor = Cursors.Move;
			btn.Tooltip = "Drag to the specified location";

			y += btn.Size.y + PADDING; 

			#endregion

			#region jointuretype

			// Title
			lb = new Label();
			lb.Text = "Jointure";
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuTitle";
			lb.Parent = this;

			y += lb.Size.y + PADDING;

			// Piece jointure type
			rb_revolute.Text = "Revolute";
			rb_revolute.Style = "itemMenuButton";
			rb_revolute.Size = new Squid.Point(MENU_WIDTH / 2, ITEM_HEIGHT);
			rb_revolute.Position = new Squid.Point(0, y);
			rb_revolute.Parent = this;
			rb_revolute.Tooltip = "Circular motor";

			rb_prismatic.Text = "Prismatic";
			rb_prismatic.Style = "itemMenuButton";
			rb_prismatic.Size = new Squid.Point(MENU_WIDTH / 2, ITEM_HEIGHT);
			rb_prismatic.Position = new Squid.Point(MENU_WIDTH / 2, y);
			rb_prismatic.Parent = this;
			rb_prismatic.Tooltip = "Piston motor";

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

			y += ITEM_HEIGHT + PADDING * 4;

			#endregion

			#endregion

			#region piece

			// Piece section
			lb = new Label();
			lb.Text = "Piece information";
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuTitle";
			lb.Parent = this;

			y += lb.Size.y;

			// Piece name
			lb = new Label();
			lb.Text = "Weight";
			lb.Size = new Squid.Point(50, ITEM_HEIGHT);
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
			lb.Size = new Squid.Point(50, ITEM_HEIGHT);
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
			piece_x.Size = new Squid.Point(56, ITEM_HEIGHT - PADDING * 3);
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
			piece_y.Size = new Squid.Point(56, ITEM_HEIGHT - PADDING * 3);
			piece_y.Position = new Squid.Point(lb.Position.x + lb.Size.x + PADDING, y + PADDING + 1);
			piece_y.Style = "textbox";
			piece_y.Enabled = false;
			piece_y.Parent = this;

			y += ITEM_HEIGHT;

			// Piece rotation
			piece_rotation_conainer.Size = new Squid.Point(Size.x, ITEM_HEIGHT);
			piece_rotation_conainer.Position = new Squid.Point(0, y);
			piece_rotation_conainer.Parent = this;

			lb = new Label();
			lb.Text = "Rotation";
			lb.Size = new Squid.Point(50, ITEM_HEIGHT);
			lb.Position = new Squid.Point(8, 0);
			lb.Style = "itemMenu";
			lb.Parent = piece_rotation_conainer;

			piece_rotation.Text = "8.23";
			piece_rotation.Size = new Squid.Point(124, ITEM_HEIGHT - PADDING * 3);
			piece_rotation.Position = new Squid.Point(lb.Size.x + 8, PADDING + 1);
			piece_rotation.Style = "textbox";
			piece_rotation.Parent = piece_rotation_conainer;
			piece_rotation.Enabled = false;

			y += lb.Size.y + PADDING * 2;

			// Spot section
			spot_container = new Frame();
			spot_container.Parent = this;
			spot_container.Position = new Squid.Point(0, y);
			spot_container.Size = new Squid.Point(Size.x, Size.y - y);

			y = 0;

			lb = new Label();
			lb.Text = "Spot information";
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuTitle";
			lb.Parent = spot_container;

			y += lb.Size.y + PADDING;

			// Spot name
			lb = new Label();
			lb.Text = "Name";
			lb.Size = new Squid.Point(50, ITEM_HEIGHT);
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
				_spot.Name = ((TextBox)snd).Text;
			};

			y += ITEM_HEIGHT;

			// Spot size
			lb = new Label();
			lb.Text = "Force";
			lb.Size = new Squid.Point(50, ITEM_HEIGHT);
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
					_spot.Force = 0;
				else
					_spot.Force = float.Parse(((TextBox)snd).Text, CultureInfo.InvariantCulture.NumberFormat);
			};

			y += ITEM_HEIGHT;

			// Spot distance
			spot_distance_container.Size = new Squid.Point(Size.x, ITEM_HEIGHT);
			spot_distance_container.Position = new Squid.Point(0, y);
			spot_distance_container.Parent = spot_container;

			lb = new Label();
			lb.Text = "Distance";
			lb.Size = new Squid.Point(50, ITEM_HEIGHT);
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

			#region action

			y = ScreenManager.Height - PADDING - ITEM_HEIGHT;

			// Link
			btn = new Button();
			btn.Text = "Link";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.Tooltip = "Link two pieces together with specified jointure";
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				if (flag_jointure == FJointure.Prismatic)
					_robot_editor.doAction(ActionTypes.PRIS_LINK);
				else
					_robot_editor.doAction(ActionTypes.REV_LINK);
			};

			y -= btn.Size.y + PADDING;

			// Remove
			btn = new Button();
			btn.Text = "Delete";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.Tooltip = "Remove selected piece";
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				_robot_editor.doAction(ActionTypes.DELETE_PIECE);
			};

			y -= btn.Size.y + PADDING;

			// Title
			lb = new Label();
			lb.Text = "Actions";
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuTitle";
			lb.Parent = this;

			#endregion

			#endregion
		}

		void dragPiece(Control sender, MouseEventArgs e)
		{
			ImageControl img = new ImageControl();

			if (sender.Tag.Equals(FPiece.Circle))
				img.Texture = "RobotEditor/revolute.png";
			else
				img.Texture = "RobotEditor/prismatic.png";

			img.Size = new Squid.Point(32, 32);
			img.Position = new Point((int) Input.position().X - MainMenu.MENU_WIDTH - 16, (int) Input.position().Y - 16);
			img.Style = "itemMainMenu";
			img.Tag = sender.Tag;

			DoDragDrop(img);
		}

		void dropPiece(Control sender, DragDropEventArgs e)
		{
			// Change set
			 if (e.DraggedControl.Tag.Equals(FPiece.Pipe))
				 _robot_editor.doAction(ActionTypes.CHOOSE_SET);

			 if (flag_jointure == FJointure.Prismatic)
				_robot_editor.doAction(ActionTypes.PRIS_SPOT);
			else
				_robot_editor.doAction(ActionTypes.REV_SPOT);

			// Restore set
			if (e.DraggedControl.Tag.Equals(FPiece.Pipe))
				_robot_editor.doAction(ActionTypes.CHOOSE_SET);
		}

		public bool hasFocus()
		{
			return (background.Position.x + MainMenu.MENU_WIDTH <= Input.position().X &&
				background.Position.x + background.Size.x + MainMenu.MENU_WIDTH >= Input.position().X &&
				background.Position.y <= Input.position().Y &&
				background.Position.y + background.Size.y >= Input.position().Y);
		}

		public void swap_jointure()
		{
			rb_revolute.Checked = !rb_revolute.Checked;
			rb_prismatic.Checked = !rb_prismatic.Checked;

			flag_jointure = (flag_jointure == FJointure.Prismatic ? FJointure.Revolute : FJointure.Prismatic);
		}

		public void Update(Piece piece, ISpot spot)
		{
			_piece = piece;
			_spot = spot;

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

		private void updateRod(Piece piece)
		{
			// Update Rod if is
			if (piece.GetType() != typeof(Rod))
				piece_rotation_conainer.Visible = false;
			else
			{
				piece_rotation_conainer.Visible = true;
				piece_rotation.Text = ((Rod)piece).Rotation.ToString();
			}
		}
	}
}
