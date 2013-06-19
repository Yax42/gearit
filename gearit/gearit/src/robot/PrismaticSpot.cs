using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;
using gearit.src.utility;

namespace gearit.src.robot
{
    class PrismaticSpot : PrismaticJoint, ISpot
    {
        // private DistanceJoint _distJoint;
        private float _size;

        public PrismaticSpot(Robot robot, Piece p1, Piece p2) :
            this(robot, p1, p2, Vector2.Zero, Vector2.Zero)
        {
        }

        public PrismaticSpot(Robot robot, Piece p1, Piece p2, Vector2 anchor1, Vector2 anchor2) :
            base(p1, p2, anchor1, anchor2, new Vector2(1, 1))
        {
            Name = "piston" + robot.revCount();
            updateAxis();
            robot.getWorld().AddJoint(this);
            Enabled = true;
            MaxMotorForce = 100;
            MotorSpeed = 0f;
            MotorEnabled = true;
            _size = ((BodyA.Position + LocalAnchorA) -
                    (BodyB.Position + LocalAnchorB)).Length();
            LimitEnabled = false;
            ColorValue = Color.Black;
            robot.addSpot(this);
        }

        public PrismaticSpot(SerializationInfo info, StreamingContext ctxt)
        {
            _size = (float)info.GetValue("Size", typeof(float));
            updateAxis();
            Enabled = true;
            MaxMotorForce = 100;
            MotorSpeed = 0f;
            MotorEnabled = true;
            LimitEnabled = false;
            ColorValue = Color.Black;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Size", _size, typeof(float));
        }

        public void updateLimit()
        {
            LowerLimit = _size / 2;
            UpperLimit = _size * 2;
            LimitEnabled = true;
        }

        public void updateAxis()
        {
            LocalXAxis1 = ((BodyA.Position + LocalAnchorA) -
                    (BodyB.Position + LocalAnchorB));
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

        public void moveAnchor(Piece p, Vector2 anchor)
        {
            if (BodyA == p)
                LocalAnchorA = anchor;
            if (BodyB == p)
                LocalAnchorB = anchor;
            _size = ((BodyA.Position + LocalAnchorA) -
                    (BodyB.Position + LocalAnchorB)).Length();
        }

        public float getSize()
        {
            return (_size);
        }

        public void draw(DrawGame game)
        {
            if (((Piece)BodyA).Shown == false || ((Piece)BodyB).Shown == false)
                return;
            game.addLine(BodyA.Position + LocalAnchorA, BodyB.Position + LocalAnchorA, ColorValue);
        }

        public float Force
        {
            get { return MotorForce; }
            set { MotorForce = value; }
        }

        public float MaxForce
        {
            get { return MaxMotorForce; }
            set { MaxMotorForce = value; }
        }

        public Color ColorValue { get; set; }
        public string Name { get; set; }
    }
}
