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
    [Serializable()]
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
        }

		//
		// SERIALISATION
		//
        public Wheel(SerializationInfo info, StreamingContext ctxt) :
            base(info)
        {
            _size = (float)info.GetValue("Size", typeof(float));
            setShape(new CircleShape(_size, (float)info.GetValue("Density", typeof(float))), Robot._robotIdCounter);
            Weight = (float)info.GetValue("Weight", typeof(float));
        }

        override public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            serializePiece(info);
            info.AddValue("Size", _size, typeof(float));
        }
		//--------- END SERIALISATION

        override public float getSize()
        {
            return (_size);
        }

        public void resetShape()
        {
            ((CircleShape)_shape).Radius = _size;
            DestroyFixture(_fix);
            _fix = CreateFixture(_shape);
        }

        public float Size
        {
            get { return _size; }
            set
            {
                float backup = _size;
                _size = value;
                resetShape();
                if (areSpotsOk() == false)
                {
                    _size = backup;
                    resetShape();
                }
            }
        }
    }
}