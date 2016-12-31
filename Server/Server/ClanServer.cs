//*************************************************************
//  File: ClanServer.cs
//  Date created: 11/28/2016
//  Date edited: 12/30/2016
//  Author: Nathan Martindale
//  Copyright © 2016 Digital Warrior Labs
//  Description: Main server that has all the outward facing REST API functions
//*************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Xml.Linq;

using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;

using GameClansServer.TableEntities;

using DWL.Utility;

namespace GameClansServer
{
	public class ClanServer
	{
		// member variables
		private CloudStorageAccount m_pStorageAccount;
		private CloudTableClient m_pTableClient;
		private CloudBlobClient m_pBlobClient;

		private CloudTable m_pTable = null;
		private CloudBlobContainer m_pContainer = null;

		// construction
		public ClanServer()
		{
			this.Initialize();
		}
		
		// properties
		public CloudTable Table
		{
			get
			{
				if (m_pTable == null) { m_pTable = this.FetchTable(); }
				return m_pTable;
			}
		}
		public CloudBlobContainer Container
		{
			get
			{
				if (m_pContainer == null) { m_pContainer = this.FetchContainer(); }
				return m_pContainer;
			}
		}

		// outward facing methods
		public string ServerVersion() { return Master.MessagifySimple(Master.SERVER_VERSION); }
		public string RequiredAppVersion() { return Master.MessagifySimple(Master.APP_VERSION); }
		public string RequiredClientVersion() { return Master.MessagifySimple(Master.CLIENT_VERSION); }


		public string RegisterUser(string sEmail, string sPassword)
		{
			// make sure a user with this email doesn't already exist
			TableOperation pUserRetrieveOp = TableOperation.Retrieve<UserRegistrationTableEntity>("USER", sEmail);
			if (this.Table.Execute(pUserRetrieveOp).Result != null) { return Master.MessagifyError("A user with this email already exists."); }

			// made it to this point, it's okay to make a new user
			UserRegistrationTableEntity pUser = new UserRegistrationTableEntity(sEmail);
			pUser.Password = Security.Sha256Hash(sPassword);
			pUser.Key = Master.GenerateKey();
			this.Table.Execute(TableOperation.Insert(pUser));

			XElement pKey = new XElement("Key", pUser.Key);
			XElement pEmail = new XElement("Email", sEmail);

			return Master.Messagify("You have successfully registered!", Master.MSGTYPE_BOTH, pKey.ToString() + pEmail.ToString());
		}

		public string ReturningUser(string sEmail, string sPassword)
		{
			UserRegistrationTableEntity pUser = VerifyUserRegistration(sEmail, sPassword);
			if (pUser == null) { return Master.MessagifyError("Incorrect login information"); }

			// otherwise return them their key and all their clan stubs
			string sKey = pUser.Key;

			XElement pKey = new XElement("Key", sKey);
			XElement pEmail = new XElement("Email", sEmail);

			// query their email partition key
			TableQuery<RegistrationClanUserTableEntity> pQuery = new TableQuery<RegistrationClanUserTableEntity>().Where("PartitionKey eq '" + pUser.RowKey + "'");
			List<RegistrationClanUserTableEntity> lClanInfos = this.Table.ExecuteQuery(pQuery).ToList();

			XElement pClanStubs = new XElement("ClanStubs");
			foreach (RegistrationClanUserTableEntity pClanInfo in lClanInfos)
			{
				string sCombined = pClanInfo.RowKey;

				int iIndex = sCombined.IndexOf("|");
				string sClanName = sCombined.Substring(0, iIndex);
				string sUserName = sCombined.Substring(iIndex + 1);
				
				XElement pClanStub = new XElement("ClanStub");
				pClanStub.SetAttributeValue("ClanName", sClanName);
				pClanStub.SetAttributeValue("UserName", sUserName);
				pClanStubs.Add(pClanStub);
			}

			return Master.Messagify("Logged in successfully!", Master.MSGTYPE_BOTH, pClanStubs.ToString() + pKey.ToString() + pEmail.ToString());
		}

		public string ChangeUserPassword(string sEmail, string sOldPassword, string sNewPassword)
		{
			UserRegistrationTableEntity pUser = VerifyUserRegistration(sEmail, sOldPassword);
			if (pUser == null) { return Master.MessagifyError("Incorrect login information"); }

			// update the password
			pUser.Password = Security.Sha256Hash(sNewPassword);
			this.Table.Execute(TableOperation.Replace(pUser));
			
			return Master.MessagifySimple("Password changed successfully!");
		}

		public string CreateClan(string sClanName, string sClanPassPhrase)
		{
			// make sure a clan with this name doesn't already exist	
			TableOperation pClanRetrieveOp = TableOperation.Retrieve<ClanTableEntity>("CLAN", sClanName);
			if (this.Table.Execute(pClanRetrieveOp).Result != null) { return Master.MessagifyError("A clan with this name already exists."); }

			// if we make it to this point we're good, add the new clan!
			ClanTableEntity pClan = new ClanTableEntity(sClanName);
			pClan.PassPhrase = this.Sha256Hash(sClanPassPhrase);
			this.Table.Execute(TableOperation.Insert(pClan));

			return Master.MessagifySimple("You have successfully created the clan " + sClanName + "!");
		}

		public string JoinClan(string sEmail, string sClanName, string sClanPassPhrase, string sUserName, string sUserPassPhrase)
		{
			// make sure the clan password is correct
			if (!VerifyClanPassPhrase(sClanName, sClanPassPhrase)) { return Master.MessagifyError("Clan password incorrect."); }

			// make sure the username doesn't already exist
			TableOperation pUserRetrieveOp = TableOperation.Retrieve<UserTableEntity>(Master.BuildUserPartitionKey(sClanName), sUserName);
			if (this.Table.Execute(pUserRetrieveOp).Result != null) 
			{
				// if just user logging in on a different device
				if (VerifyUserPassPhrase(sClanName, sUserName, sUserPassPhrase)) { return Master.Messagify("You have successfully logged into " + sClanName + " as " + sUserName, Master.MSGTYPE_BOTH, "<ClanStub ClanName='" + sClanName + "' UserName='" + sUserName + "' />"); }
			
				// (password incorrect)
				return Master.MessagifyError("User with this name already exists in this clan."); 
			}

			// if we make it to this point, we're good, add a new user!
			UserTableEntity pUser = new UserTableEntity(sClanName, sUserName);
			pUser.PassPhrase = this.Sha256Hash(sUserPassPhrase);
			pUser.Email = sEmail;
			this.Table.Execute(TableOperation.Insert(pUser));

			// add the email/clan/username association
			RegistrationClanUserTableEntity pUserClan = new RegistrationClanUserTableEntity(sEmail, sClanName, sUserName);
			this.Table.Execute(TableOperation.Insert(pUserClan));

			//return "You have successfully joined the clan " + sClanName + " as " + sUserName;
			return Master.Messagify("You have successfully joined the clan " + sClanName + " as " + sUserName, Master.MSGTYPE_BOTH, "<ClanStub ClanName='" + sClanName + "' UserName='" + sUserName + "' />");
		}

		/*public string ChangePassword(string sClanNameList, string sUserNameList, string sUserOldPassPhrase, string sUserNewPassPhrase)
		{
		
		}*/

		public string ListActiveGames(string sClanName, string sUserName, string sUserPassPhrase)
		{
			// make sure the user has permission
			if (!VerifyUserPassPhrase(sClanName, sUserName, sUserPassPhrase)) { return Master.MessagifyError("Invalid login."); }

			TableQuery<GameTableEntity> pQuery = new TableQuery<GameTableEntity>().Where("PartitionKey eq'" + Master.BuildGamePartitionKey(sClanName) + "' and Active eq true");
			List<GameTableEntity> lGames = this.Table.ExecuteQuery(pQuery).ToList();
			lGames.Reverse();

			string sResponse = "<Games>";
			foreach (GameTableEntity pGame in lGames) { sResponse += "<Game GameType='" + pGame.GameType + "' GameName='" + pGame.GameName + "'>" + pGame.RowKey + "</Game>"; }
			sResponse += "</Games>";

			return Master.MessagifyData(sResponse);
		}

		// only retains the last 20 notifications
		public string GetLastNNotifications(string sClanName, string sUserName, string sUserPassPhrase)
		{
			// make sure the user has permission
			if (!VerifyUserPassPhrase(sClanName, sUserName, sUserPassPhrase)) { return Master.MessagifyError("Invalid login."); }

			TableQuery<UserNotifTableEntity> pQuery = new TableQuery<UserNotifTableEntity>().Where("PartitionKey eq '" + Master.BuildUserNotifPartitionKey(sClanName, sUserName) + "'");
			List<UserNotifTableEntity> lUserNotifications = this.Table.ExecuteQuery(pQuery).ToList();
			lUserNotifications.Reverse();

			// make the xml list of unread notifications (and delete any past 20)
			TableBatchOperation pBatch = new TableBatchOperation();
			int iCount = 0;
			XElement pList = new XElement("Notifications");
			foreach (UserNotifTableEntity pNotif in lUserNotifications)
			{
				iCount++;
				if (iCount >= 20) { pBatch.Add(TableOperation.Delete(pNotif)); }
				XElement pNotifXML = new XElement("Notification");
				pNotifXML.Value = pNotif.Content;
				pNotifXML.SetAttributeValue("GameID", pNotif.GameID);
				pNotifXML.SetAttributeValue("GameName", pNotif.GameName);
				pNotifXML.SetAttributeValue("DateTime", pNotif.Time);
				pList.Add(pNotifXML);
			}

			if (pBatch.Count > 0) { this.Table.ExecuteBatch(pBatch); }
			
			return Master.MessagifyData(pList.ToString()); ;
		}

		public string MarkUnreadNotificationsRead(string sClanName, string sUserName, string sUserPassPhrase)
		{
			// make sure the user has permission
			if (!VerifyUserPassPhrase(sClanName, sUserName, sUserPassPhrase)) { return Master.MessagifyError("Invalid login."); }

			// get the list of notification entries for this user
			TableQuery<UserNotifTableEntity> pQuery = new TableQuery<UserNotifTableEntity>().Where("PartitionKey eq '" + Master.BuildUserNotifPartitionKey(sClanName, sUserName) + "' and Read eq false");
			List<UserNotifTableEntity> lUserNotifications = this.Table.ExecuteQuery(pQuery).ToList();
			lUserNotifications.Reverse();

			// set all notifications to seen
			TableBatchOperation pBatch = new TableBatchOperation();
			foreach (UserNotifTableEntity pNotif in lUserNotifications) 
			{ 
				pNotif.Read = true;
				pNotif.Seen = true;
				pBatch.Add(TableOperation.Replace(pNotif));
			}
			if (pBatch.Count > 0) { this.Table.ExecuteBatch(pBatch); }

			return "";
		}

		public string GetUnreadNotifications(string sClanName, string sUserName, string sUserPassPhrase)
		{
			// make sure the user has permission
			if (!VerifyUserPassPhrase(sClanName, sUserName, sUserPassPhrase)) { return Master.MessagifyError("Invalid login."); }

			// get the list of notification entries for this user
			TableQuery<UserNotifTableEntity> pQuery = new TableQuery<UserNotifTableEntity>().Where("PartitionKey eq '" + Master.BuildUserNotifPartitionKey(sClanName, sUserName) + "' and Read eq false");
			List<UserNotifTableEntity> lUserNotifications = this.Table.ExecuteQuery(pQuery).ToList();
			lUserNotifications.Reverse();

			// set all notifications to seen
			TableBatchOperation pBatch = new TableBatchOperation();
			foreach (UserNotifTableEntity pNotif in lUserNotifications) 
			{ 
				pNotif.Seen = true;
				pBatch.Add(TableOperation.Replace(pNotif));
			}
			if (pBatch.Count > 0) { this.Table.ExecuteBatch(pBatch); }

			// make the xml list of unread notifications
			XElement pList = new XElement("Notifications");
			foreach (UserNotifTableEntity pNotif in lUserNotifications)
			{
				XElement pNotifXML = new XElement("Notification");
				pNotifXML.Value = pNotif.Content;
				pNotifXML.SetAttributeValue("GameID", pNotif.GameID);
				pNotifXML.SetAttributeValue("GameName", pNotif.GameName);
				pNotifXML.SetAttributeValue("DateTime", pNotif.Time);
				pList.Add(pNotifXML);
			}
			return Master.MessagifyData(pList.ToString());
		}

		public string GetUnseenNotifications(string sClanName, string sUserName, string sUserPassPhrase)
		{
			// make sure the user has permission
			if (!VerifyUserPassPhrase(sClanName, sUserName, sUserPassPhrase)) { return Master.MessagifyError("Invalid login."); }

			// get the list of notification entries for this user
			TableQuery<UserNotifTableEntity> pQuery = new TableQuery<UserNotifTableEntity>().Where("PartitionKey eq '" + Master.BuildUserNotifPartitionKey(sClanName, sUserName) + "' and Seen eq false");
			List<UserNotifTableEntity> lUserNotifications = this.Table.ExecuteQuery(pQuery).ToList();
			lUserNotifications.Reverse();

			// set all notifications to seen
			TableBatchOperation pBatch = new TableBatchOperation();
			foreach (UserNotifTableEntity pNotif in lUserNotifications) 
			{ 
				pNotif.Seen = true;
				pBatch.Add(TableOperation.Replace(pNotif));
			}
			if (pBatch.Count > 0) { this.Table.ExecuteBatch(pBatch); }

			// make the xml list of unread notifications
			XElement pList = new XElement("Notifications");
			foreach (UserNotifTableEntity pNotif in lUserNotifications)
			{
				XElement pNotifXML = new XElement("Notification");
				pNotifXML.Value = pNotif.Content;
				pNotifXML.SetAttributeValue("GameID", pNotif.GameID);
				pNotifXML.SetAttributeValue("GameName", pNotif.GameName);
				pNotifXML.SetAttributeValue("DateTime", pNotif.Time);
				pList.Add(pNotifXML);
			}
			return Master.MessagifyData(pList.ToString());
		}

		// also returns what place passed user is
		public string GetClanLeaderboard(string sClanName, string sUserName, string sUserPassPhrase)
		{
			// make sure user has permission
			if (!VerifyUserPassPhrase(sClanName, sUserName, sUserPassPhrase)) { return Master.MessagifyError("Invalid login."); }

			// get all rows of users from that clan
			TableQuery<UserTableEntity> pQuery = new TableQuery<UserTableEntity>().Where("PartitionKey eq '" + Master.BuildUserPartitionKey(sClanName) + "'");
			List<UserTableEntity> lUsers = this.Table.ExecuteQuery(pQuery).ToList();

			// sort user list by zendo score (TODO: eventually make this a composite from the different games)
			// thanks to http://stackoverflow.com/questions/20902248/sorting-a-list-in-c-sharp-using-list-sortcomparisont-comparison
			lUsers.Sort((x, y) => -x.ZendoScore.CompareTo(y.ZendoScore));

			// return an xml list
			XElement pLeaderboard = new XElement("Leaderboard");
			int iPlace = 0;
			foreach (UserTableEntity pUser in lUsers)
			{
				iPlace++;
				XElement pScore = new XElement("Score");
				pScore.SetAttributeValue("User", pUser.RowKey);
				pScore.SetAttributeValue("Score", pUser.ZendoScore);
				pScore.SetAttributeValue("Place", this.Numberify(iPlace));
				pLeaderboard.Add(pScore);

				// we found the desired user!
				if (pUser.RowKey == sUserName)
				{
					pLeaderboard.SetAttributeValue("Place", this.Numberify(iPlace));
					pLeaderboard.SetAttributeValue("Score", pUser.ZendoScore);
				}
			}
		
			return Master.MessagifyData(pLeaderboard.ToString());
		}


		// inner methods

		/*public List<string> GetUsersInClan(string sClanName)
		{
			
		}*/

		public void AddNotification(string sClanName, string sUserName, string sContent, string sGameID, string sGameName)
		{
			UserNotifTableEntity pNotif = new UserNotifTableEntity(sClanName, sUserName);
			pNotif.Time = DateTime.Now;
			pNotif.Content = sContent;
			pNotif.GameID = sGameID;
			pNotif.GameName = sGameName;
			pNotif.Seen = false;
			pNotif.Read = false;
			this.Table.Execute(TableOperation.Insert(pNotif));
		}

		public void UpdateUserScore(string sClanName, string sUserName, string sGameType, int iScore)
		{
			// get the requested user row from the table
			TableOperation pUserRetrieveOp = TableOperation.Retrieve<UserTableEntity>(Master.BuildUserPartitionKey(sClanName), sUserName);
			TableResult pUserRetrieveResult = this.Table.Execute(pUserRetrieveOp);

			if (pUserRetrieveResult.Result == null) { return; }
			UserTableEntity pUser = (UserTableEntity)pUserRetrieveResult.Result;

			if (sGameType == "Zendo") { pUser.ZendoScore += iScore; }
			this.Table.Execute(TableOperation.Replace(pUser));
		}

		public void SaveGame(XElement pStateXml, string sGameID)
		{
			CloudBlockBlob pBlob = this.Container.GetBlockBlobReference(sGameID);
			pBlob.UploadText(pStateXml.ToString());
		}

		public XElement LoadGame(string sGameID)
		{
			CloudBlockBlob pBlob = this.Container.GetBlockBlobReference(sGameID);
			string sContents = pBlob.DownloadText();
			XElement pStateXml = XElement.Parse(sContents);
			return pStateXml;
		}

		public void AddActiveGame(string sClanName, string sGameID, string sGameType, string sGameName)
		{
			GameTableEntity pGame = new GameTableEntity(sClanName, sGameID);
			pGame.Active = true;
			pGame.GameName = sGameName;
			pGame.GameType = sGameType;
			this.Table.Execute(TableOperation.Insert(pGame));
		}

		public UserRegistrationTableEntity VerifyUserRegistration(string sEmail, string sPassword)
		{
			if (sEmail == null || sPassword == null) { return null; }

			// get registration row from table
			TableOperation pRegistrationOp = TableOperation.Retrieve<UserRegistrationTableEntity>("USER", sEmail);
			TableResult pRegistrationResult = this.Table.Execute(pRegistrationOp);

			if (pRegistrationResult.Result == null) { return null; }
			UserRegistrationTableEntity pRegistration = (UserRegistrationTableEntity)pRegistrationResult.Result;

			// hash clan passphrase
			string sPassHash = Security.Sha256Hash(sPassword);

			// return if hashes match

			if (sPassHash == pRegistration.Password) { return pRegistration; }
			return null;
		}
		
		public bool VerifyClanPassPhrase(string sClanName, string sClanPassPhrase)
		{
			if (sClanName == null || sClanPassPhrase == null) { return false; }
			
			// get the requested clan row from the table 
			TableOperation pClanRetrieveOp = TableOperation.Retrieve<ClanTableEntity>("CLAN", sClanName);
			TableResult pClanRetrieveResult = this.Table.Execute(pClanRetrieveOp);

			if (pClanRetrieveResult.Result == null) { return false; }
			ClanTableEntity pClan = (ClanTableEntity)pClanRetrieveResult.Result;

			// hash clan passphrase
			string sClanHash = Security.Sha256Hash(sClanPassPhrase);

			// check if the passphrase matches 
			return sClanHash == pClan.PassPhrase;
		}

		public bool VerifyUserPassPhrase(string sClanName, string sUserName, string sUserPassPhrase)
		{
			if (sClanName == null || sUserName == null || sUserPassPhrase == null || sUserPassPhrase == "") { return false; }
		
			// get the requested user row from the table
			TableOperation pUserRetrieveOp = TableOperation.Retrieve<UserTableEntity>(Master.BuildUserPartitionKey(sClanName), sUserName);
			TableResult pUserRetrieveResult = this.Table.Execute(pUserRetrieveOp);

			if (pUserRetrieveResult.Result == null) { return false; }
			UserTableEntity pUser = (UserTableEntity)pUserRetrieveResult.Result;

			// hash user passphrase
			string sUserHash = Security.Sha256Hash(sUserPassPhrase);

			// check if the passphrase matches
			return sUserHash == pUser.PassPhrase;
		}

		// make text number (cardinal?)
		private string Numberify(int iNumber)
		{
			string sSuffix = "";

			// determine the suffix by the last digit
			int iLastDigit = iNumber % 10;
			if (iLastDigit == 1) { sSuffix = "st"; }
			else if (iLastDigit == 2) { sSuffix = "nd"; }
			else if (iLastDigit == 3) { sSuffix = "rd"; }
			else { sSuffix = "th"; }

			// then, double check exceptions, because english sucks...
			if (iNumber == 11 || iNumber == 12 || iNumber == 13) { sSuffix = "th"; }

			return iNumber + sSuffix;
		}

		// thanks to http://www.codeshare.co.uk/blog/sha-256-and-sha-512-hash-examples/
		// TODO: make private
		public string Sha256Hash(string sString)
		{
			SHA256 p256 = SHA256Managed.Create();
			byte[] aBytes = Encoding.UTF8.GetBytes(sString);
			byte[] aHashBytes = p256.ComputeHash(aBytes);
			string sHash = this.GetStringFromHash(aHashBytes);
			return sHash;
		}

		private string GetStringFromHash(byte[] aHashBytes)
		{
			StringBuilder pResult = new StringBuilder();
			for (int i = 0; i < aHashBytes.Length; i++)
			{
				pResult.Append(aHashBytes[i].ToString("X2"));
			}
			return pResult.ToString();
		}

		// set up cloud storage account stuff
		private void Initialize()
		{
			m_pStorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
			m_pTableClient = m_pStorageAccount.CreateCloudTableClient();
			m_pBlobClient = m_pStorageAccount.CreateCloudBlobClient();
		}

		private CloudTable FetchTable()
		{
			CloudTable pTable = m_pTableClient.GetTableReference("gameclansdb");

			// make sure it exists
			pTable.CreateIfNotExists();
			return pTable;
		}

		private CloudBlobContainer FetchContainer()
		{
			CloudBlobContainer pContainer = m_pBlobClient.GetContainerReference("gameclansgames");

			// make sure it exists
			pContainer.CreateIfNotExists();
			return pContainer;
		}
	}
}
