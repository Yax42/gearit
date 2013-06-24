using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
    class ActionMainSelect : IAction
    {
        public void init() { }

        public bool shortcut()
        {
            return (Input.ctrlAltShift(false, false, false) && Input.justPressed(MouseKeys.LEFT));
        }

        public bool run(ref Robot robot, ref Piece selected1, ref Piece selected2)
        {
            selected1 = robot.getPiece(Input.SimMousePos);
            return (false);
        }
    }
}
