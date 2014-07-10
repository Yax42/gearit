using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using gearit.src.output;
using gearit.src.robot;

namespace gearit.src.editor
{
	public static class Serializer
	{
		/// <summary>
		/// Serializer used for the serialization. (Duh!)
		/// </summary>
		private static IFormatter _formatter;
		private const string _path = "data/";

		public static void init()
		{
			if (!System.IO.Directory.Exists(_path))
				System.IO.Directory.CreateDirectory(_path);
			_formatter = new BinaryFormatter();
			SerializerHelper.Ptrmap = new Dictionary<int, Piece>();
		}

		/// <summary>
		/// Serializes an object.
		/// </summary>
		/// <param name="filename">The file to store the object.</param>
		/// <param name="obj">The object to serialize.</param>
		public static bool SerializeItem(string filename, ISerializable obj)
		{
			try
			{
				FileStream s = new FileStream(_path + filename, FileMode.Create);
				_formatter.Serialize(s, obj);
				s.Close();
				OutputManager.LogInfo("Saving - success", filename);
				return true;
			}
			catch (IOException e)
			{
				OutputManager.LogError("Saving - fail", filename);
				return false;
			}
		}

		/// <summary>
		/// Deserializes an object.
		/// </summary>
		/// <param name="filename">The file in which the object is stored.</param>
		/// <returns>The object as an ISerializable if the deserialization worked, null otherwise.</returns>
		public static ISerializable DeserializeItem(string filename)
		{
			try
			{
				FileStream s = new FileStream(_path + filename, FileMode.Open);
				ISerializable obj = (ISerializable)_formatter.Deserialize(s);
				s.Close();
				OutputManager.LogInfo("Loading - success", filename);
				return (obj);
			}
			catch (Exception e)
			{
				OutputManager.LogError("Loading - fail", filename);
				return null;
			}
		}
	}
}