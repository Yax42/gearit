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
			public Color PrimitiveLight;
			public Color PrimitiveHalfLight;
			public Color PrimitiveSuperLight;
			public Color Blackie;
			public Color Dark;
			public Color DarkLight;
			public Color Light;
			public Color Grayie;
			public Color Weird;
			public Color White;
		};

		static	public Dictionary<String, ThemeInfo> Themes = new Dictionary<string,ThemeInfo>();
		static public ThemeInfo CurrentTheme;

		static public void init()
		{
			// Create basic theme
			#region themes
			ThemeInfo info = new ThemeInfo();
			info.Primitive = new Color(159, 23, 27, 255);
			info.PrimitiveDark = new Color(123, 31, 35, 255);
			info.PrimitiveLight = new Color(198, 134, 129, 255);
			info.PrimitiveHalfLight = new Color(204, 159, 146, 255);
			info.PrimitiveSuperLight = new Color(191, 56, 56, 255);
			info.Blackie = new Color(0, 0, 0, 255);
			info.Dark = new Color(73, 35, 33, 255);
			info.DarkLight = new Color(95, 61, 49, 255);
			info.Light = new Color(207, 177, 158, 255);
			info.Grayie = new Color(226, 212, 201, 255);
			info.Weird = new Color(164, 80, 70, 255);
			info.White = new Color(255, 255, 255, 255);
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

			ControlStyle messageStyle = new ControlStyle();
			messageStyle.TextAlign = Alignment.MiddleCenter;
			messageStyle.Font = "Arial14";

			ControlStyle textStyle = new ControlStyle();
			textStyle.TextPadding = new Margin(8, 0, 8, 0);
			textStyle.TextAlign = Alignment.MiddleLeft;

			ControlStyle textStyleB = new ControlStyle();
			textStyleB.TextPadding = new Margin(8, 0, 8, 0);
			textStyleB.TextAlign = Alignment.MiddleLeft;

			ControlStyle itemStyle = new ControlStyle();
			itemStyle.TextPadding = new Margin(8, 0, 8, 0);
			itemStyle.TextAlign = Alignment.MiddleLeft;

			ControlStyle buttonStyle = new ControlStyle();
			buttonStyle.TextPadding = new Margin(0);
			buttonStyle.TextAlign = Alignment.MiddleCenter;

			ControlStyle buttonListStyle = new ControlStyle();
			buttonListStyle.TextPadding = new Margin(8, 0, 8, 0);
			buttonListStyle.TextAlign = Alignment.MiddleLeft;

			ControlStyle tooltipStyle = new ControlStyle();
			tooltipStyle.TextPadding = new Margin(8);
			tooltipStyle.TextAlign = Alignment.TopLeft;

			ControlStyle inputStyle = new ControlStyle();
			inputStyle.TextPadding = new Margin(4);
			//inputStyle.Tiling = TextureMode.Grid;

			ControlStyle messageBoxStyle = new ControlStyle();
			messageBoxStyle.TextPadding = new Margin(4);
			messageBoxStyle.TextAlign = Alignment.TopLeft;


			ControlStyle windowStyle = new ControlStyle();
			windowStyle.Tiling = TextureMode.Grid;
			windowStyle.Grid = new Margin(9);

			ControlStyle frameStyle = new ControlStyle();
			frameStyle.Tiling = TextureMode.Grid;
			frameStyle.Grid = new Margin(4);
			frameStyle.TextPadding = new Margin(8);

			ControlStyle vscrollTrackStyle = new ControlStyle();
			vscrollTrackStyle.Tiling = TextureMode.Grid;
			vscrollTrackStyle.Grid = new Margin(3);

			ControlStyle vscrollButtonStyle = new ControlStyle();
			vscrollButtonStyle.Tiling = TextureMode.Grid;
			vscrollButtonStyle.Grid = new Margin(3);

			ControlStyle vscrollUp = new ControlStyle();

			ControlStyle hscrollTrackStyle = new ControlStyle();
			hscrollTrackStyle.Tiling = TextureMode.Grid;
			hscrollTrackStyle.Grid = new Margin(3);

			ControlStyle hscrollButtonStyle = new ControlStyle();
			hscrollButtonStyle.Tiling = TextureMode.Grid;
			hscrollButtonStyle.Grid = new Margin(3);

			ControlStyle hscrollUp = new ControlStyle();

			ControlStyle checkButtonStyle = new ControlStyle();
			checkButtonStyle.Default.TextPadding = new Margin(15);

			ControlStyle comboLabelStyle = new ControlStyle();
			comboLabelStyle.TextPadding = new Margin(10, 0, 0, 0);
			comboLabelStyle.Tiling = TextureMode.Grid;
			comboLabelStyle.Grid = new Margin(3, 0, 0, 0);

			ControlStyle comboButtonStyle = new ControlStyle();

			ControlStyle scoringStyle = new ControlStyle();
			ControlStyle itemScoringStyle = new ControlStyle();
			itemScoringStyle.TextAlign = Alignment.MiddleCenter;

			ControlStyle multilineStyle = new ControlStyle();
			multilineStyle.TextAlign = Alignment.TopLeft;
			multilineStyle.TextPadding = new Margin(8);

			ControlStyle labelStyle = new ControlStyle();
			labelStyle.TextPadding = new Margin(8, 0, 8, 0);
			labelStyle.TextAlign = Alignment.MiddleLeft;

			ControlStyle mainMenuStyle = new ControlStyle();
			ControlStyle mainMenuListContainerStyle = new ControlStyle();
			ControlStyle mainMenuListStyle = new ControlStyle();
			mainMenuListStyle.TextAlign = Alignment.MiddleCenter;
			ControlStyle titleMainMenuStyle = new ControlStyle();
			titleMainMenuStyle.Font = "Title";

			ControlStyle itemMainMenuStyle = new ControlStyle();
			itemMainMenuStyle.TextAlign = Alignment.MiddleCenter;

			ControlStyle menuStyle = new ControlStyle();

			ControlStyle msgboxStyle = new ControlStyle();

			ControlStyle itemMenuStyle = new ControlStyle();
			itemMenuStyle.TextAlign = Alignment.MiddleCenter;

			ControlStyle itemListStyle = new ControlStyle();
			itemListStyle.TextAlign = Alignment.MiddleCenter;

			ControlStyle itemListFrameStyle = new ControlStyle();
			itemListFrameStyle.TextAlign = Alignment.MiddleCenter;

			ControlStyle itemMenuTitleStyle = new ControlStyle();
			itemMenuTitleStyle.TextAlign = Alignment.MiddleCenter;
			itemMenuTitleStyle.TextPadding = new Squid.Margin(8, 0, 0, 0);

			ControlStyle itemMenuSubtitleStyle = new ControlStyle();
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

			ControlStyle panelStyle = new ControlStyle();

			skin.Styles.Add("itemScoring", itemScoringStyle);
			skin.Styles.Add("scoring", scoringStyle);
			skin.Styles.Add("message", messageStyle);
			skin.Styles.Add("textwhite", textStyle);
			skin.Styles.Add("textblack", textStyleB);
			skin.Styles.Add("item", itemStyle);
			skin.Styles.Add("menuTextbox", inputStyle);
			skin.Styles.Add("messagebox", messageBoxStyle);
			skin.Styles.Add("button", buttonStyle);
			skin.Styles.Add("buttonList", buttonListStyle);
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
			skin.Styles.Add("mainMenuList", mainMenuListStyle);
			skin.Styles.Add("mainMenuListContainer", mainMenuListContainerStyle);
			skin.Styles.Add("titleMainMenu", titleMainMenuStyle);
			skin.Styles.Add("itemMainMenu", itemMainMenuStyle);
			skin.Styles.Add("itemPickButton", itemPickButtonStyle);
			skin.Styles.Add("menu", menuStyle);
			skin.Styles.Add("itemMenu", itemMenuStyle);
			skin.Styles.Add("itemList", itemListStyle);
			skin.Styles.Add("itemListFrame", itemListFrameStyle);
			skin.Styles.Add("itemMenuTitle", itemMenuTitleStyle);
			skin.Styles.Add("itemMenuSubtitle", itemMenuSubtitleStyle);
			skin.Styles.Add("itemMenuButton", itemMenuButtonStyle);
			skin.Styles.Add("eventPanel", eventPanelStyle);
			skin.Styles.Add("treeNodeText", treeNodeTextStyle);
			skin.Styles.Add("addEventButton", addEventButtonStyle);
			skin.Styles.Add("panel", panelStyle);
			skin.Styles.Add("msgbox", msgboxStyle);

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

            style = styles["menuTextbox"];
			style.TextColor = toInt(info.Dark);
			style.BackColor = toInt(info.PrimitiveHalfLight);
			style.Default.BackColor = toInt(info.Light);// removeAlpha(toInt(info.Light), 150);
			style.Default.TextColor = toInt(info.Weird);// removeAlpha(toInt(info.Light), 150);

			style = styles["messagebox"];
			style.BackColor = ColorInt.RGBA(0.2f, 0.2f, 0.2f, 0.3f);
			style.Focused.Tint = ColorInt.RGBA(1, 1, 1, 1f);

			style = styles["label"];
			style.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			style.BackColor = ColorInt.RGBA(1, 1, 1, 0.1f);
			style.Default.BackColor = 0;

			style = styles["mainMenu"];
			style.BackColor = toInt(info.Primitive);

			style = styles["mainMenuListContainer"];
			style.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			style.BackColor = toInt(info.Grayie);

			style = styles["mainMenuList"];

			style = styles["titleMainMenu"];
			style.TextColor = toInt(info.Primitive);
			style.TextAlign = Alignment.MiddleCenter;

			style = styles["itemMainMenu"];
			style.TextColor = 0;
			style.Default.BackColor = 0;
			style.Default.TextColor = toInt(info.Primitive);
			style.Disabled.TextColor = toInt(info.Primitive);

			style = styles["menu"];
			style.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			style.BackColor = toInt(info.Grayie);

			style = styles["msgbox"];
			style.TextColor = toInt(info.Grayie);
			style.BackColor = toInt(info.Dark);

			style = styles["itemMenu"];
			style.TextColor = toInt(info.Weird);

			style = styles["itemList"];
			style.Checked.TextColor = toInt(info.PrimitiveLight);
			style.Checked.BackColor = toInt(info.PrimitiveDark);
			style.TextColor = toInt(info.Primitive);
			style.BackColor = toInt(info.PrimitiveLight);
			style.Default.TextColor = toInt(info.Primitive);
			style.Default.BackColor = 0;

			style = styles["itemMenuTitle"];
			style.BackColor = toInt(info.PrimitiveDark);
			style.TextColor = toInt(info.White);

			style = styles["itemMenuSubtitle"];
			style.BackColor = toInt(info.PrimitiveLight);
			style.TextColor = toInt(info.Primitive);

			style = styles["itemMenuButton"];
			style.TextColor = toInt(info.Grayie);
			style.BackColor = toInt(info.PrimitiveSuperLight);
			style.Default.BackColor = toInt(info.Primitive);
			style.Checked.BackColor = toInt(info.Primitive);
			style.CheckedHot.BackColor = toInt(info.Light);

			style = styles["itemPickButton"];
			style.TextColor = ColorInt.RGBA(1f, 1f, 1f, 1);
			style.BackColor = toInt(info.Light);
			style.Default.BackColor = toInt(info.Primitive);
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

			style = styles["button"];
			style.TextColor = toInt(info.White);
			style.BackColor = toInt(info.PrimitiveSuperLight);
			style.Default.BackColor = toInt(info.Primitive);
			style.Checked.BackColor = toInt(info.Dark);

			style = styles["buttonList"];
			style.TextColor = toInt(info.White);
			style.BackColor = toInt(info.PrimitiveSuperLight);
			style.Default.BackColor = toInt(info.Primitive);

			style = styles["frame"];
			style.TextColor = toInt(info.White);
			style.BackColor = toInt(info.Primitive);

			style = styles["panel"];
			style.TextColor = ColorInt.RGBA(.8f, .8f, .8f, 1);
			style.BackColor = toInt(info.Grayie);

			style = styles["item"];
			style.TextColor = toInt(info.White);
			style.BackColor = toInt(info.Primitive);
			style.Default.TextColor = toInt(info.White);
			style.Default.BackColor = toInt(info.PrimitiveSuperLight);

			style = styles["itemListFrame"];
			style.Default.BackColor = toInt(info.PrimitiveSuperLight);
			style.BackColor = toInt(info.PrimitiveSuperLight);

			style = styles["itemScoring"];
			style.Checked.BackColor = removeAlpha(toInt(info.Primitive), 10);
			style.CheckedHot.BackColor = removeAlpha(toInt(info.Primitive), 10);
			style.TextColor = toInt(info.Grayie);
			style.BackColor = removeAlpha(toInt(info.Primitive), 10);
			style.Default.BackColor = 0;

			style = styles["scoring"];
			style.Default.BackColor = toInt(info.PrimitiveLight);
		}

		static public int removeAlpha(int color, int alpha)
		{
			return (color - ColorInt.FromArgb(alpha, 0, 0, 0));
		}

		static public int toInt(Color color)
		{
			return (ColorInt.FromArgb(color.A, color.R, color.G, color.B));
		}

		static public void InitComboBox(DropDownList combo)
		{
			combo.Label.Style = "buttonList";
			combo.Button.Style = "";
			combo.Listbox.Margin = new Margin(0, 0, 0, 0);
			combo.Listbox.Style = "itemListFrame";
			combo.Listbox.Scrollbar.Size = new Squid.Point(0, 0);
			combo.Listbox.Scrollbar.ButtonUp.Style = "vscrollUp";
			combo.Listbox.Scrollbar.ButtonUp.Size = new Squid.Point(0, 0);
			combo.Listbox.Scrollbar.ButtonDown.Style = "vscrollUp";
			combo.Listbox.Scrollbar.ButtonDown.Size = new Squid.Point(0, 0);
			combo.Listbox.Scrollbar.Slider.Style = "vscrollTrack";
			combo.Listbox.Scrollbar.Slider.Button.Style = "vscrollButton";
			combo.DropdownSize = new Squid.Point(50, 200);
		}
	}
}
