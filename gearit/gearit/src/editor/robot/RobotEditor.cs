﻿using System;
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
    /// <summary>
    /// Handle the whole Robot editor
    /// </summary>
	class RobotEditor : GameScreen, IDemoScreen
	{
		public World World { get { return _world; } }
		private World _world;
		private EditorCamera _camera;

		// Graphic
		private RectangleOverlay _background;

		// Robot
		private DrawGame DrawGame;

		// Action
		private Piece _Select1;
		public Piece Select1
		{
			get
			{
				if (MirrorAxis.Active)
					return MirrorSelect1;
				else
					return _Select1;
			}
			set
			{
				if (MirrorAxis.Active)
					MirrorSelect1 = value;
				else
					_Select1 = value;
			}
		}

		private Piece _Select2;
		public Piece Select2
		{
			get
			{
				if (MirrorAxis.Active)
					return MirrorSelect2;
				else
					return _Select2;
			}
			set
			{
				if (MirrorAxis.Active)
					MirrorSelect2 = value;
				else
					_Select2 = value;
			}
		}
		private Piece MirrorSelect1 {get; set;}
		private Piece MirrorSelect2 {get; set;}
		public RevoluteSpot SelectedSpot { get { return Select1.getConnection(Select2); } }
		private List<IAction> _actionsLog;
		private List<IAction> _redoActionsLog;
		private IAction _currentAction;
		private IAction _mirrorAction;

		private int _time = 0;

		public RobotEditor() : base(false)
		{
			Serializer.init();
			ActionFactory.init();
			Instance = this;
			DrawPriority = 1;
			TransitionOnTime = TimeSpan.FromSeconds(0.75);
			TransitionOffTime = TimeSpan.FromSeconds(0.75);
			HasCursor = true;
			_world = null;
			VisibleMenu = false;
		}

		public static RobotEditor Instance { set; get; }

		#region IDemoScreen Members

		public string GetTitle()
		{
			return "Robot";
		}

		public string GetDetails()
		{
			return ("");
		}

		#endregion

		public override void LoadContent()
		{
			base.LoadContent();

			// Action
			_actionsLog = new List<IAction>();
			_redoActionsLog = new List<IAction>();
			_currentAction = ActionFactory.create(ActionTypes.NONE);
			_mirrorAction = ActionFactory.create(ActionTypes.NONE);

			// World
			if (_world == null)
				_world = new World(Vector2.Zero);
			else
				_world.Clear();

			// Loading may take a while... so prevent the game from "catching up" once we finished loading
			ScreenManager.Game.ResetElapsedTime();

			// Initialize camera controls
			_camera = new EditorCamera(ScreenManager.GraphicsDevice.Viewport);
			_camera.Position = new Vector2(-500f, -100f);
			//_screenCenter = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2f, ScreenManager.GraphicsDevice.Viewport.Height / 2f);

			_world.Gravity = new Vector2(0f, 0f);
			HasCursor = true;
			HasVirtualStick = true;

			SerializerHelper.World = _world;

			// Robot
			DrawGame = DrawGame.Instance;
			_Robot = new Robot(_world, true);

			selectHeart();

			// Menu : cette partie requiert le Robot d'etre charge
			new MenuRobotEditor(ScreenManager);
			ActionChooseSet.IsWheel = false;

			// Cette partie requiert le Menu d'etre charge
			ActionLoadRobot.LoadLua();
			LuaScript = new GI_File(RobotEditor.Instance.Robot.LuaFullPath);
		}

		public override void positionChanged(int x, int y)
		{
			MenuRobotEditor.Instance.positionChanged(x, y);
		}

		public override Squid.Point getMenuSize()
		{
			return (MenuRobotEditor.Instance.getMenuSize());
		}

		public override Squid.Point getMenuPosition()
		{
			return (MenuRobotEditor.Instance.getMenuPosition());
		}

		public override void QuickLoadContent()
		{
			MenuRobotEditor.Instance.setFocus(false);
		}

		public void selectHeart()
		{
			Select1 = Robot.Heart;
			Select2 = Robot.Heart;
			MirrorSelect1 = Robot.Heart;
			MirrorSelect2 = Robot.Heart;
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
			if (LuaScript.HasBeenModified)
				ActionLoadRobot.LoadLua();

			base.Update(gameTime);
		}

		public void fallAsleep(Piece p, SleepingPack pack)
		{
			Robot.fallAsleep(p, pack);
			AsleepSelections();
		}

		public void fallAsleep(RevoluteSpot s, SleepingPack pack)
		{
			Robot.fallAsleep(s, pack);
			AsleepSelections();
		}

		private void AsleepSelections()
		{
			if (Select1.Sleeping)
				Select1 = Robot.Heart;
			if (Select2.Sleeping)
				Select2 = Robot.Heart;
			if (MirrorSelect1.Sleeping)
				MirrorSelect1 = Robot.Heart;
			if (MirrorSelect2.Sleeping)
				MirrorSelect2 = Robot.Heart;
		}

		public void doAction(ActionTypes action = ActionTypes.NONE)
		{
			if (_currentAction.Type() == ActionTypes.NONE)
			{
				if (action == ActionTypes.NONE)
					_currentAction = ActionFactory.createFromShortcut();
				else
					_currentAction = ActionFactory.create(action);
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
				if (MenuRobotEditor.Instance.hasFocus())
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

		public ActionTypes CurrentAction
		{
			get
			{
				return _currentAction.Type();
			}
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
		public string NamePath
		{
			get
			{
				return Robot.FullPath;
			}
			set
			{
				_prevName = Robot.FullPath;
				Robot.FullPath = value;
				MenuRobotEditor.Instance.updateButtonRobotName();
			}
		}
		public string ActualPath { get { return Robot.FullPath; } }

		public void resetNamePath()
		{
			Robot.FullPath = _prevName;
			_prevName = "";
		}


		//-----------------------ROBOT-------------------------------------


		private Robot _Robot;
		public Robot Robot
		{
			get
			{
				return _Robot;
			}
			set
			{
				_Robot = value;
				LuaScript = new GI_File(RobotEditor.Instance.Robot.LuaFullPath);
			}
		}

		public void resetRobot(Robot bot)
		{
			if (Robot != null)
				Robot.ExtractFromWorld();
			_actionsLog.Clear();
			_redoActionsLog.Clear();
			Robot = bot;
			selectHeart();
		}

		public void deleteRobot()
		{
		}

		private GI_File LuaScript;

		//-----------------------------------------------------------------

		private void DrawRobot()
		{
			if (ActionLaunch.Running)
			{
				ActionLaunch.Robot.draw(DrawGame);
			}
			else if (Input.pressed(Keys.T))
			{
				Robot.draw(DrawGame);
			}
			else
			{
				Color c1 = Select1.Color;
				Color c2 = Select2.Color;
				if (_currentAction.Type() == ActionTypes.RESIZE_HEART)
					Select1.Color = Color.White;
				else if (Select2 == Select1)
					Select2.Color = Color.Violet;
				else
				{
					Select2.Color = Color.Blue;
					Select1.Color = Color.Red;
				}
				if (Select1.isConnected(Select2))
					Select1.getConnection(Select2).ColorValue =
						new Color(MathLib.LoopIn(_time * 10, 255), 255, MathLib.LoopIn(_time * 10, 255));

				Robot.drawDebug(DrawGame);

				if (Select1.isConnected(Select2))
					Select1.getConnection(Select2).ColorValue = Color.Black;
				Select2.Color = c2;
				Select1.Color = c1;
			}
		}

		private void DrawMarks()
		{
			Color col = Color.Red;
			col.A = 160;
			DrawGame.DrawLine(LockAxis.Origin - 1000 * LockAxis.Dir,
							LockAxis.Origin + 1000 * LockAxis.Dir,
							col);
			col = Color.White;
			col.A = 176;
			DrawGame.DrawLine(MirrorAxis.Origin - 1000 * MirrorAxis.Dir,
							MirrorAxis.Origin + 1000 * MirrorAxis.Dir,
							col);
		}

		private int SaveCount = 0;
		public void DrawStatics()
		{
			const int cycle = 80;
			DrawGame.Begin();
			if (ActionSaveRobot.JustSaved)
			{
				SaveCount++;
				DrawGame.DrawSpirale(new Vector2(ScreenManager.Width - 20, ScreenManager.Height - 20),
							cycle, SaveCount, 5);
				if (SaveCount >= cycle)
				{
					SaveCount = 0;
					ActionSaveRobot.JustSaved = false;
				}
			}
			DrawGame.End();
		}

		public void DrawRobotTexture()
		{
			Robot.drawDebugTexture(DrawGame);
		}

		private void DrawValues(IAction action)
		{
			if (action.Type() == ActionTypes.CHANGE_VALUES)
			{
				var act = (ActionChangeValues)action;
				int power = act.Power + 1;
				Vector2 ori = act.Origin;
				int unit = act.Unit;
				float delta = (float) (2f * Math.PI) / 10f;
				for (int i = 0; i < 10; i++)
				{
					var pos = ori + 0.5f * new Vector2((float)Math.Cos(i * delta),
						(float)Math.Sin(i * delta));
					DrawGame.DrawCircle(pos, (i == unit ? power * 0.05f : 0.05f),
						(i == unit) ? new Color(0.5f, 0.2f + power / 10f, power / 20f, 1) : Color.Brown , true);
				}
			}
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
			ScreenManager.GraphicsDevice.Clear(new Color(1f, 0.7f, 0.3f, 1f));

			//DrawGame.Begin(_camera);
			//DrawGame.End();

			DrawGame.BeginPrimitive(_camera);
			DrawGame.DrawGrille(new Color(0, 0, 0, 0.1f));
			//DrawRobotTexture();
			MirrorAxis.Active = Input.Alt;
			DrawRobot();
			MirrorAxis.Active = false;
			DrawValues(_currentAction);
			DrawValues(_mirrorAction);
			DrawMarks();
			DrawPickColor();
			DrawGame.EndPrimitive();

			DrawStatics();
			
			MenuRobotEditor.Instance.Draw();
		}

		public bool IsEmpty()
		{
			return _actionsLog.Count == 0 && _redoActionsLog.Count == 0 && Robot.Name == null;
		}
	}
}