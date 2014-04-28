﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;
using System.Diagnostics;
using GUI;
using gearit.src.output;
using System.Text.RegularExpressions;
using gearit.src.GUI;

namespace gearit.src.editor.robot.action
{
	class ActionSaveRobot : IAction
	{
		static public bool MustExit = false;

		public void init() { }

		public bool shortcut()
		{
			if (Input.ctrlAltShift(true, false, false) && (Input.justPressed(Keys.S)))
				MenuRobotEditor.Instance.saveRobot();
			if (Input.ctrlAltShift(true, false, true) && (Input.justPressed(Keys.S)))
				MenuRobotEditor.Instance.saveasRobot();
			return false;
		}

		public bool run()
		{
			Debug.Assert(RobotEditor.Instance.NamePath != "");
			RobotEditor.Instance.Robot.Name = RobotEditor.Instance.NamePath;
			string actualName = "robot/" + RobotEditor.Instance.NamePath + ".gir";
			if (!Serializer.SerializeItem(actualName, RobotEditor.Instance.Robot))
				RobotEditor.Instance.resetNamePath();
			else if (MustExit)
			{
				MustExit = false;
				ScreenMainMenu.GoBack = true;
			}


			#region Lua

			var filename = "data/script/" + RobotEditor.Instance.Robot.Name + ".lua";

			// Read the file as one string.
			try
			{
				System.IO.StreamReader myFile =
				   new System.IO.StreamReader(filename);
				string lua = myFile.ReadToEnd();
				myFile.Close();

				// Lua found - Replace Generated Lua with new one
				OutputManager.LogInfo("Lua - Replace generated script");
				Match match = System.Text.RegularExpressions.Regex.Match(lua, "#!#!.*?\n(.*?)// #!#!", RegexOptions.Singleline);

				string fullReplace = lua;
				// Found something
				if (match.Success)
				{
					var firstPart = lua.Substring(0, match.Groups[1].Index);
					var secondPart = lua.Substring(match.Groups[1].Index + match.Groups[1].Length);
					fullReplace = firstPart + "// Generated" + Environment.NewLine + MenuRobotEditor.Instance.getLua() + secondPart;
				}
				// Set new file
				System.IO.File.WriteAllText(filename, fullReplace);
			}
			// No Lua - First time save
			catch (System.IO.IOException e)
			{
				OutputManager.LogInfo("Lua - Not found : generate new file");
				string emptylua = "require 'header'" + Environment.NewLine + "jump_time = 0" + Environment.NewLine + "" + Environment.NewLine +
					"while true do" + Environment.NewLine + "// #!#! Lua generated - Do not modify" + Environment.NewLine + "// Generated" + Environment.NewLine
					+ MenuRobotEditor.Instance.getLua() + "// #!#! End Lua generated" + Environment.NewLine + "end";

				System.IO.File.WriteAllText(filename, emptylua);
			}

			#endregion


			return (false);
		}

		public void revert() { }

		public bool canBeReverted() { return false; }

		public ActionTypes type() { return ActionTypes.SAVE_ROBOT; }
	}
}
