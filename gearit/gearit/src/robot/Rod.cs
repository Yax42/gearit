using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Common;

namespace gearit
{
    class RodSide : Body
    {
        public RodSide(World world)
            : base(world)
        {
           BodyType = BodyType.Dynamic;
        }
    }
    class Rod : Piece
    {
        private float _size;
        private RodSide _side1;
        private RodSide _side2;
        private PrismaticJoint _prisJoint;
        private LineJoint _lineJoint;


        public Rod(Robot robot, Spot spot, Vector2 start, float size) :
            base(robot, new EdgeShape(start, new Vector2(10, 10) + new Vector2(0, size)))
        {
            _side1 = new RodSide(robot.getWorld());
            _side2 = new RodSide(robot.getWorld());
            _lineJoint = new LineJoint(_side1, _side2, new Vector2(0, 0), new Vector2(0, size));
            robot.getWorld().AddJoint(_lineJoint);
            _prisJoint = new PrismaticJoint(_side1, _side2, new Vector2(0, 0), new Vector2(0, size), Vector2.Zero);
            robot.getWorld().AddJoint(_prisJoint);
            Size = size;
            spot.connect(robot.getWorld(), this, true);
            Console.WriteLine("Rod created.");
        }

        public RodSide getSide1()
        {
            return (_side1);
        }

        public RodSide getSide2()
        {
            return (_side2);
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

        public void update()
        {
        }

        private float distance(RodSide side, Vector2 p)
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