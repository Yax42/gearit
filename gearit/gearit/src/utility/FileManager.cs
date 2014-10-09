using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace gearit.src.utility
{
	class FileManager
	{
		public static string[] GetFolders(string dir)
		{
			try
			{
				return Directory.GetDirectories(dir);
			}
			catch (System.Exception excpt)
			{
				Console.WriteLine(excpt.Message);
			}
			return new string[0];
		}

		public static string[] GetFiles(string dir, string ext = null)
		{
			List<string> res = new List<string>();
			try
			{
				if (ext == null)
					return Directory.GetFiles(dir);
				foreach (string f in Directory.GetFiles(dir))
				{
					if (f.EndsWith(ext))
						res.Add(f);
				}
			}
			catch (System.Exception excpt)
			{
				Console.WriteLine(excpt.Message);
			}
			return res.ToArray();
		}

		static List<string> GetFilesRecursive(string sDir, string ext = null)
		{
			Debug.Assert(false, "Not implemented yet.");
			return null;
			List<string> res = new List<string>();
			try
			{
				foreach (string d in Directory.GetDirectories(sDir))
				{
					foreach (string f in Directory.GetFiles(d))
					{
						Console.WriteLine(f);
					}
					GetFiles(d);
				}
			}
			catch (System.Exception excpt)
			{
				Console.WriteLine(excpt.Message);
			}
		}

		public static string PathToScriptPath(string path)
		{
#if true
			return Path.GetDirectoryName(path) + "/script/" + Path.GetFileNameWithoutExtension(path) + ".lua";
#else
			string[] decomposed = path.Split("/".ToCharArray());
			string res = "";
			for (int i = 0; i < decomposed.Count() - 1; i++)
			{
				res += decomposed[i] + "/";
			}
			res += "script/";
			string[] nameDecomposed = decomposed.Last().Split(".".ToCharArray());
			for (int i = 0; i < nameDecomposed.Count() - 1; i++)
			{
				res += nameDecomposed[i] + ".";
			}
			res += "lua";
			return res;
#endif
		}
	}
}
