using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;

namespace gearit.src.editor.robot.action
{
    class ActionSelect2 : IAction
    {
        public void init() { }

        public bool shortcut(Input input)
        {
            return (input.ctrlAltShift(false, false, true) && input.justPressed(MouseKeys.LEFT));
        }

        public bool run(Input input, Robot robot, ref Piece selected1, ref Piece selected2)
        {
            selected2 = robot.getPiece(input.simUnitPosition());
            return (false);
        }
    }
}
