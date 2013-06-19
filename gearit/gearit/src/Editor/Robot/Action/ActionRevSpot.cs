using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;

namespace gearit.src.robot.Editor.Action
{
    class ActionRevSpot : IAction
    {
        public void init() { }

        public ActionTypes run(Input input, Robot robot, ref Piece selected)
        {
            if (input.justPressed(MouseKeys.RIGHT))
            {
                Piece p = new Wheel(robot, 0.5f, input.simUnitPosition());
                new RevoluteSpot(robot, selected, p);
                return (ActionTypes.NONE);
            }
            return (ActionTypes.REV_SPOT);
        }
    }
}
