//*************************************************************
//  File: Master.cs
//  Date created: 12/9/2016
//  Date edited: 12/22/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: 
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

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
		
		public static bool VERSION_CHECKED = false;
		public static string APP_VERSION = "1.0.0";
	
		// member variables
		private static string s_sBaseDir = "";
		private static string s_sActiveClan = "";
		private static string s_sActiveUserName = "";
		private static string s_sKey = "";

		private static Dictionary<string, string> s_dSettings;

		// properties
		public static string GetBaseDir()
		{
			if (s_sBaseDir == "") { s_sBaseDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); }
			return s_sBaseDir + "/";
		}

		public static string GetBaseURL() { return "http://dwlapi.azurewebsites.net/api/reflection/GameClansServer/"; }
		public static string GetServerURL() { return "GameClansServer/ClanServer/"; }
		public static string GetGameURL(string sGame) { return "GameClansServer.Games/" + sGame + "/"; }

		public static void SetActiveClan(string sClanName) { s_sActiveClan = sClanName; } 
		public static string GetActiveClan() { return s_sActiveClan; }

		public static void SetActiveUserName(string sUserName) { s_sActiveUserName = sUserName; }
		public static string GetActiveUserName() { return s_sActiveUserName; }

		public static void SetKey(string sKey) { s_sKey = sKey; }
		public static string GetKey() { return s_sKey; }

		// common functions
		public static string CleanResponse(string sResponse) { return sResponse.Trim('\"').Replace("\\\"", "\"").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\\\", "\\"); }
		public static string EncodeXML(string sXML) { return sXML.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"); }

		public static void Popup(Context pContext, string sMsg)
		{
			var pBuilder = new AlertDialog.Builder(pContext);
			pBuilder.SetMessage(sMsg);
			pBuilder.SetPositiveButton("Ok", (e, s) => { return; });
			pBuilder.Create().Show();
		}

		public static int GetPieceImage(string sName)
		{
			if (sName == "BD") { return Resource.Drawable.BD; }
			else if (sName == "BL") { return Resource.Drawable.BL; }
			else if (sName == "BR") { return Resource.Drawable.BR; }
			else if (sName == "BU") { return Resource.Drawable.BU; }
			else if (sName == "GD") { return Resource.Drawable.GD; }
			else if (sName == "GL") { return Resource.Drawable.GL; }
			else if (sName == "GR") { return Resource.Drawable.GR; }
			else if (sName == "GU") { return Resource.Drawable.GU; }
			else if (sName == "OD") { return Resource.Drawable.OD; }
			else if (sName == "OL") { return Resource.Drawable.OL; }
			else if (sName == "OR") { return Resource.Drawable.OR; }
			else if (sName == "OU") { return Resource.Drawable.OU; }
			else if (sName == "PD") { return Resource.Drawable.PD; }
			else if (sName == "PL") { return Resource.Drawable.PL; }
			else if (sName == "PR") { return Resource.Drawable.PR; }
			else if (sName == "PU") { return Resource.Drawable.PU; }
			else if (sName == "RD") { return Resource.Drawable.RD; }
			else if (sName == "RL") { return Resource.Drawable.RL; }
			else if (sName == "RR") { return Resource.Drawable.RR; }
			else if (sName == "RU") { return Resource.Drawable.RU; }
			else if (sName == "YD") { return Resource.Drawable.YD; }
			else if (sName == "YL") { return Resource.Drawable.YL; }
			else if (sName == "YR") { return Resource.Drawable.YR; }
			else if (sName == "YU") { return Resource.Drawable.YU; }
			else if (sName == "T") { return Resource.Drawable.T; }
			else if (sName == "F") { return Resource.Drawable.F; }

			return 0;
		}

		public static List<string> GetPieceParts(string sText)
		{
			sText = sText.ToUpper();
			List<string> lParts = new List<string>();

			if (sText.StartsWith("T") || sText.StartsWith("F"))
			{
				lParts.Add(sText[0].ToString());
				if (sText.Length > 1) { sText = sText.Substring(1); }
			}

			for (int i = 0; i < sText.Length; i += 2)
			{
				if (i + 1 >= sText.Length) { break; }
				string sPiece = sText[i].ToString() + sText[i + 1].ToString();
				lParts.Add(sPiece);
			}

			return lParts;
		}

		// returns true if koan valid, false if koan invalid. NOTE: Takes care of toupper
		public static bool FillKoanDisplay(Context pContext, FlowLayout pFlow, string sKoan)
		{
			bool bValid = true;

			// reset layout
			pFlow.RemoveAllViews();
		
			// get the image for each piece and add it
			List<string> lPieces = GetPieceParts(sKoan);
			foreach (string sPiece in lPieces)
			{
				int iRes = GetPieceImage(sPiece);
				if (iRes == 0) { bValid = false; continue; }

				ImageView pImageView = new ImageView(pContext);
				pImageView.SetImageResource(iRes);
				pFlow.AddView(pImageView);
			}
		
			// return whether koan was valid or naw
			return bValid;
		}

		public static string BuildCommonBody() { return BuildCommonBody(""); }
		public static string BuildCommonBody(string sOtherXML)
		{
			return "<params><param name='sClanName'>" + s_sActiveClan + "</param><param name='sUserName'>" + s_sActiveUserName + "</param><param name='sUserPassPhrase'>" + s_sKey + "</param>" + sOtherXML + "</params>";
		}

		public static string BuildGameIDBodyPart(string sGameID) { return "<param name='sGameID'>" + sGameID + "</param>"; }

		public static XElement ReadResponse(string sResponse)
		{
			XElement pResponse = null;
			try { pResponse = XElement.Parse(CleanResponse(sResponse)); }
			catch (Exception e)
			{
				sResponse = "<Message Type='Error'><Text>" + e.Message + "</Text><Data /></Message>";
				//sResponse = "<Message Type='Error'><Text>" + EncodeXML(CleanResponse(sResponse)) + "</Text><Data /></Message>";
				pResponse = XElement.Parse(sResponse);
			}

			return pResponse;
		}

		private static void ReadSettings()
		{
			// clear old settings
			s_dSettings = new Dictionary<string, string>();

			// read in settings file
			List<string> lSettings = File.ReadAllLines(GetBaseDir() + "_settings.dat").ToList();
			foreach (string sSetting in lSettings)
			{
				int iEquals = sSetting.IndexOf('=');
				string sName = sSetting.Substring(0, iEquals);
				string sValue = sSetting.Substring(iEquals + 1);

				s_dSettings.Add(sName, sValue);
			}
		}
		private static void WriteSettings()
		{
			List<string> lSettings = new List<string>();
			foreach (string sSetting in s_dSettings.Keys) { lSettings.Add(sSetting + "=" + s_dSettings[sSetting]); }
			File.WriteAllLines(GetBaseDir() + "_settings.dat", lSettings.ToArray());
		}

		public static void SetSetting(string sSettingName, string sValue)
		{
			ReadSettings();
			if (s_dSettings.Keys.Contains(sSettingName)) { s_dSettings[sSettingName] = sValue; }
			else { s_dSettings.Add(sSettingName, sValue); }
			WriteSettings();
		}
		
		public static string GetSetting(string sSettingName)
		{
			ReadSettings();
			return s_dSettings[sSettingName];
		}
	}
}