using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using gearit.xna;

namespace gearit.src.utility.Menu
{
    enum MenuLayout
    {
        Vertical,
        Horizontal
    }

    class MenuOverlay
    {
        // Persistant
        private ScreenManager _screen;
        private MenuLayout _layout;
        private RectangleOverlay _rectangle;

        // Properties
        private bool _adjusting = false;
        private bool _visible = true;
        private Vector2 _size;
        public Vector2 _pos;
        private Color _bg_color;
        private Vector2 _filled;

        // Items
        private List<MenuItem> _items;
        private MenuItem _item_focused = null;
        private MenuItem _item_pressed = null;
        private MenuItem _just_pressed = null;
        
        // Focus
        private bool _mouse_on = false;
        private bool _movable = false;
        private bool _moving = false;
        
        public MenuOverlay(ScreenManager screen, Vector2 pos, Vector2 size, Color bg, MenuLayout layout)
        {
            _screen = screen;
            _items = new List<MenuItem>();
            _layout = layout;
            _bg_color = bg;
            Rectangle rec = new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
            _rectangle = new RectangleOverlay(rec, _bg_color, _screen.GraphicsDevice);
            Geometry = rec;
        }
       
        public MenuItem addItemMenu(MenuItem item)
        {
            _items.Add(item);
            refreshMenu();
            return (item);
        }

        public void refreshMenu()
        {
            _filled = Vector2.Zero;
            Vector2 size = Vector2.Zero;
            bool is_w_min = (_size.X > _size.Y ? false : true);

            // Resettings position and size of every MenuItem
            foreach (MenuItem item in _items)
            {
                if (!item.Visible)
                    continue;

                // Getting the size depending on the layout
                switch (item.Layout)
                {
                    case ItemMenuLayout.MaxFromMin:
                        size = new Vector2((is_w_min ? _size.X : item.getSize().X), (!is_w_min ? _size.Y : item.getSize().Y));
                        break;
                    case ItemMenuLayout.Adaptive:
                        size = new Vector2(item.getSize().X, item.getSize().Y);
                        break;
                    case ItemMenuLayout.MaxHeight:
                        size = new Vector2(item.getSize().X, _size.Y);
                        break;
                    case ItemMenuLayout.MaxWidth:
                        size = new Vector2(_size.X, item.getSize().Y);
                        break;
                    case ItemMenuLayout.Maximum:
                        size = _size - _filled;
                        break;
                }

                // Refreshing our item
                item.refresh(_filled, size);

                // Adjusting the filled area of the menu depending on the orientation
                if (_layout == MenuLayout.Horizontal)
                    _filled.X += size.X;
                else
                    _filled.Y += size.Y;
            }

            adjust();
        }

        // Refreshing focus by mouse
        public void Update()
        {
            // Update focused item if any
            if (_item_pressed != null)
                _item_pressed.inputHandler();
            if (isIn(new Rectangle((int)_pos.X, (int)_pos.Y, (int)_size.X, (int)_size.Y), Input.position()))
            {
                _mouse_on = true;
                // Moving
                if (_moving)
                {
                    manageMovement();
                    return;
                }
                // Manage all state/position of mouse
                if (itemToggle())
                    return;
                // Manage movable
                if (_movable && Input.justPressed(MouseKeys.LEFT) && !_moving)
                    _moving = true;
            }
            else
            {
                if (Input.justPressed(MouseKeys.LEFT))
                    releasePressed();
                _mouse_on = false;
            }
            // Nothing happens - release
            releaseFocus();
        }

        public bool isMouseOn()
        {
            return (_mouse_on);
        }

        public ScreenManager Screen
        {
            get { return _screen; }
            set { _screen = value; }
        }

        private bool itemToggle()
        {
            // Checking if item (un)pressed
            if (Input.justPressed(MouseKeys.LEFT) && _item_focused != null)
            {
                togglePressed();
                return (true);
            }
            // Checking if in an item
            foreach (MenuItem item in _items)
                if (item.Focusable && isIn(item.getRectangle(), Input.position()))
                {
                    if (!item.Focused)
                    {
                        releaseFocus();
                        item.Focused = true;
                        _item_focused = item;
                    }
                    return (true);
                }
            return (false);
        }

        // (UN)Press item
        public void togglePressed()
        {
            // Release old pressed if not focused
            if (_item_pressed != null && _item_pressed != _item_focused)
            {
                _item_pressed.Pressed = false;
                _item_pressed = null;
            }
            
            // Toggle current
            if (_item_pressed != null)
            {
                _item_pressed.Pressed = false;
                _item_pressed = null;
            }
            else
            {
                _just_pressed = _item_focused;
                _item_pressed = _item_focused;
                _item_pressed.Pressed = true;
            }
        }

        private void manageMovement()
        {
            if (Input.justReleased(MouseKeys.LEFT))
                _moving = false;

            // Need to move 
            else if (_movable)
            {
                Vector2 pos = _pos + Input.mouseOffset();

                // Don't move if out of window
                if (pos.Y < 0)
                    pos.Y = 0;
                else if ((int)pos.Y + _size.Y > _screen.GraphicsDevice.Viewport.Height)
                    pos.Y = _pos.Y;
                if (pos.X < 0)
                    pos.X = 0;
                else if ((int)pos.X + _size.X > _screen.GraphicsDevice.Viewport.Width)
                    pos.X = _pos.X;
                Geometry = new Rectangle((int) pos.X, (int) pos.Y, (int)_size.X, (int)_size.Y);
            }
        }

        public Color Color
        {
            get { return _bg_color; }
            set { _bg_color = value; }
        }

        // Drawing - Loop on all itemMenu and draw them
        public void draw(DrawGame drawer)
        {
            if (_visible)
            {
                _rectangle.draw(drawer);
                foreach (MenuItem item in _items)
                    item.draw(drawer);
            }
        }

        public void releaseFocus()
        {
            if (_item_focused != null)
            {
                _item_focused.Focused = false;
                _item_focused = null;
            }
        }

        public void releasePressed()
        {
            if (_item_pressed != null)
            {
                _item_pressed.Pressed = false;
                _item_pressed = null;
            }
        }

        // Get last pressed - reset it
        public MenuItem justPressed()
        {
            MenuItem item = _just_pressed;

            _just_pressed = null;
            return (item);
        }

        // Adjusting the size if needed
        private void adjust()
        {
//            Console.WriteLine(_size + " // " + _filled);
            if (_adjusting)
            {
                
                if (_layout == MenuLayout.Horizontal)
                    _size.X = _filled.X;
                else
                    _size.Y = _filled.Y;
                Geometry = new Rectangle((int)_pos.X, (int)_pos.Y, (int)_size.X, (int)_size.Y);
            }
        }

        public bool Adjusting
        {
            get { return _adjusting; }
            set
            {
                _adjusting = value;
                adjust();
            }
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public bool hasItemPressed()
        {
            return (_item_pressed != null);
        }

        public MenuItem getFocused()
        {
            return (_item_focused);
        }

        public Rectangle Geometry
        {
            set
            {
                _rectangle.Geometry = value;
                _pos = new Vector2(value.X, value.Y);
                _size = new Vector2(value.Width, value.Height);
            }
            get { return _rectangle.Geometry; }
        }

        public bool canAdd(string text, SpriteFont font, Vector2 padding)
        {
            Vector2 filled = Vector2.Zero;

            foreach (MenuItem item in _items)
                filled += item.getSize();

            filled += font.MeasureString(text) + padding;

            if (filled.X > _size.X || filled.Y > _size.Y)
                return (false);
            return (true);
        }

        public MenuItem getItem(string rsrc)
        {
            foreach (MenuItem item in _items)
                if (item.Display == rsrc)
                    return (item);
            return (null);
        }

        public MenuItem getItem(int id)
        {
            foreach (MenuItem item in _items)
                if (item.Id == id)
                    return (item);
            return (null);
        }

        public bool isIn(Rectangle rec, Vector2 p)
        {
            if (p.X > rec.X && p.X < rec.X + rec.Width
                && p.Y > rec.Y && p.Y < rec.Y + rec.Height)
                return (true);
            return (false);
        }

        public bool Movable
        {
            get { return _movable; }
            set { _movable = value; }
        }

        public Vector2 Position
        {
            get { return (_pos); }
        }

        public Vector2 Size
        {
            get { return (_size); }
        }

    }
}
