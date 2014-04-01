﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;
using gearit.src.map;

namespace gearit.src.editor.map.action
{
	class ActionCreate : IAction
	{
		public void init()
		{
			MapChunk chunk = new PolygonChunk(MapEditor.Instance.World, false, Input.SimMousePos);
			MapEditor.Instance.Map.addChunk(chunk);
			MapEditor.Instance.Select = chunk;
		}

		public bool shortcut()
		{
			return Input.justPressed(Keys.W)
				&& Input.ctrlAltShift(false, false, false);
		}

		public bool run() { return false; }

		public ActionTypes type() { return ActionTypes.CREATE; }
	}
}
