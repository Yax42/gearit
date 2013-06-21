using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FarseerPhysics.Dynamics;

namespace gearit.src.map
{
    [Serializable()]
    class Map : ISerializable
    {
        private string      _mapName;
        private World       _world;
        private List<Body>  _mapBody;

        //[NonSerialized]

        public Map()
        {
            _mapName = "test";
            _mapBody = new List<Body>();
        }

        public Map(SerializationInfo info, StreamingContext ctxt)
        {
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
        }

        public World world
        {
            get { return _world; }
            set { _world = value; }
        }

        public string name
        {
            get { return _mapName; }
            set { _mapName = value; }
        }

        public void addBody(Body body)
        {
            _mapBody.Add(body);
        }
    }
}
