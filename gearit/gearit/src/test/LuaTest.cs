using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;

namespace gearit.src.utility
{


    class LuaTest
    {
        private MyGame _game;
        public Lua lua;
        public Lua _luaInterpret;

        public void saySomething(string something)
        {
            Console.WriteLine(something);
        }

        public LuaTest(MyGame game)
        {
            _game = game;
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

            _luaInterpret = new Lua();
            _luaInterpret.RegisterFunction("ChangeWheel", _game, _game.GetType().GetMethod("ChangeWheel"));
            /*_luaInterpret.RegisterFunction("getKeyboardState", _game, _game.GetType().GetMethod("getKeyboardState"));*/
            _luaInterpret.RegisterFunction("getKeysAction", _game, _game.GetType().GetMethod("getKeysAction"));
            _luaInterpret.RegisterFunction("getWheel", _game, _game.GetType().GetMethod("getWheel"));
        }

        public void execFile()
        {
            _luaInterpret.DoFile(@"scripts/MyRobot.lua");
        }
    }
}
