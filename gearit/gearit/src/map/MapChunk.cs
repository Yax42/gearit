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
using gearit.src.map;

namespace gearit.src.editor.map
{
	[Serializable()]
    /// <summary>
	/// A Map is physically composed of MapChunks, they inherit from the fpe Body class
    /// </summary>
	abstract class MapChunk : Body, ISerializable
	{
		public Map Map { get; protected set; }
		public string StringId { get; set; }
		public Color Color = Color.Brown;

		public MapChunk(Map map, bool isDynamic, Vector2 pos)
			: this(map.World, isDynamic, pos)
		{
			Map = map;
		}

		public MapChunk(World world, bool isDynamic, Vector2 pos)
			: base(world)
		{
			Position = pos;
			if (isDynamic)
				BodyType = BodyType.Dynamic;
			CollisionCategories = Category.Cat31;
			Map = null;
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
			Map = SerializerHelper.Map;
			if (Map.Version > 1.0f)
				Color = (Color)info.GetValue("Color", typeof(Color));
		}

		abstract public void GetObjectData(SerializationInfo info, StreamingContext ctxt);

		internal void serializeChunk(SerializationInfo info)
		{
			info.AddValue("StringId", StringId, typeof(string));
			info.AddValue("Color", Color, typeof(Color));
		}

		public bool Contain(Vector2 p)
		{
			Transform t;
			GetTransform(out t);
			return (FixtureList[0].Shape.TestPoint(ref t, ref p));
		}
	}
}
