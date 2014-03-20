using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;

namespace gearit.src.editor.robot.action
{
	enum ActionTypes
	{
		NONE = 0,
		MAIN_SELECT,
		SELECT2,
		DELETE_PIECE,
		MOVE_PIECE,
		PRIS_SPOT,
		REV_SPOT,
		SHOW_ALL,
		HIDE,
		LAUNCH,
		MOVE_ANCHOR,
		DELETE_SPOT,
		PRIS_LINK,
		REV_LINK,
		RESIZE_PIECE,
		CHOOSE_SET,
		MOVE_ROBOT,
		LOAD_ROBOT,
		SAVE_ROBOT,
		CHANGE_LIMIT,
		SWAP_LIMIT,
		UNDO,
		REDO,
		COUNT
	}

	interface IAction
	{
		bool canBeReverted();
		void init();
		bool shortcut();
		bool run();
		void revert();
		ActionTypes type();
	}
}
