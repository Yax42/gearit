using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using gearit.src.test;

namespace gearit
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            World world = new World(new Vector2(0, 9.82f));

            Robot wallE = new Robot(world);

            // Test robot
            //BruteRobot game = new BruteRobot();
            //game.Run();

            // Test lua
            LuaTest l = new LuaTest();
            SpiderBot game = new SpiderBot();
            game.Run();
        }
    }
#endif
}

