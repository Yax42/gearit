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

        public bool shortcut(Input input)
        {
            return (input.ctrlAltShift(false, false, false) && input.justPressed(MouseKeys.RIGHT));
        }

        public bool run(Input input, Robot robot, ref Piece selected1, ref Piece selected2)
        {
            selected1.move(input.simUnitPosition());
            return (input.pressed(MouseKeys.RIGHT));
        }
    }
}
