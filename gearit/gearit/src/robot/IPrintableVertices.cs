using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gearit.src.robot
{
    interface IPrintableVertices
    {
        void vertices(VertexPositionColor[] vertices, ref int count);

	Color ColorValue { get; set; }
    }
}
