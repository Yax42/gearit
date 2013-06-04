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

<<<<<<< HEAD
    public	Heart(World world) :
          base(world)
    {
      Console.WriteLine("new heart.");
      _vertices = new Vertices(50);
      _vertices.Add(new Vector2(0, 30));
      _vertices.Add(new Vector2(30, 30));
      _vertices.Add(new Vector2(30, 0));
      _shape = _polygon = new PolygonShape(_vertices, 0); //density 0
      _fix = CreateFixture(_shape, null);
=======
    public Heart(World world):
	base(world)
    {
        _vertices = PolygonTools.CreateRectangle(50f / 2, 50f / 2);
        _shape = new PolygonShape(_vertices, 50f);
        this.CreateFixture(_shape, null);
>>>>>>> a71bec8ee3b926cd9a22cb080cfba020d4845323
    }
  }
}
