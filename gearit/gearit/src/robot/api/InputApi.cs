using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.robot.api
{
    class InputApi
    {
        static private SortedList<string, Keys> _keysMap;

        public InputApi()
        {
            _keysMap = new SortedList<string, Keys>();
            _keysMap.Add("A", Keys.A);
            _keysMap.Add("B", Keys.B);
            _keysMap.Add("C", Keys.C);
            _keysMap.Add("D", Keys.D);
            _keysMap.Add("E", Keys.E);
            _keysMap.Add("F", Keys.F);
            _keysMap.Add("G", Keys.G);
            _keysMap.Add("H", Keys.H);
            _keysMap.Add("I", Keys.I);
            _keysMap.Add("J", Keys.J);
            _keysMap.Add("K", Keys.K);
            _keysMap.Add("L", Keys.L);
            _keysMap.Add("M", Keys.M);
            _keysMap.Add("N", Keys.N);
            _keysMap.Add("O", Keys.O);
            _keysMap.Add("P", Keys.P);
            _keysMap.Add("Q", Keys.Q);
            _keysMap.Add("R", Keys.R);
            _keysMap.Add("S", Keys.S);
            _keysMap.Add("T", Keys.T);
            _keysMap.Add("U", Keys.U);
            _keysMap.Add("V", Keys.V);
            _keysMap.Add("W", Keys.W);
            _keysMap.Add("X", Keys.X);
            _keysMap.Add("Y", Keys.Y);
            _keysMap.Add("Z", Keys.Z);
            _keysMap.Add("0", Keys.D0);
            _keysMap.Add("1", Keys.D1);
            _keysMap.Add("2", Keys.D2);
            _keysMap.Add("3", Keys.D3);
            _keysMap.Add("4", Keys.D4);
            _keysMap.Add("5", Keys.D5);
            _keysMap.Add("6", Keys.D6);
            _keysMap.Add("7", Keys.D7);
            _keysMap.Add("8", Keys.D8);
            _keysMap.Add("9", Keys.D9);
            _keysMap.Add("NumPad0", Keys.NumPad0);
            _keysMap.Add("NumPad1", Keys.NumPad1);
            _keysMap.Add("NumPad2", Keys.NumPad2);
            _keysMap.Add("NumPad3", Keys.NumPad3);
            _keysMap.Add("NumPad4", Keys.NumPad4);
            _keysMap.Add("NumPad5", Keys.NumPad5);
            _keysMap.Add("NumPad6", Keys.NumPad6);
            _keysMap.Add("NumPad7", Keys.NumPad7);
            _keysMap.Add("NumPad8", Keys.NumPad8);
            _keysMap.Add("NumPad9", Keys.NumPad9);
            _keysMap.Add("Space", Keys.Space);
            _keysMap.Add("LShift", Keys.LeftShift);
            _keysMap.Add("RShift", Keys.RightShift);
            _keysMap.Add("RCtrl", Keys.LeftControl);
            _keysMap.Add("LCtrl", Keys.RightControl);
            _keysMap.Add("Left", Keys.Left);
            _keysMap.Add("Right", Keys.Right);
            _keysMap.Add("Up", Keys.Up);
            _keysMap.Add("Down", Keys.Down);
            _keysMap.Add("Enter", Keys.Enter);
        }

        public bool justPressed(int key)
        {
            return (Input.justPressed((Keys)key));
        }

        public bool pressed(int key)
        {
            return (Input.pressed((Keys)key));
        }

        public bool justReleased(int key)
        {
            return (Input.justReleased((Keys)key));
        }

        public bool released(int key)
        {
            return (Input.released((Keys)key));
        }

        public bool justPressed(string key)
        {
            if (_keysMap.ContainsKey(key))
                return (Input.justPressed(_keysMap[key]));
            else
                return (false);
        }
    }
}
