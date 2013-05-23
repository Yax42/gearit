using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace gearit
{
  class main
  {

    public static void Main()
    {
        World world = new World(new Vector2(0, 9.82f));

        Robot WallE = new Robot();
    }
  }
}