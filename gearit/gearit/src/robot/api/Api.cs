using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gearit.src.robot.api
{
    abstract class Api
    {
        internal ISpot _spot;

        public Api(ISpot spot)
        {
            _spot = spot;
        }

        public string name()
        {
            return (_spot.Name);
        }

        public float motorForce 
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
