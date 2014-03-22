using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;
using gearit.src.utility;
using gearit.src.editor;
using FarseerPhysics.Common;

namespace gearit.src.robot
{
	[Serializable()]
	class PrismaticSpot : PrismaticJoint, ISpot, ISerializable
	{
		// private DistanceJoint _distJoint;
		private float _size;
		private Joint _joint;
		private CommonSpot _common;

		public Joint Joint { get { return _joint; } }

		public PrismaticSpot(Robot robot, Piece p1, Piece p2) :
			this(robot, p1, p2, Vector2.Zero, Vector2.Zero)
		{
		}

		public PrismaticSpot(Robot robot, Piece p1, Piece p2, Vector2 anchor1, Vector2 anchor2) :
			base(p1, p2, anchor1, anchor2, new Vector2(1, 1))
		{
			Name = "spot" + robot.revCount();
			updateAxis();
			robot.getWorld().AddJoint(this);
			Enabled = true;
			MaxMotorForce = 10;
			MotorSpeed = 0f;
			MotorEnabled = true;
			LimitEnabled = false;
			ColorValue = Color.Black;
			robot.addSpot(this);
			_joint = (Joint)this;
			_common = new CommonSpot(this);
		}

		public void wakeUp(Robot robot)
		{
			_common.wakeUp(robot);
		}

		public void fallAsleep(Robot robot, bool toForget = false)
		{
			_common.fallAsleep(robot, toForget);
		}

		//
		// SERIALISATION
		//
		public PrismaticSpot(SerializationInfo info, StreamingContext ctxt) :
			base(SerializerHelper.Ptrmap[(int)info.GetValue("PAHashCode", typeof(int))],
		SerializerHelper.Ptrmap[(int)info.GetValue("PBHashCode", typeof(int))],
		(Vector2)info.GetValue("AnchorA", typeof(Vector2)),
		(Vector2)info.GetValue("AnchorB", typeof(Vector2)), new Vector2(1, 1))
		{
			Name = (string)info.GetValue("Name", typeof(string));
			_size = (float)info.GetValue("Size", typeof(float));
			updateAxis();
			SerializerHelper.World.AddJoint(this);
			Enabled = true;
			MaxMotorForce = (float)info.GetValue("Force", typeof(float));
			MotorSpeed = 0f;
			MotorEnabled = true;
			LimitEnabled = false;
			ColorValue = Color.Black;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("Size", _size, typeof(float));
			info.AddValue("PAHashCode", BodyA.GetHashCode(), typeof(int));
			info.AddValue("PBHashCode", BodyB.GetHashCode(), typeof(int));
			info.AddValue("Name", Name, typeof(string));
			info.AddValue("Force", MaxMotorForce, typeof(float));
			info.AddValue("AnchorA", LocalAnchorA, typeof(Vector2));
			info.AddValue("AnchorB", LocalAnchorB, typeof(Vector2));
		}
		//--------- END SERIALISATION

		public void updateLimit()
		{
			_size = (WorldAnchorA - WorldAnchorB).Length();
			LowerLimit = -_size * 2;
			UpperLimit = -_size / 2;
			LimitEnabled = true;
		}

		public void updateAxis()
		{
			LocalXAxis1 = WorldAnchorA - WorldAnchorB;
			LocalXAxis1 /= LocalXAxis1.Length();
			_size = (WorldAnchorA - WorldAnchorB).Length();
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

		public float getSize()
		{
			return (_size);
		}

		public void draw(DrawGame game)
		{
			if (((Piece)BodyA).Shown == false || ((Piece)BodyB).Shown == false)
				return;
			game.drawLine(BodyA.Position + LocalAnchorA, BodyB.Position + LocalAnchorB, ColorValue);
		}

		public float Force
		{
			get { return MotorForce; }
			set { MotorForce = value; }
		}

		public float MaxForce
		{
			get { return MaxMotorForce; }
			set { MaxMotorForce = value; }
		}
//ROTATE--------------------------
		public void rotate(Piece piece, float angle)
		{
			if (piece == BodyA)
			{
				((Piece)BodyB).moveDelta(MathLib.RotateDelta(WorldAnchorA, WorldAnchorB, angle) - WorldAnchorB);
				((Piece)BodyB).rotateDelta(angle);
			}
			else
			{
				((Piece)BodyA).moveDelta(MathLib.RotateDelta(WorldAnchorB, WorldAnchorA, angle) - WorldAnchorA);
				((Piece)BodyA).rotateDelta(angle);
			}
			updateAxis();
		}
		public void rotateNoRepercussion(Piece piece, float angle)
		{
			if (piece == BodyA)
				LocalAnchorA = MathLib.RotatePoint(LocalAnchorA, Vector2.Zero, MathLib.RadiansToDegrees(angle));
			else
				LocalAnchorB = MathLib.RotatePoint(LocalAnchorB, Vector2.Zero, MathLib.RadiansToDegrees(angle));
		}
//MOVE--------------------------
		public void moveAnchor(Piece p, Vector2 pos)
		{
			if (BodyA == p)
				LocalAnchorA = pos - p.Position;
			else if (BodyB == p)
				LocalAnchorB = pos - p.Position;
			updateAxis();
		}

		public void move(Piece piece, Vector2 pos)
		{
			moveAnchor(piece, pos);
		}

		public void moveRigid(Piece piece, Vector2 pos)
		{
			if (BodyA == piece)
				moveRigidDelta(piece, pos - LocalAnchorA);
			else if (BodyB == piece)
				moveRigidDelta(piece, pos - LocalAnchorB);
		}

		public void moveRigidDelta(Piece piece, Vector2 offset)
		{
			if (BodyA == piece)
			{
				LocalAnchorA += offset;
				((Piece)BodyB).moveDelta(offset);
			}
			else if (BodyB == piece)
			{
				LocalAnchorB += offset;
				((Piece)BodyA).moveDelta(offset);
			}
			updateAxis();
		}

		public Vector2 getLocalAnchor(Piece piece)
		{
			if (BodyA == piece)
				return LocalAnchorA;
			else
				return LocalAnchorB;
		}

		public Vector2 getWorldAnchor(Piece piece)
		{
			if (BodyA == piece)
				return WorldAnchorA;
			else
				return WorldAnchorB;
		}

		public void moveLocal(Piece p, Vector2 pos)
		{
			if (BodyA == p)
				LocalAnchorA = pos;
			else if (BodyB == p)
				LocalAnchorB = pos;
			updateAxis();
		}

		public Color ColorValue { get; set; }
		public string Name { get; set; }
	}
}
