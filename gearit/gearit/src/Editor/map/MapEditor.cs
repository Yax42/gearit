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
		private World _world;
		private Map _map;
		private EditorCamera _camera;
		private DrawGame _draw_game;
		private const int PropertiesMenuSize = 40;
		private MapChunk _dummyChunk;
		private Trigger _dummyTrigger;
		private MapChunk _selectedChunk;
		private Trigger _selectedTrigger;

		//Action
		private List<IAction> _actionsLog;
		private List<IAction> _redoActionsLog;
		private IAction _currentAction;

		public static MapEditor Instance { private set; get; }

		public MapChunk SelectChunk
		{
			get
			{
				return _selectedChunk;
			}
			set
			{
				if (value == null)
					_selectedChunk = _dummyChunk;
				else
					_selectedChunk = value;
			}
		}

		public Trigger SelectTrigger
		{
			get
			{
				return _selectedTrigger;
			}
			set
			{
				if (value == null)
					_selectedTrigger = _dummyTrigger;
				else
					_selectedTrigger = value;
			}
		}
		public bool IsSelectDummy()
		{
			if (ActionSwapEventMode.EventMode)
				return _selectedTrigger == _dummyTrigger;
			else
				return _selectedChunk == _dummyChunk;
		}

		public Map Map { get { return _map; } }
		public World World { get { return _world; } }

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
			_world = null;
			HasCursor = true;
			Instance = this;
		}


		public override void LoadContent()
		{
			base.LoadContent();

			if (_world == null)
				_world = new World(Vector2.Zero);
			else
				_world.Clear();
			_dummyChunk = new PolygonChunk(_world, false, Vector2.Zero);
			_dummyTrigger = new Trigger(Vector2.Zero);
			_actionsLog = new List<IAction>();
			_redoActionsLog = new List<IAction>();
			_currentAction = ActionFactory.Dummy;

			SelectChunk = null;
			SelectTrigger = null;
			ActionSwapEventMode.EventMode = false;

			SerializerHelper.World = _world;
			_map = new Map(_world);
			ScreenManager.Game.ResetElapsedTime();
			_camera = new EditorCamera(ScreenManager.GraphicsDevice.Viewport);
			_camera.Position = new Vector2(0, 0);
			_world.Gravity = new Vector2(0f, 0f);
			HasVirtualStick = true;
			SelectChunk = null;

			// TMP
			_draw_game = new DrawGame(ScreenManager.GraphicsDevice);
			Rectangle rec = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);

			new MenuMapEditor(ScreenManager);
			NamePath = "";
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
			_world.Step(0f);
			base.Update(gameTime);
		}

		public void doAction(ActionTypes action)
		{
			if (_currentAction.type() == ActionTypes.NONE)
			{
				_currentAction = ActionFactory.create(action);
				if (IsSelectDummy() && _currentAction.actOnSelect())
					_currentAction = ActionFactory.Dummy;
				_currentAction.init();
			}
		}

		private void HandleInput()
		{
			if (_currentAction.type() == ActionTypes.NONE)
			{
				if (MenuMapEditor.Instance.hasFocus())
					return;
				_currentAction = ActionFactory.createFromShortcut();
				if (_currentAction.actOnSelect() && IsSelectDummy())
					_currentAction = ActionFactory.Dummy;
				_currentAction.init();
			}
			if (_currentAction.run() == false)
			{
				if (_currentAction.canBeReverted())
				{
					_actionsLog.Insert(0, _currentAction);
					if (_actionsLog.Count > 200)
						_actionsLog.RemoveAt(_actionsLog.Count - 1);
					_redoActionsLog.Clear();
				}
				_currentAction = ActionFactory.Dummy;
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

		private string _prevName = "";
		public string NamePath
		{
			get
			{
				return _map.FullPath;
			}
			set
			{
				_prevName = _map.FullPath;
				_map.FullPath = value;
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
			if (_map != null)
				_map.ExtractFromWorld();
			_actionsLog.Clear();
			_redoActionsLog.Clear();
			_map = map;
			SelectChunk = null;
			SelectTrigger = null;
		}
		//-----------------------------------------------------------------------

		public override void Draw(GameTime gameTime)
		{
			if (ActionSwapEventMode.EventMode)
				ScreenManager.GraphicsDevice.Clear(Color.LightSeaGreen);
			_draw_game.Begin(_camera);
			_draw_game.DrawGrille();

			_map.DrawDebug(_draw_game, ActionSwapEventMode.EventMode);

			_draw_game.End();
			base.Draw(gameTime);
			MenuMapEditor.Instance.Draw();
		}

		public override void positionChanged(int x, int y)
		{
			MenuMapEditor.Instance.positionChanged(x, y);
		}

		public override int getMenuWidth()
		{
			return (MenuMapEditor.Instance.getMenuWidth());
		}
	}
}
