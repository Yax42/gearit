using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace gearit
{
    class Spot : Body
    {
        Piece		_base;
	Vector2		_baseAnchor;

	public Spot(Robot robot, Heart h, Vector2 anchor) :
		base(robot.getWorld())
	{
	  _base = p;
	  _baseAnchor = anchor;
           // CollideConnected = true;
	}

        public void connect(World world, Piece p, Vector2 localAnchor)
        {
            world.AddJoint(new RevoluteJoint(_base, p, _baseAnchor, localAnchor));
        }
    }
}
