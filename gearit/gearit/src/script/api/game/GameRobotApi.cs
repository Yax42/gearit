using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.game;
using System.Diagnostics;
using gearit.src.robot;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using gearit.src.editor.map;
using gearit.src.Network;

namespace gearit.src.script.api.game
{
    /// <summary>
    /// Lua api for Robot in a game
    /// </summary>
	class GameRobotApi : GameObjectApi
	{
		public Robot __Robot;
		private Dictionary<string, Score.VictoryState> _stateDictionnary;

		public GameRobotApi(Robot robot)
		{
			_stateDictionnary = new Dictionary<string,Score.VictoryState>();
			_stateDictionnary.Add("WIN", Score.VictoryState.Win);
			_stateDictionnary.Add("LOSE", Score.VictoryState.Lose);
			_stateDictionnary.Add("DRAW", Score.VictoryState.Draw);
			_stateDictionnary.Add("UNKNOWN", Score.VictoryState.Unknown);
			_stateDictionnary.Add("STILL", Score.VictoryState.StillInProgress);
			__Robot = robot;
		}

		public bool IsTouching(GameRobotApi robotapi)
		{
			foreach (Body b in __Robot.Pieces)
			{
				foreach (Body b2 in robotapi.__Robot.Pieces)
				{
					for (ContactEdge c = b2.ContactList; c != null; c = c.Next)
					{
						if (c.Other == b && c.Contact.IsTouching)
							return (true);
					}
				}
			}

			return (false);
		}

		public bool IsTouching(GameChunkApi chunkapi)
		{
			MapChunk chunk = chunkapi.__Chunk;
			foreach (Body b in __Robot.Pieces)
			{
				for (ContactEdge c = chunkapi.__Chunk.ContactList; c != null; c = c.Next)
				{
					if (c.Other == b && c.Contact.IsTouching)
						return (true);
				}
			}

			return (false);
		}


		#region Score
		public int IntScore
		{
			get
			{
				return __Robot.Score.IntScore;
			}
			set
			{
				__Robot.Score.IntScore = value;
			}
		}

		public float FloatScore
		{
			get
			{
				return __Robot.Score.FloatScore;
			}
			set
			{
				__Robot.Score.FloatScore = value;
			}
		}

		public void StateScore(string strState)
		{
			var state = _stateDictionnary[strState];
			if (state != null)
			__Robot.Score.State = state;
		}
		#endregion

		public int LastTrigger
		{
			get
			{
				return __Robot.LastTrigger;
			}
		}

		public void DynamicCamera()
		{
			if (GameLuaScript.IsServer)
				NetworkServer.Instance.PushRequest(GameLuaScript.PacketManager.Camera(__Robot.Id, true, Vector2.Zero, 0), __Robot.Id);
			else
				GameLuaScript.Instance.Game.Camera.EnablePositionTracking = true;
		}

		public void StaticCamera(GameObjectApi o, float zoom)
		{
			if (GameLuaScript.IsServer)
				NetworkServer.Instance.PushRequest(GameLuaScript.PacketManager.Camera(__Robot.Id, false, o.Position, zoom), __Robot.Id);
			else
				GameLuaScript.Instance.Game.Camera.StaticCamera(o.Position, zoom);
		}

		public int Id
		{
			get
			{
				return __Robot.Id;
			}
		}

		public int State
		{
			get
			{
				return __Robot.State;
			}
			set
			{
				__Robot.State = value;
			}
		}

		public bool TriggerData(int idx)
		{
			bool res = __Robot.TriggersData[idx];
			__Robot.TriggersData[idx] = false;
			return res;
		}

		public override Vector2 Position
		{
			get
			{
				return __Robot.Position;
			}

			set
			{
				__Robot.Position = value;
				if (GameLuaScript.IsServer)
					NetworkServer.Instance.PushRequest(GameLuaScript.PacketManager.TeleportRobot(__Robot.Id, value));
				else
					GameLuaScript.Instance.Game.Camera.Jump2Target();
			}
		}

		public override Vector2 Speed
		{
			get { return __Robot.Heart.LinearVelocity; }
			set { __Robot.Heart.LinearVelocity = value; }
		}

		public void Reset()
		{
			Body b = __Robot.Heart;
			b.LinearVelocity = Vector2.Zero;
			b.AngularVelocity = 0;
		}
	}
}