using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;

namespace gearit.src.test
{


    class LuaTest
    {

        public Lua lua;

        public void saySomething(string something)
        {
            Console.WriteLine(something);
        }

        public LuaTest()
        {
            // Creating interpreter
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
            lua.DoFile("test.lua");
        }
    }
}
