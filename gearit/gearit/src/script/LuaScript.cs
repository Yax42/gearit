using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;
using System.Threading;
using System.Diagnostics;
using gearit.src.editor.robot;
using gearit.src.output;

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

		private Thread _thread;
		private bool _done;
		private string _filePath;

		public LuaScript(string filePath)
		{
			_filePath = filePath;
			_done = true;
			_thread = new Thread(new ThreadStart(exec));
			_thread.Priority = ThreadPriority.Lowest;
		}

		internal void run()
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
				DoFile(LuaManager.LuaFile(_filePath));
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