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
using gearit.src.GUI.Tools;

namespace gearit.src.GUI.OptionsMenu
{
	class OptionsMenu : GameScreen, IDemoScreen
	{
		public static OptionsMenu Instance { get; private set; }

		private Desktop _desktop;
		//Panel _dialog_co;
		private DropDownList _resolution;
		private Button _save_btn;
        private FrameRateCounter _frc = null;
        private Panel _background = new Panel();
		private Button _btnShowFps;
		private Button _btnFullScreen;
		private int _volume;
		private Button[] _btnVolume;

		private const int DIALOG_WIDTH = 400;
		private const int TAB_WIDTH = 156;
		private const int VOLUME_WIDTH = 7;
        static public int MENU_WIDTH = 260;
        static public int ITEM_HEIGHT = 30;
        static public int PADDING = 8;

		public OptionsMenu(ScreenManager screen) : base(false)
		{
			Instance = this;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			int id = 0;
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
            addLabel(0, id, MENU_WIDTH, ITEM_HEIGHT, "OPTIONS", "itemMenuTitle");
			id++;

			// Resolution Option
            var lb = addLabel(0, id, MENU_WIDTH / 2, ITEM_HEIGHT, "Resolution", "itemMenu");
            _resolution = initDropBox(MENU_WIDTH / 2, id, MENU_WIDTH / 2 - PADDING, ITEM_HEIGHT);
			int resolCpt = 0;
			foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
			{
				if (resolCpt > 4) // Temporary. Hide low resolution so it does not crash.
					item = addListBoxItem(_resolution, mode.Width.ToString() + " x " + mode.Height.ToString(), resolCpt.ToString());
				if (ScreenManager.Instance.GraphicsDevice.Viewport.Width == mode.Width && ScreenManager.Instance.GraphicsDevice.Viewport.Height == mode.Height)
					_resolution.SelectedItem = item;
				resolCpt++;
			}

			// Fullscreen option
			id++;
             lb = addLabel(0, id, MENU_WIDTH / 2, ITEM_HEIGHT, "Fullscren", "itemMenu");
			_btnFullScreen = new Button();
			Button btn = _btnFullScreen;
			btn.Size = new Squid.Point(MENU_WIDTH / 2 - PADDING, ITEM_HEIGHT);
			btn.Position = new Squid.Point(lb.Position.x + lb.Size.x, (ITEM_HEIGHT + PADDING) * id);
			if (ScreenManager.Instance.IsFullScreen)
				btn.Text = "Yes";
			else
				btn.Text = "No";
			btn.Style = "button";
			_background.Content.Controls.Add(btn);
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				if (_btnFullScreen.Text == "No")
					_btnFullScreen.Text = "Yes";
				else
					_btnFullScreen.Text = "No";
			};

            // Show fps
			id++;
            lb = addLabel(0, id, MENU_WIDTH / 2, ITEM_HEIGHT, "Show fps", "itemMenu");
			_btnShowFps = new Button();
			btn = _btnShowFps;
			btn.Size = new Squid.Point(MENU_WIDTH / 2 - PADDING, ITEM_HEIGHT);
			btn.Position = new Squid.Point(lb.Position.x + lb.Size.x, (ITEM_HEIGHT + PADDING) * id);
            if (ScreenManager.Game.Components.Contains(_frc))
				btn.Text = "Yes";
			else
				btn.Text = "No";
			btn.Style = "button";
			_background.Content.Controls.Add(btn);
			btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				if (_btnShowFps.Text == "No")
					_btnShowFps.Text = "Yes";
				else
					_btnShowFps.Text = "No";
			};

            // Volume control
			id++;
            lb = addLabel(0, id, MENU_WIDTH / 2, ITEM_HEIGHT, "Volume", "itemMenu");

			_btnVolume = new Button[10];
			for (int i = 0; i < 10; i++)
			{
				btn = new Button();
				btn.Size = new Squid.Point(VOLUME_WIDTH, ITEM_HEIGHT);
				btn.Position = new Squid.Point(MENU_WIDTH - (11 - i) * (VOLUME_WIDTH + 1) - PADDING, (ITEM_HEIGHT + PADDING) * id);
				btn.Style = "button";
				btn.Tag = i;
				btn.MouseClick += delegate(Control snd, MouseEventArgs e)
				{
					_volume = (int) snd.Tag;
					for (int j = 0; j < 10; j++)
						_btnVolume[j].Checked = (j > _volume);
				};
				_background.Content.Controls.Add(btn);
				_btnVolume[i] = btn;
			}

			// Save button
			id += 2;
			_save_btn = new Button();
			_save_btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
            _save_btn.Position = new Squid.Point(0, (ITEM_HEIGHT + PADDING) * id);
			_save_btn.Text = "SAVE";
            _save_btn.Style = "itemMenuButton";
			_background.Content.Controls.Add(_save_btn);

			_save_btn.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				// Apply fullscreen option
				if (_btnFullScreen.Text == "Yes")
					ScreenManager.Instance.activeFullScreen();
				else
					ScreenManager.Instance.deactivFullScreen();

				// Apply resolution option
				if (_resolution.SelectedItem.Text != ScreenManager.Instance.GraphicsDevice.Viewport.Width.ToString() + " x " + ScreenManager.Instance.GraphicsDevice.Viewport.Height.ToString())
				{
					string res = _resolution.SelectedItem.Text;
					ScreenManager.Instance.Graphics.PreferredBackBufferWidth = Convert.ToInt32((res.Substring(0, res.IndexOf(" "))));
					ScreenManager.Instance.Graphics.PreferredBackBufferHeight = Convert.ToInt32((res.Substring(res.LastIndexOf(" ") + 1)));
					ScreenManager.Instance.Graphics.ApplyChanges();
				}

                //Apply fps
                if (_btnShowFps.Text == "Yes")
                {
                    if (!ScreenManager.Game.Components.Contains(_frc))
                    {
                        ScreenManager.Game.Components.Add(_frc);
                    }
                }
                else
                    ScreenManager.Game.Components.Remove(_frc);

				// Apply volume level

                ScreenManager.Instance.QuickLoadContent();
                GameScreen[] _screens = ScreenManager.GetScreens();
                foreach (GameScreen screen in _screens)
                {
                    screen.QuickLoadContent();
                }
                
			};
		}

        public override void QuickLoadContent()
        {
            base.QuickLoadContent();
            _desktop.Size = new Squid.Point(ScreenManager.Width, ScreenManager.Height);
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

		private Label addLabel(int posX, int idY, int sizeX, int sizeY, string label, string style)
		{
			Label lb = new Label();
			lb.Position = new Squid.Point(posX, (ITEM_HEIGHT + PADDING) * idY);
			lb.Size = new Squid.Point(sizeX, sizeY);
			//lb.TextAlign = Alignment.MiddleRight;
			lb.Text = label;
            lb.Style = style;
			_background.Content.Controls.Add(lb);
			return lb;
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
			/*dropBox.Label.Style = "comboLabel";
			dropBox.Button.Style = "comboButton";
			dropBox.Listbox.Margin = new Margin(0, 0, 0, 0);
			dropBox.Listbox.Style = "frame";
            dropBox.Opened += delegate(Control sender, SquidEventArgs args)
            {
                dropBox.Dropdown.Position = new Squid.Point(dropBox.Dropdown.Position.x , dropBox.Dropdown.Position.y);
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
			*/
			Theme.InitComboBox(dropBox);
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
