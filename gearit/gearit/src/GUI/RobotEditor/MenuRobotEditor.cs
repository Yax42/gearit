﻿using System;
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
		static public int MENU_WIDTH = 220;
		static public int TOOLS_HEIGHT = 32;
		static public int PIECE_HEIGHT = 400;
		static public int SPOT_HEIGHT = 400;
		static public int ITEM_HEIGHT = 42;
		static public int PADDING = 4;
        static public int PADDING_NODE = 24;
        static public int HEIGHT_NODE = 50;

		//// Gui
		private bool _has_focus = false;
        private int padding_x;

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

		private Frame piece_rotation_container = new Frame();
		private TextBox piece_rotation = new TextBox();

		private TextBox piece_weight = new TextBox();
		private TextBox piece_size = new TextBox();
		private TextBox piece_x = new TextBox();
		private TextBox piece_y = new TextBox();

        // Script editor
        private Panel panel_script = new Panel();
        private List<TreeNodeEvent> _nodes = new List<TreeNodeEvent>();
        private Button btn_add_event = new Button();

		public MenuRobotEditor(ScreenManager ScreenManager, RobotEditor robot_editor)
		{

            //piece_rotation_conainer.Enabled = false;
		   
			#region main

			#region init

			_ScreenManager = ScreenManager;
			_robot_editor = robot_editor;

            padding_x = _robot_editor.VisibleMenu ? MainMenu.MENU_WIDTH : 0;

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
			btn.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.Tag = FPiece.Circle;
			btn.MouseDrag += dragPiece;
			btn.Cursor = Cursors.Move;
			btn.Tooltip = "Drag to the specified location";

			btn = new Button();
			btn.Text = "Pipe";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
			btn.Position = new Squid.Point(MENU_WIDTH / 2 + 1, y);
			btn.Parent = this;
			btn.Tag = FPiece.Pipe;
			btn.MouseDrag += dragPiece;
			btn.Cursor = Cursors.Move;
			btn.Tooltip = "Drag to the specified location";

			y += btn.Size.y + 2; 

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

			y += ITEM_HEIGHT;

			// Piece rotation
			piece_rotation_container.Size = new Squid.Point(Size.x, ITEM_HEIGHT);
			piece_rotation_container.Position = new Squid.Point(0, y);
			piece_rotation_container.Parent = this;

			lb = new Label();
			lb.Text = "Rotation";
			lb.Size = new Squid.Point(70, ITEM_HEIGHT);
			lb.Position = new Squid.Point(8, 0);
			lb.Style = "itemMenu";
			lb.Parent = piece_rotation_container;

			piece_rotation.Text = "8.23";
			piece_rotation.Size = new Squid.Point(124, ITEM_HEIGHT - PADDING * 3);
			piece_rotation.Position = new Squid.Point(lb.Size.x + 8, PADDING + 1);
			piece_rotation.Style = "textbox";
			piece_rotation.Parent = piece_rotation_container;
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
				_spot.Name = ((TextBox)snd).Text;
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

			#region action

			y = ScreenManager.Height - ITEM_HEIGHT;

			// Link
			btn = new Button();
			btn.Text = "Link";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
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

			// Remove
			btn = new Button();
			btn.Text = "Delete";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
            btn.Position = new Squid.Point(MENU_WIDTH / 2 + 1, y);
			btn.Parent = this;
			btn.Tooltip = "Remove selected piece";
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				_robot_editor.doAction(ActionTypes.DELETE_PIECE);
			};

			y -= btn.Size.y + 2;

            // Load
            btn = new Button();
            btn.Text = "Load";
            btn.Style = "itemMenuButton";
            btn.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
            btn.Position = new Squid.Point(0, y);
            btn.Parent = this;
            btn.Tooltip = "Load a robot with his script";
            btn.MouseClick += delegate(Control snd, MouseEventArgs e)
            {
                // TODO
            };

            btn = new Button();
            btn.Text = "Save";
            btn.Style = "itemMenuButton";
            btn.Size = new Squid.Point(MENU_WIDTH / 2 - 1, ITEM_HEIGHT);
            btn.Position = new Squid.Point(MENU_WIDTH / 2 + 1, y);
            btn.Parent = this;
            btn.Tooltip = "Save the robot and his script";
            btn.MouseClick += delegate(Control snd, MouseEventArgs e)
            {
                // TODO
            };

            y -= btn.Size.y + 2;

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

            y -= btn.Size.y + PADDING;

			// Title
			lb = new Label();
			lb.Text = "Actions";
			lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			lb.Position = new Squid.Point(0, y);
			lb.Style = "itemMenuTitle";
			lb.Parent = this;

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

            #endregion
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

                Console.WriteLine("dsad");

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

			if (sender.Tag.Equals(FPiece.Circle))
				img.Texture = "RobotEditor/revolute.png";
			else
				img.Texture = "RobotEditor/prismatic.png";

			img.Size = new Squid.Point(32, 32);
			img.Position = new Point((int) Input.position().X - 16 - padding_x, (int) Input.position().Y - 16);
			img.Style = "itemMainMenu";
			img.Tag = sender.Tag;

			DoDragDrop(img);
		}

		void dropPiece(Control sender, DragDropEventArgs e)
		{
			// Change set
			 if (e.DraggedControl.Tag.Equals(FPiece.Pipe) && ActionChooseSet.value)
				 _robot_editor.doAction(ActionTypes.CHOOSE_SET);

			 if (flag_jointure == FJointure.Prismatic)
				_robot_editor.doAction(ActionTypes.PRIS_SPOT);
			else
				_robot_editor.doAction(ActionTypes.REV_SPOT);

			// Restore set
             if (e.DraggedControl.Tag.Equals(FPiece.Pipe) && ActionChooseSet.value)
				_robot_editor.doAction(ActionTypes.CHOOSE_SET);
		}

		public bool hasFocus()
		{
            if (_has_focus)
                return (true);

            int padding = _robot_editor.VisibleMenu ? MainMenu.MENU_WIDTH : 0;

            bool menu_has_focus = background.Position.x + padding <= Input.position().X &&
                background.Position.x + background.Size.x + padding >= Input.position().X &&
				background.Position.y <= Input.position().Y &&
				background.Position.y + background.Size.y >= Input.position().Y;

            bool script_has_focus = panel_script.Visible && 
                panel_script.Position.x + padding <= Input.position().X &&
                panel_script.Position.x + panel_script.Size.x + padding >= Input.position().X &&
                panel_script.Position.y <= Input.position().Y &&
                panel_script.Position.y + panel_script.Size.y >= Input.position().Y;

            return (script_has_focus);
		}

		public void swap_jointure()
		{
			rb_revolute.Checked = !rb_revolute.Checked;
			rb_prismatic.Checked = !rb_prismatic.Checked;

			flag_jointure = (flag_jointure == FJointure.Prismatic ? FJointure.Revolute : FJointure.Prismatic);
		}

		public void Update(Piece piece, ISpot spot)
		{
            if (piece == _piece && spot == _spot)
                return;

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
