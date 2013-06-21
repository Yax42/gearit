using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace gearit.src.utility.Menu
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

    abstract class MenuItem
    {
        // Persistant
        protected MenuOverlay _menu;
        protected float _scale;
        protected ItemMenuLayout _layout;
        protected ItemMenuAlignement _alignement;
        protected Vector2 _padding;
        protected bool _visible = true;

        // Focus
        protected bool _focusable = false;
        protected int _id = 0;
        protected Color _bg_focus;
        protected bool _focused = false;
        protected RectangleOverlay _rectangle_bg;
        protected bool _pressed = false;

        // Refreshing
        protected Vector2 _pos;
        protected Vector2 _pos_rsrc;
        protected Vector2 _size;

        public MenuItem(MenuOverlay menu, Vector2 padding, ItemMenuLayout layout, ItemMenuAlignement alignement, float scale)
        {
            _menu = menu;
            _scale = scale;
            _layout = layout;
            _alignement = alignement;
            _padding = padding;
        }

        virtual public void inputHandler(Input input)
        {
        }

        public bool hasFlag(ItemMenuAlignement alignement, ItemMenuAlignement flag)
        {
            return ((alignement & flag) == flag);
        }

        virtual public void refresh(Vector2 pos, Vector2 size)
        {
            _pos = pos;
            _pos_rsrc = _pos;
            _size = size;
   
            // Refreshing position ressource depending on alignment
            _pos_rsrc += _padding;
            if (hasFlag(_alignement, ItemMenuAlignement.Bottom))
                _pos_rsrc += new Vector2(_padding.X, size.Y - getRsrcSize().Y);
            if (hasFlag(_alignement, ItemMenuAlignement.HorizontalCenter))
                _pos_rsrc += new Vector2(size.X / 2 - getSize().X / 2 - _padding.X, 0);
            if (hasFlag(_alignement, ItemMenuAlignement.VerticalCenter))
                _pos_rsrc += new Vector2(_padding.X, size.Y / 2 - getRsrcSize().Y / 2 - padding.Y);
            if (hasFlag(_alignement, ItemMenuAlignement.Right))
                _pos_rsrc += new Vector2(size.X - _padding.X - getRsrcSize().X, _padding.Y);
        }

        public void addFocus(int id, Color bg_focus)
        {
            _id = id;
            _focusable = true;
            _bg_focus = bg_focus;
            Rectangle rec = new Rectangle((int)(_pos.X + _menu.Position.X), (int)_pos.Y + (int)_menu.Position.Y, (int)_size.X, (int)_size.Y);
            _rectangle_bg = new RectangleOverlay(rec, _bg_focus, _menu.Screen.GraphicsDevice);
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

        virtual public void Draw(SpriteBatch batch)
        {
            // Focus background
            if (_focusable && (_focused || _pressed))
                _rectangle_bg.Draw(batch, _pos + _menu.Position);
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

        abstract public  Vector2 getRsrcSize();

        public Vector2 getSize()
        {
            return (getRsrcSize() + _padding * 2);
        }

        virtual public string Display
        {
            get;
            set;
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
