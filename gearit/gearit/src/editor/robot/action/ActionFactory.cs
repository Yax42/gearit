using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace gearit.src.editor.robot.action
{
	class ActionFactory
	{
		static private bool _isInit = false;
		static private IAction[] _shortCuts = new IAction[(int)ActionTypes.COUNT];
		static private ActionDummy _dummy;

		public static void init()
		{
			if (_isInit)
				return;
			_dummy = new ActionDummy();
			_isInit = true;
			for (int i = 0; i < (int)ActionTypes.COUNT; i++)
				_shortCuts[i] = create(i);
		}

		public static ActionDummy Dummy
		{
			get
			{
				return _dummy;
			}
		}

		public static IAction createFromShortcut()
		{
			for (int i = 0; i < (int)ActionTypes.COUNT; i++)
				if (_shortCuts[i].shortcut())
					return create(i);
			return create(ActionTypes.NONE);
		}

		public static IAction create(int action)
		{
			return create((ActionTypes)action);
		}

		public static IAction create(ActionTypes action)
		{
			Debug.Assert(_isInit);
			if (action == ActionTypes.NONE) return _dummy;
			if (action == ActionTypes.MAIN_SELECT) return new ActionMainSelect();
			if (action == ActionTypes.SELECT2) return new ActionSelect2();
			if (action == ActionTypes.DELETE_PIECE) return new ActionDeletePiece();
			if (action == ActionTypes.MOVE_PIECE) return new ActionMovePiece();
			if (action == ActionTypes.PRIS_SPOT) return new ActionPrisSpot();
			if (action == ActionTypes.REV_SPOT) return new ActionRevSpot();
			if (action == ActionTypes.SHOW_ALL) return new ActionShowAll();
			if (action == ActionTypes.HIDE) return new ActionHide();
			if (action == ActionTypes.LAUNCH) return new ActionLaunch();
			if (action == ActionTypes.MOVE_ANCHOR) return new ActionMoveAnchor();
			if (action == ActionTypes.DELETE_SPOT) return new ActionDeleteSpot();
			if (action == ActionTypes.PRIS_LINK) return new ActionPrisLink();
			if (action == ActionTypes.REV_LINK) return new ActionRevLink();
			if (action == ActionTypes.RESIZE_PIECE) return new ActionResizePiece();
			if (action == ActionTypes.CHOOSE_SET) return new ActionChooseSet();
			if (action == ActionTypes.MOVE_ROBOT) return new ActionMoveRobot();
			if (action == ActionTypes.SAVE_ROBOT) return new ActionSaveRobot();
			if (action == ActionTypes.LOAD_ROBOT) return new ActionLoadRobot();
			if (action == ActionTypes.CHANGE_LIMIT) return new ActionChangeLimit();
			if (action == ActionTypes.SWAP_LIMIT) return new ActionSwapLimit();
			if (action == ActionTypes.UNDO) return new ActionUndo();
			return _dummy;
		}
	}
}
