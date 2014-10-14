using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Squid;
using GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gearit.src.GUI.OptionsMenu
{
	class OptionsMenu : GameScreen, IDemoScreen
	{
		public static OptionsMenu Instance { get; private set; }

		ScreenManager _screen;
		Desktop _desktop;
		Panel _dialog_co;
		DropDownList _resolution;
		DropDownList _fullscreen;
		DropDownList _volume;
		Button _save_btn;
        DropDownList _showFps;
        FrameRateCounter _frc = null;

		const int DIALOG_WIDTH = 400;
		const int TAB_WIDTH = 156;

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
			_desktop.Position = new Squid.Point(ScreenMainMenu.MENU_WIDTH, 0);
			_desktop.Size = new Squid.Point(ScreenManager.Width - ScreenMainMenu.MENU_WIDTH, ScreenManager.Height);

			_dialog_co = new Panel();
			_dialog_co.Position = new Squid.Point(ScreenManager.Width / 2 - DIALOG_WIDTH, ScreenManager.Height / 4);
			_dialog_co.Size = new Squid.Point((ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 2, DIALOG_WIDTH / 2 + 120);
			_dialog_co.Parent = _desktop;
			_dialog_co.Style = "menu";

			Label TitleLabel = new Label();
			TitleLabel.Size = new Squid.Point(100, 45);
			TitleLabel.Dock = DockStyle.Top;
			TitleLabel.Text = "OPTIONS";
			TitleLabel.Style = "itemMenuTitle";
			TitleLabel.TextAlign = Alignment.MiddleLeft;
			TitleLabel.Margin = new Margin(0, 0, 0, -1);
			_dialog_co.Content.Controls.Add(TitleLabel);

			ListBoxItem item = null;

			// Resolution Option
			addLabel(0, 70, (int)((float)_dialog_co.Size.x / 2.5), 34, "Resolution");
			_resolution = initDropBox(158, 34, _dialog_co.Size.x / 2 - 16, 70);
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
			addLabel(0, 120, (int)((float)_dialog_co.Size.x / 2.5), 34, "Fullscreen");
			_fullscreen = initDropBox(158, 34, _dialog_co.Size.x / 2 - 16, 120);
			item = addListBoxItem(_fullscreen, "Yes");
			if (_screen.IsFullScreen)
				_fullscreen.SelectedItem = item;
			item = addListBoxItem(_fullscreen, "No");
			if (!_screen.IsFullScreen)
				_fullscreen.SelectedItem = item;

            // Show fps
            addLabel(0, 220, (int)((float)_dialog_co.Size.x / 2.5), 34, "Show fps");
            _showFps = initDropBox(158, 34, _dialog_co.Size.x / 2 - 16, 220);
            item = addListBoxItem(_showFps, "Yes");
            if (ScreenManager.Game.Components.Contains(_frc))
                _showFps.SelectedItem = item;
            item = addListBoxItem(_showFps, "No");
            if (!ScreenManager.Game.Components.Contains(_frc))
                _showFps.SelectedItem = item;

            // Volume control
                addLabel(0, 170, (int)((float)_dialog_co.Size.x / 2.5), 34, "Volume");
			_volume = initDropBox(158, 34, _dialog_co.Size.x / 2 - 16, 170);
			for (int i = 1; i <= 10; i++)
				item = addListBoxItem(_volume, i.ToString());
			_volume.SelectedItem = item;

			// Save button
			_save_btn = new Button();
			_save_btn.Size = new Squid.Point(124, 34);
			_save_btn.Position = new Squid.Point(_dialog_co.Size.x - 124 - 8,_dialog_co.Size.y - 34 - 8);
			_save_btn.Text = "Save";
			_dialog_co.Content.Controls.Add(_save_btn);

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

		private void addLabel(int posX, int posY, int sizeX, int sizeY, string label)
		{
			Label lb = new Label();
			lb.Position = new Squid.Point(posX, posY);
			lb.Size = new Squid.Point(sizeX, sizeY);
			lb.TextAlign = Alignment.MiddleRight;
			lb.Text = label;
			_dialog_co.Content.Controls.Add(lb);
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

		private DropDownList initDropBox(int sizeX, int sizeY, int posX, int posY)
		{
			DropDownList dropBox = new DropDownList();
			dropBox.Size = new Squid.Point(sizeX, sizeY);
			dropBox.Position = new Squid.Point(posX, posY);
			dropBox.Label.Style = "comboLabel";
			dropBox.Button.Style = "comboButton";
			dropBox.Listbox.Margin = new Margin(0, 0, 0, 0);
			dropBox.Listbox.Style = "frame";
            dropBox.Opened += delegate(Control sender, SquidEventArgs args)
            {
                dropBox.Dropdown.Position = new Squid.Point(dropBox.Dropdown.Position.x - ScreenMainMenu.MENU_WIDTH, dropBox.Dropdown.Position.y);
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

			_dialog_co.Content.Controls.Add(dropBox);
			dropBox.Focus();
			return (dropBox);
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
