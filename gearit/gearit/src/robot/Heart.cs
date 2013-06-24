using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using gearit.src;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace gearit
{
    [Serializable()]
    class Heart : Piece, ISerializable
    {
        private Vertices _vertices; //Le PolygonShape sera composé de ces vertices (elles sont les cotés du polygone).

        public Heart(Robot robot) :
            base(robot)
        {
            Position = new Vector2(3, 3);
            _vertices = PolygonTools.CreateRectangle(1, 1);
            _shape = new PolygonShape(_vertices, 50f);
            _fix = CreateFixture(_shape);
            //_vertices = ((PolygonShape)_fix.Shape).Vertices;
            //_tex = robot.getAsset().TextureFromShape(_shape, MaterialType.Blank, Color.White, 1f);
        }

        public Heart(SerializationInfo info, StreamingContext ctxt) :
            base(info, ctxt)
        {
            List<Vector2> v = (List<Vector2>)info.GetValue("Vertices", typeof(List<Vector2>));
            _vertices = new Vertices(v);
            setShape(new PolygonShape(_vertices, (float)info.GetValue("Density", typeof(float))), Robot._robotIdCounter);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            List<Vector2> v = new List<Vector2>();
            foreach (Vector2 vert in _vertices)
                v.Add(vert);
            info.AddValue("Vertices", v, typeof(List<Vector2>));
            base.GetObjectData(info, ctxt);
        }

        public void draw(SpriteBatch batch)
        {
            batch.Draw(_tex, ConvertUnits.ToDisplayUnits(this.Position), null, Color.White, this.Rotation,
                       new Vector2(_tex.Width / 2f, _tex.Height / 3f), 1f, SpriteEffects.None, 0f);
        }

        private bool checkShape()
        {
            return (areSpotsOk() && _vertices.GetArea() > 3f);
        }

        public int getCorner(Vector2 p)
        {
            float dist = 99999;
            int res = 0;
            p -= Position;

            for (int i = 0; i < _vertices.Count; i++)
                if ((_vertices[i] - p).Length() < dist)
                {
                    dist = (_vertices[i] - p).Length();
                    res = i;
                }
            return (res);
        }

        private void updateShape()
        {
            _shape = new PolygonShape(_vertices, _shape.Density);
            DestroyFixture(_fix);
            _fix = CreateFixture(_shape);
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
            _vertices[id] = pos - Position;
            if (_vertices.IsConvex() == false)
                _vertices[id] = backupPos;
            else
            {
                updateShape();
                if (checkShape() == false)
                {
                    _vertices[id] = backupPos;
                    updateShape();
                }
            }
        }

        public void addCorner(Vector2 p)
        {
            p -= Position;
            if (_vertices.Contains(p))
                return;
            _vertices.Add(p);
            if (_vertices.IsConvex() == false)
                    _vertices.Remove(p);
            else
            {
                updateShape();
                if (checkShape() == false)
                {
                    _vertices.Remove(p);
                    updateShape();
                }
            }
        }

        public void removeCorner(int id)
        {
            if (id >= _vertices.Count || _vertices.Count < 3)
                return;
            Vector2 backupPos = _vertices[id];
            _vertices.RemoveAt(id);
            if (_vertices.IsConvex() == false)
                _vertices[id] = backupPos;
            else
            {
                updateShape();
                if (checkShape() == false)
                {
                    _vertices.Insert(id, backupPos);
                    updateShape();
                }
            }
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

        override public float getSize()
        {
            return (_shape.MassData.Area);
        }
    }
}