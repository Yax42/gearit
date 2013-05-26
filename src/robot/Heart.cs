using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace gearit
{
  class Heart : Piece
  {
    private Vertices		_vertices; //Le PolygonShape sera composé de ces vertices.

    public Heart(World world):
	base(world)
    {
        _vertices = PolygonTools.CreateRectangle(50f / 2, 50f / 2);
        _shape = new PolygonShape(_vertices, 50f);
        this.CreateFixture(_shape, null);
    }
  }
}
