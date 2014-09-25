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
	public class GameRobotApi : GameObjectApi
	{
		private Robot _Robot;
		private Dictionary<string, Score.VictoryState> _stateDictionnary;

		public GameRobotApi(Robot robot)
		{
			_stateDictionnary = new Dictionary<string,Score.VictoryState>();
			_stateDictionnary.Add("WIN", Score.VictoryState.Win);
			_stateDictionnary.Add("LOSE", Score.VictoryState.Lose);
			_stateDictionnary.Add("DRAW", Score.VictoryState.Draw);
			_stateDictionnary.Add("UNKNOWN", Score.VictoryState.Unknown);
			_stateDictionnary.Add("STILL", Score.VictoryState.StillInProgress);
			_Robot = robot;
		}

		public bool IsTouching(GameChunkApi chunkapi)
		{
			MapChunk chunk = chunkapi.__Chunk;
			foreach (Body b in _Robot.Pieces)
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
			_Robot.Score.IntScore = score;
		}

		public void FloatScore(float score)
		{
			_Robot.Score.FloatScore = score;
		}

		public void StateScore(string strState)
		{
			var state = _stateDictionnary[strState];
			if (state != null)
			_Robot.Score.State = state;
		}
		#endregion

		public int LastTrigger
		{
			get
			{
				return _Robot.LastTrigger;
			}
		}

		public int State
		{
			get
			{
				return _Robot.State;
			}
			set
			{
				_Robot.State = value;
			}
		}

		public bool TriggerData(int idx)
		{
			bool res = _Robot.TriggersData[idx];
			_Robot.TriggersData[idx] = false;
			return res;
		}

		public override Vector2 Position
		{
			get
			{
				return _Robot.Position;
			}

			set
			{
				_Robot.Position = value;
			}
		}
	}
}