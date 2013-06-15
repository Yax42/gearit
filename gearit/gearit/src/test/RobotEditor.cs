using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using Microsoft.Xna.Framework.Graphics;

namespace gearit.src.utility
{
    class RobotEditor : PhysicsGameScreen, IDemoScreen
    {
        private Matrix _view;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;
        private const int PropertiesMenuSize = 200;

        // Graphic
        private RectangleOverlay _background;
        private MenuOverlay _menu_properties;

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

            World.Gravity = new Vector2(0f, 8f);
            HasCursor = true;
            EnableCameraControl = true;
            HasVirtualStick = true;

            // Initialize camera controls
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;
            _screenCenter = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2f, ScreenManager.GraphicsDevice.Viewport.Height / 2f);

            // Graphic
            Rectangle rec = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width - PropertiesMenuSize, ScreenManager.GraphicsDevice.Viewport.Height);
            _background = new RectangleOverlay(rec, new Color(126, 126, 126, 245), ScreenManager.GraphicsDevice);

            // Menu
            Vector2 pos = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width - PropertiesMenuSize, 0);
            Vector2 size = new Vector2(PropertiesMenuSize, ScreenManager.GraphicsDevice.Viewport.Height);
            _menu_properties = new MenuOverlay(ScreenManager.GraphicsDevice, ScreenManager.Content, pos, size, new Color(120, 120, 120, 245), MenuLayout.Vertical);
            _menu_properties.addItemMenu("Properties", ScreenManager.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.Default, 1.5f);
            _menu_properties.addItemMenu("Common/stick", new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.HorizontalCenter, 1.5f);
            _menu_properties.addItemMenu("TEST", ScreenManager.Fonts.DetailsFont, Color.Red, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.Right, 1f);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            HandleKeyboard();

            //We update the world
            World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private void HandleKeyboard()
        {
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteFont font = ScreenManager.Fonts.DetailsFont;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString("Properties") * 1.5f;
            Vector2 textPosition = new Vector2(viewportSize.X - PropertiesMenuSize / 2 - textSize.X / 2f, 10f);

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);
            _background.Draw(ScreenManager.SpriteBatch);
            _menu_properties.Draw(ScreenManager.SpriteBatch);
            //ScreenManager.SpriteBatch.DrawString(font, "Properties", textPosition, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
