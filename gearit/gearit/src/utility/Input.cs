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
    class Input
    {
        // Mouse
        private MouseState _old_mouse;
        private MouseState _mouse;
        private ICamera _camera;

        // Keyboard
        private KeyboardState _keyboard;
        private KeyboardState _old_keyboard;

        public Input(ICamera camera)
        {
            _camera = camera;
            _keyboard = Keyboard.GetState();
            _mouse = Mouse.GetState();
        }

        public void update()
        {
            _old_mouse = _mouse;
            _old_keyboard = _keyboard;
            _mouse = Mouse.GetState();
            _keyboard = Keyboard.GetState();
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
            return (ConvertUnits.ToSimUnits(position()) / _camera.Zoom + _camera.Position + (_camera.center() - _camera.center() / _camera.Zoom));
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
            if (key == MouseKeys.MIDDLE)
               return (_mouse.MiddleButton == ButtonState.Pressed);
	    return (false);
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
            if (key == MouseKeys.MIDDLE)
                return (_mouse.MiddleButton == ButtonState.Pressed && _old_mouse.MiddleButton != ButtonState.Pressed);
            if (key == MouseKeys.WHEEL_UP)
                return (_mouse.ScrollWheelValue - _old_mouse.ScrollWheelValue < 0);
            if (key == MouseKeys.WHEEL_DOWN)
                return (_mouse.ScrollWheelValue - _old_mouse.ScrollWheelValue > 0);
	    return (false);
        }

        public bool justReleased(MouseKeys key)
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
    }
}
