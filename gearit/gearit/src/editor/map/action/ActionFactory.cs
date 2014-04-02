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
		DELETE,
		CREATE,
		CHANGE_TYPE,
		RESIZE,

		UNDO,
		REDO,
		SAVE,
		LOAD,
		SAVE_AS,
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
			if (action == ActionTypes.MOVE) return new ActionMove();
			if (action == ActionTypes.DELETE) return new ActionDelete();
			if (action == ActionTypes.CREATE) return new ActionCreate();
			if (action == ActionTypes.RESIZE) return new ActionResize();
			if (action == ActionTypes.CHANGE_TYPE) return new ActionChangeType();
			return _dummy;
		}
	}
}
