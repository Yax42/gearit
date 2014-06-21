using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;
using System.Diagnostics;
using gearit.src.editor.map;
using GUI;

namespace gearit.src.editor.map.action
{
	class ActionSaveMap : IAction
	{
		static public bool MustExit = false;

		public void init() { }

		public bool shortcut()
		{
			if (Input.ctrlAltShift(true, false, false) && (Input.justPressed(Keys.S)))
			{
				MenuMapEditor.Instance.saveMap();
				return true;
			}
			if (Input.ctrlAltShift(true, false, true) && (Input.justPressed(Keys.S)))
			{
				MenuMapEditor.Instance.saveasMap();
				return false;
			}
			return false;
		}

		public bool run()
		{
			Debug.Assert(MapEditor.Instance.NamePath != "");
			MapEditor.Instance.Map.Name = MapEditor.Instance.NamePath;
			if (!Serializer.SerializeItem("map/" + MapEditor.Instance.NamePath + ".gim", MapEditor.Instance.Map))
				MapEditor.Instance.resetNamePath();
			else if (MustExit)
			{
				MustExit = false;
				ScreenMainMenu.GoBack = true;
			}
			return false;
		}

		public void revert() { }

		public bool canBeReverted() { return false; }

		public bool actOnSelect() { return false; }

		public ActionTypes type() { return ActionTypes.SAVE; }
	}
}
