using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
	class ActionHide : IAction
	{
		public void init() { }

		public bool shortcut()
		{
			return (Input.justPressed(Keys.E));
		}

		public bool run(ref Robot robot, ref Piece selected1, ref Piece selected2)
		{
			selected1.Shown = !selected1.Shown;
			return (false);
		}
	}
}
