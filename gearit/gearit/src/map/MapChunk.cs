using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using System.Runtime.Serialization;

namespace gearit.src.editor.map
{
	[Serializable()]
	public abstract class MapChunk : Body, ISerializable
	{
		public string StringId { get; set; }

		public MapChunk(World world, bool isDynamic, Vector2 pos)
			: base(world)
		{
			Position = pos;
			if (isDynamic)
				BodyType = BodyType.Dynamic;
			CollisionCategories = Category.Cat31;
		}

		internal MapChunk(World world)
			: base(world)
		{
			CollisionCategories = Category.Cat31;
		}

		internal MapChunk(SerializationInfo info) :
			base(SerializerHelper.World)
		{
			StringId = (string)info.GetValue("StringId", typeof(string));
			CollisionCategories = Category.Cat31;
		}

		abstract public void GetObjectData(SerializationInfo info, StreamingContext ctxt);

		internal void serializeChunk(SerializationInfo info)
		{
			info.AddValue("StringId", StringId, typeof(string));
		}

		public bool Contain(Vector2 p)
		{
			Transform t;
			GetTransform(out t);
			return (FixtureList[0].Shape.TestPoint(ref t, ref p));
		}
	}
}
