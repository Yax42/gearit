using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.robot;

namespace gearit.src.editor.robot.action
{
    class ActionResizePiece : IAction
    {
        private int _corner;
        private bool _moving;
        private bool _begin;
        private bool _side;

        public void init()
        {
            _corner = 0;
            _moving = false;
            _begin = true;
        }

        public bool shortcut(Input input)
        {
            return (input.ctrlAltShift(false, false, false) && input.justPressed(Keys.S));
        }

        public bool run(Input input, Robot robot, ref Piece selected1, ref Piece selected2)
        {

            if (selected1.GetType() == typeof(Heart))
            {
                if (input.justPressed(MouseKeys.LEFT))
                {
                    _moving = true;
                    _corner = robot.getHeart().getCorner(input.simUnitPosition());
                }
                if (input.pressed(MouseKeys.LEFT))
                {
                    if (input.justPressed(MouseKeys.RIGHT))
                    {
                        robot.getHeart().removeCorner(_corner);
                        _corner = 0;
                        _moving = false;
                    }
                    else if (_moving)
                        robot.getHeart().moveCorner(_corner, input.simUnitPosition());
                }
                else if (input.justPressed(MouseKeys.RIGHT))
                    robot.getHeart().addCorner(input.simUnitPosition());
                return (input.justPressed(Keys.S) == false);
            }
            else if (selected1.GetType() == typeof(Wheel))
                ((Wheel)selected1).Size = (input.simUnitPosition() - selected1.Position).Length();
            else
            {
                if (_begin)
                {
                    _side = ((Rod)selected1).getSide(input.simUnitPosition());
                    _begin = false;
                }
                ((Rod)selected1).setPos2(input.simUnitPosition(), _side);
            }
            return (input.justPressed(MouseKeys.LEFT) == false);
        }
    }
}
