using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Squid;
using GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;

namespace gearit.src.GUI.OptionsMenu
{
	class OptionsMenu : GameScreen, IDemoScreen
	{
		public static OptionsMenu Instance { get; private set; }

		ScreenManager _screen;
		Desktop _desktop;
		//Panel _dialog_co;
		DropDownList _resolution;
		DropDownList _fullscreen;
		DropDownList _volume;
		Button _save_btn;
        DropDownList _showFps;
        FrameRateCounter _frc = null;
        Panel _background = new Panel();

		const int DIALOG_WIDTH = 400;
		const int TAB_WIDTH = 156;
        static public int MENU_WIDTH = 220;
        static public int ITEM_HEIGHT = 30;
        static public int PADDING = 2;

		public OptionsMenu(ScreenManager screen) : base(false)
		{
			Instance = this;
			_screen = screen;
		}


		public override void LoadContent()
		{
			base.LoadContent();
            if (_frc == null)
                _frc = new FrameRateCounter(ScreenManager);

			VisibleMenu = true;

			_desktop = new Desktop();
            _desktop.Position = new Squid.Point(0, 0);
            _desktop.Size = new Squid.Point(ScreenManager.Width, ScreenManager.Height);

            _background.Parent = _desktop;
            _background.Style = "menu";
            _background.Position = new Squid.Point(0, 0);
            _background.Size = new Squid.Point(MENU_WIDTH, (ITEM_HEIGHT + PADDING) * 11);

			ListBoxItem item = null;

            //Video
            addLabel(0, 0, MENU_WIDTH, ITEM_HEIGHT, "VIDEO", "itemMenuTitle");

			// Resolution Option
			addLabel(0, 1, MENU_WIDTH, ITEM_HEIGHT, "RESOLUTION", "itemMenuSubtitle");
            _resolution = initDropBox(0, 2, MENU_WIDTH, ITEM_HEIGHT);
			int resolCpt = 0;
			foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
			{
				if (resolCpt > 4) // Temporary. Hide low resolution so it does not crash.
					item = addListBoxItem(_resolution, mode.Width.ToString() + " x " + mode.Height.ToString(), resolCpt.ToString());
				if (_screen.GraphicsDevice.Viewport.Width == mode.Width && _screen.GraphicsDevice.Viewport.Height == mode.Height)
					_resolution.SelectedItem = item;
				resolCpt++;
			}

			// Fullscreen option
			addLabel(0, 3, MENU_WIDTH, ITEM_HEIGHT, "FULLSCREEN", "itemMenuSubtitle");
			_fullscreen = initDropBox(0, 4, MENU_WIDTH, ITEM_HEIGHT);
			item = addListBoxItem(_fullscreen, "Yes");
			if (_screen.IsFullScreen)
				_fullscreen.SelectedItem = item;
			item = addListBoxItem(_fullscreen, "No");
			if (!_screen.IsFullScreen)
				_fullscreen.SelectedItem = item;


            // Volume control
            addLabel(0, 5, MENU_WIDTH, ITEM_HEIGHT, "VOLUME", "itemMenuTitle");
			_volume = initDropBox(0, 6, MENU_WIDTH, ITEM_HEIGHT);
			for (int i = 1; i <= 10; i++)
				item = addListBoxItem(_volume, i.ToString());
			_volume.SelectedItem = item;

            //Other
            addLabel(0, 7, MENU_WIDTH, ITEM_HEIGHT, "OTHER", "itemMenuTitle");

            // Show fps
            addLabel(0, 8, MENU_WIDTH, ITEM_HEIGHT, "SHOW FPS", "itemMenuSubtitle");
            _showFps = initDropBox(0, 9, MENU_WIDTH, ITEM_HEIGHT);
            item = addListBoxItem(_showFps, "Yes");
            if (ScreenManager.Game.Components.Contains(_frc))
                _showFps.SelectedItem = item;
            item = addListBoxItem(_showFps, "No");
            if (!ScreenManager.Game.Components.Contains(_frc))
                _showFps.SelectedItem = item;

			// Save button
			_save_btn = new Button();
			_save_btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
            _save_btn.Position = new Squid.Point(0, (ITEM_HEIGHT + PADDING) * 10);
			_save_btn.Text = "SAVE";
            _save_btn.Style = "itemMenuButton";
			_background.Content.Controls.Add(_save_btn);

			_save_btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				// Apply fullscreen option
				if (_fullscreen.SelectedItem.Text == "Yes" && !_screen.IsFullScreen)
					_screen.activeFullScreen();
				else if (_fullscreen.SelectedItem.Text == "No" && _screen.IsFullScreen)
					_screen.deactivFullScreen();

				// Apply resolution option
				if (_resolution.SelectedItem.Text != _screen.GraphicsDevice.Viewport.Width.ToString() + " x " + _screen.GraphicsDevice.Viewport.Height.ToString())
				{
					string res = _resolution.SelectedItem.Text;
					_screen._graphics.PreferredBackBufferWidth = Convert.ToInt32((res.Substring(0, res.IndexOf(" "))));
					_screen._graphics.PreferredBackBufferHeight = Convert.ToInt32((res.Substring(res.LastIndexOf(" ") + 1)));
					_screen._graphics.ApplyChanges();
				}

                //Apply fps
                if (_showFps.SelectedItem.Text == "Yes")
                {
                    if (!ScreenManager.Game.Components.Contains(_frc))
                    {
                        ScreenManager.Game.Components.Add(_frc);
                    }
                }
                else
                    ScreenManager.Game.Components.Remove(_frc);

				// Apply volume level


                
			};
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			_desktop.Update();
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			_desktop.Draw();
		}

		private void addLabel(int posX, int idY, int sizeX, int sizeY, string label, string style)
		{
			Label lb = new Label();
			lb.Position = new Squid.Point(posX, (ITEM_HEIGHT + PADDING) * idY);
			lb.Size = new Squid.Point(sizeX, sizeY);
			//lb.TextAlign = Alignment.MiddleRight;
			lb.Text = label;
            lb.Style = style;
			_background.Content.Controls.Add(lb);
		}

		private ListBoxItem addListBoxItem(DropDownList list, string name, string tag = "")
		{
			ListBoxItem item = new ListBoxItem();
			item.Text = name;
			item.Size = new Squid.Point(100, 35);
			item.Margin = new Margin(0, 0, 0, 4);
			item.Style = "item";
			item.Tag = tag;
			list.Items.Add(item);
			return (item);
		}

		private DropDownList initDropBox(int posX, int idY, int sizeX, int sizeY)
		{
			DropDownList dropBox = new DropDownList();
			dropBox.Size = new Squid.Point(sizeX, sizeY);
			dropBox.Position = new Squid.Point(posX, (ITEM_HEIGHT + PADDING) * idY);
			dropBox.Label.Style = "comboLabel";
			dropBox.Button.Style = "comboButton";
			dropBox.Listbox.Margin = new Margin(0, 0, 0, 0);
			dropBox.Listbox.Style = "frame";
            dropBox.Opened += delegate(Control sender, SquidEventArgs args)
            {
                dropBox.Dropdown.Position = new Squid.Point(dropBox.Dropdown.Position.x /*- ScreenMainMenu.MENU_WIDTH*/, dropBox.Dropdown.Position.y);
            };

			dropBox.Listbox.ClipFrame.Margin = new Margin(8, 8, 8, 8);
			dropBox.Listbox.Scrollbar.Margin = new Margin(0, 4, 4, 4);
			dropBox.Listbox.Scrollbar.Size = new Squid.Point(14, 10);
			dropBox.Listbox.Scrollbar.ButtonUp.Style = "vscrollUp";
			dropBox.Listbox.Scrollbar.ButtonUp.Size = new Squid.Point(10, 20);
			dropBox.Listbox.Scrollbar.ButtonDown.Style = "vscrollUp";
			dropBox.Listbox.Scrollbar.ButtonDown.Size = new Squid.Point(10, 20);
			dropBox.Listbox.Scrollbar.Slider.Margin = new Margin(0, 2, 0, 2);
			dropBox.Listbox.Scrollbar.Slider.Style = "vscrollTrack";
			dropBox.Listbox.Scrollbar.Slider.Button.Style = "vscrollButton";

			_background.Content.Controls.Add(dropBox);
			dropBox.Focus();
			return (dropBox);
		}

        override public void positionChanged(int x, int y)
        {
            _background.Position = new Squid.Point(x, y);
        }

        override public Squid.Point getMenuSize()
        {
            return (_background.Size);
        }

        override public Squid.Point getMenuPosition()
        {
            return (_background.Position);
        }

		public string GetTitle()
		{
			return "Options";
		}

		public string GetDetails()
		{
			return "Configure various options";
		}
	}
}
