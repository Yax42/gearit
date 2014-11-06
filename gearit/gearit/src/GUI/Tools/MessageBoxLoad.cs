using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using System.IO;
using gearit.src.output;
using gearit.src.GUI.Tools;

namespace gearit.src.GUI
{
	public class MessageBoxLoad
	{
		Action _cbCancel;
		MessageBox _msg;
		DropDownList combo = new DropDownList();

		public MessageBoxLoad(string folder, string extension, Desktop dk, Action<String> cbLoad, Action<bool> setFocus)
		{
			_msg = MessageBox.Show(new Point(250, 160), "Load", null, MessageBoxButtons.OKCancel, dk);

			// Event type choice
			combo.Size = new Squid.Point(250 - 32, 34);
			combo.Position = new Squid.Point(16, 70);
#if false
			combo.Size = new Squid.Point(200, 34);
			combo.Position = new Squid.Point(40 + 16, 70);
//			combo.Style = "button";
			combo.Label.Style = "button";
			combo.Label.TextAlign = Alignment.MiddleLeft;
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
			combo.DropdownSize = new Point(50, 200);
#else
			Theme.InitComboBox(combo);
#endif

			// list of files in robot folder
			DirectoryInfo d = new DirectoryInfo(folder);
			if (!d.Exists)
			{
				OutputManager.LogWarning("Folder doesn't exist", folder);
				return;
			}
			FileInfo[] Files = d.GetFiles("*" + extension); //Getting Text files
			if (Files.Count() == 0)
			{
				OutputManager.LogWarning("Folder is empty", folder);
				return;
			}
			foreach (FileInfo file in Files)
			{
				ListBoxItem item = new ListBoxItem();
				item.Text = file.Name.Substring(0, file.Name.Length - 4);
				item.Size = new Squid.Point(100, 34);
				item.Margin = new Margin(0, 0, 0, 0);
				item.Style = "item";
				combo.Items.Add(item);
				item.Selected = true;
			}

			_msg.Controls.Add(combo);

			_msg.GetControl(DialogResult.OK.ToString()).MouseClick += delegate(Control sender, MouseEventArgs args)
			{
				_msg.Close();
				cbLoad(folder + combo.SelectedItem.Text + extension);
			};
			_cbCancel = delegate()
			{
				_msg.Close();
				setFocus(false);
			};
			_msg.GetControl(DialogResult.Cancel.ToString()).MouseClick += delegate(Control sender, MouseEventArgs args)
			{
				cancel();
			};
		}

		public void cancel()
		{
			_cbCancel();
		}
	}
}
