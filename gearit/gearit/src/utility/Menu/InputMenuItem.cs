using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace gearit.src.utility.Menu
{
    enum InputMenuItemType
    {
        Numeric,
        Alpha,
        AlphaNumeric
    }

    class InputMenuItem : MenuItem
    {
        // Persistant
        private RectangleOverlay _border_bg;
        private RectangleOverlay _input_bg;
        private InputMenuItemType _type = InputMenuItemType.AlphaNumeric;
        private int _border_size;

        // Properties
        private Vector2 _size;
        private string _text;
        private SpriteFont _text_font = null;
        private Color _bg_color;
        private Color _border_color;
        private Color _text_color;

        // Label
        private SpriteFont _label_font = null;
        private string _label_text = "";
        private Color _label_color;

        public InputMenuItem(MenuOverlay menu, int border_size, Vector2 padding, Vector2 size, ItemMenuLayout layout, ItemMenuAlignement alignement, float scale)
            : base(menu, padding, layout, alignement, scale)
        {
            _border_size = border_size;
            _border_color = Color.Black;
            _bg_color = _menu.Color;
            _size = size;
            _border_bg = new RectangleOverlay(Rectangle.Empty, _border_color, _menu.Screen.GraphicsDevice);
            _input_bg = new RectangleOverlay(Rectangle.Empty, _bg_color, _menu.Screen.GraphicsDevice);
            updateGeometry();

            // Adding to menu
            _menu.addItemMenu(this);
        }

        override public void inputHandler(Input input)
        {

        }

        public void setLabel(string text, SpriteFont font, Color color)
        {
            _label_font = font;
            _label_color = color;
            _label_text = text;
        }

        override public void refresh(Vector2 pos, Vector2 size)
        {
            base.refresh(pos, size);
            updateGeometry();
        }

        private void updateGeometry()
        {
            Rectangle rec = new Rectangle((int)_pos_rsrc.X + (int)_menu.Position.X, (int)_pos_rsrc.Y + (int)_menu.Position.Y, (int)_size.X, (int)_size.Y);

            _border_bg.Geometry = rec;
            rec.X += _border_size;
            rec.Y += _border_size;
            rec.Width -= _border_size * 2;
            rec.Height -= _border_size * 2;
            _input_bg.Geometry = rec;
        }

        public InputMenuItemType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public Color BackgroundColor
        {
            get { return _bg_color; }
            set { _bg_color = value; }
        }

        public Color LabelColor
        {
            get { return _label_color; }
            set { _label_color = value; }
        }

        public Color TextColor
        {
            get { return _text_color; }
            set { _text_color = value; }
        }

        public Color BorderColor
        {
            get { return _border_color; }
            set { _border_color = value; }
        }

        override public void Draw(SpriteBatch batch)
        {
            if (_visible)
            {
                base.Draw(batch);
                _border_bg.Draw(batch);
                _input_bg.Draw(batch);
            }
        }

        override public Vector2 getRsrcSize()
        {
            Vector2 size = _size;
            if (_text_font != null)
                size += _text_font.MeasureString(_text) * _scale;
            return (size);
        }

        override public string Display
        {
            get { return _text; }
            set { _text = value; }
        }

        public Color Color
        {
            get { return _label_color; }
            set { _label_color = value; }
        }

        public SpriteFont TextFont
        {
            get { return _text_font; }
            set { _text_font = value; }
        }

        public SpriteFont LabelFont
        {
            get { return _label_font; }
            set { _label_font = value; }
        }
    }
}
