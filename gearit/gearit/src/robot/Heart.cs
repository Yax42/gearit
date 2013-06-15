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
            for (int i = 0; i < _vertices.Count - 1; i++)
            {
                vertices[count++] = new VertexPositionColor(new Vector3(_vertices[i], 0f), ColorValue);
                vertices[count++] = new VertexPositionColor(new Vector3(_vertices[i + 1], 0f), ColorValue);
            }
            vertices[count++] = new VertexPositionColor(new Vector3(_vertices[_vertices.Count - 1], 0f), ColorValue);
            vertices[count++] = new VertexPositionColor(new Vector3(_vertices[0], 0f), ColorValue);
        }

        private bool checkShape()
        {
            return (_vertices.IsConvex() && areSpotsOk() && _vertices.GetArea() > 0.5f);
        }

        public int getCorner(Vector2 p)
        {
            float dist = 99999;
            int res = 0;

            for (int i = 0; i < _vertices.Count; i++)
                if ((_vertices[i] - p).Length() < dist)
                {
                    dist = (_vertices[i] - p).Length();
                    res = i;
                }
	    return (res);
        }

        public Vector2 getCorner(int id)
        {
            if (id >= _vertices.Count)
                return (Vector2.Zero);
            return (_vertices[id]);
        }

        public void moveCorner(int id, Vector2 pos)
        {
            if (id >= _vertices.Count)
                return;
            Vector2 backupPos = _vertices[id];
            _vertices[id] = pos;
            if (checkShape())
                return;
            _vertices[id] = backupPos;
            //update shape
        }

        public void addCorner(Vector2 p)
        {
            if (_vertices.Contains(p))
                return ;
            _vertices.Add(p);
            if (checkShape() == false)
                _vertices.Remove(p);
	      
            //else
                //FixtureList[0].Shape = _shape;
	    //update shape
        }

        public void removeCorner(int id)
        {
            if (id >= _vertices.Count || _vertices.Count < 3)
                return;
            Vector2 backupPos = _vertices[id];
            _vertices.RemoveAt(id);
            if (checkShape())
                return;
            _vertices.Insert(id, backupPos);
        }

        public override float Weight
        {
            set
            {
                if (value < 50)
                    value = 50;
                FixtureList[0].Shape.Density = value;
                ResetMassData();
            }
            get { return FixtureList[0].Shape.Density; }
        }
    }
}