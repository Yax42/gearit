using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

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
        static private SortedList<string, Keys> _keys;
        static Dictionary<Keys, string> _map_keys;

        // Mouse
        static private MouseState _old_mouse;
        static private MouseState _mouse;

        // Keyboard
        static private KeyboardState _keyboard;
        static private KeyboardState _old_keyboard;

        static public void init()
        {
            _keyboard = Keyboard.GetState();
            _mouse = Mouse.GetState();
            SimMousePos = Vector2.Zero;

            // MAPPING DICTONARY KEYBOARD
            _map_keys = new Dictionary<Keys, string>();
            _map_keys.Add(Keys.A, "A");
            _map_keys.Add(Keys.B, "B");
            _map_keys.Add(Keys.C, "C");
            _map_keys.Add(Keys.D, "D");
            _map_keys.Add(Keys.E, "E");
            _map_keys.Add(Keys.F, "F");
            _map_keys.Add(Keys.G, "G");
            _map_keys.Add(Keys.H, "H");
            _map_keys.Add(Keys.I, "I");
            _map_keys.Add(Keys.J, "J");
            _map_keys.Add(Keys.K, "K");
            _map_keys.Add(Keys.L, "L");
            _map_keys.Add(Keys.M, "M");
            _map_keys.Add(Keys.N, "N");
            _map_keys.Add(Keys.O, "O");
            _map_keys.Add(Keys.P, "P");
            _map_keys.Add(Keys.Q, "Q");
            _map_keys.Add(Keys.R, "R");
            _map_keys.Add(Keys.S, "S");
            _map_keys.Add(Keys.T, "T");
            _map_keys.Add(Keys.U, "U");
            _map_keys.Add(Keys.V, "V");
            _map_keys.Add(Keys.W, "W");
            _map_keys.Add(Keys.X, "X");
            _map_keys.Add(Keys.Y, "Y");
            _map_keys.Add(Keys.Z, "Z");
            _map_keys.Add(Keys.NumPad0, "0");
            _map_keys.Add(Keys.NumPad1, "1");
            _map_keys.Add(Keys.NumPad2, "2");
            _map_keys.Add(Keys.NumPad3, "3");
            _map_keys.Add(Keys.NumPad4, "4");
            _map_keys.Add(Keys.NumPad5, "5");
            _map_keys.Add(Keys.NumPad6, "6");
            _map_keys.Add(Keys.NumPad7, "7");
            _map_keys.Add(Keys.NumPad8, "8");
            _map_keys.Add(Keys.NumPad9, "9");
            _map_keys.Add(Keys.Space, " ");
            // END MAPPING

            _keys = new SortedList<string, Keys>();
            _keys.Add("A", Keys.A);
            _keys.Add("B", Keys.B);
            _keys.Add("C", Keys.C);
            _keys.Add("D", Keys.D);
            _keys.Add("E", Keys.E);
            _keys.Add("F", Keys.F);
            _keys.Add("G", Keys.G);
            _keys.Add("H", Keys.H);
            _keys.Add("I", Keys.I);
            _keys.Add("J", Keys.J);
            _keys.Add("K", Keys.K);
            _keys.Add("L", Keys.L);
            _keys.Add("M", Keys.M);
            _keys.Add("N", Keys.N);
            _keys.Add("O", Keys.O);
            _keys.Add("P", Keys.P);
            _keys.Add("Q", Keys.Q);
            _keys.Add("R", Keys.R);
            _keys.Add("S", Keys.S);
            _keys.Add("T", Keys.T);
            _keys.Add("U", Keys.U);
            _keys.Add("V", Keys.V);
            _keys.Add("W", Keys.W);
            _keys.Add("X", Keys.X);
            _keys.Add("Y", Keys.Y);
            _keys.Add("Z", Keys.Z);
            _keys.Add("Space", Keys.Space);
            _keys.Add("Shift", Keys.LeftShift);
            _keys.Add("Ctrl-L", Keys.LeftControl);
            _keys.Add("Ctrl-R", Keys.RightControl);
        }

        static public void update()
        {
            _old_mouse = _mouse;
            _old_keyboard = _keyboard;
            _mouse = Mouse.GetState();
            _keyboard = Keyboard.GetState();
        }

        static public Vector2 SimMousePos
        {
            get;
            set;
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
            return (_keyboard.IsKeyDown(key));
        }

        static public bool released(Keys key)
        {
            return (!pressed(key));
        }

        static public bool justPressed(Keys key)
        {
            return (_keyboard.IsKeyDown(key) && !_old_keyboard.IsKeyDown(key));
        }

        static public bool justReleased(Keys key)
        {
            return (!_keyboard.IsKeyDown(key) && _old_keyboard.IsKeyDown(key));
        }

        static public List<Keys> getJustPressed()
        {
            List<Keys> keys = new List<Keys>();

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

        static public string keyToString(Keys key)
        {
            if (_map_keys.ContainsKey(key))
                return (_map_keys[key]);
            else
                return ("");
        }

        static public bool ctrlAltShift(bool ctrl, bool alt, bool shift)
        {
            return (pressed(Keys.LeftControl) == ctrl && pressed(Keys.LeftAlt) == alt && pressed(Keys.LeftShift) == shift);
        }

        /********************/
        /* KEYBOARD ACTIONS */
        /********************/

        static public bool getKeysAction(string key)
        {
            _keyboard = Keyboard.GetState();
            if (_keyboard.IsKeyDown(_keys[key]))
                return (true);
            else
                return (false);
        }
    }
}
