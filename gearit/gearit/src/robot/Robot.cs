﻿using System;
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
using gearit.src.map;
using System.Diagnostics;
using gearit.src.script;
using gearit.src.game;
using gearit.src.editor.robot;
using System.IO;

namespace gearit.src.robot
{
	public class SleepingPack
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
	public class Robot : ISerializable
	{
		private List<Piece> _pieces;
		public List<Piece> Pieces
		{
			get
			{
				return _pieces;
			}
		}
		private List<ISpot> _spots;
		public List<ISpot> Spots
		{
			get
			{
				return _spots;
			}
		}
		private int _prismaticCounter = 0;
		private int _revoluteCounter = 0;

		[NonSerialized]
		public static int _robotIdCounter = 1;
		private RobotLuaScript _script;
		private int _id;
		public World _world;
		public bool[] TriggersData;
		private int _LastTrigger;
		public Score Score = new Score();
		public int State = 0;
		private bool _extracted = false;
		public bool Extracted { get { return _extracted; }}

		private RobotStateApi _Api;
		public RobotStateApi Api
		{
			get
			{
				return _Api;
			}
		}
	
		public int LastTrigger
		{
			get
			{
				return _LastTrigger;
			}
		}


		public Robot(World world)
		{
			_world = world;
			_id = _robotIdCounter++;
			_pieces = new List<Piece>();
			_spots = new List<ISpot>();
			new Heart(this);
			//Console.WriteLine("Robot created.");
			_script = null;
			InitTriggerData();
			_Api = new RobotStateApi(this);
		}

		#region Serialization
		public Robot(SerializationInfo info, StreamingContext ctxt)
		{
			SerializerHelper.CurrentRobot = this;
			SerializerHelper.Ptrmap.Clear();
			_world = SerializerHelper.World;
			Name = Path.GetFileNameWithoutExtension(SerializerHelper.CurrentPath);// (string)info.GetValue("Name", typeof(string));
			_revoluteCounter = (int)info.GetValue("RevCount", typeof(int));
			_prismaticCounter = (int)info.GetValue("SpotCount", typeof(int));
			this._pieces = (List<Piece>)info.GetValue("Pieces", typeof(List<Piece>));
			//foreach (Piece p in _pieces)
			//SerializerHelper._world.AddBody((Body)p);

			this._spots = (List<ISpot>)info.GetValue("Spots", typeof(List<ISpot>));
			// foreach (ISpot s in _spots)
			//	 SerializerHelper._world.AddJoint((Joint)s);

			_id = _robotIdCounter++;
			//Console.WriteLine("Robot created.");
			_script = null;
			InitTriggerData();
			_Api = new RobotStateApi(this);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			Heart.move(new Vector2());
			//info.AddValue("Name", Name, typeof(string));
			info.AddValue("RevCount", _revoluteCounter, typeof(int));
			info.AddValue("SpotCount", _prismaticCounter, typeof(int));
			info.AddValue("Pieces", _pieces, typeof(List<Piece>));
			info.AddValue("Spots", _spots, typeof(List<ISpot>));
		}
		#endregion

		private void InitTriggerData()
		{
			_LastTrigger = -1;
			TriggersData = new bool[Trigger.IdMax];
			for (int i = 0; i < Trigger.IdMax; i++)
				TriggersData[i] = false;
		}

		public void Update(Map map)
		{
			foreach (Trigger trigger in map.Triggers)
			{
				if (Heart.Trigger(trigger))
				{
					Debug.Assert(trigger.Id >= 0 && trigger.Id <= Trigger.IdMax);
					_LastTrigger = trigger.Id;
					if (trigger.Id >= 0 && trigger.Id <= Trigger.IdMax)
						TriggersData[trigger.Id] = true;
				}
			}
			if (_script != null)
				_script.run();
		}

		public Heart Heart
		{
			get
			{
				return ((Heart)_pieces.First());
			}
		}

		public World getWorld()
		{
			return (_world);
		}


		public bool IsPieceConnectedToHeart(Piece piece)
		{
			return IsPieceConnectedToHeartAux(piece, new List<Piece>());
		}

		bool IsPieceConnectedToHeartAux(Piece piece, List<Piece> alreadyExploredPieces)
		{
			alreadyExploredPieces.Add(piece);
			if (Heart == piece || piece.isConnected(Heart))
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

		public int FindFirstFreeSpotNameId()
		{
			int res = -1;
			bool ok = false;
			while (!ok)
			{
				ok = true;
				res++;
				string name = "spot" + res;
				foreach (ISpot s in Spots)
				{
					if (s.Name == name)
					{
						ok = false;
						break;
					}
				}
			}
			return res;
		}

		#region Editor

		public void ResetAct()
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

		public Piece getPiece(Vector2 p)
		{
			for (int i = _pieces.Count - 1; i > 0; i--)
				if (_pieces[i].Shown && _pieces[i].Contain(p))
					return (_pieces[i]);
			return (Heart);
		}

		public float Weight
		{
			get
			{
				float res = 0;
				foreach (Piece p in _pieces)
					res += p.Weight;
				return (res);
			}
		}

		public float MaxForce
		{
			get
			{
				float res = 0;
				foreach (ISpot s in _spots)
					res += s.MaxForce;
				return (res);
			}
		}

		public float getMotorStrength()
		{
			float res = 0;
			for (int i = 0; i < _spots.Count; i++)
				res += _spots[i].MaxForce;
			return (res);
		}

		public void showAll()
		{
			for (int i = 0; i < _pieces.Count; i++)
				_pieces[i].Shown = true;
		}

		public void fallAsleep(Piece p, SleepingPack pack, bool DidCheck = false)
		{
			if (p == Heart)
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

		public void setCategorie(int v)
		{
			v %= 31; //31 is not an acceptable categorie because
					//	it's already the map chunks categorie
			Category cat = (Category) (1 << v);
			foreach (Piece p in _pieces)
			{
				p.CollisionCategories = cat;
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

		#endregion

		#region Draw
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

		public void drawDebugTexture(DrawGame dg)
		{
			for (int i = 0; i < _pieces.Count; i++)
				if (_pieces[i].Shown)
					dg.drawTexture(_pieces[i], _pieces[i].ColorValue);
			// add else as in drawDebug()?
		}

		public void draw(DrawGame dg)
		{
			for (int i = 0; i < _pieces.Count; i++)
				dg.draw(_pieces[i], _pieces[i].ColorValue);
				//_pieces[i].draw(dg);
			for (int i = 0; i < _spots.Count; i++)
				//if (_spots[i].GetType() == typeof(PrismaticSpot))
					_spots[i].drawDebug(dg);
		}
		#endregion

		//-----------------REMOVE--------------------

		// For runtime
		public void remove(Piece p)
		{
			if (p == Heart)
				return;
			for (JointEdge i = p.JointList; i != null; i = i.Next)
				_spots.Remove((ISpot)i.Joint); // FIXME should remove link between bodies and spots.
			_pieces.Remove(p);
			_world.RemoveBody(p);
		}

		// For runtime
		public void Destroy(ISpot s)
		{
			_spots.Remove(s);
			_world.RemoveJoint(s.Joint);
		}

		// For lua
		public Boolean hasSpot(string name)
		{
			foreach (var spot in _spots)
			{
				if (name == spot.Name)
					return (true);
			}
			return (false);
		}

		public void ExtractFromWorld()
		{
			Debug.Assert(!Extracted);
			if (Extracted)
				return;
			_extracted = true;
			if (_script != null)
				_script.stop();
			_script = null;
			foreach (ISpot i in _spots)
			{
				_world.RemoveJoint(i.Joint);
			}
			foreach (Piece i in _pieces)
			{
				i.Destroy();
			}
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
			//for (int i = 1; i < _pieces.Count; i++)
			//	_pieces[i].Position = (pos + _pieces[i].Position - Heart.Position);
			Heart.move(pos, this);
		}

		public Vector2 Position
		{
			get
			{
				return Heart.Position;
			}
			set
			{
				move(value);
			}
		}

		public List<SpotApi> GetSpotApi()
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

		public void InitScript()
		{
			/*
			foreach (ISpot i in _spots)
				if (i.GetType() == typeof(PrismaticSpot))
					((PrismaticSpot)i).updateLimit();
			*/
			if (_script == null)
				_script = new RobotLuaScript(GetSpotApi(), _Api, LuaManager.LuaFile(Name));
		}

		public void InitScript(string script)
		{
			if (_script == null)
				_script = new RobotLuaScript(GetSpotApi(), _Api, script, false);
		}

		public void StopScript()
		{
			if (_script != null)
			{
				_script.stop();
				_script = null;
			}
		}

		public int revCount() { return (_revoluteCounter++); }
		public int prisCount() { return (_prismaticCounter++); }

		// Filename for robot & lua
		public string Name { get; set; }

		public bool IsValid()
		{
            return VerifyAllPieceConnected() && AllPiecesValid() && AllDifferentSpots() && AllSpotsConnected() && AllPiecesHaveSpotsThatAreContainedInSurface();
		}
		bool AllPiecesValid()
		{
			return _pieces.All((Piece p) =>
				{
					return p.IsValid();
				});
		}
        bool AllPiecesHaveSpotsThatAreContainedInSurface()
        {
            return _pieces.All((Piece p) =>
            {
                return p.AllSpotsAreContainedInSurface();
            });
        }

		bool VerifyAllPieceConnected()
		{
			return VerifyAllPieceConnectedAux(new List<Piece>(_pieces));
		}
		bool VerifyAllPieceConnectedAux(List<Piece> pieces_to_verify)
		{
			if (pieces_to_verify.Count == 0)
				return true;
			Piece piece_to_verify = pieces_to_verify[0];
			List<Piece> already_explored_pieces = new List<Piece>();
			if (IsPieceConnectedToHeartAux(piece_to_verify, already_explored_pieces ) == false)
				return false;
			foreach (var alreadyExploredPiece in already_explored_pieces)
				pieces_to_verify.Remove(alreadyExploredPiece);

			return VerifyAllPieceConnectedAux(pieces_to_verify);
		}
        bool AllSpotsConnected()
        {
            return Spots.All((ISpot sp) =>
                {
                    Joint joint = (Joint)sp;
                    return joint.BodyA != null && joint.BodyB != null;
                });
        }
        bool AllDifferentSpots()
        {
            return Spots.All((ISpot sp) =>
            {
                Joint joint = (Joint)sp;
                return Spots.All((ISpot sp_other) =>
                {
                    if (sp == sp_other)
                        return true;
                    Joint joint_other = (Joint)sp_other;
                    if (joint.BodyA == joint_other.BodyA && joint.BodyB == joint_other.BodyB ||
                        joint.BodyA == joint_other.BodyB && joint.BodyB == joint_other.BodyA)
                        return false;
                    return true;
                });
            });
		}

		#region Network
		public byte[]	PacketMotor
		{
			get
			{
				byte[] res = new byte[Spots.Count * 4];
				for (int i = 0; i < Spots.Count; i++)
				{
					var tmp = BitConverter.GetBytes(Spots[i].Force);
					for (int j = 0; j < 4; j++)
						res[i * 4 + j] = tmp[j];
				}
				return res;
			}
			set
			{
				byte[] motors = value;
				for (int i = 0; i < Spots.Count; i++)
				{
					Spots[i].Force = System.BitConverter.ToSingle(motors, i * 4);
				}
			}

		}


		#endregion



	}
}