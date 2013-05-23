using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gearit
{
  class Robot
  {
    private static int	_robotIdCounter = 0;
    private Heart       _heart;
    private int		_id;
    private List<Rod>	_rods;
    private List<Wheel>	_wheels;

    public Robot()
    {
        _id = _robotIdCounter++;
        Console.WriteLine("new robot.");
        _heart = new Heart();
    }
  }
}
