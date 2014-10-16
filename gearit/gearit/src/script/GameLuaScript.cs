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

namespace gearit.src.script
{
	class GameLuaScript : LuaScript
	{
		public static GameLuaScript Instance { private set; get; }
		public NetworkServerGame ServerGame { private set; get; }

		public GameLuaScript(IGearitGame game, string path)
			: base(path)
		{
			Instance = this;
			if (game.GetType() == typeof(NetworkServerGame))
				ServerGame = (NetworkServerGame)game;
			else
				ServerGame = null;
			var g = new GameApi(game);
			this["Game"] = g;
			this["G"] = g;
		}

		public static bool IsServer { get { return Instance.ServerGame != null; } }
		public static PacketManager PacketManager { get { return Instance.ServerGame.PacketManager; } }
		public static void PushEvent(byte[] data)
		{
			Instance.ServerGame.Events = Instance.ServerGame.Events.Concat(data).ToArray();
		}

	}
}
