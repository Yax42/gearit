using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace gearit
{
    class Spot
    {
	private Piece		    _main;

	public Spot(Robot robot, Piece p)
	{
	  _main = p;
	}

        public void connect(World world, Wheel p, Vector2 localAnchor)
        {
            world.AddJoint(new RevoluteJoint(_main, p, Vector2.Zero, localAnchor));
        }

        public void connect(World world, Heart p, Vector2 localAnchor)
        {
            world.AddJoint(new RevoluteJoint(_main, p, Vector2.Zero, localAnchor));
        }

        public void connect(World world, Rod r, bool isSide1)
        {
            world.AddJoint(new RevoluteJoint(_main, (isSide1 ? r.getSide1() : r.getSide2()), Vector2.Zero, Vector2.Zero));
        }

        public void connect(World world, RodSide side)
        {
            world.AddJoint(new RevoluteJoint(_main, side, Vector2.Zero, Vector2.Zero));
        }

        public void merge(Spot other)
        {
        }

        public void split()
        {
        }

    }
}
