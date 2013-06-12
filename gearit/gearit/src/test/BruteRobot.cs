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

namespace gearit.src.test
{
    public class BruteRobot : Game
    {
        // XNA
        // Window
        private GraphicsDeviceManager   _graphics;
        // Draw on window
        private SpriteBatch             _batch;
        // Texture & Sprite
        private Texture2D               _ground_tex;
        private Texture2D               _box_text;
        private Texture2D               _ball_tex;
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
        private World                   _world;
        // Environnement
        private Body                    _ground;
        private Body                    _ground_up_right;
        private Body                    _ground_up_left;
        private Pyramid                 _pyramid;
        private Body                    _ball;

        // Robot
        private Body                    _heart;
        private Body                    _wheel_left;
        private Body                    _wheel_right;
        private Texture2D               _wheel_right_tex;
        private Texture2D               _wheel_left_tex;
        private Texture2D               _heart_tex;
	private Body[]			_joints = new Body[4];
        private Body			_rod1Start;
        private Body			_rod1End;
        private Body			_rod2;
        private Body			_rod2Start;
        private Body			_rod2End;
        private Body			_rod1;

	private PrismaticJoint		_jointRod1;
	private PrismaticJoint		_jointRod2;
	private RevoluteJoint[]		_spotJoint = new RevoluteJoint[8];
        private float			_sizeRod1 = 1f;
        private float		        _sizeRod2 = 1f;
        private bool			_keyP = false;



        // Utility
        private AssetCreator            _asset;

        // Constructor.
        public BruteRobot()
        {
            // Window properties
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 768;
            
            // Creating the worl for farseer with his gravity
            _world = new World(new Vector2(0, 20));
        }

        // LoadContent will be called once per game and is the place to load all of your content.
        protected override void LoadContent()
        {
            #region init
            // Setting the root path for content (sprite/font..)
            Content.RootDirectory = "Content";

 	    _debug = new DebugViewXNA(_world);
            _debug.AppendFlags(DebugViewFlags.DebugPanel);
            _debug.DefaultShapeColor = Color.White;
            _debug.SleepingShapeColor = Color.LightGray;
            _debug.LoadContent(GraphicsDevice, Content);
            // Initialize camera controls
            _view = Matrix.Identity;
            _camera_position = Vector2.Zero;
            _screen_center = new Vector2(ConvertUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Width), ConvertUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Height)) / 2f;

            // Link painter to window 
            _batch = new SpriteBatch(_graphics.GraphicsDevice);

            // Utility
            _asset = new AssetCreator(GraphicsDevice);
            
            _asset.LoadContent(Content);

            // Initialize grounds
            Vector2 wall_position = new Vector2(_screen_center.X, _screen_center.Y + 2f);
            _ground = BodyFactory.CreateRectangle(_world, 8f, 0.5f, 1f, wall_position);
            _ground_tex = _asset.TextureFromShape(_ground.FixtureList[0].Shape, MaterialType.Blank, Color.LightGreen, 1f);
            _ground.BodyType = BodyType.Static;
            //_ground.SetTransform(_ground.Position, 1.5f);
            // Up grounds
            wall_position -= new Vector2(7f, 2f);
            _ground_up_left = BodyFactory.CreateRectangle(_world, 8f, 0.5f, 1f, wall_position);
            //_ground_up_left.SetTransform(wall_position, 1.5f);
            _ground.BodyType = BodyType.Static;
            wall_position += new Vector2(14f, 1f);
            _ground_up_right = BodyFactory.CreateRectangle(_world, 8f, 0.5f, 1f, wall_position);
            //_ground_up_right.SetTransform(wall_position, 1.5f);
            _ground.BodyType = BodyType.Static;

            // Ball
            _ball = BodyFactory.CreateCircle(_world, 0.5f, 1f, _screen_center);
            _ball.BodyType = BodyType.Dynamic;
            _ball_tex = _asset.TextureFromShape(_ball.FixtureList[0].Shape, MaterialType.Blank, Color.LightGray, 1f);
            _ball.SetTransform(_screen_center, 1f);

            // Pyramid
            _pyramid = new Pyramid(_world, new Vector2(0.3f, 0f), 5, 1f, _asset);

            // Robot

            //_pyramid = new Pyramid(_world, new Vector2(9.3f, 0f), 8, 1f, _asset);
            #endregion

            /***************************************/
/***************** ROBOT ***************/
/***************************************/
	    // Heart

            Vertices vertices = new Vertices(8);
            vertices.Add(new Vector2(-0.5f, -1f));
            vertices.Add(new Vector2(0.5f, -1f));
            vertices.Add(new Vector2(0.5f, 0.5f));
            vertices.Add(new Vector2(-0.5f, 0.5f));
	    /*
            Vertices vertices = new Vertices(8);
            vertices.Add(new Vector2(-1.5f, -0.5f));
            vertices.Add(new Vector2(1.5f, -0.5f));
            vertices.Add(new Vector2(1.5f, 0.0f));
            vertices.Add(new Vector2(0.0f, 0.9f));
            vertices.Add(new Vector2(-1.15f, 0.9f));
            vertices.Add(new Vector2(-1.5f, 0.2f));
	    */

            _heart = new Body(_world);
            _heart.BodyType = BodyType.Dynamic;
            _heart.CreateFixture(new PolygonShape(vertices, 20f));
            _heart.FixtureList[0].Shape.Density = 3f;
            _heart.ResetMassData();
            _heart.FixtureList[0].CollisionGroup = 42;
            _heart_tex = _asset.TextureFromVertices(vertices, MaterialType.Blank, Color.Blue * 0.8f, 1f);
            _heart.SetTransform(new Vector2(14, 0), 3.1415926f);

	    //Rod
	    /*
	    _rod1Start = BodyFactory.CreateCircle(_world, 0.1f, 1f, Vector2.Zero);
	    _rod1Start.BodyType = BodyType.Dynamic;
            _rod1Start.CollisionGroup = 42;
	    _rod2Start = BodyFactory.CreateCircle(_world, 0.1f, 1f, Vector2.Zero);
	    _rod2Start.BodyType = BodyType.Dynamic;
            _rod2Start.CollisionGroup = 42;
	    */
        //public static Body CreateEdge(World world, Vector2 start, Vector2 end)
	    /*
	    _rod1End = BodyFactory.CreateCircle(_world, 0.1f, 1f, Vector2.Zero);
	    */
	    //_rod2End = BodyFactory.CreateCircle(_world, 0.5f, 0.1f, Vector2.Zero);

	    /*
            _rod1End = new Body(_world);
            CircleShape circleShape1 = new CircleShape(0.1f, 1f);
            (_rod1End.CreateFixture(circleShape1)).CollisionGroup = 42;
            _rod1End.BodyType = BodyType.Dynamic;

            _rod2End = new Body(_world);
            CircleShape circleShape2 = new CircleShape(0.1f, 1f);
            (_rod2End.CreateFixture(circleShape2)).CollisionGroup = 42;
            _rod2End.BodyType = BodyType.Dynamic;
	    */


	    _rod1End = BodyFactory.CreateCircle(_world, 0.1f, 1f, Vector2.Zero);
            _rod1End.CollisionGroup = 42;
            _rod1End.BodyType = BodyType.Dynamic;

	    _rod2End = BodyFactory.CreateCircle(_world, 0.1f, 1f, Vector2.Zero);
            _rod2End.CollisionGroup = 42;
            _rod2End.BodyType = BodyType.Dynamic;

	    _rod1Start = BodyFactory.CreateCircle(_world, 0.1f, 1f, Vector2.Zero);
            _rod1Start.CollisionGroup = 42;
            _rod1Start.BodyType = BodyType.Dynamic;

	    _rod2Start = BodyFactory.CreateCircle(_world, 0.1f, 1f, Vector2.Zero);
            _rod2Start.CollisionGroup = 42;
            _rod2Start.BodyType = BodyType.Dynamic;

	    /*
	    _rod1End = BodyFactory.CreateCircle(_world, 0.5f, 0.1f, Vector2.Zero);
            _rod1End.BodyType = BodyType.Dynamic;
            _rod1End.CollisionGroup = 42;
             */

	    //RodSpot
            _jointRod1 = new PrismaticJoint(_heart, _rod1End, Vector2.Zero, new Vector2(0, 0), new Vector2(-1, 1));
            _jointRod1.LimitEnabled = true;
            _jointRod1.Enabled = true;
            _jointRod1.LowerLimit = 1;
            _jointRod1.UpperLimit = _sizeRod1 * 3;
            _jointRod1.MotorEnabled = true;
            _jointRod1.MaxMotorForce = 100;
            _jointRod1.MotorSpeed = 0f;
            _world.AddJoint(_jointRod1);

            _jointRod2 = new PrismaticJoint(_heart, _rod2End, Vector2.Zero, new Vector2(0, 0), new Vector2(1, 1));
            _jointRod2.LimitEnabled = true;
            _jointRod2.Enabled = true;
            _jointRod2.LowerLimit = 1;
            _jointRod2.UpperLimit = _sizeRod1 * 3;
            _jointRod2.MotorEnabled = true;
            _jointRod2.MaxMotorForce = 100;
            _jointRod2.MotorSpeed = 0f;
            _world.AddJoint(_jointRod2);


	    // Wheel
            _wheel_right = BodyFactory.CreateCircle(_world, 0.5f, 1f, Vector2.Zero);
            _wheel_right.FixtureList[0].Shape.Density = 1f;
            _wheel_right.ResetMassData();
            _wheel_right.BodyType = BodyType.Dynamic;
            _wheel_right_tex = _asset.TextureFromShape(_wheel_right.FixtureList[0].Shape, MaterialType.Blank, Color.Gray, 1f);
            _wheel_right.CollisionGroup = 42;
            _wheel_right.Position = new Vector2(13, 4);

            _wheel_left = BodyFactory.CreateCircle(_world, 0.5f, 1f, Vector2.Zero);
            _wheel_left.FixtureList[0].Shape.Density = 1f;
            _wheel_left.ResetMassData();
            _wheel_left.BodyType = BodyType.Dynamic;
            _wheel_left_tex = _asset.TextureFromShape(_wheel_left.FixtureList[0].Shape, MaterialType.Blank, Color.LightGray, 1f);
            _wheel_left.FixtureList[0].CollisionGroup = 42;
            _wheel_left.Position = new Vector2(12, 4);

            //Joint test
            _spotJoint[1] = new RevoluteJoint(_wheel_right, _rod1End, Vector2.Zero, Vector2.Zero);
            _world.AddJoint(_spotJoint[1]);
	    _spotJoint[1].Enabled = true;
            _spotJoint[1].MotorEnabled = true;
            _spotJoint[1].MaxMotorTorque = 1000f;
            _spotJoint[1].MotorSpeed = 0f;

            _spotJoint[0] = new RevoluteJoint(_wheel_left, _rod2End, Vector2.Zero, Vector2.Zero);
            _world.AddJoint(_spotJoint[0]);
	    _spotJoint[0].Enabled = true;
            _spotJoint[0].MotorEnabled = true;
            _spotJoint[0].MaxMotorTorque = 1000f;
            _spotJoint[0].MotorSpeed = 0f;
#region test construction
/*
            _spotJoint[2] = new RevoluteJoint(_heart, _rod1Start,  new Vector2(-0.5f, -1f), Vector2.Zero);
            _world.AddJoint(_spotJoint[2]);
	    _spotJoint[2].Enabled = true;
            _spotJoint[2].MotorEnabled = true;
            _spotJoint[2].MaxMotorTorque = 1000f;
            _spotJoint[2].MotorSpeed = 0f;
            _spotJoint[2].LimitEnabled = true;

            _spotJoint[3] = new RevoluteJoint(_heart, _rod2Start,  new Vector2(0.5f, -1f), Vector2.Zero);
            _world.AddJoint(_spotJoint[3]);
	    _spotJoint[3].Enabled = true;
            _spotJoint[3].MotorEnabled = false;
            _spotJoint[3].MaxMotorTorque = 1000f;
            _spotJoint[3].MotorSpeed = 0f;

            _spotJoint[2] = new RevoluteJoint(_rod2Start, _heart, Vector2.Zero, new Vector2(-1, -0.5f));
            _world.AddJoint(_spotJoint[2]);
	    _spotJoint[2].Enabled = true;
            _spotJoint[2].LowerLimit = 0;
            _spotJoint[2].UpperLimit = 0;
            _spotJoint[2].LimitEnabled = true;
            _spotJoint[2].MotorEnabled = false;
            _spotJoint[2].MaxMotorTorque = 1000f;
            _spotJoint[2].MotorSpeed = 0f;
	    */

	    /*
	    _spotJoint[2] = new RevoluteJoint(_heart, _joints[2], Vector2.Zero, Vector2.Zero);
	    _spotJoint[2].Enabled = true;
            _spotJoint[2].MotorEnabled = false;
            _spotJoint[2].MaxMotorTorque = 100f;
            _spotJoint[2].MotorSpeed = 0f;
            _world.AddJoint(_spotJoint[2]);
	    */
/*
	    //Spot1
            _joints[0] = BodyFactory.CreateCircle(_world, 0.01f, 1f, Vector2.Zero);
            _joints[0].BodyType = BodyType.Dynamic;
	    _spotJoint[0] = new RevoluteJoint(_wheel_left, _joints[0], Vector2.Zero, Vector2.Zero);
	    _spotJoint[0].Enabled = true;
            _spotJoint[0].MotorEnabled = true;
            _spotJoint[0].MaxMotorTorque = 100f;
            _spotJoint[0].MotorSpeed = 0f;
            _world.AddJoint(_spotJoint[0]);
	    _spotJoint[7 - 0] = new RevoluteJoint(_rod1End, _joints[0], Vector2.Zero, Vector2.Zero);
	    _spotJoint[7 - 0].Enabled = true;
            _spotJoint[7 - 0].MotorEnabled = true;
            _spotJoint[7 - 0].MaxMotorTorque = 100f;
            _spotJoint[7 - 0].MotorSpeed = 0f;
            _world.AddJoint(_spotJoint[7 - 0]);

	    //Spot2
            _joints[1] = BodyFactory.CreateCircle(_world, 0.01f, 1f, Vector2.Zero);
            _joints[1].BodyType = BodyType.Dynamic;
	    _spotJoint[1] = new RevoluteJoint(_wheel_right, _joints[1], Vector2.Zero, Vector2.Zero);
	    _spotJoint[1].Enabled = true;
            _spotJoint[1].MotorEnabled = true;
            _spotJoint[1].MaxMotorTorque = 100f;
            _spotJoint[1].MotorSpeed = 0f;
            _world.AddJoint(_spotJoint[1]);
	    _spotJoint[7 - 1] = new RevoluteJoint(_rod2End, _joints[1], Vector2.Zero, Vector2.Zero);
	    _spotJoint[7 - 1].Enabled = true;
            _spotJoint[7 - 1].MotorEnabled = true;
            _spotJoint[7 - 1].MaxMotorTorque = 100f;
            _spotJoint[7 - 1].MotorSpeed = 0f;
            _world.AddJoint(_spotJoint[7 - 1]);

	    //Spot3
            _joints[2] = BodyFactory.CreateCircle(_world, 0.01f, 1f, Vector2.Zero);
            _joints[2].BodyType = BodyType.Dynamic;
	    _spotJoint[2] = new RevoluteJoint(_heart, _joints[2], Vector2.Zero, Vector2.Zero);
	    _spotJoint[2].Enabled = true;
            _spotJoint[2].MotorEnabled = true;
            _spotJoint[2].MaxMotorTorque = 100f;
            _spotJoint[2].MotorSpeed = 0f;
            _world.AddJoint(_spotJoint[2]);
	    _spotJoint[7 - 2] = new RevoluteJoint(_rod1Start, _joints[2], Vector2.Zero, Vector2.Zero);
	    _spotJoint[7 - 2].Enabled = true;
            _spotJoint[7 - 2].MotorEnabled = true;
            _spotJoint[7 - 2].MaxMotorTorque = 100f;
            _spotJoint[7 - 2].MotorSpeed = 0f;
            _world.AddJoint(_spotJoint[7 - 2]);

	    //Spot4
            _joints[3] = BodyFactory.CreateCircle(_world, 0.01f, 1f, Vector2.Zero);
            _joints[3].BodyType = BodyType.Dynamic;
	    _spotJoint[3] = new RevoluteJoint(_heart, _joints[3], Vector2.Zero, Vector2.Zero);
	    _spotJoint[3].Enabled = true;
            _spotJoint[3].MotorEnabled = true;
            _spotJoint[3].MaxMotorTorque = 100f;
            _spotJoint[3].MotorSpeed = 0f;
            _world.AddJoint(_spotJoint[3]);
	    _spotJoint[7 - 3] = new RevoluteJoint(_rod2Start, _joints[3], Vector2.Zero, Vector2.Zero);
	    _spotJoint[7 - 3].Enabled = true;
            _spotJoint[7 - 3].MotorEnabled = true;
            _spotJoint[7 - 3].MaxMotorTorque = 100f;
            _spotJoint[7 - 3].MotorSpeed = 0f;
            _world.AddJoint(_spotJoint[7 - 3]);
*/
            /*
                private RevoluteJoint[]		_spotJoint = new RevoluteJoint[8];
                private Body[]			_joints = new Body[4];
            */
#endregion

        }

        // Allows the game to run logic such as updating the world,
        // checking for collisions, gathering input, and playing audio.
        protected override void Update(GameTime gameTime)
        {
            HandleKeyboard();

            // Update the world
            _world.Step((float) gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            base.Update(gameTime);
        }

        // Manage the keyboard.
        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();
	    
            if (state.IsKeyDown(Keys.D))
            {
                _spotJoint[0].MotorSpeed = -10f;
                _spotJoint[1].MotorSpeed = -10f;
            }
            else if (state.IsKeyDown(Keys.A))
            {
                _spotJoint[0].MotorSpeed = 10f;
                _spotJoint[1].MotorSpeed = 10f;
            }
            else
            {
                _spotJoint[0].MotorSpeed = 0f;
                _spotJoint[1].MotorSpeed = 0f;
            }
	    

            if (state.IsKeyDown(Keys.W))
            {
                _jointRod1.MotorSpeed = -20f;
                _jointRod2.MotorSpeed = -20f;
            }
            else if (state.IsKeyDown(Keys.S))
            {
                _jointRod1.MotorSpeed = 20f;
                _jointRod2.MotorSpeed = 20f;
            }
            else
            {
                _jointRod1.MotorSpeed = 0f;
                _jointRod2.MotorSpeed = 0f;
            }

            if (state.IsKeyDown(Keys.Space))
            {
            }
            else if (state.IsKeyDown(Keys.Escape))
              Exit();
        }

        // This is called when the game should draw itself.
        protected override void Draw(GameTime gameTime)
        {
            // Erase & Draw background
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Drawing
            _batch.Begin();
            #region test affichage

            /*
            // Grounds
            Vector2 orig = new Vector2(_ground_tex.Width / 2f, _ground_tex.Height / 2f);
            _batch.Draw(_ground_tex, ConvertUnits.ToDisplayUnits(_ground.Position), null,  Color.LightGreen, _ground.Rotation, orig, 1f, SpriteEffects.None, 0f);
            _batch.Draw(_ground_tex, ConvertUnits.ToDisplayUnits(_ground_up_left.Position), null, Color.LightGreen, _ground.Rotation, orig, 1f, SpriteEffects.None, 0f);
            _batch.Draw(_ground_tex, ConvertUnits.ToDisplayUnits(_ground_up_right.Position), null, Color.LightGreen, _ground.Rotation, orig, 1f, SpriteEffects.None, 0f);
            // Ball
            orig = new Vector2(_ball_tex.Width / 2f, _ball_tex.Height / 2f);
            _batch.Draw(_ball_tex, ConvertUnits.ToDisplayUnits(_ball.Position), null, Color.LightGreen, _ball.Rotation, orig, 1f, SpriteEffects.None, 0f);
	    */

	    //ROBOT
/*            _batch.Draw(_wheel_left_tex, ConvertUnits.ToDisplayUnits(_wheel_left.Position), null, Color.LightGreen, _wheel_left.Rotation, 
		    new Vector2(_wheel_left_tex.Width / 2f, _wheel_left_tex.Height / 2f), 1f, SpriteEffects.None, 0f);
            _batch.Draw(_wheel_right_tex, ConvertUnits.ToDisplayUnits(_wheel_right.Position), null, Color.LightGreen, _wheel_right.Rotation, 
		    new Vector2(_wheel_right_tex.Width / 2f, _wheel_right_tex.Height / 2f), 1f, SpriteEffects.None, 0f);
	    _batch.Draw(_heart_tex, ConvertUnits.ToDisplayUnits(_heart.Position), null, Color.White, _heart.Rotation, 
		    new Vector2(_heart_tex.Width / 2f, _heart_tex.Height / 3f), 1f, SpriteEffects.None, 0f);

	    /*_batch.Draw(_join, ConvertUnits.ToDisplayUnits(_heart.Position), null, Color.White, _heart.Rotation, 
		    new Vector2(_heart_tex.Width / 2f, _heart_tex.Height / 3f), 1f, SpriteEffects.None, 0f);
	    _batch.Draw(_heart_tex, ConvertUnits.ToDisplayUnits(_heart.Position), null, Color.White, _heart.Rotation, 
		    new Vector2(_heart_tex.Width / 2f, _heart_tex.Height / 3f), 1f, SpriteEffects.None, 0f);*/

	/*private Body[]			_joints = new Body[4];
        private Body			_rod1Start;
        private Body			_rod1End;*/
            // Pyramid
            // calculate the projection and view adjustments for the debug view
            #endregion
            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, ConvertUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Width),
                                                         ConvertUnits.ToSimUnits(_graphics.GraphicsDevice.Viewport.Height), 0f, 0f,
                                                         1f);
        Matrix view = Matrix.CreateTranslation(new Vector3((ConvertUnits.ToSimUnits(_camera_position) -
	ConvertUnits.ToSimUnits(_screen_center)), 0f)) * Matrix.CreateTranslation(new Vector3(ConvertUnits.ToSimUnits(_screen_center), 0f));
        _debug.RenderDebugData(ref projection, ref view);

            //_pyramid.Draw(_batch);

            _batch.End();
        }
    }
}
