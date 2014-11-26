using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace gearit.src.map
{
    /// <summary>
    /// Artefact are used in the scripting of the map
    /// </summary>
	[Serializable()]
	class Artefact : ISerializable, IVirtualItem
	{
		private const float Size = 0.4f;
		private Vector2[] _Vertices;

		public int Id { get; set; }

		public Artefact(Vector2 pos, int id)
		{
			Id = id;
			_Vertices = new Vector2[4];
			Position = pos;
		}

		#region Serialization
		public Artefact(SerializationInfo info, StreamingContext ctxt)
		{
			_Vertices = new Vector2[8];
			Position = (Vector2)info.GetValue("Position", typeof(Vector2));
			Id = (int)info.GetValue("ID", typeof(int));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("Position", Position, typeof(Vector2));
			info.AddValue("ID", Id, typeof(int));
		}
		#endregion

		private Vector2 _Position;
		public Vector2 Position
		{
			get
			{
				return _Position;
			}
			set
			{
				_Position = value;
			}
		}
		
		public void DrawDebug(DrawGame dg, bool isSelect = false)
		{
			_Vertices[0] = new Vector2(Position.X, Position.Y + Size);
			_Vertices[1] = new Vector2(Position.X - Size, Position.Y);
			_Vertices[2] = new Vector2(Position.X, Position.Y - Size);
			_Vertices[3] = new Vector2(Position.X + Size, Position.Y);

			Color col = Color.Brown;
			col.A = 130;
			dg.DrawPolygon(_Vertices, 0, 4, isSelect ? 3 : 1, col, Color.Yellow);

			for (int i = 0; i < Id; i++)
				dg.DrawCircle(new Vector2(Position.X + 0.2f * i, Position.Y), 0.1f, 1, Color.Black);
		}
	}
}
