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
	public class RevoluteSpot : RevoluteJoint, ISpot, ISerializable
	{
		private static float _spotSize = 0.05f;

		 //private AngleJoint _angleJoint;
		static private Texture2D _tex;
		private Joint _joint;
		private CommonSpot _common;
		private Robot Robot;
		public Joint Joint { get { return _joint; } }
		public float VirtualLimitBegin;

		public RevoluteSpot(Robot robot, Piece p1, Piece p2) :
			this(robot, p1, p2, Vector2.Zero, Vector2.Zero)
		{
		}

		public RevoluteSpot(Robot robot, Piece p1, Piece p2, Vector2 anchor1, Vector2 anchor2) :
			base(p1, p2, anchor1, anchor2)
		{
			Robot = robot;
			Name = "spot" + robot.FindFirstFreeSpotNameId();
			Robot.getWorld().AddJoint(this);
			Enabled = true;
			MaxMotorTorque = 10;
			MotorSpeed = 0f;
			MotorEnabled = true;
			ColorValue = Color.Black;
			Robot.addSpot(this);
			SynchroniseAnchors(p2, false, true);
			_joint = (Joint)this;
			_common = new CommonSpot(this);
			if (p1.GetType() == typeof(Rod))
				LocalAnchorA = new Vector2(LocalAnchorA.X, 0);

		}

		public void wakeUp()
		{
			_common.wakeUp(Robot);
		}

		public void fallAsleep(Piece p)
		{
			_common.fallAsleep(Robot, p);
		}

		#region Serialization
		public RevoluteSpot(SerializationInfo info, StreamingContext ctxt) :
			base(
			SerializerHelper.Ptrmap[(int)info.GetValue("PAHashCode", typeof(int))],
			SerializerHelper.Ptrmap[(int)info.GetValue("PBHashCode", typeof(int))],
			(Vector2)info.GetValue("AnchorA", typeof(Vector2)),
			(Vector2)info.GetValue("AnchorB", typeof(Vector2)))
		{
			Robot = SerializerHelper.CurrentRobot;
			Name = (string)info.GetValue("Name", typeof(string));
			SerializerHelper.World.AddJoint(this);
			Enabled = true;

			MotorEnabled = true;
			MaxMotorTorque = (float)info.GetValue("MaxForce", typeof(float));	
			MaxAngle = (float)info.GetValue("MaxAngle", typeof(float));	
			MinAngle = (float)info.GetValue("MinAngle", typeof(float));	
			SpotLimitEnabled = (bool)info.GetValue("LimitEnabled", typeof(bool));	
			VirtualLimitBegin = (float)info.GetValue("VirtualLimitBegin", typeof(float));	
			Frozen = (bool)info.GetValue("Frozen", typeof(bool));	

			MotorSpeed = 0f;
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
			info.AddValue("VirtualLimitBegin", VirtualLimitBegin, typeof(float));
			info.AddValue("Frozen", Frozen, typeof(bool));
		}
		#endregion

		#region MotorControl

		private bool AutoFreezeOnLimits = true;
		LimitState AutoFreezeState = LimitState.Inactive;
		public void ProcessAutoFreeze()
		{
			if (!AutoFreezeOnLimits
				|| AutoFreezeState == LimitState
				|| (LimitState != LimitState.AtLower
				&& LimitState != LimitState.AtUpper)
				|| Frozen)
					return;
			AutoFreezeState = LimitState;
			if (AutoFreezeState == LimitState.AtLower)
			{
				LowerLimit = MinAngle;
				UpperLimit = MinAngle;
			}
			else
			{
				LowerLimit = MaxAngle;
				UpperLimit = MaxAngle;
			}
			//LimitEnabled = true; // normalement deja actif
			MotorEnabled = false;
			_Frozen = true;
		}

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
					value = 0;
				_MaxAngle = value;
				if (!Frozen && !Robot.IsInEditor)
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
					value = 0;
				_MinAngle = value;
				if (!Frozen && !Robot.IsInEditor)
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
				if (Robot.IsInEditor)
					return;
				if (_Frozen)
				{
					LowerLimit = JointAngle;
					UpperLimit = JointAngle;
					LimitEnabled = true;
					MotorEnabled = false;
				}
				else
				{
					UpperLimit = MaxAngle;
					LowerLimit = MinAngle;
					LimitEnabled = _SpotLimitEnabled;
					AutoFreezeState = LimitState.Inactive;
					MotorEnabled = true;
				}
			}
		}

		private bool _SpotLimitEnabled;
		public bool SpotLimitEnabled
		{
			get
			{
				return _SpotLimitEnabled;
			}

			set
			{
				_SpotLimitEnabled = value;
				if (!Frozen && !Robot.IsInEditor)
					base.LimitEnabled = _SpotLimitEnabled;
			}
		}

		public float Force
		{
			get { return MotorSpeed / 15; }
			set
			{
				MotorSpeed = value * 15;
				if ((value <= 0 && AutoFreezeState == LimitState.AtUpper)
					|| (value >= 0 && AutoFreezeState == LimitState.AtLower))
					Frozen = false;
			}
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

		public void moveAnchor(Piece p, Vector2 anchor)
		{
			Robot.ResetActEnds();
			if (BodyA == p)
			{
				LocalAnchorA = BodyA.GetLocalPoint(anchor);
				p = (Piece)BodyB;
			}
			else if (BodyB == p)
			{
				LocalAnchorB = BodyB.GetLocalPoint(anchor);
				p = (Piece)BodyA;
			}
			SynchroniseAnchors(p, true, false);
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

		public void SynchroniseAnchors(Piece piece, bool dynamic, bool reset)
		{
			if (reset)
				Robot.ResetAct();
			if (dynamic && piece.GetType() == typeof(Rod))
			{
				((Rod) piece).GenerateEndFromAnchor(this);
			}
			else if (piece == BodyB)
			{
				((Piece)BodyB).move(WorldAnchorA - ActualWorldAnchorB, dynamic);
			}
			else
				((Piece)BodyA).move(WorldAnchorB - ActualWorldAnchorA, dynamic);
		}

		public void rotate(Piece piece, float angle, Robot robot = null)
		{
			if (Robot != null)
				Robot.ResetAct();
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
			DrawDebug(game, WorldAnchorA);
			DrawDebug(game, WorldAnchorB);
			DrawLimits(game);
		}

		private void DrawDebug(DrawGame game, Vector2 pos)
		{
			//game.Batch().Draw(_tex, new Rectangle((int)corner.X, (int)corner.Y, (int)_spotSize * 2, (int)_spotSize * 2), ColorValue);
			Color tmp = ColorValue;
			if (SpotLimitEnabled)
				ColorValue = Color.Pink;
			game.drawSquare(pos, _spotSize, ColorValue, false);
			if (Frozen)
				game.drawSquare(pos, _spotSize * 0.7f, Color.Cyan, true);
			ColorValue = tmp;

		}

		private void DrawLimits(DrawGame game)
		{
			if (!SpotLimitEnabled)
				;// return;
			int count = 0;
			for (float angleBefore = MinAngle; angleBefore < MaxAngle; angleBefore += 0.1f)
			{
				DrawLimit(game, angleBefore + VirtualLimitBegin, 0.1f + 0.001f * count);
				count++;
			}
			DrawLimit(game, MinAngle + VirtualLimitBegin, 0.001f * count + 0.25f);
			DrawLimit(game, MaxAngle + VirtualLimitBegin, 0.001f * count + 0.25f);
		}

		private void DrawLimit(DrawGame game, float angle, float factor)
		{
			bool isVisible = ((Piece)BodyA).Shown || ((Piece)BodyB).Shown;
			Vector2 target;
			target.X = (float)Math.Cos(angle) * factor;
			target.Y = (float)Math.Sin(angle) * factor;
			game.drawLine(WorldAnchorA, WorldAnchorA + target,
					new Color(new Vector4(ColorValue.ToVector3(), isVisible ? 1f : 0.16f)));
		}


		#endregion

		public string Name { get; set; }
		public Color ColorValue { get; set; }
	}
}
