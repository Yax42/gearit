using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using gearit.src.utility;
using gearit.src.utility.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using gearit.src.editor.robot;
using FarseerPhysics.Factories;
using gearit.src.map;
using gearit.src.editor.map.action;
using gearit.src.editor.map;
using FarseerPhysics.DebugViews;
using Squid;
using GUI;

namespace gearit.src.gui
{

    class MenuOptions : GameScreen, IDemoScreen
    {
        static private Desktop _desktop;
        static private ScreenManager _screen;
        static private Control background = new Control();
        static private Button btn_save;
        static public int Width = 21;
        static public int Height = 20;
        private ListBox lb_resolution;
        private ListBox lb_antialiasing;
        private CheckBox cb_fullScreen;
        private CheckBox cb_blockFps;
        private CheckBox cb_showFps;
        private FrameRateCounter frc = null;
        DropDownList combo = new DropDownList();

		public MenuOptions() : base(false)
		{
		}

        public override void LoadContent()
        {
            base.LoadContent();
            if (frc == null)
            frc = new FrameRateCounter(ScreenManager);
            VisibleMenu = true;
            _desktop = new Desktop();
            _desktop.Position = new Squid.Point(MainMenu.MENU_WIDTH, 0);
            _desktop.Size = new Squid.Point(ScreenManager.Width - MainMenu.MENU_WIDTH, ScreenManager.Height);

           
            combo.Size = new Squid.Point(158, 100 / 2);
            combo.Position = new Squid.Point(200, 100 / 2 - combo.Size.y / 2);
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
            combo.Parent = _desktop;
            combo.Opened += delegate(Control sender, SquidEventArgs args)
            {
                combo.Dropdown.Position = new Squid.Point(combo.Dropdown.Position.x - MainMenu.MENU_WIDTH, combo.Dropdown.Position.y);
            };

            Console.WriteLine("One one one");
            // Button pressed
            ListBoxItem item = new ListBoxItem();
            item.Text = "Button Pressed";
            item.Size = new Squid.Point(100, 35);
            item.Margin = new Margin(0, 0, 0, 4);
            item.Style = "item";
            combo.Items.Add(item);

            // Autopress button pressed
            item.Selected = true;

            cb_fullScreen = new CheckBox();
            cb_fullScreen.Text = "Fullscreen";
            cb_fullScreen.Parent = _desktop;
            cb_fullScreen.Size = new Squid.Point(157, 26);
            cb_fullScreen.Position = new Squid.Point(180, 220);
            cb_fullScreen.Button.Style = "checkBox";
            cb_fullScreen.Button.Size = new Squid.Point(26, 26);
            cb_fullScreen.Button.Cursor = Cursors.Link;
            if (ScreenManager.IsFullScreen == true)
            {
                cb_fullScreen.Checked = true;
            }
            else
                cb_fullScreen.Checked = false;
            //System.Diagnostics.Debug.WriteLine("isfullscreen =" + ScreenManager.IsFullScreen);


            cb_blockFps = new CheckBox();
            cb_blockFps.Text = "Unlock fps";
            cb_blockFps.Parent = _desktop;
            cb_blockFps.Size = new Squid.Point(157, 26);
            cb_blockFps.Position = new Squid.Point(180, 246);
            cb_blockFps.Button.Style = "checkBox";
            cb_blockFps.Button.Size = new Squid.Point(26, 26);
            cb_blockFps.Button.Cursor = Cursors.Link;
            if (ScreenManager.fpsIsLocked == true)
                cb_blockFps.Checked = false;
            else
                cb_blockFps.Checked = true;

            cb_showFps = new CheckBox();
            cb_showFps.Text = "Show fps";
            cb_showFps.Parent = _desktop;
            cb_showFps.Size = new Squid.Point(157, 26);
            cb_showFps.Position = new Squid.Point(180, 272);
            cb_showFps.Button.Style = "checkBox";
            cb_showFps.Button.Size = new Squid.Point(26, 26);
            cb_showFps.Button.Cursor = Cursors.Link;
            if (ScreenManager.Game.Components.Contains(frc))
                cb_showFps.Checked = true;
            else
                cb_showFps.Checked = false;

            /*background.Parent = cb_fullScreen;
            background.Style = "menu";
            background.Dock = DockStyle.Fill;*/
            btn_save = new Button();
            btn_save.Parent = _desktop;
            btn_save.Text = "Apply";
            btn_save.Position = new Squid.Point(200, 0);

            btn_save.MouseClick += delegate(Control snd, MouseEventArgs e)
            {
                if (cb_fullScreen.Checked == true)
                {
                    ScreenManager.activeFullScreen();
                }
                else
                {
                    ScreenManager.deactivFullScreen();
                }

                if (cb_blockFps.Checked == true)
                {
                    ScreenManager.fpsUnlock();
                }
                else
                    ScreenManager.fpsLock();
                if (cb_showFps.Checked == true)
                {
                    if (!ScreenManager.Game.Components.Contains(frc))
                    {
                        ScreenManager.Game.Components.Add(frc);
                    }
                }
                else
                    ScreenManager.Game.Components.Remove(frc);
            };

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _desktop.Update();
        }



        private void HandleInput()
        {

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            _desktop.Draw();
        }

        public string GetTitle()
        {
            return "Options";
        }

        public string GetDetails()
        {
            return "Quit menu";
        }
    }
}
