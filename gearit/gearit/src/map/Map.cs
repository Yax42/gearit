using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using gearit.src.editor;

namespace gearit.src.map
{
    [Serializable()]
    class Map : ISerializable
    {
        private string _mapName;
        private List<Body> _mapBody;

        [NonSerialized]
        private World _world;

        public Map(World world)
        {
            _world = world;
            _mapName = "test";
            _mapBody = new List<Body>();
        }

        public Map(SerializationInfo info, StreamingContext ctxt)
        {
            _world = SerializerHelper._world;
            _mapName = (string)info.GetValue("Name", typeof(string));
            List<SeralizedBody> bodyList = (List<SeralizedBody>)
              info.GetValue("Bodies", typeof(List<SeralizedBody>));

            _mapBody = new List<Body>();
            foreach (SeralizedBody sb in bodyList)
                _mapBody.Add(SeralizedBody.convertSBody(sb));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            List<SeralizedBody> bodyList = new List<SeralizedBody>();
            foreach (Body b in _mapBody)
                bodyList.Add(SeralizedBody.convertBody(b));

            info.AddValue("Name", _mapName, typeof(string));
            info.AddValue("Bodies", bodyList, typeof(List<SeralizedBody>));
        }

        public Body getBody(Vector2 p)
        {
            Transform t;
            for (int i = 0; i < _mapBody.Count(); i++)
            {
                _mapBody[i].GetTransform(out t);
                if (_mapBody[i].FixtureList[0].Shape.TestPoint(ref t, ref p))
                    return (_mapBody[i]);
            }
            return null;
        }

        public void deleteBody(Body tmp)
        {
            _mapBody.Remove(tmp);
        }

        public List<Body> getBodies()
        {
            return (_mapBody);
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

        public void drawDebug(DrawGame game)
        {
            for (int i = 0; i < _mapBody.Count; i++)
            {
                game.draw(_mapBody[i], Color.Black);

                //drawPoly(game, ((PolygonShape)_mapBody[i].FixtureList[0].Shape).Vertices, _mapBody[i].Position);
            }
        }
    }
}