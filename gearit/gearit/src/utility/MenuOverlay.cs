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
        private Vector2 _size;
        public Vector2 _pos;
        private Color _bg_color;

        // Items
        private List<MenuItem> _items;
        private int _item_focused = 0;
        private int _item_pressed = 0;
        
        // Focus
        private bool _movable = false;
        private bool _moving = false;
        private MouseState _old_mouse;
        private Vector2 _focus_pos;
        
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
            Vector2 filled = Vector2.Zero;
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
                        size = _size - filled;
                        break;
                }

                // Refreshing our item
                item.refresh(filled, size);

                // Adjusting the filled area of the menu depending on the orientation
                if (_layout == MenuLayout.Horizontal)
                    filled.X += size.X;
                else
                    filled.Y += size.Y;
            }
        }

        // Refreshing focus by mouse
        public void Update(MouseState mouse)
        {
            Vector2 mouse_pos = new Vector2(mouse.X, mouse.Y);

            if (_moving)
                manageMovement(mouse);
            else if (isIn(new Rectangle((int)_pos.X, (int)_pos.Y, (int)_size.X, (int)_size.Y), mouse_pos))
            {
                // Manage all state/position of mouse
                if (itemToggle(mouse, mouse_pos))
                    return;
                // Manage movable
                if (mouse.LeftButton == ButtonState.Pressed && _movable && !_moving)
                    _moving = true;
            }
            releaseFocus();
            _old_mouse = mouse;
        }

        // Drawing - Loop on all itemMenu and draw them
        public void Draw(SpriteBatch batch)
        {
            _rectangle.Draw(batch);
            foreach (MenuItem item in _items)
                item.Draw(batch);
        }

        private bool itemToggle(MouseState mouse, Vector2 mouse_pos)
        {
            if (mouse.LeftButton == ButtonState.Pressed && _item_focused != 0)
                _item_pressed = _item_focused;
            foreach (MenuItem item in _items)
                if (item.Focusable && isIn(item.getRectangle(), mouse_pos))
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

        private void manageMovement(MouseState mouse)
        {
            if (mouse.LeftButton == ButtonState.Released)
                _moving = false;
            // Need to move 
            else if (_movable)
            {
                int x = (int)_pos.X + mouse.X - (int)_old_mouse.X;
                int y = (int)_pos.Y + mouse.Y - (int)_old_mouse.Y;
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

        public void releaseFocus()
        {
            if (_item_focused != 0)
            {
                getItem(_item_focused).Focused = false;
                _item_focused = 0;
            }
        }

        public int itemPressed()
        {
            int item = _item_pressed;

            if (_item_pressed != 0)
            {
                _item_pressed = 0;
                return (item);
            }
            return (0);
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
