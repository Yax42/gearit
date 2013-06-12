using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Joints;

namespace gearit
{
    class Piece : Body
    {
        internal Shape _shape;
        internal Fixture _fix;

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

        internal void setShape(Shape shape, int id)
        {
            _shape = shape;
            _fix = CreateFixture(_shape, null);
            _fix.CollisionGroup = (short) id;
        }

        internal void initShapeAndFixture(Shape shape)
        {
            _shape = shape;
            _fix = CreateFixture(_shape, null);
        }

        public float Weight
        {
            set
            {
                FixtureList[0].Shape.Density = value;
                ResetMassData();
            }
            get { return FixtureList[0].Shape.Density; }
        }

        public virtual Vector2 getSpotPos(Vector2 p)
        {
            Transform t;
            base.GetTransform(out t);
            if (_shape.TestPoint(ref t, ref p))
                return (p);
            else
                return (new Vector2(-1, -1));
        }

        public virtual bool isOn(Vector2 p)
        {

            return (getSpotPos(p) != new Vector2(-1, -1));
        }


        public void revoluteLink(World world, Piece p, Vector2 anchor1, Vector2 anchor2)
        {
            RevoluteJoint joint;
            joint = new RevoluteJoint(this, p, anchor1, anchor2);
            world.AddJoint(joint);
            joint.Enabled = true;
            joint.MaxMotorTorque = 100;
            joint.MotorSpeed = 0f;
            joint.LimitEnabled = true;
            joint.MotorEnabled = true;
        }
	
    }
}
