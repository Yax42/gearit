using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.robot;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;

namespace gearit.src.editor.robot.action
{
    class ActionSwapLimit : IAction
    {
        public void init() { }

        public bool shortcut()
        {
            return (Input.ctrlAltShift(false, false, true) && (Input.justPressed(Keys.F)));
        }

        public bool run(ref Robot robot, ref Piece selected1, ref Piece selected2)
        {
            ISpot spot = selected1.getConnection(selected2);
            if (spot != null && spot.GetType() == typeof(RevoluteSpot))
                ((RevoluteSpot)spot).LimitEnabled = !((RevoluteSpot)spot).LimitEnabled;
            return (false);
        }
    }
}
