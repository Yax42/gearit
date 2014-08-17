using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.script.api.game;
using gearit.src.game;
using gearit.src.robot;
using gearit.src.map;

namespace gearit.src.script
{
	public class GameLuaScript : LuaScript
	{
		public GameLuaScript(IGearitGame game, string path)
			: base(path)
		{
			int i = 0;
			foreach (Robot r in game.Robots)
			{
				this["Robot_" + i++] = new GameRobotApi(r);
			}
			foreach (Artefact a in game.Map.Artefacts)
				this["Art_" + a.Id] = new GameArtefactApi(a);
			this["Game"] = new GameApi(game);
		}
	}
}
