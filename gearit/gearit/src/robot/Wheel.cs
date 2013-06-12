using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;

namespace gearit
{
    class Wheel : Piece
    {
        public Wheel(ref Robot robot, ref float size) :
            base(robot, new CircleShape(size, 0)) //density 0 ~= poids
        {
            Console.WriteLine("Wheel created.");
        }
    }
}
