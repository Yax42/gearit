using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticAlgorithm.src.Genome;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using gearit.src.editor;
using System.Xml;

namespace GeneticAlgorithm.src
{
	class LifeManager
	{
		static public World World;

		private int PopulationSize;
		private int NumberOfTests;
		private string DirectoryPath;
		private bool Reset;
		private GeneticGame Game;

		private RawDna[] Population;

		public LifeManager()
		{
			World = new World(new Vector2(0, 9.8f));
			SerializerHelper.World = World;

			var configXml = new XmlDocument();
			configXml.Load("GeneticConfig.xml");
			{
				XmlNodeList node = configXml.SelectNodes("//PopulationSize");
				if (node.Count == 0)
					PopulationSize = 50;
				else
					PopulationSize = int.Parse(node[0].Attributes["Value"].Value);
			}
			{
				XmlNodeList node = configXml.SelectNodes("//Reset");
				if (node.Count == 0)
					Reset = true;
				else
					Reset = bool.Parse(node[0].Attributes["Value"].Value);
			}
			{
				XmlNodeList node = configXml.SelectNodes("//NumberOfTests");
				if (node.Count == 0)
					NumberOfTests = 1;
				else
					NumberOfTests = int.Parse(node[0].Attributes["Value"].Value);
			}
			{
				XmlNodeList node = configXml.SelectNodes("//DirectoryPath");
				if (node.Count == 0)
					DirectoryPath = "genetic/0/";
				else
					DirectoryPath = node[0].Attributes["Value"].Value;
			}

			Game = new GeneticGame();
			Game.SetMap(DirectoryPath + "map.gim");
			Population = new RawDna[PopulationSize];
			for (int i = 0; i < PopulationSize; i++)
				Population[i] = new RawDna();
		}

		public void Iteration()
		{
			for (int i = 0; i < PopulationSize; i++)
			{
				Game.SetRobot(Population[i]);
				for (int j = 0; j < NumberOfTests; j++)
				{
					Game.Run(DirectoryPath + "GameMaster_" + j + ".lua");
				}
			}
		}
	}
}
