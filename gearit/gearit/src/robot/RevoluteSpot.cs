﻿using System;
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
	public class RevoluteSpot : RevoluteJoint, ISpot, ISerializable
	{
		private static float _spotSize = 0.05f;
		private static Vector2 _topLeft = new Vector2(-_spotSize, -_spotSize);
		private static Vector2 _topRight = new Vector2(_spotSize, -_spotSize);
		private static Vector2 _botLeft = new Vector2(-_spotSize, _spotSize);
		private static Vector2 _botRight = new Vector2(_spotSize, _spotSize);

		 //private AngleJoint _angleJoint;
		static private Texture2D _tex;
		private Joint _joint;
		private CommonSpot _common;
		public Joint Joint { get { return _joint; } }

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
			SynchroniseAnchors(p1, robot);
			_joint = (Joint)this;
			_common = new CommonSpot(this);
			if (p1.GetType() == typeof(Rod))
				LocalAnchorA.Y = 0;

		}

		public void wakeUp(Robot robot)
		{
			_common.wakeUp(robot);
		}

		public void fallAsleep(Robot robot, Piece p)
		{
			_common.fallAsleep(robot, p);
		}

		#region Serialization
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

			MaxMotorTorque = (float)info.GetValue("MaxForce", typeof(float));	
			MaxAngle = (float)info.GetValue("MaxAngle", typeof(float));	
			MinAngle = (float)info.GetValue("MinAngle", typeof(float));	
			SpotLimitEnabled = (bool)info.GetValue("LimitEnabled", typeof(bool));	

			MotorSpeed = 0f;
			MotorEnabled = true;
			LimitEnabled = false;
			ColorValue = Color.Black;
			_joint = (Joint)this;
			_common = new CommonSpot(this);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("Name", Name, typeof(string));
			info.AddValue("MaxForce", MaxMotorTorque, typeof(float));
			info.AddValue("PAHashCode", BodyA.GetHashCode(), typeof(int));
			info.AddValue("PBHashCode", BodyB.GetHashCode(), typeof(int));
			info.AddValue("AnchorA", LocalAnchorA, typeof(Vector2));
			info.AddValue("AnchorB", LocalAnchorB, typeof(Vector2));

			info.AddValue("MaxAngle", MaxAngle, typeof(float));
			info.AddValue("MinAngle", MinAngle, typeof(float));
			info.AddValue("LimitEnabled", SpotLimitEnabled, typeof(bool));
		}
		#endregion

		#region MotorControl

		private float _MaxAngle;
		public float MaxAngle
		{
			get
			{
				return _MaxAngle;
			}
			set
			{
				if (value < 0)
					return;
				_MaxAngle = value;
				if (!Frozen)
					UpperLimit = _MaxAngle;
			}
		}

		private float _MinAngle;
		public float MinAngle
		{
			get
			{
				return _MinAngle;
			}
			set
			{
				if (value > 0)
					return;
				_MinAngle = value;
				if (!Frozen)
					LowerLimit = _MinAngle;
			}
		}

		private bool _Frozen;
		public bool Frozen
		{
			get
			{
				return _Frozen;
			}
			set
			{
				if (_Frozen == value)
					return;
				_Frozen = value;
				if (_Frozen)
				{
					LowerLimit = JointAngle;
					UpperLimit = JointAngle;
					base.LimitEnabled = true;
					MotorEnabled = false;
				}
				else
				{
					LowerLimit = MaxAngle;
					UpperLimit = MinAngle;
					base.LimitEnabled = _LimitEnabled;
					MotorEnabled = true;
				}
			}
		}

		private bool _LimitEnabled;
		public bool SpotLimitEnabled
		{
			get
			{
				return _LimitEnabled;
			}

			set
			{
				_LimitEnabled = value;
				if (!Frozen)
					base.LimitEnabled = _LimitEnabled;
			}
		}

		public float Force
		{
			get { return MotorSpeed / 15; }
			set { MotorSpeed = value * 15; }
		}

		public float MaxForce
		{
			get { return MaxMotorTorque; }
			set { MaxMotorTorque = value; }
		}
		#endregion

		#region Editor
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

		public void moveAnchor(Piece p, Vector2 anchor, Robot robot = null)
		{
			if (robot != null)
				robot.ResetAct();
			if (BodyA == p)
				LocalAnchorA = BodyA.GetLocalPoint(anchor);
			if (BodyB == p)
				LocalAnchorB = BodyB.GetLocalPoint(anchor);
			SynchroniseAnchors(p);
		}

		public Vector2 ActualWorldAnchorA
		{
			get { return BodyA.GetWorldVector(LocalAnchorA); }
		}

		public Vector2 ActualWorldAnchorB
		{
			get { return BodyB.GetWorldVector(LocalAnchorB); }
		}

		public Vector2 DeltaAnchor
		{
			get { return ActualWorldAnchorA - ActualWorldAnchorB; }
		}

		public void SynchroniseAnchors(Piece piece, Robot robot = null)
		{
			if (robot != null)
				robot.ResetAct();
			if (piece == BodyA)
				((Piece)BodyB).move(WorldAnchorA - ActualWorldAnchorB);
			else
				((Piece)BodyA).move(WorldAnchorB - ActualWorldAnchorA);
		}

		public void rotate(Piece piece, float angle, Robot robot = null)
		{
			if (robot != null)
				robot.ResetAct();
			if (piece == BodyA)
				((Piece)BodyB).rotateDelta(angle);
			else
				((Piece)BodyA).rotateDelta(angle);
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
		#endregion

		#region Draw
		public void drawDebug(DrawGame game)
		{
			_drawDebug(game, WorldAnchorA);
			_drawDebug(game, WorldAnchorB);
		}

		private void _drawDebug(DrawGame game, Vector2 pos)
		{
			bool isVisible = ((Piece)BodyA).Shown || ((Piece)BodyB).Shown;
			Vector2 corner = pos - _topLeft;
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
					game.drawLine(pos, pos + target,
						new Color(new Vector4(ColorValue.ToVector3(), isVisible ? 1f : 0.16f)));
			}
		}
		#endregion

		public string Name { get; set; }
		public Color ColorValue { get; set; }
	}
}
