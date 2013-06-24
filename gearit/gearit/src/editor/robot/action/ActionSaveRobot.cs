using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;

namespace gearit.src.editor.robot.action
{
    class ActionSaveRobot : IAction
    {
        public void init() { }

        public bool shortcut()
        {
            return (Input.ctrlAltShift(true, false, false) && (Input.justPressed(Keys.S)));
        }

        public bool run(ref Robot robot, ref Piece selected1, ref Piece selected2)
        {
            Serializer.SerializeItem("r2d2.gir", robot);
            return (false);
        }
    }
}
