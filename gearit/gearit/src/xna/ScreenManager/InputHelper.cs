﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;

namespace gearit.xna
{
    /// <summary>
    ///   an enum of all available mouse buttons.
    /// </summary>
    public enum MouseButtons
    {
        LeftButton,
        MiddleButton,
        RightButton,
        ExtraButton1,
        ExtraButton2
    }

    public class InputHelper
    {
        private SortedList<string, Keys> _keys;

        private GamePadState _currentGamePadState;
        private KeyboardState _currentKeyboardState;
        private MouseState _currentMouseState;
        private GamePadState _currentVirtualState;

        private GamePadState _lastGamePadState;
        private KeyboardState _lastKeyboardState;
        private MouseState _lastMouseState;
        private GamePadState _lastVirtualState;
        private bool _handleVirtualStick;

        private Vector2 _cursor;
        private bool _cursorIsValid;
        private bool _cursorIsVisible;
        private bool _cursorMoved;
        private Sprite _cursorSprite;

#if WINDOWS_PHONE
        private VirtualStick _phoneStick;
        private VirtualButton _phoneA;
        private VirtualButton _phoneB;
#endif

        private ScreenManager _manager;
        private Viewport _viewport;

        /// <summary>
        ///   Constructs a new input state.
        /// </summary>
        public InputHelper(ScreenManager manager)
        {
            _currentKeyboardState = new KeyboardState();
            _currentGamePadState = new GamePadState();
            _currentMouseState = new MouseState();
            _currentVirtualState = new GamePadState();

            _lastKeyboardState = new KeyboardState();
            _lastGamePadState = new GamePadState();
            _lastMouseState = new MouseState();
            _lastVirtualState = new GamePadState();

            _manager = manager;

            _cursorIsVisible = false;
            _cursorMoved = false;
#if WINDOWS_PHONE
            _cursorIsValid = false;
#else
            _cursorIsValid = true;
#endif
            _cursor = Vector2.Zero;

            _handleVirtualStick = false;
        }

        public GamePadState GamePadState
        {
            get { return _currentGamePadState; }
        }

        public KeyboardState KeyboardState
        {
            get { return _currentKeyboardState; }
        }

        public MouseState MouseState
        {
            get { return _currentMouseState; }
        }

        public GamePadState VirtualState
        {
            get { return _currentVirtualState; }
        }

        public GamePadState PreviousGamePadState
        {
            get { return _lastGamePadState; }
        }

        public KeyboardState PreviousKeyboardState
        {
            get { return _lastKeyboardState; }
        }

        public MouseState PreviousMouseState
        {
            get { return _lastMouseState; }
        }

        public GamePadState PreviousVirtualState
        {
            get { return _lastVirtualState; }
        }


        public bool getKeysAction(string key)
        {
            _currentKeyboardState = Keyboard.GetState();
            if (_currentKeyboardState.IsKeyDown(_keys[key]))
                return (true);
            else
                return (false);
        }

        public bool ShowCursor
        {
            get { return _cursorIsVisible && _cursorIsValid; }
            set { _cursorIsVisible = value; }
        }

        public bool EnableVirtualStick
        {
            get { return _handleVirtualStick; }
            set { _handleVirtualStick = value; }
        }

        public Vector2 Cursor
        {
            get { return _cursor; }
        }

        public bool IsCursorMoved
        {
            get { return _cursorMoved; }
        }

        public bool IsCursorValid
        {
            get { return _cursorIsValid; }
        }

        public void LoadContent()
        {
            _keys = new SortedList<string, Keys>();
            _keys.Add("Q", Keys.Q);
            _keys.Add("D", Keys.D);
            _keys.Add("S", Keys.S);
            _keys.Add("A", Keys.A);
            _keys.Add("Z", Keys.Z);
            _keys.Add("W", Keys.W);
            _keys.Add("X", Keys.X);
            _keys.Add("E", Keys.E);
            _keys.Add("Space", Keys.Space);

            _cursorSprite = new Sprite(_manager.Content.Load<Texture2D>("Common/cursor"));
            _viewport = _manager.GraphicsDevice.Viewport;
        }

        /// <summary>
        ///   Reads the latest state of the keyboard and gamepad and mouse/touchpad.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            _lastKeyboardState = _currentKeyboardState;
            _lastGamePadState = _currentGamePadState;
            _lastMouseState = _currentMouseState;
            if (_handleVirtualStick)
            {
                _lastVirtualState = _currentVirtualState;
            }

            _currentKeyboardState = Keyboard.GetState();
            _currentGamePadState = GamePad.GetState(PlayerIndex.One);
            _currentMouseState = Mouse.GetState();

            if (_handleVirtualStick)
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    _currentVirtualState = GamePad.GetState(PlayerIndex.One);
                }
                else
                {
                    _currentVirtualState = HandleVirtualStickWin();
                }
            }

            // Update cursor
            Vector2 oldCursor = _cursor;
            if (_currentGamePadState.IsConnected && _currentGamePadState.ThumbSticks.Left != Vector2.Zero)
            {
                Vector2 temp = _currentGamePadState.ThumbSticks.Left;
                _cursor += temp * new Vector2(300f, -300f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Mouse.SetPosition((int)_cursor.X, (int)_cursor.Y);
            }
            else
            {
                _cursor.X = _currentMouseState.X;
                _cursor.Y = _currentMouseState.Y;
            }
            _cursor.X = MathHelper.Clamp(_cursor.X, 0f, _viewport.Width);
            _cursor.Y = MathHelper.Clamp(_cursor.Y, 0f, _viewport.Height);

            if (_cursorIsValid && oldCursor != _cursor)
            {
                _cursorMoved = true;
            }
            else
            {
                _cursorMoved = false;
            }

            if (_viewport.Bounds.Contains(_currentMouseState.X, _currentMouseState.Y))
            {
                _cursorIsValid = true;
            }
            else
            {
                _cursorIsValid = false;
            }
        }

        public void Draw()
        {
            if (_cursorIsVisible && _cursorIsValid)
            {
                _manager.SpriteBatch.Begin();
                _manager.SpriteBatch.Draw(_cursorSprite.Texture, _cursor, null, Color.White, 0f, _cursorSprite.Origin, 1f, SpriteEffects.None, 0f);
                _manager.SpriteBatch.End();
            }
        }

        private GamePadState HandleVirtualStickWin()
        {
            Vector2 _leftStick = Vector2.Zero;
            List<Buttons> _buttons = new List<Buttons>();

            if (_currentKeyboardState.IsKeyDown(Keys.A))
            {
                _leftStick.X -= 1f;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.S))
            {
                _leftStick.Y -= 1f;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.D))
            {
                _leftStick.X += 1f;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.W))
            {
                _leftStick.Y += 1f;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Space))
            {
                _buttons.Add(Buttons.A);
            }
            if (_currentKeyboardState.IsKeyDown(Keys.LeftControl))
            {
                _buttons.Add(Buttons.B);
            }
            if (_leftStick != Vector2.Zero)
            {
                _leftStick.Normalize();
            }

            return new GamePadState(_leftStick, Vector2.Zero, 0f, 0f, _buttons.ToArray());
        }

        private GamePadState HandleVirtualStickWP7()
        {
            List<Buttons> _buttons = new List<Buttons>();
            Vector2 _stick = Vector2.Zero;
            return new GamePadState(_stick, Vector2.Zero, 0f, 0f, _buttons.ToArray());
        }

        /// <summary>
        ///   Helper for checking if a key was newly pressed during this update.
        /// </summary>
        public bool IsNewKeyPress(Keys key)
        {
            return (_currentKeyboardState.IsKeyDown(key) &&
                    _lastKeyboardState.IsKeyUp(key));
        }

        public bool IsNewKeyRelease(Keys key)
        {
            return (_lastKeyboardState.IsKeyDown(key) &&
                    _currentKeyboardState.IsKeyUp(key));
        }

        /// <summary>
        ///   Helper for checking if a button was newly pressed during this update.
        /// </summary>
        public bool IsNewButtonPress(Buttons button)
        {
            return (_currentGamePadState.IsButtonDown(button) &&
                    _lastGamePadState.IsButtonUp(button));
        }

        public bool IsNewButtonRelease(Buttons button)
        {
            return (_lastGamePadState.IsButtonDown(button) &&
                    _currentGamePadState.IsButtonUp(button));
        }

        /// <summary>
        ///   Helper for checking if a mouse button was newly pressed during this update.
        /// </summary>
        public bool IsNewMouseButtonPress(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (_currentMouseState.LeftButton == ButtonState.Pressed &&
                            _lastMouseState.LeftButton == ButtonState.Released);
                case MouseButtons.RightButton:
                    return (_currentMouseState.RightButton == ButtonState.Pressed &&
                            _lastMouseState.RightButton == ButtonState.Released);
                case MouseButtons.MiddleButton:
                    return (_currentMouseState.MiddleButton == ButtonState.Pressed &&
                            _lastMouseState.MiddleButton == ButtonState.Released);
                case MouseButtons.ExtraButton1:
                    return (_currentMouseState.XButton1 == ButtonState.Pressed &&
                            _lastMouseState.XButton1 == ButtonState.Released);
                case MouseButtons.ExtraButton2:
                    return (_currentMouseState.XButton2 == ButtonState.Pressed &&
                            _lastMouseState.XButton2 == ButtonState.Released);
                default:
                    return false;
            }
        }


        /// <summary>
        /// Checks if the requested mouse button is released.
        /// </summary>
        /// <param name="button">The button.</param>
        public bool IsNewMouseButtonRelease(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (_lastMouseState.LeftButton == ButtonState.Pressed &&
                            _currentMouseState.LeftButton == ButtonState.Released);
                case MouseButtons.RightButton:
                    return (_lastMouseState.RightButton == ButtonState.Pressed &&
                            _currentMouseState.RightButton == ButtonState.Released);
                case MouseButtons.MiddleButton:
                    return (_lastMouseState.MiddleButton == ButtonState.Pressed &&
                            _currentMouseState.MiddleButton == ButtonState.Released);
                case MouseButtons.ExtraButton1:
                    return (_lastMouseState.XButton1 == ButtonState.Pressed &&
                            _currentMouseState.XButton1 == ButtonState.Released);
                case MouseButtons.ExtraButton2:
                    return (_lastMouseState.XButton2 == ButtonState.Pressed &&
                            _currentMouseState.XButton2 == ButtonState.Released);
                default:
                    return false;
            }
        }

        /// <summary>
        ///   Checks for a "menu select" input action.
        /// </summary>
        public bool IsMenuSelect()
        {
            return IsNewKeyPress(Keys.Space) ||
                   IsNewKeyPress(Keys.Enter) ||
                   IsNewButtonPress(Buttons.A) ||
                   IsNewButtonPress(Buttons.Start) ||
                   IsNewMouseButtonPress(MouseButtons.LeftButton);
        }

        public bool IsMenuPressed()
        {
            return _currentKeyboardState.IsKeyDown(Keys.Space) ||
                   _currentKeyboardState.IsKeyDown(Keys.Enter) ||
                   _currentGamePadState.IsButtonDown(Buttons.A) ||
                   _currentGamePadState.IsButtonDown(Buttons.Start) ||
                   _currentMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsMenuReleased()
        {
            return IsNewKeyRelease(Keys.Space) ||
                   IsNewKeyRelease(Keys.Enter) ||
                   IsNewButtonRelease(Buttons.A) ||
                   IsNewButtonRelease(Buttons.Start) ||
                   IsNewMouseButtonRelease(MouseButtons.LeftButton);
        }

        /// <summary>
        ///   Checks for a "menu cancel" input action.
        /// </summary>
        public bool IsMenuCancel()
        {
            return IsNewKeyPress(Keys.Escape) ||
                   IsNewButtonPress(Buttons.Back);
        }
    }
}