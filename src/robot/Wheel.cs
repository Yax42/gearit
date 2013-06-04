using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;

namespace gearit
{
    class Wheel : Piece
    {
      public	Wheel(World world, Spot spot, Vector2 start, float size) : 
          base(world,  new CircleShape(size, 0)) //density 0
      {
          spot.connect(world, this, new Vector2(0, 0));
          Console.WriteLine("Wheel created.");
      }
    }
}
