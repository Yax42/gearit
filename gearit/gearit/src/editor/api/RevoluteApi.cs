using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.robot;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace gearit.src.editor.api
{
    class RevoluteApi : PieceApi
    {
        public RevoluteApi(ISpot spot) :
	    base(spot)
        {
        }

        public float angle()
        {
	    return (MathHelper.ToDegrees(((RevoluteSpot)_spot).JointAngle));
        }
    }
}
