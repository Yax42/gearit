﻿using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using gearit.src;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace gearit.src.robot
{
    /// <summary>
    /// Wheel is one of the three kind of Piece
    /// </summary>
	[Serializable()]
	class Wheel : Piece, ISerializable
	{
        float SIZE_MIN = 0.1f;
        float SIZE_MAX = 20;
        private const int _circleSegments = 32;

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
			SetShape(new CircleShape(_size, 1));//, Robot._robotIdCounter);
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

		override public void resetShape()
		{
			SetShape(new CircleShape(_size, Shape.Density));
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

		public override Vector2 ClosestPositionInside(Vector2 p)
		{
			var dir = p - Position;
			if (dir.LengthSquared() <= (_size * _size))
				return p;
			return Position + Vector2.Normalize(dir) * _size;
		}

		public override float DistanceSquared(Vector2 p)
		{
			return Vector2.DistanceSquared(p, Position) - (_size * _size);
		}

		public override bool IsValid()
		{
            return Weight > 0 && getSize() > SIZE_MIN && getSize() < SIZE_MAX;
		}

		public override Vector2 ShapeLocalOrigin()
		{
			return Vector2.Zero;
		}
	}
}