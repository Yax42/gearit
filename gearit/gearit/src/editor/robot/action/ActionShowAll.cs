using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
    class ActionShowAll : IAction
    {
        public void init() { }

        public bool shortcut(Input input)
        {
            return (input.justPressed(Keys.Space));
        }

        public bool run(Input input, Robot robot, ref Piece selected1, ref Piece selected2)
        {
            robot.showAll();
            return (false);
        }
    }
}
