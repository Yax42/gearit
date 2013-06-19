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
using gearit.src.editor.robot.action;

namespace gearit.src.editor.robot
{
    enum ActionTypes
    {
	NONE,
	PRIS_SPOT,
	REV_SPOT,
	COUNT,
	MOVE_ANCHOR,
    }
    class RobotEditor : PhysicsGameScreen, IDemoScreen
    {
        private Input _input;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;
        private const int PropertiesMenuSize = 200;

        // Graphic
        private RectangleOverlay _background;
        private MenuOverlay _menu_properties;
        private MenuOverlay _menu_tools;


        // Robot
        private DrawGame _draw_game;
        private Robot _robot;

	// Action
        private Piece _selected;
        private ActionTypes _actionType;
        private IAction[] _actions = new IAction[(int) ActionTypes.COUNT];

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

            _input = new Input();
            World.Gravity = new Vector2(0f, 0f);
            HasCursor = true;
            EnableCameraControl = true;
            HasVirtualStick = true;

            // Robot
            _draw_game = new DrawGame(ScreenManager.GraphicsDevice);
            _robot = new Robot(World);
            _selected = _robot.getHeart();
            _actionType = ActionTypes.NONE;

	    //actions
            _actions[(int) ActionTypes.NONE] = new ActionNone();
            _actions[(int) ActionTypes.PRIS_SPOT] = new ActionPrisSpot();
            _actions[(int) ActionTypes.REV_SPOT] = new ActionRevSpot();
            //_actions[(int) ActionTypes.REMOVE] = new ActionRemove();

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
            item.addFocus((int) ActionTypes.REV_SPOT, new Color(120, 120, 120), ScreenManager.GraphicsDevice);
            item = _menu_tools.addItemMenu("Spring", ScreenManager.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
            item.addFocus((int) ActionTypes.PRIS_SPOT, new Color(120, 120, 120), ScreenManager.GraphicsDevice);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            _input.update(_cameraPosition);
            _menu_properties.Update(_input);
            _menu_tools.Update(_input);
            HandleInput();

            //We update the world
            //World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private void HandleInput()
        {
	    if (_actionType == ActionTypes.NONE || _menu_tools.hasItemPressed())
	    {
               _actionType = (ActionTypes)_menu_tools.getPressed();
	       _actions[(int) _actionType].init();
	       if (_actionType != ActionTypes.NONE)
                 Console.WriteLine(_actionType);
	    }
            _actionType = _actions[(int)_actionType].run(_input, _robot, ref _selected);
            if (_input.pressed(MouseKeys.MIDDLE) || _input.pressed(Keys.LeftShift))
                _cameraPosition += _input.mouseOffset();
        }

        public override void Draw(GameTime gameTime)
        {
            _draw_game.Begin(ScreenManager.GraphicsDevice.Viewport, _cameraPosition, _screenCenter);

            _background.Draw(_draw_game.Batch());
            _menu_properties.Draw(_draw_game.Batch());
            _menu_tools.Draw(_draw_game.Batch());

            _draw_game.End();

            _draw_game.Begin(ScreenManager.GraphicsDevice.Viewport, _cameraPosition, _screenCenter);
            _selected.ColorValue = Color.Red;
            _robot.drawDebug(_draw_game);
            _selected.ColorValue = Color.Black;
            _draw_game.End();

            base.Draw(gameTime);
        }

    }
}
