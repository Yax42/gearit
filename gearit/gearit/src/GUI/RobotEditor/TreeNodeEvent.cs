using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using gearit.src.editor.robot;
using gearit.src.utility;

namespace gearit.src.GUI
{
    class TreeNodeEvent : Panel
    {
        static int PADDING = 16;
        DropDownList combo = new DropDownList();

        MenuRobotEditor _robot_editor;
        Label lb_status = new Label();
        MessageBox _msgbox = null;
        Button btn_add_key = new Button();

        // Let menu manage nodes
        public List<TreeNodeAction> _nodes = new List<TreeNodeAction>();
        private bool _open = false;

		string key_binded = "";

        public TreeNodeEvent(int width, MenuRobotEditor robot_editor)
        {
            _robot_editor = robot_editor;

            int height = MenuRobotEditor.HEIGHT_NODE;

            Size = new Point(width, height);
            Style = "eventPanel";

            int x = 0;

            lb_status.Text = "+";
            lb_status.Size = new Point(50, height);
            lb_status.Position = new Point(x, 0);
            lb_status.Style = "treeNodeText";
            lb_status.MouseClick += delegate(Control sender, MouseEventArgs args)
            {
                toggleVisibility();
            };
            Content.Controls.Add(lb_status);


            x += lb_status.Size.x + PADDING;

            Label lb = new Label();
            lb.Text = "Event";
            lb.Size = new Point(50, height);
            lb.Position = new Point(x, 0);
            lb.Style = "treeNodeText";
            Content.Controls.Add(lb);
            x += lb.Size.x + PADDING;
            lb.MouseClick += delegate(Control sender, MouseEventArgs args)
            {
                toggleVisibility();
            };

            // Event type choice
            Content.Controls.Add(combo);
            combo.Size = new Squid.Point(158, height / 2);
            combo.Position = new Squid.Point(x, height / 2 - combo.Size.y / 2);
            combo.Label.Style = "comboLabel";
            combo.Button.Style = "comboButton";
            combo.Listbox.Margin = new Margin(0, 0, 0, 0);
            combo.Listbox.Style = "frame";
            combo.Listbox.ClipFrame.Margin = new Margin(8, 8, 8, 8);
            combo.Listbox.Scrollbar.Margin = new Margin(0, 4, 4, 4);
            combo.Listbox.Scrollbar.Size = new Squid.Point(14, 10);
            combo.Listbox.Scrollbar.ButtonUp.Style = "vscrollUp";
            combo.Listbox.Scrollbar.ButtonUp.Size = new Squid.Point(10, 20);
            combo.Listbox.Scrollbar.ButtonDown.Style = "vscrollUp";
            combo.Listbox.Scrollbar.ButtonDown.Size = new Squid.Point(10, 20);
            combo.Listbox.Scrollbar.Slider.Margin = new Margin(0, 2, 0, 2);
            combo.Listbox.Scrollbar.Slider.Style = "vscrollTrack";
            combo.Listbox.Scrollbar.Slider.Button.Style = "vscrollButton";

            combo.Opened += delegate(Control sender, SquidEventArgs args)
            {
                combo.Dropdown.Position = new Point(combo.Dropdown.Position.x, combo.Dropdown.Position.y - combo.Dropdown.Size.y - combo.Size.y);
            };

            // Button pressed
            ListBoxItem item = new ListBoxItem();
            item.Text = "Button Pressed";
            item.Size = new Squid.Point(100, 35);
            item.Margin = new Margin(0, 0, 0, 4);
            item.Style = "item";
            combo.Items.Add(item);

            // Autopress button pressed
            item.Selected = true;

            x += combo.Size.x + PADDING;

            // Button key press
            btn_add_key.Text = "Add a key";
            btn_add_key.Size = new Point(120, 28);
            btn_add_key.Position = new Point(x, Size.y / 2 - 14);
            btn_add_key.MouseClick += delegate(Control sender, MouseEventArgs args)
            {
                _robot_editor.setFocus(true);
                _msgbox = MessageBox.Show(new Point(300, 100), "Add Event", "Press any key", MessageBoxButtons.None, _robot_editor);
            };
            Content.Controls.Add(btn_add_key);


            // Button remove
            Button btn = new Button();
            btn.Text = "Remove";
            btn.Size = new Point(80, 28);
            btn.Position = new Point(Size.x - 90, Size.y / 2 - 14);
            btn.MouseClick += delegate(Control sender, MouseEventArgs args)
            {
                _robot_editor.deleteEvent(this);
            };
            Content.Controls.Add(btn);

            // Button add action
            btn = new Button();
            btn.Text = "Add Action";
            btn.Size = new Point(120, 28);
            btn.Position = new Point(Size.x - 220, Size.y / 2 - 14);
            btn.MouseClick += delegate(Control sender, MouseEventArgs args)
            {
				addAction();
            };
            Content.Controls.Add(btn);

            MouseClick += delegate(Control sender, MouseEventArgs args)
            {
                toggleVisibility();
            };
        }

		public void setKey(string key)
		{
			key_binded = key;
			btn_add_key.Text = "Key [" + key_binded + "]";
		}

		public TreeNodeAction addAction()
		{
			// Force open
			Open();

			TreeNodeAction action = new TreeNodeAction(Size.x - MenuRobotEditor.HEIGHT_NODE, removeNode);
			_nodes.Add(action);
			_robot_editor.refreshScriptEditor();
			return (action);
		}

		public string toLua()
		{
			return ("if Input:pressed(K_" + key_binded + ") then");
		}

        public void toggleVisibility()
        {
            if (_open)
                Close();
            else
                Open();

            _robot_editor.refreshScriptEditor();
        }

		public bool isValide()
		{
			
			return (!key_binded.Equals("")) ;
		}

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (_msgbox == null)
                return;

            List<Microsoft.Xna.Framework.Input.Keys> inputs = Input.getJustReleased();
            if (inputs.Count != 0)
            {
				setKey(inputs.ElementAt(0).ToString());

                _msgbox.Close();
                _robot_editor.setFocus(false);
                _msgbox = null;
            }
        }

        public void addAKey(Microsoft.Xna.Framework.Input.Keys key)
        {
            
        }

        public bool isOpen()
        {
            return _open;
        }

        public void Close()
        {
            _open = false;
            lb_status.Text = "+";
        }

        public void Open()
        {
            _open = true;
            lb_status.Text = "-";
        }

        public void removeNode(TreeNodeAction node)
        {
            _nodes.Remove(node);
            _robot_editor.refreshScriptEditor();
        }

    }
}
