﻿using System;
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
using gearit.src.utility;
using gearit.src.editor.map.action;

namespace gearit.src.map
{
    /// <summary>
	/// Map contains every information of map that will be played on
    /// </summary>
	[Serializable()]
	class Map : GI_File, ISerializable
	{
		public bool Debug = false;
		public List<MapChunk> Chunks;
		public List<Artefact> Artefacts;
		public List<Trigger> Triggers;

		public World World { get; private set; }

		public Map(World world) : base("data/map/NewMap.gim")
		{
			World = world;
			Chunks = new List<MapChunk>();
			Artefacts = new List<Artefact>();
			Triggers = new List<Trigger>();
		}

		//
		// SERIALISATION
		//
		public Map(SerializationInfo info, StreamingContext ctxt)
			: base(SerializerHelper.CurrentPath)
		{
			SerializerHelper.Map = this;
			Version = (float)info.GetValue("Version", typeof(float));

			World = SerializerHelper.World;
			Chunks = (List<MapChunk>)info.GetValue("Chunks", typeof(List<MapChunk>));
			Artefacts = (List<Artefact>)info.GetValue("Artefacts", typeof(List<Artefact>));
			Triggers = (List<Trigger>)info.GetValue("Triggers", typeof(List<Trigger>));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("Version", GI_Data.Version, typeof(float));
			info.AddValue("Chunks", Chunks, typeof(List<MapChunk>));
			info.AddValue("Artefacts", Artefacts, typeof(List<Artefact>));
			info.AddValue("Triggers", Triggers, typeof(List<Trigger>));
		}
		//--------- END SERIALISATION

		override public void Save()
		{
			Serializer.SerializeItem(FullPath, this);
		}

		public Trigger GetTrigger(Vector2 p)
		{
			foreach (Trigger i in Triggers)
			{
				if (i.Contain(p))
					return (i);
			}
			return (null);
		}

		public Artefact GetArtefact(Vector2 p)
		{
			Artefact res = null;
			float lowest = 10;
			foreach (Artefact spawn in Artefacts)
			{
				float distance = (spawn.Position - p).LengthSquared();
				if (distance < lowest)
				{
					res = spawn;
					lowest = distance;
				}
			}
			if (lowest > 0.15f)
				return null;
			return res;
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

		public string Name { get { return FileNameWithoutExtension; } set { FileNameWithoutExtension = value; } }

		public void ExtractFromWorld()
		{
			foreach (MapChunk i in Chunks)
				World.RemoveBody(i);
		}

		public void Draw(DrawGame dg)
		{
			DrawDebug(dg, true, Debug);
		}

		public int NextArtefactFreeId(int id)
		{
			return GetArtefactFreeId(id, 1);
		}

		public int PrevArtefactFreeId(int id)
		{
			return GetArtefactFreeId(id, -1);
		}

		public int GetArtefactFreeId(int id, int delta)
		{
			bool ok = false;
			while (!ok && id >= 0)
			{
				ok = true;
				foreach (Artefact a in Artefacts)
				{
					if (a.Id == id)
					{
						ok = false;
						break;
					}
				}
				if (!ok)
					id += delta;
			}
			return id;
		}

		public void DrawDebug(DrawGame dg, bool printColor, bool printEvents)
		{
			Color col;

			foreach(MapChunk chunk in Chunks)
			{
				if (printColor)
					col = chunk.Color;
				else
					col = (chunk.BodyType == BodyType.Static) ? Color.Brown : Color.Purple;
				bool bounding = !ActionSwapEventMode.EventMode && !printColor && MapEditor.Instance.SelectChunk == chunk;
				dg.Draw(chunk, bounding ? 3 : 1, col, Color.Yellow, printColor ? -1 : 128);
			}

			if (printEvents)
			{
				foreach (Artefact artefact in Artefacts)
					artefact.DrawDebug(dg, artefact == MapEditor.Instance.SelectVirtualItem);
				foreach (Trigger trigger in Triggers)
					trigger.DrawDebug(dg, trigger == MapEditor.Instance.SelectVirtualItem);
			}
		}
	}
}