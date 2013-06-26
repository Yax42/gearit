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
using gearit.src.editor.map;

namespace gearit.src.map
{
    [Serializable()]
    class Map : ISerializable
    {
        private string _mapName;
        private List<MapChunk> _mapChunk;

        [NonSerialized]
        private World _world;

        public Map(World world)
        {
            _world = world;
            _mapName = "test";
            _mapChunk = new List<MapChunk>();
        }

        public Map(SerializationInfo info, StreamingContext ctxt)
        {
            _world = SerializerHelper.World;
            _mapName = (string)info.GetValue("Name", typeof(string));
            List<SeralizedBody> bodyList = (List<SeralizedBody>)
              info.GetValue("Bodies", typeof(List<SeralizedBody>));
            _mapChunk = new List<MapChunk>();
            foreach (SeralizedBody sb in bodyList)
                _mapChunk.Add(SeralizedBody.convertSBody(sb));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            List<SeralizedBody> bodyList = new List<SeralizedBody>();
            foreach (Body b in _mapChunk)
                bodyList.Add(SeralizedBody.convertBody(b));

            info.AddValue("Name", _mapName, typeof(string));
            info.AddValue("Bodies", bodyList, typeof(List<SeralizedBody>));
        }

        public MapChunk getChunk(Vector2 p)
        {
            Transform t;
            for (int i = 0; i < _mapChunk.Count(); i++)
            {
                _mapChunk[i].GetTransform(out t);
                if (_mapChunk[i].FixtureList[0].Shape.TestPoint(ref t, ref p))
                    return (_mapChunk[i]);
            }
            return null;
        }

        public void deleteChunk(MapChunk tmp)
        {
            _mapChunk.Remove(tmp);
        }

        public List<MapChunk> getChunks()
        {
            return (_mapChunk);
        }

        public string name
        {
            get { return _mapName; }
            set { _mapName = value; }
        }

        public void addChunk(MapChunk chunk)
        {
            _mapChunk.Add(chunk);
        }

        public void drawDebug(DrawGame game)
        {
            for (int i = 0; i < _mapChunk.Count; i++)
            {
                game.draw(_mapChunk[i], Color.Black);
            }
        }
    }
}