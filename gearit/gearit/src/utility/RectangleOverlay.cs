using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gearit.src.utility
{
	class RectangleOverlay
	{
		Texture2D _tex;
		Rectangle _rec;
		Color _color;

		public RectangleOverlay(Rectangle rect, Color colori, GraphicsDevice graph)
		{
			_rec = rect;
			_color = colori;

			_tex = new Texture2D(graph, 1, 1);
			_tex.SetData(new Color[] { Color.White });
		}

		public Color Color
		{
			get { return _color; }
			set { _color = value; }
		}

		public void draw(DrawGame drawer)
		{
			drawer.drawTexture(_tex, _rec, _color);
		}

		public void draw(DrawGame drawer, Vector2 pos)
		{
			drawer.drawTexture(_tex, new Rectangle((int)pos.X, (int)pos.Y, _rec.Width, _rec.Height), _color);
		}

		public Rectangle Geometry
		{
			get { return _rec; }
			set { _rec = value; }
		}
	}
}
