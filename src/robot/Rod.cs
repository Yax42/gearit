using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace gearit
{
    class Rod : Piece
    {
      private float	 _size;

      public	Rod(World world, FarseerPhysics.Dynamics.Body body, Vector2 start, float size) : 
          base(world, new EdgeShape(start, new Vector2(10, 10) + new Vector2(0, size)))
      {
          _size = size;
          Console.WriteLine("Rod created.");
      }
    }
}
