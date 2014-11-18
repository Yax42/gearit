using System.Collections.Generic;
using gearit.src.editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using gearit.xna;
using System;

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

		// Pad
		public const int NumberOfPad = 4;
		static private GamePadState[] _pad;
		static private GamePadState[] _old_pad;

		static private bool _usedExit = false;
		static private bool _usedEnter = false;

		static public void Init()
		{
			_pad = new  GamePadState[NumberOfPad];
			_old_pad = new  GamePadState[NumberOfPad];
			VirtualSimMousePos = Vector2.Zero;
			UpdateStates();
			_old_keyboard = Keyboard.GetState();
			_old_mouse = Mouse.GetState();
			for (int i = 0; i < NumberOfPad; i++)
				_old_pad[i] = GamePad.GetState((PlayerIndex) i);
		}

		static private void UpdateStates()
		{
			_keyboard = Keyboard.GetState();
			_mouse = Mouse.GetState();
			for (int i = 0; i < NumberOfPad; i++)
				_pad[i] = GamePad.GetState((PlayerIndex)i);
		
		}

		static public void Update()
		{
			_usedExit = false;
			_usedEnter = false;

			_old_mouse = _mouse;
			_old_keyboard = _keyboard;
			for (int i = 0; i < NumberOfPad; i++)
				_old_pad[i] = _pad[i];
			UpdateStates();
		}

		static public bool Enter
		{
			get
			{
				//Microsoft.Xna.Framework.Input.GamePad.GetState
				if (justReleased(Keys.Enter) && !_usedEnter)
				{
					_usedEnter = true;
					return true;
				}
				else
					return false;
			}
		}

		static public bool Exit
		{
			get
			{
				//Microsoft.Xna.Framework.Input.GamePad.GetState
				if (justReleased(Keys.Escape) && !_usedExit)
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

		#region Mouse
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
		#endregion

		#region Keyboard
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

		#region CtrlAltShift
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
		#endregion
		#endregion

		#region GamePad
		static private bool PadPressed(GamePadState state, Buttons button)
		{
			return state.IsButtonDown(button);
		}

		static public bool PadPressed(Buttons button, int id = 0)
		{
			if (!ScreenManager.HasFocus) return false;
			return PadPressed(_pad[id], button);
		}

		static public bool PadReleased(Buttons button, int id = 0)
		{
			if (!ScreenManager.HasFocus) return true;
			return (!PadPressed(button, id));
		}

		static public bool PadJustPressed(Buttons button, int id = 0)
		{
			if (!ScreenManager.HasFocus) return false;
			return PadPressed(_pad[id], button) && !PadPressed(_old_pad[id], button);
		}

		static public bool PadJustReleased(Buttons button, int id = 0)
		{
			if (!ScreenManager.HasFocus) return false;
			return !PadPressed(_pad[id], button) && PadPressed(_old_pad[id], button);
		}

		static public float PadTrigger(bool isLeft, int id = 0)
		{
			if (isLeft)
				return _pad[id].Triggers.Left;
			else
				return _pad[id].Triggers.Right;
		}

		static public float PadAngle(bool isLeft, int id = 0)
		{
			Vector2 dir;
			if (isLeft)
				dir = _pad[id].ThumbSticks.Left;
			else
				dir = _pad[id].ThumbSticks.Right;
			return (float) Math.Atan2(dir.X, dir.Y);
		}

		static public Vector2 PadStick(bool isLeft, int id = 0)
		{
			if (isLeft)
				return _pad[id].ThumbSticks.Left;
			else
				return _pad[id].ThumbSticks.Right;
		}

		#endregion
	}
}
