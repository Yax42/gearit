using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using gearit.src.robot;

namespace gearit
{
    class Rod : Piece, IMotor
    {
        private Motor _motor;
      private float	 _size;

      public	Rod(World world, Spot spot, Vector2 start, float size) : 
          base(world, new EdgeShape(start, new Vector2(10, 10) + new Vector2(0, size)))
      {
          _size = size;
          spot.connect(world, this, new Vector2(0, 0));
          Console.WriteLine("Rod created.");
      }
    }
}