using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using gearit.src.output;
using gearit.src.GUI;
using System.Text.RegularExpressions;

namespace gearit.src.editor.robot.action
{
	class ActionLoadRobot : IAction
	{
		public void init() { }

		public bool shortcut()
		{
			if (Input.ctrlAltShift(true, false, false) && (Input.justPressed(Keys.D)))
				MenuRobotEditor.Instance.loadRobot();
			return false;
		}

		public bool run()
		{
			Debug.Assert(RobotEditor.Instance.NamePath != "");
			Robot robot = (Robot)Serializer.DeserializeItem("robot/" + RobotEditor.Instance.NamePath + ".gir");
			if (robot == null)
				RobotEditor.Instance.resetNamePath();
			else
				RobotEditor.Instance.resetRobot(robot);

			// Lua
			var filename = "data/script/" + RobotEditor.Instance.Robot.Name + ".lua";
			string lua = "";

			// Read the file as one string.
			try
			{
				System.IO.StreamReader myFile =
				   new System.IO.StreamReader(filename);
				lua = myFile.ReadToEnd();
				myFile.Close();

				// Lua found - Replace Generated Lua with new one
				Match match = System.Text.RegularExpressions.Regex.Match(lua, "#!#!.*?\n(.*?)// #!#!", RegexOptions.Singleline);

				string fullReplace = lua;
				// Found something
				if (match.Success)
					lua = match.Groups[1].ToString();
				else
					lua = "";
			}
			// No Lua - First time save
			catch (System.IO.IOException e)
			{
			}
			OutputManager.LogInfo("Lua - Generate script editor based on Lua", filename);
			MenuRobotEditor.Instance.setLua(lua);

			return false;
		}

		public void revert() { }

		public bool canBeReverted() { return false; }

		public ActionTypes type() { return ActionTypes.LOAD_ROBOT; }
	}
}