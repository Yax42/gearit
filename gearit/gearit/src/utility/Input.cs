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
	MIDDLE
	}
    class Input
    {
        // Input for LUA
        private SortedList<string, Keys> _keys;

        // Mouse
        private MouseState _old_mouse;
        private MouseState _mouse;

        // Keyboard
        private KeyboardState _keyboard;
        private KeyboardState _old_keyboard;
        private Vector2 _cameraPos;

        public Input()
        {
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

            _keyboard = Keyboard.GetState();
            _mouse = Mouse.GetState();
        }

        public void update(Vector2 cameraPos)
        {
            _old_mouse = _mouse;
            _old_keyboard = _keyboard;
            _mouse = Mouse.GetState();
            _keyboard = Keyboard.GetState();
	    _cameraPos = cameraPos;
        }

        public bool mouseChanged()
        {
            return (_old_mouse != _mouse);
        }

/*******************/
/* MOUSE POSITIONS */
/*******************/
        public Vector2 position()
        {
            return (new Vector2(_mouse.X, _mouse.Y));
        }

        public Vector2 simUnitPosition()
        {
            return (ConvertUnits.ToSimUnits(new Vector2(_mouse.X, _mouse.Y) - _cameraPos));
        }
        public Vector2 mouseOffset()
        {
            return (new Vector2(_mouse.X - _old_mouse.X, _mouse.Y - _old_mouse.Y));
        }

/*****************/
/* MOUSE ACTIONS */
/*****************/

        public bool pressed(MouseKeys key)
        {
            if (key == MouseKeys.LEFT)
                return (_mouse.LeftButton == ButtonState.Pressed);
            if (key == MouseKeys.RIGHT)
                return (_mouse.RightButton == ButtonState.Pressed);
            return (_mouse.MiddleButton == ButtonState.Pressed);
        }

        public bool released(MouseKeys key)
        {
            return (!pressed(key));
        }

        public bool justPressed(MouseKeys key)
        {
            if (key == MouseKeys.LEFT)
                return (_mouse.LeftButton == ButtonState.Pressed && _old_mouse.LeftButton != ButtonState.Pressed);
            if (key == MouseKeys.RIGHT)
                return (_mouse.RightButton == ButtonState.Pressed && _old_mouse.RightButton != ButtonState.Pressed);
            return (_mouse.MiddleButton == ButtonState.Pressed && _old_mouse.MiddleButton != ButtonState.Pressed);
        }

        public bool justReleased(MouseKeys key)
        {
            if (key == MouseKeys.LEFT)
                return (_mouse.LeftButton != ButtonState.Pressed && _old_mouse.LeftButton == ButtonState.Pressed);
            if (key == MouseKeys.RIGHT)
                return (_mouse.MiddleButton != ButtonState.Pressed && _old_mouse.MiddleButton == ButtonState.Pressed);
            return (_mouse.MiddleButton != ButtonState.Pressed && _old_mouse.MiddleButton == ButtonState.Pressed);
        }

/********************/
/* KEYBOARD ACTIONS */
/********************/
        public bool pressed(Keys key)
        {
            return (_keyboard.IsKeyDown(key));
        }

        public bool released(Keys key)
        {
            return (!pressed(key));
        }

        public bool justPressed(Keys key)
        {
            return (_keyboard.IsKeyDown(key) && !_old_keyboard.IsKeyDown(key));
        }

        public bool justReleased(Keys key)
        {
            return (!_keyboard.IsKeyDown(key) && _old_keyboard.IsKeyDown(key));
        }

        public bool ctrlAltShift(bool ctrl, bool alt, bool shift)
        {
            return (pressed(Keys.LeftControl) == ctrl && pressed(Keys.LeftAlt) == alt && pressed(Keys.LeftShift) == shift);
        }

        /********************/
        /* KEYBOARD ACTIONS */
        /********************/

        public bool getKeysAction(string key)
        {
            _keyboard = Keyboard.GetState();
            if (_keyboard.IsKeyDown(_keys[key]))
                return (true);
            else
                return (false);
        }
    }
}
