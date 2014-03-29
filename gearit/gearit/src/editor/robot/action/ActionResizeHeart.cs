using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.robot;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using FarseerPhysics.Common;

namespace gearit.src.editor.robot.action
{
	class ActionResizeHeart : IAction
	{
		private Heart Heart;
		private Vertices From;
		private Vertices To;
		private bool HasBeenRevert;

		private int _corner;
		private bool _moving;
		private bool _isFirstFrame;
		private bool _didSomething;

		public void init()
		{
			HasBeenRevert = false;
			Debug.Assert(RobotEditor.Instance.Select1.GetType() == typeof(Heart));
			Heart = (Heart) RobotEditor.Instance.Select1;
			From = Heart.getShapeClone();
			_corner = Heart.getCorner(Input.SimMousePos);
			_moving = true;
			_isFirstFrame = true;
			_didSomething = false;
		}

		public bool shortcut()
		{
			return Input.ctrlAltShift(false, false, false)
					&& Input.justPressed(Keys.S)
					&& RobotEditor.Instance.Select1.GetType() == typeof(Heart);
		}

		public bool run()
		{
			if (HasBeenRevert)
			{
				Heart.setShape(To);
				return false;
			}
			if (Input.justPressed(MouseKeys.LEFT))
			{
				_moving = true;
				_corner = Heart.getCorner(Input.SimMousePos);
			}
			if (Input.pressed(MouseKeys.LEFT))
			{
				if (Input.justPressed(MouseKeys.RIGHT))
				{
					Heart.removeCorner(_corner);
					_didSomething = true;
					_corner = 0;
					_moving = false;
				}
				else if (_moving)
				{
					Heart.moveCorner(_corner, Input.SimMousePos);
					_didSomething = true;
				}
			}
			else if (Input.justPressed(MouseKeys.RIGHT))
			{
				_didSomething = true;
				Heart.addCorner(Input.SimMousePos);
			}
			if (Input.justPressed(Keys.S) == true && !_isFirstFrame)
			{
				To = Heart.getShapeClone();
				return false;
			}
			if (Input.released(Keys.S))
				_isFirstFrame = false;
			return true;
		}

		public void revert()
		{
			HasBeenRevert = true;
			Heart.setShape(From);
		}

		public bool canBeReverted() { return _didSomething; }

		public ActionTypes type() { return ActionTypes.RESIZE_HEART; }
	}
}
