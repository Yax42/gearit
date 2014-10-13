using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;

namespace gearit.src.GUI.Editors
{
	class ConditionModifierBox : Panel
	{
		static public ConditionModifierBox Instance;
		private Condition Condition;
		private Button Button;
		private DropDownList TypeList;

		public ConditionModifierBox()
		{
			Instance = this;
			Size = new Point(300, 200);
			Visible = false;

			TypeList = new DropDownList();
			TypeList.AutoSize = Squid.AutoSize.HorizontalVertical;
			TypeList.DropdownAutoSize = true;

			ListBoxItem item = new ListBoxItem();
			item.Text = "None";
			item.Size = new Squid.Point(100, 35);
			item.Margin = new Margin(0, 0, 0, 4);
			item.Style = "item";
			TypeList.Items.Add(item);

			item = new ListBoxItem();
			item.Text = "Input";
			item.Size = new Squid.Point(100, 35);
			item.Margin = new Margin(0, 0, 0, 4);
			item.Style = "item";
			TypeList.Items.Add(item);

			item = new ListBoxItem();
			item.Text = "Other";
			item.Size = new Squid.Point(100, 35);
			item.Margin = new Margin(0, 0, 0, 4);
			item.Style = "item";
			TypeList.Items.Add(item);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			//TypeList.((int) Condition.Type)
		}

		public void Show(Condition cond)
		{
			Position = cond.Position;
			Visible = true;
			MenuRobotEditor.Instance.setFocus(true);
			Condition = cond;
			/*
			Button.Text = "Set Spot";
			Button.Size = new Point(160, 28);
			btn_select.Position = new Point(x, Size.y / 2 - 14);
			btn_select.MouseClick += delegate(Control sender, MouseEventArgs args)
			{
				ISpot spot = RobotEditor.Instance.Select1.getConnection(RobotEditor.Instance.Select2);
				if (spot == null)
					return ;

				_valide = true;
				btn_select.Text = spot.Name;
			};
			Content.Controls.Add(btn_select);
			*/

		}
	}
}