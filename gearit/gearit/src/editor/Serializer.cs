using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace gearit.src.Editor
{
    class Serializer
    {
        /// <summary>
        /// Serializer used for the serialization. (Duh!)
        /// </summary>
        private IFormatter _formatter;

        public Serializer()
        {
            _formatter = new BinaryFormatter();
        }

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="filename">The file to store the object.</param>
        /// <param name="obj">The object to serialize.</param>
        public void SerializeItem(string filename, ISerializable obj)
        {
            FileStream s = new FileStream(filename, FileMode.Create);
            _formatter.Serialize(s, obj);
            s.Close();
        }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="filename">The file where in which the object is stored.</param>
        /// <returns>The object as an ISerializable if the deserialization worked, null otherwise.</returns>
        public ISerializable DeserializeItem(string filename)
        {
            FileStream s = new FileStream(filename, FileMode.Open);
            ISerializable obj = (ISerializable)_formatter.Deserialize(s);
            return (obj);
        }
    }

    /* TEST */
    [Serializable()]
    class SerializerTest : ISerializable
    {
        private List<int> _val;

        public List<int> Val
        {
            get { return _val; }
            set { _val = value; }
        }

        public SerializerTest(int v)
        {
            _val = new List<int>();
            _val.Add(v);
        }

        public SerializerTest()
        {
            _val = new List<int>();
            _val.Add(0);
        }

        public SerializerTest(SerializationInfo info, StreamingContext context)
        {
            _val = (List<int>)info.GetValue("Value", typeof(List<int>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Value", _val, typeof(List<int>));
        }
    }

    public static class SerializerUnit
    {
        static void unit() // Rename to Main
        // static void Main()
        {
            SerializerTest t = new SerializerTest(15);
            Serializer s = new Serializer();
            string filename = "my_test";

            s.SerializeItem(filename, t);
            SerializerTest t2 = (SerializerTest)s.DeserializeItem(filename);

            Console.WriteLine("Value of test: " + t2.Val[0] + ".");
        }
    }
}
