//*************************************************************
//  File: Master.cs
//  Date created: 12/9/2016
//  Date edited: 12/13/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: 
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

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

		public static bool TEMP_RUN = false; 

	
		// member variables
		private static string s_sBaseDir = "";
		private static string s_sActiveClan = "";
		private static string s_sActiveUserName = "";
		private static string s_sKey = "";

		// properties
		public static string GetBaseDir()
		{
			if (s_sBaseDir == "") { s_sBaseDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); }
			return s_sBaseDir + "/";
		}

		public static void SetActiveClan(string sClanName) { s_sActiveClan = sClanName; } 
		public static string GetActiveClan() { return s_sActiveClan; }

		public static void SetActiveUserName(string sUserName) { s_sActiveUserName = sUserName; }
		public static string GetActiveUserName() { return s_sActiveUserName; }

		public static void SetKey(string sKey) { s_sKey = sKey; }
		public static string GetKey() { return s_sKey; }

		// common functions
		public static string CleanResponse(string sResponse) { return sResponse.Trim('\"').Replace("\\\"", "\"").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\\\", "\\"); }
		public static void Popup(Context pContext, string sMsg)
		{
			var pBuilder = new AlertDialog.Builder(pContext);
			pBuilder.SetMessage(sMsg);
			pBuilder.SetPositiveButton("Ok", (e, s) => { return; });
			pBuilder.Create().Show();
		}

		public static string BuildCommonBody() { return BuildCommonBody(""); }
		public static string BuildCommonBody(string sOtherXML)
		{
			return "<params><param name='sClanName'>" + s_sActiveClan + "</param><param name='sUserName'>" + s_sActiveUserName + "</param><param name='sUserPassPhrase'>" + s_sKey + "</param>" + sOtherXML + "</params>";
		}

		public static XElement ReadResponse(string sResponse)
		{
			XElement pResponse = null;
			try { pResponse = XElement.Parse(CleanResponse(sResponse)); }
			catch (Exception e)
			{
				sResponse = "<Message Type='Error'><Text>" + e.Message + "</Text><Data /></Message>";
				pResponse = XElement.Parse(sResponse);
			}

			return pResponse;
		}
	}
}