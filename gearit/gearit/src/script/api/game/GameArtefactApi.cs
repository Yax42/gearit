﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.map;
using Microsoft.Xna.Framework;

namespace gearit.src.script.api.game
{
    /// <summary>
    /// Lua API for artefact (Position in space)
    /// </summary>
	class GameArtefactApi : GameObjectApi
	{
		private Artefact _Artefact;

		public GameArtefactApi(Artefact art)
		{
			_Artefact = art;
		}

		public bool isTouching(GameObjectApi o)
		{
			return (false);
		}

		public override Vector2 Position
		{
			get
			{
				return _Artefact.Position;
			}

			set
			{
				_Artefact.Position = value;
			}
		}
	}
}
