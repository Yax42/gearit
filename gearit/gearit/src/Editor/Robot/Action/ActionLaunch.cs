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

namespace gearit.src.editor.robot.action
{
	class ActionLaunch : IAction
	{
		static public bool Running = false;
		static private World SimulationWorld = null;
		static public Robot Robot;
		public void init()
		{
			if (SimulationWorld == null)
				SimulationWorld = new World(new Vector2(0, 9.8f));
			if (RobotEditor.Instance.NamePath != "")
			{
				new ActionSaveRobot().run();
				SerializerHelper.World = SimulationWorld;
				Robot = (Robot)Serializer.DeserializeItem(RobotEditor.Instance.ActualPath);
				Debug.Assert(Robot != null);
				Running = (Robot != null);
				Robot.Heart.IsStatic = true;
				Robot.InitScript();
			}
		}

		//feature non fonctionnelle yet
		public bool shortcut()
		{
			return (Input.CtrlShift(false, false) && Input.justPressed(Keys.Enter))
				|| (Input.CtrlShift(false, true) && Input.justPressed(Keys.Space));
		}

		public bool run()
		{
			if (!Running || Input.Exit)
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
			return true;
		}

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return false; } }
		public ActionTypes Type() { return ActionTypes.LAUNCH; }
	}
}
