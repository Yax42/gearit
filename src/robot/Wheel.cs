using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;

namespace gearit
{
    class Wheel : Piece
    {
      public	Wheel(Robot robot, Spot spot, Vector2 start, float size) : 
          base(robot, new CircleShape(size, 0)) //density 0
      {
          spot.connect(robot.getWorld(), this, new Vector2(0, 0));
          Console.WriteLine("Wheel created.");
      }
    }
}
