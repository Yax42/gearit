using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gearit.src.robot.api
{
    class PrismaticApi : Api
    {
        public PrismaticApi(ISpot spot) :
	    base(spot)
        {
        }

        public float size()
        {
            return (((PrismaticSpot)_spot).getSize());
        }
    }
}
