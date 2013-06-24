﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.robot;

namespace gearit.src.editor.robot.action
{
    class ActionPrisLink : IAction
    {
        public void init() { }

        public bool shortcut()
        {
            return (Input.ctrlAltShift(false, false, true) && Input.justPressed(Keys.W));
        }

        public bool run(ref Robot robot, ref Piece selected1, ref Piece selected2)
        {
            if (selected1 != selected2 && selected1.isConnected(selected2) == false)
              new PrismaticSpot(robot, selected1, selected2);
            return (false);
        }
    }
}
