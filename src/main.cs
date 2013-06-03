using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace gearit
{
  class main
  {

    public static void Main()
    {
        World		world = new World(new Vector2(0, 9.82f));

        Robot		wallE = new Robot(world);
	Piece		arm1 = new Rod(world, wallE.getHeart(), new Vector2(0, 0), 30);
	Piece		arm2 = new Rod(world, arm2, new Vector2(0, 0), 30);
	wallE.addPiece(arm1);
	wallE.addPiece(arm2);
    }
  }
}