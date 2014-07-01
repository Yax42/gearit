using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gearit.src.game
{
	public class Score
	{
		public enum VictoryState
		{
			StillInProgress,
			Win,
			Lose,
			Draw,
			Unknown,
		};

		public VictoryState State = VictoryState.StillInProgress;
		public float FloatScore = 0;
		public int IntScore = 0;
	}
}
