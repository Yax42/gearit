﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gearit.src.GeneticAlgorithm.Genome
{
	class MutationManager
	{
		static public Random Random = new Random();
		static private Byte[] RandomBuffer = new Byte[100];
		static private int RandomBufferIdx = 100;

		const double PourcentageMutation = 2f / 100.0f;
		const double PourcentageSegmentSwap = 0.4f / 100.0f;
		const double PourcentageSegmentReplace = 0.2f / 100.0f;

		static public Byte RandomByte()
		{
			if (RandomBufferIdx >= 100)
			{
				Random.NextBytes(RandomBuffer);
				RandomBufferIdx = 0;
			}
			int tmp = RandomBufferIdx;
			RandomBufferIdx++;
			return RandomBuffer[tmp];
		}

		public static void Cross(
					Byte[] father,
					Byte[] mother,
					Byte[] son,
					int start,
					int nbSegment,
					int segmentSize,
					double fatherForce)
		{
			for (int i = 0; i < nbSegment; i++)
			{
				Byte[] winningParent = mother;
				if (Random.NextDouble() < fatherForce)
					winningParent = father;

				for (int j = 0; j < segmentSize; j++)
				{
					if (Random.NextDouble() < PourcentageMutation)
						son[start + i * segmentSize + j] = RandomByte();
					else
						son[start + i * segmentSize + j] = winningParent[start + i * segmentSize + j];
				}
			}
			for (int i = 0; i < nbSegment; i++)
			{
				if (Random.NextDouble() < PourcentageSegmentSwap)
					SwapExclusive(son,
						start + i * segmentSize,
						start + Random.Next(0, nbSegment) * segmentSize,
						segmentSize);
				if (Random.NextDouble() < PourcentageSegmentReplace)
					ReplaceExclusive(son,
						start + i * segmentSize,
						start + Random.Next(0, nbSegment) * segmentSize,
						segmentSize);
			}
		}

		public static void SwapExclusive(Byte[] data, int start1, int start2, int size)
		{
			if (start2 == start1)
				return;
			for (int i = 0; i < size; i++)
			{
				Byte tmp = data[start1 + i];
				data[start1 + i] = data[start2 + i];
				data[start2 + i] = tmp;
			}
		}

		public static void ReplaceExclusive(Byte[] data, int victim, int killer, int size)
		{
			if (victim == killer)
				return;
			for (int i = 0; i < size; i++)
			{
				data[victim + i] = data[killer + i];
			}
		}

		public static Byte PiecesNumber(
					Byte father,
					Byte mother,
					double fatherForce)
		{
			Byte res;
			if (Random.NextDouble() < fatherForce)
				res = father;
			else
				res = mother;
			if (Random.NextDouble() < PourcentageMutation)
				res = (Byte) (((int) res) + Random.Next(-1, 1));
			if (res < 1)
				return 1;
			return res;
		}
	}
}
