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
		public int Count = 0;

		public void Add(Score s)
		{
			if (s.IntScore != IntScore)
			{
				if (s.IntScore == 1)
				{
					IntScore = 1;
					FloatScore = s.FloatScore;
					Count = 1;
				}
			}
			else
			{
				FloatScore += s.FloatScore;
				Count++;
			}
		}
		public void Average()
		{
			FloatScore /= Count;
			Count = 1;
		}
	}
}
