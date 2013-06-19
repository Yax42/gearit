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
        ActionTypes run(Input input, Robot robot, ref Piece selected);
    }
}
