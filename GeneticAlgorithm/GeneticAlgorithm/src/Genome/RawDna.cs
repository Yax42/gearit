using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.robot;
using FarseerPhysics.Dynamics;
using gearit.src.game;

namespace GeneticAlgorithm.src.Genome
{
	class RawDna
	{
		public const int MaxPieces = 32;
		public const int MaxSequences = 4;
		public const int MaxSequenceSize = 255;
		public const int SoftwareChromosomeWeight = MaxSequences * 5 + 1;
		public static int HardwareChromosomeWeight { get { return 48 + CycleNumber * 4; } }
		public static int TotalHardwareChromosomeWeight { get { return HardwareChromosomeNumber * HardwareChromosomeWeight; } }
		public const int HardwareChromosomeNumber = MaxPieces;
		public static int SoftwareChromosomeNumber { get { return MaxPieces * CycleNumber * 2; } }
		public static int TotalSoftwareChromosomeWeight { get { return SoftwareChromosomeNumber * SoftwareChromosomeWeight; } }
						// le *2 c'est pour qu'il y est une partie du génome
						// qui soit pas utilisé et que des séquences d'adn
						// puissent se transmettre silencieusement.
		public static int TotalDnaWeight { get { return 1 + TotalHardwareChromosomeWeight + TotalSoftwareChromosomeWeight; } }
		public const int FirstHardwareIndex = 1;
		public static int FirstSoftwareIndex { get { return 1 + TotalHardwareChromosomeWeight; } }


		static public int CycleNumber;

		public Score[] Scores = new Score[CycleNumber];
		public Byte[] Data;
		public Robot Robot;
		public string Script;
		public int Rank;

		public RawDna()
		{
			Data = new Byte[TotalDnaWeight];
			MutationManager.Random.NextBytes(Data);
			Data[0] = 2;
		}

		public RawDna(Byte[] data)
		{
			Data = data;
		}

		public RawDna(RawDna other)
		{
			Data = new Byte[TotalDnaWeight];
			Data[0] = MutationManager.PiecesNumber(other.Data[0], other.Data[0], 1.0f);
		}

		public RawDna(RawDna father, RawDna mother)
		{
			Data = new Byte[TotalDnaWeight];
			double fatherForce = 0.5f;// MutationManager.Random.NextDouble();

			Data[0] = MutationManager.PiecesNumber(father.Data[0], mother.Data[0], fatherForce);
			MutationManager.Cross(
					father.Data,
					mother.Data,
					Data,
					FirstHardwareIndex,
					HardwareChromosomeNumber,
					HardwareChromosomeWeight,
					fatherForce);
			MutationManager.Cross(
					father.Data,
					mother.Data,
					Data,
					FirstSoftwareIndex,
					SoftwareChromosomeNumber,
					SoftwareChromosomeWeight,
					fatherForce);
		}

		public Robot GeneratePhenotype()
		{
			Robot = new Robot(LifeManager.World);
			Script = "";
			int numberOfPieces = Data[0];
			for (int i = 0; i < numberOfPieces; i++)
				GenerateHardwarePheonotype(i);
			var res = Robot;
			Robot = null;
			return res;
		}

		public void GenerateSoftwarePheonotype(int id, int cycleId, ISpot spot)
		{
			int actualId = FirstSoftwareIndex + id * SoftwareChromosomeWeight;
			SoftwareGenome.Affect(this, actualId, cycleId, spot);
		}

		public void GenerateHardwarePheonotype(int id)
		{
			HardwareGenome.Affect(this, FirstHardwareIndex + id * HardwareChromosomeWeight);
		}
	}
}
