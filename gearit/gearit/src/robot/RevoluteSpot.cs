using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace gearit.src.robot
{
    class RevoluteSpot : Spot
    {
        private RevoluteJoint _revJoint;
        private AngleJoint _angleJoint;

        public RevoluteSpot(World world, Piece p1, Piece p2, Vector2 anchor1, Vector2 anchor2) :
	    base(p1, p2)
        {
            _revJoint = new RevoluteJoint(p1, p2, anchor1, anchor2);
            world.AddJoint(_revJoint);
            _revJoint.Enabled = true;
            _revJoint.MaxMotorTorque = 100;
            _revJoint.MotorSpeed = 0f;
            _revJoint.LimitEnabled = true;
            _revJoint.MotorEnabled = true;
            //if (base.JointList.GetType() == typeof(PrismaticJoint))
	    /*
            joint.LowerLimit = 1;
            joint.UpperLimit = _sizeRod1 * 3;
	    */
        }

        public void publicSwap(Piece p1, Piece p2, Vector2 anchor2)
        {
	    Piece   p;
	    if (p1 == p2)
	      p
        }
    }
}
