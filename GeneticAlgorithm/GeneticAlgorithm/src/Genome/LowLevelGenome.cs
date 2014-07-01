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
		private Byte[] m_Data;
		private int m_Current;

		internal LowLevelGenome(Byte[] data)
		{
			m_Current = 0;
			m_Data = data;
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
				return m_Data[NextByteIndex];
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
				return System.BitConverter.ToSingle(m_Data, NextFloatIndex);
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
