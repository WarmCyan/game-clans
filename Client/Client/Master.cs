//*************************************************************
//  File: Master.cs
//  Date created: 12/8/2016
//  Date edited: 12/8/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: 
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
	public class Master
	{
		public static string CleanResponse(string sResponse) { return sResponse.Trim('\"').Replace("\\\"", "\"").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\\\", "\\"); }
		public static string EncodeXML(string sXML) { return sXML.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"); }
	}
}
