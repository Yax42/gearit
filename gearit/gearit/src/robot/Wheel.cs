using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using gearit.src;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace gearit
{
    class Wheel : Piece, ISerializable
    {
        private const int _circleSegments = 32;

        private float _size;

        public Wheel(Robot robot, float size) :
            this(robot, size, Vector2.Zero)
        {
        }

        public Wheel(Robot robot, float size, Vector2 pos) :
            base(robot, new CircleShape(size, 1f)) //density ~= poids
        {
            Position = pos;
            _size = size;
            //_tex = robot.getAsset().TextureFromShape(_shape, MaterialType.Blank, Color.White, 1f);
            Console.WriteLine("Wheel created.");
        }

        public Wheel(SerializationInfo info, StreamingContext ctxt) :
            base(info, ctxt)
        {
            Position = Vector2.Zero;
            _size = (float)info.GetValue("Size", typeof(float));
            _shape = new CircleShape(_size, 1f);
            setShape(_shape, Robot._robotIdCounter);
            Console.WriteLine("Wheel created.");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            base.GetObjectData(info, ctxt);
            info.AddValue("Size", _size, typeof(float));
        }

        public float Size
        {
            get { return _size; }
            set
            {
                ((CircleShape)_shape).Radius = value;
                if (areSpotsOk() == false)
                    ((CircleShape)_shape).Radius = _size;
                else
                {
                    _size = value;
                    DestroyFixture(_fix);
                    _fix = CreateFixture(_shape);
                }
            }
        }
    }
}