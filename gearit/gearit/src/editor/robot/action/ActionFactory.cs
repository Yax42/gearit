using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace gearit.src.editor.robot.action
{
	enum ActionTypes
	{
		NONE = 0,
		SELECT,
		SWAP_SELECT,
		DELETE_PIECE,
		MOVE_PIECE,
		SHOW_ALL,
		HIDE,
		LAUNCH,
		MOVE_ANCHOR,
		DELETE_SPOT,
		LINK,
		RESIZE_HEART,
		RESIZE_WHEEL,
		CHOOSE_SET,
		MOVE_ROBOT,
		LOAD_ROBOT,
		SAVE_ROBOT,
		CHANGE_LIMIT,
		SWAP_LIMIT,
		LIMIT_FROZEN,
		UNDO,
		REDO,
		CREATE_PIECE,
		SET_AXIS,
		SHOW_HELP,
		EXIT,
		COUNT
	}

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
			if (action == ActionTypes.SELECT) return new ActionSelect();
			if (action == ActionTypes.SWAP_SELECT) return new ActionSwapSelects();
			if (action == ActionTypes.DELETE_PIECE) return new ActionDeletePiece();
			if (action == ActionTypes.MOVE_PIECE) return new ActionMovePiece();
			if (action == ActionTypes.SHOW_ALL) return new ActionShowAll();
			if (action == ActionTypes.HIDE) return new ActionHide();
			if (action == ActionTypes.LAUNCH) return new ActionLaunch();
			if (action == ActionTypes.MOVE_ANCHOR) return new ActionMoveAnchor();
			if (action == ActionTypes.DELETE_SPOT) return new ActionDeleteSpot();
			if (action == ActionTypes.LINK) return new ActionLink();
			if (action == ActionTypes.RESIZE_HEART) return new ActionResizeHeart();
			if (action == ActionTypes.RESIZE_WHEEL) return new ActionResizeWheel();
			if (action == ActionTypes.CHOOSE_SET) return new ActionChooseSet();
			if (action == ActionTypes.MOVE_ROBOT) return new ActionMoveRobot();
			if (action == ActionTypes.SAVE_ROBOT) return new ActionSaveRobot();
			if (action == ActionTypes.LOAD_ROBOT) return new ActionLoadRobot();
			if (action == ActionTypes.CHANGE_LIMIT) return new ActionChangeLimit();
			if (action == ActionTypes.SWAP_LIMIT) return new ActionSwapLimit();
			if (action == ActionTypes.LIMIT_FROZEN) return new ActionFreezeSpot();
			if (action == ActionTypes.UNDO) return new ActionUndo();
			if (action == ActionTypes.REDO) return new ActionRedo();
			if (action == ActionTypes.CREATE_PIECE) return new ActionCreatePiece();
			if (action == ActionTypes.SET_AXIS) return new ActionSetAxis();
			if (action == ActionTypes.SHOW_HELP) return new ActionShowHelp();
			if (action == ActionTypes.EXIT) return new ActionExit();
			return _dummy;
		}
	}
}
