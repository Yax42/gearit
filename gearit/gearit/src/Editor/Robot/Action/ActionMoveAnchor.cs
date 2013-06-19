using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;

namespace gearit.src.editor.robot.action
{
    class ActionMoveAnchor : IAction
    {
        private bool _firstStep;

        public void init()
        {
            _firstStep = true;
        }

        public bool run(Input input, Robot robot, ref Piece selected)
        {
            if (_firstStep == false)
            {

            }
            selected.move(input.simUnitPosition());
            return (true);
        }
    }
}
