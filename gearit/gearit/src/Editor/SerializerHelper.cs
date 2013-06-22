using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;

namespace gearit.src.editor
{
    /// <summary>
    /// Helper class for the serialization of Gear It objects.
    /// </summary>
    static class SerializerHelper
    {
        public static World _world = null;
        public static Robot _currentRobot = null;
        public static Dictionary<int, Piece> _ptrmap = null;
    }

    /// <summary>
    /// Structure holding the necessary information to recreate a Body.
    /// </summary>
    public struct SeralizedBody
    {
        private BodyType _bodytype;

        /// <summary>
        /// Converts a Body to a SeralizedBody.
        /// </summary>
        /// <param name="body">The body to convert.</param>
        /// <returns>The converted body.</returns>
        public static SeralizedBody convertBody(Body body)
        {
            SeralizedBody sb = new SeralizedBody();

            sb._bodytype = body.BodyType;
            return (sb);
        }

        /// <summary>
        /// Converts a SeralizedBody to a Body.
        /// </summary>
        /// <param name="body">The SeralizedBody to convert.</param>
        /// <returns>The converted SeralizedBody.</returns>
        public static Body convertSBody(SeralizedBody sbody)
        {
            Body b = new Body(SerializerHelper._world);

            b.BodyType = sbody._bodytype;
            return (b);
        }
    }
}