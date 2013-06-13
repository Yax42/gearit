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
        public Wheel(ref Robot robot, ref float size) :
            base(robot, new CircleShape(size, 0)) //density 0 ~= poids
        {
            _tex = robot.getAsset().TextureFromShape(_shape, MaterialType.Blank, Color.White, 1f);
            Console.WriteLine("Wheel created.");
        }

        public override void draw(SpriteBatch batch)
        {
            batch.Draw(_tex, ConvertUnits.ToDisplayUnits(this.Position), null, Color.LightGreen, this.Rotation,
                       new Vector2(_tex.Width / 2f, _tex.Height / 2f), 1f, SpriteEffects.None, 0f);
        }
    }
}
