using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using gearit.src.robot;

namespace gearit.src.editor.robot.action
{
    class ActionPrisSpot : IAction
    {
        private bool _isCreated;

        public void init()
        {
            _isCreated = false;
        }

        public ActionTypes run(Input input, Robot robot, ref Piece selected)
        {
            if (_isCreated == false)
            {
                _isCreated = true;
                Piece p = new Wheel(robot, 0.5f, input.simUnitPosition());
                new PrismaticSpot(robot, selected, p);
                selected = p;
            }
            else if (input.justPressed(MouseKeys.LEFT))
            {
                return (ActionTypes.NONE);
            }
            selected.move(input.simUnitPosition());
            return (ActionTypes.PRIS_SPOT);
        }
    }
}
