﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using System.Runtime.Serialization;

namespace gearit.src.editor.map
{
    [Serializable()]
    abstract class MapChunk : Body, ISerializable
    {
        public MapChunk(World world, bool isDynamic, Vector2 pos)
            : base(world)
        {
            Position = pos;
            if (isDynamic)
                BodyType = BodyType.Dynamic;
            Friction = 10;
        }

        internal MapChunk(World world)
            : base(world)
        {
            Friction = 100;
        }

        public bool isOn(Vector2 p)
        {
            Transform t;
            GetTransform(out t);
            return (FixtureList[0].Shape.TestPoint(ref t, ref p));
        }

        abstract public void GetObjectData(SerializationInfo info, StreamingContext ctxt);
        abstract public void resize(Vector2 p);
    }
}