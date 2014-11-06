﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using gearit.src.GUI;

namespace gearit.src.editor.robot
{
    /// <summary>
    /// Handle the whole Robot editor
    /// </summary>
	class LuaManager
	{
		public static string Header
		{
			get
			{
				return "-- #BeginAutoGenerated" + Environment.NewLine;
			}
		}
		public static string Footer
		{
			get
			{
				return "-- #EndAutoGenerated" + Environment.NewLine;
			}
		}
		public static string EarlyWarning
		{
			get
			{
				return "-- Auto generated code, do NOT modify" + Environment.NewLine;
			}
		}

		public static string GenerateAllScript(string firstPart, string secondPart)
		{
			return firstPart
				+ LuaManager.EarlyWarning
				+ GetLua()
				+ Environment.NewLine
				+ secondPart;
		}

		// Generate Lua depending on the user added to our table
		private static string GetLua()
		{
			string lua = "";

			foreach (var event_node in MenuRobotEditor.Instance.EventNodes)
			{
				string evt_lua = event_node.toLua();
				if (evt_lua == "")
					continue;

				lua += evt_lua + Environment.NewLine;

				foreach (var action_node in event_node._nodes)
					lua += action_node.toLua() + Environment.NewLine;

				lua += "end" + Environment.NewLine;
			}
			return (lua);
		}

		public static void SetLua(string lua)
		{
			// Clear
			MenuRobotEditor.Instance.EventNodes.Clear();
			MenuRobotEditor.Instance.refreshScriptEditor();

			Match match;

			// Check each line and create table
			// Show MessageBox if fail
			using (StringReader reader = new StringReader(lua))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					match = System.Text.RegularExpressions.Regex.Match(line, "if Input:pressed\\(K_(.)");
					// New bloc
					if (match.Success)
					{
						TreeNodeEvent node_event = new TreeNodeEvent(MenuRobotEditor.Instance.PanelScript.Size.x);
						MenuRobotEditor.Instance.EventNodes.Add(node_event);
						node_event.setKey(match.Groups[1].ToString());

						// Get all actions
						while ((line = reader.ReadLine()) != null && !line.StartsWith("end"))
						{
							match = System.Text.RegularExpressions.Regex.Match(line, "\t(.*?)\\..*?= (.*)");
							// Found action
							if (match.Success)
							{
								Console.WriteLine("[" + match.Groups[2].ToString() + "]");
								TreeNodeAction node_action = node_event.addAction();
								node_action.setSpot(match.Groups[1].ToString());
								node_action.setState(match.Groups[2].ToString());
							}
						}

						node_event.Close();
					}
				}

				MenuRobotEditor.Instance.refreshScriptEditor();
			}
		}

		public static string Regex
		{
			get
			{
				return Header + "(.*?)" + Footer;
			}

		}
	}
}
