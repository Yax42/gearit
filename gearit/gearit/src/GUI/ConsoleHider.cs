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
    static public class ConsoleHider
    {
        static private Desktop _desktop;
        static private ScreenManager _screen;
        static private Control background = new Control();
        static private Button btn_hide;
        static public int Width = 21;
        static public int Height = 20;

        static public void init(ScreenManager screen)
        {
            _desktop = new Desktop();
            _screen = screen;
            _desktop.Size = new Point(Width, Height);
            _desktop.Position = new Point(screen.Width - Width - 4, 4);

            System.Diagnostics.Debug.WriteLine("opacity =" + _desktop.GetOpacity());

            btn_hide = new Button();
            btn_hide.Size = new Squid.Point(Width, Height);
            btn_hide.Position = new Squid.Point(0, 0);
            btn_hide.Text = "X\n";

            background.Parent = _desktop;
            background.Style = "menu";
            background.Dock = DockStyle.Fill;

            _desktop.Controls.Add(btn_hide);
            btn_hide.MouseClick += delegate(Control snd, MouseEventArgs e)
            {
                ChatBox.Hide();
            };
        }

        static public void Update()
        {
            _desktop.Update();

            if (Input.justReleased(Microsoft.Xna.Framework.Input.Keys.Enter))
                ;// toggleInputMode(); //FIXME: Temporary disabled (because it's triggered on every single enter being pressed)
        }

        static public Desktop getDesktop()
        {
            return (_desktop);
        }

    }
}
