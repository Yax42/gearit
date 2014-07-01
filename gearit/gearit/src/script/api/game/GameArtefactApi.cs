using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.map;
using Microsoft.Xna.Framework;

namespace gearit.src.script.api.game
{
	class GameArtefactApi : GameObjectApi
	{
		private Artefact _Artefact;

		public GameArtefactApi(Artefact art)
		{
			_Artefact = art;
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
