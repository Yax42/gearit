using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace gearit.src.utility
{
    enum ItemMenuLayout
    {
        MaxWidth,
        MaxHeight,
        Maximum,
        MaxFromMin,
        Adaptive
    }

    [Flags]
    enum ItemMenuAlignement
    {
        Default = 0,
        VerticalCenter = 1,
        HorizontalCenter = 2,
        Left = 4,
        Bottom = 8
    }

    class MenuItem
    {
        // Persistant
        MenuOverlay _menu;
        private float _scale;
        private SpriteFont _font;
        private ItemMenuLayout _layout;
        private ItemMenuAlignement _alignement;
        private string _text;
        private Color _color;
        private Vector2 _padding;

        // Refreshing
        private Vector2 _pos;
        private Vector2 _size;
        

        public MenuItem(MenuOverlay menu, string text, SpriteFont font, Color color, float scale, ItemMenuLayout layout, Vector2 padding, ItemMenuAlignement alignement)
        {
            _menu = menu;
            _font = font;
            _scale = scale;
            _layout = layout;
            _color = color;
            _alignement = alignement;
            _text = text;
            _padding = padding;
        }

        public void refresh(Vector2 pos, Vector2 size)
        {
            _pos = _menu.Position + pos;
            _size = size;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.DrawString(_font, _text, _pos + _padding, _color, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
        }

        public Vector2 getTextSize()
        {
            return (_font.MeasureString(_text) * _scale);
        }

        public Vector2 getSize()
        {
            return (getTextSize() + _padding* 2);
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public Vector2 Position
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public Vector2 Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public Vector2 padding
        {
            get { return _padding; }
            set { _padding = value; }
        }

        public SpriteFont Font
        {
            get { return _font; }
            set { _font = value; }
        }

        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public ItemMenuLayout Layout
        {
            get { return _layout; }
            set { _layout = value; }
        }
    }
}
