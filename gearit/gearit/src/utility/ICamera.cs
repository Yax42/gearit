using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace gearit.src.utility
{
	public interface ICamera
	{
		Matrix view();
		Matrix projection();
        Matrix GetScrollMatrix(Vector2 textureSize);

	/*
		Vector2 Position { get; set; }
		float Zoom { get; set; }
		float Rotation { get; set; }
		Vector2 center();
	*/
	}
}
