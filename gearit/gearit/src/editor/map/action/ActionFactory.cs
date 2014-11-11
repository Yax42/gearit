using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace gearit.src.editor.map.action
{
	enum ActionTypes
	{
		NONE = 0,
		SELECT,
		MOVE,
		DELETE_CHUNK,
		CREATE_WALL,
		CREATE_BALL,
		CHANGE_TYPE,
		RESIZE_POLYGON,
		RESIZE_CIRCLE,
		UNDO,
		REDO,
		SAVE,
		LOAD,
		SHOW_HELP,
		EXIT,
		SWAP_EVENT_MODE,
		CREATE_ARTEFACT,
		CREATE_TRIGGER,
		DELETE_TRIGGER,
		DELETE_ARTEFACT,
		SET_TRIGGER_ID,
		SET_AXIS,
		PICK_COLOR,
		COUNT,
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
			if (action == ActionTypes.MOVE) return new ActionMove();
			if (action == ActionTypes.DELETE_CHUNK) return new ActionDeleteChunk();
			if (action == ActionTypes.CREATE_BALL) return new ActionCreateBall();
			if (action == ActionTypes.CREATE_WALL) return new ActionCreateWall();
			if (action == ActionTypes.RESIZE_CIRCLE) return new ActionResizeCircle();
			if (action == ActionTypes.RESIZE_POLYGON) return new ActionResizePolygon();
			if (action == ActionTypes.CHANGE_TYPE) return new ActionChangeType();
			if (action == ActionTypes.UNDO) return new ActionUndo();
			if (action == ActionTypes.REDO) return new ActionRedo();
			if (action == ActionTypes.SAVE) return new ActionSaveMap();
			if (action == ActionTypes.LOAD) return new ActionLoadMap();
			if (action == ActionTypes.SHOW_HELP) return new ActionShowHelp();
			if (action == ActionTypes.EXIT) return new ActionExit();
			if (action == ActionTypes.SWAP_EVENT_MODE) return new ActionSwapEventMode();
			if (action == ActionTypes.CREATE_ARTEFACT) return new ActionCreateArtefact();
			if (action == ActionTypes.CREATE_TRIGGER) return new ActionCreateTrigger();
			if (action == ActionTypes.DELETE_TRIGGER) return new ActionDeleteTrigger();
			if (action == ActionTypes.DELETE_ARTEFACT) return new ActionDeleteArtefact();
			if (action == ActionTypes.SET_TRIGGER_ID) return new ActionSetTriggerId();
			if (action == ActionTypes.SET_AXIS) return new ActionSetAxis();
			if (action == ActionTypes.PICK_COLOR) return new ActionPickColor();
			return _dummy;
		}
	}
}
