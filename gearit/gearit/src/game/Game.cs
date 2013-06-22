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

namespace gearit.src.game
{
    class Game : GameScreen, IDemoScreen
    {
        private World _world;
        private Input _input;
        private Camera2D _camera;

        private Map _map;
        private List<Robot> _robots;
        private DrawGame _drawGame;

        // Graphic
        private RectangleOverlay _background;

        // Action
        private int _time = 0;

        #region IDemoScreen Members

        public Game()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.75);
            TransitionOffTime = TimeSpan.FromSeconds(0.75);
            HasCursor = true;
            _world = new World(Vector2.Zero);
            _drawGame = new DrawGame(ScreenManager.GraphicsDevice);
            _camera = new Camera2D(ScreenManager.GraphicsDevice);
            _input = new Input(_camera);
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
            _world.Clear();
            _world.Gravity = new Vector2(0f, 0f);

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
            _robots.Clear();
        }

        public void addRobot(Robot robot)
        {
            _robots.Add(robot);
            if (_robots.Count == 1)
                _camera.TrackingBody = robot.getHeart();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            _time++;
            _input.update();
            HandleInput();

            _world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));

            _camera.Update(gameTime);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }


        private void HandleInput()
        {
            if (_input.pressed(Keys.Escape))
            {
                ScreenManager.RemoveScreen(this);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.LightYellow);
            _drawGame.Begin(_camera);

            foreach (Robot r in _robots)
                r.drawDebug(_drawGame);
            _map.drawDebug(_drawGame);
            _drawGame.End();

            base.Draw(gameTime);
        }
    }
}

