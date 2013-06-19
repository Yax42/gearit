using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;
using gearit.xna;
using gearit.src.robot;
using gearit.src.robot.api;

namespace gearit.src.utility
{


    class LuaTest
    {
        private Robot _robot;
        private List<Api> _api;
        private InputHelper _input;
        private String _name;
        private MyGame _game;
        public Lua _luaInterpret;

        public void saySomething(string something)
        {
            Console.WriteLine(something);
        }

        public LuaTest(ScreenManager screen, Robot robot, string name)
        {
            _input = new InputHelper(screen);
            _input.LoadContent();
            _robot = robot;
            _name = name;
            /*// Creating interpreter
            lua = new Lua();

            // Setting some global variable 
            lua["number"] = 5;
            lua["string"] = "foo";
            lua["nocast"] = 0xF00;

            // Get it
            double num = (double) lua["number"];
            string str = (string) lua["string"];

            // Display it
            Console.WriteLine("number:" + num);
            Console.WriteLine("string:" + str);
            Console.WriteLine("nocast:" + lua["nocast"]);

            // Register a function
            // Create an object (needed to call the function) or use this
            lua.RegisterFunction("saySomething", this, this.GetType().GetMethod("saySomething"));

            // Call the function with lua
            lua.DoString("saySomething('Motherfucker !'); saySomething('Command are separated with ; but not the last one !')");

            // Getting error
            try
            {
                lua.DoString("wtfamidoing");
            }
            catch (LuaException ex)
            {
                Console.WriteLine("Exception caught :");
                Console.WriteLine(ex.Message);
            }

            // File loading with Lua
            lua.DoFile("test.lua");*/

            /*Lua luaInterpret = new Lua();
            luaInterpret.DoFile(@"scripts/test.lua");
            string chaine = (string) luaInterpret["chaine"];
            double entier = (double) luaInterpret["entier"];
            Console.WriteLine("chaine = {0}= {1}", chaine, entier);*/
            
            _api = robot.getApi();

            _luaInterpret = new Lua();
            /*_luaInterpret.RegisterFunction("ChangeWheel", _game, _game.GetType().GetMethod("ChangeWheel"));*/
            /*_luaInterpret.RegisterFunction("getKeyboardState", _game, _game.GetType().GetMethod("getKeyboardState"));*/
            _luaInterpret.RegisterFunction("getRobot", this, this.GetType().GetMethod("getRobot"));
            _luaInterpret.RegisterFunction("getKeysAction", _input, _input.GetType().GetMethod("getKeysAction"));
            /*_luaInterpret.RegisterFunction("getWheel", _game, _game.GetType().GetMethod("getWheel"));*/

            //_luaInterpret["piece1"] = _api[1];
            for (int i = _api.Count - 1; i >= 0; i--)
                _luaInterpret[_api[i].name()] = _api[i];
        }

        public void getRobot()
        {
            Console.WriteLine(" {0} = {1}", _api[0].name(), _api[0].MotorForce);
            Console.WriteLine(" {0} = {1}", _api[1].name(), _api[1].MotorForce);
        }

        public void execFile()
        {
            _luaInterpret.DoFile(@"scripts/" + _name + ".lua");
        }
    }
}
