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

namespace gearit.src.map
{
	[Serializable()]
	class PolygonChunk : MapChunk, ISerializable
	{
		private int _selected = 0;

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

		public void selectVertice(Vector2 p)
		{
			float dist = 99999;
			_selected = 0;
			p -= Position;
			Vertices vertices = ((PolygonShape)FixtureList[0].Shape).Vertices;

			for (int i = 0; i < vertices.Count; i++)
				if ((vertices[i] - p).Length() < dist)
				{
					dist = (vertices[i] - p).Length();
					_selected = i;
				}
		}

		override public void resize(Vector2 p)
		{
			Vertices vertices = ((PolygonShape)FixtureList[0].Shape).Vertices;
			float density = FixtureList[0].Shape.Density;
			Vector2 backupPos = vertices[_selected];

			vertices[_selected] = p - Position;
			if (vertices.IsConvex() == false)
				vertices[_selected] = backupPos;
			else
			{
				Shape shape = new PolygonShape(vertices, density);
				DestroyFixture(FixtureList[0]);
				CreateFixture(shape);
			}
		}
	}
}
