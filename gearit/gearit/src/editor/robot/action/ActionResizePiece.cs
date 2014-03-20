using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.robot;
using Microsoft.Xna.Framework;

namespace gearit.src.editor.robot.action
{
	class ActionResizePiece : IAction
	{
		private Piece P1;
		private float From;
		private float To;
		bool HasBeenRevert;

		private int _corner;
		private bool _moving;
		private bool _side;
		private bool _isInit;

		public void init()
		{
			HasBeenRevert = false;
			P1 = RobotEditor.Instance.Select1;
			From = P1.getSize();

			_corner = 0;
			_moving = false;
			_isInit = false;
		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(false, false, false) && Input.justPressed(Keys.S));
		}

		public bool run()
		{
			bool res = privateRun(RobotEditor.Instance.Robot, RobotEditor.Instance.Select1, RobotEditor.Instance.Select2);
			_isInit = res;
			return res;
		}

		private bool privateRun(Robot robot, Piece selected1, Piece selected2)
		{
			if (selected1.GetType() == typeof(Heart))
			{
				if (Input.justPressed(MouseKeys.LEFT))
				{
					_moving = true;
					_corner = robot.getHeart().getCorner(Input.SimMousePos);
				}
				if (Input.pressed(MouseKeys.LEFT))
				{
					if (Input.justPressed(MouseKeys.RIGHT))
					{
						robot.getHeart().removeCorner(_corner);
						_corner = 0;
						_moving = false;
					}
					else if (_moving)
						robot.getHeart().moveCorner(_corner, Input.SimMousePos);
				}
				else if (Input.justPressed(MouseKeys.RIGHT))
					robot.getHeart().addCorner(Input.SimMousePos);
				return (Input.justPressed(Keys.S) == false);
			}
			else if (selected1.GetType() == typeof(Wheel))
			{
				((Wheel)selected1).Size = (Input.SimMousePos - selected1.Position).Length();
				return (Input.justPressed(MouseKeys.LEFT) == false && Input.justReleased(Keys.S) == false);
			}
			else
			{
				if (!_isInit)
					((Rod)selected1).GenerateEnds();
				((Rod)selected1).setEnd(Input.SimMousePos, Input.pressed(Keys.LeftShift));
				return (!Input.justPressed(MouseKeys.LEFT));
			}
		}

		public void revert()
		{
			HasBeenRevert = true;
			//P1.move(From);
		}

		public bool canBeReverted() { return true; }

		public ActionTypes type() { return ActionTypes.RESIZE_PIECE; }
	}
}
