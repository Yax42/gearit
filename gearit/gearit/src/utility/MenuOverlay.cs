using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;

namespace gearit.src.utility
{
    enum MenuLayout
    {
        Vertical,
        Horizontal
    }

    class MenuOverlay
    {
        private Vector2 _pos;
        private Vector2 _size;
        private Color _bg_color;
        private List<MenuItem> _items;
        private RectangleOverlay _background;
        private GraphicsDevice _graph;
        private MenuLayout _layout;

        public MenuOverlay(GraphicsDevice graph, Vector2 pos, Vector2 size, Color bg, MenuLayout layout)
        {
            _items = new List<MenuItem>();
            _layout = layout;
            _graph = graph;
            _bg_color = bg;
            setGeometry(pos, size);
        }

        public void setGeometry(Vector2 pos, Vector2 size)
        {
            _pos = pos;
            _size = size;

            // Init background
            Rectangle rec = new Rectangle((int)_pos.X, (int)_pos.Y, (int)_size.X, (int)_size.Y);
            _background = new RectangleOverlay(rec, _bg_color, _graph);
        }
       
        public bool addItemMenu(string text, SpriteFont font, Color color, Vector2 padding, ItemMenuLayout layout = ItemMenuLayout.MaxFromMin, ItemMenuAlignement alignement = ItemMenuAlignement.Default, float scale = 1f)
        {
            Vector2 filled = Vector2.Zero;

            foreach (MenuItem item in _items)
              filled += item.getSize();

            if (filled.X > _size.X || filled.Y > _size.Y)
                return (false);
            _items.Add(new MenuItem(this, text, font, color, scale, layout, padding, alignement));
            refreshMenu();
            return (true);
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

                Console.WriteLine(filled);

                // Adjusting the filled area of the menu depending on the orientation
                if (_layout == MenuLayout.Horizontal)
                    filled.X += size.X;
                else
                    filled.Y += size.Y;
            }
        }

        public MenuItem getItem(int index)
        {
            return (_items[index]);
        }

        // Drawing - Loop on all itemMenu and draw them
        public void Draw(SpriteBatch batch)
        {
            _background.Draw(batch);
            foreach (MenuItem item in _items)
                item.Draw(batch);
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
