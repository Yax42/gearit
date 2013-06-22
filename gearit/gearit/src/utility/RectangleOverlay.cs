using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gearit.src.utility
{
    public class RectangleOverlay
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

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(_tex, _rec, _color);
        }

        public void Draw(SpriteBatch batch, Vector2 pos)
        {
            batch.Draw(_tex, new Rectangle((int)pos.X, (int)pos.Y, _rec.Width, _rec.Height), _color);
        }

        public Rectangle Geometry
        {
            get { return _rec; }
            set { _rec = value; }
        }
    }
}
