using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
    class ActionResizeHeart : IAction
    {
        private int _corner;
        private bool _moving;

        public void init() { _corner = 0; }

        public bool shortcut(Input input)
        {
            return (input.ctrlAltShift(false, false, true) && input.justPressed(Keys.S));
        }

        public bool run(Input input, Robot robot, ref Piece selected1, ref Piece selected2)
        {
            if (input.justPressed(MouseKeys.LEFT))
            {
                _moving = true;
                _corner = robot.getHeart().getCorner(input.simUnitPosition());
            }
            if (input.pressed(MouseKeys.LEFT))
            {
                if (input.justPressed(MouseKeys.RIGHT))
                {
                    robot.getHeart().removeCorner(_corner);
                    _corner = 0;
                    _moving = false;
                }
                else if (_moving)
                  robot.getHeart().moveCorner(_corner, input.simUnitPosition());
            }
            else if (input.justPressed(MouseKeys.RIGHT))
                robot.getHeart().addCorner(input.simUnitPosition());
            return (input.justPressed(Keys.S) == false && input.justPressed(Keys.CapsLock) == false && input.justPressed(Keys.Escape) == false);
        }
    }
}
