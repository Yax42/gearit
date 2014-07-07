﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit;
using gearit.src.robot;
using Microsoft.Xna.Framework;

namespace GeneticAlgorithm.src.Genome
{
	class LowLevelGenome
	{
		internal const float kMaxSize = 3;
		internal RawDna m_RawDna;
		internal int m_Current;

		internal LowLevelGenome(RawDna rawDna, int beginning)
		{
			m_RawDna = rawDna;
			m_Current = beginning;
		}

		private int NextByteIndex
		{
			get
			{
				int res = m_Current;
				m_Current++;
				return res;
			}
		}

		private int NextFloatIndex
		{
			get
			{
				int res = m_Current;
				m_Current += 4;
				return res;
			}
		}

		internal Byte NextByte
		{
			get
			{
				return m_RawDna.Data[NextByteIndex];
			}
		}

		internal bool NextBool(float trueChance)
		{
			return (NextByteIndex <= trueChance * 256);
		}


		internal float NextFloat
		{
			get
			{
				return System.BitConverter.ToSingle(m_RawDna.Data, NextFloatIndex);
			}
		}

		internal int NextInt
		{
			get
			{
				return Math.Abs(System.BitConverter.ToInt32(m_RawDna.Data, NextFloatIndex));
			}
		}

		internal float NextAbsRange1
		{
			get
			{
				return Math.Abs(NextRange1);
			}
		}

		internal float NextRange1
		{
			get
			{
				float res = NextFloat;
				return res - (float)Math.Floor(res);
			}
		}

		internal Vector2 NextVector2
		{
			get
			{
				return new Vector2(NextRange1, NextRange1);
			}
		}
	}
}
