using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;

namespace gearit
{
    class Robot
    {
        private static int _robotIdCounter = 0;
        private Heart _heart;
        private int _id;
        private List<Piece> _pieces;
        private List<Spot> _spots;
        private World _world;

        public Robot(World world)
        {
            _world = world;
            _id = _robotIdCounter++;
            Console.WriteLine("new robot.");
            _heart = new Heart(world);
	    _pieces = new List<Piece>();
	    _spots = new List<Spot>();
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

        public Heart getHeart()
        {
            return (_heart);
        }
    }
}
