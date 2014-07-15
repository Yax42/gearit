using System;
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
			return (NextByte <= trueChance * 256);
		}


		internal float NextFloat
		{
			get
			{
				return (NextInt % 1000000) / 1000.0f;
			}
		}

		internal int NextInt
		{
			get
			{
				return System.BitConverter.ToInt32(m_RawDna.Data, NextFloatIndex);
			}
		}

		internal int NextAbsInt
		{
			get
			{
				return Math.Abs(NextInt);
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
				return (NextInt % 1000) / 1000.0f;
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
