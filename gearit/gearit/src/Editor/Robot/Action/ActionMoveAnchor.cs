﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
    class ActionMoveAnchor : IAction
    {
        public void init() { }

        public bool shortcut(Input input)
        {
            return (input.ctrlAltShift(false, false, true) && input.justPressed(MouseKeys.RIGHT));
        }

        public bool run(Input input, Robot robot, ref Piece selected1, ref Piece selected2)
        {
            if (selected1.isConnected(selected2) == false)
                return (false);
            if (selected1.isOn(input.simUnitPosition()))
                selected1.getConnection(selected2).moveAnchor(selected1, input.simUnitPosition());
            return (input.pressed(MouseKeys.RIGHT));
        }
    }
}
