using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.robot;
using Microsoft.Xna.Framework;

namespace gearit.src.GeneticAlgorithm.Genome
{
	class HardwareGenome : LowLevelGenome
	{
		public HardwareGenome(RawDna rawDna, int beginning)
			: base(rawDna, beginning)
		{
		}

		public static void Affect(RawDna rawDna, int beginning)
		{
			var cur = new HardwareGenome(rawDna, beginning);
			cur.run();
		}

		private void run()
		{
			CreatePiece();
		}

		private void CreateHeart()
		{
			//Heart carré et de poids fixe dans un premier temps.
		}

		private void CreatePiece() // so far, costs 45 DNA atoms
		{
			int PieceLinkedTo = NextByteMax(m_RawDna.Robot.Pieces.Count);
			Piece p1 = m_RawDna.Robot.Pieces[PieceLinkedTo];
			Piece p2;
			bool isWheel = NextBool(0.4f);
			bool isAnchor1Center = NextBool(0.8f);
			bool isAnchor2Center = NextBool(0.8f);

			float maxAngle = (float) (NextFloat % Math.PI);
			maxAngle = maxAngle * maxAngle;
			float minAngle = (float) (NextFloat % Math.PI);
			minAngle = -minAngle * minAngle;
			bool limitEnabled = NextBool(0.3f);

			float size = NextAbsRange1 * kMaxSize + 0.1f;
			Vector2 anchor1 = NextVector2;
			Vector2 anchor2 = NextVector2;
			float weight = NextAbsRange1 * 15 + 1;
			float maxForce = NextAbsRange1 * 50; // probablement a trouver empiriquement la bonne valeur a mettre
			float angle = NextFloat;


			if (isWheel)
				p2 = new Wheel(m_RawDna.Robot, size); //Je met position zéro parce que normalement elle reset au moment du link
			else
			{
				p2 = new Rod(m_RawDna.Robot, 2);
				anchor2.Y = 0;
			}

			if (isAnchor1Center)
				anchor1 = p1.ShapeLocalOrigin();
			else
			{
				anchor1 = anchor1 * size;
				while (!p1.LocalContain(anchor1))
					anchor1 *= 0.7f;
			}

			if (isAnchor2Center)
				anchor2 = p2.ShapeLocalOrigin();
			else
			{
				anchor2 = anchor2 * size;
				while (!p2.LocalContain(anchor2))
					anchor2 *= 0.7f;
			}
			p2.Weight = weight;
			maxForce *= (p1.Weight < p2.Weight ? p1.Weight : p2.Weight) * 0.7f;
			RevoluteSpot spot = new RevoluteSpot(m_RawDna.Robot, p1, p2, anchor1, anchor2);
			spot.MaxForce = maxForce;
			spot.rotate(p1, angle, m_RawDna.Robot);
			spot.MaxAngle = maxAngle;
			spot.MinAngle = minAngle;
			spot.SpotLimitEnabled = limitEnabled;

			for (int i = 0; i < RawDna.CycleNumber; i++)
				m_RawDna.GenerateSoftwarePheonotype(NextAbsInt % RawDna.SoftwareChromosomeNumber, i, spot);
		}
	}
}
