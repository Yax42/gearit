using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gearit.src.GUI.Picker
{
	class ScreenPickMap : AScreenPickItem
	{
		public static ScreenPickMap Instance { get; private set; }

		public ScreenPickMap() : base(true)
		{
			Instance = this;
		}
	}
}
