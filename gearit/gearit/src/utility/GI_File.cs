using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using gearit.src.editor;
using System.Runtime.Serialization;

namespace gearit.src.utility
{
	class GI_File
	{
		public float Version = GI_Data.Version;
		private DateTime LastTimeWritten;
		public string RelativePath = "";
		public string FileNameWithoutExtension = "";
		public string Extension = "";
		public string Label = "";
		public string Owner = "";
		public string LuaFullPath { get { return RelativePath + "/script/" + FileNameWithoutExtension + ".lua"; } }
		public string FullPath
		{
			get
			{
				return RelativePath + "/" + FileNameWithoutExtension + Extension;
			}
			set
			{
				if (value == "")
				{
					RelativePath = "";
					FileNameWithoutExtension = "";
					Extension = "";
				}
				else
				{
					RelativePath = Path.GetDirectoryName(value);
					FileNameWithoutExtension = Path.GetFileNameWithoutExtension(value);
					Extension = Path.GetExtension(value);
					ResetLastTimeModified();
				}
			}
		}

		public void ResetLastTimeModified()
		{
			LastTimeWritten = File.GetLastWriteTime(FullPath);
		}

		public bool HasBeenModified
		{
			get
			{
				var tmp = File.GetLastWriteTime(FullPath);
				if (tmp.CompareTo(LastTimeWritten) > 0)
				{
					LastTimeWritten = tmp;
					return true;
				}
				return false;
			}
		}

		public GI_File(string path, string name, string ext)
		{
			RelativePath = path;
			FileNameWithoutExtension = name;
			Extension = ext;
			ResetLastTimeModified();
		}

		public GI_File(string fullPath)
		{
			FullPath = fullPath;
		}

		virtual public void Save()
		{
		}
	}
}
