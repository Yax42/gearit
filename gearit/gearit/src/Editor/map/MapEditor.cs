using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using gearit.src.utility.Menu;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using gearit.src.editor.robot;
using FarseerPhysics.Factories;
using gearit.src.map;

namespace gearit.src.editor.map
{
    enum Mode
    {
        MOVE = 0,
        ROTATE,
        DELETE,
        PLACE
    }
    class MapEditor : GameScreen, IDemoScreen
    {
        private World               _world;
        private Map                 _map;
        private Input               _input;
        private EditorCamera        _camera;
        private DrawGame            _draw_game;
        private RectangleOverlay    _background;
        private MenuOverlay         _menu_tools;
        private MenuOverlay         _menu_properties;
        private Mode                _mode;
        private const int PropertiesMenuSize = 50;

        private Matrix _view;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Map Editor";
        }

        public string GetDetails()
        {
            return "";
        }

        #endregion

        public MapEditor()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.75);
            TransitionOffTime = TimeSpan.FromSeconds(0.75);
            _world = null;
            HasCursor = true;
        }

        public override void LoadContent()
        {
            base.LoadContent();


            if (_world == null)
                _world = new World(Vector2.Zero);
            else
                _world.Clear();
            _map = new Map(_world);
            _mode = Mode.PLACE;
            ScreenManager.Game.ResetElapsedTime();
            _camera = new EditorCamera(ScreenManager.GraphicsDevice);
            _camera.Position = new Vector2(0, 0);
            _input = new Input(_camera);
            _world.Gravity = new Vector2(0f, 0f);
            HasVirtualStick = true;
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;
            _screenCenter = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2f,
                                                ScreenManager.GraphicsDevice.Viewport.Height / 2f);

            _draw_game = new DrawGame(ScreenManager.GraphicsDevice);
            Rectangle rec = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
            _background = new RectangleOverlay(rec, Color.LightSkyBlue, ScreenManager.GraphicsDevice);

            // Menu
            MenuItem item;
            Vector2 pos = new Vector2(0, 50);
            Vector2 size = new Vector2(PropertiesMenuSize, ScreenManager.GraphicsDevice.Viewport.Height - 28);
            _menu_properties = new MenuOverlay(ScreenManager, pos, size, Color.LightSteelBlue, MenuLayout.Vertical);
            item = new InputMenuItem(_menu_properties, 1, new Vector2(8), new Vector2(48, 48), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.HorizontalCenter | ItemMenuAlignement.VerticalCenter, 1f);
            pos.X = 0;
            pos.Y = 0;
            size = new Vector2(400, 50);
            _menu_tools = new MenuOverlay(ScreenManager, pos, size, Color.LightGray, MenuLayout.Horizontal);
            item = new TextMenuItem(_menu_tools, "New", ScreenManager.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
            item.addFocus((int)ActionTypes.REV_SPOT, new Color(120, 120, 120));
            item = new TextMenuItem(_menu_tools, "Open", ScreenManager.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
            item.addFocus((int)ActionTypes.PRIS_SPOT, new Color(120, 120, 120));
            item = new TextMenuItem(_menu_tools, "Save", ScreenManager.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
            item.addFocus((int)ActionTypes.LAUNCH, new Color(120, 180, 120));

            _menu_tools.Adjusting = true;
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            _camera.flush();
            _input.update();
            _menu_tools.Update(_input);
            HandleInput();
            _world.Step(0f);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private void HandleInput()
        {
            if (_input.pressed(Keys.Escape))
            {
                ScreenManager.RemoveScreen(this);
            }
            if (_input.pressed(Keys.P))
            {
                _mode = Mode.PLACE;
            }
            if (_input.pressed(Keys.D))
            {
                _mode = Mode.DELETE;
            }
            if (_input.pressed(Keys.R))
            {
                _mode = Mode.ROTATE;
            }
            if (_input.pressed(Keys.M))
            {
                _mode = Mode.MOVE;
            }

            if (_input.justPressed(MouseKeys.LEFT))
            {
                switch (_mode)
                {
                    case Mode.MOVE:
                        break;
                    case Mode.PLACE:
                        _map.addBody(BodyFactory.CreateRectangle(_world, 8f, 0.5f, 1f, _input.simUnitPosition()));
                        break;
                    case Mode.ROTATE:
                        break;
                    case Mode.DELETE:
                        for (int i = 0; i < _map.getBodies().Count(); i++)
                        {
                            _map.getBodies().Remove(_map.getBody(_input.simUnitPosition()));
                        }
                        break;
                }
            }
            if (_input.pressed(MouseKeys.MIDDLE) || (_input.pressed(Keys.V)))
                _camera.move(_input.mouseOffset() / _camera.Zoom);
            if (_input.justPressed(MouseKeys.WHEEL_DOWN))
                _camera.Zoom *= 2;
            if (_input.justPressed(MouseKeys.WHEEL_UP))
                _camera.Zoom /= 2;
        }

        public override void Draw(GameTime gameTime)
        {
            
            _draw_game.Begin(ScreenManager.GraphicsDevice.Viewport, _camera);
            _background.Draw(_draw_game.Batch());
            _draw_game.End();
            _draw_game.Begin(ScreenManager.GraphicsDevice.Viewport, _camera);
            _map.drawDebug(_draw_game);
            _menu_tools.Draw(_draw_game.Batch());
            _menu_properties.Draw(_draw_game.Batch());
            _draw_game.End();
            base.Draw(gameTime);
        }

    }
}
