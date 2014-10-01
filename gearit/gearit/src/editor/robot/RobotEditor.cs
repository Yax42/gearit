using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using gearit.src.robot;
using gearit.src.editor.robot.action;
using FarseerPhysics.Dynamics;
using gearit.src.utility.Menu;
using System.Runtime.Serialization;
using System.Diagnostics;
using gearit.src.GUI;
using FarseerPhysics.DebugViews;

namespace gearit.src.editor.robot
{
	class RobotEditor : GameScreen, IDemoScreen
	{
		private World _world;
		private EditorCamera _camera;

		// Graphic
		private RectangleOverlay _background;

		// Robot
		private DrawGame _draw_game;

		// Action
		public Piece Select1 {get; set;}
		public Piece Select2 {get; set;}
		public RevoluteSpot SelectedSpot { get { return Select1.getConnection(Select2); } }
		private List<IAction> _actionsLog;
		private List<IAction> _redoActionsLog;
		private IAction _currentAction;

		private int _time = 0;

		public RobotEditor()
		{
			Serializer.init();
			ActionFactory.init();
			Instance = this;
			DrawPriority = 1;
			TransitionOnTime = TimeSpan.FromSeconds(0.75);
			TransitionOffTime = TimeSpan.FromSeconds(0.75);
			HasCursor = true;
			_world = null;
		}

		public static RobotEditor Instance { set; get; }

		#region IDemoScreen Members

		public string GetTitle()
		{
			return "Robot Editor";
		}

		public string GetDetails()
		{
			return ("");
		}

		#endregion

		public override void LoadContent()
		{
			base.LoadContent();

			// Menu
			new MenuRobotEditor(ScreenManager);

			ActionChooseSet.IsWheel = false;

			// Action
			_actionsLog = new List<IAction>();
			_redoActionsLog = new List<IAction>();
			_currentAction = ActionFactory.create(ActionTypes.NONE);

			// World
			if (_world == null)
				_world = new World(Vector2.Zero);
			else
				_world.Clear();

			// Loading may take a while... so prevent the game from "catching up" once we finished loading
			ScreenManager.Game.ResetElapsedTime();

			// Initialize camera controls
			_camera = new EditorCamera(ScreenManager.GraphicsDevice);
			_camera.Position = new Vector2(-500f, -100f);
			//_screenCenter = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2f, ScreenManager.GraphicsDevice.Viewport.Height / 2f);

			NamePath = "";
			_world.Gravity = new Vector2(0f, 0f);
			HasCursor = true;
			HasVirtualStick = true;

			SerializerHelper.World = _world;

			// Robot
			_draw_game = new DrawGame(ScreenManager.GraphicsDevice);
			Robot = new Robot(_world, true);
			Select1 = Robot.Heart;
			Select2 = Robot.Heart;
		}

		public void selectHeart()
		{
			Select1 = Robot.Heart;
			Select2 = Robot.Heart;
		}


		public override void Update(GameTime gameTime)
		{
			_time++;
			_camera.update();
			HandleInput();
			MenuRobotEditor.Instance.Update(Select1, Select1.getConnection(Select2));

			MenuRobotEditor.Instance.Update();

			// Permet d'update le robot sans le faire bouger (vu qu'il avance de zéro secondes dans le temps)
			_world.Step(0f);

			base.Update(gameTime);
		}

		public void fallAsleep(Piece p, SleepingPack pack)
		{
			Robot.fallAsleep(p, pack);

			if (Select1.Sleeping)
				Select1 = Robot.Heart;
			if (Select2.Sleeping)
				Select2 = Robot.Heart;
		}

		public void fallAsleep(RevoluteSpot s, SleepingPack pack)
		{
			Robot.fallAsleep(s, pack);

			if (Select1.Sleeping)
				Select1 = Robot.Heart;
			if (Select2.Sleeping)
				Select2 = Robot.Heart;
		}

		public void doAction(ActionTypes action)
		{
			if (_currentAction.type() == ActionTypes.NONE)
			{
				_currentAction = ActionFactory.create(action);
				_currentAction.init();
			}
		}

		private void HandleInput()
		{
			if (_currentAction.type() == ActionTypes.NONE)
			{
				if (MenuRobotEditor.Instance.hasFocus())
					return;
				_currentAction = ActionFactory.createFromShortcut();
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

		public ActionTypes CurrentAction
		{
			get
			{
				return _currentAction.type();
			}
		}

		private void drawRobot()
		{
			if (_currentAction.type() == ActionTypes.RESIZE_HEART
				|| _currentAction.type() == ActionTypes.MOVE_PIECE
				|| _currentAction.type() == ActionTypes.RESIZE_WHEEL)
				Select1.ColorValue = Color.GreenYellow;
			else if (Select2 == Select1)
				Select2.ColorValue = Color.Violet;
			else
			{
				Select2.ColorValue = Color.Blue;
				Select1.ColorValue = Color.Red;
			}
			if (Select1.isConnected(Select2))
				Select1.getConnection(Select2).ColorValue =
					new Color(MathLib.LoopIn(_time * 10, 255), 255, MathLib.LoopIn(_time * 10, 255));

			Robot.drawDebug(_draw_game);

			if (Select1.isConnected(Select2))
				Select1.getConnection(Select2).ColorValue = Color.Black;
			Select2.ColorValue = Color.DarkSeaGreen;
			Select1.ColorValue = Color.DarkSeaGreen;
		}

		public void drawRobotTexture()
		{
			Robot.drawDebugTexture(_draw_game);
		}

		//----------------------------NAME-------------------------------------

		private bool _hasBeenModified;
		public bool HasBeenModified
		{
			get
			{
				return _hasBeenModified;
			}
			set
			{
				if (_hasBeenModified != value)
					;//Update;
				_hasBeenModified = value;
			}
		}

		private string _prevName = "";
		private string _namePath = "";
		public string NamePath
		{
			get
			{
				return _namePath;
			}
			set
			{
				_prevName = _namePath;
				_namePath = value;
				MenuRobotEditor.Instance.updateButtonMapName();
			}
		}

		public void resetNamePath()
		{
			_namePath = _prevName;
			_prevName = "";
		}


		//-----------------------ROBOT-------------------------------------


		public Robot Robot;

		public void resetRobot(Robot bot)
		{
			if (Robot != null)
				Robot.ExtractFromWorld();
			_actionsLog.Clear();
			_redoActionsLog.Clear();
			Robot = bot;
			Select1 = Robot.Heart;
			Select2 = Robot.Heart;
		}

		public void deleteRobot()
		{
		}

		//-----------------------------------------------------------------

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			//_draw_game.Begin(_camera);
			//_draw_game.End();

			_draw_game.BeginPrimitive(_camera);
			drawRobotTexture();
			drawRobot();
			_draw_game.EndPrimitive();
			
			MenuRobotEditor.Instance.Draw();
		}

		public bool IsEmpty()
		{
			return _actionsLog.Count == 0 && _redoActionsLog.Count == 0 && Robot.Name == null;
		}
	}
}