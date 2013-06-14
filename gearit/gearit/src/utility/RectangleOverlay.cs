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
        Texture2D dummyTexture;
        Rectangle dummyRectangle;
        Color Colori;

        public RectangleOverlay(Rectangle rect, Color colori, GraphicsDevice graph)
        {
            dummyRectangle = rect;
            Colori = colori;

            dummyTexture = new Texture2D(graph, 1, 1);
            dummyTexture.SetData(new Color[] { Color.White });
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(dummyTexture, dummyRectangle, Colori);
        }
    }
}
