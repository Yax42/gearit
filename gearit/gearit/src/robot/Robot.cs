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
			SList = new List<RevoluteSpot>();
			PList = new List<Piece>();
		}
		public List<RevoluteSpot> SList;
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
		private List<RevoluteSpot> _spots;
		public List<RevoluteSpot> Spots
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
		public int Id
		{
			get;
			set;
		}
		public World _world;
		public bool[] TriggersData;
		private int _LastTrigger;
		public Score Score = new Score();
		public int State = 0;
		public bool Extracted
		{
			get;
			private set;
		}

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

		public bool IsInEditor = false;

		public Robot(World world, bool isInEditor = false)
		{
			IsInEditor = isInEditor;
			_world = world;
			Id = _robotIdCounter++;
			_pieces = new List<Piece>();
			_spots = new List<RevoluteSpot>();
			new Heart(this);
			//Console.WriteLine("Robot created.");
			_script = null;
			InitTriggerData();
			_Api = new RobotStateApi(this);
		}

		#region Serialization
		public Robot(SerializationInfo info, StreamingContext ctxt)
		{
			IsInEditor = SerializerHelper.IsNextRobotInEditor;
			SerializerHelper.CurrentRobot = this;
			SerializerHelper.Ptrmap.Clear();
			_world = SerializerHelper.World;
			Name = Path.GetFileNameWithoutExtension(SerializerHelper.CurrentPath);// (string)info.GetValue("Name", typeof(string));
			_revoluteCounter = (int)info.GetValue("RevCount", typeof(int));
			_prismaticCounter = (int)info.GetValue("SpotCount", typeof(int));
			this._pieces = (List<Piece>)info.GetValue("Pieces", typeof(List<Piece>));
			//foreach (Piece p in _pieces)
			//SerializerHelper._world.AddBody((Body)p);

			this._spots = (List<RevoluteSpot>)info.GetValue("Spots", typeof(List<RevoluteSpot>));
			// foreach (RevoluteSpot s in _spots)
			//	 SerializerHelper._world.AddJoint((Joint)s);

			Id = _robotIdCounter++;
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
			info.AddValue("Spots", _spots, typeof(List<RevoluteSpot>));
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
			if (Extracted)
				return;
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

		public int FindFirstFreeSpotNameId()
		{
			int res = -1;
			bool ok = false;
			while (!ok)
			{
				ok = true;
				res++;
				string name = "spot" + res;
				foreach (RevoluteSpot s in Spots)
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

		public void ResetActEnds()
		{
			ResetAct();
			foreach (Piece p in _pieces)
				if (p.GetType() == typeof(Rod))
				{
					((Rod)p).DidAct_EndA = false;
					((Rod)p).DidAct_EndB = false;
				}
		}

		public void addSpot(RevoluteSpot spot)
		{
			_spots.Add(spot);
		}

		public void addPiece(Piece piece)
		{
			_pieces.Add(piece);
		}

		public Piece ClosePiece(Vector2 p)
		{
			float minFound = 1000000f;
			Piece res = Heart; 
			for (int i = _pieces.Count - 1; i > 0; i--)
				if (_pieces[i].Shown)
				{
					float curDist = _pieces[i].DistanceSquared(p);
					if (curDist < minFound)
					{
						minFound = curDist;
						res = Pieces[i];
					}
				}
			return res;
		}

		public Piece GetPiece(Vector2 p)
		{
			for (int i = _pieces.Count - 1; i > 0; i--)
				if (_pieces[i].Shown && _pieces[i].Contain(p))
					return _pieces[i];
			if (Heart.Shown && Heart.Contain(p))
				return Heart;
			return ClosePiece(p);
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
				foreach (RevoluteSpot s in _spots)
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
				RevoluteSpot spot = (RevoluteSpot)i.Joint;
				spot.fallAsleep(p);
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
		public void fallAsleep(RevoluteSpot s, SleepingPack pack)
		{
			s.fallAsleep(null);
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
			foreach (RevoluteSpot i in pack.SList)
				i.wakeUp();
			pack.SList.Clear();
		}

		public void forget(RevoluteSpot s)
		{
			_spots.Remove(s);
		}

		#endregion

		#region Draw
		public void drawDebug(DrawGame dg)
		{
			for (int i = 0; i < _pieces.Count; i++)
				dg.draw(_pieces[i], _pieces[i].ColorValue, _pieces[i].Shown ? 128 : 32);
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
				_spots.Remove((RevoluteSpot)i.Joint); // FIXME should remove link between bodies and spots.
			_pieces.Remove(p);
			_world.RemoveBody(p);
		}

		// For runtime
		public void Destroy(RevoluteSpot s)
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
			Extracted = true;
			if (_script != null)
				_script.stop();
			_script = null;
			foreach (RevoluteSpot i in _spots)
			{
				_world.RemoveJoint(i.Joint);
			}
			foreach (Piece i in _pieces)
			{
				i.Destroy(_world);
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
			Heart.move(pos, false, true);
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

		public List<RevoluteApi> GetSpotApi()
		{
			List<RevoluteApi> res = new List<RevoluteApi>();
			for (int i = 0; i < _spots.Count; i++)
				res.Add(new RevoluteApi(_spots[i]));
			return (res);
		}

		public void InitScript()
		{
			/*
			foreach (RevoluteSpot i in _spots)
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
            return Spots.All((RevoluteSpot sp) =>
                {
                    Joint joint = (Joint)sp;
                    return joint.BodyA != null && joint.BodyB != null;
                });
        }
        bool AllDifferentSpots()
        {
            return Spots.All((RevoluteSpot sp) =>
            {
                Joint joint = (Joint)sp;
                return Spots.All((RevoluteSpot sp_other) =>
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
	}
}