﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace gearit.src.editor
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
            try
            {
                FileStream s = new FileStream(filename, FileMode.Create);
                _formatter.Serialize(s, obj);
                s.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("Error while opening/creating the file {0}.", filename);
            }
        }


        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <param name="filename">The file in which the object is stored.</param>
        /// <returns>The object as an ISerializable if the deserialization worked, null otherwise.</returns>
        public ISerializable DeserializeItem(string filename)
        {
            try
            {
                FileStream s = new FileStream(filename, FileMode.Open);
                ISerializable obj = (ISerializable)_formatter.Deserialize(s);
                s.Close();
                return (obj);
            }
            catch (IOException e)
            {
                Console.WriteLine("Error while opening the file {0}.", filename);
                return (null);
            }
        }
    }
}