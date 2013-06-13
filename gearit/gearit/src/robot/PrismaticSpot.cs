﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    class PrismaticSpot : PrismaticJoint, Spot
    {
        private DistanceJoint _distJoint;
        private float _size;
        static private Texture2D _tex = null;

        public PrismaticSpot(Robot robot, Piece p1, Piece p2, Vector2 anchor1, Vector2 anchor2) :
	  base(p1, p2, anchor1, anchor2, new Vector2(1, 1))
        {
            robot.getWorld().AddJoint(this);
            robot.addSpot(this);
            Enabled = true;
            MaxMotorForce = 100;
            MotorSpeed = 0f;
            MotorEnabled = true;
            if (_tex != null)
                _tex = robot.getAsset().CreateCircle(2, Color.Red);
        }

        public void updateLimit()
        {
            _size = ((BodyA.Position + LocalAnchorA) -
                    (BodyB.Position + LocalAnchorB)).Length();
            LowerLimit = _size / 2;
            UpperLimit = _size * 2;
            LimitEnabled = true;
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

        public void draw(SpriteBatch batch)
        {
          
        }
    }
}
