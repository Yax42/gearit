using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.robot.Editor.Action
{
    class ActionNone : IAction
    {
        public void init() { }

        public ActionTypes run(Input input, Robot robot, ref Piece selected)
        {
            if (input.justPressed(MouseKeys.LEFT) && input.position().Y > 50)
                selected = robot.getPiece(input.simUnitPosition());
            if (input.pressed(MouseKeys.RIGHT))
                selected.move(input.simUnitPosition());
            if (input.justPressed(Keys.E))
                selected.Shown = !selected.Shown;
            if (input.justPressed(Keys.Space))
                robot.showAll();
            if (input.justPressed(Keys.Delete) || input.justPressed(Keys.Back) && selected != robot.getHeart())
            {
                robot.remove(selected);
                selected = robot.getHeart();
            }
            return (ActionTypes.NONE);
        }
    }
}
