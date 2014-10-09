using System;
using System.Collections.Generic;
using System.Text;
using Squid;

namespace gearit.src.GUI
{
	public class SampleWindow : Window
	{
		public TitleBar Titlebar { get; private set; }

		public SampleWindow()
		{
			AllowDragOut = true;
			Padding = new Margin(4);

			Titlebar = new TitleBar();
			Titlebar.Dock = DockStyle.Top;
			Titlebar.Size = new Squid.Point(122, 35);
			Titlebar.MouseDown += delegate(Control sender, MouseEventArgs args) { StartDrag(); };
			Titlebar.MouseUp += delegate(Control sender, MouseEventArgs args) { StopDrag(); };
			Titlebar.Cursor = Cursors.Move;
			Titlebar.Style = "frame";
			Titlebar.Margin = new Margin(-4, -4, -4, -1);
			Titlebar.Button.MouseClick += Button_OnMouseClick;
			Titlebar.TextAlign = Alignment.MiddleLeft;
			Titlebar.BBCodeEnabled = true;
			AllowDragOut = false;

			Controls.Add(Titlebar);
		}

		void Button_OnMouseClick(Control sender, MouseEventArgs args)
		{
			Animation.Custom(FadeAndClose());
		}

		private System.Collections.IEnumerator FadeAndClose()
		{
			yield return Animation.Opacity(0, 500);
			Close();
		}
	}

	public class TitleBar : Label
	{
		public Button Button { get; private set; }

		public TitleBar()
		{
			Button = new Button();
			Button.Size = new Point(30, 30);
			Button.Style = "button";
			Button.Tooltip = "Close Window";
			Button.Dock = DockStyle.Right;
			Button.Margin = new Margin(0, 8, 8, 8);
			Elements.Add(Button);
		}
	}
}
