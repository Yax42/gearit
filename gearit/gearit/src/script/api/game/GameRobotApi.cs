using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.game;
using System.Diagnostics;
using gearit.src.robot;
using Microsoft.Xna.Framework;

namespace gearit.src.script.api.game
{
	class GameRobotApi : GameObjectApi
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