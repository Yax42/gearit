using gearit.src.utility;
using gearit.src.robot;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
    class ActionRevSpot : IAction
    {
        public void init() { }

        public bool shortcut()
        {
            return (Input.ctrlAltShift(false, false, false) && Input.justPressed(Keys.Q));
        }

        public bool run(Robot robot, ref Piece selected1, ref Piece selected2)
        {
            Piece p;
	    if (ActionChooseSet.value)
	      p = new Wheel(robot, 0.5f, Input.SimMousePos);
	    else
	      p = new Rod(robot, 2f, Input.SimMousePos);
            new RevoluteSpot(robot, selected1, p);
            return (false);
        }
    }
}
