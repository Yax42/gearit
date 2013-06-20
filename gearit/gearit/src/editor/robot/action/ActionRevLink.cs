using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.robot;

namespace gearit.src.editor.robot.action
{
    class ActionRevLink : IAction
    {
        public void init() { }

        public bool shortcut(Input input)
        {
            return (input.ctrlAltShift(false, false, true) && input.justPressed(Keys.Q));
        }

        public bool run(Input input, Robot robot, ref Piece selected1, ref Piece selected2)
        {
            if (selected1.isConnected(selected2) == false)
              new RevoluteSpot(robot, selected1, selected2);
            return (false);
        }
    }
}
