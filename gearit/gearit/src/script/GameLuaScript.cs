using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.script.api.game;
using gearit.src.game;
using gearit.src.robot;
using gearit.src.map;
using gearit.src.editor.map;
using gearit.src.Network;
using LuaInterface;
using System.IO;
using gearit.src.output;

namespace gearit.src.script
{
	class GameLuaScript : LuaScript
	{
		public static GameLuaScript Instance { private set; get; }

		public NetworkServerGame ServerGame { private set; get; }
		private GameApi GameApi;

		public GameLuaScript(IGearitGame game, string path)
			: base(path)
		{
			string robotConnectedPath = Path.ChangeExtension(FileName, ".robot.lua");
			LuaFunction robotConnectedScript;
			try
			{
				robotConnectedScript = LoadFile(robotConnectedPath);
			}
			catch (Exception ex)
			{
				OutputManager.LogWarning("Lua script (" + robotConnectedPath + ") - " + ex.Message);
				robotConnectedScript = null;
			}

			Instance = this;
			if (game.GetType() == typeof(NetworkServerGame))
				ServerGame = (NetworkServerGame)game;
			else
				ServerGame = null;
			GameApi = new GameApi(game,
				delegate(int lastRobot)
				{
					if (robotConnectedScript != null)
					{
						DoString("LastRobot = Game:Robot(" + lastRobot + " )");
						robotConnectedScript.Call();
					}
				}
			);
			this["Game"] = GameApi;
			this["G"] = GameApi;
			Load();
		}

		public void RobotConnect(Robot r)
		{
			GameApi.AddRobot(r);
		}

		public void RobotDisconnect(Robot r)
		{
			GameApi.RemoveRobot(r);
		}

		public static bool IsServer { get { return Instance.ServerGame != null; } }
		public static PacketManager PacketManager { get { return Instance.ServerGame.PacketManager; } }
		public static void PushEvent(byte[] data)
		{
			Instance.ServerGame.Events = Instance.ServerGame.Events.Concat(data).ToArray();
		}
	}
}
