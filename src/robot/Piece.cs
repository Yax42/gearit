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
	internal Shape	    	    _shape;
	internal Fixture	    _fix;

	internal Piece(World world) :
		base(world)
	 {
            BodyType = BodyType.Dynamic;
	 }

	internal Piece(World world, Shape shape) :
		base(world)
	 {
            BodyType = BodyType.Dynamic;
            _shape = shape;
            _fix = CreateFixture(_shape, null);
	 }

	internal void initShapeAndFixture(Shape shape)
	{
            _shape = shape;
            _fix = CreateFixture(_shape, null);
	}
    }
}
