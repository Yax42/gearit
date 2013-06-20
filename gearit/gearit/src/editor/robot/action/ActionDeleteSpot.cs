using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
    class ActionDeleteSpot : IAction
    {
        public void init() { }

        public bool shortcut(Input input)
        {
           return (input.ctrlAltShift(false, false, true) && (input.justPressed(Keys.Delete) || input.justPressed(Keys.Back) || input.justPressed(Keys.R)));
        }

        public bool run(Input input, Robot robot, ref Piece selected1, ref Piece selected2)
        {
            if (selected1.isConnected(selected2))
              robot.remove(selected1.getConnection(selected2));
            return (false);
        }
    }
}
