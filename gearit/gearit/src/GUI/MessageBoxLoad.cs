using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using System.IO;
using gearit.src.output;

namespace gearit.src.GUI
{
	public class MessageBoxLoad
	{
		MessageBox _msg;
		DropDownList combo = new DropDownList();

		public MessageBoxLoad(string folder, string extension, Desktop dk, Action<String> cbLoad)
		{
			_msg = MessageBox.Show(new Point(300, 160), "Load Robot", "Name", MessageBoxButtons.OK, dk);

			// Event type choice
			combo.Size = new Squid.Point(158, 34);
			combo.Position = new Squid.Point(_msg.MessageLabel.Size.x + 16, 70);
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

			// list of files in robot folder
			DirectoryInfo d = new DirectoryInfo(folder);
			if (!d.Exists)
			{
				OutputManager.LogError("Folder " + folder + " doesn't exist.");
				return;
			}
			FileInfo[] Files = d.GetFiles("*" + extension); //Getting Text files
			foreach (FileInfo file in Files)
			{
				ListBoxItem item = new ListBoxItem();
				item.Text = file.Name.Substring(0, file.Name.Length - 4);
				item.Size = new Squid.Point(100, 35);
				item.Margin = new Margin(0, 0, 0, 4);
				item.Style = "item";
				combo.Items.Add(item);
				item.Selected = true;
			}

			_msg.Controls.Add(combo);

			_msg.GetControl(MessageBoxButtons.OK.ToString()).MouseClick += delegate(Control sender, MouseEventArgs args)
			{
				_msg.Close();
				cbLoad(combo.SelectedItem.Text);
			};
		}
	}
}
