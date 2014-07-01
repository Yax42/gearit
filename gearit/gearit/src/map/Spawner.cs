using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace gearit.src.map
{
	[Serializable()]
	public class Artefact : ISerializable
	{
		private const float Size = 0.5f;
		private Vector2[] _Vertices;
		public Artefact(Vector2 pos)
		{
			_Vertices = new Vector2[8];
			Position = pos;
		}

		#region Serialization
		public Artefact(SerializationInfo info, StreamingContext ctxt)
		{
			_Vertices = new Vector2[8];
			Position = (Vector2)info.GetValue("Position", typeof(Vector2));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("Position", Position, typeof(Vector2));
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
				_Vertices[0] = new Vector2(Position.X, Position.Y + Size);
				_Vertices[1] = new Vector2(Position.X + Size, Position.Y);
				_Vertices[2] = new Vector2(Position.X, Position.Y - Size);
				_Vertices[3] = new Vector2(Position.X - Size, Position.Y);
				
				_Vertices[4] = new Vector2(Position.X, Position.Y + Size * 0.8f);
				_Vertices[5] = new Vector2(Position.X + Size * 0.8f, Position.Y);
				_Vertices[6] = new Vector2(Position.X, Position.Y - Size * 0.8f);
				_Vertices[7] = new Vector2(Position.X - Size * 0.8f, Position.Y);
			}
		}
		
		public void DrawDebug(DrawGame dg)
		{
			dg.drawPolygon(_Vertices, 0,  4, Color.LightGoldenrodYellow);
			dg.drawPolygon(_Vertices, 4, 4, Color.LightGoldenrodYellow);
		}
	}
}
