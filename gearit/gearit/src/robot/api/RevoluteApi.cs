using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gearit.src.robot.api
{
    class RevoluteApi : Api
    {
        public RevoluteApi(ISpot spot) :
	    base(spot)
        {
        }

        public float angle()
        {
	    return (((RevoluteSpot)_spot).JointAngle);
        }
    }
}
