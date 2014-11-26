using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace gearit.src.map
{
	interface IVirtualItem
	{
		Vector2 Position { get; set; }
		int Id { get; set; }
	}
}
