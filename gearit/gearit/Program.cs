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
            BruteRobot game = new BruteRobot();
            game.Run();
        }
    }
#endif
}

