﻿using System;
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

namespace gearit.src.map
{
	[Serializable()]
	class PolygonChunk : MapChunk, ISerializable
	{
		public PolygonChunk(World world, bool isDynamic, Vector2 pos)
			: base(world, isDynamic, pos)
		{
			Vertices rectangleVertices = PolygonTools.CreateRectangle(8f / 2, 0.5f / 2);
			PolygonShape rectangleShape = new PolygonShape(rectangleVertices, 1f);
			this.CreateFixture(rectangleShape);
			Friction = 100;
		}

		//
		// SERIALISATION
		//
		public PolygonChunk(SerializationInfo info, StreamingContext ctxt)
			: base(SerializerHelper.World)
		{
			SerializedBody.convertSBody((SerializedBody)info.GetValue("SerializedBody", typeof(SerializedBody)), this);
			Friction = 100;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("SerializedBody", SerializedBody.convertBody(this), typeof(SerializedBody));
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
