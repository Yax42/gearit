using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace gearit.src.map
{
    [Serializable()]
    class Map : ISerializable
    {
        private string _mapName;

        //[NonSerialized]

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
        }
    }
}
