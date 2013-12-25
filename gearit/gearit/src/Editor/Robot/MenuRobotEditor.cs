using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using gearit.xna;
using GUI;

namespace gearit.src.editor.robot
{
    class MenuRobotEditor : Desktop
    {
        // Propertie
        static public int MENU_WIDTH = 200;
        static public int TOOLS_HEIGHT = 32;
        static public int PIECE_HEIGHT = 400;
        static public int SPOT_HEIGHT = 400;
        static public int ITEM_HEIGHT = 42;
        static public int PADDING = 4;

        // Gui
        private ScreenManager _ScreenManager;
        private ListBox menu_listbox;
        private Control background;

        public MenuRobotEditor(ScreenManager ScreenManager)
        {
            #region main

            _ScreenManager = ScreenManager;

            ShowCursor = true;
            Position = new Squid.Point(MainMenu.MENU_WIDTH, 0);
            Size = new Squid.Point(MENU_WIDTH, ScreenManager.Height);

            int y = 0;

            // Background
            background = new Control();
            background.Parent = this;
            background.Style = "menu";
            background.Position = new Point(0, y);
            background.Size = new Point(MENU_WIDTH, _ScreenManager.Height);

            // Tools section
            ImageControl img = new ImageControl();
            img.Texture = "RobotEditor/revolute.png";
            img.Size = new Squid.Point(32, TOOLS_HEIGHT);
            img.Position = new Squid.Point(0, y);
            img.Style = "itemMainMenu";
            img.Parent = this;

            y += TOOLS_HEIGHT;

            // Piece section
            Label lb = new Label();
            lb.Text = "Piece";
            lb.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
            lb.Position = new Squid.Point(0, y);
            lb.Style = "itemMenuTitle";
            lb.Parent = this;

            y += lb.Size.y;

            // Piece name
            lb = new Label();
            lb.Text = "Name";
            lb.Size = new Squid.Point(50, ITEM_HEIGHT);
            lb.Position = new Squid.Point(8, y);
            lb.Style = "itemMenu";
            lb.Parent = this;

            TextBox tb = new TextBox();
            tb.Text = "pieceNumber";
            tb.Size = new Squid.Point(124, ITEM_HEIGHT - PADDING * 3);
            tb.Position = new Squid.Point(lb.Size.x + 8, y + PADDING + 1);
            tb.Style = "textbox";
            tb.Parent = this;

            y += ITEM_HEIGHT;

            // Piece size
            lb = new Label();
            lb.Text = "Size";
            lb.Size = new Squid.Point(50, ITEM_HEIGHT);
            lb.Position = new Squid.Point(8, y);
            lb.Style = "itemMenu";
            lb.Parent = this;

            tb = new TextBox();
            tb.Text = "8";
            tb.Size = new Squid.Point(124, ITEM_HEIGHT - PADDING * 3);
            tb.Position = new Squid.Point(lb.Size.x + 8, y + PADDING + 1);
            tb.Style = "textbox";
            tb.Parent = this;

            y += ITEM_HEIGHT;
            
            // Piece position
            lb = new Label();
            lb.Text = "X";
            lb.Size = new Squid.Point(20, ITEM_HEIGHT);
            lb.Position = new Squid.Point(8, y);
            lb.Style = "itemMenu";
            lb.Parent = this;

            tb = new TextBox();
            tb.Text = "15";
            tb.Size = new Squid.Point(56, ITEM_HEIGHT - PADDING * 3);
            tb.Position = new Squid.Point(lb.Position.x + lb.Size.x + PADDING, y + PADDING + 1);
            tb.Style = "textbox";
            tb.Enabled = false;
            tb.Parent = this;

            lb = new Label();
            lb.Text = "Y";
            lb.Size = new Squid.Point(20, ITEM_HEIGHT);
            lb.Position = new Squid.Point(tb.Position.x + tb.Size.x + PADDING * 3, y);
            lb.Style = "itemMenu";
            lb.Parent = this;

            tb = new TextBox();
            tb.Text = "81";
            tb.Size = new Squid.Point(56, ITEM_HEIGHT - PADDING * 3);
            tb.Position = new Squid.Point(lb.Position.x + lb.Size.x + PADDING, y + PADDING + 1);
            tb.Style = "textbox";
            tb.Enabled = false;
            tb.Parent = this;

            // Spot section

            #endregion
        }

        public void Update(Piece piece, ISpot spot)
        {
        }
    }
}
