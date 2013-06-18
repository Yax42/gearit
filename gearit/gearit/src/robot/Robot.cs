using System;
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
using FarseerPhysics.Dynamics.Joints;
using gearit.src;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace gearit
{
    [Serializable()]
    class Robot : ISerializable
    {

        private List<Piece> _pieces;
        private List<ISpot> _spots;

	[NonSerialized]
        private static int _robotIdCounter = 1;
        private int _id;
        private World _world;

        public Robot(World world)
        {
            _world = world;
            _id = _robotIdCounter++;
            Console.WriteLine("Robot created.");
	    _pieces = new List<Piece>();
	    _spots = new List<ISpot>();
            addPiece(new Heart(this));
            //x_heart = new Heart();

        }

	public Robot(SerializationInfo info, StreamingContext ctxt)
	{
	  this._pieces = (List<Piece>) info.GetValue("Pieces", typeof(List<Piece>));
	  this._spots = (List<ISpot>) info.GetValue("Spots", typeof(List<ISpot>));
	}

	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
	  info.AddValue("Pieces", typeof(List<Piece>));
	  info.AddValue("Spots", typeof(List<ISpot>));
	}

	/*
        public GraphicsDevice getDevice()
        {
            return (_device);
        }

        public AssetCreator getAsset()
        {
            return (_asset);
        }*/

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
            //getHeart().Position = p;
            for (int i = 1; i < _pieces.Count; i++)
                if (_pieces[i].isOn(p))
                    return (_pieces[i]);
            return (getHeart());
        }

        public float getWeight()
        {
	    float res = 0;
            for (int i = 0; i < _pieces.Count; i++)
              res += _pieces[i].Weight;
            return (res);
        }

        public float getMotorStrength()
        {
	    float res = 0;
            for (int i = 0; i < _spots.Count; i++)
              res += _spots[i].MotorStrength;
            return (res);
        }

        public void drawDebug(DrawGame game)
        {
            for (int i = 0; i < _pieces.Count; i++)
                _pieces[i].drawLines(game);
            for (int i = 0; i < _spots.Count; i++)
                _spots[i].draw(game);
        }

        public void draw(SpriteBatch batch)
        {
	    /*
	    int	    count = 0;

            for (int i = 0; i < _pieces.Count; i++)
               _pieces[i].vertices(_lineVertices, ref count);
            for (int i = 0; i < _spots.Count; i++)
               if (_spots[i].GetType() == typeof(PrismaticSpot))
	         _spots[i].vertices(_lineVertices, ref count);
            _device.DrawUserPrimitives(PrimitiveType.LineList, _lineVertices, 0, count);
            for (int i = 1; i < _pieces.Count; i++)
                _pieces[i].draw(batch);
            _pieces[0].draw(batch);
	    */
        }

        public void remove(Piece p)
	{
            if (p == getHeart())
              return ;
            for (JointEdge i = p.JointList; i != null; i = i.Next)
              _spots.Remove((ISpot) i.Joint);
            _pieces.Remove(p);
            _world.RemoveBody(p);
	}

        public void remove(ISpot s)
        {
            _spots.Remove(s);
            _world.RemoveJoint((Joint) s);
        }
    }
}
