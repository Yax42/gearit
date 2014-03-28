using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using System.IO;

namespace gearit.src.GUI
{
    public class MessageBoxSave
    {
        MessageBox _msg;
        TextBox _tb;

        public MessageBoxSave(Desktop dk, String name, Action<String> cbSave)
        {
            _msg = MessageBox.Show(new Point(300, 160), "Load Robot", "Name", MessageBoxButtons.OK, dk);

            _tb = new TextBox();
            _tb.Size = new Squid.Point(158, 34);
            _tb.Position = new Squid.Point(_msg.MessageLabel.Size.x + 16, 70);
            _tb.Text = name;

            _msg.Controls.Add(_tb);

            _msg.GetControl(MessageBoxButtons.OK.ToString()).MouseClick += delegate(Control sender, MouseEventArgs args)
            {
                _msg.Close();
                cbSave(_tb.Text);
            };
        }
    }
}
