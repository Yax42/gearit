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

			int i = 0;
			foreach (Robot r in game.Robots)
			{
				this["Robot_" + i++] = new GameRobotApi(r);
			}
			foreach (Artefact a in game.Map.Artefacts)
				this["Art_" + a.Id] = new GameArtefactApi(a);
			this["Game"] = new GameApi(game);
			
			foreach (MapChunk c in game.Map.Chunks)
				if (c.StringId != null && c.StringId != "")
					this["Object_" + c.StringId] = new GameChunkApi(c);
		}

		public static bool IsServer { get { return Instance.ServerGame != null; } }
		public static InGamePacketManager PacketManager { get { return Instance.ServerGame.PacketManager; } }
		public static void PushEvent(byte[] data)
		{
			Instance.ServerGame.Events.Concat(data);
		}

	}
}
