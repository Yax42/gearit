using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility.Menu;
using gearit.xna;
using Microsoft.Xna.Framework;
using gearit.src.robot;

namespace gearit.src.editor.robot
{

    enum Id
    {
        
    }

    class MenuPiece
    {
        MenuOverlay _menu_tools;
        MenuOverlay _menu_properties;
        ScreenManager _screen;
        private const int MenuWidth = 200;
        private const int ToolsMenuHeight = 50;
        private InputMenuItem _p_weight;
        private TextMenuItem _p_size;
        private TextMenuItem _p_position;
        private TextMenuItem _r_rotation;
        private TextMenuItem _t_spot;
        private InputMenuItem _s_name;
        private InputMenuItem _s_maxforce;
        private TextMenuItem _s_size;
        

        public MenuPiece(ScreenManager screen)
        {
            _screen = screen;
            InputMenuItem input_item;

            Vector2 pos = new Vector2(_screen.GraphicsDevice.Viewport.Width - MenuWidth, 0);
            Vector2 size;

            Color bg_focus = new Color(180, 180, 180);
            Color bg_pressed = new Color(120, 120, 120);
            Color c_text = Color.Black;
            Vector2 padding = new Vector2(8);
            ItemMenuAlignement align = ItemMenuAlignement.HorizontalCenter | ItemMenuAlignement.VerticalCenter;

            // Menu tools
            MenuItem item;
            size = new Vector2(MenuWidth, ToolsMenuHeight);
            _menu_tools = new MenuOverlay(_screen, pos, size, new Color(200, 200, 200), MenuLayout.Horizontal);
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

            // Piece
            new TextMenuItem(_menu_properties, "Piece", _screen.Fonts.DetailsFont, new Color(150, 100, 100), new Vector2(16), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.Default, 1.5f);
            _p_weight = new InputMenuItem(_menu_properties, _screen.Fonts.DetailsFont, Color.Black, 1, padding, new Vector2(100, 28), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.HorizontalCenter | ItemMenuAlignement.VerticalCenter, 1f);
            _p_weight.addFocus(42, new Color(180, 180, 180), new Color(140, 140, 140));
            _p_weight.setLabel("Weight", _screen.Fonts.DetailsFont, Color.Black);
            _p_size = new TextMenuItem(_menu_properties, "Size", _screen.Fonts.DetailsFont, c_text, padding, ItemMenuLayout.MaxFromMin, align, 1f);
            _p_position = new TextMenuItem(_menu_properties, "Position", _screen.Fonts.DetailsFont, c_text, padding, ItemMenuLayout.MaxFromMin, align, 1f);

            // Rod
            _r_rotation = new TextMenuItem(_menu_properties, "Rotation", _screen.Fonts.DetailsFont, c_text, padding, ItemMenuLayout.MaxFromMin, align, 1f);

            // Ispot
            _t_spot = new TextMenuItem(_menu_properties, "Spot", _screen.Fonts.DetailsFont, new Color(150, 100, 100), new Vector2(16), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.Default, 1.5f);
            _s_name = new InputMenuItem(_menu_properties, _screen.Fonts.DetailsFont, Color.Black, 1, padding, new Vector2(100, 28), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.HorizontalCenter | ItemMenuAlignement.VerticalCenter, 1f);
            _s_name.addFocus(42, new Color(180, 180, 180), new Color(140, 140, 140));
            _s_name.setLabel("Name", _screen.Fonts.DetailsFont, Color.Black);
            _s_maxforce = new InputMenuItem(_menu_properties, _screen.Fonts.DetailsFont, Color.Black, 1, padding, new Vector2(100, 28), ItemMenuLayout.MaxFromMin, ItemMenuAlignement.HorizontalCenter | ItemMenuAlignement.VerticalCenter, 1f);
            _s_maxforce.addFocus(42, new Color(180, 180, 180), new Color(140, 140, 140));
            _s_maxforce.setLabel("Max force", _screen.Fonts.DetailsFont, Color.Black);
            
            // Prismatic spot
            _s_size = new TextMenuItem(_menu_properties, "Size", _screen.Fonts.DetailsFont, c_text, padding, ItemMenuLayout.MaxFromMin, align, 1f);

            _menu_tools.Adjusting = true;
         }

        public void Update(Piece piece, ISpot spot)
        {
            _menu_properties.Update();
            _menu_tools.Update();

            if (!hasFocus())
            {
                // Update piece
                _p_weight.Display = piece.Weight.ToString();
                _p_size.Display = "Size: " + piece.getSize().ToString();
                _p_position.Display = piece.Position.ToString();

                updateRod(piece);
                updateSpot(spot);

                _menu_properties.refreshMenu();
            }
            // Update info
            else
            {
                piece.Weight = Convert.ToSingle(_p_weight.Display);
                if (spot != null)
                {
                    spot.Name = _s_name.Display;
                    spot.MaxForce = Convert.ToSingle(_s_maxforce.Display);
                }
            }
        }

        public bool hasFocus()
        {
            return (_menu_properties.isMouseOn() || _menu_properties.hasItemPressed() || _menu_tools.isMouseOn() || _menu_properties.hasItemPressed());
        }

        private void updateSpot(ISpot spot)
        {
            // Update ISpot
            bool visible = (spot != null);

            _s_name.Visible = visible;
            _s_maxforce.Visible = visible;
            _s_size.Visible = visible;
            _t_spot.Visible = visible;

            if (visible)
            {
                _s_name.Display = spot.Name;
                _s_maxforce.Display = spot.MaxForce.ToString();

                // Check if Prismatic
                if (spot.GetType() == typeof(PrismaticSpot))
                {
                    _s_size.Display = "Distance: " + ((PrismaticSpot)spot).getSize().ToString();
                    _s_size.Visible = true;
                }
                else
                    _s_size.Visible = false;
            }
        }

        private void updateRod(Piece piece)
        {
            // Update Rod if is
            if (piece.GetType() != typeof(Rod))
                _r_rotation.Visible = false;
            else
            {
                _r_rotation.Visible = true;
                _r_rotation.Display = "Rotation: " + ((Rod)piece).Rotation.ToString();
            }
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
