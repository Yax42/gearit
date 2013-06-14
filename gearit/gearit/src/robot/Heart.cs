﻿using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;

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
            _tex = robot.getAsset().TextureFromShape(_shape, MaterialType.Blank, Color.White, 1f);
            Console.WriteLine("Heart created.");
            ColorValue = Color.Red;
        }

        public override void draw(SpriteBatch batch)
        {
            batch.Draw(_tex, ConvertUnits.ToDisplayUnits(this.Position), null, Color.White, this.Rotation,
                       new Vector2(_tex.Width / 2f, _tex.Height / 3f), 1f, SpriteEffects.None, 0f);
        }

        public override void vertices(VertexPositionColor[] vertices, ref int count)
        {
            for (int i = 1; i < _vertices.Count; i++)
            {
                vertices[count++] = new VertexPositionColor(new Vector3(_vertices[i], 0f), ColorValue);
                vertices[count++] = new VertexPositionColor(new Vector3(_vertices[i + 1], 0f), ColorValue);
            }
            vertices[count++] = new VertexPositionColor(new Vector3(_vertices[_vertices.Count - 1], 0f), ColorValue);
            vertices[count++] = new VertexPositionColor(new Vector3(_vertices[0], 0f), ColorValue);
        }
    }
}