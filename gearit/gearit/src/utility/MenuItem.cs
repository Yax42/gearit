using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

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
        Right = 4,
        Bottom = 8
    }

    class MenuItem
    {
        // Persistant
        MenuOverlay _menu;
        private float _scale;
        private ItemMenuLayout _layout;
        private ItemMenuAlignement _alignement;
        private Vector2 _padding;
        private ContentManager _content;
        private bool _visible = true;

        // Text
        private SpriteFont _font;
        private string _text;
        private Color _color;

        // Sprite
        private Texture2D _sprite = null;

        // Focus
        private bool _focusable = false;
        private int _id = 0;
        private Color _bg_focus;
        private bool _focused = false;
        private RectangleOverlay _rectangle_bg;
        private bool _pressed = false;

        // Refreshing
        private Vector2 _pos;
        private Vector2 _pos_rsrc;
        private Vector2 _size;

        public MenuItem(MenuOverlay menu, string text, SpriteFont font, Color color, Vector2 padding, ItemMenuLayout layout, ItemMenuAlignement alignement, float scale)
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

        public MenuItem(ContentManager content, MenuOverlay menu, string rsrc_name, Vector2 padding, ItemMenuLayout layout, ItemMenuAlignement alignement, float scale)
        {
            _menu = menu;
            _scale = scale;
            _layout = layout;
            _alignement = alignement;
            _padding = padding;
            _content = content;
            _sprite = _content.Load<Texture2D>(rsrc_name);
        }

        public void refresh(Vector2 pos, Vector2 size)
        {
            _pos = pos;
            _pos_rsrc = _pos;
            _size = size;
   
            // Refreshing position ressource depending on alignment
            switch (_alignement)
            {
                case ItemMenuAlignement.Default:
                    _pos_rsrc += _padding;
                    break;
                case ItemMenuAlignement.Bottom:
                    _pos_rsrc += new Vector2(_padding.X, size.Y - _padding.Y);
                    break;
                case ItemMenuAlignement.HorizontalCenter:
                    _pos_rsrc += new Vector2(size.X / 2 - getSize().X / 2, _padding.Y);
                    break;
                case ItemMenuAlignement.VerticalCenter:
                    _pos_rsrc += new Vector2(_padding.X, size.Y / 2 - getRsrcSize().Y / 2);
                    break;
                case ItemMenuAlignement.Right:
                    _pos_rsrc += new Vector2(size.X - _padding.X - getRsrcSize().X, _padding.Y);
                    break;
            }
        }

        public void addFocus(int id, Color bg_focus, GraphicsDevice graph)
        {
            _id = id;
            _focusable = true;
            _bg_focus = bg_focus;
            Rectangle rec = new Rectangle((int)(_pos.X + _menu.Position.X), (int)_pos.Y + (int)_menu.Position.Y, (int)_size.X, (int)_size.Y);
            _rectangle_bg = new RectangleOverlay(rec, _bg_focus, graph);
        }

        public bool Focused
        {
            set { _focused = value; }
            get { return _focused; }
        }

        public bool Focusable
        {
            set { _focusable = value; }
            get { return _focusable; }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public void Draw(SpriteBatch batch)
        {
            if (_visible)
            {
                // Focus background
                if (_focusable && (_focused || _pressed))
                    _rectangle_bg.Draw(batch, _pos + _menu.Position);
                // Draw texture or string
                if (_sprite == null)
                    batch.DrawString(_font, _text, _pos_rsrc + _menu.Position, _color, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
                else
                    batch.Draw(_sprite, _pos_rsrc + _menu.Position, Color.White);
            }
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public bool Pressed
        {
            get { return _pressed; }
            set { _pressed = value; }
        }

        public Rectangle getRectangle()
        {
            return (new Rectangle((int)_pos.X + (int)_menu.Position.X, (int)_pos.Y + (int)_menu.Position.Y,
                (int)_size.X, (int)_size.Y));
        }

        public Vector2 getTextSize()
        {
            return (_font.MeasureString(_text) * _scale);
        }

        public Vector2 getSpriteSize()
        {
            return (new Vector2(_sprite.Width, _sprite.Height));
        }

        public Vector2 getSize()
        {
            if (_sprite == null)
                return (getTextSize() + _padding * 2);
            return (getSpriteSize() + _padding * 2);
        }

        public Vector2 getRsrcSize()
        {
            if (_sprite == null)
                return (getTextSize());
            return (getSpriteSize());
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public string Display
        {
            get 
            { 
                if (_sprite == null)
                    return _text;
                return _sprite.Name;
            }
            set
            {
                if (_sprite == null)
                    _text = value;
                else
                    _sprite = _content.Load<Texture2D>(value);
            }
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
