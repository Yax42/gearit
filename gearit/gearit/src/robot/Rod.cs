using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Joints;

namespace gearit.src.robot
{
	[Serializable()]
	class Rod : Piece, ISerializable
	{

		private const float _width = 0.02f;

		private float _size;

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

		public void resetShape()
		{
			_shape = new PolygonShape(PolygonTools.CreateRectangle(_size, _width), _shape.Density);
			DestroyFixture(_fix);
			_fix = CreateFixture(_shape);
		}
		/// <summary>
		/// Converts an angle in decimal degress to radians.
		/// </summary>
		static double DegreesToRadians(double angleInDegrees)
		{
			return angleInDegrees * (Math.PI / 180);
		}

		static double RadiansToDegrees(double angleInRadian)
		{
			return angleInRadian * (180 / Math.PI);
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
			
			Vector2 dif = Position - pos;
			float angle = (float)Math.Atan2(dif.X, -dif.Y);

			double oldAngle = RadiansToDegrees(Rotation);
			Rotation = angle + (float)Math.PI / 2;
			double diffAngle = RadiansToDegrees(Rotation) - oldAngle;

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

				if (areSpotsOk() == false)
				{
					_size = backup;
					resetShape();
				}
			}
		}
	}
}