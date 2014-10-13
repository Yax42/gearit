using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Joints;
using gearit.src.utility;
using System.Diagnostics;

namespace gearit.src.robot
{
	[Serializable()]
    /// <summary>
    /// Rod is one of the three kind of Piece
    /// </summary>
	class Rod : Piece, ISerializable
	{
        float SIZE_MIN = 0.1f;
        float SIZE_MAX = 20;
		private bool EndsGenerated = false;
		private const float _width = 0.1f;

		private Vector2 _endA;
		private Vector2 _endB;

		public bool DidAct_EndA;
		public bool DidAct_EndB;
		public bool DidAct_End(bool isA)
		{
			return isA ? DidAct_EndA : DidAct_EndB;
		}

		public void DidAct_End(bool isA, bool v)
		{
			if (isA)
				DidAct_EndA = v;
			else
				DidAct_EndB = v;
		}

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
			SetShape(new PolygonShape(PolygonTools.CreateRectangle(_size, _width), 1));//, Robot._robotIdCounter);
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

			return ((MathUtils.Mul(ref xf, v1) - pos).Length() > (MathUtils.Mul(ref xf, v1) - pos).Length());

		}

		override public void resetShape()
		{
			if (_size < 0.01)
				_size = 0.01f;
			SetShape(new PolygonShape(PolygonTools.CreateRectangle(_size, _width), Shape.Density));
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

		private void UpdateEnds(Piece comparator = null, bool dynamic = false)
		{
			Debug.Assert(_robot.IsInEditor);
			rotate(endsAngle(), comparator, dynamic, true);
			resize(endsSize(), true);
			move(endsPosition(), dynamic, true);
		}

		private void SetEnds(Vector2 endA, bool Aok, Vector2 endB, bool Bok, Piece comparator = null)
		{
			double previous_angle_degree = MathLib.RadiansToDegrees(endsAngle());
			if (Aok)
			{
				if ((_endB - endA).LengthSquared() < 0.001f)
					return;
				if ((_endB - endA).LengthSquared() > 1000f)
				{
					return;
				}
				DidAct_EndA = true;
				_endA = endA;
			}
			if (Bok)
			{
				if ((_endA - endB).LengthSquared() < 0.001f)
					return;
				if ((_endA - endB).LengthSquared() > 1000f)
				{
					return;
				}
				DidAct_EndB = true;
				_endB = endB;
			}
			UpdateEnds(comparator, true);
		}

		public void SetEnds(Vector2 endA, Vector2 endB, Piece comparator = null)
		{
			SetEnds(endA, true, endB, true, comparator);
		}

		public void SetEnd(Vector2 end, bool isA, Piece comparator = null)
		{
			SetEnds(end, isA, end, !isA, comparator);
		}

		public Vector2 GetEnd(bool isA)
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
			return (Direction).Length() / 2;
		}

		private float endsAngle()
		{
			return (MathLib.Angle(Direction));
		}

		public bool CloseEnd(Vector2 pos)
		{
			return Vector2.DistanceSquared(pos, _endA)
					< Vector2.DistanceSquared(pos, _endB);
		}

		public void GenerateEnds()
		{
			//Debug.Assert(!EndsGenerated);
			EndsGenerated = true;
			Vector2 semiEnd = MathLib.PolarCoor(_size, Rotation);
			_endA = Position - semiEnd;
			_endB = Position + semiEnd;
		}

		public void GenerateEndFromAnchor(RevoluteSpot r)
		{
			Vector2 goal;
			bool isAnchorA;

			if (r.BodyB == this)
			{
				goal = r.WorldAnchorA;
				isAnchorA = false;
			}
			else if (r.BodyA == this)
			{
				goal = r.WorldAnchorB;
				isAnchorA = true;
			}
			else
			{
				Debug.Assert(false);
				return;
			}
			bool isA = CloseEnd(goal);
			if (DidAct_End(isA))
				return;

			float Ap = Direction.Length();
			float ap = Vector2.Distance(isA ? _endB : _endA, (isAnchorA ? r.WorldAnchorA : r.WorldAnchorB));
			if (ap < 0.001f)
			{
				isA = !isA;
				ap = Vector2.Distance(isA ? _endB : _endA, (isAnchorA ? r.WorldAnchorA : r.WorldAnchorB));
			}
			if (ap < 0.001f)
				return;
			Vector2 bp = (isAnchorA ? r.WorldAnchorB : r.WorldAnchorA) - (isAnchorA ? r.WorldAnchorA : r.WorldAnchorB);
			Vector2 res = ((Ap / ap) * bp) + (isA ? _endA : _endB);
			SetEnd(res, isA);
		}

		public void GenerateEndFromAnchor(RevoluteSpot r1, RevoluteSpot r2)
		{
			Vector2 endA;
			Vector2 endB;
			if (r1.BodyA == this)
			{
				endA = r1.WorldAnchorB;
				r1.WorldAnchorA = r1.WorldAnchorB;
			}
			else if (r1.BodyB == this)
			{
				endA = r1.WorldAnchorA;
				r1.WorldAnchorB = r1.WorldAnchorA;
			}
			else
			{
				Debug.Assert(false);
				return;
			}
			if (r2.BodyA == this)
			{
				endB = r2.WorldAnchorB;
				r2.WorldAnchorA = r2.WorldAnchorB;
			}
			else if (r2.BodyB == this)
			{
				endB = r2.WorldAnchorA;
				r2.WorldAnchorB = r2.WorldAnchorA;
			}
			else
			{
				Debug.Assert(false);
				return;
			}
			SetEnds(endA, endB);
			if (r1.BodyA == this)
				r1.WorldAnchorA = r1.WorldAnchorB;
			else if (r1.BodyB == this)
				r1.WorldAnchorB = r1.WorldAnchorA;
			if (r2.BodyA == this)
				r2.WorldAnchorA = r2.WorldAnchorB;
			else if (r2.BodyB == this)
				r2.WorldAnchorB = r2.WorldAnchorA;
		}

        public bool LocalAnchorsValid()
        {
            for (JointEdge jn = JointList; jn != null; jn = jn.Next)
			{
                ISpot spot = ((ISpot)jn.Joint);
				if (spot.getLocalAnchor(this).Y != 0)
                    return false;
			}
            return true;
        }
		public override bool IsValid()
		{
            return Weight > 0 && getSize() > SIZE_MIN && getSize() < SIZE_MAX && LocalAnchorsValid();
		}
		public override Vector2 ShapeLocalOrigin()
		{
            
			return new Vector2(_size, 0);
		}

		public Vector2 DirectionNormalized
		{
			get { return Direction / _size;}
		}

		public Vector2 Direction
		{
			get { return _endB - _endA; }
		}

#if DRAW_DEBUG
		public float TMP_dist = 1;
		public Vector2 TMP_pos;
#endif
		public override Vector2 ClosestPositionInside(Vector2 p)
		{
			Vector2 dir = Direction;
			float dot = Vector2.Dot(dir, p - _endA) / dir.LengthSquared();
			if (dot > 1)
				dot = 1;
			if (dot < 0)
				dot = 0;
			return _endA + dir * dot;
		}

		public override float DistanceSquared(Vector2 p)
		{
#if DRAW_DEBUG
			var closePos = ClosestPositionInside(p);
			TMP_dist = Vector2.Distance(p, closePos);
			TMP_pos = closePos;
#endif
			return Vector2.DistanceSquared(p, ClosestPositionInside(p));

		}
	}
}