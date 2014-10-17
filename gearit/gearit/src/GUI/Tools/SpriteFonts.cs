using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using Microsoft.Xna.Framework;

namespace gearit.xna
{
	public class SpriteFonts
	{
		public SpriteFont DetailsFont;
		public SpriteFont FrameRateCounterFont;
		public SpriteFont MenuSpriteFont;
		public SpriteFont TitleFont;

		public SpriteFonts(ContentManager contentManager)
		{
			MenuSpriteFont = contentManager.Load<SpriteFont>("Fonts/menuFont");
			FrameRateCounterFont = contentManager.Load<SpriteFont>("Fonts/frameRateCounterFont");
			DetailsFont = contentManager.Load<SpriteFont>("Fonts/detailsFont");
			TitleFont = contentManager.Load<SpriteFont>("GUI/Fonts/Title");
			FrameRateCounterFont = contentManager.Load<SpriteFont>("Fonts/frameRateCounterFont");
			DetailsFont = contentManager.Load<SpriteFont>("Fonts/detailsFont");
		}

		static public Squid.Point GetTextSize(string text, SpriteFont font)
		{
			Vector2 size = font.MeasureString(text);
			return new Squid.Point((int)size.X, (int)size.Y);
		}
	}
}