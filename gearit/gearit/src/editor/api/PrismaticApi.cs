using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.robot;

namespace gearit.src.editor.api
{
	class PrismaticApi : SpotApi
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
