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
using gearit.src.Network;

namespace gearit.src.script
{
	class RobotLuaScript : LuaScript
	{
		public static NetworkClientGame NetworkGame { private set; get; }
		public static bool IsNetwork { get { return NetworkGame != null; } }

		public RobotLuaScript(List<RevoluteApi> api, RobotStateApi robotApi, string text, bool isFile, NetworkClientGame game)
			: base(text, isFile)
		{
			NetworkGame = game;
			for (int i = 0; i < api.Count; i++)
				this[api[i].Name()] = api[i];
			this["Input"] = new InputApi();
			this["Robot"] = robotApi;
			try
			{
				if (isFile)
					DoString("require 'header'\n");
			}
			catch (Exception ex)
			{
				OutputManager.LogError("Lua exception: " + ex.Message);
				throw (ex);
			}
			Load();
		}
	}
}
