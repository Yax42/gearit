using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace gearit
{
  class Heart : Piece
  {
    private Vertices		_vertices; //Le PolygonShape sera composé de ces vertices.

    public	Heart(World world) :
          base(world)
    {
        Console.WriteLine("new heart.");
        _vertices = PolygonTools.CreateRectangle(50f / 2, 50f / 2);
        _shape = new PolygonShape(_vertices, 50f);
        _fix = CreateFixture(_shape, null);
    }
  }
}
