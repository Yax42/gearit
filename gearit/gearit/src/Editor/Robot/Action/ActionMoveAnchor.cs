using System;
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

        public bool shortcut()
        {
            return (Input.ctrlAltShift(false, false, true) && Input.justPressed(MouseKeys.RIGHT));
        }

        public bool run(Robot robot, ref Piece selected1, ref Piece selected2)
        {
            if (selected1.isConnected(selected2) == false)
                return (false);
            if (selected1.isOn(Input.SimMousePos))
                selected1.getConnection(selected2).moveAnchor(selected1, Input.SimMousePos);
            return (Input.pressed(MouseKeys.RIGHT));
        }
    }
}
