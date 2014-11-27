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
			To = From;
			_corner = Heart.getCorner(Input.VirtualSimMousePos);
			_moving = true;
			_isFirstFrame = true;
			_didSomething = false;
		}

		public bool shortcut()
		{
			return Input.CtrlShift(false, false)
					&& Input.justPressed(Keys.S)
					&& RobotEditor.Instance.Select1.GetType() == typeof(Heart);
		}

		public bool run()
		{
			if (HasBeenRevert)
			{
				Heart.ResetShape(To);
				return false;
			}
			if (Input.justPressed(MouseKeys.LEFT))
			{
				_moving = true;
				_corner = Heart.getCorner(Input.VirtualSimMousePos);
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
					Heart.moveCorner(_corner, Input.VirtualSimMousePos);
					_didSomething = true;
				}
			}
			else if (Input.justPressed(MouseKeys.RIGHT))
			{
				if (Heart.addCorner(Input.VirtualSimMousePos))
					_didSomething = true;
			}
			return Input.pressed(Keys.S);
		}

		public void revert()
		{
			HasBeenRevert = true;
			Heart.ResetShape(From);
		}

		public bool canBeReverted { get { return _didSomething; } }
		public bool canBeMirrored { get { return true; } }
		public ActionTypes Type() { return ActionTypes.RESIZE_HEART; }
	}
}
