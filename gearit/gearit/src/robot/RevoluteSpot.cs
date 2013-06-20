﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
        private static float _spotSize = 0.05f;
        private static Vector2 _topLeft = new Vector2(-_spotSize, -_spotSize);
        private static Vector2 _topRight = new Vector2(_spotSize, -_spotSize);
        private static Vector2 _botLeft = new Vector2(-_spotSize, _spotSize);
        private static Vector2 _botRight = new Vector2(_spotSize, _spotSize);

        // private AngleJoint _angleJoint;
        static private Texture2D _tex;

        public RevoluteSpot(Robot robot, Piece p1, Piece p2) :
            this(robot, p1, p2, Vector2.Zero, Vector2.Zero)
        {
        }

        public RevoluteSpot(Robot robot, Piece p1, Piece p2, Vector2 anchor1, Vector2 anchor2) :
            base(p1, p2, anchor1, anchor2)
        {
            Name = "spot" + robot.revCount();
            robot.getWorld().AddJoint(this);
            Enabled = true;
            MaxMotorTorque = 100;
            MotorSpeed = 0f;
            MotorEnabled = true;
            ColorValue = Color.Black;
            robot.addSpot(this);
            move(p1, p1.Position);
        }

        public RevoluteSpot(SerializationInfo info, StreamingContext ctxt)
        {
            Enabled = true;
            MaxMotorTorque = 100;
            MotorSpeed = 0f;
            MotorEnabled = true;
            ColorValue = Color.Black;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
        }

        public static void initTex(AssetCreator asset)
        {
            _tex = asset.CreateCircle(2, Color.Red);
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
                LocalAnchorA = anchor - p.Position;
            if (BodyB == p)
                LocalAnchorB = anchor - p.Position;
            move(p, p.Position);
        }

	/*
        public void move(Vector2 pos)
        {
            //BodyA.Position = pos - LocalAnchorA;
            //BodyB.Position = pos - LocalAnchorB;
            ((Piece)BodyA).move(pos - LocalAnchorA);
            ((Piece)BodyB).move(pos - LocalAnchorB);
        }
	*/

        public void move(Piece piece, Vector2 pos)
        {
            if (piece == BodyA)
                ((Piece)BodyB).move(pos + LocalAnchorA - LocalAnchorB);
            else
                ((Piece)BodyA).move(pos + LocalAnchorB - LocalAnchorA);
        }

        public void draw(DrawGame game)
        {
            if (((Piece)BodyA).Shown == false || ((Piece)BodyB).Shown == false)
                return;
            Vector2 pos = BodyA.Position + LocalAnchorA;
            Vector2 corner = BodyA.Position + LocalAnchorA - _topLeft;
            //Vector2 corner2 = BodyA.Position + LocalAnchorA + _botRight;

            //game.Batch().Draw(_tex, new Rectangle((int)corner.X, (int)corner.Y, (int)_spotSize * 2, (int)_spotSize * 2), ColorValue);
            game.addLine(pos + _topLeft, pos + _topRight, ColorValue);
            game.addLine(pos + _topRight, pos + _botRight, ColorValue);
            game.addLine(pos + _botRight, pos + _botLeft, ColorValue);
            game.addLine(pos + _botLeft, pos + _topLeft, ColorValue);
        }


        public float Force
        {
            get { return MotorTorque; }
            set { MotorTorque = value; }
        }

        public float MaxForce
        {
            get { return MaxMotorTorque; }
            set { MaxMotorTorque = value; }
        }

        public string Name { get; set; }
        public Color ColorValue { get; set; }
    }
}
