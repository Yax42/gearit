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
			_world.Gravity = new Vector2(0f, 9.8f);
			//clearRobot();
			SerializerHelper.World = _world;
			Console.Write("One ");

			addDummyPrismatic();

			addRobot((Robot)Serializer.DeserializeItem("robot/r2d2.gir"));
			Debug.Assert(_robots != null);
			_robots[0].getPiece(Vector2.Zero).Weight = 30;
			_map = (Map)Serializer.DeserializeItem("map/moon.gim");
			Debug.Assert(_map != null);
			// Loading may take a while... so prevent the game from "catching up" once we finished loading
			ScreenManager.Game.ResetElapsedTime();

			// I have no idea what this is.
			//HasVirtualStick = true;
		}

		private void addDummyPrismatic()
		{
			Body b1 = new Body();
			//b1.Position = new Vector2(10000, 10000);
			b1.IsStatic = false;

			Body b2 = new Body();
			//b2.Position = new Vector2(10000, 10000);
			b2.IsStatic = false;

			Vector2 anchor = new Vector2(0, 0);
			Joint j = new PrismaticJoint(b1, b2, anchor, anchor, anchor);
			_world.AddBody(b1);
			_world.AddBody(b2);
			_world.AddJoint(j);
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
			Input.update();
			HandleInput();

			_world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));

			//_camera.update();
			//_camera.input();
			_camera.Update(gameTime);
			base.Update(gameTime);
		}


		private void HandleInput()
		{
			if (Input.Exit)
			{
				clearRobot();
				ScreenManager.RemoveScreen(this);
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

