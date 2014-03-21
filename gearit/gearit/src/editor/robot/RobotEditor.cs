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

namespace gearit.src.editor.robot
{
	class RobotEditor : GameScreen, IDemoScreen
	{
		private World _world;
		private EditorCamera _camera;
		private static RobotEditor instance = null;

		// Graphic
		private MenuRobotEditor _menu;
		private RectangleOverlay _background;
		private MenuPiece _menus;

		// Robot
		private DrawGame _draw_game;

		// Action
		public Piece Select1 {get; set;}
		public Piece Select2 {get; set;}
		//private ActionTypes _actionType;
		//private IAction[] _actions = new IAction[(int)ActionTypes.COUNT];
		private List<IAction> _actionsLog;
		private IAction _currentAction;
		private int _time = 0;

		public RobotEditor()
		{
			ActionFactory.init();
			instance = this;
			DrawPriority = 1;
			TransitionOnTime = TimeSpan.FromSeconds(0.75);
			TransitionOffTime = TimeSpan.FromSeconds(0.75);
			HasCursor = true;
			_world = null;
		}

		public static RobotEditor Instance
		{
			get
			{
				return instance;
			}
		}

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
			_menu = new MenuRobotEditor(ScreenManager, this);

			// Action
			_actionsLog = new List<IAction>();
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

			_world.Gravity = new Vector2(0f, 0f);
			HasCursor = true;
			HasVirtualStick = true;

			SerializerHelper.World = _world;

			// Robot
			_draw_game = new DrawGame(ScreenManager.GraphicsDevice);
			Robot = new Robot(_world);
			Select1 = Robot.getHeart();
			Select2 = Robot.getHeart();

			// Menu
			_menus = new MenuPiece(ScreenManager);
		}

		public override void Update(GameTime gameTime)
		{
			_time++;
			Robot.resetAct();
			_camera.update();
			HandleInput();

			_menu.Update(Select1, Select1.getConnection(Select2));
			_menu.Update();

			// Permet d'update le robot sans le faire bouger (vu qu'il avance de zéro secondes dans le temps)
			_world.Step(0f);

			base.Update(gameTime);
		}

		public void remove(ISpot s)
		{
			Robot.weakRemove(s);
		}

		public void remove(Piece p)
		{
			Robot.weakRemove(p);
			if (Select1 == p)
				Select1 = Robot.getHeart();
			if (Select2 == p)
				Select2 = Robot.getHeart();
		}

		/*
		private bool arunShortcut()
		{
			for (int i = 1; i < (int)ActionTypes.COUNT; i++)
				//if (_actions[i].shortcut())
				{
					//_actionType = (ActionTypes);
					return (true);
				}
			return (false);
		}
		*/

		public void doAction(ActionTypes action)
		{
			_currentAction = ActionFactory.create(action);
			_currentAction.init();
		}

		private void HandleInput()
		{
			if (_currentAction.type() == ActionTypes.NONE)
			{
				if (_menu.hasFocus())
					return;
				_currentAction = ActionFactory.createFromShortcut();
				_currentAction.init();
			}
			if (_currentAction.run() == false)
			{
				if (_currentAction.canBeReverted())
					_actionsLog.Insert(0, _currentAction);
				_currentAction = ActionFactory.Dummy;
			}
			_camera.input();
		}

		public void undo()
		{
			IAction a = _actionsLog.ElementAt(0);
			a.revert();
			_actionsLog.Remove(a);
		}

		public void clearActionLog()
		{
			_actionsLog.Clear();
		}

		private void drawRobot()
		{
			if (_currentAction.type() == ActionTypes.RESIZE_PIECE && Select1 == Robot.getHeart())
				Select1.ColorValue = Color.GreenYellow;
			else if (Select2 == Select1)
				Select2.ColorValue = Color.Violet;
			else
			{
				Select2.ColorValue = Color.Blue;
				Select1.ColorValue = Color.Red;
			}
			if (Select1.isConnected(Select2))
				Select1.getConnection(Select2).ColorValue = new Color(255, (_time * 10) % 255, (_time * 10) % 255);

			Robot.drawDebug(_draw_game);

			if (Select1.isConnected(Select2))
				Select1.getConnection(Select2).ColorValue = Color.Black;
			Select2.ColorValue = Color.Black;
			Select1.ColorValue = Color.Black;
		}

		public Robot Robot
		{
			get;
			set;
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			_draw_game.Begin(_camera);
			drawRobot();
			_draw_game.End();

			_menu.Draw();
		}
	}
}