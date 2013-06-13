﻿using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace gearit
{
    class Heart : Piece
    {
        private Vertices _vertices; //Le PolygonShape sera composé de ces vertices (elles sont les cotés du polygone).

        public Heart(Robot robot) :
            base(robot)
        {
            _vertices = PolygonTools.CreateRectangle(50f / 2, 50f / 2);
            _shape = new PolygonShape(_vertices, 50f);
            _fix = CreateFixture(_shape, null);
            Console.WriteLine("Heart created.");
        }

        public override void draw()
        {
        }
    }
}