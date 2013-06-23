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

        public bool shortcut()
        {
            return (Input.ctrlAltShift(false, false, false) && Input.justPressed(Keys.S));
        }

        public bool run(Robot robot, ref Piece selected1, ref Piece selected2)
        {

            if (selected1.GetType() == typeof(Heart))
            {
                if (Input.justPressed(MouseKeys.LEFT))
                {
                    _moving = true;
                    _corner = robot.getHeart().getCorner(Input.SimMousePos);
                }
                if (Input.pressed(MouseKeys.LEFT))
                {
                    if (Input.justPressed(MouseKeys.RIGHT))
                    {
                        robot.getHeart().removeCorner(_corner);
                        _corner = 0;
                        _moving = false;
                    }
                    else if (_moving)
                        robot.getHeart().moveCorner(_corner, Input.SimMousePos);
                }
                else if (Input.justPressed(MouseKeys.RIGHT))
                    robot.getHeart().addCorner(Input.SimMousePos);
                return (Input.justPressed(Keys.S) == false);
            }
            else if (selected1.GetType() == typeof(Wheel))
            {
                ((Wheel)selected1).Size = (Input.SimMousePos - selected1.Position).Length();
                return (Input.justPressed(MouseKeys.LEFT) == false);
            }
            else
            {
                if (Input.justPressed(MouseKeys.LEFT))
                {
                    if (_begin)
                        _begin = false;
                    else
                        return (false);
                }
                if (_begin)
                    ((Rod)selected1).Size = (Input.SimMousePos - selected1.Position).Length();
                else
                    ((Rod)selected1).setPos2(Input.SimMousePos, true);
                return (true);
            }
        }
    }
}
