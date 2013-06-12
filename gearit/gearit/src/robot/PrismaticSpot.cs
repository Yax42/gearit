using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Joints;

namespace gearit.src.robot
{
    class PrismaticSpot
    {
        private PrismaticJoint _prismJoint;
        private DistanceJoint _distJoint;

        public PrismaticSpot(World world, Piece p1, Piece p2, Vector2 anchor1, Vector2 anchor2)
        {
            _prismJoint = new PrismaticJoint(p1, p2, anchor1, anchor2, new Vector2(1, 1));
            world.AddJoint(_prismJoint);
            _prismJoint.Enabled = true;
            _prismJoint.MaxMotorForce = 100;
            _prismJoint.MotorSpeed = 0f;
            _prismJoint.LimitEnabled = true;
            _prismJoint.MotorEnabled = true;
            //if (base.JointList.GetType() == typeof(PrismaticJoint))
	    /*
            joint.LowerLimit = 1;
            joint.UpperLimit = _sizeRod1 * 3;
	    */
        }
    }
}
