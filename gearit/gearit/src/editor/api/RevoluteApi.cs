using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.robot;

namespace gearit.src.editor.api
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
