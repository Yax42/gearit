using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.robot;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace gearit.src.editor.api
{
	class RevoluteApi : SpotApi
	{
		RevoluteSpot _revolute;
		public RevoluteApi(ISpot spot) :
		base(spot)
		{
			_revolute = (RevoluteSpot)spot;
		}

		public float Angle()
		{
			return (_revolute.JointAngle);
		}

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

		public bool Frozen
		{
			get { return _revolute.Frozen; }
			set { _revolute.Frozen = value; }
		}

		public bool LimitEnabled
		{
			get { return _revolute.LimitEnabled; }
			set { _revolute.LimitEnabled = value; }
		}
	}
}
