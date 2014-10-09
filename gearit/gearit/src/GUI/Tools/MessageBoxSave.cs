using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using System.IO;
using GUI;

namespace gearit.src.GUI
{
	public class MessageBoxSave
	{
		MessageBox _msg;
		TextBox _tb;
		Action _cbCancel;

		public MessageBoxSave(Desktop dk, String name, Action<String, bool> cbSave, Action<bool> setFocus, string objectName, bool isExiting = false)
		{
			if (isExiting)
				_msg = MessageBox.Show(new Point(300, 160), "Save " + objectName + " before exiting?", "Name", MessageBoxButtons.YesNoCancel, dk);
			else
				_msg = MessageBox.Show(new Point(300, 160), "Save " + objectName, "Name", MessageBoxButtons.OKCancel, dk);

			_tb = new TextBox();
			_tb.Size = new Squid.Point(158, 34);
			_tb.Position = new Squid.Point(_msg.MessageLabel.Size.x + 16, 70);
			_tb.Text = name;
			_msg.Controls.Add(_tb);
			_tb.Focus();

			if (isExiting)
			{
				_msg.GetControl(DialogResult.Yes.ToString()).MouseClick += delegate(Control sender, MouseEventArgs args)
				{
					_msg.Close();
					cbSave(_tb.Text, true);
				};
				_msg.GetControl(DialogResult.No.ToString()).MouseClick += delegate(Control sender, MouseEventArgs args)
				{
					_msg.Close();
					ScreenMainMenu.GoBack = true;
				};
			}
			else
			{
				_msg.GetControl(DialogResult.OK.ToString()).MouseClick += delegate(Control sender, MouseEventArgs args)
				{
					_msg.Close();
					cbSave(_tb.Text, false);
				};
			}
			_cbCancel = delegate()
			{
				_msg.Close();
				setFocus(false);
			};
			_msg.GetControl(DialogResult.Cancel.ToString()).MouseClick += delegate(Control sender, MouseEventArgs args)
			{
				cancel();
			};
		}

		public void cancel()
		{
			_cbCancel();
		}
	}
}
