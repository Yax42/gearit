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
    class MapEditor : GameScreen, IDemoScreen
    {
        private Map                 _map;
        private Input               _input;
        private EditorCamera        _camera;
        private DrawGame            _draw_game;
        private RectangleOverlay    _background;
        private MenuOverlay         _menu_tools;
        private const int PropertiesMenuSize = 200;

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
            _map = new Map();
            TransitionOnTime = TimeSpan.FromSeconds(0.75);
            TransitionOffTime = TimeSpan.FromSeconds(0.75);
            _map.world = null;
            HasCursor = true;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (_map.world == null)
                _map.world = new World(Vector2.Zero);
            else
                _map.world.Clear();

            ScreenManager.Game.ResetElapsedTime();
            _camera = new EditorCamera(ScreenManager.GraphicsDevice);
            _camera.Position = new Vector2(0, 0);
            _input = new Input(_camera);
            _map.world.Gravity = new Vector2(0f, 0f);
            HasVirtualStick = true;

            _draw_game = new DrawGame(ScreenManager.GraphicsDevice);
            Rectangle rec = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
            _background = new RectangleOverlay(rec, Color.LightSkyBlue, ScreenManager.GraphicsDevice);

            // Menu
            MenuItem item;
            Vector2 pos = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width - PropertiesMenuSize, 0);
            Vector2 size = new Vector2(PropertiesMenuSize, ScreenManager.GraphicsDevice.Viewport.Height);
            pos.X = 0;
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
            _map.world.Step(0f);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private void HandleInput()
        {
            if (_input.pressed(Keys.Escape))
            {
                ScreenManager.RemoveScreen(this);
            }
            /*
            if (_input.pressed(MouseKeys.LEFT))
            {
                Body tmp = BodyFactory.CreateRectangle(_map.world, 8f, 0.5f, 1f, new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
               // _ground_tex = _asset.TextureFromShape(tmp.FixtureList[0].Shape, MaterialType.Blank, Color.LightGreen, 1f);
                _map.addBody(tmp);
            }
             * */
        }

        public override void Draw(GameTime gameTime)
        {
            
            _draw_game.Begin(ScreenManager.GraphicsDevice.Viewport, _camera);
            _background.Draw(_draw_game.Batch());

            _menu_tools.Draw(_draw_game.Batch());

            _draw_game.End();
            base.Draw(gameTime);
        }

    }
}
