using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.robot;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace gearit.src.script
{
	class RevoluteApi
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
				if (RobotLuaScript.IsNetwork)
					RobotLuaScript.NetworkGame.NetworkClient.PushRequest(RobotLuaScript.NetworkGame.PacketManager.Motor(_spot, value));
			}
		}

		public float Angle { get { return _spot.JointAngle; } }
		public float Cycle
		{
			get
			{
				float a = 0.5f * _spot.JointAngle / (float)Math.PI;
				float b =  a - (float) Math.Floor(a);
				if (a < 0)
					b -= 1;
				return b;
			}
		}

		public bool IsBetween(float min, float max)
		{
			float v = Cycle;
			Console.Out.WriteLine(v);
			return v >= min && v <= max;
		}

		public bool Frozen
		{
			get { return _spot.Frozen; }
			set
			{
				if (_spot.Frozen == value)
					return;
				_spot.Frozen = value;
				if (RobotLuaScript.IsNetwork)
				{
					var packet = RobotLuaScript.NetworkGame.PacketManager.Motor(_spot, Network.PacketManager.MotorType.Frozen, value);
					RobotLuaScript.NetworkGame.NetworkClient.PushRequest(packet);
				}
			}
		}

		public bool HitMax
		{
			get { return _spot.AutoFreezeState == FarseerPhysics.Dynamics.Joints.LimitState.AtUpper; }
		}

		public bool HitMin
		{
			get { return _spot.AutoFreezeState == FarseerPhysics.Dynamics.Joints.LimitState.AtLower; }
		}

		public void AddLimitsCycle(int count)
		{
			if (RobotLuaScript.IsNetwork)
				RobotLuaScript.NetworkGame.NetworkClient.PushRequest(RobotLuaScript.NetworkGame.PacketManager.Motor(_spot, count));
			_spot.AddLimitsCycle(count);
		}

		public bool FreeWheel
		{
			get { return _spot.FreeWheel; }
			set
			{
				if (_spot.FreeWheel == value)
					return;
				_spot.FreeWheel = value;
				if (RobotLuaScript.IsNetwork)
				{
					var packet = RobotLuaScript.NetworkGame.PacketManager.Motor(_spot, Network.PacketManager.MotorType.FreeWheel, value);
					RobotLuaScript.NetworkGame.NetworkClient.PushRequest(packet);
				}
			}
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
