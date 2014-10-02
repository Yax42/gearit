using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.robot;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace gearit.src.script
{
	public class RevoluteApi
	{
		internal RevoluteSpot _spot;

		public RevoluteApi(RevoluteSpot spot)
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
				_spot.Force = value;
			}
		}

		public float Angle { get { return _spot.JointAngle; } }

		public bool Frozen
		{
			get { return _spot.Frozen; }
			set { _spot.Frozen = value; }
		}

		public bool HitMax
		{
			get { return _spot.LimitState == FarseerPhysics.Dynamics.Joints.LimitState.AtUpper; }
		}

		public bool HitMin
		{
			get { return _spot.LimitState == FarseerPhysics.Dynamics.Joints.LimitState.AtLower; }
		}

#if false //Ces fontionnalités me paraissent dangereuses à être modifiable en temps réel
		public float MaxAngle
		{
			get { return _revolute.MaxAngle; }
			set { _revolute.MaxAngle = value; }
		}

		public float MinAngle
		{
			get { return _revolute.MinAngle; }
			set { _revolute.MinAngle = value; }
		}


		public bool Limited
		{
			get { return _revolute.SpotLimitEnabled; }
			set { _revolute.SpotLimitEnabled = value; }
		}
#endif
	}
}
