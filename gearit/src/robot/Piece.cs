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

	internal Piece(Robot robot) :
		base(robot.getWorld())
	 {
            BodyType = BodyType.Dynamic;
	    robot.addPiece(this);
	 }

	internal Piece(Robot robot, Shape shape) :
		base(robot.getWorld())
	 {
            BodyType = BodyType.Dynamic;
	    robot.addPiece(this);
	    setShape(shape, robot.getId());
	 }

	internal void	setShape(Shape shape, int id)
	{
            _shape = shape;
            _fix = CreateFixture(_shape, null);
            _fix.CollisionGroup = (short) id;
	}

	internal void	initShapeAndFixture(Shape shape)
	{
            _shape = shape;
            _fix = CreateFixture(_shape, null);
	}
    }
}
