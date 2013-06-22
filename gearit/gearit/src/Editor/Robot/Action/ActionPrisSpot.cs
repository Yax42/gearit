using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using gearit.src.robot;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
    class ActionPrisSpot : IAction
    {
        private int _state;

        public void init()
        {
            _state = 0;
        }

        public bool shortcut(Input input)
        {
            return (input.ctrlAltShift(false, false, false) && input.justPressed(Keys.W));
        }

        public bool run(Input input, Robot robot, ref Piece selected1, ref Piece selected2)
        {
            if (_state == 0)
            {
                _state = 1;
                Piece p;
                if (ActionChooseSet.value)
                    p = new Wheel(robot, 0.5f, input.simUnitPosition());
                else
                    p = new Rod(robot, 2, input.simUnitPosition());
                new PrismaticSpot(robot, selected1, p);
                selected1 = p;
            }
            else if (input.justPressed(MouseKeys.LEFT))
                return (false);
            selected1.move(input.simUnitPosition());
            return (true);
        }
    }
}
