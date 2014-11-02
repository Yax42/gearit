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
		public IGearitGame Game { private set; get; }
		private GameApi GameApi;
		private LuaFunction RobotScript;

		public GameLuaScript(IGearitGame game, string path)
			: base(path)
		{
			LuaFunction robotConnectedScript = LoadScript(Path.ChangeExtension(FileName, ".robot.init.lua"));
			RobotScript = LoadScript(Path.ChangeExtension(FileName, ".robot.lua"));

			Instance = this;
			if (game.GetType() == typeof(NetworkServerGame))
				ServerGame = (NetworkServerGame)game;
			else
				ServerGame = null;
			Game = game;
			GameApi = new GameApi(game,
				delegate(int lastRobot)
				{
					DoString("Robot = Game:Robot(" + lastRobot + " )");
					if (robotConnectedScript != null)
					{
						robotConnectedScript.Call();
					}
				}
			);
			this["Game"] = GameApi;
			this["G"] = GameApi;
			this["Math"] = new MathApi();
			Load();
		}

		public override void run()
		{
			try
			{
				if (RobotScript != null)
				{
					for (int i = 0; i < GameApi.RobotCount; i++)
					{
						DoString("Robot = Game:Robot(" + i + " )");
						RobotScript.Call();
					}
				}
			}
			catch (Exception ex)
			{
				OutputManager.LogError("Lua script (" + Path.ChangeExtension(FileName, ".robot.lua") + ") - " + ex.Message);
			}
			base.run();
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
