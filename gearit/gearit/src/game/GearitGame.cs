using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using gearit.src.utility;
using FarseerPhysics.Dynamics;
using gearit.src.map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using gearit.src.editor;
using System.Diagnostics;
using FarseerPhysics.Dynamics.Joints;
using gearit.src.output;
using GUI;

namespace gearit.src.game
{
	class GearitGame : GameScreen, IDemoScreen
	{
		private World _world;
		//private Camera2D _camera;
		private Camera2D _camera;

		private Map _map;
		private List<Robot> _robots;
		private DrawGame _drawGame;

		// Graphic
		private RectangleOverlay _background;

		// Action
	private int _time = 0;

		#region IDemoScreen Members

		public GearitGame()
		{
			TransitionOnTime = TimeSpan.FromSeconds(0.75);
			TransitionOffTime = TimeSpan.FromSeconds(0.75);
			HasCursor = true;
			_robots = new List<Robot>();
			_world = new World(new Vector2(0, 9.8f));
		}

		public string GetTitle()
		{
			return "Game";
		}

		public string GetDetails()
		{
			return ("");
		}

		#endregion

		public override void LoadContent()
		{
			base.LoadContent();
			_time = 0;
			_drawGame = new DrawGame(ScreenManager.GraphicsDevice);
			_camera = new Camera2D(ScreenManager.GraphicsDevice);
			_world.Clear();
			_world.Gravity = new Vector2(0f, 19.8f);
			//clearRobot();
			SerializerHelper.World = _world;

			addRobot((Robot)Serializer.DeserializeItem("robot/default.gir"));
			Debug.Assert(_robots != null);
			_map = (Map)Serializer.DeserializeItem("map/default.gim");
			Debug.Assert(_map != null);
			// Loading may take a while... so prevent the game from "catching up" once we finished loading
			ScreenManager.Game.ResetElapsedTime();

			// I have no idea what this is.
			//HasVirtualStick = true;
		}


		public World getWorld()
		{
			return (_world);
		}

		public void setMap(Map map)
		{
			_map = map;
		}

		public void clearRobot()
		{
			foreach (Robot r in _robots)
				r.remove();
			_robots.Clear();
		}

		public void addRobot(Robot robot)
		{
			_robots.Add(robot);
			if (_robots.Count == 1)
				_camera.TrackingBody = robot.getHeart();
			robot.turnOn();
		robot.move(new Vector2(0, -20));
		}

		public override void Update(GameTime gameTime)
		{
			_time++;
			HandleInput();

			_world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));

			//_camera.update();
			_camera.Update(gameTime);

			PrintRobot(gameTime);
		}

		private float SecCount = 0;
		public void PrintRobot(GameTime gameTime)
		{
			SecCount += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (SecCount < 2)
				return;

			foreach (Robot r in _robots)
				foreach (Piece p in r.Pieces)
					p.ResetMassData();
			SecCount = 0;
#if false
			foreach (Robot r in _robots)
			{
				string res = "W:";
				foreach (Piece p in r.Pieces)
					res += " " + p.Mass + "/" + p.Weight;
				OutputManager.LogInfo(res);

				res = "F:";
				foreach (ISpot s in r.Spots)
					res += " " + s.Force;
				OutputManager.LogInfo(res);
			}
#endif
		}


		private void HandleInput()
		{
			if (Input.Exit)
			{
				clearRobot();
				//ScreenManager.RemoveScreen(this);
				ScreenMainMenu.GoBack = true;
			}
			if (Input.justPressed(MouseKeys.WHEEL_DOWN))
				_camera.zoomIn();
			if (Input.justPressed(MouseKeys.WHEEL_UP))
				_camera.zoomOut();
		}

		public override void Draw(GameTime gameTime)
		{
			ScreenManager.GraphicsDevice.Clear(Color.LightYellow);
			_drawGame.Begin(_camera);

			foreach (Robot r in _robots)
				r.draw(_drawGame);
			_map.drawDebug(_drawGame);
			_drawGame.End();

			base.Draw(gameTime);
		}
	}
}

