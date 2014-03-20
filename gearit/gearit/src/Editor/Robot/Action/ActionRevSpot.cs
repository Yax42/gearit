using gearit.src.utility;
using gearit.src.robot;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
	class ActionRevSpot : IAction
	{
		Piece P1;
		bool HasBeenRevert;

		public void init()
		{
			HasBeenRevert = false;
			if (ActionChooseSet.value)
				P1 = new Wheel(RobotEditor.Instance.Robot, 0.5f, Input.SimMousePos);
			else
				P1 = new Rod(RobotEditor.Instance.Robot, 2, Input.SimMousePos);
			new RevoluteSpot(RobotEditor.Instance.Robot, RobotEditor.Instance.Select1, P1);
			RobotEditor.Instance.Select1 = P1;
		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(false, false, false) && Input.justPressed(Keys.Q));
		}

		public bool run()
		{
			if (HasBeenRevert)
				P1.BackIntoWorld(RobotEditor.Instance.Robot);
			else if (Input.justPressed(MouseKeys.LEFT) || Input.justReleased(Keys.W))
				return (false);
			P1.move(Input.SimMousePos);
			return false;
		}

		public void revert()
		{
			HasBeenRevert = true;
			RobotEditor.Instance.remove(P1);
		}

		public bool canBeReverted() { return true; }

		public ActionTypes type() { return ActionTypes.REV_SPOT; }
	}
}
