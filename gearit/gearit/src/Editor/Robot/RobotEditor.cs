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
        NONE = 0,
	MAIN_SELECT,
	SELECT2,
	DELETE_PIECE,
	MOVE_PIECE,
        PRIS_SPOT,
        REV_SPOT,
	SHOW_ALL,
	HIDE,
        LAUNCH,
        MOVE_ANCHOR,
	DELETE_SPOT,
	PRIS_LINK,
	REV_LINK,
	RESIZE_WHEEL,
	RESIZE_HEART,
        COUNT
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
        private Piece _mainSelected;
        private Piece _selected2;
        private ActionTypes _actionType;
        private IAction[] _actions = new IAction[(int) ActionTypes.COUNT];
        private int _time = 0;

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
            _mainSelected = _robot.getHeart();
            _selected2 = _robot.getHeart();
            _actionType = ActionTypes.NONE;

	    //actions
            _actions[(int) ActionTypes.NONE] = null;
            _actions[(int) ActionTypes.MAIN_SELECT] = new ActionMainSelect();
            _actions[(int) ActionTypes.SELECT2] = new ActionSelect2();
            _actions[(int) ActionTypes.DELETE_PIECE] = new ActionDeletePiece();
            _actions[(int) ActionTypes.MOVE_PIECE] = new ActionMovePiece();
            _actions[(int) ActionTypes.PRIS_SPOT] = new ActionPrisSpot();
            _actions[(int) ActionTypes.REV_SPOT] = new ActionRevSpot();
            _actions[(int) ActionTypes.SHOW_ALL] = new ActionShowAll();
            _actions[(int) ActionTypes.HIDE] = new ActionHide();
            _actions[(int) ActionTypes.LAUNCH] = new ActionLaunch();
            _actions[(int) ActionTypes.MOVE_ANCHOR] = new ActionMoveAnchor();
            _actions[(int) ActionTypes.DELETE_SPOT] = new ActionDeleteSpot();
            _actions[(int) ActionTypes.PRIS_LINK] = new ActionPrisLink();
	    _actions[(int) ActionTypes.REV_LINK] = new ActionRevLink();
            _actions[(int) ActionTypes.RESIZE_WHEEL] = new ActionResizeWheel();
            _actions[(int) ActionTypes.RESIZE_HEART] = new ActionResizeHeart();

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
            item = _menu_tools.addItemMenu("Launch", ScreenManager.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
            item.addFocus((int)ActionTypes.LAUNCH, new Color(120, 180, 120), ScreenManager.GraphicsDevice);

            _menu_tools.Adjusting = true;
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            _time++;
            _input.update(_cameraPosition);
            _menu_properties.Update(_input);
            _menu_tools.Update(_input);
            HandleInput();

            //We update the world
	    // /!\ Faut pas pas faire ça, c'est deja fait dans base.Update(...)
            //World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private bool runShortcut()
        {
            for (int i = 1; i < (int)ActionTypes.COUNT; i++)
                if (_actions[i].shortcut(_input))
                {
                    _actionType = (ActionTypes)i;
                    return (true);
                }
            return (false);
        }

        private void HandleInput()
        {
            if (_actionType == ActionTypes.NONE)
            {
                if (_menu_tools.hasItemPressed() && false) // enlever false pour que le menu marche
                {
                    _actionType = (ActionTypes)_menu_tools.getPressed();
                    _actions[(int)_actionType].init();
                }
                else if (runShortcut())
                    _actions[(int)_actionType].init();
            }
            else if (_actions[(int)_actionType].run(_input, _robot, ref _mainSelected, ref _selected2) == false)
            {
	       // à decomenter pour avoir un menu effectif.
               // _menu_tools.getItem((int)_actionType).Pressed = false;
                _actionType = ActionTypes.NONE;
            }
            if (_input.pressed(MouseKeys.MIDDLE) || (_input.pressed(Keys.V)))
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

            if (_selected2 == _mainSelected)
                _selected2.ColorValue = Color.Violet;
            else
            {
                _selected2.ColorValue = Color.Blue;
                _mainSelected.ColorValue = Color.Red;
            }
            if (_mainSelected.isConnected(_selected2))
                _mainSelected.getConnection(_selected2).ColorValue = new Color(255, (_time * 10) % 255, (_time * 10) % 255);

            _robot.drawDebug(_draw_game);

	    if (_mainSelected.isConnected(_selected2))
              _mainSelected.getConnection(_selected2).ColorValue = Color.Black;
            _selected2.ColorValue = Color.Black;
            _mainSelected.ColorValue = Color.Black;
            _draw_game.End();

            base.Draw(gameTime);
        }

    }
}
