	using System;
using Microsoft.Xna.Framework;
using Squid;
using GUI;
using gearit.src.server;

namespace SquidXNA
{
	/// <summary>
	/// Game class
	/// </summary>
	public class Game : Microsoft.Xna.Framework.Game
	{
		public gearit.xna.ScreenManager ScreenManager;

		public Game()
		{
			IsMouseVisible = true;

			Content.RootDirectory = "Content";

			this.IsFixedTimeStep = false;

			ScreenManager = new gearit.xna.ScreenManager(this);
			ScreenManager.SetResolutionScreen(1280, 700);
			Components.Add(ScreenManager);

			this.Window.Title = "Gear It!";
		}

		protected override void Initialize()
		{
			GuiHost.Renderer = new RendererXNA(this);

			InputManager input = new InputManager(this);
			Components.Add(input);

			ScreenManager.Content.RootDirectory = "Content/GUI";

			#region styles

			Skin skin = new Skin();

			ControlStyle baseStyle = new ControlStyle();
			baseStyle.Tiling = TextureMode.Grid;
			baseStyle.Grid = new Margin(3);
			baseStyle.Texture = "button_hot.dds";
			baseStyle.Default.Texture = "button_default.dds";
			baseStyle.Pressed.Texture = "button_down.dds";
			baseStyle.SelectedPressed.Texture = "button_down.dds";
			baseStyle.Focused.Texture = "button_down.dds";
			baseStyle.SelectedFocused.Texture = "button_down.dds";
			baseStyle.Selected.Texture = "button_down.dds";
			baseStyle.SelectedHot.Texture = "button_down.dds";

			ControlStyle itemStyle = new ControlStyle(baseStyle);
			itemStyle.TextPadding = new Margin(8, 0, 8, 0);
			itemStyle.TextAlign = Alignment.MiddleLeft;

			ControlStyle buttonStyle = new ControlStyle(baseStyle);
			buttonStyle.TextPadding = new Margin(0);
			buttonStyle.TextAlign = Alignment.MiddleCenter;

			ControlStyle tooltipStyle = new ControlStyle(buttonStyle);
			tooltipStyle.TextPadding = new Margin(8);
			tooltipStyle.TextAlign = Alignment.TopLeft;

			ControlStyle inputStyle = new ControlStyle();
			inputStyle.Texture = "input_default.dds";
			inputStyle.Hot.Texture = "input_focused.dds";
			inputStyle.Focused.Texture = "input_focused.dds";
			inputStyle.TextPadding = new Margin(4);
			inputStyle.Tiling = TextureMode.Grid;
			inputStyle.Focused.Tint = ColorInt.RGBA(1, 1, 1, 1);

			ControlStyle messageBoxStyle = new ControlStyle();
			//messageBoxStyle.Focused.Texture = "input_focused.dds";
			messageBoxStyle.TextPadding = new Margin(4);
			messageBoxStyle.BackColor = ColorInt.RGBA(0.2f, 0.2f, 0.2f, 0.3f);
			messageBoxStyle.Focused.Tint = ColorInt.RGBA(1, 1, 1, 1f);
			messageBoxStyle.TextAlign = Alignment.TopLeft;


			ControlStyle windowStyle = new ControlStyle();
			windowStyle.Tiling = TextureMode.Grid;
			windowStyle.Grid = new Margin(9);
			windowStyle.Texture = "window.dds";

			ControlStyle frameStyle = new ControlStyle();
			frameStyle.Tiling = TextureMode.Grid;
			frameStyle.Grid = new Margin(4);
			frameStyle.Texture = "frame.dds";
			frameStyle.TextPadding = new Margin(8);

			ControlStyle vscrollTrackStyle = new ControlStyle();
			vscrollTrackStyle.Tiling = TextureMode.Grid;
			vscrollTrackStyle.Grid = new Margin(3);
			vscrollTrackStyle.Texture = "vscroll_track.dds";

			ControlStyle vscrollButtonStyle = new ControlStyle();
			vscrollButtonStyle.Tiling = TextureMode.Grid;
			vscrollButtonStyle.Grid = new Margin(3);
			vscrollButtonStyle.Texture = "vscroll_button.dds";
			vscrollButtonStyle.Hot.Texture = "vscroll_button_hot.dds";
			vscrollButtonStyle.Pressed.Texture = "vscroll_button_down.dds";

			ControlStyle vscrollUp = new ControlStyle();
			vscrollUp.Default.Texture = "vscrollUp_default.dds";
			vscrollUp.Hot.Texture = "vscrollUp_hot.dds";
			vscrollUp.Pressed.Texture = "vscrollUp_down.dds";
			vscrollUp.Focused.Texture = "vscrollUp_hot.dds";

			ControlStyle hscrollTrackStyle = new ControlStyle();
			hscrollTrackStyle.Tiling = TextureMode.Grid;
			hscrollTrackStyle.Grid = new Margin(3);
			hscrollTrackStyle.Texture = "hscroll_track.dds";

			ControlStyle hscrollButtonStyle = new ControlStyle();
			hscrollButtonStyle.Tiling = TextureMode.Grid;
			hscrollButtonStyle.Grid = new Margin(3);
			hscrollButtonStyle.Texture = "hscroll_button.dds";
			hscrollButtonStyle.Hot.Texture = "hscroll_button_hot.dds";
			hscrollButtonStyle.Pressed.Texture = "hscroll_button_down.dds";

			ControlStyle hscrollUp = new ControlStyle();
			hscrollUp.Default.Texture = "hscrollUp_default.dds";
			hscrollUp.Hot.Texture = "hscrollUp_hot.dds";
			hscrollUp.Pressed.Texture = "hscrollUp_down.dds";
			hscrollUp.Focused.Texture = "hscrollUp_hot.dds";

			ControlStyle checkButtonStyle = new ControlStyle();
			checkButtonStyle.Default.Texture = "checkbox_default.dds";
			checkButtonStyle.Hot.Texture = "checkbox_hot.dds";
			checkButtonStyle.Pressed.Texture = "checkbox_down.dds";
			checkButtonStyle.Checked.Texture = "checkbox_checked.dds";
			checkButtonStyle.CheckedFocused.Texture = "checkbox_checked_hot.dds";
			checkButtonStyle.CheckedHot.Texture = "checkbox_checked_hot.dds";
			checkButtonStyle.CheckedPressed.Texture = "checkbox_down.dds";
			checkButtonStyle.Default.TextPadding = new Margin(15);

			ControlStyle comboLabelStyle = new ControlStyle();
			comboLabelStyle.TextPadding = new Margin(10, 0, 0, 0);
			comboLabelStyle.Default.Texture = "combo_default.dds";
			comboLabelStyle.Hot.Texture = "combo_hot.dds";
			comboLabelStyle.Pressed.Texture = "combo_down.dds";
			comboLabelStyle.Focused.Texture = "combo_hot.dds";
			comboLabelStyle.Tiling = TextureMode.Grid;
			comboLabelStyle.Grid = new Margin(3, 0, 0, 0);

			ControlStyle comboButtonStyle = new ControlStyle();
			comboButtonStyle.Default.Texture = "combo_button_default.dds";
			comboButtonStyle.Hot.Texture = "combo_button_hot.dds";
			comboButtonStyle.Pressed.Texture = "combo_button_down.dds";
			comboButtonStyle.Focused.Texture = "combo_button_hot.dds";

			ControlStyle multilineStyle = new ControlStyle();
			multilineStyle.TextAlign = Alignment.TopLeft;
			multilineStyle.TextPadding = new Margin(8);

			ControlStyle labelStyle = new ControlStyle();
			labelStyle.TextPadding = new Margin(8, 0, 8, 0);
			labelStyle.TextAlign = Alignment.MiddleLeft;
			labelStyle.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			labelStyle.BackColor = ColorInt.RGBA(1, 1, 1, 0.1f);
			labelStyle.Default.BackColor = 0;

			ControlStyle mainMenuStyle = new ControlStyle();
			mainMenuStyle.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			mainMenuStyle.BackColor = ColorInt.RGBA(0, 0, 0, 0.6f);

			ControlStyle itemMainMenuStyle = new ControlStyle();
			itemMainMenuStyle.TextAlign = Alignment.MiddleCenter;
			itemMainMenuStyle.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			itemMainMenuStyle.BackColor = ColorInt.RGBA(1, 1, 1, 0.02f);
			itemMainMenuStyle.Default.BackColor = 0;
			itemMainMenuStyle.Selected.BackColor = ColorInt.RGBA(1, 1, 1, 0.05f);
			itemMainMenuStyle.SelectedHot.BackColor = ColorInt.RGBA(1, 1, 1, 0.05f);

			ControlStyle menuStyle = new ControlStyle();
			menuStyle.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			menuStyle.BackColor = ColorInt.RGBA(0, 0, 0, 0.528f);

			ControlStyle itemMenuStyle = new ControlStyle();
			itemMenuStyle.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			itemMenuStyle.Default.BackColor = 0;

			ControlStyle itemMenuTitleStyle = new ControlStyle();
			itemMenuTitleStyle.BackColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			itemMenuTitleStyle.TextColor = ColorInt.RGBA(0f, 0f, 0f, 1);
			itemMenuTitleStyle.TextAlign = Alignment.MiddleCenter;
			itemMenuTitleStyle.TextPadding = new Squid.Margin(8, 0, 0, 0);

			ControlStyle itemMenuButtonStyle = new ControlStyle();
			itemMenuButtonStyle.TextAlign = Alignment.MiddleCenter;
			itemMenuButtonStyle.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			itemMenuButtonStyle.BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.08f);
			itemMenuButtonStyle.Default.BackColor = ColorInt.RGBA(1, 1, 1, 0.04f);
			itemMenuButtonStyle.Checked.BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.2f);
			itemMenuButtonStyle.CheckedHot.BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.2f);

			ControlStyle eventPanelStyle = new ControlStyle();
			eventPanelStyle.TextAlign = Alignment.MiddleCenter;
			eventPanelStyle.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			eventPanelStyle.BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.08f);
			eventPanelStyle.Default.BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.08f);
			eventPanelStyle.Checked.BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.2f);
			eventPanelStyle.CheckedHot.BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.2f);

			ControlStyle treeNodeTextStyle = new ControlStyle();
			treeNodeTextStyle.TextAlign = Alignment.MiddleCenter;
			treeNodeTextStyle.BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.0f);

			ControlStyle addEventButtonStyle = new ControlStyle();
			addEventButtonStyle.TextAlign = Alignment.MiddleCenter;
			addEventButtonStyle.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			addEventButtonStyle.BackColor = ColorInt.RGBA(0.5f, 1, 0.5f, 0.15f);
			addEventButtonStyle.Default.BackColor = ColorInt.RGBA(0.5f, 1, 0.5f, 0.1f);

			skin.Styles.Add("item", itemStyle);
			skin.Styles.Add("textbox", inputStyle);
			skin.Styles.Add("messagebox", messageBoxStyle);
			skin.Styles.Add("button", buttonStyle);
			skin.Styles.Add("window", windowStyle);
			skin.Styles.Add("frame", frameStyle);
			skin.Styles.Add("checkBox", checkButtonStyle);
			skin.Styles.Add("comboLabel", comboLabelStyle);
			skin.Styles.Add("comboButton", comboButtonStyle);
			skin.Styles.Add("vscrollTrack", vscrollTrackStyle);
			skin.Styles.Add("vscrollButton", vscrollButtonStyle);
			skin.Styles.Add("vscrollUp", vscrollUp);
			skin.Styles.Add("hscrollTrack", hscrollTrackStyle);
			skin.Styles.Add("hscrollButton", hscrollButtonStyle);
			skin.Styles.Add("hscrollUp", hscrollUp);
			skin.Styles.Add("multiline", multilineStyle);
			skin.Styles.Add("tooltip", tooltipStyle);
			skin.Styles.Add("label", labelStyle);
			skin.Styles.Add("mainMenu", mainMenuStyle);
			skin.Styles.Add("itemMainMenu", itemMainMenuStyle);
			skin.Styles.Add("menu", menuStyle);
			skin.Styles.Add("itemMenu", itemMenuStyle);
			skin.Styles.Add("itemMenuTitle", itemMenuTitleStyle);
			skin.Styles.Add("itemMenuButton", itemMenuButtonStyle);
			skin.Styles.Add("eventPanel", eventPanelStyle);
			skin.Styles.Add("treeNodeText", treeNodeTextStyle);
			skin.Styles.Add("addEventButton", addEventButtonStyle);

			GuiHost.SetSkin(skin);

			#endregion

			#region cursors

			Squid.Point cursorSize = new Squid.Point(32, 32);
			Squid.Point halfSize = cursorSize / 2;

			skin.Cursors.Add(Cursors.Default, new Cursor { Texture = "cursors\\Arrow.png", Size = cursorSize, HotSpot = Squid.Point.Zero });
			skin.Cursors.Add(Cursors.Link, new Cursor { Texture = "cursors\\Link.png", Size = cursorSize, HotSpot = Squid.Point.Zero });
			skin.Cursors.Add(Cursors.Move, new Cursor { Texture = "cursors\\Move.png", Size = cursorSize, HotSpot = halfSize });
			skin.Cursors.Add(Cursors.Select, new Cursor { Texture = "cursors\\Select.png", Size = cursorSize, HotSpot = halfSize });
			skin.Cursors.Add(Cursors.SizeNS, new Cursor { Texture = "cursors\\SizeNS.png", Size = cursorSize, HotSpot = halfSize });
			skin.Cursors.Add(Cursors.SizeWE, new Cursor { Texture = "cursors\\SizeWE.png", Size = cursorSize, HotSpot = halfSize });
			skin.Cursors.Add(Cursors.HSplit, new Cursor { Texture = "cursors\\SizeNS.png", Size = cursorSize, HotSpot = halfSize });
			skin.Cursors.Add(Cursors.VSplit, new Cursor { Texture = "cursors\\SizeWE.png", Size = cursorSize, HotSpot = halfSize });
			skin.Cursors.Add(Cursors.SizeNESW, new Cursor { Texture = "cursors\\SizeNESW.png", Size = cursorSize, HotSpot = halfSize });
			skin.Cursors.Add(Cursors.SizeNWSE, new Cursor { Texture = "cursors\\SizeNWSE.png", Size = cursorSize, HotSpot = halfSize });

			#endregion

			ScreenManager.Content.RootDirectory = "Content";

			base.Initialize();

			ScreenManager.AddScreen(new ScreenMainMenu());


            // To remove
            NetworkServer.Start(25552);
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GuiHost.TimeElapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			base.Draw(gameTime);
		}

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            // Stop the threads
            NetworkServer.Stop();
        }
	}
}
