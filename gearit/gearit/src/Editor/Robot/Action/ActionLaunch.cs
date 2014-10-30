using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using gearit.src.robot;
using System.Diagnostics;
using gearit.src.editor.map;
using gearit.src.map;

namespace gearit.src.editor.robot.action
{
	class ActionLaunch : IAction
	{
		static public bool Running = false;
		static public World SimulationWorld = null;
		static public Robot Robot;
		private PolygonChunk Ground;
		private bool FirstFrame;

		public static void ResetWorld()
		{
			if (SimulationWorld == null)
				SimulationWorld = new World(new Vector2(0, 9.8f));
			else
				SimulationWorld.Clear();
		}

		public void init()
		{
			ResetWorld();
			if (RobotEditor.Instance.NamePath != "")
			{
				new ActionSaveRobot().run();
				SerializerHelper.World = SimulationWorld;
				Robot = (Robot)Serializer.DeserializeItem(RobotEditor.Instance.ActualPath);
				Debug.Assert(Robot != null);
				Running = (Robot != null);
				Robot.InitScript();
				FirstFrame = true;
				if (!Input.Ctrl)
				{
					Robot.Heart.IsStatic = true;
				}
				else
				{
					Ground = new PolygonChunk(SimulationWorld, false, new Vector2(-100, 20));
					Ground.ShapeRectangle(200, 5);
				}
			}
			else
				Running = false;
		}

		//feature non fonctionnelle yet
		public bool shortcut()
		{
			return (!Input.Shift && Input.justPressed(Keys.Enter))
				|| (Input.Shift && Input.justPressed(Keys.Space));
		}

		public bool run()
		{
			if (!Running || (shortcut() && !FirstFrame))
			{
				if (Robot != null)
					Robot.ExtractFromWorld();
				SimulationWorld.Clear();
				Robot = null;
				Running = false;
				return false;
			}
			Robot.Update();
			SimulationWorld.Step(1/30f);
			FirstFrame = false;
			return true;
		}

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return false; } }
		public ActionTypes Type() { return ActionTypes.LAUNCH; }
	}
}
