using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;

namespace gearit.src.editor
{
    /// <summary>
    /// Helper class for the serialization of Gear It objects.
    /// </summary>
    static class SerializerHelper
    {
        public static World World = null;
        public static Robot CurrentRobot = null;
        public static Dictionary<int, Piece> Ptrmap = null;

        public static void Add(int code, Piece p)
        {
            if (Ptrmap == null)
                Ptrmap = new Dictionary<int, Piece>();
            if (!Ptrmap.ContainsKey(code))
                Ptrmap.Add(code, p);
            else
                Ptrmap[code] = p;
        }
    }

    /// <summary>
    /// Structure holding the necessary information to recreate a Fixture.
    /// </summary>
    [Serializable()]
    public struct SerializedFixture
    {
        private float _density;
        private float _friction;
        private List<Vector2> _vertices;

        /// <summary>
        /// Converts a fixture to a SerializedFixture.
        /// </summary>
        /// <param name="f">The fixture to convert.</param>
        /// <returns>The converted fixture.</returns>
        public static SerializedFixture convertFixture(Fixture f)
        {
            SerializedFixture sf = new SerializedFixture();

            sf._density = f.Shape.Density;
            sf._friction = f.Friction;
            sf._vertices = new List<Vector2>();
            foreach (Vector2 vert in ((PolygonShape)f.Shape).Vertices)
                sf._vertices.Add(vert);
            return (sf);
        }

        /// <summary>
        /// Converts a SerializedFixture to a Fixture.
        /// </summary>
        /// <param name="sf">The SerializedFixture to convert.</param>
        /// <param name="b">The necessary body to create the fixture.</param>
        /// <returns>The converted SerializedFixture.</returns>
        public static Fixture convertSFixture(SerializedFixture sf, Body b)
        {
            Vertices v = new Vertices(sf._vertices);
            Fixture f = b.CreateFixture(new PolygonShape(v, sf._density));
            f.Friction = sf._friction;
            return (f);
        }
    }

    /// <summary>
    /// Structure holding the necessary information to recreate a Body.
    /// </summary>
    [Serializable()]
    public struct SeralizedBody
    {
        private Vector2 _position;
        private float _rotation;
        private List<SerializedFixture> _fixtures;

        /// <summary>
        /// Converts a Body to a SeralizedBody.
        /// </summary>
        /// <param name="body">The body to convert.</param>
        /// <returns>The converted body.</returns>
        public static SeralizedBody convertBody(Body body)
        {
            SeralizedBody sb = new SeralizedBody();

            sb._position = body.Position;
            sb._rotation = body.Rotation;
            sb._fixtures = new List<SerializedFixture>(body.FixtureList.Count());
            foreach (Fixture f in body.FixtureList)
                sb._fixtures.Add(SerializedFixture.convertFixture(f));
            return (sb);
        }
        /// <summary>
        /// Converts a SeralizedBody to a Body.
        /// </summary>
        /// <param name="body">The SeralizedBody to convert.</param>
        /// <returns>The converted SeralizedBody.</returns>
        public static Body convertSBody(SeralizedBody sbody)
        {
            Body b = new Body(SerializerHelper.World);

            b.Position = sbody._position;
            b.Rotation = sbody._rotation;
            foreach (SerializedFixture sf in sbody._fixtures)
                SerializedFixture.convertSFixture(sf, b);
            //b.Friction = 100;
            return (b);
        }
    }
}
