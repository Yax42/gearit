using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;

namespace gearit
{
    class Robot
    {
        private static int _robotIdCounter = 1;
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

        public Piece getHeart()
        {
            return (_pieces.First());
        }

        public int getId()
        {
            return (_id);
        }

        public Piece getPiece(Vector2 p)
        {
            for (int i = 1; i < _pieces.Count; i++)
                if (_pieces[i].isOn(p))
                    return (_pieces[i]);
            return (getHeart());
        }

        public void draw()
        {
            for (int i = 1; i < _pieces.Count; i++)
              _pieces[i].draw();
            for (int i = 1; i < _spots.Count; i++)
              _spots[i].draw();
        }
    }
}
