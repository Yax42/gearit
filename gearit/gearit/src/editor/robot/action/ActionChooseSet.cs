using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.editor.robot;
using gearit.src.GUI;

namespace gearit.src.editor.robot.action
{
	class ActionChooseSet : IAction
	{
		static public bool IsWheel
		{
			set
			{
				MenuRobotEditor.Instance.IsWheel = value;
			}
			get
			{
				return MenuRobotEditor.Instance.IsWheel;
			}
		}

		public void init()
		{
			if (Input.justPressed(Keys.A))
				IsWheel = !IsWheel;
			else if (Input.justPressed(Keys.G))
				MenuRobotEditor.Instance.SwitchScriptVisibility();
		}

		public bool shortcut()
		{
			return Input.CtrlShift(false, false)
					&& (Input.justPressed(Keys.A)
					|| Input.justPressed(Keys.G));
		}

		public bool run()
		{
			return false;
		}

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return false; } }
		public ActionTypes Type() { return ActionTypes.CHOOSE_SET; }
	}
}
