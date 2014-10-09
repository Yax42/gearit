﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Squid;

namespace gearit.src.GUI.Tools
{
	static public class Theme
	{
		public class ThemeInfo
		{
			public Color Primitive;
			public Color PrimitiveDark;
			public Color Blackie;
			public Color Dark;
			public Color DarkLight;
			public Color Light;
			public Color Grayie;
			public Color Weird;
		};

		static	public Dictionary<String, ThemeInfo> Themes = new Dictionary<string,ThemeInfo>();
		static public ThemeInfo CurrentTheme;

		static public void init()
		{
			// Create basic theme
			#region themes
			ThemeInfo info = new ThemeInfo();
			info.Primitive = new Color(159, 23, 27, 255);
			info.PrimitiveDark = new Color(73, 31, 35, 255);
			info.Blackie = new Color(0, 0, 0, 255);
			info.Dark = new Color(73, 35, 33, 255);
			info.DarkLight = new Color(95, 61, 49, 255);
			info.Light = new Color(181, 126, 96, 255);
			info.Grayie = new Color(226, 212, 201, 255);
			info.Weird = new Color(164, 80, 70, 255);
			Themes.Add("Red", info);
			CurrentTheme = info;

			info = new ThemeInfo();
			info.Primitive = new Color(77, 173, 172, 255);
			info.PrimitiveDark = new Color(10, 57, 53, 255);
			info.Blackie = new Color(1, 1, 1, 255);
			info.Dark = new Color(32, 51, 55, 255);
			info.DarkLight = new Color(52, 97, 104, 255);
			info.Light = new Color(101, 161, 172, 255);
			info.Grayie = new Color(225, 210, 199, 255);
			info.Weird = new Color(84, 156, 135, 255);
			Themes.Add("Blue", info);

			#endregion

			// Create Squid Skin
			Skin skin = new Skin();

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

			#region skinfix

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

			ControlStyle messageStyle = new ControlStyle();
			messageStyle.TextAlign = Alignment.MiddleCenter;
			messageStyle.Font = "Arial14";

			ControlStyle textStyle = new ControlStyle();
			textStyle.TextPadding = new Margin(8, 0, 8, 0);
			textStyle.TextAlign = Alignment.MiddleLeft;

			ControlStyle textStyleB = new ControlStyle();
			textStyleB.TextPadding = new Margin(8, 0, 8, 0);
			textStyleB.TextAlign = Alignment.MiddleLeft;

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

			ControlStyle messageBoxStyle = new ControlStyle();
			messageBoxStyle.TextPadding = new Margin(4);
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

			ControlStyle mainMenuStyle = new ControlStyle();

			ControlStyle itemMainMenuStyle = new ControlStyle();
			itemMainMenuStyle.TextAlign = Alignment.MiddleCenter;

			ControlStyle menuStyle = new ControlStyle();

			ControlStyle itemMenuStyle = new ControlStyle();

			ControlStyle itemMenuTitleStyle = new ControlStyle();
			itemMenuTitleStyle.TextAlign = Alignment.MiddleCenter;
			itemMenuTitleStyle.TextPadding = new Squid.Margin(8, 0, 0, 0);

			ControlStyle itemMenuButtonStyle = new ControlStyle();
			itemMenuButtonStyle.TextAlign = Alignment.MiddleCenter;

			ControlStyle itemPickButtonStyle = new ControlStyle();
			itemPickButtonStyle.TextAlign = Alignment.BottomCenter;

			ControlStyle eventPanelStyle = new ControlStyle();
			eventPanelStyle.TextAlign = Alignment.MiddleCenter;

			ControlStyle treeNodeTextStyle = new ControlStyle();
			treeNodeTextStyle.TextAlign = Alignment.MiddleCenter;
			treeNodeTextStyle.BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.0f);

			ControlStyle addEventButtonStyle = new ControlStyle();
			addEventButtonStyle.TextAlign = Alignment.MiddleCenter;

			skin.Styles.Add("message", messageStyle);
			skin.Styles.Add("textwhite", textStyle);
			skin.Styles.Add("textblack", textStyleB);
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
			skin.Styles.Add("itemPickButton", itemPickButtonStyle);
			skin.Styles.Add("menu", menuStyle);
			skin.Styles.Add("itemMenu", itemMenuStyle);
			skin.Styles.Add("itemMenuTitle", itemMenuTitleStyle);
			skin.Styles.Add("itemMenuButton", itemMenuButtonStyle);
			skin.Styles.Add("eventPanel", eventPanelStyle);
			skin.Styles.Add("treeNodeText", treeNodeTextStyle);
			skin.Styles.Add("addEventButton", addEventButtonStyle);

			#endregion

			GuiHost.SetSkin(skin);
		}

		static public void setTheme(string theme)
		{
			ThemeInfo info = Themes[theme];

			Skin skin = GuiHost.GetSkin();
			ControlStyleCollection styles = skin.Styles;
			ControlStyle style;

            styles["message"].BackColor = 0;

			style = styles["addEventButton"];
			style.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			style.BackColor = ColorInt.RGBA(0.5f, 1, 0.5f, 0.15f);
			style.Default.BackColor = ColorInt.RGBA(0.5f, 1, 0.5f, 0.1f);

			style = styles["textwhite"];
            style.BackColor = 0;
            style.TextColor = ColorInt.RGBA(0.0f, 0.0f, 0.0f, 1.0f);

			style = styles["textblack"];
            style.BackColor = 0;
            style.TextColor = ColorInt.RGBA(0.0f, 0.0f, 0.0f, 1.0f);

            styles["textbox"].Focused.Tint = ColorInt.RGBA(1, 1, 1, 1);

			style = styles["messagebox"];
			style.BackColor = ColorInt.RGBA(0.2f, 0.2f, 0.2f, 0.3f);
			style.Focused.Tint = ColorInt.RGBA(1, 1, 1, 1f);

			style = styles["label"];
			style.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			style.BackColor = ColorInt.RGBA(1, 1, 1, 0.1f);
			style.Default.BackColor = 0;

			style = styles["mainMenu"];
			style.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			style.BackColor = ColorInt.RGBA(0, 0, 0, 0.6f);

			style = styles["itemMainMenu"];
			style.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			style.BackColor = ColorInt.RGBA(1, 1, 1, 0.02f);
			style.Default.BackColor = 0;
			style.Selected.BackColor = ColorInt.RGBA(1, 1, 1, 0.05f);
			style.SelectedHot.BackColor = ColorInt.RGBA(1, 1, 1, 0.05f);

			style = styles["menu"];
			style.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			style.BackColor = ColorInt.RGBA(0, 0, 0, 0.528f);

			style = styles["itemMenuTitle"];
			style.BackColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			style.TextColor = ColorInt.RGBA(0f, 0f, 0f, 1);

			style = styles["itemMenuButton"];
			style.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			style.BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.08f);
			style.Default.BackColor = ColorInt.RGBA(1, 1, 1, 0.04f);
			style.Checked.BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.2f);
			style.CheckedHot.BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.2f);

			style = styles["itemPickButton"];
			style.TextColor = ColorInt.RGBA(1f, 1f, 1f, 1);
			style.BackColor = ColorInt.RGBA(0.4f, 0.1f, 0.1f, 1f);
			style.Default.BackColor = ColorInt.RGBA(0.5f, 0.5f, 0.2f, 1f);
			style.Checked.BackColor = ColorInt.RGBA(1f, 0.1f, 0.1f, 1f);
			style.CheckedHot.BackColor = ColorInt.RGBA(0.4f, 0.1f, 0.1f, 1f);

			style = styles["eventPanel"];
			style.BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.08f);
			style.Default.BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.08f);
			style.Checked.BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.2f);
			style.CheckedHot.BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.2f);

			styles["treeNodeText"].BackColor = ColorInt.RGBA(0.5f, 0.5f, 1, 0.0f);

			style = styles["addEventButton"];
			style.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			style.BackColor = ColorInt.RGBA(0.5f, 1, 0.5f, 0.15f);
			style.Default.BackColor = ColorInt.RGBA(0.5f, 1, 0.5f, 0.1f);
		}
	}
}