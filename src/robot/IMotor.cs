using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gearit.src.robot
{
    interface IMotor
    {
        public void	run();
        public float	State;
	public bool	IsFree;
    }
}
