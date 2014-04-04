﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using gearit.src.output;
using gearit.src.editor.map;
using gearit.src.map;

namespace gearit.src.editor.map.action
{
	class ActionLoadMap : IAction
	{
		public void init() { }

		public bool shortcut()
		{
			if (Input.ctrlAltShift(true, false, false) && (Input.justPressed(Keys.D)))
				MenuMapEditor.Instance.loadMap();
			return false;
		}

		public bool run()
		{
			Debug.Assert(MapEditor.Instance.NamePath != "");
			Map robot = (Map)Serializer.DeserializeItem("map/" + MapEditor.Instance.NamePath + ".gim");
			if (robot == null)
				MapEditor.Instance.resetNamePath();
			else
				MapEditor.Instance.resetMap(robot);
			return false;
		}

		public void revert() { }

		public bool canBeReverted() { return false; }
		
		public bool actOnSelect() { return false; }

		public ActionTypes type() { return ActionTypes.LOAD; }
	}
}