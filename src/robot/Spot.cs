using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;

namespace gearit
{
    class Spot : RevoluteJoint
    {
	public Spot(Piece p1, Piece p2, Vector2 localAnchorA, Vector2 localAnchorB) :
          base(p1, p2, localAnchorA, localAnchorB)
	{
            CollideConnected = true;
	}
    }
}
