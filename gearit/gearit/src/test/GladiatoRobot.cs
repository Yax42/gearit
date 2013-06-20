using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;
using FarseerPhysics.DebugViews;
using gearit.xna;
using gearit.src.robot;

namespace gearit.src.utility
{

    internal class GladiatoRobot : PhysicsGameScreen, IDemoScreen
    {
        private LuaTest _lua;
        private SpriteBatch _batch;
        private Matrix _view;
        private Vector2 _camera_position;
        private Vector2 _screen_center;
        private DebugViewXNA _debug;
        private DrawGame _drawGame;

        private Body _ground;
        private Body _ground_up_right;
        private Body _ground_up_left;
        private Body _ball;

        // Robot
        private Robot _robot;

        // Utility
        private AssetCreator _asset;

        public string GetTitle()
        {
            return "GladiatorRobot";
        }

        public string GetDetails()
        {
            return (string.Empty);
        }

        // LoadContent will be called once per game and is the place to load all of your content.
        public override void LoadContent()
        {
            base.LoadContent();
            _drawGame = new DrawGame(ScreenManager.GraphicsDevice);
            World.Gravity = new Vector2(0f, 9.8f);

            _debug = new DebugViewXNA(World);
            _debug.AppendFlags(DebugViewFlags.DebugPanel);
            _debug.DefaultShapeColor = Color.White;
            _debug.SleepingShapeColor = Color.LightGray;
            _debug.LoadContent(ScreenManager.Game.GraphicsDevice, ScreenManager.Game.Content);
            _view = Matrix.Identity;
            _camera_position = new Vector2(300, 300);
            _screen_center = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2f,
                                                ScreenManager.GraphicsDevice.Viewport.Height / 2f);


            // Link painter to window 
            _batch = new SpriteBatch(ScreenManager.GraphicsDevice);

            // Utility
            _asset = new AssetCreator(ScreenManager.Game.GraphicsDevice);
            _asset.LoadContent(ScreenManager.Game.Content);

            // Initialize grounds
            Vector2 wall_position = (ConvertUnits.ToSimUnits(_screen_center)) + new Vector2(-5f, 1.25f);
            _ground = BodyFactory.CreateRectangle(World, 8f, 0.5f, 1f, wall_position);
            _ground.BodyType = BodyType.Static;

            // Up grounds
            wall_position -= new Vector2(7f, 2f);
            _ground_up_left = BodyFactory.CreateRectangle(World, 8f, 0.5f, 1f, wall_position);
            //_ground_up_left.SetTransform(wall_position, 1.5f);
            _ground.BodyType = BodyType.Static;
            wall_position += new Vector2(14f, 1f);
            _ground_up_right = BodyFactory.CreateRectangle(World, 8f, 0.5f, 1f, wall_position);
            //_ground_up_right.SetTransform(wall_position, 1.5f);
            _ground.BodyType = BodyType.Static;

            // Ball
            _ball = BodyFactory.CreateCircle(World, 0.5f, 1f, _screen_center);
            _ball.BodyType = BodyType.Dynamic;
            _ball.SetTransform(_screen_center, 1f);

            /***************************************/
            /***************** ROBOT ***************/
            /***************************************/
            _robot = new Robot(World);
	    /*
	    Piece wheel1 = new Wheel(_robot, 1f);
	    Piece dot1 = new Wheel(_robot, 0.2f);
            new PrismaticSpot(_robot, _robot.getHeart(), dot1);//.moveAnchor(_robot.getHeart(), new Vector2(-0.5f, -0.5f));
            new RevoluteSpot(_robot, wheel1, dot1);
	    dot1.move(new Vector2(1, 1));
	    */
            
	    Piece wheel2 = new Wheel(_robot, 1f, Vector2.Zero);
	    Piece dot2 = new Wheel(_robot, 0.2f, Vector2.Zero);
            dot2.move(new Vector2(1, -1));
            new PrismaticSpot(_robot, _robot.getHeart(), dot2);//).moveAnchor(_robot.getHeart(), new Vector2(0.5f, -0.5f));
            new RevoluteSpot(_robot, wheel2, dot2);
	    
            _lua = new LuaTest(_robot, "bruterobot");
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            HandleKeyboard();
            
            _lua.execFile();
            
            World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Space))
            {
            }
        }

        // This is called when the game should draw itself.
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Drawing
            _drawGame.Begin(ScreenManager.GraphicsDevice.Viewport, _camera_position, _screen_center);

            _robot.drawDebug(_drawGame);

            _drawGame.End();

            //_batch.End();
        }
    }
}