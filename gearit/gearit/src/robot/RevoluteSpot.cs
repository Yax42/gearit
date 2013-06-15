using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using FarseerPhysics.Collision.Shapes;

namespace gearit.src.robot
{
    class RevoluteSpot : RevoluteJoint, ISpot
    {
	private static float _spotSize = 0.2f;
        private static Vector2 _topLeft = new Vector2(-_spotSize, -_spotSize);
        private static Vector2 _topRight = new Vector2(_spotSize, -_spotSize);
        private static Vector2 _botLeft = new Vector2(-_spotSize, _spotSize);
        private static Vector2 _botRight = new Vector2(_spotSize, _spotSize);

        private AngleJoint _angleJoint;
        static private Texture2D _tex = null;

        public RevoluteSpot(Robot robot, Piece p1, Piece p2) :
            this(robot, p1, p2, Vector2.Zero, Vector2.Zero)
        {
        }

        public RevoluteSpot(Robot robot, Piece p1, Piece p2, Vector2 anchor1, Vector2 anchor2) :
	  base(p1, p2, anchor1, anchor2)
        {
            robot.getWorld().AddJoint(this);
            robot.addSpot(this);
            Enabled = true;
            MaxMotorTorque = 100;
            MotorSpeed = 0f;
            MotorEnabled = true;
            if (_tex != null)
                _tex = robot.getAsset().CreateCircle(2, Color.Red);
            ColorValue = Color.Yellow;
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
        }

        public void move(Vector2 pos)
        {
            BodyA.Position = pos - LocalAnchorA;
            BodyB.Position = pos - LocalAnchorB;
        }

        public float getSize() { return (0); }

        public void vertices(VertexPositionColor[] vertices, ref int count)
        {
	    vertices[count++] = new VertexPositionColor(new Vector3(BodyA.Position + _topLeft, 0f), ColorValue);
	    vertices[count++] = new VertexPositionColor(new Vector3(BodyA.Position + _topRight, 0f), ColorValue);

	    vertices[count++] = new VertexPositionColor(new Vector3(BodyA.Position + _topRight, 0f), ColorValue);
	    vertices[count++] = new VertexPositionColor(new Vector3(BodyA.Position + _botRight, 0f), ColorValue);

	    vertices[count++] = new VertexPositionColor(new Vector3(BodyA.Position + _topRight, 0f), ColorValue);
	    vertices[count++] = new VertexPositionColor(new Vector3(BodyA.Position + _botRight, 0f), ColorValue);

	    vertices[count++] = new VertexPositionColor(new Vector3(BodyA.Position + _botRight, 0f), ColorValue);
	    vertices[count++] = new VertexPositionColor(new Vector3(BodyA.Position + _botLeft, 0f), ColorValue);

	    vertices[count++] = new VertexPositionColor(new Vector3(BodyA.Position + _botLeft, 0f), ColorValue);
	    vertices[count++] = new VertexPositionColor(new Vector3(BodyA.Position + _topLeft, 0f), ColorValue);
        }

        public Color ColorValue { get; set; }

        public float MotorStrength
        {
            get { return MotorStrength; }
            set { MotorStrength = value; }
        }
    }
}
