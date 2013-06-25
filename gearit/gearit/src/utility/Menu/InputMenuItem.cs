using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
        private RectangleOverlay _cursor;
        private InputMenuItemType _type = InputMenuItemType.AlphaNumeric;
        private int _border_size;
        private const int PADDING_X = 4;
        private int _max_text_size = 9999;

        // Properties
        private Vector2 _size_input;
        private string _text = "";
        private SpriteFont _text_font;
        private Color _bg_color;
        private Vector2 _pos_text;
        private Color _border_color;
        private Color _text_color;

        // Label
        private SpriteFont _label_font = null;
        private string _label_text = "";
        private Color _label_color;
        private Vector2 _label_pos;
        private float _label_scale;
        private const int LABEL_SPACING = 16;

        // Focus
        private Color _focus_color = new Color(173, 216, 230);
        private int _cursor_pos = 0;
        private int _char_selected = 0;
        private int _direction;
        private RectangleOverlay _selected_bg;

        public InputMenuItem(MenuOverlay menu, SpriteFont text_font, Color text_color, int border_size, Vector2 padding, Vector2 size_input, ItemMenuLayout layout, ItemMenuAlignement alignement, float scale)
            : base(menu, padding, layout, alignement, scale)
        {
            _text_font = text_font;
            _text_color = text_color;
            _border_size = border_size;
            _border_color = Color.Black;
            _bg_color = _menu.Color;
            _size_input = size_input;
            _border_bg = new RectangleOverlay(Rectangle.Empty, _border_color, _menu.Screen.GraphicsDevice);
            _input_bg = new RectangleOverlay(Rectangle.Empty, _bg_color, _menu.Screen.GraphicsDevice);
            _cursor = new RectangleOverlay(Rectangle.Empty, Color.Black, _menu.Screen.GraphicsDevice);
            _selected_bg = new RectangleOverlay(Rectangle.Empty, _focus_color, _menu.Screen.GraphicsDevice);
            updateGeometry();

            // Adding to menu
            _menu.addItemMenu(this);
        }

        // Item is pressed - manage input
        override public void inputHandler()
        {
            List<Keys> keys;

            if ((keys = Input.getJustPressed()).Count > 0)
            {
                foreach (Keys key in keys)
                {
                    // Check special key first
                    if (!manageSpecialKey(key))
                        insertInput(key);
                }
            }
        }

        public bool insertInput(Keys key)
        {
            string input = Input.keyToString(key);
            string concat = input + _text;

            // Clear selection
            if (input == "")
                return (false);

            clearSelection();

            // Check length
            if (_text.Length + input.Length >= _max_text_size || _text_font.MeasureString(concat).X + PADDING_X * 2 > _size_input.X)
                return (false);

            // Concatenate depending on the cursor position
            string start_text = _text.Substring(0, _cursor_pos);
            string end_text = (_cursor_pos >= _text.Length ? "" : _text.Substring(_cursor_pos, _text.Length - _cursor_pos));
            _text = start_text + input + end_text;

            _cursor_pos += input.Length;
            return (true);
        }

        public bool manageSpecialKey(Keys key)
        {
            // Enter : unfocus
            if (key == Keys.Enter)
                _menu.releasePressed();
            // Delete selection
            if (key == Keys.Delete)
            {
                if (_char_selected > 0)
                    clearSelection();
                else if (_cursor_pos < _text.Length)
                    _text = _text.Remove(_cursor_pos, 1);
                return (true);
            }
            // Delete selection or left cursor char
            else if (key == Keys.Back)
            {
                if (_char_selected > 0)
                    clearSelection();
                else if (_cursor_pos > 0)
                {
                    _text = _text.Remove(_cursor_pos - 1, 1);
                    --_cursor_pos;
                }
                return (true);
            }
            // Manage CTRL+A - Select all
            else if ((key == Keys.A || key == Keys.LeftControl) && (Input.pressed(Keys.A) && Input.pressed(Keys.LeftControl)))
            {
                _char_selected = _text.Length;
                _cursor_pos = 0;    
                return (true);
            }
            // Manage cursor
            else if (key == Keys.Right || key == Keys.Left)
            {
                tryMoveCursor((key == Keys.Right ? 1 : -1));
                return (true);
            }
            return (false);
        }

        public bool tryMoveCursor(int direction)
        {
            int try_cursor = _cursor_pos + direction;

            if (try_cursor < 0 || try_cursor > _text.Length)
                return (false);
            
            // Manage (un)selection by shift
            if (Input.pressed(Keys.LeftShift) || Input.pressed(Keys.RightShift))
            {
                // Manage end of input
                if (direction == 1 && _cursor_pos + _char_selected == _text.Length)
                    return (false);

                Console.WriteLine(_cursor_pos + " == " + _text.Length);

                // First time selected
                if (_char_selected == 0)
                    _direction = direction;

                // Do not increase selection if direction is inversed
                if (_direction != direction)
                {
                    if (direction == 1)
                        ++_cursor_pos;
                     --_char_selected;
                }
                else
                {
                    if (direction == -1)
                        --_cursor_pos;
                     ++_char_selected;
                }
            }
            // Reset selection and move cursor
            else
            {
                _char_selected = 0;
                _cursor_pos = try_cursor;
            }

            return (true);
        }

        override public void draw(DrawGame drawer)
        {
            if (_visible)
            {
                // Init size/geometry
                Rectangle rec = _input_bg.Geometry;

                base.draw(drawer);
                _border_bg.draw(drawer);
                _input_bg.draw(drawer);

                // Draw label
                if (_label_font != null)
                    drawer.drawString(_label_font, _label_text, _label_pos, _label_color, 0f, _scale, SpriteEffects.None, _label_scale);

                // Draw cursor
                if (_char_selected == 0)
                {
                    if (_pressed)
                    {
                        Rectangle pos = rec;

                        pos.Width = 1;
                        pos.Height -= 8;
                        pos.Y += 4;
                        int text_x = (int)_text_font.MeasureString(_text.Substring(0, _cursor_pos)).X;
                        int cursor_x = text_x + PADDING_X + 1;
                        pos.X += cursor_x;
                        _cursor.Geometry = pos;
                        _cursor.draw(drawer);
                    }
                }
                // Manage selection
                else
                {
                    rec.X += (int)_text_font.MeasureString(_text.Substring(0, _cursor_pos)).X;
                    rec.Width = (int)_text_font.MeasureString(_text.Substring(_cursor_pos, _char_selected)).X;
                    rec.Y += 2;
                    rec.Height -= 4;

                    // FIX PADDING
                    rec.X += PADDING_X;
                    rec.Width += 1;

                    _selected_bg.Geometry = rec;
                    _selected_bg.draw(drawer);
                }

                drawer.drawString(_text_font, _text, _pos_text, _text_color, 0f, _scale, SpriteEffects.None, 0f);
            }
        }

        public void clearSelection()
        {
            if (_char_selected > 0)
            {
                _text = _text.Remove(_cursor_pos, _char_selected);
                _char_selected = 0;
            }
        }

        public void setLabel(string text, SpriteFont font, Color color, float label_scale = 1f)
        {
            _label_font = font;
            _label_color = color;
            _label_text = text;
            _label_scale = label_scale;

            _menu.refreshMenu();
        }

        public int MaxSize
        {
            get { return _max_text_size; }
            set { _max_text_size = value; }
        }

        override public void refresh(Vector2 pos, Vector2 size)
        {
            base.refresh(pos, size);
            updateGeometry();
        }

        private void updateGeometry()
        {
            Rectangle rec = new Rectangle((int)_pos_rsrc.X + (int)_menu.Position.X, (int)_pos_rsrc.Y + (int)_menu.Position.Y, (int)_size_input.X, (int)_size_input.Y);

            // Adjust if label
            if (_label_font != null)
            {
                Vector2 label_size = _label_font.MeasureString(_label_text) * _label_scale;

                _label_pos = new Vector2(rec.X, rec.Y + rec.Height / 2 - label_size.Y / 2);
                rec.X += (int)label_size.X + LABEL_SPACING;
            }

            _border_bg.Geometry = rec;
            rec.X += _border_size;
            rec.Y += _border_size;
            rec.Width -= _border_size * 2;
            rec.Height -= _border_size * 2;
            _input_bg.Geometry = rec;

            // Update text if possible
            _pos_text.X = rec.X + PADDING_X;
            _pos_text.Y = rec.Y + rec.Height / 2 - _text_font.MeasureString("X").Y / 2;
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

        public Color FocusColor
        {
            get { return _focus_color; }
            set { _focus_color = value; }
        }

        public Color BorderColor
        {
            get { return _border_color; }
            set { _border_color = value; }
        }

        override public Vector2 getRsrcSize()
        {
            Vector2 size = _size_input;

            if (_label_font != null)
            {
                Vector2 label_size = _label_font.MeasureString(_label_text) * _label_scale;
                size.X += label_size.X;
                if (label_size.Y > size.Y)
                    size.Y = label_size.Y;
                size.X += LABEL_SPACING;
            }

            return (size);
        }

        override public string Display
        {
            get { return _text; }
            set
            {
                _text = value;
                _cursor_pos = _text.Length;
            }
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
