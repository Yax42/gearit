using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace gearit.src.editor.map.action
{
	class ActionPickColor : IAction
	{
		public const float Ray = 1f;
		public Vector2 Origin {get; private set;}
		private MapChunk Select;
		private Color PreviousColor;
		private bool HasBeenRevert;
		private Color Color;

		public void init()
		{
			Select = MapEditor.Instance.SelectChunk;
			Origin = Input.SimMousePos;
			PreviousColor = Select.Color;
			HasBeenRevert = false;
		}

		public bool shortcut()
		{
			if (ActionSwapEventMode.EventMode)
				return false;
			return Input.CtrlShift(false, false) &&
				Input.justPressed(Keys.T);
		}

		public bool run()
		{
			if (HasBeenRevert)
			{
				Select.Color = Color;
				return false;
			}
			Color = DrawGame.GenerateColor(Origin, Input.SimMousePos, Ray);
			Select.Color = Color;
			return Input.pressed(Keys.T);
		}

		public void revert()
		{
			HasBeenRevert = true;
			Select.Color = PreviousColor;
		}

		public bool canBeReverted { get { return true; } }
		public bool canBeMirrored { get { return true; } }

		public bool actOnSelect() { return false; }

		public ActionTypes Type() { return ActionTypes.PICK_COLOR; }
	}
}