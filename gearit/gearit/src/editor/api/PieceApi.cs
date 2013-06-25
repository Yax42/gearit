using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gearit.src.editor.api
{
    abstract class PieceApi
    {
        internal ISpot _spot;

        public PieceApi(ISpot spot)
        {
            _spot = spot;
        }

        public string name()
        {
            return (_spot.Name);
        }

        public float motor 
        {
            get { return _spot.Force; }
            set
            {
                if (value > 1)
                    value = 1;
                else if (value < -1)
                    value = -1;
                _spot.Force = value * _spot.MaxForce;
            }
        }
    }
}
