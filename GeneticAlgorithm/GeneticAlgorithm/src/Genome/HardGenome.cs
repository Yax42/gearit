using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit;
using System.Diagnostics;
using gearit.src.robot;
using Microsoft.Xna.Framework;

namespace GeneticAlgorithm.src.Genome
{
	class HardGenome : Genome
	{
		public HardGenome(Robot robot) : base(robot)
		{
		}

		public void CreateHeart()
		{
			//Heart carré et de poids fixe dans un premier temps.
		}

		public void CreatePiece()
		{
			Piece p1 = NextPiece;
			Piece p2;
			bool isWheel = NextBool(0.4f);
			bool isAnchor1Center = NextBool(0.8f);
			bool isAnchor2Center = NextBool(0.8f);
			float size = NextAbsRange1 * kMaxSize + 0.1f;
			Vector2 anchor1 = NextVector2;
			Vector2 anchor2 = NextVector2;
			float weight = NextAbsRange1 * 15 + 1;
			float maxForce = NextAbsRange1;
			float angle = NextFloat;

			if (isWheel)
				p2 = new Wheel(m_Robot, size); //Je met position zéro parce que normalement elle reset au moment du link
			else
			{
				p2 = new Rod(m_Robot, 2);
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
			maxForce *= (p1.Weight < p2.Weight ? p1.Weight : p2.Weight);
			RevoluteSpot spot = new RevoluteSpot(m_Robot, p1, p2, anchor1, anchor2);
			spot.MaxForce = maxForce;
			spot.rotate(p1, angle);
		}
	}
}
