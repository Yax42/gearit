using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;

namespace gearit
{
    class Robot
    {
        private static int _robotIdCounter = 1;
        private Heart _heart;
        private int _id;
        private List<Piece> _pieces;
        private List<Spot> _spots;
        private World _world;

        public Robot(World world)
        {
            _world = world;
            _id = _robotIdCounter++;
            Console.WriteLine("Robot created.");
	    _pieces = new List<Piece>();
	    _spots = new List<Spot>();
            new Heart(this);
            //x_heart = new Heart();
        }

        public void addSpot(Spot spot)
        {
            _spots.Add(spot);
        }

        public void addPiece(Piece piece)
        {
            _pieces.Add(piece);
        }

        public World getWorld()
        {
            return (_world);
        }

        public Heart getHeart()
        {
            return (_heart);
        }

        public int getId()
        {
            return (_id);
        }
    }
}
