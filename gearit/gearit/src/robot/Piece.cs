﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using gearit.src.robot;

namespace gearit
{
    abstract class Piece : Body, IPrintableVertices
    {
        internal Shape _shape;
        internal Fixture _fix;
        internal Texture2D _tex;

        internal Piece(Robot robot) :
            base(robot.getWorld())
        {
            BodyType = BodyType.Dynamic;
            robot.addPiece(this);
            ColorValue = Color.Green;
        }

        internal Piece(Robot robot, Shape shape) :
            base(robot.getWorld())
        {
            BodyType = BodyType.Dynamic;
            robot.addPiece(this);
            setShape(shape, robot.getId());
            ColorValue = Color.Green;
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

        public bool isOn(Vector2 p)
        {
            Transform t;
            base.GetTransform(out t);
            return (_shape.TestPoint(ref t, ref p));
        }

	//  return the closest spot
        public ISpot getSpot(Vector2 p)
        {
	   // Spot res = null;
	    //for (int i = 0; i <	JointList.
            return (null);
        }

        public abstract void draw(SpriteBatch batch);

        public abstract void vertices(VertexPositionColor[] vertices, ref int count);
        public Color ColorValue { get; set; }
    }
}
