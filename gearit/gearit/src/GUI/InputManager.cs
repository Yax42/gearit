using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SquidXNA
{
    public class InputManager : GameComponent
    {
        private class InputKey
        {
            public Keys Key = Keys.None;
            public int ScanCode = 0;
            public double Repeat = REPEAT_DELAY;
        }

        private static Dictionary<Keys, int> SpecialKeys = new Dictionary<Keys, int>();
        private Dictionary<Keys, InputKey> InputKeys = new Dictionary<Keys, InputKey>();

        private KeyboardState LastKeyboardState;
        private int LastScroll;

        private const int REPEAT_DELAY = 500;
        private const int REPEAT_RATE = 25;

        public InputManager(Game game)
            : base(game)
        {
            SpecialKeys.Add(Keys.Home, 0xC7);
            SpecialKeys.Add(Keys.Up, 0xC8);
            SpecialKeys.Add(Keys.Left, 0xCB);
            SpecialKeys.Add(Keys.Right, 0xCD);
            SpecialKeys.Add(Keys.End, 0xCF);
            SpecialKeys.Add(Keys.Down, 0xD0);
            SpecialKeys.Add(Keys.Insert, 0xD2);
            SpecialKeys.Add(Keys.Delete, 0xD3);
            SpecialKeys.Add(Keys.MediaPreviousTrack, 0x90);

            foreach (Keys k in System.Enum.GetValues(typeof(Keys)))
            {
                InputKey key = new InputKey();
                key.Key = k;
                key.ScanCode = VirtualKeyToScancode(k);
                InputKeys.Add(k, key);
            }
        }

        private int VirtualKeyToScancode(Keys key)
        {
            int sc = RendererXNA.VirtualKeyToScancode((int)key);

            if (SpecialKeys.ContainsKey(key))
                sc = SpecialKeys[key];

            return sc;
        }


        public override void Update(GameTime gameTime)
        {
            // Mouse
            MouseState mouseState = Mouse.GetState();

            //if(mouseState.LeftButton == ButtonState.Pressed)
            //    System.Diagnostics.Debug.WriteLine("Mouse Clicked."); 

            int wheel = mouseState.ScrollWheelValue > LastScroll ? -1 : (mouseState.ScrollWheelValue < LastScroll ? 1 : 0);
            LastScroll = mouseState.ScrollWheelValue;

            Squid.GuiHost.SetMouse(mouseState.X, mouseState.Y, wheel);
            Squid.GuiHost.SetButtons(mouseState.LeftButton == ButtonState.Pressed, mouseState.RightButton == ButtonState.Pressed);

            // Keyboard
            KeyboardState keyboardState = Keyboard.GetState();
            List<Squid.KeyData> squidKeys = new List<Squid.KeyData>();

            double ms = Squid.GuiHost.TimeElapsed;

            Keys[] now = keyboardState.GetPressedKeys();
            Keys[] last = LastKeyboardState.GetPressedKeys();

            foreach (Keys key in now)
            {
                bool wasDown = LastKeyboardState.IsKeyDown(key);
              
                InputKeys[key].Repeat -= ms;

                if (InputKeys[key].Repeat < 0 || !wasDown)
                {
                    squidKeys.Add(new Squid.KeyData()
                    {
                        Scancode = InputKeys[key].ScanCode,
                        Pressed = true
                    });
                    InputKeys[key].Repeat = !wasDown ? REPEAT_DELAY : REPEAT_RATE;
                }
            }

            foreach (Keys key in last)
            {
                bool isDown = keyboardState.IsKeyDown(key);

                if (!isDown)
                {
                    squidKeys.Add(new Squid.KeyData()
                    {
                        Scancode = InputKeys[key].ScanCode,
                        Released = true
                    });
                    InputKeys[key].Repeat = REPEAT_DELAY;
                }
            }

            LastKeyboardState = keyboardState;

            Squid.GuiHost.SetKeyboard(squidKeys.ToArray());
            Squid.GuiHost.TimeElapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            base.Update(gameTime);
        }
    }
}
