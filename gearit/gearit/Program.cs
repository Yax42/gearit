using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

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
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}

