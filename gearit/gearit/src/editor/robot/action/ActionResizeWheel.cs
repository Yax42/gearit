using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
    class ActionResizeWheel : IAction
    {
        public void init() { }

        public bool shortcut(Input input)
        {
            return (input.ctrlAltShift(false, false, false) && input.justPressed(Keys.S));
        }

        public bool run(Input input, Robot robot, ref Piece selected1, ref Piece selected2)
        {
            if (selected1 == robot.getHeart())
		return (false);
            ((Wheel)selected1).Size = (input.simUnitPosition() - selected1.Position).Length();
            return (input.justPressed(MouseKeys.LEFT) == false);
        }
    }
}
