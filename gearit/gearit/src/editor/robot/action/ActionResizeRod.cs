﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.robot;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace gearit.src.editor.robot.action
{
	class ActionResizeRod : IAction
	{
		private Rod Rod;
		private Vector2 From;
		private Vector2 To;
		private bool HasBeenRevert;
		private bool EndId;
		private Piece Select;


		public void init()
		{
			HasBeenRevert = false;
			Debug.Assert(RobotEditor.Instance.Select1.GetType() == typeof(Rod));
			Rod = (Rod) RobotEditor.Instance.Select1;
			Rod.GenerateEnds();
			EndId = Rod.CloseEnd(Input.SimMousePos);
			From = Rod.getEnd(EndId);
			if (Input.pressed(Keys.LeftShift))
				Select = RobotEditor.Instance.Select2;
			else
				Select = null;

		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(false, false, false)
					|| Input.ctrlAltShift(false, false, true))
					&& Input.justPressed(Keys.S)
					&& RobotEditor.Instance.Select1.GetType() == typeof(Rod);
		}

		public bool run()
		{
			if (!HasBeenRevert)
			{
				To = Input.SimMousePos;
			}
			Rod.setEnd(To, EndId, Select);
			return (Input.justPressed(MouseKeys.LEFT) == false && Input.justReleased(Keys.S) == false);
		}

		public void revert()
		{
			HasBeenRevert = true;
			Rod.setEnd(From, EndId, Select);
		}

		public bool canBeReverted() { return true; }

		public ActionTypes type() { return ActionTypes.RESIZE_ROD; }
	}
}
