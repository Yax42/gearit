using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gearit.src.utility
{
	class GI_Data
	{
		public const float Version = 1.11f;
		public static string Pseudo = "";
		public static string ClientPath { get {return "data/net/client/";} }
		public static string ServerPath { get {return "data/net/server/";} }
	}
}
