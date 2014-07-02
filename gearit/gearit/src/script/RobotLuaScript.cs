﻿using System;
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
	class RobotLuaScript : LuaScript
	{

		public RobotLuaScript(List<SpotApi> api, RobotStateApi robotApi, string name)
			: base(name)
		{
			for (int i = 0; i < api.Count; i++)
				this[api[i].Name()] = api[i];
			this["Input"] = new InputApi();
			this["Robot"] = robotApi;
			run();
		}
	}
}