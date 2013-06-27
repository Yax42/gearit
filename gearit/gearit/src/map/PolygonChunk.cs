using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.editor.map;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using System.Runtime.Serialization;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using gearit.src.editor;

namespace gearit.src.map
{
    [Serializable()]
    class PolygonChunk : MapChunk, ISerializable
    {
        public PolygonChunk(World world, bool isDynamic, Vector2 pos)
            : base(world, isDynamic, pos)
        {
            Vertices rectangleVertices = PolygonTools.CreateRectangle(8f / 2, 0.5f / 2);
            PolygonShape rectangleShape = new PolygonShape(rectangleVertices, 1f);
            this.CreateFixture(rectangleShape);
        }

        public PolygonChunk(SerializationInfo info, StreamingContext ctxt)
            : base(SerializerHelper.World)
        {
            SerializedBody.convertSBody((SerializedBody)info.GetValue("SerializedBody", typeof(SerializedBody)), this);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("SerializedBody", SerializedBody.convertBody(this), typeof(SerializedBody));
        }
    }
}
