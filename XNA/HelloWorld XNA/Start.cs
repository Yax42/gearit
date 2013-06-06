using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FarseerPhysics.HelloWorld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Start : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _batch;
        private KeyboardState _oldKeyState;
        private GamePadState _oldPadState;
        private SpriteFont _font;

        private World _world;

        private Body _circleBody;
        private Body _groundBody;

        private Body _circle;
        private Texture2D _circleTex;
        private CircleShape _circleShape;

        Vertices vertices;

        private Body _square;
        private PolygonShape _squareShape;
        private Texture2D _squareTex;

        private Body _wallLeft;
        private Body _wallRight;

        private Texture2D _circleSprite;
        private Texture2D _circleSprite2;
        private Texture2D _groundSprite;
        
        // Simple camera controls
        private Matrix _view;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;

        // New Asset
        private AssetCreator _asset;

        /*// physics simulator debug view
        DebugViewXNA _debugView;*/


#if !XBOX360
        const string Text = "Press A or D to rotate the ball\n" +
                            "Press Space to jump\n" +
                            "Press Shift + W/S/A/D to move the camera";
#else
                const string Text = "Use left stick to move\n" +
                                    "Use right stick to move camera\n" +
                                    "Press A to jump\n";
#endif
        // Farseer expects objects to be scaled to MKS (meters, kilos, seconds)
        // 1 meters equals 64 pixels here
        // (Objects should be scaled to be between 0.1 and 10 meters in size)
        private const float MeterInPixels = 64f;

        public Start()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 768;

            Content.RootDirectory = "Content";

            _world = new World(new Vector2(0, 20));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Initialize camera controls
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;

            _screenCenter = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2f,
                                                _graphics.GraphicsDevice.Viewport.Height / 2f);

            _batch = new SpriteBatch(_graphics.GraphicsDevice);
            _font = Content.Load<SpriteFont>("font");

            _asset = new AssetCreator(GraphicsDevice);
            _asset.LoadContent(Content);

            // Load sprites
            _circleSprite = Content.Load<Texture2D>("circleSprite"); //  96px x 96px => 1.5m x 1.5m
            _groundSprite = Content.Load<Texture2D>("groundSprite"); // 512px x 64px =>   8m x 1m
            _circleSprite2 = Content.Load<Texture2D>("texture_foot");

            /* Circle */
            // Convert screen center from pixels to meters
            Vector2 circlePosition = (_screenCenter / MeterInPixels) + new Vector2(0, -1.5f);

            // Create the circle fixture
            
            _circleBody = BodyFactory.CreateCircle(_world, 96f / (2f * MeterInPixels), 1f, circlePosition);
            _circleBody.BodyType = BodyType.Dynamic;

            // Create new circle
            _circleShape = new CircleShape(96f / (2f * MeterInPixels), 1f);
            //_circleShape.Position = circlePosition;
            //_circleShape.MassData.Mass = 500f;

            _circle = new Body(_world);
            _circle.BodyType = BodyType.Dynamic;
            _circle.Position = circlePosition;
            Fixture fix = _circle.CreateFixture(_circleShape);
            fix.Friction = 0.5f;
            fix.Restitution = 1f;


            _circleTex = _asset.TextureFromShape(_circleShape, MaterialType.Blank, Color.Red, 1f);

            // Create Square
            vertices = new Vertices(8);
            vertices.Add(new Vector2(-1.5f, -0.5f));
            vertices.Add(new Vector2(1.5f, -0.5f));
            vertices.Add(new Vector2(1.5f, 0.0f));
            vertices.Add(new Vector2(0.0f, 0.9f));
            vertices.Add(new Vector2(-1.15f, 0.9f));
            vertices.Add(new Vector2(-1.5f, 0.2f));

            _squareShape = new PolygonShape(vertices, 1);

            _square = new Body(_world);
            _square.BodyType = BodyType.Dynamic;
            _square.Position = circlePosition;
            _square.CreateFixture(_squareShape);

            _squareTex = _asset.TextureFromVertices(vertices, MaterialType.Blank, Color.Blue * 0.8f, 1f);

            // Give it some bounce and friction
            _circleBody.Restitution = 0.3f;
            _circleBody.Friction = 0.5f;

            /* Ground */
            Vector2 groundPosition = (_screenCenter / MeterInPixels) + new Vector2(0, 1.25f);

            // Create the ground fixture
            _groundBody = BodyFactory.CreateRectangle(_world, 512f / MeterInPixels, 64f / MeterInPixels, 1f, groundPosition);
            _groundBody.IsStatic = true;
            _groundBody.Restitution = 0.3f;
            _groundBody.Friction = 0.5f;

            /* Wall LEFT*/
            Vector2 wallPosition = (_screenCenter / MeterInPixels) + new Vector2(-5f, 1.25f);

            // Create the wall fixture
            _wallLeft = BodyFactory.CreateRectangle(_world, 512f / MeterInPixels, 64f / MeterInPixels, 1f, wallPosition);

            _wallLeft.SetTransform(wallPosition, 1.5f); 
            _wallLeft.IsStatic = true;
            _wallLeft.Restitution = 0.3f;
            _wallLeft.Friction = 0.5f;

            /* Wall RIGHT*/
            wallPosition = (_screenCenter / MeterInPixels) + new Vector2(5f, 1.25f);

            // Create the wall fixture
            _wallRight = BodyFactory.CreateRectangle(_world, 512f / MeterInPixels, 64f / MeterInPixels, 1f, wallPosition);
            _wallRight.SetTransform(wallPosition, 1.5f);
            _wallRight.IsStatic = true;
            _wallRight.Restitution = 0.3f;
            _wallRight.Friction = 0.5f;

            /*// create and configure the debug view
            _debugView = new DebugViewXNA(_world);
            _debugView.AppendFlags(DebugViewFlags.DebugPanel);
            _debugView.DefaultShapeColor = Color.White;
            _debugView.SleepingShapeColor = Color.LightGray;
            _debugView.LoadContent(GraphicsDevice, Content);*/
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            HandleGamePad();
            HandleKeyboard();

            //We update the world
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            

            base.Update(gameTime);
        }

        private void HandleGamePad()
        {
            GamePadState padState = GamePad.GetState(0);

            if (padState.IsConnected)
            {
                if (padState.Buttons.Back == ButtonState.Pressed)
                    Exit();

                if (padState.Buttons.A == ButtonState.Pressed && _oldPadState.Buttons.A == ButtonState.Released)
                    _circleBody.ApplyLinearImpulse(new Vector2(0, -10));

                _circleBody.ApplyForce(padState.ThumbSticks.Left);
                _cameraPosition.X -= padState.ThumbSticks.Right.X;
                _cameraPosition.Y += padState.ThumbSticks.Right.Y;

                _view = Matrix.CreateTranslation(new Vector3(_cameraPosition - _screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));

                _oldPadState = padState;
            }
        }

        private void HandleInput()
        {
        }

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

            // Switch between circle body and camera control
            if (state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift))
            {
                // Move camera
                if (state.IsKeyDown(Keys.Q))
                    _cameraPosition.X += 1.5f;

                if (state.IsKeyDown(Keys.D))
                    _cameraPosition.X -= 1.5f;

                if (state.IsKeyDown(Keys.Z))
                    _cameraPosition.Y += 1.5f;

                if (state.IsKeyDown(Keys.S))
                    _cameraPosition.Y -= 1.5f;

                _view = Matrix.CreateTranslation(new Vector3(_cameraPosition - _screenCenter, 0f)) *
                        Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));
            }
            else
            {
                // We make it possible to rotate the circle body
                if (state.IsKeyDown(Keys.Q))
                    _circleBody.ApplyTorque(-10);

                if (state.IsKeyDown(Keys.D))
                    _circleBody.ApplyTorque(10);

                if (state.IsKeyDown(Keys.Space) && _oldKeyState.IsKeyUp(Keys.Space))
                    _circleBody.ApplyLinearImpulse(new Vector2(0, -10));
            }

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            _oldKeyState = state;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            /* Circle position and rotation */
            // Convert physics position (meters) to screen coordinates (pixels)
            Vector2 circlePos = _circleBody.Position * MeterInPixels;
            float circleRotation = _circleBody.Rotation;

            Vector2 circlePos2 = _circle.Position * MeterInPixels;
            float circleRotation2 = _circle.Rotation;

            Vector2 squarePos = _square.Position * MeterInPixels;
            float squareRotation = _square.Rotation;

            /* Ground position and origin */
            Vector2 groundPos = _groundBody.Position * MeterInPixels;
            Vector2 wallPosL = _wallLeft.Position * MeterInPixels;
            Vector2 wallPosR = _wallRight.Position * MeterInPixels;
            float wallRotationL = _wallLeft.Rotation;
            float wallRotationR = _wallRight.Rotation;

            Vector2 groundOrigin = new Vector2(_groundSprite.Width / 2f, _groundSprite.Height / 2f);

            // Align sprite center to body position
            Vector2 circleOrigin = new Vector2(_circleSprite.Width / 2f, _circleSprite.Height / 2f);
            Vector2 circleOrigin2 = new Vector2(_circleTex.Width / 2f, _circleTex.Height / 2f);
            Vector2 squareOrigin = new Vector2(_squareTex.Width / 2f, _squareTex.Height / 2f);

            _batch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);

            //Draw circle
            _batch.Draw(_circleSprite, circlePos, null, Color.White, circleRotation, circleOrigin, 1f, SpriteEffects.None, 0f);

            _batch.Draw(_circleTex, circlePos2, null, Color.White, circleRotation2, circleOrigin2, 1f, SpriteEffects.None, 0f);

            _batch.Draw(_squareTex, squarePos, null, Color.White, squareRotation, squareOrigin, 1f, SpriteEffects.None, 0f);

            //BodyFactory.CreatePolygon(_world, vertices, 1f);
            
            //Draw ground
            _batch.Draw(_groundSprite, groundPos, null, Color.White, 0f, groundOrigin, 1f, SpriteEffects.None, 0f);

            //Draw Wall Left
            _batch.Draw(_groundSprite, wallPosL, null, Color.White, wallRotationL, groundOrigin, 1f, SpriteEffects.None, 0f);

            //Draw Wall Right
            _batch.Draw(_groundSprite, wallPosR, null, Color.White, wallRotationR, groundOrigin, 1f, SpriteEffects.None, 0f);

            _batch.End();

            _batch.Begin();

            // Display instructions
            _batch.DrawString(_font, Text, new Vector2(14f, 14f), Color.Black);
            _batch.DrawString(_font, Text, new Vector2(12f, 12f), Color.White);

            _batch.End();

            

            /*// calculate the projection and view adjustments for the debug view
            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, _graphics.GraphicsDevice.Viewport.Width / MeterInPixels,
                                                             _graphics.GraphicsDevice.Viewport.Height / MeterInPixels, 0f, 0f,
                                                             1f);
            Matrix view = Matrix.CreateTranslation(new Vector3((_cameraPosition / MeterInPixels) - (_screenCenter / MeterInPixels), 0f)) * Matrix.CreateTranslation(new Vector3((_screenCenter / MeterInPixels), 0f));
            // draw the debug view
            _debugView.RenderDebugData(ref projection, ref view);*/

            base.Draw(gameTime);
        }
    }
}