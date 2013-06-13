namespace FarseerPhysics.HelloWorld
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
        }
    }
#endif
}

