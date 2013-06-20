using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace gearit.src.utility.Menu
{
    class SpriteMenuItem : MenuItem
    {
        private Texture2D _sprite;

        public SpriteMenuItem(MenuOverlay menu, string rsrc_name, Vector2 padding, ItemMenuLayout layout, ItemMenuAlignement alignement, float scale)
            : base(menu, padding, layout, alignement, scale)
        {
            _sprite = _menu.Screen.Content.Load<Texture2D>(rsrc_name);

            // Adding to menu
            _menu.addItemMenu(this);
        }

        override public void Draw(SpriteBatch batch)
        {
            if (_visible)
            {
                base.Draw(batch);
                batch.Draw(_sprite, _pos_rsrc + _menu.Position, Color.White);
            }
        }

        override public Vector2 getRsrcSize()
        {
            return (new Vector2(_sprite.Width, _sprite.Height));
        }

        override public string Display
        {
            get { return _sprite.Name; }
            set { _sprite = _menu.Screen.Content.Load<Texture2D>(value); }
        }
    }
}
