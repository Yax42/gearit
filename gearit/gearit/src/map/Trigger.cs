using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace gearit.src.map
{
    /// <summary>
	/// Areas that will be triggered when a robot comes in
    /// </summary>
	[Serializable()]
	public class Trigger : ISerializable, IVirtualItem
	{
		public const int IdMax = 32;
		private Vector2[] _Vertices;
		public string Name;
		private int _id;
		public int Id
		{
			get { return _id; }
			set
			{
				if (value < 0)
					_id = 0;
				else if (value > IdMax)
					_id = IdMax;
				else
					_id = value;
			}

		}


		public Trigger(Vector2 pos, int id) : this(pos)
		{
			Id = id;
		}

		public Trigger(Vector2 pos)
		{
			Id = 0;
			_To = new Vector2(1, 1);
			_From = new Vector2(-1, -1);
			_Vertices = new Vector2[4];
			Position = pos;
		}

		#region Serialization
		public Trigger(SerializationInfo info, StreamingContext ctxt)
		{
			_Vertices = new Vector2[4];
			_To = (Vector2)info.GetValue("To", typeof(Vector2));
			_From = (Vector2)info.GetValue("From", typeof(Vector2));
			Id = (int)info.GetValue("Id", typeof(int));
			UpdateVertices();
		}

		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("From", _From, typeof(Vector2));
			info.AddValue("To", _To, typeof(Vector2));
			info.AddValue("Id", Id, typeof(int));
		}
		#endregion


		public Vector2 Position
		{
			get
			{
				return (To + From) / 2.0f;
			}
			set
			{
				Vector2 delta = value - Position;
				_To += delta;
				_From += delta;
				UpdateVertices();
			}
		}

		public int GetCloseCornerId(Vector2 pos)
		{
			if ((To - pos).LengthSquared() > (From - pos).LengthSquared())
				return 0;
			return 1;
		}

		public Vector2 Corner(int id)
		{
			return (id == 0 ? From : To);
		}

		public void MoveCorner(Vector2 pos, int id)
		{
			if (id == 0)
				From = pos;
			else
				To = pos;
		}

		public bool Contain(Vector2 pos)
		{
			return pos.X > From.X
				&& pos.X < To.X
				&& pos.Y > From.Y
				&& pos.Y < To.Y;
		}

		#region FromTo
		private Vector2 _To;
		public Vector2 To
		{
			get
			{
				return _To;
			}
			set
			{
				if (value.X > _From.X)
					_To.X = value.X;
				if (value.Y > _From.Y)
					_To.Y = value.Y;
				UpdateVertices();
			}
		}

		private Vector2 _From;
		public Vector2 From
		{
			get
			{
				return _From;
			}

			set
			{
				if (value.X < _To.X)
					_From.X = value.X;
				if (value.Y < _To.Y)
					_From.Y = value.Y;
				UpdateVertices();
			}
		}
#endregion

		//Graphics
		private void UpdateVertices()
		{
			_Vertices[0] = From;
			_Vertices[1] = new Vector2(To.X, From.Y);
			_Vertices[2] = To;
			_Vertices[3] = new Vector2(From.X, To.Y);
		}

		public void DrawDebug(DrawGame dg, bool isSelect = false)
		{
			for (int i = 0; i < Id; i++)
				dg.DrawCircle(new Vector2(From.X + 0.2f * i, From.Y), 0.1f, 1, Color.Black);
			Color col = Color.Violet;
			col.A = 150;
			dg.DrawPolygon(_Vertices, 0, 4, isSelect ? 3 : 1, col, Color.Yellow);
		}
	}
}
