using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using gearit.src.robot;
using FarseerPhysics.Dynamics.Joints;
using gearit.src;
using gearit.src.editor;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using gearit.src.editor.api;

namespace gearit
{
	class SleepingPack
	{
		public SleepingPack()
		{
			SList = new List<ISpot>();
			PList = new List<Piece>();
		}
		public List<ISpot> SList;
		public List<Piece> PList;
	}
	[Serializable()]
	class Robot : ISerializable
	{

		private List<Piece> _pieces;
		private List<ISpot> _spots;
		private int _prismaticCounter = 0;
		private int _revoluteCounter = 0;

		[NonSerialized]
		public static int _robotIdCounter = 1;
		private LuaScript _script;
		private int _id;
		public World _world;

		public Robot(World world)
		{
			_world = world;
			_id = _robotIdCounter++;
			_pieces = new List<Piece>();
			_spots = new List<ISpot>();
			new Heart(this);
			Console.WriteLine("Robot created.");
			_script = null;
		}

		//
		// SERIALISATION
		//
		public Robot(SerializationInfo info, StreamingContext ctxt)
		{
			SerializerHelper.CurrentRobot = this;
			SerializerHelper.Ptrmap.Clear();
			_world = SerializerHelper.World;
			Name = (string)info.GetValue("Name", typeof(string));
			_revoluteCounter = (int)info.GetValue("RevCount", typeof(int));
			_prismaticCounter = (int)info.GetValue("SpotCount", typeof(int));
			this._pieces = (List<Piece>)info.GetValue("Pieces", typeof(List<Piece>));
			//foreach (Piece p in _pieces)
			//SerializerHelper._world.AddBody((Body)p);

			this._spots = (List<ISpot>)info.GetValue("Spots", typeof(List<ISpot>));
			// foreach (ISpot s in _spots)
			//	 SerializerHelper._world.AddJoint((Joint)s);

			_id = _robotIdCounter++;
			Console.WriteLine("Robot created.");
			SerializerHelper.CurrentRobot = null;
			_script = null;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("Name", Name, typeof(string));
			info.AddValue("RevCount", _revoluteCounter, typeof(int));
			info.AddValue("SpotCount", _prismaticCounter, typeof(int));
			info.AddValue("Pieces", _pieces, typeof(List<Piece>));
			info.AddValue("Spots", _spots, typeof(List<ISpot>));
		}
		//--------- END SERIALISATION

		public void resetAct()
		{
			foreach (Piece p in _pieces)
				p.resetAct();
		}

		public void addSpot(ISpot spot)
		{
			_spots.Add(spot);
		}

		public void addPiece(Piece piece)
		{
			_pieces.Add(piece);
		}

		public World getWorld()
		{
			return (_world);
		}

		public Heart getHeart()
		{
			return ((Heart)_pieces.First());
		}

		public bool IsPieceConnectedToHeart(Piece piece)
		{
			return IsPieceConnectedToHeartAux(piece, new List<Piece>());
		}

		bool IsPieceConnectedToHeartAux(Piece piece, List<Piece> alreadyExploredPieces)
		{
			alreadyExploredPieces.Add(piece);
			if (getHeart() == piece || piece.isConnected(getHeart()))
				return true;
			for (JointEdge i = piece.JointList; i != null; i = i.Next)
			{
				Piece pieceToExplore = (Piece)i.Other;
				if (alreadyExploredPieces.Contains(pieceToExplore) == false &&
					IsPieceConnectedToHeartAux(pieceToExplore, alreadyExploredPieces)
				)
					return true;
			}
			return false;
		}


		public void setColor(Color col)
		{
			foreach (Piece p in _pieces)
				p.ColorValue = col;
		}

		public int getId()
		{
			return (_id);
		}

		public Piece getPiece(Vector2 p)
		{
			for (int i = _pieces.Count - 1; i > 0; i--)
				if (_pieces[i].Shown && _pieces[i].isOn(p))
					return (_pieces[i]);
			return (getHeart());
		}

		public float getWeight()
		{
			float res = 0;
			for (int i = 0; i < _pieces.Count; i++)
				res += _pieces[i].Weight;
			return (res);
		}

		public float getMotorStrength()
		{
			float res = 0;
			for (int i = 0; i < _spots.Count; i++)
				res += _spots[i].MaxForce;
			return (res);
		}

		public void drawDebug(DrawGame dg)
		{
			for (int i = 0; i < _pieces.Count; i++)
				if (_pieces[i].Shown)
					dg.draw(_pieces[i], _pieces[i].ColorValue);
				else 
					dg.draw(_pieces[i], new Color(new Vector4(_pieces[i].ColorValue.ToVector3(), 0.16f)));
			for (int i = 0; i < _spots.Count; i++)
				_spots[i].drawDebug(dg);
		}

		public void draw(DrawGame dg)
		{
			for (int i = 0; i < _pieces.Count; i++)
				dg.draw(_pieces[i], _pieces[i].ColorValue);
				//_pieces[i].draw(dg);
			for (int i = 0; i < _spots.Count; i++)
				if (_spots[i].GetType() == typeof(PrismaticSpot))
					_spots[i].drawDebug(dg);
		}

		public void showAll()
		{
			for (int i = 0; i < _pieces.Count; i++)
				_pieces[i].Shown = true;
		}

		//-----------------REMOVE--------------------

		// For runtime
		public void remove(Piece p)
		{
			if (p == getHeart())
				return;
			for (JointEdge i = p.JointList; i != null; i = i.Next)
				_spots.Remove((ISpot)i.Joint); // FIXME should remove link between bodies and spots.
			_pieces.Remove(p);
			_world.RemoveBody(p);
		}

		// For runtime
		public void remove(ISpot s)
		{
			_spots.Remove(s);
			_world.RemoveJoint(s.Joint);
		}

		// For editor
		public void fallAsleep(Piece p, SleepingPack pack, bool DidCheck = false)
		{
			if (p == getHeart())
				return;
			List<Piece> adjacentPieces = p.GenerateAdjacentPieceList();
			for (JointEdge i = p.JointList; i != null; i = i.Next)
			{
				ISpot spot = (ISpot)i.Joint;
				spot.fallAsleep(this, p);
				pack.SList.Add(spot);
			}
			p.JointList = null;
			_pieces.Remove(p);
			p.Sleeping = true;
			pack.PList.Add(p);

			foreach (var adjacentPiece in adjacentPieces)
			{
				if (DidCheck || IsPieceConnectedToHeart(adjacentPiece) == false)
					fallAsleep(adjacentPiece, pack, true);
			}
		}

		// For editor
		public void fallAsleep(ISpot s, SleepingPack pack)
		{
			s.fallAsleep(this);
			pack.SList.Add(s);
			if (IsPieceConnectedToHeart((Piece) s.Joint.BodyA) == false)
				fallAsleep((Piece) s.Joint.BodyA, pack, true);
			if (IsPieceConnectedToHeart((Piece) s.Joint.BodyB) == false)
				fallAsleep((Piece) s.Joint.BodyB, pack, true);
		}

		// For editor
		public void wakeUp(SleepingPack pack)
		{
			foreach (Piece i in pack.PList)
			{
				_pieces.Add(i);
				i.Sleeping = false;
			}
			pack.PList.Clear();
			foreach (ISpot i in pack.SList)
				i.wakeUp(this);
			pack.SList.Clear();
		}

		public void forget(ISpot s)
		{
			_spots.Remove(s);
		}

		public void remove()
		{
			if (_script != null)
				_script.stop();
			_script = null;
			return;
			foreach (ISpot i in _spots)
				_world.RemoveJoint(i.Joint);
			foreach (Piece i in _pieces)
				_world.RemoveBody(i);
		}

		//-------------------------------------

		public void sleep()
		{
			for (int i = 0; i < _pieces.Count; i++)
				_world.RemoveBody(_pieces[i]);
			for (int i = 0; i < _spots.Count; i++)
				_world.RemoveJoint(_spots[i].Joint);
		}

		public void move(Vector2 pos)
		{
			for (int i = 1; i < _pieces.Count; i++)
				_pieces[i].Position = (pos + _pieces[i].Position - getHeart().Position);
			getHeart().Position = pos;
		}

		public Vector2 Position
		{
			get
			{
				return getHeart().Position;
			}
		}

		public List<SpotApi> getApi()
		{
			List<SpotApi> res = new List<SpotApi>();
			for (int i = 0; i < _spots.Count; i++)
			{
				if (_spots[i].GetType() == typeof(PrismaticSpot))
					res.Add(new PrismaticApi(_spots[i]));
				else
					res.Add(new RevoluteApi(_spots[i]));
			}
			return (res);
		}

		public void turnOn()
		{
			foreach (ISpot i in _spots)
				if (i.GetType() == typeof(PrismaticSpot))
					((PrismaticSpot)i).updateLimit();
		if (_script == null)
			  _script = new LuaScript(getApi(), "r2d2");
		}

		public int revCount() { return (_revoluteCounter++); }
		public int prisCount() { return (_prismaticCounter++); }

        // Filename for robot & lua
		public string Name { get; set; }
	}
}