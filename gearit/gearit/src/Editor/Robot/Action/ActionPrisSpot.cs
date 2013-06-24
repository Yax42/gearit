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

        public bool shortcut()
        {
            return (Input.ctrlAltShift(false, false, false) && Input.justPressed(Keys.W));
        }

        public bool run(ref Robot robot, ref Piece selected1, ref Piece selected2)
        {
            if (_state == 0)
            {
                _state = 1;
                Piece p;
                if (ActionChooseSet.value)
                    p = new Wheel(robot, 0.5f, Input.SimMousePos);
                else
                    p = new Rod(robot, 2, Input.SimMousePos);
                new PrismaticSpot(robot, selected1, p);
                selected1 = p;
            }
            else if (Input.justPressed(MouseKeys.LEFT))
                return (false);
            selected1.move(Input.SimMousePos);
            return (true);
        }
    }
}
