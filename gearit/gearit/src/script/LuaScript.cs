using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;
using gearit.xna;
using System.Threading;

using gearit.src.robot;
using gearit.src.output;
using gearit.src.editor.robot;
using System.Diagnostics;

namespace gearit.src.script
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
		private bool _done;
		private RobotStateApi _robotApi;

		public void saySomething(string something)
		{
			Console.WriteLine(something);
		}

		public LuaScript(List<SpotApi> api, RobotStateApi robotApi, string name)
		{
			_done = true;
			_name = name;
			_api = api;
			_robotApi = robotApi;
			_inputApi = new InputApi();
			_thread = new Thread(new ThreadStart(exec));
			for (int i = 0; i < api.Count; i++)
				this[api[i].Name()] = api[i];
			this["Input"] = _inputApi;
			this["Robot"] = _robotApi;
			//RegisterFunction("getKeysAction", _input, _input.GetType().GetMethod("getKeysAction"));
			run();
		}

		private void run()
		{
			Debug.Assert(_done, "Thread already running and trying to run it again.");
			_done = false;
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
				if (!_done)
					OutputManager.LogError("Lua exception: " + ex.Message);
			}
		}

		public void stop()
		{
			LuaThreads.Remove(_thread);
			_done = true;
			_thread.Abort();
			base.Close();
		}
	}
}
