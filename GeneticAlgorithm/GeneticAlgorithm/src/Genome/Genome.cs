using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit;
using gearit.src.robot;
using Microsoft.Xna.Framework;

namespace GeneticAlgorithm.src.Genome
{
	class Genome
	{
		public const float kMaxSize = 3;
		private Byte[] m_Data;
		internal Robot m_Robot;
		private int m_Current;

		public Genome(Robot robot)
		{
			m_Current = 0;
			m_Data = new Byte[32];
			m_Robot = robot;
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

		public Byte NextByte
		{
			get
			{
				return m_Data[NextByteIndex];
			}
		}

		public bool NextBool(float trueChance)
		{
			return (NextByteIndex <= trueChance * 256);
		}

		public Piece NextPiece
		{
			get
			{
				return m_Robot.Pieces[m_Data[NextByteIndex] % m_Robot.Pieces.Count];
			}
		}

		public RevoluteSpot NextSpot
		{
			get
			{
				return (RevoluteSpot)m_Robot.Spots[m_Data[NextByteIndex] % m_Robot.Spots.Count];
			}
		}

		public float NextFloat
		{
			get
			{
				return System.BitConverter.ToSingle(m_Data, NextFloatIndex);
			}
		}

		public float NextAbsRange1
		{
			get
			{
				return Math.Abs(NextRange1);
			}
		}

		public float NextRange1
		{
			get
			{
				float res = NextFloat;
				return res - (float)Math.Floor(res);
			}
		}

		public Vector2 NextVector2
		{
			get
			{
				return new Vector2(NextRange1, NextRange1);
			}
		}
	}
}
