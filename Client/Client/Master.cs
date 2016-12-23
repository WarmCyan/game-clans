//*************************************************************
//  File: Master.cs
//  Date created: 12/8/2016
//  Date edited: 12/23/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: 
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
	public class Master
	{
		private static List<string> s_lValidZendoImageNames = new List<string>() { "BD", "BL", "BR", "BU", "F", "GD", "GL", "GR", "GU", "OD", "OL", "OR", "OU", "PD", "PL", "PR", "PU", "RD", "RL", "RR", "RU", "T", "YD", "YL", "YR", "YU" };

	
		private static string s_sActiveClan = "";
		private static string s_sActiveUserName = "";
		private static string s_sKey = "";

		public static SolidColorBrush BUTTON_NORMAL = new SolidColorBrush(Color.FromRgb(21, 21, 21)); 
		public static SolidColorBrush BUTTON_HOVER = new SolidColorBrush(Color.FromRgb(69, 186, 255));
		public static SolidColorBrush EDITOR_BAD = new SolidColorBrush(Color.FromRgb(200, 20, 20));
		public static SolidColorBrush EDITOR_NORMAL = new SolidColorBrush(Color.FromRgb(17, 17, 17));

		public static void SetActiveClan(string sClanName) { s_sActiveClan = sClanName; }
		public static string GetActiveClan() { return s_sActiveClan; }

		public static void SetActiveUserName(string sUserName) { s_sActiveUserName = sUserName; }
		public static string GetActiveUserName() { return s_sActiveUserName; }

		public static void SetKey(string sKey) { s_sKey = sKey; }
		public static string GetKey() { return s_sKey; }

		public static string GetBaseDir() { return AppDomain.CurrentDomain.BaseDirectory; } // NOTE: assuming this has the trailing slash?

		public static string GetBaseURL() { return "http://dwlapi.azurewebsites.net/api/reflection/GameClansServer/"; }
		public static string GetServerURL() { return "GameClansServer/ClanServer/"; }
		public static string GetGameURL(string sGame) { return "GameClansServer.Games/" + sGame + "/"; }


		public static string CleanResponse(string sResponse) { return sResponse.Trim('\"').Replace("\\\"", "\"").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\\\", "\\"); }
		public static string EncodeXML(string sXML) { return sXML.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"); }

		public static string BuildCommonBody() { return BuildCommonBody(""); } 
		public static string BuildCommonBody(string sOtherXML) { return "<params><param name='sClanName'>" + s_sActiveClan + "</param><param name='sUserName'>" + s_sActiveUserName + "</param><param name='sUserPassPhrase'>" + s_sKey + "</param>" + sOtherXML + "</params>"; }
		public static string BuildGameIDBodyPart(string sGameID) { return "<param name='sGameID'>" + sGameID + "</param>"; }


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


		public static void AddImageToWrapPanel(WrapPanel pPanel, string sImageName)
		{
			Rectangle pContainer = new Rectangle();
			ImageBrush pImage = new ImageBrush(new BitmapImage(new Uri(Master.GetBaseDir() + "/Resources/Zendo/" + sImageName + ".png")));
			pContainer.Fill = pImage;
			pContainer.Height = 30;
			pContainer.Width = 30;

			pPanel.Children.Add(pContainer);
		}

		private static List<string> GetPieceParts(string sText)
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

		public static bool FillKoanDisplay(WrapPanel pPanel, string sKoan)
		{
			bool bValid = true;

			// empty panel display
			pPanel.Children.Clear();

			// get the image for each piece and add it
			List<string> lPieces = GetPieceParts(sKoan);
			foreach (string sPiece in lPieces)
			{
				if (!s_lValidZendoImageNames.Contains(sPiece)) { bValid = false; continue; }
				AddImageToWrapPanel(pPanel, sPiece);
			}

			return bValid;
		}
	}

}
