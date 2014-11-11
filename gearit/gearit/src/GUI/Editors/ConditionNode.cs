using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using gearit.src.robot;
using gearit.xna;

namespace gearit.src.GUI.Editors
{
	public enum ConditionType
	{
		None,
		Input,
		Other
	}
	public enum InputType
	{
		Pressed,
		JustPressed,
		Released,
		JustReleased,
	}

	interface EventItem
	{
		Point PositionOnSide { get; }
	}

	class Condition : EventItem
	{
		public EventTree EventTree;
		public ConditionType Type;
		public InputType InputType;
		public string Value;
		public Condition Next;
		public Button Button;
		private bool IsFirst;

		public Condition(EventTree ctrl, bool v = true) : this(ctrl, ConditionType.None, InputType.Pressed, v ? "True" : null)
		{}

		public Condition(EventTree ctrl, string str) : this(ctrl, ConditionType.Other, InputType.Pressed, str) {}

		public Condition(EventTree ctrl, InputType input, string key) : this(ctrl, ConditionType.Input, input, key)
		{}

		public Point PositionOnSide
		{
			get
			{
				return Button.Position + EventTree.Position + new Point(Button.Size.x, 0);
			}
		}

		private Condition(EventTree ctrl, ConditionType type, InputType input, string str)
		{
			EventTree = ctrl;
			Type = type;
			InputType = input;
			Value = str;
			Next = null;
			IsFirst = true;
			Button  = new Button();
			Button.Position = ctrl.LastPosition;
			Button.Size = new Point(0, EventTree.BTN_HEIGHT);
			Button.AutoSize = AutoSize.Horizontal;
			Button.Parent = ctrl;
			Button.Style = "button";
			Button.Text = "AAAAAAAAAAAAAAAAAa";
			Button.MouseClick += delegate(Control sender, MouseEventArgs args)
			{
				ConditionModifierBox.Instance.Show(this);
			};
		}

		private void Update(Point pos)
		{
			Button.Text = ButtonContent;
			Button.Position = pos;
		}

		public void Add(Condition c)
		{
			Next = c;
			c.IsFirst = false;
		}

		private string ButtonContent
		{
			get
			{
				string res = "";
				if (Type == ConditionType.None)
				{
					if (Value != null)
						res += "true";
					else
						res += "false";
				}
				else if (Type == ConditionType.Other)
				{
					res += Value;
				}
				else
				{
					res += "Input:";
					switch (InputType)
					{
						case InputType.JustPressed:
							res += "justPressed";
							break;
						case InputType.JustReleased:
							res += "justReleased";
							break;
						case InputType.Released:
							res += "released";
							break;
						case InputType.Pressed:
							res += "pressed";
							break;
					}
					res += "(" + Value + ")";
				}
				return res;
			}
		}


		public string ToLua(bool previousEventExist = false)
		{
			string res = "";
			if (IsFirst && previousEventExist)
				res += "elsif ";
			else if (IsFirst)
				res += "if ";
			else
				res += " and ";
			res += ButtonContent;
			if (Next == null)
				return res + " then";
			else return res + Next.ToLua();
		}
	}

	class Action
	{
		public RevoluteSpot Spot;
		public float Value;
	}

	class EventNode
	{
		public Condition Condition;
		public Action Action;

		public EventNode(EventTree ctrl)
		{
			Condition = new Condition(ctrl, "LOLILOL");
		}
	}

	class EventTree : Window
	{
		public const int BTN_HEIGHT = 20;
		public const int BTN_WIDTH = 100;

		private List<EventNode> List;
		private Point AbsolutePos;
		private Button AddButton;

		public EventTree(Control parent)
		{
			Visible = true;
			Parent = parent;
			List = new List<EventNode>();
			Position = new Point(300, 300);

			Button btn = new Button();
			btn.Style = "button";
			btn.Text = "Add Condition";
			btn.Size = new Squid.Point(BTN_WIDTH, BTN_HEIGHT);
			//btn.AutoSize = Squid.AutoSize.Horizontal;
			btn.Position = new Squid.Point(0, 0);
			btn.Margin = new Squid.Margin(40);
			btn.Cursor = Cursors.Move;
			btn.Checked = false;
			btn.Parent = this;
			btn.MouseClick += delegate(Control sender, MouseEventArgs args)
			{
				AddNewCondition();
			};
			AddButton = btn;
			UpdateSize();
		}

		public void UpdateSize()
		{
			int max = BTN_WIDTH;// SpriteFonts.GetTextSize(AddButton.Text).x;
			foreach (EventNode e in List)
			{
				int tmp = SpriteFonts.GetTextSize(e.Condition.Button.Text).x;
				if (tmp > max)
					max = tmp;
			}
			max += 2;
			Size = new Point(max, LastPosition.y + BTN_HEIGHT);
		}

		public void AddNewCondition()
		{
			List.Add(new EventNode(this));
			AddButton.Position = LastPosition;
			UpdateSize();
		}

		public Point LastPosition
		{
			get
			{
				return new Point(0, List.Count * BTN_HEIGHT);
			}
		}

	}
}