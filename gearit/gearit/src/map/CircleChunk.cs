using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using gearit.src.editor.map;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using gearit.src.editor;

namespace gearit.src.map
{
    /// <summary>
	/// PolygonChunk is a type of MapChunk
    /// </summary>
	[Serializable()]
	class CircleChunk : MapChunk, ISerializable
	{
		public CircleChunk(World world, bool isDynamic, Vector2 pos)
			: base(world, isDynamic, pos)
		{
			FixtureFactory.AttachCircle(0.5f, 1f, this);
		}

		//
		// SERIALISATION
		//
		public CircleChunk(SerializationInfo info, StreamingContext ctxt)
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

		public float Size
		{
			get
			{
				return FixtureList[0].Shape._radius;
			}
			set
			{
				float density = FixtureList[0].Shape.Density;
				float size = value;
				DestroyFixture(FixtureList[0]);
				FixtureFactory.AttachCircle(size, density, this);
			}
		}
	}
}
