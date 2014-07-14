using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticAlgorithm.src.Genome;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using gearit.src.editor;
using System.Xml;
using gearit.src.game;
using System.Diagnostics;
using System.IO;

namespace GeneticAlgorithm.src
{
	#region Comparer
	class ScoreComparer : IComparer<Score[]>
	{
		private int _Idx;
		public ScoreComparer(int idx)
		{
			_Idx = idx;
		}

		public int Compare(Score[] x, Score[] y)
		{
			Score X = x[_Idx];
			Score Y = y[_Idx];
			if (X.IntScore != Y.IntScore)
			{
				return Y.IntScore - X.IntScore;
			}
			else
			{
				return (int) (Y.FloatScore - X.FloatScore);
			}
		}
	}
	class IndividualComparer : IComparer<RawDna>
	{
		public int Compare(RawDna x, RawDna y)
		{
			return y.Rank - x.Rank;
		}
	}
	#endregion

	class LifeManager
	{
		static public World World;

		private int PopulationSize;
		private int NumberOfTests;
		private string DirectoryPath;
		private bool Reset;
		private GeneticGame Game;
		private int Generation;

		private RawDna[] Population;

		public LifeManager()
		{
			World = new World(new Vector2(0, 9.8f));
			SerializerHelper.World = World;

			#region config.xml
			var configXml = new XmlDocument();
			configXml.Load("data/config.xml");
			XmlNodeList node = configXml.SelectNodes("//Config");
			Debug.Assert(node.Count == 1);
			{
				PopulationSize = int.Parse(node[0].Attributes["PopulationSize"].Value);
				Reset = bool.Parse(node[0].Attributes["Reset"].Value);
				NumberOfTests = int.Parse(node[0].Attributes["NumberOfTests"].Value);
				DirectoryPath = "data/" + node[0].Attributes["DirectoryPath"].Value + "/";
			}
			// must add pourcentage of mutations!
			// pourcentage de la population tuee
			#endregion

			Generation = 0;
			Game = new GeneticGame();
			Game.SetMap(DirectoryPath + "map.gim");
			Population = new RawDna[PopulationSize];
			for (int i = 0; i < PopulationSize; i++)
				Population[i] = new RawDna();
			GeneticGame.Init();
		}

		public void SaveBest()
		{
			string path = DirectoryPath + "/trash/" + Generation;

			Debug.Assert(Serializer.SerializeItem(path + ".gir", Population[0].Robot));
			File.WriteAllBytes(path + ".dna.", Population[0].Data);
			File.WriteAllText(path + ".lua", Population[0].Script);
		}

		public void Run()
		{
			while (true)
				Iteration();
		}


		private void Iteration()
		{
			Console.WriteLine("Generation: " + Generation);
			#region RunningGames
			Score[][] scores = new Score[PopulationSize][];
			for (int i = 0; i < PopulationSize; i++)
			{
				scores[i] = new Score[NumberOfTests];
				Population[i].Rank = PopulationSize;
				Game.SetRobot(Population[i]);
				for (int j = 0; j < NumberOfTests; j++)
				{
					scores[i][j] = Game.Run(DirectoryPath + "test_" + j + ".lua", Population[i].Script);
				}
			}
			#endregion

			#region RankingPopulation
			for (int j = 0; j < NumberOfTests; j++)
			{
				ScoreComparer scoreComparer = new ScoreComparer(j);
				Array.Sort(scores, Population, scoreComparer);
				Console.WriteLine("Test " + j + " Score: " + scores[j][0].IntScore + ", " + scores[j][0].FloatScore);
				for (int i = 0; i < PopulationSize; i++)
				{
					if (Population[i].Rank > i)
						Population[i].Rank = i;
				}
			}
			Array.Sort(Population, new IndividualComparer());
			#endregion

			#region DeathBirth
			for (int i = PopulationSize / 2; i < PopulationSize; i++)
			{
				int fatherId = MutationManager.Random.Next(0, PopulationSize / 4);
				int motherId = fatherId;
				while (motherId == fatherId)
					motherId = MutationManager.Random.Next(0, PopulationSize / 2);
				Population[i] = new RawDna(Population[fatherId], Population[motherId]);
			}
			#endregion
			SaveBest();
			Generation++;
		}
	}
}
