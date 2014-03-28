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
        MenuRobotEditor _robot_editor;
        bool _selecting = false;
        ISpot _old_spot;
        Button btn_select = new Button();

        public TreeNodeAction(int width, Action<TreeNodeAction> cbEventRemove, MenuRobotEditor robot_editor)
        {
            _robot_editor = robot_editor;
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
                _old_spot = _robot_editor._spot;
            };
            Content.Controls.Add(btn_select);


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
                if (_old_spot != _robot_editor._spot)
                {
                    btn_select.Text = _robot_editor._spot.Name;
                }
            }
        }
    }
}
