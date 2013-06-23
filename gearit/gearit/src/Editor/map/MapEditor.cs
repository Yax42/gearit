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
        NONE = 0,
        MOVE,
        ROTATE,
        DELETE,
        PLACE
    }
    class MapEditor : GameScreen, IDemoScreen
    {
        private World _world;
        private Map _map;
        private EditorCamera _camera;
        private DrawGame _draw_game;
        private MenuOverlay _menu_tools;
        private MenuOverlay _menu_properties;
        private Mode _mode;
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
            _world.Gravity = new Vector2(0f, 0f);
            HasVirtualStick = true;
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;
            _screenCenter = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2f,
                                                ScreenManager.GraphicsDevice.Viewport.Height / 2f);

            _draw_game = new DrawGame(ScreenManager.GraphicsDevice);
            Rectangle rec = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);

            // Menu
            MenuItem item;
            Vector2 pos = new Vector2(0, 50);
            Vector2 size = new Vector2(PropertiesMenuSize, ScreenManager.GraphicsDevice.Viewport.Height - 28);
            _menu_properties = new MenuOverlay(ScreenManager, pos, size, Color.LightSteelBlue, MenuLayout.Vertical);
            item = new SpriteMenuItem(_menu_properties, "EditorIcon/rotate", new Vector2(1), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
            item.addFocus((int)Mode.ROTATE, new Color(120, 120, 120));

            pos.X = 0;
            pos.Y = 0;
            size = new Vector2(400, 50);
            _menu_tools = new MenuOverlay(ScreenManager, pos, size, Color.LightGray, MenuLayout.Horizontal);
            item = new TextMenuItem(_menu_tools, "New", ScreenManager.Fonts.DetailsFont, Color.Black, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
            item = new TextMenuItem(_menu_tools, "Open", ScreenManager.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);
            item = new TextMenuItem(_menu_tools, "Save", ScreenManager.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter, 1.5f);

            _menu_tools.Adjusting = true;
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            Input.update();
            _camera.flush();

            _menu_tools.Update();
            _menu_properties.Update();
            HandleInput();
            _world.Step(0f);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private void HandleInput()
        {
            /* test */
            MenuItem pressed;
            if ((pressed = _menu_properties.justPressed()) != null)
            {
                _mode = (Mode)pressed.Id;
            }
            /**/

            if (Input.pressed(Keys.Escape))
            {
                ScreenManager.RemoveScreen(this);
            }
            if (Input.pressed(Keys.P))
            {
                _mode = Mode.PLACE;
            }
            if (Input.pressed(Keys.D))
            {
                _mode = Mode.DELETE;
            }
            if (Input.pressed(Keys.R))
            {
                _mode = Mode.ROTATE;
            }
            if (Input.pressed(Keys.M))
            {
                _mode = Mode.MOVE;
            }

            if (Input.justPressed(MouseKeys.LEFT))
            {
                switch (_mode)
                {
                    case Mode.MOVE:
                        break;
                    case Mode.PLACE:
                        _map.addBody(BodyFactory.CreateRectangle(_world, 8f, 0.5f, 1f, Input.SimMousePos));
                        break;
                    case Mode.ROTATE:
                        break;
                    case Mode.DELETE:
                        _map.getBodies().Remove(_map.getBody(Input.SimMousePos));
                        break;
                }
            }
            if (Input.pressed(MouseKeys.LEFT) && _mode == Mode.ROTATE)
            {
                Body tmp = _map.getBody(Input.SimMousePos);
                if (tmp != null)
                {
                    tmp.Rotation += 0.02f;
                }
            }
            if (Input.pressed(MouseKeys.RIGHT) && _mode == Mode.ROTATE)
            {
                Body tmp = _map.getBody(Input.SimMousePos);
                if (tmp != null)
                {
                    tmp.Rotation -= 0.02f;
                }
            }

            if (Input.pressed(MouseKeys.MIDDLE) || (Input.pressed(Keys.V)))
                _camera.move(Input.mouseOffset());
            if (Input.justPressed(MouseKeys.WHEEL_DOWN))
                _camera.zoomIn();
            if (Input.justPressed(MouseKeys.WHEEL_UP))
                _camera.zoomOut();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.LightSeaGreen);
            _draw_game.Begin(_camera);
            _map.drawDebug(_draw_game);
            _menu_tools.draw(_draw_game);
            _menu_properties.draw(_draw_game);
            _draw_game.End();
            base.Draw(gameTime);
        }

    }
}
