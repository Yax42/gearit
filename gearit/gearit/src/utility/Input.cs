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
            _map_keys.Add(Keys.A, "a");
            _map_keys.Add(Keys.B, "b");
            _map_keys.Add(Keys.C, "c");
            _map_keys.Add(Keys.D, "d");
            _map_keys.Add(Keys.E, "e");
            _map_keys.Add(Keys.F, "f");
            _map_keys.Add(Keys.G, "g");
            _map_keys.Add(Keys.H, "h");
            _map_keys.Add(Keys.I, "i");
            _map_keys.Add(Keys.J, "j");
            _map_keys.Add(Keys.K, "k");
            _map_keys.Add(Keys.L, "l");
            _map_keys.Add(Keys.M, "m");
            _map_keys.Add(Keys.N, "n");
            _map_keys.Add(Keys.O, "o");
            _map_keys.Add(Keys.P, "p");
            _map_keys.Add(Keys.Q, "q");
            _map_keys.Add(Keys.R, "r");
            _map_keys.Add(Keys.S, "s");
            _map_keys.Add(Keys.T, "t");
            _map_keys.Add(Keys.U, "u");
            _map_keys.Add(Keys.V, "v");
            _map_keys.Add(Keys.W, "w");
            _map_keys.Add(Keys.X, "x");
            _map_keys.Add(Keys.Y, "y");
            _map_keys.Add(Keys.Z, "z");
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
            _map_keys.Add(Keys.D0, "0");
            _map_keys.Add(Keys.D1, "1");
            _map_keys.Add(Keys.D2, "2");
            _map_keys.Add(Keys.D3, "3");
            _map_keys.Add(Keys.D4, "4");
            _map_keys.Add(Keys.D5, "5");
            _map_keys.Add(Keys.D6, "6");
            _map_keys.Add(Keys.D7, "7");
            _map_keys.Add(Keys.D8, "8");
            _map_keys.Add(Keys.D9, "9");
            //_map_keys.Add(Keys.Space, " "); rajoute le si tu veux, je l'ai désactivé pour pas qu'on mette d'espaces dans les noms
            // END MAPPING
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

	/*
        static public bool getKeysAction(string key)
        {
            _keyboard = Keyboard.GetState();
            if (_keyboard.IsKeyDown(_keys[key]))
                return (true);
            else
                return (false);
        }
	*/
    }
}
