using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility.Menu;
using gearit.xna;
using Microsoft.Xna.Framework;

namespace gearit.src.editor.robot
{
    class MenuPiece
    {
        MenuOverlay _menu_tools;
        MenuOverlay _menu_properties;
        ScreenManager _screen;
        private const int MenuWidth = 200;
        private const int ToolsMenuHeight = 50;

        public MenuPiece(ScreenManager screen)
        {
            _screen = screen;
            InputMenuItem input_item;

            Vector2 pos = new Vector2(_screen.GraphicsDevice.Viewport.Width - MenuWidth, 0);
            Vector2 size;

            Color bg_focus = new Color(180, 180, 180);
            Color bg_pressed = new Color(120, 120, 120);

            // Menu tools
            MenuItem item;
            size = new Vector2(MenuWidth, 50);
            _menu_tools = new MenuOverlay(_screen, pos, size, Color.LightGray, MenuLayout.Horizontal);
            item = new TextMenuItem(_menu_tools, "Rotation", _screen.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter | ItemMenuAlignement.HorizontalCenter, 1.5f);
            item.addFocus((int)ActionTypes.REV_SPOT, bg_focus, bg_pressed);
            item = new TextMenuItem(_menu_tools, "Spring", _screen.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter | ItemMenuAlignement.HorizontalCenter, 1.5f);
            item.addFocus((int)ActionTypes.PRIS_SPOT, bg_focus, bg_pressed);
            item = new TextMenuItem(_menu_tools, "Launch", _screen.Fonts.DetailsFont, Color.White, new Vector2(8), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.VerticalCenter | ItemMenuAlignement.HorizontalCenter, 1.5f);
            item.addFocus((int)ActionTypes.LAUNCH, bg_focus, bg_pressed);

            // Menu properties
            pos += new Vector2(0, ToolsMenuHeight);
            size = new Vector2(MenuWidth, _screen.GraphicsDevice.Viewport.Height);
            _menu_properties = new MenuOverlay(_screen, pos, size, Color.LightGray, MenuLayout.Vertical);
            input_item = new InputMenuItem(_menu_properties, _screen.Fonts.DetailsFont, Color.Black, 1, new Vector2(8), new Vector2(100, 28), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.HorizontalCenter | ItemMenuAlignement.VerticalCenter, 1f);
            input_item.addFocus(42, new Color(180, 180, 180), new Color(140, 140, 140));
            input_item.setLabel("Test", _screen.Fonts.DetailsFont, Color.Black);

            _menu_tools.Adjusting = true;
         }

        public void Update()
        {
            _menu_properties.Update();
            _menu_tools.Update();
        }

        public void Draw(DrawGame draw_game)
        {
            draw_game.Begin();
            _menu_properties.draw(draw_game);
            _menu_tools.draw(draw_game);
            draw_game.End();
        }
    }
}
