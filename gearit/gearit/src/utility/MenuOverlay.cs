using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.utility
{
    enum MenuLayout
    {
        Vertical,
        Horizontal
    }

    class MenuOverlay
    {
        // Persistant
        private GraphicsDevice _graph;
        private ContentManager _content;
        private MenuLayout _layout;
        private RectangleOverlay _rectangle;

        // Properties
        private bool _adjusting = false;
        private Vector2 _size;
        public Vector2 _pos;
        private Color _bg_color;
        private Vector2 _filled;

        // Items
        private List<MenuItem> _items;
        private int _item_focused = 0;
        private int _item_pressed = 0;
        
        // Focus
        private bool _movable = false;
        private bool _moving = false;
        
        public MenuOverlay(GraphicsDevice graph, ContentManager content, Vector2 pos, Vector2 size, Color bg, MenuLayout layout)
        {
            _items = new List<MenuItem>();
            _layout = layout;
            _content = content;
            _graph = graph;
            _bg_color = bg;
            Rectangle rec = new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
            _rectangle = new RectangleOverlay(rec, _bg_color, _graph);
            Geometry = rec;
        }
       
        public MenuItem addItemMenu(string text, SpriteFont font, Color color, Vector2 padding, ItemMenuLayout layout = ItemMenuLayout.MaxFromMin, ItemMenuAlignement alignement = ItemMenuAlignement.Default, float scale = 1f)
        {
            MenuItem item = new MenuItem(this, text, font, color, padding, layout, alignement, scale);

            _items.Add(item);
            refreshMenu();
            return (item);
        }

        public MenuItem addItemMenu(string rsrc_name, Vector2 padding, ItemMenuLayout layout = ItemMenuLayout.MaxFromMin, ItemMenuAlignement alignement = ItemMenuAlignement.Default, float scale = 1f)
        {
            MenuItem item = new MenuItem(_content, this, rsrc_name, padding, layout, alignement, scale);

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
        public void Update(Input input)
        {
            if (_moving)
                manageMovement(input);
            else if (isIn(new Rectangle((int)_pos.X, (int)_pos.Y, (int)_size.X, (int)_size.Y), input.position()))
            {
                // Manage all state/position of mouse
                if (itemToggle(input))
                    return;
                // Manage movable
                if (input.justPressed(MouseKeys.LEFT) && _movable && !_moving)
                    _moving = true;
            }
            releaseFocus();
        }

        private bool itemToggle(Input input)
        {
            // Checking if item (un)pressed
            if (input.justPressed(MouseKeys.LEFT) && _item_focused != 0)
            {
                MenuItem item_focused = getItem(_item_focused);

                if (item_focused.Pressed)
                {
                    item_focused.Pressed = false;
                    _item_pressed = 0;
                }
                else
                {
                    item_focused.Pressed = true;
                    _item_pressed = _item_focused;
                }
            }
            // Checking if in an item
            foreach (MenuItem item in _items)
                if (item.Focusable && isIn(item.getRectangle(), input.position()))
                {
                    if (!item.Focused)
                    {
                        releaseFocus();
                        item.Focused = true;
                        _item_focused = item.Id;
                    }
                    return (true);
                }
            return (false);
        }

        private void manageMovement(Input input)
        {
            if (input.justReleased(MouseKeys.LEFT))
                _moving = false;

            // Need to move 
            else if (_movable)
            {
		/*
		Je pense que c'est plus propre comme ça :

                Vector2 pos = _pos + input.mouseOffset();

                // Don't move if out of window
                if (pos.Y < 0)
                    pos.Y = 0;
                else if ((int)pos.Y + _size.Y > _graph.Viewport.Height)
                    pos.Y = _pos.Y;
                if (pos.X < 0)
                    pos.X = 0;
                else if ((int)pos.X + _size.X > _graph.Viewport.Width)
                    pos.X = _pos.X;
                Geometry = new Rectangle(pos.X, pos.Y, (int)_size.X, (int)_size.Y);
		*/
                Vector2 offset = input.mouseOffset();
                int x = (int)_pos.X + (int)offset.X;
                int y = (int)_pos.Y + (int)offset.Y;

                // Don't move if out of window
                if (y < 0)
                    y = 0;
                else if (y + _size.Y > _graph.Viewport.Height)
                    y = (int)_pos.Y;
                if (x < 0)
                    x = 0;
                else if (x + _size.X > _graph.Viewport.Width)
                    x = (int)_pos.X;
                Geometry = new Rectangle(x, y, (int)_size.X, (int)_size.Y);
            }
        }

        // Drawing - Loop on all itemMenu and draw them
        public void Draw(SpriteBatch batch)
        {
            _rectangle.Draw(batch);
            foreach (MenuItem item in _items)
                item.Draw(batch);
        }

        public void releaseFocus()
        {
            if (_item_focused != 0)
            {
                getItem(_item_focused).Focused = false;
                _item_focused = 0;
            }
        }

        public int getPressed()
        {
            int item = _item_pressed;

            _item_pressed = 0;
            return (item);
        }

        // Adjusting the size if needed
        private void adjust()
        {
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

        public bool hasItemPressed()
        {
            foreach (MenuItem item in _items)
                if (item.Pressed)
                    return (true);
            return (false);
        }

        public int getFocused()
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
