﻿using System;using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using gearit.src.editor;
using System.Xml;
using gearit.src.game;
using System.Diagnostics;
using System.IO;
using gearit.src;
using gearit.xna;
using gearit.src.robot;
using gearit.src.GeneticAlgorithm.Genome;
using gearit.src.output;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.GeneticAlgorithm
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
			return Compare(X, Y);
		}

		static public int Compare(Score X, Score Y)
		{
			if (X.IntScore != Y.IntScore)
			{
				return Y.IntScore - X.IntScore;
			}
			else
			{
				return (int)(X.FloatScore - Y.FloatScore);
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

	public class LifeManager : GameScreen
	{
		static public World World;

		private bool IsVisual = true;
		private int PopulationSize;
		private int NumberOfTests;
		private string DirectoryPath;
		private bool Reset;
		private GeneticGame Game;
		private int Generation;
		private Robot CurrentBest = null;
		private Robot TestedRobot = null;
		private int CurrentTested = 0;
		private bool IsGameRunning = false;

		private RawDna[] Population;

		//Graphics

		public Camera2D Camera;
		public DrawGame DrawGame;

		public LifeManager() : base(true)
		{
			TransitionOnTime = TimeSpan.FromSeconds(0.75);
			TransitionOffTime = TimeSpan.FromSeconds(0.75);
			HasCursor = true;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			World = new World(new Vector2(0, 9.8f));
			SerializerHelper.World = World;

			#region config.xml
			var configXml = new XmlDocument();
			configXml.Load("data/genetic/config.xml");
			XmlNodeList node = configXml.SelectNodes("//Config");
			Debug.Assert(node.Count == 1);
			{
				PopulationSize = int.Parse(node[0].Attributes["PopulationSize"].Value);
				Reset = bool.Parse(node[0].Attributes["Reset"].Value);
				NumberOfTests = int.Parse(node[0].Attributes["NumberOfTests"].Value);
				DirectoryPath = "data/genetic/" + node[0].Attributes["DirectoryPath"].Value + "/";
			}
			// must add pourcentage of mutations!
			// pourcentage de la population tuee
			#endregion


			ScreenManager.Game.ResetElapsedTime();
			DrawGame = new DrawGame(ScreenManager.GraphicsDevice);
			Camera = new Camera2D(ScreenManager.GraphicsDevice);

			RawDna.CycleNumber = NumberOfTests;
			Generation = 0;
			Game = new GeneticGame();
			Game.SetMap(DirectoryPath + "map.gim");
			Population = new RawDna[PopulationSize];
			for (int i = 0; i < PopulationSize; i++)
				Population[i] = new RawDna();
			if (!Directory.Exists(DirectoryPath + "/trash/"))
				Directory.CreateDirectory(DirectoryPath + "/trash/");
		}

		public string GetTitle()
		{
			return "GeneticAlgorithm";
		}

		public string GetDetails()
		{
			return ("");
		}

		public void SaveBest(string path)
		{
			bool ok = Serializer.SerializeItem(path + ".gir", CurrentBest);
			Debug.Assert(ok);
			File.WriteAllBytes(path + ".dna.", Population[0].Data);
			File.WriteAllText(path + ".lua", Population[0].Script);
		}

		private void HandleInput()
		{
			if (Input.justPressed(Keys.Space))
				IsVisual = !IsVisual;
			if (Input.justPressed(MouseKeys.WHEEL_DOWN))
				Camera.zoomIn();
			if (Input.justPressed(MouseKeys.WHEEL_UP))
				Camera.zoomOut();
		}

		int KCount = 0;
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			Camera.Update(gameTime);
			HandleInput();

			if (!IsGameRunning)
			{
				#region RunningGames
				Population[CurrentTested].Rank = PopulationSize;
				TestedRobot = Population[CurrentTested].GeneratePhenotype();
				Game.SetRobot(TestedRobot);
				for (int j = 0; j < NumberOfTests; j++)
				{
					if (KCount == 0)
						Population[CurrentTested].Scores[j] = new Score();
					Score cur =  Game.Run(DirectoryPath + "test_" + j + ".lua", Population[CurrentTested].Script);
					Population[CurrentTested].Scores[j].Add(cur);

				}
#if false
				OutputManager.LogMessage("Generation[" + Generation + "]."
										+ "Robot[" + CurrentTested + "] = "
										+ Population[CurrentTested].Scores[0].FloatScore + "|"
										+ Population[CurrentTested].Scores[0].IntScore);
#endif
				KCount++;
				if (KCount < 3)
					return;
				if (CurrentTested == 0)
					OutputManager.LogMessage(0 + "");
				else if (CurrentTested % 10 == 0)
					OutputManager.LogMerge(" " + CurrentTested);
				Population[CurrentTested].Scores[0].Average();
				CurrentTested++;
				KCount = 0;

				if (CurrentTested < PopulationSize)
					return;
				else
					CurrentTested = 0;
				#endregion

				#region RankingPopulation
				for (int j = 0; j < NumberOfTests; j++)
				{
					Array.Sort(Population, new ScoreComparer(j));
					for (int i = 0; i < PopulationSize; i++)
					{
						Population[i].LifeTimeLeft--;
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
				for (int i = 0; i < PopulationSize; i++)
				{
					if (Population[i].LifeTimeLeft > 0 && i < PopulationSize / 2)
						continue;
					int fatherId = MutationManager.Random.Next(0, PopulationSize / 2);
					int motherId = fatherId;
					while (motherId == fatherId)
						motherId = MutationManager.Random.Next(0, PopulationSize / 2);
					if (fatherId > motherId)
					{
						int tmp = fatherId;
						fatherId = motherId;
						motherId = tmp;
					}
					Population[i] = new RawDna(Population[fatherId], Population[motherId]);
				}

				#endregion
				string path = DirectoryPath + "/trash/" + Generation;
				CurrentBest = Population[0].GeneratePhenotype();
				SaveBest(path);
				OutputManager.LogMessage("Best[" + Generation + "] = "
											+ Population[0].Scores[0].FloatScore + "|"
											+ Population[0].Scores[0].IntScore);
				if (IsVisual)
				{
					Game.SetRobot(CurrentBest);
					Game.ManualInit(DirectoryPath + "test_" + 0 + ".lua", Population[0].Script);
					Camera.TrackingBody = CurrentBest.Heart;
					IsGameRunning = true;
				}
				else
					CurrentBest.ExtractFromWorld();
				Generation++;
			}
			else // if (IsGameRunning)
			{
				if (!Game.ManualUpdate())
				{
#if false
					Drawing = true;
					CurrentBest = Population[0].GeneratePhenotype();
					Game.SetRobot(CurrentBest);
					Game.ManualInit(DirectoryPath + "test_" + 0 + ".lua", Population[0].Script);
					Camera.TrackingBody = CurrentBest.Heart;
					IsGameRunning = true;
#else
					IsGameRunning = false;
#endif
				}
				else
					Drawing = false;
			}
		}
		bool Drawing = true;

		public override void Draw(GameTime gameTime)
		{
			if (IsGameRunning)
			{
				//if (Drawing)
				{
					ScreenManager.GraphicsDevice.Clear(Color.LightYellow);

					DrawGame.Begin(Camera);
#if true
					Game.Robot.drawDebug(DrawGame);
					Game.Map.DrawDebug(DrawGame, true);
#else
					foreach (Body b in World.BodyList)
						DrawGame.draw(b, Color.Pink);
#endif
					DrawGame.End();
				}
			}
			else if (IsVisual)
			{
				ScreenManager.GraphicsDevice.Clear(Color.AntiqueWhite);

				DrawGame.Begin(Camera);
				if (TestedRobot != null && !TestedRobot.Extracted)
				{
					TestedRobot.move(Vector2.Zero);
					//TestedRobot.Heart.Rotation = (Vector2.Zero);
					Camera.TrackingBody = TestedRobot.Heart;
					TestedRobot.drawDebug(DrawGame);
				}
				DrawGame.End();
			}
			else
			{
				ScreenManager.GraphicsDevice.Clear(Color.BlueViolet);
			}

			base.Draw(gameTime);
		}
	}
}
