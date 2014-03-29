using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using gearit.src.editor.robot;

namespace gearit.src.GUI.RobotEditor
{
    class TreeNodeAction : Panel
    {
        static int PADDING = 16;
        static public int height = 50;
        DropDownList combo = new DropDownList();
        bool _selecting = false;
        ISpot _old_spot;
        Button btn_select = new Button();

        public TreeNodeAction(int width, Action<TreeNodeAction> cbEventRemove)
        {
            int height = MenuRobotEditor.HEIGHT_NODE;

            Size = new Point(width, height);


            int x = PADDING;
            Label lb = new Label();
            lb.Text = "Action";
            lb.Size = new Point(50, height);
            lb.Position = new Point(x, 0);
            lb.Style = "treeNodeText";
            Content.Controls.Add(lb);
            x += lb.Size.x + PADDING;

            // Button add
            btn_select.Text = "Add a piece";
            btn_select.Size = new Point(160, 28);
            btn_select.Position = new Point(x, Size.y / 2 - 14);
            btn_select.MouseClick += delegate(Control sender, MouseEventArgs args)
            {
                btn_select.Text = "Select a piece";
                _selecting = true;
                _old_spot = MenuRobotEditor.Instance.Spot;
                MenuRobotEditor.Instance.selectHeart();
            };
            Content.Controls.Add(btn_select);

            x += btn_select.Size.x + PADDING * 3;


            lb = new Label();
            lb.Text = "Motor";
            lb.Size = new Point(50, height);
            lb.Position = new Point(x, 0);
            lb.Style = "treeNodeText";
            Content.Controls.Add(lb);

            x += lb.Size.x + PADDING;

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
            item.Text = "Forward";
            item.Size = new Squid.Point(100, 35);
            item.Margin = new Margin(0, 0, 0, 4);
            item.Style = "item";
            combo.Items.Add(item);

            item = new ListBoxItem();
            item.Text = "Backward";
            item.Size = new Squid.Point(100, 35);
            item.Margin = new Margin(0, 0, 0, 4);
            item.Style = "item";
            combo.Items.Add(item);

            item = new ListBoxItem();
            item.Text = "Stopped";
            item.Size = new Squid.Point(100, 35);
            item.Margin = new Margin(0, 0, 0, 4);
            item.Style = "item";
            combo.Items.Add(item);

            item.Selected = true;


            // Button remove
            Button btn = new Button();
            btn.Text = "Remove";
            btn.Size = new Point(80, 28);
            btn.Position = new Point(Size.x - 90, Size.y / 2 - 14);
            btn.MouseClick += delegate(Control sender, MouseEventArgs args)
            {
                cbEventRemove(this);
            };
            Content.Controls.Add(btn);
        }

        // Let the menu manage the position
        public void setPosition(Point pos)
        {
            Position = pos;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (_selecting)
            {
                if (_old_spot != MenuRobotEditor.Instance.Spot)
                {
                    btn_select.Text = MenuRobotEditor.Instance.Spot.Name;
                }
            }
        }
    }
}
