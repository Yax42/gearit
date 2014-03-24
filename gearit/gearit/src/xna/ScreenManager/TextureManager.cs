using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Textures
{
    class TextureManager
    {
        public enum ETexture
        {
            Diamond = 0,
            Spade,
            Club,
            Heart
        };
        private Dictionary<ETexture, Texture2D> _textures;
        private static TextureManager _instance = null;

        private TextureManager()
        {
            _textures = new Dictionary<ETexture, Texture2D>();
            _textures.Add(ETexture.Diamond, SquidXNA.Game.ContentManager.Load<Texture2D>("diamond"));
            _textures.Add(ETexture.Spade, SquidXNA.Game.ContentManager.Load<Texture2D>("spade"));
            _textures.Add(ETexture.Club, SquidXNA.Game.ContentManager.Load<Texture2D>("club"));
            _textures.Add(ETexture.Heart, SquidXNA.Game.ContentManager.Load<Texture2D>("heart"));
        }

        public static TextureManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TextureManager();
                return _instance;
            }
        }

        public Texture2D getTexture(ETexture t)
        {
            return _textures[t];
        }
    }
}
