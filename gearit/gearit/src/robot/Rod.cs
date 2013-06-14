/*
using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Common;

namespace gearit
{
    class Rod
    {
        private float _size;
        private float _strength;
        private Piece _start;
        private Piece _end;
        private PrismaticJoint _prisJoint;
        //private LineJoint _lineJoint;


        public Rod(Robot robot, Piece p1, Piece p2, Vector2 anchor1, Vector2 anchor2)
        {
            //_lineJoint = new LineJoint(_side1, _side2, new Vector2(0, 0), new Vector2(0, size));
            //robot.getWorld().AddJoint(_lineJoint);
            _prisJoint = new PrismaticJoint(p1, p2, anchor1, anchor2, Vector2.Zero);
            _prisJoint.LimitEnabled = true;
            _prisJoint.Enabled = true;
            robot.getWorld().AddJoint(_prisJoint);
            Strength = 0;
            //Size = size;
            //spot.connect(robot.getWorld(), this, true);
            Console.WriteLine("Rod created.");
        }

        public Piece getPiece(int v)
        {
	    if (v == 0)
	      return (_start);
            return (_end);
        }

        public float Size
        {
            get { return _size; }
            set
            {
                _size = value;
                if (_size < 3)
                    _size = 3;
                if (_size > 300)
                    _size = 300;
                _prisJoint.LowerLimit = _size / 3;
                _prisJoint.UpperLimit = _size * 3;
            }
        }

        public float Strength
        {
            get { return _strength; }
            set
            {
                _strength = value;
                if (_strength < 0)
                    _strength = 0;
                if (_strength > 300)
                    _strength = 300;
                _prisJoint.MotorEnabled = (_strength != 0);
                _prisJoint.MaxMotorForce = _strength;
                _prisJoint.MotorSpeed = 0f;
            }
        }

    /*	
        public void refreshShape()
        {
            EdgeShape s = (EdgeShape)_shape;
            s.Vertex1 = _side1.Position;
            s.Vertex2 = _side2.Position;
            _shape = s;
        }

        private float distance(, Vector2 p)
        {
            Vector2 p2;

            p2 = side.Position;
            p2 -= p;
            return (p2.Length());
        }

        public override Vector2 getSpotPos(Vector2 p)
        {
            if (distance(_side1, p) > distance(_side2, p))
                return (_side2.Position);
            else
                return (_side1.Position);
        }

        public override bool isOn(Vector2 p)
        {
            Transform t;
            base.GetTransform(out t);
            return (base._shape.TestPoint(ref t, ref p));
        }

        public override bool isRod()
        {
            return (true);
        }
    }
}
*/