//*************************************************************
//  File: Master.cs
//  Date created: 12/9/2016
//  Date edited: 12/11/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: 
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App
{
	public class Master
	{
		private static string s_sBaseDir = "";

		public static string GetBaseDir()
		{
			if (s_sBaseDir == "") { s_sBaseDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); }
			return s_sBaseDir + "/";
		}

		public static string CleanResponse(string sResponse) { return sResponse.Trim('\"').Replace("\\\"", "\"").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\\\", "\\"); }

		public static void Popup(Context pContext, string sMsg)
		{
			var pBuilder = new AlertDialog.Builder(pContext);
			pBuilder.SetMessage(sMsg);
			pBuilder.SetPositiveButton("Ok", (e, s) => { return; });
			pBuilder.Create().Show();
		}
	}
}