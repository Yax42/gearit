using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
    class ActionMoveRobot : IAction
    {
        public void init() { }

        public bool shortcut()
        {
            return (Input.ctrlAltShift(false, false, false) && (Input.justPressed(Keys.C)));
        }

        public bool run(ref Robot robot, ref Piece selected1, ref Piece selected2)
        {
            robot.move(Input.SimMousePos);
            return (Input.pressed(Keys.C));
        }
    }
}
