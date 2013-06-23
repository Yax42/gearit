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

        public bool shortcut()
        {
           return (Input.ctrlAltShift(false, false, true) && (Input.justPressed(Keys.Delete) || Input.justPressed(Keys.Back) || Input.justPressed(Keys.R)));
        }

        public bool run(Robot robot, ref Piece selected1, ref Piece selected2)
        {
            if (selected1.isConnected(selected2))
              robot.remove(selected1.getConnection(selected2));
            return (false);
        }
    }
}
