using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
    class ActionMovePiece : IAction
    {
        public void init() { }

        public bool shortcut()
        {
            return (Input.ctrlAltShift(false, false, false) && Input.justPressed(MouseKeys.RIGHT));
        }

        public bool run(Robot robot, ref Piece selected1, ref Piece selected2)
        {
            selected1.move(Input.SimMousePos);
            return (Input.pressed(MouseKeys.RIGHT));
        }
    }
}
