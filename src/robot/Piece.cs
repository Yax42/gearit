using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;

namespace gearit
{
    class Piece : Body
    {
	Shape	    _shape;
	Fixture	    _fix;

	internal Piece(World world, Shape shape) :
		base(world)
	 {
            BodyType = BodyType.Dynamic;
            _shape = shape;
            _fix = CreateFixture(_shape, null);
	 }
    }
}
