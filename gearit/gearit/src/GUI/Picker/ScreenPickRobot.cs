using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gearit.src.GUI.Picker
{
	class ScreenPickRobot : AScreenPickItem
	{
		public static ScreenPickRobot Instance { get; private set; }

		public ScreenPickRobot() : base(false)
		{
			Instance = this;
		}
	}
}
