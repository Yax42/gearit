﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
    class ActionLaunch : IAction
    {
        public void init() { }

        public bool shortcut(Input input)
        {
            return (input.ctrlAltShift(false, false, false) && input.justPressed(Keys.Enter));
        }

        public bool run(Input input, Robot robot, ref Piece selected1, ref Piece selected2)
        {
            robot.getWorld().Gravity = new Vector2(0f, 9.8f);
            return (true);
        }
    }
}