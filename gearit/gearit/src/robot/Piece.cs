using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using gearit.src.editor;
using gearit.src.robot;
using gearit.src;

namespace gearit
{
    [Serializable()]
    abstract class Piece : Body, ISerializable
    {
        internal Shape _shape;
        internal Fixture _fix; //punaise
        internal Texture2D _tex;

        internal Piece(Robot robot) :
            base(robot.getWorld())
        {
            if (SerializerHelper.World == null)
                SerializerHelper.World = robot.getWorld();
            BodyType = BodyType.Dynamic;
            ColorValue = Color.Black;
            robot.addPiece(this);
            Shown = true;
            _tex = null;
        }

        internal Piece(Robot robot, Shape shape) :
            base(robot.getWorld())
        {
            if (SerializerHelper.World == null)
                SerializerHelper.World = robot.getWorld();
            BodyType = BodyType.Dynamic;
            setShape(shape, robot.getId());
            ColorValue = Color.Black;
            robot.addPiece(this);
            Shown = true;
        }

        internal Piece(SerializationInfo info) :
            base(SerializerHelper.World)
        {
            BodyType = BodyType.Dynamic;
            ColorValue = Color.Black;
            Shown = true;
            int oldHashMap = (int)info.GetValue("HashCode", typeof(int));
            SerializerHelper.Add(oldHashMap, this);
            Position = (Vector2)info.GetValue("Position", typeof(Vector2));
        }

        abstract public void GetObjectData(SerializationInfo info, StreamingContext ctxt);

        internal void serializePiece(SerializationInfo info)
        {
            info.AddValue("HashCode", this.GetHashCode(), typeof(int));
            info.AddValue("Position", this.Position, typeof(Vector2));
            info.AddValue("Density", _shape.Density, typeof(float));
            info.AddValue("Weight", this.Weight, typeof(float));
        }

        public void setTexture(DrawGame dg, MaterialType mater)
        {
            _tex = dg.textureFromShape(_shape, mater);
        }

        internal void setShape(Shape shape, int id)
        {
            _shape = shape;
            _fix = CreateFixture(_shape, null);
            _fix.CollisionGroup = (short)id;
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
                _fix.Shape.Density = value * _fix.Shape.Density / _fix.Shape.MassData.Mass;
                ResetMassData();
            }
            get { return _fix.Shape.MassData.Mass; } // a check
        }

        public bool isOn(Vector2 p)
        {
            Transform t;
            GetTransform(out t);
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
            if ((int) (Position.X * 1000) == (int) (pos.X * 1000) && (int) (Position.Y * 1000) == (int) (pos.Y * 1000))
                return;
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

        public bool isConnected(Piece other)
        {
            return (getConnection(other) != null);
        }

        public ISpot getConnection(Piece other)
        {
            if (other == this)
                return (null);
            for (JointEdge i = JointList; i != null; i = i.Next)
                if (i.Other == other)
                    return ((ISpot)i.Joint);
            return (null);
        }

        public Color ColorValue { get; set; }
        public bool Shown { get; set; }
        abstract public float getSize();

        public void draw(DrawGame dg)
        {
            dg.draw(this, ColorValue);
            if (_tex != null)
                dg.drawTexture(_tex, _tex.Bounds, Color.White);
        }
    }
}