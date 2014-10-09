using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using Squid;

namespace gearit.xna
{
	/// <summary>
	/// Displays the FPS
	/// </summary>
	public class FrameRateCounter : DrawableGameComponent
	{
		private NumberFormatInfo _format;
		private Vector2 _position;
		private ScreenManager _screenManager;
        private Desktop _desktop = new Desktop();
        private TextBox tb_ping = new TextBox();
        private TextBox tb_ping_shadow = new TextBox();

		public FrameRateCounter(ScreenManager screenManager)
			: base(screenManager.Game)
		{
			_screenManager = screenManager;
			_format = new NumberFormatInfo();
			_format.NumberDecimalSeparator = ".";

            _desktop.Size = new Squid.Point(100, 100);
            _desktop.Position = new Squid.Point(0, 0);

            tb_ping_shadow.Position = new Squid.Point(30 + 1, 20 + 1);
            tb_ping_shadow.Size = new Squid.Point(100, 40);
            tb_ping_shadow.Style = "textblack";
            tb_ping_shadow.TextColor = ColorInt.RGBA(0.0f, 0.0f, 0.0f, 1.0f);
            tb_ping_shadow.Parent = _desktop;

            tb_ping.Position = new Squid.Point(30, 20);
            tb_ping.Size = new Squid.Point(100, 40);
            tb_ping.Parent = _desktop;
            tb_ping.Style = "textwhite";
		}

		public override void Update(GameTime gameTime)
		{
			string fps = string.Format(_format, "{0} fps", _screenManager.getFPS());
            tb_ping.Text = fps;
            tb_ping_shadow.Text = fps;

            _desktop.Update();
		}

		public override void Draw(GameTime gameTime)
		{
            _desktop.Draw();
		}
	}
}