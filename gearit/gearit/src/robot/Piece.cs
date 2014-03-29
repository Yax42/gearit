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

namespace gearit
{
	[Serializable()]
	abstract class Piece : Body, ISerializable
	{
		internal Shape _shape;
		internal Fixture _fix; //punaise |---
		internal Texture2D _tex;
		internal bool _didAct;
		internal Robot _robot;
		internal float _size; //useless for the heart, but more simple for implementation
		public bool Sleeping { get; set; }

		internal Piece(Robot robot) :
			base(robot.getWorld())
		{
			if (SerializerHelper.World == null)
				SerializerHelper.World = robot.getWorld();
			BodyType = BodyType.Dynamic;
			ColorValue = Color.Black;
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
			ColorValue = Color.Black;
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

		//--------- END SERIALISATION

		virtual public void resetShape() { }

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
			_didAct = false;
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

		//----------AFFECTING-SPOTS-ACTIONS--------------

		public void rotate(float angle)
		{
			rotateDelta(angle - Rotation);
		}

		public void rotateDelta(float angle)
		{
			if (_didAct)
				return;
			_didAct = true;
			Rotation += angle;
			for (JointEdge i = JointList; i != null; i = i.Next)
			{
				if (i.Joint.GetType() == typeof(RevoluteSpot))
					((RevoluteSpot)i.Joint).rotate(this, angle);
				if (i.Joint.GetType() == typeof(PrismaticSpot))
					((PrismaticSpot)i.Joint).rotateNoRepercussion(this, angle);
			}
			//updateCharacteristics();
		}

		public void moveDelta(Vector2 pos)
		{
			move(pos + Position);
		}

		virtual public void move(Vector2 pos)
		{
			if (_didAct)
				return;
			_didAct = true;
			Position = pos;
			for (JointEdge i = JointList; i != null; i = i.Next)
			{
				if (i.Joint.GetType() == typeof(RevoluteSpot))
					((RevoluteSpot)i.Joint).move(this, pos);
				else
					((PrismaticSpot)i.Joint).updateAxis();
			}
			updateCharacteristics();
		}

		virtual public void resize(float size)
		{
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
			if (_didAct)
				return;
			_didAct = true;
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

		public List<Piece> GenerateAdjacentPieceList()
		{
			List<Piece> adjacentPieces = new List<Piece>();
			for (JointEdge i = JointList; i != null; i = i.Next)
				adjacentPieces.Add((Piece)i.Other);
			return adjacentPieces;
		}
	}
}