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
using gearit.src;

namespace gearit
{
    abstract class Piece : Body
    {
        internal Shape _shape;
        internal Fixture _fix;
        internal Texture2D _tex;

        internal Piece(Robot robot) :
            base(robot.getWorld())
        {
            BodyType = BodyType.Dynamic;
            ColorValue = Color.Black;
            robot.addPiece(this);
        }

        internal Piece(Robot robot, Shape shape) :
            base(robot.getWorld())
        {
            BodyType = BodyType.Dynamic;
            setShape(shape, robot.getId());
            ColorValue = Color.Black;
            robot.addPiece(this);
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

        public virtual float Weight
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
            Console.WriteLine(p);
            Console.WriteLine(Position);
            Transform t;
            base.GetTransform(out t);
            return (_shape.TestPoint(ref t, ref p));
        }

	//  return the closest spot
        public ISpot getSpot(Vector2 p)
        {
            Joint res = null;
            Vector2 anchorPos;
            float min = 1000000;

            for (JointEdge i = JointList; i != null; i = i.Next)
            {
                if (i.Joint.BodyA == this)
                    anchorPos = i.Joint.WorldAnchorA;
                else
                    anchorPos = i.Joint.WorldAnchorB;
                if (res == null || (p - anchorPos).Length() < min)
                {
                    min = (p - anchorPos).Length();
                    res = i.Joint;
                }
            }
            return ((ISpot)res);
        }

        public void move(Vector2 pos)
        {
            if (Position.X == pos.X && Position.Y == pos.Y)
                return ;
            Position = pos;
            for (JointEdge i = JointList; i != null; i = i.Next)
            {
                if (i.Joint.GetType() == typeof(RevoluteSpot))
                    ((RevoluteSpot)i.Joint).move(this, pos);
                else
                    ((PrismaticSpot)i.Joint).updateAxis();
            }
        }

        public bool areSpotsOk()
        {
            Vector2 anchorPos;

            for (JointEdge i = JointList; i != null; i = i.Next)
            {
                if (i.Joint.BodyA == this)
                    anchorPos = i.Joint.WorldAnchorA;
                else
                    anchorPos = i.Joint.WorldAnchorB;
                if (isOn(anchorPos) == false)
                    return (false);
            }
            return (true);
        }

        public abstract void draw(SpriteBatch batch);

        public abstract void drawLines(DrawGame game);
        public Color ColorValue { get; set; }
    }
}
