using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.GUI;

namespace gearit.src.output
{
    /// <summary>
    /// That controls the console
    /// </summary>
	class OutputManager
	{
		static public void LogError(String msg)
		{
			Console.WriteLine("ERROR: #{0}", msg);
			ChatBox.addEntry(msg, ChatBox.Entry.Error);
		}
		static public void LogWarning(String msg)
		{
			Console.WriteLine("WARNING: #{0}", msg);
			ChatBox.addEntry(msg, ChatBox.Entry.Warning);
		}
		static public void LogNetwork(String msg)
		{
			Console.WriteLine("Network: #{0}", msg);
			ChatBox.addEntry(msg, ChatBox.Entry.Network);
		}
		static public void LogInfo(String msg)
		{
			Console.WriteLine("Info: #{0}", msg);
			ChatBox.addEntry(msg, ChatBox.Entry.Info);
		}
		static public void LogMerge(String msg)
		{
			Console.Write(msg);
			ChatBox.mergeEntry(msg);
		}

		static public void LogError(String msg, String path)
		{
			LogError(msg + ": " + path);
		}
		static public void LogWarning(String msg, String path)
		{
			LogWarning(msg + ": " + path);
		}
		static public void LogInfo(String msg, String path)
		{
			LogInfo(msg + ": " + path );
		}
		static public void LogMessage(String msg)
		{
			Console.WriteLine("Message: #{0}", msg);
			ChatBox.addEntry(msg, ChatBox.Entry.Message);
		}

		//// STATIC method
		//private static OutputManager _instance;
		//private IOutputMessageManager _output_message_manager;

		//public static OutputManager GetInstance()
		//{
		//    if (_instance == null)
		//        _instance = new OutputManager();
		//    return _instance;
		//}
		//public static void SetInstance(OutputManager om)
		//{
		//    _instance = om;
		//}
		//// END

		//public IOutputMessageManager GetOutputMessageManager()
		//{
		//    if (_instance == null)
		//        throw new System.MethodAccessException("_output_message_manager was not set");
		//    return _output_message_manager;
		//}
		//public void SetOutputMessageManager(IOutputMessageManager value)
		//{
		//    _output_message_manager = value;
		//}

	}
}
