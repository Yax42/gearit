using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
    class ActionDeletePiece : IAction
    {
        public void init() { }

        public bool shortcut()
        {
           return (Input.ctrlAltShift(false, false, false) && (Input.justPressed(Keys.Delete) || Input.justPressed(Keys.Back) || Input.justPressed(Keys.R)));
        }

        public bool run(ref Robot robot, ref Piece selected1, ref Piece selected2)
        {
            if (selected1 != robot.getHeart())
            {
		if (selected2 == selected1)
                  selected2 = robot.getHeart();

                robot.remove(selected1);
                selected1 = robot.getHeart();
            }
            return (false);
        }
    }
}
