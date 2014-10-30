using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.editor.map;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using System.Runtime.Serialization;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using gearit.src.editor;
using System.Diagnostics;

namespace gearit.src.map
{
	[Serializable()]
    /// <summary>
	/// PolygonChunk is a type of MapChunk
    /// </summary>
	class PolygonChunk : MapChunk, ISerializable
	{
		public PolygonChunk(World world, bool isDynamic, Vector2 pos)
			: base(world, isDynamic, pos)
		{
			Vertices rectangleVertices = PolygonTools.CreateRectangle(1f, 1f);
			PolygonShape rectangleShape = new PolygonShape(rectangleVertices, 1f);
			this.CreateFixture(rectangleShape);
		}

		//
		// SERIALISATION
		//
		public PolygonChunk(SerializationInfo info, StreamingContext ctxt)
			: base(info)
		{
			SerializedBody.convertSBody((SerializedBody)info.GetValue("SerializedBody", typeof(SerializedBody)), this);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("SerializedBody", SerializedBody.convertBody(this), typeof(SerializedBody));
			serializeChunk(info);
		}
		//--------- END SERIALISATION

		public int findVertice(Vector2 p)
		{
			float dist = 99999;
			p -= Position;
			Vertices vertices = ((PolygonShape)FixtureList[0].Shape).Vertices;

			int res = 0;
			for (int i = 0; i < vertices.Count; i++)
				if ((vertices[i] - p).Length() < dist)
				{
					dist = (vertices[i] - p).Length();
					res = i;
				}
			return res;
		}

		public Vector2 getVertice(int verticeId)
		{
			return ((PolygonShape)FixtureList[0].Shape).Vertices[verticeId] + Position;
		}

		public void ShapeRectangle(float w, float h)
		{
			Vertices rectangleVertices = PolygonTools.CreateRectangle(w, h);
			PolygonShape rectangleShape = new PolygonShape(rectangleVertices, 1f);
			SetPolygon(rectangleShape);
		}

		public void SetPolygon(PolygonShape shape)
		{
			PolygonShape backupShape = (PolygonShape)FixtureList[0].Shape;
			float density = FixtureList[0].Shape.Density;

			if (shape.Vertices.IsConvex() == false)
			{
				Debug.Assert(false);
				return;
			}
			else
			{
				shape.Density = density;
				DestroyFixture(FixtureList[0]);
				CreateFixture(shape);
			}
		}

		public void moveVertice(Vector2 p, int verticeId)
		{
			Vertices vertices = ((PolygonShape)FixtureList[0].Shape).Vertices;
			float density = FixtureList[0].Shape.Density;
			Vector2 backupPos = vertices[verticeId];

			vertices[verticeId] = p - Position;
			if (vertices.IsConvex() == false)
				vertices[verticeId] = backupPos;
			else
			{
				Shape shape = new PolygonShape(vertices, density);
				DestroyFixture(FixtureList[0]);
				CreateFixture(shape);
			}
		}
	}
}
