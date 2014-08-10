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
using System.Diagnostics;

namespace gearit.src.robot
{
	[Serializable()]
	public abstract class Piece : Body, ISerializable
	{
		internal const float MaxMass = 1000;

		internal Shape Shape
		{
			get
			{
				return _fix.Shape;
			}
		}
		internal Fixture _fix; //punaise |---
		internal Texture2D _tex;
		internal bool DidAct { get; set; }
		internal Robot _robot;
		internal float _size; //useless for the heart, but more simple for implementation
		public bool Sleeping { get; set; }

		internal Piece(Robot robot) :
			base(robot.getWorld())
		{
			if (SerializerHelper.World == null)
				SerializerHelper.World = robot.getWorld();
			BodyType = BodyType.Dynamic;
			ColorValue = Color.ForestGreen;
			robot.addPiece(this);
			_robot = robot;
			Shown = true;
			_tex = null;
			Sleeping = false;
		}

		internal Piece(Robot robot, Shape shape) :
			base(robot.getWorld())
		{
			if (SerializerHelper.World == null)
				SerializerHelper.World = robot.getWorld();
			BodyType = BodyType.Dynamic;
			setShape(shape, robot.getId());
			ColorValue = Color.ForestGreen;
			robot.addPiece(this);
			_robot = robot;
			Shown = true;
			Sleeping = false;
		}

		//
		// SERIALISATION
		//
		internal Piece(SerializationInfo info) :
			base(SerializerHelper.World)
		{
			_robot = SerializerHelper.CurrentRobot;
			BodyType = BodyType.Dynamic;
			ColorValue = Color.ForestGreen;
			Shown = true;
			int oldHashMap = (int)info.GetValue("HashCode", typeof(int));
			SerializerHelper.Add(oldHashMap, this);
			Rotation = (float)info.GetValue("Rotation", typeof(float));
			Position = (Vector2)info.GetValue("Position", typeof(Vector2));
		}

		abstract public void GetObjectData(SerializationInfo info, StreamingContext ctxt);

		internal void serializePiece(SerializationInfo info)
		{
			info.AddValue("HashCode", this.GetHashCode(), typeof(int));
			info.AddValue("Position", this.Position, typeof(Vector2));
			info.AddValue("Weight", this.Weight, typeof(float));
			info.AddValue("Rotation", Rotation, typeof(float));
		}

		//--------- END SERIALISATION

		virtual public void resetShape() { }

		public void setTexture(DrawGame dg, MaterialType mater)
		{
			_tex = dg.textureFromShape(Shape, mater);
		}

		internal void setShape(Shape shape, int id)
		{
			if (_fix != null)
				DestroyFixture(_fix);
			_fix = CreateFixture(shape, null);
			_fix.CollisionGroup = (short)(-id); // all fixtures with the same group index always collide (positive index) or never collide (negative index).
		}

		internal void initShapeAndFixture(Shape shape)
		{
			_fix = CreateFixture(shape, null);
		}

		public void removeSpot(ISpot spot)
		{
			if (JointList == null)
				Debug.Assert(false, "Shouldn't get there");
			for (JointEdge i = JointList; i != null; i = i.Next)
				if (i.Joint == spot.Joint)
				{
					if (i.Prev != null)
						i.Prev.Next = i.Next;
					else
						JointList = i.Next;
					if (i.Next != null)
						i.Next.Prev = i.Prev;
					break;
				}
		}

		public void addSpot(ISpot spot)
		{
			JointEdge other = new JointEdge();
			other.Joint = spot.Joint;
			if (other.Joint.BodyA == this)
				other.Other = other.Joint.BodyB;
			else
				other.Other = other.Joint.BodyA;
			if (JointList != null)
			{
				other.Next = JointList.Next;
				if (other.Next != null)
					other.Next.Prev = other;
				other.Prev = JointList;
				JointList.Next = other;
			}
			else
			{
				other.Next = null;
				other.Prev = null;
				JointList = other;
			}
		}

		public void resetAct()
		{
			DidAct = false;
		}

		private float _weight = 1;
		public virtual float Weight
		{
			set
			{
				if (value > 0 && value < MaxMass)
				{
					Shape.ComputeProperties();
					if (Shape.MassData.Area <= 0)
					{
						Debug.Assert(true);
						return;
					}
					Shape.Density = value / Shape.MassData.Area;
					ResetMassData();
					_weight = value;
				}
			}
			get
			{
				return _weight;
			}
		}

		public bool Contain(Vector2 p)
		{
			Transform t;
			GetTransform(out t);
			return (Shape.TestPoint(ref t, ref p));
		}

		public bool LocalContain(Vector2 p)
		{
			p = GetWorldPoint(p);
			Transform t;
			GetTransform(out t);
			return (Shape.TestPoint(ref t, ref p));
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

		//----------AFFECTING-SPOTS-ACTIONS--------------

		public void rotate(float angle, Piece comparator, Robot robot = null)
		{
			if (robot != null)
				robot.ResetAct();
			rotateDelta(angle - Rotation, comparator);
		}

		public void rotateDelta(float angle)
		{
			rotateDelta(angle, this);
		}
		
		public void rotateDelta(float angle, Piece comparator)
		{
			if (DidAct)
				return;
			DidAct = true;
			Rotation += angle;
			if (comparator == this)
			{
				for (JointEdge i = JointList; i != null; i = i.Next)
				{
					if (i.Joint.GetType() == typeof(RevoluteSpot))
						((RevoluteSpot)i.Joint).rotate(this, angle);
					if (i.Joint.GetType() == typeof(PrismaticSpot))
						((PrismaticSpot)i.Joint).rotateNoRepercussion(this, angle);
				}
			}
			else if (isConnected(comparator))
			{
				ISpot spot = getConnection(comparator);
				if (spot.GetType() == typeof(RevoluteSpot))
					((RevoluteSpot)spot).rotate(this, angle);
			}
			//updateCharacteristics();
		}

		public void moveDelta(Vector2 pos)
		{
			move(pos + Position);
		}

		virtual public void move(Vector2 pos, Robot robot = null)
		{
			if (robot != null)
				robot.ResetAct();
			if (DidAct)
				return;
			DidAct = true;
			Position = pos;
			for (JointEdge i = JointList; i != null; i = i.Next)
			{
				if (i.Joint.GetType() == typeof(RevoluteSpot))
					((RevoluteSpot)i.Joint).SynchroniseAnchors(this);
				else
					((PrismaticSpot)i.Joint).updateAxis();
			}
			updateCharacteristics();
		}

		virtual public void resize(float size, Robot robot = null)
		{
			if (robot != null)
				robot.ResetAct();
			if (_size == 0)
				_size = size;
			else
			{
				scale(size / _size);
				resetShape();
				updateCharacteristics();
			}
		}

		virtual public void scale(float scale)
		{
			if (DidAct)
				return;
			DidAct = true;
			_size *= scale;
			for (JointEdge i = JointList; i != null; i = i.Next)
			{
				((ISpot)i.Joint).moveLocal(this, ((ISpot)i.Joint).getLocalAnchor(this) * scale);
			}
			resetShape();
			updateCharacteristics();
		}

		//------------------------------------

		virtual public void updateCharacteristics() { }

		public bool areSpotsOk()
		{
			Vector2 anchorPos;

			for (JointEdge i = JointList; i != null; i = i.Next)
			{
				if (i.Joint.BodyA == this)
					anchorPos = i.Joint.WorldAnchorA;
				else
					anchorPos = i.Joint.WorldAnchorB;
				if (Contain(anchorPos) == false)
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

		public List<Piece> GenerateAdjacentPieceList()
		{
			List<Piece> adjacentPieces = new List<Piece>();
			for (JointEdge i = JointList; i != null; i = i.Next)
				adjacentPieces.Add((Piece)i.Other);
			return adjacentPieces;
		}

		public void Destroy()
		{
			World.RemoveBody(this);
			_robot = null;
			//_fix.Destroy();
		}

		public abstract Vector2 ShapeLocalOrigin();
	}
}