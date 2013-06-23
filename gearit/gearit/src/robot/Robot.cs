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
using gearit.src.editor;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using gearit.src.robot.api;

namespace gearit
{
    [Serializable()]
    class Robot : ISerializable
    {

        private List<Piece> _pieces;
        private List<ISpot> _spots;
        private int _prismaticCounter = 0;
        private int _revoluteCounter = 0;

        [NonSerialized]
        public static int _robotIdCounter = 1;
        private int _id;
        private World _world;

        public Robot(World world)
        {
            _world = world;
            _id = _robotIdCounter++;
            _pieces = new List<Piece>();
            _spots = new List<ISpot>();
            new Heart(this);
            Console.WriteLine("Robot created.");
        }

        public Robot(SerializationInfo info, StreamingContext ctxt)
        {
            SerializerHelper._currentRobot = this;
            _world = SerializerHelper._world;
            this._pieces = (List<Piece>)info.GetValue("Pieces", typeof(List<Piece>));
            //foreach (Piece p in _pieces)
            //SerializerHelper._world.AddBody((Body)p);

            this._spots = (List<ISpot>)info.GetValue("Spots", typeof(List<ISpot>));
            // foreach (ISpot s in _spots)
            //     SerializerHelper._world.AddJoint((Joint)s);

            _id = _robotIdCounter++;
            Console.WriteLine("Robot created.");
            SerializerHelper._currentRobot = null;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Pieces", _pieces, typeof(List<Piece>));
            info.AddValue("Spots", _spots, typeof(List<ISpot>));
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

        public Heart getHeart()
        {
            return ((Heart)_pieces.First());
        }

        public void setColor(Color col)
        {
            foreach (Piece p in _pieces)
                p.ColorValue = col;
        }

        public int getId()
        {
            return (_id);
        }

        public Piece getPiece(Vector2 p)
        {
            for (int i = _pieces.Count - 1; i > 0; i--)
                if (_pieces[i].Shown && _pieces[i].isOn(p))
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
                res += _spots[i].MaxForce;
            return (res);
        }

        public void drawDebug(DrawGame game)
        {
            for (int i = 0; i < _pieces.Count; i++)
                if (_pieces[i].Shown)
                    game.draw(_pieces[i], _pieces[i].ColorValue);
            for (int i = 0; i < _spots.Count; i++)
                _spots[i].draw(game);
        }

        public void draw(DrawGame dg)
        {
            for (int i = 0; i < _pieces.Count; i++)
                _pieces[i].draw(dg);
            for (int i = 0; i < _spots.Count; i++)
                if (_spots[i].GetType() == typeof(PrismaticSpot))
                    _spots[i].draw(dg);
        }

        public void showAll()
        {
            for (int i = 0; i < _pieces.Count; i++)
                _pieces[i].Shown = true;
        }

        public void remove(Piece p)
        {
            if (p == getHeart())
                return;
            for (JointEdge i = p.JointList; i != null; i = i.Next)
                _spots.Remove((ISpot)i.Joint);
            _pieces.Remove(p);
            _world.RemoveBody(p);
        }

        public void remove(ISpot s)
        {
            _spots.Remove(s);
            _world.RemoveJoint((Joint)s);
        }

        public void wake()
        {
            for (int i = 0; i < _pieces.Count; i++)
                _world.RemoveBody(_pieces[i]);
            for (int i = 0; i < _spots.Count; i++)
                _world.RemoveJoint((Joint)_spots[i]);
        }

        public void sleep()
        {
            for (int i = 0; i < _pieces.Count; i++)
                _world.AddBody(_pieces[i]);
            for (int i = 0; i < _spots.Count; i++)
                _world.AddJoint((Joint)_spots[i]);
        }

        public List<Api> getApi()
        {
            List<Api> res = new List<Api>();
            for (int i = 0; i < _spots.Count; i++)
            {
                if (_spots[i].GetType() == typeof(PrismaticSpot))
                    res.Add(new PrismaticApi(_spots[i]));
                else
                    res.Add(new RevoluteApi(_spots[i]));
            }
            return (res);
        }

        public int revCount() { return (_revoluteCounter++); }
        public int prisCount() { return (_prismaticCounter++); }

        public string Name
        {
            get;
            set;
        }
    }
}