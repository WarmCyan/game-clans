//*************************************************************
//  File: Master.cs
//  Date created: 11/28/2016
//  Date edited: 12/8/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: Superclass of static functions and properties
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClansServer
{
	public class Master
	{

		// constants
		public const string MSGTYPE_DEFAULT = "Default"; // message only
		public const string MSGTYPE_DATA = "Data"; // data only
		public const string MSGTYPE_BOTH = "Both"; // data and message
		public const string MSGTYPE_ERROR = "Error"; // error message (can include data)
	
		// methods
		
		public static string BuildUserPartitionKey(string sClanName) { return sClanName + "|USER"; }
		public static string BuildUserNotifPartitionKey(string sClanName, string sUserName) { return sClanName + "|" + sUserName + "|NOTIF"; }
		public static string BuildGamePartitionKey(string sClanName) { return sClanName + "|GAME"; }
		
		public static string MessagifySimple(string sMsg) { return Messagify(sMsg, MSGTYPE_DEFAULT, ""); } // why is this necessary?
		public static string MessagifyError(string sMsg) { return Messagify(sMsg, MSGTYPE_ERROR, ""); }
		public static string MessagifyError(string sMsg, string sData) { return Messagify(sMsg, MSGTYPE_ERROR, sData); }
		public static string MessagifyData(string sData) { return Messagify("", MSGTYPE_DATA, sData); }
		public static string Messagify(string sMsg, string sType, string sData) { return "<Message Type='" + sType + "'><Text>" + sMsg + "</Text><Data>" + sData + "</Data></Message>"; }
	}

}
