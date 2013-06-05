using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using gearit.src.robot;

namespace gearit
{
    class Rod : Piece
    {
      private Motor	 _motor;
      private float	 _size;

      public	Rod(Robot robot, Spot spot, Vector2 start, float size) : 
          base(robot, new EdgeShape(start, new Vector2(10, 10) + new Vector2(0, size)))
      {
          _size = size;
          spot.connect(robot.getWorld(), this, new Vector2(0, 0));
          Console.WriteLine("Rod created.");
      }
    }
}