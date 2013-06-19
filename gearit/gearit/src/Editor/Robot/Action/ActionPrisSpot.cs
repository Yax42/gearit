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
        private int _state;

        public void init()
        {
            _state = 0;
        }

        public ActionTypes run(Input input, Robot robot, ref Piece selected)
        {
            if (_state == 0)
            {
                _state = 1;
                Piece p = new Wheel(robot, 0.5f, input.simUnitPosition());
                new PrismaticSpot(robot, selected, p);
                selected = p;
            }
            else if (_state == 1)
            {
                if (input.released(MouseKeys.LEFT))
                    _state = 2;
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
