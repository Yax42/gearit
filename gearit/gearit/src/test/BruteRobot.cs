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
        private Texture2D               _wheel_right_tex;
        private Texture2D               _wheel_left_tex;
        private Texture2D               _heart_tex;
        private Texture2D               _ground_tex;
        private Texture2D               _box_text;
        private Texture2D               _ball_tex;
        // Camera
        private Matrix                  _view;
        private Vector2                 _camera_position;
        private Vector2                 _screen_center;

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

        // Utility
        private const float             MeterInPixels = 64f;
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
            // Setting the root path for content (sprite/font..)
            Content.RootDirectory = "Content";

            // Initialize camera controls
            _view = Matrix.Identity;
            _camera_position = Vector2.Zero;
            _screen_center = new Vector2(_graphics.GraphicsDevice.Viewport.Width, _graphics.GraphicsDevice.Viewport.Height) / MeterInPixels / 2f;

            // Link painter to window 
            _batch = new SpriteBatch(_graphics.GraphicsDevice);

            // Utility
            _asset = new AssetCreator(GraphicsDevice);
            
            _asset.LoadContent(Content);

            // Initialize grounds
            Vector2 wall_position = new Vector2(_screen_center.X / 2, _screen_center.Y + 2f);
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
            _pyramid = new Pyramid(_world, new Vector2(9.3f, 0f), 3, 1f, _asset);
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
            }
            else if (state.IsKeyDown(Keys.A))
            {
            }
            else if (state.IsKeyDown(Keys.Space))
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

            // Grounds
            _batch.Draw(_ground_tex, _ground.Position * MeterInPixels, Color.LightGreen);
            _batch.Draw(_ground_tex, _ground_up_right.Position * MeterInPixels, Color.LightGreen);
            _batch.Draw(_ground_tex, _ground_up_left.Position * MeterInPixels, Color.LightGreen);
            // Ball
            _batch.Draw(_ball_tex, _ball.Position * MeterInPixels, Color.LightGreen);
            // Pyramid
            _pyramid.Draw(_batch);

            _batch.End();
        }
    }
}
