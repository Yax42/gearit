using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;
using gearit.xna;
using System.Threading;

using gearit.src.robot;
using gearit.src.editor.api;
using gearit.src.output;
using gearit.src.editor.robot;
namespace gearit.src.utility
{

	class LuaScript : Lua
	{
		private static List<Thread> LuaThreads = new List<Thread>();
		public static void Clear()
		{
			foreach (Thread t in LuaThreads)
				t.Abort();
			LuaThreads.Clear();
		}

		private String _name;
		private List<SpotApi> _api;
		private InputApi _inputApi;
		private Thread _thread;

		public void saySomething(string something)
		{
			Console.WriteLine(something);
		}

		public LuaScript(List<SpotApi> api, string name)
		{
			_name = name;
			_api = api;
			_inputApi = new InputApi();
			_thread = new Thread(new ThreadStart(exec));
			for (int i = 0; i < api.Count; i++)
				this[api[i].name()] = api[i];
			this["Input"] = _inputApi;
			//RegisterFunction("getKeysAction", _input, _input.GetType().GetMethod("getKeysAction"));
			run();
		}

		private void run()
		{
			_thread.Start();
			LuaThreads.Add(_thread);
		}

		private void exec()
		{
			try
			{
				DoFile(LuaManager.LuaFile(_name));
			}
			catch (Exception ex)
			{
				OutputManager.LogError("Lua exception: " + ex.Message);
			}
		}

		public void stop()
		{
			LuaThreads.Remove(_thread);
			_thread.Abort();
			base.Close();
		}
	}
}
