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
			GenerateEnds();
			//_tex = robot.getAsset().TextureFromShape(_shape, MaterialType.Blank, Color.White, 1f);
		}

		//
		// SERIALISATION
		//
		public Rod(SerializationInfo info, StreamingContext ctxt) :
			base(info)
		{
			_size = (float)info.GetValue("Size", typeof(float));
			Rotation = (float)info.GetValue("Rotation", typeof(float));
			setShape(new PolygonShape(PolygonTools.CreateRectangle(_size, _width), (float)info.GetValue("Density", typeof(float))), Robot._robotIdCounter);
			Weight = (float)info.GetValue("Weight", typeof(float));
		}

		override public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			serializePiece(info);
			info.AddValue("Size", _size, typeof(float));
			info.AddValue("Rotation", Rotation, typeof(float));
		}
		//--------- END SERIALISATION

		public bool getSide(Vector2 pos)
		{
			Transform xf;
			GetTransform(out xf);
			Vector2 v1 = ((PolygonShape)_shape).Vertices[0];
			Vector2 v2 = ((PolygonShape)_shape).Vertices[2];

			return ((MathUtils.Multiply(ref xf, v1) - pos).Length() > (MathUtils.Multiply(ref xf, v1) - pos).Length());

		}

		override public void resetShape()
		{
			_shape = new PolygonShape(PolygonTools.CreateRectangle(_size, _width), _shape.Density);
			DestroyFixture(_fix);
			_fix = CreateFixture(_shape);
		}

		static Vector2 RotatePoint(Vector2 pointToRotate, Vector2 centerPoint, double angleInDegrees)
		{
			double angleInRadians = angleInDegrees * (Math.PI / 180);
			double cosTheta = Math.Cos(angleInRadians);
			double sinTheta = Math.Sin(angleInRadians);
			return new Vector2
			{
				X =
					(float)
					(cosTheta * (pointToRotate.X - centerPoint.X) -
					sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
				Y =
					(float)
					(sinTheta * (pointToRotate.X - centerPoint.X) +
					cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
			};
		}

		public void setPos2(Vector2 pos, bool side)
		{
//            Console.WriteLine("angle: {0}", MathLib.AngleFromUp(new Vector2(1, 0)));

			Vector2 dif = Position - pos;
			float angle = (float)Math.Atan2(dif.X, -dif.Y);

			double oldAngle = MathLib.RadiansToDegrees(Rotation);
			Rotation = angle + (float)Math.PI / 2;
			double diffAngle = MathLib.RadiansToDegrees(Rotation) - oldAngle;

			if (JointList.Joint.WorldAnchorA != null)
			{
//				Position = JointList.Joint.WorldAnchorA;
				Position = RotatePoint(Position, JointList.Joint.WorldAnchorA, diffAngle);
				
			}
			return;
			/*
					if (areSpotsOk() == false)
						Rotation = oldAngle;
			*/

			Vector2 anchor;

			for (JointEdge i = JointList; i != null; i = i.Next)
			{
				if (i.Joint.BodyA == this)
					anchor = i.Joint.WorldAnchorA;
				else
					anchor = i.Joint.WorldAnchorB;
				anchor -= Position;

				((ISpot)i.Joint).moveAnchor(this,
				  (new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle))) * anchor.Length() + Position);
			}

			//Position = absMainPos + (new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle)) * _size);
		}
		/*
		Transform xf;
		GetTransform(out xf);
		Vector2 relMainPos = ((PolygonShape)_shape).Vertices[(side ? 0 : 2)];
		Vector2 absMainPos = MathUtils.Multiply(ref xf, relMainPos);

		Vector2 dif = absMainPos - pos;
		float angle = (float)Math.Atan2(dif.X, -dif.Y) * (side ? 1 : -1);
		float newSize = dif.Length();

		_shape = new PolygonShape(PolygonTools.CreateRectangle(newSize, _width), _shape.Density);

		if (areSpotsOk() == false)
			_shape = new PolygonShape(PolygonTools.CreateRectangle(_size, _width), _shape.Density);
		else
		{
			_size = newSize;
			DestroyFixture(_fix);
			_fix = CreateFixture(_shape);
			//Rotation = angle;
			//Position = absMainPos + (new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle)) * _size);
		}
	*/

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

		private void updateEnds()
		{
			//float newSize = endsSize();
			//float scale = newSize / _size;

			move(endsPosition());
			_robot.resetAct();
			rotate(endsAngle());
			_robot.resetAct();
			resize(endsSize());
			_robot.resetAct();
		}

		public void setEnds(Vector2 A, Vector2 B)
		{
			_endA = A;
			_endB = B;
			updateEnds();
		}

		public void setEnd(Vector2 end, bool isA)
		{
			if (isA)
				_endA = end;
			else
				_endB = end;
			updateEnds();
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
		
		private void GenerateEnds()
		{
			Vector2 semiEnd = MathLib.PolarCoor(_size / 2, Rotation);
			_endA = Position - semiEnd;
			_endB = Position + semiEnd;
		}
	}
}