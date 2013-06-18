using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using gearit.src.robot;

namespace gearit.src.utility
{
    enum Action
    {
	NONE,
	PRIS_SPOT,
	REV_SPOT,
	MOVE,
	REMOVE,
	COUNT
    }
    class RobotEditor : PhysicsGameScreen, IDemoScreen
    {
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;
        private const int PropertiesMenuSize = 200;

        // Graphic
        private RectangleOverlay _background;
        private MenuOverlay _menu_properties;
        private MenuOverlay _menu_tools;

        // Mouse
        private MouseState _old_mouse_state;
        private MouseState _mouse;

        // Robot
        private DrawGame _draw_game;
        private Robot _robot;

	// Action
        private Piece _selected;
        private Action _action;

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Robot Editor";
        }

        public string GetDetails()
        {
            return ("");
        }

        #endregion

        public override void LoadContent()
        {
            base.LoadContent();

            World.Gravity = new Vector2(0f, 0f);
            HasCursor = true;
            EnableCameraControl = true;
            HasVirtualStick = true;

            // Robot
            _draw_game = new DrawGame(ScreenManager.GraphicsDevice);
            _robot = new Robot(World);
	    _selected = null;
            Selected = _robot.getHeart();
            _action = Action.NONE;

            // Initialize camera controls
            _cameraPosition = new Vector2(300, 300);
            _screenCenter = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2f, ScreenManager.GraphicsDevice.Viewport.Height / 2f);

            // Graphic
            Rectangle rec = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
            _background = new RectangleOverlay(rec, Color.WhiteSmoke, ScreenManager.GraphicsDevice);

            // Menu
            Vector2 pos = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width - PropertiesMenuSize, 0);
            Vector2 size = new Vector2(PropertiesMenuSize, ScreenManager.GraphicsDevice.Viewport.Height);
            _menu_properties = new MenuOverlay(ScreenManager.GraphicsDevice, ScreenManager.Content, pos, size, Color.LightGray, MenuLayout.Vertical);
            _menu_properties.addItemMenu("Properties", ScreenManager.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.Default, 1.5f);

            pos.X = 200;
            size = new Vector2(400, 50);
            _menu_tools = new MenuOverlay(ScreenManager.GraphicsDevice, ScreenManager.Content, pos, size, Color.LightGray, MenuLayout.Horizontal);
            MenuItem item;
            item = _menu_tools.addItemMenu("Rotation", ScreenManager.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
            item.addFocus(1, new Color(120, 120, 120), ScreenManager.GraphicsDevice);
            item = _menu_tools.addItemMenu("Spring", ScreenManager.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
            item.addFocus(2, new Color(120, 120, 120), ScreenManager.GraphicsDevice);
        }

        public Piece Selected
        {
            get { return _selected; }
            set
            {
                if (_selected != null)
                  _selected.ColorValue = Color.Black;
                _selected = value;
                _selected.ColorValue = Color.Red;
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            HandleMouse();

            //We update the world
            World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public Vector2 sim_mouse_pos()
        {
          return (ConvertUnits.ToSimUnits(new Vector2(_mouse.X, _mouse.Y) - _cameraPosition));
        }

        private void HandleMouse()
        {
            _mouse = Mouse.GetState();

            if (_mouse.LeftButton == ButtonState.Pressed &&
          _old_mouse_state.LeftButton != ButtonState.Pressed)
            {
                Selected = _robot.getPiece(sim_mouse_pos());
            }
            if (_mouse.RightButton == ButtonState.Pressed &&
          _old_mouse_state.RightButton != ButtonState.Pressed)
            {
                Piece p = new Wheel(_robot, 0.5f);
                _robot.addPiece(p);
                _robot.addSpot(new PrismaticSpot(_robot, _selected, p));
            }
            if (_mouse.MiddleButton == ButtonState.Pressed)
            {
                _cameraPosition += new Vector2(_mouse.X - _old_mouse_state.X, _mouse.Y - _old_mouse_state.Y);
            }
            if (_mouse != _old_mouse_state)
            {
                _old_mouse_state = _mouse;
                _menu_properties.Update(_mouse);
                _menu_tools.Update(_mouse);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _draw_game.Begin(ScreenManager.GraphicsDevice.Viewport, _cameraPosition, _screenCenter);

            _background.Draw(_draw_game.Batch());
            _menu_properties.Draw(_draw_game.Batch());
            _menu_tools.Draw(_draw_game.Batch());

            _draw_game.End();

            _draw_game.Begin(ScreenManager.GraphicsDevice.Viewport, _cameraPosition, _screenCenter);
            _robot.drawDebug(_draw_game);
            _draw_game.End();

            base.Draw(gameTime);
        }

    }
}
