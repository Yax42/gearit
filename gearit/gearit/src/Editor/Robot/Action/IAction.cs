using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;

namespace gearit.src.editor.robot.action
{
    interface IAction
    {
        void init();
        bool shortcut(Input input);
        bool run(Input input, Robot robot, ref Piece selected1, ref Piece selected2);
    }
}
