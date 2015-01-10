using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;

namespace gearit.xna
{
	/// <summary>
	/// The background screen sits behind all the other menu screens.
	/// It draws a background image that remains fixed in place regardless
	/// of whatever transitions the screens on top of it may be doing.
	/// </summary>
	public class BackgroundScreen : GameScreen
	{
		private const float LogoScreenHeightRatio = 0.25f;
		private const float LogoScreenBorderRatio = 0.0375f;
		private const float LogoWidthHeightRatio = 1.4f;

		private Texture2D _backgroundTexture;
		private Rectangle _logoDestination;
		private Texture2D _logoTexture;
		private Texture2D _logoBeautyTexture;
		private Rectangle _viewport;
		private Texture2D _logoGearit;
		private Rectangle _logoGearitDestination;

		/// <summary>
		/// Constructor.
		/// </summary>
		public BackgroundScreen() : base(false)
		{
			TransitionOnTime = TimeSpan.FromSeconds(0.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);
		}

		public override void LoadContent()
		{
			_logoTexture = ScreenManager.Content.Load<Texture2D>("Common/logo");
			_logoBeautyTexture = ScreenManager.Content.Load<Texture2D>("GUI/logo");
			//_backgroundTexture = ScreenManager.Content.Load<Texture2D>("Common/gradient");
			_backgroundTexture = ScreenManager.Content.Load<Texture2D>("Common/bg");
			_logoGearit = ScreenManager.Content.Load<Texture2D>("Common/bg_gearit");

			// Logo Farseer
			Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
			Vector2 logoSize = new Vector2();
			logoSize.Y = viewport.Height * LogoScreenHeightRatio;
			logoSize.X = logoSize.Y * LogoWidthHeightRatio;

			float border = viewport.Height * LogoScreenBorderRatio;
			Vector2 logoPosition = new Vector2(viewport.Width - border - logoSize.X,
											   viewport.Height - border - logoSize.Y);
			_logoDestination = new Rectangle((int)logoPosition.X, (int)logoPosition.Y, (int)logoSize.X,
											 (int)logoSize.Y);

			// Logo Gear it
			Vector2 logoSize2 = new Vector2();
			logoSize2.Y = viewport.Height * LogoScreenHeightRatio;
			logoSize2.X = logoSize2.Y * LogoWidthHeightRatio;

			float border2 = viewport.Height * 0.7f;
			Vector2 logoPosition2 = new Vector2(viewport.Width - border2 - logoSize2.X,
											   viewport.Height - border2 - logoSize2.Y);
			_logoGearitDestination = new Rectangle((int)logoPosition2.X, (int)logoPosition2.Y, (int)logoSize2.X,
											 (int)logoSize2.Y);

			_viewport = viewport.Bounds;
		}

		/// <summary>
		/// Updates the background screen. Unlike most screens, this should not
		/// transition off even if it has been covered by another screen: it is
		/// supposed to be covered, after all! This overload forces the
		/// coveredByOtherScreen parameter to false in order to stop the base
		/// Update method wanting to transition off.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		/// <summary>
		/// Draws the background screen.
		/// </summary>
		public override void Draw(GameTime gameTime)
		{
			ScreenManager.SpriteBatch.Begin();
			ScreenManager.SpriteBatch.Draw(_backgroundTexture, ScreenManager.GraphicsDevice.Viewport.Bounds, Color.White);
			ScreenManager.SpriteBatch.Draw(_logoBeautyTexture, _logoDestination, Color.White * 0.6f);
			//ScreenManager.SpriteBatch.Draw(_logoTexture, _logoDestination, Color.White * 0.6f);
			//ScreenManager.SpriteBatch.Draw(_logoGearit, _logoGearitDestination, Color.White * 0.6f);
			ScreenManager.SpriteBatch.End();
		}
	}
}