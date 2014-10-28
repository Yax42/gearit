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
						if (c.Other == b)
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
					if (c.Other == b)
						return (true);
				}
			}

			return (false);
		}


		#region Score
		public void IntScore(int score)
		{
			__Robot.Score.IntScore = score;
		}

		public void FloatScore(float score)
		{
			__Robot.Score.FloatScore = score;
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
				if (GameLuaScript.IsServer)
					GameLuaScript.PacketManager.TeleportRobot(__Robot.Id, value);
				__Robot.Position = value;
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
			b.Rotation = 0;
			b.LinearVelocity = Vector2.Zero;
			b.AngularVelocity = 0;
		}
	}
}