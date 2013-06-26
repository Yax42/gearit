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
        private string _name;
        private List<MapChunk> _chunks;

        [NonSerialized]
        private World _world;

        public Map(World world)
        {
            _world = world;
            _name = "test";
            _chunks = new List<MapChunk>();
        }

        public Map(SerializationInfo info, StreamingContext ctxt)
        {
            _world = SerializerHelper.World;
            _name = (string)info.GetValue("Name", typeof(string));
	    _chunks = (List<MapChunk>)info.GetValue("Chunks", typeof(List<MapChunk>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Chunks", _chunks, typeof(List<MapChunk>));
            info.AddValue("Name", _name, typeof(string));
        }

        public MapChunk getChunk(Vector2 p)
        {
            Transform t;
            for (int i = 0; i < _chunks.Count(); i++)
            {
                _chunks[i].GetTransform(out t);
                if (_chunks[i].FixtureList[0].Shape.TestPoint(ref t, ref p))
                    return (_chunks[i]);
            }
            return null;
        }

        public void deleteChunk(MapChunk tmp)
        {
            _chunks.Remove(tmp);
        }

        public List<MapChunk> getChunks()
        {
            return (_chunks);
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public void addChunk(MapChunk chunk)
        {
            _chunks.Add(chunk);
        }

        public void drawDebug(DrawGame game)
        {
            Color col;

            for (int i = 0; i < _chunks.Count; i++)
            {
                col = (_chunks[i].BodyType == BodyType.Static) ? Color.Black : Color.Red;
                game.draw(_chunks[i], col);
            }
        }
    }
}