using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using gearit.src.editor.robot;
using gearit.src.robot;

namespace gearit.src.GUI
{
	class TreeNodeAction : Panel
	{
		static int PADDING = 16;
		static public int height = 50;
		DropDownList combo = new DropDownList();
		ISpot _old_spot;
		Button btn_select = new Button();
		bool _valide = false;

		public TreeNodeAction(int width, Action<TreeNodeAction> cbEventRemove)
		{
			int height = MenuRobotEditor.HEIGHT_NODE;

			Size = new Point(width, height);


			int x = PADDING;
			Label lb = new Label();
			lb.Text = "Action";
			lb.Size = new Point(50, height);
			lb.Position = new Point(x, 0);
			lb.Style = "treeNodeText";
			Content.Controls.Add(lb);
			x += lb.Size.x + PADDING;

			// Button add
			btn_select.Text = "Set Spot";
			btn_select.Size = new Point(160, 28);
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

			x += btn_select.Size.x + PADDING * 3;


			lb = new Label();
			lb.Text = "Motor";
			lb.Size = new Point(50, height);
			lb.Position = new Point(x, 0);
			lb.Style = "treeNodeText";
			Content.Controls.Add(lb);

			x += lb.Size.x + PADDING;

			// Event type choice
			Content.Controls.Add(combo);
			combo.Size = new Squid.Point(158, height / 2);
			combo.Position = new Squid.Point(x, height / 2 - combo.Size.y / 2);
			combo.Label.Style = "comboLabel";
			combo.Button.Style = "comboButton";
			combo.Listbox.Margin = new Margin(0, 0, 0, 0);
			combo.Listbox.Style = "frame";
			combo.Listbox.ClipFrame.Margin = new Margin(8, 8, 8, 8);
			combo.Listbox.Scrollbar.Margin = new Margin(0, 4, 4, 4);
			combo.Listbox.Scrollbar.Size = new Squid.Point(14, 10);
			combo.Listbox.Scrollbar.ButtonUp.Style = "vscrollUp";
			combo.Listbox.Scrollbar.ButtonUp.Size = new Squid.Point(10, 20);
			combo.Listbox.Scrollbar.ButtonDown.Style = "vscrollUp";
			combo.Listbox.Scrollbar.ButtonDown.Size = new Squid.Point(10, 20);
			combo.Listbox.Scrollbar.Slider.Margin = new Margin(0, 2, 0, 2);
			combo.Listbox.Scrollbar.Slider.Style = "vscrollTrack";
			combo.Listbox.Scrollbar.Slider.Button.Style = "vscrollButton";

			combo.Opened += delegate(Control sender, SquidEventArgs args)
			{
				combo.Dropdown.Position = new Point(combo.Dropdown.Position.x, combo.Dropdown.Position.y - combo.Dropdown.Size.y - combo.Size.y);
			};

			// Button pressed
			ListBoxItem item = new ListBoxItem();
			item.Text = "Forward";
			item.Size = new Squid.Point(100, 35);
			item.Margin = new Margin(0, 0, 0, 4);
			item.Style = "item";
			item.Tag = "1";
			combo.Items.Add(item);

			item = new ListBoxItem();
			item.Text = "Backward";
			item.Size = new Squid.Point(100, 35);
			item.Margin = new Margin(0, 0, 0, 4);
			item.Style = "item";
			item.Tag = "-1";
			combo.Items.Add(item);

			item = new ListBoxItem();
			item.Text = "Stopped";
			item.Size = new Squid.Point(100, 35);
			item.Margin = new Margin(0, 0, 0, 4);
			item.Style = "item";
			item.Tag = "0";
			combo.Items.Add(item);

			item.Selected = true;


			// Button remove
			Button btn = new Button();
			btn.Text = "Remove";
			btn.Size = new Point(80, 28);
			btn.Position = new Point(Size.x - 90, Size.y / 2 - 14);
			btn.MouseClick += delegate(Control sender, MouseEventArgs args)
			{
				cbEventRemove(this);
			};
			Content.Controls.Add(btn);
		}

		public bool isValide()
		{
			return (_valide);
		}

		public string toLua()
		{
			string lua = "";

			if (!RobotEditor.Instance.Robot.hasSpot(btn_select.Text))
				lua = "-- Undefined spot " + btn_select.Text;
			else
				lua = "\t" + btn_select.Text + ".Motor = " + combo.SelectedItem.Tag;

			return (lua);
		}

		public void setSpot(string spot)
		{
			btn_select.Text = spot;
		}

		public void setState(string state)
		{
			List<ListBoxItem> items = combo.Items;

			foreach (var item in items)
			{
				if (item.Tag.Equals(state))
					combo.SelectedItem = item;
			}
		}

		// Let the menu manage the position
		public void setPosition(Point pos)
		{
			Position = pos;
		}
	}
}
