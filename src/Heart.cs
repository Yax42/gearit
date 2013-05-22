using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace gearit
{
  class Heart
  {
    private PolygonShape	_polygon;
    private Vertices		_vertices; //Le PolygonShape sera composé de ces vertices.

    public Heart()
    {
      Console.WriteLine("new heart.");
      _vertices = new Vertices(50);
      _vertices.Add(new Vector2(0, 30));
      _vertices.Add(new Vector2(30, 30));
      _vertices.Add(new Vector2(30, 0));
      _polygon = new PolygonShape(_vertices, 0);
    }
  }
}
