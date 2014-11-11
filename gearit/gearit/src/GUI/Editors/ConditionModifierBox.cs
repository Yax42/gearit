using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using gearit.src.utility;
using System.Diagnostics;

namespace gearit.src.GUI.Editors
{
	class ConditionModifierBox : Dialog
	{
		static public ConditionModifierBox Instance;
		private Condition Condition;
		private Button Button;
		private DropDownList TypeList;
		private Panel Panel;
		private DropDownList InputList;
		private TextBox PersonalText;
		private Button NoneBool;
		private Button AddKey;
		private MessageBox AddKeyBox;

		public ConditionModifierBox(Control ctrl, Panel pnl)
		{
			Modal = true; // make sure its modal
			Scissor = false;
			Padding = new Margin(7);

			Panel = pnl;
			Panel.Container.Controls.Add(this);
			Instance = this;
			Size = new Point(300, 200);
			Visible = false;
			Style = "eventPanel";

			TypeList = new DropDownList();
			//TypeList.AutoSize = Squid.AutoSize.HorizontalVertical;
			TypeList.Size = new Point(100, 20);
			TypeList.DropdownAutoSize = true;
			TypeList.Position = new Point(0, 0);
			TypeList.Label.Style = "item";
			TypeList.Button.Size = Point.Zero;
			TypeList.Button.Style = "item";
			TypeList.Listbox.Margin = new Margin(0, 0, 0, 0);
			TypeList.Listbox.Style = "frame";
			TypeList.DropdownSize = new Point(200, 200);
			Controls.Add(TypeList);

			List<string> names = new List<string>();
			names.Add("None");
			names.Add("Input");
			names.Add("Other");

			for (int i = 0; i < 3; i++)
			{
				ListBoxItem item = new ListBoxItem();
				item.Text = names[i];
				item.Size = new Squid.Point(100, 35);
				item.Margin = new Margin(0, 0, 0, 4);
				item.Style = "item";
				item.Parent = this;
				item.Selected = true;
				TypeList.Items.Add(item);
			}

			InputList = new DropDownList();
			//TypeList.AutoSize = Squid.AutoSize.HorizontalVertical;
			InputList.Size = new Point(100, 20);
			InputList.DropdownAutoSize = true;
			InputList.Position = new Point(0, 30);
			InputList.Label.Style = "item";
			InputList.Button.Size = Point.Zero;
			InputList.Button.Style = "item";
			InputList.Listbox.Margin = new Margin(0, 0, 0, 0);
			InputList.Listbox.Style = "frame";
			InputList.DropdownSize = new Point(200, 200);
			Controls.Add(InputList);

			names = new List<string>();
			names.Add("Pressed");
			names.Add("JustPressed");
			names.Add("Released");
			names.Add("JustReleased");

			for (int i = 0; i < 4; i++)
			{
				ListBoxItem item = new ListBoxItem();
				item.Text = names[i];
				item.Size = new Squid.Point(100, 35);
				item.Margin = new Margin(0, 0, 0, 4);
				item.Style = "item";
				item.Parent = this;
				item.Selected = true;
				InputList.Items.Add(item);
			}

			PersonalText = new TextBox();
			PersonalText.Size = new Squid.Point(158, 34);
			PersonalText.Position = new Squid.Point(0, 60);
			PersonalText.Text = "BBBB";
			PersonalText.Style = "item";
			PersonalText.Parent = this;
			//Controls.Add(PersonalText);
			PersonalText.Focus();

			NoneBool = new Button();
			NoneBool.Text = "X";
			NoneBool.Size = new Point(30, 30);
			NoneBool.Style = "item";
			NoneBool.TextAlign = Alignment.MiddleCenter;
			NoneBool.Position = new Squid.Point(0, 90);
			NoneBool.Parent = this;
			NoneBool.MouseClick += delegate(Control sender, MouseEventArgs args)
			{
				if (NoneBool.Text == "X")
					NoneBool.Text = "";
				else
					NoneBool.Text = "X";
			};

			// Button key press
			AddKeyBox = null;
			AddKey = new Button();
			AddKey.Text = "Choose a key";
			AddKey.Size = new Point(120, 28);
			AddKey.Position = new Point(0, 120);
			AddKey.MouseClick += delegate(Control sender, MouseEventArgs args)
			{
				MenuRobotEditor.Instance.setFocus(true);
				AddKeyBox = MessageBox.Show(new Point(300, 100), "Add Event", "Press any key", MessageBoxButtons.None, MenuRobotEditor.Instance);
			};
			Controls.Add(AddKey);

			/*
			Button Button = new Button();
			Button.Text = "Set Spot";
			Button.Size = new Point(160, 28);
			//Button.Parent = this;
			Button.MouseClick += delegate(Control sender, MouseEventArgs args)
			{
			};
			*/
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			NoneBool.Visible = TypeList.Items[0].Selected;
			InputList.Visible = TypeList.Items[1].Selected;
			PersonalText.Visible = TypeList.Items[2].Selected;

			if (TypeList.Items[0].Selected)
			{
				Condition.Type = ConditionType.None;
				if (NoneBool.Text == "X")
					Condition.Value = "";
				else
					Condition.Value = null;
			}
			else if (TypeList.Items[1].Selected)
			{
				Condition.Type = ConditionType.Input;
				//Condition.InputType = (InputType)InputList;
			}
			else if (TypeList.Items[2].Selected)
			{
				Condition.Type = ConditionType.Other;
				Condition.Value = PersonalText.Text;
			}
			else
				Debug.Assert(false);

			//TypeList.((int) Condition.Type)

			if (AddKeyBox != null)
			{
				List<Microsoft.Xna.Framework.Input.Keys> inputs = Input.getJustReleased();
				if (inputs.Count != 0)
				{
					SetKey(inputs.ElementAt(0).ToString());

					AddKeyBox.Close();
					MenuRobotEditor.Instance.setFocus(false);
					AddKeyBox = null;
				}
			}
		}

		public void SetKey(string key)
		{
			Condition.Value = key;
			AddKey.Text = "Key [" + key + "]";
		}

		public void Show(Condition cond)
		{
			Position = cond.PositionOnSide;
			Visible = true;
			MenuRobotEditor.Instance.setFocus(true);
			Condition = cond;
		}
	}
}