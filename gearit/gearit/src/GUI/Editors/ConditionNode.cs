using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using gearit.src.robot;

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
		JustPressed,
		Released,
		JustReleased,
		Pressed,
	}

	interface EventItem
	{
		Point Position { get; }
	}

	class Condition : EventItem
	{
		public ConditionType Type;
		public InputType InputType;
		public string Value;
		public Condition Next;
		public Button Button;
		private bool IsFirst;

		public Condition(bool v = true) : this(ConditionType.None, InputType.Pressed, v ? "True" : null)
		{}

		public Condition(string str) : this(ConditionType.Other, InputType.Pressed, str)
		{}

		public Condition(InputType input, string key) : this(ConditionType.Input, input, key)
		{}

		public Point Position { get { return Button.Position; } }

		private Condition(ConditionType type, InputType input, string str)
		{
			Type = type;
			InputType = input;
			Value = str;
			Next = null;
			IsFirst = true;
			Button  = new Button();
			Button.AutoSize = AutoSize.HorizontalVertical;
			Button.Parent = MenuRobotEditor.Instance;
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

	public class Action
	{
		public RevoluteSpot Spot;
		public float Value;
	}

	class EventNode
	{
		Condition Condition;
		Action Action;
	}

	class EventTree
	{
		private List<EventNode> List;

		public EventTree()
		{
			/*
			List = new List<EventNode>();
			Button btn = new Button();
			btn.Text = "Piece";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			btn.Parent = this;
			btn.MouseDrag += dragPiece;
			btn.Cursor = Cursors.Move;
			btn.Tooltip = "(W)";
			btn.Checked = false;
			y += btn.Size.y + PADDING;
			*/
		}
	}
}