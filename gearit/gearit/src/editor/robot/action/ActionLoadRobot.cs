using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
    class ActionLoadRobot : IAction
    {
        public void init() { }

        public bool shortcut()
        {
            return (Input.ctrlAltShift(true, false, false) && (Input.justPressed(Keys.D)));
        }

        public bool run(ref Robot robot, ref Piece selected1, ref Piece selected2)
        {
            robot.remove();
            robot = (Robot)RobotEditor._serial.DeserializeItem("r2d2.bot");
            selected2 = robot.getHeart();
            selected1 = robot.getHeart();
            return (false);
        }
    }
}
