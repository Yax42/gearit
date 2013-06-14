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

namespace gearit.src.utility
{
    internal class SpiderBot : PhysicsGameScreen, IDemoScreen
    {

        private class Legg
        {
            private Body _wheel;
            private Body _spot;
            private PrismaticJoint _pris;
            private RevoluteJoint _rev;
            private static int _nb = 0;
            private int _id;
            private int _nbLeggs;

            public Legg(World world, Body heart, int nbLeggs)
            {
                _nbLeggs = nbLeggs;
                _id = ++_nb;
                _spot = new Body(world);
                CircleShape circleShape1 = new CircleShape(0.1f, 4f);
                (_spot.CreateFixture(circleShape1)).CollisionGroup = 42;
                _spot.BodyType = BodyType.Dynamic;
                _spot.FixtureList[0].Shape.Density = 2f;
                _spot.ResetMassData();

		if (_id <= nbLeggs / 2)
                  _pris = new PrismaticJoint(heart, _spot, Vector2.Zero, Vector2.Zero,
		    new Vector2(((_id % (nbLeggs / 2)) / (float) (nbLeggs / 2)), (1f - (_id % (nbLeggs / 2)) / (float) (nbLeggs / 2))));
		else
                  _pris = new PrismaticJoint(heart, _spot, Vector2.Zero, Vector2.Zero,
		    new Vector2(-((_id % (nbLeggs / 2)) / (float) (nbLeggs / 2)), -(1f - (_id % (nbLeggs / 2)) / (float) (nbLeggs / 2))));

		//new Vector2(((_rand.Next() % 2000) / 1000.0f - 1000),((_rand.Next() % 2000) / 1000.0f - 1000)));

                _pris.LimitEnabled = true;
                _pris.Enabled = true;
                _pris.LowerLimit = 1f;
                _pris.UpperLimit = 4f;
                _pris.MotorEnabled = true;
                _pris.MaxMotorForce = 500f;
                _pris.MotorSpeed = 0f;
                world.AddJoint(_pris);

                _wheel = BodyFactory.CreateCircle(world, 0.5f, 1f, Vector2.Zero);
                _wheel.FixtureList[0].Shape.Density = 2f;
                _wheel.ResetMassData();
                _wheel.BodyType = BodyType.Dynamic;
                _wheel.CollisionGroup = 42;
                _wheel.Position = new Vector2(13, 4);

                _rev = new RevoluteJoint(_wheel, _spot, Vector2.Zero, Vector2.Zero);
                _rev.Enabled = true;
                _rev.MotorEnabled = true;
                _rev.MaxMotorTorque = 100f;
                _rev.MotorSpeed = 0f;
                world.AddJoint(_rev);
            }
            public void setRotation(float v)
            {
   //             if (_id % (_nbLeggs / 2) > _nbLeggs / 4)
    //                v = -v;
                _rev.MotorSpeed = v;
            }

            public void setTranslation(float v)
            {
      //          if (_id % (_nbLeggs / 2) > _nbLeggs / 4)
       //             v = -v;
                _pris.MotorForce = v;
            }

            public Body getBody()
            {
                return (_wheel);
            }
        }

        // XNA
        // Window
        // Draw on window
        private SpriteBatch             _batch;
        // Texture & Sprite
        // Camera
        private Matrix                  _view;
        private Vector2                 _camera_position;
        private Vector2                 _screen_center;
        private DebugViewXNA		_debug;

        // FARSEER
        // Farseer expects objects to be scaled to MKS (meters, kilos, seconds)
        // 1 meters equals 64 pixels here
        // (Objects should be scaled to be between 0.1 and 10 meters in size)
        // World
        // Environnement
        private Body                    _ground;
        private Body                    _ground_up_right;
        private Body                    _ground_up_left;
        private Body                    _ball;

        // Robot
        private Body                    _heart;
        private Legg[] _leggs;
        private int _nbLeggs;

        // Utility
        private AssetCreator            _asset;

        public string GetTitle()
        {
            return "SpiderBot";
        }

        public string GetDetails()
        {
	    /*
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("TODO: Add sample description!");
            sb.AppendLine(string.Empty);
            sb.AppendLine("GamePad:");
            sb.AppendLine("  - Exit to menu: Back button");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Keyboard:");
            sb.AppendLine("  - Exit to menu: Escape");
	    */
            return (string.Empty);
        }

        // LoadContent will be called once per game and is the place to load all of your content.
        public override void LoadContent()
        {
            base.LoadContent();

            #region init
            World.Gravity = new Vector2(0f, 9.8f);
            // Setting the root path for content (sprite/font..)
            //Content.RootDirectory = "Content";

            _debug = new DebugViewXNA(World);
            _debug.AppendFlags(DebugViewFlags.DebugPanel);
            _debug.DefaultShapeColor = Color.White;
            _debug.SleepingShapeColor = Color.LightGray;
            _debug.LoadContent(ScreenManager.Game.GraphicsDevice, ScreenManager.Game.Content);
            // Initialize camera controls
            _view = Matrix.Identity;
            _camera_position = Vector2.Zero;
           // _screen_center = new Vector2(ConvertUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Width), ConvertUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Height)) / 2f;
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
            //_ground.SetTransform(_ground.Position, 1.5f);
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

            // Pyramid
            //_pyramid = new Pyramid(World, new Vector2(9.3f, 0f), 8, 1f, _asset);
            #endregion

            /***************************************/
            /***************** ROBOT ***************/
            /***************************************/
            // Heart

            Vertices vertices = new Vertices(8);
            vertices.Add(new Vector2(-0.5f, -0.5f));
            vertices.Add(new Vector2(0.5f, -0.5f));
            vertices.Add(new Vector2(0.5f, 0.5f));
            vertices.Add(new Vector2(-0.5f, 0.5f));

            _heart = new Body(World);
            _heart.BodyType = BodyType.Dynamic;
            _heart.CreateFixture(new PolygonShape(vertices, 20f));
            _heart.FixtureList[0].Shape.Density = 200f;
            _heart.ResetMassData();
            _heart.FixtureList[0].CollisionGroup = 42;
            _heart.SetTransform(new Vector2(14, 0), 3.1415926f);

            /***********aaa************************************************************************************************/
            _nbLeggs = 50;
            /***********************************************************************************************************/
            _leggs = new Legg[_nbLeggs];
            for (int i = 0; i < _nbLeggs; i++)
                _leggs[i] = new Legg(World, _heart, _nbLeggs);
        }

        // Allows the game to run logic such as updating the world,
        // checking for collisions, gathering input, and playing audio.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            HandleKeyboard();

            // Update the world
            World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        // Manage the keyboard.
        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();


            for (int i = 0; i < _nbLeggs; i++)
            {
                if (state.IsKeyDown(Keys.W))
                    _leggs[i].setTranslation(400f);
                else if (state.IsKeyDown(Keys.S))
                    _leggs[i].setTranslation(-400f);
                else
                    _leggs[i].setTranslation(0f);

                if (state.IsKeyDown(Keys.A))
                    _leggs[i].setRotation(30f);
                else if (state.IsKeyDown(Keys.D))

                    _leggs[i].setRotation(-20f);
                else
                    _leggs[i].setRotation(0f);
            }

            if (state.IsKeyDown(Keys.Space))
            {
            }
        }

        // This is called when the game should draw itself.
        public override void Draw(GameTime gameTime)
        {
            // Erase & Draw background
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Drawing
            _batch.Begin();
        Matrix projection = Matrix.CreateOrthographicOffCenter(0f, ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Width),
                                             ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Height), 0f, 0f,
                                             1f);
        Matrix view = Matrix.CreateTranslation(new Vector3((ConvertUnits.ToSimUnits(_camera_position) -
    ConvertUnits.ToSimUnits(_screen_center)), 0f)) * Matrix.CreateTranslation(new Vector3(ConvertUnits.ToSimUnits(_screen_center), 0f));
        _debug.RenderDebugData(ref projection, ref view);

            //_pyramid.Draw(_batch);

            _batch.End();
        }
    }
}
