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

        // Gui
        private ScreenManager _ScreenManager;
        private ListBox menu_listbox;

        public MenuRobotEditor(ScreenManager ScreenManager)
        {
            #region main

            _ScreenManager = ScreenManager;
            Opacity = 0.88f;

            ShowCursor = true;
            Position = new Squid.Point(MainMenu.MENU_WIDTH, 0);
            Size = new Squid.Point(MENU_WIDTH, ScreenManager.Height);

            // MainMenu
            menu_listbox = new ListBox();
            menu_listbox.Position = new Point(0, 0);
            menu_listbox.Size = new Point(MENU_WIDTH, _ScreenManager.Height);
            menu_listbox.Scrollbar.Size = new Squid.Point(14, 10);
            menu_listbox.Scrollbar.Slider.Style = "vscrollTrack";
            menu_listbox.Scrollbar.Slider.Button.Style = "vscrollButton";
            menu_listbox.Scrollbar.ButtonUp.Style = "vscrollUp";
            menu_listbox.Scrollbar.ButtonUp.Size = new Squid.Point(10, 20);
            menu_listbox.Scrollbar.ButtonDown.Style = "vscrollUp";
            menu_listbox.Scrollbar.ButtonDown.Size = new Squid.Point(10, 20);
            menu_listbox.Scrollbar.Slider.Margin = new Margin(0, 2, 0, 2);
            menu_listbox.Multiselect = false;
            menu_listbox.Parent = this;
            menu_listbox.Style = "menu";

            ListBoxItem item = new ListBoxItem();
            item.Text = "dsad";
            item.BBCodeEnabled = true;
            item.Size = new Squid.Point(MENU_WIDTH, 42);
            item.Style = "itemMenu";
            menu_listbox.Items.Add(item);

            #endregion
        }
    }
}
