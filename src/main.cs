using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Factories;

namespace gearit
{
  class main
  {
    public static void Main()
    {
        World myWorld = new World(new Vector2(0, 9.82f));
        //BodyFactory.CreateRectangle(myWorld, 50f, 50f, 50f);
        Heart myHeart = new Heart(myWorld);
        //PolygonShape polygon = new PolygonShape(vertices, 1);
        //Heart myHeart = new Heart(myWorld, polygon);
        //Robot WallE = new Robot(myWorld, polygon);
    }
  }
}