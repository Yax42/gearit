using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.utility;
using gearit.src.editor;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;

namespace gearit.src.robot
{
	[Serializable()]
	class RevoluteSpot : RevoluteJoint, ISpot, ISerializable
	{
		private static float _spotSize = 0.05f;
		private static Vector2 _topLeft = new Vector2(-_spotSize, -_spotSize);
		private static Vector2 _topRight = new Vector2(_spotSize, -_spotSize);
		private static Vector2 _botLeft = new Vector2(-_spotSize, _spotSize);
		private static Vector2 _botRight = new Vector2(_spotSize, _spotSize);

		 //private AngleJoint _angleJoint;
		static private Texture2D _tex;

		public RevoluteSpot(Robot robot, Piece p1, Piece p2) :
			this(robot, p1, p2, Vector2.Zero, Vector2.Zero)
		{
		}

		public RevoluteSpot(Robot robot, Piece p1, Piece p2, Vector2 anchor1, Vector2 anchor2) :
			base(p1, p2, anchor1, anchor2)
		{
			Name = "spot" + robot.revCount();
			robot.getWorld().AddJoint(this);
			Enabled = true;
			MaxMotorTorque = 10;
			MotorSpeed = 0f;
			MotorEnabled = true;
			ColorValue = Color.Black;
			robot.addSpot(this);
			move(p1, p1.Position);
		}

		public void BackIntoWorld(Robot robot)
		{
			robot.getWorld().AddJoint(this);
			robot.addSpot(this);
		}

		//
		// SERIALISATION
		//
		public RevoluteSpot(SerializationInfo info, StreamingContext ctxt) :
			base(
	SerializerHelper.Ptrmap[(int)info.GetValue("PAHashCode", typeof(int))],
		SerializerHelper.Ptrmap[(int)info.GetValue("PBHashCode", typeof(int))],
		(Vector2)info.GetValue("AnchorA", typeof(Vector2)),
		(Vector2)info.GetValue("AnchorB", typeof(Vector2)))
		{
			Name = (string)info.GetValue("Name", typeof(string));
			SerializerHelper.World.AddJoint(this);
			Enabled = true;
			MaxMotorTorque = (float)info.GetValue("Force", typeof(float));
		LimitEnabled = (bool)info.GetValue("LimitEnabled", typeof(bool));
		UpperLimit = (float)info.GetValue("UpperLimit", typeof(float));
		LowerLimit = (float)info.GetValue("LowerLimit", typeof(float));
		/*
		if (UpperLimit < LowerLimit)
		{
			float tmp = UpperLimit;
			UpperLimit = LowerLimit;
			LowerLimit = tmp;

		}
		Console.WriteLine((int) MathHelper.ToDegrees(LowerLimit) + " " + (int) MathHelper.ToDegrees(UpperLimit));
		//Console.WriteLine((LowerLimit) + " " + (UpperLimit));
		*/
			MotorSpeed = 0f;
			MotorEnabled = true;
			ColorValue = Color.Black;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("Name", Name, typeof(string));
			info.AddValue("Force", MaxMotorTorque, typeof(float));
			info.AddValue("PAHashCode", BodyA.GetHashCode(), typeof(int));
			info.AddValue("PBHashCode", BodyB.GetHashCode(), typeof(int));
			info.AddValue("AnchorA", LocalAnchorA, typeof(Vector2));
			info.AddValue("AnchorB", LocalAnchorB, typeof(Vector2));
			info.AddValue("LimitEnabled", LimitEnabled, typeof(bool));
			info.AddValue("UpperLimit", UpperLimit, typeof(float));
			info.AddValue("LowerLimit", LowerLimit, typeof(float));
		}
		//--------- END SERIALISATION

		public static void initTex(AssetCreator asset)
		{
			_tex = asset.CreateCircle(2, Color.Red);
		}

		public void swap(Piece p1, Piece p2, Vector2 anchor)
		{
			if (BodyA == p1)
			{
				BodyA = p2;
				LocalAnchorA = anchor;
			}
			if (BodyB == p1)
			{
				BodyB = p2;
				LocalAnchorB = anchor;
			}
		}

		public void swap(Piece p1, Piece p2)
		{
			swap(p1, p2, Vector2.Zero);
		}

		public void moveAnchor(Piece p, Vector2 anchor)
		{
			if (BodyA == p)
				LocalAnchorA = anchor - p.Position;
			if (BodyB == p)
				LocalAnchorB = anchor - p.Position;
			move(p, p.Position);
		}

		/*
			public void move(Vector2 pos)
			{
				//BodyA.Position = pos - LocalAnchorA;
				//BodyB.Position = pos - LocalAnchorB;
				((Piece)BodyA).move(pos - LocalAnchorA);
				((Piece)BodyB).move(pos - LocalAnchorB);
			}
		*/

		public void move(Piece piece, Vector2 pos)
		{
			if (piece == BodyA)
				((Piece)BodyB).move(pos + LocalAnchorA - LocalAnchorB);
			else
				((Piece)BodyA).move(pos + LocalAnchorB - LocalAnchorA);
		}

		public void rotate(Piece piece, float angle)
		{
			
			if (piece == BodyA)
			{
				((Piece)BodyB).rotateDelta(angle);
				LocalAnchorB = MathLib.RotatePoint(LocalAnchorB, Vector2.Zero, MathLib.RadiansToDegrees(angle));
			}
			else
			{
				((Piece)BodyA).rotateDelta(angle);
				LocalAnchorA = MathLib.RotatePoint(LocalAnchorA, Vector2.Zero, MathLib.RadiansToDegrees(angle));
			}
		}

		public void draw(DrawGame game)
		{
			if (((Piece)BodyA).Shown == false || ((Piece)BodyB).Shown == false)
				return;
			Vector2 pos = WorldAnchorA;
			Vector2 corner = WorldAnchorA - _topLeft;
			//Vector2 corner2 = BodyA.Position + LocalAnchorA + _botRight;

			//game.Batch().Draw(_tex, new Rectangle((int)corner.X, (int)corner.Y, (int)_spotSize * 2, (int)_spotSize * 2), ColorValue);
			Color tmp = ColorValue;
			if (LimitEnabled)
				ColorValue = Color.Pink;
			game.drawLine(pos + _topLeft, pos + _topRight, ColorValue);
			game.drawLine(pos + _topRight, pos + _botRight, ColorValue);
			game.drawLine(pos + _botRight, pos + _botLeft, ColorValue);
			game.drawLine(pos + _botLeft, pos + _topLeft, ColorValue);
			ColorValue = tmp;

			if (LimitEnabled == false)
				return;
			Vector2 target;
			//float tmpUpper = UpperLimit/ +(LowerLimit > UpperLimit ? (float)Math.PI * 2 : 0);
			int count = 0;
			bool limitReached = false;
			for (float angle = LowerLimit; angle < UpperLimit; angle += 0.01f)
			{
				count++;
				target.X = (float)Math.Cos(angle) * 0.1f;
				target.Y = (float)Math.Sin(angle) * 0.1f;
				if (angle >= 0 && limitReached == false)
				{
					game.drawLine(pos, pos + target, new Color(0, 255, 0));
					limitReached = true;
				}
				else
					game.drawLine(pos, pos + target, new Color(255, 255 - count * 2, 255 - count * 2));
			}
		}

		public Vector2 getLocalAnchor(Piece piece)
		{
			if (BodyA == piece)
				return LocalAnchorA;
			else
				return LocalAnchorB;
		}

		public Vector2 getWorldAnchor(Piece piece)
		{
			if (BodyA == piece)
				return WorldAnchorA;
			else
				return WorldAnchorB;
		}

		public void moveLocal(Piece p, Vector2 pos)
		{
			if (BodyA == p)
				LocalAnchorA = pos;
			else if (BodyB == p)
				LocalAnchorB = pos;
		}

		public float Force
		{
			get { return MotorTorque; }
			set { MotorTorque = value; }
		}

		public float MaxForce
		{
			get { return MaxMotorTorque; }
			set { MaxMotorTorque = value; }
		}

		public string Name { get; set; }
		public Color ColorValue { get; set; }
	}
}
