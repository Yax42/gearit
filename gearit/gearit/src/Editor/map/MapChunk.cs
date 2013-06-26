using System;
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
    enum ChunkType
    {
        RECTANGLE = 0,
        CIRCLE
    }

    [Serializable()]
    class MapChunk : Body, ISerializable
    {
        private ChunkType _type;

        public MapChunk(ChunkType t, Boolean phys, Vector2 pos, World world)
            : base(world, pos)
        {
            _type = t;
            this.Position = pos;
            if (phys)
                this.BodyType = BodyType.Dynamic;
            switch (t)
            {
                case ChunkType.RECTANGLE:
                    Vertices rectangleVertices = PolygonTools.CreateRectangle(8f / 2, 0.5f / 2);
                    PolygonShape rectangleShape = new PolygonShape(rectangleVertices, 1f);
                    this.CreateFixture(rectangleShape);
                    break;
                case ChunkType.CIRCLE:
                    FixtureFactory.AttachCircle(0.5f, 1f, this);
                    break;
            }
        }

        public MapChunk(SerializationInfo info, StreamingContext ctxt)
            : base(SerializerHelper.World, (Vector2)info.GetValue("Position", typeof(Vector2)))
        {
            _type = (ChunkType)info.GetValue("Type", typeof(ChunkType));
            BodyType = (BodyType)info.GetValue("BodyType", typeof(BodyType));
            Rotation = (float)info.GetValue("Rotation", typeof(float));
            switch (_type)
            {
                case ChunkType.RECTANGLE:
                    SerializedFixture.convertSFixture((SerializedFixture)info.GetValue("Fixture", typeof(SerializedFixture)), this);
                    break;
                case ChunkType.CIRCLE:
                    FixtureFactory.AttachCircle(0.5f, 1f, this);
                    break;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            List<SeralizedBody> bodyList = new List<SeralizedBody>();
            foreach (Body b in _mapChunk)
                bodyList.Add(SeralizedBody.convertBody(b));

            info.AddValue("Name", _mapName, typeof(string));
            info.AddValue("Bodies", bodyList, typeof(List<SeralizedBody>));
        }
    }
}
