using gearit.src.utility;
using gearit.src.robot;

namespace gearit.src.editor.robot.action
{
    class ActionRevSpot : IAction
    {
        public void init() { }

        public ActionTypes run(Input input, Robot robot, ref Piece selected)
        {
            Piece p = new Wheel(robot, 0.5f, input.simUnitPosition());
            new RevoluteSpot(robot, selected, p);
            return (ActionTypes.NONE);
        }
    }
}
