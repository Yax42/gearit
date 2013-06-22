using gearit.src.utility;
using gearit.src.robot;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
    class ActionRevSpot : IAction
    {
        public void init() { }

        public bool shortcut(Input input)
        {
            return (input.ctrlAltShift(false, false, false) && input.justPressed(Keys.Q));
        }

        public bool run(Input input, Robot robot, ref Piece selected1, ref Piece selected2)
        {
            Piece p;
	    if (ActionChooseSet.value)
	      p = new Wheel(robot, 0.5f, input.simUnitPosition());
	    else
	      p = new Rod(robot, 2f, input.simUnitPosition());
            new RevoluteSpot(robot, selected1, p);
            return (false);
        }
    }
}
