﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Joints;
using gearit.src.utility;

namespace gearit.src.robot
{
	[Serializable()]
	class Rod : Piece, ISerializable
	{

		private const float _width = 0.02f;

		private Vector2 _endA;
		private Vector2 _endB;

		public Rod(Robot robot, float size) :
			this(robot, size, Vector2.Zero)
		{
		}

		public Rod(Robot robot, float size, Vector2 pos) :
			base(robot, new PolygonShape(PolygonTools.CreateRectangle(size, _width), 1f)) //density ~= poids
		{
			Position = pos;
			_size = size;
			//_tex = robot.getAsset().TextureFromShape(_shape, MaterialType.Blank, Color.White, 1f);
		}

		//
		// SERIALISATION
		//
		public Rod(SerializationInfo info, StreamingContext ctxt) :
			base(info)
		{
			_size = (float)info.GetValue("Size", typeof(float));
			setShape(new PolygonShape(PolygonTools.CreateRectangle(_size, _width), 1), Robot._robotIdCounter);
			Weight = (float)info.GetValue("Weight", typeof(float));
		}

		override public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			serializePiece(info);
			info.AddValue("Size", _size, typeof(float));
		}
		//--------- END SERIALISATION

		public bool getSide(Vector2 pos)
		{
			Transform xf;
			GetTransform(out xf);
			Vector2 v1 = ((PolygonShape) Shape).Vertices[0];
			Vector2 v2 = ((PolygonShape) Shape).Vertices[2];

			return ((MathUtils.Multiply(ref xf, v1) - pos).Length() > (MathUtils.Multiply(ref xf, v1) - pos).Length());

		}

		override public void resetShape()
		{
			if (_size < 0.01)
				_size = 0.01f;
			Shape shape = new PolygonShape(PolygonTools.CreateRectangle(_size, _width), Shape.Density);
			DestroyFixture(_fix);
			_fix = CreateFixture(shape);
		}

		virtual public void updateCharacteristics()
		{
			GenerateEnds();
		}

		override public float getSize()
		{
			return (_size);
		}

		public float Size
		{
			get { return _size; }
			set
			{
				float backup = _size;
				_size = value;
				resetShape();

				return;
				if (areSpotsOk() == false)
				{
					_size = backup;
					resetShape();
				}
			}
		}
		
		//--------ENDS-------------

		private void updateEnds(Piece comparator = null)
		{
			rotate(endsAngle(), comparator);
			_robot.resetAct();

			resize(endsSize());
			_robot.resetAct();

			move(endsPosition());
			_robot.resetAct();
		}

		public void setEnd(Vector2 end, bool isA, Piece comparator = null)
		{
			double previous_angle_degree = MathLib.RadiansToDegrees(endsAngle());
			if (isA)
			{
				if (_endB == end)
					return;
				_endA = end;
			}
			else
			{
				if (_endA == end)
					return;
				_endB = end;
			}
			updateEnds(comparator);
		}

		public Vector2 getEnd(bool isA)
		{
			if (isA)
				return _endA;
			else
				return _endB;
		}

		private Vector2 endsPosition()
		{
			return (_endA + _endB) / 2;
		}

		private float endsSize()
		{
			return (_endB - _endA).Length() / 2;
		}

		private float endsAngle()
		{
			return (MathLib.Angle(_endB - _endA));
		}

		public bool CloseEnd(Vector2 pos)
		{
			return (pos - _endA).LengthSquared() < (pos - _endB).LengthSquared();
		}
		public void GenerateEnds()
		{
			Vector2 semiEnd = MathLib.PolarCoor(_size, Rotation);
			_endA = Position - semiEnd;
			_endB = Position + semiEnd;
		}

		public override Vector2 ShapeLocalOrigin()
		{
			return new Vector2(_size, 0);
		}
	}
}