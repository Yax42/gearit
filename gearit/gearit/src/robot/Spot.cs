using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace gearit
{
    interface Spot
    {
        void swap(Piece p1, Piece p2, Vector2 anchor);

        void swap(Piece p1, Piece p2);

        void move(Vector2 pos);

        void draw();
    }
}