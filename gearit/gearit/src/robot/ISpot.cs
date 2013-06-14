using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.robot;

namespace gearit
{
    interface ISpot : IPrintableVertices
    {
        void swap(Piece p1, Piece p2, Vector2 anchor);

        void swap(Piece p1, Piece p2);

        void moveAnchor(Piece p, Vector2 anchor);

        float getSize();
    }
}