﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using gearit.src.robot;

namespace gearit
{
    class Robot
    {
        private static int _robotIdCounter = 1;
        private VertexPositionColor[] _lineVertices = new VertexPositionColor[5000];
        private int _id;
        private List<Piece> _pieces;
        private List<ISpot> _spots;
        private World _world;
        private GraphicsDevice _graph;
        private AssetCreator _asset;

        public Robot(World world, GraphicsDevice graph)
        {
            _graph = graph;
            _asset = new AssetCreator(_graph);
            _world = world;
            _id = _robotIdCounter++;
            Console.WriteLine("Robot created.");
	    _pieces = new List<Piece>();
	    _spots = new List<ISpot>();
            new Heart(this);
            //x_heart = new Heart();
        }

        public GraphicsDevice getGraphic()
        {
            return (_graph);
        }

        public AssetCreator getAsset()
        {
            return (_asset);
        }

        public void addSpot(ISpot spot)
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

        public void drawDebug(SpriteBatch batch)
        {
	    int	    count = 0;

            for (int i = 0; i < _pieces.Count; i++)
                _pieces[i].vertices(_lineVertices, ref count);
            for (int i = 0; i < _spots.Count; i++)
	      _spots[i].vertices(_lineVertices, ref count);
            _graph.DrawUserPrimitives(PrimitiveType.LineList, _lineVertices, 0, count);
        }

        public void draw(SpriteBatch batch)
        {
	    int	    count = 0;

            for (int i = 0; i < _pieces.Count; i++)
                if (_spots[i].GetType() == typeof(PrismaticSpot))
                  _pieces[i].vertices(_lineVertices, ref count);
            for (int i = 0; i < _spots.Count; i++)
	      _spots[i].vertices(_lineVertices, ref count);
            _graph.DrawUserPrimitives(PrimitiveType.LineList, _lineVertices, 0, count);
            for (int i = 1; i < _pieces.Count; i++)
                _pieces[i].draw(batch);
            _pieces[0].draw(batch);
        }
    }
}
