﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace gearit.src.utility
{
	class GI_File
	{
		public string RelativePath = "";
		public string FileNameWithoutExtension = "";
		public string Extension = "";
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
				}
			}
		}

		public GI_File(string path, string name, string ext)
		{
			RelativePath = path;
			FileNameWithoutExtension = name;
			Extension = ext;
		}

		public GI_File(string fullPath)
		{
			FullPath = fullPath;
		}

		public GI_File() { }
	}
}
