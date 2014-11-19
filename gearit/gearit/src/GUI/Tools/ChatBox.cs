using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using gearit.xna;
using gearit.src.output;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using GUI;

namespace gearit.src.GUI
{
    /// <summary>
    /// ChatBox for logging messages and communication between players
    /// </summary>
	static public class ChatBox
	{
		static public string LastInput;
		static private Desktop _chat_box;
		static private Frame _container_chat = new Frame();
		static private Control background = new Control();
		static public Frame ButtonFrame;
		static ListBox listbox = new ListBox();
		static private Boolean _has_input = false;
		static private TextBox _input = new TextBox();
		static public Action<String, Entry> InterceptNewItem = null;
		static private Button btn_hide;


		public enum Entry
		{
			Message,
			Info,
			Warning,
			Network,
			Error,
		};

		static public int Width = 720;
		static public int Height = 160;
		static public int SIZE_ITEM = 22;

		static public void UpdateSize()
		{
			if (!ScreenManager.IsIngame)
			{
				Width = ScreenManager.Instance.Width - ScreenMainMenu.Instance.PosX;
				_chat_box.Position = new Point(ScreenMainMenu.Instance.PosX, ScreenManager.Instance.Height - Height);
			}
			else
			{
				Width = ScreenManager.Instance.Width;
				_chat_box.Position = new Point(0, ScreenManager.Instance.Height - Height);
			}
			_chat_box.Size = new Point(Width, Height);
			btn_hide.Position = new Squid.Point(Width - 21, Height - 20);
		}

		static public void Init()
		{
			_chat_box = new Desktop();
			_container_chat.Parent = _chat_box;
			_container_chat.Dock = DockStyle.Fill;
			//Width = 10;// ScreenManager.Instance.Width;
			_chat_box.Position = new Point(0, ScreenManager.Instance.Height - Height);
			_chat_box.Size = new Point(Width, Height);

			// Background
			background.Parent = _container_chat;
			background.Style = "chatbox";
			background.Dock = DockStyle.Fill;
			background.Opacity = 0.9f;

			listbox.Margin = new Squid.Margin(2);
			listbox.Size = new Point(Width, Height);
			listbox.Scrollbar.Size = new Squid.Point(14, 10);
			listbox.Scrollbar.Slider.Style = "vscrollTrack";
			listbox.Scrollbar.Slider.Button.Style = "vscrollButton";
			listbox.Scrollbar.ButtonUp.Style = "vscrollUp";
			listbox.Scrollbar.ButtonUp.Size = new Squid.Point(10, 20);
			listbox.Scrollbar.ButtonDown.Style = "vscrollUp";
			listbox.Scrollbar.ButtonDown.Size = new Squid.Point(10, 20);
			listbox.Scrollbar.Slider.Margin = new Margin(0, 2, 0, 2);
			listbox.Multiselect = false;
			listbox.MaxSelected = 0;
			listbox.Parent = _container_chat;
			listbox.Scrollbar.MouseScrollSpeed = 0.15f;
			
			OutputManager.LogMessage("Init ChatBox");

			_input.Size = new Point(Width, SIZE_ITEM);
			_input.Position = new Point(0, Height - SIZE_ITEM);
			_input.Visible = false;
			_input.Style = "textbox";
			_input.Parent = _container_chat;



			btn_hide = new Button();
			btn_hide.Size = new Squid.Point(21, 20);
			btn_hide.Text = "-\n";
			btn_hide.Parent = _chat_box;
			btn_hide.Position = new Squid.Point(Width - 21, Height - 20);

			btn_hide.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				ChatBox.Toggle();
			};
			Toggle();
		}

        static public void QuickLoadContent()
        {
            _chat_box.Size = new Squid.Point(Math.Min(Width, ScreenManager.Instance.Width), Height);
            _chat_box.Position = new Point(ScreenManager.Instance.Width - Math.Min(Width, ScreenManager.Instance.Width) - 4, 4);
            _chat_box.Update();
        }

		static public void Update()
		{
			UpdateSize();
			_chat_box.Update();

            if (Input.justReleased(Microsoft.Xna.Framework.Input.Keys.F2)
				|| Input.PadJustPressed(Buttons.Back))
            {
                ChatBox.Toggle();
            }

			if (ScreenManager.IsIngame)
			{
				if (Input.Enter)
					toggleInputMode();
			}
			else
			{
				_has_input = false;
			}
		}

        static public void Toggle()
        {
            if (_container_chat.Visible == false)
				_container_chat.Visible = true;
            else
				_container_chat.Visible = false;
			btn_hide.Checked = _container_chat.Visible;
        }

		private static void toggleInputMode()
		{
			if (_has_input)
			{
				listbox.Size = new Point(Width, Height);

				if (_input.Text != "")
				{
					//OutputManager.LogMessage(_input.Text);
					LastInput = _input.Text;
					_input.Text = "";
				}
			}
			else
			{
				listbox.Size = new Point(Width, Height - SIZE_ITEM);
				_input.Focus();
			}

			_has_input = !_has_input;
			_input.Visible = _has_input;
		}

		static public bool hasFocus()
		{
			if (_has_input)
				return (true);

			return (_chat_box.Visible &&
				_chat_box.Position.x <= Input.position().X &&
				_chat_box.Position.x + _chat_box.Size.x >= Input.position().X &&
				_chat_box.Position.y <= Input.position().Y &&
				_chat_box.Position.y + _chat_box.Size.y >= Input.position().Y);
		}

		static public Desktop getDesktop()
		{
			return (_chat_box);
		}

		static public void addEntry(string text, Entry entry = Entry.Message, Boolean intercept = false)
		{
			ScreenManager.Instance.beginDrawing();

			if (intercept && InterceptNewItem != null)
			{
				InterceptNewItem(text, entry);
				return;
			}

			ListBoxItem item = new ListBoxItem();

			if (entry != Entry.Message)
			{
				switch (entry)
				{
					case Entry.Error:
						text = "[color=ffFF4040]" + text + "[/color]";
						break;
					case Entry.Warning:
						text = "[color=ffF4A460]" + text + "[/color]";
						break;
					case Entry.Info:
						text = "[color=ff70DB93]" + text + "[/color]";
						break;
					case Entry.Network:
						text = "[color=ff3F9FF6]" + text + "[/color]";
						break;
				}
				item.BBCodeEnabled = true;
			}

			item.Text = text;
			item.Margin = new Margin(0, 0, 0, 1);
			item.Size = new Squid.Point(Width, 22);
			item.Style = "label";
			listbox.Items.Add(item);

			listbox.Scrollbar.SetValue(1);

			ScreenManager.Instance.stopDrawing();
		}

		static public void mergeEntry(string text)
		{
			listbox.Items.Last().Text += text;
			listbox.Scrollbar.SetValue(1);
		}
	}
}
