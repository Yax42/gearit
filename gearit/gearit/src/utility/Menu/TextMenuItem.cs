using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace gearit.src.utility.Menu
{
	class TextMenuItem : MenuItem
	{
		private SpriteFont _font;
		private string _text;
		private Color _color;

		public TextMenuItem(MenuOverlay menu, string text, SpriteFont font, Color color, Vector2 padding, ItemMenuLayout layout, ItemMenuAlignement alignement, float scale)
			: base(menu, padding, layout, alignement, scale)
		{
			_font = font;
			_color = color;
			_text = text;

			// Adding to menu
			_menu.addItemMenu(this);
		}

		override public void draw(DrawGame drawer)
		{
			if (_visible)
			{
				base.draw(drawer);
				drawer.drawString(_font, _text, _pos_rsrc + _menu.Position, _color, 0f, _scale, SpriteEffects.None, 0f);
			}
		}

		override public Vector2 getRsrcSize()
		{
			return (_font.MeasureString(_text) * _scale);
		}

		override public string Display
		{
			get { return _text; }
			set { _text = value; }
		}

		public Color Color
		{
			get { return _color; }
			set { _color = value; }
		}

		public SpriteFont Font
		{
			get { return _font; }
			set { _font = value; }
		}
	}
}
