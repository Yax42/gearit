using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;

namespace gearit
{
    class Wheel : Piece
    {
        public Wheel(ref Robot robot, ref Spot spot, ref Vector2 start, ref float size) :
            base(robot, new CircleShape(size, 0)) //density 0
        {
            spot.connect(robot.getWorld(), this, new Vector2(0, 0));
            Console.WriteLine("Wheel created.");
        }
    }
}
