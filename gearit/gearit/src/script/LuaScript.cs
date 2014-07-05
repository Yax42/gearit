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
		private string _filePath;
		private LuaFunction _loadedFile;
		private bool _ok;

		public LuaScript(string filePath)
		{
			_filePath = LuaManager.LuaFile(filePath);
			_ok = true;
			try
			{
				_loadedFile = LoadFile(_filePath);
				OutputManager.LogInfo("Lua - script correctly loaded: " +_filePath);
			}
			catch (Exception ex)
			{
				OutputManager.LogError("Lua exception: " + ex.Message);
				_ok = false;
			}
		}

		internal void run()
		{
			if (!_ok)
				return;
			try
			{
				_loadedFile.Call();
			}
			catch (Exception ex)
			{
					OutputManager.LogError("Lua exception: " + ex.Message);
				_ok = false;
			}
		}

		public void stop()
		{
			_ok = false;
			base.Close();
		}
	}
}