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

namespace gearit.src.editor.map
{
    class MapEditor : GameScreen, IDemoScreen
    {
        private World _world;
        private Input _input;
        private EditorCamera _camera;
        private const int PropertiesMenuSize = 200;
        private DrawGame _draw_game;
        private RectangleOverlay _background;
        private MenuOverlay _menu_tools;

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

            ScreenManager.Game.ResetElapsedTime();
            _camera = new EditorCamera(ScreenManager.GraphicsDevice);
            _camera.Position = new Vector2(0, 0);
            _input = new Input(_camera);
            _world.Gravity = new Vector2(0f, 0f);
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
            _world.Step(0f);
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
            
            _draw_game.Begin(ScreenManager.GraphicsDevice.Viewport, _camera);
            _background.Draw(_draw_game.Batch());

            _menu_tools.Draw(_draw_game.Batch());

            _draw_game.End();
            base.Draw(gameTime);
        }

    }
}
