﻿using System;
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
        // Text
        private SpriteFont _font;
        private string _text;
        private Color _color;
        // Sprite
        private Texture2D _sprite = null;

        // Refreshing
        private Vector2 _pos;
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

        public MenuItem(MenuOverlay menu, Texture2D sprite, Vector2 padding, ItemMenuLayout layout, ItemMenuAlignement alignement, float scale)
        {
            _menu = menu;
            _scale = scale;
            _layout = layout;
            _alignement = alignement;
            _padding = padding;
            _sprite = sprite;
        }

        public void refresh(Vector2 pos, Vector2 size)
        {
            _pos = _menu.Position + pos;
            _size = size;

            switch (_alignement)
            {
                case ItemMenuAlignement.Default:
                    _pos += _padding;
                    break;
                case ItemMenuAlignement.Bottom:
                    _pos += new Vector2(_padding.X, size.Y - _padding.Y);
                    break;
                case ItemMenuAlignement.HorizontalCenter:
                    _pos += new Vector2(size.X / 2 - getSize().X / 2, _padding.Y);
                    break;
                case ItemMenuAlignement.VerticalCenter:
                    _pos += new Vector2(_padding.X, size.Y / 2 - getSize().Y / 2);
                    break;
                case ItemMenuAlignement.Right:
                    _pos += new Vector2(size.X - _padding.X - getSize().X, _padding.Y);
                    break;
            }
        }

        public void Draw(SpriteBatch batch)
        {
            if (_sprite == null)
                batch.DrawString(_font, _text, _pos + _padding, _color, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
            else
                batch.Draw(_sprite, _pos + _padding, Color.White);
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
