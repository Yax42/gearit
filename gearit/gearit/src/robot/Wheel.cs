using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;

namespace gearit
{
    class Wheel : Piece
    {
        private const int _circleSegments = 32;

        private float _size;

        public Wheel(ref Robot robot, ref float size) :
            base(robot, new CircleShape(size, 0)) //density 0 ~= poids
        {
            _size = size;
            _tex = robot.getAsset().TextureFromShape(_shape, MaterialType.Blank, Color.White, 1f);
            Console.WriteLine("Wheel created.");
        }

        public override void draw(SpriteBatch batch)
        {
            batch.Draw(_tex, ConvertUnits.ToDisplayUnits(this.Position), null, Color.LightGreen, this.Rotation,
                       new Vector2(_tex.Width / 2f, _tex.Height / 2f), 1f, SpriteEffects.None, 0f);
        }

        public override void vertices(VertexPositionColor[] vertices, ref int count)
        {
            const double increment = Math.PI * 2.0 / _circleSegments;
            double theta = 0.0;

            for (int i = 0; i < _circleSegments; i++)
            {
                Vector2 v1 = Position + _size * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                Vector2 v2 = Position + _size *
                             new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));

                vertices[count++] = new VertexPositionColor(new Vector3(v1, 0f), ColorValue);
                vertices[count++] = new VertexPositionColor(new Vector3(v2, 0f), ColorValue);

                theta += increment;
            }
        }
    }
}
