﻿using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using gearit.src;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace gearit
{
    class Wheel : Piece, ISerializable
    {
        private const int _circleSegments = 32;

        private float _size;

        public Wheel(Robot robot, float size) :
	    this(robot, size, Vector2.Zero)
        {
        }
        public Wheel(Robot robot, float size, Vector2 pos) :
            base(robot, new CircleShape(size, 1f)) //density 0 ~= poids
        {
            Position = pos;
            _size = size;
            //_tex = robot.getAsset().TextureFromShape(_shape, MaterialType.Blank, Color.White, 1f);
            Console.WriteLine("Wheel created.");
        }

	public Wheel(SerializationInfo info, StreamingContext ctxt) :
	  base()
	{
	  _size = (float) info.GetValue("Size", typeof(float));
	  // Position = (Vector2) info.GetValue("Position", typeof(Vector2));
	  _shape = (PolygonShape) info.GetValue("Shape", typeof(PolygonShape));
	  _fix = (Fixture) info.GetValue("Fixture", typeof(Fixture));
	}

	public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
	{
	  info.AddValue("Size", _size);
	  // info.AddValue("Position", Position);
	  info.AddValue("Shape", _shape);
	  info.AddValue("Fixture", _fix);
	}

        public override void draw(SpriteBatch batch)
        {
            batch.Draw(_tex, ConvertUnits.ToDisplayUnits(this.Position), null, Color.LightGreen, this.Rotation,
                       new Vector2(_tex.Width / 2f, _tex.Height / 2f), 1f, SpriteEffects.None, 0f);
        }

        public override void drawLines(DrawGame game)
        {
            const double increment = Math.PI * 2.0 / _circleSegments;
            double theta = 0.0;

            for (int i = 0; i < _circleSegments; i++)
            {
                Vector2 v1 = Position + _size * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                Vector2 v2 = Position + _size *
                             new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));

                game.addLine(v1, v2, ColorValue);
                theta += increment;
            }
        }

        public float Size
        {
            get { return _size; }
            set
            {
                ((CircleShape)_shape).Radius = value;
		if (areSpotsOk() == false)
                  ((CircleShape)_shape).Radius = _size;
		else
                  _size = value;
            }
        }
    }
}
