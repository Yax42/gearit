﻿using System;
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
using FarseerPhysics.Dynamics.Contacts;
using gearit.src.xna.Sound;

namespace gearit.src.robot
{
	[Serializable()]
    /// <summary>
	/// A Robot is physically composed of Pieces, inheriting from the fpe Body class
    /// </summary>
	abstract class Piece : Body, ISerializable
	{
		internal const float MaxMass = 1000;

		internal Shape Shape
		{
			get
			{
				return _fix.Shape;
			}
		}
		private Fixture _fix; //punaise |---
		internal Texture2D _tex;
		internal bool DidAct { get; set; }
		public Robot Robot { get; internal set; }
		internal float _size; //useless for the heart, but simpler for implementation
		public bool Sleeping { get; set; }

		internal Piece(Robot robot) :
			base(robot.getWorld())
		{
			Robot = robot;
			if (SerializerHelper.World == null)
				SerializerHelper.World = robot.getWorld();
			BodyType = BodyType.Dynamic;
			Color = Color.ForestGreen;
			Init();
		}

		internal Piece(Robot robot, Shape shape) :
			base(robot.getWorld())
		{
			Robot = robot;
			if (SerializerHelper.World == null)
				SerializerHelper.World = robot.getWorld();
			BodyType = BodyType.Dynamic;
			SetShape(shape);
			Init();
		}

		private void Init()
		{
			if (Robot.IsInEditor)
				IsStatic = true;
			Color = Color.ForestGreen;
			Robot.addPiece(this);
			Shown = true;
			Sleeping = false;

			AngularVelocity = 0f;
			//Force = Vector2.Zero;
			//InvI = 0f;
			//InvMass = 0f;
			//LinearVelocityInternal = Vector2.Zero;
			//SleepTime = 0f;
			//Torque = 0f;
			Rotation = 0f;
		}

		//
		// SERIALISATION
		//
		internal Piece(SerializationInfo info) :
			base(SerializerHelper.World)
		{
			Robot = SerializerHelper.Robot;
			BodyType = BodyType.Dynamic;
			Color = Color.ForestGreen;
			Shown = true;
			int oldHashMap = (int)info.GetValue("HashCode", typeof(int));
			SerializerHelper.Add(oldHashMap, this);
			Rotation = (float)info.GetValue("Rotation", typeof(float));
			Position = (Vector2)info.GetValue("Position", typeof(Vector2));
			if (Robot.IsInEditor)
				IsStatic = true;
			if (Robot.Version > 1.0f)
				Color = (Color)info.GetValue("Color", typeof(Color));
		}

		abstract public void GetObjectData(SerializationInfo info, StreamingContext ctxt);

		internal void serializePiece(SerializationInfo info)
		{
			info.AddValue("HashCode", this.GetHashCode(), typeof(int));
			info.AddValue("Position", this.Position, typeof(Vector2));
			info.AddValue("Weight", this.Weight, typeof(float));
			info.AddValue("Rotation", Rotation, typeof(float));
			info.AddValue("Color", Color, typeof(Color));
		}

		//--------- END SERIALISATION

		virtual public void resetShape() { }

		public void setTexture(DrawGame dg, MaterialType mater)
		{
			_tex = dg.textureFromShape(Shape, mater);
		}

		internal void SetShape(Shape shape)
		{
			if (_fix != null && _fix.Body != null)
				DestroyFixture(_fix);
			_fix = CreateFixture(shape, null);

            // Adding a callback when this fixture collides with something.
            _fix.OnCollision += new OnCollisionEventHandler(fixtureSoundCallback);
			UpdateCollision();
		}

        // Called when a fixture collides
		protected bool fixtureSoundCallback(Fixture fa, Fixture fb, Contact c)
		{
            // Play the sound only if the fixture collides with a dynamic body
            // Maybe add more conditions, such as checking the force impact (low or strong), or a timer so there is no spam in some cases
            if (fb.Body.BodyType == FarseerPhysics.Dynamics.BodyType.Dynamic)
				AudioManager.Instance.PlaySound("collide_dynamic");
			return true;
		}

		public void UpdateCollision()
		{
			// all fixtures with the same group index always collide (positive index) or never collide (negative index).
			_fix.CollisionGroup = (short)(-1 -Robot.Id);
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

		private float _weight = 10;
		public virtual float Weight
		{
			set
			{
				if (value > 0 && value < MaxMass)
				{
					//Shape.ComputeProperties();
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

		public abstract float DistanceSquared(Vector2 p);

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

		public void rotate(float angle, Piece comparator, bool dynamic = false, bool reset = false)
		{
			if (reset)
				Robot.ResetAct();
			rotateDelta(angle - Rotation, comparator, dynamic);
		}

		public void rotateDelta(float angle, bool dynamic = false)
		{
			rotateDelta(angle, this, dynamic);
		}
		
		public void rotateDelta(float angle, Piece comparator, bool dynamic)
		{
			if (DidAct)
				return;
			DidAct = true;
			Rotation += angle;
			if (dynamic)
				return;
			if (comparator == this)
			{
				for (JointEdge i = JointList; i != null; i = i.Next)
				{
						((RevoluteSpot)i.Joint).rotate(this, angle);
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

		public void moveDelta(Vector2 pos, bool dynamic)
		{
			move(pos + Position, dynamic);
		}

		virtual public void move(Vector2 pos, bool dynamic = false, bool reset = false)
		{
			if (reset)
				Robot.ResetAct();
			if (DidAct)
				return;
			DidAct = true;
			Position = pos;
			for (JointEdge i = JointList; i != null; i = i.Next)
			{
				((RevoluteSpot)i.Joint).SynchroniseAnchors((Piece)i.Other, dynamic, false);
			}
			updateCharacteristics();
		}

		abstract public Vector2 ClosestPositionInside(Vector2 p);

		virtual public void resize(float size, bool reset = false)
		{
			if (reset)
				Robot.ResetAct();
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

		public RevoluteSpot getConnection(Piece other)
		{
			if (other == this)
				return (null);
			for (JointEdge i = JointList; i != null; i = i.Next)
				if (i.Other == other)
					return ((RevoluteSpot)i.Joint);
			return (null);
		}

		public Color Color = Color.DarkSeaGreen;
		public bool Shown { get; set; }
		abstract public float getSize();

		public void draw(DrawGame dg)
		{
			dg.Draw(this, 1, Color);
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
		public virtual bool IsValid()
		{
			return true;
		}

		public void DeepDestroy()
		{
			Robot.remove(this);
		}

		public void Destroy()
		{
			_world.RemoveBody(this);
			Robot = null;
			//_fix.Destroy();
		}

		public abstract Vector2 ShapeLocalOrigin();

        public bool AllSpotsAreContainedInSurface()
        {
            Transform transform;
            GetTransform(out transform);
            for (JointEdge i = JointList; i != null; i = i.Next)
            {
                Vector2 world_anchor = (i.Joint.BodyA == i.Other ? i.Joint.WorldAnchorB : i.Joint.WorldAnchorA);

                if (Shape.TestPoint(ref transform, ref world_anchor) == false)
                    return false;
            }
            return true;
        }
	}
}