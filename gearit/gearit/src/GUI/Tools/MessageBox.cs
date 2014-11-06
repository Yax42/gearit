using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;

namespace gearit.src.GUI
{
    /// <summary>
    /// Popup messagebox for interaction with user
    /// </summary>
	public class MessageBox : Dialog
	{
		public Label TitleLabel;
		public Label MessageLabel;
		public Frame ButtonFrame;

		private MessageBox(string title, string message)
		{
			Modal = true; // make sure its modal
			Scissor = false;
			Padding = new Margin(7);

			Style = "panel";

			TitleLabel = new Label();
			TitleLabel.Size = new Point(100, 55);
			TitleLabel.Dock = DockStyle.Top;
			TitleLabel.Text = title;
			TitleLabel.MouseDown += delegate(Control sender, MouseEventArgs args) { StartDrag(); };
			TitleLabel.MouseUp += delegate(Control sender, MouseEventArgs args) { StopDrag(); };
			TitleLabel.Cursor = Cursors.Move;
			TitleLabel.Style = "frame";
			TitleLabel.Margin = new Margin(0, 0, 0, -1);
			Controls.Add(TitleLabel);

			ButtonFrame = new Frame();
			ButtonFrame.Size = new Point(100, 35);
			ButtonFrame.Dock = DockStyle.Bottom;
			Controls.Add(ButtonFrame);

			if (message != null)
			{
				MessageLabel = new Label();
				MessageLabel.Dock = DockStyle.Top;
				MessageLabel.Margin = new Margin(16, 12, 16, 12);
				MessageLabel.TextWrap = true;
				MessageLabel.Text = message;
				MessageLabel.Style = "frame";
				Controls.Add(MessageLabel);
			}
		}

		public static MessageBox Show(Point size, string title, string message, MessageBoxButtons buttons, Desktop target)
		{
			MessageBox box = new MessageBox(title, message);
			box.Size = size;
			box.Position = (target.Size - size) / 2;
			box.InitButtons(buttons);
			box.Show(target);
			return box;
		}

		private void InitButtons(MessageBoxButtons buttons)
		{
			switch (buttons)
			{
				case MessageBoxButtons.OK:
					AddButton("OK", DialogResult.OK, 1);
					break;
				case MessageBoxButtons.OKCancel:
					AddButton("Cancel", DialogResult.Cancel, 2);
					AddButton("OK", DialogResult.OK, 2);
					break;
				case MessageBoxButtons.RetryCancel:
					AddButton("Cancel", DialogResult.Cancel, 2);
					AddButton("Retry", DialogResult.Retry, 2);
					break;
				case MessageBoxButtons.YesNo:
					AddButton("No", DialogResult.No, 2);
					AddButton("Yes", DialogResult.Yes, 2);
					break;
				case MessageBoxButtons.YesNoCancel:
					AddButton("Cancel", DialogResult.Cancel, 3);
					AddButton("No", DialogResult.No, 3);
					AddButton("Yes", DialogResult.Yes, 3);
					break;
				case MessageBoxButtons.AbortRetryIgnore:
					AddButton("Retry", DialogResult.Retry, 3);
					AddButton("Ignore", DialogResult.Ignore, 3);
					AddButton("Abort", DialogResult.Abort, 3);
					break;
			}
		}

		private void AddButton(string text, DialogResult result, int divide)
		{
			Button button = new Button();
			button.Style = "button";
			button.Cursor = Cursors.Link;
			button.Margin = new Margin(2);
			button.Size = new Point(Size.x / (divide + 1), 35);
			button.Text = text;
			button.Tag = result;
			button.Name = result.ToString();
			button.Dock = DockStyle.Right;
			button.MouseClick += button_OnMouseClick;
			ButtonFrame.Controls.Add(button);
		}

		void button_OnMouseClick(Control sender, MouseEventArgs args)
		{
			if (args.Button > 0) return;

			Animation.Stop();
			Close();
		}

		private System.Collections.IEnumerator FadeAndClose(Control sender)
		{
			yield return Animation.Opacity(0, 500);

			Result = (DialogResult)sender.Tag;

			Close();
		}
	}

	public enum MessageBoxButtons
	{
		OK = 0,
		OKCancel = 1,
		AbortRetryIgnore = 2,
		YesNoCancel = 3,
		YesNo = 4,
		RetryCancel = 5,
		None = 6,
	}
}
