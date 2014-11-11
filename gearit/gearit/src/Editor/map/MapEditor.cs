using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using gearit.src.utility.Menu;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using gearit.src.editor.robot;
using FarseerPhysics.Factories;
using gearit.src.map;
using gearit.src.editor.map.action;
using gearit.src.editor.map;
using FarseerPhysics.DebugViews;

namespace gearit.src.editor.map
{

    /// <summary>
    /// Handle the whole Map editor
    /// </summary>
	class MapEditor : GameScreen, IDemoScreen
	{
		public World World { get; private set; }
		public Map Map { get; private set; }
		private EditorCamera _camera;
		private DrawGame DrawGame;
		private const int PropertiesMenuSize = 40;
		private MapChunk _dummyChunk;
		private Trigger _dummyTrigger;
		private MapChunk _selectedChunk;
		private MapChunk _selectedMirrorChunk;
		private Trigger _selectedTrigger;
		private Trigger _selectedMirrorTrigger;

		//Action
		private List<IAction> _actionsLog;
		private List<IAction> _redoActionsLog;
		private IAction _currentAction;
		private IAction _mirrorAction;

		public static MapEditor Instance { private set; get; }

		public MapChunk SelectChunk
		{
			get
			{
				if (MirrorAxis.Active)
					return _selectedMirrorChunk;
				else
					return _selectedChunk;
			}
			set
			{
				if (value == null)
					value = _dummyChunk;
				if (MirrorAxis.Active)
					_selectedMirrorChunk = value;
				else
					_selectedChunk = value;
			}
		}

		public Trigger SelectTrigger
		{
			get
			{
				if (MirrorAxis.Active)
					return _selectedMirrorTrigger;
				else
					return _selectedTrigger;
			}
			set
			{
				if (value == null)
					value = _dummyTrigger;
				if (MirrorAxis.Active)
					_selectedMirrorTrigger = value;
				else
					_selectedTrigger = value;
			}
		}

		public bool IsSelectDummy()
		{
			if (ActionSwapEventMode.EventMode)
				return SelectTrigger == _dummyTrigger;
			else
				return SelectChunk == _dummyChunk;
		}

		#region IDemoScreen Members

		public string GetTitle()
		{
			return "Map";
		}

		public string GetDetails()
		{
			return "";
		}

		#endregion

		public MapEditor() : base(false)
		{
			ActionFactory.init();
			TransitionOnTime = TimeSpan.FromSeconds(0.75);
			TransitionOffTime = TimeSpan.FromSeconds(0.75);
			World = null;
			HasCursor = true;
			Instance = this;
		}


		public override void LoadContent()
		{
			base.LoadContent();

			if (World == null)
				World = new World(Vector2.Zero);
			else
				World.Clear();

			ActionSwapEventMode.EventMode = false;

			SerializerHelper.World = World;
			Map = new Map(World);

			_dummyChunk = new PolygonChunk(Map, false, Vector2.Zero);
			_dummyTrigger = new Trigger(Vector2.Zero);
			_actionsLog = new List<IAction>();
			_redoActionsLog = new List<IAction>();
			_currentAction = ActionFactory.Dummy;
			_mirrorAction = ActionFactory.create(ActionTypes.NONE);

			_selectedMirrorChunk = _dummyChunk;
			_selectedChunk = _dummyChunk;
			_selectedMirrorTrigger = _dummyTrigger;
			_selectedTrigger = _dummyTrigger;

			_prevName = Map.FullPath;
			ScreenManager.Game.ResetElapsedTime();
			_camera = new EditorCamera(ScreenManager.GraphicsDevice.Viewport);
			_camera.Position = new Vector2(0, 0);
			World.Gravity = new Vector2(0f, 0f);
			HasVirtualStick = true;
			SelectChunk = null;

			// TMP
			DrawGame = DrawGame.Instance;
			Rectangle rec = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);

			new MenuMapEditor(ScreenManager);
		}

		public override void QuickLoadContent()
		{
			MenuMapEditor.Instance.setFocus(false);
		}

		public override void Update(GameTime gameTime)
		{
			_camera.update();
			HandleInput();
			MenuMapEditor.Instance.Update();
			World.Step(0f);
			base.Update(gameTime);
		}

		public void doAction(ActionTypes action = ActionTypes.NONE)
		{
			if (_currentAction.Type() == ActionTypes.NONE)
			{
				if (action == ActionTypes.NONE)
					_currentAction = ActionFactory.createFromShortcut();
				else
					_currentAction = ActionFactory.create(action);
				if (IsSelectDummy() && _currentAction.actOnSelect())
					_currentAction = ActionFactory.Dummy;
				_currentAction.init();

				if (Input.Alt && _currentAction.canBeMirrored)
				{
					MirrorAxis.Active = true;
					_mirrorAction = ActionFactory.createFromShortcut();
					if (_mirrorAction.Type() == _currentAction.Type())
						_mirrorAction.init();
					else
						_mirrorAction = ActionFactory.Dummy;
					MirrorAxis.Active = false;
				}
			}
		}

		private void PushRevertAction(IAction action)
		{
			if (!action.canBeReverted)
				return;
			_actionsLog.Insert(0, action);
			if (_actionsLog.Count > 200)
				_actionsLog.RemoveAt(_actionsLog.Count - 1);
			_redoActionsLog.Clear();
		}

		private void HandleInput()
		{
			if (Input.justPressed(Keys.Z))
				LockAxis.Origin = Input.SimMousePos;
			LockAxis.Active = Input.pressed(Keys.Z) && _currentAction.Type() != ActionTypes.SET_AXIS;
			if (_currentAction.Type() == ActionTypes.NONE)
			{
				if (MenuMapEditor.Instance.hasFocus())
					return;
				doAction();
			}
			MirrorAxis.Active = true;
			_mirrorAction.run();
			MirrorAxis.Active = false;
			if (_currentAction.run() == false)
			{
				PushRevertAction(_currentAction);
				PushRevertAction(_mirrorAction);
				_currentAction = ActionFactory.Dummy;
				_mirrorAction = ActionFactory.Dummy;
			}
			_camera.input();
		}

		public void undo()
		{
			if (_actionsLog.Count == 0)
				return;
			IAction a = _actionsLog.ElementAt(0);
			a.revert();
			_actionsLog.Remove(a);
			_redoActionsLog.Insert(0, a);
		}

		public void redo()
		{
			if (_redoActionsLog.Count == 0)
				return;
			IAction a = _redoActionsLog.ElementAt(0);
			a.run();
			_redoActionsLog.Remove(a);
			_actionsLog.Insert(0, a);
		}

		//---------------------------NAME----------------------------------------

		private string _prevName;
		public string NamePath
		{
			get
			{
				return Map.FullPath;
			}
			set
			{
				_prevName = Map.FullPath;
				Map.FullPath = value;
				MenuMapEditor.Instance.updateButtonMapName();
			}
		}

		public void resetNamePath()
		{
			NamePath = _prevName;
			_prevName = "";
		}

		public void resetMap(Map map)
		{
			if (Map != null)
				Map.ExtractFromWorld();
			_actionsLog.Clear();
			_redoActionsLog.Clear();
			Map = map;
			SelectChunk = null;
			SelectTrigger = null;
		}
		//-----------------------------------------------------------------------

		private int SaveCount = 0;
		public void DrawStatics()
		{
			const int cycle = 80;
			DrawGame.Begin();
			if (ActionSaveMap.JustSaved)
			{
				SaveCount++;
				DrawGame.DrawSpirale(new Vector2(ScreenManager.Width - 20, ScreenManager.Height - 20),
							cycle, SaveCount, 5);
				if (SaveCount >= cycle)
				{
					SaveCount = 0;
					ActionSaveMap.JustSaved = false;
				}
			}
			DrawGame.End();
		}

		private void DrawMarks()
		{
			Color col = Color.Red;
			col.A = 230;
			DrawGame.DrawLine(LockAxis.Origin - 1000 * LockAxis.Dir,
							LockAxis.Origin + 1000 * LockAxis.Dir,
							col);
			col = Color.White;
			col.A = 236;
			DrawGame.DrawLine(MirrorAxis.Origin - 1000 * MirrorAxis.Dir,
							MirrorAxis.Origin + 1000 * MirrorAxis.Dir,
							col);
		}

		private void DrawPickColor()
		{
			if (_currentAction.Type() == ActionTypes.PICK_COLOR)
			{
				var act = (ActionPickColor)_currentAction;
				DrawGame.DrawCircle(act.Origin, 0.1f, Color.White, true);
				DrawGame.DrawCircle(act.Origin, ActionPickColor.Ray, Color.Red);
				DrawGame.DrawCircle(act.Origin, ActionPickColor.Ray * 2, Color.Black);
			}
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			if (ActionSwapEventMode.EventMode)
				ScreenManager.GraphicsDevice.Clear(Color.LightSeaGreen);
			else
				ScreenManager.GraphicsDevice.Clear(Color.LightGreen);
			DrawGame.BeginPrimitive(_camera);
			DrawGame.DrawGrille(new Color(0, 0, 0, 0.1f));

			MirrorAxis.Active = Input.Alt;
			Map.DrawDebug(DrawGame, Input.Shift || _currentAction.Type() == ActionTypes.PICK_COLOR, ActionSwapEventMode.EventMode);
			MirrorAxis.Active = false;
			DrawMarks();
			DrawPickColor();
			DrawGame.EndPrimitive();
			DrawStatics();
			MenuMapEditor.Instance.Draw();
		}

		public override void positionChanged(int x, int y)
		{
			MenuMapEditor.Instance.positionChanged(x, y);
		}

		public override Squid.Point getMenuPosition()
		{
			return (MenuMapEditor.Instance.getMenuPosition());
		}

		public override Squid.Point getMenuSize()
		{
			return (MenuMapEditor.Instance.getMenuSize());
		}
	}
}
