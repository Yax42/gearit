﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace gearit.src.map
{
    [Serializable()]
    class Map : ISerializable
    {
        private string      _mapName;
        private World       _world;
        private List<Body>  _mapBody;

        //[NonSerialized]
        


        public Map(World world)
        {
            _world = world;
            _mapName = "test";
            _mapBody = new List<Body>();
        }

        public Map(SerializationInfo info, StreamingContext ctxt)
        {
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
        }

        public string name
        {
            get { return _mapName; }
            set { _mapName = value; }
        }

        public void addBody(Body body)
        {
            body.BodyType = BodyType.Static;
            body.FixtureList[0].CollisionGroup = 1337;
            _mapBody.Add(body);
        }

        private void drawPoly(DrawGame game, Vertices vertices, Vector2 pos)
        {
            for (int i = 0; i < vertices.Count - 1; i++)
                game.addLine(pos + vertices[i], pos + vertices[i + 1], Color.Black);
            game.addLine(pos + vertices[vertices.Count - 1], pos + vertices[0], Color.Black);
        }

        public void drawDebug(DrawGame game)
        {
            for (int i = 0; i < _mapBody.Count; i++)
            {
                drawPoly(game, ((PolygonShape)_mapBody[i].FixtureList[0].Shape).Vertices, _mapBody[i].Position);
            }
        }
    }
}
