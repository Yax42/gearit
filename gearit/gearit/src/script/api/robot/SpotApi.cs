﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.robot;

namespace gearit.src.script
{
	public abstract class SpotApi
	{
		internal ISpot _spot;

		public SpotApi(ISpot spot)
		{
			_spot = spot;
		}

		public string Name()
		{
			return (_spot.Name);
		}

		public float Motor
		{
			get { return _spot.Force; }
			set
			{
				if (value > 1)
					value = 1;
				else if (value < -1)
					value = -1;
				_spot.Force = value * _spot.MaxForce;
			}
		}
	}
}