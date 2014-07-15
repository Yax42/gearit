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
using gearit.src;
using gearit.xna;

namespace GeneticAlgorithm.src
{
	#region Comparer
	class ScoreComparer : IComparer<RawDna>
	{
		private int _Idx;
		public ScoreComparer(int idx)
		{
			_Idx = idx;
		}

		public int Compare(RawDna x, RawDna y)
		{
			Score X = x.Scores[_Idx];
			Score Y = y.Scores[_Idx];
			if (X.IntScore != Y.IntScore)
			{
				return Y.IntScore - X.IntScore;
			}
			else
			{
				return (int) (X.FloatScore - Y.FloatScore);
			}
		}
	}
	class IndividualComparer : IComparer<RawDna>
	{
		public int Compare(RawDna x, RawDna y)
		{
			return x.Rank - y.Rank;
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

		//Graphics
		private ScreenManager _screenManager;



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

			RawDna.CycleNumber = NumberOfTests;
			Generation = 0;
			Game = new GeneticGame();
			Game.SetMap(DirectoryPath + "map.gim");
			Population = new RawDna[PopulationSize];
			for (int i = 0; i < PopulationSize; i++)
				Population[i] = new RawDna();
		}

		public void SaveBest()
		{
			string path = DirectoryPath + "/trash/" + Generation;

			bool ok = Serializer.SerializeItem(path + ".gir", Population[0].GeneratePhenotype());
			Debug.Assert(ok);
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
			//Console.WriteLine("Generation: " + Generation);
			#region RunningGames
			for (int i = 0; i < PopulationSize; i++)
			{
				Population[i].Rank = PopulationSize;
				Game.SetRobot(Population[i]);
				for (int j = 0; j < NumberOfTests; j++)
				{
					Population[i].Scores[j] = Game.Run(DirectoryPath + "test_" + j + ".lua", Population[i].Script);
				}
			}
			#endregion

			#region RankingPopulation
			for (int j = 0; j < NumberOfTests; j++)
			{
				Array.Sort(Population, new ScoreComparer(j));
				for (int i = 0; i < PopulationSize; i++)
				{
					if (i < Population[i].Rank)
						Population[i].Rank = i;
					Console.Write(Population[i].Scores[0].FloatScore + "  ");
				}
				Console.Write("\n");
				//Console.WriteLine("Test " + j + " Score: " + Population[0].Scores[j].IntScore + ", " + Population[0].Scores[j].FloatScore);
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
