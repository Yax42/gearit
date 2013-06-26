using System.Text;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;
using gearit.xna;

namespace gearit.src.utility
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    internal class MyGame : PhysicsGameScreen, IDemoScreen
    {
        private SoundManager _sound;

        private KeyboardState state;
        private KeyboardState _oldKeyState;
        private GamePadState _oldPadState;

        private Body _circleBody;
        private Body _groundBody;

        // Cercle voiture
        private Body _circle;
        private Texture2D _circleTex;
        private CircleShape _circleShape;

        // Cercle voiture
        private Body _circle2;
        private Texture2D _circleTex2;
        private CircleShape _circleShape2;

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

        // Revolute Joint
        private RevoluteJoint _motorJoint;
        private RevoluteJoint _motorJoint2;

        private PrismaticJoint _motorPris;
        private SliderJoint _motorSlider;
        /*// physics simulator debug view
        DebugViewXNA _debugView;*/

        //private LuaTest _lua;

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Alex-mobile";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("TODO: Add sample description!");
            sb.AppendLine(string.Empty);
            sb.AppendLine("GamePad:");
            sb.AppendLine("  - Exit to menu: Back button");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Keyboard:");
            sb.AppendLine("  - Exit to menu: Escape");
            return sb.ToString();
        }

        #endregion

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

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            World.Gravity = new Vector2(0f, 8f);
            HasCursor = true;
            EnableCameraControl = true;
            HasVirtualStick = true;

            _sound = new SoundManager(ScreenManager, "Audio/soupeGame");
            _sound.ActiveLoop();
            
            //_lua = new LuaTest(ScreenManager, );

            // Initialize camera controls
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;
            _screenCenter = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2f,
                                                ScreenManager.GraphicsDevice.Viewport.Height / 2f);

            FarseerPhysics.Settings.UseFPECollisionCategories = true;

            // Load sprites
            _circleSprite = ScreenManager.Content.Load<Texture2D>("circleSprite"); //  96px x 96px => 1.5m x 1.5m
            _groundSprite = ScreenManager.Content.Load<Texture2D>("groundSprite"); // 512px x 64px =>   8m x 1m

            /* Circle */
            // Convert screen center from pixels to meters
            Vector2 circlePosition = ConvertUnits.ToSimUnits(_screenCenter) + new Vector2(0, -5.5f);

            // Create the circle fixture

            _circleBody = BodyFactory.CreateCircle(World, ConvertUnits.ToSimUnits(96f) / 2f, 1f, circlePosition);
            _circleBody.BodyType = BodyType.Dynamic;

            // Create new circle
            _circleShape = new CircleShape(ConvertUnits.ToSimUnits(96f) / 2f , 10f);
            //_circleShape.Position = circlePosition;
            //_circleShape.MassData.Mass = 500f;

            _circle = new Body(World);
            _circle.BodyType = BodyType.Dynamic;
            _circle.Position = circlePosition;
            Fixture fix = _circle.CreateFixture(_circleShape);
            fix.Friction = 0.5f;
            fix.Restitution = 1f;
            fix.CollisionGroup = 42;

            _circleTex = ScreenManager.Assets.TextureFromShape(_circleShape, MaterialType.Blank, Color.Red, 1f);

            // Create new circle 2
            _circleShape2 = new CircleShape(ConvertUnits.ToSimUnits(96f) / 2f, 10f);
            //_circleShape.Position = circlePosition;
            //_circleShape.MassData.Mass = 500f;

            _circle2 = new Body(World);
            _circle2.BodyType = BodyType.Dynamic;
            _circle2.Position = circlePosition;
            Fixture fix2 = _circle2.CreateFixture(_circleShape2);
            fix2.Friction = 0.5f;
            fix2.Restitution = 1f;
            fix2.CollisionGroup = 42;


            _circleTex2 = ScreenManager.Assets.TextureFromShape(_circleShape2, MaterialType.Blank, Color.Gray, 1f);

            // Create Square
            vertices = new Vertices(8);
            vertices.Add(new Vector2(-1.5f, -0.5f));
            vertices.Add(new Vector2(1.5f, -0.5f));
            vertices.Add(new Vector2(1.5f, 0.0f));
            vertices.Add(new Vector2(0.0f, 0.9f));
            vertices.Add(new Vector2(-1.15f, 0.9f));
            vertices.Add(new Vector2(-1.5f, 0.2f));

            _squareShape = new PolygonShape(vertices, 20f);

            _square = new Body(World);
            _square.SetTransform(circlePosition, 3f);
            _square.BodyType = BodyType.Dynamic;
            _square.Position = circlePosition;
            _square.CreateFixture(_squareShape);
            _square.CollisionGroup = 42;
            _squareTex = ScreenManager.Assets.TextureFromVertices(vertices, MaterialType.Blank, Color.Blue * 0.8f, 1f);

            // Give it some bounce and friction
            _circleBody.Restitution = 0.3f;
            _circleBody.Friction = 0.5f;

            /* Ground */
            Vector2 groundPosition = (ConvertUnits.ToSimUnits(_screenCenter)) + new Vector2(0, 1.25f);

            // Create the ground fixture
            _groundBody = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(512), ConvertUnits.ToSimUnits(64f), 1f, groundPosition);
            _groundBody.IsStatic = true;
            _groundBody.Restitution = 0.3f;
            _groundBody.Friction = 0.5f;

            /* Wall LEFT*/
            Vector2 wallPosition = ConvertUnits.ToSimUnits(_screenCenter) + new Vector2(-5f, 1.25f);

            // Create the wall fixture
            _wallLeft = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(512f), ConvertUnits.ToSimUnits(64f), 1f, wallPosition);

            _wallLeft.SetTransform(wallPosition, 1.5f);
            _wallLeft.IsStatic = true;
            _wallLeft.Restitution = 0.3f;
            _wallLeft.Friction = 0.5f;

            /* Wall RIGHT*/
            wallPosition = ConvertUnits.ToSimUnits(_screenCenter) + new Vector2(5f, 1.25f);

            // Create the wall fixture
            _wallRight = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(512f), ConvertUnits.ToSimUnits(64f), 1f, wallPosition);
            _wallRight.SetTransform(wallPosition, 1.5f);
            _wallRight.IsStatic = true;
            _wallRight.Restitution = 0.3f;
            _wallRight.Friction = 0.5f;

            // Joint body 1
            _motorJoint = new RevoluteJoint(_circle, _square, _circle.GetLocalPoint(_square.Position), new Vector2(1f, -0.5f));
            _motorJoint.CollideConnected = false;
            _motorJoint.MotorSpeed = 0f;
            _motorJoint.MaxMotorTorque = 400f;

            _motorJoint.MotorEnabled = true;
            World.AddJoint(_motorJoint);

            // Joint body 2
            /*_motorPris = new PrismaticJoint(_circle2, _square, _circle.GetLocalPoint(_square.Position), new Vector2(-4f, 0f), new Vector2(1f, 0f));
            _motorPris.CollideConnected = false;
            _motorPris.MaxMotorForce = 400f;
            _motorPris.UpperLimit = 20f;
            _motorPris.LowerLimit = -10f;
            _motorPris.MotorEnabled = true;
            _motorPris.MotorSpeed = 0f;
            _motorPris.Enabled = true;
            World.AddJoint(_motorPris);

            _motorSlider = new SliderJoint(_circle2, _square, _circle.GetLocalPoint(_square.Position), new Vector2(1f, 0f), 2f, 10f);
            _motorSlider.Enabled = true;
            _motorSlider.CollideConnected = false;*/
            //_world.AddJoint(_motorSlider);

            _motorJoint2 = new RevoluteJoint(_circle2, _square, _circle.GetLocalPoint(_square.Position), new Vector2(-1f, 0f));
            _motorJoint2.CollideConnected = false;
            _motorJoint2.MotorSpeed = -5.0f;
            _motorJoint2.MaxMotorTorque = 400f;

            _motorJoint2.MotorEnabled = true;
            World.AddJoint(_motorJoint2);

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
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            HandleGamePad();
            HandleKeyboard();

            //We update the world
            World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            //Play sound
            if (!_sound.isPlaying())
                _sound.playSound();

            if (this.IsExiting)
                _sound.StopSound();
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public void ChangeWheel(float value)
        {
            _motorJoint.MotorSpeed = value;
            _motorJoint2.MotorSpeed = value;
        }

        public RevoluteJoint getWheel(int num)
        {
            if (num == 1)
                return (_motorJoint);
            else
                return (_motorJoint2);
        }

        private void HandleGamePad()
        {
            GamePadState padState = GamePad.GetState(0);

            if (padState.IsConnected)
            {
                if (padState.Buttons.Back == ButtonState.Pressed)
                    ScreenManager.Game.Exit();

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
            state = Keyboard.GetState();

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
                /*if (state.IsKeyDown(Keys.Q))
                {
                    _circleBody.ApplyTorque(-10);
                    _motorJoint.MotorSpeed = 2.0f;
                    _motorJoint2.MotorSpeed =2.0f;
                    //_motorPris.MotorSpeed = 5.0f;
                }

                if (state.IsKeyDown(Keys.Right))
                {
                    _motorJoint.MotorSpeed = -2.0f;
                    _motorJoint2.MotorSpeed = -2.0f;
                    //_motorPris.MotorSpeed = -5.0f;
                }

                // We make it possible to rotate the circle body
                if (state.IsKeyDown(Keys.Left))
                {
                    _motorJoint.MotorSpeed = 2.0f;
                    _motorJoint2.MotorSpeed = 2.0f;
                    //_motorPris.MotorSpeed = 5.0f;
                }

                if (state.IsKeyDown(Keys.D))
                {
                    _circleBody.ApplyTorque(10);
                    _motorJoint.MotorSpeed = -2.0f;
                    _motorJoint2.MotorSpeed = -2.0f;
                    //_motorPris.MotorSpeed = -5.0f;
                }

                if (state.IsKeyDown(Keys.S))
                {
                    _motorJoint.MotorSpeed = 0f;
                    _motorJoint2.MotorSpeed = 0f;
                }

                if (state.IsKeyDown(Keys.Space) && _oldKeyState.IsKeyUp(Keys.Space))
                {
                    _circleBody.ApplyLinearImpulse(new Vector2(0, -10));
                    _square.ApplyLinearImpulse(new Vector2(0, -500));
                }*/
            }

            /*if (state.IsKeyDown(Keys.Escape))
                ScreenManager.Game.Exit();*/

            _oldKeyState = state;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            /* Circle position and rotation */
            // Convert physics position (meters) to screen coordinates (pixels)
            Vector2 circlePos = ConvertUnits.ToDisplayUnits(_circleBody.Position);
            float circleRotation = _circleBody.Rotation;

            Vector2 circlePos2 = ConvertUnits.ToDisplayUnits(_circle.Position);
            float circleRotation2 = _circle.Rotation;

            Vector2 circlePos3 = ConvertUnits.ToDisplayUnits(_circle2.Position);
            float circleRotation3 = _circle2.Rotation;

            Vector2 squarePos = ConvertUnits.ToDisplayUnits(_square.Position);
            float squareRotation = _square.Rotation;

            /* Ground position and origin */
            Vector2 groundPos = ConvertUnits.ToDisplayUnits(_groundBody.Position);
            Vector2 wallPosL = ConvertUnits.ToDisplayUnits(_wallLeft.Position);
            Vector2 wallPosR = ConvertUnits.ToDisplayUnits(_wallRight.Position);
            float wallRotationL = _wallLeft.Rotation;
            float wallRotationR = _wallRight.Rotation;

            Vector2 groundOrigin = new Vector2(_groundSprite.Width / 2f, _groundSprite.Height / 2f);

            // Align sprite center to body position
            Vector2 circleOrigin = new Vector2(_circleSprite.Width / 2f, _circleSprite.Height / 2f);
            Vector2 circleOrigin2 = new Vector2(_circleTex.Width / 2f, _circleTex.Height / 2f);
            Vector2 circleOrigin3 = new Vector2(_circleTex2.Width / 2f, _circleTex2.Height / 2f);
            Vector2 squareOrigin = new Vector2(_squareTex.Width / 2f, _squareTex.Height / 3f);

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);

            //Draw circle
            ScreenManager.SpriteBatch.Draw(_circleSprite, circlePos, null, Color.White, circleRotation, circleOrigin, 1f, SpriteEffects.None, 0f);

            ScreenManager.SpriteBatch.Draw(_circleTex, circlePos2, null, Color.White, circleRotation2, circleOrigin2, 1f, SpriteEffects.None, 0f);

            ScreenManager.SpriteBatch.Draw(_circleTex2, circlePos3, null, Color.White, circleRotation3, circleOrigin3, 1f, SpriteEffects.None, 0f);

            ScreenManager.SpriteBatch.Draw(_squareTex, squarePos, null, Color.White, squareRotation, squareOrigin, 1f, SpriteEffects.None, 0f);

            //BodyFactory.CreatePolygon(_world, vertices, 1f);

            //Draw ground
            ScreenManager.SpriteBatch.Draw(_groundSprite, groundPos, null, Color.White, 0f, groundOrigin, 1f, SpriteEffects.None, 0f);

            //Draw Wall Left
            ScreenManager.SpriteBatch.Draw(_groundSprite, wallPosL, null, Color.White, wallRotationL, groundOrigin, 1f, SpriteEffects.None, 0f);

            //Draw Wall Right
            ScreenManager.SpriteBatch.Draw(_groundSprite, wallPosR, null, Color.White, wallRotationR, groundOrigin, 1f, SpriteEffects.None, 0f);

            ScreenManager.SpriteBatch.End();

            ScreenManager.SpriteBatch.Begin();

            // Display instructions
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.MenuSpriteFont, Text, new Vector2(14f, 14f), Color.Black);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.MenuSpriteFont, Text, new Vector2(12f, 12f), Color.White);

            ScreenManager.SpriteBatch.End();



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