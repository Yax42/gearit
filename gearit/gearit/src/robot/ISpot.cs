﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.robot;
using gearit.src;

namespace gearit
{
	interface ISpot : ISerializable
	{
		void swap(Piece p1, Piece p2, Vector2 anchor);

		void swap(Piece p1, Piece p2);

		void moveAnchor(Piece p, Vector2 anchor);

		float MaxForce { get; set; }

		float Force { get; set; }

		string Name { get; set; }

		Color ColorValue { get; set; }

		void draw(DrawGame game);

		Vector2 getLocalAnchor(Piece piece);

		Vector2 getWorldAnchor(Piece piece);

		void moveLocal(Piece p, Vector2 pos);
	}
}