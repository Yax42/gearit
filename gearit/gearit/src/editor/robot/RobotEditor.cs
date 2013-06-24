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
using FarseerPhysics.Dynamics;
using gearit.src.utility.Menu;

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
        CHOOSE_SET,
        MOVE_ROBOT,
        LOAD_ROBOT,
        SAVE_ROBOT,
        COUNT
    }

    class RobotEditor : GameScreen, IDemoScreen
    {
        static public World _world; // (static public) est temporaire
        private EditorCamera _camera;
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
        private IAction[] _actions = new IAction[(int)ActionTypes.COUNT];
        private int _time = 0;

        public RobotEditor()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.75);
            TransitionOffTime = TimeSpan.FromSeconds(0.75);
            HasCursor = true;
            _world = null;
        }

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

            if (_world == null)
                _world = new World(Vector2.Zero);
            else
                _world.Clear();

            // Loading may take a while... so prevent the game from "catching up" once we finished loading
            ScreenManager.Game.ResetElapsedTime();

            // Initialize camera controls
            _camera = new EditorCamera(ScreenManager.GraphicsDevice);
            _camera.Position = new Vector2(0, 0);
            //_screenCenter = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2f, ScreenManager.GraphicsDevice.Viewport.Height / 2f);

            _world.Gravity = new Vector2(0f, 0f);
            HasCursor = true;
            HasVirtualStick = true;

            SerializerHelper.World = _world;

            // Robot
            _draw_game = new DrawGame(ScreenManager.GraphicsDevice);
            _robot = new Robot(_world);
            _mainSelected = _robot.getHeart();
            _selected2 = _robot.getHeart();
            _actionType = ActionTypes.NONE;

            //actions
            _actions[(int)ActionTypes.NONE] = null;
            _actions[(int)ActionTypes.MAIN_SELECT] = new ActionMainSelect();
            _actions[(int)ActionTypes.SELECT2] = new ActionSelect2();
            _actions[(int)ActionTypes.DELETE_PIECE] = new ActionDeletePiece();
            _actions[(int)ActionTypes.MOVE_PIECE] = new ActionMovePiece();
            _actions[(int)ActionTypes.PRIS_SPOT] = new ActionPrisSpot();
            _actions[(int)ActionTypes.REV_SPOT] = new ActionRevSpot();
            _actions[(int)ActionTypes.SHOW_ALL] = new ActionShowAll();
            _actions[(int)ActionTypes.HIDE] = new ActionHide();
            _actions[(int)ActionTypes.LAUNCH] = new ActionLaunch();
            _actions[(int)ActionTypes.MOVE_ANCHOR] = new ActionMoveAnchor();
            _actions[(int)ActionTypes.DELETE_SPOT] = new ActionDeleteSpot();
            _actions[(int)ActionTypes.PRIS_LINK] = new ActionPrisLink();
            _actions[(int)ActionTypes.REV_LINK] = new ActionRevLink();
            _actions[(int)ActionTypes.RESIZE_WHEEL] = new ActionResizePiece();
            _actions[(int)ActionTypes.CHOOSE_SET] = new ActionChooseSet();
            _actions[(int)ActionTypes.MOVE_ROBOT] = new ActionMoveRobot();
            _actions[(int)ActionTypes.SAVE_ROBOT] = new ActionSaveRobot();
            _actions[(int)ActionTypes.LOAD_ROBOT] = new ActionLoadRobot();


            // Menu
            InputMenuItem input_item;

            Vector2 pos = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width - PropertiesMenuSize, 0);
            Vector2 size = new Vector2(PropertiesMenuSize, ScreenManager.GraphicsDevice.Viewport.Height);
            _menu_properties = new MenuOverlay(ScreenManager, pos, size, Color.LightGray, MenuLayout.Vertical);
            input_item = new InputMenuItem(_menu_properties, ScreenManager.Fonts.DetailsFont, Color.Black, 1, new Vector2(8), new Vector2(100, 28), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.HorizontalCenter | ItemMenuAlignement.VerticalCenter, 1f);
            input_item.addFocus(42, new Color(180, 180, 180), new Color(140, 140, 140));
            input_item.setLabel("Test", ScreenManager.Fonts.DetailsFont, Color.Black);

            MenuItem item;
            pos.X = 200;
            size = new Vector2(400, 50);
            _menu_tools = new MenuOverlay(ScreenManager, pos, size, Color.LightGray, MenuLayout.Horizontal);
            item = new TextMenuItem(_menu_tools, "Rotation", ScreenManager.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter | ItemMenuAlignement.HorizontalCenter, 1.5f);
            //item.addFocus((int)ActionTypes.REV_SPOT, new Color(120, 120, 120));
            item = new TextMenuItem(_menu_tools, "Spring", ScreenManager.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter | ItemMenuAlignement.HorizontalCenter, 1.5f);
            //item.addFocus((int)ActionTypes.PRIS_SPOT, new Color(120, 120, 120));
            item = new TextMenuItem(_menu_tools, "Launch", ScreenManager.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter | ItemMenuAlignement.HorizontalCenter, 1.5f);
            //item.addFocus((int)ActionTypes.LAUNCH, new Color(120, 180, 120));

            _menu_tools.Adjusting = true;
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            _time++;
            Input.update();
            _camera.update();
            _menu_properties.Update();
            _menu_tools.Update();
            HandleInput();

            // Permet d'update le robot sans le faire bouger (vu qu'il avance de zéro secondes dans le temps)
            _world.Step(0f);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private bool runShortcut()
        {
            for (int i = 1; i < (int)ActionTypes.COUNT; i++)
                if (_actions[i].shortcut())
                {
                    _actionType = (ActionTypes)i;
                    return (true);
                }
            return (false);
        }

        private void HandleInput()
        {
            if (Input.pressed(Keys.Escape))
                ScreenManager.RemoveScreen(this);
            if (_actionType == ActionTypes.NONE)
            {
                MenuItem pressed;
                if ((pressed = _menu_tools.justPressed()) != null)
                {
                    _actionType = (ActionTypes)pressed.Id;
                    _actions[(int)_actionType].init();
                }
                else if (_menu_properties.isMouseOn() || _menu_properties.hasItemPressed() || _menu_tools.isMouseOn() || _menu_properties.hasItemPressed())
                    return;
                else if (runShortcut())
                    _actions[(int)_actionType].init();
            }
            else if (_actions[(int)_actionType].run(ref _robot, ref _mainSelected, ref _selected2) == false)
            {
                // à decomenter pour avoir un menu effectif.
                //_menu_tools.getItem((int)_actionType).Pressed = false;
                _actionType = ActionTypes.NONE;
            }
            _camera.input();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.LightSkyBlue);

            _draw_game.Begin(_camera);
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

            _draw_game.Begin();
            _menu_properties.draw(_draw_game);
            _menu_tools.draw(_draw_game);
            _draw_game.End();

            base.Draw(gameTime);
        }
    }
}