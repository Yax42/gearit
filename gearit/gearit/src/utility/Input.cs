using System.Collections.Generic;
using gearit.src.editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using gearit.xna;

namespace gearit.src.utility
{
	enum MouseKeys
	{
		LEFT,
		RIGHT,
		MIDDLE,
		WHEEL_UP,
		WHEEL_DOWN
	}
	static class Input
	{
		// Input for LUA

		// Mouse
		static private MouseState _old_mouse;
		static private MouseState _mouse;

		// Keyboard
		static private KeyboardState _keyboard;
		static private KeyboardState _old_keyboard;

		static private bool _usedExit = false;

		static public void init()
		{
			_keyboard = Keyboard.GetState();
			_mouse = Mouse.GetState();
			VirtualSimMousePos = Vector2.Zero;

		}

		static public void update()
		{
			_usedExit = false;
			_old_mouse = _mouse;
			_old_keyboard = _keyboard;
			_mouse = Mouse.GetState();
			_keyboard = Keyboard.GetState();
		}

		static public bool Exit
		{
			get
			{
				//Microsoft.Xna.Framework.Input.GamePad.GetState
				if (justPressed(Keys.Escape) && !_usedExit)
				{
					_usedExit = true;
					return true;
				}
				else
					return false;
			}
		}

		static public Vector2 SimMousePos;
		static public Vector2 VirtualSimMousePos
		{
			get
			{
				var res = SimMousePos;
				if (LockAxis.Active)
					res = LockAxis.Transform(res);
				if (MirrorAxis.Active)
					res = MirrorAxis.Transform(res);
				return res;
			}
			set
			{
				SimMousePos = value;
			}

		}

		static public bool mouseChanged()
		{
			return (_old_mouse != _mouse);
		}

		/*******************/
		/* MOUSE POSITIONS */
		/*******************/
		static public Vector2 position()
		{
			return (new Vector2(_mouse.X, _mouse.Y));
		}

		static public Vector2 mouseOffset()
		{
			return (new Vector2(_mouse.X - _old_mouse.X, _mouse.Y - _old_mouse.Y));
		}

		/*****************/
		/* MOUSE ACTIONS */
		/*****************/

		static public bool pressed(MouseKeys key)
		{
			if (!ScreenManager.HasFocus) return false;
			if (key == MouseKeys.LEFT)
				return (_mouse.LeftButton == ButtonState.Pressed);
			if (key == MouseKeys.RIGHT)
				return (_mouse.RightButton == ButtonState.Pressed);
			if (key == MouseKeys.MIDDLE)
				return (_mouse.MiddleButton == ButtonState.Pressed);
			return (false);
		}

		static public bool released(MouseKeys key)
		{
			return (!pressed(key));
		}

		static public bool justPressed(MouseKeys key)
		{
			if (!ScreenManager.HasFocus) return false;
			if (key == MouseKeys.LEFT)
				return (_mouse.LeftButton == ButtonState.Pressed && _old_mouse.LeftButton != ButtonState.Pressed);
			if (key == MouseKeys.RIGHT)
				return (_mouse.RightButton == ButtonState.Pressed && _old_mouse.RightButton != ButtonState.Pressed);
			if (key == MouseKeys.MIDDLE)
				return (_mouse.MiddleButton == ButtonState.Pressed && _old_mouse.MiddleButton != ButtonState.Pressed);
			if (key == MouseKeys.WHEEL_UP)
				return (_mouse.ScrollWheelValue - _old_mouse.ScrollWheelValue < 0);
			if (key == MouseKeys.WHEEL_DOWN)
				return (_mouse.ScrollWheelValue - _old_mouse.ScrollWheelValue > 0);
			return (false);
		}

		static public bool justReleased(MouseKeys key)
		{
			if (!ScreenManager.HasFocus) return false;
			if (key == MouseKeys.LEFT)
				return (_mouse.LeftButton != ButtonState.Pressed && _old_mouse.LeftButton == ButtonState.Pressed);
			if (key == MouseKeys.RIGHT)
				return (_mouse.MiddleButton != ButtonState.Pressed && _old_mouse.MiddleButton == ButtonState.Pressed);
			if (key == MouseKeys.MIDDLE)
				return (_mouse.MiddleButton != ButtonState.Pressed && _old_mouse.MiddleButton == ButtonState.Pressed);
			return (false);
		}

		/********************/
		/* KEYBOARD ACTIONS */
		/********************/
		static public bool pressed(Keys key)
		{
			if (!ScreenManager.HasFocus) return false;
			return (_keyboard.IsKeyDown(key));
		}

		static public bool released(Keys key)
		{
			return (!pressed(key));
		}

		static public bool justPressed(Keys key)
		{
			if (!ScreenManager.HasFocus) return false;
			return (_keyboard.IsKeyDown(key) && !_old_keyboard.IsKeyDown(key));
		}

		static public bool justReleased(Keys key)
		{
			if (!ScreenManager.HasFocus) return false;
			return (!_keyboard.IsKeyDown(key) && _old_keyboard.IsKeyDown(key));
		}

		static public List<Keys> getJustPressed()
		{
			List<Keys> keys = new List<Keys>();

			if (!ScreenManager.HasFocus) return keys;

			if (_keyboard != _old_keyboard)
			{
				bool found;
				Keys[] new_keys = _keyboard.GetPressedKeys();
				Keys[] old_keys = _old_keyboard.GetPressedKeys();

				foreach (Keys new_key in new_keys)
				{
					found = false;
					foreach (Keys old_key in old_keys)
						if (new_key == old_key)
						{
							found = true;
							break;
						}
					if (!found)
						keys.Add(new_key);
				}
			}
			return (keys);
		}

		static public List<Keys> getJustReleased()
		{
			List<Keys> keys = new List<Keys>();
			if (!ScreenManager.HasFocus) return keys;

			if (_keyboard != _old_keyboard)
			{
				bool found;
				Keys[] new_keys = _keyboard.GetPressedKeys();
				Keys[] old_keys = _old_keyboard.GetPressedKeys();

				foreach (Keys old_key in old_keys)
				{
					found = false;
					foreach (Keys new_key in new_keys)
						if (new_key == old_key)
						{
							found = true;
							break;
						}
					if (!found)
						keys.Add(old_key);
				}
			}
			return (keys);
		}

		static public bool Alt
		{
			get
			{
				return pressed(Keys.LeftAlt) || pressed(Keys.RightAlt);
			}
		}

		static public bool Shift
		{
			get
			{
				return pressed(Keys.LeftShift) || pressed(Keys.RightShift);
			}
		}

		static public bool Ctrl
		{
			get
			{
				return pressed(Keys.LeftControl) || pressed(Keys.RightControl);
			}
		}

		static public bool CtrlAltShift(bool ctrl, bool alt, bool shift)
		{
			return (ctrl == Ctrl && alt == Alt && shift == Shift);
		}

		static public bool CtrlShift(bool ctrl, bool shift)
		{
			return (ctrl == Ctrl && shift == Shift);
		}
	}
}
