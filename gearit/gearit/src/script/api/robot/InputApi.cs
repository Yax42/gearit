using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.script
{
    /// <summary>
    /// Lua API for accessing input
    /// </summary>
	class InputApi
	{
		public InputApi()
		{
		}

		public bool Pressed(int key)
		{
			return (Input.pressed((Keys)key));
		}

		public bool Released(int key)
		{
			return (Input.released((Keys)key));
		}

		public bool JustPressed(int key)
		{
			return (Input.justPressed((Keys)key));
		}

		public bool JustReleased(int key)
		{
			return (Input.justReleased((Keys)key));
		}

		public bool pressed(int key)
		{
			return (Input.pressed((Keys)key));
		}

		public bool released(int key)
		{
			return (Input.released((Keys)key));
		}

		public bool justPressed(int key)
		{
			return (Input.justPressed((Keys)key));
		}

		public bool justReleased(int key)
		{
			return (Input.justReleased((Keys)key));
		}

		public bool PadPressed(int button, int id = 0)
		{
			return Input.PadPressed((Buttons)button, id);
		}

		public bool PadReleased(int button, int id = 0)
		{
			return Input.PadReleased((Buttons)button, id);
		}

		public bool PadJustPressed(int button, int id = 0)
		{
			return Input.PadJustPressed((Buttons)button, id);
		}

		public bool PadJustReleased(int button, int id = 0)
		{
			return Input.PadJustReleased((Buttons)button, id);
		}

		public float PadTrigger(bool isLeft, int id = 0)
		{
			return Input.PadTrigger(isLeft, id);
		}

		public float PadStick_X(bool isLeft, int id = 0)
		{
			return Input.PadStick(isLeft, id).X;
		}

		public float PadStick_Y(bool isLeft, int id = 0)
		{
			return Input.PadStick(isLeft, id).Y;
		}

		public float PadAngle(bool isLeft, int id = 0)
		{
			return Input.PadAngle(isLeft, id);
		}
	}
}