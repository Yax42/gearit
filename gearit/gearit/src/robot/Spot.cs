using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace gearit
{
    class Spot()
    {
        internal Piece _p1;
        internal Piece _p2;

        public Spot(World world, Piece p1, Piece p2)
        {
            _p1 = p1;
            _p2 = p2;
	}

    }
}