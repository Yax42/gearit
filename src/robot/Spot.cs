using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using gearit.src.robot;

namespace gearit
{
    class Spot
    {
        Piece		_base;
	Vector2		_baseAnchor;

	public Spot(Piece p, Vector2 anchor)
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
