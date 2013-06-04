using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gearit.src.robot
{
    class Motor : IMotor
    {
      private int	_strength;
      private float	_state;
      private bool	_isFree;
      private float	_speed;

      Motor(int strength)
      {
          _strength = strength;
          _state = 0;
          _speed = 0;
          _isFree = false;
      }
      public void runBasics()
      {
          _speed += _state * _strength;
      }

      public float	State
       {
            get { return _state; }

            set
            {
                if (value > 1)
                    _state = 1;
                else if (value < -1)
                    _state = -1;
                else
                    _state = value;
            }
	}

	public bool	    IsFree
	{
	  get { return _isFree; }
	  set { _isFree = value; }
	}
    }
}
