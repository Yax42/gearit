using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using gearit.src.utility;

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
            using (GearIt game = new GearIt())
            {
                game.Run();
            }
            //World world = new World(new Vector2(0, 9.82f));

            // Test robot
            //BruteRobot game = new BruteRobot();
            //game.Run();

            // Test lua
            //BruteRobot game = new BruteRobot();
            
            //SpiderBot game = new SpiderBot();
            //game.Run();
            //LuaTest l = new LuaTest();
        }
    }
#endif
}

