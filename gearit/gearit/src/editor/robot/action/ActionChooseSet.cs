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
		static public bool IsPrismatic
		{
			set
			{
				MenuRobotEditor.Instance.IsPrismatic = false;
			}
			get
			{
				return MenuRobotEditor.Instance.IsPrismatic;
			}
		}

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
			if (Input.ctrlAltShift(false, false, true))
				IsPrismatic = !IsPrismatic;
			else
				IsWheel = !IsWheel;
		}

		public bool shortcut()
		{
			return ((Input.ctrlAltShift(false, false, false)
					|| Input.ctrlAltShift(false, false, true))
					&& (Input.justPressed(Keys.A)));
		}

		public bool run()
		{
			return false;
		}

		public void revert() { }

		public bool canBeReverted() { return false; }

		public ActionTypes type() { return ActionTypes.CHOOSE_SET; }
	}
}
