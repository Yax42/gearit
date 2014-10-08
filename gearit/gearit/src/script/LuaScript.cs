using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;
using System.Threading;
using System.Diagnostics;
using gearit.src.editor.robot;
using gearit.src.output;
using System.IO;

namespace gearit.src.script
{
	public class LuaScript : Lua
	{
		private LuaFunction _loadedScript;
		private LuaFunction _frameCountScript;
		private bool _ok;
		private string FileName;

        /// <summary>
        /// Lua implementation of lua library to manage script
        /// </summary>
		public LuaScript(string text, bool isFile = true)
		{
			FileName = text;
			_ok = true;
			try
			{
				if (isFile)
				{
					_loadedScript = LoadFile(text);
					//OutputManager.LogInfo("Lua - script correctly loaded: " + text);
				}
				else
				{
					_loadedScript = LoadString(text, String.Empty);
				}
				DoString("FrameCount = 0\n");

				_frameCountScript = LoadString("FrameCount = FrameCount + 1", String.Empty);
			}
			catch (Exception ex)
			{
				OutputManager.LogError("Lua script (" + FileName + ") - " + ex.Message);
				_ok = false;
				File.WriteAllText("test.lua", text);
			}
		}

		public void run()
		{
			if (!_ok)
				return;
			try
			{
				_loadedScript.Call();
				_frameCountScript.Call();
			}
			catch (Exception ex)
			{
				OutputManager.LogError("Lua script (" + FileName + ") - " + ex.Message);
				_ok = false;
			}
		}

		public void stop()
		{
			if (_ok)
			{
				_ok = false;
				base.Close();
			}
		}
	}
}