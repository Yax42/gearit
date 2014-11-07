using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using System.IO;
using GUI;

namespace gearit.src.GUI
{
	public class MessageBoxInput
	{
		MessageBox _msg;
		TextBox _tb;
		Action _cbCancel;

		public MessageBoxInput(Desktop dk, String title, Action<String> cbDone)
		{
			_msg = MessageBox.Show(new Point(250, 160), title, "", MessageBoxButtons.OK, dk);

			_tb = new TextBox();
			_tb.Size = new Squid.Point(250 - 32, 34);
			_tb.Position = new Squid.Point(16, 70);
			_tb.Style = "menuTextbox";
			_msg.Controls.Add(_tb);
			_tb.Focus();

			_msg.GetControl(DialogResult.OK.ToString()).MouseClick += delegate(Control sender, MouseEventArgs args)
				{
					_msg.Close();
					cbDone(_tb.Text);
				};
			_cbCancel = delegate()
			{
				_msg.Close();
			};
		}

		public void cancel()
		{
			_cbCancel();
		}
	}
}
