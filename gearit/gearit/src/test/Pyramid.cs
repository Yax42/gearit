using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;

namespace gearit.src.utility
{
    public class Pyramid
    {
        private Sprite _box;
        private List<Body> _boxes;

        public Pyramid(World world, Vector2 position, int count, float density, AssetCreator creator)
        {
            Vertices rect = PolygonTools.CreateRectangle(0.1f, 0.1f);
            PolygonShape shape = new PolygonShape(rect, density);

            Vector2 rowStart = position;
            rowStart.Y -= 0.5f + count * 1.1f;

            Vector2 deltaRow = new Vector2(-0.625f, 0.4f);
            const float spacing = 0.25f;

            _boxes = new List<Body>();

            for (int i = 0; i < count; ++i)
            {
                Vector2 pos = rowStart;

                for (int j = 0; j < i + 1; ++j)
                {
                    Body body = BodyFactory.CreateBody(world);
                    body.BodyType = BodyType.Dynamic;
                    body.Position = pos;
                    body.CreateFixture(shape);
                    _boxes.Add(body);

                    pos.X += spacing;
                }

                rowStart += deltaRow;
            }

            //GFX
            _box = new Sprite(creator.TextureFromVertices(rect, MaterialType.Blank, Color.SaddleBrown, 2f));
        }

        public void Draw(SpriteBatch batch)
        {
            for (int i = 0; i < _boxes.Count; ++i)
            {
                batch.Draw(_box.Texture, ConvertUnits.ToDisplayUnits(_boxes[i].Position), null,
                            Color.White, _boxes[i].Rotation, _box.Origin, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}