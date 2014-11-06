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
		static public bool JustSaved = false;

		public void init() { }

		public bool shortcut()
		{
			if (Input.CtrlShift(true, false) && (Input.justPressed(Keys.S)))
				return MenuMapEditor.Instance.saveMap();
			if (Input.CtrlShift(true, true) && (Input.justPressed(Keys.S)))
				MenuMapEditor.Instance.saveasMap();
			return false;
		}

		public bool run()
		{
			Debug.Assert(MapEditor.Instance.NamePath != "");
			if (!Serializer.SerializeItem(MapEditor.Instance.NamePath, MapEditor.Instance.Map))
				MapEditor.Instance.resetNamePath();
			else if (MustExit)
			{
				MustExit = false;
				//ScreenMainMenu.GoBack = true;
			}
			JustSaved = true;
			return false;
		}

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return false; } }

		public bool actOnSelect() { return false; }

		public ActionTypes Type() { return ActionTypes.SAVE; }
	}
}
