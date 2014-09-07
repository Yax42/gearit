using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using gearit.src.utility;

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

		public FrameRateCounter(ScreenManager screenManager)
			: base(screenManager.Game)
		{
			_screenManager = screenManager;
			_format = new NumberFormatInfo();
			_format.NumberDecimalSeparator = ".";
#if XBOX
			_position = new Vector2(55, 35);
#else
			_position = new Vector2(30, 25);
#endif
		}

		public override void Update(GameTime gameTime)
		{
		}

		public override void Draw(GameTime gameTime)
		{
			

			string fps = string.Format(_format, "{0} fps", _screenManager.getFPS());

			_screenManager.SpriteBatch.Begin();
			_screenManager.SpriteBatch.DrawString(_screenManager.Fonts.FrameRateCounterFont, fps,
												  _position + Vector2.One, Color.Black);
			_screenManager.SpriteBatch.DrawString(_screenManager.Fonts.FrameRateCounterFont, fps,
												  _position, Color.White);
			_screenManager.SpriteBatch.End();
		}
	}
}