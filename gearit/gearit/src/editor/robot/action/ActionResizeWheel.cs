using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace gearit.src.editor.robot.action
{
	class ActionResizeWheel : IAction
	{
		private Wheel Wheel;
		private float From;
		private float To;
		private bool HasBeenRevert;

		public void init()
		{
			HasBeenRevert = false;
			Debug.Assert(RobotEditor.Instance.Select1.GetType() == typeof(Wheel));
			Wheel = (Wheel) RobotEditor.Instance.Select1;
			From = Wheel.Size;
		}

		public bool shortcut()
		{
			return Input.ctrlAltShift(false, false, false)
					&& Input.justPressed(Keys.S)
					&& RobotEditor.Instance.Select1.GetType() == typeof(Wheel);
		}

		public bool run()
		{
			if (!HasBeenRevert)
			{
				To = (Input.SimMousePos - Wheel.Position).Length();
			}
			Wheel.Size = To;
			return (Input.justPressed(MouseKeys.LEFT) == false && Input.justReleased(Keys.S) == false);
		}

		public void revert()
		{
			HasBeenRevert = true;
			Wheel.Size = From;
		}

		public bool canBeReverted() { return true; }

		public ActionTypes type() { return ActionTypes.RESIZE_WHEEL; }
	}
}
