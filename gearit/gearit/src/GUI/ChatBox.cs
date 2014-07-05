using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using gearit.xna;
using gearit.src.output;
using gearit.src.utility;

namespace gearit.src.GUI
{
	static public class ChatBox
	{
		static private Desktop _chat_box;
		static private ScreenManager _screen;
		static private Control background = new Control();
		static public Frame ButtonFrame;
		static ListBox listbox = new ListBox();
		static private Boolean _has_input = false;
		static private TextBox _input = new TextBox();
		static public Action<String, Entry> InterceptNewItem = null;


		public enum Entry
		{
			Message,
			Info,
			Warning,
			Error
		};

		static public int Width = 750;
		static public int Height = 160;
		static public int SIZE_ITEM = 22;

		static public void init(ScreenManager screen)
		{
			_chat_box = new Desktop();
			_screen = screen;
			_chat_box.Size = new Point(Width, Height);
			_chat_box.Position = new Point(screen.Width - Width - 4, 4);

			// Background
			background.Parent = _chat_box;
			background.Style = "menu";
			background.Dock = DockStyle.Fill;

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
			listbox.Parent = _chat_box;
			listbox.Scrollbar.MouseScrollSpeed = 0.15f;
			
			OutputManager.LogMessage("Init ChatBox");

			_input.Size = new Point(Width, SIZE_ITEM);
			_input.Position = new Point(0, Height - SIZE_ITEM);
			_input.Visible = false;
			_input.Style = "textbox";
			_input.Parent = _chat_box;
		}

		static public void Update()
		{
			_chat_box.Update();

			if (Input.justReleased(Microsoft.Xna.Framework.Input.Keys.Enter))
				toggleInputMode();
		}

		private static void toggleInputMode()
		{
			if (_has_input)
			{
				listbox.Size = new Point(Width, Height);

				if (_input.Text != "")
				{
					OutputManager.LogMessage(_input.Text);
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
			if (intercept && InterceptNewItem != null)
			{
				InterceptNewItem(text, entry);
				return;
			}

			ListBoxItem item = new ListBoxItem();

			if (entry != Entry.Message)
			{
				if (entry == Entry.Error)
					text = "[color=ffFF4040]" + text + "[/color]";
				else if (entry == Entry.Warning)
					text = "[color=ffF4A460]" + text + "[/color]";
				else if (entry == Entry.Info)
					text = "[color=ff70DB93]" + text + "[/color]";
				item.BBCodeEnabled = true;
			}

			item.Text = text;
			item.Margin = new Margin(0, 0, 0, 1);
			item.Size = new Squid.Point(Width, 22);
			item.Style = "label";
			listbox.Items.Add(item);

			listbox.Scrollbar.SetValue(1);
		}
	}
}
