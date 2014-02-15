using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gearit.src.output
{
	class OutputMessageManagerConsole : IOutputMessageManager
	{
		public void DisplayRobotScriptingErrorMessage(String msg)
		{
			Console.WriteLine("DisplayRobotScriptingErrorMessage: #{0}", msg);
		}
	}
}
