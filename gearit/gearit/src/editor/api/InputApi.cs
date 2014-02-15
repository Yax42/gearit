using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.api
{
	class InputApi
	{
		public InputApi()
		{
		}

		public bool pressed(int key)
		{
			return (Input.pressed((Keys)key));
		}

		public bool released(int key)
		{
			return (Input.released((Keys)key));
		}

	/*
		public bool justPressed(int key)
		{
			return (Input.justPressed((Keys)key));
		}

		public bool justReleased(int key)
		{
			return (Input.justReleased((Keys)key));
		}
	*/
	}
}
