using System;
using gearit.src.utility;
using gearit.src.script;

namespace SquidXNA
{
#if WINDOWS || XBOX
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			using (Game game = new Game())
			{
				game.Run();
			}
			LuaScript.Clear();
		}
	}
#endif
}

