using System;
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
            _fix = CreateFixture(_shape, null);
            //_tex = robot.getAsset().TextureFromShape(_shape, MaterialType.Blank, Color.White, 1f);
            Console.WriteLine("Heart created.");
        }

	public Heart(SerializationInfo info, StreamingContext ctxt) :
	  base()
	{
	  // Position = (Vector2) info.GetValue("Position", typeof(Vector2));
	  _vertices = (Vertices) info.GetValue("Vertices", typeof(Vertices));
	  _shape = (PolygonShape) info.GetValue("Shape", typeof(PolygonShape));
	  _fix = (Fixture) info.GetValue("Fixture", typeof(Fixture));
	}

	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
	  // info.AddValue("Position", Position);
	  info.AddValue("Vertices", _vertices);
	  info.AddValue("Shape", _shape);
	  info.AddValue("Fixture", _fix);
	}

        public override void draw(SpriteBatch batch)
        {
            batch.Draw(_tex, ConvertUnits.ToDisplayUnits(this.Position), null, Color.White, this.Rotation,
                       new Vector2(_tex.Width / 2f, _tex.Height / 3f), 1f, SpriteEffects.None, 0f);
        }

        public override void drawLines(DrawGame game)
        {
            for (int i = 0; i < _vertices.Count - 1; i++)
		game.addLine(Position + _vertices[i], Position + _vertices[i + 1], ColorValue);
	    game.addLine(Position + _vertices[_vertices.Count - 1], Position + _vertices[0], ColorValue);
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