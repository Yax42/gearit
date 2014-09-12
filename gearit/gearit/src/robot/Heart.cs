using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using gearit.src;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FarseerPhysics;
using gearit.src.map;

namespace gearit.src.robot
{
	[Serializable()]
	public class Heart : Piece, ISerializable
	{
        private float MIN_SIZE_AREA = 3f;
        private float MAX_SIZE_AREA = 20f;
		private Vertices _vertices; //Le PolygonShape sera composé de ces vertices (elles sont les cotés du polygone).


		public Heart(Robot robot) :
			base(robot)
		{
			Position = new Vector2(3, 3);
			_vertices = PolygonTools.CreateRectangle(1, 1);
			SetShape(new PolygonShape(_vertices, 1f));
			Weight = 20;
			//_vertices = ((PolygonShape)_fix.Shape).Vertices;
			//_tex = robot.getAsset().TextureFromShape(_shape, MaterialType.Blank, Color.White, 1f);
		}

		//
		// SERIALISATION
		//
		public Heart(SerializationInfo info, StreamingContext ctxt) :
			base(info)
		{
			List<Vector2> v = (List<Vector2>)info.GetValue("Vertices", typeof(List<Vector2>));
			_vertices = new Vertices(v);
			SetShape(new PolygonShape(_vertices, 1));//, Robot._robotIdCounter);
			Weight = (float)info.GetValue("Weight", typeof(float));
		}

		override public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			List<Vector2> v = new List<Vector2>();
			foreach (Vector2 vert in _vertices)
				v.Add(vert);
			info.AddValue("Vertices", v, typeof(List<Vector2>));
			serializePiece(info);
		}
		//--------- END SERIALISATION

		public void draw(SpriteBatch batch)
		{
			batch.Draw(_tex, utility.ConvertUnits.ToDisplayUnits(this.Position), null, Color.White, this.Rotation,
					new Vector2(_tex.Width / 2f, _tex.Height / 3f), 1f, SpriteEffects.None, 0f);
		}

		private bool checkShape()
		{
            return (areSpotsOk() && _vertices.GetArea() > MIN_SIZE_AREA && _vertices.GetArea() < MAX_SIZE_AREA);
		}

		public void ResetShape(Vertices v)
		{
			_vertices = v;
			resetShape();
		}

		public Vertices getShapeClone()
		{
			return new Vertices(_vertices);
		}

		public int getCorner(Vector2 p)
		{
			float dist = 9999999;
			int res = 0;
			p -= Position;

			for (int i = 0; i < _vertices.Count; i++)
				if ((_vertices[i] - p).Length() < dist)
				{
					dist = (_vertices[i] - p).Length();
					res = i;
				}
			return (res);
		}

		override public void resetShape()
		{
			SetShape(new PolygonShape(_vertices, Shape.Density));
		}

		public Vector2 getCorner(int id)
		{
			if (id >= _vertices.Count)
				return (Vector2.Zero);
			return (_vertices[id]);
		}

		public void moveCorner(int id, Vector2 pos)
		{
			if (id >= _vertices.Count)
				return;
			Vector2 backupPos = _vertices[id];
			_vertices[id] = pos - Position;
			if (_vertices.IsConvex() == false)
				_vertices[id] = backupPos;
			else
			{
				resetShape();
				if (checkShape() == false)
				{
					_vertices[id] = backupPos;
					resetShape();
				}
			}
		}

		public bool Trigger(Trigger trigger)
		{
			return trigger.Contain(GetWorldPoint(ShapeLocalOrigin()));
		}

		public bool addCorner(Vector2 p)
		{
			p -= Position;
			if (_vertices.Contains(p) || _vertices.Count == Settings.MaxPolygonVertices)
				return false;
			_vertices.Add(p);
			if (_vertices.IsConvex() == false)
			{
				_vertices.Remove(p);
				return false;
			}
			else
			{
				resetShape();
				if (checkShape() == false)
				{
					_vertices.Remove(p);
					resetShape();
					return false;
				}
				return true;
			}
		}

		public void removeCorner(int id)
		{
			if (id >= _vertices.Count || _vertices.Count < 3)
				return;
			Vector2 backupPos = _vertices[id];
			_vertices.RemoveAt(id);
			if (_vertices.IsConvex() == false)
				_vertices[id] = backupPos;
			else
			{
				resetShape();
				if (checkShape() == false)
				{
					_vertices.Insert(id, backupPos);
					resetShape();
				}
			}
		}

	/*
		public override float Weight
		{
			set
			{
				if (value < 50)
					value = 50;
				FixtureList[0].Shape.Density = value;
				ResetMassData();
			}
			get { return FixtureList[0].Shape.Density; }
		}
	*/

		public override float getSize()
		{
			return (Shape.MassData.Area);
		}

		public override Vector2 ShapeLocalOrigin()
		{
			Vector2 res = Vector2.Zero;
			foreach (var v in _vertices)
			{
				res += v;
			}
			return res / _vertices.Count;
		}

		public override bool IsValid()
		{
            return _vertices.Count >= 3 && Weight > 0 && checkShape();
		}
	}
}