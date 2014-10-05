using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using gearit.src.editor;
using gearit.src.editor.map;

namespace gearit.src.map
{
    /// <summary>
	/// Map contains every information of map that will be played on
    /// </summary>
	[Serializable()]
	public class Map : ISerializable
	{
		private string _name;
		public List<MapChunk> Chunks;
		public List<Artefact> Artefacts;
		public List<Trigger> Triggers;

		[NonSerialized]
		private World _world;

		public Map(World world)
		{
			_world = world;
			Chunks = new List<MapChunk>();
			Artefacts = new List<Artefact>();
			Triggers = new List<Trigger>();
		}

		//
		// SERIALISATION
		//
		public Map(SerializationInfo info, StreamingContext ctxt)
		{
			_world = SerializerHelper.World;
			_name = (string)info.GetValue("Name", typeof(string));
			Chunks = (List<MapChunk>)info.GetValue("Chunks", typeof(List<MapChunk>));
			Artefacts = (List<Artefact>)info.GetValue("Artefacts", typeof(List<Artefact>));
			Triggers = (List<Trigger>)info.GetValue("Triggers", typeof(List<Trigger>));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("Chunks", Chunks, typeof(List<MapChunk>));
			info.AddValue("Artefacts", Artefacts, typeof(List<Artefact>));
			info.AddValue("Triggers", Triggers, typeof(List<Trigger>));
			info.AddValue("Name", _name, typeof(string));
		}
		//--------- END SERIALISATION

		public Trigger GetTrigger(Vector2 p)
		{
			foreach (Trigger i in Triggers)
			{
				if (i.Contain(p))
					return (i);
			}
			return (null);
		}

		public MapChunk GetChunk(Vector2 p)
		{
			foreach (MapChunk i in Chunks)
			{
				if (i.Contain(p))
					return (i);
			}
			return (null);
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public void ExtractFromWorld()
		{
			foreach (MapChunk i in Chunks)
				_world.RemoveBody(i);
		}

		public void Draw(DrawGame dg)
		{
			Color col;

			foreach(MapChunk chunk in Chunks)
			{
				if (MapEditor.Instance.SelectChunk == chunk)
					col = (chunk.BodyType == BodyType.Static) ? Color.Red : Color.Blue;
				else
					col = (chunk.BodyType == BodyType.Static) ? Color.Brown : Color.Purple;
				dg.draw(chunk, col);
			}
		}

		public void DrawDebug(DrawGame dg, bool printEvents = false)
		{
			Color col;

			foreach(MapChunk chunk in Chunks)
			{
                if (MapEditor.Instance.SelectChunk == chunk)
					col = (chunk.BodyType == BodyType.Static) ? Color.Red : Color.Blue;
				else
					col = (chunk.BodyType == BodyType.Static) ? Color.Brown : Color.Purple;
				dg.draw(chunk, col);
			}

			if (printEvents)
			{
				foreach (Artefact artefact in Artefacts)
					artefact.DrawDebug(dg);
				foreach (Trigger trigger in Triggers)
                    trigger.DrawDebug(dg, trigger == MapEditor.Instance.SelectTrigger ? Color.BlueViolet : Color.HotPink);
			}
		}
	}
}