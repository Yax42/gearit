using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace gearit.src.robot
{
    class RevoluteSpot : RevoluteJoint, Spot
    {
        private AngleJoint _angleJoint;

        public RevoluteSpot(Robot robot, Piece p1, Piece p2, Vector2 anchor1, Vector2 anchor2) :
	  base(p1, p2, anchor1, anchor2)
        {

            robot.getWorld().AddJoint(this);
            robot.addSpot(this);
            Enabled = true;
            MaxMotorTorque = 100;
            MotorSpeed = 0f;
            MotorEnabled = true;
        }

        public void swap(Piece p1, Piece p2, Vector2 anchor)
        {
            if (BodyA == p1)
            {
                BodyA = p2;
                LocalAnchorA = anchor;
            }
            if (BodyB == p1)
            {
                BodyB = p2;
                LocalAnchorB = anchor;
            }
        }

        public void swap(Piece p1, Piece p2)
        {
            swap(p1, p2, Vector2.Zero);
        }

        public void move(Vector2 pos)
        {
            BodyA.Position = pos - LocalAnchorA;
            BodyB.Position = pos - LocalAnchorB;
        }

        public void draw()
        {
        }
    }
}
